# 0007 — Testes com xUnit, WebApplicationFactory e Postgres via Testcontainers

- **Status:** accepted
- **Data:** 2026-11-07
- **Spec de origem:** —
- **Substitui:** —

## Contexto

O fluxo `/goal` ([ADR 0006](0006-fluxo-goal-com-testes-embutidos.md)) exige que o agente se teste sozinho: chamar a API e **consultar o banco** para validar o resultado. As regras críticas do desafio — rollback total, `SELECT … FOR UPDATE`, `CHECK (saldo >= 0)`, concorrência — só são provadas contra um **PostgreSQL real** (ADR 0004). O CVL não tem projeto de testes para servir de referência. Playwright/E2E foi descartado: não há frontend no escopo.

## Decisão

Vamos usar:

- **xUnit** como framework, com asserts nativos (`Assert.*`).
- **`Pagamentos.UnitTests`** — Domain e Handlers com ports falsos.
- **`Pagamentos.IntegrationTests`** — `WebApplicationFactory<Program>` (`Microsoft.AspNetCore.Mvc.Testing`) chamando os endpoints via HTTP em memória, com **Postgres em container** (`Testcontainers.PostgreSql`) onde as migrations SQL do projeto são aplicadas; asserts no banco com SQL direto (Dapper).
- Serviços externos (autorizador, notificação) **sempre substituídos por fakes** no DI dos testes.

Convenções operacionais: [`.agents/rules/testes.md`](../../.agents/rules/testes.md).

## Alternativas consideradas

| Alternativa | Prós | Contras |
| --- | --- | --- |
| xUnit + WebApplicationFactory + Testcontainers (escolhida) | Testa o Postgres real (locks, constraints, transações); isolado e reproduzível; padrão de mercado no .NET | Exige Docker para rodar a integração; testes mais lentos |
| NUnit / MSTest | Equivalentes em recursos | xUnit é o padrão dos templates e da documentação do ASP.NET Core |
| SQLite / provider em memória | Rápido, sem Docker | Não suporta `FOR UPDATE` nem o mesmo comportamento transacional — não prova as regras do desafio |
| Banco compartilhado do `docker compose` | Sem dependência extra | Estado compartilhado entre execuções; testes não isolados nem reproduzíveis |
| FluentAssertions nos asserts | Leitura fluente | v8 passou a ter licença comercial; `Assert` nativo basta |
| E2E com Playwright | Exercita a stack completa | Sem frontend; HTTP em memória já cobre a API |

## Consequências

### Positivas

- Rollback, concorrência e constraints provados contra o banco real.
- Cada critério de aceite do `/goal` vira um teste rastreável.
- Fakes dos serviços externos permitem testar autorização negada, timeout e falha de notificação de forma determinística.

### Negativas / trade-offs

- Docker obrigatório para `dotnet test` completo (local e CI).
- Startup do container adiciona segundos por classe/coleção de teste.
- `Program` precisa ser acessível ao projeto de testes (`public partial class Program;` ou `InternalsVisibleTo`).

### O que fazer se a decisão mudar

Novo ADR. Trocar framework afeta só os projetos de teste; trocar Testcontainers por outro provisionamento afeta apenas a fixture do banco.

