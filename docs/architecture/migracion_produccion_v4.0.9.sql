-- =========================================================================================
-- SISTEMA SAT HOSPITALARIO - SCRIPT DE MIGRACIÓN Y CONVERSIÓN DE PRODUCCIÓN (v4.0.9)
-- =========================================================================================
-- OBJETIVO:
--   1. Crear tablas faltantes sin afectar ninguna data existente.
--   2. Agregar columnas dinámicas a tablas existentes de forma segura e idempotente.
--   3. Convertir y normalizar datos históricos (Insumos, Sedes, Stocks, Cirugías, Categorías).
--   4. Preservar el 100% de la integridad referencial y transaccional histórica.
-- =========================================================================================

USE `SatHospitalario`;

SET FOREIGN_KEY_CHECKS = 0;

-- -----------------------------------------------------------------------------------------
-- PROCEDIMIENTO AUXILIAR: AGREGAR COLUMNAS DE FORMA SEGURA (IDEMPOTENTE)
-- -----------------------------------------------------------------------------------------
DELIMITER $$

DROP PROCEDURE IF EXISTS `AddColumnIfNotExists` $$
CREATE PROCEDURE `AddColumnIfNotExists`(
    IN p_tableName VARCHAR(64),
    IN p_columnName VARCHAR(64),
    IN p_columnDefinition VARCHAR(255)
)
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.COLUMNS 
        WHERE TABLE_SCHEMA = DATABASE() 
          AND TABLE_NAME = p_tableName 
          AND COLUMN_NAME = p_columnName
    ) THEN
        SET @sql = CONCAT('ALTER TABLE `', p_tableName, '` ADD COLUMN `', p_columnName, '` ', p_columnDefinition);
        PREPARE stmt FROM @sql;
        EXECUTE stmt;
        DEALLOCATE PREPARE stmt;
    END IF;
END $$

DELIMITER ;

-- =========================================================================================
-- FASE 1: CREACIÓN DE NUEVAS TABLAS (IF NOT EXISTS)
-- =========================================================================================

