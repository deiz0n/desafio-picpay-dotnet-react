---
name: picpay-challenge
description: Contexto do desafio back-end "pagamento simplificado" (transferências entre usuários comuns e lojistas). Use sempre que for planejar, implementar, testar, revisar ou documentar qualquer parte deste projeto — regras de negócio, endpoint POST /transfer, autorizador externo, notificações, containers, testes, CI ou README.
---

# Desafio Back-end — Pagamento Simplificado

Fonte original: [docs/challenge.md](../../../docs/challenge.md). Decisões em vigor: [docs/adr/](../../../docs/adr/README.md). Consulte-o em caso de dúvida; este skill resume o que importa para tomar decisões de código.

**Stack (ADRs 0002–0007):** C# / .NET 10, ASP.NET Core Controllers, Clean Architecture no padrão do CVL, PostgreSQL + Dapper, Unit of Work, testes com xUnit + Testcontainers. Dicas de PHP abaixo valem só como referência; use os equivalentes .NET. **Modo didático:** no `/goal` o agente implementa com checkpoints explicativos; fora dele, código só com pedido explícito (`.agents/rules/didatico.md`).

## Propósito

Projeto **de estudos**: o objetivo é aprender e praticar os conceitos, não entregar a um processo seletivo. Prefira explicar o porquê das decisões e evoluir o projeto em camadas (básico → intermediário → avançado).

- Commits pequenos e com mensagens claras.
- Framework livre, mas **evitar mágica/atalhos prontos**: o valor está em implementar a solução de fato. Registrar a escolha em um ADR.
- Tudo deve subir com um comando (ex.: `docker compose up`).

## Domínio

- Dois tipos de usuário: **comum** e **lojista**. Ambos têm carteira com saldo.
- Campos obrigatórios: nome completo, documento (CPF para comum, CNPJ para lojista), e-mail, senha.
- **CPF/CNPJ e e-mail são únicos** — garantir com constraint `UNIQUE` no banco, não só validação na aplicação.
- Fora do escopo: fluxo de cadastro, frontend, autenticação. Pode-se usar seed/migration para criar usuários de teste.

## Regras da transferência (núcleo do desafio)

1. Comum → comum e comum → lojista são permitidos.
2. **Lojista nunca envia** dinheiro.
3. Validar **saldo suficiente** do pagador.
4. Antes de concluir, consultar o **autorizador externo**: `GET https://util.devi.tools/api/v2/authorize`. Tratar resposta negativa, erro HTTP e timeout como "não autorizado".
5. A operação é **transacional**: qualquer inconsistência reverte tudo e o saldo volta ao pagador.
6. Após sucesso, o recebedor é **notificado** via `POST https://util.devi.tools/api/v1/notify`. Esse serviço é instável: a falha de notificação **não pode** desfazer a transferência — notificar de forma assíncrona (fila/mensageria + retry com backoff), fora da transação.

Validações adicionais esperadas: `value > 0`, `payer != payee`, ambos existem.

## Contrato

```http
POST /transfer
Content-Type: application/json

{ "value": 100.0, "payer": 4, "payee": 15 }
```

Mantenha esse contrato. Melhorias podem ser propostas (ex.: `Idempotency-Key`, `201 Created` com o recurso da transferência, `GET /transfers/{id}`), mas sem quebrar o formato acima.

Sugestão de respostas: `201` sucesso · `422` validação/saldo insuficiente/lojista pagador · `403` não autorizado pelo autorizador · `404` usuário inexistente · `503` autorizador indisponível. Corpo de erro consistente (ex.: RFC 7807 `application/problem+json`).

## Decisões técnicas recomendadas

- **Dinheiro**: nunca usar float. Armazenar em centavos (`BIGINT`) ou `DECIMAL(15,2)`; converter o `value` da entrada na borda.
- **Concorrência**: travar as carteiras com `SELECT ... FOR UPDATE` (em ordem determinística de id para evitar deadlock) ou locking otimista com versão. Adicionar `CHECK (balance >= 0)` como rede de segurança.
- **Ordem do fluxo**: validar → iniciar transação → lock → checar saldo → chamar autorizador (com timeout curto) → debitar/creditar → registrar transferência → commit → publicar evento de notificação. Registrar em ADR o trade-off de chamar o autorizador dentro vs. fora da transação.
- **Histórico**: persistir a transferência (tabela `transfers` com status), não só alterar saldos — ajuda auditoria e idempotência.
- **Arquitetura**: camadas claras (controller → caso de uso/serviço → domínio → repositórios/gateways). Clientes HTTP externos atrás de interfaces (DIP) para permitir mocks nos testes. Regras de tipo de usuário via polimorfismo/strategy em vez de `if` espalhado.

## Competências a praticar (checklist)

- REST, Git, código limpo, SOLID, design patterns.
- Docker/compose bem estruturado (app, banco relacional, fila/cache).
- **Testes de unidade e integração** — cobrir: saldo insuficiente, lojista pagador, autorizador negando/fora do ar, rollback, falha de notificação não afetando a transferência, concorrência. Convenção (integração validando o banco): `.agents/rules/testes.md`.
- README: como rodar, decisões de arquitetura, justificativa do framework, melhorias futuras.
- Diferenciais: CI (GitHub Actions) com testes + análise estática; observabilidade (logs estruturados, correlation id, métricas, health check); mensageria; cache; idempotência; noções de escalabilidade.
- Se PHP: seguir PSRs (PSR-4/PSR-12), usar PHPStan, PHP-CS-Fixer e PHPMD:
  `docker run -it --rm -v $(pwd):/project -w /project jakzal/phpqa phpmd app text cleancode,codesize,controversial,design,naming,unusedcode`
  Em outra linguagem, aplicar os linters/formatadores equivalentes da comunidade.

## Ao trabalhar neste projeto

- Priorize o fluxo de transferência; o resto é suporte.
- Decisão arquitetural não óbvia → ADR via `/adr` (ver `docs/adr/README.md`). Features e correções seguem o fluxo `/goal {objetivo}` (entrevista → fatias com teste primeiro → `/code-review` → ADR + SPEC final → PR para `develop`).
