# 0001: Arquitetura em camadas e Blazor com API unificada

**Estado:** aceita.

## Contexto

A evolução do Survey Data Entry demanda sair do formato desktop legado de 2014 para uma aplicação web moderna, segura e com base sólida para expansão. O sistema precisa atender à interface web para gestores e operadores, além de servir como provedor de dados para o futuro aplicativo móvel numa segunda etapa. A solução deve funcionar em qualquer serviço de hospedagem compatível com .NET, sem custos operacionais proibitivos ou sobrecarga desnecessária de infraestrutura.

## Decisão

- Adotar .NET 10 LTS, ASP.NET Core, Blazor Web App, Entity Framework Core e SQL Server como base tecnológica oficial.
- Utilizar formato moderno de solução `.slnx` e organizar o repositório em camadas de responsabilidade única segundo os princípios de Clean Architecture:
  - `Domain`: concentra entidades puras, agregados, contratos de repositórios e invariantes de negócio. Não possui dependências de frameworks de interface, bibliotecas de persistência ou ferramentas de telemetria.
  - `Application`: coordena os casos de uso, regras de validação da aplicação e serviços orquestradores. Interage com o domínio e com abstrações da infraestrutura.
  - `Infrastructure`: implementa acesso a dados com EF Core, provedores de e-mail transacional (Resend configurável e fallback local para desenvolvimento) e integrações externas.
  - `Contracts`: compartilha DTOs de entrada e saída, enums e códigos estáveis de resultado com a interface web e clientes externos.
  - `Web`: projeto executável único em ASP.NET Core que hospeda a interface Blazor Web App e os endpoints de API HTTP versionados sob o prefixo `/api/v1`.
- A interface Blazor utiliza renderização estática no servidor por padrão (SSR estático) para otimizar velocidade e indexação, ativando interatividade nos componentes e formulários que realmente demandam comportamento dinâmico.
- Domínio e contratos públicos permanecem estritamente desacoplados de Blazor, EF Core e Application Insights.

## Alternativas avaliadas

- Separar a interface em um projeto Node.js/TypeScript e a API em .NET: duplicaria regras de validação, modelos de dados e esforços de implantação, aumentando a complexidade de desenvolvimento sem benefício tangível para o tamanho do time.
- Arquitetura baseada em múltiplos microsserviços: introduziria latência de rede, necessidade de orquestradores distribuídos e sobrecarga desproporcional para as fases atuais de entrega do produto.
- Renderização exclusiva via Blazor WebAssembly com download integral de binários: prejudicaria o tempo de carregamento inicial em redes móveis e exigiria configuração adicional de proxy para páginas públicas.

## Consequências

- Um único processo ASP.NET Core reduz a complexidade de deploy, o consumo de recursos e os custos operacionais de hospedagem.
- A camada de aplicação atende tanto a interface web interna quanto a API externa consumida pelo aplicativo móvel, eliminando divergências de regras de negócio.
- O isolamento do domínio garante testabilidade limpa através de testes de unidade sem necessidade de simular conexões de banco de dados ou requisições HTTP.
