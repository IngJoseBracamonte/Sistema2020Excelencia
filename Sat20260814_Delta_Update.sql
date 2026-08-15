-- ============================================================================
-- SCRIPT DE ACTUALIZACION DIRECTO PARA PRODUCCION
-- Base de Datos: sathospitalario (MySQL 8.0+)
-- Corrige incompatibilidad de Charset/Collation (Error 3780) y crea tablas/FKs faltantes
-- ============================================================================

SET FOREIGN_KEY_CHECKS = 0;
SET SQL_MODE = 'NO_AUTO_VALUE_ON_ZERO';

-- ----------------------------------------------------------------------------
-- PASO 1: CREAR O AJUSTAR COLUMNAS FALTANTES CON CHARSET COMPATIBLE
-- ----------------------------------------------------------------------------

-- 1.1 AreasClinicas
ALTER TABLE `areasclinicas` ADD COLUMN `AreaPadreId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL;
ALTER TABLE `areasclinicas` MODIFY COLUMN `AreaPadreId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL;
ALTER TABLE `areasclinicas` ADD COLUMN `EsSubAreaAlmacenPrincipal` tinyint(1) NOT NULL DEFAULT '0';

-- 1.2 DetallesPago
ALTER TABLE `detallespago` ADD COLUMN `MetodoPagoId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL;
ALTER TABLE `detallespago` MODIFY COLUMN `MetodoPagoId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL;
ALTER TABLE `detallespago` ADD COLUMN `UsuarioCargaId` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL;

-- 1.3 MovimientosInsumo
ALTER TABLE `movimientosinsumo` ADD COLUMN `UsuarioId` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL;

-- 1.4 OrdenesCirugia (Alinear charsets con tablas referenciadas)
ALTER TABLE `ordenescirugia` ADD COLUMN `AreaClinicaId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL;
ALTER TABLE `ordenescirugia` MODIFY COLUMN `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL;
ALTER TABLE `ordenescirugia` MODIFY COLUMN `CuentaServicioId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL;
ALTER TABLE `ordenescirugia` MODIFY COLUMN `PacienteId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL;
ALTER TABLE `ordenescirugia` MODIFY COLUMN `MedicoId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL;
ALTER TABLE `ordenescirugia` MODIFY COLUMN `AreaClinicaId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL;

-- 1.5 InsumosCirugiasPacientes (Alinear charsets con CuentasServicios e Insumos)
ALTER TABLE `insumoscirugiaspacientes` MODIFY COLUMN `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL;
ALTER TABLE `insumoscirugiaspacientes` MODIFY COLUMN `CuentaServicioId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL;
ALTER TABLE `insumoscirugiaspacientes` MODIFY COLUMN `InsumoId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL;

-- 1.6 InsumosPrincipiosActivos y PrincipiosActivos
ALTER TABLE `principiosactivos` MODIFY COLUMN `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL;
ALTER TABLE `insumosprincipiosactivos` MODIFY COLUMN `InsumoId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL;
ALTER TABLE `insumosprincipiosactivos` MODIFY COLUMN `PrincipioActivoId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL;

-- 1.7 CirugiaLogs
ALTER TABLE `cirugialogs` MODIFY COLUMN `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL;
ALTER TABLE `cirugialogs` MODIFY COLUMN `OrdenCirugiaId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL;

-- 1.8 CompromisosPago
ALTER TABLE `compromisospago` MODIFY COLUMN `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL;
ALTER TABLE `compromisospago` MODIFY COLUMN `CuentaPorCobrarId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL;

-- 1.9 CuentasServicios
ALTER TABLE `cuentasservicios` MODIFY COLUMN `AreaClinicaId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL;
ALTER TABLE `cuentasservicios` MODIFY COLUMN `CamaRetenidaId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL;

-- 1.10 DetallesServicioCuenta
ALTER TABLE `detallesserviciocuenta` MODIFY COLUMN `AreaClinicaId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL;
ALTER TABLE `detallesserviciocuenta` MODIFY COLUMN `DetallePadreId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL;

-- 1.11 CitasMedicas
ALTER TABLE `citasmedicas` MODIFY COLUMN `AreaClinicaId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL;

-- 1.12 ServiciosClinicos
ALTER TABLE `serviciosclinicos` MODIFY COLUMN `ServicioInformeId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL;

