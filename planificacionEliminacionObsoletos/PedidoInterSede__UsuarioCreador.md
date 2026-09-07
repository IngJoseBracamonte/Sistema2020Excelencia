# Análisis: `PedidoInterSede.UsuarioCreador`

## 1. Identificación

| Campo | Valor |
|-------|-------|
| **Entidad** | `PedidoInterSede` |
| **Propiedad obsoleta** | `UsuarioCreador` |
| **Reemplazo** | `UsuarioCreadorId` |
| **Mensaje de obsolescencia** | "Usar UsuarioCreadorId. Columna legacy pendiente de DROP." |
| **Archivo de definición** | `src/SistemaSatHospitalario.Core.Domain/Entities/Admision/PedidoInterSede.cs` |

## 2. Impacto en Backend

**Usos detectados en el build (CS0618):**
- `Queries/Admision/GetPedidosInterSedePendientesQueryHandler.cs` (línea 106)
- `Queries/Admision/GetPedidosInterSedeHistorialQueryHandler.cs` (línea 45)
- `WebAPI/Controllers/Admin/InventoryController.cs` (línea 282)

**Tipo de cambio:** Migrar de `UsuarioCreador` (string) a `UsuarioCreadorId` (Guid).

## 3. Impacto en Frontend

- Verificar si la API expone `usuarioCreador`. Migrar a `usuarioCreadorId`.

## 4. Análisis paso a paso

- [ ] 1. Identificar todos los usos de `PedidoInterSede.UsuarioCreador`.
- [ ] 2. Verificar que `UsuarioCreadorId` existe en la entidad.
- [ ] 3. Migrar asignaciones/lecturas de `UsuarioCreador` → `UsuarioCreadorId`.
- [ ] 4. Actualizar DTOs y frontend.
- [ ] 5. Compilar backend y frontend.
- [ ] 6. Crear migración para eliminar la columna `UsuarioCreador`.

## 5. Verificación

- `dotnet build` sin CS0618 para `PedidoInterSede.UsuarioCreador`.
- Los pedidos inter-sede muestran el usuario creador correcto.

## 6. Riesgos

- Requiere backfill de `UsuarioCreadorId` para pedidos legacy.