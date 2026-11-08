# ADRs — Architecture Decision Records

Registro das decisões arquiteturais: o contexto, o que foi decidido, as alternativas e as consequências.

## Quando criar um ADR

Crie quando a decisão:

- escolhe tecnologia, biblioteca, padrão ou estilo arquitetural;
- tem **alternativas reais** com trade-offs;
- seria difícil ou cara de reverter;
- alguém perguntaria "por que foi feito assim?" daqui a seis meses.

Não crie para detalhes de implementação locais, nomes de variáveis ou escolhas óbvias.

## Regras

- Numeração sequencial com 4 dígitos: `NNNN-slug.md`.
- Status: `proposed` → `accepted` | `rejected`; depois, eventualmente, `deprecated` ou `superseded by NNNN`.
- ADR `accepted` é **imutável**. Mudou a decisão → novo ADR com `Substitui: NNNN`, e o antigo recebe só a atualização de status.
- Sempre que nascer de um `/goal`, citar a SPEC de origem (`../specs/NN-slug.md`) — e a SPEC cita o ADR.
- Modelo: [0000-template.md](0000-template.md). Use `/adr {decisão}` ou deixe o `/goal` propor (decisões surgem na entrevista e são gravadas na fase de documentação).

## Índice

| Nº | Título | Status | Spec de origem |
| --- | --- | --- | --- |
| [0001](0001-registrar-decisoes-com-adr.md) | Registrar decisões arquiteturais com ADR | accepted | — |
| [0002](0002-usar-csharp-dotnet-aspnet-core.md) | Usar C# com .NET 10 e ASP.NET Core (Controllers) | accepted | — |
| [0003](0003-clean-architecture-em-camadas-padrao-cvl.md) | Clean Architecture em camadas no padrão do CVL | accepted | — |
| [0004](0004-postgres-com-dapper-e-migrations-sql.md) | PostgreSQL com Dapper e migrations em SQL | accepted | — |
| [0005](0005-unit-of-work-para-transacoes.md) | Usar Unit of Work para transações entre repositórios | accepted | — |
| [0006](0006-fluxo-goal-com-testes-embutidos.md) | Substituir o fluxo SDD pelo `/goal` com testes embutidos | accepted | — |
| [0007](0007-xunit-com-testcontainers.md) | Testes com xUnit, WebApplicationFactory e Postgres via Testcontainers | accepted | — |
| [0008](0008-spa-react-vite-typescript-com-react-router.md) | Adotar SPA React + Vite + TypeScript com react-router-dom | accepted | — |
| [0009](0009-ui-com-tailwind-shadcn-e-identidade-dudupay.md) | UI com Tailwind v4, shadcn/ui e identidade visual DuduPay | accepted | — |
| [0010](0010-formularios-com-react-hook-form-e-zod.md) | Formulários com React Hook Form e Zod | accepted | — |
| [0011](0011-estado-de-servidor-com-tanstack-query.md) | Estado de servidor com TanStack Query | accepted | — |
| [0012](0012-mock-da-api-com-json-server-e-wrapper.md) | Mockar a API com json-server e um wrapper com as regras | accepted | — |
| [0013](0013-testes-do-front-com-vitest-testing-library-e-msw.md) | Testes do front com Vitest, Testing Library e MSW (sem E2E) | accepted | — |
| [0014](0014-notificacoes-persistidas-com-polling.md) | Notificações persistidas e entregues por polling | accepted | — |

## Decisões pendentes (candidatas a ADR)

- Representação de dinheiro (`decimal` + `NUMERIC(15,2)` vs. centavos `long`/`BIGINT`)
- Controle de concorrência nas carteiras (lock pessimista vs. otimista)
- Notificação assíncrona (fila em tabela/outbox, `BackgroundService`, RabbitMQ…)
- Autorizador chamado dentro ou fora da transação
