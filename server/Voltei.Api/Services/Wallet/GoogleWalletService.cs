using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Walletobjects.v1;
using Google.Apis.Walletobjects.v1.Data;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Voltei.Api.Configuration;
using Voltei.Api.Models;

namespace Voltei.Api.Services.Wallet;

public class GoogleWalletService
{
    private readonly GoogleWalletOptions _options;
    private readonly ILogger<GoogleWalletService> _logger;
    private readonly WalletobjectsService? _walletService;
    private readonly ServiceAccountCredential? _credential;

    public bool IsConfigured => !string.IsNullOrEmpty(_options.IssuerId)
                                && !string.IsNullOrEmpty(_options.ServiceAccountEmail)
                                && !string.IsNullOrEmpty(_options.ServiceAccountPrivateKey);

    public GoogleWalletService(IOptions<GoogleWalletOptions> options, ILogger<GoogleWalletService> logger)
    {
        _options = options.Value;
        _logger = logger;

        if (!IsConfigured)
        {
            _logger.LogWarning("Google Wallet não configurado — funcionalidade desabilitada.");
            return;
        }

        _credential = new ServiceAccountCredential(
            new ServiceAccountCredential.Initializer(_options.ServiceAccountEmail)
            {
                Scopes = ["https://www.googleapis.com/auth/wallet_object.issuer"],
            }.FromPrivateKey(_options.ServiceAccountPrivateKey));

        _walletService = new WalletobjectsService(new BaseClientService.Initializer
        {
            HttpClientInitializer = _credential,
            ApplicationName = _options.ApplicationName,
        });
    }

    /// <summary>
    /// Cria uma Loyalty Class no Google Wallet para a campanha.
    /// Retorna o classId criado, ou null se não configurado.
    /// </summary>
    public async Task<string?> CreateLoyaltyClassAsync(Campaign campaign, Business business)
    {
        if (!IsConfigured || _walletService == null) return null;

        var classId = $"{_options.IssuerId}.voltei-{campaign.Id}";

        // Checar se já existe
        try
        {
            await _walletService.Loyaltyclass.Get(classId).ExecuteAsync();
            _logger.LogInformation("Loyalty Class {ClassId} já existe.", classId);
            return classId;
        }
        catch (Google.GoogleApiException ex) when (ex.HttpStatusCode == System.Net.HttpStatusCode.NotFound)
        {
            // Não existe, vamos criar
        }

        var loyaltyClass = new LoyaltyClass
        {
            Id = classId,
            IssuerName = business.Nome,
            ProgramName = campaign.Nome,
            ReviewStatus = "UNDER_REVIEW",
            HexBackgroundColor = business.CoresPrimaria,
            AccountIdLabel = "Check-ins",
            AccountNameLabel = "Cliente",
        };

        if (!string.IsNullOrEmpty(business.LogoUrl))
        {
            loyaltyClass.ProgramLogo = new Image
            {
                SourceUri = new ImageUri { Uri = business.LogoUrl },
            };
        }

        try
        {
            await _walletService.Loyaltyclass.Insert(loyaltyClass).ExecuteAsync();
            _logger.LogInformation("Loyalty Class {ClassId} criada.", classId);
            return classId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar Loyalty Class {ClassId}.", classId);
            return null;
        }
    }

    /// <summary>
    /// Cria um Loyalty Object e retorna a save URL para o Google Wallet.
    /// </summary>
    public async Task<string?> CreateLoyaltyObjectAsync(
        Enrollment enrollment, Campaign campaign, Business business, Customer customer)
    {
        if (!IsConfigured || _credential == null) return null;

        var classId = campaign.WalletClassId;
        if (string.IsNullOrEmpty(classId)) return null;

        var objectId = $"{_options.IssuerId}.voltei-{enrollment.Id}";

        var loyaltyObject = new LoyaltyObject
        {
            Id = objectId,
            ClassId = classId,
            State = "ACTIVE",
            AccountId = $"{enrollment.CheckinsAtuais}/{campaign.CheckinsNecessarios}",
            AccountName = customer.Nome,
            LoyaltyPoints = new LoyaltyPoints
            {
                Label = "Check-ins",
                Balance = new LoyaltyPointsBalance
                {
                    Int__ = enrollment.CheckinsAtuais,
                },
            },
            Barcode = new Barcode
            {
                Type = "QR_CODE",
                Value = enrollment.Token,
                AlternateText = enrollment.Token,
            },
        };

        // Gerar JWT assinado com as credenciais do service account
        var claims = new Dictionary<string, object>
        {
            ["iss"] = _options.ServiceAccountEmail,
            ["aud"] = "google",
            ["origins"] = new[] { "https://voltei.app" },
            ["typ"] = "savetowallet",
            ["payload"] = new
            {
                loyaltyObjects = new[] { loyaltyObject },
            },
        };

        try
        {
            var rsaKey = _credential.Key;
            var signingCredentials = new SigningCredentials(
                new RsaSecurityKey(rsaKey), SecurityAlgorithms.RsaSha256);

            var token = new JwtSecurityToken(
                issuer: _options.ServiceAccountEmail,
                audience: "google",
                claims: [
                    new Claim("iss", _options.ServiceAccountEmail),
                    new Claim("aud", "google"),
                    new Claim("typ", "savetowallet"),
                    new Claim("origins", "[\"https://voltei.app\"]", JsonClaimValueTypes.JsonArray),
                    new Claim("payload", System.Text.Json.JsonSerializer.Serialize(new
                    {
                        loyaltyObjects = new object[]
                        {
                            new
                            {
                                id = objectId,
                                classId,
                                state = "ACTIVE",
                                accountId = $"{enrollment.CheckinsAtuais}/{campaign.CheckinsNecessarios}",
                                accountName = customer.Nome,
                                loyaltyPoints = new
                                {
                                    label = "Check-ins",
                                    balance = new { @int = enrollment.CheckinsAtuais },
                                },
                                barcode = new
                                {
                                    type = "QR_CODE",
                                    value = enrollment.Token,
                                    alternateText = enrollment.Token,
                                },
                            },
                        },
                    }), JsonClaimValueTypes.Json),
                ],
                signingCredentials: signingCredentials);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            var saveUrl = $"https://pay.google.com/gp/v/save/{jwt}";

            _logger.LogInformation("Save URL gerada para enrollment {EnrollmentId}.", enrollment.Id);
            return saveUrl;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gerar save URL para enrollment {EnrollmentId}.", enrollment.Id);
            return null;
        }
    }

    /// <summary>
    /// Atualiza o Loyalty Object no Google Wallet após um check-in.
    /// </summary>
    public async Task UpdateLoyaltyObjectAsync(Enrollment enrollment, Campaign campaign)
    {
        if (!IsConfigured || _walletService == null) return;

        if (string.IsNullOrEmpty(enrollment.WalletObjectId)) return;

        var patch = new LoyaltyObject
        {
            LoyaltyPoints = new LoyaltyPoints
            {
                Label = "Check-ins",
                Balance = new LoyaltyPointsBalance
                {
                    Int__ = enrollment.CheckinsAtuais,
                },
            },
            AccountId = $"{enrollment.CheckinsAtuais}/{campaign.CheckinsNecessarios}",
        };

        try
        {
            await _walletService.Loyaltyobject
                .Patch(patch, enrollment.WalletObjectId)
                .ExecuteAsync();
            _logger.LogInformation("Loyalty Object {ObjectId} atualizado.", enrollment.WalletObjectId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar Loyalty Object {ObjectId}.", enrollment.WalletObjectId);
        }
    }
}
