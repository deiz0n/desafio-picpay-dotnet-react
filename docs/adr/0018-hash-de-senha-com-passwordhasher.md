# 0018 — Hash de senha com o `PasswordHasher<T>` do ASP.NET Core Identity

- **Status:** accepted
- **Data:** 2026-11-20
- **Spec de origem:** [02-fundacao-backend-cadastro](../specs/02-fundacao-backend-cadastro.md)
- **Substitui:** —

## Contexto

O cadastro recebe uma senha (requisito do enunciado) que será usada na autenticação. A senha nunca pode ser gravada em texto — o protótipo anterior fazia isso numa coluna chamada `password_hash`. É preciso um algoritmo lento de propósito, com salt por senha, e que permita aumentar o custo no futuro sem invalidar os hashes existentes.

## Decisão

Vamos gerar e verificar hashes com o **`PasswordHasher<T>`** do pacote `Microsoft.Extensions.Identity.Core` (PBKDF2 com HMAC-SHA512 e salt aleatório; algoritmo, iterações e salt ficam embutidos no próprio hash). Ele fica atrás do port **`IHasherSenha`** (Domain), implementado por `HasherSenha` (Infrastructure). Só o pacote de core é usado, não o Identity completo (sem tabelas, `UserManager` ou cookies).

## Alternativas consideradas

| Alternativa | Prós | Contras |
| --- | --- | --- |
| `PasswordHasher<T>` (escolhida) | Da própria Microsoft, mantido junto com o .NET; formato versionado (`SuccessRehashNeeded` permite migrar o custo); sem dependência nativa | PBKDF2 é menos resistente a GPU que Argon2/bcrypt; exige um tipo `TUser` que não usamos |
| BCrypt.Net-Next | bcrypt é consagrado; API simples | Pacote de terceiro; limite de 72 bytes na senha |
| Argon2 (Konscious, Isopoh…) | Vencedor da Password Hashing Competition; resistente a GPU | Bibliotecas de terceiros, algumas com código nativo; mais parâmetros para errar |

## Consequências

### Positivas

- Senha nunca persistida nem devolvida pela API; testes conferem que o hash não contém a senha.
- Trocar o algoritmo é trocar só `HasherSenha` — o Domain depende do port.

### Negativas / trade-offs

- Cada cadastro gasta algumas dezenas de milissegundos de CPU no hash (é o objetivo). Hoje o hash é calculado antes da checagem de e-mail/documento duplicado, então um 409 também paga esse custo.

### O que fazer se a decisão mudar

Novo ADR; implementar o novo algoritmo em `HasherSenha` mantendo a verificação dos hashes antigos (detectar pelo prefixo/versão) e regravar o hash no próximo login bem-sucedido.
