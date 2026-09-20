# 0007: Modalidades de pesquisa e conferência assistida por OCR

**Estado:** aceita.

## Contexto

Pesquisas de opinião e mercado operam tanto por questionários físicos impressos para preenchimento em prancheta quanto por questionários exclusivamente online. A aplicação original de 2014 provou o valor de uma digitação manual extremamente rápida por teclado. A nova versão precisa preservar essa produtividade de digitação rápida e, em etapas futuras, introduzir suporte assistivo a OCR sem abrir mão da confiabilidade dos dados apurados.

## Decisão

- A modalidade da pesquisa (compatível com ficha física em papel ou exclusivamente online) é fixada no momento da sua criação e não pode ser alterada posteriormente.
- Pesquisas com modalidade em papel geram fichas com numeração de controle e estrutura fixa para impressão, garantindo correspondência exata entre o formulário físico e a tela de digitação.
- Preservar o modo clássico de digitação por teclado herdado de 2014:
  - Navegação contínua entre perguntas via teclado (Enter / Tab / setas).
  - Suporte a retorno rápido (Backspace ou atalho de teclado) para corrigir respostas anteriores sem perder o preenchimento da ficha em andamento.
- Reconhecimento óptico de caracteres (OCR) implementado como mecanismo assistivo de pré-preenchimento executado no lado do cliente:
  - O processamento visual apenas sugere marcações e valores nos campos da ficha.
  - É obrigatória a conferência e confirmação por um operador humano antes de qualquer envio da ficha ao servidor. Não há lançamento automatizado sem validação humana.
- O servidor valida rigorosamente a coerência dos tipos de resposta para cada pergunta (valores numéricos, opções de múltipla escolha válidas, limites de texto), rejeitando qualquer payload que viole o gabarito da pesquisa.

## Alternativas avaliadas

- Permitir alteração de modalidade da pesquisa após a criação: geraria inconsistências graves caso perguntas fossem modificadas enquanto operadores estivessem em campo com fichas impressas.
- Gravação direta de dados lidos por OCR sem conferência humana: aumentaria drasticamente a taxa de dados corrompidos devido a falhas de iluminação, rasuras ou marcas tênues em formulários físicos.

## Consequências

- A imutabilidade da modalidade assegura a integridade de todo o ciclo de coleta e auditoria.
- A combinação de digitação ágil por teclado e assistência por OCR com revisão humana maximiza a produtividade dos operadores com precisão nos dados.
