using Pagamentos.Domain.Usuarios.Entidades;
using Pagamentos.Domain.Usuarios.Interfaces;

namespace Pagamentos.UnitTests.Fakes;

public sealed class CarteiraRepositorioFalso : ICarteiraRepositorio
{
    public List<Carteira> Adicionadas { get; } = [];
    public bool Falhar { get; set; }

    public Task AdicionarAsync(Carteira carteira, CancellationToken cancellationToken = default)
    {
        if (Falhar)
            throw new InvalidOperationException("Falha simulada ao gravar a carteira.");

        Adicionadas.Add(carteira);
        return Task.CompletedTask;
    }
}
