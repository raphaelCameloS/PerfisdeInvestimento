using Microsoft.AspNetCore.Mvc;
using PerfisdeInvestimento.Application.DTOs;
using PerfisdeInvestimento.Application.Interfaces.IServices;

namespace PerfisdeInvestimento.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TelemetriaController : ControllerBase
{
    private readonly ITelemetriaService _telemetriaService;

    public TelemetriaController(ITelemetriaService telemetriaService)
    {
        _telemetriaService = telemetriaService;
    }

    [HttpGet]
    public async Task<ActionResult<TelemetriaResponse>> GetTelemetria(
        [FromQuery] DateTime? inicio = null,
        [FromQuery] DateTime? fim = null)
    {
        try
        {
            var dataInicio = inicio ?? DateTime.UtcNow.AddMonths(-1);
            var dataFim = fim ?? DateTime.UtcNow;

            var resultado = await _telemetriaService.GetTelemetriaPorPeriodoAsync(dataInicio, dataFim);
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
}
