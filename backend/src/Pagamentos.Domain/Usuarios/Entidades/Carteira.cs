namespace Pagamentos.Domain.Usuarios.Entidades;

/// <summary>Carteira do usuário. Valores em reais (<c>decimal</c>, ADR 0017).</summary>
public sealed class Carteira
{
    /// <summary>Crédito de boas-vindas de todo usuário novo (contrato do front).</summary>
    public const decimal SaldoInicial = 1000.00m;

    private Carteira(long usuarioId, decimal saldo)
    {
        UsuarioId = usuarioId;
        Saldo = saldo;
    }

    public long UsuarioId { get; }

    public decimal Saldo { get; private set; }

    public static Carteira Abrir(long usuarioId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(usuarioId);
        return new Carteira(usuarioId, SaldoInicial);
    }
}
