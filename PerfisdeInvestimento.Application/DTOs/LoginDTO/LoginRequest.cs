using System.ComponentModel.DataAnnotations;

namespace PerfisdeInvestimento.Application.DTOs;

public class LoginRequest
{
    [Required(ErrorMessage = "Pessoa usuária obrigatória")]
    public string PessoaUsuaria { get; set; } = string.Empty;

    [Required(ErrorMessage = "Senha obrigatória")]
    public string Senha { get; set; } = string.Empty;
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public string TokenType { get; set; } = "Bearer";
}