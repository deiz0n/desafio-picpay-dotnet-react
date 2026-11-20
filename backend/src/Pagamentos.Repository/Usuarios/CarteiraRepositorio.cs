using Dapper;
using Pagamentos.Domain.Usuarios.Entidades;
using Pagamentos.Domain.Usuarios.Interfaces;
using Pagamentos.Infrastructure.Data;

namespace Pagamentos.Repository.Usuarios;

public sealed class CarteiraRepositorio(IConexaoDeBanco banco) : ICarteiraRepositorio
{
    public async Task AdicionarAsync(Carteira carteira, CancellationToken cancellationToken = default)
    {
        const string sql = "INSERT INTO carteiras (usuario_id, saldo) VALUES (@UsuarioId, @Saldo)";

        var conexao = await banco.ObterConexaoAsync(cancellationToken);
        await conexao.ExecuteAsync(
            new CommandDefinition(sql, new { carteira.UsuarioId, carteira.Saldo }, banco.Transacao,
                cancellationToken: cancellationToken));
    }
}
