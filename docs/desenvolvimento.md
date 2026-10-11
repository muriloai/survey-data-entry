# Desenvolvimento

## Pré-requisitos

- SDK .NET 10.0.404 ou atualização compatível de patch.
- SQL Server acessível e credenciais adequadas para desenvolvimento.
- Ferramenta `dotnet-ef` compatível com EF Core 10.

## Banco de desenvolvimento

Configure `ConnectionStrings:DefaultConnection` por variáveis de ambiente ou user secrets.

E para criar o esquema em um banco vazio de desenvolvimento, execute:

```powershell
dotnet ef database update --project src/SurveyDataEntry.Infrastructure --startup-project src/SurveyDataEntry.Web
```

A aplicação não aplica migrações ao iniciar.

O script completo do banco gerado está em `src/SurveyDataEntry.Infrastructure/Identity/Migrations/identity.sql`.

Em produção, use uma tarefa de migração separada e credenciais distintas para DDL e operação.

## Execução

```powershell
dotnet run --project src/SurveyDataEntry.Web
```

Em `Development`, as mensagens de confirmação e recuperação são gravadas em `src/SurveyDataEntry.Web/App_Data/mail`, que não deve ser compartilhado.

E fora de desenvolvimento ainda é necessário implementar e configurar o envio de e-mail, contudo, não use esta versão com dados reais.

## Testes

```powershell
dotnet build SurveyDataEntry.slnx
dotnet test tests/SurveyDataEntry.Domain.Tests
dotnet test tests/SurveyDataEntry.Integration.Tests
```

Os **testes unitários** cobrem regras iniciais do domínio.

O **teste de integração** requer SQL Server LocalDB no Windows o qual será usado para criar um banco exclusivo e removê-lo ao finalizar, portanto, este teste busca verificar migração, confirmação de conta, troca de senha e rejeição de token reutilizado.

A cobertura dos fluxos web ainda está em desenvolvimento.

## Catálogo

Após confirmar a conta e entrar, acesse `/surveys` para cadastrar clientes e pesquisas. O catálogo confere o estado da conta a cada operação e restringe os registros ao proprietário. Não é possível criar uma pesquisa vinculada ao cliente de outra conta.

## Questionários

Abra uma pesquisa pelo catálogo para adicionar perguntas de texto, número, escolha única ou múltipla. Perguntas de escolha exigem opções válidas; as demais não aceitam opções. A estrutura pode ser congelada após revisão explícita, impedindo inclusões e remoções futuras.

A integração também verifica atualização de pesquisas existentes, preservação de identificadores, persistência das opções e conflito entre duas conexões SQL.

## Digitação de fichas

Abra o questionário e selecione `Record responses`. Preencha as respostas e confirme a revisão antes de salvar. A tecla Tab permite navegar pelos campos; em múltipla escolha, informe uma opção por linha. Nesta versão, números usam ponto decimal sem separadores de milhar.

A gravação valida todas as respostas antes de persistir a ficha. O primeiro envio também bloqueia a estrutura do questionário na mesma transação. Cada envio possui um identificador: repetir o mesmo envio com as mesmas respostas retorna a ficha já gravada; reutilizá-lo com respostas diferentes é recusado.

A coleta está disponível ao proprietário e aos operadores com atribuição ativa. Os testes verificam isolamento, rejeição de respostas inválidas, revisão obrigatória, reenvio, bloqueio de conta e encerramento da pesquisa.

## Operadores

No questionário, abra `Manage operators` e atribua o e-mail de uma conta já cadastrada e confirmada. O operador acessa `/operator` para consultar pesquisas ativas atribuídas e registrar fichas. Ele não pode editar a estrutura do questionário nem consultar o catálogo do gestor. Seu contador de fichas considera somente seus próprios lançamentos.

Bloquear ou remover uma atribuição corta o acesso na próxima operação, inclusive em formulários já abertos. O bloqueio vale apenas naquela pesquisa; atribuições do mesmo operador em projetos de outros gestores continuam independentes. Encerrar a pesquisa também impede novas operações do operador.

O cadastro de operadores por convite e a política de senhas por projeto descrita na ADR 0004 ainda estão pendentes. Esta entrega não permite que um gestor altere a senha global da conta de outro usuário.
