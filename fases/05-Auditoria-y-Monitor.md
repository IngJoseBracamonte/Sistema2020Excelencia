# Fase 5: Auditoría y Monitor

## Prompt para Gemini Flash
Analiza los módulos de monitorización en tiempo real y trazas de auditoría. Identifica fallos de rendimiento y riesgos de manipulación de logs. Escribe pruebas para verificar la resiliencia de la conexión y la inmutabilidad de la auditoría.

## Contexto de Archivos

### Frontend (Angular)
- Directorios principales: `src/SistemaSatHospitalario.Frontend/src/app/features/admision/auditing` y `processing-monitor`
- Archivos clave: `auditing.component.ts`.

### Backend (.NET Core)
- Middleware de auditoría o eventos de dominio.

## Vectores de Fallo a Evaluar
1. Pérdida de eventos importantes en las trazas de auditoría (ej. cambios críticos en historiales).
2. Tiempos de inactividad o fallos en el WebSocket/SignalR para el monitor en tiempo real.
3. Posibilidad de eludir los logs de auditoría mediante peticiones directas.
