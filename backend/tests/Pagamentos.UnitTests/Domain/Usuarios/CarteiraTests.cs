using Pagamentos.Domain.Usuarios.Entidades;

namespace Pagamentos.UnitTests.Domain.Usuarios;

public class CarteiraTests
{
    [Fact]
    public void Abrir_NovoUsuario_SaldoInicialMil()
    {
        var carteira = Carteira.Abrir(usuarioId: 7);

        Assert.Equal(7, carteira.UsuarioId);
        Assert.Equal(1000.00m, carteira.Saldo);
    }

    [Fact]
    public void Abrir_UsuarioSemId_LancaArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Carteira.Abrir(usuarioId: 0));
    }
}
