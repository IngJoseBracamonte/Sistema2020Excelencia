# Análisis: `HonorariumMappingRule.UsuarioCreo`

## 1. Identificación

| Campo | Valor |
|-------|-------|
| **Entidad** | `HonorariumMappingRule` |
| **Propiedad obsoleta** | `UsuarioCreo` |
| **Reemplazo** | `UsuarioCreoId` |
| **Mensaje de obsolescencia** | "Usar UsuarioCreoId. Columna legacy pendiente de DROP." |
| **Archivo de definición** | `src/SistemaSatHospitalario.Core.Domain/Entities/Admision/HonorariumMappingRule.cs` |

## 2. Impacto en Backend

**Usos detectados en el build (CS0618):**
- No aparece en las advertencias del build actual (posiblemente sin usos activos).

**Tipo de cambio:** Migrar de `UsuarioCreo` (string) a `UsuarioCreoId` (Guid).

## 3. Impacto en Frontend

- Verificar si la API expone `usuarioCreo`. Migrar a `usuarioCreoId`.

## 4. Análisis paso a paso

- [ ] 1. Identificar todos los usos de `HonorariumMappingRule.UsuarioCreo` (grep).
- [ ] 2. Verificar que `UsuarioCreoId` existe en la entidad.
- [ ] 3. Migrar asignaciones/lecturas de `UsuarioCreo` → `UsuarioCreoId`.
- [ ] 4. Actualizar DTOs y frontend.
- [ ] 5. Compilar backend y frontend.
- [ ] 6. Crear migración para eliminar la columna `UsuarioCreo`.

## 5. Verificación

- `dotnet build` sin CS0618 para `HonorariumMappingRule.UsuarioCreo`.
- La creación de reglas de mapeo registra el usuario correcto.

## 6. Riesgos

- Requiere backfill de `UsuarioCreoId` para reglas legacy.