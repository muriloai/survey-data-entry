# 0011: Persistência do questionário e concorrência otimista

**Estado:** aceita.

## Contexto

O questionário pertence ao agregado Pesquisa. Sua estrutura deve ser carregada e validada como uma unidade, preservando os identificadores das perguntas. Edições simultâneas não podem sobrescrever o congelamento ou encerramento da pesquisa.

## Decisão

- Persistir a definição das perguntas e opções em uma coluna JSON da pesquisa no SQL Server. Contas, clientes e pesquisas continuam relacionais.
- Usar `rowversion` para detectar alterações concorrentes. Em conflito, recusar a gravação e pedir que a interface recarregue o estado atual.
- Reconstituir o agregado do domínio com os identificadores persistidos antes de executar alterações.
- Aplicar validações no servidor e manter a verificação de proprietário e estado da conta a cada operação.
- Inicializar pesquisas existentes com uma lista JSON vazia por migração e verificar esse caminho em banco real.

## Alternativas

Tabelas separadas para perguntas e opções facilitariam consultas analíticas diretamente sobre a definição. Neste estágio, o acesso ocorre pelo agregado completo; uma coluna de definição permite atualizar estrutura e bloqueio na mesma gravação. A persistência das fichas respondidas permanece independente, conforme ADR 0002.

## Consequências

A definição não deve ser consultada por varreduras de JSON para relatórios de grande volume. Se esse requisito surgir, a modelagem deverá ser revisada. A concorrência exige que a interface apresente o conflito e recarregue a pesquisa antes de uma nova tentativa.
