using Microsoft.EntityFrameworkCore;
using PerfisdeInvestimento.Application.Interfaces.IRepositories;
using PerfisdeInvestimento.Domain.Entities;
using PerfisdeInvestimento.Infrastructure.Data;

namespace PerfisdeInvestimento.Infrastructure.Repositories;

public class TelemetriaRepository : ITelemetriaRepository
{
    private readonly ApplicationDbContext _context;

    public TelemetriaRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Telemetria>> GetPorPeriodoAsync(DateTime inicio, DateTime fim)
    {
        return await _context.Telemetrias
            .Where(t => t.RequestTime >= inicio && t.RequestTime <= fim)
            .ToListAsync();
    }

    public async Task AddAsync(Telemetria telemetria)
    {
        await _context.Telemetrias.AddAsync(telemetria);
        await _context.SaveChangesAsync();
    }
}