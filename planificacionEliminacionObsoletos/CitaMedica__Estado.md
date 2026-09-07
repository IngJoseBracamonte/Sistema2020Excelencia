# Análisis: `CitaMedica.Estado`

## 1. Identificación

| Campo | Valor |
|-------|-------|
| **Entidad** | `CitaMedica` |
| **Propiedad obsoleta** | `Estado` |
| **Reemplazo** | `EstadoId / EstadoNav` |
| **Mensaje de obsolescencia** | "Usar EstadoId / EstadoNav. Columna legacy pendiente de DROP." |
| **Archivo de definición** | `src/SistemaSatHospitalario.Core.Domain/Entities/Admision/CitaMedica.cs` |

## 2. Impacto en Backend

**Usos detectados en el build (CS0618):**
- `Commands/ReservarTurnoTemporalCommand.cs` (línea 54)
- `Commands/BloquearHorarioCommand.cs` (línea 40)
- `Commands/AgendarTurnoCommandHandler.cs` (línea 65)
- `Queries/Admision/GetTechnicalValidationListQuery.cs` (línea 77)
- `Queries/Admision/GetActiveAppointmentsQuery.cs` (línea 70)
- `Queries/Admision/GetControlCitasQuery.cs` (línea 110)
- `Queries/Admision/GetDoctorScheduleQueryHandler.cs` (línea 50)
- `Queries/Admision/ExportGenericListQuery.cs` (líneas 105, 199)
- `Queries/Admision/GetExpedienteFacturacionQuery.cs` (línea 51)
- `Queries/Admin/GetDoctorHonorariumSummaryQuery.cs` (línea 45)
- `Queries/Admision/GetBusinessInsightsQuery.cs` (línea 72)
- `Infrastructure/Persistence/Repositories/BillingRepository.cs` (línea 78)

**Tipo de cambio:** Migrar de `Estado` (string) a `EstadoId` (int FK) + `EstadoNav`.

## 3. Impacto en Frontend

- Verificar si la API expone `estado` (string) en los DTOs de citas. Migrar a `estadoId`.

## 4. Análisis paso a paso

- [ ] 1. Identificar todos los usos de `CitaMedica.Estado`.
- [ ] 2. Verificar que `EstadoId` y `EstadoNav` existen en la entidad.
- [ ] 3. Migrar comparaciones de string a comparaciones de `EstadoId`.
- [ ] 4. Actualizar DTOs y frontend.
- [ ] 5. Compilar backend y frontend.
- [ ] 6. Crear migración para eliminar la columna `Estado`.

## 5. Verificación

- `dotnet build` sin CS0618 para `CitaMedica.Estado`.
- El flujo de citas (agendar, bloquear, reservar) funciona correctamente.

## 6. Riesgos

- Los valores de `Estado` (string) deben mapearse a los `EstadoId` correctos.
- Es una de las propiedades con más usos; requiere cuidado en la migración.