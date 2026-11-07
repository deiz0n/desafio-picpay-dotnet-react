# Desafio Back-end — Pagamento Simplificado

Projeto de estudos: API RESTful de transferências entre usuários comuns e lojistas, com autorizador externo, transação com rollback e notificação assíncrona.

> Instruções canônicas para todos os agentes (Claude Code, Antigravity, Codex). Skills em `.agents/skills/`, regras em `.agents/rules/`.

## ⚠️ Modo didático (o usuário está aprendendo C#/.NET)

- **Dentro de `/goal`:** o agente implementa código **e** testes daquele objetivo, mas **para em cada checkpoint** para explicar o que fez, os conceitos de C#/.NET e o porquê.
- **Fora de `/goal`:** o agente explica, orienta e revisa; código só com pedido explícito, e só o que foi pedido.

Regras completas: [`.agents/rules/didatico.md`](.agents/rules/didatico.md).

## Stack e arquitetura

C# / .NET 10, ASP.NET Core com Controllers, Clean Architecture no padrão do CVL (`Pagamentos.Api` → `Application` → `Domain` ← `Repository` / `Infrastructure`), Handlers sem MediatR, Result pattern, PostgreSQL + Dapper, migrations SQL, Unit of Work. Testes: xUnit + `WebApplicationFactory` + Testcontainers. Detalhes e porquês: ADRs 0002–0007.

- Enunciado e regras de negócio: **`docs/challenge.md`** (skill `picpay-challenge` resume).
- O que falta entregar: **`docs/roadmap.md`** — confira antes de tratar um pedido como feature nova.
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
6. Responder em **Português Brasil**.

## Comandos

> A solução ainda não existe. O primeiro `/goal` que criá-la deve confirmar/ajustar estes caminhos.

| Ação | Comando |
| --- | --- |
| Build | `dotnet build backend/Pagamentos.sln` |
| Testes (todos) | `dotnet test backend/Pagamentos.sln` — integração exige Docker rodando |
| Testes filtrados | `dotnet test backend/Pagamentos.sln --filter "FullyQualifiedName~NomeDaClasse"` |
| Formatação | `dotnet format backend/Pagamentos.sln --verify-no-changes` |
| PR | `gh pr create --base develop --title "…" --body-file <arquivo>` |

