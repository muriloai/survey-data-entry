using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace survey_data_entry_tabulador_pesquisas
{
    class objeto_pergunta_respondida
    {
        //items que compoe uma pergunta respondida
        //lista de respostas lançadas, varia de zer, uma a any items!
        private List<objeto_resposta_respondida> respostas = new List<objeto_resposta_respondida>();
        //id do banco da pergunta que o objeto representa!
        private int id_pergunta=0;
        private int maxPerm=0;
        //items-fim

        //****************estilos perante as variações de resposta existentes****************
        //resposta unica simples!
        public void setID_pergunta(int id_perg)
        {
            this.id_pergunta = id_perg;
        }
        public int getId_pergunta()
        {
            return this.id_pergunta;
        }
        public void item_respondido(int id_resposta)
        {
            objeto_resposta_respondida novo = new objeto_resposta_respondida();
            novo.id_resposta = id_resposta;
            novo.tipo = 1;
            if (respostas.Count < this.maxPerm)
            {
                respostas.Add(novo);
            }
        }
        //resposta unica com texto editavel!
        public void item_respondido(int id_resposta, String texto)
        {
            objeto_resposta_respondida novo = new objeto_resposta_respondida();
            novo.id_resposta = id_resposta;
            novo.texto = texto;
            novo.tipo = 2;
            if (respostas.Count < this.maxPerm)
            {
                respostas.Add(novo);
            }
        }
        //resposta com valor quantitativo associado!
        public void item_respondido(int id_resposta, int indiceQttvo)
        {
            objeto_resposta_respondida novo = new objeto_resposta_respondida();
            novo.id_resposta = id_resposta;
            novo.tipo = 3;
            novo.valorQuantitativo = indiceQttvo.ToString();
            if (respostas.Count < this.maxPerm)
            {
                respostas.Add(novo);
            }
        }
        public List<objeto_resposta_respondida> getTodasResposta()
        {
            return respostas;
        }
        public void setMaxPerm(int maxPerm)
        {
            this.maxPerm = maxPerm;
        }
        public int getMaxPerm()
        {
            return this.maxPerm;
        }
        public int getQtdRespondida()
        {
            return respostas.Count;
        }
        public void remover_todasRespostas()
        {
            int cont = 0;
            for (cont = 0; cont < respostas.Count; cont++)
            {               
                    respostas.Remove(respostas[cont]);               
            }  
        }
    }
}