# Fase 5: Integridad del Dominio (Entities & Events)

**Objetivo**: Fortalecer las reglas de negocio dentro del núcleo del sistema (Dominio) para garantizar consistencia y facilitar la extensibilidad mediante eventos.

## Implementaciones Clave
1.  **Lógica en Entidades**:
    - Mover validaciones de estado y reglas de cálculo (ej. totales, impuestos) desde los Handlers hacia los métodos de las entidades del dominio.
2.  **Eventos de Dominio**:
    - Implementar un sistema de despacho de eventos para notificaciones internas (ej. `CuentaCerradaEvent`) que desacople procesos secundarios (notificaciones externas, cierres de lote).
3.  **Uso de Value Objects**:
    - Sustituir primitivos por Objetos de Valor en campos críticos (ej. Moneda, RIF) para asegurar que los datos sean siempre válidos al ser instanciados.

## Beneficios Arquitectónicos
- **Integridad**: Es imposible crear un estado de dominio inválido en la base de datos.
- **Escalabilidad**: Nuevos requerimientos (como enviar un email al facturar) se agregan simplemente suscribiendo un nuevo manejador al evento de dominio.

---
**Senior Strategy**: "El dominio es la verdad absoluta; si algo no es válido en el negocio, no puede existir en el código."
