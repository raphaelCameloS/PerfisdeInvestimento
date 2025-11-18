using PerfisdeInvestimento.Application.DTOs;
using PerfisdeInvestimento.Application.Interfaces.IRepositories;
using PerfisdeInvestimento.Application.Interfaces.IServices;
using PerfisdeInvestimento.Domain.Entities;
using PerfisdeInvestimento.Domain.Exceptions;
using PerfisdeInvestimento.Infrastructure.Repositories;
using System.Globalization;

namespace PerfisdeInvestimento.Application.Services;

public class RecomendacaoService : IRecomendacaoService
{
    private readonly IHistoricoInvestimentoRepository _historicoRepository;
    private readonly IProdutoRepository _produtoRepository;

    public RecomendacaoService(IHistoricoInvestimentoRepository historicoRepository, IProdutoRepository produtoRepository)
    {
        _historicoRepository = historicoRepository;
        _produtoRepository = produtoRepository;
    }

    public async Task<PerfilRiscoResponse> CalcularPerfilRisco(int clienteId)
    {
        try
        {
            var historico = await _historicoRepository.GetByClienteIdAsync(clienteId);
            if (!historico.Any())
            {
                throw new NotFoundException($"Cliente {clienteId} não possui histórico de investimentos necessários para calcular o perfil de risco."); 
            }           
            var pontuacao = CalcularPontuacaoPerfil(historico);
            var perfil = DeterminarPerfil(pontuacao);

            return new PerfilRiscoResponse
            {
                ClienteId = clienteId,
                Perfil = perfil,
                Pontuacao = pontuacao,
                Descricao = ObterDescricaoPerfil(perfil)
            };
        }
        catch (Exception ex) when (ex is not NotFoundException)
        {
            throw new BusinessException($"Erro ao calcular perfil do cliente {clienteId}: {ex.Message}");
        }
    }

    public async Task<List<ProdutoRecomendadoResponse>> GetProdutosRecomendados(string perfil)
    {
        var perfilNormalizado = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(perfil.ToLower());

        var produtos = await _produtoRepository.GetProdutosPorPerfilAsync(perfil);

        if (!produtos.Any())
        {
            //throw new NotFoundException($"Nenhum produto encontrado para o perfil '{perfil}'. " +
            //                          $"Perfis disponíveis: Conservador, Moderado e Agressivo.");
            throw new NotFoundException(
            $"Nenhum produto encontrado para perfil '{perfil}' (normalizado: '{perfilNormalizado}'). " +
            $"Perfis disponíveis no banco: {string.Join(", ", await GetPerfisDisponiveis())}"
        );
        }

        return produtos.Select(p => new ProdutoRecomendadoResponse
        {
            Id = p.Id,
            Nome = p.Nome,
            Tipo = p.Tipo,
            Rentabilidade = p.Rentabilidade,
            Risco = p.Risco,
        }).ToList();

    }
    private async Task<List<string>> GetPerfisDisponiveis()
    {
        var todosProdutos = await _produtoRepository.GetAllAsync();
        return todosProdutos.Select(p => p.PerfilRecomendado).Distinct().ToList();
    }

    private int CalcularPontuacaoPerfil(List<HistoricoInvestimento> historico)
    {
        if (historico == null || !historico.Any())
            return 25; // Perfil neutro se não há histórico

        var pontuacao = 0;

        // Volume total de investimentos
        var volumeTotal = historico.Sum(h => h.Valor);
        pontuacao += volumeTotal switch
        {
            > 50000 => 30,
            > 20000 => 20,
            > 5000 => 10,
            _ => 5
        };

        // Frequência de movimentações (últimos 6 meses)
        var ultimos6Meses = historico.Where(h => h.Data >= DateTime.Now.AddMonths(-6));
        var frequencia = ultimos6Meses.Count();
        pontuacao += frequencia switch
        {
            > 10 => 25,
            > 5 => 15,
            > 2 => 10,
            _ => 5
        };

        // Preferência por rentabilidade
        var mediaRentabilidade = historico.Average(h => h.Rentabilidade);
        pontuacao += mediaRentabilidade switch
        {
            > 0.15m => 25,
            > 0.10m => 15,
            > 0.05m => 10,
            _ => 5
        };

        return Math.Min(pontuacao, 100);
    }

    private string DeterminarPerfil(int pontuacao)
    {
        return pontuacao switch
        {
            < 30 => "Conservador",
            < 70 => "Moderado",
            _ => "Agressivo"
        };
    }

    private string ObterDescricaoPerfil(string perfil)
    {
        return perfil switch
        {
            "Conservador" => "Perfil com foco em segurança e liquidez, priorizando preservação do capital.",
            "Moderado" => "Perfil equilibrado entre segurança e rentabilidade, com diversificação moderada.",
            "Agressivo" => "Perfil com tolerância a riscos elevados em busca de maior rentabilidade.",
            _ => "Perfil não identificado."
        };
    }
}
