using Npgsql;

namespace Pagamentos.Infrastructure.Data;

/// <summary>
/// O que os repositórios enxergam da unidade de trabalho: a conexão da requisição e a transação
/// em andamento (ou <c>null</c>, quando nenhuma foi iniciada). Repositórios nunca abrem conexão própria (ADR 0005).
/// </summary>
public interface IConexaoDeBanco
{
    Task<NpgsqlConnection> ObterConexaoAsync(CancellationToken cancellationToken = default);

    NpgsqlTransaction? Transacao { get; }
}
