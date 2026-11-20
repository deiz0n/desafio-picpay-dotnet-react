using Pagamentos.Domain.Usuarios.Entidades;

namespace Pagamentos.Api.Usuarios;

/// <summary>Tradução entre o enum do Domain e os valores do contrato (<c>"common"</c> / <c>"merchant"</c>).</summary>
public static class TipoUsuarioNaApi
{
    public const string Comum = "common";
    public const string Lojista = "merchant";

    /// <summary>Valor desconhecido (inclusive "admin") vira <c>null</c>, que o Domain recusa como erro de validação.</summary>
    public static TipoUsuario? De(string? valor) => valor switch
    {
        Comum => TipoUsuario.Comum,
        Lojista => TipoUsuario.Lojista,
        _ => null,
    };

    public static string Para(TipoUsuario tipo) => tipo switch
    {
        TipoUsuario.Comum => Comum,
        TipoUsuario.Lojista => Lojista,
        _ => throw new ArgumentOutOfRangeException(nameof(tipo), tipo, "Tipo de usuário sem mapeamento na API."),
    };
}
