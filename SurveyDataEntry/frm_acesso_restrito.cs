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
    public partial class frm_acesso_restrito : Form
    {
        frm_menu menu;
        public frm_acesso_restrito(frm_menu menu)
        {
            this.menu = menu;
            InitializeComponent();            
        }
       
        private void btn_entrar_Click(object sender, EventArgs e)
        {
            String login = this.txt_login.Text;
            String senha = this.txt_senha.Text;
            if ((login.ToString().ToUpper().Equals(
                Properties.Settings.Default.loginAdmin.ToString().ToUpper()))
                && (senha.ToString().ToUpper().Equals(Properties.Settings.Default.senhaAdmin.ToString().ToUpper())))
            {
                menu.liberar_areaRestrita(1);
                this.Close();
            }
            else
            {
                menu.liberar_areaRestrita(0);
                this.Close();
            }
        }

        private void frm_acesso_restrito_Load(object sender, EventArgs e)
        {

        }

        private void frm_acesso_restrito_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Enter) && (!txt_senha.Text.Equals(new TextBox().Text)) && (!txt_login.Text.Equals(new TextBox().Text)))
            {
                btn_entrar_Click(this, null);
            }
        }
    }
}
