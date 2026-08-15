-- MySQL dump 10.13  Distrib 8.0.46, for Win64 (x86_64)
--
-- Host: localhost    Database: sathospitalario
-- ------------------------------------------------------
-- Server version	8.4.9

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `__efmigrationshistory`
--

DROP TABLE IF EXISTS `__efmigrationshistory`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `__efmigrationshistory` (
  `MigrationId` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ProductVersion` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`MigrationId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `areasclinicas`
--

DROP TABLE IF EXISTS `areasclinicas`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `areasclinicas` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `SedeId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `Codigo` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Nombre` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Activo` tinyint(1) NOT NULL,
  `EsAreaAdmision` tinyint(1) NOT NULL DEFAULT '0',
  `Estado` int NOT NULL DEFAULT '0',
  `ServicioTarifaBaseId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_AreasClinicas_SedeId_Codigo` (`SedeId`,`Codigo`),
  KEY `IX_AreasClinicas_ServicioTarifaBaseId` (`ServicioTarifaBaseId`),
  CONSTRAINT `FK_AreasClinicas_Sedes_SedeId` FOREIGN KEY (`SedeId`) REFERENCES `sedes` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_AreasClinicas_ServiciosClinicos_ServicioTarifaBaseId` FOREIGN KEY (`ServicioTarifaBaseId`) REFERENCES `serviciosclinicos` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `auditlogsprecios`
--

DROP TABLE IF EXISTS `auditlogsprecios`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `auditlogsprecios` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `DetalleServicioId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `DescripcionServicio` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `PrecioOriginal` decimal(18,2) NOT NULL,
  `PrecioModificado` decimal(18,2) NOT NULL,
  `HonorarioAnterior` decimal(65,30) NOT NULL,
  `NuevoHonorario` decimal(65,30) NOT NULL,
  `UsuarioOperador` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `AutorizadoPor` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `FechaModificacion` datetime(6) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `bloqueoshorarios`
--

DROP TABLE IF EXISTS `bloqueoshorarios`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `bloqueoshorarios` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `MedicoId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `HoraPautada` datetime(6) NOT NULL,
  `Motivo` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `FechaRegistro` datetime(6) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_BloqueosHorarios_MedicoId_HoraPautada` (`MedicoId`,`HoraPautada`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `cajasdiarias`
--

DROP TABLE IF EXISTS `cajasdiarias`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `cajasdiarias` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `FechaApertura` datetime(6) NOT NULL,
  `FechaCierre` datetime(6) DEFAULT NULL,
  `MontoInicialDivisa` decimal(18,2) NOT NULL,
  `MontoInicialBs` decimal(18,2) NOT NULL,
  `Estado` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `UsuarioId` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `NombreUsuario` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `DeclaracionCierreJson` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Diferencia` decimal(18,2) DEFAULT NULL,
  `TotalCobrado` decimal(18,2) DEFAULT NULL,
  `TotalIngresado` decimal(18,2) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `catalogometodospago`
--

DROP TABLE IF EXISTS `catalogometodospago`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `catalogometodospago` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `Nombre` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Valor` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `EsUSD` tinyint(1) NOT NULL,
  `EsVuelto` tinyint(1) NOT NULL,
  `Activo` tinyint(1) NOT NULL,
  `Orden` int NOT NULL,
  `GrupoMoneda` int NOT NULL DEFAULT '1',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_CatalogoMetodosPago_Valor` (`Valor`),
  KEY `IX_CatalogoMetodosPago_GrupoMoneda` (`GrupoMoneda`),
  CONSTRAINT `FK_CatalogoMetodosPago_Monedas_GrupoMoneda` FOREIGN KEY (`GrupoMoneda`) REFERENCES `monedas` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `cierresinventario`
--

DROP TABLE IF EXISTS `cierresinventario`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `cierresinventario` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `FechaCierre` datetime(6) NOT NULL,
  `Usuario` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Observaciones` varchar(1000) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `SedeId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000',
  PRIMARY KEY (`Id`),
  KEY `IX_CierresInventario_SedeId` (`SedeId`),
  CONSTRAINT `FK_CierresInventario_Sedes_SedeId` FOREIGN KEY (`SedeId`) REFERENCES `sedes` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `cierresinventariodetalles`
--

DROP TABLE IF EXISTS `cierresinventariodetalles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `cierresinventariodetalles` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `CierreInventarioId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `InsumoId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `StockTeoricoBase` decimal(18,4) NOT NULL,
  `StockRealBase` decimal(18,4) NOT NULL,
  `CostoBaseUSD` decimal(18,4) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_CierresInventarioDetalles_CierreInventarioId` (`CierreInventarioId`),
  KEY `IX_CierresInventarioDetalles_InsumoId` (`InsumoId`),
  CONSTRAINT `FK_CierresInventarioDetalles_CierresInventario_CierreInventario~` FOREIGN KEY (`CierreInventarioId`) REFERENCES `cierresinventario` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_CierresInventarioDetalles_Insumos_InsumoId` FOREIGN KEY (`InsumoId`) REFERENCES `insumos` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `cirugialogs`
--

DROP TABLE IF EXISTS `cirugialogs`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `cirugialogs` (
  `Id` char(36) NOT NULL,
  `OrdenCirugiaId` char(36) NOT NULL,
  `UsuarioId` longtext NOT NULL,
  `Evento` longtext NOT NULL,
  `Detalle` longtext NOT NULL,
  `Timestamp` datetime(6) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `citasmedicas`
--

DROP TABLE IF EXISTS `citasmedicas`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `citasmedicas` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `MedicoId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `PacienteId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `CuentaServicioId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `HoraPautada` datetime(6) NOT NULL,
  `Estado` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Comentario` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `FechaRegistro` datetime(6) NOT NULL,
  `AreaClinicaId` char(36) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_CitasMedicas_CuentaServicioId` (`CuentaServicioId`),
  KEY `IX_CitasMedicas_HoraPautada` (`HoraPautada`),
  KEY `IX_CitasMedicas_MedicoId` (`MedicoId`),
  CONSTRAINT `FK_CitasMedicas_CuentasServicios_CuentaServicioId` FOREIGN KEY (`CuentaServicioId`) REFERENCES `cuentasservicios` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_CitasMedicas_Medicos_MedicoId` FOREIGN KEY (`MedicoId`) REFERENCES `medicos` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `compromisospago`
--

DROP TABLE IF EXISTS `compromisospago`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `compromisospago` (
  `Id` char(36) NOT NULL,
  `CuentaPorCobrarId` char(36) NOT NULL,
  `Omitido` tinyint(1) NOT NULL,
  `Observacion` longtext,
  `UsuarioCreacion` longtext NOT NULL,
  `FechaCreacion` datetime(6) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `configuraciongeneral`
--

DROP TABLE IF EXISTS `configuraciongeneral`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `configuraciongeneral` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `NombreEmpresa` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Rif` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Iva` decimal(5,2) NOT NULL,
  `ClaveSupervisor` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `FacturarLaboratorio` tinyint(1) NOT NULL,
  `MostrarDetalleFacturacion` tinyint(1) NOT NULL,
  `UltimaActualizacion` datetime(6) NOT NULL,
  `LogoBase64` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `consumosserviciosrealizados`
--

DROP TABLE IF EXISTS `consumosserviciosrealizados`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `consumosserviciosrealizados` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `DetalleServicioCuentaId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `InsumoId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `CantidadConsumidaBase` decimal(18,4) NOT NULL,
  `CostoTotalUSD` decimal(18,4) NOT NULL,
  `FechaConsumo` datetime(6) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_ConsumosServiciosRealizados_DetalleServicioCuentaId` (`DetalleServicioCuentaId`),
  KEY `IX_ConsumosServiciosRealizados_InsumoId` (`InsumoId`),
  CONSTRAINT `FK_ConsumosServiciosRealizados_DetallesServicioCuenta_DetalleSe~` FOREIGN KEY (`DetalleServicioCuentaId`) REFERENCES `detallesserviciocuenta` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_ConsumosServiciosRealizados_Insumos_InsumoId` FOREIGN KEY (`InsumoId`) REFERENCES `insumos` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `convenioperfilprecios`
--

DROP TABLE IF EXISTS `convenioperfilprecios`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `convenioperfilprecios` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `SeguroConvenioId` int NOT NULL,
  `PerfilId` int NOT NULL,
  `PrecioHNL` decimal(18,2) NOT NULL,
  `PrecioUSD` decimal(18,2) NOT NULL,
  `Activo` tinyint(1) NOT NULL,
  `UltimaActualizacion` datetime(6) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_ConvenioPerfilPrecios_SeguroConvenioId_PerfilId` (`SeguroConvenioId`,`PerfilId`),
  CONSTRAINT `FK_ConvenioPerfilPrecios_SegurosConvenios_SeguroConvenioId` FOREIGN KEY (`SeguroConvenioId`) REFERENCES `segurosconvenios` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `cuentasporcobrar`
--

DROP TABLE IF EXISTS `cuentasporcobrar`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `cuentasporcobrar` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `CuentaServicioId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `PacienteId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `MontoTotalBase` decimal(18,2) NOT NULL,
  `MontoPagadoBase` decimal(18,2) NOT NULL,
  `FechaCreacion` datetime(6) NOT NULL,
  `Estado` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `IsAudited` tinyint(1) NOT NULL,
  `CompromisoGenerado` tinyint(1) NOT NULL DEFAULT '0',
  `FechaAuditoria` datetime(6) DEFAULT NULL,
  `UsuarioAuditoria` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `GarantiaGenerada` tinyint(1) NOT NULL DEFAULT '0',
  `DoctorProcedimiento` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `InformacionAdicional` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `QuienAutorizo` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`Id`),
  KEY `IX_CuentasPorCobrar_CuentaServicioId` (`CuentaServicioId`),
  CONSTRAINT `FK_CuentasPorCobrar_CuentasServicios_CuentaServicioId` FOREIGN KEY (`CuentaServicioId`) REFERENCES `cuentasservicios` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `cuentasservicios`
