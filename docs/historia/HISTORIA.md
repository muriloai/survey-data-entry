# História do Projeto | Survey Data Entry

**Survey Data Entry** foi criado em **2014** como uma aplicação desktop (C# / Windows Forms / .NET Framework) para digitação rápida, controle e tabulação de pesquisas de campo.

## O contexto em campo

O projeto nasceu de uma necessidade prática após cerca de **um mês e meio** de coleta presencial através de quase 2mil **formulários físicos aplicados em inúmeros pontos de estradas no Brasil**. Ao final desta etapa, era preciso computar e consolidar todo o volume de questionários respondidos e cada um contendo **36 questões**.

- **Primeira tentativa | Excel**
  O lançamento manual em planilhas passava de **1 hora por formulário**, tornando o trabalho inviável para o volume coletado.

- **Segunda tentativa | Solução Web**
  Foi criada uma aplicação web para centralizar os dados, mas entre cadastrar o local de coleta do formulário e preencher todas as questões, o processo ainda levava **quase 38 minutos por formulário**, o tempo caiu, mas ainda era muito perante a quantidade de formulários.

- **A solução final | Desktop**
  Para resolver de vez este gargalo, desenvolvi uma aplicação Windows Forms derrubando para 17.5 minutos por formulário, e após refinamentos e melhorias no fluxo, escolhi aplicar uma abordagem **dirigida a atalhos de teclado** eliminando a dependência constante do mouse e focando em velocidade e confiabilidade na digitação, derrubando assim para menos de 2 minutos por formulário.

## Como o sistema resolveu o problema?

- **Operação em rede multiusuário**
  O aplicativo rodava em 4 computadores fixos da equipe apontando para o mesmo servidor **MySQL** na rede local, permitindo até mesmo um auxílio extra e ocasional de membros da gerência participando nesta etapa de tabulação, computando assim inúmeros formulários ao mesmo tempo.
- **Velocidade de entrada de dados**
  Durante a digitação (`frm_responder_pesquisa`), o operador utiliza atalhos diretos no teclado, sendo teclas numéricas para selecionar alternativas e `Enter` para avançar, reduzindo drasticamente o tempo de lançamento de cada questionário.
- **Confiança e correção imediata**
  Com suporte a retorno rápido (`Backspace` / botão Voltar), era possível voltar às perguntas anteriores para revisar e corrigir dados lançados sem precisar reiniciar o formulário.
- **O prazo foi cumprido**
  Em meio às demandas da rotina, levei apenas um dia e meio para idealizar, desenvolver, testar e apresentar esta solução. Após validar os dados processados nos testes e levar essas informações para o Excel, comprovamos a confiabilidade do sistema, permitindo gerar amostragens fiéis, insights acionáveis e valor real para o negócio. Com 4 operadores fixos e auxílio de outros ocasionais, foi possível processar todos formuláris físicos coletados em apenas 4 dias. O prazo final era 15 dias.

## Houve algum tipo de econômia no processo?

A **Solução Web**, que foi a segunda tentativa, tinha sido cotada para ser levada para tablets e o custo do projeto naquela época seria:

- pelo menos 5 tablets;
- o custeio de disponibilizar a solução em um serviço online;
- e conexão remota através de chips 3g;
- criação de app para Android em prol de obter meios de mitigar problemas de conexão, tendo como sincrozinar os dados quando chegar na base.

A operação passou a rodar 100% offline e sem custos adicionais, garantindo estabilidade mesmo em locais remotos ou sob intempéries. Além disso, a solução foi pensada para ser intuitiva, dispensando qualquer investimento em treinamento de equipe e ampliando melhor a frente de captação.

## Onde está o sistema original de 2014?

Todo o código-fonte legado e os scripts de banco de dados da versão original de 2014 estão preservados em [`docs/historia/sistema-versao-2014/`](sistema-versao-2014/):

- [`SurveyDataEntry.sln`](sistema-versao-2014/SurveyDataEntry.sln) e [`SurveyDataEntry/`](sistema-versao-2014/SurveyDataEntry/) | Código-fonte da aplicação Windows Forms em C#.
- [`db_mysql_script.sql`](sistema-versao-2014/db_mysql_script.sql) | Script de criação do banco de dados MySQL (`db_survey_data_entry`).
- [`db_mysql_user.sql`](sistema-versao-2014/db_mysql_user.sql) | Script de criação do usuário e permissões de acesso no MySQL.

---

[Voltar para o README principal](../../README.md)
