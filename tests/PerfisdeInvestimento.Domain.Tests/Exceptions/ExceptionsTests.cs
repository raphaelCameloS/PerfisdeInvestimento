using PerfisdeInvestimento.Domain.Exceptions;
using Xunit;

namespace PerfisdeInvestimento.Domain.Tests.Exceptions;

public class DomainExceptionsTests
{
    [Fact]
    public void NotFoundException_ShouldCreateWithMessage()
    {
        var mensagem = "Cliente não encontrado";

        
        var exception = new NotFoundException(mensagem);

      
        Assert.Equal(mensagem, exception.Message);
        Assert.Null(exception.InnerException);
    }

    [Fact]
    public void NotFoundException_ShouldBeOfCorrectType()
    {
        var exception = new NotFoundException("Teste");

       
        Assert.IsType<NotFoundException>(exception);
        Assert.IsAssignableFrom<Exception>(exception);
    }

    [Fact]
    public void BusinessException_ShouldCreateWithMessage()
    {
       
        var mensagem = "Erro de negócio na simulação";

       
        var exception = new BusinessException(mensagem);

        
        Assert.Equal(mensagem, exception.Message);
        Assert.Null(exception.InnerException);
    }

    [Fact]
    public void BusinessException_ShouldBeOfCorrectType()
    {
        
        var exception = new BusinessException("Teste");

        
        Assert.IsType<BusinessException>(exception);
        Assert.IsAssignableFrom<Exception>(exception);
    }

    [Fact]
    public void ValidationException_ShouldCreateWithMessage()
    {
       
        var mensagem = "Dados de entrada inválidos";

        
        var exception = new ValidationException(mensagem);

        
        Assert.Equal(mensagem, exception.Message);
        Assert.Null(exception.InnerException);
    }

    [Fact]
    public void ValidationException_ShouldBeOfCorrectType()
    {
        
        var exception = new ValidationException("Teste");

      
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
        
        var exception = new NotFoundException(mensagem);

        Assert.Equal(mensagem, exception.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("Regra de negócio violada")]
    [InlineData("Erro ao calcular perfil do cliente: Dados inconsistentes")]
    public void BusinessException_WithDifferentMessages_ShouldStoreCorrectly(string mensagem)
    {
     
        var exception = new BusinessException(mensagem);

       
        Assert.Equal(mensagem, exception.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("Valor deve ser maior que zero")]
    [InlineData("Prazo em meses deve ser um número positivo")]
    public void ValidationException_WithDifferentMessages_ShouldStoreCorrectly(string mensagem)
    {
        
        var exception = new ValidationException(mensagem);

        
        Assert.Equal(mensagem, exception.Message);
    }

    [Fact]
    public void Exceptions_ShouldBeUsedInAppropriateScenarios()
    {
        
        var notFoundMessage = "Produto com ID 999 não encontrado";
        var businessMessage = "Cliente não elegível para este tipo de investimento";
        var validationMessage = "Valor do investimento não pode ser negativo";

       
        var notFoundEx = new NotFoundException(notFoundMessage);
        var businessEx = new BusinessException(businessMessage);
        var validationEx = new ValidationException(validationMessage);

        
        Assert.Contains("não encontrado", notFoundEx.Message);
        Assert.Contains("não elegível", businessEx.Message);
        Assert.Contains("não pode ser negativo", validationEx.Message);
    }

    [Fact]
    public void Exceptions_ShouldHaveDifferentTypesForDifferentPurposes()
    {
        var notFoundEx = new NotFoundException("Não encontrado");
        var businessEx = new BusinessException("Erro de negócio");
        var validationEx = new ValidationException("Validação falhou");

        
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
       
        var mensagem = "Erro específico do domínio";

   
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