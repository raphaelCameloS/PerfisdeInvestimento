// Crie um middleware para registrar todas as chamadas
using Google.Protobuf.WellKnownTypes;
using PerfisdeInvestimento.Domain.Entities;

public class TelemetriaMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public TelemetriaMiddleware(RequestDelegate next, IServiceScopeFactory serviceScopeFactory)
    {
        _next = next;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        await _next(context);

        stopwatch.Stop();

        // Registra a telemetria em background
        _ = Task.Run(async () =>
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<ITelemetriaRepository>();

            await repository.AddAsync(new Telemetria
            {
                Endpoint = context.Request.Path,
                RequestTime = DateTime.UtcNow,
                ResponseTimeMs = stopwatch.ElapsedMilliseconds,
                StatusCode = context.Response.StatusCode,
                UserId = context.User.Identity?.Name
            });
        });
    }
}


