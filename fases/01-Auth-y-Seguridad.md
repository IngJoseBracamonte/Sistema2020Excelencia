# Fase 1: Autenticación y Seguridad

## Prompt para Gemini Flash
Analiza los archivos de esta fase buscando posibles vulnerabilidades o fallos de lógica (como expiración de tokens, inyección de dependencias maliciosa, fallos en protección de rutas, etc.). Genera pruebas automatizadas (unitarias e integración) que aseguren el correcto comportamiento tanto en el frontend (Angular) como en el backend (.NET Core).

## Contexto de Archivos

### Frontend (Angular)
- Directorio principal: `src/SistemaSatHospitalario.Frontend/src/app/features/auth`
- Archivos clave: Componentes de login, interceptores HTTP para tokens, `AuthGuard` para rutas.

### Backend (.NET Core)
- Directorio principal: `src/SistemaSatHospitalario.Core.Application/Commands/Auth`
- Archivos clave: `LoginCommandHandler.cs`, validación de credenciales, emisión de JWT.

## Vectores de Fallo a Evaluar
1. Interceptación y manipulación de JWT.
2. Escalada de privilegios en el frontend (modificación de roles en el almacenamiento local).
3. Fuerza bruta en el endpoint de login.
4. Fallos en el refresco de tokens (si aplica).
