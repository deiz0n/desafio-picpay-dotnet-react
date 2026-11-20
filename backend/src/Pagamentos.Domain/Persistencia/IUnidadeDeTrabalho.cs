namespace Pagamentos.Domain.Persistencia;

/// <summary>
/// Unidade de trabalho (ADR 0005): uma transação por requisição, compartilhada pelos repositórios.
/// Fica no Domain junto dos outros ports para a Infrastructure poder implementá-la (ADR 0019).
/// Não expõe tipos de banco.
/// </summary>
public interface IUnidadeDeTrabalho
{
    Task IniciarAsync(CancellationToken cancellationToken = default);

    Task ConfirmarAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Desfaz a transação em andamento; sem transação, não faz nada. Nunca lança, para não esconder
    /// a exceção original quando é chamado dentro de um <c>catch</c>.
    /// </summary>
    Task DesfazerAsync(CancellationToken cancellationToken = default);
}
