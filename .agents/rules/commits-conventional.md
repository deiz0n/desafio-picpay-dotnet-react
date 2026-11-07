---
description: Padrão de commits (Conventional Commits) e de branches
trigger: always_on
---

# Commits — Conventional Commits

Sempre que criar commit neste repo, use o formato:

```
tipo: descrição curta do que mudou
```

## Tipos

| Tipo | Quando usar |
|------|-------------|
| `feat` | Nova funcionalidade / comportamento visível |
| `fix` | Correção de bug |
| `config` | Configuração, env, pipeline, tooling de deploy |
| `refactor` | Mudança de código sem alterar comportamento |
| `docs` | Só documentação (SPEC, ADR, roadmap, README) |
| `style` | Formatação, lint cosmético (sem lógica) |
| `test` | Só testes (sem código de produção) |
| `chore` | Manutenção genérica (deps, limpeza) que não se encaixa acima |
| `perf` | Melhoria de performance |

## Regras

- Mensagem em **português** ou inglês — preferir português curto e claro.
- Só a **primeira linha** obrigatória; corpo opcional se precisar explicar o porquê.
- **Uma fatia do `/goal` = um commit**, com o teste e o código juntos (tipo `feat`/`fix`/`refactor`, conforme a fatia).
- No fim do `/goal`, um commit `docs:` com ADRs, SPEC e roadmap.
- Correções vindas do `/code-review` → commit próprio (`fix: …` ou `refactor: …`), não `--amend` em commit já mostrado ao usuário.
- Exemplos bons:
  - `feat(transfer): rejeitar lojista como pagador`
  - `fix(transfer): não debitar quando o autorizador der timeout`
  - `test(carteira): cobrir débito com saldo exato`
  - `docs: spec 03 e ADR 0008 do lock pessimista`
- Exemplos ruins: `ajuste`, `wip`, `update`, `correções`.

## Escopo opcional

`feat(transfer): …`, `fix(api): …`, `config(ci): …` — ok quando ajuda a achar a área.

# Branches

- Formato: `tipo/NN-slug` — `NN` é o número da SPEC que o goal vai gerar (`feat/03-endpoint-transfer`, `fix/04-timeout-autorizador`, `docs/05-readme`).
- Sempre criada a partir de `develop` atualizada (`git switch develop && git pull --ff-only`).
- PR sempre com base `develop`. Nunca push direto em `develop`/`main`, nunca `--force` em branch compartilhada.

