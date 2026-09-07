# Análisis: `ErrorTicket.UsuarioAsociado`

## 1. Identificación

| Campo | Valor |
|-------|-------|
| **Entidad** | `ErrorTicket` |
| **Propiedad obsoleta** | `UsuarioAsociado` |
| **Reemplazo** | `UsuarioAsociadoId` |
| **Mensaje de obsolescencia** | "Usar UsuarioAsociadoId. Columna legacy pendiente de DROP." |
| **Archivo de definición** | `src/SistemaSatHospitalario.Core.Domain/Entities/ErrorTicket.cs` |

## 2. Impacto en Backend

**Usos detectados en el build (CS0618):**
- `Commands/System/CreateErrorTicketCommandHandler.cs` (línea 30)

**Tipo de cambio:** Migrar de `UsuarioAsociado` (string) a `UsuarioAsociadoId` (Guid).

## 3. Impacto en Frontend

- Verificar si la API expone `usuarioAsociado`. Migrar a `usuarioAsociadoId`.

## 4. Análisis paso a paso

- [ ] 1. Identificar todos los usos de `ErrorTicket.UsuarioAsociado`.
- [ ] 2. Verificar que `UsuarioAsociadoId` existe en la entidad.
- [ ] 3. Migrar asignaciones/lecturas de `UsuarioAsociado` → `UsuarioAsociadoId`.
- [ ] 4. Actualizar DTOs y frontend.
- [ ] 5. Compilar backend y frontend.
- [ ] 6. Crear migración para eliminar la columna `UsuarioAsociado`.

## 5. Verificación

- `dotnet build` sin CS0618 para `ErrorTicket.UsuarioAsociado`.
- Los tickets de error muestran el usuario asociado correcto.

## 6. Riesgos

- Requiere backfill de `UsuarioAsociadoId` para tickets legacy.