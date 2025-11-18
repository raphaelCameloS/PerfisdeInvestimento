using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PerfisdeInvestimento.API.Controllers;
using PerfisdeInvestimento.Application.DTOs;
using PerfisdeInvestimento.Application.Interfaces.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PerfisdeInvestimento.API.Tests.Controllers;

public class InvestimentoControllerTests
{
    private readonly Mock<IHistoricoInvestimentoService> _mockHistoryService;
    private readonly Mock<ILogger<InvestimentoController>> _mockLogger;
    private readonly InvestimentoController _controller;

    public InvestimentoControllerTests()
    {
        _mockHistoryService = new Mock<IHistoricoInvestimentoService>();
        _mockLogger = new Mock<ILogger<InvestimentoController>>();
        _controller = new InvestimentoController(_mockHistoryService.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetHistoricoInvestimentos_WhenClienteHasInvestments_ShouldReturnHistoricoList()
    {
        // Arrange
        var clienteId = 123;
        var historicoEsperado = new List<HistoricoInvestimentosResponse>
        {
            new HistoricoInvestimentosResponse
            {
                Id = 1,
                Tipo = "CDB",
                Valor = 5000.50m,
                Rentabilidade = 0.12m,
                Data = "2024-01-15"
            },
            new HistoricoInvestimentosResponse
            {
                Id = 2,
                Tipo = "Tesouro Direto",
                Valor = 3000.00m,
                Rentabilidade = 0.10m,
                Data = "2024-01-10"
            }
        };

        _mockHistoryService
            .Setup(service => service.GetHistoricoInvestimentos(clienteId))
            .ReturnsAsync(historicoEsperado);

        // Act
        var resultado = await _controller.GetHistoricoInvestimentos(clienteId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
        var retorno = Assert.IsType<List<HistoricoInvestimentosResponse>>(okResult.Value);

        Assert.Equal(2, retorno.Count);
        Assert.Equal("CDB", retorno[0].Tipo);
        Assert.Equal(5000.50m, retorno[0].Valor);
        Assert.Equal(0.12m, retorno[0].Rentabilidade);

        _mockHistoryService.Verify(
            service => service.GetHistoricoInvestimentos(clienteId),
            Times.Once);
    }

    [Fact]
    public async Task GetHistoricoInvestimentos_WhenClienteHasNoInvestments_ShouldReturnEmptyList()
    {
        // Arrange
        var clienteId = 999;
        var listaVazia = new List<HistoricoInvestimentosResponse>();

        _mockHistoryService
            .Setup(service => service.GetHistoricoInvestimentos(clienteId))
            .ReturnsAsync(listaVazia);

        // Act
        var resultado = await _controller.GetHistoricoInvestimentos(clienteId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
        var retorno = Assert.IsType<List<HistoricoInvestimentosResponse>>(okResult.Value);

        Assert.Empty(retorno);

        _mockHistoryService.Verify(
            service => service.GetHistoricoInvestimentos(clienteId),
            Times.Once);
    }

    [Fact]
    public async Task GetHistoricoInvestimentos_WhenSingleInvestment_ShouldReturnSingleItem()
    {
        // Arrange
        var clienteId = 456;
        var historicoEsperado = new List<HistoricoInvestimentosResponse>
        {
            new HistoricoInvestimentosResponse
            {
                Id = 3,
                Tipo = "LCI",
                Valor = 10000.00m,
                Rentabilidade = 0.14m,
                Data = "2024-01-20"
            }
        };

        _mockHistoryService
            .Setup(service => service.GetHistoricoInvestimentos(clienteId))
            .ReturnsAsync(historicoEsperado);

        // Act
        var resultado = await _controller.GetHistoricoInvestimentos(clienteId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
        var retorno = Assert.IsType<List<HistoricoInvestimentosResponse>>(okResult.Value);

        Assert.Single(retorno);
        Assert.Equal("LCI", retorno[0].Tipo);
        Assert.Equal(10000.00m, retorno[0].Valor);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(999)]
    [InlineData(12345)]
    public async Task GetHistoricoInvestimentos_WithDifferentClientIds_ShouldCallServiceWithCorrectId(int clienteId)
    {
        // Arrange
        var listaVazia = new List<HistoricoInvestimentosResponse>();

        _mockHistoryService
            .Setup(service => service.GetHistoricoInvestimentos(clienteId))
            .ReturnsAsync(listaVazia);

        // Act
        var resultado = await _controller.GetHistoricoInvestimentos(clienteId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(resultado.Result);

        _mockHistoryService.Verify(
            service => service.GetHistoricoInvestimentos(clienteId),
            Times.Once);
    }
}
