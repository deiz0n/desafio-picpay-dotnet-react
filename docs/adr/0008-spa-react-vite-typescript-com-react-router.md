# 0008 — Adotar SPA React + Vite + TypeScript com react-router-dom

- **Status:** accepted
- **Data:** 2026-11-08
- **Spec de origem:** —
- **Substitui:** —

## Contexto

O [enunciado](../challenge.md#fora-do-escopo) coloca frontend, fluxo de cadastro e autenticação **fora do escopo**, e o [ADR 0007](0007-xunit-com-testcontainers.md) descartou E2E porque "não há frontend no escopo". O projeto é de estudos e o repositório se chama `dotnet-react`: o objetivo agora é praticar React junto com o backend .NET, entregando uma aplicação de ponta a ponta chamada **DuduPay**.

O front passa a ter: login, cadastro, dashboard com saldo, transferência, perfil editável e um drawer de notificações. O `challenge.md` continua como cópia fiel do enunciado; o escopo do **projeto** passa a ser maior que o do enunciado.

## Decisão

Vamos criar uma **SPA** em `frontend/`, com **React**, **Vite** e **TypeScript** (`strict`), e **`react-router-dom`** para as rotas (incluindo rotas protegidas por sessão). Gerenciador de pacotes: `npm`.

Cadastro, login e perfil entram no escopo do projeto. A **autenticação real** (JWT ou similar) não é decidida aqui: virá num goal de backend com ADR próprio. Até lá, o login usa usuários mockados ([ADR 0012](0012-mock-da-api-com-json-server-e-wrapper.md)).

## Alternativas consideradas

| Alternativa | Prós | Contras |
| --- | --- | --- |
| Vite + React + TS + react-router-dom (escolhida) | Leve, rápido, padrão de mercado; poucas camadas escondidas; combina com API separada | Sem SSR/SEO (irrelevante para um app logado) |
| Next.js | Ecossistema grande; SSR e roteamento por arquivos | Mais "mágica" e convenções; SSR não agrega a um app logado com API própria |
| Blazor (C# no front) | Mesma linguagem do backend | Foge do objetivo de praticar React |
| TanStack Router | Rotas com tipagem forte | Menos comum; o objetivo é aprender o roteador de referência |
| JavaScript puro (sem TS) | Menos configuração | Perde checagem de tipos num domínio com dinheiro e contratos de API |

## Consequências

### Positivas

- Duas aplicações independentes (`backend/` e `frontend/`) com contrato HTTP entre elas.
- TypeScript estrito ajuda a manter o contrato do mock e, depois, da API real.

### Negativas / trade-offs

- Duas stacks, dois conjuntos de ferramentas e de testes.
- A API real precisará de **CORS** para o front (item novo no roadmap).
- O escopo do projeto agora diverge do enunciado: skill `picpay-challenge`, `AGENTS.md` e roadmap precisam refletir isso.
- Cadastro e auth viram trabalho de backend futuro (itens ⏳ no roadmap).

### O que fazer se a decisão mudar

Novo ADR. Trocar de roteador afeta só a camada de rotas; trocar de framework exige reescrever a UI, mas o contrato HTTP e o backend não mudam.
