-- phpMyAdmin SQL Dump
-- Clean MySQL Schema & Structural Seed
--
-- Host: 127.0.0.1

CREATE DATABASE IF NOT EXISTS `db_survey_data_entry`
  DEFAULT CHARACTER SET utf8
  DEFAULT COLLATE utf8_bin;
  
USE `db_survey_data_entry`;

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;

--
-- Database: `db_survey_data_entry`
--

-- --------------------------------------------------------

--
-- Estrutura da tabela `cliente_requerente`
--

CREATE TABLE IF NOT EXISTS `cliente_requerente` (
  `id_cliente_req` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `nome_cliente` varchar(60) COLLATE utf8_bin DEFAULT NULL,
  `data_criacao` date NOT NULL,
  `estado_ativo` char(1) COLLATE utf8_bin NOT NULL DEFAULT 's',
  PRIMARY KEY (`id_cliente_req`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin AUTO_INCREMENT=1 ;

-- --------------------------------------------------------

--
-- Estrutura da tabela `coleta_dados_questionario`
--

CREATE TABLE IF NOT EXISTS `coleta_dados_questionario` (
  `id_questionario` int(11) NOT NULL AUTO_INCREMENT,
  `id_posto_coleta` int(11) NOT NULL,
  `id_formulario_fk` int(11) NOT NULL,
  PRIMARY KEY (`id_questionario`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin AUTO_INCREMENT=1 ;

--
-- Acionadores `coleta_dados_questionario`
--
DROP TRIGGER IF EXISTS `apagar_respostas_se_apagar_questionarioPai`;
DELIMITER //
CREATE TRIGGER `apagar_respostas_se_apagar_questionarioPai` AFTER DELETE ON `coleta_dados_questionario`
 FOR EACH ROW BEGIN
DELETE FROM coleta_dados_resposta WHERE coleta_dados_resposta.id_questionario_fk = OLD.id_questionario;
END
//
DELIMITER ;

-- --------------------------------------------------------

--
-- Estrutura da tabela `coleta_dados_resposta`
--

CREATE TABLE IF NOT EXISTS `coleta_dados_resposta` (
  `id_coleta_dados_resposta` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `id_resposta_fk` int(10) unsigned DEFAULT NULL,
  `id_pergunta_fk` int(10) unsigned NOT NULL,
  `id_questionario_fk` int(11) NOT NULL,
  PRIMARY KEY (`id_coleta_dados_resposta`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin AUTO_INCREMENT=1 ;

--
-- Acionadores `coleta_dados_resposta`
--
DROP TRIGGER IF EXISTS `apagar_textos_se_apagar_respostaPai`;
DELIMITER //
CREATE TRIGGER `apagar_textos_se_apagar_respostaPai` AFTER DELETE ON `coleta_dados_resposta`
 FOR EACH ROW BEGIN
DELETE FROM coleta_dados_resposta_texto WHERE coleta_dados_resposta_texto.id_coleta_dados_resposta_fk = OLD.id_coleta_dados_resposta;
END
//
DELIMITER ;

-- --------------------------------------------------------

--
-- Estrutura da tabela `coleta_dados_resposta_texto`
--

CREATE TABLE IF NOT EXISTS `coleta_dados_resposta_texto` (
  `id_coleta_dados_resposta_texto` int(11) NOT NULL AUTO_INCREMENT,
  `id_coleta_dados_resposta_fk` int(11) NOT NULL,
  `texto` varchar(200) NOT NULL,
  PRIMARY KEY (`id_coleta_dados_resposta_texto`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1 AUTO_INCREMENT=1 ;

-- --------------------------------------------------------

--
-- Estrutura da tabela `coleta_posto`
--

CREATE TABLE IF NOT EXISTS `coleta_posto` (
  `id_posto` int(11) NOT NULL AUTO_INCREMENT,
  `nome` varchar(200) COLLATE utf8_bin NOT NULL,
  `data_criacao` date NOT NULL,
  PRIMARY KEY (`id_posto`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin AUTO_INCREMENT=1 ;

-- --------------------------------------------------------

--
-- Estrutura da tabela `formulario_pesquisa`
--

CREATE TABLE IF NOT EXISTS `formulario_pesquisa` (
  `id_formulario` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `id_cliente_req` int(10) unsigned NOT NULL,
  `data_criacao` date NOT NULL,
  `data_entrega` date DEFAULT NULL,
  `estado_situacao` char(1) COLLATE utf8_bin NOT NULL DEFAULT 'a' COMMENT 'a=andamento, f=finalizado, c=cancelado',
  `titulo_formulario` varchar(100) COLLATE utf8_bin NOT NULL,
  `estado_ativo` char(1) COLLATE utf8_bin NOT NULL DEFAULT 's',
  PRIMARY KEY (`id_formulario`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin AUTO_INCREMENT=1 ;

-- --------------------------------------------------------

--
-- Stand-in structure for view `partedois_somenterespostatipodois_contabilizarresultados`
--
CREATE TABLE IF NOT EXISTS `partedois_somenterespostatipodois_contabilizarresultados` (
`posicao_noFormulario` int(10) unsigned
,`pergunta` varchar(511)
,`resposta` text
,`qtd_respondida` bigint(21)
,`id_tipo_resposta_fk` int(10) unsigned
,`texto` varchar(200)
);
-- --------------------------------------------------------

--
-- Stand-in structure for view `partetres_somenterespostatipotres_contabilizarresultados`
--
CREATE TABLE IF NOT EXISTS `partetres_somenterespostatipotres_contabilizarresultados` (
`posicao_noFormulario` int(10) unsigned
,`pergunta` varchar(511)
,`resposta` text
,`qtd_respondida` bigint(21)
,`id_tipo_resposta_fk` int(10) unsigned
,`texto` varchar(200)
);
-- --------------------------------------------------------

--
-- Stand-in structure for view `parteum_somenterespostatipoum_contabilizarresultados`
--
CREATE TABLE IF NOT EXISTS `parteum_somenterespostatipoum_contabilizarresultados` (
`posicao_noFormulario` int(10) unsigned
,`pergunta` varchar(511)
,`resposta` text
,`qtd_respondida` bigint(21)
,`id_tipo_resposta_fk` int(10) unsigned
,`texto` varchar(200)
);
-- --------------------------------------------------------

--
-- Estrutura da tabela `pergunta_formulario`
--

CREATE TABLE IF NOT EXISTS `pergunta_formulario` (
  `id_pergunta` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `id_formulario` int(10) unsigned NOT NULL,
  `estado_ativo` char(1) COLLATE utf8_bin NOT NULL DEFAULT 's',
  `nomenclatura` varchar(500) COLLATE utf8_bin NOT NULL,
  `posicao_noFormulario` int(10) unsigned NOT NULL,
  `permite_nulo` char(1) COLLATE utf8_bin NOT NULL DEFAULT 's',
  `respostas_maxPermitidas` int(11) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id_pergunta`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin AUTO_INCREMENT=1 ;

-- --------------------------------------------------------

--
-- Estrutura da tabela `resposta_pergunta`
--

CREATE TABLE IF NOT EXISTS `resposta_pergunta` (
  `id_resposta` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `id_tipo_resposta_fk` int(10) unsigned NOT NULL,
  `id_pergunta` int(10) unsigned NOT NULL,
  `nomenclatura` text COLLATE utf8_bin NOT NULL,
  `posicao_naPergunta` int(10) unsigned NOT NULL,
  `estado_ativo` char(1) COLLATE utf8_bin NOT NULL DEFAULT 's',
  PRIMARY KEY (`id_resposta`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin AUTO_INCREMENT=1 ;

-- --------------------------------------------------------

--
-- Estrutura da tabela `tipo_resposta`
--

CREATE TABLE IF NOT EXISTS `tipo_resposta` (
  `id_tipo_resposta` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `nomenclatura` text COLLATE utf8_bin NOT NULL,
  `estado_ativo` char(1) COLLATE utf8_bin NOT NULL DEFAULT 's',
  PRIMARY KEY (`id_tipo_resposta`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin AUTO_INCREMENT=4 ;

--
-- Dados estruturais do sistema para `tipo_resposta`
--

INSERT IGNORE INTO `tipo_resposta` (`id_tipo_resposta`, `nomenclatura`, `estado_ativo`) VALUES
(1, 'Único Sem Texto', 's'),
(2, 'Único Com Texto', 's'),
(3, 'Multiescolha Quantitativa', 's');

-- --------------------------------------------------------

--
-- Structure for view `partedois_somenterespostatipodois_contabilizarresultados`
--
DROP TABLE IF EXISTS `partedois_somenterespostatipodois_contabilizarresultados`;

CREATE ALGORITHM=UNDEFINED DEFINER=`root`@`localhost` SQL SECURITY DEFINER VIEW `partedois_somenterespostatipodois_contabilizarresultados` AS select `pf`.`posicao_noFormulario` AS `posicao_noFormulario`,concat(`pf`.`posicao_noFormulario`,'.',`pf`.`nomenclatura`) AS `pergunta`,`rp`.`nomenclatura` AS `resposta`,count(`cdr`.`id_coleta_dados_resposta`) AS `qtd_respondida`,`rp`.`id_tipo_resposta_fk` AS `id_tipo_resposta_fk`,`cdrt`.`texto` AS `texto` from (((`pergunta_formulario` `pf` join `resposta_pergunta` `rp` on((`rp`.`id_pergunta` = `pf`.`id_pergunta`))) left join `coleta_dados_resposta` `cdr` on((`cdr`.`id_resposta_fk` = `rp`.`id_resposta`))) left join `coleta_dados_resposta_texto` `cdrt` on((`cdrt`.`id_coleta_dados_resposta_fk` = `cdr`.`id_coleta_dados_resposta`))) where ((`pf`.`estado_ativo` = 's') and (`pf`.`id_formulario` = 1) and (`rp`.`id_tipo_resposta_fk` = 2)) group by `cdrt`.`texto`;

-- --------------------------------------------------------

--
-- Structure for view `partetres_somenterespostatipotres_contabilizarresultados`
--
DROP TABLE IF EXISTS `partetres_somenterespostatipotres_contabilizarresultados`;

CREATE ALGORITHM=UNDEFINED DEFINER=`root`@`localhost` SQL SECURITY DEFINER VIEW `partetres_somenterespostatipotres_contabilizarresultados` AS select `pf`.`posicao_noFormulario` AS `posicao_noFormulario`,concat(`pf`.`posicao_noFormulario`,'.',`pf`.`nomenclatura`) AS `pergunta`,`rp`.`nomenclatura` AS `resposta`,count(`cdr`.`id_coleta_dados_resposta`) AS `qtd_respondida`,`rp`.`id_tipo_resposta_fk` AS `id_tipo_resposta_fk`,`cdrt`.`texto` AS `texto` from (((`pergunta_formulario` `pf` join `resposta_pergunta` `rp` on((`rp`.`id_pergunta` = `pf`.`id_pergunta`))) left join `coleta_dados_resposta` `cdr` on((`cdr`.`id_resposta_fk` = `rp`.`id_resposta`))) left join `coleta_dados_resposta_texto` `cdrt` on((`cdrt`.`id_coleta_dados_resposta_fk` = `cdr`.`id_coleta_dados_resposta`))) where ((`pf`.`estado_ativo` = 's') and (`pf`.`id_formulario` = 1) and (`rp`.`id_tipo_resposta_fk` = 3)) group by `rp`.`id_resposta`;

-- --------------------------------------------------------

--
-- Structure for view `parteum_somenterespostatipoum_contabilizarresultados`
--
DROP TABLE IF EXISTS `parteum_somenterespostatipoum_contabilizarresultados`;

CREATE ALGORITHM=UNDEFINED DEFINER=`root`@`localhost` SQL SECURITY DEFINER VIEW `parteum_somenterespostatipoum_contabilizarresultados` AS select `pf`.`posicao_noFormulario` AS `posicao_noFormulario`,concat(`pf`.`posicao_noFormulario`,'.',`pf`.`nomenclatura`) AS `pergunta`,`rp`.`nomenclatura` AS `resposta`,count(`cdr`.`id_coleta_dados_resposta`) AS `qtd_respondida`,`rp`.`id_tipo_resposta_fk` AS `id_tipo_resposta_fk`,`cdrt`.`texto` AS `texto` from (((`pergunta_formulario` `pf` join `resposta_pergunta` `rp` on((`rp`.`id_pergunta` = `pf`.`id_pergunta`))) left join `coleta_dados_resposta` `cdr` on((`cdr`.`id_resposta_fk` = `rp`.`id_resposta`))) left join `coleta_dados_resposta_texto` `cdrt` on((`cdrt`.`id_coleta_dados_resposta_fk` = `cdr`.`id_coleta_dados_resposta`))) where ((`pf`.`estado_ativo` = 's') and (`pf`.`id_formulario` = 1) and (`rp`.`id_tipo_resposta_fk` = 1)) group by `rp`.`id_resposta`;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
