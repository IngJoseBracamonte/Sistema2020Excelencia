# Fase 9: Reportes y Honorarios Médicos

## Prompt para Gemini Flash
Analiza críticamente el sistema de reportes financieros y específicamente el flujo de honorarios médicos (ej. `GetDoctorHonorariumSummaryQuery`). En revisiones anteriores se reportó que "los honorarios ni aparecían". Identifica problemas de mapeo, uniones SQL defectuosas (JOINs) y lógica de presentación en UI. Escribe pruebas exhaustivas de backend y E2E para evitar la pérdida de visibilidad en pagos a doctores.

## Contexto de Archivos

### Frontend (Angular)
- Componentes asociados a la vista de reportes de honorarios (probablemente en `features/admin` o similares).

### Backend (.NET Core)
- Archivo clave: `src/SistemaSatHospitalario.Core.Application/Queries/Admin/GetDoctorHonorariumSummaryQuery.cs`
- Servicios y repositorios de Honorarios (`HonorariumMapperService`, `HonorariumCategory`).

## Vectores de Fallo a Evaluar
1. Honorarios que "desaparecen" de los reportes por fallos en los filtros de fecha (Timezone issues).
2. Agrupación incorrecta (GROUP BY) de honorarios que excluya a ciertos médicos o especialidades.
3. Fallos en la visualización o renderizado del reporte en el frontend al procesar listas vacías o valores nulos.
4. Descuadres entre el honorario calculado y la facturación pagada por el paciente (USD vs Bs).
