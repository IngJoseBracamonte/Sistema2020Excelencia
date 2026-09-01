-- Scope: SatHospitalario only. Do not run against sistema2020 Legacy.
-- Prerequisite: apply EF migration 20260901124719_AddProveedorReferenceToOrdenCompra.
-- ProveedorNombre remains a read-compatibility alias until this report is empty
-- and all consumers have migrated to ProveedorId.

SELECT DATABASE() AS database_en_uso;

-- Abort manually unless database_en_uso is exactly SatHospitalario.

-- 1. Orders that cannot be safely linked to exactly one active provider.
SELECT
    o.Id AS OrdenCompraId,
    o.NumeroFactura,
    o.ProveedorNombre,
    o.ProveedorId,
    COUNT(p.Id) AS CoincidenciasActivas,
    GROUP_CONCAT(p.Id ORDER BY p.Id SEPARATOR ',') AS ProveedorCandidatoIds
FROM OrdenesCompraInventario o
LEFT JOIN Proveedores p
    ON p.Activo = 1
   AND UPPER(TRIM(p.RazonSocial)) = UPPER(TRIM(o.ProveedorNombre))
WHERE o.ProveedorId IS NULL
GROUP BY o.Id, o.NumeroFactura, o.ProveedorNombre, o.ProveedorId
HAVING COUNT(p.Id) <> 1
ORDER BY o.NumeroFactura;

-- 2. Existing provider IDs that are orphaned, inactive, or disagree with the alias.
SELECT
    o.Id AS OrdenCompraId,
    o.NumeroFactura,
    o.ProveedorId,
    o.ProveedorNombre,
    p.RazonSocial AS ProveedorCanonico,
    p.Activo AS ProveedorActivo
FROM OrdenesCompraInventario o
LEFT JOIN Proveedores p ON p.Id = o.ProveedorId
WHERE o.ProveedorId IS NOT NULL
  AND (p.Id IS NULL OR p.Activo = 0 OR UPPER(TRIM(o.ProveedorNombre)) <> UPPER(TRIM(p.RazonSocial)))
ORDER BY o.NumeroFactura;

-- 3. Controlled backfill. Execute only after reviewing both reports.
-- START TRANSACTION;
-- UPDATE OrdenesCompraInventario o
-- INNER JOIN Proveedores p
--     ON p.Activo = 1
--    AND UPPER(TRIM(p.RazonSocial)) = UPPER(TRIM(o.ProveedorNombre))
-- LEFT JOIN Proveedores duplicate
--     ON duplicate.Activo = 1
--    AND UPPER(TRIM(duplicate.RazonSocial)) = UPPER(TRIM(o.ProveedorNombre))
--    AND duplicate.Id <> p.Id
-- SET o.ProveedorId = p.Id,
--     o.ProveedorNombre = p.RazonSocial
-- WHERE o.ProveedorId IS NULL
--   AND duplicate.Id IS NULL;
-- SELECT ROW_COUNT() AS filas_actualizadas;
-- COMMIT;