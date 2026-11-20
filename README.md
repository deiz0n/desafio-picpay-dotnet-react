# Desafio PicPay — Pagamento Simplificado (.NET & React)

Projeto de estudos para implementação do desafio do PicPay (plataforma de pagamentos simplificada com transferências entre usuários comuns e lojistas).

- **Enunciado e regras de negócio:** [`docs/challenge.md`](docs/challenge.md)
- **Instruções para agentes e fluxo de desenvolvimento:** [`AGENTS.md`](AGENTS.md)
- **Roadmap de entrega:** [`docs/roadmap.md`](docs/roadmap.md)
- **Decisões de arquitetura (ADRs):** [`docs/adr/`](docs/adr/)
- **Plano do front-end (DuduPay):** [`docs/frontend.md`](docs/frontend.md)

## Front-end (DuduPay)

SPA em React + Vite + TypeScript, em [`frontend/`](frontend/). Requer Node 22.12+ ou 24+.

```bash
cd frontend
npm install
npm run dev      # servidor de desenvolvimento
npm test         # testes (Vitest)
npm run build    # typecheck + build de produção
```

Lint, formatação e demais comandos: seção **Comandos** do [`AGENTS.md`](AGENTS.md). Por enquanto a tela inicial é uma vitrine dos componentes e dos temas claro e escuro; as telas reais chegam nas próximas fatias do [roadmap](docs/roadmap.md).
