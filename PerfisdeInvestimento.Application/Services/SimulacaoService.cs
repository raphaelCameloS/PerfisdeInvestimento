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

    public async Task<SimulacaoResponse> SimularInvestimento(SimulacaoRequest request)
    {
        if (request.Valor <= 0)
            throw new ValidationException("O valor do investimento deve ser maior que zero.");

        if (request.PrazoMeses <= 0)
            throw new ValidationException("O prazo em meses deve ser maior que zero.");

        var produtoSelecionado = await SelecionarProdutoAdequado(request);

        if (produtoSelecionado == null)
        {
          
            throw new NotFoundException(
            $"Nenhum produto compatível encontrado. " +
            $"Filtros: Tipo='{request.TipoProduto}', Valor={request.Valor}, Prazo={request.PrazoMeses} meses. " +
            $"Produtos disponíveis: {await GetProdutosDisponiveisFormatados()}"

        );
        }

        
        var valorFinal = CalcularValorFinal(request.Valor, produtoSelecionado.Rentabilidade, request.PrazoMeses);


        var valorFinalArredondado = Math.Round(valorFinal, 2);
        var rentabilidadeArredondada = Math.Round(produtoSelecionado.Rentabilidade, 2);

        var simulacao = new SimulacaoInvestimento
        {
            ClienteId = request.ClienteId,
            Produto = produtoSelecionado.Nome,
            ValorInvestido = request.Valor,
            ValorFinal = valorFinalArredondado,
            PrazoMeses = request.PrazoMeses,
            DataSimulacao = DateTime.UtcNow
        };

        await _simulacaoRepository.AddAsync(simulacao);

        return new SimulacaoResponse
        {
            ProdutoValidado = new ProdutoValidado
            {
                Id = produtoSelecionado.Id,
                Nome = produtoSelecionado.Nome,
                Tipo = produtoSelecionado.Tipo,
                Rentabilidade = rentabilidadeArredondada,
                Risco = produtoSelecionado.Risco
            },
            ResultadoSimulacao = new ResultadoSimulacao
            {
                ValorFinal = valorFinalArredondado,
                RentabilidadeEfetiva =rentabilidadeArredondada,
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
        var valorCalculado = valorInicial * (decimal)Math.Pow(1 + (double)rentabilidade, prazoMeses / 12.0);
        return Math.Round(valorCalculado, 2);
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

