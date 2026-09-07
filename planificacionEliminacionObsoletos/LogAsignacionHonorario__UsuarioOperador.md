# Análisis: `LogAsignacionHonorario.UsuarioOperador`

## 1. Identificación

| Campo | Valor |
|-------|-------|
| **Entidad** | `LogAsignacionHonorario` |
| **Propiedad obsoleta** | `UsuarioOperador` |
| **Reemplazo** | `UsuarioOperadorId` |
| **Mensaje de obsolescencia** | "Usar UsuarioOperadorId. Columna legacy pendiente de DROP." |
| **Archivo de definición** | `src/SistemaSatHospitalario.Core.Domain/Entities/Admision/LogAsignacionHonorario.cs` |

## 2. Impacto en Backend

**Usos detectados en el build (CS0618):**
- No aparece en las advertencias del build actual (posiblemente sin usos activos).

**Tipo de cambio:** Migrar de `UsuarioOperador` (string) a `UsuarioOperadorId` (Guid).

## 3. Impacto en Frontend

- Verificar si la API expone `usuarioOperador`. Migrar a `usuarioOperadorId`.

## 4. Análisis paso a paso

- [ ] 1. Identificar todos los usos de `LogAsignacionHonorario.UsuarioOperador` (grep).
- [ ] 2. Verificar que `UsuarioOperadorId` existe en la entidad.
- [ ] 3. Migrar asignaciones/lecturas de `UsuarioOperador` → `UsuarioOperadorId`.
- [ ] 4. Actualizar DTOs y frontend.
- [ ] 5. Compilar backend y frontend.
- [ ] 6. Crear migración para eliminar la columna `UsuarioOperador`.

## 5. Verificación

- `dotnet build` sin CS0618 para `LogAsignacionHonorario.UsuarioOperador`.
- El log de asignación de honorarios muestra el operador correcto.

## 6. Riesgos

- Requiere backfill de `UsuarioOperadorId` para logs legacy.