# Análisis: `OrdenCompraInventario.MontoTotalBs`

## 1. Identificación

| Campo | Valor |
|-------|-------|
| **Entidad** | `OrdenCompraInventario` |
| **Propiedad obsoleta** | `MontoTotalBs` |
| **Reemplazo** | `MontoTotalUSD * tasaCambio` |
| **Mensaje de obsolescencia** | "Calcular como MontoTotalUSD * tasaCambio. Columna legacy pendiente de DROP." |
| **Archivo de definición** | `src/SistemaSatHospitalario.Core.Domain/Entities/Admision/OrdenCompraInventario.cs` |

## 2. Impacto en Backend

**Usos detectados en el build (CS0618):**
- `Queries/Inventario/GetOrdenesCompraQuery.cs` (línea 77)
- `Infrastructure/Persistence/Contexts/SatHospitalarioDbContext.cs` (línea 1582)

**Tipo de cambio:** Calcular el monto en Bs derivado de `MontoTotalUSD * tasaCambio`.

## 3. Impacto en Frontend

- Verificar si la API expone `montoTotalBs`. Debe seguir exponiéndose pero calculado.

## 4. Análisis paso a paso

- [ ] 1. Identificar todos los usos de `OrdenCompraInventario.MontoTotalBs`.
- [ ] 2. Reemplazar por `MontoTotalUSD * tasaCambio` (calculado).
- [ ] 3. Mantener el campo en el DTO de respuesta (calculado).
- [ ] 4. Compilar backend y frontend.
- [ ] 5. Crear migración para eliminar la columna `MontoTotalBs`.

## 5. Verificación

- `dotnet build` sin CS0618 para `OrdenCompraInventario.MontoTotalBs`.
- El monto en Bs de las órdenes de compra es correcto.

## 6. Riesgos

- Depende de la tasa de cambio vigente al momento del cálculo.