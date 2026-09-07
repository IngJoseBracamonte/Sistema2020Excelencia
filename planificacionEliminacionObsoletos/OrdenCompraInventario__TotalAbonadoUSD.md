# Análisis: `OrdenCompraInventario.TotalAbonadoUSD`

## 1. Identificación

| Campo | Valor |
|-------|-------|
| **Entidad** | `OrdenCompraInventario` |
| **Propiedad obsoleta** | `TotalAbonadoUSD` |
| **Reemplazo** | `Pagos.Sum(p => p.MontoAbonadoUSD)` |
| **Mensaje de obsolescencia** | "Calcular como Pagos.Sum(p => p.MontoAbonadoUSD). Columna legacy pendiente de DROP." |
| **Archivo de definición** | `src/SistemaSatHospitalario.Core.Domain/Entities/Admision/OrdenCompraInventario.cs` |

## 2. Impacto en Backend

**Usos detectados en el build (CS0618):**
- `Queries/Inventario/GetOrdenesCompraQuery.cs` (línea 78)
- `Infrastructure/Persistence/Contexts/SatHospitalarioDbContext.cs` (línea 1583)

**Tipo de cambio:** Calcular el total abonado derivado de `Pagos.Sum(p => p.MontoAbonadoUSD)`.

## 3. Impacto en Frontend

- Verificar si la API expone `totalAbonadoUSD`. Debe seguir exponiéndose pero calculado.

## 4. Análisis paso a paso

- [ ] 1. Identificar todos los usos de `OrdenCompraInventario.TotalAbonadoUSD`.
- [ ] 2. Reemplazar por `Pagos.Sum(p => p.MontoAbonadoUSD)` (calculado).
- [ ] 3. Mantener el campo en el DTO de respuesta (calculado).
- [ ] 4. Compilar backend y frontend.
- [ ] 5. Crear migración para eliminar la columna `TotalAbonadoUSD`.

## 5. Verificación

- `dotnet build` sin CS0618 para `OrdenCompraInventario.TotalAbonadoUSD`.
- El total abonado de las órdenes de compra es correcto.

## 6. Riesgos

- Verificar que `Pagos` esté siempre cargada (Include) antes de calcular.