# Análisis: `TriageEnfermeria.UsuarioRegistro`

## 1. Identificación

| Campo | Valor |
|-------|-------|
| **Entidad** | `TriageEnfermeria` |
| **Propiedad obsoleta** | `UsuarioRegistro` |
| **Reemplazo** | `UsuarioRegistroId` |
| **Mensaje de obsolescencia** | "Usar UsuarioRegistroId. Columna legacy pendiente de DROP." |
| **Archivo de definición** | `src/SistemaSatHospitalario.Core.Domain/Entities/Admision/TriageEnfermeria.cs` |

## 2. Impacto en Backend

**Usos detectados en el build (CS0618):**
- `Commands/Admision/RegistrarTriageYValoracionCommandHandler.cs` (línea 142)
- `Commands/Admision/ModificarTriageYValoracionCommandHandler.cs` (línea 104)
- `Queries/Admision/GetTriageYValoracionHistoryQueryHandler.cs` (línea 77)
- `Queries/Admision/GetCamasMonitoreoQuery.cs` (línea 125)
- `Queries/Admision/GetNurseAuditReportQueryHandler.cs` (líneas 41, 75)
- `Infrastructure/Persistence/Contexts/SatHospitalarioDbContext.cs` (línea 600)

**Tipo de cambio:** Migrar de `UsuarioRegistro` (string) a `UsuarioRegistroId` (Guid).

## 3. Impacto en Frontend

- Verificar si la API expone `usuarioRegistro`. Migrar a `usuarioRegistroId`.

## 4. Análisis paso a paso

- [ ] 1. Identificar todos los usos de `TriageEnfermeria.UsuarioRegistro`.
- [ ] 2. Verificar que `UsuarioRegistroId` existe en la entidad.
- [ ] 3. Migrar asignaciones/lecturas de `UsuarioRegistro` → `UsuarioRegistroId`.
- [ ] 4. Actualizar DTOs y frontend.
- [ ] 5. Compilar backend y frontend.
- [ ] 6. Crear migración para eliminar la columna `UsuarioRegistro`.

## 5. Verificación

- `dotnet build` sin CS0618 para `TriageEnfermeria.UsuarioRegistro`.
- El triage de enfermería muestra el usuario de registro correcto.

## 6. Riesgos

- Requiere backfill de `UsuarioRegistroId` para triages legacy.