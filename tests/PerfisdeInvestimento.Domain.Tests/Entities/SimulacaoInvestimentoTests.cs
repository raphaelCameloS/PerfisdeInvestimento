using PerfisdeInvestimento.Domain.Entities;
using Xunit;

namespace PerfisdeInvestimento.Domain.Tests.Entities;

public class SimulacaoInvestimentoTests
{
    [Fact]
    public void SimulacaoInvestimento_Constructor_ShouldSetDataSimulacaoToUtcNow()
    {
      
        var antesCriacao = DateTime.UtcNow;

        
        var simulacao = new SimulacaoInvestimento();

        var depoisCriacao = DateTime.UtcNow;

        
        Assert.InRange(simulacao.DataSimulacao, antesCriacao, depoisCriacao);
        Assert.Equal(DateTimeKind.Utc, simulacao.DataSimulacao.Kind);
    }

    [Fact]
    public void SimulacaoInvestimento_WithValidData_ShouldCreateSuccessfully()
    {
       
        var dataSimulacao = new DateTime(2024, 1, 15, 14, 30, 0, DateTimeKind.Utc);

        
        var simulacao = new SimulacaoInvestimento
        {
            Id = 1,
            ClienteId = 123,
            Produto = "CDB Banco XYZ",
            ValorInvestido = 5000.50m,
            ValorFinal = 5600.75m,
            PrazoMeses = 12,
            DataSimulacao = dataSimulacao
        };

        
        Assert.Equal(1, simulacao.Id);
        Assert.Equal(123, simulacao.ClienteId);
        Assert.Equal("CDB Banco XYZ", simulacao.Produto);
        Assert.Equal(5000.50m, simulacao.ValorInvestido);
        Assert.Equal(5600.75m, simulacao.ValorFinal);
        Assert.Equal(12, simulacao.PrazoMeses);
        Assert.Equal(dataSimulacao, simulacao.DataSimulacao);
    }

    [Fact]
    public void SimulacaoInvestimento_WithDefaultValues_ShouldInitializeCorrectly()
    {
        
        var simulacao = new SimulacaoInvestimento();

        
        Assert.Equal(0, simulacao.Id);
        Assert.Equal(0, simulacao.ClienteId);
        Assert.Null(simulacao.Produto);
        Assert.Equal(0, simulacao.ValorInvestido);
        Assert.Equal(0, simulacao.ValorFinal);
        Assert.Equal(0, simulacao.PrazoMeses);
        Assert.NotEqual(default(DateTime), simulacao.DataSimulacao); 
    }

    [Theory]
    [InlineData(1000, 1120, 120)]     // Lucro de 12% em 1 ano
    [InlineData(5000, 5000, 0)]       // Sem lucro (valor final igual)
    [InlineData(1000, 900, -100)]     // Prejuízo (valor final menor)
    [InlineData(100, 150, 50)]        // Investimento pequeno com bom retorno
    [InlineData(100000, 112000, 12000)] // Investimento grande
    public void SimulacaoInvestimento_WithDifferentValores_ShouldStoreCorrectly(
        decimal valorInvestido, decimal valorFinal, decimal diferencaEsperada)
    {
       
        var simulacao = new SimulacaoInvestimento
        {
            ValorInvestido = valorInvestido,
            ValorFinal = valorFinal,
            PrazoMeses = 12
        };

        var diferencaCalculada = valorFinal - valorInvestido;

        
        Assert.Equal(valorInvestido, simulacao.ValorInvestido);
        Assert.Equal(valorFinal, simulacao.ValorFinal);
        Assert.Equal(diferencaEsperada, diferencaCalculada);
    }

    [Theory]
    [InlineData(1)]    // 1 mês
    [InlineData(6)]    // 6 meses
    [InlineData(12)]   // 1 ano
    [InlineData(24)]   // 2 anos
    [InlineData(60)]   // 5 anos
    [InlineData(120)]  // 10 anos
    [InlineData(0)]    // Prazo zero
    [InlineData(-12)]  // Prazo negativo
    public void SimulacaoInvestimento_WithDifferentPrazos_ShouldStoreCorrectly(int prazoMeses)
    {
       
        var simulacao = new SimulacaoInvestimento
        {
            PrazoMeses = prazoMeses
        };

        Assert.Equal(prazoMeses, simulacao.PrazoMeses);
    }

