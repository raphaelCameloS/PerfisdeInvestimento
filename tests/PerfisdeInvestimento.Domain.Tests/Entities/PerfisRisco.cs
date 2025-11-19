using PerfisdeInvestimento.Domain.Entities;
using Xunit;

namespace PerfisdeInvestimento.Domain.Tests.Entities;

public class PerfilRiscoTests
{
    [Fact]
    public void PerfilRisco_Constructor_ShouldSetDataCalculoToUtcNow()
    {
        // Arrange
        var antesCriacao = DateTime.UtcNow;

        // Act
        var perfil = new PerfilRisco();

        var depoisCriacao = DateTime.UtcNow;

        // Assert
        Assert.InRange(perfil.DataCalculo, antesCriacao, depoisCriacao);
        Assert.Equal(DateTimeKind.Utc, perfil.DataCalculo.Kind);
    }

    [Fact]
    public void PerfilRisco_WithValidData_ShouldCreateSuccessfully()
    {
        // Arrange
        var dataCalculo = new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Utc);

        // Act
        var perfil = new PerfilRisco
        {
            Id = 1,
            ClienteId = 123,
            TipoPerfil = "Moderado",
            Pontuacao = 65,
            Descricao = "Perfil equilibrado entre segurança e rentabilidade",
            DataCalculo = dataCalculo
        };

        // Assert
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
    [InlineData("Arrojado")] // Outro tipo possível
    [InlineData("Ultra Conservador")] // Outro tipo possível
    public void PerfilRisco_WithDifferentTiposPerfil_ShouldStoreCorrectly(string tipoPerfil)
    {
        // Arrange & Act
        var perfil = new PerfilRisco
        {
            TipoPerfil = tipoPerfil
        };

        // Assert
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
        // Arrange & Act
        var perfil = new PerfilRisco
        {
            Pontuacao = pontuacao
        };

        // Assert - A entidade aceita QUALQUER valor inteiro
        Assert.Equal(pontuacao, perfil.Pontuacao);
    }

    [Fact]
    public void PerfilRisco_WithEmptyDescription_ShouldStoreNull()
    {
        // Arrange & Act
        var perfil = new PerfilRisco
        {
            Descricao = null!
        };

        // Assert
        Assert.Null(perfil.Descricao);
    }

    [Fact]
    public void PerfilRisco_WithLongDescription_ShouldStoreCorrectly()
    {
        // Arrange
        var descricaoLonga = "Este é um perfil de risco que descreve um investidor com tolerância moderada a riscos, buscando equilíbrio entre segurança e rentabilidade em sua carteira de investimentos.";

        // Act
        var perfil = new PerfilRisco
        {
            Descricao = descricaoLonga
        };

        // Assert
        Assert.Equal(descricaoLonga, perfil.Descricao);
    }

    [Fact]
    public void PerfilRisco_DataCalculo_CanBeOverridden()
    {
        // Arrange
        var dataEspecifica = new DateTime(2023, 12, 1, 14, 30, 0, DateTimeKind.Utc);

        // Act
        var perfil = new PerfilRisco
        {
            DataCalculo = dataEspecifica
        };

        // Assert
        Assert.Equal(dataEspecifica, perfil.DataCalculo);
        Assert.NotEqual(DateTime.UtcNow, perfil.DataCalculo);
    }

    [Fact]
    public void PerfilRisco_CanStoreExtremePontuacaoValues()
    {
        // Arrange & Act - Testa valores extremos
        var perfilMinimo = new PerfilRisco { Pontuacao = int.MinValue };
        var perfilMaximo = new PerfilRisco { Pontuacao = int.MaxValue };

        // Assert - Documenta que aceita QUALQUER inteiro
        Assert.Equal(int.MinValue, perfilMinimo.Pontuacao);
        Assert.Equal(int.MaxValue, perfilMaximo.Pontuacao);
    }

    [Theory]
    [InlineData(25, "Conservador")]
    [InlineData(65, "Moderado")]
    [InlineData(85, "Agressivo")]
    [InlineData(120, "Ultra Agressivo")] // Pontuação acima de 100 com tipo customizado
    [InlineData(-5, "Não Definido")] // Pontuação negativa com tipo customizado
    public void PerfilRisco_CanCombineAnyPontuacaoWithAnyTipoPerfil(int pontuacao, string tipoPerfil)
    {
        // Arrange & Act
        var perfil = new PerfilRisco
        {
            Pontuacao = pontuacao,
            TipoPerfil = tipoPerfil
        };

        // Assert - A entidade permite QUALQUER combinação
        Assert.Equal(pontuacao, perfil.Pontuacao);
        Assert.Equal(tipoPerfil, perfil.TipoPerfil);
    }
}