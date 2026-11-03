using System.Text.Json.Serialization;

namespace DesafioBackend.Application.DTOs;

public record CreateMotorcycleRequest(
    [property: JsonPropertyName("identificador")] string Identificador,
    [property: JsonPropertyName("ano")] int Ano,
    [property: JsonPropertyName("modelo")] string Modelo,
    [property: JsonPropertyName("placa")] string Placa
);

public record UpdateMotorcyclePlateRequest(
    [property: JsonPropertyName("placa")] string Placa
);

public record MotorcycleResponse(
    [property: JsonPropertyName("identificador")] string Identificador,
    [property: JsonPropertyName("ano")] int Ano,
    [property: JsonPropertyName("modelo")] string Modelo,
    [property: JsonPropertyName("placa")] string Placa
);

