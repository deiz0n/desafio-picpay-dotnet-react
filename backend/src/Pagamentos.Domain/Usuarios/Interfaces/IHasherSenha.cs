namespace Pagamentos.Domain.Usuarios.Interfaces;

/// <summary>Gera e confere o hash de uma senha. A senha em texto nunca é persistida.</summary>
public interface IHasherSenha
{
    string GerarHash(string senha);

    bool Verificar(string senha, string hash);
}
