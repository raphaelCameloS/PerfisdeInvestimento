using Moq;
using PerfisdeInvestimento.Application.DTOs;
using PerfisdeInvestimento.Application.Interfaces.IRepositories;
using PerfisdeInvestimento.Application.Services;
using PerfisdeInvestimento.Domain.Entities;
using PerfisdeInvestimento.Domain.Exceptions;
using PerfisdeInvestimento.Infrastructure.Repositories;
using Xunit;

namespace PerfisdeInvestimento.Application.Tests.Services;

public class RecomendacaoServiceTests
{
    private readonly Mock<IHistoricoInvestimentoRepository> _mockHistoricoRepository;
    private readonly Mock<IProdutoRepository> _mockProdutoRepository;
    private readonly RecomendacaoService _service;

    public RecomendacaoServiceTests()
    {
        _mockHistoricoRepository = new Mock<IHistoricoInvestimentoRepository>();
        _mockProdutoRepository = new Mock<IProdutoRepository>();
        _service = new RecomendacaoService(_mockHistoricoRepository.Object, _mockProdutoRepository.Object);
    }

    [Fact]
    public async Task CalcularPerfilRisco_WithInvestmentHistory_ShouldReturnPerfilCalculado()
    {
        
        var clienteId = 1;
        var historico = new List<HistoricoInvestimento>
        {
            new HistoricoInvestimento
            {
                Id = 1,
                ClienteId = clienteId,
                Tipo = "CDB",
                Valor = 30000,
                Rentabilidade = 0.12m,
                Data = DateTime.Now.AddMonths(-1) 
            },
            new HistoricoInvestimento
            {
                Id = 2,
                ClienteId = clienteId,
                Tipo = "Ação",
                Valor = 25000,
                Rentabilidade = 0.15m,
                Data = DateTime.Now.AddMonths(-2) 
            }
        };

        _mockHistoricoRepository
            .Setup(repo => repo.GetByClienteIdAsync(clienteId))
            .ReturnsAsync(historico);

        // Act
        var resultado = await _service.CalcularPerfilRisco(clienteId);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(clienteId, resultado.ClienteId);
        Assert.True(resultado.Pontuacao >= 0 && resultado.Pontuacao <= 100);
        Assert.Contains(resultado.Perfil, new[] { "Conservador", "Moderado", "Agressivo" });
        Assert.False(string.IsNullOrEmpty(resultado.Descricao));

        _mockHistoricoRepository.Verify(repo => repo.GetByClienteIdAsync(clienteId), Times.Once);
    }


    [Fact]
    public async Task CalcularPerfilRisco_WithNullHistory_ShouldThrowNotFoundException()
    {
     
        var clienteId = 1;
        List<HistoricoInvestimento> historicoNulo = null!;

        _mockHistoricoRepository
            .Setup(repo => repo.GetByClienteIdAsync(clienteId))
            .ReturnsAsync(historicoNulo);

       
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _service.CalcularPerfilRisco(clienteId));

        Assert.Contains("não possui histórico de investimentos", exception.Message);
        Assert.Contains(clienteId.ToString(), exception.Message);

