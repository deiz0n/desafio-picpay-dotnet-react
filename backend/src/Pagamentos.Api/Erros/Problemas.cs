using Microsoft.AspNetCore.Mvc;
using Pagamentos.Domain.Erros;

namespace Pagamentos.Api.Erros;

/// <summary>
/// Converte erros em Problem Details (RFC 9457) no formato do contrato:
/// <c>type</c>, <c>title</c>, <c>status</c>, <c>detail</c>, <c>code</c> e <c>errors[{ field, message }]</c>.
/// </summary>
public static class Problemas
{
    public const string ContentType = "application/problem+json";
    public const string CodigoValidacao = "validation_error";
    public const string CodigoInterno = "internal_error";

    // Nomes de campo do Domain (português) → nomes do contrato da API.
    private static readonly Dictionary<string, string> CamposDaApi = new()
    {
        ["nome"] = "name",
        ["email"] = "email",
        ["documento"] = "document",
        ["senha"] = "password",
        ["tipo"] = "type",
    };

    public static ObjectResult De(Erro erro) =>
        Resultado(Criar(StatusDe(erro.Tipo), erro.Codigo, erro.Mensagem, erro.Campos));

    public static ProblemDetails Criar(int status, string codigo, string mensagem, IEnumerable<ErroDeCampo> campos)
    {
        var problema = new ProblemDetails
        {
            Type = $"https://dudupay.local/errors/{codigo}",
            Title = TituloDe(status),
            Status = status,
            Detail = mensagem,
        };
        problema.Extensions["code"] = codigo;

        var erros = campos
            .Select(c => new { field = CamposDaApi.GetValueOrDefault(c.Campo, c.Campo), message = c.Mensagem })
            .ToList();
        if (erros.Count > 0)
            problema.Extensions["errors"] = erros;

        return problema;
    }

    public static ObjectResult Resultado(ProblemDetails problema) => new(problema)
    {
        StatusCode = problema.Status,
        ContentTypes = { ContentType },
    };

    private static int StatusDe(TipoErro tipo) => tipo switch
    {
        TipoErro.Validacao => StatusCodes.Status422UnprocessableEntity,
        TipoErro.Conflito => StatusCodes.Status409Conflict,
        TipoErro.NaoEncontrado => StatusCodes.Status404NotFound,
        _ => StatusCodes.Status500InternalServerError,
    };

    private static string TituloDe(int status) => status switch
    {
        StatusCodes.Status422UnprocessableEntity => "Dados inválidos",
        StatusCodes.Status409Conflict => "Conflito com um registro existente",
        StatusCodes.Status404NotFound => "Recurso não encontrado",
        _ => "Erro inesperado",
    };
}
