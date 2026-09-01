-- Scope: SatHospitalario only. Do not run against sistema2020 Legacy.
-- Prerequisite: apply EF migration 20260901125821_NormalizeCajaClosingDeclarations.
-- This script does not drop CajasDiarias.DeclaracionCierreJson.

SELECT DATABASE() AS database_en_uso;

-- Abort manually unless database_en_uso is exactly SatHospitalario.

-- 1. Identify non-empty declarations that cannot be parsed as a JSON array.
SELECT Id AS CajaDiariaId, FechaCierre, DeclaracionCierreJson
FROM CajasDiarias
WHERE DeclaracionCierreJson IS NOT NULL
  AND DeclaracionCierreJson <> ''
  AND JSON_VALID(DeclaracionCierreJson) = 0;

-- 2. Identify parsed rows with no exactly-one active payment-method match.
SELECT
    c.Id AS CajaDiariaId,
    j.MetodoPago,
    COUNT(m.Id) AS CoincidenciasActivas,
    GROUP_CONCAT(m.Id ORDER BY m.Id SEPARATOR ',') AS MetodoCandidatoIds
FROM CajasDiarias c
JOIN JSON_TABLE(
    c.DeclaracionCierreJson,
    '$[*]' COLUMNS (
        MetodoPago VARCHAR(150) PATH '$.MetodoPago'
    )
) j
LEFT JOIN CatalogoMetodosPago m
    ON m.Activo = 1
   AND (UPPER(TRIM(m.Valor)) = UPPER(TRIM(j.MetodoPago))
        OR UPPER(TRIM(m.Nombre)) = UPPER(TRIM(j.MetodoPago)))
WHERE c.DeclaracionCierreJson IS NOT NULL
  AND c.DeclaracionCierreJson <> ''
  AND JSON_VALID(c.DeclaracionCierreJson) = 1
GROUP BY c.Id, j.MetodoPago
HAVING COUNT(m.Id) <> 1;

-- 3. Controlled backfill. Execute only after reviewing reports 1 and 2.
-- START TRANSACTION;
-- INSERT INTO CajasDeclaracionesMetodos (
--     Id, CajaDiariaId, MetodoPagoId, MontoIngresado, MontoVueltos,
--     MontoEsperadoIngreso, MontoEsperadoVueltos, DiferenciaOriginal, DiferenciaBase)
-- SELECT
--     UUID(),
--     c.Id,
--     m.Id,
--     j.MontoIngreso,
--     j.MontoVueltos,
--     j.MontoEsperadoIngreso,
--     j.MontoEsperadoVueltos,
--     j.DiferenciaOriginal,
--     j.DiferenciaBase
-- FROM CajasDiarias c
-- JOIN JSON_TABLE(
--     c.DeclaracionCierreJson,
--     '$[*]' COLUMNS (
--         MetodoPago VARCHAR(150) PATH '$.MetodoPago',
--         MontoIngreso DECIMAL(18,2) PATH '$.MontoIngreso',
--         MontoVueltos DECIMAL(18,2) PATH '$.MontoVueltos',
--         MontoEsperadoIngreso DECIMAL(18,2) PATH '$.MontoEsperadoIngreso',
--         MontoEsperadoVueltos DECIMAL(18,2) PATH '$.MontoEsperadoVueltos',
--         DiferenciaOriginal DECIMAL(18,2) PATH '$.DiferenciaOriginal',
--         DiferenciaBase DECIMAL(18,2) PATH '$.DiferenciaBase'
--     )
-- ) j
-- JOIN CatalogoMetodosPago m
--     ON m.Activo = 1
--    AND (UPPER(TRIM(m.Valor)) = UPPER(TRIM(j.MetodoPago))
--         OR UPPER(TRIM(m.Nombre)) = UPPER(TRIM(j.MetodoPago)))
-- LEFT JOIN CatalogoMetodosPago duplicate
--     ON duplicate.Activo = 1
--    AND (UPPER(TRIM(duplicate.Valor)) = UPPER(TRIM(j.MetodoPago))
--         OR UPPER(TRIM(duplicate.Nombre)) = UPPER(TRIM(j.MetodoPago)))
--    AND duplicate.Id <> m.Id
-- LEFT JOIN CajasDeclaracionesMetodos existing
--     ON existing.CajaDiariaId = c.Id
--    AND existing.MetodoPagoId = m.Id
-- WHERE JSON_VALID(c.DeclaracionCierreJson) = 1
--   AND duplicate.Id IS NULL
--   AND existing.Id IS NULL;
-- SELECT ROW_COUNT() AS filas_insertadas;
-- COMMIT;