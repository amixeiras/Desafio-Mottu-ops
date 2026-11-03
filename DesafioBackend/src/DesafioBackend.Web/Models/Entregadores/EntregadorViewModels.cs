using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace DesafioBackend.Web.Models.Entregadores;

public class EntregadorViewModel
{
    [JsonPropertyName("identificador")]
    public string Identificador { get; set; } = string.Empty;

    [JsonPropertyName("nome")]
    public string Nome { get; set; } = string.Empty;

    [JsonPropertyName("cnpj")]
    public string Cnpj { get; set; } = string.Empty;

    [JsonPropertyName("data_nascimento")]
    public DateTime DataNascimento { get; set; }

    [JsonPropertyName("numero_cnh")]
    public string NumeroCnh { get; set; } = string.Empty;

    [JsonPropertyName("tipo_cnh")]
    public string TipoCnh { get; set; } = string.Empty;

    [JsonPropertyName("imagem_cnh")]
    public string? ImagemCnh { get; set; }
}

public class CadastrarEntregadorViewModel
{
    [Required(ErrorMessage = "O identificador é obrigatório.")]
    [Display(Name = "Identificador")]
    [JsonPropertyName("identificador")]
    public string Identificador { get; set; } = string.Empty;

    [Required(ErrorMessage = "O nome é obrigatório.")]
    [Display(Name = "Nome")]
    [JsonPropertyName("nome")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O CNPJ é obrigatório.")]
    [Display(Name = "CNPJ")]
    [JsonPropertyName("cnpj")]
    public string Cnpj { get; set; } = string.Empty;

    [Required(ErrorMessage = "A data de nascimento é obrigatória.")]
    [DataType(DataType.Date)]
    [Display(Name = "Data de Nascimento")]
    [JsonPropertyName("data_nascimento")]
    public DateTime DataNascimento { get; set; } = DateTime.UtcNow.AddYears(-25).Date;

    [Required(ErrorMessage = "O número da CNH é obrigatório.")]
    [Display(Name = "Número da CNH")]
    [JsonPropertyName("numero_cnh")]
    public string NumeroCnh { get; set; } = string.Empty;

    [Required(ErrorMessage = "O tipo da CNH é obrigatório.")]
    [Display(Name = "Tipo de CNH")]
    [JsonPropertyName("tipo_cnh")]
    public string TipoCnh { get; set; } = "A";

    [Required(ErrorMessage = "A foto da CNH é obrigatória.")]
    [Display(Name = "Foto da CNH (PNG ou BMP)")]
    public IFormFile? FotoCnh { get; set; }
}

public class EnviarFotoCnhViewModel
{
    public string Id { get; set; } = string.Empty;

    [Required(ErrorMessage = "Selecione o arquivo da imagem.")]
    [Display(Name = "Nova Foto da CNH (PNG ou BMP)")]
    public IFormFile? FotoCnh { get; set; }
}

