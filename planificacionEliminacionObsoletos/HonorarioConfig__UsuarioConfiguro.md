# Análisis: `HonorarioConfig.UsuarioConfiguro`

## 1. Identificación

| Campo | Valor |
|-------|-------|
| **Entidad** | `HonorarioConfig` |
| **Propiedad obsoleta** | `UsuarioConfiguro` |
| **Reemplazo** | `UsuarioConfiguroId` |
| **Mensaje de obsolescencia** | "Usar UsuarioConfiguroId. Columna legacy pendiente de DROP." |
| **Archivo de definición** | `src/SistemaSatHospitalario.Core.Domain/Entities/Admision/HonorarioConfig.cs` |

## 2. Impacto en Backend

**Usos detectados en el build (CS0618):**
- `WebAPI/Controllers/Admin/HonorarioConfigController.cs` (línea 34)

**Tipo de cambio:** Migrar de `UsuarioConfiguro` (string) a `UsuarioConfiguroId` (Guid).

## 3. Impacto en Frontend

- Verificar si la API expone `usuarioConfiguro`. Migrar a `usuarioConfiguroId`.

## 4. Análisis paso a paso

- [ ] 1. Identificar todos los usos de `HonorarioConfig.UsuarioConfiguro`.
- [ ] 2. Verificar que `UsuarioConfiguroId` existe en la entidad.
- [ ] 3. Migrar asignaciones/lecturas de `UsuarioConfiguro` → `UsuarioConfiguroId`.
- [ ] 4. Actualizar DTOs y frontend.
- [ ] 5. Compilar backend y frontend.
- [ ] 6. Crear migración para eliminar la columna `UsuarioConfiguro`.

## 5. Verificación

- `dotnet build` sin CS0618 para `HonorarioConfig.UsuarioConfiguro`.
- La configuración de honorarios registra el usuario correcto.

## 6. Riesgos

- Requiere backfill de `UsuarioConfiguroId` para configuraciones legacy.