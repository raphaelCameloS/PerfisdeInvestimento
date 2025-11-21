using Moq;
using PerfisdeInvestimento.Application.DTOs;
using PerfisdeInvestimento.Application.Interfaces.IRepositories;
using PerfisdeInvestimento.Application.Interfaces.IServices;
using PerfisdeInvestimento.Application.Services;
using PerfisdeInvestimento.Domain.Entities;
using PerfisdeInvestimento.Domain.Exceptions;
using Xunit;

namespace PerfisdeInvestimento.Application.Tests.Services;

public class SimulacaoServiceTests
{
    private readonly Mock<ISimulacaoRepository> _mockSimulacaoRepository;
    private readonly Mock<IProdutoRepository> _mockProdutoRepository;
    private readonly Mock<IRecomendacaoService> _mockRecomendacaoService;
    private readonly SimulacaoService _service;

    public SimulacaoServiceTests()
    {
        _mockSimulacaoRepository = new Mock<ISimulacaoRepository>();
        _mockProdutoRepository = new Mock<IProdutoRepository>();
        _mockRecomendacaoService = new Mock<IRecomendacaoService>();
        _service = new SimulacaoService(
            _mockSimulacaoRepository.Object,
            _mockProdutoRepository.Object,
            _mockRecomendacaoService.Object
        );
    }

    [Fact]
    public async Task SimularInvestimento_WithValidRequest_ShouldReturnSimulationResult()
    {
       
        var request = new SimulacaoRequest
        {
            ClienteId = 1,
            TipoProduto = "CDB",
            Valor = 1000,
            PrazoMeses = 12
        };

        var produtoMock = new ProdutoInvestimento
        {
            Id = 1,
            Nome = "CDB Banco XYZ",
            Tipo = "CDB",
            Rentabilidade = 0.12m,
            Risco = "Médio",
            ValorMinimo = 500,
            PrazoMinimoMeses = 6,
            PerfilRecomendado = "Moderado"
        };

        var simulacaoSalva = new SimulacaoInvestimento
        {
            Id = 1,
            ClienteId = 1,
            Produto = "CDB Banco XYZ",
            ValorInvestido = 1000,
            ValorFinal = 1120,
            PrazoMeses = 12,
            DataSimulacao = DateTime.UtcNow
        };

        var produtos = new List<ProdutoInvestimento> { produtoMock };

        _mockProdutoRepository
            .Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(produtos);

        _mockSimulacaoRepository
            .Setup(repo => repo.AddAsync(It.IsAny<SimulacaoInvestimento>()))
            .ReturnsAsync(simulacaoSalva);

       
        var resultado = await _service.SimularInvestimento(request);

     
        Assert.NotNull(resultado);
        Assert.Equal("CDB Banco XYZ", resultado.ProdutoValidado.Nome);
        Assert.Equal(0.12m, resultado.ProdutoValidado.Rentabilidade);
        Assert.Equal("Médio", resultado.ProdutoValidado.Risco);
        Assert.True(resultado.ResultadoSimulacao.ValorFinal > 1000); 

        _mockProdutoRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
        _mockSimulacaoRepository.Verify(repo => repo.AddAsync(It.IsAny<SimulacaoInvestimento>()), Times.Once);
    }

    [Fact]
    public async Task SimularInvestimento_WithZeroValue_ShouldThrowValidationException()
    {
        
        var request = new SimulacaoRequest
        {
            ClienteId = 1,
            TipoProduto = "CDB",
            Valor = 0, 
            PrazoMeses = 12
        };

        
        await Assert.ThrowsAsync<ValidationException>(() => _service.SimularInvestimento(request));

        // Verifica que NÃO chamou os repositórios
        _mockProdutoRepository.Verify(repo => repo.GetAllAsync(), Times.Never);
        _mockSimulacaoRepository.Verify(repo => repo.AddAsync(It.IsAny<SimulacaoInvestimento>()), Times.Never);
    }

    [Fact]
    public async Task SimularInvestimento_WithNegativeValue_ShouldThrowValidationException()
    {
        
        var request = new SimulacaoRequest
        {
            ClienteId = 1,
            TipoProduto = "CDB",
            Valor = -100, 
            PrazoMeses = 12
        };

      
        await Assert.ThrowsAsync<ValidationException>(() => _service.SimularInvestimento(request));

        _mockProdutoRepository.Verify(repo => repo.GetAllAsync(), Times.Never);
        _mockSimulacaoRepository.Verify(repo => repo.AddAsync(It.IsAny<SimulacaoInvestimento>()), Times.Never);
    }

    [Fact]
    public async Task SimularInvestimento_WithZeroMonths_ShouldThrowValidationException()
    {
        
        var request = new SimulacaoRequest
        {
            ClienteId = 1,
            TipoProduto = "CDB",
            Valor = 1000,
            PrazoMeses = 0 
        };

        await Assert.ThrowsAsync<ValidationException>(() => _service.SimularInvestimento(request));

        _mockProdutoRepository.Verify(repo => repo.GetAllAsync(), Times.Never);
        _mockSimulacaoRepository.Verify(repo => repo.AddAsync(It.IsAny<SimulacaoInvestimento>()), Times.Never);
    }


    [Fact]
    public async Task SimularInvestimento_WhenNoCompatibleProduct_ShouldThrowNotFoundException()
    {
       
        var request = new SimulacaoRequest
        {
            ClienteId = 1,
            TipoProduto = "CDB",
            Valor = 1000,
            PrazoMeses = 12
        };

        var produtos = new List<ProdutoInvestimento>
    {
        new ProdutoInvestimento
        {
            Id = 1,
            Nome = "LCI",
            Tipo = "LCI", 
            Rentabilidade = 0.10m,
            ValorMinimo = 500,
            PrazoMinimoMeses = 6,
            PerfilRecomendado = "Conservador"
        }
    };

        _mockProdutoRepository
            .Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(produtos);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _service.SimularInvestimento(request));
        Assert.Contains("Nenhum produto compatível encontrado", exception.Message);


