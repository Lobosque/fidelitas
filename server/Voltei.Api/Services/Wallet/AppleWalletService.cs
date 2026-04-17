using System.IO.Compression;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Voltei.Api.Configuration;
using Voltei.Api.Models;

namespace Voltei.Api.Services.Wallet;

public class AppleWalletService
{
    private readonly AppleWalletOptions _options;
    private readonly ILogger<AppleWalletService> _logger;
    private readonly X509Certificate2? _certificate;

    public bool IsConfigured => _certificate != null
                                && !string.IsNullOrEmpty(_options.PassTypeIdentifier)
                                && !string.IsNullOrEmpty(_options.TeamIdentifier);

    public AppleWalletService(
        IOptions<AppleWalletOptions> options,
        ILogger<AppleWalletService> logger,
        IWebHostEnvironment env)
    {
        _options = options.Value;
        _logger = logger;

        // Resolver caminho relativo ao content root
        var certPath = _options.CertificatePath;
        if (!string.IsNullOrEmpty(certPath) && !Path.IsPathRooted(certPath))
        {
            certPath = Path.Combine(env.ContentRootPath, certPath);
        }

        if (string.IsNullOrEmpty(certPath) || !File.Exists(certPath))
        {
            _logger.LogWarning(
                "Apple Wallet não configurado — certificado não encontrado em '{Path}'.",
                certPath);
            return;
        }

        try
        {
            _certificate = X509CertificateLoader.LoadPkcs12FromFile(
                certPath,
                _options.CertificatePassword,
                X509KeyStorageFlags.Exportable);
            _logger.LogInformation("Apple Wallet certificado carregado: {Subject}", _certificate.Subject);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao carregar certificado Apple Wallet.");
        }
    }

    /// <summary>
    /// Gera um .pkpass (ZIP assinado) para o enrollment.
    /// Retorna os bytes do arquivo ou null se não configurado.
    /// </summary>
    public byte[]? GeneratePass(
        Enrollment enrollment, Campaign campaign, Business business, Customer customer)
    {
        if (!IsConfigured || _certificate == null) return null;

        try
        {
            var serialNumber = enrollment.ApplePassSerial ?? Guid.NewGuid().ToString("N");
            var authToken = enrollment.ApplePassAuthToken ?? GenerateAuthToken();

            var passJson = BuildPassJson(enrollment, campaign, business, customer, serialNumber, authToken);

            var files = new Dictionary<string, byte[]>
            {
                ["pass.json"] = Encoding.UTF8.GetBytes(passJson),
                ["icon.png"] = GeneratePlaceholderIcon(1),
                ["icon@2x.png"] = GeneratePlaceholderIcon(2),
                ["logo.png"] = GeneratePlaceholderIcon(1),
                ["logo@2x.png"] = GeneratePlaceholderIcon(2),
            };

            // Gerar manifest.json (SHA256 de cada arquivo)
            var manifest = new Dictionary<string, string>();
            foreach (var (name, data) in files)
            {
                var hash = SHA256.HashData(data);
                manifest[name] = Convert.ToHexStringLower(hash);
            }
            var manifestJson = JsonSerializer.Serialize(manifest);
            var manifestBytes = Encoding.UTF8.GetBytes(manifestJson);
            files["manifest.json"] = manifestBytes;

            // Assinar manifest com CMS/PKCS#7 (detached signature)
            var signature = SignManifest(manifestBytes);
            files["signature"] = signature;

            // Empacotar em ZIP
            return CreateZip(files);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gerar .pkpass para enrollment {Id}.", enrollment.Id);
            return null;
        }
    }

