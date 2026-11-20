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

app.UseExceptionHandler();
app.MapControllers();

app.Run();
