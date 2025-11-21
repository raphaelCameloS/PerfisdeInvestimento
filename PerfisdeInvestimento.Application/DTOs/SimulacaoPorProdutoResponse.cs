namespace PerfisdeInvestimento.Application.DTOs
{
    public class SimulacaoPorProdutoResponse
    {
        public required string Produto { get; set; }
        public required string Data { get; set; }
        public int QuantidadeSimulacoes { get; set; }
        public decimal MediaValorFinal { get; set; }
    }
}
