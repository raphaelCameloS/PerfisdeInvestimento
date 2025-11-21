using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PerfisdeInvestimento.Domain.Entities;

namespace PerfisdeInvestimento.Application.Interfaces.IRepositories;

public interface IProdutoRepository
{
    Task<List<ProdutoInvestimento>> GetProdutosPorPerfilAsync(string perfil);
    Task<ProdutoInvestimento?> GetByIdAsync(int id);
    Task<List<ProdutoInvestimento>> GetAllAsync();
}
