# Template — SPEC final (`docs/specs/NN-slug.md`)

Escrita **depois** de implementar e com os testes verdes. Descreve o que **foi** construído e como funciona hoje — não o plano. Seja factual; nada de "deveria".

````markdown
# NN — Título

- **Status:** done
- **Data:** YYYY-MM-DD
- **Branch / PR:** `feat/NN-slug` · [#PR](url)
- **Item do roadmap:** … | —
- **ADRs:** [NNNN — título](../adr/NNNN-slug.md) | —

## Objetivo

Uma ou duas frases: o que mudou para quem usa a API e por quê.

## Decisões da entrevista

| Pergunta | Resposta |
| -------- | -------- |
| ...      | ...      |

## Comportamento implementado

### Contrato

```http
POST /rota
Content-Type: application/json

{ ... }
```

| Situação | Status | Corpo                              |
| -------- | ------ | ---------------------------------- |
| Sucesso  | 201    | ...                                |
| ...      | ...    | `application/problem+json` com ... |

### Fluxo

1. ...
2. ...

### Banco de dados

- Migrations: `database/migrations/NN-slug.sql` — o que criam/alteram.
- Tabelas/constraints relevantes.

### Código

| Camada                      | Arquivos principais |
| --------------------------- | ------------------- |
| Domain                      | ...                 |
| Application                 | ...                 |
| Repository / Infrastructure | ...                 |
| Api                         | ...                 |

## Critérios de aceite → testes

| Critério | Teste                             | Tipo                 |
| -------- | --------------------------------- | -------------------- |
| ...      | `Classe.Metodo_Cenario_Resultado` | unidade / integração |

## Fora de escopo / melhorias futuras

- ... (inclui 🟡 do code review adiados)

## Histórico

- YYYY-MM-DD: entregue via /goal ([#PR](url))
````

## Em ajustes posteriores

Atualize as seções de comportamento para refletir o estado atual e acrescente ao **Histórico**:

```markdown
- YYYY-MM-DD: {o que mudou} — {por quê}; tentado e descartado: {…} ([#PR](url))
```
