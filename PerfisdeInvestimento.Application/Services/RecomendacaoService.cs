using PerfisdeInvestimento.Application.DTOs;
using PerfisdeInvestimento.Application.Interfaces.IRepositories;
using PerfisdeInvestimento.Domain.Entities;
using PerfisdeInvestimento.Infrastructure.Repositories;
using PerfisdeInvestimento.Application.Interfaces.IServices;

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
        var historico = await _historicoRepository.GetByClienteIdAsync(clienteId);
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

    public async Task<List<ProdutoRecomendadoResponse>> GetProdutosRecomendados(string perfil)
    {
        var produtos = await _produtoRepository.GetProdutosPorPerfilAsync(perfil);

        return produtos.Select(p => new ProdutoRecomendadoResponse
        {
            Id = p.Id,
            Nome = p.Nome,
            Tipo = p.Tipo,
            Rentabilidade = p.Rentabilidade,
            Risco = p.Risco
        }).ToList();
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
