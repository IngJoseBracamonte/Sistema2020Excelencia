-- Scope: SatHospitalario only. Do not run against sistema2020 Legacy.
-- Prerequisite: review before applying migration 20260901130739_AddAuditLogReferences.

SELECT DATABASE() AS database_en_uso;

-- Abort manually unless database_en_uso is exactly SatHospitalario.

-- 1. Price audit logs with missing service-detail references or stale descriptions.
SELECT
    a.Id AS AuditLogPrecioId,
    a.DetalleServicioId,
    a.DescripcionServicio AS DescripcionHistorica,
    d.Descripcion AS DescripcionCanonica,
    CASE WHEN d.Id IS NULL THEN 'HUERFANO'
         WHEN UPPER(TRIM(a.DescripcionServicio)) <> UPPER(TRIM(d.Descripcion)) THEN 'DIVERGENTE'
         ELSE 'OK' END AS Estado
FROM AuditLogsPrecios a
LEFT JOIN DetallesServicioCuenta d ON d.Id = a.DetalleServicioId
WHERE d.Id IS NULL
   OR UPPER(TRIM(a.DescripcionServicio)) <> UPPER(TRIM(d.Descripcion));

-- 2. Honorarium assignment logs with orphan references or copied names that differ from master data.
SELECT
    l.Id AS LogAsignacionId,
    l.DetalleServicioId,
    l.NombreServicio AS NombreServicioHistorico,
    d.Descripcion AS NombreServicioCanonico,
    l.MedicoAnteriorId,
    l.MedicoAnteriorNombre,
    ma.Nombre AS MedicoAnteriorCanonico,
    l.MedicoNuevoId,
    l.MedicoNuevoNombre,
    mn.Nombre AS MedicoNuevoCanonico
FROM LogsAsignacionHonorario l
LEFT JOIN DetallesServicioCuenta d ON d.Id = l.DetalleServicioId
LEFT JOIN Medicos ma ON ma.Id = l.MedicoAnteriorId
LEFT JOIN Medicos mn ON mn.Id = l.MedicoNuevoId
WHERE d.Id IS NULL
   OR (l.MedicoAnteriorId IS NOT NULL AND ma.Id IS NULL)
   OR (l.MedicoNuevoId IS NOT NULL AND mn.Id IS NULL)
   OR (d.Id IS NOT NULL AND UPPER(TRIM(l.NombreServicio)) <> UPPER(TRIM(d.Descripcion)))
   OR (ma.Id IS NOT NULL AND COALESCE(UPPER(TRIM(l.MedicoAnteriorNombre)), '') <> UPPER(TRIM(ma.Nombre)))
   OR (mn.Id IS NOT NULL AND COALESCE(UPPER(TRIM(l.MedicoNuevoNombre)), '') <> UPPER(TRIM(mn.Nombre)));

-- Do not update copied labels automatically. They are prohibited snapshots under
-- the policy and must be removed only after all readers derive names by FK.