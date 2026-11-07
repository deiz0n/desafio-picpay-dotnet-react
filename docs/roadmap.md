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
