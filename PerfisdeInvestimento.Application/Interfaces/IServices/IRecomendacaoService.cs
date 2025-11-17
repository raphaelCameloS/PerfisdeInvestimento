using PerfisdeInvestimento.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfisdeInvestimento.Application.Interfaces.IServices
{
    public interface IRecomendacaoService
    {
        Task<PerfilRiscoResponse> CalcularPerfilRisco(int clienteId);
        Task<List<ProdutoRecomendadoResponse>> GetProdutosRecomendados(string perfil);
    }
}
