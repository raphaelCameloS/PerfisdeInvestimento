using PerfisdeInvestimento.Application.DTOs;
using PerfisdeInvestimento.Application.Interfaces.IRepositories;
using PerfisdeInvestimento.Application.Interfaces.IServices;
using PerfisdeInvestimento.Domain.Entities;
using PerfisdeInvestimento.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace PerfisdeInvestimento.Application.Services;

public class SimulacaoService : ISimulacaoService
{
    private readonly ISimulacaoRepository _simulacaoRepository;
    private readonly IProdutoRepository _produtoRepository;
    private readonly IRecomendacaoService _recomendacaoService;

    public SimulacaoService(ISimulacaoRepository simulacaoRepository, IProdutoRepository produtoRepository, IRecomendacaoService recomendacaoService)
    {
        _simulacaoRepository = simulacaoRepository;
        _produtoRepository = produtoRepository;
        _recomendacaoService = recomendacaoService;
    }

    //public async Task<SimulacaoResponse> SimularInvestimento(SimulacaoRequest request)
    //{
    //    // Buscar produtos que se adequam ao perfil (simulação simples)
    //    var produtos = await _produtoRepository.GetAllAsync();
    //    var produtoSelecionado = produtos.First(); // Lógica simples por enquanto

    //    // Calcular valor final
    //    var valorFinal = CalcularValorFinal(request.Valor, produtoSelecionado.Rentabilidade, request.PrazoMeses);

    //    // Criar entidade para salvar
    //    var simulacao = new SimulacaoInvestimento
    //    {
    //        ClienteId = request.ClienteId,
    //        Produto = produtoSelecionado.Nome,
    //        ValorInvestido = request.Valor,
    //        ValorFinal = valorFinal,
    //        PrazoMeses = request.PrazoMeses,
    //        DataSimulacao = DateTime.UtcNow
    //    };

    //    await _simulacaoRepository.AddAsync(simulacao);

    //    // Retornar response
    //    return new SimulacaoResponse
    //    {
    //        ProdutoValidado = new ProdutoValidado
    //        {
    //            Id = produtoSelecionado.Id,
    //            Nome = produtoSelecionado.Nome,
    //            Tipo = produtoSelecionado.Tipo,
    //            Rentabilidade = produtoSelecionado.Rentabilidade,
    //            Risco = produtoSelecionado.Risco
    //        },
    //        ResultadoSimulacao = new ResultadoSimulacao
    //        {
    //            ValorFinal = valorFinal,
    //            RentabilidadeEfetiva = produtoSelecionado.Rentabilidade,
    //            PrazoMeses = request.PrazoMeses
    //        },
    //        DataSimulacao = DateTime.UtcNow
    //    };
    //}

    //public async Task<SimulacaoResponse> SimularInvestimento(SimulacaoRequest request)
    //{
    //    // Buscar produto adequado baseado nos critérios
    //    var produtoSelecionado = await SelecionarProdutoAdequado(request);

    //    // Calcular valor final
    //    var valorFinal = CalcularValorFinal(request.Valor, produtoSelecionado.Rentabilidade, request.PrazoMeses);

    //    // Criar entidade para salvar
    //    var simulacao = new SimulacaoInvestimento
    //    {
    //        ClienteId = request.ClienteId,
    //        Produto = produtoSelecionado.Nome,
    //        ValorInvestido = request.Valor,
    //        ValorFinal = valorFinal,
    //        PrazoMeses = request.PrazoMeses,
    //        DataSimulacao = DateTime.UtcNow
    //    };

    //    await _simulacaoRepository.AddAsync(simulacao);

