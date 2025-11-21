using PerfisdeInvestimento.Domain.Entities;
using Xunit;

namespace PerfisdeInvestimento.Domain.Tests.Entities;

public class PerfilRiscoTests
{
    [Fact]
    public void PerfilRisco_Constructor_ShouldSetDataCalculoToUtcNow()
    {
       
        var antesCriacao = DateTime.UtcNow;

        
        var perfil = new PerfilRisco();

        var depoisCriacao = DateTime.UtcNow;

        
        Assert.InRange(perfil.DataCalculo, antesCriacao, depoisCriacao);
        Assert.Equal(DateTimeKind.Utc, perfil.DataCalculo.Kind);
    }

    [Fact]
    public void PerfilRisco_WithValidData_ShouldCreateSuccessfully()
    {
        
        var dataCalculo = new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Utc);

        
        var perfil = new PerfilRisco
        {
            Id = 1,
            ClienteId = 123,
            TipoPerfil = "Moderado",
            Pontuacao = 65,
            Descricao = "Perfil equilibrado entre segurança e rentabilidade",
            DataCalculo = dataCalculo
        };

       
        Assert.Equal(1, perfil.Id);
        Assert.Equal(123, perfil.ClienteId);
        Assert.Equal("Moderado", perfil.TipoPerfil);
        Assert.Equal(65, perfil.Pontuacao);
        Assert.Equal("Perfil equilibrado entre segurança e rentabilidade", perfil.Descricao);
        Assert.Equal(dataCalculo, perfil.DataCalculo);
    }

    [Theory]
    [InlineData("Conservador")]
    [InlineData("Moderado")]
    [InlineData("Agressivo")]
    public void PerfilRisco_WithDifferentTiposPerfil_ShouldStoreCorrectly(string tipoPerfil)
    {
        var perfil = new PerfilRisco
        {
            TipoPerfil = tipoPerfil
        };

        Assert.Equal(tipoPerfil, perfil.TipoPerfil);
    }

    [Theory]
    [InlineData(0)]     // Mínimo
    [InlineData(50)]    // Médio
    [InlineData(100)]   // Alto
    [InlineData(150)]   // Acima de 100
    [InlineData(-10)]   // Negativo
    [InlineData(999)]   // Valor muito alto
    public void PerfilRisco_WithAnyPontuacao_ShouldStoreCorrectly(int pontuacao)
    {
        var perfil = new PerfilRisco
        {
            Pontuacao = pontuacao
        };

        
        Assert.Equal(pontuacao, perfil.Pontuacao);
    }

    [Fact]
    public void PerfilRisco_WithEmptyDescription_ShouldStoreNull()
    {
        var perfil = new PerfilRisco
        {
            Descricao = null!
        };

        Assert.Null(perfil.Descricao);
    }

    [Fact]
    public void PerfilRisco_WithLongDescription_ShouldStoreCorrectly()
    {
        var descricaoLonga = "Este é um perfil de risco que descreve um investidor com tolerância moderada a riscos, buscando equilíbrio entre segurança e rentabilidade em sua carteira de investimentos.";

        var perfil = new PerfilRisco
        {
            Descricao = descricaoLonga
        };

        Assert.Equal(descricaoLonga, perfil.Descricao);
    }

    [Fact]
    public void PerfilRisco_DataCalculo_CanBeOverridden()
    {
        var dataEspecifica = new DateTime(2023, 12, 1, 14, 30, 0, DateTimeKind.Utc);

        
        var perfil = new PerfilRisco
        {
            DataCalculo = dataEspecifica
        };

      
        Assert.Equal(dataEspecifica, perfil.DataCalculo);
        Assert.NotEqual(DateTime.UtcNow, perfil.DataCalculo);
    }

    [Fact]
    public void PerfilRisco_CanStoreExtremePontuacaoValues()
    {
       
        var perfilMinimo = new PerfilRisco { Pontuacao = int.MinValue };
        var perfilMaximo = new PerfilRisco { Pontuacao = int.MaxValue };

       
        Assert.Equal(int.MinValue, perfilMinimo.Pontuacao);
        Assert.Equal(int.MaxValue, perfilMaximo.Pontuacao);
    }

    [Theory]
    [InlineData(25, "Conservador")]
    [InlineData(65, "Moderado")]
    [InlineData(85, "Agressivo")]
    [InlineData(-5, "Não Definido")] // Pontuação negativa com tipo customizado
    public void PerfilRisco_CanCombineAnyPontuacaoWithAnyTipoPerfil(int pontuacao, string tipoPerfil)
    {
        
        var perfil = new PerfilRisco
        {
            Pontuacao = pontuacao,
            TipoPerfil = tipoPerfil
        };

  
        Assert.Equal(pontuacao, perfil.Pontuacao);
        Assert.Equal(tipoPerfil, perfil.TipoPerfil);
    }
}