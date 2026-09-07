# Análisis: `CirugiaObservacionHistorial.UsuarioRegistro`

## 1. Identificación

| Campo | Valor |
|-------|-------|
| **Entidad** | `CirugiaObservacionHistorial` |
| **Propiedad obsoleta** | `UsuarioRegistro` |
| **Reemplazo** | `UsuarioRegistroId` |
| **Mensaje de obsolescencia** | "Usar UsuarioRegistroId. Columna legacy pendiente de DROP." |
| **Archivo de definición** | `src/SistemaSatHospitalario.Core.Domain/Entities/Admision/CirugiaObservacionHistorial.cs` |

## 2. Impacto en Backend

**Usos detectados en el build (CS0618):**
- `Infrastructure/Persistence/Contexts/SatHospitalarioDbContext.cs` (línea 1561)

**Tipo de cambio:** Migrar de `UsuarioRegistro` (string) a `UsuarioRegistroId` (Guid).

## 3. Impacto en Frontend

- Verificar si la API expone `usuarioRegistro`. Migrar a `usuarioRegistroId`.

## 4. Análisis paso a paso

- [ ] 1. Identificar todos los usos de `CirugiaObservacionHistorial.UsuarioRegistro`.
- [ ] 2. Verificar que `UsuarioRegistroId` existe en la entidad.
- [ ] 3. Migrar asignaciones/lecturas de `UsuarioRegistro` → `UsuarioRegistroId`.
- [ ] 4. Actualizar DTOs y frontend.
- [ ] 5. Compilar backend y frontend.
- [ ] 6. Crear migración para eliminar la columna `UsuarioRegistro`.

## 5. Verificación

- `dotnet build` sin CS0618 para `CirugiaObservacionHistorial.UsuarioRegistro`.
- El historial de observaciones muestra el usuario correcto.

## 6. Riesgos

- Requiere backfill de `UsuarioRegistroId` para registros legacy.