# Decisões de Arquitetura | ADRs

Este diretório registra as Decisões de Arquitetura (Architecture Decision Records) que orientam o desenvolvimento, a organização de código e a modernização do **Survey Data Entry**.

Cada documento segue uma estrutura padronizada contendo contexto, decisão adotada, alternativas avaliadas e consequências práticas para o sistema.

---

## Índice de Decisões

| Número                                                                | Título                                                          | Estado | Escopo                                                         |
| :-------------------------------------------------------------------- | :-------------------------------------------------------------- | :----- | :------------------------------------------------------------- |
| [0001](0001-arquitetura-em-camadas-e-blazor-com-api.md)               | Arquitetura em camadas e Blazor com API unificada               | Aceita | Estrutura da solução, Clean Architecture e hospedagem          |
| [0002](0002-limites-do-dominio-agregados-e-uuid-v7.md)                | Limites do domínio, agregados e identificadores UUID v7         | Aceita | Agregados Conta, Pesquisa, Ficha respondida e chaves primárias |
| [0003](0003-autenticacao-identity-e-vinculo-social.md)                | Autenticação via ASP.NET Core Identity e vínculos sociais       | Aceita | Segurança de contas, confirmação de e-mail e OAuth social      |
| [0004](0004-isolamento-de-pesquisas-e-revogacao-de-operadores.md)     | Isolamento de pesquisas, autorização e revogação imediata       | Aceita | Permissões em tempo real e formato ProblemDetails              |
| [0005](0005-localizacao-bilingue-e-roteamento-geografico.md)          | Localização bilíngue e roteamento com orientação geográfica     | Aceita | Recursos .resx, detecção por proxy e URLs localizadas          |
| [0006](0006-aplicativo-movel-dotnet-maui-e-contratos-api.md)          | Aplicativo móvel em .NET MAUI e contratos de API compartilhados | Aceita | Planejamento da Fase 2 e contratos em biblioteca compartilhada |
| [0007](0007-modalidades-de-pesquisa-e-conferencia-ocr.md)             | Modalidades de pesquisa e conferência assistida por OCR         | Aceita | Modalidade imutável, digitação rápida e apoio de OCR           |
| [0008](0008-migracoes-versionadas-ef-core-e-sql-server.md)            | Migrações versionadas com Entity Framework Core e SQL Server    | Aceita | Evolução de esquema, expandir e contrair, e segregação DDL/DML |
| [0009](0009-observabilidade-com-opentelemetry-e-telemetria-segura.md) | Observabilidade com OpenTelemetry e telemetria segura           | Aceita | Tracing, métricas locais, Application Insights e higienização  |
| [0010](0010-politica-de-selecao-e-revisao-de-dependencias.md)         | Política de seleção, fixação e revisão de dependências          | Aceita | Governança de pacotes NuGet, version pinning e auditoria       |

---

## Critérios de Evolução

- Uma ADR aceita não deve ser editada retrospectivamente para alterar decisões históricas.
- Caso uma decisão seja superada por uma nova abordagem, cria-se uma nova ADR referenciando a decisão anterior com o status `substituída por [ADRX]`.
- Todas as decisões devem manter alinhamento estrito com as diretrizes registradas no plano do produto e com as boas práticas de engenharia de software em .NET.

## Decisões adicionais

- [0011: Persistência do questionário e concorrência otimista](0011-persistencia-do-questionario-e-concorrencia.md)
