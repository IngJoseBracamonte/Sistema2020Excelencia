# Fase 10: Anulaciones, Reversos y Estados de Facturación

## Prompt para Gemini Flash
Analiza críticamente los flujos de anulación de servicios y órdenes médicas, así como los diferentes estados de la facturación (Pendiente, Pagado, Anulado). Identifica vulnerabilidades y fallos lógicos que ocurren cuando se anula un servicio (por ejemplo: el doctor no asistió, el equipo de Rx falló o el paciente canceló). Genera pruebas automatizadas E2E y de integración para validar la consistencia de los datos financieros y médicos tras una anulación.

## Contexto de Archivos

### Frontend (Angular)
- Directorios principales: `src/SistemaSatHospitalario.Frontend/src/app/features/admision/facturacion` y vistas de anulación de servicios en `expediente`.
- Archivos clave: Componentes encargados de anular recibos, órdenes médicas o servicios individuales.

### Backend (.NET Core)
- Directorios principales: `Commands` y `Queries` de Facturación y Órdenes.
- Archivos clave: Servicios encargados de cambiar el estado de las facturas, reversar pagos y anular órdenes médicas.

## Vectores de Fallo a Evaluar
1. **Consistencia Financiera:** Anular una factura pagada sin generar el reverso de caja o nota de crédito correspondiente.
2. **Estados de Órdenes:** Anular una orden de Rx/Laboratorio y que esta siga apareciendo como pendiente de realizar o facturada.
3. **Manejo de Honorarios:** Reversar un servicio médico y que el honorario del doctor siga apareciendo en el reporte de pagos (`GetDoctorHonorariumSummaryQuery`).
4. **Validaciones de Estado:** Permitir anular una orden que ya fue procesada/entregada sin los permisos gerenciales requeridos.
5. **Cuentas por Cobrar:** Cancelar un "Compromiso de Pago" y no limpiar adecuadamente la deuda pendiente del paciente.
