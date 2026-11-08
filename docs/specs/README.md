# Specs

Registro do **que foi construído**: uma SPEC por `/goal`, escrita no fim, depois que o código e os testes estão verdes. Descreve o comportamento implementado, o contrato, o banco e a tabela critério de aceite → teste.

Não há mais `spec.md` / `plan.md` / `tasks.md` antes do código (fluxo SDD descontinuado — [ADR 0006](../adr/0006-fluxo-goal-com-testes-embutidos.md)).

## Como nasce

`/goal {objetivo}` → entrevista → fatias com teste primeiro → `/code-review` → **SPEC final** + ADRs → PR para `develop`.

Template: [`.agents/skills/goal/references/spec-template.md`](../../.agents/skills/goal/references/spec-template.md).

## Arquivos

```text
docs/specs/
  01-slug.md
  02-slug.md
```

- `NN` = número sequencial (2 dígitos); igual ao da branch `tipo/NN-slug`.
- Status: `done` ou `superseded by NN` (quando uma SPEC posterior a substitui por completo).

## Regras

- Ajuste em algo já entregue → novo `/goal` que **atualiza a SPEC existente** (comportamento atual + Histórico com o que mudou, o que foi descartado e o PR), em vez de criar outra.
- SPEC e ADR se referenciam mutuamente (campo **ADRs** na SPEC; **Spec de origem** no ADR).
- SPEC que entrega item do [roadmap](../roadmap.md) → atualizar a linha no mesmo PR.

## Índice

| Nº | Título | PR | ADRs |
| --- | --- | --- | --- |
| [01](01-fundacao-frontend.md) | Fundação do front-end (DuduPay) | [#4](https://github.com/deiz0n/desafio-picpay-dotnet-react/pull/4) | [0008](../adr/0008-spa-react-vite-typescript-com-react-router.md), [0009](../adr/0009-ui-com-tailwind-shadcn-e-identidade-dudupay.md), [0013](../adr/0013-testes-do-front-com-vitest-testing-library-e-msw.md) |
