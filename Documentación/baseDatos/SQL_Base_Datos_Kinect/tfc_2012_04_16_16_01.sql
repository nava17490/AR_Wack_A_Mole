-- MySQL dump 10.13  Distrib 5.5.9, for Win32 (x86)
--
-- Host: localhost    Database: tfc
-- ------------------------------------------------------
-- Server version	5.5.16

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Current Database: `tfc`
--

CREATE DATABASE /*!32312 IF NOT EXISTS*/ `tfc` /*!40100 DEFAULT CHARACTER SET utf8 */;

USE `tfc`;

--
-- Table structure for table `configuracion`
--

DROP TABLE IF EXISTS `configuracion`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `configuracion` (
  `codigo` int(11) NOT NULL AUTO_INCREMENT,
  `numSesiones` int(11) DEFAULT NULL,
  `margenTiempo` int(11) DEFAULT NULL,
  `tiempoDescanso` int(11) DEFAULT NULL,
  `duracionSesion` int(11) DEFAULT NULL,
  `paciente` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`codigo`),
  UNIQUE KEY `codigo_UNIQUE` (`codigo`),
  KEY `paciente-configuracion` (`paciente`),
  CONSTRAINT `paciente-configuracion` FOREIGN KEY (`paciente`) REFERENCES `paciente` (`codigo`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=15 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `ejercicio`
--

DROP TABLE IF EXISTS `ejercicio`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `ejercicio` (
  `codigo` int(11) NOT NULL AUTO_INCREMENT,
  `tiempoReaccion` bigint(20) DEFAULT NULL,
  `sesion` int(11) DEFAULT NULL,
  `tiempoInicio` bigint(20) DEFAULT NULL,
  `tiempoFinal` bigint(20) DEFAULT NULL,
  `esAcierto` tinyint(1) DEFAULT NULL,
  PRIMARY KEY (`codigo`),
  KEY `sesion-ejercicio` (`sesion`),
  CONSTRAINT `sesion-ejercicio` FOREIGN KEY (`sesion`) REFERENCES `sesion` (`codigo`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=168 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `paciente`
--

DROP TABLE IF EXISTS `paciente`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `paciente` (
  `codigo` varchar(100) NOT NULL,
  `borrado` tinyint(1) DEFAULT '0',
  PRIMARY KEY (`codigo`),
  UNIQUE KEY `codigo_UNIQUE` (`codigo`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `posicion`
--

DROP TABLE IF EXISTS `posicion`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `posicion` (
  `codigo` int(11) NOT NULL AUTO_INCREMENT,
  `instanteTiempo` bigint(20) DEFAULT NULL,
  `extremidad` varchar(45) DEFAULT NULL,
  `x` float DEFAULT NULL,
  `y` float DEFAULT NULL,
  `z` float DEFAULT NULL,
  `ejercicio` int(11) DEFAULT NULL,
  PRIMARY KEY (`codigo`),
  KEY `posicion-ejercicio` (`ejercicio`),
  CONSTRAINT `posicion-ejercicio` FOREIGN KEY (`ejercicio`) REFERENCES `ejercicio` (`codigo`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=509 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `sesion`
--

DROP TABLE IF EXISTS `sesion`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `sesion` (
  `codigo` int(11) NOT NULL AUTO_INCREMENT,
  `fallos` int(11) NOT NULL,
  `aciertos` int(11) NOT NULL,
  `paciente` varchar(100) DEFAULT NULL,
  `tiempoInicio` bigint(20) DEFAULT NULL,
  `tiempoFinal` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`codigo`),
  KEY `paciente-sesion` (`paciente`),
  CONSTRAINT `paciente-sesion` FOREIGN KEY (`paciente`) REFERENCES `paciente` (`codigo`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=14 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2012-04-16 15:56:37
