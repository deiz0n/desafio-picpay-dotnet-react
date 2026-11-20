using Pagamentos.Domain.Erros;
using Pagamentos.Domain.Usuarios.Entidades;

namespace Pagamentos.Domain.Usuarios.Interfaces;

public interface IUsuarioRepositorio
{
    Task<bool> EmailExisteAsync(string email, CancellationToken cancellationToken = default);

    Task<bool> DocumentoExisteAsync(string documento, CancellationToken cancellationToken = default);

    /// <summary>
    /// Grava o usuário e atribui o id gerado. Se o banco recusar por e-mail ou documento
    /// duplicado (corrida com outro cadastro), devolve o erro de conflito correspondente.
    /// </summary>
    Task<Result> AdicionarAsync(Usuario usuario, CancellationToken cancellationToken = default);
}
