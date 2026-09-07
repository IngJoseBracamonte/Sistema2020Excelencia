# Análisis: `LogAuditoriaPrecio.AutorizadoPor`

## 1. Identificación

| Campo | Valor |
|-------|-------|
| **Entidad** | `LogAuditoriaPrecio` |
| **Propiedad obsoleta** | `AutorizadoPor` |
| **Reemplazo** | `AutorizadoPorId` |
| **Mensaje de obsolescencia** | "Usar AutorizadoPorId. Columna legacy pendiente de DROP." |
| **Archivo de definición** | `src/SistemaSatHospitalario.Core.Domain/Entities/Admision/LogAuditoriaPrecio.cs` |

## 2. Impacto en Backend

**Usos detectados en el build (CS0618):**
- `Queries/Admision/GetPriceAuditLogsQuery.cs` (línea 69)
- `Infrastructure/Persistence/Contexts/SatHospitalarioDbContext.cs` (línea 794)

**Tipo de cambio:** Migrar de `AutorizadoPor` (string) a `AutorizadoPorId` (Guid).

## 3. Impacto en Frontend

- Verificar si la API expone `autorizadoPor`. Migrar a `autorizadoPorId`.

## 4. Análisis paso a paso

- [ ] 1. Identificar todos los usos de `LogAuditoriaPrecio.AutorizadoPor`.
- [ ] 2. Verificar que `AutorizadoPorId` existe en la entidad.
- [ ] 3. Migrar asignaciones/lecturas de `AutorizadoPor` → `AutorizadoPorId`.
- [ ] 4. Actualizar DTOs y frontend.
- [ ] 5. Compilar backend y frontend.
- [ ] 6. Crear migración para eliminar la columna `AutorizadoPor`.

## 5. Verificación

- `dotnet build` sin CS0618 para `LogAuditoriaPrecio.AutorizadoPor`.
- La auditoría de precios muestra el autorizador correcto.

## 6. Riesgos

- Requiere backfill de `AutorizadoPorId` para logs legacy.