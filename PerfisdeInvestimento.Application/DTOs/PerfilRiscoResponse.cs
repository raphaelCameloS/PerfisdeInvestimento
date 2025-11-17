using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfisdeInvestimento.Application.DTOs
{
    public class PerfilRiscoResponse
    {
        public int ClienteId { get; set; }
        public string Perfil { get; set; }
        public int Pontuacao { get; set; }
        public string Descricao { get; set; }
    }
}
