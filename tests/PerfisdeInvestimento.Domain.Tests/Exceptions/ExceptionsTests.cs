using PerfisdeInvestimento.Domain.Exceptions;
using Xunit;

namespace PerfisdeInvestimento.Domain.Tests.Exceptions;

public class DomainExceptionsTests
{
    [Fact]
    public void NotFoundException_ShouldCreateWithMessage()
    {
        // Arrange
        var mensagem = "Cliente não encontrado";

        // Act
        var exception = new NotFoundException(mensagem);

        // Assert
        Assert.Equal(mensagem, exception.Message);
        Assert.Null(exception.InnerException);
    }

    [Fact]
    public void NotFoundException_ShouldBeOfCorrectType()
    {
        // Arrange & Act
        var exception = new NotFoundException("Teste");

        // Assert
        Assert.IsType<NotFoundException>(exception);
        Assert.IsAssignableFrom<Exception>(exception);
    }

    [Fact]
    public void BusinessException_ShouldCreateWithMessage()
    {
        // Arrange
        var mensagem = "Erro de negócio na simulação";

        // Act
        var exception = new BusinessException(mensagem);

        // Assert
        Assert.Equal(mensagem, exception.Message);
        Assert.Null(exception.InnerException);
    }

    [Fact]
    public void BusinessException_ShouldBeOfCorrectType()
    {
        // Arrange & Act
        var exception = new BusinessException("Teste");

        // Assert
        Assert.IsType<BusinessException>(exception);
        Assert.IsAssignableFrom<Exception>(exception);
    }

    [Fact]
    public void ValidationException_ShouldCreateWithMessage()
    {
        // Arrange
        var mensagem = "Dados de entrada inválidos";

        // Act
        var exception = new ValidationException(mensagem);

        // Assert
        Assert.Equal(mensagem, exception.Message);
        Assert.Null(exception.InnerException);
    }

    [Fact]
    public void ValidationException_ShouldBeOfCorrectType()
    {
        // Arrange & Act
        var exception = new ValidationException("Teste");

        // Assert
        Assert.IsType<ValidationException>(exception);
        Assert.IsAssignableFrom<Exception>(exception);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("Mensagem de erro detalhada")]
    [InlineData("Erro: Cliente ID 123 não possui histórico de investimentos")]
    public void NotFoundException_WithDifferentMessages_ShouldStoreCorrectly(string mensagem)
    {
        // Arrange & Act
        var exception = new NotFoundException(mensagem);

        // Assert
        Assert.Equal(mensagem, exception.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("Regra de negócio violada")]
    [InlineData("Erro ao calcular perfil do cliente: Dados inconsistentes")]
    public void BusinessException_WithDifferentMessages_ShouldStoreCorrectly(string mensagem)
    {
        // Arrange & Act
        var exception = new BusinessException(mensagem);

        // Assert
        Assert.Equal(mensagem, exception.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("Valor deve ser maior que zero")]
    [InlineData("Prazo em meses deve ser um número positivo")]
    public void ValidationException_WithDifferentMessages_ShouldStoreCorrectly(string mensagem)
    {
        // Arrange & Act
        var exception = new ValidationException(mensagem);

        // Assert
        Assert.Equal(mensagem, exception.Message);
    }

    [Fact]
    public void Exceptions_ShouldBeUsedInAppropriateScenarios()
    {
        // Arrange
        var notFoundMessage = "Produto com ID 999 não encontrado";
        var businessMessage = "Cliente não elegível para este tipo de investimento";
        var validationMessage = "Valor do investimento não pode ser negativo";

        // Act
        var notFoundEx = new NotFoundException(notFoundMessage);
        var businessEx = new BusinessException(businessMessage);
        var validationEx = new ValidationException(validationMessage);

        // Assert - Verifica que cada exception tem uma mensagem específica do domínio
        Assert.Contains("não encontrado", notFoundEx.Message);
        Assert.Contains("não elegível", businessEx.Message);
        Assert.Contains("não pode ser negativo", validationEx.Message);
    }

    [Fact]
    public void Exceptions_ShouldHaveDifferentTypesForDifferentPurposes()
    {
        // Arrange & Act
        var notFoundEx = new NotFoundException("Não encontrado");
        var businessEx = new BusinessException("Erro de negócio");
        var validationEx = new ValidationException("Validação falhou");

        // Assert - Cada exception tem um tipo distinto para diferentes cenários
        Assert.IsType<NotFoundException>(notFoundEx);
        Assert.IsType<BusinessException>(businessEx);
        Assert.IsType<ValidationException>(validationEx);

        Assert.NotEqual(notFoundEx.GetType(), businessEx.GetType());
        Assert.NotEqual(businessEx.GetType(), validationEx.GetType());
        Assert.NotEqual(validationEx.GetType(), notFoundEx.GetType());
    }

  
    [Fact]
    public void Exceptions_CanBeUsedInTryCatchBlocks()
    {
        // Arrange
        var mensagem = "Erro específico do domínio";

        // Act & Assert - Testa o uso prático em blocos try-catch
        try
        {
            throw new BusinessException(mensagem);
        }
        catch (BusinessException ex)
        {
            Assert.Equal(mensagem, ex.Message);
        }

        try
        {
            throw new NotFoundException(mensagem);
        }
        catch (NotFoundException ex)
        {
            Assert.Equal(mensagem, ex.Message);
        }

        try
        {
            throw new ValidationException(mensagem);
        }
        catch (ValidationException ex)
        {
            Assert.Equal(mensagem, ex.Message);
        }
    }
}