# Fase 2: Gestión de Expedientes y Citas

## Prompt para Gemini Flash
Revisa detalladamente la lógica de agendamiento de turnos, historiales y expediente clínico. Identifica condiciones de carrera, errores en manipulación de fechas/zonas horarias y bloqueos indebidos de horarios. Genera pruebas automatizadas para el frontend y backend que cubran estos escenarios de riesgo.

## Contexto de Archivos

### Frontend (Angular)
- Directorios principales: `src/SistemaSatHospitalario.Frontend/src/app/features/admision/expediente` y `patient-history`
- Archivos clave: `control-citas.component.ts`, `control-citas.component.html`, servicios de API para agendar citas.

### Backend (.NET Core)
- Directorios principales: `Commands/Admision`, `Queries/Admision`
- Archivos clave: `AgendarTurnoCommandHandler.cs`, `ReservarTurnoTemporalCommand.cs`, `BloquearHorarioCommand.cs`.

## Vectores de Fallo a Evaluar
1. Condiciones de carrera al agendar dos pacientes en el mismo bloque horario.
2. Desincronización de la vista del calendario en el Frontend después de que un usuario reserva.
3. Fallos en la visualización o carga del expediente del paciente.
4. Estados inconsistentes al liberar un turno temporal bloqueado pero no confirmado.
