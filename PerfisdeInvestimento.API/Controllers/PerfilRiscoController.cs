using Microsoft.AspNetCore.Mvc;
using PerfisdeInvestimento.Application.Interfaces.IServices;

namespace PerfisdeInvestimento.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PerfilRiscoController : ControllerBase
{
    private readonly IRecomendacaoService _recomendacaoService;
    private readonly ILogger<PerfilRiscoController> _logger;

    public PerfilRiscoController(IRecomendacaoService recomendacaoService, ILogger<PerfilRiscoController> logger)
    {
        _recomendacaoService = recomendacaoService;
        _logger = logger;
    }

    [HttpGet("{clienteId}")]
    public async Task<IActionResult> GetPerfilRisco(int clienteId)
    {
        try
        {
            _logger.LogInformation("Calculando perfil de risco para cliente {ClienteId}", clienteId);
            var perfil = await _recomendacaoService.CalcularPerfilRisco(clienteId);
            return Ok(perfil);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao calcular perfil para cliente {ClienteId}", clienteId);
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("produtos-recomendados/{perfil}")]
    public async Task<IActionResult> GetProdutosRecomendados(string perfil)
    {
        try
        {
            var produtos = await _recomendacaoService.GetProdutosRecomendados(perfil);
            return Ok(produtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar produtos para perfil {Perfil}", perfil);
            return BadRequest(new { error = ex.Message });
        }
    }
}