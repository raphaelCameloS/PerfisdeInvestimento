using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PerfisdeInvestimento.Application.DTOs;
using PerfisdeInvestimento.Application.Interfaces.IRepositories;
using PerfisdeInvestimento.Application.Interfaces.IServices;
using PerfisdeInvestimento.Domain.Entities;
namespace PerfisdeInvestimento.Application.Services;

public class SimulacaoService : ISimulacaoService
{
    private readonly ISimulacaoRepository _simulacaoRepository;
    private readonly IProdutoRepository _produtoRepository;

    public SimulacaoService(ISimulacaoRepository simulacaoRepository, IProdutoRepository produtoRepository)
    {
        _simulacaoRepository = simulacaoRepository;
        _produtoRepository = produtoRepository;
    }

    public async Task<SimulacaoResponse> SimularInvestimento(SimulacaoRequest request)
    {
        // Buscar produtos que se adequam ao perfil (simulação simples)
        var produtos = await _produtoRepository.GetAllAsync();
        var produtoSelecionado = produtos.First(); // Lógica simples por enquanto

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

    public async Task<List<SimulacaoPorProdutoResponse>> GetSimulacoesPorProdutoDia()
    {
        var simulacoes = await _simulacaoRepository.GetSimulacoesPorProdutoDiaAsync();

        // Converter para DTO - implementação simples por enquanto
        return new List<SimulacaoPorProdutoResponse>();
    }

    private decimal CalcularValorFinal(decimal valorInicial, decimal rentabilidade, int prazoMeses)
    {
        // Fórmula simples de juros compostos
        return valorInicial * (decimal)Math.Pow(1 + (double)rentabilidade, prazoMeses / 12.0);
    }
}

