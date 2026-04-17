using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Voltei.Api.Dto;
using Voltei.Api.IntegrationTests.Fixtures;
using Voltei.Api.IntegrationTests.Helpers;

namespace Voltei.Api.IntegrationTests.Enrollment;

public class EnrollmentTests : IClassFixture<VolteiApiFactory>, IAsyncLifetime
{
    private readonly HttpClient _client;
    private string _campaignId = string.Empty;

    public EnrollmentTests(VolteiApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        var (_, auth) = await AuthHelper.SignupAsync(
            _client, "Enroll Test Biz", AuthHelper.UniqueEmail(), "senha123");
        AuthHelper.SetBearerToken(_client, auth!.Token);

        var response = await _client.PostAsJsonAsync("/api/campaigns", new
        {
            nome = "Corte Fidelidade",
            descricao = "1 corte grátis",
            checkinsNecessarios = 10,
        });
        var campaign = await response.Content.ReadFromJsonAsync<CampaignResponse>();
        _campaignId = campaign!.Id;

        // Clear auth for public endpoints
        _client.DefaultRequestHeaders.Authorization = null;
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetCampaignInfo_ValidId_ReturnsCampaignPublicData()
    {
        var response = await _client.GetAsync($"/api/enroll/{_campaignId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var info = await response.Content.ReadFromJsonAsync<CampaignPublicResponse>();
        info.Should().NotBeNull();
        info!.Nome.Should().Be("Corte Fidelidade");
        info.Descricao.Should().Be("1 corte grátis");
        info.CheckinsNecessarios.Should().Be(10);
        info.NegocioNome.Should().Be("Enroll Test Biz");
        info.CoresPrimaria.Should().NotBeNullOrWhiteSpace();
        info.CoresSecundaria.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task GetCampaignInfo_NonExistentId_Returns404()
    {
        var response = await _client.GetAsync($"/api/enroll/{Guid.NewGuid()}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Enroll_NewCustomer_ReturnsEnrollment()
    {
        var response = await _client.PostAsJsonAsync($"/api/enroll/{_campaignId}", new
        {
            nome = "Maria Silva",
            telefone = "+5511999990001",
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var enrollment = await response.Content.ReadFromJsonAsync<EnrollResponse>();
        enrollment.Should().NotBeNull();
        enrollment!.CheckinsAtuais.Should().Be(0);
        enrollment.CheckinsNecessarios.Should().Be(10);
        enrollment.AlreadyEnrolled.Should().BeFalse();
        Guid.TryParse(enrollment.EnrollmentId, out _).Should().BeTrue();
    }

    [Fact]
    public async Task Enroll_SamePhoneTwice_ReturnsAlreadyEnrolled()
    {
        var phone = $"+551199999{Random.Shared.Next(1000, 9999)}";

        await _client.PostAsJsonAsync($"/api/enroll/{_campaignId}", new
        {
            nome = "João Santos",
            telefone = phone,
        });

        var response = await _client.PostAsJsonAsync($"/api/enroll/{_campaignId}", new
        {
            nome = "João Santos",
            telefone = phone,
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var enrollment = await response.Content.ReadFromJsonAsync<EnrollResponse>();
        enrollment!.AlreadyEnrolled.Should().BeTrue();
    }

    [Fact]
    public async Task Enroll_NonExistentCampaign_Returns404()
    {
        var response = await _client.PostAsJsonAsync($"/api/enroll/{Guid.NewGuid()}", new
        {
            nome = "Ana Costa",
            telefone = "+5511999990002",
        });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CheckStatus_ExistingEnrollment_ReturnsProgress()
    {
        var phone = $"+551199999{Random.Shared.Next(1000, 9999)}";

        await _client.PostAsJsonAsync($"/api/enroll/{_campaignId}", new
        {
            nome = "Carlos Lima",
            telefone = phone,
        });

        var response = await _client.GetAsync(
            $"/api/enroll/{_campaignId}/status?telefone={Uri.EscapeDataString(phone)}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var enrollment = await response.Content.ReadFromJsonAsync<EnrollResponse>();
        enrollment!.AlreadyEnrolled.Should().BeTrue();
        enrollment.CheckinsAtuais.Should().Be(0);
        enrollment.CheckinsNecessarios.Should().Be(10);
    }

    [Fact]
    public async Task CheckStatus_NotEnrolled_Returns404()
    {
        var response = await _client.GetAsync(
            $"/api/enroll/{_campaignId}/status?telefone=+5511000000000");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CheckStatus_MissingPhone_Returns400()
    {
        var response = await _client.GetAsync($"/api/enroll/{_campaignId}/status?telefone=");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
