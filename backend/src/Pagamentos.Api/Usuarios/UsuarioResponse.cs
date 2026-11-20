namespace Pagamentos.Api.Usuarios;

/// <summary>Usuário como a API devolve: nunca inclui senha nem hash.</summary>
public sealed record UsuarioResponse(long Id, string Name, string Email, string Document, string Type);
