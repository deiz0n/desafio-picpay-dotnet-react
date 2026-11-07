---
description: Convenção de testes automatizados — xUnit, unidade + integração com WebApplicationFactory e Postgres via Testcontainers validando o banco
trigger: always_on
---

# Testes automatizados

Decisão e porquês: [ADR 0007](../../docs/adr/0007-xunit-com-testcontainers.md). Nenhuma fatia do `/goal` é concluída sem um teste que a prove.

## Tipos

| Tipo | Projeto | O que prova | Ferramentas |
| --- | --- | --- | --- |
| Unidade | `Pagamentos.UnitTests` | Regras do Domain (entidades) e Handlers com ports falsos | xUnit |
| Integração | `Pagamentos.IntegrationTests` | Endpoint HTTP de ponta a ponta **+ estado no banco** | xUnit, `Microsoft.AspNetCore.Mvc.Testing` (`WebApplicationFactory<Program>`), `Testcontainers.PostgreSql`, Dapper nos asserts |

Asserts com o próprio xUnit (`Assert.*`). Sem FluentAssertions (v8 tem licença comercial).

## Regras

1. **Teste primeiro:** escreva o teste da fatia e rode — ele precisa **falhar pelo motivo certo** (não por erro de compilação esquecido ou setup quebrado) antes de implementar.
2. **Nome:** `Metodo_Cenario_ResultadoEsperado` — ex.: `Transferir_PagadorLojista_Retorna422ENaoAlteraSaldos`.
3. **Integração valida o banco.** Todo teste de integração que escreve (ou deveria falhar sem escrever) consulta o Postgres com SQL direto e confere:
   - saldos de pagador e recebedor;
   - linha registrada (ou ausente) em `transferencias`;
   - em erro/rollback: **nada** mudou.
   Checar só o status HTTP não basta.
4. **Serviços externos sempre fakes.** Autorizador e notificação são substituídos no DI do `WebApplicationFactory` (`ConfigureTestServices`) por fakes controláveis (autoriza / nega / timeout / erro). Nenhum teste chama `util.devi.tools`.
5. **Schema real:** as migrations de `database/migrations/` são aplicadas no container antes dos testes — os mesmos arquivos usados em produção.
6. **Isolamento:** um container Postgres por classe/coleção de teste (`IClassFixture` / `ICollectionFixture` + `IAsyncLifetime`); cada teste cria seus próprios dados (ou limpa as tabelas antes) e não depende da ordem.
7. **Caminhos de erro contam:** cada regra de negócio tem pelo menos um teste do caminho feliz e um de rejeição.
8. **Proibido** para ficar verde: apagar, comentar, `Skip` ou afrouxar um assert sem combinar com o usuário.
9. Concorrência (duas transferências simultâneas do mesmo pagador) é testada na integração com `Task.WhenAll`, conferindo o saldo final no banco.

## Esqueleto de referência (integração)

```csharp
public class TransferirTests(PostgresFixture db) : IClassFixture<PostgresFixture>
{
    [Fact]
    public async Task Transferir_SaldoInsuficiente_Retorna422ENaoAlteraSaldos()
    {
        // Arrange: seed via SQL direto no container
        // Act: client.PostAsJsonAsync("/transfer", new { value = 100.0m, payer = 1, payee = 2 })
        // Assert: status 422 + SELECT saldo FROM carteiras ... igual ao inicial + COUNT(*) em transferencias = 0
    }
}
```

## Comandos

- Tudo: `dotnet test backend/Pagamentos.sln` (Docker precisa estar rodando)
- Uma classe: `dotnet test backend/Pagamentos.sln --filter "FullyQualifiedName~TransferirTests"`
- Só unidade: `dotnet test backend/tests/Pagamentos.UnitTests`

