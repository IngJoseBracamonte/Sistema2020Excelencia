# 📝 Diario de Pasos de Ingeniería (StepJournal.md)

Registro detallado de acciones atómicas y decisiones tomadas en tiempo real.

## 🗓️ 2026-03-24 (Fase de Telemetría e Inteligencia)
### 1. Corrección de Tipo en OpenTelemetry JS
- **Acción**: Se refactorizó `telemetry.service.ts` para usar `resourceFromAttributes`.
- **Contexto**: El error `TS2693` bloqueaba el build de Angular 19.
- **Resultado**: Build exitosa en 20s.

### 2. Visibilidad Total de Base de Datos
- **Acción**: Se activó `SetDbStatementForText` en `Extensions.cs`.
- **Razón**: El usuario no veía las operaciones SQL en Aspire.
- **Resultado**: Trazas detalladas habilitadas.

### 3. Evolución a Arquitectura de Memoria v2.1 (Deep Context)
- **Acción**: Se desglosó la documentación en 7 archivos especializados y se enriqueció cada uno con contexto profundo.
- **Propósito**: Satisfacer la necesidad de una IA con memoria técnica exhaustiva y evitar re-lecturas de código.
- **Impacto**: Se incluyeron diagramas Mermaid en `Architecture.md` y patrones de ingeniería en `Rules.md`.

### 4. Corrección de Autenticación 401 (DTO Mismatch)
- **Acción**: Se sincronizó el DTO `JwtAuthResult` del backend con el `AuthResponse` del frontend.
- **Razón**: El backend devolvía `userId` y no incluía `username`, lo que causaba que el frontend invalidara la sesión al no encontrar campos esperados, dejando de enviar el token Bearer en las peticiones.
- **Resultado**: Sesión persistente y fin de los errores 401 en el catálogo.

### 5. Verificación de la Ruta de Agendamiento (Scheduling Path)
- **Acción**: Auditoría completa del flujo desde `FacturacionComponent` -> `BillingController` -> `MediatR` (ReservarTurno y CargarServicio) -> `MySQL`.
- **Contexto**: El usuario solicitó verificar "toda la ruta para agendar" usando skills de arquitectura.
- **Resultado**: Ruta validada con consistencia en normalización de fechas (minutos) y protección de slots mediante `ReservaTemporal` (15 min) y `CitaMedica`. Documentación actualizada en `DataFlow.md`.

## 📌 Lecciones del Día
- **Serialization Mismatch**: Recordar que `.NET` serializa Guid como string `userId` (camelCase) por defecto, mientras que el frontend esperaba `id`. Siempre mapear explícitamente en el pipe `tap` del `AuthService`.
- **PowerShell Policies**: No usar `npm run build` directo en Windows; preferir `npm.cmd` o bypass de políticas para evitar fallos de seguridad en la terminal.
- **File Locking**: Matar siempre el proceso de la API antes de compilar `ServiceDefaults`, de lo contrario, la DLL estará bloqueada.
