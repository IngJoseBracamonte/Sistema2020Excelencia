# Fase 6: Órdenes de Rx y Laboratorio

## Prompt para Gemini Flash
Revisa el flujo de interoperabilidad entre el sistema moderno y el sistema legacy (Sistema2020 - Base de datos MySQL). Busca puntos de falla en la sincronización, IDs desfasados y resolución de identidades. Genera pruebas automatizadas que simulen desconexiones y conflictos de integración.

## Contexto de Archivos

### Frontend (Angular)
- Directorio principal: `src/SistemaSatHospitalario.Frontend/src/app/features/rx-orders`

### Backend (.NET Core)
- Servicios de Sincronización Legacy (ver conocimiento en KI `legacy_sync_patterns.md`).

## Vectores de Fallo a Evaluar
1. Interrupciones durante la sincronización o envíos dobles.
2. Fallos al resolver la identidad del paciente (JIT Patient Onboarding).
3. Estados huérfanos de órdenes de radiología.
