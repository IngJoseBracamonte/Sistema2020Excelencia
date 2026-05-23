# Fase 11: Métodos de Pago Dinámicos y CRUD de Configuraciones

## Prompt para Gemini Flash
Analiza y audita la integridad del sistema de métodos de pago dinámicos y su clasificación por Grupo de Moneda (Grupo 1 = USD base, Grupo 2 = VES bolívares). Verifica que todas las pantallas de facturación y cobro (cierre de cuenta, pago multidivisa, liquidación de cuentas por cobrar) y los arqueos de caja utilicen el grupo de moneda del método de pago para realizar los cálculos y validaciones de equivalentes en el backend. Asegura que los campos `MontoAbonadoMoneda`, `EquivalenteAbonadoBase`, `TasaCambioAplicada`, `FechaPago` y `UsuarioCarga` se almacenen de forma atómica en `DetallePago`. Diseña y ejecuta pruebas unitarias para el motor de conversión y el CRUD administrativo de métodos de pago en el panel de configuraciones.

## Contexto de Archivos

### Frontend (Angular)
- **Directorio de configuración**: `src/SistemaSatHospitalario.Frontend/src/app/features/admin/settings` (system-settings.component.ts y system-settings.component.html)
- **Directorio de pagos**: `src/SistemaSatHospitalario.Frontend/src/app/features/admision/facturacion/components/payment-module` (payment-module.component.ts)
- **Servicios clave**: `catalog.service.ts` y `billing-facade.service.ts`

### Backend (.NET Core)
- **Dominio**: `SistemaSatHospitalario.Core.Domain/Entities/Admision/CatalogoMetodoPago.cs` y `DetallePago.cs`
- **Comandos CRUD**: `CreatePaymentMethodCommand.cs`, `UpdatePaymentMethodCommand.cs`, `DeletePaymentMethodCommand.cs`
- **Comandos de Negocio**: `RegistrarReciboFacturaCommand.cs`, `CloseAccountCommandHandler.cs`, `SettleARCommandHandler.cs`, `CerrarCajaCommand.cs`
- **Queries**: `GetPaymentMethodsQuery.cs`
- **API**: `CatalogController.cs`
- **Persistencia**: `SatHospitalarioDbContext.cs` y `SystemDbInitializer.cs`

## Vectores de Fallo a Evaluar
1. **División por Cero**: Intentar registrar abonos en métodos de Grupo 2 (VES) cuando la tasa de cambio es `0` o nula. Debe fallar de forma segura en el backend y emitir una excepción controlada.
2. **Inconsistencia de Redondeo**: Diferencias decimales acumuladas en la base de datos entre el total del recibo facturado y la suma de los equivalentes debido a redondeos inadecuados.
3. **Integridad Referencial en Eliminación**: Permitir la eliminación física de un método de pago del catálogo (`CatalogoMetodosPago`) cuando ya existen recibos y cobros históricos con dicho método, rompiendo la trazabilidad. Debe deactivarse en su lugar (`Activo = false`).
4. **Desincronización de Tasa Aplicada**: Almacenar la tasa de cambio del día en el recibo pero no guardarla en la fila de detalle de pago (`TasaCambioAplicada`), impidiendo auditorías de transacciones multidivisa retroactivas.
5. **Modificación de Valor Único**: Permitir crear o actualizar dos métodos de pago con el mismo `Valor` interno (ej: dos "Zelle"), causando fallos en los mapeos y sumas de cierres de caja.
