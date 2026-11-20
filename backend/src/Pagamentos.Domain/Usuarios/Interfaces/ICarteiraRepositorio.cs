using Pagamentos.Domain.Usuarios.Entidades;

namespace Pagamentos.Domain.Usuarios.Interfaces;

public interface ICarteiraRepositorio
{
    Task AdicionarAsync(Carteira carteira, CancellationToken cancellationToken = default);
}
