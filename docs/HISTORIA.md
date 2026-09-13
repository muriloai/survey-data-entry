# História do Projeto — Survey Data Entry

Este documento registra a origem real do **Survey Data Entry**, criado em **2014** como uma aplicação desktop (C# / Windows Forms / .NET Framework) para digitação rápida, controle e tabulação de pesquisas de campo.

## O contexto em campo

O projeto nasceu de uma necessidade prática após cerca de **um mês e meio** de coleta presencial com **formulários físicos aplicados em inúmeros pontos na estrada**. Ao final dessa etapa, era preciso computar e consolidar todo o volume de questionários respondidos — cada um com **36 questões**.

- **Primeira tentativa (Excel):** o lançamento manual em planilhas passava de **1 hora por formulário**, tornando o trabalho inviável para o volume coletado.
- **Segunda tentativa (solução web):** foi criada uma aplicação web para centralizar os dados, mas entre cadastrar o local de coleta do formulário e preencher todas as questões, o processo ainda levava **quase 60 minutos por formulário**.
- **A solução desktop (2014):** para resolver o gargalo, desenvolvi a aplicação Windows Forms **dirigida a atalhos de teclado**, eliminando a dependência constante do mouse e focando em velocidade e confiabilidade na digitação.

## Como o sistema resolveu o problema

- **Operação em rede multiusuário:** o aplicativo rodava em todos os computadores da equipe apontando para o mesmo servidor **MySQL** na rede local, permitindo que vários operadores computassem formulários ao mesmo tempo.
- **Velocidade de entrada de dados:** durante a digitação (`frm_responder_pesquisa`), o operador utiliza atalhos diretos no teclado (teclas numéricas para selecionar alternativas e `Enter` para avançar), reduzindo drasticamente o tempo de lançamento de cada questionário.
- **Confiança e correção imediata:** com suporte a retorno rápido (`Backspace` / botão Voltar), era possível voltar às perguntas anteriores para revisar e corrigir dados lançados sem precisar reiniciar o formulário.

## Onde está o sistema original de 2014

Todo o código-fonte legado e os scripts de banco de dados da versão original de 2014 estão preservados em [`docs/sistema-versao-2014/`](sistema-versao-2014/):

- [`docs/sistema-versao-2014/SurveyDataEntry.sln`](sistema-versao-2014/SurveyDataEntry.sln) e [`docs/sistema-versao-2014/SurveyDataEntry/`](sistema-versao-2014/SurveyDataEntry/) — Código-fonte da aplicação Windows Forms em C#.
- [`docs/sistema-versao-2014/db_mysql_script.sql`](sistema-versao-2014/db_mysql_script.sql) — Script de criação do banco de dados MySQL (`db_survey_data_entry`).
- [`docs/sistema-versao-2014/db_mysql_user.sql`](sistema-versao-2014/db_mysql_user.sql) — Script de criação do usuário e permissões de acesso no MySQL.

---

[Voltar para o README principal](../README.md)
