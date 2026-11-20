using Pagamentos.Domain.Usuarios.Entidades;

namespace Pagamentos.Application.Usuarios.CadastrarUsuario;

/// <summary>Dados de entrada do cadastro. <c>Tipo</c> nulo = tipo desconhecido (vira erro de validação).</summary>
public sealed record CadastrarUsuarioCommand(
    string Nome,
    string Email,
    string Documento,
    TipoUsuario? Tipo,
    string Senha);
