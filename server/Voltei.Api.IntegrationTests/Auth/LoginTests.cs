using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Voltei.Api.IntegrationTests.Fixtures;
using Voltei.Api.IntegrationTests.Helpers;

namespace Voltei.Api.IntegrationTests.Auth;

public class LoginTests : IClassFixture<VolteiApiFactory>, IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly string _email = AuthHelper.UniqueEmail();
    private const string Password = "senha123";

    public LoginTests(VolteiApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        // Create a user to test login against
        await AuthHelper.SignupAsync(_client, "Login Test Biz", _email, Password);
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Login_CorrectCredentials_ReturnsOkWithToken()
    {
        var (response, body) = await AuthHelper.LoginAsync(_client, _email, Password);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body.Should().NotBeNull();
        body!.Token.Should().NotBeNullOrWhiteSpace();
        body.User.Email.Should().Be(_email);
        body.User.Nome.Should().Be("Login Test Biz");
    }

    [Fact]
    public async Task Login_WrongPassword_Returns401()
    {
        var (response, _) = await AuthHelper.LoginAsync(_client, _email, "wrong-password");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var error = await response.Content.ReadFromJsonAsync<ErrorMessage>();
        error!.Message.Should().Be("Email ou senha incorretos.");
    }

    [Fact]
    public async Task Login_NonExistentEmail_Returns401()
    {
        var (response, _) = await AuthHelper.LoginAsync(_client, "nobody@nowhere.com", Password);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var error = await response.Content.ReadFromJsonAsync<ErrorMessage>();
        error!.Message.Should().Be("Email ou senha incorretos.");
    }

    [Fact]
    public async Task Login_InvalidEmailFormat_Returns400()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            email = "bad",
            password = Password,
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_MissingPassword_Returns400()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            email = _email,
            password = "",
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    private record ErrorMessage(string Message);
}
