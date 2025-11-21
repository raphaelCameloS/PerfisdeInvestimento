using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PerfisdeInvestimento.Domain.Entities;

namespace PerfisdeInvestimento.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<SimulacaoInvestimento> Simulacoes { get; set; }
    public DbSet<PerfilRisco> PerfisRisco { get; set; }
    public DbSet<ProdutoInvestimento> Produtos { get; set; }
    public DbSet<HistoricoInvestimento> HistoricosInvestimentos { get; set; }
    public DbSet<Telemetria> Telemetrias { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=perfisinvestimento.db");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<ProdutoInvestimento>().HasData(
            new ProdutoInvestimento
            {
                Id = 1,
                Nome = "CDB Caixa 2026",
                Tipo = "CDB",
                Rentabilidade = 0.12m,
                Risco = "Baixo",
                ValorMinimo = 1000.00m,
                PrazoMinimoMeses = 6,
                PerfilRecomendado = "Conservador"
            },
            new ProdutoInvestimento
            {
                Id = 2,
                Nome = "Fundo de Investimento Caixa Asset",
                Tipo = "Fundo",
                Rentabilidade = 0.18m,
                Risco = "Alto",
                ValorMinimo = 10000.00m,
                PrazoMinimoMeses = 12,
                PerfilRecomendado = "Agressivo"
            },
            new ProdutoInvestimento
            {
                Id = 3,
                Nome = "LCI Imobiliário Caixa",
                Tipo = "LCI",
                Rentabilidade = 0.14m,
                Risco = "Médio",
                ValorMinimo = 2000,
                PrazoMinimoMeses = 24,
                PerfilRecomendado = "Moderado"
            }
        );
        modelBuilder.Entity<HistoricoInvestimento>().HasData(
        new HistoricoInvestimento
        {
            Id = 1,
            ClienteId = 123,
            Tipo = "CDB",
            Valor = 5000.00m,
            Rentabilidade = 0.12m,
            Data = new DateTime(2025, 1, 15)
        },
        new HistoricoInvestimento
        {
            Id = 2,
            ClienteId = 123,
            Tipo = "Fundo",
            Valor = 3000.00m,
            Rentabilidade = 0.08m,
            Data = new DateTime(2025, 3, 10)
        },
        new HistoricoInvestimento
        {
            Id = 3,
            ClienteId = 456,
            Tipo = "LCI",
            Valor = 8000.00m,
            Rentabilidade = 0.14m,
            Data = new DateTime(2025, 2, 20)
        }
    );


    }
}


