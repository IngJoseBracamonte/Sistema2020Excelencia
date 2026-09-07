# Análisis: `CuentaServicios.Estado`

## 1. Identificación

| Campo | Valor |
|-------|-------|
| **Entidad** | `CuentaServicios` |
| **Propiedad obsoleta** | `Estado` |
| **Reemplazo** | `EstadoId / EstadoNav` |
| **Mensaje de obsolescencia** | "Usar EstadoId / EstadoNav. Columna legacy pendiente de DROP." |
| **Archivo de definición** | `src/SistemaSatHospitalario.Core.Domain/Entities/Admision/CuentaServicios.cs` |

## 2. Impacto en Backend

**Usos detectados en el build (CS0618):**
- `Commands/Admision/RegistrarCambioCamaCommandHandler.cs` (línea 23)
- `Commands/Admision/RegistrarReciboFacturaCommand.cs` (línea 56)
- `Commands/Admision/RegistrarTrasladoAreaCommandHandler.cs` (línea 25)
- `Commands/Admision/RegistrarAltaPacienteCommandHandler.cs` (líneas 28, 77)
- `Commands/Admision/TrasladarPacienteCommandHandler.cs` (línea 27)
- `Commands/Admision/TrasladarPacienteCirugiaCommand.cs` (línea 62)
- `Commands/Admision/CloseAccountCommandHandler.cs` (líneas 56, 156)
- `Commands/Admision/AbrirCuentaClinicaCommandHandler.cs` (línea 48)
- `Commands/Admision/CrearOrdenCirugiaCommand.cs` (línea 90)
- `Queries/Admision/GetPatientHistoryQuery.cs` (línea 37)
- `Queries/Admision/GetOpenAccountQueryHandler.cs` (línea 29)
- `Queries/Admision/GetCamasMonitoreoQuery.cs` (línea 46)
- `Queries/Admision/GetCuentasAdministrativasQuery.cs` (líneas 60, 93)
- `Queries/Admision/GetDailyBilledPatientsQuery.cs` (línea 48)
- `Queries/Admision/ExportGenericListQuery.cs` (líneas 184, 199)
- `Queries/Admision/GetExpedienteFacturacionQuery.cs` (línea 136)
- `Queries/Admin/GetDoctorHonorariumSummaryQuery.cs` (líneas 46, 73)
- `Queries/Admision/GetBusinessInsightsQuery.cs` (líneas 64, 103, 126, 157, 342)
- `Infrastructure/Persistence/Repositories/BillingRepository.cs` (línea 27)
- `Infrastructure/Persistence/Contexts/SatHospitalarioDbContext.cs` (líneas 144, 164, 176)

**Tipo de cambio:** Migrar de `Estado` (string) a `EstadoId` (int FK) + `EstadoNav`.

## 3. Impacto en Frontend

- Verificar si la API expone `estado` (string) en los DTOs de cuentas. Migrar a `estadoId`.

## 4. Análisis paso a paso

- [ ] 1. Identificar todos los usos de `CuentaServicios.Estado`.
- [ ] 2. Verificar que `EstadoId` y `EstadoNav` existen en la entidad.
- [ ] 3. Migrar comparaciones de string a comparaciones de `EstadoId`.
- [ ] 4. Actualizar DTOs y frontend.
- [ ] 5. Compilar backend y frontend.
- [ ] 6. Crear migración para eliminar la columna `Estado`.

## 5. Verificación

- `dotnet build` sin CS0618 para `CuentaServicios.Estado`.
- El flujo de cuentas (abrir, cerrar, trasladar, alta) funciona correctamente.

## 6. Riesgos

- Es la propiedad con MÁS usos del sistema. Requiere migración cuidadosa y pruebas exhaustivas.
- Los valores de `Estado` (string) deben mapearse a los `EstadoId` correctos.