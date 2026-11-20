using Pagamentos.Domain.Usuarios.Entidades;

namespace Pagamentos.Repository.Usuarios;

/// <summary>Tradução entre o enum do Domain e o texto da coluna <c>usuarios.tipo</c>.</summary>
internal static class TipoUsuarioNoBanco
{
    public static string Para(TipoUsuario tipo) => tipo switch
    {
        TipoUsuario.Comum => "comum",
        TipoUsuario.Lojista => "lojista",
        _ => throw new ArgumentOutOfRangeException(nameof(tipo), tipo, "Tipo de usuário sem mapeamento no banco."),
    };
}
