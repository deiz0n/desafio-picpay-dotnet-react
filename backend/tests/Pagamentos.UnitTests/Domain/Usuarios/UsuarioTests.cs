using Pagamentos.Domain.Erros;
using Pagamentos.Domain.Usuarios.Entidades;
using Pagamentos.UnitTests.Fakes;

namespace Pagamentos.UnitTests.Domain.Usuarios;

public class UsuarioTests
{
    private const string CpfValido = "52998224725";
    private const string CnpjValido = "11222333000181";
    private static readonly HasherSenhaFalso Hasher = new();

    private static Result<Usuario> Criar(
        string nome = "Maria da Silva",
        string email = "maria@exemplo.com",
        string documento = CpfValido,
        TipoUsuario? tipo = TipoUsuario.Comum,
        string senha = "senha-forte") =>
        Usuario.Criar(nome, email, documento, tipo, senha, Hasher);

    private static void AssertFalhaNoCampo(Result<Usuario> resultado, string campo)
    {
        Assert.False(resultado.Sucesso);
        Assert.Equal(TipoErro.Validacao, resultado.Erro!.Tipo);
        Assert.Equal("validation_error", resultado.Erro.Codigo);
        Assert.Contains(resultado.Erro.Campos, c => c.Campo == campo);
    }

    [Fact]
    public void Criar_ComumComCpfValido_RetornaUsuario()
    {
        var resultado = Criar();

        Assert.True(resultado.Sucesso);
        var usuario = resultado.Valor;
        Assert.Equal("Maria da Silva", usuario.Nome);
        Assert.Equal("maria@exemplo.com", usuario.Email);
        Assert.Equal(CpfValido, usuario.Documento);
        Assert.Equal(TipoUsuario.Comum, usuario.Tipo);
    }

    [Fact]
    public void Criar_LojistaComCnpjValido_RetornaUsuario()
    {
        var resultado = Criar(documento: CnpjValido, tipo: TipoUsuario.Lojista);

        Assert.True(resultado.Sucesso);
        Assert.Equal(TipoUsuario.Lojista, resultado.Valor.Tipo);
        Assert.Equal(CnpjValido, resultado.Valor.Documento);
    }

    [Theory]
    [InlineData(CnpjValido, TipoUsuario.Comum)]
    [InlineData(CpfValido, TipoUsuario.Lojista)]
    public void Criar_DocumentoIncompativelComTipo_RetornaFalhaNoCampoDocumento(string documento, TipoUsuario tipo)
    {
        AssertFalhaNoCampo(Criar(documento: documento, tipo: tipo), "documento");
    }

    [Theory]
    [InlineData("52998224724")]    // dígito verificador errado
    [InlineData("11111111111")]    // todos os dígitos iguais
    [InlineData("529.982.247-25")] // formatado: a API só aceita dígitos
    [InlineData("5299822472")]     // tamanho errado
    [InlineData("")]
    public void Criar_CpfInvalido_RetornaFalha(string cpf)
    {
        AssertFalhaNoCampo(Criar(documento: cpf, tipo: TipoUsuario.Comum), "documento");
    }

    [Theory]
    [InlineData("11222333000182")]     // dígito verificador errado
    [InlineData("00000000000000")]     // todos os dígitos iguais
    [InlineData("11.222.333/0001-81")] // formatado
    [InlineData("1122233300018")]      // tamanho errado
    public void Criar_CnpjInvalido_RetornaFalha(string cnpj)
    {
        AssertFalhaNoCampo(Criar(documento: cnpj, tipo: TipoUsuario.Lojista), "documento");
    }

    public static TheoryData<string, string, string, TipoUsuario?, string> CamposInvalidos => new()
    {
        { "nome", "   ", "maria@exemplo.com", TipoUsuario.Comum, "senha-forte" },
        { "nome", new string('a', 101), "maria@exemplo.com", TipoUsuario.Comum, "senha-forte" },
        { "email", "Maria", "", TipoUsuario.Comum, "senha-forte" },
        { "email", "Maria", "maria.exemplo.com", TipoUsuario.Comum, "senha-forte" },
        { "email", "Maria", "maria@exemplo", TipoUsuario.Comum, "senha-forte" },
        { "email", "Maria", new string('a', 250) + "@x.com", TipoUsuario.Comum, "senha-forte" },
        { "senha", "Maria", "maria@exemplo.com", TipoUsuario.Comum, "1234567" },
        { "senha", "Maria", "maria@exemplo.com", TipoUsuario.Comum, new string('a', 101) },
        { "tipo", "Maria", "maria@exemplo.com", null, "senha-forte" },
    };

    [Theory]
    [MemberData(nameof(CamposInvalidos))]
    public void Criar_CampoInvalido_RetornaFalhaNoCampo(
        string campo, string nome, string email, TipoUsuario? tipo, string senha)
    {
        var resultado = Usuario.Criar(nome, email, CpfValido, tipo, senha, Hasher);

        AssertFalhaNoCampo(resultado, campo);
        Assert.Single(resultado.Erro!.Campos);
    }

    [Fact]
    public void Criar_VariosCamposInvalidos_RetornaTodosOsErros()
    {
        var resultado = Usuario.Criar("", "invalido", "123", TipoUsuario.Comum, "curta", Hasher);

        Assert.False(resultado.Sucesso);
        var campos = resultado.Erro!.Campos.Select(c => c.Campo).ToList();
        Assert.Equal(["nome", "email", "documento", "senha"], campos);
    }

    [Fact]
    public void Criar_EmailComMaiusculasEEspacos_NormalizaEmail()
    {
        var resultado = Criar(email: "  Maria@Exemplo.COM ");

        Assert.Equal("maria@exemplo.com", resultado.Valor.Email);
    }

    [Fact]
    public void Criar_NomeComEspacosNasPontas_RemoveEspacos()
    {
        var resultado = Criar(nome: "  Maria da Silva  ");

        Assert.Equal("Maria da Silva", resultado.Valor.Nome);
    }

    [Fact]
    public void Criar_DadosValidos_GuardaHashENaoASenha()
    {
        var resultado = Criar(senha: "senha-forte");

        Assert.Equal("hash:senha-forte", resultado.Valor.HashSenha);
    }

    [Fact]
    public void AtribuirId_SegundaVez_LancaInvalidOperationException()
    {
        var usuario = Criar().Valor;
        usuario.AtribuirId(1);

        Assert.Equal(1, usuario.Id);
        Assert.Throws<InvalidOperationException>(() => usuario.AtribuirId(2));
    }
}
