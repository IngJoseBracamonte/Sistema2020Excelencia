# Fase 5: Auditoría de Modificación de Precios (Price Audit Log)

**Objetivo**: Implementar un sistema de trazabilidad para cada cambio de precio autorizado en el punto de facturación.

## Actividades
- [ ] Crear la entidad `LogAuditoriaPrecio` en la Capa de Dominio (Id, Usuario, Supervisor, PrecioAnterior, PrecioNuevo, Motivo, Fecha).
- [ ] Configurar el mapeo en `SatHospitalarioDbContext`.
- [ ] Modificar el comando `SincronizarCarrito` o crear un comando específico para registrar la auditoría.
- [ ] Crear una vista administrativa en Angular para consultar estos logs con filtros por fecha y usuario.

## Criterios de Aceptación
- Cada cambio de precio queda registrado de forma inalterable.
- El administrador puede ver quién autorizó cada descuento o aumento.