    [Theory]
    [InlineData("CDB")]
    [InlineData("LCI")]
    [InlineData("LCA")]
    [InlineData("Tesouro Direto")]
    [InlineData("Fundo Imobiliário")]
    [InlineData("Ações")]
    [InlineData("Fundo de Ações")]
    [InlineData("Previdência Privada")]
    public void SimulacaoInvestimento_WithDifferentProdutos_ShouldStoreCorrectly(string produto)
    {
        
        var simulacao = new SimulacaoInvestimento
        {
            Produto = produto
        };

       
        Assert.Equal(produto, simulacao.Produto);
    }

    [Fact]
    public void SimulacaoInvestimento_DataSimulacao_CanBeOverridden()
    {
        
        var dataEspecifica = new DateTime(2023, 12, 1, 10, 0, 0, DateTimeKind.Utc);

        
        var simulacao = new SimulacaoInvestimento
        {
            DataSimulacao = dataEspecifica
        };

       
        Assert.Equal(dataEspecifica, simulacao.DataSimulacao);
        Assert.NotEqual(DateTime.UtcNow, simulacao.DataSimulacao); // Diferente do valor default
    }

    [Fact]
    public void SimulacaoInvestimento_WithNegativeValorInvestido_ShouldStoreButIsInvalid()
    {
        
        var simulacao = new SimulacaoInvestimento
        {
            ValorInvestido = -1000m
        };

        
        Assert.Equal(-1000m, simulacao.ValorInvestido);
        Assert.True(simulacao.ValorInvestido < 0);
    }

    [Fact]
    public void SimulacaoInvestimento_WithNegativeValorFinal_ShouldStoreButIsInvalid()
    {
        
        var simulacao = new SimulacaoInvestimento
        {
            ValorFinal = -500m
        };

        
        Assert.Equal(-500m, simulacao.ValorFinal);
        Assert.True(simulacao.ValorFinal < 0);
    }

    [Fact]
    public void SimulacaoInvestimento_CanCalculateRentabilidade()
    {
        
        var simulacao = new SimulacaoInvestimento
        {
            ValorInvestido = 1000m,
            ValorFinal = 1120m,
            PrazoMeses = 12
        };

       
        var rentabilidade = (simulacao.ValorFinal - simulacao.ValorInvestido) / simulacao.ValorInvestido;

        
        Assert.Equal(0.12m, rentabilidade); // 12% de rentabilidade
    }

    [Fact]
    public void SimulacaoInvestimento_WithZeroValorInvestido_ShouldStoreButIsInvalid()
    {
       
        var simulacao = new SimulacaoInvestimento
        {
            ValorInvestido = 0m,
            ValorFinal = 0m
        };

       
        Assert.Equal(0m, simulacao.ValorInvestido);
        Assert.Equal(0m, simulacao.ValorFinal);
    }

    [Fact]
    public void SimulacaoInvestimento_CanRepresentShortTermInvestment()
    {
        
        var simulacao = new SimulacaoInvestimento
        {
            Produto = "CDB Diário",
            ValorInvestido = 5000m,
            ValorFinal = 5020m,
            PrazoMeses = 1 // 1 mês
        };

       
        Assert.Equal("CDB Diário", simulacao.Produto);
        Assert.True(simulacao.PrazoMeses <= 3); // Curto prazo
        Assert.True(simulacao.ValorFinal > simulacao.ValorInvestido); // Teve rendimento
    }

    [Fact]
    public void SimulacaoInvestimento_CanRepresentLongTermInvestment()
    {
       
        var simulacao = new SimulacaoInvestimento
        {
            Produto = "Tesouro IPCA+ 2035",
            ValorInvestido = 10000m,
            ValorFinal = 21500m,
            PrazoMeses = 120 // 10 anos
        };

       
        Assert.Equal("Tesouro IPCA+ 2035", simulacao.Produto);
        Assert.True(simulacao.PrazoMeses >= 60); // Longo prazo (5+ anos)
        Assert.True(simulacao.ValorFinal > simulacao.ValorInvestido * 2); // Mais que dobrou
    }
}
