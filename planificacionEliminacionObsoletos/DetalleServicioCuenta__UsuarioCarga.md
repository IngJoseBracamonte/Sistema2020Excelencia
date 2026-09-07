# Análisis: `DetalleServicioCuenta.UsuarioCarga`

## 1. Identificación

| Campo | Valor |
|-------|-------|
| **Entidad** | `DetalleServicioCuenta` |
| **Propiedad obsoleta** | `UsuarioCarga` |
| **Reemplazo** | `UsuarioCargaId` |
| **Mensaje de obsolescencia** | "Usar UsuarioCargaId. Columna legacy pendiente de DROP." |
| **Archivo de definición** | `src/SistemaSatHospitalario.Core.Domain/Entities/Admision/DetalleServicioCuenta.cs` |

## 2. Impacto en Backend

**Usos detectados en el build (CS0618):**
- `Queries/Admision/GetNurseAuditReportQueryHandler.cs` (líneas 56, 92)
- `Queries/Admision/ExportFinancialReportQuery.cs` (líneas 86, 117, 145)
- `Queries/Admision/ExportGenericListQuery.cs` (líneas 151, 184)
- `Queries/Admision/GetExpedienteFacturacionQuery.cs` (líneas 104, 168, 188)

**Tipo de cambio:** Migrar de `UsuarioCarga` (string) a `UsuarioCargaId` (Guid).

## 3. Impacto en Frontend

- Verificar si la API expone `usuarioCarga`. Migrar a `usuarioCargaId`.

## 4. Análisis paso a paso

- [ ] 1. Identificar todos los usos de `DetalleServicioCuenta.UsuarioCarga`.
- [ ] 2. Verificar que `UsuarioCargaId` existe en la entidad.
- [ ] 3. Migrar asignaciones/lecturas de `UsuarioCarga` → `UsuarioCargaId`.
- [ ] 4. Actualizar DTOs y frontend.
- [ ] 5. Compilar backend y frontend.
- [ ] 6. Crear migración para eliminar la columna `UsuarioCarga`.

## 5. Verificación

- `dotnet build` sin CS0618 para `DetalleServicioCuenta.UsuarioCarga`.
- La auditoría de servicios muestra el usuario de carga correcto.

## 6. Riesgos

- Requiere backfill de `UsuarioCargaId` para servicios legacy.