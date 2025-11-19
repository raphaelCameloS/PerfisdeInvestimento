using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfisdeInvestimento.Domain.Entities
{
    public class ProdutoInvestimento
    {
        public int Id { get; set; }
        public  string? Nome { get; set; }
        public  string? Tipo { get; set; } // "CDB", "Fundo", "Ação", etc.
        public decimal Rentabilidade { get; set; }
        public  string? Risco { get; set; } // "Baixo", "Médio", "Alto"
        public decimal ValorMinimo { get; set; }
        public int PrazoMinimoMeses { get; set; }
        public  string? PerfilRecomendado { get; set; } // "Conservador", "Moderado", "Agressivo"
    }
}
