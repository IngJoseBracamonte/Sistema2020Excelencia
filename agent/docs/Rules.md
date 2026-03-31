# ⚖️ Leyes y Estándares de Ingeniería (Rules.md)

Este documento define las directrices inquebrantables de desarrollo para garantizar la consistencia, calidad y mantenibilidad del sistema.

## 🏛️ Leyes del Proyecto (Unbreakable Laws) - V11.10
1. **Legacy Build Integrity**: El sistema original de WinForms/C# NO SE MODIFICA en su lógica de negocio. Sin embargo, los archivos de proyecto (`.csproj`) deben mantenerse sincronizados con rutas relativas y frameworks soportados (v4.8) para garantizar el build de la solución.
2. **EF Core 9 Control**: Mantener `Microsoft.EntityFrameworkCore` en la versión `9.0.2`. El uso de v10 está prohibido por incompatibilidades con el proveedor Pomelo MySQL.
4. **Identidad por GUID**: Todos los nuevos registros de pacientes y entidades transaccionales DEBEN usar `Guid` como clave primaria. El uso de `int` para PKs queda prohibido para nuevas entidades.
5. **Admission Atomicity (MD-001)**: Cada finalización del Wizard de Facturación genera un **NUEVO** objeto `CuentaServicios`. Prohibido reutilizar cuentas abiertas previas para garantizar unicidad clínica.
6. **Conditional Closing (MD-002)**: El estado `Facturada` (Cerrada) es exclusivo para balances en cero. Pagos parciales mantienen la cuenta `Abierta` y disparan la creación de `CuentaPorCobrar`.
7. **Legacy Concatenation (MD-003)**: El sistema debe permitir el "Onboarding JIT" (Just-In-Time). Si un paciente existe en Legacy pero no en Native durante un Sync, se crea el stub nativo automáticamente.
8. **Tipado Estricto de Identidad**: Todos los `Commands` (IRequest) y Handlers DEBEN recibir `Guid` para `PacienteId`.
9. **Mapeo Seguro en DbContext**: Propiedades calculadas en el dominio (sin setters) DEBEN ignorarse explícitamente en el `DbContext` con `.Ignore()`.
10. **Fiscal Nullability (MD-004)**: El campo `NroControlFiscal` debe ser nullable (`string?`) para permitir estados de presolicitud/borrador sin número de serie fiscal.
11. **No-Tracking for Master Data**: Al consultar entidades maestras (Médicos, Cajas) solo para obtener IDs de referencia en comandos, usar `.AsNoTracking()` para evitar inserciones duplicadas por tracking de navegación.
12. **Snapshot Pinning (Legacy Migrations)**: Para añadir modificaciones de esquema a DBs heredadas (Legacy) trackeadas en EF Core, NUNCA ejecutar migraciones que recreen o toquen tablas existentes. Se debe generar la primera migración `InitialCreate`, vaciar sus métodos `Up()` y `Down()`, y luego aplicar la migración deseada con el delta exacto (ej. `ALTER TABLE`).
13. **Concurrency DB Locking**: Para números secuenciales atómicos en sistemas Legacy concurrentes (ej. `NumeroDia`), NO usar LINQ ni Entity Framework. Es mandatorio inyectar un query de bajo nivel con Dapper usando candado transaccional SQL: `SELECT COUNT(...) FOR UPDATE`.
14. **Multi-Currency AR Settlement**: Las liquidaciones de cuentas por cobrar (Receivables) DEBEN soportar conversión automática. TODO monto de base en la base de datos se guarda en USD ($). El payload enviado al backend debe contener el equivalente en USD como monto principal.
15. **Monetary Precision (Bs. 2-Decimal)**: Todos los cálculos financieros que resulten en Bolívares (Bs.) DEBEN redondearse a exactamente 2 decimales. En conversiones (ej. de Bs a USD), se debe mantener la precisión máxima en USD pero el monto origen en Bs debe ser estrictamente de 2 decimales.
16. **Master Currency (USD Balance)**: El sistema opera bajo una arquitectura **USD-First**. El saldo de las Cuentas por Cobrar (AR) y Abonos se gestiona y persiste principalmente en USD ($). El equivalente en Bs. es una capa de visualización/proyección volátil basada en la tasa de cambio activa.
17. **JWT Interceptor Restriction (SEC-001)**: El `AuthInterceptor` en Angular DEBE validar que la URL de destino comience con `environment.apiUrl` antes de adjuntar el token. Prohibido el envío automático de tokens a dominios externos.
18. **Generic Error Masking (SEC-002)**: Las respuestas de error (Catch blocks) en Controladores y el `GlobalExceptionHandler` NUNCA deben devolver `ex.Message` o `StackTrace` al cliente. Se deben usar mensajes genéricos y IDs de ticket para depuración interna.
19. **CORS Explicit Origins (SEC-003)**: Prohibido el uso de `SetIsOriginAllowed(origin => true)` en producción. Se deben configurar orígenes explícitos mediante variables de entorno o `appsettings`.
20. **JWT Transport Security (SEC-004)**: La configuración de `JwtBearer` debe tener siempre `RequireHttpsMetadata = true`.
21. **HSTS Enforcement (SEC-005)**: Todas las APIs en producción DEBEN activar el middleware `UseHsts()` para forzar exclusivamente el transporte mediante TLS.
22. **Rate Limiting (SEC-006)**: Los endpoints de escritura o autenticación DEBEN estar protegidos por políticas de `RateLimiting`. Umbral estándar: 100 req/min por IP.
23. **Financial Idempotency (SEC-007)**: Los comandos que modifiquen cuentas o generen cobros DEBEN aplicar el atributo `[Idempotent]`, exigiendo la cabecera `X-Idempotency-Key` del cliente para evitar cargos duplicados.
24. **PII Scrubbing (SEC-008)**: Prohibido persistir información de identificación personal (Cédula, Nombres) en mensajes de excepción o logs. Se DEBE utilizar el método `ScrubPii` antes de generar un ticket de error.
25. **Database Encryption (SEC-009)**: Todas las conexiones a bases de datos en producción DEBEN usar TLS/SSL (`SslMode=VerifyFull` o `Required`).

