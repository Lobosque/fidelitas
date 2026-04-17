using System.IO.Compression;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Voltei.Api.Dto;
using Voltei.Api.IntegrationTests.Fixtures;
using Voltei.Api.IntegrationTests.Helpers;

namespace Voltei.Api.IntegrationTests.Wallet;

public class AppleWalletIntegrationTests : IClassFixture<VolteiApiFactory>, IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly VolteiApiFactory _factory;
    private string _authToken = string.Empty;
    private string _campaignId = string.Empty;

    public AppleWalletIntegrationTests(VolteiApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        var (_, auth) = await AuthHelper.SignupAsync(
            _client, "Apple Wallet Biz", AuthHelper.UniqueEmail(), "senha123");
        _authToken = auth!.Token;
        AuthHelper.SetBearerToken(_client, _authToken);

        var response = await _client.PostAsJsonAsync("/api/campaigns", new
        {
            nome = "Campanha Apple Test",
            descricao = "1 prêmio grátis",
            checkinsNecessarios = 3,
        });
        var campaign = await response.Content.ReadFromJsonAsync<CampaignResponse>();
        _campaignId = campaign!.Id;
    }

    public Task DisposeAsync() => Task.CompletedTask;

    private async Task<(EnrollResponse enrollment, string token, string? appleSerial, string? appleAuthToken)>
        EnrollAndGetDetailsAsync(string? phone = null)
    {
        phone ??= $"+551199999{Random.Shared.Next(1000, 9999)}";

        _client.DefaultRequestHeaders.Authorization = null;
        var response = await _client.PostAsJsonAsync($"/api/enroll/{_campaignId}", new
        {
            nome = "Apple Test Customer",
            telefone = phone,
        });
        var enrollment = await response.Content.ReadFromJsonAsync<EnrollResponse>();

        // Buscar token e apple fields do DB
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<Voltei.Api.Data.AppDbContext>();
        var dbEnrollment = await db.Enrollments.FindAsync(Guid.Parse(enrollment!.EnrollmentId));

        AuthHelper.SetBearerToken(_client, _authToken);

        return (enrollment, dbEnrollment!.Token, dbEnrollment.ApplePassSerial, dbEnrollment.ApplePassAuthToken);
    }

    [Fact]
    public async Task AppleWalletService_IsRegisteredInDI()
    {
        using var scope = _factory.Services.CreateScope();
        var service = scope.ServiceProvider
            .GetService<Voltei.Api.Services.Wallet.AppleWalletService>();
        service.Should().NotBeNull();
    }

    [Fact]
    public async Task Enroll_WithAppleCert_SetsApplePassSerial()
    {
        var (enrollment, _, appleSerial, appleAuthToken) = await EnrollAndGetDetailsAsync();

        // Se o certificado de dev existe, os campos Apple são preenchidos
        var service = _factory.Services.GetService<Voltei.Api.Services.Wallet.AppleWalletService>();
        if (service?.IsConfigured == true)
        {
            appleSerial.Should().NotBeNullOrEmpty();
            appleAuthToken.Should().NotBeNullOrEmpty();
            enrollment.ApplePassUrl.Should().NotBeNullOrEmpty();
            enrollment.ApplePassUrl.Should().Contain("pass.pkpass");
        }
        else
        {
            // Sem certificado, campos ficam null
            enrollment.ApplePassUrl.Should().BeNull();
        }

        await Task.CompletedTask;
    }

    [Fact]
    public async Task DownloadPass_ValidEnrollment_ReturnsPkpass()
    {
        var (enrollment, _, appleSerial, _) = await EnrollAndGetDetailsAsync();

        var service = _factory.Services.GetService<Voltei.Api.Services.Wallet.AppleWalletService>();
        if (service?.IsConfigured != true)
        {
            // Skip se Apple não configurado
            return;
        }

        _client.DefaultRequestHeaders.Authorization = null;
        var response = await _client.GetAsync($"/api/enroll/{enrollment.EnrollmentId}/pass.pkpass");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType!.MediaType.Should().Be("application/vnd.apple.pkpass");

        // Verificar que é um ZIP válido
        var bytes = await response.Content.ReadAsByteArrayAsync();
        bytes.Length.Should().BeGreaterThan(0);

        using var ms = new MemoryStream(bytes);
        using var archive = new ZipArchive(ms, ZipArchiveMode.Read);

        var entryNames = archive.Entries.Select(e => e.FullName).ToList();
        entryNames.Should().Contain("pass.json");
        entryNames.Should().Contain("manifest.json");
        entryNames.Should().Contain("signature");
        entryNames.Should().Contain("icon.png");
        entryNames.Should().Contain("logo.png");
    }

    [Fact]
    public async Task DownloadPass_PkpassContainsValidPassJson()
    {
        var (enrollment, _, _, _) = await EnrollAndGetDetailsAsync();

        var service = _factory.Services.GetService<Voltei.Api.Services.Wallet.AppleWalletService>();
        if (service?.IsConfigured != true) return;

        _client.DefaultRequestHeaders.Authorization = null;
        var response = await _client.GetAsync($"/api/enroll/{enrollment.EnrollmentId}/pass.pkpass");
        var bytes = await response.Content.ReadAsByteArrayAsync();

        using var ms = new MemoryStream(bytes);
        using var archive = new ZipArchive(ms, ZipArchiveMode.Read);

        var passEntry = archive.GetEntry("pass.json")!;
        using var reader = new StreamReader(passEntry.Open());
        var passJsonStr = await reader.ReadToEndAsync();
        var passJson = JsonDocument.Parse(passJsonStr);
        var root = passJson.RootElement;

        root.GetProperty("formatVersion").GetInt32().Should().Be(1);
        root.GetProperty("passTypeIdentifier").GetString().Should().NotBeNullOrEmpty();
        root.GetProperty("teamIdentifier").GetString().Should().NotBeNullOrEmpty();
        root.GetProperty("serialNumber").GetString().Should().NotBeNullOrEmpty();
        root.GetProperty("organizationName").GetString().Should().Be("Apple Wallet Biz");
        root.GetProperty("storeCard").Should().NotBeNull();
        root.GetProperty("barcode").GetProperty("format").GetString().Should().Be("PKBarcodeFormatQR");
    }

    [Fact]
    public async Task DownloadPass_ManifestHashesMatchFiles()
    {
        var (enrollment, _, _, _) = await EnrollAndGetDetailsAsync();

        var service = _factory.Services.GetService<Voltei.Api.Services.Wallet.AppleWalletService>();
        if (service?.IsConfigured != true) return;

        _client.DefaultRequestHeaders.Authorization = null;
        var response = await _client.GetAsync($"/api/enroll/{enrollment.EnrollmentId}/pass.pkpass");
        var bytes = await response.Content.ReadAsByteArrayAsync();

        using var ms = new MemoryStream(bytes);
        using var archive = new ZipArchive(ms, ZipArchiveMode.Read);

        // Ler manifest
        var manifestEntry = archive.GetEntry("manifest.json")!;
        using var manifestReader = new StreamReader(manifestEntry.Open());
        var manifestStr = await manifestReader.ReadToEndAsync();
        var manifest = JsonSerializer.Deserialize<Dictionary<string, string>>(manifestStr)!;

        // Verificar que cada arquivo no manifest tem hash correto
        foreach (var (filename, expectedHash) in manifest)
        {
            var entry = archive.GetEntry(filename);
            entry.Should().NotBeNull($"manifest referencia '{filename}' que deve existir no ZIP");

            using var entryMs = new MemoryStream();
            using var entryStream = entry!.Open();
            await entryStream.CopyToAsync(entryMs);

            var actualHash = Convert.ToHexStringLower(
                System.Security.Cryptography.SHA256.HashData(entryMs.ToArray()));
            actualHash.Should().Be(expectedHash, $"hash de '{filename}' deve bater com o manifest");
        }
    }

    [Fact]
    public async Task DownloadPass_NonExistentEnrollment_Returns404()
    {
        _client.DefaultRequestHeaders.Authorization = null;
        var response = await _client.GetAsync($"/api/enroll/{Guid.NewGuid()}/pass.pkpass");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // --- Apple Web Service endpoints ---

    [Fact]
    public async Task RegisterDevice_ValidAuth_Returns201()
    {
        var (_, _, appleSerial, appleAuthToken) = await EnrollAndGetDetailsAsync();

        var service = _factory.Services.GetService<Voltei.Api.Services.Wallet.AppleWalletService>();
        if (service?.IsConfigured != true || appleSerial == null) return;

        var passTypeId = "pass.com.voltei.loyalty";
        var deviceId = Guid.NewGuid().ToString("N");

        var request = new HttpRequestMessage(HttpMethod.Post,
            $"/api/apple-wallet/v1/devices/{deviceId}/registrations/{passTypeId}/{appleSerial}");
        request.Headers.Authorization = new AuthenticationHeaderValue("ApplePass", appleAuthToken);
        request.Content = JsonContent.Create(new { pushToken = "fake-push-token-123" });

        var response = await _client.SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task RegisterDevice_InvalidAuth_Returns401()
    {
        var request = new HttpRequestMessage(HttpMethod.Post,
            "/api/apple-wallet/v1/devices/device123/registrations/pass.com.voltei.loyalty/fake-serial");
        request.Headers.Authorization = new AuthenticationHeaderValue("ApplePass", "invalid-token");

        var response = await _client.SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RegisterDevice_Twice_Returns200OnSecond()
    {
        var (_, _, appleSerial, appleAuthToken) = await EnrollAndGetDetailsAsync();

        var service = _factory.Services.GetService<Voltei.Api.Services.Wallet.AppleWalletService>();
        if (service?.IsConfigured != true || appleSerial == null) return;

        var passTypeId = "pass.com.voltei.loyalty";
        var deviceId = Guid.NewGuid().ToString("N");

        // First registration
        var req1 = new HttpRequestMessage(HttpMethod.Post,
            $"/api/apple-wallet/v1/devices/{deviceId}/registrations/{passTypeId}/{appleSerial}");
        req1.Headers.Authorization = new AuthenticationHeaderValue("ApplePass", appleAuthToken);
        req1.Content = JsonContent.Create(new { pushToken = "token-1" });
        var resp1 = await _client.SendAsync(req1);
        resp1.StatusCode.Should().Be(HttpStatusCode.Created);

        // Second registration (same device)
        var req2 = new HttpRequestMessage(HttpMethod.Post,
            $"/api/apple-wallet/v1/devices/{deviceId}/registrations/{passTypeId}/{appleSerial}");
        req2.Headers.Authorization = new AuthenticationHeaderValue("ApplePass", appleAuthToken);
        req2.Content = JsonContent.Create(new { pushToken = "token-2" });
        var resp2 = await _client.SendAsync(req2);
        resp2.StatusCode.Should().Be(HttpStatusCode.OK); // 200 = já existia
    }

    [Fact]
    public async Task GetLatestPass_ValidAuth_ReturnsPkpass()
    {
        var (_, _, appleSerial, appleAuthToken) = await EnrollAndGetDetailsAsync();

        var service = _factory.Services.GetService<Voltei.Api.Services.Wallet.AppleWalletService>();
        if (service?.IsConfigured != true || appleSerial == null) return;

        var passTypeId = "pass.com.voltei.loyalty";

        var request = new HttpRequestMessage(HttpMethod.Get,
            $"/api/apple-wallet/v1/passes/{passTypeId}/{appleSerial}");
        request.Headers.Authorization = new AuthenticationHeaderValue("ApplePass", appleAuthToken);

        var response = await _client.SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType!.MediaType.Should().Be("application/vnd.apple.pkpass");
    }

    [Fact]
    public async Task LogEndpoint_Returns200()
    {
        var response = await _client.PostAsJsonAsync("/api/apple-wallet/v1/log", new { logs = new[] { "test error" } });
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
