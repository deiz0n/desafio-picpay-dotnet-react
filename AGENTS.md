# Desafio Back-end — Pagamento Simplificado

Projeto de estudos: API RESTful de transferências entre usuários comuns e lojistas, com autorizador externo, transação com rollback e notificação assíncrona.

> Instruções canônicas para todos os agentes (Claude Code, Antigravity, Codex). Skills em `.agents/skills/`, regras em `.agents/rules/`.

## ⚠️ Modo didático (o usuário está aprendendo C#/.NET)

- **Dentro de `/goal`:** o agente implementa código **e** testes daquele objetivo, mas **para em cada checkpoint** para explicar o que fez, os conceitos de C#/.NET e o porquê.
- **Fora de `/goal`:** o agente explica, orienta e revisa; código só com pedido explícito, e só o que foi pedido.

Regras completas: [`.agents/rules/didatico.md`](.agents/rules/didatico.md).

## Stack e arquitetura

C# / .NET 10, ASP.NET Core com Controllers, Clean Architecture no padrão do CVL (`Pagamentos.Api` → `Application` → `Domain` ← `Repository` / `Infrastructure`), Handlers sem MediatR, Result pattern, PostgreSQL + Dapper, migrations SQL, Unit of Work. Testes: xUnit + `WebApplicationFactory` + Testcontainers. Detalhes e porquês: ADRs 0002–0007.

Front-end **DuduPay** (`frontend/`, ainda não criado): SPA React + Vite + TypeScript, `react-router-dom`, Tailwind v4 + shadcn/ui, React Hook Form + Zod, TanStack Query, Vitest + Testing Library + MSW (sem E2E), construído antes do backend sobre um mock json-server. Plano, contrato da API e fatias: **`docs/frontend.md`**; porquês: ADRs 0008–0014.

- Enunciado e regras de negócio: **`docs/challenge.md`** (skill `picpay-challenge` resume).
- O que falta entregar: **`docs/roadmap.md`** — confira antes de tratar um pedido como feature nova (inclui as fatias F1–F9 do front).
- Contrato da API e plano do front: **`docs/frontend.md`**.
- Decisões em vigor: **`docs/adr/README.md`** — respeite os ADRs `accepted`.
- O que já foi construído: **`docs/specs/`** — SPECs finais de cada `/goal`.

## Fluxo de trabalho

```text
/goal {objetivo}
  → pré-checagem → entrevista (até ter confiança) → checkpoint
  → branch feat/NN-slug a partir de develop
  → fatias: teste vermelho → código → teste verde → checkpoint didático → commit
  → dotnet build/format/test verdes
  → /code-review do diff develop...HEAD
  → ADRs + SPEC final + roadmap
  → checkpoint final → push + PR para develop
```

Skill: [`.agents/skills/goal/SKILL.md`](.agents/skills/goal/SKILL.md). Também podem ser usados sozinhos:

| Comando | Função |
| --- | --- |
| `/code-review [base]` | Revisa o diff `base...HEAD` (padrão `develop`) + mudanças não commitadas |
| `/adr {decisão}` | Registra uma decisão arquitetural em `docs/adr/` |

## Regras

1. `docs/` é a fonte de verdade. Leia [`.agents/rules/docs.md`](.agents/rules/docs.md) antes de decidir estrutura ou convenções.
2. Nenhuma fatia é concluída sem um teste que a prove; testes de integração validam o banco ([`.agents/rules/testes.md`](.agents/rules/testes.md)).
3. Decisão arquitetural nova (tecnologia, padrão, trade-off difícil de reverter) → confirmar com o usuário e registrar ADR. Não decidir em silêncio.
4. Commits e branches: [`.agents/rules/commits-conventional.md`](.agents/rules/commits-conventional.md).
5. Nunca fazer push direto em `develop`/`main`, nem `--force`. PR sempre com base `develop`. Push e PR só após confirmação do usuário.
6. Hooks de git versionados em `.githooks/` (bloqueio de `.env`, build do backend, lint/tipos do front). Ativar uma vez por clone com `git config core.hooksPath .githooks`. Nunca usar `--no-verify`: hook falhou → corrija a causa ([ADR 0015](docs/adr/0015-guardas-de-commit-com-git-hooks.md)).
7. Responder em **Português Brasil**.

## Comandos

### Backend

> Solução em `backend/Pagamentos.slnx` (formato `.slnx`, padrão do .NET 10): `src/` com os 5 projetos do ADR 0003 e `tests/` com `Pagamentos.UnitTests` e `Pagamentos.IntegrationTests`. Migrations SQL em `database/migrations/`.

| Ação | Comando |
| --- | --- |
| Postgres local (exige `.env` a partir do `.env.example`; aplica as migrations num volume novo) | `docker compose up -d postgres` |
| Connection string da API (uma vez por máquina) | `dotnet user-secrets set "ConnectionStrings:Pagamentos" "Host=localhost;Port=5432;Database=pagamentos;Username=<usuario>;Password=<senha>" --project backend/src/Pagamentos.Api` |
| Build | `dotnet build backend/Pagamentos.slnx` |
| Testes (todos) | `dotnet test backend/Pagamentos.slnx` — integração exige Docker rodando |
| Testes filtrados | `dotnet test backend/Pagamentos.slnx --filter "FullyQualifiedName~NomeDaClasse"` |
| Formatação | `dotnet format backend/Pagamentos.slnx --verify-no-changes` |
| PR | `gh pr create --base develop --title "…" --body-file <arquivo>` |

### Front-end (`frontend/`, rode dentro da pasta)

> Node ≥ 22.12 ou ≥ 24 (`frontend/.nvmrc` = 24). Instalação: `npm install` (as versões são exatas, `save-exact`). O mock da API (`npm run mock`) chega na fatia F2.

| Ação | Comando |
| --- | --- |
| Rodar em desenvolvimento | `npm run dev` |
| Testes (todos) | `npm test` (Vitest) |
| Testes filtrados | `npm test -- <trecho do arquivo ou do nome>` |
| Testes em modo observação | `npm run test:watch` |
| Tipos | `npm run typecheck` |
| Lint | `npm run lint` |
| Formatação (verificar / aplicar) | `npm run format:check` / `npm run format` |
| Build | `npm run build` |

Antes de concluir uma fatia do front: `npm run lint`, `npm run format:check`, `npm run typecheck`, `npm test` e `npm run build` verdes.

