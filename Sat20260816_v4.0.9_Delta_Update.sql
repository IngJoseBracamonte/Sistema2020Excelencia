-- ============================================================================
-- SCRIPT DE ACTUALIZACIÓN DELTA COMPLETO - RELEASE v4.0.9
-- Base de Datos: sathospitalario (MySQL 8.0+)
-- Módulo de Cirugía / Pabellón Quirúrgico, Gestión de Honorarios (3FN),
-- Checklist Preoperatorio y Reposición e Intercambio de Insumos Multi-Sede
-- Resuelve y previene incompatibilidades de Charset/Collation (Error 3780)
-- ============================================================================

USE `sathospitalario`;

SET FOREIGN_KEY_CHECKS = 0;
SET SQL_MODE = 'NO_AUTO_VALUE_ON_ZERO';

-- ----------------------------------------------------------------------------
-- PROCEDIMIENTO AUXILIAR PARA AGREGAR COLUMNAS DE FORMA SEGURA E IDEMPOTENTE
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
-- 1. TABLA: ordenescirugia (Ajustes de Columnas, Charset y Collation)
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
    `FechaCreacion` datetime(6) NOT NULL,
    `UsuarioCreacion` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
    `AreaClinicaId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
    PRIMARY KEY (`Id`),
    KEY `IX_ordenescirugia_CuentaServicioId` (`CuentaServicioId`),
    KEY `IX_ordenescirugia_PacienteId` (`PacienteId`),
    KEY `IX_ordenescirugia_MedicoId` (`MedicoId`),
    KEY `IX_ordenescirugia_Estado` (`Estado`),
    KEY `IX_ordenescirugia_FechaHoraProgramada` (`FechaHoraProgramada`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Homologar Charset en ordenescirugia
ALTER TABLE `ordenescirugia` 
    MODIFY COLUMN `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
    MODIFY COLUMN `CuentaServicioId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
    MODIFY COLUMN `PacienteId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
    MODIFY COLUMN `MedicoId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
    MODIFY COLUMN `AreaClinicaId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL;

-- Columnas nuevas en ordenescirugia
CALL AddColumnIfNotExists('ordenescirugia', 'SalaQuirofano', 'varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT "Quirófano 1"');
CALL AddColumnIfNotExists('ordenescirugia', 'ModalidadAnestesia', 'varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT "General"');
CALL AddColumnIfNotExists('ordenescirugia', 'EsAlquilado', 'tinyint(1) NOT NULL DEFAULT 0');
CALL AddColumnIfNotExists('ordenescirugia', 'PrecioDerechoSalaUsd', 'decimal(18,2) NOT NULL DEFAULT 0.00');
CALL AddColumnIfNotExists('ordenescirugia', 'PrecioBaseUsd', 'decimal(18,2) NOT NULL DEFAULT 0.00');