--

DROP TABLE IF EXISTS `cuentasservicios`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `cuentasservicios` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `PacienteId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `UsuarioCarga` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `FechaCarga` datetime(6) NOT NULL,
  `FechaCierre` datetime(6) DEFAULT NULL,
  `Estado` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `TipoIngreso` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ConvenioId` int DEFAULT NULL,
  `FechaAuditoria` datetime(6) DEFAULT NULL,
  `FechaValidacion` datetime(6) DEFAULT NULL,
  `UsuarioAuditoria` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `UsuarioValidacion` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `LegacyOrderId` int DEFAULT NULL,
  `ProcesamientoEstado` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `CuentaPrincipalId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `DestinoPaciente` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `PersonalRelevo` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `AreaClinicaId` char(36) DEFAULT NULL,
  `SubAreaClinica` varchar(100) DEFAULT NULL,
  `MedicoId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `CamaRetenidaId` char(36) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_CuentasServicios_FechaCarga` (`FechaCarga`),
  KEY `IX_CuentasServicios_PacienteId` (`PacienteId`),
  KEY `IX_CuentasServicios_ConvenioId` (`ConvenioId`),
  KEY `IX_CuentasServicios_CuentaPrincipalId` (`CuentaPrincipalId`),
  KEY `IX_CuentasServicios_MedicoId` (`MedicoId`),
  CONSTRAINT `FK_CuentasServicios_CuentasServicios_CuentaPrincipalId` FOREIGN KEY (`CuentaPrincipalId`) REFERENCES `cuentasservicios` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_CuentasServicios_Medicos_MedicoId` FOREIGN KEY (`MedicoId`) REFERENCES `medicos` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_CuentasServicios_PacientesAdmision_PacienteId` FOREIGN KEY (`PacienteId`) REFERENCES `pacientesadmision` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_CuentasServicios_SegurosConvenios_ConvenioId` FOREIGN KEY (`ConvenioId`) REFERENCES `segurosconvenios` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `detallespago`