    //    // Retornar response
    //    return new SimulacaoResponse
    //    {
    //        ProdutoValidado = new ProdutoValidado
    //        {
    //            Id = produtoSelecionado.Id,
    //            Nome = produtoSelecionado.Nome,
    //            Tipo = produtoSelecionado.Tipo,
    //            Rentabilidade = produtoSelecionado.Rentabilidade,
    //            Risco = produtoSelecionado.Risco
    //        },
    //        ResultadoSimulacao = new ResultadoSimulacao
    //        {
    //            ValorFinal = valorFinal,
    //            RentabilidadeEfetiva = produtoSelecionado.Rentabilidade,
    //            PrazoMeses = request.PrazoMeses
    //        },
    //        DataSimulacao = DateTime.UtcNow
    //    };
    //}
    public async Task<SimulacaoResponse> SimularInvestimento(SimulacaoRequest request)
    {
        if (request.Valor <= 0)
            throw new ValidationException("O valor do investimento deve ser maior que zero.");

        if (request.PrazoMeses <= 0)
            throw new ValidationException("O prazo em meses deve ser maior que zero.");

        var produtoSelecionado = await SelecionarProdutoAdequado(request);

        if (produtoSelecionado == null)
        {
            //throw new NotFoundException(
            //$"Nenhum produto do tipo '{request.TipoProduto}' encontrado para valor R$ {request.Valor} e prazo {request.PrazoMeses} meses. " +
            //"Sugerimos ajustar os valores ou consultar os produtos disponíveis."
            throw new NotFoundException(
            $"Nenhum produto compatível encontrado. " +
            $"Filtros: Tipo='{request.TipoProduto}', Valor={request.Valor}, Prazo={request.PrazoMeses} meses. " +
            $"Produtos disponíveis: {await GetProdutosDisponiveisFormatados()}"

        );
        }

        // Calcular valor final
        var valorFinal = CalcularValorFinal(request.Valor, produtoSelecionado.Rentabilidade, request.PrazoMeses);

        // Criar entidade para salvar
        var simulacao = new SimulacaoInvestimento
        {
            ClienteId = request.ClienteId,
            Produto = produtoSelecionado.Nome,
            ValorInvestido = request.Valor,
            ValorFinal = valorFinal,
            PrazoMeses = request.PrazoMeses,
            DataSimulacao = DateTime.UtcNow
        };

        await _simulacaoRepository.AddAsync(simulacao);

        // Retornar response
        return new SimulacaoResponse
        {
            ProdutoValidado = new ProdutoValidado
            {
                Id = produtoSelecionado.Id,
                Nome = produtoSelecionado.Nome,
                Tipo = produtoSelecionado.Tipo,
                Rentabilidade = produtoSelecionado.Rentabilidade,
                Risco = produtoSelecionado.Risco
            },
            ResultadoSimulacao = new ResultadoSimulacao
            {
                ValorFinal = valorFinal,
                RentabilidadeEfetiva = produtoSelecionado.Rentabilidade,
                PrazoMeses = request.PrazoMeses
            },
            DataSimulacao = DateTime.UtcNow
        };
    }

    private async Task<string> GetProdutosDisponiveisFormatados()
    {
        var produtos = await _produtoRepository.GetAllAsync();
        return string.Join("; ", produtos.Select(p =>
            $"{p.Nome} (Tipo: {p.Tipo}, Min: R${p.ValorMinimo}, Prazo: {p.PrazoMinimoMeses} meses)"));
    }

    public async Task<List<HistoricoSimulacaoResponse>> GetHistoricoSimulacoes()
    {
        var simulacoes = await _simulacaoRepository.GetAllAsync();

        return simulacoes.Select(s => new HistoricoSimulacaoResponse
        {
            Id = s.Id,
            ClienteId = s.ClienteId,
            Produto = s.Produto,
            ValorInvestido = s.ValorInvestido,
            ValorFinal = s.ValorFinal,
            PrazoMeses = s.PrazoMeses,
            DataSimulacao = s.DataSimulacao
        }).ToList();
    }


    private decimal CalcularValorFinal(decimal valorInicial, decimal rentabilidade, int prazoMeses)
    {
        // Fórmula simples de juros compostos
        return valorInicial * (decimal)Math.Pow(1 + (double)rentabilidade, prazoMeses / 12.0);
    }

    public async Task<List<SimulacaoPorProdutoResponse>> GetSimulacoesPorProdutoDia()
    {
        var simulacoesAgrupadas = await _simulacaoRepository.GetSimulacoesAgrupadasPorProdutoDiaAsync();

        return simulacoesAgrupadas.Select(s => new SimulacaoPorProdutoResponse
        {
            Produto = s.Produto,
            Data = s.Data.ToString("yyyy-MM-dd"),
            QuantidadeSimulacoes = s.QuantidadeSimulacoes,
            MediaValorFinal = s.MediaValorFinal
        }).ToList();
    }

    //private async Task<ProdutoInvestimento> SelecionarProdutoAdequado(SimulacaoRequest request)
    //{
    //    var todosProdutos = await _produtoRepository.GetAllAsync();

    //    // Filtra por tipo de produto solicitado
    //    var produtosFiltrados = todosProdutos
    //        .Where(p => p.Tipo.Equals(request.TipoProduto, StringComparison.OrdinalIgnoreCase))
    //        .ToList();

    //    // Se não encontrou do tipo específico, usa todos
    //    if (!produtosFiltrados.Any())
    //    {
    //        produtosFiltrados = todosProdutos;
    //    }

