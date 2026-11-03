using System.Net.Http.Json;
using System.Text.Json;
using DesafioBackend.Web.Configuration;
using DesafioBackend.Web.Models.Auth;
using Microsoft.Extensions.Options;

namespace DesafioBackend.Web.Services;

public sealed class AuthService(
    IHttpClientFactory httpClientFactory,
    IOptions<ApiOptions> apiOptions)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<string?> LoginAsync(
        LoginViewModel model,
        CancellationToken cancellationToken = default)
    {
        using var client = httpClientFactory.CreateClient("Api");
        using var response = await client.PostAsJsonAsync(
            apiOptions.Value.AuthPath,
            model,
            JsonOptions,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var result = await response.Content.ReadFromJsonAsync<LoginResponse>(
            JsonOptions,
            cancellationToken);

        return result?.GetToken();
    }
}