--

DROP TABLE IF EXISTS `detallespago`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `detallespago` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `ReciboFacturaId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `MetodoPago` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ReferenciaBancaria` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `MontoAbonadoMoneda` decimal(18,2) NOT NULL,
  `EquivalenteAbonadoBase` decimal(18,2) NOT NULL,
  `FechaPago` datetime(6) NOT NULL,
  `UsuarioCarga` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `TasaCambioAplicada` decimal(18,4) NOT NULL DEFAULT '0.0000',
  PRIMARY KEY (`Id`),
  KEY `IX_DetallesPago_FechaPago` (`FechaPago`),
  KEY `IX_DetallesPago_ReciboFacturaId` (`ReciboFacturaId`),
  CONSTRAINT `FK_DetallesPago_RecibosFacturas_ReciboFacturaId` FOREIGN KEY (`ReciboFacturaId`) REFERENCES `recibosfacturas` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `detallesserviciocuenta`
--

DROP TABLE IF EXISTS `detallesserviciocuenta`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `detallesserviciocuenta` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `CuentaServicioId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `ServicioId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `Descripcion` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Precio` decimal(18,2) NOT NULL,
  `Honorario` decimal(65,30) NOT NULL,
  `Cantidad` decimal(18,4) NOT NULL,
  `TipoServicio` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `UsuarioCarga` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `FechaCarga` datetime(6) NOT NULL,
  `LegacyMappingId` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Realizado` tinyint(1) NOT NULL,
  `FechaRealizacion` datetime(6) DEFAULT NULL,
  `UsuarioTecnico` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `CategoriaHonorario` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `MedicoResponsableId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `AreaClinicaId` char(36) DEFAULT NULL,
  `TipoServicioId` int NOT NULL DEFAULT '0',
  `IncluidoEnTarifaBase` tinyint(1) NOT NULL DEFAULT '0',
  `PrecioCatalogoHistorico` decimal(18,2) NOT NULL DEFAULT '0.00',
  `DetallePadreId` char(36) DEFAULT NULL,
  `UsuarioCargaId` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_DetallesServicioCuenta_CuentaServicioId` (`CuentaServicioId`),
  KEY `IX_DetallesServicioCuenta_MedicoResponsableId` (`MedicoResponsableId`),
  KEY `IX_DetallesServicioCuenta_TipoServicioId` (`TipoServicioId`),
  CONSTRAINT `FK_DetallesServicioCuenta_CuentasServicios_CuentaServicioId` FOREIGN KEY (`CuentaServicioId`) REFERENCES `cuentasservicios` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_DetallesServicioCuenta_Medicos_MedicoResponsableId` FOREIGN KEY (`MedicoResponsableId`) REFERENCES `medicos` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_DetallesServicioCuenta_TiposServicio_TipoServicioId` FOREIGN KEY (`TipoServicioId`) REFERENCES `tiposservicio` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `detallesserviciosmedicosresponsables`
--

DROP TABLE IF EXISTS `detallesserviciosmedicosresponsables`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `detallesserviciosmedicosresponsables` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `DetalleServicioCuentaId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `MedicoId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `Rol` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `MontoHonorario` decimal(18,2) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_DetallesServiciosMedicosResponsables_DetalleServicioCuentaId` (`DetalleServicioCuentaId`),
  KEY `IX_DetallesServiciosMedicosResponsables_MedicoId` (`MedicoId`),
  CONSTRAINT `FK_DetallesServiciosMedicosResponsables_DetallesServicioCuenta_~` FOREIGN KEY (`DetalleServicioCuentaId`) REFERENCES `detallesserviciocuenta` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_DetallesServiciosMedicosResponsables_Medicos_MedicoId` FOREIGN KEY (`MedicoId`) REFERENCES `medicos` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `documentlogs`
--

DROP TABLE IF EXISTS `documentlogs`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `documentlogs` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `DocumentType` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ReferenceId` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Action` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `UserId` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `UserName` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Timestamp` datetime(6) NOT NULL,
  `Details` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`Id`),
  KEY `IX_DocumentLogs_ReferenceId` (`ReferenceId`),
  KEY `IX_DocumentLogs_Timestamp` (`Timestamp`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `errortickets`
--

DROP TABLE IF EXISTS `errortickets`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `errortickets` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `RequestPath` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `MetodoHTTP` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `MensajeExcepcion` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `StackTrace` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `UsuarioAsociado` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `FechaCreacion` datetime(6) NOT NULL,
  `Resuelto` tinyint(1) NOT NULL,
  `ComentariosResolucion` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `FechaResolucion` datetime(6) DEFAULT NULL,
  `ResueltoPor` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `especialidades`
--

DROP TABLE IF EXISTS `especialidades`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `especialidades` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `Nombre` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Activo` tinyint(1) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `garantiasitems`
--

