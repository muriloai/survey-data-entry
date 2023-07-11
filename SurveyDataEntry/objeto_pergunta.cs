using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace survey_data_entry_tabulador_pesquisas
{
    class objeto_pergunta
    {
        //um objeto pergunta de leitura tem: id da pergunta, nomenclatura, se permite nulo ou não como resposta e a posicao da pergunta no formulario!
        public int id_pergunta;
        public int id_formulario;
        public String nomenclatura;
        public char permite_nulo;
        public int posicao;
        public int maxResp;
    }
}
