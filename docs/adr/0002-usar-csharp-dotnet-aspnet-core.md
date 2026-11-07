# 0002 — Usar C# com .NET 10 e ASP.NET Core (Controllers)

- **Status:** accepted
- **Data:** 2026-11-06
- **Spec de origem:** —
- **Substitui:** —

## Contexto

O desafio permite qualquer linguagem. O projeto é de estudos e o objetivo é aprender C# aplicando os mesmos padrões de um projeto real já em uso (CVL), para que o aprendizado seja transferível.

## Decisão

Vamos usar **C# / .NET 10** com **ASP.NET Core**, expondo endpoints via **Controllers** (`[ApiController]` + `ControllerBase`), não Minimal API.

## Alternativas consideradas

| Alternativa | Prós | Contras |
| --- | --- | --- |
| .NET 10 + Controllers (escolhida) | Mesmo padrão do CVL; separação explícita; fácil de navegar com F12/debug | Mais cerimônia que Minimal API |
| .NET 10 + Minimal API | Menos código; moderno | Diverge do CVL; tende a misturar roteamento e lógica em quem está aprendendo |
| PHP (sugestão do enunciado) | Alinhado às dicas do desafio (PSRs, PHPStan) | Não atende ao objetivo de aprender C# |

## Consequências

### Positivas

- Conhecimento reaproveitável no CVL e vice-versa.
- Ferramentas maduras: DI nativa, `IHttpClientFactory`, `ILogger`, testes com xUnit.

### Negativas / trade-offs

- Dicas de qualidade do enunciado (PHPMD, PSRs) não se aplicam; usar equivalentes .NET (analyzers, `dotnet format`, `.editorconfig`).

### O que fazer se a decisão mudar

Novo ADR; as regras de negócio e specs continuam válidas, só o código muda.
