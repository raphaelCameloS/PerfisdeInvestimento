namespace PerfisdeInvestimento.Application.DTOs
{
    public class SimulacaoRequest
    {
        public int ClienteId { get; set; }
        public decimal Valor { get; set; }
        public int PrazoMeses { get; set; }
        public required string TipoProduto { get; set; }
    }
}
