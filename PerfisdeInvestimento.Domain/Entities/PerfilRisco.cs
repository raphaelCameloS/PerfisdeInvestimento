using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfisdeInvestimento.Domain.Entities
{
    public class PerfilRisco
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public string TipoPerfil { get; set; } // "Conservador", "Moderado", "Agressivo"
        public int Pontuacao { get; set; }
        public string Descricao { get; set; }
        public DateTime DataCalculo { get; set; }

        public PerfilRisco()
        {
            DataCalculo = DateTime.UtcNow;
        }
    }
}
