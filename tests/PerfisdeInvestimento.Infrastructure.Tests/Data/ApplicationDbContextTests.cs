using Microsoft.EntityFrameworkCore;
using PerfisdeInvestimento.Domain.Entities;
using PerfisdeInvestimento.Infrastructure.Data;
using Xunit;

namespace PerfisdeInvestimento.Infrastructure.Tests.Data;

public class ApplicationDbContextTests
{
    private DbContextOptions<ApplicationDbContext> CreateOptions()
    {
        return new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite("DataSource=:memory:") // Banco em memória
            .Options;
    }

    [Fact]
    public async Task ApplicationDbContext_CanConnectToDatabase()
    {
        // Arrange
        var options = CreateOptions();

        using var context = new ApplicationDbContext(options);
        await context.Database.OpenConnectionAsync();
        await context.Database.EnsureCreatedAsync();

        // Act & Assert
        Assert.True(await context.Database.CanConnectAsync());
    }

    [Fact]
    public async Task ApplicationDbContext_SeedData_ShouldBeLoaded()
    {
        // Arrange
        var options = CreateOptions();

        using var context = new ApplicationDbContext(options);
        await context.Database.OpenConnectionAsync();
        await context.Database.EnsureCreatedAsync();

        // Act
        var produtos = await context.Produtos.ToListAsync();
        var historicos = await context.HistoricosInvestimentos.ToListAsync();

        // Assert
        Assert.Equal(3, produtos.Count);
        Assert.Equal(3, historicos.Count);
        Assert.Contains(produtos, p => p.Nome == "CDB Caixa 2025");
        Assert.Contains(historicos, h => h.ClienteId == 123);
    }

    [Fact]
    public async Task ApplicationDbContext_CanSaveAndRetrieveEntities()
    {
        // Arrange
        var options = CreateOptions();

        using var context = new ApplicationDbContext(options);
        await context.Database.OpenConnectionAsync();
        await context.Database.EnsureCreatedAsync();

        var novaSimulacao = new SimulacaoInvestimento
        {
            ClienteId = 999,
            Produto = "Teste",
            ValorInvestido = 1000,
            ValorFinal = 1100,
            PrazoMeses = 12
        };

        // Act
        context.Simulacoes.Add(novaSimulacao);
        await context.SaveChangesAsync();

        var simulacaoSalva = await context.Simulacoes
            .FirstAsync(s => s.ClienteId == 999);

        // Assert
        Assert.NotNull(simulacaoSalva);
        Assert.Equal("Teste", simulacaoSalva.Produto);
        Assert.Equal(1000, simulacaoSalva.ValorInvestido);
    }
}