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
        _ = Task.Run(async () =>
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var repository = scope.ServiceProvider.GetRequiredService<ITelemetriaRepository>();

                var userId = context.User?.Identity?.IsAuthenticated == true
                    ? context.User.Identity.Name
                    : "Anonymous";

                await repository.AddAsync(new Telemetria
                {
                    Endpoint = context.Request.Path,
                    RequestTime = DateTime.UtcNow,
                    ResponseTimeMs = stopwatch.ElapsedMilliseconds,
                    StatusCode = context.Response.StatusCode,
                    UserId = userId 
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao salvar telemetria: {ex.Message}");
            }
        });
    }
}