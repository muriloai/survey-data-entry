# 0009: Observabilidade com OpenTelemetry e telemetria segura

**Estado:** aceita.

## Contexto

Ambientes de produção exigem acompanhamento de saúde da aplicação, diagnóstico precoce de gargalos de rede ou banco de dados e notificação imediata de exceções não tratadas. Ao mesmo tempo, dados coletados em pesquisas contêm informações de negócios confidenciais e credenciais de usuários, demandando regras rígidas de privacidade e conformidade com a LGPD.

## Decisão

- Instrumentar a aplicação ASP.NET Core utilizando a biblioteca nativa do OpenTelemetry para coleta de métricas, traces distribuídos e logs estruturados.
- Suporte configurável para exportação de dados ao Azure Application Insights:
  - A conexão com o Application Insights é ativada exclusivamente quando a cadeia de conexão (`APPLICATIONINSIGHTS_CONNECTION_STRING`) for fornecida nas variáveis de ambiente.
  - Em ambiente de desenvolvimento local, a aplicação utiliza apenas logs estruturados no console por padrão, operando de maneira 100% autônoma e sem necessidade de credenciais de nuvem.
- Monitoramento voltado para aspectos operacionais essenciais:
  - Duração e códigos de status das requisições HTTP recebidas.
  - Desempenho e latência de comandos executados no SQL Server.
  - Taxa de sucesso e falhas na entrega de e-mails transacionais.
  - Rastreamento de exceções não tratadas com pilha de chamadas e códigos de diagnóstico.
- Política de segurança e higienização estrita de telemetria:
  - Senhas, hashes de autenticação, segredos de aplicação e tokens de confirmação ou recuperação são estritamente excluídos dos logs e traces.
  - Parâmetros com valores literais de consultas SQL não são gravados na telemetria.
  - O conteúdo confidencial das respostas registradas em fichas de pesquisa jamais é enviado a coletores de telemetria externos.

## Alternativas avaliadas

- Adoção exclusiva do SDK proprietário do Application Insights sem OpenTelemetry: acoplaria a telemetria ao ecossistema de um único provedor de nuvem, dificultando migrações futuras para outros agregadores (ex: Prometheus, Grafana ou Datadog).
- Registro indiscriminado de corpo de requisições e payloads nos logs: exporia dados pessoais sensíveis e credenciais de acesso, configurando violação das diretrizes de privacidade do projeto.

## Consequências

- Adoção de padrão aberto de telemetria com portabilidade de exportadores de métricas e rastreamento.
- Proteção garantida para dados pessoais e credenciais em logs e monitoramentos.
- Ambiente de desenvolvimento local leve, rápido e independente de qualquer serviço externo de telemetria.
