# Análisis: `DetallePago.MetodoPago`

## 1. Identificación

| Campo | Valor |
|-------|-------|
| **Entidad** | `DetallePago` |
| **Propiedad obsoleta** | `MetodoPago` |
| **Reemplazo** | `MetodoPagoId / MetodoPagoNav` |
| **Mensaje de obsolescencia** | "Usar MetodoPagoId / MetodoPagoNav. Columna legacy pendiente de DROP." |
| **Archivo de definición** | `src/SistemaSatHospitalario.Core.Domain/Entities/Admision/DetallePago.cs` |

## 2. Impacto en Backend

**Usos detectados en el build (CS0618):**
- `Queries/Admision/GetReciboPdfQuery.cs` (línea 64)
- `Queries/Admision/GetPendingARQueryHandler.cs` (línea 113)
- `Commands/Admision/ForzarCierreCajaCommand.cs` (líneas 73, 81)
- `Commands/Admision/CerrarCajaCommand.cs` (líneas 83, 92)
- `Commands/Admision/DeletePaymentMethodCommand.cs` (líneas 40, 66)
- `Queries/Admision/ExportFinancialReportQuery.cs` (línea 91)
- `Queries/Admision/ExportCashierAuditQuery.cs` (líneas 154-155, 164, 199, 207)
- `Queries/Admision/GetCajaSummariesQuery.cs` (líneas 162, 170)
- `Queries/Admision/GetDailyClosingQueryHandler.cs` (línea 55)
- `Queries/Admision/GetExpedienteFacturacionQuery.cs` (línea 118)

**Tipo de cambio:** Migrar de `MetodoPago` (string) a `MetodoPagoId` (int FK) + `MetodoPagoNav`.

## 3. Impacto en Frontend

- Verificar si la API expone `metodoPago` (string) en los DTOs de pagos. Migrar a `metodoPagoId`.

## 4. Análisis paso a paso

- [ ] 1. Identificar todos los usos de `DetallePago.MetodoPago`.
- [ ] 2. Verificar que `MetodoPagoId` y `MetodoPagoNav` existen en la entidad.
- [ ] 3. Migrar comparaciones de string a comparaciones de `MetodoPagoId`.
- [ ] 4. Actualizar DTOs y frontend.
- [ ] 5. Compilar backend y frontend.
- [ ] 6. Crear migración para eliminar la columna `MetodoPago`.

## 5. Verificación

- `dotnet build` sin CS0618 para `DetallePago.MetodoPago`.
- El flujo de pagos (efectivo, punto, transferencia) funciona correctamente.

## 6. Riesgos

- Los valores de `MetodoPago` (string) deben mapearse a los `MetodoPagoId` correctos.
- Es una propiedad con muchos usos en reportes financieros.