# 0004 — PostgreSQL com Dapper e migrations em SQL

- **Status:** accepted
- **Data:** 2026-11-06
- **Spec de origem:** —
- **Substitui:** —

## Contexto

O desafio exige transação com rollback e garantia de saldo sob concorrência — o que exige controle explícito do SQL (locks, constraints). O CVL usa PostgreSQL + Dapper + Npgsql, com schema mantido em migrations SQL versionadas.

## Decisão

Vamos usar **PostgreSQL 17**, acessado via **Dapper + Npgsql** nos `*Repositorio`, com o schema definido em **migrations SQL** (`database/migrations/NN-slug.sql`), seguindo as convenções do CVL: `snake_case`, português, tabelas no plural, `data_criacao timestamptz NOT NULL DEFAULT now()`. A API não cria nem altera schema.

## Alternativas consideradas

| Alternativa | Prós | Contras |
| --- | --- | --- |
| Dapper + SQL explícito (escolhida) | SQL visível (`FOR UPDATE`, `CHECK`); mesmo padrão do CVL; "sem mágica", como pede o desafio | Mapeamento e transações manuais |
| Entity Framework Core | Produtividade; migrations geradas | Abstrai o SQL que o desafio quer ver; locks menos explícitos |
| MySQL | Também comum | Diverge do CVL |

## Consequências

### Positivas

- Controle total sobre transação, isolamento e locks.
- Constraints do banco (`UNIQUE`, `CHECK (saldo >= 0)`) como rede de segurança.

### Negativas / trade-offs

- Transação precisa atravessar vários repositórios — exige uma decisão sobre **Unit of Work** (futuro ADR).
- Mais código de mapeamento.

### O que fazer se a decisão mudar

Novo ADR. Repositórios estão atrás de interfaces do Domain, então trocar Dapper por EF afeta só `Pagamentos.Repository`.
