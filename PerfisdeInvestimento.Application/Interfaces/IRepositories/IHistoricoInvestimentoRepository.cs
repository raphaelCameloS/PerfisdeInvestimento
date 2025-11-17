using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PerfisdeInvestimento.Domain.Entities;

namespace PerfisdeInvestimento.Infrastructure.Repositories;

public interface IHistoricoInvestimentoRepository
{
    Task<List<HistoricoInvestimento>> GetByClienteIdAsync(int clienteId);
    Task<HistoricoInvestimento> AddAsync(HistoricoInvestimento historico);
}
