using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Voltei.Api.Dto;
using Voltei.Api.IntegrationTests.Fixtures;
using Voltei.Api.IntegrationTests.Helpers;

namespace Voltei.Api.IntegrationTests.Wallet;

/// <summary>
/// Testa a integração do Google Wallet no fluxo de campanhas e enrollment.
/// Funciona tanto com credenciais configuradas quanto sem.
/// </summary>
public class GoogleWalletIntegrationTests : IClassFixture<VolteiApiFactory>, IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly VolteiApiFactory _factory;
    private string _authToken = string.Empty;
    private string _campaignId = string.Empty;

    public GoogleWalletIntegrationTests(VolteiApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        var (_, auth) = await AuthHelper.SignupAsync(
            _client, "Wallet Test Biz", AuthHelper.UniqueEmail(), "senha123");
        _authToken = auth!.Token;
        AuthHelper.SetBearerToken(_client, _authToken);

        var response = await _client.PostAsJsonAsync("/api/campaigns", new
        {
            nome = "Campanha Wallet Test",
            descricao = "1 prêmio grátis",
            checkinsNecessarios = 3,
        });
        var campaign = await response.Content.ReadFromJsonAsync<CampaignResponse>();
        _campaignId = campaign!.Id;
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task CreateCampaign_Succeeds_RegardlessOfWalletConfig()
    {
        var response = await _client.PostAsJsonAsync("/api/campaigns", new
        {
            nome = "Wallet Config Test",
            descricao = "Teste Google",
            checkinsNecessarios = 3,
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var campaign = await response.Content.ReadFromJsonAsync<CampaignResponse>();
        campaign.Should().NotBeNull();
        campaign!.Nome.Should().Be("Wallet Config Test");
    }

    [Fact]
    public async Task Enroll_Succeeds_RegardlessOfWalletConfig()
    {
        _client.DefaultRequestHeaders.Authorization = null;

        var phone = $"+551199999{Random.Shared.Next(1000, 9999)}";
        var response = await _client.PostAsJsonAsync($"/api/enroll/{_campaignId}", new
        {
            nome = "Cliente Google Test",
            telefone = phone,
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var enrollment = await response.Content.ReadFromJsonAsync<EnrollResponse>();
        enrollment.Should().NotBeNull();
        enrollment!.EnrollmentId.Should().NotBeNullOrEmpty();
        enrollment.CheckinsAtuais.Should().Be(0);
        enrollment.CheckinsNecessarios.Should().Be(3);
        enrollment.AlreadyEnrolled.Should().BeFalse();
    }

    [Fact]
    public async Task EnrollResponse_ContainsAllWalletFields()
    {
        _client.DefaultRequestHeaders.Authorization = null;

        var phone = $"+551199999{Random.Shared.Next(1000, 9999)}";
        var response = await _client.PostAsJsonAsync($"/api/enroll/{_campaignId}", new
        {
            nome = "Check Fields",
            telefone = phone,
        });

        var json = await response.Content.ReadAsStringAsync();
        json.Should().Contain("googleWalletSaveUrl");
        json.Should().Contain("applePassUrl");
        json.Should().Contain("enrollmentId");
        json.Should().Contain("checkinsAtuais");
        json.Should().Contain("checkinsNecessarios");
        json.Should().Contain("alreadyEnrolled");
    }

    [Fact]
    public async Task Checkin_Succeeds_RegardlessOfWalletConfig()
    {
        // Enroll
        _client.DefaultRequestHeaders.Authorization = null;
        var phone = $"+551199999{Random.Shared.Next(1000, 9999)}";

        var enrollResp = await _client.PostAsJsonAsync($"/api/enroll/{_campaignId}", new
        {
            nome = "Checkin Wallet Test",
            telefone = phone,
        });
        var enrollment = await enrollResp.Content.ReadFromJsonAsync<EnrollResponse>();

        // Get token from DB
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<Voltei.Api.Data.AppDbContext>();
        var dbEnrollment = await db.Enrollments.FindAsync(Guid.Parse(enrollment!.EnrollmentId));
        var token = dbEnrollment!.Token;

        // Checkin with auth
        AuthHelper.SetBearerToken(_client, _authToken);

        var checkinResp = await _client.PostAsJsonAsync("/api/checkins", new
        {
            enrollmentToken = token,
        });

        checkinResp.StatusCode.Should().Be(HttpStatusCode.OK);

        var checkin = await checkinResp.Content.ReadFromJsonAsync<CheckinResponse>();
        checkin.Should().NotBeNull();
        checkin!.CheckinsAtuais.Should().Be(1);
        checkin.CheckinsNecessarios.Should().Be(3);
        checkin.RewardReached.Should().BeFalse();
    }

    [Fact]
    public async Task GoogleWalletService_IsRegisteredInDI()
    {
        using var scope = _factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetService<Voltei.Api.Services.Wallet.GoogleWalletService>();
        service.Should().NotBeNull();
        await Task.CompletedTask;
    }
}
