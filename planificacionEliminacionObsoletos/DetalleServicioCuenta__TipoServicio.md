# Análisis: `DetalleServicioCuenta.TipoServicio`

## 1. Identificación

| Campo | Valor |
|-------|-------|
| **Entidad** | `DetalleServicioCuenta` |
| **Propiedad obsoleta** | `TipoServicio` |
| **Reemplazo** | `TipoServicioId / TipoServicioNav` |
| **Mensaje de obsolescencia** | "Usar TipoServicioId / TipoServicioNav. Columna legacy pendiente de DROP." |
| **Archivo de definición** | `src/SistemaSatHospitalario.Core.Domain/Entities/Admision/DetalleServicioCuenta.cs` |

## 2. Impacto en Backend

**Usos detectados en el build (CS0618):**
- `Queries/Admision/GetWorklistQuery.cs` (líneas 108, 117)
- `Queries/Admision/GetPatientHistoryQuery.cs` (línea 45)
- `Queries/Admision/GetOpenAccountQueryHandler.cs` (línea 91)
- `Queries/Admision/GetNurseAuditReportQueryHandler.cs` (línea 95)
- `Queries/Admin/GetServiciosSinAsignarQuery.cs` (líneas 44, 69)
- `Queries/Admision/ExportGenericListQuery.cs` (líneas 123, 145, 202)
- `Queries/Admision/GetExpedienteFacturacionQuery.cs` (línea 137)
- `Queries/Admin/GetDoctorHonorariumSummaryQuery.cs` (líneas 51-54, 78-81)
- `Queries/Admision/GetBusinessInsightsQuery.cs` (línea 105)
- `Commands/Admision/CloseAccountCommandHandler.cs` (líneas 172, 328, 395, 404, 408)

**Tipo de cambio:** Migrar de `TipoServicio` (string) a `TipoServicioId` (int FK) + `TipoServicioNav`.

## 3. Impacto en Frontend

- Verificar si la API expone `tipoServicio` (string) en los DTOs de servicios. Migrar a `tipoServicioId`.

## 4. Análisis paso a paso

- [ ] 1. Identificar todos los usos de `DetalleServicioCuenta.TipoServicio`.
- [ ] 2. Verificar que `TipoServicioId` y `TipoServicioNav` existen en la entidad.
- [ ] 3. Migrar comparaciones de string a comparaciones de `TipoServicioId`.
- [ ] 4. Actualizar DTOs y frontend.
- [ ] 5. Compilar backend y frontend.
- [ ] 6. Crear migración para eliminar la columna `TipoServicio`.

## 5. Verificación

- `dotnet build` sin CS0618 para `DetalleServicioCuenta.TipoServicio`.
- El flujo de servicios (consulta, laboratorio, imagen) funciona correctamente.

## 6. Riesgos

- Es una de las propiedades con más usos. Requiere migración cuidadosa.
- Los valores de `TipoServicio` (string) deben mapearse a los `TipoServicioId` correctos.