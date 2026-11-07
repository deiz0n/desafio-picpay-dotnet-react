---
name: code-review
description: >
  Code review crítico do diff do repositório (base...HEAD + mudanças não commitadas), no estilo
  do /code-review do Claude: lê o contexto de cada trecho, aplica as regras do desafio, ADRs,
  Clean Architecture, idiomas C# e convenção de testes, e classifica em 🔴/🟡/🟢 com veredito.
  Use quando o usuário invocar /code-review, pedir revisão do código/diff/branch, ou na fase 6 do /goal.
argument-hint: "[base]"
---

Atalho do Claude Code. Leia e siga integralmente `.agents/skills/code-review/SKILL.md`, repassando os argumentos: $ARGUMENTS

