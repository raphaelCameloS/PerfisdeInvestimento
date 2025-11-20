using Microsoft.AspNetCore.Mvc;
using PerfisdeInvestimento.Application.DTOs;
using PerfisdeInvestimento.Application.Interfaces.IServices;
using System.Globalization;

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

    [HttpGet("perfil-risco/{clienteId}")]
    [ProducesResponseType(typeof(PerfilRiscoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErroResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErroResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErroResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PerfilRiscoResponse>> GetPerfilRisco(int clienteId)
    {
        _logger.LogInformation("Calculando perfil de risco para cliente {ClienteId}", clienteId);
        var perfil = await _recomendacaoService.CalcularPerfilRisco(clienteId);
        return Ok(perfil);
    }

    [HttpGet("produtos-recomendados/{perfil}")]
    public async Task<IActionResult> GetProdutosRecomendados(string perfil)
    {
        //try
        //{
        //    var perfilNormalizado = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(perfil.ToLower());

        //    _logger.LogInformation("Buscando produtos para perfil: '{Perfil}' -> '{PerfilNormalizado}'",
        //        perfil, perfilNormalizado);

        //    var produtos = await _recomendacaoService.GetProdutosRecomendados(perfilNormalizado);
        //    return Ok(produtos);
        //}
        //catch (Exception ex)
        //{
        //    _logger.LogError(ex, "Erro ao buscar produtos para perfil {Perfil}", perfil);
        //    return BadRequest(new { error = ex.Message });
        //}
        var perfilNormalizado = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(perfil.ToLower());

        _logger.LogInformation("Buscando produtos para perfil: '{Perfil}' -> '{PerfilNormalizado}'",
            perfil, perfilNormalizado);

        var produtos = await _recomendacaoService.GetProdutosRecomendados(perfilNormalizado);
        return Ok(produtos);
    }
}