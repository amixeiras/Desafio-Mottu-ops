using System.Text.Json.Serialization;

namespace DesafioBackend.Web.Models.Admin;

public sealed class AdminDashboardViewModel
{
    public IReadOnlyList<MotorcycleItem> Motorcycles { get; init; } = [];
    public IReadOnlyList<DriverItem> Drivers { get; init; } = [];
    public IReadOnlyList<RentalItem> Rentals { get; init; } = [];
    public IReadOnlyList<DeliveryItem> AvailableDeliveries { get; init; } = [];
    public IReadOnlyList<DeliveryItem> AcceptedDeliveries { get; init; } = [];
    public IReadOnlyList<DeliveryItem> CompletedDeliveries { get; init; } = [];
    public IReadOnlyList<EligibleDriverItem> EligibleDrivers { get; init; } = [];
    public string? ErrorMessage { get; init; }
}

public sealed record EligibleDriverItem(
    string Id,
    string Nome,
    string Cnh,
    string MotoId);

public sealed record MotorcycleItem(
    [property: JsonPropertyName("identificador")] string Identificador,
    [property: JsonPropertyName("ano")] int Ano,
    [property: JsonPropertyName("modelo")] string Modelo,
    [property: JsonPropertyName("placa")] string Placa);

public sealed record DriverItem(
    [property: JsonPropertyName("identificador")] string Identificador,
    [property: JsonPropertyName("nome")] string Nome,
    [property: JsonPropertyName("cnpj")] string Cnpj,
    [property: JsonPropertyName("data_nascimento")] DateTime DataNascimento,
    [property: JsonPropertyName("numero_cnh")] string NumeroCnh,
    [property: JsonPropertyName("tipo_cnh")] string TipoCnh,
    [property: JsonPropertyName("imagem_cnh")] string? ImagemCnh);

public sealed record RentalItem(
    [property: JsonPropertyName("identificador")] string Identificador,
    [property: JsonPropertyName("valor_diaria")] decimal ValorDiaria,
    [property: JsonPropertyName("entregador_id")] string EntregadorId,
    [property: JsonPropertyName("moto_id")] string MotoId,
    [property: JsonPropertyName("data_inicio")] DateTime DataInicio,
    [property: JsonPropertyName("data_termino")] DateTime DataTermino,
    [property: JsonPropertyName("data_previsao_termino")] DateTime DataPrevisaoTermino,
    [property: JsonPropertyName("data_devolucao")] DateTime? DataDevolucao,
    [property: JsonPropertyName("valor_total")] decimal? ValorTotal = null);

public sealed record DeliveryItem(
    [property: JsonPropertyName("identificador")] string Identificador,
    [property: JsonPropertyName("pedido_id")] string PedidoId,
    [property: JsonPropertyName("descricao")] string Descricao,
    [property: JsonPropertyName("entregador_id")] string? EntregadorId,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("aceita_em")] DateTime? AceitaEm,
    [property: JsonPropertyName("concluida_em")] DateTime? ConcluidaEm);
