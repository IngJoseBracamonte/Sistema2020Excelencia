-- ============================================================================
-- SCRIPT DE MIGRACIÓN Y LIMPIEZA DE SEDES Y SUB-ÁREAS CLÍNICAS
-- SistemaSatHospitalario v2.0
-- ============================================================================

-- ----------------------------------------------------------------------------
-- FASE 1: DESVINCULAR DE CUENTAS DE SERVICIO CUALQUIER SUB-ÁREA DE SEDES SECUNDARIAS
-- ----------------------------------------------------------------------------

UPDATE CuentasServicios 
SET AreaClinicaId = NULL
WHERE AreaClinicaId IN (
    SELECT ac.Id FROM AreasClinicas ac JOIN Sedes s ON ac.SedeId = s.Id WHERE s.EsPrincipal = 0 OR s.Codigo = 'SUC_87908'
);

UPDATE DetallesServicioCuenta 
SET AreaClinicaId = NULL
WHERE AreaClinicaId IN (
    SELECT ac.Id FROM AreasClinicas ac JOIN Sedes s ON ac.SedeId = s.Id WHERE s.EsPrincipal = 0 OR s.Codigo = 'SUC_87908'
);

UPDATE CitasMedicas 
SET AreaClinicaId = NULL
WHERE AreaClinicaId IN (
    SELECT ac.Id FROM AreasClinicas ac JOIN Sedes s ON ac.SedeId = s.Id WHERE s.EsPrincipal = 0 OR s.Codigo = 'SUC_87908'
);

-- ----------------------------------------------------------------------------
-- FASE 2: ELIMINAR SUB-ÁREAS DE SEDES QUE NO SEAN EL ALMACÉN PRINCIPAL
-- (Sólo el Almacén Principal permite sub-áreas configurables)
-- ----------------------------------------------------------------------------

DELETE FROM HistorialesLimpiezasCamas WHERE CamaId IN (
    SELECT ac.Id FROM AreasClinicas ac JOIN Sedes s ON ac.SedeId = s.Id WHERE s.EsPrincipal = 0 OR s.Codigo = 'SUC_87908'
);

DELETE FROM AreasClinicas WHERE SedeId IN (
    SELECT Id FROM Sedes WHERE EsPrincipal = 0 OR Codigo = 'SUC_87908'
);

-- ----------------------------------------------------------------------------
-- FASE 3: ELIMINAR SEDE OBSOLETA SUC_87908
-- ----------------------------------------------------------------------------

DELETE FROM StocksSede WHERE SedeId IN (SELECT Id FROM Sedes WHERE Codigo = 'SUC_87908');
DELETE FROM Sedes WHERE Codigo = 'SUC_87908';

-- ----------------------------------------------------------------------------
-- FASE 4: GARANTIZAR LAS 4 SEDES OPERATIVAS BASE
-- (Almacén Principal, Área de Emergencia, Área de Hospitalización, UCI)
-- ----------------------------------------------------------------------------

INSERT INTO Sedes (Id, Codigo, Nombre, EsPrincipal, Activo)
VALUES ('10000000-0000-0000-0000-000000000001', 'PRINCIPAL', 'Almacen Principal', 1, 1)
ON DUPLICATE KEY UPDATE Nombre = 'Almacen Principal', EsPrincipal = 1, Activo = 1;

INSERT INTO Sedes (Id, Codigo, Nombre, EsPrincipal, Activo)
VALUES ('10000000-0000-0000-0000-000000000002', 'EMERGENCIA', 'Área de Emergencia', 0, 1)
ON DUPLICATE KEY UPDATE Nombre = 'Área de Emergencia', Activo = 1;

INSERT INTO Sedes (Id, Codigo, Nombre, EsPrincipal, Activo)
VALUES ('10000000-0000-0000-0000-000000000003', 'HOSPITALIZACION', 'Área de Hospitalización', 0, 1)
ON DUPLICATE KEY UPDATE Nombre = 'Área de Hospitalización', Activo = 1;

INSERT INTO Sedes (Id, Codigo, Nombre, EsPrincipal, Activo)
VALUES ('10000000-0000-0000-0000-000000000004', 'UCI', 'Unidad de Cuidados Intensivos', 0, 1)
ON DUPLICATE KEY UPDATE Nombre = 'Unidad de Cuidados Intensivos', Activo = 1;
