# Análisis: `Insumo.Categoria`

## 1. Identificación

| Campo | Valor |
|-------|-------|
| **Entidad** | `Insumo` |
| **Propiedad obsoleta** | `Categoria` |
| **Reemplazo** | `CategoriaInsumoId / CategoriaInsumo` |
| **Mensaje de obsolescencia** | "Usar CategoriaInsumoId / CategoriaInsumo. Columna legacy pendiente de DROP." |
| **Archivo de definición** | `src/SistemaSatHospitalario.Core.Domain/Entities/Admision/Insumo.cs` |

## 2. Impacto en Backend

**Usos detectados en el build (CS0618):**
- `WebAPI/Controllers/Admin/InventoryController.cs` (líneas 134, 177, 499, 539, 620, 846)
- `Infrastructure/Persistence/Contexts/SatHospitalarioDbContext.cs` (línea 962)

**Tipo de cambio:** Migrar de `Categoria` (string) a `CategoriaInsumoId` (int FK) + `CategoriaInsumo`.

## 3. Impacto en Frontend

- Verificar si la API expone `categoria` (string) en los DTOs de insumos. Migrar a `categoriaInsumoId`.

## 4. Análisis paso a paso

- [ ] 1. Identificar todos los usos de `Insumo.Categoria`.
- [ ] 2. Verificar que `CategoriaInsumoId` y `CategoriaInsumo` existen en la entidad.
- [ ] 3. Migrar comparaciones de string a comparaciones de `CategoriaInsumoId`.
- [ ] 4. Actualizar DTOs y frontend.
- [ ] 5. Compilar backend y frontend.
- [ ] 6. Crear migración para eliminar la columna `Categoria`.

## 5. Verificación

- `dotnet build` sin CS0618 para `Insumo.Categoria`.
- El catálogo de insumos muestra la categoría correctamente.

## 6. Riesgos

- Los valores de `Categoria` (string) deben mapearse a los `CategoriaInsumoId` correctos.