-- ----------------------------------------------------------------------------
-- 2. TABLA: cirugiasobservacioneshistorial (Resolución Error 3780)
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `cirugiasobservacioneshistorial` (
    `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
    `FechaRegistro` datetime(6) NOT NULL,
    `Observacion` varchar(1000) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
    `OrdenCirugiaId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
    `Tipo` int NOT NULL DEFAULT '0',
    `UsuarioRegistro` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
    `UsuarioRegistroId` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
    PRIMARY KEY (`Id`),
    KEY `IX_CirugiasObservacionesHistorial_OrdenCirugiaId` (`OrdenCirugiaId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Eliminar FK previa si estuviese corrupta o con charset incompatible
ALTER TABLE `cirugiasobservacioneshistorial` 
    DROP FOREIGN KEY IF EXISTS `FK_CirugiasObservacionesHistorial_OrdenesCirugia_OrdenCirugiaId`;

-- Homologar Charset
ALTER TABLE `cirugiasobservacioneshistorial` 
    MODIFY COLUMN `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
    MODIFY COLUMN `OrdenCirugiaId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL;

-- Recrear FK limpia
ALTER TABLE `cirugiasobservacioneshistorial` 
    ADD CONSTRAINT `FK_CirugiasObservacionesHistorial_OrdenesCirugia_OrdenCirugiaId` 
    FOREIGN KEY (`OrdenCirugiaId`) REFERENCES `ordenescirugia` (`Id`) ON DELETE CASCADE;

-- ----------------------------------------------------------------------------
-- 3. TABLA: cirugiasmedicoshonorarios (Normalizada en 3FN con EspecialidadId)
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
    DROP FOREIGN KEY IF EXISTS `FK_cirugiasmedicoshonorarios_OrdenCirugia`,
    DROP FOREIGN KEY IF EXISTS `FK_cirugiasmedicoshonorarios_Medico`,
    DROP FOREIGN KEY IF EXISTS `FK_cirugiasmedicoshonorarios_Especialidad`;

ALTER TABLE `cirugiasmedicoshonorarios` 
    MODIFY COLUMN `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
    MODIFY COLUMN `OrdenCirugiaId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
    MODIFY COLUMN `MedicoId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
    MODIFY COLUMN `EspecialidadId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL;

CALL AddColumnIfNotExists('cirugiasmedicoshonorarios', 'EspecialidadId', 'char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL');
CALL AddColumnIfNotExists('cirugiasmedicoshonorarios', 'EsCirujanoPrincipal', 'tinyint(1) NOT NULL DEFAULT 0');

ALTER TABLE `cirugiasmedicoshonorarios`
    ADD CONSTRAINT `FK_cirugiasmedicoshonorarios_OrdenCirugia` FOREIGN KEY (`OrdenCirugiaId`) REFERENCES `ordenescirugia` (`Id`) ON DELETE CASCADE,
    ADD CONSTRAINT `FK_cirugiasmedicoshonorarios_Medico` FOREIGN KEY (`MedicoId`) REFERENCES `medicos` (`Id`) ON DELETE RESTRICT,
    ADD CONSTRAINT `FK_cirugiasmedicoshonorarios_Especialidad` FOREIGN KEY (`EspecialidadId`) REFERENCES `especialidades` (`Id`) ON DELETE RESTRICT;

-- ----------------------------------------------------------------------------
-- 4. TABLA: solicitudesinsumoscirugia (Requerimientos Ad-hoc desde Quirófano)
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
    DROP FOREIGN KEY IF EXISTS `FK_solicitudesinsumoscirugia_OrdenCirugia`,
    DROP FOREIGN KEY IF EXISTS `FK_solicitudesinsumoscirugia_Insumo`,
    DROP FOREIGN KEY IF EXISTS `FK_solicitudesinsumoscirugia_AlmacenOrigen`;

ALTER TABLE `solicitudesinsumoscirugia` 
    MODIFY COLUMN `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
    MODIFY COLUMN `OrdenCirugiaId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
    MODIFY COLUMN `InsumoId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
    MODIFY COLUMN `AlmacenOrigenId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL;

ALTER TABLE `solicitudesinsumoscirugia`
    ADD CONSTRAINT `FK_solicitudesinsumoscirugia_OrdenCirugia` FOREIGN KEY (`OrdenCirugiaId`) REFERENCES `ordenescirugia` (`Id`) ON DELETE CASCADE,
    ADD CONSTRAINT `FK_solicitudesinsumoscirugia_Insumo` FOREIGN KEY (`InsumoId`) REFERENCES `insumos` (`Id`) ON DELETE RESTRICT,
    ADD CONSTRAINT `FK_solicitudesinsumoscirugia_AlmacenOrigen` FOREIGN KEY (`AlmacenOrigenId`) REFERENCES `sedes` (`Id`) ON DELETE RESTRICT;

-- ----------------------------------------------------------------------------
-- 5. TABLA: transferenciasreposicionstock (Reposición e Intercambio de Insumos)
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `transferenciasreposicionstock` (
    `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
    `SedeOrigenId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
    `SedeDestinoId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
    `AreaClinicaOrigenId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
    `AreaClinicaDestinoId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
    `InsumoEntregadoId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
    `CantidadEntregada` decimal(18,2) NOT NULL,
    `InsumoDevueltoId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
    `CantidadDevuelta` decimal(18,2) NOT NULL DEFAULT '0.00',
    `TipoOperacion` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT 'Reposicion',
    `Motivo` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
    `UsuarioSupervisorId` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
    `FechaMovimiento` datetime(6) NOT NULL,
    PRIMARY KEY (`Id`),
    KEY `IX_transferenciasreposicionstock_SedeOrigenId` (`SedeOrigenId`),
    KEY `IX_transferenciasreposicionstock_SedeDestinoId` (`SedeDestinoId`),
    KEY `IX_transferenciasreposicionstock_InsumoEntregadoId` (`InsumoEntregadoId`),
    KEY `IX_transferenciasreposicionstock_InsumoDevueltoId` (`InsumoDevueltoId`),
    KEY `IX_transferenciasreposicionstock_FechaMovimiento` (`FechaMovimiento`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

ALTER TABLE `transferenciasreposicionstock`
    DROP FOREIGN KEY IF EXISTS `FK_transf_SedeOrigen`,
    DROP FOREIGN KEY IF EXISTS `FK_transf_SedeDestino`,
    DROP FOREIGN KEY IF EXISTS `FK_transf_InsumoEntregado`,
    DROP FOREIGN KEY IF EXISTS `FK_transf_InsumoDevuelto`;

ALTER TABLE `transferenciasreposicionstock` 
    MODIFY COLUMN `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
    MODIFY COLUMN `SedeOrigenId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
    MODIFY COLUMN `SedeDestinoId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
    MODIFY COLUMN `AreaClinicaOrigenId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
    MODIFY COLUMN `AreaClinicaDestinoId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
    MODIFY COLUMN `InsumoEntregadoId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
    MODIFY COLUMN `InsumoDevueltoId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL;

ALTER TABLE `transferenciasreposicionstock`
    ADD CONSTRAINT `FK_transf_SedeOrigen` FOREIGN KEY (`SedeOrigenId`) REFERENCES `sedes` (`Id`) ON DELETE RESTRICT,
    ADD CONSTRAINT `FK_transf_SedeDestino` FOREIGN KEY (`SedeDestinoId`) REFERENCES `sedes` (`Id`) ON DELETE RESTRICT,
    ADD CONSTRAINT `FK_transf_InsumoEntregado` FOREIGN KEY (`InsumoEntregadoId`) REFERENCES `insumos` (`Id`) ON DELETE RESTRICT,
    ADD CONSTRAINT `FK_transf_InsumoDevuelto` FOREIGN KEY (`InsumoDevueltoId`) REFERENCES `insumos` (`Id`) ON DELETE RESTRICT;

-- ----------------------------------------------------------------------------
-- 6. TABLAS DE CHECKLIST PREOPERATORIO (Requisitos y Verificaciones)
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `requisitoscirugia` (
    `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
    `Nombre` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
    `Descripcion` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
    `EsObligatorio` tinyint(1) NOT NULL DEFAULT '1',
    `Activo` tinyint(1) NOT NULL DEFAULT '1',
    PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

ALTER TABLE `requisitoscirugia` 
    MODIFY COLUMN `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL;

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
    DROP FOREIGN KEY IF EXISTS `FK_ordenescirugiareq_OrdenCirugia`,
    DROP FOREIGN KEY IF EXISTS `FK_ordenescirugiareq_Requisito`;

ALTER TABLE `ordenescirugiarequisitos` 
    MODIFY COLUMN `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
    MODIFY COLUMN `OrdenCirugiaId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
    MODIFY COLUMN `RequisitoCirugiaId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL;

ALTER TABLE `ordenescirugiarequisitos`
    ADD CONSTRAINT `FK_ordenescirugiareq_OrdenCirugia` FOREIGN KEY (`OrdenCirugiaId`) REFERENCES `ordenescirugia` (`Id`) ON DELETE CASCADE,
    ADD CONSTRAINT `FK_ordenescirugiareq_Requisito` FOREIGN KEY (`RequisitoCirugiaId`) REFERENCES `requisitoscirugia` (`Id`) ON DELETE RESTRICT;

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
    DROP FOREIGN KEY IF EXISTS `FK_cirugialogs_OrdenCirugia`;

ALTER TABLE `cirugialogs` 
    MODIFY COLUMN `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
    MODIFY COLUMN `OrdenCirugiaId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL;

ALTER TABLE `cirugialogs`
    ADD CONSTRAINT `FK_cirugialogs_OrdenCirugia` FOREIGN KEY (`OrdenCirugiaId`) REFERENCES `ordenescirugia` (`Id`) ON DELETE CASCADE;

-- ----------------------------------------------------------------------------
-- 8. SEED DATA: Requisitos Preoperatorios Base
-- ----------------------------------------------------------------------------
INSERT INTO `requisitoscirugia` (`Id`, `Nombre`, `Descripcion`, `EsObligatorio`, `Activo`)
SELECT '11111111-1111-1111-1111-111111111101', 'Ayuno Completo (>= 8 horas)', 'Verificación estricta de ingesta de alimentos y líquidos', 1, 1
WHERE NOT EXISTS (SELECT 1 FROM `requisitoscirugia` WHERE `Id` = '11111111-1111-1111-1111-111111111101');

INSERT INTO `requisitoscirugia` (`Id`, `Nombre`, `Descripcion`, `EsObligatorio`, `Activo`)
SELECT '11111111-1111-1111-1111-111111111102', 'Evaluación Cardiovascular y EKG', 'Informe cardiológico vigente y apto quirúrgico firmado', 1, 1
WHERE NOT EXISTS (SELECT 1 FROM `requisitoscirugia` WHERE `Id` = '11111111-1111-1111-1111-111111111102');

INSERT INTO `requisitoscirugia` (`Id`, `Nombre`, `Descripcion`, `EsObligatorio`, `Activo`)
SELECT '11111111-1111-1111-1111-111111111103', 'Laboratorios (Hematología, PT, PTT, Glicemia)', 'Perfil preoperatorio completo emitido en los últimos 15 días', 1, 1
WHERE NOT EXISTS (SELECT 1 FROM `requisitoscirugia` WHERE `Id` = '11111111-1111-1111-1111-111111111103');

INSERT INTO `requisitoscirugia` (`Id`, `Nombre`, `Descripcion`, `EsObligatorio`, `Activo`)
SELECT '11111111-1111-1111-1111-111111111104', 'Consentimiento Informado Firmado', 'Documento legal de autorización quirúrgica y anestésica firmado por paciente o familiar', 1, 1
WHERE NOT EXISTS (SELECT 1 FROM `requisitoscirugia` WHERE `Id` = '11111111-1111-1111-1111-111111111104');

INSERT INTO `requisitoscirugia` (`Id`, `Nombre`, `Descripcion`, `EsObligatorio`, `Activo`)
SELECT '11111111-1111-1111-1111-111111111105', 'Reserva de Hemoderivados / Tipaje Sanguíneo', 'Disponibilidad de concentrado globular o plasma en banco de sangre si aplica', 0, 1
WHERE NOT EXISTS (SELECT 1 FROM `requisitoscirugia` WHERE `Id` = '11111111-1111-1111-1111-111111111105');

-- Limpieza del procedimiento auxiliar
DROP PROCEDURE IF EXISTS AddColumnIfNotExists;

SET FOREIGN_KEY_CHECKS = 1;

-- ============================================================================
-- FIN DEL SCRIPT v4.0.9 - EJECUCIÓN EXITOSA
-- ============================================================================
