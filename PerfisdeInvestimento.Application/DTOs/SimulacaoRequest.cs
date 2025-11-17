using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfisdeInvestimento.Application.DTOs
{
    public class SimulacaoRequest
    {
        public int ClienteId { get; set; }
        public decimal Valor { get; set; }
        public int PrazoMeses { get; set; }
        public string TipoProduto { get; set; }
    }
}
