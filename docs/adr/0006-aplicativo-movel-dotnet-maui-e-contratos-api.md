# 0006: Aplicativo móvel em .NET MAUI e contratos de API compartilhados

**Estado:** aceita.

## Contexto

A Fase 2 do projeto contempla disponibilizar um aplicativo móvel voltado para operadores em campo realizarem coletas presenciais. O aplicativo precisa consultar pesquisas atribuídas, autenticar o operador e registrar fichas de questionários de forma ágil, integrando-se diretamente ao mesmo backend e mantendo coerência com os fluxos da aplicação web.

## Decisão

- Adotar .NET MAUI como tecnologia oficial de planejamento para o aplicativo móvel dos operadores, iniciando com entrega voltada à plataforma Android em modo conectado.
- O aplicativo consumirá os mesmos casos de uso da aplicação web por meio da API HTTP versionada sob o prefixo `/api/v1`.
- Criar a biblioteca de classes compartilhada `SurveyDataEntry.Contracts` contendo:
  - DTOs tipados de requisição e resposta.
  - Enums e estruturas de dados de perguntas e opções.
  - Códigos estáveis de erro para tratamento programático de falhas.
- Reutilizar os arquivos de recursos `.resx` do projeto compartilhado para que labels, validações e mensagens de erro exibidas no aplicativo tenham a mesma redação e localização da interface web.
- A comunicação com a API utiliza o formato RFC 7807 (`ProblemDetails`), permitindo ao aplicativo interpretar os códigos constantes de erro sem depender de parsing de textos ou mensagens dinâmicas.

## Alternativas avaliadas

- Desenvolver o aplicativo em ecossistema separado com React Native ou Flutter: exigiria manter duas bases de código em linguagens distintas, duplicar modelos de contratos e despender tempo reescrevendo regras de serialização e validação.
- Desenvolver apenas uma Progressive Web App (PWA): apresentaria limitações em desempenho de digitação intensa em dispositivos móveis de entrada e menor integração com recursos futuros de câmera e hardware.

## Consequências

- A equipe de desenvolvimento foca em um único ecossistema (.NET e C#), compartilhando tipos e contratos entre servidor e clientes.
- A padronização em `Contracts` garante que evoluções na API sejam refletidas de imediato no aplicativo móvel com checagem em tempo de compilação.
- O registro desta decisão estabelece uma premissa clara de planejamento para a Fase 2, separando as entregas imediatas da web das etapas subsequentes.
