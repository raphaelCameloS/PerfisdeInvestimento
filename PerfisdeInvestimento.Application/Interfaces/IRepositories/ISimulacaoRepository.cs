using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PerfisdeInvestimento.Domain.Entities;

namespace PerfisdeInvestimento.Application.Interfaces.IRepositories;

public interface ISimulacaoRepository
{
    Task<SimulacaoInvestimento> AddAsync(SimulacaoInvestimento simulacao);
    Task<List<SimulacaoInvestimento>> GetByClienteIdAsync(int clienteId);
    Task<List<SimulacaoInvestimento>> GetAllAsync();
    Task<List<object>> GetSimulacoesPorProdutoDiaAsync();
    Task<List<SimulacaoPorProdutoDia>> GetSimulacoesAgrupadasPorProdutoDiaAsync();

}
public class SimulacaoPorProdutoDia
{
    public required string Produto { get; set; }
    public DateTime Data { get; set; }
    public int QuantidadeSimulacoes { get; set; }
    public decimal MediaValorFinal { get; set; }
}
