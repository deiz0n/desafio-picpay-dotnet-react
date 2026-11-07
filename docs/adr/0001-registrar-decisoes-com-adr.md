# 0001 — Registrar decisões arquiteturais com ADR

- **Status:** accepted
- **Data:** 2026-11-06
- **Spec de origem:** —
- **Substitui:** —

## Contexto

O projeto é de estudos e usa Spec-Driven Development: cada feature tem `spec.md`, `plan.md` e `tasks.md`. As specs descrevem features, mas decisões arquiteturais (stack, representação de dinheiro, concorrência, mensageria) atravessam várias features e continuam valendo depois que a spec fecha. Sem um registro próprio, o "porquê" fica espalhado em históricos de specs e se perde.

## Decisão

Vamos registrar cada decisão arquitetural significativa como um ADR em `docs/adr/NNNN-slug.md`, no formato do [template](0000-template.md), ligado à spec que o originou. O `/spec-plan` propõe ADRs quando o plano envolve decisões desse tipo.

## Alternativas consideradas

| Alternativa | Prós | Contras |
| --- | --- | --- |
| ADR em arquivos Markdown no repositório (escolhida) | Versionado com o código; revisável em PR; leve | Exige disciplina para manter o índice |
| Seção "Decisões" dentro de cada spec | Zero estrutura nova | Decisões transversais ficam presas a uma feature; difícil de achar |
| Só o README | Um lugar único | Vira texto longo sem histórico de mudanças de decisão |

## Consequências

### Positivas

- O porquê de cada escolha fica rastreável e explicável.
- Mudanças de decisão ficam visíveis (ADR substituído, não editado).

### Negativas / trade-offs

- Mais um artefato a manter; o índice em `docs/adr/README.md` precisa ser atualizado.

### O que fazer se a decisão mudar

Novo ADR substituindo este; ADRs existentes permanecem como histórico.
