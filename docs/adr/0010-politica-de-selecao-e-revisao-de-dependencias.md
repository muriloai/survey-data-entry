# 0010: Política de seleção, fixação e revisão de dependências

**Estado:** aceita.

## Contexto

A adição indiscriminada de pacotes externos aumenta a superfície de ataque da aplicação, introduz dependências transitivas desconhecidas, dificulta diagnósticos de falhas e pode causar incompatibilidades silenciosas em atualizações de versão. A governança do projeto precisa de regras claras para a escolha, homologação e manutenção de pacotes no ecossistema .NET.

## Decisão

- Priorizar o uso de bibliotecas e recursos nativos fornecidos diretamente pelo framework .NET 10 LTS antes de avaliar a inclusão de qualquer pacote de terceiros.
- Estabelecer critérios mandatórios para homologação de novos pacotes NuGet:
  1. Procedência e manutenção: pacotes devem ser mantidos pela Microsoft, por fundações reconhecidas (como a .NET Foundation) ou por organizações com histórico consolidado e commits recentes.
  2. Licença permissiva: licenças compatíveis com os termos de distribuição e propriedade intelectual do projeto (ex: MIT, Apache-2.0, BSD).
  3. Segurança: ausência de vulnerabilidades ativas registradas nos bancos de dados públicos de segurança de pacotes.
  4. Tamanho de árvore de dependências: pacotes com poucas ou nenhuma dependência transitiva são preferidos em relação a pacotes volumosos com múltiplos módulos auxiliares.
- Adoção de fixação estrita de versões (version pinning):
  - Todas as referências a pacotes utilizam números de versão exatos e imutáveis nos arquivos de projeto.
  - Proibição de intervalos de versão ou caracteres curinga (ex: `10.*` ou `[1.0, 2.0)`).
- Registro de auditoria datada:
  - Qualquer alteração ou inclusão de dependência deve ser acompanhada de revisão registrada na documentação de segurança do projeto, indicando a data da avaliação, versão homologada e finalidade técnica.

## Alternativas avaliadas

- Utilização de referências dinâmicas com atualização automática para a versão mais recente em tempo de compilação: introduz riscos de quebra inesperada de compilação ou inserção de código comprometido (ataques de supply chain).
- Instalação livre de utilitários de terceiros para tarefas rotineiras (ex: manipulação de datas, serialização customizada ou helpers de string): introduz complexidade e redundância desnecessárias, uma vez que o runtime do .NET moderno já oferece APIs otimizadas para essas operações.

## Consequências

- Compilações reprodutíveis e determinísticas em qualquer máquina ou pipeline de integração contínua.
- Minimização drástica de vulnerabilidades de segurança decorrentes de dependências externas.
- Manutenção de uma base de código enxuta, confiável e alinhada às melhores práticas de engenharia de software.
