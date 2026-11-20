using Dapper;
using Pagamentos.Domain.Usuarios;
using Pagamentos.Domain.Usuarios.Entidades;
using Pagamentos.Infrastructure.Data;
using Pagamentos.Infrastructure.Seguranca;
using Pagamentos.IntegrationTests.Infra;
using Pagamentos.Repository.Usuarios;

namespace Pagamentos.IntegrationTests.Banco;

/// <summary>Repositórios + unidade de trabalho contra o Postgres real, conferindo o banco com SQL direto.</summary>
[Collection(PostgresCollection.Nome)]
public class RepositoriosTests(PostgresFixture db) : IAsyncLifetime
{
    public Task InitializeAsync() => db.LimparAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    private static Usuario NovoUsuario(string email = "maria@exemplo.com", string documento = "52998224725") =>
        Usuario.Criar("Maria da Silva", email, documento, TipoUsuario.Comum, "senha-forte", new HasherSenha()).Valor;

    private async Task<long> ContarAsync(string tabela)
    {
        await using var conexao = await db.DataSource.OpenConnectionAsync();
        return await conexao.ExecuteScalarAsync<long>($"SELECT COUNT(*) FROM {tabela}");
    }

    [Fact]
    public async Task Confirmar_AposInserirUsuarioECarteira_GravaAmbos()
    {
        await using var unidade = new UnidadeDeTrabalho(db.DataSource);
        var usuarios = new UsuarioRepositorio(unidade);
        var carteiras = new CarteiraRepositorio(unidade);
        var usuario = NovoUsuario();

        await unidade.IniciarAsync();
        var adicao = await usuarios.AdicionarAsync(usuario);
        await carteiras.AdicionarAsync(Carteira.Abrir(usuario.Id));
        await unidade.ConfirmarAsync();

        Assert.True(adicao.Sucesso);
        Assert.True(usuario.Id > 0);

        await using var conexao = await db.DataSource.OpenConnectionAsync();
        var linha = await conexao.QuerySingleAsync<(string Email, string Tipo, string HashSenha, decimal Saldo)>("""
            SELECT u.email, u.tipo, u.hash_senha, c.saldo
            FROM usuarios u JOIN carteiras c ON c.usuario_id = u.id
            WHERE u.id = @Id
            """, new { usuario.Id });
        Assert.Equal("maria@exemplo.com", linha.Email);
        Assert.Equal("comum", linha.Tipo);
        Assert.Equal(1000.00m, linha.Saldo);
        Assert.NotEqual("senha-forte", linha.HashSenha);
    }

    [Fact]
    public async Task Desfazer_AposInserirUsuario_NaoGravaNada()
    {
        await using var unidade = new UnidadeDeTrabalho(db.DataSource);
        var usuarios = new UsuarioRepositorio(unidade);

        await unidade.IniciarAsync();
        await usuarios.AdicionarAsync(NovoUsuario());
        await unidade.DesfazerAsync();

        Assert.Equal(0, await ContarAsync("usuarios"));
    }

    [Fact]
    public async Task Dispose_SemConfirmar_NaoGravaNada()
    {
        await using (var unidade = new UnidadeDeTrabalho(db.DataSource))
        {
            await unidade.IniciarAsync();
            await new UsuarioRepositorio(unidade).AdicionarAsync(NovoUsuario());
        }

        Assert.Equal(0, await ContarAsync("usuarios"));
    }

    [Fact]
    public async Task ExisteAsync_EmailEDocumentoGravados_RetornaVerdadeiro()
    {
        await using (var unidade = new UnidadeDeTrabalho(db.DataSource))
        {
            await unidade.IniciarAsync();
            await new UsuarioRepositorio(unidade).AdicionarAsync(NovoUsuario());
            await unidade.ConfirmarAsync();
        }

        await using var leitura = new UnidadeDeTrabalho(db.DataSource);
        var repositorio = new UsuarioRepositorio(leitura);
        Assert.True(await repositorio.EmailExisteAsync("maria@exemplo.com"));
        Assert.True(await repositorio.DocumentoExisteAsync("52998224725"));
        Assert.False(await repositorio.EmailExisteAsync("outra@exemplo.com"));
        Assert.False(await repositorio.DocumentoExisteAsync("11144477735"));
    }

    [Theory]
    [InlineData("maria@exemplo.com", "11144477735", "email_taken")]
    [InlineData("outra@exemplo.com", "52998224725", "document_taken")]
    public async Task AdicionarAsync_ViolacaoDeUnique_RetornaConflito(string email, string documento, string codigo)
    {
        await using (var unidade = new UnidadeDeTrabalho(db.DataSource))
        {
            await unidade.IniciarAsync();
            await new UsuarioRepositorio(unidade).AdicionarAsync(NovoUsuario());
            await unidade.ConfirmarAsync();
        }

        await using var segunda = new UnidadeDeTrabalho(db.DataSource);
        await segunda.IniciarAsync();
        var resultado = await new UsuarioRepositorio(segunda).AdicionarAsync(NovoUsuario(email, documento));
        await segunda.DesfazerAsync();

        Assert.False(resultado.Sucesso);
        Assert.Equal(codigo, resultado.Erro!.Codigo);
        Assert.Same(codigo == "email_taken" ? ErrosUsuario.EmailEmUso : ErrosUsuario.DocumentoEmUso, resultado.Erro);
        Assert.Equal(1, await ContarAsync("usuarios"));
    }
}
