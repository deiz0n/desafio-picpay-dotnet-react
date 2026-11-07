---
name: code-review
description: >
  Code review crítico do diff do repositório (base...HEAD + mudanças não commitadas), no estilo
  do /code-review do Claude: lê o contexto de cada trecho, aplica as regras do desafio, ADRs,
  Clean Architecture, idiomas C# e convenção de testes, e classifica em 🔴/🟡/🟢 com veredito.
  Use quando o usuário invocar /code-review, pedir revisão do código/diff/branch, ou na fase 6 do /goal.
argument-hint: "[base]"
---

# /code-review [base]

## Entrada

`$ARGUMENTS` = branch/commit base (padrão `develop`; se não existir, `origin/develop`, depois `main`).

## Passos

1. **Coletar o diff**
   ```bash
   git fetch origin
   git diff {base}...HEAD --stat
   git diff {base}...HEAD
   git diff HEAD            # mudanças não commitadas (staged + unstaged)
   git log {base}..HEAD --oneline
   ```
   Diff vazio → avise e pare.
2. **Ler o contexto:** para cada arquivo alterado, leia o arquivo inteiro (ou o método/classe ao redor), as interfaces que ele implementa e os testes correspondentes. Não revise só as linhas do diff.
3. **Ler as regras:** `docs/adr/README.md` (ADRs `accepted`), `docs/challenge.md` (regras de negócio), `.agents/rules/testes.md`. Se houver goal em andamento, os critérios de aceite em `.goal/`.
4. **Aplicar o checklist** (abaixo). Para cada achado, confirme que é real (releia o código; não presuma) antes de reportar.
5. **Reportar** no formato de saída. Não altere código — quem corrige é o usuário ou o `/goal`.

## Checklist

### Negócio e decisões
- Regras do desafio: lojista não envia, saldo suficiente, `value > 0`, `payer != payee`, ambos existem, autorizador antes de concluir, rollback total, notificação fora da transação.
- Respeita todos os ADRs `accepted`? Decisão arquitetural nova sem ADR?

### Arquitetura (ADR 0003)
- Regra de dependência entre projetos (nada aponta para fora; Domain sem dependências).
- Handler magro (carrega → domínio → ports → `Result`); > 4–5 dependências = fatiar.
- Domain rico e sem I/O; entidades não recebem repositório/client.
- Ports nomeados pelo papel (`IAutorizadorClient`, não `IDeviToolsClient`); `IHttpClientFactory`.
- Result pattern para fluxo esperado; exceção só para o inesperado.
- Nomenclatura em português; sufixos técnicos mantidos.

### C# / .NET
- `async`/`await` até a ponta; sem `.Result`/`.Wait()`/`async void`.
- `CancellationToken` propagado.
- Nullable reference types respeitados (sem `!` para silenciar).
- `record` para Commands/DTOs; `decimal` para dinheiro (nunca `double`/`float`).
- `using`/`IDisposable`/`IAsyncDisposable` corretos; lifetimes de DI coerentes (Unit of Work = Scoped).
- SOLID (SRP e DIP principalmente); métodos curtos; nomes claros.

### Banco e transação (ADRs 0004/0005)
- SQL parametrizado (nunca concatenação).
- Operações da transferência na mesma conexão/transação do Unit of Work; confirma/desfaz em todos os caminhos.
- Locks em ordem determinística; constraints como rede de segurança (`UNIQUE`, `CHECK (saldo >= 0)`).
- Migrations SQL: convenções (snake_case, plural, `data_criacao`), sem alterar migration já aplicada.

### Testes (`.agents/rules/testes.md`)
- Todo comportamento novo/alterado tem teste? Caminho de erro coberto?
- Teste de integração valida o **estado do banco**, não só o status HTTP?
- Serviços externos falsos; nada chama `util.devi.tools`.
- Testes isolados (sem dependência de ordem/dados compartilhados); nomes `Metodo_Cenario_Resultado`.
- Algum teste foi apagado, comentado, `Skip` ou teve assert afrouxado?

### Segurança e operação
- Segredos, connection strings ou URLs fixas no código (devem vir de configuração).
- Logs com dados sensíveis (senha, documento completo)?
- Correlation id e logs com ids de negócio.

## Formato de saída

```markdown
## Code review — {base}...HEAD ({N} arquivos, +X/−Y)

### 🔴 Bloqueante
- **`caminho/Arquivo.cs:42`** — {problema}. **Por que importa:** {impacto}. **Direção:** {como corrigir, sem reescrever o código}.

### 🟡 Melhoria
- **`caminho/Arquivo.cs:10`** — ...

### 🟢 Bom
- **`caminho/Arquivo.cs:5`** — {o que está certo e por quê}

### Testes
- Cobertura dos critérios: ... | Lacunas: ...

**Veredito:** aprovado | aprovado com ressalvas | bloqueado
```

- 🔴 **Bloqueante** — bug, regra de negócio violada, quebra de ADR, risco de dinheiro inconsistente, comportamento sem teste.
- 🟡 **Melhoria** — SOLID, Clean Code, nomenclatura, idioma C#, testabilidade.
- 🟢 **Bom** — só com algo concreto (reforça o aprendizado). Não elogiar por padrão.
- Seções vazias: escreva "nenhum".
- Veredito: qualquer 🔴 → `bloqueado`; só 🟡 → `aprovado com ressalvas`; nada → `aprovado`.

## Regras

- Responder em Português Brasil. Ser crítico e específico, sempre com `arquivo:linha`.
- Não alterar código neste skill.

