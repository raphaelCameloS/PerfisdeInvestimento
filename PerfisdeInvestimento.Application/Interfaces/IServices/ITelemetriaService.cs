using PerfisdeInvestimento.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface ITelemetriaService
{
    Task<TelemetriaResponse> GetTelemetriaPorPeriodoAsync(DateTime inicio, DateTime fim);
}
