namespace Pagamentos.Domain.Usuarios.Entidades;

/// <summary>
/// Tipos de usuário do desafio. Não existe Administrador (ADR 0016).
/// </summary>
public enum TipoUsuario
{
    Comum = 1,
    Lojista = 2,
}
