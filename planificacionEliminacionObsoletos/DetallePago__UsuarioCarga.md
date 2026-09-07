# Análisis: `DetallePago.UsuarioCarga`

## 1. Identificación

| Campo | Valor |
|-------|-------|
| **Entidad** | `DetallePago` |
| **Propiedad obsoleta** | `UsuarioCarga` |
| **Reemplazo** | `UsuarioCargaId` |
| **Mensaje de obsolescencia** | "Usar UsuarioCargaId. Columna legacy pendiente de DROP." |
| **Archivo de definición** | `src/SistemaSatHospitalario.Core.Domain/Entities/Admision/DetallePago.cs` |

## 2. Impacto en Backend

**Usos detectados en el build (CS0618):**
- `Queries/Admision/ExportCashierAuditQuery.cs` (línea 168)

**Tipo de cambio:** Migrar de `UsuarioCarga` (string) a `UsuarioCargaId` (Guid).

## 3. Impacto en Frontend

- Verificar si la API expone `usuarioCarga`. Migrar a `usuarioCargaId`.

## 4. Análisis paso a paso

- [ ] 1. Identificar todos los usos de `DetallePago.UsuarioCarga`.
- [ ] 2. Verificar que `UsuarioCargaId` existe en la entidad.
- [ ] 3. Migrar asignaciones/lecturas de `UsuarioCarga` → `UsuarioCargaId`.
- [ ] 4. Actualizar DTOs y frontend.
- [ ] 5. Compilar backend y frontend.
- [ ] 6. Crear migración para eliminar la columna `UsuarioCarga`.

## 5. Verificación

- `dotnet build` sin CS0618 para `DetallePago.UsuarioCarga`.
- La auditoría de pagos muestra el usuario de carga correcto.

## 6. Riesgos

- Requiere backfill de `UsuarioCargaId` para pagos legacy.