# Documentação

Fonte de verdade do projeto. Leia antes de decidir estrutura, convenções ou arquitetura.

| Documento | Conteúdo |
| --- | --- |
| [challenge.md](challenge.md) | Enunciado do desafio, regras de negócio, mocks e notas técnicas |
| [roadmap.md](roadmap.md) | Competências a praticar, com status e a SPEC que entregou cada uma |
| [specs/](specs/README.md) | SPECs finais — registro do que foi construído por cada `/goal` |
| [adr/](adr/README.md) | Architecture Decision Records — decisões arquiteturais e seus porquês |

Fluxo de trabalho dos agentes: [`AGENTS.md`](../AGENTS.md) (`/goal`, `/code-review`, `/adr`).

## SPEC × ADR

| | SPEC | ADR |
| --- | --- | --- |
| Responde | **O que foi construído** e como funciona hoje (uma entrega) | **Por que** escolhemos X em vez de Y |
| Quando nasce | No **fim** do `/goal`, com código e testes verdes | Quando a decisão é tomada (entrevista do `/goal` ou `/adr`) |
| Vida útil | Enquanto o comportamento existir; ajustes vão no Histórico | Vale enquanto a decisão estiver em vigor |
| Mudança | Atualizada por um novo `/goal` (comportamento atual + Histórico) | Imutável depois de `accepted`; mudar = novo ADR que substitui |
| Exemplo | `03-endpoint-transfer.md` | `0008-lock-pessimista-nas-carteiras` |
