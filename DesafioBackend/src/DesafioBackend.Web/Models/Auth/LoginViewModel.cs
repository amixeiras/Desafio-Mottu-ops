using System.ComponentModel.DataAnnotations;

namespace DesafioBackend.Web.Models.Auth;

public sealed class LoginViewModel
{
    [Required]
    [Display(Name = "Usuário")]
    public string Username { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Senha")]
    public string Password { get; set; } = string.Empty;

    public string? ReturnUrl { get; set; }
}
