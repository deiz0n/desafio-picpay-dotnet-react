using System.Text.RegularExpressions;
using Pagamentos.Domain.Erros;
using Pagamentos.Domain.Usuarios.Interfaces;

namespace Pagamentos.Domain.Usuarios.Entidades;

/// <summary>Usuário da plataforma: comum (CPF) ou lojista (CNPJ).</summary>
public sealed partial class Usuario
{
    public const int TamanhoMaximoNome = 100;
    public const int TamanhoMaximoEmail = 254;
    public const int TamanhoMinimoSenha = 8;
    public const int TamanhoMaximoSenha = 100;

    private Usuario(string nome, string email, string documento, TipoUsuario tipo, string hashSenha)
    {
        Nome = nome;
        Email = email;
        Documento = documento;
        Tipo = tipo;
        HashSenha = hashSenha;
    }

    /// <summary>0 até o banco gerar o id (<see cref="AtribuirId"/>).</summary>
    public long Id { get; private set; }

    public string Nome { get; }

    /// <summary>Sem espaços nas pontas e em minúsculas.</summary>
    public string Email { get; }

    /// <summary>CPF ou CNPJ, só dígitos.</summary>
    public string Documento { get; }

    public TipoUsuario Tipo { get; }

    public string HashSenha { get; }

    /// <summary>
    /// Valida todos os campos de uma vez (devolve todos os erros juntos) e, se estiverem válidos,
    /// cria o usuário guardando só o hash da senha.
    /// </summary>
    public static Result<Usuario> Criar(
        string nome, string email, string documento, TipoUsuario? tipo, string senha, IHasherSenha hasher)
    {
        nome = nome.Trim();
        email = email.Trim().ToLowerInvariant();

        var erros = new List<ErroDeCampo>();
        ValidarNome(nome, erros);
        ValidarEmail(email, erros);
        ValidarDocumento(documento, tipo, erros);
        ValidarSenha(senha, erros);
        if (tipo is null)
            erros.Add(new("tipo", "Tipo de usuário inválido."));

        if (erros.Count > 0)
            return ErrosUsuario.Validacao(erros);

        return Result.Ok(new Usuario(nome, email, documento, tipo!.Value, hasher.GerarHash(senha)));
    }

    /// <summary>Recebe o id gerado pelo banco. Só pode ser chamado uma vez.</summary>
    public void AtribuirId(long id)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        if (Id != 0)
            throw new InvalidOperationException("O usuário já tem id.");
        Id = id;
    }

    private static void ValidarNome(string nome, List<ErroDeCampo> erros)
    {
        if (nome.Length == 0)
            erros.Add(new("nome", "Informe o nome completo."));
        else if (nome.Length > TamanhoMaximoNome)
            erros.Add(new("nome", $"O nome deve ter no máximo {TamanhoMaximoNome} caracteres."));
    }

    private static void ValidarEmail(string email, List<ErroDeCampo> erros)
    {
        if (email.Length == 0 || email.Length > TamanhoMaximoEmail || !FormatoEmail().IsMatch(email))
            erros.Add(new("email", "Informe um e-mail válido."));
    }

    private static void ValidarDocumento(string documento, TipoUsuario? tipo, List<ErroDeCampo> erros)
    {
        if (!ValidadorDeDocumento.SoDigitos(documento))
        {
            erros.Add(new("documento", "O documento deve conter só dígitos."));
            return;
        }

        switch (tipo)
        {
            case TipoUsuario.Comum when documento.Length != ValidadorDeDocumento.TamanhoCpf:
                erros.Add(new("documento", "Usuário comum deve informar um CPF (11 dígitos)."));
                break;
            case TipoUsuario.Comum when !ValidadorDeDocumento.CpfValido(documento):
                erros.Add(new("documento", "CPF inválido."));
                break;
            case TipoUsuario.Lojista when documento.Length != ValidadorDeDocumento.TamanhoCnpj:
                erros.Add(new("documento", "Lojista deve informar um CNPJ (14 dígitos)."));
                break;
            case TipoUsuario.Lojista when !ValidadorDeDocumento.CnpjValido(documento):
                erros.Add(new("documento", "CNPJ inválido."));
                break;
            // Tipo desconhecido já gera erro em "tipo"; o documento ainda precisa ser um CPF ou CNPJ válido.
            case null when !ValidadorDeDocumento.CpfValido(documento) && !ValidadorDeDocumento.CnpjValido(documento):
                erros.Add(new("documento", "Informe um CPF ou CNPJ válido."));
                break;
        }
    }

    private static void ValidarSenha(string senha, List<ErroDeCampo> erros)
    {
        if (senha.Length < TamanhoMinimoSenha || senha.Length > TamanhoMaximoSenha)
            erros.Add(new("senha",
                $"A senha deve ter entre {TamanhoMinimoSenha} e {TamanhoMaximoSenha} caracteres."));
    }

    // Formato simples: algo@algo.algo, sem espaços. Confirmar que o e-mail existe exigiria enviar uma mensagem.
    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex FormatoEmail();
}
