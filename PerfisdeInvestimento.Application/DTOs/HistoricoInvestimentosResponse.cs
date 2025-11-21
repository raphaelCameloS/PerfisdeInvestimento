namespace PerfisdeInvestimento.Application.DTOs
{
    public class HistoricoInvestimentosResponse
    {
        public int Id { get; set; }
        public required string Tipo { get; set; }
        public decimal Valor { get; set; }
        public decimal Rentabilidade { get; set; }
        public required string Data { get; set; }
    }
}
