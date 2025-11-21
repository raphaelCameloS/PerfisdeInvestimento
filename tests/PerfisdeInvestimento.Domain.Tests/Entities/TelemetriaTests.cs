using PerfisdeInvestimento.Domain.Entities;
using Xunit;

namespace PerfisdeInvestimento.Domain.Tests.Entities;

public class TelemetriaTests
{
    [Fact]
    public void Telemetria_WithValidData_ShouldCreateSuccessfully()
    {
        
        var requestTime = new DateTime(2024, 1, 15, 14, 30, 0, DateTimeKind.Utc);

        var telemetria = new Telemetria
        {
            Id = 1,
            Endpoint = "/api/simulacoes/simular-investimento",
            RequestTime = requestTime,
            ResponseTimeMs = 150,
            StatusCode = 200,
            UserId = "user-12345"
        };

        
        Assert.Equal(1, telemetria.Id);
        Assert.Equal("/api/simulacoes/simular-investimento", telemetria.Endpoint);
        Assert.Equal(requestTime, telemetria.RequestTime);
        Assert.Equal(150, telemetria.ResponseTimeMs);
        Assert.Equal(200, telemetria.StatusCode);
        Assert.Equal("user-12345", telemetria.UserId);
    }

    [Fact]
    public void Telemetria_WithDefaultValues_ShouldInitializeCorrectly()
    {
        
        var telemetria = new Telemetria();

        
        Assert.Equal(0, telemetria.Id);
        Assert.Equal("",telemetria.Endpoint);
        Assert.Equal(default(DateTime), telemetria.RequestTime);
        Assert.Equal(0, telemetria.ResponseTimeMs);
        Assert.Equal(0, telemetria.StatusCode);
        Assert.Null(telemetria.UserId);
    }

    [Theory]
    [InlineData("/api/simulacoes/simular-investimento")]
    [InlineData("/api/perfil-risco/123")]
    [InlineData("/api/investimentos/456")]
    [InlineData("/api/produtos-recomendados/moderado")]
    [InlineData("/health")]
    [InlineData("/metrics")]
    public void Telemetria_WithDifferentEndpoints_ShouldStoreCorrectly(string endpoint)
    {
        
        var telemetria = new Telemetria
        {
            Endpoint = endpoint
        };

        
        Assert.Equal(endpoint, telemetria.Endpoint);
    }

    [Theory]
    [InlineData(1)]        // Resposta muito rápida
    [InlineData(50)]       // Resposta rápida
    [InlineData(150)]      // Resposta normal
    [InlineData(500)]      // Resposta lenta
    [InlineData(2000)]     // Resposta muito lenta
    [InlineData(10000)]    // Timeout potencial
    [InlineData(0)]        // Resposta instantânea
    [InlineData(-100)]     // Tempo negativo (inválido)
    public void Telemetria_WithDifferentResponseTimes_ShouldStoreCorrectly(long responseTimeMs)
    {
       
        var telemetria = new Telemetria
        {
            ResponseTimeMs = responseTimeMs
        };

        
        Assert.Equal(responseTimeMs, telemetria.ResponseTimeMs);
    }

    [Theory]
    [InlineData(200)]      // OK
    [InlineData(201)]      // Created
    [InlineData(400)]      // Bad Request
    [InlineData(401)]      // Unauthorized
    [InlineData(404)]      // Not Found
    [InlineData(500)]      // Internal Server Error
    [InlineData(503)]      // Service Unavailable
    [InlineData(100)]      // Continue
    [InlineData(301)]      // Moved Permanently
    public void Telemetria_WithDifferentStatusCodes_ShouldStoreCorrectly(int statusCode)
    {
       
        var telemetria = new Telemetria
        {
            StatusCode = statusCode
        };

        
        Assert.Equal(statusCode, telemetria.StatusCode);
    }

    [Theory]
    [InlineData("user-12345")]
    [InlineData("admin-001")]
    [InlineData("anonymous")]
    [InlineData("system")]
    [InlineData("batch-processor")]
    [InlineData("")]      
    public void Telemetria_WithDifferentUserIds_ShouldStoreCorrectly(string userId)
    {
       
        var telemetria = new Telemetria
        {
            UserId = userId
        };

        
        Assert.Equal(userId, telemetria.UserId);
    }

