# Desafio Back-end — PicPay Simplificado

> Projeto de estudos baseado no desafio público: <https://github.com/PicPay/picpay-desafio-backend>

Este documento descreve o problema, as regras de negócio e os conceitos que o desafio permite praticar.

---

## Sumário

- [Sobre o ambiente da aplicação](#sobre-o-ambiente-da-aplicação)
- [Objetivo: PicPay Simplificado](#objetivo-picpay-simplificado)
- [Requisitos](#requisitos)
- [Endpoint de transferência](#endpoint-de-transferência)
- [Serviços externos (mocks)](#serviços-externos-mocks)
- [Competências praticadas](#competências-praticadas)
- [Fora do escopo](#fora-do-escopo)
- [Boas práticas](#boas-práticas)
- [Notas adicionais](#notas-adicionais)

---

## Sobre o ambiente da aplicação

- Use **qualquer linguagem ou framework** — de preferência um que você queira aprofundar.
- É possível **não usar framework nenhum**. Nesse caso, uma implementação via script reduz a sobrecarga de criar um servidor web.
- Se usar um framework, **evite métodos mágicos ou atalhos prontos**: o objetivo é exercitar a resolução do problema com o seu próprio código.
- Monte uma **boa estrutura de containers** (Docker, Podman etc.).

---

## Objetivo: PicPay Simplificado

O PicPay Simplificado é uma plataforma de pagamentos simplificada. Nela é possível **depositar** e realizar **transferências** de dinheiro entre usuários.

Existem **2 tipos de usuário**, ambos com carteira e saldo:

| Tipo | Envia dinheiro | Recebe dinheiro | Documento |
| --- | :---: | :---: | --- |
| Comum | ✅ | ✅ | CPF |
| Lojista | ❌ | ✅ | CNPJ |

---

## Requisitos

1. **Cadastro:** para ambos os tipos de usuário são necessários **Nome Completo, CPF/CNPJ, e-mail e Senha**. CPF/CNPJ e e-mail devem ser **únicos** no sistema — só pode existir um cadastro com o mesmo documento ou endereço de e-mail.
2. **Transferências permitidas:** usuários podem enviar dinheiro para lojistas e para outros usuários.
3. **Lojistas só recebem** transferências; não enviam dinheiro para ninguém.
4. **Validar saldo** do usuário antes da transferência.
5. **Autorização externa:** antes de finalizar a transferência, consultar um serviço autorizador externo via `GET` (ver [mocks](#serviços-externos-mocks)).
6. **Transação:** a operação de transferência deve ser uma transação, ou seja, **revertida em qualquer caso de inconsistência**, com o dinheiro voltando para a carteira de quem enviou.
7. **Notificação:** no recebimento do pagamento, o usuário ou lojista precisa receber uma notificação (e-mail, SMS) enviada por um serviço de terceiro via `POST`. Esse serviço **pode estar indisponível/instável** (ver [mocks](#serviços-externos-mocks)).
8. O serviço deve ser **RESTful**.

---

## Endpoint de transferência

O foco do desafio é o **fluxo de transferência entre dois usuários**. A implementação deve seguir o contrato abaixo:

```http
POST /transfer
Content-Type: application/json

{
  "value": 100.0,
  "payer": 4,
  "payee": 15
}
```

| Campo | Tipo | Descrição |
| --- | --- | --- |
| `value` | número decimal | Valor a ser transferido |
| `payer` | inteiro | ID do usuário que envia (pagador) |
| `payee` | inteiro | ID do usuário que recebe (recebedor) |

> Fique à vontade para propor e implementar endpoints adicionais (ex.: consulta de saldo, extrato, depósito).

---

## Serviços externos (mocks)

| Serviço | Método | URL |
| --- | --- | --- |
| Autorizador | `GET` | <https://util.devi.tools/api/v2/authorize> |
| Notificação | `POST` | <https://util.devi.tools/api/v1/notify> |

**Respostas típicas** *(observadas nos mocks; confirme antes de implementar, pois podem mudar)*:

- **Autorizador** — alterna aleatoriamente entre:
  - `200 OK` → `{"status": "success", "data": {"authorization": true}}`
  - `403 Forbidden` → `{"status": "fail", "data": {"authorization": false}}`
- **Notificação** — alterna aleatoriamente entre:
  - `204 No Content` → notificação enviada
  - `504 Gateway Timeout` → `{"status": "error", "message": "The service is not available, try again later"}`

---

## Competências praticadas

Use esta lista como roteiro de estudo — dá para evoluir o projeto em camadas, do básico ao avançado.

### Básico — criação de projetos back-end

- REST
- Git (commits pequenos e descritivos)
- Capacidade analítica
- Código limpo e organizado

### Intermediário — projetos manuteníveis

- Recomendações de implementação, como as PSRs
- SOLID
- Design Patterns
- Cache
- Containers (Docker, Podman etc.)
- Documentação do projeto e de suas funcionalidades
- Testes de unidade e integração
- Identificação e proposta de melhorias
- Bancos de dados relacionais

### Avançado — aplicações de alta qualidade

- Observabilidade
- CI para rodar testes e análises estáticas
- Bancos de dados não relacionais
- Arquiteturas (CQRS, Event Sourcing, Microsserviços, Monolito Modular)
- Mensageria
- Escalabilidade
- Aplicação do conhecimento do negócio no software
- Ferramentas de qualidade (análise estática, PHPMD, PHPStan, PHP-CS-Fixer etc.)
- PHP assíncrono

---

## Fora do escopo

- Fluxo de cadastro de usuários e lojistas
- Frontend (o foco é a [API RESTful](https://www.devmedia.com.br/rest-tutorial/28912))
- Autenticação

---

## Boas práticas

Caso use PHP, siga as **PSRs**. Caso use outra linguagem ou framework, siga as boas práticas da comunidade.

Para revisar a qualidade do código, rode o **PHPMD**:

```bash
docker run -it --rm -v $(pwd):/project -w /project jakzal/phpqa \
  phpmd app text cleancode,codesize,controversial,design,naming,unusedcode
```

---

## Notas adicionais

> Seção complementar — **não faz parte do enunciado original**. Reúne pontos implícitos e armadilhas comuns do desafio.

### Regras implícitas a validar

- `value` deve ser **maior que zero**.
- `payer` e `payee` devem **existir** e ser **diferentes** entre si.
- Respostas de erro devem ter **status HTTP adequado** e corpo consistente (ex.: [RFC 7807 — Problem Details](https://www.rfc-editor.org/rfc/rfc7807)).

### Fluxo sugerido da transferência

```text
1. Validar entrada (value > 0, payer != payee)
2. Iniciar transação no banco
3. Carregar e travar as carteiras (SELECT ... FOR UPDATE, em ordem de id)
4. Verificar se o pagador é lojista  → rejeitar
5. Verificar saldo do pagador        → rejeitar se insuficiente
6. Consultar autorizador (timeout curto) → rejeitar se negar/falhar
7. Debitar pagador, creditar recebedor, registrar a transferência
8. Commit
9. Publicar evento de notificação (assíncrono, com retry) — fora da transação
```

### Armadilhas comuns

| Armadilha | Como evitar |
| --- | --- |
| Usar `float` para dinheiro | Armazenar em centavos (inteiro) ou `DECIMAL` |
| Condição de corrida em transferências simultâneas | Lock pessimista (`FOR UPDATE`) ou otimista (versão), e `CHECK (balance >= 0)` |
| Deadlock ao travar duas carteiras | Travar sempre na mesma ordem (ex.: menor id primeiro) |
| Falha na notificação desfazendo a transferência | Notificar após o commit, de forma assíncrona (fila + retry/backoff) |
| Autorizador lento travando a requisição | Timeout curto; tratar timeout como não autorizado |
| Requisição duplicada gerando duas transferências | Header `Idempotency-Key` |
| Unicidade só na aplicação | Constraints `UNIQUE` no banco para documento e e-mail |

### Sugestão de códigos de resposta

| Situação | Status |
| --- | --- |
| Transferência realizada | `201 Created` |
| Payload inválido / saldo insuficiente / lojista como pagador | `422 Unprocessable Entity` |
| Usuário não encontrado | `404 Not Found` |
| Não autorizado pelo serviço externo | `403 Forbidden` |
| Autorizador indisponível | `503 Service Unavailable` |

### Checklist do projeto

- [ ] Aplicação sobe com um único comando (`docker compose up`)
- [ ] Seed com usuários comuns e lojistas para teste
- [ ] Testes de unidade e integração cobrindo os cenários de erro
- [ ] CI rodando testes e análise estática
- [ ] README com instruções de execução, decisões de arquitetura e melhorias futuras
