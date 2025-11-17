using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfisdeInvestimento.Domain.Entities
{
    public class HistoricoInvestimento
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public string Tipo { get; set; }
        public decimal Valor { get; set; }
        public decimal Rentabilidade { get; set; }
        public DateTime Data { get; set; }
    }
}
