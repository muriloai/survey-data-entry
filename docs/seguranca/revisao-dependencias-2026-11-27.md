# Revisão de dependências: 27/11/2026

A base web utiliza os pacotes Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore, Microsoft.AspNetCore.Identity.EntityFrameworkCore, Microsoft.EntityFrameworkCore.SqlServer, Microsoft.EntityFrameworkCore.Design e Microsoft.EntityFrameworkCore.Tools na versão 10.0.15, para integração de identidade, SQL Server e migrações. Os pacotes são publicados pela Microsoft e distribuídos sob licença MIT.

A consulta `dotnet list package --vulnerable --include-transitive --no-restore` ao NuGet não retornou vulnerabilidades conhecidas para o projeto web na data desta revisão. Essa consulta reflete os avisos disponíveis naquele momento e deve ser repetida nas próximas alterações de dependências.

Testes: Microsoft.NET.Test.Sdk 18.0.1 (Microsoft, MIT), xunit 2.9.3 e xunit.runner.visualstudio 3.1.5 (projeto xUnit.net, Apache-2.0). A restauração foi executada com a auditoria do NuGet habilitada e não apresentou alertas de vulnerabilidade.
