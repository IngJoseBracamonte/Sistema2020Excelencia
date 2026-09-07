# Análisis: `OrdenCompraInventario.SaldoPendienteUSD`

## 1. Identificación

| Campo | Valor |
|-------|-------|
| **Entidad** | `OrdenCompraInventario` |
| **Propiedad obsoleta** | `SaldoPendienteUSD` |
| **Reemplazo** | `MontoTotalUSD - TotalAbonadoUSD` |
| **Mensaje de obsolescencia** | "Calcular como MontoTotalUSD - TotalAbonadoUSD. Columna legacy pendiente de DROP." |
| **Archivo de definición** | `src/SistemaSatHospitalario.Core.Domain/Entities/Admision/OrdenCompraInventario.cs` |

## 2. Impacto en Backend

**Usos detectados en el build (CS0618):**
- `Queries/Inventario/GetOrdenesCompraQuery.cs` (línea 79)
- `Infrastructure/Persistence/Contexts/SatHospitalarioDbContext.cs` (línea 1584)

**Tipo de cambio:** Calcular el saldo pendiente derivado de `MontoTotalUSD - TotalAbonadoUSD`.

## 3. Impacto en Frontend

- Verificar si la API expone `saldoPendienteUSD`. Debe seguir exponiéndose pero calculado.

## 4. Análisis paso a paso

- [ ] 1. Identificar todos los usos de `OrdenCompraInventario.SaldoPendienteUSD`.
- [ ] 2. Reemplazar por `MontoTotalUSD - TotalAbonadoUSD` (calculado).
- [ ] 3. Mantener el campo en el DTO de respuesta (calculado).
- [ ] 4. Compilar backend y frontend.
- [ ] 5. Crear migración para eliminar la columna `SaldoPendienteUSD`.

## 5. Verificación

- `dotnet build` sin CS0618 para `OrdenCompraInventario.SaldoPendienteUSD`.
- El saldo pendiente de las órdenes de compra es correcto.

## 6. Riesgos

- Depende de que `MontoTotalUSD` y `TotalAbonadoUSD` estén correctamente calculados.