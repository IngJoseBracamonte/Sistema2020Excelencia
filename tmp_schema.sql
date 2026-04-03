ALTER DATABASE CHARACTER SET utf8mb4;


CREATE TABLE `BloqueosHorarios` (
    `Id` char(36) COLLATE ascii_general_ci NOT NULL,
    `MedicoId` char(36) COLLATE ascii_general_ci NOT NULL,
    `HoraPautada` datetime(6) NOT NULL,
    `Motivo` longtext CHARACTER SET utf8mb4 NOT NULL,
    `FechaRegistro` datetime(6) NOT NULL,
    CONSTRAINT `PK_BloqueosHorarios` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;


CREATE TABLE `CajasDiarias` (
    `Id` char(36) COLLATE ascii_general_ci NOT NULL,
    `FechaApertura` datetime(6) NOT NULL,
    `FechaCierre` datetime(6) NULL,
    `MontoInicialDivisa` decimal(18,2) NOT NULL,
    `MontoInicialBs` decimal(18,2) NOT NULL,
    `Estado` longtext CHARACTER SET utf8mb4 NOT NULL,
    `UsuarioId` longtext CHARACTER SET utf8mb4 NOT NULL,
    `NombreUsuario` longtext CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_CajasDiarias` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;


CREATE TABLE `ConfiguracionGeneral` (
    `Id` char(36) COLLATE ascii_general_ci NOT NULL,
    `NombreEmpresa` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Rif` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Iva` decimal(5,2) NOT NULL,
    `UltimaActualizacion` datetime(6) NOT NULL,
    CONSTRAINT `PK_ConfiguracionGeneral` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;


CREATE TABLE `CuentasServicios` (
    `Id` char(36) COLLATE ascii_general_ci NOT NULL,
    `PacienteId` char(36) COLLATE ascii_general_ci NOT NULL,
    `UsuarioCarga` longtext CHARACTER SET utf8mb4 NOT NULL,
    `FechaCarga` datetime(6) NOT NULL,
    `FechaCierre` datetime(6) NULL,
    `Estado` longtext CHARACTER SET utf8mb4 NOT NULL,
    `TipoIngreso` longtext CHARACTER SET utf8mb4 NOT NULL,
    `ConvenioId` int NULL,
    CONSTRAINT `PK_CuentasServicios` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;


CREATE TABLE `ErrorTickets` (
    `Id` char(36) COLLATE ascii_general_ci NOT NULL,
    `RequestPath` varchar(500) CHARACTER SET utf8mb4 NOT NULL,
    `MetodoHTTP` varchar(10) CHARACTER SET utf8mb4 NOT NULL,
    `MensajeExcepcion` longtext CHARACTER SET utf8mb4 NOT NULL,
    `StackTrace` longtext CHARACTER SET utf8mb4 NOT NULL,
    `UsuarioAsociado` longtext CHARACTER SET utf8mb4 NULL,
    `FechaCreacion` datetime(6) NOT NULL,
    `Resuelto` tinyint(1) NOT NULL,
    `ComentariosResolucion` longtext CHARACTER SET utf8mb4 NULL,
    `FechaResolucion` datetime(6) NULL,
    `ResueltoPor` longtext CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PK_ErrorTickets` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;


CREATE TABLE `Especialidades` (
    `Id` char(36) COLLATE ascii_general_ci NOT NULL,
    `Nombre` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Activo` tinyint(1) NOT NULL,
    CONSTRAINT `PK_Especialidades` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;


CREATE TABLE `IncidenciasHorario` (
    `Id` char(36) COLLATE ascii_general_ci NOT NULL,
    `MedicoId` char(36) COLLATE ascii_general_ci NOT NULL,
    `Inicio` datetime(6) NOT NULL,
    `Fin` datetime(6) NOT NULL,
    `Tipo` int NOT NULL,
    `Descripcion` longtext CHARACTER SET utf8mb4 NOT NULL,
    `CreadoPor` char(36) COLLATE ascii_general_ci NOT NULL,
    `FechaCreacion` datetime(6) NOT NULL,
    CONSTRAINT `PK_IncidenciasHorario` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;


CREATE TABLE `PacientesAdmision` (
    `Id` char(36) COLLATE ascii_general_ci NOT NULL,
    `IdPacienteLegacy` int NULL,
    `CedulaPasaporte` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `NombreCorto` longtext CHARACTER SET utf8mb4 NOT NULL,
    `TelefonoContact` longtext CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_PacientesAdmision` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;


CREATE TABLE `RegistroAuditoriaIncidencias` (
    `Id` char(36) COLLATE ascii_general_ci NOT NULL,
    `TurnoMedicoId` char(36) COLLATE ascii_general_ci NOT NULL,
    `IncidenciaIgnoradaId` char(36) COLLATE ascii_general_ci NOT NULL,
    `OperadorId` char(36) COLLATE ascii_general_ci NOT NULL,
    `FechaTraza` datetime(6) NOT NULL,
    `Motivo` longtext CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_RegistroAuditoriaIncidencias` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;


CREATE TABLE `ReservasTemporales` (
    `Id` char(36) COLLATE ascii_general_ci NOT NULL,
    `MedicoId` char(36) COLLATE ascii_general_ci NOT NULL,
    `HoraPautada` datetime(6) NOT NULL,
    `UsuarioId` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Comentario` longtext CHARACTER SET utf8mb4 NULL,
    `ExpiracionUtc` datetime(6) NOT NULL,
    CONSTRAINT `PK_ReservasTemporales` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;


CREATE TABLE `SegurosConvenios` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Nombre` varchar(200) CHARACTER SET utf8mb4 NOT NULL,
    `Rtn` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
    `Direccion` varchar(500) CHARACTER SET utf8mb4 NOT NULL,
    `Telefono` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
    `Email` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `Activo` tinyint(1) NOT NULL,
    CONSTRAINT `PK_SegurosConvenios` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;


CREATE TABLE `ServiciosClinicos` (
    `Id` char(36) COLLATE ascii_general_ci NOT NULL,
    `Codigo` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Descripcion` longtext CHARACTER SET utf8mb4 NOT NULL,
    `PrecioBase` decimal(18,2) NOT NULL,
    `TipoServicio` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Category` int NOT NULL,
    `Activo` tinyint(1) NOT NULL,
    CONSTRAINT `PK_ServiciosClinicos` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;


CREATE TABLE `tasacambio` (
    `Id` char(36) COLLATE ascii_general_ci NOT NULL,
    `Fecha` datetime(6) NOT NULL,
    `Monto` decimal(18,4) NOT NULL,
    `Activo` tinyint(1) NOT NULL,
    CONSTRAINT `PK_tasacambio` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;


CREATE TABLE `TurnosMedicos` (
    `Id` char(36) COLLATE ascii_general_ci NOT NULL,
    `MedicoId` char(36) COLLATE ascii_general_ci NOT NULL,
    `PacienteId` char(36) COLLATE ascii_general_ci NOT NULL,
    `FechaHoraToma` datetime(6) NOT NULL,
    `IgnorandoIncidencia` tinyint(1) NOT NULL,
    `IncidenciaIgnoradaId` char(36) COLLATE ascii_general_ci NULL,
    CONSTRAINT `PK_TurnosMedicos` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;


CREATE TABLE `CuentasPorCobrar` (
    `Id` char(36) COLLATE ascii_general_ci NOT NULL,
    `CuentaServicioId` char(36) COLLATE ascii_general_ci NOT NULL,
    `PacienteId` char(36) COLLATE ascii_general_ci NOT NULL,
    `MontoTotalBase` decimal(18,2) NOT NULL,
    `MontoPagadoBase` decimal(18,2) NOT NULL,
    `FechaCreacion` datetime(6) NOT NULL,
    `Estado` longtext CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_CuentasPorCobrar` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_CuentasPorCobrar_CuentasServicios_CuentaServicioId` FOREIGN KEY (`CuentaServicioId`) REFERENCES `CuentasServicios` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;


CREATE TABLE `DetallesServicioCuenta` (
    `Id` char(36) COLLATE ascii_general_ci NOT NULL,
    `CuentaServicioId` char(36) COLLATE ascii_general_ci NOT NULL,
    `ServicioId` char(36) COLLATE ascii_general_ci NOT NULL,
    `Descripcion` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Precio` decimal(18,2) NOT NULL,
    `Cantidad` int NOT NULL,
    `TipoServicio` longtext CHARACTER SET utf8mb4 NOT NULL,
    `UsuarioCarga` longtext CHARACTER SET utf8mb4 NOT NULL,
    `FechaCarga` datetime(6) NOT NULL,
    CONSTRAINT `PK_DetallesServicioCuenta` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_DetallesServicioCuenta_CuentasServicios_CuentaServicioId` FOREIGN KEY (`CuentaServicioId`) REFERENCES `CuentasServicios` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;


CREATE TABLE `RecibosFacturas` (
    `Id` char(36) COLLATE ascii_general_ci NOT NULL,
    `CuentaServicioId` char(36) COLLATE ascii_general_ci NOT NULL,
    `CajaDiariaId` char(36) COLLATE ascii_general_ci NULL,
    `NroControlFiscal` longtext CHARACTER SET utf8mb4 NULL,
    `TasaCambioDia` decimal(18,4) NOT NULL,
    `EstadoFiscal` longtext CHARACTER SET utf8mb4 NOT NULL,
    `PacienteId` char(36) COLLATE ascii_general_ci NOT NULL,
    `NumeroRecibo` longtext CHARACTER SET utf8mb4 NOT NULL,
    `TotalFacturadoUSD` decimal(65,30) NOT NULL,
    `MontoVueltoUSD` decimal(65,30) NOT NULL,
    `FechaEmision` datetime(6) NOT NULL,
    CONSTRAINT `PK_RecibosFacturas` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_RecibosFacturas_CajasDiarias_CajaDiariaId` FOREIGN KEY (`CajaDiariaId`) REFERENCES `CajasDiarias` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_RecibosFacturas_CuentasServicios_CuentaServicioId` FOREIGN KEY (`CuentaServicioId`) REFERENCES `CuentasServicios` (`Id`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4;


CREATE TABLE `Medicos` (
    `Id` char(36) COLLATE ascii_general_ci NOT NULL,
    `Nombre` longtext CHARACTER SET utf8mb4 NOT NULL,
    `EspecialidadId` char(36) COLLATE ascii_general_ci NOT NULL,
    `Activo` tinyint(1) NOT NULL,
    CONSTRAINT `PK_Medicos` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Medicos_Especialidades_EspecialidadId` FOREIGN KEY (`EspecialidadId`) REFERENCES `Especialidades` (`Id`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4;


CREATE TABLE `OrdenesDeServicio` (
    `Id` char(36) COLLATE ascii_general_ci NOT NULL,
    `NumeroLlegadaDiario` int NOT NULL,
    `PacienteId` char(36) COLLATE ascii_general_ci NOT NULL,
    `NombrePaciente` longtext CHARACTER SET utf8mb4 NOT NULL,
    `TipoIngreso` longtext CHARACTER SET utf8mb4 NOT NULL,
    `EstadoFacturacion` longtext CHARACTER SET utf8mb4 NOT NULL,
    `TotalCobrado` decimal(18,2) NOT NULL,
    `FechaCreacion` datetime(6) NOT NULL,
    `ConvenioId` int NULL,
    `Discriminator` varchar(21) CHARACTER SET utf8mb4 NOT NULL,
    `EstudioSolicitado` longtext CHARACTER SET utf8mb4 NULL,
    `Procesada` tinyint(1) NULL,
    `FechaProcesada` datetime(6) NULL,
    `AsistenteRxId` char(36) COLLATE ascii_general_ci NULL,
    CONSTRAINT `PK_OrdenesDeServicio` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_OrdenesDeServicio_PacientesAdmision_PacienteId` FOREIGN KEY (`PacienteId`) REFERENCES `PacientesAdmision` (`Id`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4;


CREATE TABLE `ConvenioPerfilPrecios` (
    `Id` char(36) COLLATE ascii_general_ci NOT NULL,
    `SeguroConvenioId` int NOT NULL,
    `PerfilId` int NOT NULL,
    `PrecioHNL` decimal(18,2) NOT NULL,
    `PrecioUSD` decimal(18,2) NOT NULL,
    `Activo` tinyint(1) NOT NULL,
    `UltimaActualizacion` datetime(6) NOT NULL,
    CONSTRAINT `PK_ConvenioPerfilPrecios` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_ConvenioPerfilPrecios_SegurosConvenios_SeguroConvenioId` FOREIGN KEY (`SeguroConvenioId`) REFERENCES `SegurosConvenios` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;


CREATE TABLE `PreciosServicioConvenio` (
    `Id` char(36) COLLATE ascii_general_ci NOT NULL,
    `ServicioClinicoId` char(36) COLLATE ascii_general_ci NOT NULL,
    `SeguroConvenioId` int NOT NULL,
    `PrecioDiferencial` decimal(18,2) NOT NULL,
    CONSTRAINT `PK_PreciosServicioConvenio` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_PreciosServicioConvenio_SegurosConvenios_SeguroConvenioId` FOREIGN KEY (`SeguroConvenioId`) REFERENCES `SegurosConvenios` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_PreciosServicioConvenio_ServiciosClinicos_ServicioClinicoId` FOREIGN KEY (`ServicioClinicoId`) REFERENCES `ServiciosClinicos` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;


CREATE TABLE `DetallesPago` (
    `Id` char(36) COLLATE ascii_general_ci NOT NULL,
    `ReciboFacturaId` char(36) COLLATE ascii_general_ci NOT NULL,
    `MetodoPago` longtext CHARACTER SET utf8mb4 NOT NULL,
    `ReferenciaBancaria` longtext CHARACTER SET utf8mb4 NOT NULL,
    `MontoAbonadoMoneda` decimal(18,2) NOT NULL,
    `EquivalenteAbonadoBase` decimal(18,2) NOT NULL,
    `FechaPago` datetime(6) NOT NULL,
    CONSTRAINT `PK_DetallesPago` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_DetallesPago_RecibosFacturas_ReciboFacturaId` FOREIGN KEY (`ReciboFacturaId`) REFERENCES `RecibosFacturas` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;


CREATE TABLE `CitasMedicas` (
    `Id` char(36) COLLATE ascii_general_ci NOT NULL,
    `MedicoId` char(36) COLLATE ascii_general_ci NOT NULL,
    `PacienteId` char(36) COLLATE ascii_general_ci NOT NULL,
    `CuentaServicioId` char(36) COLLATE ascii_general_ci NOT NULL,
    `HoraPautada` datetime(6) NOT NULL,
    `Estado` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Comentario` longtext CHARACTER SET utf8mb4 NULL,
    `FechaRegistro` datetime(6) NOT NULL,
    CONSTRAINT `PK_CitasMedicas` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_CitasMedicas_CuentasServicios_CuentaServicioId` FOREIGN KEY (`CuentaServicioId`) REFERENCES `CuentasServicios` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_CitasMedicas_Medicos_MedicoId` FOREIGN KEY (`MedicoId`) REFERENCES `Medicos` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;


CREATE UNIQUE INDEX `IX_BloqueosHorarios_MedicoId_HoraPautada` ON `BloqueosHorarios` (`MedicoId`, `HoraPautada`);


CREATE INDEX `IX_CitasMedicas_CuentaServicioId` ON `CitasMedicas` (`CuentaServicioId`);


CREATE INDEX `IX_CitasMedicas_MedicoId` ON `CitasMedicas` (`MedicoId`);


CREATE UNIQUE INDEX `IX_ConvenioPerfilPrecios_SeguroConvenioId_PerfilId` ON `ConvenioPerfilPrecios` (`SeguroConvenioId`, `PerfilId`);


CREATE INDEX `IX_CuentasPorCobrar_CuentaServicioId` ON `CuentasPorCobrar` (`CuentaServicioId`);


CREATE INDEX `IX_DetallesPago_ReciboFacturaId` ON `DetallesPago` (`ReciboFacturaId`);


CREATE INDEX `IX_DetallesServicioCuenta_CuentaServicioId` ON `DetallesServicioCuenta` (`CuentaServicioId`);


CREATE INDEX `IX_Medicos_EspecialidadId` ON `Medicos` (`EspecialidadId`);


CREATE INDEX `IX_OrdenesDeServicio_PacienteId` ON `OrdenesDeServicio` (`PacienteId`);


CREATE UNIQUE INDEX `IX_PacientesAdmision_CedulaPasaporte` ON `PacientesAdmision` (`CedulaPasaporte`);


CREATE UNIQUE INDEX `IX_PacientesAdmision_IdPacienteLegacy` ON `PacientesAdmision` (`IdPacienteLegacy`);


CREATE INDEX `IX_PreciosServicioConvenio_SeguroConvenioId` ON `PreciosServicioConvenio` (`SeguroConvenioId`);


CREATE INDEX `IX_PreciosServicioConvenio_ServicioClinicoId` ON `PreciosServicioConvenio` (`ServicioClinicoId`);


CREATE INDEX `IX_RecibosFacturas_CajaDiariaId` ON `RecibosFacturas` (`CajaDiariaId`);


CREATE INDEX `IX_RecibosFacturas_CuentaServicioId` ON `RecibosFacturas` (`CuentaServicioId`);


CREATE UNIQUE INDEX `IX_ReservasTemporales_MedicoId_HoraPautada` ON `ReservasTemporales` (`MedicoId`, `HoraPautada`);