        _mockProdutoRepository.Verify(repo => repo.GetAllAsync(), Times.Exactly(2));
        _mockSimulacaoRepository.Verify(repo => repo.AddAsync(It.IsAny<SimulacaoInvestimento>()), Times.Never);
    }


    [Fact]
    public async Task SimularInvestimento_WhenProductValueTooHigh_ShouldThrowNotFoundException()
    {
        
        var request = new SimulacaoRequest
        {
            ClienteId = 1,
            TipoProduto = "CDB",
            Valor = 100, // Valor muito baixo
            PrazoMeses = 12
        };

        var produtos = new List<ProdutoInvestimento>
    {
        new ProdutoInvestimento
        {
            Id = 1,
            Nome = "CDB Premium",
            Tipo = "CDB",
            Rentabilidade = 0.12m,
            ValorMinimo = 1000, // Valor mínimo maior que o solicitado
            PrazoMinimoMeses = 6,
            PerfilRecomendado = "Moderado"
        }
    };

        // Setup para 2 chamadas
        _mockProdutoRepository
            .Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(produtos);

       
        await Assert.ThrowsAsync<NotFoundException>(() => _service.SimularInvestimento(request));

        // Agora espera 2 chamadas
        _mockProdutoRepository.Verify(repo => repo.GetAllAsync(), Times.Exactly(2));
        _mockSimulacaoRepository.Verify(repo => repo.AddAsync(It.IsAny<SimulacaoInvestimento>()), Times.Never);
    }


    [Fact]
    public async Task GetHistoricoSimulacoes_ShouldReturnMappedResponses()
    {
        
        var simulacoes = new List<SimulacaoInvestimento>
        {
            new SimulacaoInvestimento
            {
                Id = 1,
                ClienteId = 1,
                Produto = "CDB",
                ValorInvestido = 1000,
                ValorFinal = 1120,
                PrazoMeses = 12,
                DataSimulacao = DateTime.UtcNow
            }
        };

        _mockSimulacaoRepository
            .Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(simulacoes);

        
        var resultado = await _service.GetHistoricoSimulacoes();

        Assert.Single(resultado);
        Assert.Equal(1, resultado[0].Id);
        Assert.Equal("CDB", resultado[0].Produto);
        Assert.Equal(1000, resultado[0].ValorInvestido);
        Assert.Equal(1120, resultado[0].ValorFinal);
        Assert.Equal(12, resultado[0].PrazoMeses);

        _mockSimulacaoRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetHistoricoSimulacoes_WhenEmpty_ShouldReturnEmptyList()
    {
        
        var simulacoes = new List<SimulacaoInvestimento>();

        _mockSimulacaoRepository
            .Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(simulacoes);

        
        var resultado = await _service.GetHistoricoSimulacoes();

       
        Assert.Empty(resultado);
        _mockSimulacaoRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetSimulacoesPorProdutoDia_ShouldReturnGroupedData()
    {
        
        var simulacoesAgrupadas = new List<SimulacaoPorProdutoDia>
        {
            new SimulacaoPorProdutoDia
            {
                Produto = "CDB",
                Data = DateTime.UtcNow.Date,
                QuantidadeSimulacoes = 5,
                MediaValorFinal = 1500.50m
            }
        };

        _mockSimulacaoRepository
            .Setup(repo => repo.GetSimulacoesAgrupadasPorProdutoDiaAsync())
            .ReturnsAsync(simulacoesAgrupadas);

      
        var resultado = await _service.GetSimulacoesPorProdutoDia();

        
        Assert.Single(resultado);
        Assert.Equal("CDB", resultado[0].Produto);
        Assert.Equal(5, resultado[0].QuantidadeSimulacoes);
        Assert.Equal(1500.50m, resultado[0].MediaValorFinal);
        Assert.Equal(DateTime.UtcNow.Date.ToString("yyyy-MM-dd"), resultado[0].Data);

        _mockSimulacaoRepository.Verify(repo => repo.GetSimulacoesAgrupadasPorProdutoDiaAsync(), Times.Once);
    }

    [Fact]
    public void CalcularValorFinal_WithValidParameters_ShouldCalculateCorrectly()
    {
        
        var valorInicial = 1000m;
        var rentabilidade = 0.12m;
        var prazoMeses = 12;

        var metodoCalcular = typeof(SimulacaoService)
            .GetMethod("CalcularValorFinal", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        
        Assert.NotNull(metodoCalcular);

        
        var valorFinal = (decimal)metodoCalcular!.Invoke(_service, new object[] { valorInicial, rentabilidade, prazoMeses })!;

        
        var valorEsperado = 1120m;
        Assert.Equal(valorEsperado, valorFinal);
    }


    [Fact]
    public void CalcularValorFinal_WithSixMonths_ShouldCalculateCorrectly()
    {
        
        var valorInicial = 1000m;
        var rentabilidade = 0.12m;
        var prazoMeses = 6;

        var metodoCalcular = typeof(SimulacaoService)
            .GetMethod("CalcularValorFinal", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

       
        Assert.NotNull(metodoCalcular);

        
        var valorFinal = (decimal)metodoCalcular!.Invoke(_service, new object[] { valorInicial, rentabilidade, prazoMeses })!;

       
        var valorEsperado = 1058.30m;
        Assert.Equal(valorEsperado, valorFinal, 2);
    }
}