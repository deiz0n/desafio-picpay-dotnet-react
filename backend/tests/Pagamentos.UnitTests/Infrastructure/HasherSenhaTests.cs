using Pagamentos.Infrastructure.Seguranca;

namespace Pagamentos.UnitTests.Infrastructure;

public class HasherSenhaTests
{
    private readonly HasherSenha _hasher = new();

    [Fact]
    public void Verificar_SenhaCorretaEIncorreta_RetornaResultadoCerto()
    {
        var hash = _hasher.GerarHash("senha-forte");

        Assert.True(_hasher.Verificar("senha-forte", hash));
        Assert.False(_hasher.Verificar("senha-errada", hash));
    }

    [Fact]
    public void GerarHash_MesmaSenhaDuasVezes_GeraHashesDiferentes()
    {
        // O salt aleatório faz cada hash ser único, mesmo para senhas iguais.
        Assert.NotEqual(_hasher.GerarHash("senha-forte"), _hasher.GerarHash("senha-forte"));
    }

    [Fact]
    public void GerarHash_QualquerSenha_NaoContemASenha()
    {
        Assert.DoesNotContain("senha-forte", _hasher.GerarHash("senha-forte"));
    }
}
