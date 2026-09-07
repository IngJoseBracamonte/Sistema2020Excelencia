# Análisis: `CajaDiaria.UsuarioId`

## 1. Identificación

| Campo | Valor |
|-------|-------|
| **Entidad** | `CajaDiaria` |
| **Propiedad obsoleta** | `UsuarioId` |
| **Reemplazo** | `UsuarioIdentityId` |
| **Mensaje de obsolescencia** | "Usar UsuarioIdentityId. Columna legacy pendiente de DROP." |
| **Archivo de definición** | `src/SistemaSatHospitalario.Core.Domain/Entities/Admision/CajaDiaria.cs` |

## 2. Impacto en Backend

**Usos detectados en el build (CS0618):**
- `Queries/Admision/ExportCashierAuditQuery.cs` (líneas 50, 124)
- `Queries/Admision/GetCajaSummariesQuery.cs` (línea 91)
- `Queries/Admision/GetDailyClosingQueryHandler.cs` (línea 28)
- `Queries/Admision/GetExpedienteFacturacionQuery.cs` (línea 104)
- `Infrastructure/Persistence/Repositories/CajaAdministrativaRepository.cs` (líneas 39, 68)

**Tipo de cambio:** Migrar de `UsuarioId` (string legacy) a `UsuarioIdentityId` (Guid).

## 3. Impacto en Frontend

- Verificar si la API expone `usuarioId` en los DTOs de caja. Migrar a `usuarioIdentityId`.

## 4. Análisis paso a paso

- [ ] 1. Identificar todos los usos de `CajaDiaria.UsuarioId`.
- [ ] 2. Verificar que `UsuarioIdentityId` existe en la entidad.
- [ ] 3. Migrar asignaciones/lecturas de `UsuarioId` → `UsuarioIdentityId`.
- [ ] 4. Actualizar DTOs y frontend si exponen `usuarioId`.
- [ ] 5. Compilar backend y frontend.
- [ ] 6. Crear migración para eliminar la columna `UsuarioId`.

## 5. Verificación

- `dotnet build` sin CS0618 para `CajaDiaria.UsuarioId`.
- La auditoría de caja muestra el usuario correcto.

## 6. Riesgos

- Requiere backfill de `UsuarioIdentityId` para cajas legacy.