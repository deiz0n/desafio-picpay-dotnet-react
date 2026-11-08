# 0011 — Estado de servidor com TanStack Query

- **Status:** accepted
- **Data:** 2026-11-08
- **Spec de origem:** —
- **Substitui:** —

## Contexto

Quase tudo que o front mostra vem da API: saldo, movimentações, perfil, notificações. Esses dados precisam de estados de carregando/erro/vazio, cache, invalidação depois de uma transferência (o saldo muda) e atualização periódica (notificações, [ADR 0014](0014-notificacoes-persistidas-com-polling.md)).

## Decisão

Vamos usar **TanStack Query** para o estado de servidor, sobre uma camada fina de cliente HTTP (`fetch` encapsulado, que adiciona o token e converte erros `application/problem+json` em um tipo de erro da aplicação). Estado de UI local continua em `useState`/contexto; **não** haverá Redux nem store global para dados da API.

Depois de uma transferência bem-sucedida, as queries de saldo e de movimentações são invalidadas.

## Alternativas consideradas

| Alternativa | Prós | Contras |
| --- | --- | --- |
| TanStack Query (escolhida) | Cache, invalidação, retry e polling prontos; muito usado | Conceito novo (query keys, staleTime) a aprender |
| `useEffect` + `fetch` manual | Sem dependência | Reimplementa cache, corrida de requisições e polling — fonte comum de bugs |
| Redux Toolkit (+ RTK Query) | Ecossistema grande | Mais cerimônia para um app pequeno |
| SWR | Mais simples | Menos recursos para mutações e invalidação granular |

## Consequências

### Positivas

- Estados de loading/erro padronizados nas telas.
- Polling das notificações vira uma opção da query (`refetchInterval`).
- Invalidação explícita após mutações evita saldo desatualizado.

### Negativas / trade-offs

- Exige disciplina com query keys e `staleTime`.
- Mais uma biblioteca no bundle.

### O que fazer se a decisão mudar

Novo ADR. Como as telas consomem hooks próprios (`useSaldo`, `useNotificacoes`…), trocar a biblioteca fica restrito a esses hooks.