## 💻 Patrones de Código (Engineering Patterns)
### Backend (WebAPI / Core)
- **CQRS con MediatR**: No se inyectan servicios de aplicación en los controladores. Se usan estrictamente `Commands` y `Queries`.
- **Dual-Write Pattern**: La creación de pacientes debe impactar simultáneamente en `SistemaSatHospitalario` (Native) y `Sistema2020` (Legacy MySQL).
- **Result Pattern**: Los comandos deben devolver un objeto de resultado que maneje el éxito/error.

### Frontend (Angular 19)
- **Signals First**: Priorizar el uso de `Signal`, `computed` y `effect` sobre `BehaviorSubject` para el manejo de estado local.
- **Smart/Dumb Pattern**: Los componentes de características (Features) deben dividirse en un orquestador (Smart) y componentes de presentación (Dumb) que se comunican mediante inputs/outputs.
- **Facade Pattern**: Todo estado compartido entre componentes modulares debe gestionarse a través de un servicio Facade único que exponga Signals.
- **Global Icon Provider**: No usar `.pick()` en componentes individuales. Configurar `provideLucideIcons` o `importProvidersFrom` en la configuración global de la app.
- **Standalone Components**: No se permiten `NgModules`. Cada componente, directiva o pipe debe ser standalone.
- **Strong Typing**: Todos los servicios de API deben devolver interfaces TypeScript que coincidan exactamente con la estructura del Backend.
- **Telemetry Integration**: Usar siempre el `TelemetryService` para trazas y métricas OTel.

## 🎨 Estándar UI/UX ("Rose on Slate" Premium)
### 1. Sistema de Color (HSL)
| Token | Uso | HSL Value | Hex (Approx) |
| :--- | :--- | :--- | :--- |
| `--primary` | Identidad / Acciones / Rose | `343 85% 55%` | `#f43f5e` |
| `--surface` | Fondo Base (Slate 950) | `222 47% 4%` | `#020617` |
| `--surface-card` | Contenedores (Slate 900) | `222 47% 8%` | `#0f172a` |
| `--text-main` | Títulos / Datos Críticos | `210 40% 98%` | `#f8fafc` |

### 2. Especificaciones de Diseño
- **Glassmorphism**: 
  - `backdrop-filter: blur(20px)`
  - Border: `1px solid rgba(255, 255, 255, 0.08)`
  - Shadow: `0 8px 32px 0 rgba(0, 0, 0, 0.37)`
- **Transiciones**:
  - `transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1)`
- **Micro-interacciones**:
  - Hovers con `scale(1.02)` y `box-shadow` pulsante con opacidad primaria del 15%.

## 🔡 Naming & Git
- **Git Strategy**: Conventional Commits en español.
- **Naming**: 
  - Backend: `IRequest<T>`, `IRequestHandler<T, R>`.
  - Frontend: Componentes con sufijo `.component.ts`, Servicios con `.service.ts`.
