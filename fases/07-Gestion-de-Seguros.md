# Fase 7: Gestión de Seguros

## Prompt para Gemini Flash
Analiza el flujo de atención para pacientes tipo "Seguro". Identifica posibles brechas en el cálculo de deducibles, validación de pólizas y emisión de facturas institucionales. Genera pruebas E2E y de integración.

## Contexto de Archivos

### Frontend (Angular)
- Directorio principal: `src/SistemaSatHospitalario.Frontend/src/app/features/seguros`

### Backend (.NET Core)
- Control de Seguros y Pólizas.

## Vectores de Fallo a Evaluar
1. Facturar por error servicios a la cuenta del paciente titular en lugar de la aseguradora.
2. Calcular incorrectamente coberturas o copagos.
3. Fallos en el control de límites de la póliza (agotamiento de monto).
