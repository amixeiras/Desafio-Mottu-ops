using System.Text.Json.Serialization;

namespace DesafioBackend.Application.DTOs;

public sealed record CreateDeliveryRequest(
    [property: JsonPropertyName("pedido_id")] string PedidoId,
    [property: JsonPropertyName("descricao")] string Descricao);

public sealed record AcceptDeliveryRequest(
    [property: JsonPropertyName("entregador_id")] string EntregadorId);

public sealed record DeliveryResponse(
    [property: JsonPropertyName("identificador")] string Identificador,
    [property: JsonPropertyName("pedido_id")] string PedidoId,
    [property: JsonPropertyName("descricao")] string Descricao,
    [property: JsonPropertyName("entregador_id")] string? EntregadorId,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("aceita_em")] DateTime? AceitaEm,
    [property: JsonPropertyName("concluida_em")] DateTime? ConcluidaEm);
