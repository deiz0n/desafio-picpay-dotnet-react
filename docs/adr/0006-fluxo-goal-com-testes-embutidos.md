# 0006 — Substituir o fluxo SDD pelo `/goal` com testes embutidos

- **Status:** accepted
- **Data:** 2026-11-07
- **Spec de origem:** —
- **Substitui:** —

## Contexto

O harness usava Spec-Driven Development: `/spec` → `/spec-plan` → `/spec-task` (ou `/spec-auto`), gerando `spec.md`, `plan.md` e `tasks.md` **antes** de qualquer código, em modo mentor estrito (o agente não escrevia código). Problemas percebidos:

- três documentos escritos antes de implementar, que envelheciam durante a implementação;
- nenhum teste automatizado embutido no fluxo — "pronto" dependia de verificação manual;
- nenhum code review sistemático do diff nem entrega padronizada (branch + PR);
- harness preso ao Claude Code (`.claude/`), sem funcionar no Antigravity nem no Codex.

## Decisão

Vamos usar um único comando **`/goal {objetivo}`** (skill `.agents/skills/goal/`) com as fases:

1. pré-checagem do ambiente;
2. **entrevista** até um checklist de confiança ficar completo, terminando num checkpoint de entendimento aprovado pelo usuário;
3. branch `tipo/NN-slug` a partir de `develop`;
4. **fatias com teste primeiro** (vermelho → código → verde), cada uma com **checkpoint didático** e commit;
5. verificação final (`dotnet build`, `dotnet format`, `dotnet test` — unidade + integração validando o banco, ADR 0007);
6. **`/code-review`** do diff `develop...HEAD`;
7. documentação **depois** de implementar: ADRs + **SPEC final** (`docs/specs/NN-slug.md`, registro do que foi feito) + roadmap;
8. checkpoint final → push + PR para `develop` via `gh`.

Complementos:

- **Modo didático** substitui o mentor estrito: invocar `/goal` é pedido explícito para o agente escrever código e testes daquele objetivo, com parada obrigatória para explicação a cada fatia.
- **Harness multiagente:** fonte única em `.agents/` (skills e rules lidas nativamente por Antigravity e Codex) + `AGENTS.md`; Claude Code via `CLAUDE.md` (`@AGENTS.md`) e stubs em `.claude/skills/`.
- Estado de trabalho do goal em `.goal/` (ignorado pelo git) para retomar entre sessões.

Este ADR não substitui o [0001](0001-registrar-decisoes-com-adr.md) (ADRs continuam); apenas torna obsoleta a menção do 0001 ao `/spec-plan` como quem propõe ADRs — agora é o `/goal` (entrevista e fase 7) ou o `/adr`.

## Alternativas consideradas

| Alternativa | Prós | Contras |
| --- | --- | --- |
| `/goal` com entrevista, testes e checkpoints didáticos (escolhida) | Um comando; "pronto" provado por testes; documentação reflete o que foi feito; PR padronizado | Perde-se o plano versionado antes do código; o usuário escreve menos código à mão |
| Manter o SDD | Planejamento explícito e versionado | Documentos envelhecem; sem testes nem review embutidos |
| SDD + etapa de testes | Mantém o planejamento formal | Ainda mais cerimônia antes do código; três comandos para uma feature |
| `/goal` totalmente autônomo, sem checkpoints | Mais rápido | Anula o objetivo de aprendizado do projeto |
| Skills duplicados em `.claude/`, `.agents/` (cópia ou symlink) | Cada ferramenta lê arquivos completos | Cópias saem de sincronia; symlinks exigem modo desenvolvedor no Windows e quebram no OneDrive |

## Consequências

### Positivas

- Todo comportamento entregue tem teste automatizado; testes de integração validam o banco.
- SPEC e ADR descrevem o que existe de fato, não o que foi planejado.
- Entrega padronizada: commits por fatia, review, PR para `develop` com rastreabilidade critério → teste.
- O mesmo harness funciona em Claude Code, Antigravity e Codex.

### Negativas / trade-offs

- O plano detalhado vive só no checkpoint 1 e em `.goal/` (não versionado) — o histórico do raciocínio prévio fica na conversa e no PR.
- O usuário digita menos código; o aprendizado depende de ler os checkpoints com atenção (e ele pode assumir uma fatia quando quiser).
- No Antigravity, o nome `/goal` pode colidir com o comando nativo; se ocorrer, renomear o skill (ex.: `/feature`).
- Stubs em `.claude/skills/` precisam ter `name`/`description` sincronizados com o canônico.

### O que fazer se a decisão mudar

Novo ADR. Os skills antigos (`spec`, `spec-plan`, `spec-task`, `spec-auto`) podem ser recuperados do histórico do git.

