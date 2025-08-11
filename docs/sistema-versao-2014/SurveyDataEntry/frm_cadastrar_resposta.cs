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
    public partial class frm_cadastrar_resposta : Form
    {
        int ID_pergunta = new int();
        objeto_pergunta pergunta;
        camada_dados dados = new camada_dados();
        public frm_cadastrar_resposta(int ID_pergunta)
        {
            this.ID_pergunta = ID_pergunta;
            InitializeComponent();
        }

        private void frm_cadastrar_resposta_Load(object sender, EventArgs e)
        {
            this.pergunta = new objeto_pergunta();
            //captar a pergunta referente as respostas que serão gravadas!
            if (ConexaoBD.abrirConexao())
            {
                this.pergunta = dados.selecionarPergunta_porID(ID_pergunta);
                if (this.pergunta.nomenclatura.Length > 100)
                {
                    lbl_pergPreview.Text = pergunta.nomenclatura.Substring(0, 100) + "...";
                }
                else 
                {
                    lbl_pergPreview.Text = pergunta.nomenclatura;
                }
            }
            rbtn_Un_C_Texto.Checked = true;
            rbtn_Un_S_Texto.Checked = true;
            captar_ultimoValorPosicaoPerguntaCadastrada();
            carregarOpcoesRespostas();
        }
        private void captar_ultimoValorPosicaoPerguntaCadastrada()
        {
            //recolhe do BD a maior posicao cadastrada em uma resposta da pergunta que você selecionou!
            if (ConexaoBD.abrirConexao())
            {
                objeto_resposta resposta = new objeto_resposta();
                resposta = dados.selecionarPosicaoUltimaRespostaAtiva_deUmaPerguntaID(dados.selecionarPergunta_porID(ID_pergunta).id_pergunta);
                txt_posicao.Text = (resposta.posicao + 1).ToString();
            }
        }
        private void escolherOpcao(TabPage tabEscolhida)
        {
            //invisiveis tabs
            tbctr_resposta.Appearance = TabAppearance.FlatButtons;
            tbctr_resposta.ItemSize = new Size(0, 1);
            tbctr_resposta.SizeMode = TabSizeMode.Fixed;
            
            //exibir as tabs
            foreach (TabPage i in tbctr_resposta.TabPages)
            {
                if (i != tabEscolhida)
                {
                    i.Hide();
                    tbctr_resposta.TabPages[i.Name].Hide();
                }else
                {
                    i.Show();
                    tbctr_resposta.SelectedTab = i;
                }
            }
        }
        //mudanças no checked
        private void rbtn_Un_S_Texto_CheckedChanged(object sender, EventArgs e)
        {
            escolherOpcao(tbpg_Un_S_Texto);
            txt_resp_un_s_texto.Focus();
        }      

        private void rbtn_Un_C_Texto_CheckedChanged(object sender, EventArgs e)
        {
            escolherOpcao(tbpg_Un_C_Texto);
            txt_resp_un_c_texto.Focus();
        }
        
        private void rbtn_Mu_Q_CheckedChanged(object sender, EventArgs e)
        {
            escolherOpcao(tbpg_Mu_Q);
            txt_resp_Mu_q.Focus();
        }
        private void limparCampos()
        {
            txt_resp_un_c_texto.Text = "";
            txt_resp_Mu_q.Text = "";
            txt_resp_un_s_texto.Text = "";
        }
        private int idDependenciaNoBD()
        {
            /*
             * IDS de dependencia no BD:
             * Un_S_Texto = 1
             * Un_C_Texto = 2
             * Mu_Q = 3 
             */
            if (rbtn_Un_S_Texto.Checked)
            {
                return 1;
            }
            if (rbtn_Un_C_Texto.Checked)
            {
                return 2;
            }
            if (rbtn_Mu_Q.Checked)
            {
                return 3;
            }
            return 0;

        }
        private void btn_cadResp_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(null, "Deseja Realmente Registar esta Resposta?\n", "ATENÇÃO!", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (ConexaoBD.abrirConexao())
                {
                    String texto = "";
                    int posicao = Convert.ToInt32(txt_posicao.Text.ToString());
                    if (idDependenciaNoBD() == 1)
                    {
                        texto = txt_resp_un_s_texto.Text;
                    }
                    else if(idDependenciaNoBD() == 2)
                    {
                        texto = txt_resp_un_c_texto.Text;
                    }
                    else if (idDependenciaNoBD() == 3)
                    {
                        texto = txt_resp_Mu_q.Text;
                    }
                    if (dados.inserir_umaRespostaNaPergunta(ID_pergunta, true, texto, posicao, idDependenciaNoBD()))
                        {
                            MessageBox.Show(null, "Resposta Cadastrada com Sucesso!", "Confirmado!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            if (MessageBox.Show(null, "Deseja cadastrar mais uma Resposta?", "Atenção", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                limparCampos();
                                captar_ultimoValorPosicaoPerguntaCadastrada();
                                carregarOpcoesRespostas();
                            }
                            else
                            {
                                this.Close();
                            }
                        }
                }
            }
        }

        private void frm_cadastrar_resposta_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (idDependenciaNoBD() == 1)
                {
                    if (txt_resp_un_s_texto.Text != String.Empty)
                    {
                        btn_cadResp_Click(this, null);
                    }
                }
                else if (idDependenciaNoBD() == 2)
                {
                    if (txt_resp_un_c_texto.Text != String.Empty)
                    {
                        btn_cadResp_Click(this, null);
                    }
                }
                else if (idDependenciaNoBD() == 3)
                {
                    if (txt_resp_Mu_q.Text != String.Empty)
                    {
                        btn_cadResp_Click(this, null);
                    }
                }
            }
        }
        //mudanças no checked - fim
        //----------------------------
        private void carregarOpcoesRespostas()
        {
            //libera no painel de respostas nas opções que preciso!
            pnl_previewResposta.AutoScroll = true;
            pnl_previewResposta.AutoSize = false;
            pnl_previewResposta.VerticalScroll.Enabled = true;
            pnl_previewResposta.HorizontalScroll.Enabled = false;
            pnl_previewResposta.Controls.Clear();
            //carregamento das respostas da pergunta!
            List<objeto_resposta> listaRespostas = dados.selecionarTodasRespostasAtivas_deUmaPerguntaID(ID_pergunta);

            //items para usar na criacao de componentes!
            int cont = 0;
            resposta novo;
            int qtdRespostas = listaRespostas.Count;

            //cria-se os objetos das respostas e altera seus valores internos!
            for (cont = 0; cont < qtdRespostas; cont++)
            {
                //gero ocomponente!
                novo = new resposta();
                //preencho a largura, batendo com o tamanho do painel!
                novo.alterarComprimentoPainel((pnl_previewResposta.Width - 30));
                //altero a componente!
                novo.alterar_posicao_resposta(listaRespostas[cont].posicao);
                novo.alterar_idResposta(listaRespostas[cont].id_resposta);
                novo.alterar_resposta(listaRespostas[cont].nomenclatura);
                novo.alterar_tipoResposta(listaRespostas[cont].tipo);
                pnl_previewResposta.Controls.Add(novo);
            }
            //inicia em -1 porque dá se a contagem logo somando a 0!
            int qtd = -1;
            foreach (Control i in pnl_previewResposta.Controls)
            {
                //identifica quantos objetos tem!
                qtd++;
                //adapta a posicao conforme a quantidade de items!            
                i.SetBounds(i.Location.X, (i.Location.Y + i.Size.Height) * qtd, i.Width, i.Height, BoundsSpecified.All);
            }
        }   
        //------------------------------
    }
}
