using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Data;
using System.Windows.Forms;

namespace survey_data_entry_tabulador_pesquisas
{
    class ConexaoBD
    {
        static String conexaoString;
        static MySqlConnection conexao;

        public ConexaoBD()
        {
            setar();
        }
        private static void setar()
        {
            conexaoString = "Server=" + Properties.Settings.Default.serverLocal.ToString() + ";"
            + "Database=" + Properties.Settings.Default.bdNome + ";"
            + "Uid=" + Properties.Settings.Default.usuario1 +
            ";Pwd=" + Properties.Settings.Default.senha1 + ";";
            conexao = new MySqlConnection(conexaoString);
        }
        public static Boolean abrirConexao()
        {
            try
            {
                setar();
                if (conexao.State == ConnectionState.Open)
                {
                    //aberta Conexão!
                    return true;
                }
                else
                {
                    conexao.Open();
                    if (conexao.State == ConnectionState.Open)
                    {
                        //aberta Conexao!
                        return true;
                    }
                    //Não conseguiu abrir!
                    return false;
                }
            }
            catch (Exception erro)
            {
                MessageBox.Show(erro.Message);
                return false;
            }
        }
        public static void fecharConexao()
        {
            if (conexao.State == ConnectionState.Open)
            {
                //fecha a Conexão!
                conexao.Close();
            }
            else
            {
                //faça nada!
            }
        }
        public static MySqlConnection getConexao()
        {
            return conexao;
        }

    }
}
