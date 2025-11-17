using PerfisdeInvestimento.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfisdeInvestimento.Application.Interfaces.IServices
{
    public interface ISimulacaoService
    {
        Task<SimulacaoResponse> SimularInvestimento(SimulacaoRequest request);
        Task<List<HistoricoSimulacaoResponse>> GetHistoricoSimulacoes();
        Task<List<SimulacaoPorProdutoResponse>> GetSimulacoesPorProdutoDia();
    }
}
