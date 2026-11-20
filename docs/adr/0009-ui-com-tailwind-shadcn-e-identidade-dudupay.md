# 0009 — UI com Tailwind v4, shadcn/ui e identidade visual DuduPay

- **Status:** accepted
- **Data:** 2026-11-08
- **Spec de origem:** —
- **Substitui:** —

## Contexto

O front precisa de componentes acessíveis (diálogos, drawer, formulários, toasts) sem escrever tudo do zero, e de uma identidade visual própria. O enunciado se chama "PicPay Simplificado", mas copiar o verde do PicPay faria o projeto parecer um app oficial; o produto do projeto é o **DuduPay**.

Um app financeiro tem requisitos visuais específicos: o significado das cores importa (dinheiro entrando, dinheiro saindo, erro) e não pode depender só de cor.

## Decisão

Vamos usar **Tailwind CSS v4** com **shadcn/ui** (componentes sobre Radix UI, copiados para o repositório) e **tokens CSS** (`--primary`, `--background`, `--destructive`…) com valores para tema **claro e escuro**.

Identidade **DuduPay**: direção "violeta + teal".

| Papel | Cor | Contraste medido (WCAG) |
| --- | --- | --- |
| Primária (violeta) | `#6D28D9` | texto branco sobre ela: **7,10** ✅ AA · sobre `#FAFAFA`: 6,81 ✅ |
| Recebido / sucesso (teal) | `#0F766E` | texto branco sobre ela: **5,47** ✅ AA · sobre `#FAFAFA`: 5,24 ✅ |
| Erro (`destructive`) | `#DC2626` | texto branco sobre ela: **4,83** ✅ AA |

No tema escuro, essas mesmas cores **não passam** sobre `#0B0F14` (violeta 2,71 e teal 3,51); o tema escuro usa variantes mais claras, escolhidas e medidas na fatia F1.

Regras semânticas:

- **Recebido:** teal, com sinal `+` e ícone.
- **Enviado:** cor neutra, com sinal `−` — enviar dinheiro não é erro, então **não** usa vermelho.
- **Vermelho (`destructive`)** é reservado a erros e ações destrutivas.
- Cor nunca é a única pista: sempre há texto, sinal ou ícone junto.
- Valores monetários com `Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' })` e algarismos tabulares.
- Tom visual **arejado**, no estilo "app de carteira": saldo em destaque e poucos elementos por tela.

## Alternativas consideradas

| Alternativa | Prós | Contras |
| --- | --- | --- |
| Tailwind v4 + shadcn/ui (escolhida) | Código dos componentes fica no repositório (sem caixa-preta); acessibilidade herdada do Radix; tokens fáceis de trocar | Manutenção dos componentes copiados passa a ser nossa |
| MUI | Biblioteca completa e madura | Visual próprio forte, difícil de customizar; mais peso |
| Chakra UI / Mantine | Boa DX e acessibilidade | Menos alinhado ao que o projeto quer praticar (utility CSS + componentes próprios) |
| CSS próprio (modules) | Controle total | Reimplementar acessibilidade de diálogo/drawer é custoso e arriscado |
| Reaproveitar o verde do PicPay | Familiar | Parece app oficial; o verde também confunde primária com "sucesso" |

## Consequências

### Positivas

- Identidade própria, com contraste AA verificado nas combinações principais do tema claro.
- A mesma base de tokens serve aos temas claro e escuro.
- Estados de dinheiro legíveis para quem não distingue cores.

### Negativas / trade-offs

- O tema escuro ainda precisa de paleta própria medida (F1).
- Componentes do shadcn ficam em `frontend/` e envelhecem se não forem atualizados.
- Tailwind v4 e shadcn mudam rápido; versões devem ser fixadas no `package.json`.

### O que fazer se a decisão mudar

Novo ADR. A paleta é só tokens CSS (troca localizada); trocar de biblioteca de componentes exige reescrever a camada de UI.
