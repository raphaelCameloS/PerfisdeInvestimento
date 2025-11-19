using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfisdeInvestimento.Domain.Entities
{
    public class Telemetria
    {
        public int Id { get; set; }
        public string Endpoint { get; set; } = string.Empty;
        public DateTime RequestTime { get; set; }
        public long ResponseTimeMs { get; set; }
        public int StatusCode { get; set; }
        public string? UserId { get; set; }
    }
}
