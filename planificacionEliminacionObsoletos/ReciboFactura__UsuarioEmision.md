# Análisis: `ReciboFactura.UsuarioEmision`

## 1. Identificación

| Campo | Valor |
|-------|-------|
| **Entidad** | `ReciboFactura` |
| **Propiedad obsoleta** | `UsuarioEmision` |
| **Reemplazo** | `UsuarioEmisionId` |
| **Mensaje de obsolescencia** | "Usar UsuarioEmisionId. Columna legacy pendiente de DROP." |
| **Archivo de definición** | `src/SistemaSatHospitalario.Core.Domain/Entities/Admision/ReciboFactura.cs` |

## 2. Impacto en Backend

**Usos detectados en el build (CS0618):**
- `Queries/Admision/ExportCashierAuditQuery.cs` (línea 156)

**Tipo de cambio:** Migrar de `UsuarioEmision` (string) a `UsuarioEmisionId` (Guid).

## 3. Impacto en Frontend

- Verificar si la API expone `usuarioEmision`. Migrar a `usuarioEmisionId`.

## 4. Análisis paso a paso

- [ ] 1. Identificar todos los usos de `ReciboFactura.UsuarioEmision`.
- [ ] 2. Verificar que `UsuarioEmisionId` existe en la entidad.
- [ ] 3. Migrar asignaciones/lecturas de `UsuarioEmision` → `UsuarioEmisionId`.
- [ ] 4. Actualizar DTOs y frontend.
- [ ] 5. Compilar backend y frontend.
- [ ] 6. Crear migración para eliminar la columna `UsuarioEmision`.

## 5. Verificación

- `dotnet build` sin CS0618 para `ReciboFactura.UsuarioEmision`.
- La auditoría de recibos muestra el usuario de emisión correcto.

## 6. Riesgos

- Requiere backfill de `UsuarioEmisionId` para recibos legacy.