# 0008: Migrações versionadas com Entity Framework Core e SQL Server

**Estado:** aceita.

## Contexto

O banco de dados relacional é o núcleo de armazenamento das contas, pesquisas e fichas digitadas. O esquema do SQL Server precisa evoluir de forma controlada, idempotente e rastreável ao longo de múltiplos ambientes (desenvolvimento local, homologação e produção), sem interrupções indesejadas de serviço ou perda acidental de dados.

## Decisão

- O esquema do banco de dados no SQL Server é gerenciado oficialmente através de migrações versionadas do Entity Framework Core (`dotnet ef migrations`), iniciando a partir de um banco de dados vazio.
- Geração de scripts SQL idempotentes completos a partir das migrações (`dotnet ef migrations script --idempotent`) para revisão prévia e arquivamento no repositório.
- Adoção da estratégia de **expandir e contrair** para mudanças de esquema incompatíveis com versões anteriores:
  1. Fase de expansão: adicionar novas colunas ou tabelas mantendo compatibilidade com a versão atual em execução.
  2. Fase de transição: preenchimento gradual e migração de leituras e escritas para as novas estruturas.
  3. Fase de contração: remoção de colunas ou tabelas antigas somente após o encerramento da janela de rollback e confirmação de estabilidade.
- Separação mandatória de credenciais de acesso ao banco de dados:
  - Credencial de migração (DDL): utilizada exclusivamente por rotinas ou tarefas controladas de implantação, com permissões para criar, alterar e descartar tabelas e índices.
  - Credencial de execução da aplicação (DML): utilizada pelo processo web do ASP.NET Core em tempo de execução, restrita às operações de manipulação de dados (SELECT, INSERT, UPDATE, DELETE).
- Exigência de cópia de segurança restaurável e validada antes da aplicação de qualquer migração que execute alterações estruturais em ambiente de produção.

## Alternativas avaliadas

- Sincronização automática de esquema em tempo de inicialização da aplicação (`Database.EnsureCreated()` ou migração automática no startup): arriscado em ambientes com múltiplas instâncias concorrentes, podendo causar deadlocks de DDL ou alterações inadvertidas em produção.
- Manutenção de scripts manuais em arquivos SQL sem rastreador centralizado: propenso a desvios entre ambientes e erros humanos na ordem de execução dos scripts.

## Consequências

- O histórico de alterações no esquema permanece 100% versionado em conjunto com o código-fonte C#.
- A separação de privilégios de DDL e DML protege o banco de dados contra execução indevida de comandos estruturais pela aplicação em execução.
- O padrão expandir e contrair viabiliza deploys contínuos e rollbacks seguros sem parada de sistema.
