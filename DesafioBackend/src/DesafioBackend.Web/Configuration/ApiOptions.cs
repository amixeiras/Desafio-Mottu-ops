namespace DesafioBackend.Web.Configuration;

public sealed class ApiOptions
{
    public const string SectionName = "Api";

    public string BaseUrl { get; set; } = "https://localhost:7001/";
    public string AuthPath { get; set; } = "auth/login";
}
