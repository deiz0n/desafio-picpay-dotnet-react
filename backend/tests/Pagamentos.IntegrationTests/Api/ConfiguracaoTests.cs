using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Pagamentos.IntegrationTests.Api;

public class ConfiguracaoTests
{
    [Fact]
    public void Subida_SemConnectionString_FalhaComMensagemClara()
    {
        using var api = new WebApplicationFactory<Program>().WithWebHostBuilder(b =>
        {
            b.UseEnvironment("Testing");
            b.UseSetting("ConnectionStrings:Pagamentos", "");
        });

        var erro = Assert.Throws<InvalidOperationException>(() => api.CreateClient());

        Assert.Contains("ConnectionStrings:Pagamentos", erro.Message);
    }
}
