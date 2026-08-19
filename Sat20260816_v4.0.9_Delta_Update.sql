-- ============================================================================
-- SCRIPT DE ACTUALIZACIÓN DELTA COMPLETO Y DEFINITIVO - RELEASE v4.0.9
-- Base de Datos: sathospitalario (MySQL 8.0+)
-- Resuelve y asegura todas las columnas requeridas por Entity Framework Core:
-- 1. InsumosCirugiasPacientes -> OrdenCirugiaId (Causa del Error 500)
-- 2. TransferenciasReposicionStock -> InsumoId, SedeOrigenId, SedeDestinoId, Cantidad, Motivo, Observaciones, FechaTransferencia, UsuarioId
-- 3. OrdenesCirugia -> MotivoCancelacion, SedeQuirofanoId, AreaClinicaId
-- 4. Incompatibilidad de Charset / FKs (Error 3780)
-- ============================================================================

USE `sathospitalario`;

SET FOREIGN_KEY_CHECKS = 0;
SET SQL_MODE = 'NO_AUTO_VALUE_ON_ZERO';

-- ----------------------------------------------------------------------------
-- PROCEDIMIENTO PARA AGREGAR COLUMNAS DE FORMA DINÁMICA E IDEMPOTENTE
-- ----------------------------------------------------------------------------
DROP PROCEDURE IF EXISTS AddColumnIfNotExists;
DELIMITER $$
CREATE PROCEDURE AddColumnIfNotExists(
    IN tableName VARCHAR(64),
    IN columnName VARCHAR(64),
    IN columnDef VARCHAR(255)
)
BEGIN
    IF NOT EXISTS (
        SELECT * FROM information_schema.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE()
          AND TABLE_NAME = tableName
          AND COLUMN_NAME = columnName
    ) THEN
        SET @ddl = CONCAT('ALTER TABLE `', tableName, '` ADD COLUMN `', columnName, '` ', columnDef);
        PREPARE stmt FROM @ddl;
        EXECUTE stmt;
        DEALLOCATE PREPARE stmt;
    END IF;
END$$
DELIMITER ;

-- ----------------------------------------------------------------------------
-- 1. TABLA: insumoscirugiaspacientes (CORRECCIÓN CRÍTICA ERROR 500)
-- ----------------------------------------------------------------------------
CALL AddColumnIfNotExists('insumoscirugiaspacientes', 'OrdenCirugiaId', 'char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL');
CALL AddColumnIfNotExists('insumoscirugiaspacientes', 'CantidadEntregada', 'decimal(18,4) NOT NULL DEFAULT 0.0000');
CALL AddColumnIfNotExists('insumoscirugiaspacientes', 'CantidadDevuelta', 'decimal(18,4) NOT NULL DEFAULT 0.0000');

-- ----------------------------------------------------------------------------
-- 2. TABLA: transferenciasreposicionstock (CORRECCIÓN CRÍTICA REPOSICIÓN)
-- ----------------------------------------------------------------------------
DROP TABLE IF EXISTS `transferenciasreposicionstock`;

