using Pagamentos.Domain.Erros;

namespace Pagamentos.Domain.Usuarios;

/// <summary>Erros de negócio do cadastro de usuário (códigos do contrato da API).</summary>
public static class ErrosUsuario
{
    public static Erro Validacao(IReadOnlyList<ErroDeCampo> campos) =>
        new(TipoErro.Validacao, "validation_error", "Os dados enviados são inválidos.", campos);

    public static readonly Erro EmailEmUso = new(
        TipoErro.Conflito, "email_taken", "E-mail já cadastrado.",
        [new ErroDeCampo("email", "Este e-mail já está cadastrado.")]);

    public static readonly Erro DocumentoEmUso = new(
        TipoErro.Conflito, "document_taken", "Documento já cadastrado.",
        [new ErroDeCampo("documento", "Este documento já está cadastrado.")]);
}
