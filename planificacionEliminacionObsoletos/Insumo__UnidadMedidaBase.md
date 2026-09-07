# Análisis: `Insumo.UnidadMedidaBase`

## 1. Identificación

| Campo | Valor |
|-------|-------|
| **Entidad** | `Insumo` |
| **Propiedad obsoleta** | `UnidadMedidaBase` |
| **Reemplazo** | `UnidadMedidaId / UnidadMedidaNav` |
| **Mensaje de obsolescencia** | "Usar UnidadMedidaId / UnidadMedidaNav. Columna legacy pendiente de DROP." |
| **Archivo de definición** | `src/SistemaSatHospitalario.Core.Domain/Entities/Admision/Insumo.cs` |

## 2. Impacto en Backend

**Usos detectados en el build (CS0618):**
- `Common/Services/InventoryService.cs` (líneas 68, 122, 141, 215, 236, 298, 351, 354, 463, 490, 521, 652)
- `Queries/Admision/GetUnifiedCatalogQueryHandler.cs` (líneas 155, 172)
- `Queries/Admision/GetKardexQueryHandler.cs` (línea 120)
- `Commands/Admision/ProcesarDevolucionInsumoCommand.cs` (línea 78)
- `Commands/Admision/ProcesarDevolucionCirugiaMasivaCommand.cs` (línea 87)
- `Commands/Admision/EnviarASubAreaCommand.cs` (líneas 66, 88, 99)
- `Commands/Admision/DevolverInsumoCirugiaCommand.cs` (línea 90)
- `Commands/Admision/AnexarCargoExtraCirugiaCommand.cs` (línea 93)
- `WebAPI/Controllers/Admin/InventoryController.cs` (líneas 92, 131, 174, 204, 227, 280, 323, 351, 496, 536, 843, 900, 1033)
- `Infrastructure/Persistence/Contexts/SatHospitalarioDbContext.cs` (líneas 953, 1041)

**Tipo de cambio:** Migrar de `UnidadMedidaBase` (string) a `UnidadMedidaId` (int FK) + `UnidadMedidaNav`.

## 3. Impacto en Frontend

- Verificar si la API expone `unidadMedidaBase` (string) en los DTOs de insumos. Migrar a `unidadMedidaId`.

## 4. Análisis paso a paso

- [ ] 1. Identificar todos los usos de `Insumo.UnidadMedidaBase`.
- [ ] 2. Verificar que `UnidadMedidaId` y `UnidadMedidaNav` existen en la entidad.
- [ ] 3. Migrar comparaciones de string a comparaciones de `UnidadMedidaId`.
- [ ] 4. Actualizar DTOs y frontend.
- [ ] 5. Compilar backend y frontend.
- [ ] 6. Crear migración para eliminar la columna `UnidadMedidaBase`.

## 5. Verificación

- `dotnet build` sin CS0618 para `Insumo.UnidadMedidaBase`.
- El flujo de inventario (kardex, devoluciones, envíos) funciona correctamente.

## 6. Riesgos

- Es una de las propiedades con más usos. Requiere migración cuidadosa.
- Los valores de `UnidadMedidaBase` (string) deben mapearse a los `UnidadMedidaId` correctos.