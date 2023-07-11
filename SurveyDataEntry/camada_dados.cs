using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Data;
using System.Windows.Forms;

namespace survey_data_entry_tabulador_pesquisas
{
    class camada_dados
    {        
        //Campos Formulario
        public const String FORMULARIO_TABELA = "formulario_pesquisa";
        public const String
        FORMULARIO_ID = "id_formulario",
        FORMULARIO_ID_CLIENTE = "id_cliente_req",
        FORMULARIO_TITULO = "titulo_formulario",
        FORMULARIO_DT_CRIACAO = "data_criacao",
        FORMULARIO_DT_ENTREGA = "data_entrega",
        FORMULARIO_SITUACAO = "estado_situacao",
        FORMULARIO_ATIVO = "estado_ativo";
        //Campos - fim
        //Campos Pergunta
        public const String PERGUNTA_TABELA = "pergunta_formulario";
        public const String
        PERGUNTA_ID = "id_pergunta",
        PERGUNTA_ID_FORMULARIO = "id_formulario",
        PERGUNTA_ATIVO = "estado_ativo",
        PERGUNTA_NOMECLATURA = "nomenclatura",
        PERGUNTA_POSICAO = "posicao_noFormulario",
        PERGUNTA_NULO = "permite_nulo",
        PERGUNTA_RESP_MAX = "respostas_maxPermitidas";
        //Campos - fim
        //Campos Questionario
        public const String COLETA_QUESTIONARIO_TABELA = "coleta_dados_questionario";
        public const String
       COLETA_QUESTIONARIO_ID = "id_questionario",
       COLETA_QUESTIONARIO_ID_POSTO_COLETA = "id_posto_coleta",
       COLETA_QUESTIONARIO_ID_FORMULARIO = "id_formulario_fk";
        //Campos - fim
        //Campos Resposta
        public const String RESPOSTA_TABELA = "resposta_pergunta";
        public const String
        RESPOSTA_ID = "id_resposta",
        RESPOSTA_ID_TIPO = "id_tipo_resposta_fk",
        RESPOSTA_ID_PERGUNTA = "id_pergunta",
        RESPOSTA_NOMECLATURA = "nomenclatura",
        RESPOSTA_POSICAO = "posicao_naPergunta",
        RESPOSTA_ATIVO = "estado_ativo";
        //Campos - fim
        //Campos Tipo_Resposta
        public const String TIPO_RESPOSTA_TABELA = "tipo_resposta";
        public const String
        TIPO_RESPOSTA_ID = "id_tipo_resposta",     
        TIPO_RESPOSTA_NOMECLATURA = "nomenclatura",
        TIPO_RESPOSTA_ATIVO = "estado_ativo";
        //Campos - fim        
        //Campos Cliente
        public const String CLIENTE_TABELA = "cliente_requerente";
        public const String
        CLIENTE_ID = "id_cliente_req",
        CLIENTE_NOME = "nome_cliente",
        CLIENTE_DATA_CRIACAO = "data_criacao";
        //Campos - fim
        //Campos coleta_dados_resposta
        public const String COLETA_DADOS_TABELA = "coleta_dados_resposta";
        public const String
        COLETA_DADOS_ID = "id_coleta_dados_resposta",
        COLETA_DADOS_ID_RESPOSTA = "id_resposta_fk",
        COLETA_DADOS_ID_QUESTIONARIO = "id_questionario_fk",
        COLETA_DADOS_ID_PERGUNTA = "id_pergunta_fk";
        //Campos - fim
        //Campos coleta_dados_resposta_texto
        public const String COLETA_DADOS_TEXTO_TABELA = "coleta_dados_resposta_texto";
        public const String
        COLETA_DADOS_TEXTO_ID = "id_coleta_dados_resposta_texto",
        COLETA_DADOS_TEXTO_COLETA_DADOS_ID_RESPOSTA = "id_coleta_dados_resposta_fk",
        COLETA_DADOS_TEXTO_TEXTO = "texto";
        //Campos - fim
        //Campos coleta_posto
        public const String COLETA_POSTO_TABELA = "coleta_posto";
        public const String
        COLETA_POSTO_ID = "id_posto",
        COLETA_POSTO_NOME = "nome",
        COLETA_POSTO_DATA_CRIACAO = "data_criacao";
        //Campos - fim
        //Campos cidade
        public const String CIDADE_TABELA = "cidade";
        public const String
        CIDADE_ID = "id_cidade",
        CIDADE_NOME = "nome",
        CIDADE_ID_ESTADO = "id_estado_fk";
        //Campos - fim
        //Campos estados
        public const String ESTADO_TABELA = "estado";
        public const String
        ESTADO_ID = "id_estado",
        ESTADO_NOME = "nome",
        ESTADO_UF = "uf",
        ESTADO_ID_PAIS = "id_pais_fk";
        //Campos - fim
       
        //**********************************************************************************************************

        //objetos banco
        MySqlCommand Query = new MySqlCommand();
        MySqlDataReader leitor;
        //objetos banco
        
        //auxiliares*************************************************************************************************************************
        private Char identificar_SimNao_BoolToChar(Boolean nulo)
        {
            switch(nulo)
            {
                case true:
                    return 's';
                    break;
                case false:
                    return 'n';
                    break;
                default:
                    return 's';
                    break;
            }
        }

