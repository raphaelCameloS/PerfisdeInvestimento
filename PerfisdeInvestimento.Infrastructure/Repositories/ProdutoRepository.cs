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

public class ProdutoRepository : IProdutoRepository
{
    private readonly ApplicationDbContext _context;

    public ProdutoRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProdutoInvestimento>> GetProdutosPorPerfilAsync(string perfil)
    {
        return await _context.Produtos
            .Where(p => p.PerfilRecomendado == perfil)
            .ToListAsync();
    }

    public async Task<ProdutoInvestimento?> GetByIdAsync(int id)
    {
        return await _context.Produtos.FindAsync(id);
    }

    public async Task<List<ProdutoInvestimento>> GetAllAsync()
    {
        return await _context.Produtos.ToListAsync();
    }
}