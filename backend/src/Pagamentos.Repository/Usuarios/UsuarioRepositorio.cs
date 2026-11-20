using Dapper;
using Npgsql;
using Pagamentos.Domain.Erros;
using Pagamentos.Domain.Usuarios;
using Pagamentos.Domain.Usuarios.Entidades;
using Pagamentos.Domain.Usuarios.Interfaces;
using Pagamentos.Infrastructure.Data;

namespace Pagamentos.Repository.Usuarios;

public sealed class UsuarioRepositorio(IConexaoDeBanco banco) : IUsuarioRepositorio
{
    private const string ConstraintEmail = "usuarios_email_key";
    private const string ConstraintDocumento = "usuarios_documento_key";

    public Task<bool> EmailExisteAsync(string email, CancellationToken cancellationToken = default) =>
        ExisteAsync("SELECT EXISTS (SELECT 1 FROM usuarios WHERE email = @valor)", email, cancellationToken);

    public Task<bool> DocumentoExisteAsync(string documento, CancellationToken cancellationToken = default) =>
        ExisteAsync("SELECT EXISTS (SELECT 1 FROM usuarios WHERE documento = @valor)", documento, cancellationToken);

    public async Task<Result> AdicionarAsync(Usuario usuario, CancellationToken cancellationToken = default)
    {
        const string sql = """
            INSERT INTO usuarios (nome, email, documento, tipo, hash_senha)
            VALUES (@Nome, @Email, @Documento, @Tipo, @HashSenha)
            RETURNING id
            """;

        var parametros = new
        {
            usuario.Nome,
            usuario.Email,
            usuario.Documento,
            Tipo = TipoUsuarioNoBanco.Para(usuario.Tipo),
            usuario.HashSenha,
        };

        var conexao = await banco.ObterConexaoAsync(cancellationToken);
        try
        {
            var id = await conexao.ExecuteScalarAsync<long>(
                new CommandDefinition(sql, parametros, banco.Transacao, cancellationToken: cancellationToken));
            usuario.AtribuirId(id);
            return Result.Ok();
        }
        catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            // Corrida com outro cadastro: a checagem prévia passou, mas o UNIQUE barrou.
            return ex.ConstraintName switch
            {
                ConstraintEmail => Result.Falha(ErrosUsuario.EmailEmUso),
                ConstraintDocumento => Result.Falha(ErrosUsuario.DocumentoEmUso),
                _ => throw new InvalidOperationException($"Violação de unicidade inesperada: {ex.ConstraintName}", ex),
            };
        }
    }

    private async Task<bool> ExisteAsync(string sql, string valor, CancellationToken cancellationToken)
    {
        var conexao = await banco.ObterConexaoAsync(cancellationToken);
        return await conexao.ExecuteScalarAsync<bool>(
            new CommandDefinition(sql, new { valor }, banco.Transacao, cancellationToken: cancellationToken));
    }
}
