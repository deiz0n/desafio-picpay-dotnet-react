# Front-end — DuduPay

Plano do front-end: o que será construído, o contrato da API que o mock simula, o design e a ordem das fatias. É um **plano** (o que vai ser feito); o registro do que foi construído continua sendo as [SPECs](specs/README.md), uma por `/goal`.

- Decisões (porquês): [ADR 0008](adr/0008-spa-react-vite-typescript-com-react-router.md) · [0009](adr/0009-ui-com-tailwind-shadcn-e-identidade-dudupay.md) · [0010](adr/0010-formularios-com-react-hook-form-e-zod.md) · [0011](adr/0011-estado-de-servidor-com-tanstack-query.md) · [0012](adr/0012-mock-da-api-com-json-server-e-wrapper.md) · [0013](adr/0013-testes-do-front-com-vitest-testing-library-e-msw.md) · [0014](adr/0014-notificacoes-persistidas-com-polling.md)
- Itens e status: seção **Front-end** do [roadmap](roadmap.md).
- **Este documento é a fonte de verdade do contrato** entre o front e a API (mock hoje, backend depois). O `db.json` do mock não é.

## Visão geral

**DuduPay** é o mini-banco do projeto: uma SPA em `frontend/` onde usuários comuns e lojistas entram, veem o saldo, transferem dinheiro, editam o perfil e acompanham o que receberam.

| Tela | Rota | Quem acessa |
| --- | --- | --- |
| Login | `/login` | público |
| Cadastro | `/cadastro` | público |
| Dashboard | `/` | logado |
| Transferência | `/transferir` | logado **comum** (lojista é redirecionado para `/` com aviso) |
| Perfil | `/perfil` | logado |
| Drawer de notificações | (aberto pelo sino no cabeçalho) | logado |

Rota protegida sem sessão → `/login` (guardando a rota de origem para voltar depois do login). Rota de visitante com sessão (`/login`, `/cadastro`) → `/`.

### Comportamento de cada tela

- **Login:** e-mail e senha. 401 mostra "e-mail ou senha incorretos" sem dizer qual dos dois errou.
- **Cadastro:** tipo (comum/lojista), nome completo, documento (CPF para comum, CNPJ para lojista, validados pelos dígitos verificadores), e-mail, senha (mínimo 8 caracteres) e confirmação. Sucesso → cria a conta com **saldo inicial de R$ 1.000,00** e leva ao login. 409 mostra o erro no campo duplicado (e-mail ou documento).
- **Dashboard:** saldo em destaque; total **enviado** e **recebido** no mês corrente; últimas 5 movimentações. Estados de carregando, vazio e erro.
- **Transferência:** busca o destinatário por e-mail ou documento exato (resolve para o id), valor, tela de confirmação, resultado. Trata 422 (saldo, valor, lojista, mesmo usuário), 404, 403 (negada) e 503 (autorizador fora) com mensagens distintas. Sucesso invalida saldo e movimentações.
- **Perfil:** edita nome, e-mail e senha (senha exige a senha atual). Documento e tipo são somente leitura.
- **Drawer de notificações:** lista as transferências recebidas (de quem, quanto, quando), com estado lida/não lida, contador no sino, "marcar todas como lidas" e polling de ~30 s ([ADR 0014](adr/0014-notificacoes-persistidas-com-polling.md)).

## Contrato da API

Convenções:

