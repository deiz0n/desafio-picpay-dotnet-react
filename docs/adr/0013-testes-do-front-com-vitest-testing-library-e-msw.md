# 0013 — Testes do front com Vitest, Testing Library e MSW (sem E2E)

- **Status:** accepted
- **Data:** 2026-11-08
- **Spec de origem:** —
- **Substitui:** —

## Contexto

O [ADR 0007](0007-xunit-com-testcontainers.md) define os testes do **backend** (xUnit + Testcontainers) e descartou E2E porque não havia front. Agora há front, e ele também precisa de testes por fatia (regra 2 do `AGENTS.md`). Decidimos **não** ter testes E2E por ora: o foco do aprendizado é teste de unidade e de componentes.

## Decisão

Vamos usar:

- **Vitest** como runner (nativo do Vite), com ambiente de DOM (`jsdom`);
- **React Testing Library** + `@testing-library/user-event` + `@testing-library/jest-dom` para testar componentes pelo que o usuário vê (consultas por papel/rótulo), não por detalhes de implementação;
- **MSW** (Mock Service Worker) para interceptar o `fetch` nos testes, de modo que o cliente HTTP real seja exercitado e as respostas (200, 401, 409, 422, 403, 503) sejam controláveis por teste;
- **sem E2E** (Playwright ou similar).

Aplica-se ao front o mesmo princípio do backend: **teste vermelho primeiro**, cada critério de aceite com pelo menos um teste e caminhos de erro cobertos. A convenção de nomes e a estrutura de pastas dos testes do front ficam em `docs/frontend.md`.

## Alternativas consideradas

| Alternativa | Prós | Contras |
| --- | --- | --- |
| Vitest + Testing Library + MSW (escolhida) | Rápido; mesma config do Vite; o cliente HTTP é testado de verdade | MSW é uma dependência a mais; handlers de teste são separados do mock de desenvolvimento |
| Jest + Testing Library | Muito difundido | Configuração extra com Vite/ESM; Vitest é mais direto |
| `vi.mock` do módulo da API | Sem dependência extra | O teste não exercita o cliente HTTP nem o tratamento de erros reais |
| Playwright (E2E) | Exercita a stack completa no navegador | Mais lento e custoso; fora do foco de aprendizado agora |

## Consequências

### Positivas

- Testes rápidos e determinísticos, sem servidor rodando.
- Estados de erro de rede testáveis sem depender do mock de desenvolvimento.
- Consultas por papel/rótulo empurram para marcação acessível.

### Negativas / trade-offs

- `jsdom` não renderiza CSS: layout e contraste **não** são verificados pelos testes (contraste é checado ao definir os tokens).
- Sem E2E, a integração real front ↔ API só é vista manualmente até existir o backend.
- Os handlers do MSW podem divergir do wrapper do mock; o contrato em `docs/frontend.md` é a referência dos dois.

### O que fazer se a decisão mudar

Novo ADR, complementando este e o 0007. Adicionar E2E depois não invalida os testes existentes.
