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
    public partial class frm_cadastrar_formulario : Form
    {
        camada_dados dados = new camada_dados();
        int id_cliente = 0;
        public frm_cadastrar_formulario(int id_cliente)
        {
            this.id_cliente = id_cliente;
            InitializeComponent();
        }
        private void btn_cad_formulario_Click(object sender, EventArgs e)
        {
            if (this.txt_formulario.Text != String.Empty) //se tiver algo como nome!
            {
                if (MessageBox.Show(null, "Deseja realmente Registrar esse Formulário?\n'"+txt_formulario.Text+"'", "CONFIRMA?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (ConexaoBD.abrirConexao()) //conectado!
                    {
                        String dataEntrega = (dtpck_entrega.Value.Year.ToString() + "-" + dtpck_entrega.Value.Month.ToString() + "-" + dtpck_entrega.Value.Day.ToString());
                        if (dados.inserir_umFormulario(txt_formulario.Text, id_cliente, dataEntrega))//e se deu certo a inserção!
                        {
                            MessageBox.Show(null, "Formulário Registrado com Sucesso!", "CONFIRMADO!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            if (MessageBox.Show(null, "Deseja cadastrar mais um Formulário?", "Atenção", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {                                
                                txt_formulario.Text = "";
                                txt_formulario.Focus();
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
                MessageBox.Show(null, "Escreva o Nome/Título do Formulário!", "REVISE!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void frm_cadastrar_posto_coleta_Load(object sender, EventArgs e)
        {
            if(this.id_cliente > 0)
            {
                this.lbl_cliente_nome.Text = dados.selecionarUmCliente_porIDCliente(id_cliente).nome;
            }
            else
            {
                this.Close();
            }
        }

        private void frm_cadastrar_formulario_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Enter) && (this.txt_formulario.Text != String.Empty))
            {
                btn_cad_formulario_Click(this, null);
            }
        }
    }
}