    //    //Filtra por valor mínimo
    //    produtosFiltrados = produtosFiltrados
    //        .Where(p => p.ValorMinimo <= request.Valor)
    //        .ToList();

    //    //Filtra por prazo mínimo
    //    produtosFiltrados = produtosFiltrados
    //        .Where(p => p.PrazoMinimoMeses <= request.PrazoMeses)
    //        .ToList();

    //    //Se ainda tem opções, escolhe a com maior rentabilidade
    //    if (produtosFiltrados.Any())
    //    {
    //        return produtosFiltrados
    //            .OrderByDescending(p => p.Rentabilidade)
    //            .First();
    //    }

    //    //Se não encontrou produto adequado, usa o padrão mais rentável
    //    return todosProdutos
    //        .OrderByDescending(p => p.Rentabilidade)
    //        .First();
    //}
    //private async Task<ProdutoInvestimento> SelecionarProdutoAdequado(SimulacaoRequest request)
    //{
    //    try
    //    {
    //        //Descobre o perfil do cliente
    //        var perfilCliente = await _recomendacaoService.CalcularPerfilRisco(request.ClienteId);
    //        Console.WriteLine($"Cliente {request.ClienteId} tem perfil: {perfilCliente.Perfil}");

    //        //Busca produtos recomendados para esse perfil
    //        var produtosRecomendados = await _produtoRepository
    //            .GetProdutosPorPerfilAsync(perfilCliente.Perfil);
    //        Console.WriteLine($"Encontrados {produtosRecomendados.Count} produtos para perfil {perfilCliente.Perfil}");

    //        //Filtra pelos critérios da simulação
    //        var produtosFiltrados = produtosRecomendados
    //            .Where(p => p.Tipo.Equals(request.TipoProduto, StringComparison.OrdinalIgnoreCase) ||
    //                       string.IsNullOrEmpty(request.TipoProduto)) // Se não especificou tipo, considera todos
    //            .Where(p => p.ValorMinimo <= request.Valor)
    //            .Where(p => p.PrazoMinimoMeses <= request.PrazoMeses)
    //            .ToList();

    //        Console.WriteLine($"Após filtros: {produtosFiltrados.Count} produtos");

    //        //Escolhe o mais rentável
    //        if (produtosFiltrados.Any())
    //        {
    //            var melhorProduto = produtosFiltrados
    //                .OrderByDescending(p => p.Rentabilidade)
    //                .First();
    //            Console.WriteLine($"Produto selecionado: {melhorProduto.Nome} (Rentabilidade: {melhorProduto.Rentabilidade})");
    //            return melhorProduto;
    //        }

    //        //Fallback - se não encontrou, usa qualquer produto adequado
    //        Console.WriteLine("Nenhum produto encontrado para o perfil. Usando fallback...");
    //        var todosProdutos = await _produtoRepository.GetAllAsync();
    //        var fallback = todosProdutos
    //            .Where(p => p.ValorMinimo <= request.Valor)
    //            .Where(p => p.PrazoMinimoMeses <= request.PrazoMeses)
    //            .OrderByDescending(p => p.Rentabilidade)
    //            .FirstOrDefault();

    //        return fallback ?? todosProdutos.First(); // Último fallback
    //    }
    //    catch (Exception ex)
    //    {
    //        // Se der erro no sistema de recomendação, usa fallback
    //        Console.WriteLine($"Erro ao recuperar a recomendação: {ex.Message}");
    //        var todosProdutos = await _produtoRepository.GetAllAsync();
    //        return todosProdutos.First();
    //    }
    //}
    //private async Task<ProdutoInvestimento> SelecionarProdutoAdequado(SimulacaoRequest request)
    //{
    //    try
    //    {
    //        var perfilCliente = await _recomendacaoService.CalcularPerfilRisco(request.ClienteId);
    //        Console.WriteLine($"Cliente {request.ClienteId} - Perfil: {perfilCliente.Perfil}");

    //        var todosProdutos = await _produtoRepository.GetAllAsync();

    //        //Tenta produto do TIPO + PERFIL + PRAZO exatos
    //        var produtoIdeal = await EncontrarProdutoIdeal(request, perfilCliente.Perfil);
    //        if (produtoIdeal != null) return produtoIdeal;

    //        //Se não encontrou, busca produtos do MESMO PERFIL (qualquer tipo)
    //        var produtosMesmoPerfil = todosProdutos
    //            .Where(p => p.PerfilRecomendado == perfilCliente.Perfil)
    //            .Where(p => p.ValorMinimo <= request.Valor)
    //            .Where(p => p.PrazoMinimoMeses <= request.PrazoMeses)
    //            .OrderByDescending(p => p.Rentabilidade)
    //            .FirstOrDefault();

