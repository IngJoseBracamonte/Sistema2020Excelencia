# Análisis: `MovimientoInsumo.UnidadMedidaOriginal`

## 1. Identificación

| Campo | Valor |
|-------|-------|
| **Entidad** | `MovimientoInsumo` |
| **Propiedad obsoleta** | `UnidadMedidaOriginal` |
| **Reemplazo** | `UnidadMedidaOriginalId / UnidadMedidaNav` |
| **Mensaje de obsolescencia** | "Usar UnidadMedidaOriginalId / UnidadMedidaNav. Columna legacy pendiente de DROP." |
| **Archivo de definición** | `src/SistemaSatHospitalario.Core.Domain/Entities/Admision/MovimientoInsumo.cs` |

## 2. Impacto en Backend

**Usos detectados en el build (CS0618):**
- `Queries/Admision/GetKardexQueryHandler.cs` (línea 120)
- `Queries/Admision/GetHistorialMovimientosQuery.cs` (línea 91)
- `Infrastructure/Persistence/Contexts/SatHospitalarioDbContext.cs` (línea 1085)

**Tipo de cambio:** Migrar de `UnidadMedidaOriginal` (string) a `UnidadMedidaOriginalId` (int FK).

## 3. Impacto en Frontend

- Verificar si la API expone `unidadMedidaOriginal` (string). Migrar a `unidadMedidaOriginalId`.

## 4. Análisis paso a paso

- [ ] 1. Identificar todos los usos de `MovimientoInsumo.UnidadMedidaOriginal`.
- [ ] 2. Verificar que `UnidadMedidaOriginalId` existe en la entidad.
- [ ] 3. Migrar comparaciones de string a comparaciones de `UnidadMedidaOriginalId`.
- [ ] 4. Actualizar DTOs y frontend.
- [ ] 5. Compilar backend y frontend.
- [ ] 6. Crear migración para eliminar la columna `UnidadMedidaOriginal`.

## 5. Verificación

- `dotnet build` sin CS0618 para `MovimientoInsumo.UnidadMedidaOriginal`.
- El kardex y el historial de movimientos muestran la unidad correcta.

## 6. Riesgos

- Los valores de `UnidadMedidaOriginal` (string) deben mapearse a los IDs correctos.