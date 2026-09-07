# Análisis: `CajaDiaria.Estado`

## 1. Identificación

| Campo | Valor |
|-------|-------|
| **Entidad** | `CajaDiaria` |
| **Propiedad obsoleta** | `Estado` |
| **Reemplazo** | `EstadoId / EstadoNav` |
| **Mensaje de obsolescencia** | "Usar EstadoId / EstadoNav. Columna legacy pendiente de DROP." |
| **Archivo de definición** | `src/SistemaSatHospitalario.Core.Domain/Entities/Admision/CajaDiaria.cs` |

## 2. Impacto en Backend

**Usos detectados en el build (CS0618):**
- `Queries/Admision/ObtenerResumenCajasQuery.cs` (líneas 44, 61)
- `Queries/Admision/GetCajaSummariesQuery.cs` (líneas 106, 125)
- `Queries/Admision/GetDailyClosingQueryHandler.cs` (línea 28)
- `Commands/Admision/ConsolidarCajasCommand.cs` (líneas 48, 57-62)
- `Commands/Admision/ForzarCierreCajaCommand.cs` (línea 40)
- `Infrastructure/Persistence/Repositories/CajaAdministrativaRepository.cs` (líneas 26, 33, 39, 55, 64)
- `Infrastructure/Persistence/Contexts/SatHospitalarioDbContext.cs` (líneas 169-172)

**Tipo de cambio:** Migrar de `Estado` (string) a `EstadoId` (int FK) + `EstadoNav`.

## 3. Impacto en Frontend

- Verificar si la API expone `estado` (string) en los DTOs de caja. El frontend probablemente compara `estado === 'Abierta'` o similar. Debe migrarse a `estadoId`.

## 4. Análisis paso a paso

- [ ] 1. Identificar todos los usos de `CajaDiaria.Estado`.
- [ ] 2. Verificar que `EstadoId` y `EstadoNav` existen en la entidad.
- [ ] 3. Migrar comparaciones de string a comparaciones de `EstadoId`.
- [ ] 4. Actualizar los DTOs de respuesta para exponer `estadoId` (y opcionalmente `estadoNombre`).
- [ ] 5. Actualizar el frontend para usar `estadoId` en lugar de `estado`.
- [ ] 6. Compilar backend y frontend.
- [ ] 7. Crear migración para eliminar la columna `Estado`.

## 5. Verificación

- `dotnet build` sin CS0618 para `CajaDiaria.Estado`.
- El flujo de apertura/cierre de caja funciona correctamente.

## 6. Riesgos

- Los valores de `Estado` (string) deben mapearse a los `EstadoId` correctos en el catálogo de estados.
- El frontend puede tener múltiples comparaciones de string que deben actualizarse.