-- ============================================================================
-- SCRIPT NATIVO DE MIGRACIÓN 3FN PARA MYSQL 8.0 (SISTEMA SAT HOSPITALARIO)
-- Fecha: 01/09/2026
-- Scope: SatHospitalario only. Do not run against sistema2020 Legacy.
--
-- Versión encapsulada en stored procedure para compatibilidad con clientes
-- (MySQL Workbench, phpMyAdmin, DBeaver) que fallan con SQL dinámico
-- (PREPARE stmt) ejecutado de forma directa.
--
-- IMPORTANTE: Este script NO elimina columnas legacy (Insumos.Categoria,
-- CajasDiarias.DeclaracionCierreJson). El DROP se realiza en un delta
-- posterior tras validar en producción (ver plan 3FN, paso 4).
--
-- Prerequisitos (migraciones EF ya aplicadas):
--   20260829150724_InitialApplication        (CategoriasInsumo, CatalogoMetodosPago)
--   20260901124325_AddCategoriaInsumoReference (FK Insumos -> CategoriasInsumo)
--   20260901125821_NormalizeCajaClosingDeclarations (CajasDeclaracionesMetodos)
--
-- Tablas reales (difieren del borrador original del plan):
--   MetodosPago         -> CatalogoMetodosPago (Id CHAR(36), Nombre, Valor, EsUSD,
--                          EsVuelto, Activo, Orden, GrupoMoneda -> FK Monedas.Id)
--   DetallesCierreCaja  -> CajasDeclaracionesMetodos (ya creada por EF)
--   CierresCaja         -> CajasDiarias
--   CatalogItems        -> NO EXISTE como tabla (el catálogo se compone de
--                          ServiciosClinicos/Insumos/Medicos); la sección de
--   remapeo CatalogItemId del borrador original NO aplica.
-- ============================================================================

SELECT DATABASE() AS database_en_uso;
-- Abortar manualmente si database_en_uso no es exactamente SatHospitalario.

DELIMITER $$

DROP PROCEDURE IF EXISTS MigrarA3FN $$

