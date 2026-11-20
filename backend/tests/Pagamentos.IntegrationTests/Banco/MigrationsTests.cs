using Dapper;
using Npgsql;
using Pagamentos.IntegrationTests.Infra;

namespace Pagamentos.IntegrationTests.Banco;

/// <summary>As constraints são a última linha de defesa: valem mesmo se o código falhar.</summary>
[Collection(PostgresCollection.Nome)]
public class MigrationsTests(PostgresFixture db) : IAsyncLifetime
{
    private const string InserirMaria = """
        INSERT INTO usuarios (nome, email, documento, tipo, hash_senha)
        VALUES ('Maria', 'maria@exemplo.com', '52998224725', 'comum', 'hash')
        RETURNING id
        """;

    public Task InitializeAsync() => db.LimparAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    private async Task<PostgresException> ExecutarEsperandoErro(string sql)
    {
        await using var conexao = await db.DataSource.OpenConnectionAsync();
        await conexao.ExecuteAsync(InserirMaria);
        return await Assert.ThrowsAsync<PostgresException>(() => conexao.ExecuteAsync(sql));
    }

    [Fact]
    public async Task InserirEmailDuplicado_ViolaUnique()
    {
        var erro = await ExecutarEsperandoErro("""
            INSERT INTO usuarios (nome, email, documento, tipo, hash_senha)
            VALUES ('Outra', 'maria@exemplo.com', '11144477735', 'comum', 'hash')
            """);

        Assert.Equal(PostgresErrorCodes.UniqueViolation, erro.SqlState);
        Assert.Equal("usuarios_email_key", erro.ConstraintName);
    }

    [Fact]
    public async Task InserirDocumentoDuplicado_ViolaUnique()
    {
        var erro = await ExecutarEsperandoErro("""
            INSERT INTO usuarios (nome, email, documento, tipo, hash_senha)
            VALUES ('Outra', 'outra@exemplo.com', '52998224725', 'comum', 'hash')
            """);

        Assert.Equal(PostgresErrorCodes.UniqueViolation, erro.SqlState);
        Assert.Equal("usuarios_documento_key", erro.ConstraintName);
    }

    [Theory]
    [InlineData("'admin'", "'11144477735'")]         // tipo fora da lista (ADR 0016)
    [InlineData("'comum'", "'11222333000181'")]      // comum com 14 dígitos
    [InlineData("'lojista'", "'11144477735'")]       // lojista com 11 dígitos
    [InlineData("'comum'", "'1114447773a'")]         // documento com letra
    public async Task InserirTipoOuDocumentoInvalido_ViolaCheck(string tipo, string documento)
    {
        var erro = await ExecutarEsperandoErro($"""
            INSERT INTO usuarios (nome, email, documento, tipo, hash_senha)
            VALUES ('Outra', 'outra@exemplo.com', {documento}, {tipo}, 'hash')
            """);

        Assert.Equal(PostgresErrorCodes.CheckViolation, erro.SqlState);
    }

    [Fact]
    public async Task InserirCarteiraComSaldoNegativo_ViolaCheck()
    {
        var erro = await ExecutarEsperandoErro(
            "INSERT INTO carteiras (usuario_id, saldo) VALUES (1, -0.01)");

        Assert.Equal(PostgresErrorCodes.CheckViolation, erro.SqlState);
    }

    [Fact]
    public async Task InserirSegundaCarteiraParaOMesmoUsuario_ViolaUnique()
    {
        await using var conexao = await db.DataSource.OpenConnectionAsync();
        var id = await conexao.ExecuteScalarAsync<long>(InserirMaria);
        await conexao.ExecuteAsync("INSERT INTO carteiras (usuario_id, saldo) VALUES (@id, 0)", new { id });

        var erro = await Assert.ThrowsAsync<PostgresException>(() =>
            conexao.ExecuteAsync("INSERT INTO carteiras (usuario_id, saldo) VALUES (@id, 0)", new { id }));

        Assert.Equal(PostgresErrorCodes.UniqueViolation, erro.SqlState);
    }

    [Fact]
    public async Task InserirCarteiraDeUsuarioInexistente_ViolaChaveEstrangeira()
    {
        var erro = await ExecutarEsperandoErro(
            "INSERT INTO carteiras (usuario_id, saldo) VALUES (999, 0)");

        Assert.Equal(PostgresErrorCodes.ForeignKeyViolation, erro.SqlState);
    }
}
