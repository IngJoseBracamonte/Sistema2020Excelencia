-- Scope: SatHospitalario only. Do not run against sistema2020 Legacy.
-- Prerequisite: apply EF migration 20260901124325_AddCategoriaInsumoReference.
-- This script does not remove Insumos.Categoria; that happens only after all
-- consumers use CategoriaInsumoId and the exception report is empty.

SELECT DATABASE() AS database_en_uso;

-- Abort manually unless database_en_uso is exactly SatHospitalario.

-- 1. Diagnostic report: rows that cannot be mapped safely by a unique active category name.
SELECT
    i.Id AS InsumoId,
    i.Codigo AS InsumoCodigo,
    i.Nombre AS InsumoNombre,
    i.Categoria AS CategoriaTexto,
    i.CategoriaInsumoId,
    COUNT(c.Id) AS CoincidenciasActivas,
    GROUP_CONCAT(c.Id ORDER BY c.Id SEPARATOR ',') AS CategoriaCandidataIds
FROM Insumos i
LEFT JOIN CategoriasInsumo c
    ON c.Activo = 1
   AND UPPER(TRIM(c.Nombre)) = UPPER(TRIM(i.Categoria))
WHERE i.CategoriaInsumoId IS NULL
GROUP BY i.Id, i.Codigo, i.Nombre, i.Categoria, i.CategoriaInsumoId
HAVING COUNT(c.Id) <> 1
ORDER BY i.Codigo;

-- 2. Diagnostic report: IDs that do not resolve to an active category or
-- whose compatibility alias diverges from the canonical category name.
SELECT
    i.Id AS InsumoId,
    i.Codigo AS InsumoCodigo,
    i.Categoria AS CategoriaTexto,
    i.CategoriaInsumoId,
    c.Nombre AS CategoriaCanonica,
    c.Activo AS CategoriaActiva
FROM Insumos i
LEFT JOIN CategoriasInsumo c ON c.Id = i.CategoriaInsumoId
WHERE i.CategoriaInsumoId IS NOT NULL
  AND (c.Id IS NULL OR c.Activo = 0 OR UPPER(TRIM(i.Categoria)) <> UPPER(TRIM(c.Nombre)))
ORDER BY i.Codigo;

-- 3. Controlled backfill. Execute only after reviewing both reports above.
-- It updates only rows with a unique active category match; ambiguous or
-- orphan rows remain untouched for explicit resolution.
-- START TRANSACTION;
-- UPDATE Insumos i
-- INNER JOIN CategoriasInsumo c
--     ON c.Activo = 1
--    AND UPPER(TRIM(c.Nombre)) = UPPER(TRIM(i.Categoria))
-- LEFT JOIN CategoriasInsumo duplicate
--     ON duplicate.Activo = 1
--    AND UPPER(TRIM(duplicate.Nombre)) = UPPER(TRIM(i.Categoria))
--    AND duplicate.Id <> c.Id
-- SET i.CategoriaInsumoId = c.Id,
--     i.Categoria = c.Nombre
-- WHERE i.CategoriaInsumoId IS NULL
--   AND duplicate.Id IS NULL;
-- SELECT ROW_COUNT() AS filas_actualizadas;
-- COMMIT;