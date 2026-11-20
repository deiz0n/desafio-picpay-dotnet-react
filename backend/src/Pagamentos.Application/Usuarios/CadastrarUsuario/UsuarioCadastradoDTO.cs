using Pagamentos.Domain.Usuarios.Entidades;

namespace Pagamentos.Application.Usuarios.CadastrarUsuario;

/// <summary>Saída do cadastro. Nunca inclui a senha nem o hash.</summary>
public sealed record UsuarioCadastradoDTO(
    long Id,
    string Nome,
    string Email,
    string Documento,
    TipoUsuario Tipo);