        //selects***************************************************************************************************************************
        //*********************************PERGUNTAS****************
        public List<int> selecionarID_TodasPerguntasDoFormulario(int id_form)
        {
            Query.Connection = ConexaoBD.getConexao();
            Query.CommandText = "SELECT "+PERGUNTA_ID+" FROM " + PERGUNTA_TABELA +
                " WHERE " + PERGUNTA_ID_FORMULARIO + " = " + id_form + " AND " + PERGUNTA_ATIVO + " = 's' ORDER BY " + PERGUNTA_POSICAO + " ASC";
            Query.Prepare();
            Query.ExecuteNonQuery();
            leitor = Query.ExecuteReader();
            List<int> resultados = new List<int>();
            while (leitor.Read())
            {
                resultados.Add(leitor.GetInt32(PERGUNTA_ID));
            }
            Query.Parameters.Clear();
            leitor.Close();
            return resultados;
        }
        public objeto_pergunta selecionarUltimaPerguntaCadastrada()
        {
            Query.Connection = ConexaoBD.getConexao();
            Query.CommandText = "SELECT " +
                PERGUNTA_ID + "," +
                PERGUNTA_ID_FORMULARIO + "," +
                PERGUNTA_NOMECLATURA + "," +
                PERGUNTA_NULO + "," +
                PERGUNTA_POSICAO + "," +
                PERGUNTA_RESP_MAX+ "" +
                " FROM " +
                PERGUNTA_TABELA +
                " WHERE " + PERGUNTA_ID + " = (SELECT max(" + PERGUNTA_ID + ") FROM "+ PERGUNTA_TABELA +" LIMIT 1)";
            Query.ExecuteNonQuery();
            leitor = Query.ExecuteReader();
            objeto_pergunta resultado = new objeto_pergunta();
            while (leitor.Read())
            {
                resultado.id_pergunta = leitor.GetInt32(PERGUNTA_ID);
                resultado.posicao = leitor.GetInt32(PERGUNTA_POSICAO);
                resultado.nomenclatura=leitor.GetString(PERGUNTA_NOMECLATURA);
                resultado.permite_nulo = leitor.GetChar(PERGUNTA_NULO);
                resultado.maxResp = leitor.GetInt32(PERGUNTA_RESP_MAX);
                resultado.id_formulario = leitor.GetInt32(PERGUNTA_ID_FORMULARIO);
            }
            Query.Parameters.Clear();
            leitor.Close();
            return resultado;
        }
        public objeto_pergunta selecionarPerguntaMaiorPosicao_porIdForm(int id_form)
        {
            Query.Connection = ConexaoBD.getConexao();
            Query.CommandText = "SELECT " +
                PERGUNTA_ID + "," +
                PERGUNTA_ID_FORMULARIO + "," +
                PERGUNTA_NOMECLATURA + "," +
                PERGUNTA_NULO + "," +
                PERGUNTA_POSICAO + "," +
                PERGUNTA_RESP_MAX + "" +
                " FROM " +
                PERGUNTA_TABELA +
                " WHERE " + PERGUNTA_POSICAO + " = (SELECT max(" + PERGUNTA_POSICAO + ") FROM " + PERGUNTA_TABELA + " WHERE " + PERGUNTA_ID_FORMULARIO + " = @Id_form LIMIT 1) AND " + PERGUNTA_ID_FORMULARIO + " = @Id_form ";
            Query.Parameters.AddWithValue("@Id_form",id_form);
            Query.Prepare();
            Query.ExecuteNonQuery();
            leitor = Query.ExecuteReader();
            objeto_pergunta resultado = new objeto_pergunta();
            while (leitor.Read())
            {
                resultado.id_pergunta = leitor.GetInt32(PERGUNTA_ID);
                resultado.posicao = leitor.GetInt32(PERGUNTA_POSICAO);
                resultado.nomenclatura = leitor.GetString(PERGUNTA_NOMECLATURA);
                resultado.permite_nulo = leitor.GetChar(PERGUNTA_NULO);
                resultado.maxResp = leitor.GetInt32(PERGUNTA_RESP_MAX);
                resultado.id_formulario = leitor.GetInt32(PERGUNTA_ID_FORMULARIO);
            }
            Query.Parameters.Clear();
            leitor.Close();
            return resultado;
        }
        public objeto_pergunta selecionarPergunta_porID(int id_pergunta)
        {
            Query.Connection = ConexaoBD.getConexao();
            Query.CommandText = "SELECT " +
                PERGUNTA_ID + "," +
                PERGUNTA_ID_FORMULARIO + "," +
                PERGUNTA_NOMECLATURA + "," +
                PERGUNTA_NULO + "," +
                PERGUNTA_POSICAO + "," +
                PERGUNTA_RESP_MAX + "" +
                " FROM " +
                PERGUNTA_TABELA +
                " WHERE " + PERGUNTA_ID + " = @IdPerg LIMIT 1";
            Query.Parameters.AddWithValue("@IdPerg", id_pergunta);
            Query.Prepare();
            Query.ExecuteNonQuery();
            leitor = Query.ExecuteReader();
            objeto_pergunta resultado = new objeto_pergunta();
            while (leitor.Read())
            {
                resultado.id_pergunta = leitor.GetInt32(PERGUNTA_ID);
                resultado.posicao = leitor.GetInt32(PERGUNTA_POSICAO);
                resultado.nomenclatura = leitor.GetString(PERGUNTA_NOMECLATURA);
                resultado.permite_nulo = leitor.GetChar(PERGUNTA_NULO);
                resultado.maxResp = leitor.GetInt32(PERGUNTA_RESP_MAX);
                resultado.id_formulario = leitor.GetInt32(PERGUNTA_ID_FORMULARIO);
            }
            Query.Parameters.Clear();
            leitor.Close();
            return resultado;
        }
        public List<objeto_pergunta> selecionarTodasPerguntasCadastradasEAtivas_porIDForm(int id_form)
        {
            Query.Connection = ConexaoBD.getConexao();
            Query.CommandText = "SELECT " +
                PERGUNTA_ID + "," +
                PERGUNTA_ID_FORMULARIO + "," +
                PERGUNTA_NOMECLATURA + "," +
                PERGUNTA_NULO + "," +
                PERGUNTA_POSICAO + "," +
                PERGUNTA_RESP_MAX + "" +
                " FROM " +
                PERGUNTA_TABELA +
                " WHERE " + PERGUNTA_ATIVO + " = 's' AND " + PERGUNTA_ID_FORMULARIO + " = @idForm ORDER BY " + PERGUNTA_POSICAO + " ASC";
            Query.Parameters.AddWithValue("@idForm", id_form);            
            Query.Prepare();
            Query.ExecuteNonQuery();
            leitor = Query.ExecuteReader();
            List<objeto_pergunta> resultados = new List<objeto_pergunta>();
            while (leitor.Read())
            {
                objeto_pergunta resultado = new objeto_pergunta();
                resultado.id_pergunta = leitor.GetInt32(PERGUNTA_ID);
                resultado.posicao = leitor.GetInt32(PERGUNTA_POSICAO);
                resultado.nomenclatura = leitor.GetString(PERGUNTA_NOMECLATURA);
                resultado.permite_nulo = leitor.GetChar(PERGUNTA_NULO);
                resultado.maxResp = leitor.GetInt32(PERGUNTA_RESP_MAX);
                resultado.id_formulario = leitor.GetInt32(PERGUNTA_ID_FORMULARIO);
                resultados.Add(resultado);
            }
            Query.Parameters.Clear();
            leitor.Close();
            return resultados;
        }
        //*********************
        //*****************************RESPOSTAS*****************
        public List<objeto_resposta> selecionarTodasRespostasAtivas_deUmaPerguntaID(int id_pergunta)
        {           
            Query.Connection = ConexaoBD.getConexao();
            Query.CommandText = "SELECT "+
                RESPOSTA_ID +","+               
                RESPOSTA_ID_TIPO+","+
                RESPOSTA_NOMECLATURA +","+
                RESPOSTA_POSICAO +
                " FROM " +
                RESPOSTA_TABELA +
                " WHERE "+RESPOSTA_ID_PERGUNTA+" = " + id_pergunta + " AND " + RESPOSTA_ATIVO + " = 's' ORDER BY " + RESPOSTA_POSICAO + " ASC";
            Query.Prepare();
            Query.ExecuteNonQuery();
            leitor = Query.ExecuteReader();
            List<objeto_resposta> resultados = new List<objeto_resposta>();
            while (leitor.Read())
            {
                objeto_resposta obj_resp = new objeto_resposta();
                obj_resp.id_resposta = leitor.GetInt32(RESPOSTA_ID);
                obj_resp.tipo = leitor.GetInt32(RESPOSTA_ID_TIPO);
                obj_resp.nomenclatura = leitor.GetString(RESPOSTA_NOMECLATURA);
                obj_resp.posicao = leitor.GetInt32(RESPOSTA_POSICAO);
                resultados.Add(obj_resp);
            }
            Query.Parameters.Clear();
            leitor.Close();
            return resultados;
        }     
        public objeto_resposta selecionarPosicaoUltimaRespostaAtiva_deUmaPerguntaID(int id_pergunta)
        {           
            Query.Connection = ConexaoBD.getConexao();
            Query.CommandText = "SELECT "+
                RESPOSTA_ID +","+               
                RESPOSTA_ID_TIPO+","+
                RESPOSTA_NOMECLATURA+","+
                RESPOSTA_POSICAO +
                " FROM " +
                RESPOSTA_TABELA +
            " WHERE " + RESPOSTA_POSICAO + " = (SELECT max(" + RESPOSTA_POSICAO + ") FROM " + RESPOSTA_TABELA + " WHERE " + RESPOSTA_ID_PERGUNTA + " = " + id_pergunta + " AND "+RESPOSTA_ATIVO+" = 's' LIMIT 1) AND " + RESPOSTA_ID_PERGUNTA + " = " + id_pergunta + " AND "+RESPOSTA_ATIVO+" = 's' LIMIT 1";
            Query.Prepare();
            Query.ExecuteNonQuery();
            leitor = Query.ExecuteReader(); 
            objeto_resposta obj_resp = new objeto_resposta(); 
            while (leitor.Read())
            {                
                obj_resp.id_resposta = leitor.GetInt32(RESPOSTA_ID);
                obj_resp.tipo = leitor.GetInt32(RESPOSTA_ID_TIPO);
                obj_resp.nomenclatura = leitor.GetString(RESPOSTA_NOMECLATURA);
                obj_resp.posicao = leitor.GetInt32(RESPOSTA_POSICAO);                
            }
            Query.Parameters.Clear();
            leitor.Close();
            return obj_resp;
        }
        public objeto_resposta selecionarIDUltimaRespostaAtiva_deUmaPerguntaID(int id_pergunta)
        {
            Query.Connection = ConexaoBD.getConexao();
            Query.CommandText = "SELECT " +
                RESPOSTA_ID + "," +
                RESPOSTA_ID_TIPO + "," +
                RESPOSTA_NOMECLATURA + "," +
                RESPOSTA_POSICAO +
                " FROM " +
                RESPOSTA_TABELA +
            " WHERE " + RESPOSTA_ID + " = (SELECT max(" + RESPOSTA_ID + ") FROM " + RESPOSTA_TABELA + " WHERE " + RESPOSTA_ID_PERGUNTA + " = @IdPerg AND " + RESPOSTA_ATIVO + " = 's' LIMIT 1) AND " + RESPOSTA_ID_PERGUNTA + " = " + id_pergunta + " AND " + RESPOSTA_ATIVO + " = 's' LIMIT 1";
            Query.Parameters.AddWithValue("@IdPerg", id_pergunta);
            Query.Prepare();
            Query.ExecuteNonQuery();
            leitor = Query.ExecuteReader();
            objeto_resposta obj_resp = new objeto_resposta();
            while (leitor.Read())
            {
                obj_resp.id_resposta = leitor.GetInt32(RESPOSTA_ID);
                obj_resp.tipo = leitor.GetInt32(RESPOSTA_ID_TIPO);
                obj_resp.nomenclatura = leitor.GetString(RESPOSTA_NOMECLATURA);
                obj_resp.posicao = leitor.GetInt32(RESPOSTA_POSICAO);
            }
            Query.Parameters.Clear();
            leitor.Close();
            return obj_resp;
        }
        
