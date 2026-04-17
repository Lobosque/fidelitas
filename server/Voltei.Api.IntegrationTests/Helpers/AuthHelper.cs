using System.Net.Http.Headers;
using System.Net.Http.Json;
using Voltei.Api.Dto;

namespace Voltei.Api.IntegrationTests.Helpers;

public static class AuthHelper
{
    public static async Task<(HttpResponseMessage Response, AuthResponse? Body)> SignupAsync(
        HttpClient client, string nomeNegocio, string email, string senha)
    {
        var response = await client.PostAsJsonAsync("/api/auth/signup", new
        {
            nomeNegocio,
            email,
            senha,
        });

        AuthResponse? body = null;
        if (response.IsSuccessStatusCode)
            body = await response.Content.ReadFromJsonAsync<AuthResponse>();

        return (response, body);
    }

    public static async Task<(HttpResponseMessage Response, AuthResponse? Body)> LoginAsync(
        HttpClient client, string email, string password)
    {
        var response = await client.PostAsJsonAsync("/api/auth/login", new
        {
            email,
            password,
        });

        AuthResponse? body = null;
        if (response.IsSuccessStatusCode)
            body = await response.Content.ReadFromJsonAsync<AuthResponse>();

        return (response, body);
    }

    public static void SetBearerToken(HttpClient client, string token)
    {
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }

    public static string UniqueEmail() => $"test-{Guid.NewGuid():N}@example.com";
}
