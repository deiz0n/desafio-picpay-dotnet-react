# Template — corpo do PR

Título: mesmo formato de commit — `feat(area): descrição curta`.

```markdown
## Resumo

1–3 frases: o que este PR entrega e por quê. Item do roadmap: … | —

## O que mudou

- **Domain:** ...
- **Application:** ...
- **Repository / Infrastructure:** ...
- **Api:** ...
- **Banco:** migration `NN-slug.sql` — ...

## Como foi testado

`dotnet test`: **N testes** (X unidade, Y integração) — todos verdes.

| Critério de aceite | Teste | Tipo |
| --- | --- | --- |
| ... | `Classe.Metodo_Cenario_Resultado` | integração (valida banco) |

## Documentação

- SPEC: [`docs/specs/NN-slug.md`](docs/specs/NN-slug.md)
- ADRs: [NNNN — título](docs/adr/NNNN-slug.md) | nenhum

## Code review (/code-review)

- Veredito: aprovado | aprovado com ressalvas
- 🔴 resolvidos: ... | nenhum
- 🟡 adiados (registrados na SPEC): ... | nenhum

## Checklist

- [x] Build sem warnings novos
- [x] `dotnet format --verify-no-changes`
- [x] `dotnet test` verde (unidade + integração)
- [x] Todo critério de aceite tem teste
- [x] SPEC, ADRs e roadmap atualizados
```

