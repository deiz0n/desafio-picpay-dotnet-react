# 0005 — Usar Unit of Work para transações entre repositórios

- **Status:** accepted
- **Data:** 2026-11-06
- **Spec de origem:** —
- **Substitui:** —

## Contexto

A transferência precisa ser atômica: debitar o pagador, creditar o recebedor e registrar a transferência — tudo ou nada (ver `docs/challenge.md`, requisito 6). Com Dapper (ADR 0004), cada comando SQL só participa da transação se receber **a mesma conexão e a mesma transação**. Se cada `*Repositorio` abrir sua própria conexão, cada operação vira uma transação isolada e um rollback parcial pode fazer dinheiro sumir.

O Handler também precisa controlar a ordem do fluxo (ex.: consultar o autorizador antes de confirmar), e o Domain não pode saber que existe banco (ADR 0003).

## Decisão

Vamos usar o padrão **Unit of Work** (Fowler, *Patterns of Enterprise Application Architecture*):

- Uma interface de unidade de trabalho na **Application**, com operações de negócio — iniciar, confirmar, desfazer — sem expor tipos de banco (`IDbConnection`, `IDbTransaction`).
- A implementação fica na **Infrastructure**, é dona da conexão Npgsql e da transação.
- Registrada no DI como **Scoped**: uma instância por requisição, compartilhada por todos os `*Repositorio` daquela requisição, que a recebem pelo construtor e não abrem conexões próprias.
- O **Handler** delimita a transação: inicia, orquestra domínio + repositórios, confirma em caso de sucesso e desfaz em caso de falha.

## Alternativas consideradas

| Alternativa | Prós | Contras |
| --- | --- | --- |
| Unit of Work Scoped (escolhida) | Handler controla os limites da transação; Domain sem conhecimento de banco; pratica DI e lifetimes | Peça extra; risco de esquecer de confirmar/desfazer |
| Transação dentro de um método do repositório (padrão atual do CVL) | Simples; explícito; igual ao CVL | Repositório passa a conhecer a operação de negócio inteira; não permite intercalar chamada HTTP (autorizador) dentro da transação |
| `TransactionScope` (transação ambiente) | Pouquíssimo código | "Mágica" implícita; com `async` exige `TransactionScopeAsyncFlowOption.Enabled`, cujo esquecimento causa bugs silenciosos |
| Passar `IDbTransaction` como parâmetro nos métodos dos repositórios | Explícito | Vaza tipo de infraestrutura para as interfaces do Domain — viola o ADR 0003 |

## Consequências

### Positivas

- Atomicidade garantida entre vários repositórios.
- Ordem do fluxo (lock → validações → autorizador → débito/crédito → confirmar) decidida no Handler, não no SQL.
- Repositórios continuam testáveis isoladamente.

### Negativas / trade-offs

- **Diverge do padrão do CVL** (transação dentro do repositório) — divergência consciente, por motivo didático e pelo requisito de intercalar o autorizador.
- Lifetime errado quebra tudo: `Transient` → cada repositório com sua transação; `Singleton` → requisições concorrentes compartilhando a mesma transação.
- Transação aberta durante a chamada ao autorizador mantém locks por mais tempo — tratar com timeout curto (decisão ainda pendente sobre a posição do autorizador).

### O que fazer se a decisão mudar

Novo ADR. Voltar ao padrão do CVL significa mover a orquestração para um método de repositório e remover a interface de unidade de trabalho.