- URL base configurável por `VITE_API_URL` (mock: `http://localhost:3000`).
- JSON; campos em **inglês** (como o contrato do desafio: `value`, `payer`, `payee`); ids inteiros; datas ISO 8601 em UTC.
- Autenticação: `Authorization: Bearer <token>` em tudo, exceto `POST /auth/login` e `POST /users`. Sem token ou token inválido → `401`.
- Tipo de usuário: `"common"` (CPF) ou `"merchant"` (CNPJ). Documento sempre **só dígitos** na API.
- Valores em reais, número decimal com 2 casas (`100.0`), como no enunciado. O front trabalha internamente em centavos inteiros para somar e validar e só converte na borda. ⚠️ A representação final de dinheiro no backend ainda é uma [decisão pendente](adr/README.md#decisões-pendentes-candidatas-a-adr); se mudar, este contrato muda junto.
- Erros em **Problem Details** (`application/problem+json`):

```json
{
  "type": "https://dudupay.local/errors/insufficient_balance",
  "title": "Saldo insuficiente",
  "status": 422,
  "detail": "O saldo disponível é menor que o valor da transferência.",
  "code": "insufficient_balance",
  "errors": [{ "field": "value", "message": "Saldo insuficiente" }]
}
```

`code` é o que o front usa para escolher a mensagem; `errors` (opcional) liga o erro a campos de formulário.

### Endpoints

| Método e rota | Corpo / parâmetros | Sucesso | Erros |
| --- | --- | --- | --- |
| `POST /auth/login` | `{ email, password }` | `200` `{ token, user }` | `401 invalid_credentials` |
| `POST /users` | `{ name, document, email, password, type }` | `201` `{ id, name, email, document, type }` — cria a carteira com R$ 1.000,00 e a movimentação `welcome_credit` | `409 email_taken` · `409 document_taken` · `422 validation_error` (documento inválido ou incompatível com o tipo, senha curta, e-mail inválido) |
| `GET /users/me` | — | `200` usuário | `401` |
| `PATCH /users/me` | `{ name?, email?, password?, currentPassword? }` | `200` usuário | `409 email_taken` · `422 validation_error` · `422 invalid_current_password` |
| `GET /users?search=` | e-mail ou documento **exato**; ignora o próprio usuário | `200` array com 0 ou 1 item `{ id, name, type }` (sem e-mail nem documento) | `401` |
| `GET /wallets/me` | — | `200` `{ userId, balance }` | `401` |
| `GET /transactions/me` | `limit?` (padrão 20), mais recentes primeiro | `200` array de `{ id, kind, value, counterparty, createdAt }` | `401` |
| `GET /transactions/me/summary` | — (mês corrente, fuso do servidor) | `200` `{ month: "2026-11", sent, received }` | `401` |
| `POST /transfer` | `{ value, payer, payee }` | `201` `{ id, value, payer, payee, createdAt }` | ver tabela abaixo |
| `GET /notifications` | mais recentes primeiro | `200` array de `{ id, type, transferId, from, value, read, createdAt }` | `401` |
| `PATCH /notifications/{id}` | `{ read: true }` | `200` notificação | `401` · `404` |
| `POST /notifications/read-all` | — | `204` | `401` |

`kind` da movimentação: `sent` · `received` · `welcome_credit`. `counterparty` é `{ id, name }` ou `null` (no crédito de boas-vindas). `type` da notificação: `transfer_received`.

Paginação fica **fora** do plano: `limit` basta para o dashboard e o extrato curto.

### Erros do `POST /transfer`

Mantém as sugestões do enunciado ([códigos de resposta](challenge.md#sugestão-de-códigos-de-resposta)):

| Situação | Status | `code` |
| --- | --- | --- |
| `value ≤ 0` | `422` | `invalid_value` |
| `payer == payee` | `422` | `same_user` |
| Pagador é lojista | `422` | `merchant_cannot_send` |
| Saldo insuficiente | `422` | `insufficient_balance` |
| Pagador ou recebedor inexistente | `404` | `user_not_found` |
| `payer` diferente do usuário autenticado | `403` | `payer_mismatch` |
| Autorizador negou | `403` | `transfer_denied` |
| Autorizador fora do ar ou timeout | `503` | `authorizer_unavailable` |

Decisão registrada: o `payer` continua no corpo (contrato do desafio) e o servidor **valida** que é o usuário autenticado; o front preenche com o id de quem está logado.

### Comportamento do mock

Detalhes de simulação ([ADR 0012](adr/0012-mock-da-api-com-json-server-e-wrapper.md)):

- Aplica **todas** as regras acima de verdade: debita o pagador, credita o recebedor, registra as duas movimentações e cria a notificação do recebedor, tudo ou nada.
- `X-Mock-Authorizer: deny` força `403 transfer_denied`; `X-Mock-Authorizer: down` força `503 authorizer_unavailable`. Sem o header, autoriza.
- Token: `mock-token-<id>`, aceito apenas se o usuário existir.
- Estado vive em `frontend/mock/db.json` (reiniciar restaura o seed por `npm run mock:reset`).

### Seed do mock

Senha de todos: `senha1234` (apenas no mock). Documentos são válidos (dígitos verificadores conferem).

| id | Nome | Tipo | Documento | E-mail | Saldo |
| --- | --- | --- | --- | --- | --- |
| 1 | Ana Souza | `common` | `52998224725` | `ana@dudupay.test` | 1500.00 |
| 2 | Bruno Lima | `common` | `11144477735` | `bruno@dudupay.test` | 800.00 |
| 3 | Carla Dias | `common` | `12345678909` | `carla@dudupay.test` | 200.00 |
| 4 | Padaria do Zé | `merchant` | `11222333000181` | `padaria@dudupay.test` | 3500.00 |
| 5 | Livraria Central | `merchant` | `11444777000161` | `livraria@dudupay.test` | 0.00 |

O seed inclui algumas movimentações e notificações para o dashboard e o drawer não nascerem vazios.

## Design

Identidade, paleta e regras de cor: [ADR 0009](adr/0009-ui-com-tailwind-shadcn-e-identidade-dudupay.md).

| Token / uso | Tema claro | Tema escuro |
| --- | --- | --- |
| `--primary` (violeta) | `#6D28D9` (texto branco: 7,10 ✅) | variante mais clara, **definida e medida na F1** |
| Recebido / sucesso (teal) | `#0F766E` (texto branco: 5,47 ✅) | variante mais clara, **definida e medida na F1** |
| `--destructive` | `#DC2626` (texto branco: 4,83 ✅) | idem |
| Enviado | cor neutra (`--foreground`), sinal `−` | idem |

Regras: recebido com `+` e ícone; enviado neutro com `−` (não vermelho); vermelho só para erro e ação destrutiva; cor nunca é a única pista; valores com `Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' })` e algarismos tabulares; tom arejado (saldo grande, poucos elementos por tela).

Acessibilidade mínima de cada fatia: navegação por teclado, foco visível, rótulos em todos os campos, erros ligados ao campo (`aria-describedby`), drawer com foco preso e fechamento por `Esc`, valores monetários legíveis por leitor de tela.

## Estrutura proposta

```text
frontend/
  src/
    app/              # providers, router, rotas protegidas
    features/
      auth/           # login, cadastro, sessão
      dashboard/
      transfer/
      profile/
      notifications/  # drawer e hook useNotificacoes
      (cada feature: api.ts, hooks, components, schemas.ts, *.test.tsx colados ao código)
    shared/
      api/            # cliente HTTP, tipo de erro (Problem Details)
      ui/             # componentes do shadcn/ui
      lib/            # format (BRL), validação CPF/CNPJ
    test/             # setup do Vitest, handlers do MSW
  mock/               # server.js (wrapper), db.json, seed, regras e testes do wrapper
```

Nomes de código em inglês no front (`useBalance`, `TransferForm`) acompanham o contrato; textos exibidos ao usuário em português. Os hooks de dados são a fronteira com a API: trocar mock por backend não toca os componentes.

## Convenção de testes do front

Decisão: [ADR 0013](adr/0013-testes-do-front-com-vitest-testing-library-e-msw.md). Vale a regra do projeto: **teste vermelho primeiro**, e nenhuma fatia termina sem um teste que a prove.

- Arquivos `*.test.ts(x)` ao lado do código. Comandos: `npm test` (todos), `npm test -- <trecho do nome>` (filtrado).
- Nome do teste: frase em português descrevendo cenário e resultado, por exemplo `it('mostra saldo insuficiente quando o servidor responde 422 insufficient_balance')`.
- Consultas por papel/rótulo (`getByRole`, `getByLabelText`); sem snapshot.
- HTTP com MSW, com handlers que seguem este contrato. Cada caminho de erro do contrato usado pela tela tem teste; todo formulário tem um teste do caminho feliz e um de rejeição.
- Validações puras (CPF/CNPJ, formatação de BRL) com testes de unidade diretos.
- O wrapper do mock tem os próprios testes: transferência válida debita e credita; cada erro devolve status e `code` do contrato; falha deixa os saldos inalterados.
- Sem E2E.

## Fatias

Cada fatia é um `/goal` futuro, com branch `feat/NN-slug` e SPEC própria. Ordem: **F1 → F2 → F3 → F4 → F5**, depois **F6, F7 e F8** em qualquer ordem, e por fim **F9**.

| # | Fatia | Pronto quando | Prova (Vitest) |
| --- | --- | --- | --- |
| F1 | **Fundação:** Vite + React + TS estrito, Tailwind v4, shadcn/ui, tokens DuduPay claro/escuro (com contraste medido), ESLint, Vitest | `npm run build` e `npm test` verdes; app mostra página inicial com tema claro e escuro | Renderiza o app; utilitário de formatação BRL |
| F2 | **Mock da API:** json-server + wrapper, seed, regras, `X-Mock-Authorizer`, `mock:reset` | Todos os endpoints do contrato respondem como documentado | Transferência válida debita/credita e gera notificação; cada erro com status e `code` certos; falha mantém saldos |
| F3 | **Dados e sessão:** cliente HTTP, tipo de erro Problem Details, TanStack Query, sessão (token em `localStorage`), rotas protegidas | Rota protegida redireciona sem sessão; 401 derruba a sessão | Redirecionamento; conversão de erro; 401 limpa a sessão |
| F4 | **Login e cadastro:** React Hook Form + Zod, validação de CPF/CNPJ | Cadastra, loga e cai no dashboard | Validação de documentos; 401, 409 e 422 exibidos nos campos certos |
| F5 | **Layout e dashboard:** cabeçalho, navegação, alternância de tema, saldo, totais do mês, últimas movimentações | Dashboard mostra os dados do seed corretamente | Valores em BRL; recebido com `+`/teal, enviado com `−`/neutro; estados vazio/erro |
| F6 | **Transferência:** busca de destinatário, valor, confirmação, erros | Transfere e o saldo do dashboard atualiza | Cada erro (`422`, `404`, `403`, `503`) mostra a mensagem certa; lojista não acessa; sucesso invalida o saldo |
| F7 | **Perfil:** edição de nome, e-mail, senha | Alterações persistem no mock | PATCH com sucesso; 409 e `invalid_current_password` exibidos |
| F8 | **Drawer de notificações:** polling, lida/não lida, contador, marcar todas | Transferência recebida aparece no drawer em até ~30 s | Contador; marcar como lida e todas; atualização por polling (timers falsos) |
| F9 | **Container do front:** build e servidor estático (nginx) no `docker compose` | `docker compose up` sobe o front junto com o resto | Build da imagem; página responde |

F9 depende de o `docker compose` existir (item do roadmap do backend).

## Impacto no backend (itens derivados)

O front-first deixa uma lista de trabalho para o backend, registrada no [roadmap](roadmap.md). Cada item abaixo sai deste contrato:

- cadastro (`POST /users`) com unicidade de e-mail e documento e **saldo inicial** (R$ 1.000,00 como `welcome_credit`);
- autenticação (`POST /auth/login`, token) — decisão de JWT em ADR próprio;
- endpoints de leitura: `/users/me`, `PATCH /users/me`, `/users?search=`, `/wallets/me`, `/transactions/me`, `/transactions/me/summary`;
- notificações persistidas: `GET /notifications`, `PATCH /notifications/{id}`, `POST /notifications/read-all`;
- validação de `payer` igual ao usuário autenticado;
- CORS para o front;
- erros em Problem Details com os `code` desta página.

## Riscos

| Risco | Mitigação |
| --- | --- |
| O backend real diverge do mock | Este documento é a especificação; testes de integração do backend afirmam o contrato |
| Dinheiro em ponto flutuante no JS | Centavos inteiros internamente; conversão só na borda; decisão pendente do backend (`decimal` vs centavos) |
| `json-server` 1.x é beta | Versão exata fixada; fallback para a 0.17 ([ADR 0012](adr/0012-mock-da-api-com-json-server-e-wrapper.md)) |
| Tema escuro sem contraste | Paleta escura definida e medida na F1, antes de qualquer tela |
| Handlers do MSW divergem do wrapper do mock | Ambos seguem este contrato; ao mudar o contrato, atualizar os dois |
| Token em `localStorage` (XSS) | Aceito só para o mock; a autenticação real decide cookie `HttpOnly` ou outro meio |
| Notificação com atraso de até ~30 s | Aceito ([ADR 0014](adr/0014-notificacoes-persistidas-com-polling.md)); tempo real fica como melhoria futura |

## Fora do escopo

- Autenticação real, refresh de token, recuperação de senha, verificação de e-mail.
- Depósito, saque, extrato paginado ou exportado.
- Notificações em tempo real (WebSocket/SSE).
- Testes E2E.
- Internacionalização (só português do Brasil) e aplicativo móvel nativo (a SPA deve ser responsiva).
