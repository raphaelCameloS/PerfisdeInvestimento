using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfisdeInvestimento.Application.DTOs
{
    public class HistoricoInvestimentosResponse
    {
        public int Id { get; set; }
        public string Tipo { get; set; }
        public decimal Valor { get; set; }
        public decimal Rentabilidade { get; set; }
        public string Data { get; set; }
    }
}
