using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Dapper;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Pagamentos.Domain.Usuarios.Entidades;
using Pagamentos.Domain.Usuarios.Interfaces;
using Pagamentos.Infrastructure.Seguranca;
using Pagamentos.IntegrationTests.Infra;

namespace Pagamentos.IntegrationTests.Api;

/// <summary><c>POST /users</c> de ponta a ponta, conferindo o banco com SQL direto.</summary>
[Collection(PostgresCollection.Nome)]
public class UsuariosEndpointTests(PostgresFixture db) : IAsyncLifetime
{
    private const string CpfValido = "52998224725";
    private const string OutroCpfValido = "11144477735";
    private const string CnpjValido = "11222333000181";

    private readonly ApiFactory _api = new(db);

    public Task InitializeAsync() => db.LimparAsync();

    public async Task DisposeAsync() => await _api.DisposeAsync();

    private static object Corpo(
        string name = "Maria da Silva",
        string document = CpfValido,
        string email = "maria@exemplo.com",
        string password = "senha-forte",
        string type = "common") =>
        new { name, document, email, password, type };

    private async Task<(long Usuarios, long Carteiras)> ContarAsync()
    {
        await using var conexao = await db.DataSource.OpenConnectionAsync();
        return (await conexao.ExecuteScalarAsync<long>("SELECT COUNT(*) FROM usuarios"),
                await conexao.ExecuteScalarAsync<long>("SELECT COUNT(*) FROM carteiras"));
    }

    private static async Task<JsonElement> LerProblemaAsync(HttpResponseMessage resposta, HttpStatusCode status, string codigo)
    {
        Assert.Equal(status, resposta.StatusCode);
        Assert.Equal("application/problem+json", resposta.Content.Headers.ContentType?.MediaType);

        var problema = await resposta.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal((int)status, problema.GetProperty("status").GetInt32());
        Assert.Equal(codigo, problema.GetProperty("code").GetString());
        Assert.Equal($"https://dudupay.local/errors/{codigo}", problema.GetProperty("type").GetString());
        Assert.False(string.IsNullOrWhiteSpace(problema.GetProperty("title").GetString()));
        return problema;
    }

    private static IEnumerable<string?> CamposComErro(JsonElement problema) =>
        problema.GetProperty("errors").EnumerateArray().Select(e => e.GetProperty("field").GetString());

    [Theory]
    [InlineData("common", CpfValido, "comum")]
    [InlineData("merchant", CnpjValido, "lojista")]
    public async Task CadastrarUsuario_DadosValidos_Retorna201ECriaUsuarioECarteira(string tipo, string documento, string tipoNoBanco)
    {
        var resposta = await _api.CreateClient().PostAsJsonAsync("/users",
            Corpo(document: documento, type: tipo, email: "  Maria@Exemplo.com "));

        Assert.Equal(HttpStatusCode.Created, resposta.StatusCode);
        var corpo = await resposta.Content.ReadFromJsonAsync<JsonElement>();
        var id = corpo.GetProperty("id").GetInt64();
        Assert.Equal("Maria da Silva", corpo.GetProperty("name").GetString());
        Assert.Equal("maria@exemplo.com", corpo.GetProperty("email").GetString());
        Assert.Equal(documento, corpo.GetProperty("document").GetString());
        Assert.Equal(tipo, corpo.GetProperty("type").GetString());
        Assert.False(corpo.TryGetProperty("password", out _));

        await using var conexao = await db.DataSource.OpenConnectionAsync();
        var linha = await conexao.QuerySingleAsync<(string Email, string Documento, string Tipo, decimal Saldo)>("""
            SELECT u.email, u.documento, u.tipo, c.saldo
            FROM usuarios u JOIN carteiras c ON c.usuario_id = u.id
            WHERE u.id = @id
            """, new { id });
        Assert.Equal("maria@exemplo.com", linha.Email);
        Assert.Equal(documento, linha.Documento);
        Assert.Equal(tipoNoBanco, linha.Tipo);
        Assert.Equal(1000.00m, linha.Saldo);
    }

    [Fact]
    public async Task CadastrarUsuario_Valido_GravaHashDaSenha()
    {
        await _api.CreateClient().PostAsJsonAsync("/users", Corpo(password: "senha-forte"));

        await using var conexao = await db.DataSource.OpenConnectionAsync();
        var hash = await conexao.ExecuteScalarAsync<string>("SELECT hash_senha FROM usuarios");
        Assert.NotNull(hash);
        Assert.DoesNotContain("senha-forte", hash);
        Assert.True(new HasherSenha().Verificar("senha-forte", hash));
    }

    [Fact]
    public async Task CadastrarUsuario_EmailDuplicadoOutraCaixa_Retorna409EmailTakenENaoGrava()
    {
        var cliente = _api.CreateClient();
        await cliente.PostAsJsonAsync("/users", Corpo());

        var resposta = await cliente.PostAsJsonAsync("/users",
            Corpo(email: "MARIA@exemplo.com", document: OutroCpfValido));

        var problema = await LerProblemaAsync(resposta, HttpStatusCode.Conflict, "email_taken");
        Assert.Equal(["email"], CamposComErro(problema));
        Assert.Equal((1, 1), await ContarAsync());
    }

