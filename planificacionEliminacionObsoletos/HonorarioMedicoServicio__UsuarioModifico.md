# Análisis: `HonorarioMedicoServicio.UsuarioModifico`

## 1. Identificación

| Campo | Valor |
|-------|-------|
| **Entidad** | `HonorarioMedicoServicio` |
| **Propiedad obsoleta** | `UsuarioModifico` |
| **Reemplazo** | `UsuarioModificoId` |
| **Mensaje de obsolescencia** | "Usar UsuarioModificoId. Columna legacy pendiente de DROP." |
| **Archivo de definición** | `src/SistemaSatHospitalario.Core.Domain/Entities/Admision/HonorarioMedicoServicio.cs` |

## 2. Impacto en Backend

**Usos detectados en el build (CS0618):**
- No aparece en las advertencias del build actual (posiblemente sin usos activos).

**Tipo de cambio:** Migrar de `UsuarioModifico` (string) a `UsuarioModificoId` (Guid).

## 3. Impacto en Frontend

- Verificar si la API expone `usuarioModifico`. Migrar a `usuarioModificoId`.

## 4. Análisis paso a paso

- [ ] 1. Identificar todos los usos de `HonorarioMedicoServicio.UsuarioModifico` (grep).
- [ ] 2. Verificar que `UsuarioModificoId` existe en la entidad.
- [ ] 3. Migrar asignaciones/lecturas de `UsuarioModifico` → `UsuarioModificoId`.
- [ ] 4. Actualizar DTOs y frontend.
- [ ] 5. Compilar backend y frontend.
- [ ] 6. Crear migración para eliminar la columna `UsuarioModifico`.

## 5. Verificación

- `dotnet build` sin CS0618 para `HonorarioMedicoServicio.UsuarioModifico`.
- La modificación de honorarios registra el usuario correcto.

## 6. Riesgos

- Requiere backfill de `UsuarioModificoId` para registros legacy.