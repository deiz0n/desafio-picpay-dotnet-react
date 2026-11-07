---
name: adr
description: >
  Registra uma decisão arquitetural (Architecture Decision Record) em docs/adr/NNNN-slug.md.
  Use quando o usuário invocar /adr, pedir para registrar/documentar uma decisão de
  arquitetura, ou quando o /goal precisar criar ou substituir um ADR (fase 7).
argument-hint: "{decisão}"
---

# /adr {decisão}

## Entrada

`$ARGUMENTS` = a decisão ou a pergunta a decidir (ex.: "dinheiro em centavos", "lock pessimista ou otimista").

## Passos

1. Leia `docs/adr/README.md` (critérios e índice) e `docs/adr/0000-template.md`.
2. Verifique se já existe ADR sobre o tema:
   - existe `accepted` e a decisão muda → este ADR **substitui** o antigo;
   - existe e não muda → avise e pare.
3. Se a decisão ainda não está tomada, apresente 2–3 alternativas com prós/contras e uma recomendação; espere o usuário escolher. (No `/goal`, isso já aconteceu na entrevista — use a escolha registrada.)
4. Calcule o próximo número (4 dígitos) e um slug kebab-case no imperativo (ex.: `0008-armazenar-dinheiro-em-centavos`).
5. Grave `docs/adr/NNNN-slug.md` a partir do template:
   - **Status:** `accepted` se o usuário decidiu; `proposed` se ficou para depois;
   - **Spec de origem:** a SPEC final do goal (`../specs/NN-slug.md`), se houver; senão `—`.
6. Atualize o índice em `docs/adr/README.md` (e remova o tema da lista de "Decisões pendentes", se estiver lá).
7. Se substitui outro ADR: no antigo, altere **somente** o status para `superseded by [NNNN](NNNN-slug.md)` e o status no índice.
8. Se há SPEC de origem: adicione o link no campo **ADRs** da SPEC.
9. Se a decisão entrega/impacta item do `docs/roadmap.md`, preencha a coluna ADRs.

## Regras

- ADR `accepted` é imutável (exceto a linha de status ao ser substituído).
- Um ADR = uma decisão. Decisões diferentes → ADRs diferentes.
- Contexto e consequências honestos: registrar também os trade-offs negativos.
- Responder em Português Brasil.

