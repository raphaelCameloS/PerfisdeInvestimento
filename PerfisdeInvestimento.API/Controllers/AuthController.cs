using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PerfisdeInvestimento.Application.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PerfisdeInvestimento.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }
   
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        //Validação básica
        if (string.IsNullOrEmpty(request.PessoaUsuaria) || string.IsNullOrEmpty(request.Senha))
        {
            return BadRequest(new { error = "Username e password são obrigatórios" });
        }
        if (!IsValidUser(request.PessoaUsuaria, request.Senha))
        {
            return Unauthorized(new { error = "Credenciais inválidas" });
        }
        //Gerar token JWT
        var token = GenerateJwtToken(request.PessoaUsuaria);

        return Ok(new LoginResponse
        {
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddHours(2),
            TokenType = "Bearer"
        });
    }

    private bool IsValidUser(string username, string password)
    {
        // USUÁRIOS FIXOS PARA TESTE
        var validUsers = new Dictionary<string, string>
        {
            { "admin", "admin123" },
            { "investidor", "invest123" },
            { "analista", "analista123" }
        };

        return validUsers.TryGetValue(username, out var validPassword)
            && password == validPassword;
    }

    private string GenerateJwtToken(string username)
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Name, username),
            new Claim("app", "PerfisInvestimento")
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    [HttpPost("validate")]
    public IActionResult ValidateToken([FromBody] string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var username = jwtToken.Claims.First(x => x.Type == ClaimTypes.Name).Value;

            return Ok(new
            {
                valid = true,
                username = username,
                expires = jwtToken.ValidTo
            });
        }
        catch
        {
            return Ok(new { valid = false });
        }
    }
}