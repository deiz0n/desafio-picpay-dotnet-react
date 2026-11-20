using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Pagamentos.IntegrationTests.Infra;

/// <summary>Sobe a API em memória apontando para o Postgres do container de teste.</summary>
public sealed class ApiFactory(PostgresFixture db) : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.UseSetting("ConnectionStrings:Pagamentos", db.ConnectionString);
    }
}