-- 1. Sedes y Áreas Clínicas
CREATE TABLE IF NOT EXISTS `Sedes` (
    `Id` CHAR(36) NOT NULL,
    `Codigo` VARCHAR(50) NOT NULL,
    `Nombre` VARCHAR(150) NOT NULL,
    `EsPrincipal` TINYINT(1) NOT NULL DEFAULT 0,
    `Activo` TINYINT(1) NOT NULL DEFAULT 1,
    PRIMARY KEY (`Id`),
    UNIQUE KEY `IX_Sedes_Codigo` (`Codigo`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `AreasClinicas` (
    `Id` CHAR(36) NOT NULL,
    `SedeId` CHAR(36) NOT NULL,
    `Codigo` VARCHAR(50) NOT NULL,
    `Nombre` VARCHAR(150) NOT NULL,
    `Activo` TINYINT(1) NOT NULL DEFAULT 1,
    `EsSubAreaAlmacenPrincipal` TINYINT(1) NOT NULL DEFAULT 0,
    `AreaPadreId` CHAR(36) NULL,
    `Estado` INT NOT NULL DEFAULT 0,
    `EsAreaAdmision` TINYINT(1) NOT NULL DEFAULT 0,
    `ServicioTarifaBaseId` CHAR(36) NULL,
    PRIMARY KEY (`Id`),
    UNIQUE KEY `IX_AreasClinicas_Sede_Codigo` (`SedeId`, `Codigo`),
    INDEX `IX_AreasClinicas_SedeId` (`SedeId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 2. Categorías de Insumos y Principios Activos
CREATE TABLE IF NOT EXISTS `CategoriasInsumo` (
    `Id` CHAR(36) NOT NULL,
    `Nombre` VARCHAR(150) NOT NULL,
    `Codigo` VARCHAR(50) NULL,
    `Activo` TINYINT(1) NOT NULL DEFAULT 1,
    `FechaCreacion` DATETIME NOT NULL,
    PRIMARY KEY (`Id`),
    UNIQUE KEY `IX_CategoriasInsumo_Nombre` (`Nombre`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `PrincipiosActivos` (
    `Id` CHAR(36) NOT NULL,
    `Nombre` VARCHAR(200) NOT NULL,
    `Activo` TINYINT(1) NOT NULL DEFAULT 1,
    PRIMARY KEY (`Id`),
    UNIQUE KEY `IX_PrincipiosActivos_Nombre` (`Nombre`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `InsumosPrincipiosActivos` (
    `InsumoId` CHAR(36) NOT NULL,
    `PrincipioActivoId` CHAR(36) NOT NULL,
    `Concentracion` VARCHAR(100) NULL,
    PRIMARY KEY (`InsumoId`, `PrincipioActivoId`),
    INDEX `IX_InsumoPrincipio_Principio` (`PrincipioActivoId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 3. Módulo Quirúrgico (Pabellón)
CREATE TABLE IF NOT EXISTS `RequisitosCirugia` (
    `Id` CHAR(36) NOT NULL,
    `Nombre` VARCHAR(250) NOT NULL,
    `Descripcion` TEXT NULL,
    `EsActivo` TINYINT(1) NOT NULL DEFAULT 1,
    PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `OrdenesCirugia` (
    `Id` CHAR(36) NOT NULL,
    `CuentaServicioId` CHAR(36) NOT NULL,
    `PacienteId` CHAR(36) NOT NULL,
    `AreaClinicaId` CHAR(36) NULL,
    `SedeQuirofanoId` CHAR(36) NULL,
    `AreaClinicaOrigenId` CHAR(36) NULL,
    `SedeOrigenId` CHAR(36) NULL,
    `DescripcionCirugia` VARCHAR(500) NOT NULL,
    `PrecioBaseUsd` DECIMAL(18,2) NOT NULL,
    `PrecioDerechoSalaUsd` DECIMAL(18,2) NOT NULL DEFAULT 0.00,
    `MedicoId` CHAR(36) NOT NULL,
    `FechaHoraProgramada` DATETIME NOT NULL,
    `Estado` VARCHAR(50) NOT NULL,
    `MotivoCancelacion` VARCHAR(500) NULL,
    `FechaCreacion` DATETIME NOT NULL,
    `UsuarioCreacion` VARCHAR(100) NOT NULL,
    `SalaQuirofano` VARCHAR(100) NOT NULL DEFAULT 'Quirófano 1',
    `ModalidadAnestesia` VARCHAR(100) NOT NULL DEFAULT 'General',
    `EsAlquilado` TINYINT(1) NOT NULL DEFAULT 0,
    PRIMARY KEY (`Id`),
    INDEX `IX_OrdenesCirugia_Fecha` (`FechaHoraProgramada`),
    INDEX `IX_OrdenesCirugia_Estado` (`Estado`),
    INDEX `IX_OrdenesCirugia_Paciente` (`PacienteId`),
    INDEX `IX_OrdenesCirugia_Cuenta` (`CuentaServicioId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `CirugiaLogs` (
    `Id` CHAR(36) NOT NULL,
    `OrdenCirugiaId` CHAR(36) NOT NULL,
    `UsuarioId` VARCHAR(100) NOT NULL,
    `Evento` VARCHAR(100) NOT NULL,
    `Detalle` TEXT NOT NULL,
    `Timestamp` DATETIME NOT NULL,
    PRIMARY KEY (`Id`),
    INDEX `IX_CirugiaLogs_Orden` (`OrdenCirugiaId`),
    INDEX `IX_CirugiaLogs_Time` (`Timestamp`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `CirugiasObservacionesHistorial` (
    `Id` CHAR(36) NOT NULL,
    `OrdenCirugiaId` CHAR(36) NOT NULL,
    `Observacion` TEXT NOT NULL,
    `Tipo` INT NOT NULL,
    `FechaRegistro` DATETIME NOT NULL,
    `UsuarioRegistro` VARCHAR(100) NOT NULL,
    `UsuarioRegistroId` VARCHAR(100) NULL,
    PRIMARY KEY (`Id`),
    INDEX `IX_CirugiaObs_Orden` (`OrdenCirugiaId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `OrdenesCirugiaRequisitos` (
    `Id` CHAR(36) NOT NULL,
    `OrdenCirugiaId` CHAR(36) NOT NULL,
    `RequisitoCirugiaId` CHAR(36) NOT NULL,
    `Cumplido` TINYINT(1) NOT NULL DEFAULT 0,
    `FechaVerificacion` DATETIME NULL,
    `VerificadoPor` VARCHAR(100) NULL,
    PRIMARY KEY (`Id`),
    INDEX `IX_OrdenReq_Orden` (`OrdenCirugiaId`),
    INDEX `IX_OrdenReq_Req` (`RequisitoCirugiaId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `CirugiasMedicosHonorarios` (
    `Id` CHAR(36) NOT NULL,
    `OrdenCirugiaId` CHAR(36) NOT NULL,
    `MedicoId` CHAR(36) NOT NULL,
    `EspecialidadId` CHAR(36) NOT NULL,
    `MontoHonorarioUsd` DECIMAL(18,2) NOT NULL,
    `EsCirujanoPrincipal` TINYINT(1) NOT NULL DEFAULT 0,
    PRIMARY KEY (`Id`),
    INDEX `IX_CirugiaMed_Orden` (`OrdenCirugiaId`),
    INDEX `IX_CirugiaMed_Med` (`MedicoId`),
    INDEX `IX_CirugiaMed_Esp` (`EspecialidadId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `SolicitudesInsumosCirugia` (
    `Id` CHAR(36) NOT NULL,
    `OrdenCirugiaId` CHAR(36) NOT NULL,
    `InsumoId` CHAR(36) NOT NULL,
    `CantidadSolicitada` DECIMAL(18,4) NOT NULL,
    `AlmacenOrigenId` CHAR(36) NOT NULL,
    `EstadoSolicitud` VARCHAR(50) NOT NULL,
    `FechaSolicitud` DATETIME NOT NULL,
    `UsuarioSolicitud` VARCHAR(100) NOT NULL,
    `FechaDespacho` DATETIME NULL,
    `UsuarioDespacho` VARCHAR(100) NULL,
    `Observaciones` VARCHAR(500) NULL,
    PRIMARY KEY (`Id`),
    INDEX `IX_SolIns_Orden` (`OrdenCirugiaId`),
    INDEX `IX_SolIns_Insumo` (`InsumoId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `InsumosCirugiasPacientes` (
    `Id` CHAR(36) NOT NULL,
    `CuentaServicioId` CHAR(36) NOT NULL,
    `OrdenCirugiaId` CHAR(36) NULL,
    `InsumoId` CHAR(36) NOT NULL,
    `CantidadEntregada` DECIMAL(18,4) NOT NULL,
    `CantidadDevuelta` DECIMAL(18,4) NOT NULL DEFAULT 0.0000,
    PRIMARY KEY (`Id`),
    INDEX `IX_InsCirugia_Cuenta` (`CuentaServicioId`),
    INDEX `IX_InsCirugia_Orden` (`OrdenCirugiaId`),
    INDEX `IX_InsCirugia_Insumo` (`InsumoId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 4. Reposición e Intercambio de Insumos Multi-Sede
CREATE TABLE IF NOT EXISTS `TransferenciasReposicionStock` (
    `Id` CHAR(36) NOT NULL,
    `InsumoId` CHAR(36) NOT NULL,
    `SedeOrigenId` CHAR(36) NOT NULL,
    `SedeDestinoId` CHAR(36) NOT NULL,
    `Cantidad` DECIMAL(18,4) NOT NULL,
    `Motivo` VARCHAR(100) NOT NULL DEFAULT 'Reposición',
    `Observaciones` VARCHAR(500) NULL,
    `FechaTransferencia` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `UsuarioId` VARCHAR(100) NOT NULL,
    PRIMARY KEY (`Id`),
    INDEX `IX_TransfRepo_Insumo` (`InsumoId`),
    INDEX `IX_TransfRepo_Origen` (`SedeOrigenId`),
    INDEX `IX_TransfRepo_Destino` (`SedeDestinoId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `PedidosInterSede` (
    `Id` CHAR(36) NOT NULL,
    `Correlativo` VARCHAR(50) NOT NULL,
    `SedeSolicitanteId` CHAR(36) NOT NULL,
    `SedeProveedoraId` CHAR(36) NOT NULL,
    `Estado` INT NOT NULL DEFAULT 0,
    `FechaCreacion` DATETIME NOT NULL,
    `FechaDespacho` DATETIME NULL,
    `FechaRecepcion` DATETIME NULL,
    `UsuarioCreador` VARCHAR(100) NOT NULL,
    `Observaciones` VARCHAR(1000) NULL,
    PRIMARY KEY (`Id`),
    UNIQUE KEY `IX_PedidosInterSede_Correlativo` (`Correlativo`),
    INDEX `IX_PedidosInterSede_Estado` (`Estado`),
    INDEX `IX_PedidosInterSede_Solicitante` (`SedeSolicitanteId`),
    INDEX `IX_PedidosInterSede_Proveedora` (`SedeProveedoraId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `PedidosInterSedeDetalles` (
    `Id` CHAR(36) NOT NULL,
    `PedidoInterSedeId` CHAR(36) NOT NULL,
    `InsumoId` CHAR(36) NOT NULL,
    `CantidadSolicitada` DECIMAL(18,4) NOT NULL,
    `CantidadDespachada` DECIMAL(18,4) NOT NULL DEFAULT 0.0000,
    `CantidadRecibida` DECIMAL(18,4) NOT NULL DEFAULT 0.0000,
    `ObservacionDespacho` VARCHAR(500) NULL,
    PRIMARY KEY (`Id`),
    INDEX `IX_PedidoDet_Pedido` (`PedidoInterSedeId`),
    INDEX `IX_PedidoDet_Insumo` (`InsumoId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `StocksSede` (
    `Id` CHAR(36) NOT NULL,
    `InsumoId` CHAR(36) NOT NULL,
    `SedeId` CHAR(36) NOT NULL,
    `StockActual` DECIMAL(18,4) NOT NULL,
    `StockMinimo` DECIMAL(18,4) NULL,
    `StockMaximo` DECIMAL(18,4) NULL,
    `RowVersion` DATETIME(6) NULL,
    PRIMARY KEY (`Id`),
    INDEX `IX_StocksSede_InsumoSede` (`InsumoId`, `SedeId`),
    INDEX `IX_StocksSede_Sede` (`SedeId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 5. Módulo de Compras, Proveedores y Cuentas por Pagar
CREATE TABLE IF NOT EXISTS `Proveedores` (
    `Id` CHAR(36) NOT NULL,
    `RIF` VARCHAR(50) NOT NULL,
    `RazonSocial` VARCHAR(250) NOT NULL,
    `Direccion` VARCHAR(500) NULL,
    `Telefono` VARCHAR(50) NULL,
    `Activo` TINYINT(1) NOT NULL DEFAULT 1,
    `FechaRegistro` DATETIME NOT NULL,
    PRIMARY KEY (`Id`),
    UNIQUE KEY `IX_Proveedores_RIF` (`RIF`),
    INDEX `IX_Proveedores_RazonSocial` (`RazonSocial`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `OrdenesCompraInventario` (
    `Id` CHAR(36) NOT NULL,
    `NumeroFactura` VARCHAR(100) NOT NULL,
    `ProveedorId` CHAR(36) NULL,
    `ProveedorNombre` VARCHAR(250) NOT NULL,
    `FechaEmision` DATETIME NOT NULL,
    `MontoTotalUSD` DECIMAL(18,2) NOT NULL,
    `MontoTotalBs` DECIMAL(18,2) NOT NULL,
    `TotalAbonadoUSD` DECIMAL(18,2) NOT NULL,
    `SaldoPendienteUSD` DECIMAL(18,2) NOT NULL,
    `Estado` VARCHAR(50) NOT NULL,
    `Observaciones` VARCHAR(1000) NULL,
    PRIMARY KEY (`Id`),
    INDEX `IX_OrdenesCompra_Numero` (`NumeroFactura`),
    INDEX `IX_OrdenesCompra_Proveedor` (`ProveedorNombre`),
    INDEX `IX_OrdenesCompra_Estado` (`Estado`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `PagosProveedores` (
    `Id` CHAR(36) NOT NULL,
    `OrdenCompraId` CHAR(36) NOT NULL,
    `FechaPago` DATETIME NOT NULL,
    `MontoAbonadoUSD` DECIMAL(18,2) NOT NULL,
    `TasaCambio` DECIMAL(18,2) NOT NULL,
    `MontoAbonadoBs` DECIMAL(18,2) NOT NULL,
    `MetodoPago` VARCHAR(50) NOT NULL,
    `Referencia` VARCHAR(100) NULL,
    `UsuarioId` VARCHAR(100) NULL,
    `Observaciones` VARCHAR(1000) NULL,
    PRIMARY KEY (`Id`),
    INDEX `IX_PagosProveedores_Orden` (`OrdenCompraId`),
    INDEX `IX_PagosProveedores_Fecha` (`FechaPago`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 6. Garantías Prendarias, Logs de Auditoría y Compromisos
CREATE TABLE IF NOT EXISTS `GarantiasItems` (
    `Id` CHAR(36) NOT NULL,
    `CuentaPorCobrarId` CHAR(36) NOT NULL,
    `Descripcion` VARCHAR(500) NOT NULL,
    `ValorEstimado` DECIMAL(18,2) NOT NULL,
    `FechaRegistro` DATETIME NOT NULL,
    PRIMARY KEY (`Id`),
    INDEX `IX_GarantiasItems_CxC` (`CuentaPorCobrarId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `DocumentLogs` (
    `Id` CHAR(36) NOT NULL,
    `DocumentType` VARCHAR(100) NOT NULL,
    `ReferenceId` VARCHAR(100) NOT NULL,
    `Action` VARCHAR(50) NOT NULL,
    `UserId` VARCHAR(100) NOT NULL,
    `UserName` VARCHAR(100) NOT NULL,
    `Timestamp` DATETIME NOT NULL,
    `Details` TEXT NULL,
    PRIMARY KEY (`Id`),
    INDEX `IX_DocumentLogs_Ref` (`ReferenceId`),
    INDEX `IX_DocumentLogs_Time` (`Timestamp`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `CompromisosPago` (
    `Id` CHAR(36) NOT NULL,
    `CuentaPorCobrarId` CHAR(36) NOT NULL,
    `Omitido` TINYINT(1) NOT NULL DEFAULT 0,
    `Observacion` TEXT NULL,
    `UsuarioCreacion` VARCHAR(100) NOT NULL,
    `FechaCreacion` DATETIME NOT NULL,
    PRIMARY KEY (`Id`),
    INDEX `IX_CompromisosPago_CxC` (`CuentaPorCobrarId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 7. Catálogo de Tipos de Servicio
CREATE TABLE IF NOT EXISTS `TiposServicio` (
    `Id` INT NOT NULL,
    `Nombre` VARCHAR(100) NOT NULL,
    `Codigo` VARCHAR(50) NOT NULL,
    PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- =========================================================================================
-- FASE 2: AGREGAR COLUMNAS DINÁMICAS A TABLAS EXISTENTES
-- =========================================================================================

-- Insumos
CALL AddColumnIfNotExists('Insumos', 'IsDeleted', 'TINYINT(1) NOT NULL DEFAULT 0');
CALL AddColumnIfNotExists('Insumos', 'FechaInactivacion', 'DATETIME NULL');
CALL AddColumnIfNotExists('Insumos', 'OcultoEnTraslados', 'TINYINT(1) NOT NULL DEFAULT 0');
CALL AddColumnIfNotExists('Insumos', 'Categoria', 'VARCHAR(150) NOT NULL DEFAULT \'Medicamento\'');

-- InsumosCirugiasPacientes
CALL AddColumnIfNotExists('InsumosCirugiasPacientes', 'OrdenCirugiaId', 'CHAR(36) NULL');

-- CuentasPorCobrar
CALL AddColumnIfNotExists('CuentasPorCobrar', 'CompromisoGenerado', 'TINYINT(1) NOT NULL DEFAULT 0');
CALL AddColumnIfNotExists('CuentasPorCobrar', 'GarantiaGenerada', 'TINYINT(1) NOT NULL DEFAULT 0');
CALL AddColumnIfNotExists('CuentasPorCobrar', 'IsAudited', 'TINYINT(1) NOT NULL DEFAULT 0');
CALL AddColumnIfNotExists('CuentasPorCobrar', 'UsuarioAuditoria', 'VARCHAR(100) NULL');
CALL AddColumnIfNotExists('CuentasPorCobrar', 'FechaAuditoria', 'DATETIME NULL');
CALL AddColumnIfNotExists('CuentasPorCobrar', 'QuienAutorizo', 'VARCHAR(150) NULL');
CALL AddColumnIfNotExists('CuentasPorCobrar', 'DoctorProcedimiento', 'VARCHAR(150) NULL');
CALL AddColumnIfNotExists('CuentasPorCobrar', 'InformacionAdicional', 'TEXT NULL');

-- DetallesServicioCuenta
CALL AddColumnIfNotExists('DetallesServicioCuenta', 'DetallePadreId', 'CHAR(36) NULL');
CALL AddColumnIfNotExists('DetallesServicioCuenta', 'TipoServicioId', 'INT NOT NULL DEFAULT 5');
CALL AddColumnIfNotExists('DetallesServicioCuenta', 'UsuarioCargaId', 'VARCHAR(255) NULL');

-- DetallesPago
CALL AddColumnIfNotExists('DetallesPago', 'MetodoPagoId', 'CHAR(36) NULL');
CALL AddColumnIfNotExists('DetallesPago', 'UsuarioCargaId', 'VARCHAR(255) NULL');

-- OrdenesImagenes
CALL AddColumnIfNotExists('OrdenesImagenes', 'LinkInforme', 'VARCHAR(1000) NULL');
CALL AddColumnIfNotExists('OrdenesImagenes', 'ObservacionesMedico', 'VARCHAR(2000) NULL');
CALL AddColumnIfNotExists('OrdenesImagenes', 'MedicoInterpreteId', 'CHAR(36) NULL');
CALL AddColumnIfNotExists('OrdenesImagenes', 'RequiereInforme', 'TINYINT(1) NOT NULL DEFAULT 0');

-- CirugiasObservacionesHistorial
CALL AddColumnIfNotExists('CirugiasObservacionesHistorial', 'UsuarioRegistroId', 'VARCHAR(255) NULL');

-- PedidosInterSedeDetalles
CALL AddColumnIfNotExists('PedidosInterSedeDetalles', 'ObservacionDespacho', 'VARCHAR(500) NULL');

-- OrdenesCirugia
CALL AddColumnIfNotExists('OrdenesCirugia', 'AreaClinicaOrigenId', 'CHAR(36) NULL');
CALL AddColumnIfNotExists('OrdenesCirugia', 'SedeOrigenId', 'CHAR(36) NULL');
CALL AddColumnIfNotExists('OrdenesCirugia', 'SedeQuirofanoId', 'CHAR(36) NULL');
CALL AddColumnIfNotExists('OrdenesCirugia', 'AreaClinicaId', 'CHAR(36) NULL');
CALL AddColumnIfNotExists('OrdenesCirugia', 'SalaQuirofano', 'VARCHAR(100) NOT NULL DEFAULT \'Quirófano 1\'');
CALL AddColumnIfNotExists('OrdenesCirugia', 'ModalidadAnestesia', 'VARCHAR(100) NOT NULL DEFAULT \'General\'');
CALL AddColumnIfNotExists('OrdenesCirugia', 'EsAlquilado', 'TINYINT(1) NOT NULL DEFAULT 0');
CALL AddColumnIfNotExists('OrdenesCirugia', 'PrecioDerechoSalaUsd', 'DECIMAL(18,2) NOT NULL DEFAULT 0.00');

-- Medicos y Especialidades
CALL AddColumnIfNotExists('Medicos', 'Activo', 'TINYINT(1) NOT NULL DEFAULT 1');
CALL AddColumnIfNotExists('Medicos', 'Telefono', 'VARCHAR(50) NULL');
CALL AddColumnIfNotExists('Medicos', 'IntervaloTurnoMinutos', 'INT NOT NULL DEFAULT 20');
CALL AddColumnIfNotExists('Especialidades', 'Activo', 'TINYINT(1) NOT NULL DEFAULT 1');

-- Sedes y Áreas Clínicas
CALL AddColumnIfNotExists('Sedes', 'EsPrincipal', 'TINYINT(1) NOT NULL DEFAULT 0');
CALL AddColumnIfNotExists('Sedes', 'Activo', 'TINYINT(1) NOT NULL DEFAULT 1');
CALL AddColumnIfNotExists('AreasClinicas', 'SedeId', 'CHAR(36) NULL');
CALL AddColumnIfNotExists('AreasClinicas', 'Activo', 'TINYINT(1) NOT NULL DEFAULT 1');
CALL AddColumnIfNotExists('AreasClinicas', 'EsAreaAdmision', 'TINYINT(1) NOT NULL DEFAULT 0');
CALL AddColumnIfNotExists('AreasClinicas', 'Estado', 'INT NOT NULL DEFAULT 0');
CALL AddColumnIfNotExists('AreasClinicas', 'EsSubAreaAlmacenPrincipal', 'TINYINT(1) NOT NULL DEFAULT 0');
CALL AddColumnIfNotExists('AreasClinicas', 'AreaPadreId', 'CHAR(36) NULL');
CALL AddColumnIfNotExists('AreasClinicas', 'ServicioTarifaBaseId', 'CHAR(36) NULL');

-- MovimientosInsumo
CALL AddColumnIfNotExists('MovimientosInsumo', 'UsuarioId', 'VARCHAR(255) NULL');

-- =========================================================================================
-- FASE 3: CONVERSIÓN Y NORMALIZACIÓN DE DATOS EXISTENTES
-- =========================================================================================

-- 1. Sedes Maestras Estándar (SeedConstants)
INSERT IGNORE INTO `Sedes` (`Id`, `Nombre`, `Codigo`, `EsPrincipal`, `Activo`) VALUES
('10000000-0000-0000-0000-000000000001', 'Almacén Principal / Farmacia Central', 'SEDE-PRINCIPAL', 1, 1),
('10000000-0000-0000-0000-000000000002', 'Depósito Emergencia', 'SEDE-EMG', 0, 1),
('10000000-0000-0000-0000-000000000003', 'Depósito Hospitalización', 'SEDE-HOSP', 0, 1),
('10000000-0000-0000-0000-000000000004', 'Depósito UCI', 'SEDE-UCI', 0, 1),
('10000000-0000-0000-0000-000000000005', 'Quirófano / Pabellón Central', 'SEDE-CIRUGIA', 0, 1);

-- Asegurar que la Sede Principal tenga el flag EsPrincipal = 1
UPDATE `Sedes` SET `EsPrincipal` = 1 WHERE `Id` = '10000000-0000-0000-0000-000000000001';

-- 2. Áreas Clínicas y Quirófanos Normalizados
INSERT IGNORE INTO `AreasClinicas` (`Id`, `SedeId`, `Codigo`, `Nombre`, `EsAreaAdmision`, `Estado`, `Activo`) VALUES
-- Quirófanos
('30000000-0000-0000-0000-000000000006', '10000000-0000-0000-0000-000000000005', 'QX-1', 'Quirófano 1 (Cirugía Mayor)', 0, 0, 1),
(UUID(), '10000000-0000-0000-0000-000000000005', 'QX-2', 'Quirófano 2 (Cirugía Menor)', 0, 0, 1),
(UUID(), '10000000-0000-0000-0000-000000000005', 'RECUP-1', 'Sala de Recuperación Post-Anestésica', 0, 0, 1),

-- Hospitalización
('30000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000003', 'HAB-101', 'Habitación 101', 0, 0, 1),
(UUID(), '10000000-0000-0000-0000-000000000003', 'HAB-102', 'Habitación 102', 0, 0, 1),

-- Emergencia
('30000000-0000-0000-0000-000000000001', '10000000-0000-0000-0000-000000000002', 'BOX-1', 'Box Emergencia 1', 1, 0, 1),
(UUID(), '10000000-0000-0000-0000-000000000002', 'BOX-2', 'Box Emergencia 2', 1, 0, 1),

-- UCI
('30000000-0000-0000-0000-000000000003', '10000000-0000-0000-0000-000000000004', 'UCI-1', 'Cama UCI 1', 0, 0, 1);

-- 3. Categorías de Insumos Predefinidas
INSERT IGNORE INTO `CategoriasInsumo` (`Id`, `Nombre`, `Codigo`, `Activo`, `FechaCreacion`) VALUES
(UUID(), 'Medicamento', 'MED', 1, NOW()),
(UUID(), 'Descartable', 'DESC', 1, NOW()),
(UUID(), 'Material Médico', 'MAT-MED', 1, NOW()),
(UUID(), 'Reactivo', 'REACT', 1, NOW()),
(UUID(), 'Material Quirúrgico', 'MAT-QX', 1, NOW()),
(UUID(), 'Otro', 'OTRO', 1, NOW());

-- 4. Tipos de Servicio Oficiales
INSERT IGNORE INTO `TiposServicio` (`Id`, `Nombre`, `Codigo`) VALUES
(1, 'Servicio Médico / Consulta', 'MEDICO'),
(2, 'Examen de Laboratorio', 'LAB'),
(3, 'Rayos X / Imagenología', 'RX'),
(4, 'Tomografía Axial', 'TOMO'),
(5, 'Insumo / Medicamento', 'INSUMO'),
(6, 'Informe / Lectura Médica', 'INFORME');

-- 5. Requisitos Quirúrgicos de Checklist Preoperatorio
INSERT IGNORE INTO `RequisitosCirugia` (`Id`, `Nombre`, `Descripcion`, `EsActivo`) VALUES
('40000000-0000-0000-0000-000000000001', 'Evaluación Cardiovascular / Riesgo Quirúrgico', 'Informe de cardiología y electrocardiograma vigente.', 1),
('40000000-0000-0000-0000-000000000002', 'Exámenes Preoperatorios (Laboratorio)', 'Hematología completa, TP, TPT, Glucemia, Urea, Creatinina y VIH/VDRL.', 1),
('40000000-0000-0000-0000-000000000003', 'Consentimiento Informado Firmado', 'Firma del paciente o familiar responsable para procedimiento quirúrgico y anestesia.', 1),
('40000000-0000-0000-0000-000000000004', 'Ayuno Verificado (Mínimo 8 Horas)', 'Verificación por enfermería de ayuno estricto.', 1),
('40000000-0000-0000-0000-000000000005', 'Valoración Anestésica', 'Aprobación formal firmada por el médico anestesiólogo.', 1),
('40000000-0000-0000-0000-000000000006', 'Reserva de Sangre / Hemoderivados', 'Disponibilidad confirmada con Banco de Sangre (cuando aplique).', 1),
('40000000-0000-0000-0000-000000000007', 'Disponibilidad de Cama Postoperatoria (UCI / Hosp)', 'Cama confirmada para el traslado post-quirúrgico.', 1);

-- 6. Normalización de Datos Existentes en Insumos
UPDATE `Insumos` 
SET `Categoria` = 'Medicamento' 
WHERE `Categoria` IS NULL OR `Categoria` = '';

UPDATE `Insumos` 
SET `IsDeleted` = 0 
WHERE `IsDeleted` IS NULL;

UPDATE `Insumos` 
SET `OcultoEnTraslados` = 0 
WHERE `OcultoEnTraslados` IS NULL;

-- 7. Migración de Stock Existente hacia StocksSede (Sede Principal)
-- Inserta registros de stock en Sede Principal para todos los insumos existentes que no tengan registro en StocksSede
INSERT INTO `StocksSede` (`Id`, `InsumoId`, `SedeId`, `StockActual`, `StockMinimo`, `StockMaximo`, `RowVersion`)
SELECT 
    UUID(), 
    i.`Id`, 
    '10000000-0000-0000-0000-000000000001', 
    0.0000, 
    0.0000, 
    0.0000, 
    NOW()
FROM `Insumos` i
WHERE NOT EXISTS (
    SELECT 1 FROM `StocksSede` s 
    WHERE s.`InsumoId` = i.`Id` 
      AND s.`SedeId` = '10000000-0000-0000-0000-000000000001'
);

-- 8. Normalización de Estados en Médicos y Especialidades
UPDATE `Medicos` SET `Activo` = 1 WHERE `Activo` IS NULL;
UPDATE `Especialidades` SET `Activo` = 1 WHERE `Activo` IS NULL;

-- 9. Normalización de Cuentas por Cobrar
UPDATE `CuentasPorCobrar` SET `CompromisoGenerado` = 0 WHERE `CompromisoGenerado` IS NULL;
UPDATE `CuentasPorCobrar` SET `GarantiaGenerada` = 0 WHERE `GarantiaGenerada` IS NULL;
UPDATE `CuentasPorCobrar` SET `IsAudited` = 0 WHERE `IsAudited` IS NULL;

-- Limpieza del procedimiento auxiliar
DROP PROCEDURE IF EXISTS `AddColumnIfNotExists`;

SET FOREIGN_KEY_CHECKS = 1;

-- =========================================================================================
-- FIN DEL SCRIPT DE MIGRACIÓN Y CONVERSIÓN
-- =========================================================================================
