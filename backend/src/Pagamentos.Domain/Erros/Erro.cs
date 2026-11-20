namespace Pagamentos.Domain.Erros;

/// <summary>
/// Erro de negócio esperado. <see cref="Codigo"/> é o identificador estável do contrato
/// (ex.: "email_taken") e <see cref="Campos"/> liga o erro aos campos da entrada.
/// </summary>
public sealed record Erro(TipoErro Tipo, string Codigo, string Mensagem, IReadOnlyList<ErroDeCampo> Campos)
{
    public Erro(TipoErro tipo, string codigo, string mensagem)
        : this(tipo, codigo, mensagem, []) { }
}
