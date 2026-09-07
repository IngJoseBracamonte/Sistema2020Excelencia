# Análisis: `HistorialModificacionCuenta.DetalleServiciosCambiosJson`

## 1. Identificación

| Campo | Valor |
|-------|-------|
| **Entidad** | `HistorialModificacionCuenta` |
| **Propiedad obsoleta** | `DetalleServiciosCambiosJson` |
| **Reemplazo** | `DetallesModificados` |
| **Mensaje de obsolescencia** | "Usar DetallesModificados. Columna legacy pendiente de DROP." |
| **Archivo de definición** | `src/SistemaSatHospitalario.Core.Domain/Entities/Admision/HistorialModificacionCuenta.cs` |

## 2. Impacto en Backend

**Usos detectados en el build (CS0618):**
- `Queries/Admision/GetHistorialModificacionesQuery.cs` (línea 91)
- `Infrastructure/Persistence/Contexts/SatHospitalarioDbContext.cs` (línea 914)

**Tipo de cambio:** Migrar de un JSON serializado a la colección tipada `DetallesModificados`.

## 3. Impacto en Frontend

- Verificar si la API expone `detalleServiciosCambiosJson`. Debe migrarse a la estructura `detallesModificados`.

## 4. Análisis paso a paso

- [ ] 1. Identificar todos los usos de `HistorialModificacionCuenta.DetalleServiciosCambiosJson`.
- [ ] 2. Verificar que `DetallesModificados` existe como colección en la entidad.
- [ ] 3. Migrar la serialización/deserialización del JSON a la colección tipada.
- [ ] 4. Actualizar DTOs y frontend.
- [ ] 5. Compilar backend y frontend.
- [ ] 6. Crear migración para eliminar la columna `DetalleServiciosCambiosJson`.

## 5. Verificación

- `dotnet build` sin CS0618 para `HistorialModificacionCuenta.DetalleServiciosCambiosJson`.
- El historial de modificaciones muestra los detalles de servicios correctamente.

## 6. Riesgos

- Los datos legacy en JSON deben migrarse a la nueva estructura.
- Verificar que `DetallesModificados` esté correctamente mapeada en EF.