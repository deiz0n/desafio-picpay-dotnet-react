# 0019 — `Result` e `IUnidadeDeTrabalho` no projeto Domain

- **Status:** accepted
- **Data:** 2026-11-20
- **Spec de origem:** [02-fundacao-backend-cadastro](../specs/02-fundacao-backend-cadastro.md)
- **Substitui:** — (complementa o [0003](0003-clean-architecture-em-camadas-padrao-cvl.md) e o [0005](0005-unit-of-work-para-transacoes.md) sem mudar suas decisões centrais)

## Contexto

Ao criar a solução, dois pontos dos ADRs anteriores não fecharam com a regra de dependência:

1. O [ADR 0003](0003-clean-architecture-em-camadas-padrao-cvl.md) lista `Result`/`Result<T>` como parte da **Application**. Mas o Domain rico precisa devolver erros de validação sem exceção (`Usuario.Criar` devolve todos os erros de campo juntos), e o Domain não referencia a Application.
2. O [ADR 0005](0005-unit-of-work-para-transacoes.md) põe a interface da unidade de trabalho na **Application** e a implementação na **Infrastructure**. Pelo ADR 0003, a Infrastructure depende **só do Domain** — ela não consegue implementar uma interface da Application.

## Decisão

Vamos colocar no **Domain**:

- `Result`, `Result<T>`, `Erro`, `ErroDeCampo` e `TipoErro`, em `Pagamentos.Domain/Erros/` (a pasta de transversais do ADR 0003). `TipoErro` (`Validacao`, `Conflito`, `NaoEncontrado`) é traduzido em status HTTP só na Api.
- `IUnidadeDeTrabalho`, em `Pagamentos.Domain/Persistencia/`, junto dos demais ports (`I*Repositorio`, `IHasherSenha`). Continua sem expor tipos de banco.

O resto dos ADRs 0003 e 0005 segue valendo: Handlers na Application, Handler delimita a transação, implementação Scoped na Infrastructure.

## Alternativas consideradas

| Alternativa | Prós | Contras |
| --- | --- | --- |
| `Result` e `IUnidadeDeTrabalho` no Domain (escolhida) | Um único tipo de resultado do Domain à Api; regra de dependência intacta; ports todos no mesmo lugar | O Domain carrega um tipo "técnico" (`Result`); desvia do texto dos ADRs 0003/0005 |
| `Result` na Application; Domain devolve lista de erros ou lança exceção | Segue o texto do ADR 0003 à risca | Dois mecanismos de erro e mais mapeamento; exceção para validação contraria o próprio ADR 0003 |
| Infrastructure referenciando a Application | Mantém a interface na Application | Quebra a regra de dependência do ADR 0003 (Infrastructure passaria a ver casos de uso) |

## Consequências

### Positivas

- O compilador garante a regra de dependência: Domain não referencia nada.
- `Sucesso` anotado com `[MemberNotNullWhen(false, nameof(Erro))]`: depois de checar a falha, o compilador sabe que `Erro` não é nulo, sem `!`.

### Negativas / trade-offs

- Quem lê só o ADR 0003 espera achar `Result` na Application — este ADR precisa ser conhecido.
- `Erro.Codigo` usa os códigos do contrato da API (`email_taken`); os **nomes de campo** ficam em português no Domain e a Api os traduz (`documento` → `document`).

### O que fazer se a decisão mudar

Mover os tipos para um projeto compartilhado (ex.: `Pagamentos.SharedKernel`) referenciado por todos, ou para a Application, ajustando as referências entre projetos e os `using`.
