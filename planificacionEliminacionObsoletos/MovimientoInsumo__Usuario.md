# Análisis: `MovimientoInsumo.Usuario`

## 1. Identificación

| Campo | Valor |
|-------|-------|
| **Entidad** | `MovimientoInsumo` |
| **Propiedad obsoleta** | `Usuario` |
| **Reemplazo** | `UsuarioIdentityId` |
| **Mensaje de obsolescencia** | "Usar UsuarioIdentityId. Columna legacy pendiente de DROP." |
| **Archivo de definición** | `src/SistemaSatHospitalario.Core.Domain/Entities/Admision/MovimientoInsumo.cs` |

## 2. Impacto en Backend

**Usos detectados en el build (CS0618):**
- `Queries/Admision/GetKardexQueryHandler.cs` (línea 135)
- `Queries/Admision/GetHistorialMovimientosQuery.cs` (líneas 74, 92)
- `Infrastructure/Persistence/Contexts/SatHospitalarioDbContext.cs` (línea 1093)

**Tipo de cambio:** Migrar de `Usuario` (string legacy) a `UsuarioIdentityId` (Guid).

## 3. Impacto en Frontend

- Verificar si la API expone `usuario` en los DTOs de movimientos. Migrar a `usuarioIdentityId`.

## 4. Análisis paso a paso

- [ ] 1. Identificar todos los usos de `MovimientoInsumo.Usuario`.
- [ ] 2. Verificar que `UsuarioIdentityId` existe en la entidad.
- [ ] 3. Migrar asignaciones/lecturas de `Usuario` → `UsuarioIdentityId`.
- [ ] 4. Actualizar DTOs y frontend.
- [ ] 5. Compilar backend y frontend.
- [ ] 6. Crear migración para eliminar la columna `Usuario`.

## 5. Verificación

- `dotnet build` sin CS0618 para `MovimientoInsumo.Usuario`.
- El kardex y el historial de movimientos muestran el usuario correcto.

## 6. Riesgos

- Requiere backfill de `UsuarioIdentityId` para movimientos legacy.