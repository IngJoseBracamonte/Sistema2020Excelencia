---
name: billing-pro-techniques
description: Proporciona patrones y técnicas para implementar sistemas de facturación unificados, gestión de cuentas por cobrar y flujos de carrito en salud.
---

# Billing Pro Techniques

## Cuándo usar este skill
- Cuando necesites implementar o refactorizar un sistema de facturación complejo en salud.
- Cuando el negocio requiera unificar flujos de servicios inmediatos (Citas) con diferidos (Hospitalización).
- Cuando se requiera integración con sistemas externos (Legacy, RX) mediante triggers de servicios.

## Principios Técnicos
1. **Abstracción de Cuenta Única**: Usar una entidad `CuentaServicios` como contenedor temporal de cargos antes de la emisión del recibo fiscal.
2. **Carga Progresiva (Cart Model)**: Permitir la adición de servicios en diferentes momentos (ideal para Hospitalización/Emergencia).
3. **Validación de Slots (Citas)**: Siempre validar la disponibilidad del recurso (Médico) antes de confirmar el cargo para evitar sobre-venta.
4. **Trigger Service Pattern**: Desacoplar la creación de órdenes en sistemas externos (RX, Lab, Legacy) de la lógica de facturación mediante una interfaz de integración.

## Workflow de Implementación
1. **Fase de Cargo**: Identificar paciente -> Abrir/Recuperar Cuenta -> Cargar Servicios -> Disparar Notificaciones Externas.
2. **Fase de Cobro**: Consolidar Cargos -> Registrar Pagos Multidivisa -> Cerrar Cuenta -> Emitir Recibo.

## Recomendaciones de UI
- **Selector de Modo**: Switch para alternar entre "Particular" y "Seguro".
- **Filtro Jerárquico**: Especialidad -> Médico -> Horario para citas.
- **Visualización de Saldo**: Mostrar siempre el total cargado vs el total abonado en tiempo real.
