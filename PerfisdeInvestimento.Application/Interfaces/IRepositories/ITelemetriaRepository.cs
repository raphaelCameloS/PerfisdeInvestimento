using PerfisdeInvestimento.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface ITelemetriaRepository
{
    Task<List<Telemetria>> GetPorPeriodoAsync(DateTime inicio, DateTime fim);
    Task AddAsync(Telemetria telemetria);
}