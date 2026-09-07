# Análisis: `LogAuditoriaPrecio.UsuarioOperador`

## 1. Identificación

| Campo | Valor |
|-------|-------|
| **Entidad** | `LogAuditoriaPrecio` |
| **Propiedad obsoleta** | `UsuarioOperador` |
| **Reemplazo** | `UsuarioOperadorId` |
| **Mensaje de obsolescencia** | "Usar UsuarioOperadorId. Columna legacy pendiente de DROP." |
| **Archivo de definición** | `src/SistemaSatHospitalario.Core.Domain/Entities/Admision/LogAuditoriaPrecio.cs` |

## 2. Impacto en Backend

**Usos detectados en el build (CS0618):**
- `Queries/Admision/GetPriceAuditLogsQuery.cs` (línea 68)
- `Infrastructure/Persistence/Contexts/SatHospitalarioDbContext.cs` (línea 793)

**Tipo de cambio:** Migrar de `UsuarioOperador` (string) a `UsuarioOperadorId` (Guid).

## 3. Impacto en Frontend

- Verificar si la API expone `usuarioOperador`. Migrar a `usuarioOperadorId`.

## 4. Análisis paso a paso

- [ ] 1. Identificar todos los usos de `LogAuditoriaPrecio.UsuarioOperador`.
- [ ] 2. Verificar que `UsuarioOperadorId` existe en la entidad.
- [ ] 3. Migrar asignaciones/lecturas de `UsuarioOperador` → `UsuarioOperadorId`.
- [ ] 4. Actualizar DTOs y frontend.
- [ ] 5. Compilar backend y frontend.
- [ ] 6. Crear migración para eliminar la columna `UsuarioOperador`.

## 5. Verificación

- `dotnet build` sin CS0618 para `LogAuditoriaPrecio.UsuarioOperador`.
- La auditoría de precios muestra el operador correcto.

## 6. Riesgos

- Requiere backfill de `UsuarioOperadorId` para logs legacy.