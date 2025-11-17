using PerfisdeInvestimento.Application.DTOs;
using PerfisdeInvestimento.Application.Interfaces.IServices;
using PerfisdeInvestimento.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfisdeInvestimento.Application.Services
{
    public class HistoricoInvestimentoService : IHistoricoInvestimentoService
    {
        private readonly IHistoricoInvestimentoRepository _historicoRepository;

        public HistoricoInvestimentoService(IHistoricoInvestimentoRepository historicoRepository)
        {
            _historicoRepository = historicoRepository;
        }

        public async Task<List<HistoricoInvestimentosResponse>> GetHistoricoInvestimentos(int clienteId)
        {
            var historicos = await _historicoRepository.GetByClienteIdAsync(clienteId);

            return historicos.Select(h => new HistoricoInvestimentosResponse
            {
                Id = h.Id,
                Tipo = h.Tipo,
                Valor = h.Valor,
                Rentabilidade = h.Rentabilidade,
                Data = h.Data.ToString("yyyy-MM-dd")
            }).ToList();
        }

    }
}
