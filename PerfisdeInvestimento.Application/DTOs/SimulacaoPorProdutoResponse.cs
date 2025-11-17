using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfisdeInvestimento.Application.DTOs
{
    public class SimulacaoPorProdutoResponse
    {
        public string Produto { get; set; }
        public DateTime Data { get; set; }
        public int QuantidadeSimulacoes { get; set; }
        public decimal MediaValorFinal { get; set; }
    }
}
