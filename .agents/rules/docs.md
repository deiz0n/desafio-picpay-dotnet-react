---
description: docs/ é a fonte de verdade — SPECs finais (registro do que foi feito), ADRs e roadmap
trigger: always_on
---

# Documentação para agentes

## Ordem de leitura

1. `docs/README.md` — índice e diferença SPEC × ADR
2. `docs/challenge.md` — enunciado e regras de negócio
3. `docs/adr/README.md` — decisões em vigor
4. `docs/roadmap.md` — o que falta entregar
5. `docs/specs/` — SPECs finais das áreas que o objetivo toca (o que já foi construído e como funciona hoje)

## SPEC × ADR

- **SPEC** = registro do **que foi construído** e como funciona hoje. Escrita **no fim** do `/goal`, depois que o código e os testes estão verdes. Uma por goal: `docs/specs/NN-slug.md`. Não existe mais `plan.md`/`tasks.md`.
- **ADR** = **por que** uma decisão foi tomada. Vale enquanto a decisão estiver em vigor.

## Obrigatório

- Não contradizer ADR `accepted`. Se precisar, proponha um novo ADR que o substitua.
- Nunca editar o conteúdo de um ADR `accepted` — só a linha de status quando for substituído.
- SPEC e ADR se referenciam mutuamente (campo **ADRs** na SPEC; **Spec de origem** no ADR).
- Ajuste posterior em algo já entregue → novo `/goal` que atualiza a SPEC existente: comportamento atual + **Histórico** (o que mudou, o que foi tentado e descartado, link do PR).
- Item de roadmap entregue → atualizar a linha (status + SPEC + ADRs) **no mesmo PR**.
- Item de roadmap abandonado vira `❌ desistimos` com data e motivo — não se apaga a linha.
- O rascunho `.goal/` é estado de trabalho local (ignorado pelo git) — nunca é fonte de verdade nem é referenciado em `docs/`.