CREATE TABLE `transferenciasreposicionstock` (
    `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
    `InsumoId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
    `SedeOrigenId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
    `SedeDestinoId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
    `Cantidad` decimal(18,4) NOT NULL DEFAULT 0.0000,
    `Motivo` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT 'Reposicion',
    `Observaciones` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
    `FechaTransferencia` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    `UsuarioId` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT 'Sistema',
    PRIMARY KEY (`Id`),
    KEY `IX_transferenciasreposicionstock_InsumoId` (`InsumoId`),
    KEY `IX_transferenciasreposicionstock_SedeOrigenId` (`SedeOrigenId`),
    KEY `IX_transferenciasreposicionstock_SedeDestinoId` (`SedeDestinoId`),
    KEY `IX_transferenciasreposicionstock_FechaTransferencia` (`FechaTransferencia`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- ----------------------------------------------------------------------------
-- 3. TABLA: ordenescirugia (Asegurar todas las columnas de entidad)
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `ordenescirugia` (
    `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
    `CuentaServicioId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
    `PacienteId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
    `MedicoId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
    `DescripcionCirugia` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
    `SalaQuirofano` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT 'Quirófano 1',
    `ModalidadAnestesia` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT 'General',
    `EsAlquilado` tinyint(1) NOT NULL DEFAULT '0',
    `PrecioDerechoSalaUsd` decimal(18,2) NOT NULL DEFAULT '0.00',
    `PrecioBaseUsd` decimal(18,2) NOT NULL DEFAULT '0.00',
    `FechaHoraProgramada` datetime(6) NOT NULL,
    `Estado` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT 'Programada',
    `MotivoCancelacion` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
    `FechaCreacion` datetime(6) NOT NULL,
    `UsuarioCreacion` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
    `AreaClinicaId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
    `SedeQuirofanoId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
    PRIMARY KEY (`Id`),
    KEY `IX_ordenescirugia_CuentaServicioId` (`CuentaServicioId`),
    KEY `IX_ordenescirugia_PacienteId` (`PacienteId`),
    KEY `IX_ordenescirugia_MedicoId` (`MedicoId`),
    KEY `IX_ordenescirugia_Estado` (`Estado`),
    KEY `IX_ordenescirugia_FechaHoraProgramada` (`FechaHoraProgramada`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

ALTER TABLE `ordenescirugia` 
    MODIFY COLUMN `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
    MODIFY COLUMN `CuentaServicioId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
    MODIFY COLUMN `PacienteId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
    MODIFY COLUMN `MedicoId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
    MODIFY COLUMN `AreaClinicaId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
    MODIFY COLUMN `SedeQuirofanoId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL;

CALL AddColumnIfNotExists('ordenescirugia', 'SalaQuirofano', 'varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT "Quirófano 1"');
CALL AddColumnIfNotExists('ordenescirugia', 'ModalidadAnestesia', 'varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT "General"');
CALL AddColumnIfNotExists('ordenescirugia', 'EsAlquilado', 'tinyint(1) NOT NULL DEFAULT 0');
CALL AddColumnIfNotExists('ordenescirugia', 'PrecioDerechoSalaUsd', 'decimal(18,2) NOT NULL DEFAULT 0.00');
CALL AddColumnIfNotExists('ordenescirugia', 'PrecioBaseUsd', 'decimal(18,2) NOT NULL DEFAULT 0.00');
CALL AddColumnIfNotExists('ordenescirugia', 'MotivoCancelacion', 'varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL');
CALL AddColumnIfNotExists('ordenescirugia', 'AreaClinicaId', 'char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL');
CALL AddColumnIfNotExists('ordenescirugia', 'SedeQuirofanoId', 'char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL');

-- ----------------------------------------------------------------------------
-- 4. TABLA: cirugiasmedicoshonorarios (Normalizada en 3FN con EspecialidadId)
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `cirugiasmedicoshonorarios` (
    `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
    `OrdenCirugiaId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
    `MedicoId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
    `EspecialidadId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
    `MontoHonorarioUsd` decimal(18,2) NOT NULL DEFAULT '0.00',
    `EsCirujanoPrincipal` tinyint(1) NOT NULL DEFAULT '0',
    `FechaAsignacion` datetime(6) NOT NULL,
    PRIMARY KEY (`Id`),
    KEY `IX_cirugiasmedicoshonorarios_OrdenCirugiaId` (`OrdenCirugiaId`),
    KEY `IX_cirugiasmedicoshonorarios_MedicoId` (`MedicoId`),
    KEY `IX_cirugiasmedicoshonorarios_EspecialidadId` (`EspecialidadId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

ALTER TABLE `cirugiasmedicoshonorarios` 
    MODIFY COLUMN `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
    MODIFY COLUMN `OrdenCirugiaId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
    MODIFY COLUMN `MedicoId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
    MODIFY COLUMN `EspecialidadId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL;

CALL AddColumnIfNotExists('cirugiasmedicoshonorarios', 'EspecialidadId', 'char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL');
CALL AddColumnIfNotExists('cirugiasmedicoshonorarios', 'EsCirujanoPrincipal', 'tinyint(1) NOT NULL DEFAULT 0');

-- ----------------------------------------------------------------------------
-- 5. TABLA: solicitudesinsumoscirugia (Requerimientos Ad-hoc desde Quirófano)
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `solicitudesinsumoscirugia` (
    `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
    `OrdenCirugiaId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
    `InsumoId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
    `CantidadSolicitada` decimal(18,2) NOT NULL,
    `AlmacenOrigenId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
    `EstadoSolicitud` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT 'Pendiente',
    `FechaSolicitud` datetime(6) NOT NULL,
    `UsuarioSolicitud` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
    `FechaDespacho` datetime(6) DEFAULT NULL,
    `UsuarioDespacho` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
    `Observaciones` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
    PRIMARY KEY (`Id`),
    KEY `IX_solicitudesinsumoscirugia_OrdenCirugiaId` (`OrdenCirugiaId`),
    KEY `IX_solicitudesinsumoscirugia_InsumoId` (`InsumoId`),
    KEY `IX_solicitudesinsumoscirugia_AlmacenOrigenId` (`AlmacenOrigenId`),
    KEY `IX_solicitudesinsumoscirugia_EstadoSolicitud` (`EstadoSolicitud`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

ALTER TABLE `solicitudesinsumoscirugia` 
    MODIFY COLUMN `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
    MODIFY COLUMN `OrdenCirugiaId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
    MODIFY COLUMN `InsumoId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
    MODIFY COLUMN `AlmacenOrigenId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL;

-- ----------------------------------------------------------------------------
-- 6. TABLAS DE CHECKLIST PREOPERATORIO (Requisitos y Verificaciones)
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `requisitoscirugia` (
    `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
    `Nombre` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
    `Descripcion` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
    `EsActivo` tinyint(1) NOT NULL DEFAULT '1',
    `FechaCreacion` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

ALTER TABLE `requisitoscirugia` 
    MODIFY COLUMN `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL;

CALL AddColumnIfNotExists('requisitoscirugia', 'EsActivo', 'tinyint(1) NOT NULL DEFAULT 1');
CALL AddColumnIfNotExists('requisitoscirugia', 'FechaCreacion', 'datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6)');

CREATE TABLE IF NOT EXISTS `ordenescirugiarequisitos` (
    `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
    `OrdenCirugiaId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
    `RequisitoCirugiaId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
    `Cumplido` tinyint(1) NOT NULL DEFAULT '0',
    `FechaVerificacion` datetime(6) DEFAULT NULL,
    `VerificadoPor` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
    PRIMARY KEY (`Id`),
    KEY `IX_ordenescirugiarequisitos_OrdenCirugiaId` (`OrdenCirugiaId`),
    KEY `IX_ordenescirugiarequisitos_RequisitoCirugiaId` (`RequisitoCirugiaId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

ALTER TABLE `ordenescirugiarequisitos` 
    MODIFY COLUMN `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
    MODIFY COLUMN `OrdenCirugiaId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
    MODIFY COLUMN `RequisitoCirugiaId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL;

-- ----------------------------------------------------------------------------
-- 7. TABLA: cirugialogs (Auditoría de Eventos Quirúrgicos)
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `cirugialogs` (
    `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
    `OrdenCirugiaId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
    `UsuarioId` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
    `Evento` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
    `Detalle` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
    `Timestamp` datetime(6) NOT NULL,
    PRIMARY KEY (`Id`),
    KEY `IX_cirugialogs_OrdenCirugiaId` (`OrdenCirugiaId`),
    KEY `IX_cirugialogs_Timestamp` (`Timestamp`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

ALTER TABLE `cirugialogs` 
    MODIFY COLUMN `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
    MODIFY COLUMN `OrdenCirugiaId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL;

-- Limpieza del procedimiento auxiliar
DROP PROCEDURE IF EXISTS AddColumnIfNotExists;

SET FOREIGN_KEY_CHECKS = 1;

-- ============================================================================
-- FIN DEL SCRIPT v4.0.9
-- ============================================================================
