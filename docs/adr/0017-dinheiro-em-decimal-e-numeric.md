# 0017 — Representar dinheiro com `decimal` e `NUMERIC(15,2)`

- **Status:** accepted
- **Data:** 2026-11-20
- **Spec de origem:** [02-fundacao-backend-cadastro](../specs/02-fundacao-backend-cadastro.md)
- **Substitui:** —

## Contexto

O cadastro cria a carteira com saldo inicial de R$ 1.000,00, então a representação de dinheiro precisava ser decidida neste goal (estava como decisão pendente). O contrato da API ([frontend.md](../frontend.md)) e o enunciado trafegam valores em reais com casas decimais (`"value": 100.0`). Ponto flutuante binário (`double`/`float`) não representa valores como `0.1` exatamente e não serve para dinheiro.

## Decisão

Vamos usar **`decimal` no C#** e **`NUMERIC(15,2)` no Postgres** para todo valor monetário, em reais. O banco garante saldo não negativo com `CHECK (saldo >= 0)`.

## Alternativas consideradas

| Alternativa | Prós | Contras |
| --- | --- | --- |
| `decimal` + `NUMERIC(15,2)` (escolhida) | Exato em base 10; mapeamento nativo no Npgsql/Dapper; bate com o contrato sem conversão; leitura direta no banco | Precisa garantir 2 casas na borda (o `NUMERIC(15,2)` arredonda o que vier com mais casas) |
| Centavos em `long` + `BIGINT` | Inteiros: soma exata e rápida; sem arredondamento | Conversão reais↔centavos em toda borda (API, logs, relatórios); fácil esquecer e errar por 100× |
| `double` + `DOUBLE PRECISION` | Simples | Inexato: `0.1 + 0.2 != 0.3`; inaceitável para dinheiro |

## Consequências

### Positivas

- Nenhuma conversão entre API, Domain e banco.
- `NUMERIC(15,2)` comporta até R$ 9.999.999.999.999,99, muito além do necessário.

### Negativas / trade-offs

- A validação de "no máximo 2 casas" no valor de uma transferência fica por conta do Domain (goal da transferência).
- O front trabalha em centavos internamente ([frontend.md](../frontend.md)); a conversão fica na borda do front.

### O que fazer se a decisão mudar

Novo ADR; migration convertendo `saldo` (e valores de transferências) para `BIGINT` em centavos (`saldo * 100`); trocar `decimal` por `long` nas entidades e converter na Api para manter o contrato.
