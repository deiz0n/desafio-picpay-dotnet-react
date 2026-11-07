---
name: goal
description: >
  Fluxo completo de entrega de um objetivo neste projeto: entrevista até ter confiança,
  branch a partir de develop, implementação em fatias com teste primeiro e checkpoints
  didáticos, dotnet test (unidade + integração validando o banco), code review do diff,
  ADR + SPEC final como documentação, commit, push e PR para develop.
  Use quando o usuário invocar /goal ou pedir para implementar uma feature/correção de ponta a ponta.
argument-hint: "{objetivo}"
---

# /goal {objetivo}

Entrega um objetivo de ponta a ponta. Regras que valem o tempo todo:
[`didatico.md`](../../rules/didatico.md) · [`testes.md`](../../rules/testes.md) · [`docs.md`](../../rules/docs.md) · [`commits-conventional.md`](../../rules/commits-conventional.md).

## Entrada

`$ARGUMENTS` = o objetivo (pode ser curto). Vazio → pergunte o objetivo antes de qualquer coisa.

## Estado de trabalho (`.goal/`)

O progresso fica em `.goal/NN-slug.md` (ignorado pelo git) para permitir retomar em outra sessão. Atualize-o **ao fim de cada fase e de cada fatia**:

```markdown
# NN-slug — {objetivo}
- Fase atual: 4 (fatia 2 de 5)
- Branch: feat/NN-slug
## Entendimento aprovado
(resumo do checkpoint 1)
## Entrevista
| Pergunta | Resposta |
## Cenários de teste
- [x] `Classe.Metodo_Cenario_Resultado` (unidade)
- [ ] ...
## Fatias
- [x] 1. ... (commit abc123)
- [ ] 2. ...
## Decisões para ADR
- ...
## Review
- 🔴 ... / 🟡 adiado: ...
```

## Fase 0 — Pré-checagem

Rode e, se algo falhar, **pare** e mostre o comando para corrigir:

| Checagem | Comando | Falhou → |
| --- | --- | --- |
| É repo git | `git rev-parse --show-toplevel` | pedir a URL / `git init` + `git remote add origin …` |
| Árvore limpa | `git status --porcelain` vazio | pedir para commitar/stash (nunca descartar mudanças do usuário) |
| `develop` existe | `git fetch origin` + `git rev-parse --verify origin/develop` | perguntar se deve criar a partir de `main` |
| GitHub CLI | `gh auth status` | `gh auth login` (só é bloqueante na fase 8) |
| Docker | `docker info` | bloqueante só se houver/for haver testes de integração |
| .NET | `dotnet --version` (≥ 10) | instalar o SDK |

Depois: se existir arquivo em `.goal/`, mostre o resumo e pergunte **retomar** ou **começar outro**. Retomar = pular para a fase registrada.

## Fase 1 — Contexto

1. Leia `docs/challenge.md`, `docs/roadmap.md`, `docs/adr/README.md` e as SPECs em `docs/specs/` das áreas afetadas.
2. Explore o código das áreas que o objetivo toca (só o necessário).
3. Identifique: item do roadmap entregue (se houver), ADRs que restringem a solução, SPEC existente que será atualizada (ajuste) ou nova SPEC.
4. Calcule `NN` (próximo número em `docs/specs/`, 2 dígitos; se for ajuste de uma SPEC existente, reutilize o número dela) e um `slug` kebab-case curto.

## Fase 2 — Entrevista (até ter confiança)

Siga [`references/entrevista.md`](references/entrevista.md).

- Rodadas de **no máximo 5 perguntas**, numeradas, com sua sugestão de resposta quando houver uma óbvia.
- Pergunte **só o que não dá para descobrir** lendo código e docs.
- A cada rodada mostre o **checklist de confiança** com ✅/⬜. Continue até todos ✅ — sem limite de rodadas.
- Objetivo grande demais (mais de ~6 fatias ou várias áreas independentes) → proponha fatiar em mais de um goal e peça confirmação.
- Decisão arquitetural (critérios em `docs/adr/README.md`) → apresente 2–3 alternativas com prós/contras e recomendação; o usuário escolhe.

### Checkpoint 1 — Entendimento

Apresente e **espere aprovação explícita** antes de criar branch ou escrever código:

1. Objetivo em 1 frase + escopo (inclui / fora).
2. Comportamento: caminho feliz e cada erro (status HTTP + corpo).
3. Impacto no banco (tabelas, constraints, migration).
4. **Cenários de teste**: tabela critério de aceite → nome do teste → tipo (unidade/integração).
5. **Fatias** planejadas, na ordem (de dentro para fora: Domain → Application → Repository/Infrastructure → Api).
6. Decisões que viram ADR.