    //        if (produtosMesmoPerfil != null)
    //        {
    //            Console.WriteLine($"Produto do MESMO PERFIL: {produtosMesmoPerfil.Nome}");
    //            return produtosMesmoPerfil;
    //        }

    //        //Se não encontrou, busca produtos de PERFIL INFERIOR (mais conservador)
    //        var produtosPerfilInferior = await BuscarProdutosPerfilInferior(request, perfilCliente.Perfil);
    //        if (produtosPerfilInferior != null) return produtosPerfilInferior;

    //        //Último recurso: qualquer produto que atenda
    //        var fallback = todosProdutos
    //            .Where(p => p.ValorMinimo <= request.Valor)
    //            .Where(p => p.PrazoMinimoMeses <= request.PrazoMeses)
    //            .OrderByDescending(p => p.Rentabilidade)
    //            .FirstOrDefault();

    //        return fallback ?? todosProdutos.First();
    //    }
    //    catch (Exception ex)
    //    {
    //        Console.WriteLine($"Erro: {ex.Message}");
    //        var todosProdutos = await _produtoRepository.GetAllAsync();
    //        return todosProdutos.First();
    //    }
    //}

    //private async Task<ProdutoInvestimento> EncontrarProdutoIdeal(SimulacaoRequest request, string perfil)
    //{
    //    var produtosPerfil = await _produtoRepository.GetProdutosPorPerfilAsync(perfil);

    //    return produtosPerfil
    //        .Where(p => p.Tipo.Equals(request.TipoProduto, StringComparison.OrdinalIgnoreCase))
    //        .Where(p => p.ValorMinimo <= request.Valor)
    //        .Where(p => p.PrazoMinimoMeses <= request.PrazoMeses)
    //        .OrderByDescending(p => p.Rentabilidade)
    //        .FirstOrDefault();
    //}

    //private async Task<ProdutoInvestimento> BuscarProdutosPerfilInferior(SimulacaoRequest request, string perfilAtual)
    //{
    //    var todosProdutos = await _produtoRepository.GetAllAsync();
    //    string perfilBusca = perfilAtual;

    //    // Hierarquia de perfis: Agressivo → Moderado → Conservador
    //    if (perfilAtual == "Agressivo")
    //        perfilBusca = "Moderado";
    //    else if (perfilAtual == "Moderado")
    //        perfilBusca = "Conservador";
    //    else
    //        return null; // Conservador não tem perfil inferior

    //    var produtoConservador = todosProdutos
    //        .Where(p => p.PerfilRecomendado == perfilBusca)
    //        .Where(p => p.ValorMinimo <= request.Valor)
    //        .Where(p => p.PrazoMinimoMeses <= request.PrazoMeses)
    //        .OrderByDescending(p => p.Rentabilidade)
    //        .FirstOrDefault();

    //    if (produtoConservador != null)
    //    {
    //        Console.WriteLine($"Produto de PERFIL INFERIOR (mais seguro): {produtoConservador.Nome}");
    //    }

    //    return produtoConservador;
    //}
    private async Task<ProdutoInvestimento> SelecionarProdutoAdequado(SimulacaoRequest request)
    {
        var todosProdutos = await _produtoRepository.GetAllAsync();

        Console.WriteLine($"Buscando produto: Tipo={request.TipoProduto}, Valor={request.Valor}, Prazo={request.PrazoMeses} meses");

        // Filtra produtos que atendem os critérios exatos
        var produtosCompatíveis = todosProdutos
            .Where(p => p.Tipo.Equals(request.TipoProduto, StringComparison.OrdinalIgnoreCase))
            .Where(p => p.ValorMinimo <= request.Valor)
            .Where(p => p.PrazoMinimoMeses <= request.PrazoMeses)
            .ToList();

        Console.WriteLine($"Produtos compatíveis encontrados: {produtosCompatíveis.Count}");

        // Se encontrou produtos compatíveis, retorna o mais rentável
        if (produtosCompatíveis.Any())
        {
            var produtoSelecionado = produtosCompatíveis
                .OrderByDescending(p => p.Rentabilidade)
                .First();

            Console.WriteLine($"Produto selecionado: {produtoSelecionado.Nome} (Rentabilidade: {produtoSelecionado.Rentabilidade})");
            return produtoSelecionado;
        }

        //Se não encontrou, retorna null (vamos tratar o erro no método principal)
        Console.WriteLine("Nenhum produto compatível encontrado");
        return null;
    }


}

