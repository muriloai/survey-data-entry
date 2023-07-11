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
    public partial class frm_cadastrar_cliente : Form
    {
        camada_dados dados = new camada_dados();
        public frm_cadastrar_cliente()
        {          
            InitializeComponent();
        }
        private void btn_cad_cliente_Click(object sender, EventArgs e)
        {
            if (this.txt_cliente_nome.Text != String.Empty) //se tiver algo como nome!
            {
                if (MessageBox.Show(null, "Deseja realmente Registrar esse Cliente?\n'"+txt_cliente_nome.Text+"'", "CONFIRMA?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (ConexaoBD.abrirConexao()) //conectado!
                    {
                        if (dados.inserir_umCliente(txt_cliente_nome.Text))//e se deu certo a inserção!
                        {
                            MessageBox.Show(null, "Cliente Registrado com Sucesso!", "CONFIRMADO!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            if (MessageBox.Show(null, "Deseja cadastrar mais um Cliente?", "Atenção", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {                                
                                txt_cliente_nome.Text = "";
                                txt_cliente_nome.Focus();
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
                MessageBox.Show(null, "Escreva o Nome do Cliente!", "REVISE!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void frm_cadastrar_posto_coleta_Load(object sender, EventArgs e)
        {
        }

        private void frm_cadastrar_cliente_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Enter)&&(this.txt_cliente_nome.Text!=String.Empty))
            {
                btn_cad_cliente_Click(this, null);
            }
        }
    }
}