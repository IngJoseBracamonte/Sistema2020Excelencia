# Análisis: `ServicioInsumoReceta.UnidadMedidaConsumo`

## 1. Identificación

| Campo | Valor |
|-------|-------|
| **Entidad** | `ServicioInsumoReceta` |
| **Propiedad obsoleta** | `UnidadMedidaConsumo` |
| **Reemplazo** | `UnidadMedidaConsumoId / UnidadMedidaNav` |
| **Mensaje de obsolescencia** | "Usar UnidadMedidaConsumoId / UnidadMedidaNav. Columna legacy pendiente de DROP." |
| **Archivo de definición** | `src/SistemaSatHospitalario.Core.Domain/Entities/Admision/ServicioInsumoReceta.cs` |

## 2. Impacto en Backend

**Usos detectados en el build (CS0618):**
- `Common/Services/InventoryService.cs` (líneas 122, 158)
- `Queries/Admision/GetUnifiedCatalogQueryHandler.cs` (línea 71)
- `WebAPI/Controllers/Admision/CatalogController.cs` (línea 132)
- `WebAPI/Controllers/Admin/InventoryController.cs` (líneas 1006, 1052)
- `Infrastructure/Persistence/Contexts/SatHospitalarioDbContext.cs` (línea 1041)

**Tipo de cambio:** Migrar de `UnidadMedidaConsumo` (string) a `UnidadMedidaConsumoId` (int FK).

## 3. Impacto en Frontend

- Verificar si la API expone `unidadMedidaConsumo` (string). Migrar a `unidadMedidaConsumoId`.

## 4. Análisis paso a paso

- [ ] 1. Identificar todos los usos de `ServicioInsumoReceta.UnidadMedidaConsumo`.
- [ ] 2. Verificar que `UnidadMedidaConsumoId` existe en la entidad.
- [ ] 3. Migrar comparaciones de string a comparaciones de `UnidadMedidaConsumoId`.
- [ ] 4. Actualizar DTOs y frontend.
- [ ] 5. Compilar backend y frontend.
- [ ] 6. Crear migración para eliminar la columna `UnidadMedidaConsumo`.

## 5. Verificación

- `dotnet build` sin CS0618 para `ServicioInsumoReceta.UnidadMedidaConsumo`.
- El catálogo de servicios con insumos muestra la unidad correcta.

## 6. Riesgos

- Los valores de `UnidadMedidaConsumo` (string) deben mapearse a los IDs correctos.