using Microsoft.EntityFrameworkCore;
using PerfisdeInvestimento.Application.Interfaces.IRepositories;
using PerfisdeInvestimento.Domain.Entities;
using PerfisdeInvestimento.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfisdeInvestimento.Infrastructure.Repositories;

public class SimulacaoRepository : ISimulacaoRepository
{
    private readonly ApplicationDbContext _context;

    public SimulacaoRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SimulacaoInvestimento> AddAsync(SimulacaoInvestimento simulacao)
    {
        _context.Simulacoes.Add(simulacao);
        await _context.SaveChangesAsync();
        return simulacao;
    }

    public async Task<List<SimulacaoInvestimento>> GetByClienteIdAsync(int clienteId)
    {
        return await _context.Simulacoes
            .Where(s => s.ClienteId == clienteId)
            .OrderByDescending(s => s.DataSimulacao)
            .ToListAsync();
    }

    public async Task<List<SimulacaoInvestimento>> GetAllAsync()
    {
        return await _context.Simulacoes
            .OrderByDescending(s => s.DataSimulacao)
            .ToListAsync();
    }

    public async Task<List<object>> GetSimulacoesPorProdutoDiaAsync()
    {
        return await _context.Simulacoes
            .GroupBy(s => new { s.Produto, s.DataSimulacao.Date })
            .Select(g => new
            {
                Produto = g.Key.Produto,
                Data = g.Key.Date,
                QuantidadeSimulacoes = g.Count(),
                MediaValorFinal = g.Average(s => s.ValorFinal)
            })
            .ToListAsync<object>();
    }
    public async Task<List<SimulacaoPorProdutoDia>> GetSimulacoesAgrupadasPorProdutoDiaAsync()
    {
        var resultado = await _context.Simulacoes
            .GroupBy(s => new
            {
                Produto = s.Produto,
                Data = s.DataSimulacao.Date
            })
            .Select(g => new
            {
                Produto = g.Key.Produto,
                Data = g.Key.Data,
                QuantidadeSimulacoes = g.Count(),
                MediaValorFinal = g.Average(s => (double)s.ValorFinal) // ← Converta para double
            })
            .ToListAsync();

        return resultado.Select(r => new SimulacaoPorProdutoDia
        {
            Produto = r.Produto,
            Data = r.Data,
            QuantidadeSimulacoes = r.QuantidadeSimulacoes,
            MediaValorFinal = (decimal)r.MediaValorFinal // ← Converta de volta para decimal
        }).ToList();
    }

}