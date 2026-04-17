using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Voltei.Api.Dto;
using Voltei.Api.IntegrationTests.Fixtures;
using Voltei.Api.IntegrationTests.Helpers;

namespace Voltei.Api.IntegrationTests.Campaigns;

public class CampaignsCrudTests : IClassFixture<VolteiApiFactory>, IAsyncLifetime
{
    private readonly HttpClient _client;

    public CampaignsCrudTests(VolteiApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        var (_, body) = await AuthHelper.SignupAsync(
            _client, "CRUD Test Biz", AuthHelper.UniqueEmail(), "senha123");
        AuthHelper.SetBearerToken(_client, body!.Token);
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task CreateCampaign_ValidRequest_Returns201()
    {
        var response = await _client.PostAsJsonAsync("/api/campaigns", new
        {
            nome = "Café Grátis",
            descricao = "1 café expresso",
            checkinsNecessarios = 10,
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var campaign = await response.Content.ReadFromJsonAsync<CampaignResponse>();
        campaign.Should().NotBeNull();
        campaign!.Nome.Should().Be("Café Grátis");
        campaign.Descricao.Should().Be("1 café expresso");
        campaign.CheckinsNecessarios.Should().Be(10);
        campaign.Ativa.Should().BeTrue();
        campaign.CriadaEm.Should().NotBeNullOrWhiteSpace();
        Guid.TryParse(campaign.Id, out _).Should().BeTrue();
        Guid.TryParse(campaign.NegocioId, out _).Should().BeTrue();
    }

    [Fact]
    public async Task CreateCampaign_CheckinsLessThan2_Returns400()
    {
        var response = await _client.PostAsJsonAsync("/api/campaigns", new
        {
            nome = "Test",
            descricao = "Test",
            checkinsNecessarios = 1,
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateCampaign_CheckinsGreaterThan50_Returns400()
    {
        var response = await _client.PostAsJsonAsync("/api/campaigns", new
        {
            nome = "Test",
            descricao = "Test",
            checkinsNecessarios = 51,
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateCampaign_MissingNome_Returns400()
    {
        var response = await _client.PostAsJsonAsync("/api/campaigns", new
        {
            nome = "",
            descricao = "Test",
            checkinsNecessarios = 5,
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ListCampaigns_ReturnsOnlyOwnCampaigns()
    {
        // Create 2 campaigns
        await _client.PostAsJsonAsync("/api/campaigns", new
        {
            nome = "Campanha A",
            descricao = "Desc A",
            checkinsNecessarios = 5,
        });
        await _client.PostAsJsonAsync("/api/campaigns", new
        {
            nome = "Campanha B",
            descricao = "Desc B",
            checkinsNecessarios = 8,
        });

        var response = await _client.GetAsync("/api/campaigns");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var campaigns = await response.Content.ReadFromJsonAsync<List<CampaignResponse>>();
        campaigns.Should().NotBeNull();
        campaigns!.Count.Should().BeGreaterThanOrEqualTo(2);

        // All should belong to the same negocio
        var negocioId = campaigns.First().NegocioId;
        campaigns.Should().OnlyContain(c => c.NegocioId == negocioId);
    }

    [Fact]
    public async Task ListCampaigns_Empty_ReturnsEmptyArray()
    {
        // Create a brand new user with no campaigns
        var freshClient = _client; // same factory but new user
        var (_, auth) = await AuthHelper.SignupAsync(
            freshClient, "Empty Biz", AuthHelper.UniqueEmail(), "senha123");

        var emptyClient = new HttpClient { BaseAddress = _client.BaseAddress };
        // Use the factory's handler
        emptyClient = _client; // reuse, but we'll need a clean approach

        // Actually, let's create a new client from the factory
        // Since we share the factory, let's just use a new HttpClient with the new token
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/campaigns");
        request.Headers.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", auth!.Token);

        var response = await _client.SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var campaigns = await response.Content.ReadFromJsonAsync<List<CampaignResponse>>();
        campaigns.Should().NotBeNull();
        campaigns!.Should().BeEmpty();
    }

    [Fact]
    public async Task GetCampaign_ExistingId_ReturnsCampaign()
    {
        var createResponse = await _client.PostAsJsonAsync("/api/campaigns", new
        {
            nome = "Get Test",
            descricao = "Get Desc",
            checkinsNecessarios = 7,
        });
        var created = await createResponse.Content.ReadFromJsonAsync<CampaignResponse>();

        var response = await _client.GetAsync($"/api/campaigns/{created!.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var campaign = await response.Content.ReadFromJsonAsync<CampaignResponse>();
        campaign.Should().NotBeNull();
        campaign!.Id.Should().Be(created.Id);
        campaign.Nome.Should().Be("Get Test");
        campaign.Descricao.Should().Be("Get Desc");
        campaign.CheckinsNecessarios.Should().Be(7);
    }

    [Fact]
    public async Task GetCampaign_NonExistentId_Returns404()
    {
        var response = await _client.GetAsync($"/api/campaigns/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var error = await response.Content.ReadFromJsonAsync<ErrorMessage>();
        error!.Message.Should().Be("Campanha não encontrada.");
    }

    private record ErrorMessage(string Message);
}
