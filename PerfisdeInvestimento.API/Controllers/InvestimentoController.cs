using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PerfisdeInvestimento.Application.DTOs;
using PerfisdeInvestimento.Application.Interfaces.IServices;


namespace PerfisdeInvestimento.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InvestimentoController : ControllerBase
{
    private readonly IHistoricoInvestimentoService _historyService;
    private readonly ILogger<InvestimentoController> _logger;

    public InvestimentoController(
        IHistoricoInvestimentoService historyService,
        ILogger<InvestimentoController> logger)
    {
        _historyService = historyService;
        _logger = logger;
    }

    [Authorize]
    [HttpGet("{clienteId}")]
    [ProducesResponseType(typeof(PerfilRiscoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErroResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErroResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErroResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<HistoricoInvestimentosResponse>>> GetHistoricoInvestimentos(int clienteId)
    {
        //try
        //{
        //    _logger.LogInformation("Buscando histórico de investimentos para cliente {ClienteId}", clienteId);
        //    var historico = await _historyService.GetHistoricoInvestimentos(clienteId);
        //    return Ok(historico);
        //}
        //catch (Exception ex)
        //{
        //    _logger.LogError(ex, "Erro ao buscar histórico para cliente {ClienteId}", clienteId);
        //    return BadRequest(new { error = ex.Message });
        //}
        _logger.LogInformation("Buscando histórico de investimentos para cliente {ClienteId}", clienteId);
        var historico = await _historyService.GetHistoricoInvestimentos(clienteId);
        return Ok(historico);

    }
}