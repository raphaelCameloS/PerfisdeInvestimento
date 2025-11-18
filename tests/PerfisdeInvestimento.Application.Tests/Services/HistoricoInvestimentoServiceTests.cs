using Moq;
using PerfisdeInvestimento.Application.DTOs;
using PerfisdeInvestimento.Application.Interfaces.IRepositories;
using PerfisdeInvestimento.Application.Services;
using PerfisdeInvestimento.Domain.Entities;
using PerfisdeInvestimento.Domain.Exceptions;
using PerfisdeInvestimento.Infrastructure.Repositories;
using Xunit;

namespace PerfisdeInvestimento.Application.Tests.Services;

public class HistoricoInvestimentoServiceTests
{
    private readonly Mock<IHistoricoInvestimentoRepository> _mockHistoricoRepository;
    private readonly HistoricoInvestimentoService _service;

    public HistoricoInvestimentoServiceTests()
    {
        _mockHistoricoRepository = new Mock<IHistoricoInvestimentoRepository>();
        _service = new HistoricoInvestimentoService(_mockHistoricoRepository.Object);
    }

    [Fact]
    public async Task GetHistoricoInvestimentos_WithValidClienteId_ShouldReturnHistoricoList()
    {
        // Arrange
        var clienteId = 1;
        var historicos = new List<HistoricoInvestimento>
        {
            new HistoricoInvestimento
            {
                Id = 1,
                ClienteId = clienteId,
                Tipo = "CDB",
                Valor = 5000.50m,
                Rentabilidade = 0.12m,
                Data = new DateTime(2024, 1, 15)
            },
            new HistoricoInvestimento
            {
                Id = 2,
                ClienteId = clienteId,
                Tipo = "Tesouro Direto",
                Valor = 3000.00m,
                Rentabilidade = 0.10m,
                Data = new DateTime(2024, 1, 10)
            }
        };

        _mockHistoricoRepository
            .Setup(repo => repo.GetByClienteIdAsync(clienteId))
            .ReturnsAsync(historicos);

        // Act
        var resultado = await _service.GetHistoricoInvestimentos(clienteId);

        // Assert
        Assert.Equal(2, resultado.Count);

        var primeiroItem = resultado[0];
        Assert.Equal(1, primeiroItem.Id);
        Assert.Equal("CDB", primeiroItem.Tipo);
        Assert.Equal(5000.50m, primeiroItem.Valor);
        Assert.Equal(0.12m, primeiroItem.Rentabilidade);
        Assert.Equal("2024-01-15", primeiroItem.Data);

        var segundoItem = resultado[1];
        Assert.Equal(2, segundoItem.Id);
        Assert.Equal("Tesouro Direto", segundoItem.Tipo);
        Assert.Equal(3000.00m, segundoItem.Valor);
        Assert.Equal(0.10m, segundoItem.Rentabilidade);
        Assert.Equal("2024-01-10", segundoItem.Data);

        _mockHistoricoRepository.Verify(repo => repo.GetByClienteIdAsync(clienteId), Times.Once);
    }

    [Fact]
    public async Task GetHistoricoInvestimentos_WithEmptyHistory_ShouldThrowNotFoundException()
    {
        // Arrange
        var clienteId = 999;
        var historicoVazio = new List<HistoricoInvestimento>();

        _mockHistoricoRepository
            .Setup(repo => repo.GetByClienteIdAsync(clienteId))
            .ReturnsAsync(historicoVazio);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _service.GetHistoricoInvestimentos(clienteId));

        Assert.Contains($"Cliente {clienteId} não possui histórico de investimentos", exception.Message);
        _mockHistoricoRepository.Verify(repo => repo.GetByClienteIdAsync(clienteId), Times.Once);
    }

    [Fact]
    public async Task GetHistoricoInvestimentos_WithSingleInvestment_ShouldReturnSingleItem()
    {
        // Arrange
        var clienteId = 456;
        var historicos = new List<HistoricoInvestimento>
        {
            new HistoricoInvestimento
            {
                Id = 3,
                ClienteId = clienteId,
                Tipo = "LCI",
                Valor = 10000.00m,
                Rentabilidade = 0.14m,
                Data = new DateTime(2024, 1, 20)
            }
        };

        _mockHistoricoRepository
            .Setup(repo => repo.GetByClienteIdAsync(clienteId))
            .ReturnsAsync(historicos);

        // Act
        var resultado = await _service.GetHistoricoInvestimentos(clienteId);

        // Assert
        Assert.Single(resultado);
        Assert.Equal("LCI", resultado[0].Tipo);
        Assert.Equal(10000.00m, resultado[0].Valor);
        Assert.Equal(0.14m, resultado[0].Rentabilidade);
        Assert.Equal("2024-01-20", resultado[0].Data);
    }

    [Fact]
    public async Task GetHistoricoInvestimentos_ShouldFormatDateCorrectly()
    {
        // Arrange
        var clienteId = 1;
        var historicos = new List<HistoricoInvestimento>
        {
            new HistoricoInvestimento
            {
                Id = 1,
                ClienteId = clienteId,
                Tipo = "CDB",
                Valor = 1000m,
                Rentabilidade = 0.12m,
                Data = new DateTime(2024, 12, 25) // Data específica para testar formato
            }
        };

        _mockHistoricoRepository
            .Setup(repo => repo.GetByClienteIdAsync(clienteId))
            .ReturnsAsync(historicos);

        // Act
        var resultado = await _service.GetHistoricoInvestimentos(clienteId);

        // Assert
        Assert.Equal("2024-12-25", resultado[0].Data); // Verifica formato yyyy-MM-dd
    }

    [Theory]
    [InlineData(1)]
    [InlineData(999)]
    [InlineData(12345)]
    public async Task GetHistoricoInvestimentos_WithDifferentClientIds_ShouldCallRepositoryWithCorrectId(int clienteId)
    {
        // Arrange
        var historicoVazio = new List<HistoricoInvestimento>();

        _mockHistoricoRepository
            .Setup(repo => repo.GetByClienteIdAsync(clienteId))
            .ReturnsAsync(historicoVazio);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetHistoricoInvestimentos(clienteId));

        _mockHistoricoRepository.Verify(repo => repo.GetByClienteIdAsync(clienteId), Times.Once);
    }
}
