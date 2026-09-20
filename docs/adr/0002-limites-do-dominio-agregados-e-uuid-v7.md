# 0002: Limites do domínio, agregados e identificadores UUID v7

**Estado:** aceita.

## Contexto

A operação de digitação e coleta de questionários envolve trabalho concorrente de vários operadores, validação estrita de tipos de respostas e integridade relacional. Além disso, a aplicação precisa gerar identificadores universais sem depender do banco de dados coordenar de forma centralizada as sequências, mantendo alta eficiência de indexação.

## Decisão

- Estruturar as regras de negócio em torno de três agregados principais:
  - **Conta**: raiz agregada que gerencia dados de identificação, credenciais, histórico de segurança e preferências do usuário (gestor ou operador).
  - **Pesquisa**: raiz agregada responsável pelo cliente associado, modalidade de coleta, questionário, perguntas, opções permitidas e atribuições de operadores autorizados.
  - **Ficha respondida**: raiz agregada independente que representa uma ficha de coleta concluída ou em andamento. Contém o conjunto de respostas registradas, referência à pesquisa, operador autor da coleta e carimbos de auditoria.
- Cada ficha respondida é persistida como uma unidade transacional atômica e independente. Múltiplos operadores podem registrar fichas simultâneas na mesma pesquisa sem carregar a coleção global de fichas em memória.
- Invariante estrutural de pesquisa: após a emissão da primeira ficha impressa ou o registro da primeira resposta online, a estrutura do questionário (perguntas, códigos e tipos) torna-se imutável, impedindo inconsistências com dados já coletados.
- Adotar identificadores UUID v7 (RFC 9562) para as chaves primárias de todas as entidades do domínio. Os identificadores são gerados pela própria aplicação no momento da instanciação.

## Alternativas avaliadas

- Manter todas as fichas respondidas como uma coleção filha dentro do agregado Pesquisa: geraria concorrência excessiva de escrita, locks de linha prolongados e estouro de memória à medida que milhares de fichas fossem adicionadas ao projeto.
- Chaves primárias baseadas em inteiros autoincrementais (`IDENTITY`): expõem o volume real de dados por meio de números sequenciais previsíveis e exigem viagem de ida e volta ao banco para obtenção do ID gerado.
- Identificadores UUID v4 aleatórios: causam fragmentação severa de índices agrupados (clustered index) no SQL Server devido à dispersão não ordenada dos dados no disco.

## Consequências

- O agregado de Ficha respondida permite lançamentos independentes e com concorrência limpa, escalando para múltiplos operadores simultâneos.
- O UUID v7 combina geração descentralizada na aplicação com ordenação cronológica natural, mantendo a performance e reduzindo a fragmentação das páginas de índice no SQL Server.
- A coluna explícita de data e hora de criação (`CreatedAtUtc`) continua existindo para auditoria, mesmo que o UUID v7 carregue o timestamp em sua composição.
