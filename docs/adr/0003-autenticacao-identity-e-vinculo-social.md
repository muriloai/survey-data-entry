# 0003: Autenticação via ASP.NET Core Identity e vínculos sociais

**Estado:** aceita.

## Contexto

O acesso ao sistema exige controle seguro de credenciais, prevenção de abusos em formulários públicos, confirmação obrigatória de titularidade de e-mail e recuperação de senha confiável. Ao mesmo tempo, é desejável permitir facilidade de login via provedores sociais como Google e Microsoft, mas sem criar vulnerabilidades de sequestro de conta ou cadastro desordenado de usuários.

## Decisão

- Utilizar o ASP.NET Core Identity como mecanismo oficial de autenticação e gerenciamento de contas, com armazenamento relacional no SQL Server via EF Core.
- Exigir confirmação de e-mail por meio de token antes de autorizar o acesso completo às áreas administrativas do painel.
- Definir tokens de confirmação de e-mail e recuperação de senha com validade máxima de 24 horas e descarte após o primeiro uso (uso único).
- Implementar tabelas de histórico para endereços de e-mail e hashes de senhas anteriores, impedindo a reutilização imediata das últimas senhas utilizadas pela conta.
- Provedores sociais (Google e Microsoft) são suportados apenas como vínculo opcional a uma conta existente:
  - O usuário deve se registrar inicialmente com e-mail e senha, ou realizar login autenticado antes de vincular sua conta externa.
  - Não é permitida a criação direta e automática de contas a partir do retorno de OAuth sem o fluxo formal de cadastro e validação de requisitos.
- E-mails transacionais utilizam abstração desacoplada na camada de infraestrutura: envio via API do Resend em ambientes remotos ou produção, e exibição direta no console/logs em ambiente de desenvolvimento local (`MAIL_DELIVERY=console`).

## Alternativas avaliadas

- Serviços externos gerenciados de autenticação (Auth0, Firebase ou AWS Cognito): criam acoplamento a um terceiro, custos recorrentes por usuário ativo e menor controle sobre o histórico de auditoria exigido pelo produto.
- Implementação manual de autenticação por tokens JWT do zero: alto risco de falhas de segurança em rotinas de refresh, invalidação de sessões e proteção contra ataques de força bruta.
- Cadastro imediato via OAuth social sem confirmação prévia: suscetível a erros de colisão de e-mails entre provedores e quebra das regras de controle de operadores.

## Consequências

- O ASP.NET Core Identity fornece suporte maduro a cookies seguros, proteção contra ataques CSRF e controle nativo de lockout após tentativas inválidas.
- A restrição no vínculo social assegura que apenas o titular legítimo da conta possa associar identidades externas do Google ou Microsoft.
- O modo local de e-mail permite que desenvolvedores testem os fluxos de confirmação e recuperação sem necessidade de chaves de API externas.
