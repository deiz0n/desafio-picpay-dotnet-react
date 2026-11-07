# 0003 — Clean Architecture em camadas no padrão do CVL

- **Status:** accepted
- **Data:** 2026-11-06
- **Spec de origem:** —
- **Substitui:** —

## Contexto

O objetivo é praticar Clean Architecture, SOLID e Clean Code. O CVL.API já usa uma variação de Clean Architecture com convenções definidas; seguir o mesmo modelo evita inventar padrões e permite comparar as duas bases.

## Decisão

Vamos organizar a solução em cinco projetos, com dependências apontando para dentro:

| Projeto | Papel | Depende de |
| --- | --- | --- |
| `Pagamentos.Api` | Controllers HTTP, middleware, composição (DI) | Application, Infrastructure, Repository |
| `Pagamentos.Application` | Casos de uso: `Command`/`Query` + `Handler` + `Result`/`Result<T>` | Domain |
| `Pagamentos.Domain` | Entidades com regras, interfaces (ports), DTOs de domínio, erros | — |
| `Pagamentos.Repository` | Implementações `*Repositorio` (acesso a banco) | Domain, Infrastructure |
| `Pagamentos.Infrastructure` | Factories de conexão (`Data/`), clientes HTTP externos (`Proxies/`, `*Client`), logging | Domain |

Padrões adotados do CVL:

- **Handlers chamados direto pelo Controller** — sem MediatR. Handler magro: carrega domínio → chama métodos do domínio → usa ports → retorna `Result`. Mais de ~4–5 dependências = fatiar.
- **Result pattern** (`Result.Ok()`, `Result.Ok(data)`, `Result.Falha(erro, codigoErro?)`) em vez de exceção para fluxo de negócio esperado.
- **Domain rico**: regras nas entidades (`carteira.Debitar(valor)`), sem I/O; entidades nunca recebem repositório/client.
- **Domain organizado por área** de negócio (`Usuarios/`, `Transferencias/`, `Notificacoes/`) com subpastas `Entidades/`, `Interfaces/`, `Dtos/`; transversais em `Erros/`.
- **Ports nomeados pelo papel, não pelo provedor**: `IAutorizadorClient`, `INotificacaoClient` (nunca `IDeviToolsClient`). Persistência: `I*Repositorio`.
- **Nomenclatura em português** para domínio, métodos e variáveis (`ObterPorIdAsync`, `carteiraRepositorio`); sufixos técnicos mantidos (`Handler`, `Command`, `Query`, `Result`, `DTO`, `Client`).
- **Sem `IHttpClient` próprio** — usar `IHttpClientFactory`.
- **Correlation id** via header `X-Correlation-Id` (gera se ausente) e logs com ids de negócio.

Não adotados do CVL (fora do escopo do desafio): autenticação JWT/Auth0, deploy GKE/pipelines Azure.

## Alternativas consideradas

| Alternativa | Prós | Contras |
| --- | --- | --- |
| Camadas padrão CVL (escolhida) | Convenções já testadas; aprendizado transferível | 5 projetos para um domínio pequeno |
| Vertical Slice | Menos cerimônia; feature autocontida | Diverge do CVL; pratica menos a separação de camadas |
| MediatR + CQRS com pipeline behaviors | Muito comum no mercado .NET | Esconde o fluxo (indireção) — pior para quem está aprendendo; CVL evitou de propósito |
| Projeto único com pastas | Simples | Não impõe a regra de dependência; fácil violar |

## Consequências

### Positivas

- Regra de dependência garantida pelo compilador (referências entre projetos).
- Regras de negócio testáveis sem banco nem HTTP.

### Negativas / trade-offs

- Mais arquivos e mapeamentos entre camadas.
- `Repository` separado de `Infrastructure` é uma escolha do CVL, não um requisito da Clean Architecture — vale discutir.

### O que fazer se a decisão mudar

Novo ADR. Migrar para Vertical Slice é possível reagrupando por feature sem tocar o Domain.
