using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace survey_data_entry_tabulador_pesquisas
{
    public partial class frm_cadastrar_perguntas : Form
    {
        //id do formulario!
        int ID_formulario = new int();
        //camada de dados!
        camada_dados dados = new camada_dados();

        public frm_cadastrar_perguntas(int ID_formulario)
        {
            this.ID_formulario = ID_formulario;
            InitializeComponent();
        }

        private void frm_cadastrar_perguntas_Load(object sender, EventArgs e)
        {
            captar_ultimoValorPosicao_cadastrado();
            txt_maxPermitido.Text = 1.ToString();
        }
        private void captar_ultimoValorPosicao_cadastrado()
        {

            if (ConexaoBD.abrirConexao())
            {
                objeto_pergunta pergunta = new objeto_pergunta();
                pergunta = dados.selecionarPerguntaMaiorPosicao_porIdForm(ID_formulario);
                txt_posicao.Text = (pergunta.posicao + 1).ToString();
            }
        }
        private Boolean checar_nulo()
        {
            if (chk_nulo.Checked == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private void btn_cad_pergunta_Click(object sender, EventArgs e)
        {
            if(txt_pergunta.Text!=String.Empty)
            {
            {
                int posicao = Convert.ToInt32(txt_posicao.Text.ToString());
                int max = Convert.ToInt32(txt_maxPermitido.Text.ToString());
                if (MessageBox.Show(null, "Deseja Realmente Registar esta pergunta?\n" + this.txt_pergunta.Text, "ATENÇÃO!", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (dados.inserir_umaPerguntaNoFormulario(ID_formulario, true, txt_pergunta.Text.ToString(), posicao, checar_nulo(), max))
                    {
                        MessageBox.Show(null, "Pregunta Cadastrada com Sucesso!", "Confirmado!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        if (MessageBox.Show(null, "Deseja cadastrar mais uma pergunta?", "Atenção", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            //dpois modificar para cadastrar direto as respostas..
                            txt_pergunta.Text = "";
                            captar_ultimoValorPosicao_cadastrado();
                            txt_maxPermitido.Text = "1";
                            txt_pergunta.Focus();
                        }
                        else
                        {
                            this.Close();
                        }
                    }
                }
            }
            }
            else
            {
               MessageBox.Show(null, "Escreva sua Pergunta e Tente Novamente!", "ATENÇÃO!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void frm_cadastrar_perguntas_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Enter) && (txt_pergunta.Focused == true) && (txt_pergunta.Text!=String.Empty))
            {
                btn_cad_pergunta_Click(this, null); 
            }
        }

    }
}
