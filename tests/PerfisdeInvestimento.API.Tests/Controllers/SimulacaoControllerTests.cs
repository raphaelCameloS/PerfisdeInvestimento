using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PerfisdeInvestimento.API.Controllers;
using PerfisdeInvestimento.Application.DTOs;
using PerfisdeInvestimento.Application.Interfaces.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PerfisdeInvestimento.API.Tests.Controllers;

public class SimulacaoControllerTests
{
    private readonly Mock<ISimulacaoService> _mockSimulacaoService;
    private readonly Mock<ILogger<SimulacaoController>> _mockLogger;
    private readonly SimulacaoController _controller;

    public SimulacaoControllerTests()
    {
        _mockSimulacaoService = new Mock<ISimulacaoService>();
        _mockLogger = new Mock<ILogger<SimulacaoController>>();
        _controller = new SimulacaoController(_mockSimulacaoService.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetSimulacoesPorProdutoDia_DeveRetornarOkComLista()
    {
        // Arrange
        var simulacoesEsperadas = new List<SimulacaoPorProdutoResponse>
        {
            new SimulacaoPorProdutoResponse
            {
                MediaValorFinal=5700.56M,
                QuantidadeSimulacoes= 10,
                Produto = "CDB",
                Data = "2025-12-31"
            }
        };

        _mockSimulacaoService
            .Setup(service => service.GetSimulacoesPorProdutoDia())
            .ReturnsAsync(simulacoesEsperadas);

        // Act
        var resultado = await _controller.GetSimulacoesPorProdutoDia();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
        var retorno = Assert.IsType<List<SimulacaoPorProdutoResponse>>(okResult.Value);

        Assert.Single(retorno);
        Assert.Equal("CDB", retorno[0].Produto);
        Assert.Equal("2025-12-31", retorno[0].Data);
    }

    [Fact]
    public async Task GetSimulacoesPorProdutoDia_DeveRetornarListaVazia()
    {
        // Arrange
        var listaVazia = new List<SimulacaoPorProdutoResponse>();

        _mockSimulacaoService
            .Setup(service => service.GetSimulacoesPorProdutoDia())
            .ReturnsAsync(listaVazia);

        // Act
        var resultado = await _controller.GetSimulacoesPorProdutoDia();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
        var retorno = Assert.IsType<List<SimulacaoPorProdutoResponse>>(okResult.Value);

        Assert.Empty(retorno);
    }

    [Fact]
    public async Task GetSimulacoes_WhenDataExists_DeveRetornarListHistorico()
    {
        // Arrange
        var historicoEsperado = new List<HistoricoSimulacaoResponse>
    {
        new HistoricoSimulacaoResponse
        {
            Id = 1,
            ClienteId = 123,
            Produto = "Tesouro Direto",
            ValorInvestido = 5000,
            DataSimulacao = DateTime.UtcNow
        }
    };

        _mockSimulacaoService
            .Setup(service => service.GetHistoricoSimulacoes())
            .ReturnsAsync(historicoEsperado);

        // Act
        var resultado = await _controller.GetSimulacoes();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
        var retorno = Assert.IsType<List<HistoricoSimulacaoResponse>>(okResult.Value);

        Assert.Single(retorno);
        Assert.Equal("Tesouro Direto", retorno[0].Produto);
    }

    [Fact]
    public async Task SimularInvestimento_WhenValidRequest_DeveRetornarResultdoSimulcao()
    {
        // Arrange
        var request = new SimulacaoRequest
        {
            ClienteId = 123,
            TipoProduto = "CDB",
            Valor = 1000.00M,
            PrazoMeses = 12
        };

        
        var produtoValidoEsperado = new ProdutoValidado
        {
            Id = 1,
            Nome = "CDB Caixa 2026",
            Tipo = "CDB",
            Rentabilidade = 0.10M,
            Risco = "Baixo"

        };

        var resultadoEsperadoSimulacao = new ResultadoSimulacao
        {

            ValorFinal = 1100.00M,
            RentabilidadeEfetiva = 100.00M,
            PrazoMeses = 12

        };

        var responseEsperado = new SimulacaoResponse
        {
            ProdutoValidado = produtoValidoEsperado,
            DataSimulacao = new DateTime(2025, 10, 31),
            ResultadoSimulacao = resultadoEsperadoSimulacao

        };

        _mockSimulacaoService
            .Setup(service => service.SimularInvestimento(request))
            .ReturnsAsync(responseEsperado);

        // Act
        var resultado = await _controller.SimularInvestimento(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
        var retorno = Assert.IsType<SimulacaoResponse>(okResult.Value);

        Assert.Equal(1100, retorno.ResultadoSimulacao.ValorFinal);
        Assert.Equal(100, retorno.ResultadoSimulacao.RentabilidadeEfetiva);
    }
}
