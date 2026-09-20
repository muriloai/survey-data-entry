# 0005: Localização bilíngue e roteamento com orientação geográfica

**Estado:** aceita.

## Contexto

O Survey Data Entry foi planejado para atender usuários em português do Brasil e em inglês. A experiência deve abranger páginas públicas, painéis de gestão, mensagens de validação, e-mails transacionais e erros retornados pela API. Além disso, as páginas institucionais precisam de URLs estruturadas para indexação correta e compartilhamento com idioma explícito.

## Decisão

- Centralizar as mensagens e termos da aplicação em arquivos de recursos padrão do .NET:
  - Recursos neutros (`.resx`): idioma padrão em inglês.
  - Recursos em português do Brasil (`.pt-BR.resx`): traduções completas para português.
  - Os recursos ficam situados em projeto compartilhado para consumo unificado pela interface web e futuros clientes.
- Implementar estratégia em camadas para determinação do idioma:
  1. Preferência explícita gravada no perfil da conta (quando o usuário estiver autenticado).
  2. Cookie de idioma definido quando o visitante escolhe a opção no seletor de idioma antes do login.
  3. Ausência de preferência prévia: utilizar `pt-BR` quando o cabeçalho de país fornecido pela infraestrutura de rede confiável indicar Brasil, e inglês em qualquer outro caso. O cabeçalho de país é aceito unicamente se proveniente de proxy reverso configurado que sobrescreva valores recebidos da internet pública.
- Páginas públicas institucionais contam com prefixos explícitos de rota para cada idioma (ex: `/br/` e `/en/`). Cada página define atributos `lang`, título, descrição, tag canônica, apontamentos `hreflang` e presença no sitemap. Uma URL com prefixo explícito sempre exibirá o idioma solicitado, independentemente de geolocalização.
- A detecção geográfica atua apenas no redirecionamento da primeira visita à raiz do site (`/`), preservando o comportamento canônico de links externos compartilhados.

## Alternativas avaliadas

- Suportar unicamente o idioma português do Brasil: limitaria a atuação do sistema e entraria em desacordo com as diretrizes do plano de evolução do produto.
- Confiar exclusivamente no cabeçalho `Accept-Language` enviado pelo navegador: frequentemente reflete apenas a instalação do sistema operacional ou navegador em inglês, contrariando a preferência real de usuários brasileiros em ambientes corporativos.

## Consequências

- O catálogo centralizado em `.resx` garante consistência textual e impede a proliferação de strings literais soltas no código.
- A separação de rotas públicas com `hreflang` previne penalizações de SEO e garante indexação clara para motores de busca internacionais.
- O isolamento da checagem de geolocalização a proxies confiáveis evita fraudes de cabeçalho por usuários maliciosos.
