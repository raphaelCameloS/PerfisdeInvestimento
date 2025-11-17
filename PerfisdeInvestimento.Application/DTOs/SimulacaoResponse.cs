using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfisdeInvestimento.Application.DTOs
{
    public class SimulacaoResponse
    {
        public ProdutoValidado ProdutoValidado { get; set; }
        public ResultadoSimulacao ResultadoSimulacao { get; set; }
        public DateTime DataSimulacao { get; set; }
    }

    public class ProdutoValidado
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Tipo { get; set; }
        public decimal Rentabilidade { get; set; }
        public string Risco { get; set; }
    }

    public class ResultadoSimulacao
    {
        public decimal ValorFinal { get; set; }
        public decimal RentabilidadeEfetiva { get; set; }
        public int PrazoMeses { get; set; }
    }
}
