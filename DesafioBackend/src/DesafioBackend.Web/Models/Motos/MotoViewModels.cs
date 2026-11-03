using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DesafioBackend.Web.Models.Motos;

public class MotoViewModel
{
    [JsonPropertyName("identificador")]
    public string Identificador { get; set; } = string.Empty;

    [JsonPropertyName("ano")]
    public int Ano { get; set; }

    [JsonPropertyName("modelo")]
    public string Modelo { get; set; } = string.Empty;

    [JsonPropertyName("placa")]
    public string Placa { get; set; } = string.Empty;
}

public class CadastrarMotoViewModel
{
    [Required(ErrorMessage = "O identificador é obrigatório.")]
    [Display(Name = "Identificador")]
    [JsonPropertyName("identificador")]
    public string Identificador { get; set; } = string.Empty;

    [Required(ErrorMessage = "O ano é obrigatório.")]
    [Range(1900, 2100, ErrorMessage = "Informe um ano válido.")]
    [Display(Name = "Ano")]
    [JsonPropertyName("ano")]
    public int Ano { get; set; } = DateTime.UtcNow.Year;

    [Required(ErrorMessage = "O modelo é obrigatório.")]
    [Display(Name = "Modelo")]
    [JsonPropertyName("modelo")]
    public string Modelo { get; set; } = string.Empty;

    [Required(ErrorMessage = "A placa é obrigatória.")]
    [Display(Name = "Placa")]
    [JsonPropertyName("placa")]
    public string Placa { get; set; } = string.Empty;
}

public class EditarPlacaViewModel
{
    public string Id { get; set; } = string.Empty;

    [Required(ErrorMessage = "A nova placa é obrigatória.")]
    [Display(Name = "Nova Placa")]
    [JsonPropertyName("placa")]
    public string Placa { get; set; } = string.Empty;
}