-- 1.13 OrdenesImagenes
ALTER TABLE `ordenesimagenes` MODIFY COLUMN `MedicoInterpreteId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL;

-- ----------------------------------------------------------------------------
-- PASO 2: NUEVAS TABLAS FALTANTES
-- ----------------------------------------------------------------------------

CREATE TABLE IF NOT EXISTS `auditlogs` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `ActionType` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `IpAddress` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `NewValue` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `OldValue` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Timestamp` datetime(6) NOT NULL,
  `UserId` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE IF NOT EXISTS `cirugiasobservacioneshistorial` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `FechaRegistro` datetime(6) NOT NULL,
  `Observacion` varchar(1000) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `OrdenCirugiaId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `Tipo` int NOT NULL DEFAULT '0',
  `UsuarioRegistro` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `UsuarioRegistroId` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_CirugiasObservacionesHistorial_OrdenCirugiaId` (`OrdenCirugiaId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE IF NOT EXISTS `requisitoscirugia` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `Descripcion` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `EsActivo` tinyint(1) NOT NULL DEFAULT '0',
  `FechaCreacion` datetime(6) NOT NULL,
  `Nombre` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE IF NOT EXISTS `ordenescirugiarequisitos` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `Cumplido` tinyint(1) NOT NULL DEFAULT '0',
  `FechaVerificacion` datetime(6) DEFAULT NULL,
  `OrdenCirugiaId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `RequisitoCirugiaId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `VerificadoPor` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_OrdenesCirugiaRequisitos_OrdenCirugiaId` (`OrdenCirugiaId`),
  KEY `IX_OrdenesCirugiaRequisitos_RequisitoCirugiaId` (`RequisitoCirugiaId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE IF NOT EXISTS `roles` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `ConcurrencyStamp` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Name` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `NormalizedName` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `RoleNameIndex` (`NormalizedName`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE IF NOT EXISTS `roleclaims` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `ClaimType` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `ClaimValue` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `RoleId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_RoleClaims_RoleId` (`RoleId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE IF NOT EXISTS `usuarios` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `AccessFailedCount` int NOT NULL DEFAULT '0',
  `ApellidoReal` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ConcurrencyStamp` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Email` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `EmailConfirmed` tinyint(1) NOT NULL DEFAULT '0',
  `EsActivo` tinyint(1) NOT NULL DEFAULT '0',
  `LegacyCajeroId` int DEFAULT NULL,
  `LockoutEnabled` tinyint(1) NOT NULL DEFAULT '0',
  `LockoutEnd` datetime(6) DEFAULT NULL,
  `NombreReal` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `NormalizedEmail` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `NormalizedUserName` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `PasswordHash` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `PhoneNumber` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `PhoneNumberConfirmed` tinyint(1) NOT NULL DEFAULT '0',
  `RequirePasswordReset` tinyint(1) NOT NULL DEFAULT '0',
  `SecurityStamp` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `TwoFactorEnabled` tinyint(1) NOT NULL DEFAULT '0',
  `UserName` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `EmailIndex` (`NormalizedEmail`),
  UNIQUE KEY `UserNameIndex` (`NormalizedUserName`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE IF NOT EXISTS `usuarioclaims` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `ClaimType` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `ClaimValue` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `UserId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_UsuarioClaims_UserId` (`UserId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE IF NOT EXISTS `usuariologins` (
  `LoginProvider` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ProviderKey` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ProviderDisplayName` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `UserId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  PRIMARY KEY (`LoginProvider`, `ProviderKey`),
  KEY `IX_UsuarioLogins_UserId` (`UserId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE IF NOT EXISTS `usuarioroles` (
  `UserId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `RoleId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  PRIMARY KEY (`UserId`, `RoleId`),
  KEY `IX_UsuarioRoles_RoleId` (`RoleId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE IF NOT EXISTS `usuariotokens` (
  `UserId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `LoginProvider` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Value` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  PRIMARY KEY (`UserId`, `LoginProvider`, `Name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE IF NOT EXISTS `passwordresetrequests` (
  `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `Estado` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `FechaProcesado` datetime(6) DEFAULT NULL,
  `FechaSolicitud` datetime(6) NOT NULL,
  `ProcesadoPor` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Username` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `UsuarioId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- ----------------------------------------------------------------------------
-- PASO 3: CLAVES FORANEAS
-- ----------------------------------------------------------------------------

-- AreasClinicas
ALTER TABLE `areasclinicas` ADD CONSTRAINT `FK_AreasClinicas_AreasClinicas_AreaPadreId` FOREIGN KEY (`AreaPadreId`) REFERENCES `areasclinicas` (`Id`) ON DELETE RESTRICT;

-- CirugiaLogs
ALTER TABLE `cirugialogs` ADD CONSTRAINT `FK_CirugiaLogs_OrdenesCirugia_OrdenCirugiaId` FOREIGN KEY (`OrdenCirugiaId`) REFERENCES `ordenescirugia` (`Id`) ON DELETE CASCADE;

-- CirugiasObservacionesHistorial
ALTER TABLE `cirugiasobservacioneshistorial` ADD CONSTRAINT `FK_CirugiasObservacionesHistorial_OrdenesCirugia_OrdenCirugiaId` FOREIGN KEY (`OrdenCirugiaId`) REFERENCES `ordenescirugia` (`Id`) ON DELETE CASCADE;

-- CitasMedicas
ALTER TABLE `citasmedicas` ADD CONSTRAINT `FK_CitasMedicas_AreasClinicas_AreaClinicaId` FOREIGN KEY (`AreaClinicaId`) REFERENCES `areasclinicas` (`Id`) ON DELETE SET NULL;

-- CompromisosPago
ALTER TABLE `compromisospago` ADD CONSTRAINT `FK_CompromisosPago_CuentasPorCobrar_CuentaPorCobrarId` FOREIGN KEY (`CuentaPorCobrarId`) REFERENCES `cuentasporcobrar` (`Id`) ON DELETE CASCADE;

-- CuentasServicios
ALTER TABLE `cuentasservicios` ADD CONSTRAINT `FK_CuentasServicios_AreasClinicas_AreaClinicaId` FOREIGN KEY (`AreaClinicaId`) REFERENCES `areasclinicas` (`Id`) ON DELETE RESTRICT;
ALTER TABLE `cuentasservicios` ADD CONSTRAINT `FK_CuentasServicios_AreasClinicas_CamaRetenidaId` FOREIGN KEY (`CamaRetenidaId`) REFERENCES `areasclinicas` (`Id`) ON DELETE RESTRICT;

-- DetallesPago
ALTER TABLE `detallespago` ADD CONSTRAINT `FK_DetallesPago_CatalogoMetodosPago_MetodoPagoId` FOREIGN KEY (`MetodoPagoId`) REFERENCES `catalogometodospago` (`Id`) ON DELETE RESTRICT;

-- DetallesServicioCuenta
ALTER TABLE `detallesserviciocuenta` ADD CONSTRAINT `FK_DetallesServicioCuenta_AreasClinicas_AreaClinicaId` FOREIGN KEY (`AreaClinicaId`) REFERENCES `areasclinicas` (`Id`) ON DELETE SET NULL;
ALTER TABLE `detallesserviciocuenta` ADD CONSTRAINT `FK_DetallesServicioCuenta_DetallesServicioCuenta_DetallePadreId` FOREIGN KEY (`DetallePadreId`) REFERENCES `detallesserviciocuenta` (`Id`) ON DELETE RESTRICT;

-- InsumosCirugiasPacientes
ALTER TABLE `insumoscirugiaspacientes` ADD CONSTRAINT `FK_InsumosCirugiasPacientes_CuentasServicios_CuentaServicioId` FOREIGN KEY (`CuentaServicioId`) REFERENCES `cuentasservicios` (`Id`) ON DELETE CASCADE;
ALTER TABLE `insumoscirugiaspacientes` ADD CONSTRAINT `FK_InsumosCirugiasPacientes_Insumos_InsumoId` FOREIGN KEY (`InsumoId`) REFERENCES `insumos` (`Id`) ON DELETE RESTRICT;

-- InsumosPrincipiosActivos
ALTER TABLE `insumosprincipiosactivos` ADD CONSTRAINT `FK_InsumosPrincipiosActivos_Insumos_InsumoId` FOREIGN KEY (`InsumoId`) REFERENCES `insumos` (`Id`) ON DELETE CASCADE;
ALTER TABLE `insumosprincipiosactivos` ADD CONSTRAINT `FK_InsumosPrincipiosActivos_PrincipiosActivos_PrincipioActivoId` FOREIGN KEY (`PrincipioActivoId`) REFERENCES `principiosactivos` (`Id`) ON DELETE RESTRICT;

-- OrdenesCirugia
ALTER TABLE `ordenescirugia` ADD CONSTRAINT `FK_OrdenesCirugia_AreasClinicas_AreaClinicaId` FOREIGN KEY (`AreaClinicaId`) REFERENCES `areasclinicas` (`Id`) ON DELETE RESTRICT;
ALTER TABLE `ordenescirugia` ADD CONSTRAINT `FK_OrdenesCirugia_CuentasServicios_CuentaServicioId` FOREIGN KEY (`CuentaServicioId`) REFERENCES `cuentasservicios` (`Id`) ON DELETE RESTRICT;
ALTER TABLE `ordenescirugia` ADD CONSTRAINT `FK_OrdenesCirugia_Medicos_MedicoId` FOREIGN KEY (`MedicoId`) REFERENCES `medicos` (`Id`) ON DELETE RESTRICT;
ALTER TABLE `ordenescirugia` ADD CONSTRAINT `FK_OrdenesCirugia_PacientesAdmision_PacienteId` FOREIGN KEY (`PacienteId`) REFERENCES `pacientesadmision` (`Id`) ON DELETE RESTRICT;

-- OrdenesCirugiaRequisitos
ALTER TABLE `ordenescirugiarequisitos` ADD CONSTRAINT `FK_OrdenesCirugiaRequisitos_OrdenesCirugia_OrdenCirugiaId` FOREIGN KEY (`OrdenCirugiaId`) REFERENCES `ordenescirugia` (`Id`) ON DELETE CASCADE;
ALTER TABLE `ordenescirugiarequisitos` ADD CONSTRAINT `FK_OrdenesCirugiaRequisitos_RequisitosCirugia_RequisitoCirugiaId` FOREIGN KEY (`RequisitoCirugiaId`) REFERENCES `requisitoscirugia` (`Id`) ON DELETE RESTRICT;

-- OrdenesImagenes
ALTER TABLE `ordenesimagenes` ADD CONSTRAINT `FK_OrdenesImagenes_Medicos_MedicoSolicitanteId` FOREIGN KEY (`MedicoSolicitanteId`) REFERENCES `medicos` (`Id`) ON DELETE SET NULL;
ALTER TABLE `ordenesimagenes` ADD CONSTRAINT `FK_OrdenesImagenes_PacientesAdmision_PacienteId` FOREIGN KEY (`PacienteId`) REFERENCES `pacientesadmision` (`Id`) ON DELETE CASCADE;

-- ServiciosClinicos
ALTER TABLE `serviciosclinicos` ADD CONSTRAINT `FK_ServiciosClinicos_ServiciosClinicos_ServicioInformeId` FOREIGN KEY (`ServicioInformeId`) REFERENCES `serviciosclinicos` (`Id`) ON DELETE RESTRICT;

-- Claves foráneas Identity
ALTER TABLE `roleclaims` ADD CONSTRAINT `FK_RoleClaims_Roles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `roles` (`Id`) ON DELETE CASCADE;
ALTER TABLE `usuarioclaims` ADD CONSTRAINT `FK_UsuarioClaims_Usuarios_UserId` FOREIGN KEY (`UserId`) REFERENCES `usuarios` (`Id`) ON DELETE CASCADE;
ALTER TABLE `usuariologins` ADD CONSTRAINT `FK_UsuarioLogins_Usuarios_UserId` FOREIGN KEY (`UserId`) REFERENCES `usuarios` (`Id`) ON DELETE CASCADE;
ALTER TABLE `usuarioroles` ADD CONSTRAINT `FK_UsuarioRoles_Roles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `roles` (`Id`) ON DELETE CASCADE;
ALTER TABLE `usuarioroles` ADD CONSTRAINT `FK_UsuarioRoles_Usuarios_UserId` FOREIGN KEY (`UserId`) REFERENCES `usuarios` (`Id`) ON DELETE CASCADE;
ALTER TABLE `usuariotokens` ADD CONSTRAINT `FK_UsuarioTokens_Usuarios_UserId` FOREIGN KEY (`UserId`) REFERENCES `usuarios` (`Id`) ON DELETE CASCADE;

SET FOREIGN_KEY_CHECKS = 1;

-- FIN DEL SCRIPT
