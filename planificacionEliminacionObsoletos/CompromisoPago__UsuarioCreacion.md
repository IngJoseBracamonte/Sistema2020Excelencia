# Análisis: `CompromisoPago.UsuarioCreacion`

## 1. Identificación

| Campo | Valor |
|-------|-------|
| **Entidad** | `CompromisoPago` |
| **Propiedad obsoleta** | `UsuarioCreacion` |
| **Reemplazo** | `UsuarioCreacionId` |
| **Mensaje de obsolescencia** | "Usar UsuarioCreacionId. Columna legacy pendiente de DROP." |
| **Archivo de definición** | `src/SistemaSatHospitalario.Core.Domain/Entities/Admision/CompromisoPago.cs` |

## 2. Impacto en Backend

**Usos detectados en el build (CS0618):**
- No aparece en las advertencias del build actual (posiblemente sin usos activos o usos no detectados).

**Tipo de cambio:** Migrar de `UsuarioCreacion` (string) a `UsuarioCreacionId` (Guid).

## 3. Impacto en Frontend

- Verificar si la API expone `usuarioCreacion`. Migrar a `usuarioCreacionId`.

## 4. Análisis paso a paso

- [ ] 1. Identificar todos los usos de `CompromisoPago.UsuarioCreacion` (grep).
- [ ] 2. Verificar que `UsuarioCreacionId` existe en la entidad.
- [ ] 3. Migrar asignaciones/lecturas de `UsuarioCreacion` → `UsuarioCreacionId`.
- [ ] 4. Actualizar DTOs y frontend.
- [ ] 5. Compilar backend y frontend.
- [ ] 6. Crear migración para eliminar la columna `UsuarioCreacion`.

## 5. Verificación

- `dotnet build` sin CS0618 para `CompromisoPago.UsuarioCreacion`.
- El compromiso de pago registra el usuario correcto.

## 6. Riesgos

- Requiere backfill de `UsuarioCreacionId` para compromisos legacy.