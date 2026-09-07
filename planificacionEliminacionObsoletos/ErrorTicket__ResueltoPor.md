# Análisis: `ErrorTicket.ResueltoPor`

## 1. Identificación

| Campo | Valor |
|-------|-------|
| **Entidad** | `ErrorTicket` |
| **Propiedad obsoleta** | `ResueltoPor` |
| **Reemplazo** | `ResueltoPorId` |
| **Mensaje de obsolescencia** | "Usar ResueltoPorId. Columna legacy pendiente de DROP." |
| **Archivo de definición** | `src/SistemaSatHospitalario.Core.Domain/Entities/ErrorTicket.cs` |

## 2. Impacto en Backend

**Usos detectados en el build (CS0618):**
- `Commands/System/ResolveTicketCommandHandler.cs` (línea 27)

**Tipo de cambio:** Migrar de `ResueltoPor` (string) a `ResueltoPorId` (Guid).

## 3. Impacto en Frontend

- Verificar si la API expone `resueltoPor`. Migrar a `resueltoPorId`.

## 4. Análisis paso a paso

- [ ] 1. Identificar todos los usos de `ErrorTicket.ResueltoPor`.
- [ ] 2. Verificar que `ResueltoPorId` existe en la entidad.
- [ ] 3. Migrar asignaciones/lecturas de `ResueltoPor` → `ResueltoPorId`.
- [ ] 4. Actualizar DTOs y frontend.
- [ ] 5. Compilar backend y frontend.
- [ ] 6. Crear migración para eliminar la columna `ResueltoPor`.

## 5. Verificación

- `dotnet build` sin CS0618 para `ErrorTicket.ResueltoPor`.
- Los tickets de error muestran el usuario que resolvió correcto.

## 6. Riesgos

- Requiere backfill de `ResueltoPorId` para tickets legacy.