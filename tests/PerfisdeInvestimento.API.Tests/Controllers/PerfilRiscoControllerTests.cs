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

public class PerfilRiscoControllerTests
{
    private readonly Mock<IRecomendacaoService> _mockRecomendacaoService;
    private readonly Mock<ILogger<PerfilRiscoController>> _mockLogger;
    private readonly PerfilRiscoController _controller;

    public PerfilRiscoControllerTests()
    {
        _mockRecomendacaoService = new Mock<IRecomendacaoService>();
        _mockLogger = new Mock<ILogger<PerfilRiscoController>>();
        _controller = new PerfilRiscoController(_mockRecomendacaoService.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetPerfilRisco_WhenClienteExists_ShouldReturnPerfilRisco()
    {
       
        var clienteId = 123;
        var perfilEsperado = new PerfilRiscoResponse
        {
            ClienteId = clienteId,
            Perfil = "Moderado",
            Pontuacao = 65,
            Descricao = "Perfil equilibrado entre segurança e rentabilidade."
        };

        _mockRecomendacaoService
            .Setup(service => service.CalcularPerfilRisco(clienteId))
            .ReturnsAsync(perfilEsperado);

       
        var resultado = await _controller.GetPerfilRisco(clienteId);

        var actionResult = Assert.IsType<ActionResult<PerfilRiscoResponse>>(resultado);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var retorno = Assert.IsType<PerfilRiscoResponse>(okResult.Value);
        
        Assert.Equal(clienteId, retorno.ClienteId);
        Assert.Equal("Moderado", retorno.Perfil);
        Assert.Equal(65, retorno.Pontuacao);

        _mockRecomendacaoService.Verify(
            service => service.CalcularPerfilRisco(clienteId),
            Times.Once);
    }

    [Fact]
    public async Task GetProdutosRecomendados_WhenValidPerfil_ShouldReturnProdutosList()
    {
        
        var perfil = "moderado";
        var perfilNormalizado = "Moderado";
        var produtosEsperados = new List<ProdutoRecomendadoResponse>
        {
            new ProdutoRecomendadoResponse
            {
                Id = 3,
                Nome = "LCI Imobiliário Caixa",
                Tipo = "LCI",
                Rentabilidade = 0.14M,
                Risco = "Médio"
            }
        };

        _mockRecomendacaoService
            .Setup(service => service.GetProdutosRecomendados(perfilNormalizado))
            .ReturnsAsync(produtosEsperados);

       
        var resultado = await _controller.GetProdutosRecomendados(perfil);

        
        var okResult = Assert.IsType<OkObjectResult>(resultado);
        var retorno = Assert.IsType<List<ProdutoRecomendadoResponse>>(okResult.Value);

        Assert.Single(retorno);
        Assert.Equal("LCI Imobiliário Caixa", retorno[0].Nome);
        Assert.Equal("LCI", retorno[0].Tipo);
        Assert.Equal(0.14M, retorno[0].Rentabilidade);

        _mockRecomendacaoService.Verify(
            service => service.GetProdutosRecomendados(perfilNormalizado),
            Times.Once);
    }

    [Theory]
    [InlineData("conservador", "Conservador")]
    [InlineData("MODERADO", "Moderado")]
    [InlineData("ArRoJAdO", "Arrojado")]
    [InlineData("aGrEssIvE", "Agressive")] // Se existir este perfil
    public async Task GetProdutosRecomendados_WithDifferentCases_ShouldNormalizePerfil(string perfilInput, string perfilNormalizadoEsperado)
    {
        
        var produtosEsperados = new List<ProdutoRecomendadoResponse>();

        _mockRecomendacaoService
            .Setup(service => service.GetProdutosRecomendados(perfilNormalizadoEsperado))
            .ReturnsAsync(produtosEsperados);

       
        var resultado = await _controller.GetProdutosRecomendados(perfilInput);

       
        var okResult = Assert.IsType<OkObjectResult>(resultado);

        _mockRecomendacaoService.Verify(
            service => service.GetProdutosRecomendados(perfilNormalizadoEsperado),
            Times.Once);
    }

    [Fact]
    public async Task GetProdutosRecomendados_WhenMultipleProducts_ShouldReturnAll()
    {
        var perfil = "conservador";
        var perfilNormalizado = "Conservador";
        var produtosEsperados = new List<ProdutoRecomendadoResponse>
        {
            new ProdutoRecomendadoResponse { Id = 1, Nome = "Tesouro Selic", Tipo = "Tesouro", Rentabilidade = 0.12M, Risco = "Baixo" },
            new ProdutoRecomendadoResponse { Id = 2, Nome = "CDB Banco XYZ", Tipo = "CDB", Rentabilidade = 0.13M, Risco = "Baixo" },
            new ProdutoRecomendadoResponse { Id = 3, Nome = "LCI Imobiliário", Tipo = "LCI", Rentabilidade = 0.14M, Risco = "Médio" }
        };

        _mockRecomendacaoService
            .Setup(service => service.GetProdutosRecomendados(perfilNormalizado))
            .ReturnsAsync(produtosEsperados);

        
        var resultado = await _controller.GetProdutosRecomendados(perfil);

        
        var okResult = Assert.IsType<OkObjectResult>(resultado);
        var retorno = Assert.IsType<List<ProdutoRecomendadoResponse>>(okResult.Value);

        Assert.Equal(3, retorno.Count);
        Assert.Equal("Tesouro Selic", retorno[0].Nome);
        Assert.Equal("CDB Banco XYZ", retorno[1].Nome);
    }
}