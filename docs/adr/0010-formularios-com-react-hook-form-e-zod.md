# 0010 — Formulários com React Hook Form e Zod

- **Status:** accepted
- **Data:** 2026-11-08
- **Spec de origem:** —
- **Substitui:** —

## Contexto

O front tem vários formulários com regras de validação do domínio: cadastro (tipo de usuário, nome, CPF/CNPJ com dígitos verificadores, e-mail, senha), login, perfil e transferência (valor > 0, destinatário). Os erros do backend (422, 409) também precisam aparecer ligados aos campos certos.

## Decisão

Vamos usar **React Hook Form** para o estado dos formulários e **Zod** para os schemas de validação, ligados por `@hookform/resolvers`. Os componentes de formulário do shadcn/ui já se integram ao React Hook Form.

Os schemas ficam junto de cada feature; a validação de CPF/CNPJ é uma função pura testável isoladamente.

## Alternativas consideradas

| Alternativa | Prós | Contras |
| --- | --- | --- |
| React Hook Form + Zod (escolhida) | Poucos re-renders; schema reutilizável e com tipos inferidos; integra com shadcn | Duas bibliotecas; resolvers como camada extra |
| Formik + Yup | Muito difundido | Mais re-renders; menos ativo; integra pior com shadcn |
| Estado manual com `useState` | Nenhuma dependência | Muito código repetido; fácil esquecer validação ou estado de erro |
| Server Actions / form nativo | Menos JS | Não se aplica a uma SPA com API separada |

## Consequências

### Positivas

- Tipos TypeScript inferidos do schema (uma fonte de verdade por formulário).
- Validação testável sem renderizar o componente.

### Negativas / trade-offs

- A validação do front **não substitui** a do backend: as regras são duplicadas e precisam ficar alinhadas ao contrato.
- Zod 4 e RHF evoluem; versões devem ser fixadas.

### O que fazer se a decisão mudar

Novo ADR. Os schemas Zod podem ser reaproveitados com outra biblioteca de formulários; a troca fica restrita aos componentes de formulário.
