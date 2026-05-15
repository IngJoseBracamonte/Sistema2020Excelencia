# Fase 8: Administración y Dashboard

## Prompt para Gemini Flash
Revisa el panel de administración central y los resúmenes del dashboard. Identifica errores de permisos, cuellos de botella en consultas masivas y datos mostrados incorrectamente. Genera pruebas para asegurar la estabilidad de las vistas gerenciales.

## Contexto de Archivos

### Frontend (Angular)
- Directorios principales: `src/SistemaSatHospitalario.Frontend/src/app/features/admin` y `dashboard`

### Backend (.NET Core)
- `Commands/Admin`, `Queries/Admin` (excluyendo honorarios que tienen su propia fase).

## Vectores de Fallo a Evaluar
1. Fallos de autorización que permitan a usuarios normales ver el dashboard gerencial.
2. Tiempos de carga inaceptables en los widgets del dashboard.
3. Cálculos de estadísticas desactualizados respecto a la fuente de la verdad (ej. ingresos diarios).
