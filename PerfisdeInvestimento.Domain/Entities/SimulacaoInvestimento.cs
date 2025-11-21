using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfisdeInvestimento.Domain.Entities
{
    public class SimulacaoInvestimento
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public required string Produto { get; set; }
        public decimal ValorInvestido { get; set; }
        public decimal ValorFinal { get; set; }
        public int PrazoMeses { get; set; }
        public DateTime DataSimulacao { get; set; }

        public SimulacaoInvestimento()
        {
            DataSimulacao = DateTime.UtcNow;
        }
    }
}
