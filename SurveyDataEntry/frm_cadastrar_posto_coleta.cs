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
    public partial class frm_cadastrar_posto_coleta : Form
    {
        camada_dados dados = new camada_dados();
        public frm_cadastrar_posto_coleta()
        {
            InitializeComponent();
        }

        private void btn_cad_posto_Click(object sender, EventArgs e)
        {
            if (this.txt_posto_nome.Text != String.Empty) //se tiver algo como nome!
            {
                if (MessageBox.Show(null, "Deseja realmente Registrar esse Posto?\n'"+txt_posto_nome.Text+"'", "CONFIRMA?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (ConexaoBD.abrirConexao()) //conecatado!
                    {
                        if (dados.inserir_umPostoColeta(txt_posto_nome.Text))//e se deu certo a inserção!
                        {
                            MessageBox.Show(null, "Posto Registrado com Sucesso!", "CONFIRMADO!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            if (MessageBox.Show(null, "Deseja cadastrar mais um Posto?", "Atenção", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {                                
                                txt_posto_nome.Text = "";
                                txt_posto_nome.Focus();
                            }
                            else
                            {
                                this.Close();
                            }
                        }
                        else 
                        {
                            MessageBox.Show(null, "Falhou em Gravá-lo, tente novamente!", "REVISE!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
                else
                { 
                    //continua o form lá a espera...
                }               
            }
            else 
            {
                MessageBox.Show(null, "Escreva o Nome do Posto!", "REVISE!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
