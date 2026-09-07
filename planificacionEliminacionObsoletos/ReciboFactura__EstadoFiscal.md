# Análisis: `ReciboFactura.EstadoFiscal`

## 1. Identificación

| Campo | Valor |
|-------|-------|
| **Entidad** | `ReciboFactura` |
| **Propiedad obsoleta** | `EstadoFiscal` |
| **Reemplazo** | `EstadoFiscalId / EstadoFiscalNav` |
| **Mensaje de obsolescencia** | "Usar EstadoFiscalId / EstadoFiscalNav. Columna legacy pendiente de DROP." |
| **Archivo de definición** | `src/SistemaSatHospitalario.Core.Domain/Entities/Admision/ReciboFactura.cs` |

## 2. Impacto en Backend

**Usos detectados en el build (CS0618):**
- `Commands/Admision/SettleARCommandHandler.cs` (línea 55)
- `Commands/Admision/RegistrarAltaPacienteCommandHandler.cs` (línea 46)
- `Commands/Admision/UpdateCuentaAdministrativaCommand.cs` (línea 73)
- `Commands/Admision/RegistrarReciboFacturaCommand.cs` (línea 56)
- `Queries/Admision/ObtenerResumenCajasQuery.cs` (línea 53)
- `Queries/Admision/GetCuentasAdministrativasQuery.cs` (líneas 73, 79)
- `Queries/Admision/GetCajaSummariesQuery.cs` (líneas 139)
- `Queries/Admision/GetDailyClosingQueryHandler.cs` (líneas 36, 40)
- `Queries/Admision/ExportCashierAuditQuery.cs` (línea 73)
- `Queries/Admision/GetBusinessInsightsQuery.cs` (líneas 56, 183)

**Tipo de cambio:** Migrar de `EstadoFiscal` (string) a `EstadoFiscalId` (int FK) + `EstadoFiscalNav`.

## 3. Impacto en Frontend

- Verificar si la API expone `estadoFiscal` (string) en los DTOs de recibos. Migrar a `estadoFiscalId`.

## 4. Análisis paso a paso

- [ ] 1. Identificar todos los usos de `ReciboFactura.EstadoFiscal`.
- [ ] 2. Verificar que `EstadoFiscalId` y `EstadoFiscalNav` existen en la entidad.
- [ ] 3. Migrar comparaciones de string a comparaciones de `EstadoFiscalId`.
- [ ] 4. Actualizar DTOs y frontend.
- [ ] 5. Compilar backend y frontend.
- [ ] 6. Crear migración para eliminar la columna `EstadoFiscal`.

## 5. Verificación

- `dotnet build` sin CS0618 para `ReciboFactura.EstadoFiscal`.
- El flujo de recibos/facturas (emitir, anular, cobrar) funciona correctamente.

## 6. Riesgos

- Los valores de `EstadoFiscal` (string) deben mapearse a los `EstadoFiscalId` correctos.
- Es una propiedad con muchos usos en reportes financieros.