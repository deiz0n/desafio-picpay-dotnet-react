using Microsoft.AspNetCore.Identity;
using Pagamentos.Domain.Usuarios.Interfaces;

namespace Pagamentos.Infrastructure.Seguranca;

/// <summary>
/// Hash de senha com o <see cref="PasswordHasher{TUser}"/> do ASP.NET Core Identity (ADR 0018):
/// PBKDF2 com salt aleatório; algoritmo, iterações e salt ficam embutidos no próprio hash.
/// </summary>
public sealed class HasherSenha : IHasherSenha
{
    // O PasswordHasher exige um tipo de usuário, mas a implementação padrão não o usa.
    private sealed class SemUsuario;

    private static readonly SemUsuario Ninguem = new();
    private readonly PasswordHasher<SemUsuario> _hasher = new();

    public string GerarHash(string senha) => _hasher.HashPassword(Ninguem, senha);

    public bool Verificar(string senha, string hash) =>
        _hasher.VerifyHashedPassword(Ninguem, hash, senha) != PasswordVerificationResult.Failed;
}
