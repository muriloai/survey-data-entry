using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace survey_data_entry_tabulador_pesquisas
{
    class objeto_formulario
    {
        //um objeto formularios de leitura tem: id, id do cliente, data criado, data entrega, situacao, titulo, ativo!
        public int id_formulario;
        public int id_cliente;
        public String data_criacao;
        public String data_entrega;
        public char situacao;
        public String titulo;
        public char ativo;       
    }
}
