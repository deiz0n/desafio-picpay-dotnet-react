# Entrevista do /goal

Objetivo: chegar ao **checklist de confiança** todo ✅ fazendo o mínimo de perguntas. Antes de perguntar, procure a resposta em `docs/` e no código — pergunte só o que não dá para descobrir.

## Checklist de confiança

Mostre a cada rodada:

```markdown
| Item | Status | Resumo |
| --- | --- | --- |
| Objetivo | ✅/⬜ | uma frase do que muda para quem usa a API |
| Escopo | ✅/⬜ | inclui / fora |
| Comportamento | ✅/⬜ | caminho feliz + cada erro com status HTTP e corpo |
| Dados | ✅/⬜ | tabelas/colunas/constraints; migration? seed? |
| Critérios de aceite | ✅/⬜ | cada um vira ≥ 1 teste nomeado |
| Decisões | ✅/⬜ | decisões arquiteturais (→ ADR) ou "nenhuma" |
| Riscos | ✅/⬜ | concorrência, serviços externos, idempotência |
```

Um item só fica ✅ quando você conseguiria **escrever o teste** dele sem perguntar mais nada.

## Formato da rodada

```markdown
**Rodada N** — faltam: Comportamento, Dados

1. {pergunta}? _Sugestão: {resposta provável e porquê}_
2. ...
(no máximo 5)
```

Prefira perguntas fechadas com opções a perguntas abertas. Agrupe por tema.

## Banco de perguntas (use só as que se aplicam)

### Objetivo e escopo
- Qual problema isto resolve / qual item do roadmap entrega?
- O que fica explicitamente de fora deste goal?
- É um ajuste de algo já entregue (qual SPEC)?

### Contrato e erros
- Rota, verbo, request e response? Mantém o contrato do desafio (`POST /transfer` com `value`, `payer`, `payee`)?
- Status HTTP de cada erro (sugestão do desafio: 201 · 422 validação/saldo/lojista · 404 usuário · 403 negado · 503 autorizador fora)?
- Corpo de erro em Problem Details (`application/problem+json`)? Quais campos?

### Dinheiro e dados
- `decimal` + `NUMERIC(15,2)` ou centavos `long`/`BIGINT`? (decisão pendente → ADR)
- Quais tabelas/colunas/constraints (`UNIQUE` documento/e-mail, `CHECK (saldo >= 0)`)?
- Precisa de migration nova? De seed para testes/uso local?

### Transação e concorrência
- Lock pessimista (`SELECT … FOR UPDATE` em ordem de id) ou otimista (versão)? (decisão pendente → ADR)
- O autorizador é chamado dentro ou fora da transação? (decisão pendente → ADR)
- O que deve acontecer com duas transferências simultâneas do mesmo pagador?

### Serviços externos
- Timeout do autorizador? Timeout = não autorizado (403) ou indisponível (503)?
- Notificação: fila em tabela (outbox) + `BackgroundService`, ou mensageria? Política de retry/backoff? (decisão pendente → ADR)

### Idempotência e observabilidade
- Header `Idempotency-Key` entra neste goal?
- Logs/correlation id: algo específico além do padrão do ADR 0003?

### Critérios de aceite
- "Está pronto quando…" — liste os cenários; para cada um, unidade ou integração?
- Algum cenário exige validação específica no banco (ex.: nenhuma linha em `transferencias` após rollback)?

