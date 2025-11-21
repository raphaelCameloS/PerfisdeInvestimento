namespace PerfisdeInvestimento.Application.DTOs
{
    public class ProdutoRecomendadoResponse
    {
        public int Id { get; set; }
        public required string Nome { get; set; }
        public required string Tipo { get; set; }
        public decimal Rentabilidade { get; set; }
        public required string Risco { get; set; }
    }
}

