# Fase 4: Cajas y Arqueo

## Prompt para Gemini Flash
Evalúa la integridad de las operaciones de cierre de caja y los reportes de arqueo. Identifica posibles vulnerabilidades de seguridad (modificación de datos post-cierre) e inconsistencias de suma. Genera pruebas E2E y de integración para estos casos.

## Contexto de Archivos

### Frontend (Angular)
- Directorios principales: `src/SistemaSatHospitalario.Frontend/src/app/features/admision/cajas` y `arqueo-caja`

### Backend (.NET Core)
- Lógica de Cajas y Arqueos en Application Core.

## Vectores de Fallo a Evaluar
1. Discrepancias de montos (especialmente en cálculos con múltiples métodos de pago).
2. Permisos incorrectos permitiendo revertir un cierre de caja a usuarios no autorizados.
3. Fallos de carga del componente de arqueo al consultar grandes volúmenes de transacciones.
