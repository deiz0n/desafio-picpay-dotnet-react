using Pagamentos.Domain.Erros;
using Pagamentos.Domain.Usuarios.Entidades;
using Pagamentos.Domain.Usuarios.Interfaces;

namespace Pagamentos.UnitTests.Fakes;

/// <summary>Repositório em memória. <see cref="ErroAoAdicionar"/> simula a violação de UNIQUE do banco.</summary>
public sealed class UsuarioRepositorioFalso : IUsuarioRepositorio
{
    public List<Usuario> Adicionados { get; } = [];
    public HashSet<string> EmailsExistentes { get; } = [];
    public HashSet<string> DocumentosExistentes { get; } = [];
    public Erro? ErroAoAdicionar { get; set; }

    public Task<bool> EmailExisteAsync(string email, CancellationToken cancellationToken = default) =>
        Task.FromResult(EmailsExistentes.Contains(email));

    public Task<bool> DocumentoExisteAsync(string documento, CancellationToken cancellationToken = default) =>
        Task.FromResult(DocumentosExistentes.Contains(documento));

    public Task<Result> AdicionarAsync(Usuario usuario, CancellationToken cancellationToken = default)
    {
        if (ErroAoAdicionar is not null)
            return Task.FromResult(Result.Falha(ErroAoAdicionar));

        usuario.AtribuirId(Adicionados.Count + 1);
        Adicionados.Add(usuario);
        return Task.FromResult(Result.Ok());
    }
}