DROP TABLE IF EXISTS `garantiasitems`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `garantiasitems` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `CuentaPorCobrarId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `Descripcion` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ValorEstimado` decimal(18,2) NOT NULL,
  `FechaRegistro` datetime(6) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_GarantiasItems_CuentaPorCobrarId` (`CuentaPorCobrarId`),
  CONSTRAINT `FK_GarantiasItems_CuentasPorCobrar_CuentaPorCobrarId` FOREIGN KEY (`CuentaPorCobrarId`) REFERENCES `cuentasporcobrar` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `historialeslimpiezascamas`
--

DROP TABLE IF EXISTS `historialeslimpiezascamas`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `historialeslimpiezascamas` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `CamaId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `FechaInicio` datetime(6) NOT NULL,
  `FechaFin` datetime(6) DEFAULT NULL,
  `UsuarioInicio` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `UsuarioFin` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Observaciones` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`Id`),
  KEY `IX_HistorialesLimpiezasCamas_CamaId_FechaFin` (`CamaId`,`FechaFin`),
  KEY `IX_HistorialesLimpiezasCamas_FechaFin` (`FechaFin`),
  CONSTRAINT `FK_HistorialesLimpiezasCamas_AreasClinicas_CamaId` FOREIGN KEY (`CamaId`) REFERENCES `areasclinicas` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `historialmodificacioncuentas`
--

DROP TABLE IF EXISTS `historialmodificacioncuentas`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `historialmodificacioncuentas` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `CuentaServicioId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `FechaModificacion` datetime(6) NOT NULL,
  `Usuario` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `PacienteAnteriorId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `PacienteAnteriorNombre` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `PacienteNuevoId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `PacienteNuevoNombre` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `TipoIngresoAnterior` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `TipoIngresoNuevo` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `ConvenioAnteriorId` int DEFAULT NULL,
  `ConvenioAnteriorNombre` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `ConvenioNuevoId` int DEFAULT NULL,
  `ConvenioNuevoNombre` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `TotalAnteriorUSD` decimal(18,2) NOT NULL,
  `TotalNuevoUSD` decimal(18,2) NOT NULL,
  `ReciboTotalAnteriorUSD` decimal(18,2) NOT NULL,
  `ReciboTotalNuevoUSD` decimal(18,2) NOT NULL,
  `ReciboVueltoAnteriorUSD` decimal(18,2) NOT NULL,
  `ReciboVueltoNuevoUSD` decimal(18,2) NOT NULL,
  `ReciboPagadoUSD` decimal(18,2) NOT NULL,
  `CxCSaldoAnteriorUSD` decimal(18,2) NOT NULL,
  `CxCSaldoNuevoUSD` decimal(18,2) NOT NULL,
  `DetalleServiciosCambiosJson` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`Id`),
  KEY `IX_HistorialModificacionCuentas_CuentaServicioId` (`CuentaServicioId`),
  KEY `IX_HistorialModificacionCuentas_FechaModificacion` (`FechaModificacion`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `honorariosconfig`
--

DROP TABLE IF EXISTS `honorariosconfig`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `honorariosconfig` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `CategoriaServicio` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `MedicoDefaultId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `UsuarioConfiguro` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `FechaConfiguracion` datetime(6) NOT NULL,
  `NotasConfig` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_HonorariosConfig_CategoriaServicio` (`CategoriaServicio`),
  KEY `IX_HonorariosConfig_MedicoDefaultId` (`MedicoDefaultId`),
  CONSTRAINT `FK_HonorariosConfig_Medicos_MedicoDefaultId` FOREIGN KEY (`MedicoDefaultId`) REFERENCES `medicos` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `honorariosmedicosservicios`
--

DROP TABLE IF EXISTS `honorariosmedicosservicios`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `honorariosmedicosservicios` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `ServicioId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `MedicoId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `MontoHonorario` decimal(18,2) NOT NULL,
  `UsuarioModifico` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `FechaModificacion` datetime(6) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_HonorariosMedicosServicios_MedicoId` (`MedicoId`),
  KEY `IX_HonorariosMedicosServicios_ServicioId` (`ServicioId`),
  CONSTRAINT `FK_HonorariosMedicosServicios_Medicos_MedicoId` FOREIGN KEY (`MedicoId`) REFERENCES `medicos` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_HonorariosMedicosServicios_ServiciosClinicos_ServicioId` FOREIGN KEY (`ServicioId`) REFERENCES `serviciosclinicos` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `honorariummappingrules`
--

DROP TABLE IF EXISTS `honorariummappingrules`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `honorariummappingrules` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `Pattern` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Category` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `MatchType` int NOT NULL,
  `Priority` int NOT NULL,
  `IsActive` tinyint(1) NOT NULL,
  `UsuarioCreo` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `FechaCreacion` datetime(6) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_HonorariumMappingRules_IsActive` (`IsActive`),
  KEY `IX_HonorariumMappingRules_Priority` (`Priority`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `horariosatencionmedicos`
--

DROP TABLE IF EXISTS `horariosatencionmedicos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `horariosatencionmedicos` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `MedicoId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `DiaSemana` int NOT NULL,
  `HoraInicio` time(6) NOT NULL,
  `HoraFin` time(6) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_HorariosAtencionMedicos_MedicoId` (`MedicoId`),
  CONSTRAINT `FK_HorariosAtencionMedicos_Medicos_MedicoId` FOREIGN KEY (`MedicoId`) REFERENCES `medicos` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `incidenciashorario`
--

DROP TABLE IF EXISTS `incidenciashorario`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `incidenciashorario` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `MedicoId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `Inicio` datetime(6) NOT NULL,
  `Fin` datetime(6) NOT NULL,
  `Tipo` int NOT NULL,
  `Descripcion` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `CreadoPor` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `FechaCreacion` datetime(6) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `insumos`
--

DROP TABLE IF EXISTS `insumos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `insumos` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `Codigo` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Nombre` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `UnidadMedidaBase` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `CostoUnitarioBaseUSD` decimal(18,4) NOT NULL,
  `Categoria` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT 'Medicamento',
  `PermiteFraccionamiento` tinyint(1) NOT NULL DEFAULT '1',
  `ReactivosCombinados` varchar(500) DEFAULT NULL,
  `Indicaciones` text,
  `FechaVencimiento` datetime DEFAULT NULL,
  `OcultoEnTraslados` tinyint(1) NOT NULL DEFAULT '0',
  `IsDeleted` tinyint(1) NOT NULL DEFAULT '0',
  `FechaInactivacion` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_Insumos_Codigo` (`Codigo`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `insumoscirugiaspacientes`
--

DROP TABLE IF EXISTS `insumoscirugiaspacientes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `insumoscirugiaspacientes` (
  `Id` char(36) NOT NULL,
  `CuentaServicioId` char(36) NOT NULL,
  `InsumoId` char(36) NOT NULL,
  `CantidadEntregada` decimal(18,2) NOT NULL,
  `CantidadDevuelta` decimal(18,2) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `insumosprincipiosactivos`
--

DROP TABLE IF EXISTS `insumosprincipiosactivos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `insumosprincipiosactivos` (
  `InsumoId` char(36) NOT NULL,
  `PrincipioActivoId` char(36) NOT NULL,
  `Concentracion` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`InsumoId`,`PrincipioActivoId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `logsasignacionhonorario`
--

DROP TABLE IF EXISTS `logsasignacionhonorario`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `logsasignacionhonorario` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `DetalleServicioId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `NombreServicio` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `TipoAccion` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `MedicoAnteriorId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `MedicoAnteriorNombre` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `MedicoNuevoId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `MedicoNuevoNombre` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `UsuarioOperador` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `FechaAccion` datetime(6) NOT NULL,
  `Observaciones` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`Id`),
  KEY `IX_LogsAsignacionHonorario_DetalleServicioId` (`DetalleServicioId`),
  KEY `IX_LogsAsignacionHonorario_FechaAccion` (`FechaAccion`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `medicos`
--

DROP TABLE IF EXISTS `medicos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `medicos` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `Nombre` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `EspecialidadId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `Activo` tinyint(1) NOT NULL,
  `HonorarioBase` decimal(18,2) NOT NULL,
  `IntervaloTurnoMinutos` int NOT NULL,
  `Telefono` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`Id`),
  KEY `IX_Medicos_EspecialidadId` (`EspecialidadId`),
  CONSTRAINT `FK_Medicos_Especialidades_EspecialidadId` FOREIGN KEY (`EspecialidadId`) REFERENCES `especialidades` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `monedas`
--

DROP TABLE IF EXISTS `monedas`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `monedas` (
  `Id` int NOT NULL,
  `Codigo` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Nombre` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Simbolo` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `EsBaseUsd` tinyint(1) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `movimientosinsumo`
--

DROP TABLE IF EXISTS `movimientosinsumo`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `movimientosinsumo` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `InsumoId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `TipoMovimiento` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `CantidadBase` decimal(18,4) NOT NULL,
  `UnidadMedidaOriginal` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `CantidadOriginal` decimal(18,4) NOT NULL,
  `Usuario` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Fecha` datetime(6) NOT NULL,
  `Motivo` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `SedeId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000',
  PRIMARY KEY (`Id`),
  KEY `IX_MovimientosInsumo_InsumoId` (`InsumoId`),
  KEY `IX_MovimientosInsumo_SedeId` (`SedeId`),
  CONSTRAINT `FK_MovimientosInsumo_Insumos_InsumoId` FOREIGN KEY (`InsumoId`) REFERENCES `insumos` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_MovimientosInsumo_Sedes_SedeId` FOREIGN KEY (`SedeId`) REFERENCES `sedes` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `notifications`
--

DROP TABLE IF EXISTS `notifications`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `notifications` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `Title` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Message` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Type` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `TargetUserId` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `TargetRole` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `IsRead` tinyint(1) NOT NULL,
  `Timestamp` datetime(6) NOT NULL,
  `ActionUrl` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`Id`),
  KEY `IX_Notifications_TargetRole` (`TargetRole`),
  KEY `IX_Notifications_TargetUserId` (`TargetUserId`),
  KEY `IX_Notifications_Timestamp` (`Timestamp`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `ordenescirugia`
--

DROP TABLE IF EXISTS `ordenescirugia`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ordenescirugia` (
  `Id` char(36) NOT NULL,
  `CuentaServicioId` char(36) NOT NULL,
  `PacienteId` char(36) NOT NULL,
  `DescripcionCirugia` longtext NOT NULL,
  `PrecioBaseUsd` decimal(18,2) NOT NULL,
  `MedicoId` char(36) NOT NULL,
  `FechaHoraProgramada` datetime(6) NOT NULL,
  `Estado` longtext NOT NULL,
  `MotivoCancelacion` longtext,
  `FechaCreacion` datetime(6) NOT NULL,
  `UsuarioCreacion` longtext NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `ordenescomprainventario`
--

DROP TABLE IF EXISTS `ordenescomprainventario`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ordenescomprainventario` (
  `Id` char(36) COLLATE utf8mb4_unicode_ci NOT NULL,
  `NumeroFactura` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `ProveedorNombre` varchar(250) COLLATE utf8mb4_unicode_ci NOT NULL,
  `ProveedorId` char(36) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `FechaEmision` datetime NOT NULL,
  `MontoTotalUSD` decimal(18,2) NOT NULL,
  `MontoTotalBs` decimal(18,2) NOT NULL,
  `TotalAbonadoUSD` decimal(18,2) NOT NULL,
  `SaldoPendienteUSD` decimal(18,2) NOT NULL,
  `Estado` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'PorPagar',
  `Observaciones` varchar(1000) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_OrdenesCompraInventario_NumeroFactura` (`NumeroFactura`),
  KEY `IX_OrdenesCompraInventario_ProveedorNombre` (`ProveedorNombre`),
  KEY `IX_OrdenesCompraInventario_Estado` (`Estado`),
  KEY `FK_OrdenesCompraInventario_Proveedores_ProveedorId` (`ProveedorId`),
  CONSTRAINT `FK_OrdenesCompraInventario_Proveedores_ProveedorId` FOREIGN KEY (`ProveedorId`) REFERENCES `proveedores` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `ordenesdeservicio`
--

DROP TABLE IF EXISTS `ordenesdeservicio`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ordenesdeservicio` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `NumeroLlegadaDiario` int NOT NULL,
  `PacienteId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `NombrePaciente` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `TipoIngreso` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `EstadoFacturacion` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `TotalCobrado` decimal(18,2) NOT NULL,
  `FechaCreacion` datetime(6) NOT NULL,
  `ConvenioId` int DEFAULT NULL,
  `Discriminator` varchar(21) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `EstudioSolicitado` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Procesada` tinyint(1) DEFAULT NULL,
  `FechaProcesada` datetime(6) DEFAULT NULL,
  `AsistenteRxId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_OrdenesDeServicio_PacienteId` (`PacienteId`),
  CONSTRAINT `FK_OrdenesDeServicio_PacientesAdmision_PacienteId` FOREIGN KEY (`PacienteId`) REFERENCES `pacientesadmision` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `ordenesimagenes`
--

DROP TABLE IF EXISTS `ordenesimagenes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ordenesimagenes` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `CuentaId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `PacienteId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `PacienteNombre` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Estudio` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `TipoServicio` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Estado` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `FechaCreacion` datetime(6) NOT NULL,
  `ProcesadoPor` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `FechaProcesado` datetime(6) DEFAULT NULL,
  `EsDirecta` tinyint(1) NOT NULL DEFAULT '0',
  `FechaValidacion` datetime(6) DEFAULT NULL,
  `MedicoSolicitanteId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `MedicoSolicitanteNombre` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `RequiereValidacion` tinyint(1) NOT NULL DEFAULT '0',
  `Validada` tinyint(1) NOT NULL DEFAULT '0',
  `ValidadorPor` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Informe` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `LinkInforme` varchar(1000) DEFAULT NULL,
  `ObservacionesMedico` varchar(2000) DEFAULT NULL,
  `MedicoInterpreteId` char(36) DEFAULT NULL,
  `RequiereInforme` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `IX_OrdenesImagenes_Estado` (`Estado`),
  KEY `IX_OrdenesImagenes_TipoServicio` (`TipoServicio`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `pacientesadmision`
--

DROP TABLE IF EXISTS `pacientesadmision`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `pacientesadmision` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `IdPacienteLegacy` int DEFAULT NULL,
  `CedulaPasaporte` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `NombreCorto` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `TelefonoContact` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `FechaNacimiento` datetime(6) DEFAULT NULL,
  `Direccion` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_PacientesAdmision_CedulaPasaporte` (`CedulaPasaporte`),
  UNIQUE KEY `IX_PacientesAdmision_IdPacienteLegacy` (`IdPacienteLegacy`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `pagosproveedores`
--

DROP TABLE IF EXISTS `pagosproveedores`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `pagosproveedores` (
  `Id` char(36) COLLATE utf8mb4_unicode_ci NOT NULL,
  `OrdenCompraId` char(36) COLLATE utf8mb4_unicode_ci NOT NULL,
  `FechaPago` datetime NOT NULL,
  `MontoAbonadoUSD` decimal(18,2) NOT NULL,
  `TasaCambio` decimal(18,4) NOT NULL,
  `MontoAbonadoBs` decimal(18,2) NOT NULL,
  `MetodoPago` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Referencia` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `UsuarioId` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Observaciones` varchar(1000) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_PagosProveedores_OrdenCompraId` (`OrdenCompraId`),
  KEY `IX_PagosProveedores_FechaPago` (`FechaPago`),
  CONSTRAINT `FK_PagosProveedores_OrdenesCompraInventario_OrdenCompraId` FOREIGN KEY (`OrdenCompraId`) REFERENCES `ordenescomprainventario` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `pedidosintersede`
--

DROP TABLE IF EXISTS `pedidosintersede`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `pedidosintersede` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `Correlativo` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `SedeSolicitanteId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `SedeProveedoraId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `Estado` int NOT NULL,
  `FechaCreacion` datetime(6) NOT NULL,
  `FechaDespacho` datetime(6) DEFAULT NULL,
  `FechaRecepcion` datetime(6) DEFAULT NULL,
  `UsuarioCreador` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Observaciones` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_PedidosInterSede_Correlativo` (`Correlativo`),
  KEY `IX_PedidosInterSede_SedeProveedoraId` (`SedeProveedoraId`),
  KEY `IX_PedidosInterSede_SedeSolicitanteId` (`SedeSolicitanteId`),
  CONSTRAINT `FK_PedidosInterSede_Sedes_SedeProveedoraId` FOREIGN KEY (`SedeProveedoraId`) REFERENCES `sedes` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_PedidosInterSede_Sedes_SedeSolicitanteId` FOREIGN KEY (`SedeSolicitanteId`) REFERENCES `sedes` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `pedidosintersededetalles`
--

DROP TABLE IF EXISTS `pedidosintersededetalles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `pedidosintersededetalles` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `PedidoInterSedeId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `InsumoId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `CantidadSolicitada` decimal(18,4) NOT NULL,
  `CantidadDespachada` decimal(18,4) NOT NULL,
  `CantidadRecibida` decimal(18,4) NOT NULL,
  `ObservacionDespacho` varchar(500) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_PedidosInterSedeDetalles_InsumoId` (`InsumoId`),
  KEY `IX_PedidosInterSedeDetalles_PedidoInterSedeId` (`PedidoInterSedeId`),
  CONSTRAINT `FK_PedidosInterSedeDetalles_Insumos_InsumoId` FOREIGN KEY (`InsumoId`) REFERENCES `insumos` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_PedidosInterSedeDetalles_PedidosInterSede_PedidoInterSedeId` FOREIGN KEY (`PedidoInterSedeId`) REFERENCES `pedidosintersede` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `preciosservicioconvenio`
--

DROP TABLE IF EXISTS `preciosservicioconvenio`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `preciosservicioconvenio` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `ServicioClinicoId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `SeguroConvenioId` int NOT NULL,
  `PrecioDiferencial` decimal(18,2) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_PreciosServicioConvenio_SeguroConvenioId` (`SeguroConvenioId`),
  KEY `IX_PreciosServicioConvenio_ServicioClinicoId` (`ServicioClinicoId`),
  CONSTRAINT `FK_PreciosServicioConvenio_SegurosConvenios_SeguroConvenioId` FOREIGN KEY (`SeguroConvenioId`) REFERENCES `segurosconvenios` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_PreciosServicioConvenio_ServiciosClinicos_ServicioClinicoId` FOREIGN KEY (`ServicioClinicoId`) REFERENCES `serviciosclinicos` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `principiosactivos`
--

DROP TABLE IF EXISTS `principiosactivos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `principiosactivos` (
  `Id` char(36) NOT NULL,
  `Nombre` varchar(200) NOT NULL,
  `Activo` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_PrincipiosActivos_Nombre` (`Nombre`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `proveedores`
--

DROP TABLE IF EXISTS `proveedores`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `proveedores` (
  `Id` char(36) COLLATE utf8mb4_unicode_ci NOT NULL,
  `RIF` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `RazonSocial` varchar(250) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Direccion` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Telefono` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Activo` tinyint(1) NOT NULL DEFAULT '1',
  `FechaRegistro` datetime NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_Proveedores_RIF` (`RIF`),
  KEY `IX_Proveedores_RazonSocial` (`RazonSocial`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `recibosfacturas`
--

DROP TABLE IF EXISTS `recibosfacturas`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `recibosfacturas` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `CuentaServicioId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `CajaDiariaId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `NroControlFiscal` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `TasaCambioDia` decimal(18,4) NOT NULL,
  `EstadoFiscal` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `PacienteId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `NumeroRecibo` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `TotalFacturadoUSD` decimal(65,30) NOT NULL,
  `MontoVueltoUSD` decimal(65,30) NOT NULL,
  `FechaEmision` datetime(6) NOT NULL,
  `UsuarioEmision` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `NumeroComprobante` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_RecibosFacturas_CajaDiariaId` (`CajaDiariaId`),
  KEY `IX_RecibosFacturas_CuentaServicioId` (`CuentaServicioId`),
  CONSTRAINT `FK_RecibosFacturas_CajasDiarias_CajaDiariaId` FOREIGN KEY (`CajaDiariaId`) REFERENCES `cajasdiarias` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_RecibosFacturas_CuentasServicios_CuentaServicioId` FOREIGN KEY (`CuentaServicioId`) REFERENCES `cuentasservicios` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `registroauditoriaincidencias`
--

DROP TABLE IF EXISTS `registroauditoriaincidencias`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `registroauditoriaincidencias` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `TurnoMedicoId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `IncidenciaIgnoradaId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `OperadorId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `FechaTraza` datetime(6) NOT NULL,
  `Motivo` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `reservastemporales`
--

DROP TABLE IF EXISTS `reservastemporales`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `reservastemporales` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `MedicoId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `HoraPautada` datetime(6) NOT NULL,
  `UsuarioId` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Comentario` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `ExpiracionUtc` datetime(6) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_ReservasTemporales_MedicoId_HoraPautada` (`MedicoId`,`HoraPautada`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `sedes`
--

DROP TABLE IF EXISTS `sedes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `sedes` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `Codigo` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Nombre` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `EsPrincipal` tinyint(1) NOT NULL,
  `Activo` tinyint(1) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_Sedes_Codigo` (`Codigo`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `segurosconvenios`
--

DROP TABLE IF EXISTS `segurosconvenios`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `segurosconvenios` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Nombre` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Rtn` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Direccion` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Telefono` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Email` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Activo` tinyint(1) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `serviciosclinicos`
--

DROP TABLE IF EXISTS `serviciosclinicos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `serviciosclinicos` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `Codigo` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Descripcion` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `PrecioBase` decimal(18,2) NOT NULL,
  `HonorarioBase` decimal(65,30) NOT NULL,
  `TipoServicio` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `LegacyMappingId` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Category` int NOT NULL,
  `Activo` tinyint(1) NOT NULL,
  `EspecialidadId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `HonorariumCategory` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `PermiteFraccionamiento` tinyint(1) NOT NULL DEFAULT '0',
  `UnidadMedida` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `RequiereInventario` tinyint(1) NOT NULL DEFAULT '0',
  `TipoServicioId` int NOT NULL DEFAULT '0',
  `ServicioInformeId` char(36) DEFAULT NULL,
  `EsServicioInforme` tinyint(1) NOT NULL DEFAULT '0',
  `DesactivadoPorUsuarioId` varchar(255) DEFAULT NULL,
  `FechaDesactivacion` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_ServiciosClinicos_EspecialidadId` (`EspecialidadId`),
  KEY `IX_ServiciosClinicos_TipoServicioId` (`TipoServicioId`),
  CONSTRAINT `FK_ServiciosClinicos_Especialidades_EspecialidadId` FOREIGN KEY (`EspecialidadId`) REFERENCES `especialidades` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_ServiciosClinicos_TiposServicio_TipoServicioId` FOREIGN KEY (`TipoServicioId`) REFERENCES `tiposservicio` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `serviciosincluidosareas`
--

DROP TABLE IF EXISTS `serviciosincluidosareas`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `serviciosincluidosareas` (
  `AreaClinicaId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `ServicioClinicoId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `Activo` tinyint(1) NOT NULL,
  PRIMARY KEY (`AreaClinicaId`,`ServicioClinicoId`),
  KEY `IX_ServiciosIncluidosAreas_ServicioClinicoId` (`ServicioClinicoId`),
  CONSTRAINT `FK_ServiciosIncluidosAreas_AreasClinicas_AreaClinicaId` FOREIGN KEY (`AreaClinicaId`) REFERENCES `areasclinicas` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ServiciosIncluidosAreas_ServiciosClinicos_ServicioClinicoId` FOREIGN KEY (`ServicioClinicoId`) REFERENCES `serviciosclinicos` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `serviciosinsumorecetas`
--

DROP TABLE IF EXISTS `serviciosinsumorecetas`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `serviciosinsumorecetas` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `ServicioClinicoId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `ServicioCodigo` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `InsumoId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `Cantidad` decimal(18,4) NOT NULL,
  `UnidadMedidaConsumo` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_ServiciosInsumoRecetas_InsumoId` (`InsumoId`),
  KEY `IX_ServiciosInsumoRecetas_ServicioClinicoId` (`ServicioClinicoId`),
  CONSTRAINT `FK_ServiciosInsumoRecetas_Insumos_InsumoId` FOREIGN KEY (`InsumoId`) REFERENCES `insumos` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_ServiciosInsumoRecetas_ServiciosClinicos_ServicioClinicoId` FOREIGN KEY (`ServicioClinicoId`) REFERENCES `serviciosclinicos` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `serviciossugerencias`
--

DROP TABLE IF EXISTS `serviciossugerencias`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `serviciossugerencias` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `ServicioOrigenId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `ServicioSugeridoId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_serviciossugerencias_ServicioOrigenId` (`ServicioOrigenId`),
  KEY `IX_serviciossugerencias_ServicioSugeridoId` (`ServicioSugeridoId`),
  CONSTRAINT `FK_serviciossugerencias_ServiciosClinicos_ServicioOrigenId` FOREIGN KEY (`ServicioOrigenId`) REFERENCES `serviciosclinicos` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_serviciossugerencias_ServiciosClinicos_ServicioSugeridoId` FOREIGN KEY (`ServicioSugeridoId`) REFERENCES `serviciosclinicos` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `stockssede`
--

DROP TABLE IF EXISTS `stockssede`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `stockssede` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `InsumoId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `SedeId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `StockActual` decimal(18,4) NOT NULL,
  `StockMinimo` decimal(18,4) DEFAULT NULL,
  `StockMaximo` decimal(18,4) DEFAULT NULL,
  `RowVersion` datetime(6) DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_StocksSede_SedeId_InsumoId` (`SedeId`,`InsumoId`),
  KEY `IX_StocksSede_InsumoId` (`InsumoId`),
  CONSTRAINT `FK_StocksSede_Insumos_InsumoId` FOREIGN KEY (`InsumoId`) REFERENCES `insumos` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_StocksSede_Sedes_SedeId` FOREIGN KEY (`SedeId`) REFERENCES `sedes` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tasacambio`
--

DROP TABLE IF EXISTS `tasacambio`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `tasacambio` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `Fecha` datetime(6) NOT NULL,
  `Monto` decimal(18,4) NOT NULL,
  `Activo` tinyint(1) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tiposservicio`
--

DROP TABLE IF EXISTS `tiposservicio`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `tiposservicio` (
  `Id` int NOT NULL,
  `Nombre` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Codigo` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `triagesenfermeria`
--

DROP TABLE IF EXISTS `triagesenfermeria`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `triagesenfermeria` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `CuentaServicioId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `MotivoConsulta` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `TensionArterial` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `FrecuenciaCardiaca` int NOT NULL,
  `FrecuenciaRespiratoria` int NOT NULL,
  `Temperatura` decimal(4,2) NOT NULL,
  `SaturacionO2` int NOT NULL,
  `GlicemiaCapilar` int DEFAULT NULL,
  `FechaRegistro` datetime(6) NOT NULL,
  `UsuarioRegistro` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `DescripcionDetallada` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `DescripcionRapida` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`Id`),
  KEY `IX_TriagesEnfermeria_CuentaServicioId` (`CuentaServicioId`),
  KEY `IX_TriagesEnfermeria_FechaRegistro` (`FechaRegistro`),
  CONSTRAINT `FK_TriagesEnfermeria_CuentasServicios_CuentaServicioId` FOREIGN KEY (`CuentaServicioId`) REFERENCES `cuentasservicios` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `turnosmedicos`
--

DROP TABLE IF EXISTS `turnosmedicos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `turnosmedicos` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `MedicoId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `PacienteId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `FechaHoraToma` datetime(6) NOT NULL,
  `IgnorandoIncidencia` tinyint(1) NOT NULL,
  `IncidenciaIgnoradaId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `valoracionesfisicas`
--

DROP TABLE IF EXISTS `valoracionesfisicas`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `valoracionesfisicas` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `CuentaServicioId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `EstadoConciencia` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `GlasgowOcular` int NOT NULL,
  `GlasgowVerbal` int NOT NULL,
  `GlasgowMotor` int NOT NULL,
  `GlasgowTotal` int NOT NULL,
  `ViaAerea` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Ventilacion` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Pulso` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `PielMucosas` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `LlenadoCapilar` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Pupilas` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Alergias` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `AccesosVenosos` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Pertenencias` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `AntecedentesMedicos` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `FechaRegistro` datetime(6) NOT NULL,
  `UsuarioRegistro` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_ValoracionesFisicas_CuentaServicioId` (`CuentaServicioId`),
  KEY `IX_ValoracionesFisicas_FechaRegistro` (`FechaRegistro`),
  CONSTRAINT `FK_ValoracionesFisicas_CuentasServicios_CuentaServicioId` FOREIGN KEY (`CuentaServicioId`) REFERENCES `cuentasservicios` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-08-14 20:14:58
