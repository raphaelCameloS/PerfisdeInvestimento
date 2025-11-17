using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PerfisdeInvestimento.Application.DTOs;


namespace PerfisdeInvestimento.Application.Interfaces.IServices
{
    public interface IHistoricoInvestimentoService
    {
        Task<List<HistoricoInvestimentosResponse>> GetHistoricoInvestimentos(int clienteId);
    }
}