CREATE PROCEDURE MigrarA3FN()
BEGIN
    -- ========================================================================
    -- 1. POBLAR CATEGORÍAS DE INSUMO DESDE EL TEXTO LEGACY (Insumos.Categoria)
    --    Idempotente: INSERT IGNORE sobre el índice único IX_CategoriasInsumo_Nombre.
    --    Id es CHAR(36) (Guid), se genera con UUID().
    -- ========================================================================
    INSERT IGNORE INTO `CategoriasInsumo` (`Id`, `Nombre`, `Codigo`, `Activo`, `FechaCreacion`)
    SELECT UUID(), TRIM(i.`Categoria`), NULL, 1, UTC_TIMESTAMP(6)
    FROM `Insumos` i
    WHERE i.`Categoria` IS NOT NULL AND TRIM(i.`Categoria`) <> '';

    -- ========================================================================
    -- 2. BACKFILL DE Insumos.CategoriaInsumoId
    --    Solo filas sin FK y con coincidencia única de categoría activa
    --    (case/space-insensitive). Filas ambiguas quedan intactas para
    --    resolución manual (ver reporte en 20260901_categoria_insumo_normalizacion.sql).
    -- ========================================================================
    UPDATE `Insumos` i
    INNER JOIN `CategoriasInsumo` c
        ON c.`Activo` = 1
       AND UPPER(TRIM(c.`Nombre`)) = UPPER(TRIM(i.`Categoria`))
    LEFT JOIN `CategoriasInsumo` duplicate
        ON duplicate.`Activo` = 1
       AND UPPER(TRIM(duplicate.`Nombre`)) = UPPER(TRIM(i.`Categoria`))
       AND duplicate.`Id` <> c.`Id`
    SET i.`CategoriaInsumoId` = c.`Id`,
        i.`Categoria` = c.`Nombre`  -- alias de compatibilidad con nombre canónico
    WHERE i.`CategoriaInsumoId` IS NULL
      AND duplicate.`Id` IS NULL;

    -- ========================================================================
    -- 3. FK Insumos -> CategoriasInsumo (normalmente ya creada por la
    --    migración EF 20260901124325; el guard la vuelve no-op).
    -- ========================================================================
    IF NOT EXISTS (
        SELECT 1 FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
        WHERE CONSTRAINT_SCHEMA = DATABASE()
          AND TABLE_NAME = 'Insumos'
          AND CONSTRAINT_NAME = 'FK_Insumos_CategoriasInsumo_CategoriaInsumoId'
    ) AND NOT EXISTS (
        SELECT 1 FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
        WHERE CONSTRAINT_SCHEMA = DATABASE()
          AND TABLE_NAME = 'Insumos'
          AND CONSTRAINT_NAME = 'FK_Insumos_CategoriasInsumo'
    ) THEN
        ALTER TABLE `Insumos`
        ADD CONSTRAINT `FK_Insumos_CategoriasInsumo`
        FOREIGN KEY (`CategoriaInsumoId`) REFERENCES `CategoriasInsumo`(`Id`);
    END IF;

    -- ========================================================================
    -- 4. SEED DE CatalogoMetodosPago (solo si la tabla está vacía; el seed
    --    operativo lo realiza el backend. GrupoMoneda: 1 = USD, 2 = Bs —
    --    verificar contra la tabla Monedas antes de usar en producción).
    -- ========================================================================
    IF NOT EXISTS (SELECT 1 FROM `CatalogoMetodosPago` LIMIT 1) THEN
        INSERT INTO `CatalogoMetodosPago`
            (`Id`, `Nombre`, `Valor`, `EsUSD`, `EsVuelto`, `Activo`, `Orden`, `GrupoMoneda`) VALUES
        (UUID(), 'EFECTIVO DOLAR ($)',      'Dolar Efectivo',        1, 0, 1, 1, 1),
        (UUID(), 'EFECTIVO BOLIVARES (Bs)', 'Bolivar Efectivo',      0, 0, 1, 2, 2),
        (UUID(), 'PAGO MOVIL (Bs)',         'Pago Movil',            0, 0, 1, 3, 2),
        (UUID(), 'TRANSFERENCIA (Bs)',      'Transferencia',         0, 0, 1, 4, 2),
        (UUID(), 'PUNTO DE VENTA (Bs)',     'Punto de Venta',        0, 0, 1, 5, 2),
        (UUID(), 'ZELLE ($)',               'Zelle',                 1, 0, 1, 6, 1),
        (UUID(), 'VUELTO DOLAR ($)',        'Vuelto Dolar',          1, 1, 1, 7, 1),
        (UUID(), 'VUELTO BOLIVARES (Bs)',   'Vuelto Bolivar',        0, 1, 1, 8, 2);
    END IF;

    -- ========================================================================
    -- 5. LIMPIEZA DEL RESIDUO CajasDiarias.DeclaracionCierreJson
    --    La columna queda (sin DROP) pero se vacía: los datos ya viven en
    --    CajasDeclaracionesMetodos (ver 20260901_cierre_caja_json_normalizacion.sql).
    --    Solo limpia cajas cuyo JSON ya fue migrado (existe al menos una
    --    declaración normalizada) o cuyo JSON está vacío/inválido.
    -- ========================================================================
    UPDATE `CajasDiarias` c
    SET c.`DeclaracionCierreJson` = NULL
    WHERE c.`DeclaracionCierreJson` IS NOT NULL
      AND (
            c.`DeclaracionCierreJson` = ''
            OR JSON_VALID(c.`DeclaracionCierreJson`) = 0
            OR EXISTS (
                SELECT 1 FROM `CajasDeclaracionesMetodos` d
                WHERE d.`CajaDiariaId` = c.`Id`
            )
          );

    -- ========================================================================
    -- 6. ÍNDICE DE APOYO (guard contra duplicados vía INFORMATION_SCHEMA)
    -- ========================================================================
    IF NOT EXISTS (
        SELECT 1 FROM INFORMATION_SCHEMA.STATISTICS
        WHERE TABLE_SCHEMA = DATABASE()
          AND TABLE_NAME = 'Insumos'
          AND INDEX_NAME = 'IX_Insumos_CategoriaInsumoId'
    ) THEN
        CREATE INDEX `IX_Insumos_CategoriaInsumoId` ON `Insumos` (`CategoriaInsumoId`);
    END IF;

    -- ========================================================================
    -- 7. REPORTE FINAL DE VERIFICACIÓN
    -- ========================================================================
    SELECT
        (SELECT COUNT(*) FROM `Insumos` WHERE `CategoriaInsumoId` IS NULL AND `IsDeleted` = 0) AS InsumosSinCategoriaFk,
        (SELECT COUNT(*) FROM `CategoriasInsumo` WHERE `Activo` = 1) AS CategoriasActivas,
        (SELECT COUNT(*) FROM `CatalogoMetodosPago` WHERE `Activo` = 1) AS MetodosPagoActivos,
        (SELECT COUNT(*) FROM `CajasDiarias` WHERE `DeclaracionCierreJson` IS NOT NULL) AS CajasConJsonResidual;
END $$

DELIMITER ;

-- EJECUTAR MIGRACIÓN Y LIMPIAR
CALL MigrarA3FN();
DROP PROCEDURE IF EXISTS MigrarA3FN;
