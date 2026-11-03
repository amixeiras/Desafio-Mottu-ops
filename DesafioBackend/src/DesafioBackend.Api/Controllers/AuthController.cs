using System.Security.Cryptography;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DesafioBackend.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace DesafioBackend.Api.Controllers;

[ApiController]
[Route("auth")]
public sealed class AuthController(IConfiguration configuration) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    public IActionResult Login(LoginRequest request)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new { mensagem = "Usuário e senha são obrigatórios." });
        }

        var configuredUsername = configuration["Authentication:Username"];
        var configuredPasswordHash = configuration["Authentication:PasswordHash"];

        if (!string.Equals(request.Username, configuredUsername, StringComparison.OrdinalIgnoreCase) ||
            !VerifyPassword(request.Password, configuredPasswordHash))
        {
            return Unauthorized(new { mensagem = "Usuário ou senha inválidos." });
        }

        var expiresAt = DateTime.UtcNow.AddHours(8);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, request.Username),
            new Claim(ClaimTypes.Name, request.Username),
            new Claim(ClaimTypes.Role, "Administrator")
        };
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(GetKey()),
            SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return Ok(new LoginResponse(
            new JwtSecurityTokenHandler().WriteToken(token),
            expiresAt));
    }

    private byte[] GetKey()
    {
        var key = configuration["Jwt:Key"];
        if (string.IsNullOrWhiteSpace(key) || key.Length < 32)
        {
            throw new InvalidOperationException("Jwt:Key deve possuir pelo menos 32 caracteres.");
        }

        return Encoding.UTF8.GetBytes(key);
    }

    private static bool VerifyPassword(string password, string? expectedHash)
    {
        if (string.IsNullOrWhiteSpace(expectedHash))
        {
            return false;
        }

        var actualHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(password)));
        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(actualHash),
            Encoding.UTF8.GetBytes(expectedHash.Trim().ToUpperInvariant()));
    }
}
