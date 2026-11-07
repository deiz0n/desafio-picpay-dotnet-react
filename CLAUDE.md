@AGENTS.md

## Claude Code

- Os skills em `.claude/skills/` são **atalhos**: o conteúdo real fica em `.agents/skills/<nome>/SKILL.md`. Sempre leia e siga o arquivo canônico (e seus `references/`).
- As regras ficam em `.agents/rules/*.md` — leia conforme indicado no `AGENTS.md`.
- Ao criar ou alterar um skill, edite **só** o arquivo em `.agents/skills/`; o stub em `.claude/skills/` muda apenas se `name`, `description` ou `argument-hint` mudarem.

