# Fase 7: Optimización de Observabilidad en Base de Datos

**Objetivo**: Asegurar la escalabilidad del sistema a medida que crecen los registros de facturación y logs.

## Actividades
- [ ] Implementar un log de "Slow Queries" (Consultas Lentas > 500ms) que se auto-notifique en OpenTelemetry.
- [ ] Optimizar `GetDoctorHonorariaReportQuery` mediante el uso de proyecciones DTO directas en SQL (evitar cargar objetos completos de dominio).
- [ ] Configurar caché de memoria (IMemoryCache) para catálogos que cambian poco (Especialidades, Convenios).
- [ ] Revisión de índices en `CitaMedica` y `CuentaServicio`.

## Criterios de Aceptación
- Reportes administrativos cargan en menos de 1 segundo con 10,000+ registros.
- Trazabilidad detallada en Aspire Dashboard sobre cuellos de botella SQL.
