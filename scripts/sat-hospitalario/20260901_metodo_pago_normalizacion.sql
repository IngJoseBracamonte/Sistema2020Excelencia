-- Scope: SatHospitalario only. Do not run against sistema2020 Legacy.
-- Prerequisite: apply EF migration 20260901125218_AddMetodoPagoReferenceToDetallePago.
-- MetodoPago remains a read-compatibility alias until all consumers use MetodoPagoId.

SELECT DATABASE() AS database_en_uso;

-- Abort manually unless database_en_uso is exactly SatHospitalario.

-- 1. Payment details that cannot be mapped to exactly one active catalog method.
SELECT
    d.Id AS DetallePagoId,
    d.ReciboFacturaId,
    d.MetodoPago AS MetodoPagoTexto,
    d.MetodoPagoId,
    COUNT(m.Id) AS CoincidenciasActivas,
    GROUP_CONCAT(m.Id ORDER BY m.Id SEPARATOR ',') AS MetodoCandidatoIds
FROM DetallesPago d
LEFT JOIN CatalogoMetodosPago m
    ON m.Activo = 1
   AND (UPPER(TRIM(m.Valor)) = UPPER(TRIM(d.MetodoPago))
        OR UPPER(TRIM(m.Nombre)) = UPPER(TRIM(d.MetodoPago)))
WHERE d.MetodoPagoId IS NULL
GROUP BY d.Id, d.ReciboFacturaId, d.MetodoPago, d.MetodoPagoId
HAVING COUNT(m.Id) <> 1
ORDER BY d.FechaPago;

-- 2. Existing IDs that are orphaned, inactive, or disagree with canonical Valor.
SELECT
    d.Id AS DetallePagoId,
    d.ReciboFacturaId,
    d.MetodoPagoId,
    d.MetodoPago AS MetodoPagoTexto,
    m.Valor AS MetodoPagoCanonico,
    m.Activo AS MetodoActivo
FROM DetallesPago d
LEFT JOIN CatalogoMetodosPago m ON m.Id = d.MetodoPagoId
WHERE d.MetodoPagoId IS NOT NULL
  AND (m.Id IS NULL OR m.Activo = 0 OR UPPER(TRIM(d.MetodoPago)) <> UPPER(TRIM(m.Valor)))
ORDER BY d.FechaPago;

-- 3. Controlled backfill. Execute only after reviewing both reports.
-- START TRANSACTION;
-- UPDATE DetallesPago d
-- INNER JOIN CatalogoMetodosPago m
--     ON m.Activo = 1
--    AND (UPPER(TRIM(m.Valor)) = UPPER(TRIM(d.MetodoPago))
--         OR UPPER(TRIM(m.Nombre)) = UPPER(TRIM(d.MetodoPago)))
-- LEFT JOIN CatalogoMetodosPago duplicate
--     ON duplicate.Activo = 1
--    AND (UPPER(TRIM(duplicate.Valor)) = UPPER(TRIM(d.MetodoPago))
--         OR UPPER(TRIM(duplicate.Nombre)) = UPPER(TRIM(d.MetodoPago)))
--    AND duplicate.Id <> m.Id
-- SET d.MetodoPagoId = m.Id,
--     d.MetodoPago = m.Valor
-- WHERE d.MetodoPagoId IS NULL
--   AND duplicate.Id IS NULL;
-- SELECT ROW_COUNT() AS filas_actualizadas;
-- COMMIT;