    [Fact]
    public void Telemetria_WithNullUserId_ShouldStoreNull()
    {
        
        var telemetria = new Telemetria
        {
            UserId = null!
        };

        
        Assert.Null(telemetria.UserId);
    }

    [Fact]
    public void Telemetria_WithFutureRequestTime_ShouldStoreCorrectly()
    {
       
        var futureTime = DateTime.UtcNow.AddDays(1);

        
        var telemetria = new Telemetria
        {
            RequestTime = futureTime
        };

        
        Assert.Equal(futureTime, telemetria.RequestTime);
        Assert.True(telemetria.RequestTime > DateTime.UtcNow);
    }

    [Fact]
    public void Telemetria_WithPastRequestTime_ShouldStoreCorrectly()
    {
      
        var pastTime = DateTime.UtcNow.AddHours(-2);

       
        var telemetria = new Telemetria
        {
            RequestTime = pastTime
        };

       
        Assert.Equal(pastTime, telemetria.RequestTime);
        Assert.True(telemetria.RequestTime < DateTime.UtcNow);
    }

    [Fact]
    public void Telemetria_CanRepresentSuccessfulRequest()
    {
        
        var telemetria = new Telemetria
        {
            Endpoint = "/api/simulacoes/simular-investimento",
            ResponseTimeMs = 120,
            StatusCode = 200,
            UserId = "user-12345"
        };

       
        Assert.Equal(200, telemetria.StatusCode);
        Assert.True(telemetria.ResponseTimeMs < 200); // Resposta rápida
        Assert.NotNull(telemetria.UserId);
    }

    [Fact]
    public void Telemetria_CanRepresentFailedRequest()
    {
        
        var telemetria = new Telemetria
        {
            Endpoint = "/api/simulacoes/simular-investimento",
            ResponseTimeMs = 45,
            StatusCode = 500,
            UserId = "user-12345"
        };

        
        Assert.Equal(500, telemetria.StatusCode);
        Assert.True(telemetria.StatusCode >= 400); // Status de erro
    }

    [Fact]
    public void Telemetria_CanRepresentClientError()
    {
        
        var telemetria = new Telemetria
        {
            Endpoint = "/api/simulacoes/simular-investimento",
            ResponseTimeMs = 25,
            StatusCode = 400,
            UserId = "user-12345"
        };

        
        Assert.Equal(400, telemetria.StatusCode);
        Assert.True(telemetria.StatusCode >= 400 && telemetria.StatusCode < 500); // Client error
    }

    [Fact]
    public void Telemetria_CanRepresentSlowRequest()
    {
       
        var telemetria = new Telemetria
        {
            Endpoint = "/api/simulacoes/por-produto-dia",
            ResponseTimeMs = 2500,
            StatusCode = 200,
            UserId = "user-12345"
        };

     
        Assert.True(telemetria.ResponseTimeMs > 1000); // Mais de 1 segundo
        Assert.Equal(200, telemetria.StatusCode); // Mas ainda bem-sucedido
    }

    [Fact]
    public void Telemetria_WithInvalidStatusCode_ShouldStoreButIsInvalid()
    {
       
        var telemetria = new Telemetria
        {
            StatusCode = 999 // Status code HTTP inválido
        };

        
        Assert.Equal(999, telemetria.StatusCode);
        Assert.True(telemetria.StatusCode > 599 || telemetria.StatusCode < 100); // Fora do range HTTP padrão
    }

    [Fact]
    public void Telemetria_CanRepresentUnauthenticatedRequest()
    {
      
        var telemetria = new Telemetria
        {
            Endpoint = "/api/produtos-recomendados/moderado",
            ResponseTimeMs = 80,
            StatusCode = 200,
            UserId = null! // Usuário não autenticado
        };

        Assert.Null(telemetria.UserId);
        Assert.Equal(200, telemetria.StatusCode); // Request bem-sucedido mesmo sem usuário
    }
}