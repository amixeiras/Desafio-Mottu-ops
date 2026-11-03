using System.Net.Http.Json;
using System.Text.Json;

namespace DesafioBackend.Web.Services;

public sealed class ApiClient(IHttpClientFactory httpClientFactory)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<T?> GetAsync<T>(string path, CancellationToken cancellationToken = default)
    {
        using var client = httpClientFactory.CreateClient("Api");
        using var response = await client.GetAsync(path, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return default;
        }
        return await response.Content.ReadFromJsonAsync<T>(JsonOptions, cancellationToken);
    }

    public async Task<ApiResponse<TResponse>> GetWithResultAsync<TResponse>(string path, CancellationToken cancellationToken = default)
    {
        using var client = httpClientFactory.CreateClient("Api");
        using var response = await client.GetAsync(path, cancellationToken);
        return await HandleResponseAsync<TResponse>(response, cancellationToken);
    }

    public async Task<ApiResponse> PostAsync<TRequest>(string path, TRequest body, CancellationToken cancellationToken = default)
    {
        using var client = httpClientFactory.CreateClient("Api");
        using var response = await client.PostAsJsonAsync(path, body, JsonOptions, cancellationToken);
        return await HandleResponseAsync(response, cancellationToken);
    }

    public async Task<ApiResponse<TResponse>> PostAsync<TRequest, TResponse>(string path, TRequest body, CancellationToken cancellationToken = default)
    {
        using var client = httpClientFactory.CreateClient("Api");
        using var response = await client.PostAsJsonAsync(path, body, JsonOptions, cancellationToken);
        return await HandleResponseAsync<TResponse>(response, cancellationToken);
    }

    public async Task<ApiResponse> PutAsync<TRequest>(string path, TRequest body, CancellationToken cancellationToken = default)
    {
        using var client = httpClientFactory.CreateClient("Api");
        using var response = await client.PutAsJsonAsync(path, body, JsonOptions, cancellationToken);
        return await HandleResponseAsync(response, cancellationToken);
    }

    public async Task<ApiResponse<TResponse>> PutAsync<TRequest, TResponse>(string path, TRequest body, CancellationToken cancellationToken = default)
    {
        using var client = httpClientFactory.CreateClient("Api");
        using var response = await client.PutAsJsonAsync(path, body, JsonOptions, cancellationToken);
        return await HandleResponseAsync<TResponse>(response, cancellationToken);
    }

    public async Task<ApiResponse> DeleteAsync(string path, CancellationToken cancellationToken = default)
    {
        using var client = httpClientFactory.CreateClient("Api");
        using var response = await client.DeleteAsync(path, cancellationToken);
        return await HandleResponseAsync(response, cancellationToken);
    }

    private static async Task<ApiResponse> HandleResponseAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
        {
            return ApiResponse.Success(response.StatusCode);
        }

        var errorMessage = await ExtractErrorMessageAsync(response, cancellationToken);
        return ApiResponse.Failure(response.StatusCode, errorMessage);
    }

    private static async Task<ApiResponse<T>> HandleResponseAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
        {
            var data = await response.Content.ReadFromJsonAsync<T>(JsonOptions, cancellationToken);
            return ApiResponse<T>.Success(data!, response.StatusCode);
        }

        var errorMessage = await ExtractErrorMessageAsync(response, cancellationToken);
        return ApiResponse<T>.Failure(response.StatusCode, errorMessage);
    }

    private static async Task<string> ExtractErrorMessageAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        try
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            if (string.IsNullOrWhiteSpace(content))
            {
                return $"Erro na requisição ({response.StatusCode}).";
            }

            using var doc = JsonDocument.Parse(content);
            if (doc.RootElement.TryGetProperty("mensagem", out var msgProp) && !string.IsNullOrWhiteSpace(msgProp.GetString()))
            {
                return msgProp.GetString()!;
            }

            if (doc.RootElement.TryGetProperty("title", out var titleProp) && !string.IsNullOrWhiteSpace(titleProp.GetString()))
            {
                return titleProp.GetString()!;
            }

            return content;
        }
        catch
        {
            return $"Erro na requisição ({response.StatusCode}).";
        }
    }
}
