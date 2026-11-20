using Dapper;
using Npgsql;
using Testcontainers.PostgreSql;

namespace Pagamentos.IntegrationTests.Infra;

/// <summary>
/// Sobe um Postgres 17 em container e aplica as migrations de <c>database/migrations/</c>, em ordem.
/// Compartilhado pela coleção "Postgres": as classes da coleção rodam em série, e cada teste
/// limpa as tabelas antes de começar (<see cref="LimparAsync"/>).
/// </summary>
public sealed class PostgresFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder("postgres:17-alpine").Build();

    public string ConnectionString => _container.GetConnectionString();

    public NpgsqlDataSource DataSource { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        DataSource = NpgsqlDataSource.Create(ConnectionString);

        var pasta = Path.Combine(AppContext.BaseDirectory, "migrations");
        await using var conexao = await DataSource.OpenConnectionAsync();
        foreach (var arquivo in Directory.GetFiles(pasta, "*.sql").Order(StringComparer.Ordinal))
            await conexao.ExecuteAsync(await File.ReadAllTextAsync(arquivo));
    }

    /// <summary>Apaga todos os dados e reinicia as sequências de id.</summary>
    public async Task LimparAsync()
    {
        await using var conexao = await DataSource.OpenConnectionAsync();
        await conexao.ExecuteAsync("TRUNCATE carteiras, usuarios RESTART IDENTITY CASCADE");
    }

    public async Task DisposeAsync()
    {
        await DataSource.DisposeAsync();
        await _container.DisposeAsync();
    }
}

[CollectionDefinition(Nome)]
public sealed class PostgresCollection : ICollectionFixture<PostgresFixture>
{
    public const string Nome = "Postgres";
}
