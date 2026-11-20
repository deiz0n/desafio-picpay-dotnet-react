using Microsoft.Extensions.Logging;
using Npgsql;
using Pagamentos.Domain.Persistencia;

namespace Pagamentos.Infrastructure.Data;

/// <summary>
/// Dona da conexão e da transação de uma requisição (registrada como Scoped, ADR 0005).
/// A conexão só é aberta quando alguém precisa dela.
/// </summary>
public sealed class UnidadeDeTrabalho(NpgsqlDataSource dataSource, ILogger<UnidadeDeTrabalho> logger)
    : IUnidadeDeTrabalho, IConexaoDeBanco, IAsyncDisposable
{
    private NpgsqlConnection? _conexao;

    public NpgsqlTransaction? Transacao { get; private set; }

    public async Task<NpgsqlConnection> ObterConexaoAsync(CancellationToken cancellationToken = default) =>
        _conexao ??= await dataSource.OpenConnectionAsync(cancellationToken);

    public async Task IniciarAsync(CancellationToken cancellationToken = default)
    {
        if (Transacao is not null)
            throw new InvalidOperationException("Já existe uma transação em andamento.");

        var conexao = await ObterConexaoAsync(cancellationToken);
        Transacao = await conexao.BeginTransactionAsync(cancellationToken);
    }

    public async Task ConfirmarAsync(CancellationToken cancellationToken = default)
    {
        var transacao = Transacao ?? throw new InvalidOperationException("Nenhuma transação em andamento.");
        await transacao.CommitAsync(cancellationToken);
        await EncerrarTransacaoAsync();
    }

    /// <summary>
    /// Desfaz a transação em andamento. Sem transação, não faz nada (pode ser chamado mais de uma vez).
    /// Nunca lança: normalmente é chamado dentro de um <c>catch</c>, e uma exceção aqui esconderia a original.
    /// Se o rollback falhar (ex.: conexão perdida), o Postgres já descarta a transação ao fechar a conexão.
    /// </summary>
    public async Task DesfazerAsync(CancellationToken cancellationToken = default)
    {
        if (Transacao is null)
            return;

        try
        {
            await Transacao.RollbackAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Falha ao desfazer a transação; a conexão será descartada.");
            await DescartarConexaoAsync();
        }

        await EncerrarTransacaoAsync();
    }

    /// <summary>Uma transação não confirmada até aqui é desfeita pelo próprio Npgsql ao ser descartada.</summary>
    public async ValueTask DisposeAsync()
    {
        await EncerrarTransacaoAsync();
        await DescartarConexaoAsync();
    }

    private async Task EncerrarTransacaoAsync()
    {
        var transacao = Transacao;
        Transacao = null;
        if (transacao is null)
            return;

        try
        {
            await transacao.DisposeAsync();
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Falha ao descartar a transação.");
        }
    }

    private async Task DescartarConexaoAsync()
    {
        var conexao = _conexao;
        _conexao = null;
        if (conexao is not null)
            await conexao.DisposeAsync();
    }
}
