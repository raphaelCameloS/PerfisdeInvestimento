namespace PerfisdeInvestimento.Application.DTOs
{
    public class SimulacaoResponse
    {
        public required ProdutoValidado ProdutoValidado { get; set; }
        public required ResultadoSimulacao ResultadoSimulacao { get; set; }
        public DateTime DataSimulacao { get; set; }
    }

    public class ProdutoValidado
    {
        public int Id { get; set; }
        public required string Nome { get; set; }
        public required string Tipo { get; set; }
        public decimal Rentabilidade { get; set; }
        public required string Risco { get; set; }
    }

    public class ResultadoSimulacao
    {
        public decimal ValorFinal { get; set; }
        public decimal RentabilidadeEfetiva { get; set; }
        public int PrazoMeses { get; set; }
    }
}
