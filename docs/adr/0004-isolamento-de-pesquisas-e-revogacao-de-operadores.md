# 0004: Isolamento de pesquisas, autorização e revogação imediata de operadores

**Estado:** aceita.

## Contexto

A plataforma atende gestores que criam pesquisas e operadores responsáveis pela digitação de respostas. É indispensável que operadores acessem estritamente os projetos para os quais foram designados. Além disso, quando um operador for bloqueado, desvinculado de um projeto ou quando a pesquisa for encerrada, o corte de acesso precisa ter efeito imediato no servidor, mesmo em sessões com cookies ainda ativos.

## Decisão

- A permissão de acesso às pesquisas é validada a cada requisição no servidor, verificando ativamente a associação entre o operador e a pesquisa informada.
- Em qualquer ação protegida (seja via componentes Blazor ou chamadas à API HTTP `/api/v1`), o sistema checa o status atual da conta do operador e o estado da pesquisa no banco de dados.
- Revogação imediata: caso a conta do operador seja bloqueada, sua atribuição seja removida ou a pesquisa seja encerrada, qualquer operação subsequente é prontamente rejeitada pelo servidor. Não se admite esperar a expiração natural da sessão para efetivar a remoção de privilégios.
- Respostas da API HTTP em casos de erro ou recusa de acesso seguem a especificação RFC 7807 (`ProblemDetails`):
  - Retornam código de status HTTP correspondente (ex: 401 Unauthorized, 403 Forbidden, 404 Not Found, 422 Unprocessable Entity).
  - Incluem código estável da regra de negócio (ex: `OPERATOR_BLOCKED`, `SURVEY_ACCESS_REVOKED`).
  - Fornecem mensagem de erro amigável obtida dos arquivos de recursos localizados do projeto.
  - Disponibilizam a lista detalhada de erros por campo em validações de formulário.
- Quando um gestor cadastrar um operador com seu e-mail, o gestor pode clicar para emitir um e-mail para ele acessar contendo dados do login.
- Quando um gestor liberar operadores em um projeto ele deve cadastrar uma senha padrão daquele projeto de forma global para os operadores daquele projeto e como o login é senha com e-mail do operador e o mesmo operador pode participar de 2 projetos de diferentes gestores com seu mesmo e-mail e senha idênticos então o sistema deve certificar-se de que os operadores cadastrados podem ou não usar a senha global, e caso negativo deve obrigar a customizar uma senha para aquele operador, e esta customização fica opcional para qualquer operador, mesmo aqueles que estão aptos usando a senha global do projeto.

## Alternativas avaliadas

- Validação baseada exclusivamente em claims embutidas no cookie de autenticação: exigiria reemitir o cookie para cada alteração de permissão e manteria operadores desautorizados com acesso ativo até o encerramento do cookie.
- Bloqueio somente na interface do usuário (ocultar botões ou rotas): ineficaz contra chamadas diretas aos endpoints da API ou manipulação de requisições no navegador.

## Consequências

- A verificação de status e permissões a cada comando garante segurança estrita e cumprimento imediato de decisões de gestão.
- A consulta frequente de atribuições é otimizada com índices específicos no SQL Server sobre as chaves de operador e pesquisa.
- A padronização via `ProblemDetails` estabelece contratos previsíveis para clientes web e móveis.
