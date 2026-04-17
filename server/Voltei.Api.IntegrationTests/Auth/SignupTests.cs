using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Voltei.Api.IntegrationTests.Fixtures;
using Voltei.Api.IntegrationTests.Helpers;

namespace Voltei.Api.IntegrationTests.Auth;

public class SignupTests(VolteiApiFactory factory) : IClassFixture<VolteiApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Signup_ValidRequest_ReturnsOkWithTokenAndUser()
    {
        var email = AuthHelper.UniqueEmail();

        var (response, body) = await AuthHelper.SignupAsync(_client, "Meu Negócio", email, "senha123");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body.Should().NotBeNull();
        body!.Token.Should().NotBeNullOrWhiteSpace();
        body.User.Email.Should().Be(email);
        body.User.Nome.Should().Be("Meu Negócio");
        body.User.NegocioId.Should().NotBeNullOrWhiteSpace();
        Guid.TryParse(body.User.Id, out _).Should().BeTrue();
        Guid.TryParse(body.User.NegocioId, out _).Should().BeTrue();
    }

    [Fact]
    public async Task Signup_DuplicateEmail_Returns409Conflict()
    {
        var email = AuthHelper.UniqueEmail();

        var (first, _) = await AuthHelper.SignupAsync(_client, "Primeiro", email, "senha123");
        first.StatusCode.Should().Be(HttpStatusCode.OK);

        var (second, _) = await AuthHelper.SignupAsync(_client, "Segundo", email, "senha123");
        second.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var error = await second.Content.ReadFromJsonAsync<ErrorMessage>();
        error!.Message.Should().Be("Email já cadastrado.");
    }

    [Fact]
    public async Task Signup_InvalidEmail_Returns400()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/signup", new
        {
            nomeNegocio = "Test",
            email = "not-an-email",
            senha = "senha123",
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Signup_MissingSenha_Returns400()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/signup", new
        {
            nomeNegocio = "Test",
            email = AuthHelper.UniqueEmail(),
            senha = "",
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Signup_SenhaTooShort_Returns400()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/signup", new
        {
            nomeNegocio = "Test",
            email = AuthHelper.UniqueEmail(),
            senha = "abc",
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Signup_TokenIsValidJwt()
    {
        var email = AuthHelper.UniqueEmail();

        var (_, body) = await AuthHelper.SignupAsync(_client, "JWT Test", email, "senha123");

        body.Should().NotBeNull();
        var handler = new JwtSecurityTokenHandler();
        handler.CanReadToken(body!.Token).Should().BeTrue();

        var jwt = handler.ReadJwtToken(body.Token);
        jwt.Subject.Should().NotBeNullOrWhiteSpace();
        jwt.Claims.Should().Contain(c => c.Type == "negocioId");
    }

    private record ErrorMessage(string Message);
}
