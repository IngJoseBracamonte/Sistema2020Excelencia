# Análisis: `CuentaServicios.UsuarioValidacion`

## 1. Identificación

| Campo | Valor |
|-------|-------|
| **Entidad** | `CuentaServicios` |
| **Propiedad obsoleta** | `UsuarioValidacion` |
| **Reemplazo** | `UsuarioValidacionId` |
| **Mensaje de obsolescencia** | "Usar UsuarioValidacionId. Columna legacy pendiente de DROP." |
| **Archivo de definición** | `src/SistemaSatHospitalario.Core.Domain/Entities/Admision/CuentaServicios.cs` |

## 2. Impacto en Backend

**Usos detectados en el build (CS0618):**
- No aparece en las advertencias del build actual (posiblemente sin usos activos).

**Tipo de cambio:** Migrar de `UsuarioValidacion` (string) a `UsuarioValidacionId` (Guid).

## 3. Impacto en Frontend

- Verificar si la API expone `usuarioValidacion`. Migrar a `usuarioValidacionId`.

## 4. Análisis paso a paso

- [ ] 1. Identificar todos los usos de `CuentaServicios.UsuarioValidacion` (grep).
- [ ] 2. Verificar que `UsuarioValidacionId` existe en la entidad.
- [ ] 3. Migrar asignaciones/lecturas de `UsuarioValidacion` → `UsuarioValidacionId`.
- [ ] 4. Actualizar DTOs y frontend.
- [ ] 5. Compilar backend y frontend.
- [ ] 6. Crear migración para eliminar la columna `UsuarioValidacion`.

## 5. Verificación

- `dotnet build` sin CS0618 para `CuentaServicios.UsuarioValidacion`.
- La validación de cuentas registra el usuario correcto.

## 6. Riesgos

- Requiere backfill de `UsuarioValidacionId` para cuentas legacy.