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

## 🗓️ 2026-03-26 (Sincronización de Memoria y AppHost)
### 1. Corrección de Error CS1061 en Agenda
- **Acción**: Se eliminó la referencia a `UsuarioCarga` en `GetDoctorScheduleQueryHandler.cs`.
- **Razón**: La entidad `CitaMedica` no posee dicha propiedad, lo que causaba el fallo del build tras una actualización previa.
- **Resultado**: Compilación exitosa del ensamblado `Application`.

### 2. Despliegue Exitoso desde AppHost
- **Acción**: Ejecución de `dotnet run` sobre el proyecto Aspire AppHost.
- **Resultado**: Orquestación activa. Dashboard disponible con token: `37a9817797ed5ee25a224774d6dc6548`.

### 3. Fix: Identity Migration Bypass (Table 'roles' already exists)
- **Acción**: INSERT directo en `__EFMigrationsHistory` de la base `SatHospitalarioIdentity`.
- **Razón**: La migración `20260325015815_RebuildIdentityPachonPro` no estaba registrada aunque el DDL ya existía en MySQL. EF Core intentaba ejecutarla de nuevo causando `MySqlException: Table 'roles' already exists`.
- **Comando**: `INSERT INTO __EFMigrationsHistory VALUES ('20260325015815_RebuildIdentityPachonPro', '9.0.2')`
- **Resultado**: Los 4 registros de migración están sincronizados. El `IdentityDbInitializer` ya no lanzará excepción al inicio.
- **Patrón**: Esta es la aplicación estricta de la **Ley #3** de `Rules.md` (Bypass de Migración).

## 📌 Lecciones del Día
- **Entity Consistency**: Siempre verificar las propiedades de la entidad en `Domain` antes de manipular Handlers en `Application`.
- **Distributed App Stability**: El AppHost de Aspire 13.1.3 es estable tras limpiar bloqueos de procesos `.exe` previos.
- **Migration Bypass Pattern**: Cuando MySQL tiene las tablas pero EF Core no tiene registro en `__EFMigrationsHistory`, el fix es un INSERT directo. No se regenera la migración. Ver `Rules.md Ley #3`.
