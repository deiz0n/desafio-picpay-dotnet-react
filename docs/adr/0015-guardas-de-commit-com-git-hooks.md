# 0015 — Guardas de commit com git hooks versionados

- **Status:** accepted
- **Data:** 2026-11-19
- **Spec de origem:** —
- **Substitui:** —

## Contexto

O harness é multiagente ([ADR 0006](0006-fluxo-goal-com-testes-embutidos.md)): Claude Code, Antigravity e Codex commitam no mesmo repo, além do próprio usuário. As verificações existiam só como instrução em texto (fase 5 do `/goal`), e nada impedia mecanicamente que um segredo fosse versionado. O `.gitignore` da raiz nem ignorava `.env`, que só estava protegido onde houvesse um `.gitignore` local. Como cada ferramenta de IA tem um sistema de hooks próprio, com formato e eventos diferentes, um hook escrito para uma delas não protege contra as outras.

## Decisão

Vamos versionar um **`pre-commit` em `.githooks/`**, ativado por clone com `git config core.hooksPath .githooks` (checado na fase 0 do `/goal`). Ele roda só checagens rápidas e condicionadas ao que está no stage:

1. bloqueia qualquer `.env`/`.env.*` (exceto `.env.example`);
2. `backend/` no stage e `backend/Pagamentos.sln` existente → `dotnet build`;
3. `frontend/` no stage → `npm run lint` e `npm run typecheck`.

Testes completos (`dotnet test`, `npm test`) continuam na fase 5 do `/goal`. O `.gitignore` da raiz passa a ignorar `.env`, `.env.*` e `!.env.example`.

## Alternativas consideradas

| Alternativa | Prós | Contras |
| --- | --- | --- |
| Git hook versionado + `core.hooksPath` (escolhida) | Vale para qualquer agente e para humanos; sem dependência extra | Precisa ser ativado em cada clone; contornável com `--no-verify` |
| Hooks de cada ferramenta de IA (ex.: `PreToolUse` do Claude Code) | Reação imediata, antes do commit | Um formato por ferramenta; prende o harness a uma delas, o que o ADR 0006 quer evitar |
| Husky / lefthook | Instalação automática e configuração declarativa | Dependência Node na raiz de um repo que é .NET + front; mais uma ferramenta para aprender |
| Rodar os testes completos no hook | Garantia máxima a cada commit | Testes de integração exigem Docker e levam segundos a minutos; incentiva `--no-verify` |
| Só CI | Impossível de contornar localmente | O segredo já teria sido enviado ao remoto quando o CI acusasse; ainda não há CI (roadmap) |

## Consequências

### Positivas

- Um `.env` não chega a um commit, independentemente de qual agente o crie.
- Erro de build/lint/tipos aparece no commit da fatia, não só no fim do `/goal`.

### Negativas / trade-offs

- Clone novo sem `core.hooksPath` fica sem proteção até a fase 0 do `/goal` acusar.
- `--no-verify` desativa tudo. A regra 6 do `AGENTS.md` proíbe, mas é convenção, não trava.
- O hook usa `bash`: no Windows depende do Git for Windows (que já acompanha o git).
- O build do backend no hook depende do caminho `backend/Pagamentos.sln`. Se o primeiro `/goal` do backend mudar o nome, ajustar o hook junto.

### O que fazer se a decisão mudar

Novo ADR. Para migrar para Husky/lefthook, os mesmos três passos viram configuração da ferramenta e `.githooks/` é removido. Quando houver CI (roadmap › Avançado), ele deve repetir essas checagens como rede de segurança.
