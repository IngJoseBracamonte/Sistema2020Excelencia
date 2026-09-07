# Análisis: `Notification.TargetUserId`

## 1. Identificación

| Campo | Valor |
|-------|-------|
| **Entidad** | `Notification` |
| **Propiedad obsoleta** | `TargetUserId` |
| **Reemplazo** | `TargetUserGuidId` |
| **Mensaje de obsolescencia** | "Usar TargetUserGuidId. Columna legacy pendiente de DROP." |
| **Archivo de definición** | `src/SistemaSatHospitalario.Core.Domain/Entities/Common/Notification.cs` |

## 2. Impacto en Backend

**Usos detectados en el build (CS0618):**
- `WebAPI/Controllers/Common/NotificationsController.cs` (líneas 32-33, 62-63)
- `Infrastructure/Persistence/Contexts/SatHospitalarioDbContext.cs` (líneas 869, 871)

**Tipo de cambio:** Migrar de `TargetUserId` (string legacy) a `TargetUserGuidId` (Guid).

## 3. Impacto en Frontend

- Verificar si la API expone `targetUserId` en los DTOs de notificaciones. Migrar a `targetUserGuidId`.

## 4. Análisis paso a paso

- [ ] 1. Identificar todos los usos de `Notification.TargetUserId`.
- [ ] 2. Verificar que `TargetUserGuidId` existe en la entidad.
- [ ] 3. Migrar asignaciones/lecturas de `TargetUserId` → `TargetUserGuidId`.
- [ ] 4. Actualizar DTOs y frontend.
- [ ] 5. Compilar backend y frontend.
- [ ] 6. Crear migración para eliminar la columna `TargetUserId`.

## 5. Verificación

- `dotnet build` sin CS0618 para `Notification.TargetUserId`.
- Las notificaciones se entregan al usuario correcto.

## 6. Riesgos

- Requiere backfill de `TargetUserGuidId` para notificaciones legacy.