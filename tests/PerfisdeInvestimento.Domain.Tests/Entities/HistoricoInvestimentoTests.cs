using PerfisdeInvestimento.Domain.Entities;
using Xunit;

namespace PerfisdeInvestimento.Domain.Tests.Entities;

public class HistoricoInvestimentoTests
{
    [Fact]
    public void HistoricoInvestimento_WithValidData_ShouldCreateSuccessfully()
    {
        var data = new DateTime(2024, 1, 15);

        var historico = new HistoricoInvestimento
        {
            Id = 1,
            ClienteId = 123,
            Tipo = "CDB",
            Valor = 5000.50m,
            Rentabilidade = 0.12m,
            Data = data
        };

        Assert.Equal(1, historico.Id);
        Assert.Equal(123, historico.ClienteId);
        Assert.Equal("CDB", historico.Tipo);
        Assert.Equal(5000.50m, historico.Valor);
        Assert.Equal(0.12m, historico.Rentabilidade);
        Assert.Equal(data, historico.Data);
    }

    [Fact]
    public void HistoricoInvestimento_WithDefaultValues_ShouldInitializeCorrectly()
    {
       
        var historico = new HistoricoInvestimento();

       
        Assert.Equal(0, historico.Id);
        Assert.Equal(0, historico.ClienteId);
        Assert.Null(historico.Tipo);
        Assert.Equal(0, historico.Valor);
        Assert.Equal(0, historico.Rentabilidade);
        Assert.Equal(default(DateTime), historico.Data);
    }

    [Theory]
    [InlineData(1000.50)]
    [InlineData(0.01)]
    [InlineData(999999.99)]
    public void HistoricoInvestimento_WithDifferentValores_ShouldStoreCorrectly(decimal valor)
    {
        
        var historico = new HistoricoInvestimento
        {
            Valor = valor,
            Rentabilidade = 0.10m
        };

        
        Assert.Equal(valor, historico.Valor);
    }

    [Theory]
    [InlineData(0.05)]   // 5%
    [InlineData(0.15)]   // 15%
    [InlineData(0.25)]   // 25%
    [InlineData(0.00)]   // 0%
    public void HistoricoInvestimento_WithDifferentRentabilidades_ShouldStoreCorrectly(decimal rentabilidade)
    {
        
        var historico = new HistoricoInvestimento
        {
            Valor = 1000m,
            Rentabilidade = rentabilidade
        };

        
        Assert.Equal(rentabilidade, historico.Rentabilidade);
    }

    [Fact]
    public void HistoricoInvestimento_WithFutureDate_ShouldStoreCorrectly()
    {
        
        var dataFutura = DateTime.Now.AddDays(30);

        
        var historico = new HistoricoInvestimento
        {
            Data = dataFutura
        };

        
        Assert.Equal(dataFutura.Date, historico.Data.Date);
    }

    [Fact]
    public void HistoricoInvestimento_WithPastDate_ShouldStoreCorrectly()
    {
        
        var dataPassada = DateTime.Now.AddMonths(-6);

        
        var historico = new HistoricoInvestimento
        {
            Data = dataPassada
        };

        
        Assert.Equal(dataPassada.Date, historico.Data.Date);
    }

    [Theory]
    [InlineData("CDB")]
    [InlineData("LCI")]
    [InlineData("LCA")]
    [InlineData("Tesouro Direto")]
    [InlineData("Fundo Imobiliário")]
    [InlineData("Ações")]
    public void HistoricoInvestimento_WithDifferentTipos_ShouldStoreCorrectly(string tipo)
    {
        
        var historico = new HistoricoInvestimento
        {
            Tipo = tipo
        };

       
        Assert.Equal(tipo, historico.Tipo);
    }
}