# 0012 — Mockar a API com json-server e um wrapper com as regras

- **Status:** accepted
- **Data:** 2026-11-08
- **Spec de origem:** —
- **Substitui:** —

## Contexto

O front será construído **antes** do backend. Precisa de uma API de mentira que se comporte como a real: login, cadastro com unicidade, transferência com as regras do desafio e erros HTTP corretos, para que cada estado de tela (saldo insuficiente, autorizador fora, e-mail duplicado) possa ser visto e testado.

O `json-server` atual (`1.0.0-beta.15`) é só CRUD sobre um `db.json`: sozinho **não** faz login, não debita saldo e não devolve 422/403/404. Inspecionando o pacote, ele exporta `createApp(db)`, que devolve um app HTTP (tinyhttp) ao qual podemos acrescentar rotas próprias.

Como o front vem primeiro, o mock vira o **contrato de fato** que o backend terá de seguir depois.

## Decisão

Vamos usar **json-server `1.0.0-beta.15`** (versão **exata** fixada) com um **wrapper Node** em `frontend/mock/`, via `npm run mock`:

- um app externo registra primeiro as rotas com regra (`POST /auth/login`, `POST /users`, `POST /transfer`, `/…/me`), e delega o resto (CRUD) ao `createApp` do json-server;
- o wrapper simula **todas** as regras do desafio: lojista não envia, saldo suficiente, `value > 0`, `payer ≠ payee`, usuário inexistente, unicidade de e-mail e documento;
- o header `X-Mock-Authorizer: deny|down` força as respostas 403 e 503, para exercitar esses estados na UI;
- erros no formato `application/problem+json`;
- usuário novo ganha **saldo inicial** (R$ 1.000,00) registrado como movimentação "Crédito de boas-vindas";
- `POST /transfer` gera a notificação do recebedor ([ADR 0014](0014-notificacoes-persistidas-com-polling.md));
- a **fonte de verdade do contrato é o `docs/frontend.md`**, não o `db.json`.

**Sessão (mock):** o login devolve um token falso; o front o guarda em `localStorage`. As senhas do `db.json` são texto puro e existem **só no mock**.

## Alternativas consideradas

| Alternativa | Prós | Contras |
| --- | --- | --- |
| json-server 1.x beta + wrapper (escolhida) | CRUD grátis; regras só onde importam; requisições visíveis na aba Network | Versão beta (pode quebrar); lógica do wrapper é descartável |
| json-server 0.17 (estável) | Suporta middleware e rotas customizadas oficialmente | Sem manutenção; sintaxe de consulta diferente da v1 |
| MSW no navegador (mock de desenvolvimento) | Sem processo extra | Não aparece como servidor real; mistura mock com o código do app |
| Express escrito à mão | Controle total | Reimplementa o que o json-server já entrega |
| Esperar o backend | Contrato real desde o início | Bloqueia o front por semanas; a escolha foi front-first |

## Consequências

### Positivas

- Todos os estados de erro e de sucesso ficam visíveis e testáveis no navegador.
- O contrato documentado serve de especificação para os goals de backend.

### Negativas / trade-offs

- O wrapper duplica regras que o backend implementará de verdade (trabalho que será descartado).
- Risco de **divergência**: o backend real pode se comportar diferente do mock. Mitigação: contrato em `docs/frontend.md` e, no backend, testes de integração que o afirmem.
- Dependência de uma versão beta; se quebrar, o fallback é a 0.17.
- Token em `localStorage` fica exposto a XSS; aceitável num projeto de estudo e **não** vale para a auth real.

### O que fazer se a decisão mudar

Novo ADR. Quando a API real existir, o front passa a apontar para ela (URL base configurável) e o `frontend/mock/` pode ser mantido só para desenvolvimento isolado ou removido.
