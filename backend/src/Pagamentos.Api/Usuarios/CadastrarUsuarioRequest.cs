namespace Pagamentos.Api.Usuarios;

/// <summary>
/// Corpo do <c>POST /users</c> (contrato em docs/frontend.md). Tudo anulável de propósito:
/// campo ausente não é erro de binding, é erro de validação do Domain (422 no campo certo).
/// </summary>
public sealed record CadastrarUsuarioRequest(
    string? Name,
    string? Document,
    string? Email,
    string? Password,
    string? Type);
