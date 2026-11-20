# 0016 — Não ter o tipo de usuário Administrador

- **Status:** accepted
- **Data:** 2026-11-20
- **Spec de origem:** [02-fundacao-backend-cadastro](../specs/02-fundacao-backend-cadastro.md)
- **Substitui:** —

## Contexto

O enunciado ([challenge.md](../challenge.md)) prevê dois tipos de usuário: **comum** (CPF, envia e recebe) e **lojista** (CNPJ, só recebe). O contrato do front ([frontend.md](../frontend.md)) também só conhece `"common"` e `"merchant"`.

Um protótipo anterior do backend tinha um enum com `Administrador = 1`, mas nenhum código o usava: não havia endpoint, permissão nem regra de negócio para ele. A dúvida ficou registrada como decisão pendente no índice de ADRs.

## Decisão

Vamos ter **só dois tipos de usuário**, `Comum` e `Lojista`. O tipo Administrador não existe no Domain, na API nem no banco: o `CHECK (tipo IN ('comum', 'lojista'))` recusa qualquer outro valor e a API responde `422 validation_error` no campo `type` a valores como `"admin"`.

## Alternativas consideradas

| Alternativa | Prós | Contras |
| --- | --- | --- |
| Só comum e lojista (escolhida) | Fiel ao enunciado e ao contrato; nada para proteger nem testar sem necessidade (YAGNI) | Se um painel administrativo surgir, exige migration e ADR novos |
| Administrador reservado no enum, sem uso | "Pronto para o futuro" | Valor morto que confunde; um administrador sem permissões definidas é um risco de segurança esperando acontecer |
| Administrador com permissões (ex.: ver todos os usuários, estornar) | Exercita autorização por papel | Fora do desafio; exige autenticação, que ainda não existe |

## Consequências

### Positivas

- Domínio menor e explícito: todo `switch` sobre `TipoUsuario` tem dois casos.
- O banco garante a regra mesmo que o código falhe (`usuarios_tipo_check`).

### Negativas / trade-offs

- Funcionalidades administrativas (suporte, estorno, auditoria) não têm onde morar hoje.

### O que fazer se a decisão mudar

Novo ADR definindo as permissões do papel; migration que afrouxa `usuarios_tipo_check` (e o `CHECK` de documento, se o administrador não tiver CPF/CNPJ); novo valor no enum e nos mapeamentos `TipoUsuarioNoBanco` e `TipoUsuarioNaApi`. Avaliar se "administrador" é um **tipo de usuário** ou um **papel** separado (uma pessoa comum com permissão extra) — a segunda opção tende a ser melhor.
