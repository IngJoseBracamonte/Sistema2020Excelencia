# Análisis: `OrdenDeServicio.TotalCobrado`

## 1. Identificación

| Campo | Valor |
|-------|-------|
| **Entidad** | `OrdenDeServicio` |
| **Propiedad obsoleta** | `TotalCobrado` |
| **Reemplazo** | Calcular desde los detalles de la orden |
| **Mensaje de obsolescencia** | "Calcular desde los detalles de la orden. Columna legacy pendiente de DROP." |
| **Archivo de definición** | `src/SistemaSatHospitalario.Core.Domain/Entities/OrdenDeServicio.cs` |

## 2. Impacto en Backend

**Usos detectados en el build (CS0618):**
- `Queries/Admision/GetBusinessInsightsQuery.cs` (línea 88)
- `Infrastructure/Persistence/Contexts/SatHospitalarioDbContext.cs` (línea 362)

**Tipo de cambio:** Calcular el total cobrado derivado de los detalles de la orden en lugar de almacenarlo.

## 3. Impacto en Frontend

- Verificar si la API expone `totalCobrado`. Debe seguir exponiéndose pero calculado.

## 4. Análisis paso a paso

- [ ] 1. Identificar todos los usos de `OrdenDeServicio.TotalCobrado`.
- [ ] 2. Definir la lógica de cálculo desde los detalles de la orden.
- [ ] 3. Reemplazar las lecturas por la lógica de cálculo.
- [ ] 4. Mantener el campo en el DTO de respuesta (calculado).
- [ ] 5. Compilar backend y frontend.
- [ ] 6. Crear migración para eliminar la columna `TotalCobrado`.

## 5. Verificación

- `dotnet build` sin CS0618 para `OrdenDeServicio.TotalCobrado`.
- El total cobrado en los insights de negocio es correcto.

## 6. Riesgos

- La lógica de cálculo debe replicar exactamente el total almacenado.
- Requiere validar con datos reales que el cálculo coincide.