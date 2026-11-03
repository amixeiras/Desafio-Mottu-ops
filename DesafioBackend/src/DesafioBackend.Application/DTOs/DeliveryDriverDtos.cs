using System.Text.Json.Serialization;

namespace DesafioBackend.Application.DTOs;

public record CreateDeliveryDriverRequest(
    [property: JsonPropertyName("identificador")] string Identificador,
    [property: JsonPropertyName("nome")] string Nome,
    [property: JsonPropertyName("cnpj")] string Cnpj,
    [property: JsonPropertyName("data_nascimento")] DateTime DataNascimento,
    [property: JsonPropertyName("numero_cnh")] string NumeroCnh,
    [property: JsonPropertyName("tipo_cnh")] string TipoCnh,
    [property: JsonPropertyName("imagem_cnh")] string ImagemCnh
);

public record UpdateCnhImageRequest(
    [property: JsonPropertyName("imagem_cnh")] string ImagemCnh
);

public record DeliveryDriverResponse(
    [property: JsonPropertyName("identificador")] string Identificador,
    [property: JsonPropertyName("nome")] string Nome,
    [property: JsonPropertyName("cnpj")] string Cnpj,
    [property: JsonPropertyName("data_nascimento")] DateTime DataNascimento,
    [property: JsonPropertyName("numero_cnh")] string NumeroCnh,
    [property: JsonPropertyName("tipo_cnh")] string TipoCnh,
    [property: JsonPropertyName("imagem_cnh")] string? ImagemCnh
);

