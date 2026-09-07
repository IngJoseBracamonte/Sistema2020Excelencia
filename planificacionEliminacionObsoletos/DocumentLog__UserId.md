# Análisis: `DocumentLog.UserId`

## 1. Identificación

| Campo | Valor |
|-------|-------|
| **Entidad** | `DocumentLog` |
| **Propiedad obsoleta** | `UserId` |
| **Reemplazo** | `UsuarioIdentityId` |
| **Mensaje de obsolescencia** | "Usar UsuarioIdentityId. Columna legacy pendiente de DROP." |
| **Archivo de definición** | `src/SistemaSatHospitalario.Core.Domain/Entities/Admision/DocumentLog.cs` |

## 2. Impacto en Backend

**Usos detectados en el build (CS0618):**
- `Infrastructure/Persistence/Contexts/SatHospitalarioDbContext.cs` (línea 856)

**Tipo de cambio:** Migrar de `UserId` (string legacy) a `UsuarioIdentityId` (Guid).

## 3. Impacto en Frontend

- Verificar si la API expone `userId` en los DTOs de logs de documentos. Migrar a `usuarioIdentityId`.

## 4. Análisis paso a paso

- [ ] 1. Identificar todos los usos de `DocumentLog.UserId`.
- [ ] 2. Verificar que `UsuarioIdentityId` existe en la entidad.
- [ ] 3. Migrar asignaciones/lecturas de `UserId` → `UsuarioIdentityId`.
- [ ] 4. Actualizar DTOs y frontend.
- [ ] 5. Compilar backend y frontend.
- [ ] 6. Crear migración para eliminar la columna `UserId`.

## 5. Verificación

- `dotnet build` sin CS0618 para `DocumentLog.UserId`.
- Los logs de documentos muestran el usuario correcto.

## 6. Riesgos

- Requiere backfill de `UsuarioIdentityId` para logs legacy.