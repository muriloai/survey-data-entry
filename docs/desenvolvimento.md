# Desenvolvimento

## Pré-requisitos

- SDK .NET 10.0.404 ou atualização compatível de patch.
- SQL Server acessível e credenciais adequadas para desenvolvimento.
- Ferramenta `dotnet-ef` compatível com EF Core 10.

## Banco de desenvolvimento

Configure `ConnectionStrings:DefaultConnection` por variáveis de ambiente ou user secrets. Para criar o esquema em um banco vazio de desenvolvimento, execute:

```powershell
dotnet ef database update --project src/SurveyDataEntry.Web
```

A aplicação não aplica migrações ao iniciar. O script idempotente gerado está em `src/SurveyDataEntry.Infrastructure/Identity/Migrations/identity.sql`. Em produção, use uma tarefa de migração separada e credenciais distintas para DDL e operação.

## Execução

```powershell
dotnet run --project src/SurveyDataEntry.Web
```

Em `Development`, as mensagens de confirmação e recuperação são gravadas em `src/SurveyDataEntry.Web/App_Data/mail`, que não deve ser compartilhado. Fora de desenvolvimento ainda é necessário implementar e configurar o envio de e-mail. Não use esta versão com dados reais.

## Verificação inicial

```powershell
dotnet build SurveyDataEntry.slnx
dotnet test tests/SurveyDataEntry.Domain.Tests
dotnet test tests/SurveyDataEntry.Integration.Tests
```

Os testes unitários cobrem regras iniciais do domínio. O teste de integração requer SQL Server LocalDB no Windows e cria um banco exclusivo, removido ao finalizar. Ele verifica migração, confirmação de conta, troca de senha e rejeição de token reutilizado. A cobertura dos fluxos web ainda está em desenvolvimento.

