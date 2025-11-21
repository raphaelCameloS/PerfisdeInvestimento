namespace PerfisdeInvestimento.Application.DTOs
{
    public class HistoricoSimulacaoResponse
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public required string Produto { get; set; }
        public decimal ValorInvestido { get; set; }
        public decimal ValorFinal { get; set; }
        public int PrazoMeses { get; set; }
        public DateTime DataSimulacao { get; set; }
    }
}
