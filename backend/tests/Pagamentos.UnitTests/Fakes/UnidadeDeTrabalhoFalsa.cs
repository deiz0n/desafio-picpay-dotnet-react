using Pagamentos.Domain.Persistencia;

namespace Pagamentos.UnitTests.Fakes;

/// <summary>Registra as chamadas para o teste conferir como o Handler delimitou a transação.</summary>
public sealed class UnidadeDeTrabalhoFalsa : IUnidadeDeTrabalho
{
    public bool Iniciada { get; private set; }
    public bool Confirmada { get; private set; }
    public bool Desfeita { get; private set; }

    public Task IniciarAsync(CancellationToken cancellationToken = default)
    {
        Iniciada = true;
        return Task.CompletedTask;
    }

    public Task ConfirmarAsync(CancellationToken cancellationToken = default)
    {
        Confirmada = true;
        return Task.CompletedTask;
    }

    public Task DesfazerAsync(CancellationToken cancellationToken = default)
    {
        Desfeita = true;
        return Task.CompletedTask;
    }
}
