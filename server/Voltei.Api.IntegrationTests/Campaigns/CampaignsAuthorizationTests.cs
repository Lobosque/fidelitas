using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Voltei.Api.Dto;
using Voltei.Api.IntegrationTests.Fixtures;
using Voltei.Api.IntegrationTests.Helpers;

namespace Voltei.Api.IntegrationTests.Campaigns;

public class CampaignsAuthorizationTests(VolteiApiFactory factory) : IClassFixture<VolteiApiFactory>
{
    [Fact]
    public async Task ListCampaigns_NoToken_Returns401()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync("/api/campaigns");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateCampaign_NoToken_Returns401()
    {
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/campaigns", new
        {
            nome = "Test",
            descricao = "Test",
            checkinsNecessarios = 5,
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetCampaign_NoToken_Returns401()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync($"/api/campaigns/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetCampaign_BelongsToDifferentBusiness_Returns404()
    {
        // User A creates a campaign
        var clientA = factory.CreateClient();
        var (_, authA) = await AuthHelper.SignupAsync(clientA, "Biz A", AuthHelper.UniqueEmail(), "senha123");
        AuthHelper.SetBearerToken(clientA, authA!.Token);

        var createResponse = await clientA.PostAsJsonAsync("/api/campaigns", new
        {
            nome = "Campanha de A",
            descricao = "Só de A",
            checkinsNecessarios = 5,
        });
        var campaignA = await createResponse.Content.ReadFromJsonAsync<CampaignResponse>();

        // User B tries to access it
        var clientB = factory.CreateClient();
        var (_, authB) = await AuthHelper.SignupAsync(clientB, "Biz B", AuthHelper.UniqueEmail(), "senha123");
        AuthHelper.SetBearerToken(clientB, authB!.Token);

        var response = await clientB.GetAsync($"/api/campaigns/{campaignA!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ListCampaigns_DoesNotIncludeOtherBusinessCampaigns()
    {
        // User A creates a campaign
        var clientA = factory.CreateClient();
        var (_, authA) = await AuthHelper.SignupAsync(clientA, "Biz X", AuthHelper.UniqueEmail(), "senha123");
        AuthHelper.SetBearerToken(clientA, authA!.Token);

        await clientA.PostAsJsonAsync("/api/campaigns", new
        {
            nome = "Campanha X",
            descricao = "De X",
            checkinsNecessarios = 5,
        });

        // User B lists their campaigns — should NOT include A's campaign
        var clientB = factory.CreateClient();
        var (_, authB) = await AuthHelper.SignupAsync(clientB, "Biz Y", AuthHelper.UniqueEmail(), "senha123");
        AuthHelper.SetBearerToken(clientB, authB!.Token);

        var response = await clientB.GetAsync("/api/campaigns");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var campaigns = await response.Content.ReadFromJsonAsync<List<CampaignResponse>>();
        campaigns.Should().NotBeNull();
        campaigns!.Should().NotContain(c => c.Nome == "Campanha X");
    }
}
