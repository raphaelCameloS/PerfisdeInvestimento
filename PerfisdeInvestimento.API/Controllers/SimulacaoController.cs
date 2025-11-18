using Microsoft.AspNetCore.Mvc;
using PerfisdeInvestimento.Application.DTOs;
using PerfisdeInvestimento.Application.Interfaces;
using PerfisdeInvestimento.Application.Interfaces.IServices;

namespace PerfisdeInvestimento.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SimulacaoController : ControllerBase
{
    private readonly ISimulacaoService _simulacaoService;
    private readonly ILogger<SimulacaoController> _logger;

    public SimulacaoController(ISimulacaoService simulacaoService, ILogger<SimulacaoController> logger)
    {
        _simulacaoService = simulacaoService;
        _logger = logger;
    }

    [HttpPost("simular-investimento")]
    public async Task<ActionResult<SimulacaoResponse>> SimularInvestimento([FromBody] SimulacaoRequest request)
    {
        _logger.LogInformation("Simulação solicitada para cliente {ClienteId}", request.ClienteId);
        var resultado = await _simulacaoService.SimularInvestimento(request);
        return Ok(resultado);
    
    }

    [HttpGet("simulacoes")]
    public async Task<ActionResult<List<HistoricoSimulacaoResponse>>> GetSimulacoes()
    {
        var simulacoes = await _simulacaoService.GetHistoricoSimulacoes();
        return Ok(simulacoes);
    }

    [HttpGet("simulacoes/por-produto-dia")]
    public async Task<ActionResult<List<SimulacaoPorProdutoResponse>>> GetSimulacoesPorProdutoDia()
    {
        var simulacoes = await _simulacaoService.GetSimulacoesPorProdutoDia();
        return Ok(simulacoes);
    }
}
