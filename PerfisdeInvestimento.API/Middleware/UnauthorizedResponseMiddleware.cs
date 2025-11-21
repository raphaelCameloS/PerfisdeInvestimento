using PerfisdeInvestimento.Application.DTOs;

namespace PerfisdeInvestimento.API.Middleware;

public class UnauthorizedResponseMiddleware
{
    private readonly RequestDelegate _next;

    public UnauthorizedResponseMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        
        var originalBodyStream = context.Response.Body;

        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        await _next(context);

       
        context.Response.Body = originalBodyStream;

        if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
        {
            context.Response.ContentType = "application/json";

            var errorResponse = ErroResponse.Create(
                "Não autorizado",
                "Autenticação necessária para acessar este recurso",
                GetTechnicalMessage(context),
                StatusCodes.Status401Unauthorized,
                context.Request.Path
            );

            await context.Response.WriteAsJsonAsync(errorResponse);
        }
        else
        {
           
            responseBody.Seek(0, SeekOrigin.Begin);
            await responseBody.CopyToAsync(originalBodyStream);
        }
    }

    private string GetTechnicalMessage(HttpContext context)
    {
        var authHeader = context.Request.Headers.Authorization.FirstOrDefault();

        if (string.IsNullOrEmpty(authHeader))
            return "Header Authorization não fornecido";

        if (!authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            return "Formato do token inválido. Use: Bearer {token}";

        return "Token JWT inválido ou expirado";
    }
}