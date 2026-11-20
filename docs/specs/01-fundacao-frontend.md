# 01 — Fundação do front-end (DuduPay)

- **Status:** done
- **Data:** 2026-11-08
- **Branch / PR:** `feat/01-fundacao-frontend` · [#4](https://github.com/deiz0n/desafio-picpay-dotnet-react/pull/4)
- **Item do roadmap:** F1 — Fundação (seção Front-end)
- **ADRs:** [0008 — SPA React + Vite + TypeScript](../adr/0008-spa-react-vite-typescript-com-react-router.md) · [0009 — Tailwind, shadcn/ui e identidade DuduPay](../adr/0009-ui-com-tailwind-shadcn-e-identidade-dudupay.md) · [0013 — Testes do front](../adr/0013-testes-do-front-com-vitest-testing-library-e-msw.md)

## Objetivo

Deixar `frontend/` pronto para receber as telas: um app React que builda e testa, com o tema DuduPay (claro e escuro, contraste medido por teste), troca de tema, formatação de moeda e uma vitrine dos componentes. Plano de origem: [frontend.md](../frontend.md).

## Decisões da entrevista

| Pergunta | Resposta |
| --- | --- |
| TypeScript | Fixo em **6.0.3**: a última versão é a 7.0.2, mas o `typescript-eslint` só aceita TS `< 6.1` |
| Paleta escura | `#A78BFA` (texto violeta), botão `#7C3AED` + branco, `#2DD4BF` (recebido), `#F87171` (erro), fundo `#0B0F14`, card `#121821`; neutro base zinc |
| Tipografia e forma | Inter Variable hospedada no projeto (`@fontsource-variable/inter`), raio `0.625rem`, `tabular-nums` nos valores |
| Tema | Classe `dark` no `<html>`, padrão = sistema, escolha em `localStorage`, botão já na F1; a F5 só o posiciona |
| Ferramentas | ESLint (flat config) + Prettier; versões exatas; `.nvmrc` 24; `engines` `^22.12 \|\| >=24` |
| Instalação | Só o que a F1 usa; componentes shadcn: `button`, `card`, `input`, `label`, `badge` |
| Suposições (confirmadas no checkpoint 1) | Tokens em hex (o teste de contraste precisa lê-los); `formatBRL` recebe centavos; Prettier com `endOfLine: "auto"` (Windows/CRLF) |

## Comportamento implementado

Não há API nem banco nesta entrega. O comportamento é do próprio front.

### Tema

1. **Escolha do usuário** = `light` ou `dark`, guardada em `localStorage` na chave `dudupay-theme`. Sem escolha, o app **segue o sistema** (`prefers-color-scheme`), inclusive se o sistema mudar com o app aberto.
2. O tema em vigor é **calculado** (`escolha ?? sistema`) e aplicado como classe `dark` e `color-scheme` no `<html>`.
3. O botão de tema alterna entre claro e escuro e grava a escolha. Seu nome acessível diz a ação (`Ativar tema escuro` / `Ativar tema claro`).
4. **`localStorage` não é confiável:** leitura e escrita ficam em `try/catch`; valor salvo inválido é ignorado e cai no sistema; sem storage o tema funciona só na sessão.
5. **Sem flash:** um script inline no `index.html` aplica a mesma regra **antes** do React montar. Ele repete a regra do `ThemeProvider` (mesma chave e prioridade); o teste `index-html.test.ts` executa o script e vigia que os dois não se separem.

### Tokens de design

Definidos em `frontend/src/index.css` (`:root` e `.dark`), em `#RRGGBB`. Além dos tokens padrão do shadcn há quatro nossos:

| Token | Uso |
| --- | --- |
| `--brand` | Violeta para **texto e ícones** (no escuro `#A78BFA`; o `--primary` do botão, `#7C3AED`, não passa como texto) |
| `--success` / `--success-foreground` | "Recebido": texto/ícone, ou fundo sólido com texto por cima |
| `--destructive-foreground` | Texto sobre o vermelho sólido |
| `--input` | Borda de campos, escurecida para atingir 3:1 |

`--border` é decorativo e fica fora da exigência de contraste. Os tokens `chart-*` e `sidebar-*` do preset foram removidos (sem uso).

| Par | Claro | Escuro |
| --- | --- | --- |
| Texto do botão primário sobre `--primary` | 7,10 | 5,70 |
| `--brand` sobre o fundo | 6,81 | 7,06 |
| `--success` sobre o fundo | 5,24 | 10,32 |
| `--success-foreground` sobre `--success` | 5,47 | 10,32 |
| `--destructive-foreground` sobre `--destructive` | 4,83 | 6,95 |
| `--muted-foreground` sobre o fundo | 7,41 | 7,50 |
| `--input` (borda) sobre o fundo / card | 3,23 / 3,37 | 3,98 / 3,69 |
| `--primary` (botão) sobre o fundo / card | 6,81 / 7,10 | 3,37 / 3,13 |

Mínimos exigidos pelo teste: **4,5** para texto e **3** para componentes de interface (22 pares por tema).

### Dinheiro (`frontend/src/lib/money.ts`)

- `formatBRL(centavos)` → `R$ 1.234,56`. Exige **inteiro** (`RangeError` caso contrário); aceita zero e negativo.
- `reaisToCents(reais)` → centavos inteiros, arredondando ao centavo mais próximo (`19.99` → `1999`; `v * 100` sozinho dá `1998.9999999999998`). Rejeita `NaN` e infinito e nunca devolve `-0`.

### Vitrine

Tela inicial **temporária**: card de saldo em BRL, movimentações com `+ R$ 200,00` e selo "Recebido" (teal) e `− R$ 35,90` com selo "Enviado" (neutro), botões das 6 variantes e um campo com rótulo. Sinal e selo em texto garantem que recebido/enviado não dependam só da cor. As telas reais a substituem a partir da F4.

### Comandos

Documentados em [AGENTS.md](../../AGENTS.md#comandos) e no [README](../../README.md): `npm run dev`, `test`, `test:watch`, `typecheck`, `lint`, `format`, `format:check`, `build` (dentro de `frontend/`).

### Banco de dados

Nenhum.

### Código

| Local (`frontend/`) | Arquivos principais |
| --- | --- |
| Raiz | `package.json`, `.npmrc` (`save-exact`), `.nvmrc`, `vite.config.ts`, `tsconfig*.json` (`strict`, alias `@/`), `eslint.config.js`, `.prettierrc.json`, `components.json`, `index.html` |
| `src/` | `main.tsx`, `App.tsx` (provider, cabeçalho e vitrine), `index.css` (tokens) |
| `src/components/theme/` | `use-theme.ts`, `theme-provider.tsx`, `theme-toggle.tsx` |
| `src/components/ui/` | `button`, `card`, `input`, `label`, `badge` (shadcn; `Badge` ganhou a variante `success`) |
| `src/features/showcase/` | `showcase.tsx` |
| `src/lib/` | `money.ts`, `utils.ts` (`cn`) |
| `src/test/` e `src/styles/` | `setup.ts`, `contrast.ts`, `match-media.ts`, `tokens.test.ts` |

### Versões (todas exatas)

React 19.3.1 · Vite 8.3.6 · TypeScript 6.0.3 · Vitest 5.1.0 · Tailwind 4.3.5 · shadcn 4.23.0 (estilo `radix-nova`, base zinc) · Radix UI 1.8.1 · lucide-react 1.70.0 · ESLint 10.14.0 · typescript-eslint 8.72.1 · Prettier 3.9.10 · jsdom 31.0.0 · Testing Library (react 16.3.3). Lista completa no `package.json`.

## Critérios de aceite → testes

Convenção: [frontend.md](../frontend.md#convenção-de-testes-do-front). Todos são de unidade ou de componente (Vitest + Testing Library); 42 testes em 10 arquivos.

| Critério | Teste | Tipo |
| --- | --- | --- |
| O app renderiza | `App` › `mostra o nome DuduPay` | componente |
| App oferece o botão de tema e mostra a vitrine | `App` › `oferece o botão de alternar tema` · `mostra o conteúdo da vitrine` | componente |
| `cn` junta classes e resolve conflitos do Tailwind | `cn` › `mescla classes e mantém a última em conflito` · `ignora valores falsos e junta o restante` | unidade |
| Botão acessível e respeita `disabled` | `Button` › `é encontrado pelo papel e pelo nome` · `dispara o clique e não dispara quando desabilitado` | componente |
| Contraste AA no tema claro | `tokens` › `pares de texto do tema claro passam em AA` | unidade |
| Contraste AA no tema escuro | `tokens` › `pares de texto do tema escuro passam em AA` | unidade |
| Os dois temas definem as mesmas variáveis, todas em hex | `tokens` › `claro e escuro definem as mesmas variáveis` · `todas as cores dos temas estão em hexadecimal #RRGGBB` | unidade |
| O cálculo de contraste está correto | `contraste` › 4 testes (21 para preto/branco, simetria, valor do violeta, formato inválido) | unidade |
| Sem escolha salva, segue o sistema (escuro e claro) | `ThemeProvider` › `usa a preferência do sistema quando não há escolha salva` · `usa o tema claro quando o sistema prefere claro…` | componente |
| Alternar aplica a classe e grava | `ThemeProvider` › `aplica dark no html e grava a escolha ao alternar` | componente |
| Escolha salva vence o sistema; valor inválido é ignorado | `ThemeProvider` › `respeita a escolha salva mesmo contra a preferência do sistema` · `ignora valor inválido salvo e usa o sistema` | componente |
| Acompanha mudança do sistema | `ThemeProvider` › `acompanha a mudança do sistema enquanto não há escolha salva` | componente |
| `localStorage` com erro não quebra | `ThemeProvider` › `continua funcionando quando o localStorage lança erro` | componente |
| `useTheme` fora do provider falha com mensagem clara | `ThemeProvider` › `useTheme lança erro claro fora do ThemeProvider` | componente |
| Botão de tema alterna e anuncia a ação | `ThemeToggle` › `alterna entre claro e escuro ao clicar` · `anuncia a ação que vai acontecer, não o estado atual` | componente |
| Script do `index.html` repete a regra do provider | `index.html (tema antes do React)` › 5 testes (dark salvo, light salvo contra o sistema, só sistema, valor inválido, storage com erro) | unidade |
| BRL formatado | `formatBRL` › `formata centavos como moeda brasileira` · `formata zero e negativo` · `rejeita centavos que não são inteiros` | unidade |
| Conversão para centavos | `reaisToCents` › `converte valores com duas casas decimais` · `não perde centavo em valores que o ponto flutuante representa mal` · `arredonda para o centavo mais próximo` · `nunca devolve -0` · `rejeita NaN e infinito` | unidade |
| Vitrine: saldo em BRL, sinais `+`/`−` com selo, rótulo do campo, botões | `Showcase` › 4 testes | componente |

Além dos testes: `npm run lint`, `format:check`, `typecheck` e `build` verdes a partir de um `npm ci` limpo.

## Fora de escopo / melhorias futuras

- **Convenção de nomes (🟡 do review):** [frontend.md](../frontend.md) diz "nomes de código em inglês", mas há internos em português (`lerEscolhaSalva`, `salvarEscolha`, `sistemaEscuro`, `contraste`, `lerBloco`…) em ~14 arquivos. Decidir entre renomear para inglês ou ajustar o doc.
- **CSP (🟡):** o script inline do `index.html` exige hash ou `nonce` se o nginx da F9 aplicar uma CSP estrita.
- **`Badge` destrutivo (🟡):** a variante `destructive` do `Badge` (sólida) não é renderizada em nenhum teste; o contraste do token está coberto, a composição de classes não. O primeiro uso real deve trazer o teste.
- **`cn` (🟡):** os componentes do shadcn importam `cn` direto do pacote `cn`, não de `@/lib/utils` (padrão do shadcn 4.23).
- **`npm audit`:** 6 avisos (2 baixos, 4 moderados) apenas em ferramentas de desenvolvimento (`deepmerge` via CLI do shadcn; `minimatch` via ESLint 10 e `ts-morph`). A auditoria de produção dá 0. O `npm audit fix --force` rebaixaria ESLint ou shadcn, então **não foi aplicado**; revisitar quando houver versões corrigidas.
- **TypeScript 7:** subir quando o `typescript-eslint` suportar.
- **Cobertura visual:** os testes não calculam CSS nem layout (ADR 0013), e o contraste é medido sobre **tokens**, não sobre cada composição de componentes. A aparência foi conferida só por quem rodou `npm run dev`.
- **A vitrine é temporária** e sai quando as telas reais chegarem; o cabeçalho com o botão de tema já está no `App` e vira o layout da F5.
- **Não instalados de propósito:** router, TanStack Query, React Hook Form, Zod e MSW entram nas fatias que os usam (F3 em diante).

## Histórico

- 2026-11-08: entregue via `/goal` ([#4](https://github.com/deiz0n/desafio-picpay-dotnet-react/pull/4)). **Desvios do plano aprovado:** (1) TypeScript fixado em 6.0.3 e o `oxlint` do gerador do Vite removido em favor do ESLint; (2) o preset `nova` do shadcn trouxe Geist e cores `oklch`, ambos substituídos (Inter e tokens em hex); (3) token `--brand` criado, porque um só `--primary` não serve como fundo de botão e como texto no tema escuro; (4) variantes `destructive` de `Button` e `Badge` trocadas de fundo translúcido (contraste 3,98) para vermelho sólido (4,83 claro, 6,95 escuro); (5) script anti-flash no `index.html`, com testes, que não estava no plano; (6) `shadcn` movido para `devDependencies` depois do code review. Tentado e descartado: `npm audit fix --force`.