Grave `.goal/NN-slug.md`.

## Fase 3 — Branch

```bash
git switch develop
git pull --ff-only
git switch -c feat/NN-slug   # ou fix/, refactor/, docs/ conforme o objetivo
```

## Fase 4 — Fatias (repetir para cada uma)

1. **Teste vermelho:** escreva o(s) teste(s) da fatia (convenção em `testes.md`) e rode filtrado. Confirme que **falha pelo motivo certo**. Se a fatia cria a estrutura (solution/projetos), o "teste vermelho" é o primeiro teste que não compila/não passa até a estrutura existir.
2. **Código mínimo** para passar, respeitando ADRs (camadas, Result pattern, Unit of Work, nomes em português).
3. **Verde:** rode o teste filtrado, depois `dotnet test` completo — nada que passava pode quebrar.
4. **Checkpoint didático** no formato de `didatico.md`. Espere "ok / segue" ou ajustes. Ajuste pedido → volte ao passo 1 desta fatia.
5. **Commit** da fatia (teste + código juntos), mensagem conforme `commits-conventional.md`.
6. Atualize `.goal/`.

Mudança de escopo no meio → volte ao checkpoint 1, atualize cenários/fatias no `.goal/` e só depois continue.

## Fase 5 — Verificação final

Todos precisam passar (comandos em `AGENTS.md`):

1. `dotnet build` — sem erros e sem warnings novos.
2. `dotnet format --verify-no-changes` — se falhar, rode `dotnet format` e faça commit `style: …`.
3. `dotnet test` completo — unidade + integração (Docker rodando).
4. **Rastreabilidade:** cada critério de aceite do checkpoint 1 tem um teste verde. Monte a tabela critério → teste → ✅. Critério sem teste = volte à fase 4.

Falhou → corrija numa nova fatia (com teste) e repita a fase 5.

## Fase 6 — Code review

Execute o skill [`code-review`](../code-review/SKILL.md) sobre `develop...HEAD`.

- 🔴 Bloqueante → corrija (fatia nova, com teste que prova a correção, commit `fix:`/`refactor:`) e rode a fase 5 e o review de novo.
- 🟡 Melhoria → pergunte ao usuário: corrigir agora ou registrar em "Fora de escopo / melhorias futuras" da SPEC.
- Só avance com veredito `aprovado` ou `aprovado com ressalvas`.

## Fase 7 — Documentação

1. **ADRs:** para cada decisão arquitetural tomada, siga o skill [`adr`](../adr/SKILL.md) com **Spec de origem** = `docs/specs/NN-slug.md`.
2. **SPEC final:** escreva `docs/specs/NN-slug.md` a partir de [`references/spec-template.md`](references/spec-template.md) descrevendo o que **foi** construído (não o que foi planejado). Ajuste de SPEC existente → atualize o comportamento e acrescente ao **Histórico**.
3. **Índice:** adicione a linha em `docs/specs/README.md`.
4. **Roadmap:** atualize a linha do item entregue (status, SPEC, ADRs).
5. **README / AGENTS.md:** se mudou como rodar, buildar ou testar.
6. Commit `docs: …`.

## Fase 8 — Checkpoint final e PR

Mostre ao usuário e **espere confirmação explícita** antes de qualquer push:

- `git log origin/develop..HEAD --oneline`
- `git diff origin/develop...HEAD --stat`
- Título do PR (formato de commit: `feat(area): …`) e corpo montado com [`references/pr-template.md`](references/pr-template.md).

Com o "ok":

```bash
git push -u origin feat/NN-slug
gh pr create --base develop --head feat/NN-slug --title "…" --body-file <arquivo temporário>
```

Mostre a URL do PR, preencha o número do PR na SPEC (commit `docs: link do PR na spec NN` + push) e apague `.goal/NN-slug.md`.

## Regras

- Responder em Português Brasil.
- Nunca pular o teste vermelho nem apagar/`Skip`/afrouxar teste para ficar verde.
- Nunca escrever código antes do checkpoint 1 aprovado.
- Nunca push sem o checkpoint final; nunca push em `develop`/`main`; nunca `--force`.
- Nunca contradizer ADR `accepted` em silêncio — proponha novo ADR.
- Serviços externos reais (`util.devi.tools`) nunca são chamados em testes.
- Se travar (ambiente, dependência, decisão do usuário) → registre em `.goal/`, explique o bloqueio e pare.

