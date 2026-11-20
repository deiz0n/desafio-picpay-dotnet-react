namespace Pagamentos.Domain.Erros;

/// <summary>
/// Categoria do erro. O Domain não conhece HTTP: a Api traduz cada tipo em um status.
/// </summary>
public enum TipoErro
{
    Validacao,
    Conflito,
    NaoEncontrado,
}
