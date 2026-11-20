using Pagamentos.Application.Usuarios.CadastrarUsuario;
using Pagamentos.Domain.Erros;
using Pagamentos.Domain.Usuarios;
using Pagamentos.Domain.Usuarios.Entidades;
using Pagamentos.UnitTests.Fakes;

namespace Pagamentos.UnitTests.Application.Usuarios;

public class CadastrarUsuarioHandlerTests
{
    private readonly UsuarioRepositorioFalso _usuarios = new();
    private readonly CarteiraRepositorioFalso _carteiras = new();
    private readonly UnidadeDeTrabalhoFalsa _unidadeDeTrabalho = new();
    private readonly CadastrarUsuarioHandler _handler;

    public CadastrarUsuarioHandlerTests()
    {
        _handler = new CadastrarUsuarioHandler(_usuarios, _carteiras, _unidadeDeTrabalho, new HasherSenhaFalso());
    }

    private static CadastrarUsuarioCommand Comando(string email = "maria@exemplo.com", string senha = "senha-forte") =>
        new("Maria da Silva", email, "52998224725", TipoUsuario.Comum, senha);

    [Fact]
    public async Task ExecutarAsync_DadosValidos_CriaUsuarioECarteiraEConfirma()
    {
        var resultado = await _handler.ExecutarAsync(Comando());

        Assert.True(resultado.Sucesso);
        Assert.Equal(1, resultado.Valor.Id);
        Assert.Equal("maria@exemplo.com", resultado.Valor.Email);
        Assert.Equal(TipoUsuario.Comum, resultado.Valor.Tipo);

        var usuario = Assert.Single(_usuarios.Adicionados);
        var carteira = Assert.Single(_carteiras.Adicionadas);
        Assert.Equal(usuario.Id, carteira.UsuarioId);
        Assert.Equal(1000.00m, carteira.Saldo);

        Assert.True(_unidadeDeTrabalho.Iniciada);
        Assert.True(_unidadeDeTrabalho.Confirmada);
        Assert.False(_unidadeDeTrabalho.Desfeita);
    }

    [Fact]
    public async Task ExecutarAsync_DadosValidos_GuardaHashENaoASenha()
    {
        await _handler.ExecutarAsync(Comando(senha: "senha-forte"));

        Assert.Equal("hash:senha-forte", Assert.Single(_usuarios.Adicionados).HashSenha);
    }

    [Fact]
    public async Task ExecutarAsync_DadosInvalidos_RetornaValidationErrorSemAbrirTransacao()
    {
        var resultado = await _handler.ExecutarAsync(Comando(email: "invalido"));

        Assert.Equal("validation_error", resultado.Erro!.Codigo);
        Assert.Empty(_usuarios.Adicionados);
        Assert.False(_unidadeDeTrabalho.Iniciada);
    }

    [Fact]
    public async Task ExecutarAsync_EmailJaCadastrado_RetornaEmailTakenSemCriar()
    {
        _usuarios.EmailsExistentes.Add("maria@exemplo.com");

        // E-mail com outra caixa: a comparação é feita depois da normalização.
        var resultado = await _handler.ExecutarAsync(Comando(email: "Maria@Exemplo.com"));

        Assert.Same(ErrosUsuario.EmailEmUso, resultado.Erro);
        Assert.Empty(_usuarios.Adicionados);
        Assert.Empty(_carteiras.Adicionadas);
        Assert.False(_unidadeDeTrabalho.Iniciada);
    }

    [Fact]
    public async Task ExecutarAsync_DocumentoJaCadastrado_RetornaDocumentTakenSemCriar()
    {
        _usuarios.DocumentosExistentes.Add("52998224725");

        var resultado = await _handler.ExecutarAsync(Comando());

        Assert.Same(ErrosUsuario.DocumentoEmUso, resultado.Erro);
        Assert.Empty(_usuarios.Adicionados);
        Assert.False(_unidadeDeTrabalho.Iniciada);
    }

    [Fact]
    public async Task ExecutarAsync_ViolacaoDeUnicidadeAoAdicionar_DesfazERetornaConflito()
    {
        // Corrida: a checagem prévia passou, mas outro cadastro gravou o mesmo e-mail antes.
        _usuarios.ErroAoAdicionar = ErrosUsuario.EmailEmUso;

        var resultado = await _handler.ExecutarAsync(Comando());

        Assert.Equal(TipoErro.Conflito, resultado.Erro!.Tipo);
        Assert.Empty(_carteiras.Adicionadas);
        Assert.True(_unidadeDeTrabalho.Desfeita);
        Assert.False(_unidadeDeTrabalho.Confirmada);
    }

    [Fact]
    public async Task ExecutarAsync_FalhaAoAbrirCarteira_DesfazTransacaoERelancaExcecao()
    {
        _carteiras.Falhar = true;

        await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.ExecutarAsync(Comando()));

        Assert.True(_unidadeDeTrabalho.Desfeita);
        Assert.False(_unidadeDeTrabalho.Confirmada);
    }
}
