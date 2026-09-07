# Análisis: `CajaDiaria.TotalCobrado`

## 1. Identificación

| Campo | Valor |
|-------|-------|
| **Entidad** | `CajaDiaria` |
| **Propiedad obsoleta** | `TotalCobrado` |
| **Reemplazo** | `DeclaracionesPorMetodo.Sum(d => d.MontoEsperadoIngreso)` |
| **Mensaje de obsolescencia** | "Calcular como DeclaracionesPorMetodo.Sum(d => d.MontoEsperadoIngreso). Columna legacy pendiente de DROP." |
| **Archivo de definición** | `src/SistemaSatHospitalario.Core.Domain/Entities/Admision/CajaDiaria.cs` |

## 2. Impacto en Backend

**Usos detectados en el build (CS0618):**
- `Queries/Admision/GetCajaSummariesQuery.cs` (línea 108)
- `Commands/Admision/ConsolidarCajasCommand.cs` (línea 62)
- `Infrastructure/Persistence/Contexts/SatHospitalarioDbContext.cs` (línea 170)

**Tipo de cambio:** Calcular el total derivado de `DeclaracionesPorMetodo`.

## 3. Impacto en Frontend

- Verificar si la API expone `totalCobrado`. Debe seguir exponiéndose pero calculado.

## 4. Análisis paso a paso

- [ ] 1. Identificar todos los usos de `CajaDiaria.TotalCobrado`.
- [ ] 2. Reemplazar por `DeclaracionesPorMetodo.Sum(d => d.MontoEsperadoIngreso)`.
- [ ] 3. Mantener el campo en el DTO de respuesta (calculado).
- [ ] 4. Compilar backend y frontend.
- [ ] 5. Crear migración para eliminar la columna `TotalCobrado`.

## 5. Verificación

- `dotnet build` sin CS0618 para `CajaDiaria.TotalCobrado`.
- Los totales de caja coinciden con la suma de declaraciones.

## 6. Riesgos

- Verificar que `DeclaracionesPorMetodo` esté siempre cargada antes de calcular.