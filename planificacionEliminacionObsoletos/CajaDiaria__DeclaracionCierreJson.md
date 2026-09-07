# Análisis: `CajaDiaria.DeclaracionCierreJson`

## 1. Identificación

| Campo | Valor |
|-------|-------|
| **Entidad** | `CajaDiaria` |
| **Propiedad obsoleta** | `DeclaracionCierreJson` |
| **Reemplazo** | `DeclaracionesPorMetodo` |
| **Mensaje de obsolescencia** | "Usar DeclaracionesPorMetodo. Columna legacy pendiente de DROP." |
| **Archivo de definición** | `src/SistemaSatHospitalario.Core.Domain/Entities/Admision/CajaDiaria.cs` |

## 2. Impacto en Backend

**Usos detectados en el build (CS0618):**
- `Infrastructure/Persistence/Contexts/SatHospitalarioDbContext.cs` (línea 172)

**Tipo de cambio:** Migrar de un JSON serializado a la colección tipada `DeclaracionesPorMetodo`.

## 3. Impacto en Frontend

- Verificar si la API expone `declaracionCierreJson`. Debe migrarse a la estructura `declaracionesPorMetodo`.

## 4. Análisis paso a paso

- [ ] 1. Identificar todos los usos de `CajaDiaria.DeclaracionCierreJson`.
- [ ] 2. Verificar que `DeclaracionesPorMetodo` existe como colección en la entidad.
- [ ] 3. Migrar la serialización/deserialización del JSON a la colección tipada.
- [ ] 4. Actualizar DTOs y frontend.
- [ ] 5. Compilar backend y frontend.
- [ ] 6. Crear migración para eliminar la columna `DeclaracionCierreJson`.

## 5. Verificación

- `dotnet build` sin CS0618 para `CajaDiaria.DeclaracionCierreJson`.
- El cierre de caja muestra las declaraciones por método correctamente.

## 6. Riesgos

- Los datos legacy en JSON deben migrarse a la nueva estructura.
- Verificar que `DeclaracionesPorMetodo` esté correctamente mapeada en EF.