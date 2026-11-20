using Npgsql;
using Pagamentos.Api.Configuracao;
using Pagamentos.Api.Erros;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .ConfigureApiBehaviorOptions(o => o.InvalidModelStateResponseFactory = RespostaDeBindingInvalido.Criar);

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<TratadorDeExcecoes>();
builder.Services.AdicionarPagamentos();

var app = builder.Build();

// Falha na subida, e não na primeira requisição, se a connection string não estiver configurada.
app.Services.GetRequiredService<NpgsqlDataSource>();

app.UseExceptionHandler();
app.MapControllers();

app.Run();
