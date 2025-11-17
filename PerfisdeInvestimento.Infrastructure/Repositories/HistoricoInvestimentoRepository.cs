using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PerfisdeInvestimento.Domain.Entities;
using PerfisdeInvestimento.Infrastructure.Data;

namespace PerfisdeInvestimento.Infrastructure.Repositories;

public class HistoricoInvestimentoRepository : IHistoricoInvestimentoRepository
{
    private readonly ApplicationDbContext _context;

    public HistoricoInvestimentoRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<HistoricoInvestimento>> GetByClienteIdAsync(int clienteId)
    {
        return await _context.HistoricosInvestimentos
            .Where(h => h.ClienteId == clienteId)
            .OrderByDescending(h => h.Data)
            .ToListAsync();
    }

    public async Task<HistoricoInvestimento> AddAsync(HistoricoInvestimento historico)
    {
        _context.HistoricosInvestimentos.Add(historico);
        await _context.SaveChangesAsync();
        return historico;
    }
}
