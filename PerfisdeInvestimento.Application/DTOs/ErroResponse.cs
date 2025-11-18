using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfisdeInvestimento.Application.DTOs
{
    public class ErroResponse
    {
        public string Tipo { get; set; }
        public string Titulo { get; set; }
        public int Status { get; set; }
        public string Mensagem { get; set; }  
        public string MensagemTecnica { get; set; }   
        public string Instancia { get; set; }
        public DateTime DataHora { get; set; }
        public string TraceId { get; set; }

        public ErroResponse()
        {
            DataHora = DateTime.UtcNow;
        }

        public static ErroResponse Create(string titulo, string mensagem, string mensagemTecnica, int status, string instancia = null)
        {
            return new ErroResponse
            {
                Tipo = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Titulo = titulo,
                Status = status,
                Mensagem = mensagem,
                MensagemTecnica = mensagemTecnica,
                Instancia = instancia
            };
        }
    }
}
