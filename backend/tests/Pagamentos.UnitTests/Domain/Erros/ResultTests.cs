using Pagamentos.Domain.Erros;

namespace Pagamentos.UnitTests.Domain.Erros;

public class ResultTests
{
    private static readonly Erro ErroQualquer =
        new(TipoErro.Validacao, "validation_error", "Dados inválidos");

    [Fact]
    public void Ok_SemValor_EhSucessoESemErro()
    {
        var resultado = Result.Ok();

        Assert.True(resultado.Sucesso);
        Assert.Null(resultado.Erro);
    }

    [Fact]
    public void Ok_ComValor_ExpoeValor()
    {
        var resultado = Result.Ok(42);

        Assert.True(resultado.Sucesso);
        Assert.Equal(42, resultado.Valor);
    }

    [Fact]
    public void Falha_ComErro_NaoEhSucessoEExpoeErro()
    {
        var resultado = Result.Falha(ErroQualquer);

        Assert.False(resultado.Sucesso);
        Assert.Equal("validation_error", resultado.Erro!.Codigo);
        Assert.Equal(TipoErro.Validacao, resultado.Erro.Tipo);
    }

    [Fact]
    public void FalhaComValor_ConvertidaDeErro_NaoEhSucesso()
    {
        Result<int> resultado = ErroQualquer;

        Assert.False(resultado.Sucesso);
        Assert.Same(ErroQualquer, resultado.Erro);
    }

    [Fact]
    public void Valor_EmFalha_LancaInvalidOperationException()
    {
        Result<int> resultado = ErroQualquer;

        Assert.Throws<InvalidOperationException>(() => resultado.Valor);
    }

    [Fact]
    public void Erro_SemCampos_TemListaDeCamposVazia()
    {
        Assert.Empty(ErroQualquer.Campos);
    }
}
