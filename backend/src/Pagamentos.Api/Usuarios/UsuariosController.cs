using Microsoft.AspNetCore.Mvc;
using Pagamentos.Api.Erros;
using Pagamentos.Application.Usuarios.CadastrarUsuario;

namespace Pagamentos.Api.Usuarios;

[ApiController]
[Route("users")]
public sealed class UsuariosController(CadastrarUsuarioHandler cadastrarUsuarioHandler) : ControllerBase
{
    /// <summary>Cadastra um usuário comum (CPF) ou lojista (CNPJ) com carteira de R$ 1.000,00.</summary>
    [HttpPost]
    [ProducesResponseType<UsuarioResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict, "application/problem+json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status422UnprocessableEntity, "application/problem+json")]
    public async Task<IActionResult> Cadastrar(CadastrarUsuarioRequest request, CancellationToken cancellationToken)
    {
        var command = new CadastrarUsuarioCommand(
            Nome: request.Name ?? "",
            Email: request.Email ?? "",
            Documento: request.Document ?? "",
            Tipo: TipoUsuarioNaApi.De(request.Type),
            Senha: request.Password ?? "");

        var resultado = await cadastrarUsuarioHandler.ExecutarAsync(command, cancellationToken);
        if (!resultado.Sucesso)
            return Problemas.De(resultado.Erro!);

        var usuario = resultado.Valor;
        // Sem Location: o contrato não tem GET /users/{id}.
        return StatusCode(StatusCodes.Status201Created, new UsuarioResponse(
            usuario.Id, usuario.Nome, usuario.Email, usuario.Documento, TipoUsuarioNaApi.Para(usuario.Tipo)));
    }
}
