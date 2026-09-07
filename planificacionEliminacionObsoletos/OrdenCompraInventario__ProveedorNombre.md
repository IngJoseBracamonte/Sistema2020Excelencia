# Análisis: `OrdenCompraInventario.ProveedorNombre`

## 1. Identificación

| Campo | Valor |
|-------|-------|
| **Entidad** | `OrdenCompraInventario` |
| **Propiedad obsoleta** | `ProveedorNombre` |
| **Reemplazo** | `Proveedor.RazonSocial` vía `ProveedorId` |
| **Mensaje de obsolescencia** | "Usar Proveedor.RazonSocial vía ProveedorId. Columna legacy pendiente de DROP." |
| **Archivo de definición** | `src/SistemaSatHospitalario.Core.Domain/Entities/Admision/OrdenCompraInventario.cs` |

## 2. Impacto en Backend

**Usos detectados en el build (CS0618):**
- `Commands/Inventario/RegistrarPagoProveedorCommand.cs` (línea 61)
- `Queries/Inventario/GetOrdenesCompraQuery.cs` (líneas 51, 74, 87)
- `Queries/Inventario/GetHistorialPagosQuery.cs` (líneas 40, 66)
- `Infrastructure/Persistence/Contexts/SatHospitalarioDbContext.cs` (líneas 1580, 1594)

**Tipo de cambio:** Derivar el nombre del proveedor desde `ProveedorId` (join con `Proveedor.RazonSocial`).

## 3. Impacto en Frontend

- Verificar si la API expone `proveedorNombre`. Debe seguir exponiéndose pero calculado.

## 4. Análisis paso a paso

- [ ] 1. Identificar todos los usos de `OrdenCompraInventario.ProveedorNombre`.
- [ ] 2. Verificar que `ProveedorId` está poblado.
- [ ] 3. En los queries, hacer join con `Proveedor` para obtener `RazonSocial`.
- [ ] 4. Mantener el campo en el DTO de respuesta (calculado).
- [ ] 5. Compilar backend y frontend.
- [ ] 6. Crear migración para eliminar la columna `ProveedorNombre`.

## 5. Verificación

- `dotnet build` sin CS0618 para `OrdenCompraInventario.ProveedorNombre`.
- Las órdenes de compra muestran el proveedor correctamente.

## 6. Riesgos

- Requiere join con la tabla de proveedores.
- Si `ProveedorId` está vacío en datos legacy, el nombre no se podrá derivar.