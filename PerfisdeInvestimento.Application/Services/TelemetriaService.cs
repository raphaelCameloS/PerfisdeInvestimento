using PerfisdeInvestimento.Application.DTOs;


namespace PerfisdeInvestimento.Application.Services;

public class TelemetriaService : ITelemetriaService
{
    private readonly ITelemetriaRepository _telemetriaRepository;

    public TelemetriaService(ITelemetriaRepository telemetriaRepository)
    {
        _telemetriaRepository = telemetriaRepository;
    }

    public async Task<TelemetriaResponse> GetTelemetriaPorPeriodoAsync(DateTime inicio, DateTime fim)
    {
        var registros = await _telemetriaRepository.GetPorPeriodoAsync(inicio, fim);

        var servicosAgrupados = registros
            .GroupBy(r => r.Endpoint)
            .Select(g => new ServicoTelemetria
            {
                Nome = ExtrairNomeServico(g.Key),
                QuantidadeChamadas = g.Count(),
                MediaTempoRespostaMs = g.Average(r => r.ResponseTimeMs)
            })
            .ToList();

        return new TelemetriaResponse
        {
            Servicos = servicosAgrupados,
            Periodo = new PeriodoTelemetria
            {
                Inicio = inicio.ToString("yyyy-MM-dd"),
                Fim = fim.ToString("yyyy-MM-dd")
            }
        };
    }

    private string ExtrairNomeServico(string endpoint)
    {
        // Converte "/api/simulacoes/simular-investimento" → "simular-investimento"
        return endpoint.Split('/').LastOrDefault() ?? endpoint;
    }
}
