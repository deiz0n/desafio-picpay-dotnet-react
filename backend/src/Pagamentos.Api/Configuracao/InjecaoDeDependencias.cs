using Npgsql;
using Pagamentos.Application.Usuarios.CadastrarUsuario;
using Pagamentos.Domain.Persistencia;
using Pagamentos.Domain.Usuarios.Interfaces;
using Pagamentos.Infrastructure.Data;
using Pagamentos.Infrastructure.Seguranca;
using Pagamentos.Repository.Usuarios;

namespace Pagamentos.Api.Configuracao;

/// <summary>Composição das camadas (ADR 0003): só a Api conhece todas as implementações.</summary>
public static class InjecaoDeDependencias
{
    public static IServiceCollection AdicionarPagamentos(this IServiceCollection services)
    {
        // Singleton: o data source é o pool de conexões. Criado sob demanda para os testes poderem
        // trocar a connection string antes do primeiro uso.
        services.AddSingleton(sp =>
        {
            var connectionString = sp.GetRequiredService<IConfiguration>().GetConnectionString("Pagamentos");
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException(
                    "ConnectionStrings:Pagamentos não configurada. Em desenvolvimento, use " +
                    "'dotnet user-secrets set \"ConnectionStrings:Pagamentos\" \"...\"' (veja o README) " +
                    "ou a variável de ambiente ConnectionStrings__Pagamentos.");
            return NpgsqlDataSource.Create(connectionString);
        });

        // Scoped (ADR 0005): uma unidade de trabalho por requisição, vista pelas duas interfaces.
        services.AddScoped<UnidadeDeTrabalho>();
        services.AddScoped<IUnidadeDeTrabalho>(sp => sp.GetRequiredService<UnidadeDeTrabalho>());
        services.AddScoped<IConexaoDeBanco>(sp => sp.GetRequiredService<UnidadeDeTrabalho>());

        services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();
        services.AddScoped<ICarteiraRepositorio, CarteiraRepositorio>();
        services.AddSingleton<IHasherSenha, HasherSenha>();

        services.AddScoped<CadastrarUsuarioHandler>();
        return services;
    }
}
