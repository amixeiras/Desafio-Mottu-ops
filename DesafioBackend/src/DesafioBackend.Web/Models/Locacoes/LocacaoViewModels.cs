using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DesafioBackend.Web.Models.Locacoes;

public class LocacaoViewModel
{
    [JsonPropertyName("identificador")]
    public string Identificador { get; set; } = string.Empty;

    [JsonPropertyName("valor_diaria")]
    public decimal ValorDiaria { get; set; }

    [JsonPropertyName("entregador_id")]
    public string EntregadorId { get; set; } = string.Empty;

    [JsonPropertyName("moto_id")]
    public string MotoId { get; set; } = string.Empty;

    [JsonPropertyName("data_inicio")]
    public DateTime DataInicio { get; set; }

    [JsonPropertyName("data_termino")]
    public DateTime DataTermino { get; set; }

    [JsonPropertyName("data_previsao_termino")]
    public DateTime DataPrevisaoTermino { get; set; }

    [JsonPropertyName("data_devolucao")]
    public DateTime? DataDevolucao { get; set; }

    [JsonPropertyName("valor_total")]
    public decimal ValorTotal { get; set; }

    public bool IsActive => !DataDevolucao.HasValue;
}

public class ReturnRentalResponse
{
    [JsonPropertyName("mensagem")]
    public string Mensagem { get; set; } = string.Empty;

    [JsonPropertyName("valor_total")]
    public decimal ValorTotal { get; set; }
}

public class NovaLocacaoViewModel
{
    [Required(ErrorMessage = "O entregador é obrigatório.")]
    [Display(Name = "Entregador")]
    public string EntregadorId { get; set; } = string.Empty;

    [Required(ErrorMessage = "A moto é obrigatória.")]
    [Display(Name = "Moto")]
    public string MotoId { get; set; } = string.Empty;

    [Required(ErrorMessage = "O plano é obrigatório.")]
    [Display(Name = "Plano de Locação")]
    public int Plano { get; set; } = 7;

    public IEnumerable<SelectListItem> EntregadoresDisponiveis { get; set; } = [];
    public IEnumerable<SelectListItem> MotosDisponiveis { get; set; } = [];
}

public class DevolverLocacaoViewModel
{
    public string Id { get; set; } = string.Empty;

    public string EntregadorId { get; set; } = string.Empty;
    public string MotoId { get; set; } = string.Empty;
    public decimal ValorDiaria { get; set; }
    public DateTime DataInicio { get; set; }
    public DateTime DataPrevisaoTermino { get; set; }

    [Required(ErrorMessage = "A data de devolução é obrigatória.")]
    [DataType(DataType.Date)]
    [Display(Name = "Data da Devolução")]
    public DateTime DataDevolucao { get; set; } = DateTime.UtcNow.Date;
}

