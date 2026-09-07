# Análisis: `CuentaServicios.TipoIngreso`

## 1. Identificación

| Campo | Valor |
|-------|-------|
| **Entidad** | `CuentaServicios` |
| **Propiedad obsoleta** | `TipoIngreso` |
| **Reemplazo** | `TipoIngresoId / TipoIngresoNav` |
| **Mensaje de obsolescencia** | "Usar TipoIngresoId / TipoIngresoNav. Columna legacy pendiente de DROP." |
| **Archivo de definición** | `src/SistemaSatHospitalario.Core.Domain/Entities/Admision/CuentaServicios.cs` |

## 2. Impacto en Backend

**Usos detectados en el build (CS0618):**
- `Common/Strategies/LegacyLabLoadingStrategy.cs` (líneas 55-57, 129)
- `Common/Strategies/ImagingLoadingStrategy.cs` (líneas 56, 158)
- `Common/Strategies/ConsultationLoadingStrategy.cs` (línea 86)
- `Common/Services/InventoryService.cs` (línea 96)
- `Commands/Admision/UpdateCuentaAdministrativaCommand.cs` (líneas 63, 286)
- `Commands/Admision/CargarServicioACuentaCommand.cs` (línea 309)
- `Commands/Admision/AbrirCuentaClinicaCommandHandler.cs` (línea 49)
- `Queries/Admision/GetTechnicalValidationListQuery.cs` (líneas 105, 123)
- `Queries/Admision/GetPendingARQueryHandler.cs` (línea 40)
- `Queries/Admision/GetPatientHistoryQuery.cs` (línea 38)
- `Queries/Admision/GetPacientesQuirurgicosListaQuery.cs` (línea 177)
- `Queries/Admision/GetPabellonCalendarioQuery.cs` (línea 128)
- `Queries/Admision/GetOpenAccountQueryHandler.cs` (líneas 33, 80, 104)
- `Queries/Admision/GetCuentasAdministrativasQuery.cs` (líneas 50, 54, 94)
- `Queries/Admision/GetControlCitasQuery.cs` (línea 114)
- `Queries/Admision/GetReciboPdfQuery.cs` (línea 51)
- `Queries/Admision/GetExpedienteFacturacionQuery.cs` (línea 129)
- `Queries/Admision/GetBusinessInsightsQuery.cs` (líneas 232, 251-253)
- `Infrastructure/Persistence/Contexts/SatHospitalarioDbContext.cs` (línea 524)

**Tipo de cambio:** Migrar de `TipoIngreso` (string) a `TipoIngresoId` (int FK) + `TipoIngresoNav`.

## 3. Impacto en Frontend

- Verificar si la API expone `tipoIngreso` (string) en los DTOs de cuentas. Migrar a `tipoIngresoId`.

## 4. Análisis paso a paso

- [ ] 1. Identificar todos los usos de `CuentaServicios.TipoIngreso`.
- [ ] 2. Verificar que `TipoIngresoId` y `TipoIngresoNav` existen en la entidad.
- [ ] 3. Migrar comparaciones de string a comparaciones de `TipoIngresoId`.
- [ ] 4. Actualizar DTOs y frontend.
- [ ] 5. Compilar backend y frontend.
- [ ] 6. Crear migración para eliminar la columna `TipoIngreso`.

## 5. Verificación

- `dotnet build` sin CS0618 para `CuentaServicios.TipoIngreso`.
- El flujo de ingreso (particular, seguro, etc.) funciona correctamente.

## 6. Riesgos

- Es una de las propiedades con más usos. Requiere migración cuidadosa.
- Los valores de `TipoIngreso` (string) deben mapearse a los `TipoIngresoId` correctos.