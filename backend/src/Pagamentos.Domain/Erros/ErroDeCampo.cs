namespace Pagamentos.Domain.Erros;

/// <summary>Erro ligado a um campo da entrada (ex.: "email").</summary>
public sealed record ErroDeCampo(string Campo, string Mensagem);