        _mockHistoricoRepository.Verify(repo => repo.GetByClienteIdAsync(clienteId), Times.Once);
    }

    [Fact]
    public async Task CalcularPerfilRisco_WithEmptyHistory_ShouldThrowNotFoundException()
    {
        
        var clienteId = 1;
        var historicoVazio = new List<HistoricoInvestimento>();

        _mockHistoricoRepository
            .Setup(repo => repo.GetByClienteIdAsync(clienteId))
            .ReturnsAsync(historicoVazio);

        
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _service.CalcularPerfilRisco(clienteId));

        Assert.Contains("não possui histórico de investimentos", exception.Message);

        _mockHistoricoRepository.Verify(repo => repo.GetByClienteIdAsync(clienteId), Times.Once);
    }

    [Fact]
    public async Task GetProdutosRecomendados_WithValidPerfil_ShouldReturnProdutosFiltrados()
    {
        
        var perfil = "Moderado";
        var produtos = new List<ProdutoInvestimento>
        {
            new ProdutoInvestimento
            {
                Id = 1,
                Nome = "CDB Moderado",
                Tipo = "CDB",
                Rentabilidade = 0.12m,
                Risco = "Médio",
                PerfilRecomendado = "Moderado"
            },
            new ProdutoInvestimento
            {
                Id = 2,
                Nome = "Fundo Moderado",
                Tipo = "Fundo",
                Rentabilidade = 0.10m,
                Risco = "Médio",
                PerfilRecomendado = "Moderado"
            }
        };

        _mockProdutoRepository
            .Setup(repo => repo.GetProdutosPorPerfilAsync(perfil))
            .ReturnsAsync(produtos);

        
        var resultado = await _service.GetProdutosRecomendados(perfil);

        
        Assert.Equal(2, resultado.Count);
        Assert.All(resultado, p => Assert.NotNull(p.Risco));
        Assert.Contains(resultado, p => p.Nome == "CDB Moderado");
        Assert.Contains(resultado, p => p.Nome == "Fundo Moderado");

        _mockProdutoRepository.Verify(repo => repo.GetProdutosPorPerfilAsync(perfil), Times.Once);
    }

    [Fact]
    public async Task GetProdutosRecomendados_WithNoProductsForPerfil_ShouldThrowNotFoundException()
    {
        
        var perfil = "Inexistente";
        var produtosVazios = new List<ProdutoInvestimento>();

        _mockProdutoRepository
            .Setup(repo => repo.GetProdutosPorPerfilAsync(perfil))
            .ReturnsAsync(produtosVazios);

        
        var todosProdutos = new List<ProdutoInvestimento>
        {
            new ProdutoInvestimento { PerfilRecomendado = "Conservador" },
            new ProdutoInvestimento { PerfilRecomendado = "Moderado" }
        };

        _mockProdutoRepository
            .Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(todosProdutos);

       
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _service.GetProdutosRecomendados(perfil));
        Assert.Contains("Nenhum produto encontrado", exception.Message);

        _mockProdutoRepository.Verify(repo => repo.GetProdutosPorPerfilAsync(perfil), Times.Once);
        _mockProdutoRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
    }

    [Theory]
    [InlineData("conservador", "Conservador")]
    [InlineData("MODERADO", "Moderado")]
    [InlineData("Agressivo", "Agressivo")]
    public async Task GetProdutosRecomendados_WithDifferentCases_ShouldNormalizePerfil(string perfilInput, string perfilNormalizado)
    {
       
        var produtos = new List<ProdutoInvestimento>
    {
        new ProdutoInvestimento
        {
            Id = 1,
            Nome = $"Produto {perfilNormalizado}",
            Tipo = "CDB",
            Rentabilidade = 0.10m,
            Risco = "Médio",
            PerfilRecomendado = perfilNormalizado
        }
    };

      
        _mockProdutoRepository
            .Setup(repo => repo.GetProdutosPorPerfilAsync(perfilNormalizado))
            .ReturnsAsync(produtos);

        var todosProdutos = new List<ProdutoInvestimento>
    {
        new ProdutoInvestimento { PerfilRecomendado = "Conservador" },
        new ProdutoInvestimento { PerfilRecomendado = "Moderado" },
        new ProdutoInvestimento { PerfilRecomendado = "Agressivo" }
    };

        _mockProdutoRepository
            .Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(todosProdutos);

        
        var resultado = await _service.GetProdutosRecomendados(perfilInput);

       
        Assert.Single(resultado);
        Assert.Equal($"Produto {perfilNormalizado}", resultado[0].Nome);

        _mockProdutoRepository.Verify(repo => repo.GetProdutosPorPerfilAsync(perfilNormalizado), Times.Once);
    }

    [Fact]
    public async Task CalcularPerfilRisco_WithHighVolumeAndFrequency_ShouldReturnAgressivo()
    {
       
        var clienteId = 1;
        var historico = new List<HistoricoInvestimento>
        {
            new HistoricoInvestimento
            {
                Id = 1, ClienteId = clienteId, Tipo = "CDB",
                Valor = 60000, Rentabilidade = 0.18m, Data = DateTime.Now.AddDays(-10)
            },
            new HistoricoInvestimento
            {
                Id = 2, ClienteId = clienteId, Tipo = "Ação",
                Valor = 55000, Rentabilidade = 0.16m, Data = DateTime.Now.AddDays(-20)
            },
            new HistoricoInvestimento
            {
                Id = 3, ClienteId = clienteId, Tipo = "Fundo",
                Valor = 48000, Rentabilidade = 0.17m, Data = DateTime.Now.AddDays(-30)
            },
            new HistoricoInvestimento
            {
                Id = 4, ClienteId = clienteId, Tipo = "CDB",
                Valor = 52000, Rentabilidade = 0.15m, Data = DateTime.Now.AddDays(-40)
            },
            new HistoricoInvestimento
            {
                Id = 5, ClienteId = clienteId, Tipo = "Ação",
                Valor = 58000, Rentabilidade = 0.19m, Data = DateTime.Now.AddDays(-50)
            },
            new HistoricoInvestimento
            {
                Id = 6, ClienteId = clienteId, Tipo = "Fundo",
                Valor = 61000, Rentabilidade = 0.20m, Data = DateTime.Now.AddDays(-60)
            },
            new HistoricoInvestimento
            {
                Id = 7, ClienteId = clienteId, Tipo = "CDB",
                Valor = 59000, Rentabilidade = 0.18m, Data = DateTime.Now.AddDays(-70)
            },
            new HistoricoInvestimento
            {
                Id = 8, ClienteId = clienteId, Tipo = "Ação",
                Valor = 63000, Rentabilidade = 0.21m, Data = DateTime.Now.AddDays(-80)
            },
            new HistoricoInvestimento
            {
                Id = 9, ClienteId = clienteId, Tipo = "Fundo",
                Valor = 57000, Rentabilidade = 0.17m, Data = DateTime.Now.AddDays(-90)
            },
            new HistoricoInvestimento
            {
                Id = 10, ClienteId = clienteId, Tipo = "CDB",
                Valor = 60000, Rentabilidade = 0.19m, Data = DateTime.Now.AddDays(-100)
            },
            new HistoricoInvestimento
            {
                Id = 11, ClienteId = clienteId, Tipo = "Ação",
                Valor = 62000, Rentabilidade = 0.22m, Data = DateTime.Now.AddDays(-110)
            },
            new HistoricoInvestimento
            {
                Id = 12, ClienteId = clienteId, Tipo = "Fundo",
                Valor = 58000, Rentabilidade = 0.18m, Data = DateTime.Now.AddDays(-120)
            }
        };

        _mockHistoricoRepository
            .Setup(repo => repo.GetByClienteIdAsync(clienteId))
            .ReturnsAsync(historico);

        
        var resultado = await _service.CalcularPerfilRisco(clienteId);

       
        Assert.Equal("Agressivo", resultado.Perfil);
        Assert.True(resultado.Pontuacao >= 70);
    }

    [Fact]
    public async Task CalcularPerfilRisco_WithLowVolumeAndFrequency_ShouldReturnConservador()
    {
        
        var clienteId = 1;
        var historico = new List<HistoricoInvestimento>
        {
            new HistoricoInvestimento
            {
                Id = 1, ClienteId = clienteId, Tipo = "CDB",
                Valor = 3000, Rentabilidade = 0.04m, Data = DateTime.Now.AddMonths(-3)
            },
            new HistoricoInvestimento
            {
                Id = 2, ClienteId = clienteId, Tipo = "Poupança",
                Valor = 2000, Rentabilidade = 0.03m, Data = DateTime.Now.AddMonths(-5)
            }
        };

        _mockHistoricoRepository
            .Setup(repo => repo.GetByClienteIdAsync(clienteId))
            .ReturnsAsync(historico);

      
        var resultado = await _service.CalcularPerfilRisco(clienteId);

       
        Assert.Equal("Conservador", resultado.Perfil);
        Assert.True(resultado.Pontuacao < 30);
    }

    [Fact]
    public async Task CalcularPerfilRisco_WithRepositoryException_ShouldThrowBusinessException()
    {
       
        var clienteId = 1;

        _mockHistoricoRepository
            .Setup(repo => repo.GetByClienteIdAsync(clienteId))
            .ThrowsAsync(new Exception("Erro de banco de dados"));

     
        var exception = await Assert.ThrowsAsync<BusinessException>(() => _service.CalcularPerfilRisco(clienteId));
        Assert.Contains("Erro ao calcular perfil", exception.Message);
        Assert.Contains(clienteId.ToString(), exception.Message);
    }
}