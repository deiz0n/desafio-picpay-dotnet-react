using Pagamentos.Domain.Erros;
using Pagamentos.Domain.Persistencia;
using Pagamentos.Domain.Usuarios;
using Pagamentos.Domain.Usuarios.Entidades;
using Pagamentos.Domain.Usuarios.Interfaces;

namespace Pagamentos.Application.Usuarios.CadastrarUsuario;

/// <summary>
/// Cadastra um usuário e abre a carteira dele com o saldo inicial, numa única transação:
/// se qualquer passo falhar, nada é gravado.
/// </summary>
public sealed class CadastrarUsuarioHandler(
    IUsuarioRepositorio usuarioRepositorio,
    ICarteiraRepositorio carteiraRepositorio,
    IUnidadeDeTrabalho unidadeDeTrabalho,
    IHasherSenha hasherSenha)
{
    public async Task<Result<UsuarioCadastradoDTO>> ExecutarAsync(
        CadastrarUsuarioCommand command, CancellationToken cancellationToken = default)
    {
        var criacao = Usuario.Criar(
            command.Nome, command.Email, command.Documento, command.Tipo, command.Senha, hasherSenha);
        if (!criacao.Sucesso)
            return criacao.Erro;

        var usuario = criacao.Valor;

        // Checagem prévia para responder rápido e sem abrir transação. A corrida entre dois
        // cadastros simultâneos é resolvida pelo UNIQUE do banco (AdicionarAsync devolve o conflito).
        if (await usuarioRepositorio.EmailExisteAsync(usuario.Email, cancellationToken))
            return ErrosUsuario.EmailEmUso;
        if (await usuarioRepositorio.DocumentoExisteAsync(usuario.Documento, cancellationToken))
            return ErrosUsuario.DocumentoEmUso;

        await unidadeDeTrabalho.IniciarAsync(cancellationToken);
        try
        {
            var adicao = await usuarioRepositorio.AdicionarAsync(usuario, cancellationToken);
            if (!adicao.Sucesso)
            {
                await unidadeDeTrabalho.DesfazerAsync(cancellationToken);
                return adicao.Erro;
            }

            await carteiraRepositorio.AdicionarAsync(Carteira.Abrir(usuario.Id), cancellationToken);
            await unidadeDeTrabalho.ConfirmarAsync(cancellationToken);
        }
        catch
        {
            // CancellationToken.None: o rollback precisa acontecer mesmo se a requisição foi cancelada.
            await unidadeDeTrabalho.DesfazerAsync(CancellationToken.None);
            throw;
        }

        return Result.Ok(new UsuarioCadastradoDTO(
            usuario.Id, usuario.Nome, usuario.Email, usuario.Documento, usuario.Tipo));
    }
}
