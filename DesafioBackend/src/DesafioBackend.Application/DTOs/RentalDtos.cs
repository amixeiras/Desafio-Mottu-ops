using System.Text.Json.Serialization;

namespace DesafioBackend.Application.DTOs;

public record CreateRentalRequest(
    [property: JsonPropertyName("entregador_id")] string EntregadorId,
    [property: JsonPropertyName("moto_id")] string MotoId,
    [property: JsonPropertyName("data_inicio")] DateTime DataInicio,
    [property: JsonPropertyName("data_termino")] DateTime DataTermino,
    [property: JsonPropertyName("data_previsao_termino")] DateTime DataPrevisaoTermino,
    [property: JsonPropertyName("plano")] int Plano
);

public record RentalResponse(
    [property: JsonPropertyName("identificador")] string Identificador,
    [property: JsonPropertyName("valor_diaria")] decimal ValorDiaria,
    [property: JsonPropertyName("entregador_id")] string EntregadorId,
    [property: JsonPropertyName("moto_id")] string MotoId,
    [property: JsonPropertyName("data_inicio")] DateTime DataInicio,
    [property: JsonPropertyName("data_termino")] DateTime DataTermino,
    [property: JsonPropertyName("data_previsao_termino")] DateTime DataPrevisaoTermino,
    [property: JsonPropertyName("data_devolucao")] DateTime? DataDevolucao,
    [property: JsonPropertyName("valor_total")] decimal? ValorTotal = null
);

public record ReturnRentalRequest(
    [property: JsonPropertyName("data_devolucao")] DateTime DataDevolucao
);

public record ReturnRentalResponse(
    [property: JsonPropertyName("mensagem")] string Mensagem,
    [property: JsonPropertyName("valor_total")] decimal ValorTotal
);

public record MessageResponse(
    [property: JsonPropertyName("mensagem")] string Mensagem
);

