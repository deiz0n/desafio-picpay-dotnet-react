using Pagamentos.Domain.Usuarios.Interfaces;

namespace Pagamentos.UnitTests.Fakes;

/// <summary>Hasher previsível para testes: não faz criptografia nenhuma.</summary>
public sealed class HasherSenhaFalso : IHasherSenha
{
    public string GerarHash(string senha) => $"hash:{senha}";

    public bool Verificar(string senha, string hash) => hash == GerarHash(senha);
}
