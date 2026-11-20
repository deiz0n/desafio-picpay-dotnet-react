using Microsoft.AspNetCore.Mvc;
using Pagamentos.Domain.Erros;

namespace Pagamentos.Api.Erros;

/// <summary>
/// Substitui o 400 automático do <c>[ApiController]</c> (JSON quebrado, tipo errado, corpo vazio)
/// por 422 <c>validation_error</c>, como o contrato pede.
/// </summary>
public static class RespostaDeBindingInvalido
{
    public static IActionResult Criar(ActionContext contexto)
    {
        var campos = contexto.ModelState
            .Where(e => e.Value?.Errors.Count > 0)
            .Select(e => new ErroDeCampo(NomeDoCampo(e.Key), "Valor inválido ou JSON malformado."))
            .DistinctBy(c => c.Campo);

        var problema = Problemas.Criar(
            StatusCodes.Status422UnprocessableEntity,
            Problemas.CodigoValidacao,
            "O corpo da requisição não pôde ser lido.",
            campos);

        return Problemas.Resultado(problema);
    }

    // Chaves do ModelState: "$.name" (System.Text.Json), "request" (parâmetro), "" (corpo vazio).
    private static string NomeDoCampo(string chave)
    {
        if (chave.StartsWith("$.", StringComparison.Ordinal))
            return chave[2..];
        return "body";
    }
}
