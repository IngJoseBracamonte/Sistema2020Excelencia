# Fase 3: Facturación y Cobranzas

## Prompt para Gemini Flash
Realiza un análisis profundo sobre los procesos financieros principales: facturación y recepción de pagos. Pon especial foco en la lógica de conversión USD/Bs y en el flujo de pacientes particulares y de aseguradora. Diseña y genera pruebas automatizadas (E2E y unitarias) que validen que no haya pérdida de precisión financiera ni inconsistencias de estado.

## Contexto de Archivos

### Frontend (Angular)
- Directorios principales: `src/SistemaSatHospitalario.Frontend/src/app/features/admision/facturacion` y `receivables`
- Archivos clave: `facturacion.component.ts`, `receivables.component.ts`, formatos de impresión (Compromiso de Pago, Comprobante).

### Backend (.NET Core)
- Directorios principales: `Commands` y `Queries` relacionados a Facturación y Cuentas por Cobrar.
- Archivos clave: Comandos para emitir comprobantes finales, lógica de pago parcial.

## Vectores de Fallo a Evaluar
1. Errores de redondeo con la conversión multi-moneda (USD-First Architecture).
2. Generación incorrecta del "Compromiso de Pago" vs "Comprobante de Pago".
3. Transiciones de estado de facturación inválidas (ej. pasar de Pendiente a Pagado con monto inferior, o problemas al transicionar a Anulado).
4. Persistencia incorrecta de datos del médico asignado durante adición de nuevos servicios.
5. Inconsistencias al cambiar el estado de una factura (Anulado vs Activo) y su impacto en las cuentas por cobrar asociadas.
