# Roadmap

Competências do desafio ([challenge.md](challenge.md#competências-praticadas)) organizadas como itens entregáveis.

**Status:** ⏳ pendente · 🚧 em andamento · ✅ feito · ❌ desistimos (com data e motivo — nunca apagar a linha)

Ao concluir um `/goal` que entrega um item, atualize a linha (status + SPEC + ADRs) no mesmo PR.

## Núcleo — fluxo de transferência

| Item | Status | Spec | ADRs |
| --- | --- | --- | --- |
| Escolha de linguagem/framework | ✅ | — | [0002](adr/0002-usar-csharp-dotnet-aspnet-core.md), [0004](adr/0004-postgres-com-dapper-e-migrations-sql.md) |
| Modelo de dados (usuários, carteiras, transferências) com `UNIQUE` em documento e e-mail | ⏳ | — | — |
| `POST /transfer` com regras (lojista não envia, saldo, valor > 0, payer ≠ payee) | ⏳ | — | — |
| Consulta ao autorizador externo (timeout, falha = não autorizado) | ⏳ | — | — |
| Transação com rollback e controle de concorrência | ⏳ | — | [0005](adr/0005-unit-of-work-para-transacoes.md) |
| Notificação assíncrona com retry | ⏳ | — | — |

## Básico

| Item | Status | Spec | ADRs |
| --- | --- | --- | --- |
| API RESTful com respostas de erro consistentes | ⏳ | — | — |
| Git com commits pequenos e descritivos | ⏳ | — | — |
| Código limpo e organizado | ⏳ | — | — |

## Intermediário

| Item | Status | Spec | ADRs |
| --- | --- | --- | --- |
| Docker Compose (app + banco + fila/cache) com um comando | ⏳ | — | — |
| Testes de unidade | ⏳ | — | [0007](adr/0007-xunit-com-testcontainers.md) |
| Testes de integração | ⏳ | — | [0007](adr/0007-xunit-com-testcontainers.md) |
| SOLID e Design Patterns aplicados | 🚧 | — | [0003](adr/0003-clean-architecture-em-camadas-padrao-cvl.md) |
| Cache | ⏳ | — | — |
| README com execução, decisões e melhorias | ⏳ | — | — |
| Seed com usuários comuns e lojistas | ⏳ | — | — |

## Avançado

| Item | Status | Spec | ADRs |
| --- | --- | --- | --- |
| CI com testes e análise estática | ⏳ | — | — |
| Observabilidade (logs estruturados, correlation id, métricas, health check) | ⏳ | — | — |
| Mensageria | ⏳ | — | — |
| Idempotência (`Idempotency-Key`) | ⏳ | — | — |
| Banco não relacional | ⏳ | — | — |
| Arquitetura (Clean Architecture em camadas; avaliar Monolito Modular/CQRS depois) | 🚧 | — | [0003](adr/0003-clean-architecture-em-camadas-padrao-cvl.md) |
| Ferramentas de qualidade (linters, análise estática) | ⏳ | — | — |

## Front-end — DuduPay

Plano e contrato em [frontend.md](frontend.md). Cada fatia é um `/goal`; a ordem é F1 → F5, depois F6/F7/F8 em qualquer ordem, por fim F9.

Exceção de convenção: na linha de planejamento, a coluna **Spec** aponta para o `frontend.md` (um plano), porque esse goal não construiu nada e não tem SPEC. As fatias F1–F9 usam a coluna normalmente, com a SPEC de cada `/goal`.

| Item | Status | Spec | ADRs |
| --- | --- | --- | --- |
| Planejamento do front (decisões, contrato do mock, fatias) | ✅ | [frontend.md](frontend.md) | [0008](adr/0008-spa-react-vite-typescript-com-react-router.md), [0009](adr/0009-ui-com-tailwind-shadcn-e-identidade-dudupay.md), [0010](adr/0010-formularios-com-react-hook-form-e-zod.md), [0011](adr/0011-estado-de-servidor-com-tanstack-query.md), [0012](adr/0012-mock-da-api-com-json-server-e-wrapper.md), [0013](adr/0013-testes-do-front-com-vitest-testing-library-e-msw.md), [0014](adr/0014-notificacoes-persistidas-com-polling.md) |
| F1 — Fundação (Vite + React + TS, Tailwind v4, shadcn/ui, tokens claro/escuro, Vitest) | ⏳ | — | [0008](adr/0008-spa-react-vite-typescript-com-react-router.md), [0009](adr/0009-ui-com-tailwind-shadcn-e-identidade-dudupay.md), [0013](adr/0013-testes-do-front-com-vitest-testing-library-e-msw.md) |
| F2 — Mock da API (json-server + wrapper com as regras e o seed) | ⏳ | — | [0012](adr/0012-mock-da-api-com-json-server-e-wrapper.md) |
| F3 — Dados e sessão (cliente HTTP, TanStack Query, rotas protegidas) | ⏳ | — | [0011](adr/0011-estado-de-servidor-com-tanstack-query.md) |
| F4 — Login e cadastro | ⏳ | — | [0010](adr/0010-formularios-com-react-hook-form-e-zod.md) |
| F5 — Layout e dashboard | ⏳ | — | — |
| F6 — Transferência | ⏳ | — | — |
| F7 — Perfil editável | ⏳ | — | — |
| F8 — Drawer de notificações (polling) | ⏳ | — | [0014](adr/0014-notificacoes-persistidas-com-polling.md) |
| F9 — Container do front no `docker compose` | ⏳ | — | — |
| Notificações em tempo real (SSE ou SignalR, substituindo o polling) | ⏳ | — | [0014](adr/0014-notificacoes-persistidas-com-polling.md) |

## Backend exigido pelo front

Itens que o contrato do [frontend.md](frontend.md#impacto-no-backend-itens-derivados) cobra do backend. O escopo do projeto agora é maior que o do enunciado ([ADR 0008](adr/0008-spa-react-vite-typescript-com-react-router.md)).

| Item | Status | Spec | ADRs |
| --- | --- | --- | --- |
| Cadastro de usuários com saldo inicial (`welcome_credit`) | ⏳ | — | — |
| Autenticação (login e token; decisão de JWT em ADR) | ⏳ | — | — |
| Endpoints de leitura (`/users/me`, busca, `/wallets/me`, `/transactions/me`, resumo) | ⏳ | — | — |
| Notificações persistidas (`/notifications`) | ⏳ | — | [0014](adr/0014-notificacoes-persistidas-com-polling.md) |
| `payer` validado contra o usuário autenticado | ⏳ | — | — |
| CORS para o front | ⏳ | — | — |
| Erros em Problem Details com os `code` do contrato | ⏳ | — | — |
