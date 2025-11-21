namespace PerfisdeInvestimento.Application.DTOs;

public class TelemetriaResponse
{
    public List<ServicoTelemetria> Servicos { get; set; } = new();
    public PeriodoTelemetria Periodo { get; set; } = new();
}

public class ServicoTelemetria
{
    public string Nome { get; set; } = string.Empty;
    public int QuantidadeChamadas { get; set; }
    public double MediaTempoRespostaMs { get; set; }
}

public class PeriodoTelemetria
{
    public string Inicio { get; set; } = string.Empty;
    public string Fim { get; set; } = string.Empty;
}
