# Fase 3: Aislamiento de Lógica (MediatR Pipelines)

**Objetivo**: Centralizar las preocupaciones transversales (Auditoría, Validación, Logging) fuera de los Handlers para mantener la lógica de negocio pura.

## Implementaciones Clave
1.  **Pipeline de Validación**:
    - Implementar `ValidationBehavior` para ejecutar automáticamente reglas de **FluentValidation** antes de que el comando llegue al Handler.
2.  **Pipeline de Auditoría de Precios**:
    - Mover la lógica de `AuditLogsPrecios` a un `IPipelineBehavior` especializado que compare el estado previo y posterior.
3.  **Manejo Global de Excepciones**:
    - Refinar el `GlobalExceptionHandler` para mapear excepciones de validación a respuestas `400 Bad Request` estandarizadas.

## Beneficios Arquitectónicos
- **Limpieza de Código**: Los Handlers se reducen en un 40-50% al eliminar chequeos repetitivos.
- **Consistencia**: Asegura que las mismas reglas de auditoría y validación se apliquen a todos los comandos de forma automática.

---
**Senior Strategy**: "El mediatR Pipeline es un aeropuerto; la seguridad (validación) y los registros (auditoría) ocurren antes del despegue."
