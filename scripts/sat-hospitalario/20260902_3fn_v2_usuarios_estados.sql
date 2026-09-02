-- ============================================================================
-- SCRIPT DE BACKFILL 3FN v2: USUARIOS, ESTADOS DE CITA Y MOTIVOS
-- Fecha: 02/09/2026
-- Scope: SatHospitalario only. Do not run against sistema2020 Legacy.
--
-- Prerequisito: aplicar la migración EF 20260902134737_Add3FnCatalogsAndUserFks
-- (este script es redundante si la migración ya corrió — es idempotente).
--
-- NO hace DROP de columnas legacy (delta posterior tras validación).
-- ============================================================================

SELECT DATABASE() AS database_en_uso;
-- Abortar manualmente si database_en_uso no es exactamente SatHospitalario.

DELIMITER $$

DROP PROCEDURE IF EXISTS Backfill3FnV2 $$

CREATE PROCEDURE Backfill3FnV2()
BEGIN
    -- ========================================================================
    -- 1. CitasMedicas.EstadoId desde el texto legacy Estado
    -- ========================================================================
    UPDATE `CitasMedicas` c
    JOIN `EstadosCitaMedica` e
      ON UPPER(TRIM(c.`Estado`)) = e.`Codigo`
      OR (e.`Codigo` = 'CANCELADA' AND UPPER(TRIM(c.`Estado`)) = 'CANCELADO')
    SET c.`EstadoId` = e.`Id`
    WHERE c.`EstadoId` IS NULL OR c.`EstadoId` = 0 OR c.`EstadoId` = 1;

    -- ========================================================================
    -- 2. CirugiasObservacionesHistorial.UsuarioRegistroId: anular no-GUIDs
    --    (la conversión longtext->char(36) deja texto truncado en inválidos)
    -- ========================================================================
    UPDATE `CirugiasObservacionesHistorial`
    SET `UsuarioRegistroId` = NULL
    WHERE `UsuarioRegistroId` IS NOT NULL
      AND (CHAR_LENGTH(`UsuarioRegistroId`) <> 36
           OR `UsuarioRegistroId` NOT REGEXP '^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$');

    -- ========================================================================
    -- 3. CuentasPorCobrar.UsuarioAuditoriaId desde username legacy
    -- ========================================================================
    UPDATE `CuentasPorCobrar` c
    JOIN `Usuarios` u ON c.`UsuarioAuditoria` = u.`UserName`
    SET c.`UsuarioAuditoriaId` = u.`Id`
    WHERE c.`UsuarioAuditoria` IS NOT NULL
      AND c.`UsuarioAuditoriaId` IS NULL;

    -- ========================================================================
    -- 4. CompromisosPago.UsuarioCreacionId desde username legacy
    -- ========================================================================
    UPDATE `CompromisosPago` c
    JOIN `Usuarios` u ON c.`UsuarioCreacion` = u.`UserName`
    SET c.`UsuarioCreacionId` = u.`Id`
    WHERE c.`UsuarioCreacion` IS NOT NULL
      AND c.`UsuarioCreacionId` IS NULL;

    -- ========================================================================
    -- 5. REPORTE DE VERIFICACIÓN
    -- ========================================================================
    SELECT
        (SELECT COUNT(*) FROM `CitasMedicas` WHERE `EstadoId` NOT IN (SELECT `Id` FROM `EstadosCitaMedica`)) AS CitasEstadoHuerfano,
        (SELECT COUNT(*) FROM `CirugiasObservacionesHistorial` WHERE `UsuarioRegistroId` IS NOT NULL) AS ObservacionesConFk,
        (SELECT COUNT(*) FROM `CuentasPorCobrar` WHERE `UsuarioAuditoriaId` IS NOT NULL) AS CuentasAuditadasConFk,
        (SELECT COUNT(*) FROM `CompromisosPago` WHERE `UsuarioCreacionId` IS NOT NULL) AS CompromisosConFk,
        (SELECT COUNT(*) FROM `EstadosCitaMedica` WHERE `Activo` = 1) AS EstadosCitaActivos,
        (SELECT COUNT(*) FROM `MotivosAutorizacion` WHERE `Activo` = 1) AS MotivosActivos;
END $$

DELIMITER ;

-- EJECUTAR BACKFILL Y LIMPIAR
CALL Backfill3FnV2();
DROP PROCEDURE IF EXISTS Backfill3FnV2;
