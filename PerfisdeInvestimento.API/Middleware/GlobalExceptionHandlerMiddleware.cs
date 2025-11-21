using PerfisdeInvestimento.Application.DTOs;
using PerfisdeInvestimento.Domain.Exceptions;
using System.Net;

namespace PerfisdeInvestimento.API.Middleware
{
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

        public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            var errorResponse = exception switch
            {
                NotFoundException notFound => ErroResponse.Create(
                    "Recurso não encontrado",
                    "Não encontramos o que você está procurando.",           
                    notFound.Message,                                        
                    (int)HttpStatusCode.NotFound,
                    context.Request.Path),

                BusinessException business => ErroResponse.Create(
                    "Regra de negócio violada",
                    "Não foi possível processar sua solicitação.",          
                    business.Message,                                       
                    (int)HttpStatusCode.BadRequest,
                    context.Request.Path),

                ValidationException validation => ErroResponse.Create(
                    "Dados inválidos",
                    "Verifique os dados informados e tente novamente.",       
                    validation.Message,                                     
                    (int)HttpStatusCode.BadRequest,
                    context.Request.Path),

                ArgumentException argument => ErroResponse.Create(
                    "Parâmetro inválido",
                    "Alguma informação está incorreta. Verifique e tente novamente.",
                    argument.Message,                                       
                    (int)HttpStatusCode.BadRequest,
                    context.Request.Path),

                _ => ErroResponse.Create(
                    "Erro interno do servidor",
                    "Estamos com problemas técnicos. Tente novamente em alguns instantes.", 
                    exception.Message,                                      
                    (int)HttpStatusCode.InternalServerError,
                    context.Request.Path)
            };

            _logger.LogError(exception, "Erro: {MensagemTecnica}", errorResponse.MensagemTecnica);

            response.StatusCode = errorResponse.Status;
            await response.WriteAsJsonAsync(errorResponse);
        }
    }
}
