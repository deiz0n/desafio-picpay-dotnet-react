# 02 — Fundação do backend e cadastro de usuário

- **Status:** done
- **Data:** 2026-11-20
- **Branch / PR:** `feat/02-fundacao-backend-cadastro` · [#5](https://github.com/deiz0n/desafio-picpay-dotnet-react/pull/5)
- **Item do roadmap:** Cadastro de usuários com saldo inicial (parcial: sem a movimentação `welcome_credit`); Modelo de dados (usuários e carteiras); Testes de unidade; Testes de integração
- **ADRs:** [0016 — Sem Administrador](../adr/0016-sem-tipo-de-usuario-administrador.md) · [0017 — Dinheiro em `decimal`/`NUMERIC`](../adr/0017-dinheiro-em-decimal-e-numeric.md) · [0018 — Hash com `PasswordHasher<T>`](../adr/0018-hash-de-senha-com-passwordhasher.md) · [0019 — `Result` e `IUnidadeDeTrabalho` no Domain](../adr/0019-result-e-unidade-de-trabalho-no-domain.md)

## Objetivo

Criar a solução .NET do backend (Clean Architecture do [ADR 0003](../adr/0003-clean-architecture-em-camadas-padrao-cvl.md), com testes de unidade e de integração contra Postgres real) e o primeiro endpoint: `POST /users`, que cadastra um usuário **comum** (CPF) ou **lojista** (CNPJ), com e-mail e documento únicos, e abre a carteira dele com **R$ 1.000,00** na mesma transação.

## Decisões da entrevista

| Pergunta | Resposta |
| --- | --- |
| Tipo Administrador (protótipo anterior) | Não existe; só comum e lojista ([ADR 0016](../adr/0016-sem-tipo-de-usuario-administrador.md)) |
| Carteira no cadastro | Sim: usuário + carteira com R$ 1.000,00 numa transação (Unit of Work). A movimentação `welcome_credit` fica para o goal que desenhar as movimentações |
| Dinheiro | `decimal` + `NUMERIC(15,2)` ([ADR 0017](../adr/0017-dinheiro-em-decimal-e-numeric.md)) |
| Hash de senha | `PasswordHasher<T>` atrás de `IHasherSenha` ([ADR 0018](../adr/0018-hash-de-senha-com-passwordhasher.md)) |
| Onde ficam `Result` e `IUnidadeDeTrabalho` | No Domain ([ADR 0019](../adr/0019-result-e-unidade-de-trabalho-no-domain.md)) — conflito entre os ADRs 0003 e 0005 descoberto ao montar a solução |
| Solução e migrations | `backend/Pagamentos.slnx` (padrão do .NET 10); `compose.yaml` só com Postgres, migrations via `docker-entrypoint-initdb.d`; testes aplicam os mesmos arquivos |
| Padrões assumidos | id `BIGINT` identity (o contrato usa inteiros); coluna única `documento`; e-mail sem espaços e em minúsculas; nome ≤ 100; senha 8–100; erro de binding → 422 |
| Fora do escopo | Autenticação, CORS, correlation id, health check, API dentro do compose, seed, demais endpoints |

## Comportamento implementado

### Contrato

```http
POST /users
Content-Type: application/json

{
  "name": "Maria da Silva",
  "document": "52998224725",
  "email": "maria@exemplo.com",
  "password": "senha-forte",
  "type": "common"
}
```

| Situação | Status | Corpo |
| --- | --- | --- |
| Sucesso | 201 | `{ "id", "name", "email", "document", "type" }` — e-mail normalizado; sem senha; sem header `Location` (não há `GET /users/{id}`) |
| E-mail já cadastrado (comparação sem diferenciar maiúsculas) | 409 | `code: email_taken`, `errors: [{ field: "email" }]` |
| Documento já cadastrado | 409 | `code: document_taken`, `errors: [{ field: "document" }]` |
| Campo inválido: nome vazio ou > 100; e-mail fora do formato `a@b.c` ou > 254; senha fora de 8–100; documento com algo além de dígitos, tamanho incompatível com o tipo ou dígitos verificadores inválidos; `type` ausente ou diferente de `common`/`merchant` (inclusive `admin`) | 422 | `code: validation_error`, um item em `errors` **por campo inválido**, todos de uma vez |
| JSON malformado, tipo errado num campo ou corpo vazio | 422 | `code: validation_error` (substitui o 400 automático do `[ApiController]`) |
| Erro inesperado | 500 | `code: internal_error`, sem mensagem interna nem stack trace (a exceção vai para o log) |

Todo erro é `application/problem+json`, no formato do [frontend.md](../frontend.md): `type` (`https://dudupay.local/errors/{code}`), `title`, `status`, `detail`, `code` e, quando há campo envolvido, `errors: [{ field, message }]`. Se e-mail **e** documento estiverem duplicados, volta só `email_taken`.

### Fluxo

1. A Api converte o corpo em `CadastrarUsuarioCommand`; `type` desconhecido vira `null`.
2. `Usuario.Criar` valida todos os campos, normaliza nome e e-mail e, se tudo estiver válido, gera o hash da senha (`IHasherSenha`). Inválido → 422 sem tocar no banco.
3. O Handler checa e-mail e depois documento (`SELECT EXISTS`), fora de transação. Duplicado → 409.
4. Abre a transação (`IUnidadeDeTrabalho`), grava o usuário (`INSERT … RETURNING id`) e a carteira com `Carteira.SaldoInicial` (1000.00), e confirma.
5. **Corrida:** se outro cadastro gravar o mesmo e-mail/documento entre o passo 3 e o 4, o `UNIQUE` barra o `INSERT` (`23505`); o repositório traduz pelo nome da constraint em 409, e o Handler desfaz a transação.
6. **Falha inesperada** em qualquer passo da transação → o Handler desfaz e relança; a Api responde 500. `DesfazerAsync` nunca lança (se o rollback falhar, registra no log e descarta a conexão), para não esconder a exceção original.

### Banco de dados

- Migration: [`database/migrations/01-usuarios-e-carteiras.sql`](../../database/migrations/01-usuarios-e-carteiras.sql).
- `usuarios`: `id BIGINT` identity, `nome VARCHAR(100)`, `email VARCHAR(254)`, `documento VARCHAR(14)`, `tipo VARCHAR(10)`, `hash_senha TEXT`, `data_criacao`.
  - `usuarios_email_key` e `usuarios_documento_key` (`UNIQUE`);
  - `usuarios_tipo_check`: `tipo IN ('comum', 'lojista')`;
  - `usuarios_documento_check`: 11 dígitos para comum, 14 para lojista.
- `carteiras`: `id BIGINT` identity, `usuario_id` (FK, `UNIQUE` — uma carteira por usuário), `saldo NUMERIC(15,2)` com `CHECK (saldo >= 0)`, `data_criacao`.
- Tipo na API `common`/`merchant` ↔ no banco `comum`/`lojista`.
- Local: `docker compose up -d postgres` aplica as migrations num volume novo (banco `pagamentos`, usuário/senha `postgres`).

### Código

| Camada | Arquivos principais |
| --- | --- |
| Domain | `Erros/` (`Result`, `Erro`, `ErroDeCampo`, `TipoErro`) · `Usuarios/Entidades/` (`Usuario`, `Carteira`, `TipoUsuario`) · `Usuarios/ValidadorDeDocumento` · `Usuarios/ErrosUsuario` · `Usuarios/Interfaces/` (`IUsuarioRepositorio`, `ICarteiraRepositorio`, `IHasherSenha`) · `Persistencia/IUnidadeDeTrabalho` |
| Application | `Usuarios/CadastrarUsuario/` (`CadastrarUsuarioCommand`, `CadastrarUsuarioHandler`, `UsuarioCadastradoDTO`) |
| Repository / Infrastructure | `Repository/Usuarios/` (`UsuarioRepositorio`, `CarteiraRepositorio`, `TipoUsuarioNoBanco`) · `Infrastructure/Data/` (`UnidadeDeTrabalho`, `IConexaoDeBanco`) · `Infrastructure/Seguranca/HasherSenha` |
| Api | `Usuarios/` (`UsuariosController`, `CadastrarUsuarioRequest`, `UsuarioResponse`, `TipoUsuarioNaApi`) · `Erros/` (`Problemas`, `RespostaDeBindingInvalido`, `TratadorDeExcecoes`) · `Configuracao/InjecaoDeDependencias` |
| Testes | `tests/Pagamentos.UnitTests` (51) · `tests/Pagamentos.IntegrationTests` (36; Postgres 17 via Testcontainers, um container por coleção, tabelas limpas antes de cada teste) |

Configuração comum em `backend/Directory.Build.props` (`net10.0`, nullable, `TreatWarningsAsErrors`). Pacotes Microsoft fixados em 10.0.15 (as versões 11.x não suportam `net10.0`).

## Critérios de aceite → testes

| Critério | Teste | Tipo |
| --- | --- | --- |
| `Result` carrega sucesso, valor ou erro | `ResultTests` (7, inclui `Erro_AposChecarSucessoFalso_CompiladorSabeQueNaoEhNulo`) | unidade |
| Comum com CPF e lojista com CNPJ válidos | `UsuarioTests.Criar_ComumComCpfValido_RetornaUsuario`, `Criar_LojistaComCnpjValido_RetornaUsuario` | unidade |
| CPF/CNPJ inválidos, formatados ou incompatíveis com o tipo | `UsuarioTests.Criar_CpfInvalido_RetornaFalha`, `Criar_CnpjInvalido_RetornaFalha`, `Criar_DocumentoIncompativelComTipo_RetornaFalhaNoCampoDocumento`, `Criar_TipoDesconhecidoEDocumentoInvalido_…` | unidade |
| Nome, e-mail, senha e tipo validados; todos os erros juntos | `UsuarioTests.Criar_CampoInvalido_RetornaFalhaNoCampo`, `Criar_VariosCamposInvalidos_RetornaTodosOsErros` | unidade |
| E-mail e nome normalizados; só o hash é guardado | `UsuarioTests.Criar_EmailComMaiusculasEEspacos_NormalizaEmail`, `Criar_NomeComEspacosNasPontas_RemoveEspacos`, `Criar_DadosValidos_GuardaHashENaoASenha` | unidade |
| Carteira abre com R$ 1.000,00 | `CarteiraTests.Abrir_NovoUsuario_SaldoInicialMil` | unidade |
| Handler: cria e confirma; recusa duplicados sem abrir transação; desfaz em conflito e em falha | `CadastrarUsuarioHandlerTests` (7) | unidade |
| Hash verifica a senha certa, recusa a errada, tem salt | `HasherSenhaTests` (3) | unidade |
| Constraints do banco (`UNIQUE`, `CHECK`, FK) | `MigrationsTests` (9) | integração |
| Unit of Work confirma, desfaz, desfaz no dispose e não lança com conexão perdida | `RepositoriosTests.Confirmar_…`, `Desfazer_AposInserirUsuario_NaoGravaNada`, `Dispose_SemConfirmar_NaoGravaNada`, `Desfazer_ConexaoPerdida_NaoLancaENaoGravaNada` | integração |
| Repositório traduz violação de `UNIQUE` em conflito | `RepositoriosTests.AdicionarAsync_ViolacaoDeUnique_RetornaConflito` | integração |
| 201 para comum e lojista, com usuário e carteira (1.000) no banco | `UsuariosEndpointTests.CadastrarUsuario_DadosValidos_Retorna201ECriaUsuarioECarteira` | integração |
| Senha gravada como hash | `UsuariosEndpointTests.CadastrarUsuario_Valido_GravaHashDaSenha` | integração |
| 409 `email_taken` (outra caixa) e `document_taken`, sem gravar | `UsuariosEndpointTests.CadastrarUsuario_EmailDuplicadoOutraCaixa_…`, `CadastrarUsuario_DocumentoDuplicado_…` | integração |
| 422 por campo, sem gravar (inclui `type: "admin"` e `type` ausente) | `UsuariosEndpointTests.CadastrarUsuario_DadosInvalidos_Retorna422ComErroNoCampoENaoGrava`, `CadastrarUsuario_VariosCamposInvalidos_…` | integração |
| 422 para JSON malformado | `UsuariosEndpointTests.CadastrarUsuario_JsonMalformado_Retorna422ValidationError` | integração |
| Falha na carteira → 500 sem vazar detalhe e sem gravar o usuário | `UsuariosEndpointTests.CadastrarUsuario_FalhaAoAbrirCarteira_Retorna500ENaoGravaUsuario` | integração |
| 10 cadastros simultâneos com o mesmo e-mail → um 201, os demais 409, uma linha no banco | `UsuariosEndpointTests.CadastrarUsuario_CadastrosSimultaneosMesmoEmail_UmRetorna201EOsDemais409` | integração |

## Fora de escopo / melhorias futuras

- Movimentação `welcome_credit` do saldo inicial (entra com o modelo de movimentações/transferências).
- **CNPJ alfanumérico:** desde julho de 2026 a Receita emite CNPJ com letras nas 12 primeiras posições; o contrato atual ("só dígitos"), o validador e o `CHECK` do banco recusam esses lojistas.
- 🟡 (review) Validar `ConnectionStrings:Pagamentos` na subida da API; hoje a ausência só aparece na primeira requisição.
- 🟡 (review) Senha do Postgres local versionada em `appsettings.Development.json` (mesmo padrão do compose); usar *user-secrets* ou variável de ambiente quando houver deploy.
- 🟡 (review) O hash da senha é calculado antes da checagem de duplicidade, então um 409 também paga o custo do PBKDF2.
- 🟡 (review) Nomes de `MigrationsTests` com duas partes; cada teste de endpoint cria sua própria `ApiFactory` (compartilhar quando a suíte crescer).
- Correlation id, health check, CORS, API no compose e seed — itens próprios do roadmap.

## Histórico

- 2026-11-20: entregue via /goal ([#5](https://github.com/deiz0n/desafio-picpay-dotnet-react/pull/5)). Tentado e descartado: o teste de `CHECK` com `'administrador'` (estoura o `VARCHAR(10)` antes do `CHECK`; trocado por `'admin'`); `[Produces("application/json")]` no controller (sobrescrevia o `application/problem+json` dos erros). Bug encontrado pela integração e corrigido: com `type` desconhecido, o documento não era validado.
