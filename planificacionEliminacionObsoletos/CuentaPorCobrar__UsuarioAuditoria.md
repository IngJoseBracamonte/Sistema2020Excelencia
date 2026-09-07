# Análisis: `CuentaPorCobrar.UsuarioAuditoria`

## 1. Identificación

| Campo | Valor |
|-------|-------|
| **Entidad** | `CuentaPorCobrar` |
| **Propiedad obsoleta** | `UsuarioAuditoria` |
| **Reemplazo** | `UsuarioAuditoriaId` |
| **Mensaje de obsolescencia** | "Usar UsuarioAuditoriaId. Columna legacy pendiente de DROP." |
| **Archivo de definición** | `src/SistemaSatHospitalario.Core.Domain/Entities/Admision/CuentaPorCobrar.cs` |

## 2. Impacto en Backend

**Usos detectados en el build (CS0618):**
- `Queries/Admision/ExportFinancialReportQuery.cs` (línea 53)

**Tipo de cambio:** Migrar de `UsuarioAuditoria` (string) a `UsuarioAuditoriaId` (Guid).

## 3. Impacto en Frontend

- Verificar si la API expone `usuarioAuditoria`. Migrar a `usuarioAuditoriaId`.

## 4. Análisis paso a paso

- [ ] 1. Identificar todos los usos de `CuentaPorCobrar.UsuarioAuditoria`.
- [ ] 2. Verificar que `UsuarioAuditoriaId` existe en la entidad.
- [ ] 3. Migrar asignaciones/lecturas de `UsuarioAuditoria` → `UsuarioAuditoriaId`.
- [ ] 4. Actualizar DTOs y frontend.
- [ ] 5. Compilar backend y frontend.
- [ ] 6. Crear migración para eliminar la columna `UsuarioAuditoria`.

## 5. Verificación

- `dotnet build` sin CS0618 para `CuentaPorCobrar.UsuarioAuditoria`.
- El reporte financiero muestra el usuario de auditoría correcto.

## 6. Riesgos

- Requiere backfill de `UsuarioAuditoriaId` para cuentas legacy.