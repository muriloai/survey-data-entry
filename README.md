# winforms-survey-data-entry

Sistema desktop em C# (Windows Forms / .NET Framework) criado originalmente em **2014** para digitação rápida, controle e tabulação de pesquisas de campo.

## A história real por trás do sistema

Este projeto nasceu de uma necessidade prática após cerca de **um mês e meio** de coleta presencial com **formulários físicos aplicados em inúmeros pontos na estrada**. Ao final dessa etapa, era preciso computar e consolidar todo o volume de questionários respondidos — cada um com **36 questões**.

- **Primeira tentativa (Excel):** o lançamento manual em planilhas passava de **1 hora por formulário**, tornando o trabalho inviável para o volume coletado.
- **Segunda tentativa (solução web):** foi criada uma aplicação web para centralizar os dados, mas entre cadastrar o local de coleta do formulário e preencher todas as questões, o processo ainda levava **quase 60 minutos por formulário**.
- **A solução desktop (2014):** para resolver o gargalo, desenvolvi esta aplicação Windows Forms **dirigida a atalhos de teclado**, eliminando a dependência constante do mouse e focando em velocidade e confiabilidade na digitação.

### Como o sistema resolveu o problema

- **Operação em rede multiusuário:** o aplicativo rodava em todos os computadores da equipe apontando para o mesmo servidor **MySQL** na rede local, permitindo que vários operadores computassem formulários ao mesmo tempo.
- **Velocidade de entrada de dados:** durante a aplicação/digitação (`frm_responder_pesquisa`), o operador utiliza atalhos diretos no teclado (teclas numéricas para selecionar alternativas e `Enter` para avançar), reduzindo drasticamente o tempo de lançamento de cada questionário.
- **Confiança e correção imediata:** com suporte a retorno rápido (`Backspace` / botão Voltar), era possível voltar às perguntas anteriores para revisar e corrigir dados lançados sem precisar reiniciar o formulário.

## Estrutura do projeto (versão original de 2014)

- `SurveyDataEntry.sln` e `SurveyDataEntry/` — Código-fonte da aplicação Windows Forms em C#.
- `db_mysql_script.sql` — Script de criação do banco de dados MySQL (`db_survey_data_entry`).
- `db_mysql_user.sql` — Script de criação do usuário e permissões de acesso no MySQL.
