# Análisis: `AuditLog.UserId`

## 1. Identificación

| Campo | Valor |
|-------|-------|
| **Entidad** | `AuditLog` |
| **Propiedad obsoleta** | `UserId` |
| **Reemplazo** | `UsuarioIdentityId` |
| **Mensaje de obsolescencia** | "Usar UsuarioIdentityId. Columna legacy pendiente de DROP." |
| **Archivo de definición** | `src/SistemaSatHospitalario.Core.Domain/Entities/Admision/AuditLog.cs` |

## 2. Impacto en Backend

**Usos detectados en el build (CS0618):**
- `Commands/Admision/RegistrarTrasladoAreaCommandHandler.cs` (línea 133)
- `Commands/Admision/RegistrarAltaPacienteCommandHandler.cs` (línea 75)
- `Queries/Admision/GetBusinessInsightsQuery.cs` (línea 295)

**Tipo de cambio:** Migrar de `UserId` (string legacy) a `UsuarioIdentityId` (Guid canónico).

## 3. Impacto en Frontend

- Verificar si la API expone `userId` en los DTOs de auditoría. Si es así, el frontend debe usar el nuevo campo `usuarioIdentityId`.

## 4. Análisis paso a paso

- [ ] 1. Identificar todos los usos de `AuditLog.UserId` en el código (grep `\.UserId` en archivos que usan `AuditLog`).
- [ ] 2. Verificar que `UsuarioIdentityId` existe en la entidad y en la tabla `AuditLog`.
- [ ] 3. Migrar cada asignación/lectura de `UserId` → `UsuarioIdentityId`.
- [ ] 4. Verificar que el `DbContext` mapea `UsuarioIdentityId` correctamente.
- [ ] 5. Actualizar los DTOs de respuesta si exponen `userId`.
- [ ] 6. Actualizar el frontend si consume `userId`.
- [ ] 7. Compilar backend y frontend (0 errores).
- [ ] 8. Crear migración EF para eliminar la columna `UserId` (solo tras confirmar que no se usa).

## 5. Verificación

- `dotnet build` sin advertencias CS0618 para `AuditLog.UserId`.
- Los registros de auditoría siguen mostrando el usuario correcto.

## 6. Riesgos

- Si `UsuarioIdentityId` no está poblado en datos legacy, la migración puede perder la referencia al usuario.
- Requiere backfill de datos si la columna `UsuarioIdentityId` está vacía en registros antiguos.