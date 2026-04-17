using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Voltei.Api.Dto;
using Voltei.Api.IntegrationTests.Fixtures;
using Voltei.Api.IntegrationTests.Helpers;

namespace Voltei.Api.IntegrationTests.Checkin;

public class CheckinTests : IClassFixture<VolteiApiFactory>, IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly VolteiApiFactory _factory;
    private string _authToken = string.Empty;
    private string _campaignId = string.Empty;

    public CheckinTests(VolteiApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        var (_, auth) = await AuthHelper.SignupAsync(
            _client, "Checkin Test Biz", AuthHelper.UniqueEmail(), "senha123");
        _authToken = auth!.Token;
        AuthHelper.SetBearerToken(_client, _authToken);

        var response = await _client.PostAsJsonAsync("/api/campaigns", new
        {
            nome = "Café Fidelidade",
            descricao = "1 café grátis",
            checkinsNecessarios = 3,
        });
        var campaign = await response.Content.ReadFromJsonAsync<CampaignResponse>();
        _campaignId = campaign!.Id;
    }

    public Task DisposeAsync() => Task.CompletedTask;

    private async Task<EnrollResponse> EnrollCustomerAsync(string phone)
    {
        // Enroll without auth
        _client.DefaultRequestHeaders.Authorization = null;
        var response = await _client.PostAsJsonAsync($"/api/enroll/{_campaignId}", new
        {
            nome = "Test Customer",
            telefone = phone,
        });
        var enrollment = await response.Content.ReadFromJsonAsync<EnrollResponse>();

        // Restore auth
        AuthHelper.SetBearerToken(_client, _authToken);
        return enrollment!;
    }

    private async Task<string> GetEnrollmentTokenAsync(string enrollmentId)
    {
        // We need the token from the enrollment — read it via the status endpoint
        // For simplicity, we'll use the DB directly via a workaround:
        // The token is not exposed in EnrollResponse, so we need another approach.
        // Let's add a helper that gets the token by looking up via enrollment ID.
        // For now, we can use the fact that the token is used for checkins.
        // Actually, we need to expose the token somehow for scanning.
        // Let's check if we can get it from the status response or enrollment response.

        // The token isn't in EnrollResponse — this is a gap we need to fix.
        // For testing, let's access the DB directly through the factory.
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<Voltei.Api.Data.AppDbContext>();
        var enrollment = await db.Enrollments.FindAsync(Guid.Parse(enrollmentId));
        return enrollment!.Token;
    }

    [Fact]
    public async Task Checkin_ValidToken_IncrementsCounter()
    {
        var phone = $"+551199999{Random.Shared.Next(1000, 9999)}";
        var enrollment = await EnrollCustomerAsync(phone);
        var token = await GetEnrollmentTokenAsync(enrollment.EnrollmentId);

        var response = await _client.PostAsJsonAsync("/api/checkins", new
        {
            enrollmentToken = token,
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var checkin = await response.Content.ReadFromJsonAsync<CheckinResponse>();
        checkin.Should().NotBeNull();
        checkin!.ClienteNome.Should().Be("Test Customer");
        checkin.CampanhaNome.Should().Be("Café Fidelidade");
        checkin.CheckinsAtuais.Should().Be(1);
        checkin.CheckinsNecessarios.Should().Be(3);
        checkin.RewardReached.Should().BeFalse();
    }

    [Fact]
    public async Task Checkin_ReachesGoal_ReturnsRewardReached()
    {
        var phone = $"+551199999{Random.Shared.Next(1000, 9999)}";
        var enrollment = await EnrollCustomerAsync(phone);
        var token = await GetEnrollmentTokenAsync(enrollment.EnrollmentId);

        // 3 checkins needed — do all 3
        for (int i = 0; i < 2; i++)
        {
            await _client.PostAsJsonAsync("/api/checkins", new { enrollmentToken = token });
        }

        var response = await _client.PostAsJsonAsync("/api/checkins", new
        {
            enrollmentToken = token,
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var checkin = await response.Content.ReadFromJsonAsync<CheckinResponse>();
        checkin!.CheckinsAtuais.Should().Be(3);
        checkin.RewardReached.Should().BeTrue();
    }

    [Fact]
    public async Task Checkin_AlreadyCompleted_Returns400()
    {
        var phone = $"+551199999{Random.Shared.Next(1000, 9999)}";
        var enrollment = await EnrollCustomerAsync(phone);
        var token = await GetEnrollmentTokenAsync(enrollment.EnrollmentId);

        // Complete all checkins
        for (int i = 0; i < 3; i++)
        {
            await _client.PostAsJsonAsync("/api/checkins", new { enrollmentToken = token });
        }

        // Extra checkin should fail
        var response = await _client.PostAsJsonAsync("/api/checkins", new
        {
            enrollmentToken = token,
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Checkin_InvalidToken_Returns400()
    {
        var response = await _client.PostAsJsonAsync("/api/checkins", new
        {
            enrollmentToken = "token-invalido-123",
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Checkin_NoAuth_Returns401()
    {
        _client.DefaultRequestHeaders.Authorization = null;

        var response = await _client.PostAsJsonAsync("/api/checkins", new
        {
            enrollmentToken = "qualquer-token",
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Redeem_CompletedCampaign_ReturnsSuccess()
    {
        var phone = $"+551199999{Random.Shared.Next(1000, 9999)}";
        var enrollment = await EnrollCustomerAsync(phone);
        var token = await GetEnrollmentTokenAsync(enrollment.EnrollmentId);

        // Complete all checkins
        for (int i = 0; i < 3; i++)
        {
            await _client.PostAsJsonAsync("/api/checkins", new { enrollmentToken = token });
        }

        var response = await _client.PostAsJsonAsync(
            $"/api/checkins/{enrollment.EnrollmentId}/redeem", new { });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Redeem_NotCompleted_Returns400()
    {
        var phone = $"+551199999{Random.Shared.Next(1000, 9999)}";
        var enrollment = await EnrollCustomerAsync(phone);

        var response = await _client.PostAsJsonAsync(
            $"/api/checkins/{enrollment.EnrollmentId}/redeem", new { });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Redeem_AlreadyRedeemed_Returns400()
    {
        var phone = $"+551199999{Random.Shared.Next(1000, 9999)}";
        var enrollment = await EnrollCustomerAsync(phone);
        var token = await GetEnrollmentTokenAsync(enrollment.EnrollmentId);

        for (int i = 0; i < 3; i++)
        {
            await _client.PostAsJsonAsync("/api/checkins", new { enrollmentToken = token });
        }

        await _client.PostAsJsonAsync(
            $"/api/checkins/{enrollment.EnrollmentId}/redeem", new { });

        // Second redeem should fail
        var response = await _client.PostAsJsonAsync(
            $"/api/checkins/{enrollment.EnrollmentId}/redeem", new { });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
