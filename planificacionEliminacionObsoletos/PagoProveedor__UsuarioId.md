# Análisis: `PagoProveedor.UsuarioId`

## 1. Identificación

| Campo | Valor |
|-------|-------|
| **Entidad** | `PagoProveedor` |
| **Propiedad obsoleta** | `UsuarioId` |
| **Reemplazo** | `UsuarioIdentityId` |
| **Mensaje de obsolescencia** | "Usar UsuarioIdentityId. Columna legacy pendiente de DROP." |
| **Archivo de definición** | `src/SistemaSatHospitalario.Core.Domain/Entities/Admision/PagoProveedor.cs` |

## 2. Impacto en Backend

**Usos detectados en el build (CS0618):**
- `Commands/Inventario/RegistrarPagoProveedorCommand.cs` (línea 68)
- `Queries/Inventario/GetOrdenesCompraQuery.cs` (línea 94)
- `Queries/Inventario/GetHistorialPagosQuery.cs` (líneas 43, 73)
- `Infrastructure/Persistence/Contexts/SatHospitalarioDbContext.cs` (línea 1607)

**Tipo de cambio:** Migrar de `UsuarioId` (string legacy) a `UsuarioIdentityId` (Guid).

## 3. Impacto en Frontend

- Verificar si la API expone `usuarioId` en los DTOs de pagos a proveedores. Migrar a `usuarioIdentityId`.

## 4. Análisis paso a paso

- [ ] 1. Identificar todos los usos de `PagoProveedor.UsuarioId`.
- [ ] 2. Verificar que `UsuarioIdentityId` existe en la entidad.
- [ ] 3. Migrar asignaciones/lecturas de `UsuarioId` → `UsuarioIdentityId`.
- [ ] 4. Actualizar DTOs y frontend.
- [ ] 5. Compilar backend y frontend.
- [ ] 6. Crear migración para eliminar la columna `UsuarioId`.

## 5. Verificación

- `dotnet build` sin CS0618 para `PagoProveedor.UsuarioId`.
- Los pagos a proveedores muestran el usuario correcto.

## 6. Riesgos

- Requiere backfill de `UsuarioIdentityId` para pagos legacy.