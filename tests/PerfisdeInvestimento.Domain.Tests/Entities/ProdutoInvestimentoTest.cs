using PerfisdeInvestimento.Domain.Entities;
using Xunit;

namespace PerfisdeInvestimento.Domain.Tests.Entities;

public class ProdutoInvestimentoTests
{
    [Fact]
    public void ProdutoInvestimento_WithValidData_ShouldCreateSuccessfully()
    {
        var produto = new ProdutoInvestimento
        {
            Id = 1,
            Nome = "CDB Banco XYZ",
            Tipo = "CDB",
            Rentabilidade = 0.12m,
            Risco = "Médio",
            ValorMinimo = 1000m,
            PrazoMinimoMeses = 6,
            PerfilRecomendado = "Moderado"
        };

        Assert.Equal(1, produto.Id);
        Assert.Equal("CDB Banco XYZ", produto.Nome);
        Assert.Equal("CDB", produto.Tipo);
        Assert.Equal(0.12m, produto.Rentabilidade);
        Assert.Equal("Médio", produto.Risco);
        Assert.Equal(1000m, produto.ValorMinimo);
        Assert.Equal(6, produto.PrazoMinimoMeses);
        Assert.Equal("Moderado", produto.PerfilRecomendado);
    }

    [Fact]
    public void ProdutoInvestimento_WithDefaultValues_ShouldInitializeCorrectly()
    {
        
        var produto = new ProdutoInvestimento();

        
        Assert.Equal(0, produto.Id);
        Assert.Null(produto.Nome);
        Assert.Null(produto.Tipo);
        Assert.Equal(0, produto.Rentabilidade);
        Assert.Null(produto.Risco);
        Assert.Equal(0, produto.ValorMinimo);
        Assert.Equal(0, produto.PrazoMinimoMeses);
        Assert.Null(produto.PerfilRecomendado);
    }

    [Theory]
    [InlineData("CDB")]
    [InlineData("LCI")]
    [InlineData("LCA")]
    [InlineData("Tesouro Direto")]
    [InlineData("Fundo Imobiliário")]
    [InlineData("Ações")]
    [InlineData("Fundo de Investimento")]
    public void ProdutoInvestimento_WithDifferentTipos_ShouldStoreCorrectly(string tipo)
    {
        
        var produto = new ProdutoInvestimento
        {
            Tipo = tipo
        };

     
        Assert.Equal(tipo, produto.Tipo);
    }

    [Theory]
    [InlineData("Baixo")]
    [InlineData("Médio")]
    [InlineData("Alto")]
    [InlineData("Muito Alto")]
    [InlineData("Baixíssimo")]
    public void ProdutoInvestimento_WithDifferentRiscos_ShouldStoreCorrectly(string risco)
    {
        
        var produto = new ProdutoInvestimento
        {
            Risco = risco
        };

       
        Assert.Equal(risco, produto.Risco);
    }

    [Theory]
    [InlineData(0.01)]   // 1%
    [InlineData(0.05)]   // 5%
    [InlineData(0.12)]   // 12%
    [InlineData(0.25)]   // 25%
    [InlineData(0.00)]   // 0%
    [InlineData(1.50)]   // 150% (produtos muito arrojados)
    public void ProdutoInvestimento_WithDifferentRentabilidades_ShouldStoreCorrectly(decimal rentabilidade)
    {
       
        var produto = new ProdutoInvestimento
        {
            Rentabilidade = rentabilidade
        };

      
        Assert.Equal(rentabilidade, produto.Rentabilidade);
    }

    [Theory]
    [InlineData(0.01)]      // Valor mínimo baixo
    [InlineData(1000)]      // Valor comum
    [InlineData(100000)]    // Valor alto
    [InlineData(0)]         // Valor zero (produto sem mínimo?)
    [InlineData(0.001)]     // Valor fracionado
    public void ProdutoInvestimento_WithDifferentValoresMinimos_ShouldStoreCorrectly(decimal valorMinimo)
    {
        // Arrange & Act
        var produto = new ProdutoInvestimento
        {
            ValorMinimo = valorMinimo
        };

        // Assert
        Assert.Equal(valorMinimo, produto.ValorMinimo);
    }

