# Análisis: `CierreInventario.Usuario`

## 1. Identificación

| Campo | Valor |
|-------|-------|
| **Entidad** | `CierreInventario` |
| **Propiedad obsoleta** | `Usuario` |
| **Reemplazo** | `UsuarioId` |
| **Mensaje de obsolescencia** | "Usar UsuarioId. Columna legacy pendiente de DROP." |
| **Archivo de definición** | `src/SistemaSatHospitalario.Core.Domain/Entities/Admision/CierreInventario.cs` |

## 2. Impacto en Backend

**Usos detectados en el build (CS0618):**
- `Infrastructure/Persistence/Contexts/SatHospitalarioDbContext.cs` (línea 1111)

**Tipo de cambio:** Migrar de `Usuario` (string) a `UsuarioId` (Guid).

## 3. Impacto en Frontend

- Verificar si la API expone `usuario` en los DTOs de cierre de inventario. Migrar a `usuarioId`.

## 4. Análisis paso a paso

- [ ] 1. Identificar todos los usos de `CierreInventario.Usuario`.
- [ ] 2. Verificar que `UsuarioId` existe en la entidad.
- [ ] 3. Migrar asignaciones/lecturas de `Usuario` → `UsuarioId`.
- [ ] 4. Actualizar DTOs y frontend.
- [ ] 5. Compilar backend y frontend.
- [ ] 6. Crear migración para eliminar la columna `Usuario`.

## 5. Verificación

- `dotnet build` sin CS0618 para `CierreInventario.Usuario`.
- El cierre de inventario registra el usuario correcto.

## 6. Riesgos

- Requiere backfill de `UsuarioId` para cierres legacy.