    [Fact]
    public async Task CadastrarUsuario_DocumentoDuplicado_Retorna409DocumentTakenENaoGrava()
    {
        var cliente = _api.CreateClient();
        await cliente.PostAsJsonAsync("/users", Corpo());

        var resposta = await cliente.PostAsJsonAsync("/users", Corpo(email: "outra@exemplo.com"));

        var problema = await LerProblemaAsync(resposta, HttpStatusCode.Conflict, "document_taken");
        Assert.Equal(["document"], CamposComErro(problema));
        Assert.Equal((1, 1), await ContarAsync());
    }

    public static TheoryData<string, object> DadosInvalidos => new()
    {
        { "name", Corpo(name: " ") },
        { "email", Corpo(email: "maria.exemplo.com") },
        { "password", Corpo(password: "curta") },
        { "document", Corpo(document: "52998224724") },        // dígito verificador
        { "document", Corpo(document: "529.982.247-25") },     // formatado
        { "document", Corpo(document: CnpjValido) },           // comum com CNPJ
        { "document", Corpo(document: CpfValido, type: "merchant") }, // lojista com CPF
        { "type", Corpo(type: "admin") },                      // ADR 0016: não existe administrador
        { "type", new { name = "Maria", document = CpfValido, email = "m@x.com", password = "senha-forte" } }, // sem type
    };

    [Theory]
    [MemberData(nameof(DadosInvalidos))]
    public async Task CadastrarUsuario_DadosInvalidos_Retorna422ComErroNoCampoENaoGrava(string campo, object corpo)
    {
        var resposta = await _api.CreateClient().PostAsJsonAsync("/users", corpo);

        var problema = await LerProblemaAsync(resposta, HttpStatusCode.UnprocessableEntity, "validation_error");
        Assert.Contains(campo, CamposComErro(problema));
        Assert.Equal((0, 0), await ContarAsync());
    }

    [Fact]
    public async Task CadastrarUsuario_VariosCamposInvalidos_Retorna422ComTodosOsCampos()
    {
        var resposta = await _api.CreateClient().PostAsJsonAsync("/users",
            new { name = "", document = "1", email = "x", password = "1", type = "x" });

        var problema = await LerProblemaAsync(resposta, HttpStatusCode.UnprocessableEntity, "validation_error");
        Assert.Equal(["name", "email", "document", "password", "type"], CamposComErro(problema));
    }

    [Theory]
    [InlineData("{ \"name\": ")]                 // JSON quebrado
    [InlineData("{ \"name\": 123 }")]            // tipo errado no campo
    [InlineData("")]                             // corpo vazio
    public async Task CadastrarUsuario_JsonMalformado_Retorna422ValidationError(string json)
    {
        var resposta = await _api.CreateClient().PostAsync("/users",
            new StringContent(json, Encoding.UTF8, "application/json"));

        await LerProblemaAsync(resposta, HttpStatusCode.UnprocessableEntity, "validation_error");
        Assert.Equal((0, 0), await ContarAsync());
    }

    [Fact]
    public async Task CadastrarUsuario_FalhaAoAbrirCarteira_Retorna500ENaoGravaUsuario()
    {
        var api = _api.WithWebHostBuilder(b => b.ConfigureTestServices(s =>
            s.AddScoped<ICarteiraRepositorio, CarteiraQueFalha>()));

        var resposta = await api.CreateClient().PostAsJsonAsync("/users", Corpo());

        var problema = await LerProblemaAsync(resposta, HttpStatusCode.InternalServerError, "internal_error");
        Assert.DoesNotContain("Falha simulada", problema.GetRawText());
        Assert.Equal((0, 0), await ContarAsync());
    }

    [Fact]
    public async Task CadastrarUsuario_CadastrosSimultaneosMesmoEmail_UmRetorna201EOsDemais409()
    {
        // Dez CPFs válidos diferentes, todos com o mesmo e-mail: só o UNIQUE garante um vencedor.
        string[] cpfs =
        [
            "52998224725", "11144477735", "39053344705", "12345678909", "98765432100",
            "71428793860", "86288366757", "28625587887", "40364478829", "15350946056",
        ];
        var cliente = _api.CreateClient();

        var respostas = await Task.WhenAll(cpfs.Select(cpf =>
            cliente.PostAsJsonAsync("/users", Corpo(document: cpf))));

        Assert.Single(respostas, r => r.StatusCode == HttpStatusCode.Created);
        Assert.All(respostas.Where(r => r.StatusCode != HttpStatusCode.Created),
            r => Assert.Equal(HttpStatusCode.Conflict, r.StatusCode));
        Assert.Equal((1, 1), await ContarAsync());
    }

    private sealed class CarteiraQueFalha : ICarteiraRepositorio
    {
        public Task AdicionarAsync(Carteira carteira, CancellationToken cancellationToken = default) =>
            throw new InvalidOperationException("Falha simulada ao gravar a carteira.");
    }
}