        //************RESPOSTA*TEXTO**********
        public List<String> selecionarTodasRespostasTextoDistintas_deUmaRespostaID(int id_resposta)
        {
            Query.Connection = ConexaoBD.getConexao();
            Query.CommandText = " SELECT distinct(" + COLETA_DADOS_TEXTO_TABELA + "." + COLETA_DADOS_TEXTO_TEXTO + ")as " + COLETA_DADOS_TEXTO_TEXTO + " FROM " + COLETA_DADOS_TEXTO_TABELA + "" +
            " INNER JOIN "+COLETA_DADOS_TABELA+" "+
            " ON "+COLETA_DADOS_TABELA+"."+COLETA_DADOS_ID+" = "+COLETA_DADOS_TEXTO_TABELA+"."+COLETA_DADOS_TEXTO_COLETA_DADOS_ID_RESPOSTA+
            " INNER JOIN "+RESPOSTA_TABELA+
            " ON "+RESPOSTA_TABELA+"."+RESPOSTA_ID +" = "+COLETA_DADOS_TABELA+"."+COLETA_DADOS_ID_RESPOSTA+
            " WHERE "+RESPOSTA_TABELA+"."+RESPOSTA_ID+" = @IdResp "+"ORDER BY coleta_dados_resposta_texto.texto ASC";
            Query.Parameters.AddWithValue("@IdResp", id_resposta);
            Query.Prepare();
            Query.ExecuteNonQuery();
            leitor = Query.ExecuteReader();
            List<String> resultados = new List<String>();
            while (leitor.Read())
            {                
                resultados.Add(leitor.GetString(COLETA_DADOS_TEXTO_TEXTO));
            }
            Query.Parameters.Clear();
            leitor.Close();
            return resultados;
        }  
        //**************************CIDADES*********************
        public List<String> selecionarTodasCidadesDistintas()
        {
            Query.Connection = ConexaoBD.getConexao();
            Query.CommandText = " SELECT distinct(" + CIDADE_NOME + ") FROM "+ CIDADE_TABELA +
            " ORDER BY " + CIDADE_NOME + " ASC";
            Query.ExecuteNonQuery();
            leitor = Query.ExecuteReader();
            List<String> resultados = new List<String>();
            while (leitor.Read())
            {
                resultados.Add(leitor.GetString(CIDADE_NOME));
            }
            Query.Parameters.Clear();
            leitor.Close();
            return resultados;
        }
        //************************ESTADOS*********************
        public List<String> selecionarTodosEstadosDistintos()
        {
            Query.Connection = ConexaoBD.getConexao();
            Query.CommandText = " SELECT distinct(" + ESTADO_UF + ") FROM " + ESTADO_TABELA +
            " ORDER BY " + ESTADO_UF + " ASC";
            Query.ExecuteNonQuery();
            leitor = Query.ExecuteReader();
            List<String> resultados = new List<String>();
            while (leitor.Read())
            {
                resultados.Add(leitor.GetString(ESTADO_UF));
            }
            Query.Parameters.Clear();
            leitor.Close();
            return resultados;
        } 
        //*****************************CLIENTE****************
        public List<objeto_cliente> selecionarTodosClientes()
        {
            Query.Connection = ConexaoBD.getConexao();
            Query.CommandText = "SELECT " +
            CLIENTE_ID + "," +
            CLIENTE_NOME + "" +
            " FROM " + CLIENTE_TABELA + " ORDER BY " + CLIENTE_NOME + " ASC ";
            Query.ExecuteNonQuery();
            leitor = Query.ExecuteReader();
            List<objeto_cliente> resultados = new List<objeto_cliente>();
            while (leitor.Read())
            {
                objeto_cliente cliente = new objeto_cliente();
                cliente.id_cliente = leitor.GetInt32(CLIENTE_ID);
                cliente.nome = leitor.GetString(CLIENTE_NOME);
                resultados.Add(cliente);
            }
            Query.Parameters.Clear();
            leitor.Close();
            return resultados;
        }
        public objeto_cliente selecionarUmCliente_porIDCliente(int id)
        {
            Query.Connection = ConexaoBD.getConexao();
            Query.CommandText = "SELECT " +
            CLIENTE_ID + "," +
            CLIENTE_NOME + "" +
            " FROM " + CLIENTE_TABELA + " WHERE " + CLIENTE_ID + " = "+id+" LIMIT 1 ";
            Query.ExecuteNonQuery();
            leitor = Query.ExecuteReader();
            objeto_cliente cliente = new objeto_cliente();
            while (leitor.Read())
            {                
                cliente.id_cliente = leitor.GetInt32(CLIENTE_ID);
                cliente.nome = leitor.GetString(CLIENTE_NOME);
            }
            Query.Parameters.Clear();
            leitor.Close();
            return cliente;
        }
        //************************
        //***************************FORMULARIOS*********************
        public List<objeto_formulario> selecionarTodosFormularios()
        {
            Query.Connection = ConexaoBD.getConexao();
            Query.CommandText = "SELECT " +
            FORMULARIO_ID + "," +
            FORMULARIO_ID_CLIENTE + "," +
            FORMULARIO_TITULO + "," +
            FORMULARIO_DT_CRIACAO + "," +
            FORMULARIO_DT_ENTREGA + "," +
            FORMULARIO_SITUACAO + "," +
            FORMULARIO_ATIVO + "" +
            " FROM " + FORMULARIO_TABELA + " ORDER BY " + FORMULARIO_DT_CRIACAO + " DESC ";
            Query.ExecuteNonQuery();
            leitor = Query.ExecuteReader();
            List<objeto_formulario> resultados = new List<objeto_formulario>();
            while (leitor.Read())
            {
                objeto_formulario form = new objeto_formulario();
                form.id_formulario= leitor.GetInt32(FORMULARIO_ID);
                form.id_cliente = leitor.GetInt32(FORMULARIO_ID_CLIENTE);
                form.titulo = leitor.GetString(FORMULARIO_TITULO);
                form.data_criacao = leitor.GetString(FORMULARIO_DT_CRIACAO);
                form.data_entrega = leitor.GetString(FORMULARIO_DT_ENTREGA);
                form.situacao = leitor.GetChar(FORMULARIO_SITUACAO);
                form.ativo = leitor.GetChar(FORMULARIO_ATIVO);
                resultados.Add(form);
            }
            Query.Parameters.Clear();
            leitor.Close();
            return resultados;
        }
        public List<objeto_formulario> selecionarTodosFormularios_somenteEmAndamento()
        {
            Query.Connection = ConexaoBD.getConexao();
            Query.CommandText = "SELECT " +
            FORMULARIO_ID + "," +
            FORMULARIO_ID_CLIENTE + "," +
            FORMULARIO_TITULO + "," +
            FORMULARIO_DT_CRIACAO + "," +
            FORMULARIO_DT_ENTREGA + "," +
            FORMULARIO_SITUACAO + "," +
            FORMULARIO_ATIVO + "" +
            " FROM " + FORMULARIO_TABELA + " WHERE " + FORMULARIO_SITUACAO + " = 'a' ORDER BY " + FORMULARIO_DT_CRIACAO + " DESC ";
            Query.ExecuteNonQuery();
            leitor = Query.ExecuteReader();
            List<objeto_formulario> resultados = new List<objeto_formulario>();
            while (leitor.Read())
            {
                objeto_formulario form = new objeto_formulario();
                form.id_formulario = leitor.GetInt32(FORMULARIO_ID);
                form.id_cliente = leitor.GetInt32(FORMULARIO_ID_CLIENTE);
                form.titulo = leitor.GetString(FORMULARIO_TITULO);
                form.data_criacao = leitor.GetString(FORMULARIO_DT_CRIACAO);
                form.data_entrega = leitor.GetString(FORMULARIO_DT_ENTREGA);
                form.situacao = leitor.GetChar(FORMULARIO_SITUACAO);
                form.ativo = leitor.GetChar(FORMULARIO_ATIVO);
                resultados.Add(form);
            }
            Query.Parameters.Clear();
            leitor.Close();
            return resultados;
        }
        public objeto_formulario selecionarUmFormulario_porID(int id)
        {
            Query.Connection = ConexaoBD.getConexao();
            Query.CommandText = "SELECT " +
            FORMULARIO_ID + "," +
            FORMULARIO_ID_CLIENTE + "," +
            FORMULARIO_TITULO + "," +
            FORMULARIO_DT_CRIACAO + "," +
            FORMULARIO_DT_ENTREGA + "," +
            FORMULARIO_SITUACAO + "," +
            FORMULARIO_ATIVO + "" +
            " FROM " + FORMULARIO_TABELA + " WHERE "+ FORMULARIO_ID +" = "+ id + " LIMIT 1 ";
            Query.ExecuteNonQuery();
            leitor = Query.ExecuteReader();
            objeto_formulario resultado = new objeto_formulario();
            while (leitor.Read())
            {
                objeto_formulario form = new objeto_formulario();
                form.id_formulario = leitor.GetInt32(FORMULARIO_ID);
                form.id_cliente = leitor.GetInt32(FORMULARIO_ID_CLIENTE);
                form.titulo = leitor.GetString(FORMULARIO_TITULO);
                form.data_criacao = leitor.GetString(FORMULARIO_DT_CRIACAO);
                form.data_entrega = leitor.GetString(FORMULARIO_DT_ENTREGA);
                form.situacao = leitor.GetChar(FORMULARIO_SITUACAO);
                form.ativo = leitor.GetChar(FORMULARIO_ATIVO);
                resultado = form;
            }
            Query.Parameters.Clear();
            leitor.Close();
            return resultado;
        }
        //****************************************
        //**************QUESTIONARIOS*************
        public int selecionarQtdQuestionariosRespondidos_porIDForm(int id_form)
        {
            Query.Connection = ConexaoBD.getConexao();
            Query.CommandText = " SELECT " +
            COLETA_QUESTIONARIO_ID + "" +
            " FROM " + COLETA_QUESTIONARIO_TABELA + " WHERE " + COLETA_QUESTIONARIO_ID_FORMULARIO + " = @Id_form ";
            Query.Parameters.AddWithValue("@Id_form", id_form);
            Query.Prepare();
            Query.ExecuteNonQuery();
            leitor = Query.ExecuteReader();
            int resultados = 0;
            while (leitor.Read())
            {
                resultados++;
            }
            Query.Parameters.Clear();
            leitor.Close();
            return resultados;
        }
        //****************************************
        //***************************POSTO*COLETA********************
        public List<objeto_posto_coleta> selecionarTodosPostos_Coleta()
        {
            Query.Connection = ConexaoBD.getConexao();
            Query.CommandText = "SELECT " +
            COLETA_POSTO_ID+ "," +
            COLETA_POSTO_NOME + "," +
             COLETA_POSTO_DATA_CRIACAO + "" +
            " FROM " + COLETA_POSTO_TABELA + " ORDER BY " + COLETA_POSTO_DATA_CRIACAO + " DESC ";
            Query.ExecuteNonQuery();
            leitor = Query.ExecuteReader();
            List<objeto_posto_coleta> resultados = new List<objeto_posto_coleta>();
            while (leitor.Read())
            {
                objeto_posto_coleta posto = new objeto_posto_coleta();
                posto.id_posto = leitor.GetInt32(COLETA_POSTO_ID);
                posto.nome = leitor.GetString(COLETA_POSTO_NOME);
                posto.data_criacao = leitor.GetString(COLETA_POSTO_DATA_CRIACAO);               
                resultados.Add(posto);
            }
            Query.Parameters.Clear();
            leitor.Close();
            return resultados;
        }
        public objeto_posto_coleta selecionarUmPosto_Coleta_porID(int id)
        {
            Query.Connection = ConexaoBD.getConexao();
            Query.CommandText = "SELECT " +
            COLETA_POSTO_ID + "," +
            COLETA_POSTO_NOME + "," +
             COLETA_POSTO_DATA_CRIACAO + "" +
            " FROM " + COLETA_POSTO_TABELA + " WHERE " + COLETA_POSTO_ID + " = " + id + " LIMIT 1 ";
            Query.ExecuteNonQuery();
            leitor = Query.ExecuteReader();
            objeto_posto_coleta resultado = new objeto_posto_coleta();
            while (leitor.Read())
            {
                objeto_posto_coleta posto = new objeto_posto_coleta();
                posto.id_posto = leitor.GetInt32(COLETA_POSTO_ID);
                posto.nome = leitor.GetString(COLETA_POSTO_NOME);
                posto.data_criacao = leitor.GetString(COLETA_POSTO_DATA_CRIACAO);
                resultado = posto;
            }
            Query.Parameters.Clear();
            leitor.Close();
            return resultado;
        }
        //
        //**************************************************INSERTS**************************************************************************************************************
       //*******************COLETA*QUESTIONARIO*************************
        public int inserir_umQuestionarioEReceberIDdele(int id_posto_coleta, int id_form, MySqlTransaction transacaoRecebida)
        {
            if(id_posto_coleta > 0)
            {
            //aviso ao banco reconhecer este obj como sinalizador de transação!
            MySqlTransaction transacao = transacaoRecebida;
            Query.Connection = ConexaoBD.getConexao();
            //seto o obj de transacao!
            Query.Transaction = transacao;
            Query.CommandText = "INSERT INTO " + COLETA_QUESTIONARIO_TABELA + " ("+
            COLETA_QUESTIONARIO_ID_POSTO_COLETA + 
            ","+COLETA_QUESTIONARIO_ID_FORMULARIO+")VALUES(@Id_posto,@Id_form)";
            Query.Parameters.AddWithValue("@Id_posto", id_posto_coleta);
            Query.Parameters.AddWithValue("@Id_form", id_form);
            Query.Prepare();
            if (Query.ExecuteNonQuery() == 1)
            {
                Query.Parameters.Clear();
                //capto o id novo do questionario para retornar ao método!
                Query.Connection = ConexaoBD.getConexao();
                Query.CommandText = "SELECT " +
                COLETA_QUESTIONARIO_ID + "" +
                " FROM " + COLETA_QUESTIONARIO_TABELA + " WHERE " + COLETA_QUESTIONARIO_ID + " = LAST_INSERT_ID() LIMIT 1";
                Query.Prepare();
                Query.ExecuteNonQuery();
                leitor = Query.ExecuteReader();
                int id_resposta_coletada = 0;
                while (leitor.Read())
                {
                    id_resposta_coletada = leitor.GetInt32(COLETA_QUESTIONARIO_ID);
                }
                Query.Parameters.Clear();
                leitor.Close();
                if (id_resposta_coletada > 0)
                {
                    return id_resposta_coletada;
                }
                else
                {
                    Query.Parameters.Clear();
                    return 0;
                }
            }
            else
            {
                Query.Parameters.Clear();
                return 0;
            }
            }
            else
            {
                return 0;
            }
        }
        //***************************************************************
        //*********************PERGUNTA****************************
        public Boolean inserir_umaPerguntaNoFormulario(int id_form, Boolean estado_ativo, String nomenclatura, int posicao, Boolean aceita_nulo, int maxRespostas)
        {
            Query.Connection = ConexaoBD.getConexao();
            Query.CommandText = "INSERT INTO " + PERGUNTA_TABELA + " (" +
                PERGUNTA_ID_FORMULARIO +
                "," + PERGUNTA_ATIVO +
                "," + PERGUNTA_NOMECLATURA +
                "," + PERGUNTA_POSICAO +
                "," + PERGUNTA_NULO +
                "," + PERGUNTA_RESP_MAX + ")" +
            "VALUES(@Id_Form,@Ativo,@Nomenclatura,@Posicao,@Nulo,@MaxResp)";
            Query.Parameters.AddWithValue("@Id_Form", id_form);
            Query.Parameters.AddWithValue("@Ativo", identificar_SimNao_BoolToChar(estado_ativo));
            Query.Parameters.AddWithValue("@Nomenclatura", nomenclatura);
            Query.Parameters.AddWithValue("@Posicao", posicao);
            Query.Parameters.AddWithValue("@Nulo", identificar_SimNao_BoolToChar(aceita_nulo));
            Query.Parameters.AddWithValue("@MaxResp", maxRespostas);
            Query.Prepare();
            if (Query.ExecuteNonQuery() >= 1)
            {
                Query.Parameters.Clear();
                return true;
            }
            else
            {
                Query.Parameters.Clear();
                return false;
            }
        }
        //***********************RESPOSTAS***********************************
        public Boolean inserir_umaRespostaNaPergunta(int id_pergunta, Boolean estado_ativo, String nomenclatura, int posicao, int tipo_resposta)
        {
            Query.Connection = ConexaoBD.getConexao();
            Query.CommandText = "INSERT INTO " + RESPOSTA_TABELA + " (" +
                RESPOSTA_ID_PERGUNTA +
                "," + RESPOSTA_ATIVO +
                "," + RESPOSTA_NOMECLATURA +
                "," + RESPOSTA_POSICAO +
                "," + RESPOSTA_ID_TIPO + ")" +
            "VALUES(@Id_Perg,@Ativo,@Nomenclatura,@Posicao,@Tipo)";
            Query.Parameters.AddWithValue("@Id_Perg", id_pergunta);
            Query.Parameters.AddWithValue("@Ativo", identificar_SimNao_BoolToChar(estado_ativo));
            Query.Parameters.AddWithValue("@Nomenclatura", nomenclatura);
            Query.Parameters.AddWithValue("@Posicao", posicao);
            Query.Parameters.AddWithValue("@Tipo", tipo_resposta);
            Query.Prepare();
            if (Query.ExecuteNonQuery() >= 1)
            {
                Query.Parameters.Clear();
                return true;
            }
            else
            {
                Query.Parameters.Clear();
                return false;
            }
        }        
        public Boolean inserir_umaRespostaCompleta_texto(int id_pergunta, int id_resposta, String texto, int id_questionario, MySqlTransaction transacaoRecebida)
        {            
            //aviso ao banco reconhecer este obj como sinalizador de transação!
            MySqlTransaction transacao = transacaoRecebida;
            Query.Connection = ConexaoBD.getConexao();
            //seto o obj de transacao!
            Query.Transaction = transacao;
            Query.CommandText = "INSERT INTO " + COLETA_DADOS_TABELA + " (" +
                COLETA_DADOS_ID_RESPOSTA +","+
                COLETA_DADOS_ID_QUESTIONARIO + 
                "," + COLETA_DADOS_ID_PERGUNTA + ")" +
            "VALUES(@Id_Resp,@Id_q,@Id_Perg)";
            Query.Parameters.AddWithValue("@Id_Resp", id_resposta);
            Query.Parameters.AddWithValue("@Id_Perg", id_pergunta);
            Query.Parameters.AddWithValue("@Id_q", id_questionario);
            Query.Prepare();
            if (Query.ExecuteNonQuery() == 1)
            {
                Query.Parameters.Clear();
                //capto o id novo da resposta para adicionar o texto referente!
                Query.Connection = ConexaoBD.getConexao();
                Query.CommandText = "SELECT " +
                COLETA_DADOS_ID + "" +
                " FROM " + COLETA_DADOS_TABELA + " WHERE " + COLETA_DADOS_ID + " = LAST_INSERT_ID() AND "+COLETA_DADOS_ID_PERGUNTA+
                " = @Id_Perg AND "+COLETA_DADOS_ID_RESPOSTA+" = @Id_Resp LIMIT 1";
                Query.Parameters.AddWithValue("@Id_Perg", id_pergunta);
                Query.Parameters.AddWithValue("@Id_Resp", id_resposta);
                Query.Prepare();
                Query.ExecuteNonQuery();
                leitor = Query.ExecuteReader(); 
                int id_resposta_coletada = 0;
                while (leitor.Read())
                {                
                    id_resposta_coletada = leitor.GetInt32(COLETA_DADOS_ID);               
                }
                Query.Parameters.Clear();
                leitor.Close();
                if (id_resposta_coletada > 0)//se recebi o ultimo id corretamente é só gravar!
                {
                    Query.CommandText = "INSERT INTO " + COLETA_DADOS_TEXTO_TABELA + " (" +
                    COLETA_DADOS_TEXTO_COLETA_DADOS_ID_RESPOSTA +
                    "," + COLETA_DADOS_TEXTO_TEXTO + ")" +
                    "VALUES(@Id_cl_Resp,@Texto)";
                    Query.Parameters.AddWithValue("@Id_cl_Resp", id_resposta_coletada);
                    Query.Parameters.AddWithValue("@Texto", texto.ToString().TrimEnd().TrimStart().ToUpper());
                    Query.Prepare();
                    if (Query.ExecuteNonQuery() == 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                Query.Parameters.Clear();
                transacao.Rollback();
                return false;
            }
        }
        public Boolean inserir_umaRespostaCompleta(int id_pergunta, int id_resposta, int id_questionario,MySqlTransaction transacaoRecebida)
        {
            //aviso ao banco reconhecer este obj como sinalizador de transação!
            MySqlTransaction transacao = transacaoRecebida;
            Query.Connection = ConexaoBD.getConexao();
            //seto o obj de transacao!
            Query.Transaction = transacao;
            Query.CommandText = "INSERT INTO " + COLETA_DADOS_TABELA + " (" +
                COLETA_DADOS_ID_RESPOSTA +
                "," + COLETA_DADOS_ID_PERGUNTA +
                ","+ COLETA_DADOS_ID_QUESTIONARIO + ")" +
            "VALUES(@Id_Resp,@Id_Perg,@Id_q)";
            Query.Parameters.AddWithValue("@Id_Resp", id_resposta);
            Query.Parameters.AddWithValue("@Id_Perg", id_pergunta);
            Query.Parameters.AddWithValue("@Id_q", id_questionario);
            Query.Prepare();
            if (Query.ExecuteNonQuery() == 1)
            {
                Query.Parameters.Clear();
                return true;
            }
            else
            {
                Query.Parameters.Clear();
                return false;
            }
        }
        //****************************************************************
        //*******************POSTO*COLETA***********************
        public Boolean inserir_umPostoColeta(String nome)
        {
            Query.Connection = ConexaoBD.getConexao();
            Query.CommandText = "INSERT INTO " + COLETA_POSTO_TABELA + " (" +
                COLETA_POSTO_NOME +
                "," + COLETA_POSTO_DATA_CRIACAO + ")" +
            "VALUES(@Nome, CURDATE())";
            Query.Parameters.AddWithValue("@Nome", nome);
            Query.Prepare();
            if (Query.ExecuteNonQuery() >= 1)
            {
                Query.Parameters.Clear();
                return true;
            }
            else
            {
                Query.Parameters.Clear();
                return false;
            }
        }
        //*******************FORMULARIO***************
        public Boolean inserir_umFormulario(String nome, int id_cliente, String Dtentrega)
        {
            Query.Connection = ConexaoBD.getConexao();
            Query.CommandText = "INSERT INTO " + FORMULARIO_TABELA + " (" +
                FORMULARIO_TITULO +
                "," + FORMULARIO_SITUACAO +
                "," + FORMULARIO_DT_CRIACAO+
                "," + FORMULARIO_ATIVO +
                 "," + FORMULARIO_ID_CLIENTE +
                  "," + FORMULARIO_DT_ENTREGA + ")" +
            "VALUES(@Nome, 'a', CURDATE(), 's',@IdCliente, @DTEntrega)";
            Query.Parameters.AddWithValue("@Nome", nome);
            Query.Parameters.AddWithValue("@IdCliente", id_cliente);
            Query.Parameters.AddWithValue("@DTEntrega", Dtentrega);
            Query.Prepare();
            if (Query.ExecuteNonQuery() >= 1)
            {
                Query.Parameters.Clear();
                return true;
            }
            else
            {
                Query.Parameters.Clear();
                return false;
            }
        }
        //*******************CLIENTES***************
        public Boolean inserir_umCliente(String nome)
        {
            Query.Connection = ConexaoBD.getConexao();
            Query.CommandText = "INSERT INTO " + CLIENTE_TABELA + " (" +
                CLIENTE_NOME +
                " , "+ CLIENTE_DATA_CRIACAO +
               ")" +
            "VALUES(@Nome, CURDATE())";
            Query.Parameters.AddWithValue("@Nome", nome);
            Query.Prepare();
            if (Query.ExecuteNonQuery() >= 1)
            {
                Query.Parameters.Clear();
                return true;
            }
            else
            {
                Query.Parameters.Clear();
                return false;
            }
        }
    }
}
