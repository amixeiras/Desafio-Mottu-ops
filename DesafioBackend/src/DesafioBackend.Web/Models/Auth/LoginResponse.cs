using System.Text.Json.Serialization;

namespace DesafioBackend.Web.Models.Auth;

public sealed class LoginResponse
{
    [JsonPropertyName("token")]
    public string? Token { get; set; }

    [JsonPropertyName("access_token")]
    public string? AccessToken { get; set; }

    public string? GetToken() => Token ?? AccessToken;
}
