namespace PerfisdeInvestimento.Application.DTOs
{
    public class PerfilRiscoResponse
    {
        public int ClienteId { get; set; }
        public required string Perfil { get; set; }
        public int Pontuacao { get; set; }
        public required string Descricao { get; set; }
    }
}