    [Theory]
    [InlineData(1)]         // 1 mês
    [InlineData(6)]         // 6 meses
    [InlineData(12)]        // 1 ano
    [InlineData(36)]        // 3 anos
    [InlineData(60)]        // 5 anos
    [InlineData(0)]         // Sem prazo mínimo
    [InlineData(120)]       // 10 anos
    public void ProdutoInvestimento_WithDifferentPrazosMinimos_ShouldStoreCorrectly(int prazoMeses)
    {
        // Arrange & Act
        var produto = new ProdutoInvestimento
        {
            PrazoMinimoMeses = prazoMeses
        };

        // Assert
        Assert.Equal(prazoMeses, produto.PrazoMinimoMeses);
    }

    [Theory]
    [InlineData("Conservador")]
    [InlineData("Moderado")]
    [InlineData("Agressivo")]
    //[InlineData("Ultra Conservador")]
    //[InlineData("Arrojado")]
    //[InlineData("Balanceado")]
    public void ProdutoInvestimento_WithDifferentPerfisRecomendados_ShouldStoreCorrectly(string perfil)
    {
     
        var produto = new ProdutoInvestimento
        {
            PerfilRecomendado = perfil
        };

       
        Assert.Equal(perfil, produto.PerfilRecomendado);
    }

    [Fact]
    public void ProdutoInvestimento_WithNegativeValorMinimo_ShouldStoreButIsInvalid()
    {
        
        var produto = new ProdutoInvestimento
        {
            ValorMinimo = -1000m
        };

        
        Assert.Equal(-1000m, produto.ValorMinimo);
        Assert.True(produto.ValorMinimo < 0);
    }

    [Fact]
    public void ProdutoInvestimento_WithNegativePrazoMinimo_ShouldStoreButIsInvalid()
    {
        
        var produto = new ProdutoInvestimento
        {
            PrazoMinimoMeses = -6
        };

      
        Assert.Equal(-6, produto.PrazoMinimoMeses);
        Assert.True(produto.PrazoMinimoMeses < 0);
    }

    [Fact]
    public void ProdutoInvestimento_WithNegativeRentabilidade_ShouldStoreButIsInvalid()
    {
        var produto = new ProdutoInvestimento
        {
            Rentabilidade = -0.05m // Rentabilidade negativa
        };

        Assert.Equal(-0.05m, produto.Rentabilidade);
        Assert.True(produto.Rentabilidade < 0);
    }

    [Fact]
    public void ProdutoInvestimento_CanCreateHighYieldProduct()
    {
        
        var produto = new ProdutoInvestimento
        {
            Nome = "Fundo Ações Emergentes",
            Tipo = "Fundo de Ações",
            Rentabilidade = 0.35m, // 35%
            Risco = "Alto",
            ValorMinimo = 5000m,
            PrazoMinimoMeses = 24,
            PerfilRecomendado = "Agressivo"
        };

        // Assert
        Assert.Equal("Fundo Ações Emergentes", produto.Nome);
        Assert.True(produto.Rentabilidade > 0.30m); // Alta rentabilidade
        Assert.Equal("Alto", produto.Risco);
        Assert.Equal("Agressivo", produto.PerfilRecomendado);
    }

    [Fact]
    public void ProdutoInvestimento_CanCreateConservativeProduct()
    {
        
        var produto = new ProdutoInvestimento
        {
            Nome = "Tesouro Selic",
            Tipo = "Tesouro Direto",
            Rentabilidade = 0.08m, // 8%
            Risco = "Baixo",
            ValorMinimo = 100m,
            PrazoMinimoMeses = 1,
            PerfilRecomendado = "Conservador"
        };

       
        Assert.Equal("Tesouro Selic", produto.Nome);
        Assert.True(produto.Rentabilidade <= 0.10m); // Rentabilidade moderada
        Assert.Equal("Baixo", produto.Risco);
        Assert.Equal("Conservador", produto.PerfilRecomendado);
    }
}
