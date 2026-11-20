# Desafio PicPay — Pagamento Simplificado (.NET & React)

Projeto de estudos para implementação do desafio do PicPay (plataforma de pagamentos simplificada com transferências entre usuários comuns e lojistas).

- **Enunciado e regras de negócio:** [`docs/challenge.md`](docs/challenge.md)
- **Instruções para agentes e fluxo de desenvolvimento:** [`AGENTS.md`](AGENTS.md)
- **Roadmap de entrega:** [`docs/roadmap.md`](docs/roadmap.md)
- **Decisões de arquitetura (ADRs):** [`docs/adr/`](docs/adr/)
- **Plano do front-end (DuduPay):** [`docs/frontend.md`](docs/frontend.md)

## Backend

API em C# / .NET 10 (ASP.NET Core), em [`backend/`](backend/), com PostgreSQL 17. Requer o SDK do .NET 10 e Docker.

Nenhuma credencial fica no repositório. Configure uma vez por máquina:

1. Copie `.env.example` para `.env` e escolha usuário e senha do Postgres local (o `.env` é ignorado pelo git e bloqueado pelo hook de pre-commit).
2. Guarde a connection string da API em [user-secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets), com os mesmos valores do `.env`:

```bash
dotnet user-secrets set "ConnectionStrings:Pagamentos" "Host=localhost;Port=5432;Database=pagamentos;Username=<usuario>;Password=<senha>" --project backend/src/Pagamentos.Api
```

Alternativa: a variável de ambiente `ConnectionStrings__Pagamentos`. Sem nenhuma das duas, a API não sobe e diz o que falta.

```bash
docker compose up -d postgres                                  # banco local (aplica database/migrations num volume novo)
dotnet run --project backend/src/Pagamentos.Api                # API em http://localhost:5186
dotnet test backend/Pagamentos.slnx                            # unidade + integração (Testcontainers, Docker ligado)
```

Os testes não precisam de `.env` nem de user-secrets: o Testcontainers cria um Postgres descartável com credenciais próprias.

Endpoints disponíveis: `POST /users` (cadastro de usuário comum ou lojista com carteira de R$ 1.000,00) — detalhes na [SPEC 02](docs/specs/02-fundacao-backend-cadastro.md).

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
