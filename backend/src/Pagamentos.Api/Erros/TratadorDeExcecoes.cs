using Microsoft.AspNetCore.Diagnostics;
using Pagamentos.Domain.Erros;

namespace Pagamentos.Api.Erros;

/// <summary>
/// Último recurso para exceções não tratadas: registra no log e responde 500 <c>internal_error</c>
/// sem vazar mensagem nem stack trace para o cliente.
/// </summary>
public sealed class TratadorDeExcecoes(ILogger<TratadorDeExcecoes> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext contexto, Exception excecao, CancellationToken cancellationToken)
    {
        logger.LogError(excecao, "Erro não tratado em {Metodo} {Caminho}", contexto.Request.Method, contexto.Request.Path);

        var problema = Problemas.Criar(
            StatusCodes.Status500InternalServerError,
            Problemas.CodigoInterno,
            "Ocorreu um erro inesperado. Tente novamente mais tarde.",
            Array.Empty<ErroDeCampo>());

        contexto.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await contexto.Response.WriteAsJsonAsync(problema, (System.Text.Json.JsonSerializerOptions?)null,
            Problemas.ContentType, cancellationToken);
        return true;
    }
}