    private string BuildPassJson(
        Enrollment enrollment, Campaign campaign, Business business,
        Customer customer, string serialNumber, string authToken)
    {
        var fgColor = ContrastColor(business.CoresPrimaria);

        var pass = new
        {
            formatVersion = 1,
            passTypeIdentifier = _options.PassTypeIdentifier,
            teamIdentifier = _options.TeamIdentifier,
            serialNumber,
            authenticationToken = authToken,
            webServiceURL = string.IsNullOrEmpty(_options.WebServiceUrl) ? (string?)null : _options.WebServiceUrl,
            organizationName = business.Nome,
            description = $"Cartão fidelidade — {business.Nome}",
            foregroundColor = fgColor,
            backgroundColor = business.CoresPrimaria,
            labelColor = fgColor,
            storeCard = new
            {
                headerFields = new[]
                {
                    new
                    {
                        key = "balance",
                        label = "Check-ins",
                        value = $"{enrollment.CheckinsAtuais}/{campaign.CheckinsNecessarios}",
                    },
                },
                primaryFields = new[]
                {
                    new
                    {
                        key = "program",
                        label = business.Nome,
                        value = campaign.Nome,
                    },
                },
                secondaryFields = new[]
                {
                    new
                    {
                        key = "customer",
                        label = "Cliente",
                        value = customer.Nome,
                    },
                    new
                    {
                        key = "reward",
                        label = "Prêmio",
                        value = campaign.Descricao,
                    },
                },
                backFields = new[]
                {
                    new
                    {
                        key = "info",
                        label = "Como funciona",
                        value = $"Acumule {campaign.CheckinsNecessarios} check-ins e ganhe: {campaign.Descricao}. " +
                                $"Apresente este cartão a cada visita para registrar seu check-in.",
                    },
                },
            },
            barcode = new
            {
                format = "PKBarcodeFormatQR",
                message = enrollment.Token,
                messageEncoding = "iso-8859-1",
                altText = enrollment.Token,
            },
            barcodes = new[]
            {
                new
                {
                    format = "PKBarcodeFormatQR",
                    message = enrollment.Token,
                    messageEncoding = "iso-8859-1",
                    altText = enrollment.Token,
                },
            },
        };

        return JsonSerializer.Serialize(pass, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = false,
        });
    }

    private byte[] SignManifest(byte[] manifestBytes)
    {
        var content = new ContentInfo(manifestBytes);
        var signedCms = new SignedCms(content, detached: true);
        var signer = new CmsSigner(SubjectIdentifierType.SubjectKeyIdentifier, _certificate!)
        {
            DigestAlgorithm = new Oid("2.16.840.1.101.3.4.2.1"), // SHA-256
            IncludeOption = X509IncludeOption.WholeChain,
        };
        signedCms.ComputeSignature(signer);
        return signedCms.Encode();
    }

    private static byte[] CreateZip(Dictionary<string, byte[]> files)
    {
        using var ms = new MemoryStream();
        using (var archive = new ZipArchive(ms, ZipArchiveMode.Create, leaveOpen: true))
        {
            foreach (var (name, data) in files)
            {
                var entry = archive.CreateEntry(name, CompressionLevel.NoCompression);
                using var entryStream = entry.Open();
                entryStream.Write(data);
            }
        }
        return ms.ToArray();
    }

    /// <summary>
    /// Gera um PNG mínimo 1x1 como placeholder.
    /// Em produção, usar o logo real do negócio.
    /// </summary>
    private static byte[] GeneratePlaceholderIcon(int scale)
    {
        // PNG mínimo 1x1 pixel transparente
        // Header (8) + IHDR chunk (25) + IDAT chunk (min) + IEND chunk (12)
        var size = scale; // 1x1 or 2x2
        return
        [
            // PNG signature
            0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A,
            // IHDR chunk
            0x00, 0x00, 0x00, 0x0D, // length = 13
            0x49, 0x48, 0x44, 0x52, // "IHDR"
            0x00, 0x00, 0x00, (byte)size, // width
            0x00, 0x00, 0x00, (byte)size, // height
            0x08, 0x02,             // bit depth=8, color type=RGB
            0x00, 0x00, 0x00,       // compression, filter, interlace
            0x00, 0x00, 0x00, 0x00, // CRC placeholder (will be invalid but functional for dev)
            // IDAT chunk (minimal valid deflate stream for 1x1 RGB)
            0x00, 0x00, 0x00, 0x0C, // length = 12
            0x49, 0x44, 0x41, 0x54, // "IDAT"
            0x08, 0xD7, 0x63, 0x60, 0x60, 0x60, 0x00, 0x00,
            0x00, 0x04, 0x00, 0x01, // deflate data
            0x00, 0x00, 0x00, 0x00, // CRC placeholder
            // IEND chunk
            0x00, 0x00, 0x00, 0x00, // length = 0
            0x49, 0x45, 0x4E, 0x44, // "IEND"
            0xAE, 0x42, 0x60, 0x82, // CRC for IEND
        ];
    }

    private static string GenerateAuthToken()
    {
        return Convert.ToHexStringLower(RandomNumberGenerator.GetBytes(16));
    }

    /// <summary>
    /// Determina se a cor de fundo é escura ou clara e retorna
    /// branco ou preto para contraste legível.
    /// </summary>
    private static string ContrastColor(string hexColor)
    {
        if (string.IsNullOrEmpty(hexColor) || hexColor.Length < 7) return "rgb(255,255,255)";

        var hex = hexColor.TrimStart('#');
        if (hex.Length < 6) return "rgb(255,255,255)";

        var r = Convert.ToInt32(hex[..2], 16);
        var g = Convert.ToInt32(hex[2..4], 16);
        var b = Convert.ToInt32(hex[4..6], 16);

        // Luminância relativa
        var luminance = (0.299 * r + 0.587 * g + 0.114 * b) / 255;
        return luminance > 0.5 ? "rgb(0,0,0)" : "rgb(255,255,255)";
    }
}
