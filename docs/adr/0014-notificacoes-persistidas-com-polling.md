# 0014 — Notificações persistidas e entregues por polling

- **Status:** accepted
- **Data:** 2026-11-08
- **Spec de origem:** —
- **Substitui:** —

## Contexto

O front terá um **drawer de notificações** com as transferências recebidas. O requisito do desafio ([item 7](../challenge.md#requisitos)) fala de notificar o recebedor por um serviço **externo** (`POST`, instável); isso é um canal diferente, de servidor para servidor. Aqui o problema é outro: mostrar, dentro do app, o que o usuário recebeu — inclusive o que chegou enquanto ele estava offline.

Dois problemas distintos: **onde a notificação fica guardada** e **como ela chega ao navegador**.

## Decisão

1. **Persistência:** cada transferência recebida gera um registro de notificação (coleção `notifications` no mock; tabela própria no backend futuro) com estado **lida/não lida**. O histórico é a fonte de verdade.
2. **Entrega:** **polling** a cada ~30 segundos com TanStack Query (`refetchInterval`, [ADR 0011](0011-estado-de-servidor-com-tanstack-query.md)), pausado com a aba em segundo plano. O ícone mostra o **contador de não lidas**.
3. O transporte fica escondido atrás do hook `useNotificacoes()`: os componentes não sabem se é polling ou outra coisa.

Endpoints: `GET /notifications` e `PATCH /notifications/{id}` (marcar como lida).

WebSocket/SSE **não** entram agora. Entram no roadmap como melhoria futura, com ADR próprio que substitua este quando for feito.

## Alternativas consideradas

| Alternativa | Prós | Contras |
| --- | --- | --- |
| Persistida + polling (escolhida) | Funciona igual no mock e no backend real; sem infraestrutura nova | Atraso de até ~30 s; requisições periódicas por usuário |
| Persistida + SSE | Fluxo de mão única, que é o caso; reconexão automática; suporte nativo no ASP.NET Core | O mock precisaria simular SSE agora, para depois descartar |
| Persistida + WebSocket (SignalR) | Tecnologia idiomática do .NET, boa para estudo | Bidirecional sem necessidade aqui; mais complexidade de conexão e autenticação |
| Webhook | — | Não se aplica: navegador não recebe chamadas HTTP |
| Derivar de `GET /transfers?payee=me` | Sem tabela nova | Sem estado de "lida"; acopla notificação ao extrato |

## Consequências

### Positivas

- Mesma implementação no front para o mock e para a API real.
- Histórico disponível mesmo para quem estava offline.
- Trocar o transporte depois não mexe nos componentes.

### Negativas / trade-offs

- Notificação aparece com atraso de até ~30 s.
- Carga constante de requisições proporcional ao número de usuários com o app aberto.
- O backend precisará de tabela, endpoints e do evento que gera a notificação (item novo no roadmap); relaciona-se com a decisão pendente de outbox da notificação externa.

### O que fazer se a decisão mudar

Novo ADR que substitua este (SSE ou SignalR). Como a persistência e os endpoints permanecem, só muda o hook de entrega.
