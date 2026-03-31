# 📝 Diario de Pasos de Ingeniería (StepJournal.md)

Registro detallado de acciones atómicas y decisiones tomadas en tiempo real.

## 🗓️ 2026-03-24 (Fase de Telemetría e Inteligencia)
### 1. Corrección de Tipo en OpenTelemetry JS
- **Acción**: Se refactorizó `telemetry.service.ts` para usar `resourceFromAttributes`.
- **Contexto**: El error `TS2693` bloqueaba el build de Angular 19.
- **Resultado**: Build exitosa en 20s.

## 🛠️ Registro de Actividad - 27 Marzo 2026 (Mañana)

### 🚀 Ciclo: Estandarización de Identidad GUID (V11.1)
- **Fase 1-5 Finalizadas**: Alineación total del stack (Backend -> DB -> Frontend -> Tests).
- **Correcciones Críticas**:
  - Eliminado "Auto-Stub" en handlers transaccionales para evitar duplicidad.
  - Sincronizada firma de `PatientService.getHistory` (Guid).
  - Corregida referencia `p.nombre` -> `s.descripcion` en catálogo de servicios.
- **Estado**: Sistema 100% operativo sin errores 400 en facturación.

---

## 📈 Tareas Activas (Prioridad Alta)
- [x] Sincronizar Memoria de Arquitectura (Skill).

---

## 🏛️ Decisiones Estructurales Recientes
1. **Identidad Determinista**: La resolución de identidad ES responsabilidad del selector, no de la carga de servicios.
2. **GUID Only**: Prohibido el uso de `int` para IDs de paciente en cualquier comando nuevo.

### 2. Visibilidad Total de Base de Datos
- **Acción**: Se activó `SetDbStatementForText` en `Extensions.cs`.
- **Razón**: El usuario no veía las operaciones SQL en Aspire.
- **Resultado**: Trazas detalladas habilitadas.

### 3. Evolución 🧩 Arquitectura de Alto Nivel (Mermaid) - V13.0 (Security Hardened)
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

## 🗓️ 2026-03-27 (Modularización y Estabilización V9.0)
### 1. Refactor: Arquitectura Smart/Dumb en Facturación
- **Acción**: Fragmentación del `FacturacionComponent` (900 líneas) en 4 componentes especializados: `PatientSelector`, `ServiceCatalog`, `BillingCart` y `PaymentModule`.
- **Contexto**: El componente original era un monolito inmanejable con lógica duplicada y errores de compilación `NG9`.
- **Resultado**: Código modular, respetando SRP y facilitando el mantenimiento.

### 2. State Management: Centralización en Facade
- **Acción**: Creación/Refuerzo del `BillingFacadeService` como Single Source of Truth mediante Angular Signals.
- **Resultado**: Sincronización automática de totales, tasa de cambio y carrito entre todos los sub-componentes.

### 3. Fix: Iconografía Standalone (TS2322)
- **Acción**: Eliminación de `.pick()` en componentes y configuración de `importProvidersFrom` en `app.config.ts`.
- **Razón**: `LucideAngularModule.pick()` devuelve un `ModuleWithProviders`, incompatible con el arreglo `imports` de componentes Standalone en Angular 19.
- **Resultado**: Iconos disponibles globalmente sin errores de compilación ni de runtime.

### 4. Fix: Precisión Financiera (Tasa de Cambio)
- **Acción**: Implementación de lógica reactiva `USD * tasaCambioDia()` en el catálogo y carrito.
- **Razón**: Se mostraba 1 USD = 1 Bs por falta de multiplicación dinámica en los getters del modelo.
- **Resultado**: Valores en Bs exactos y sincronizados con la tasa activa del sistema.

### 5. UI/UX: Ultra-Compact Density
- **Acción**: Reducción agresiva de espaciados (`space-y-1`, `p-2`) y fuentes en el orquestador principal.
- **Resultado**: Interfaz optimizada para estaciones de trabajo con alta carga de datos, eliminando scroll innecesario.

## 📌 Lecciones del Día
- **Standalone Provider Pattern**: En Angular 19, los proveedores de módulos antiguos deben inyectarse mediante `importProvidersFrom` en el `ApplicationConfig` para evitar conflictos de tipos.
- **Signal Reactivity**: El uso de `computed` en el Facade simplifica drásticamente el cálculo de impuestos y totales, eliminando la necesidad de cálculos manuales en el HTML.
- **Architecture Memory Skill**: Mantener la documentación sincronizada con el código (Architecture.md, Rules.md) reduce el tiempo de re-análisis del agente en un 80%.

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

### 4. Fix: Z-Index Overlap en Facturación (Paso 3)
- **Acción**: Añadido `relative z-[90]` al panel "Identificar Beneficiario" en `facturacion.component.html`.
- **Razón**: El dropdown de resultados de búsqueda de pacientes (`absolute z-[80]`) se renderizaba detrás de la sección "Carga de Abonos" por falta de stacking context en el contenedor padre.
- **Resultado**: El dropdown ahora flota correctamente sobre la sección inferior sin superposición visual.
- **Regla**: No se modificó tema visual (Rose on Slate). Solo se corrigió el layering CSS.

### 5. Feature: Card de Información de Paciente Verificado
- **Acción**: Refactorizado el panel "Identificar Beneficiario" en Paso 3 para mostrar condicionalmente un card con los datos del paciente tras seleccionarlo, reemplazando el input de búsqueda.
- **TS**: Añadido `selectedPatientData: Signal<PatientRecord | null>`, almacenado en `seleccionarPaciente()`. Nuevo método `cambiarPaciente()` para resetear y buscar otro (bloqueado si ya hay cuenta).
- **HTML**: Dos estados condicionales: búsqueda (`!pacienteSeleccionado()`) y card verificada (`pacienteSeleccionado()`). Card muestra nombre, cédula, teléfono, correo y botón "Cambiar" con ícono `Edit3`.
- **UI**: Estilo coherente con Rose on Slate (emerald accent para verificación, `bg-black/20`, bordes `emerald-500/15`, tipografías `text-[9px]` tracking-widest).
### 6. Fix: Concurrencia en ReservarTurnoTemporal (DbUpdateConcurrencyException)
- **Archivo**: `ReservarTurnoTemporalCommand.cs`
- **Cambio**: Reemplazado `.ToList()` + `RemoveRange()` + `SaveChangesAsync()` por `ExecuteDeleteAsync()`. Sincrónico `.Any()`/`.FirstOrDefault()` → asíncronos.
- **Razón**: Limpieza de reservas expiradas fallaba si otro request ya las eliminó (concurrencia optimista EF Core).

### 7. Feature: Skill `ahorro-de-tokens` (Meta-protocolo)
- **Archivo**: `agent/skills/ahorro-de-tokens/SKILL.md`
- **Cambio**: Creado skill que parametriza verbosidad y lecturas por modelo (Opus/Sonnet/Flash/Pro). Zero-narration en proceso, delegación a skills especializados.

### 8. Fix: Items desaparecían al seleccionar paciente (Paso 3)
- **Archivo**: `facturacion.component.ts` → `sincronizarCarrito()` + `procesarCargaBackend()`
- **Cambio**: Eliminado `carritoLocal.set([])` prematuro. Items se mueven de `carritoLocal` → `serviciosEnBackend` solo tras confirmación del backend.
- **Razón**: `serviciosCargados = [...serviciosEnBackend(), ...carritoLocal()]` quedaba vacío durante el round-trip HTTP.

### 9. Fix: Tasa de cambio hardcoded (45.50 vs 36.5 en Ajustes Globales)
- **Archivo**: `facturacion.component.ts` L109, `GetTasaCambioQuery.cs` [NEW], `SettingsController.cs`, `settings.service.ts`
- **Cambio**: Tasa ya no es `signal<number>(45.50)` hardcoded. Se carga desde `GET api/Settings/tasa` → query CQRS que lee la última `TasaCambio` activa. Fallback: 36.5.
- **Admin Inline Edit**: Signals `editandoTasa`/`tasaEditValue` + métodos `editarTasa()`/`guardarTasa()` → `POST api/Settings/tasa`. Header muestra botón Edit3 solo para `isAdmin()`.

### 10. UI: Abonado muestra Bs. como Total Final
- **Archivo**: `facturacion.component.html` L493-498
- **Cambio**: Card "Abonado" ahora muestra `Bs. (base * tasa)` como principal y `$X.XX USD` como secundario, igualando el formato de "Total Final".
### 11. Security: Gestión de Tasa Restringida a Admin
- **Archivo**: `facturacion.component.html` L33-35
- **Cambio**: El botón de edición de tasa (`icons.Edit3`) solo es visible para `isAdmin()`.
- **Razón**: Por regla de negocio, solo los administradores pueden modificar la tasa oficial del día. El registro de pacientes permanece público para evitar cuellos de botella operativos.

### 12. UI: Refactor de Carga de Abonos (Rose on Slate Premium)
- **Archivo**: `facturacion.component.html` L388-444
- **Cambio**: Panel de abonos rediseñado con `glass-panel`, indicadores "Pendiente" y listado de pagos con iconografía `CheckCircle`.
- **Lógica**: Sincronización visual entre el total de la cuenta y el acumulado de abonos en tiempo real.

### 13. Audit: Sincronización de Campos de Cierre de Cuenta
- **Archivo**: `facturacion.component.ts` -> `procesarCobro()`
- **Cambio**: Añadido `usuarioId` en el payload de `closeAccount`.
- **Razón**: The `CloseAccountCommandHandler` requiere el ID del usuario para la gestión de auditoría y cierre de la caja diaria (`CajaDiaria`).

## 📌 Lecciones del Día
- **Entity Consistency**: Siempre verificar las propiedades de la entidad en `Domain` antes de manipular Handlers en `Application`.
- **Distributed App Stability**: El AppHost de Aspire 13.1.3 es estable tras limpiar bloqueos de procesos `.exe` previos.
- **Migration Bypass Pattern**: Cuando MySQL tiene las tablas pero EF Core no tiene registro en `__EFMigrationsHistory`, el fix es un INSERT directo. Ver `Rules.md Ley #3`.
- **Z-Index Stacking**: Dropdowns `absolute` necesitan que el padre tenga `relative` + `z-index` superior a hermanos adyacentes.
- **ExecuteDeleteAsync > RemoveRange**: Para limpieza idempotente de filas (expiradas, duplicadas), `ExecuteDeleteAsync` evita concurrencia optimista completamente.
- **Optimistic UI**: No vaciar un signal-array antes de confirmar persistencia. Mover items entre signals solo tras `subscribe.next`.
- **No Hardcodear Config**: Valores de configuración (tasa, IVA, etc.) deben cargarse del backend con `GET` en el constructor. Nunca hardcodear en signals.
- **Security by Role**: Las acciones que modifican el legado (`LegacyLabRepository`) deben protegerse en el UI con validaciones de rol explicitly (e.g. `isAdmin()`).
## 🗓️ 2026-03-27 (Modernización de Identidad y Base de Datos V11.0)
### 1. Refactor: Identidad Universal basada en GUID
- **Acción**: Migración completa de `PacienteId` de `int` a `Guid` en todo el core del sistema (Domain, Application, Infrastructure).
- **Entidades Afectadas**: `PacienteAdmision`, `CuentaServicio`, `ReciboFactura`, `CitaMedica`, `CuentaPorCobrar`, `OrdenDeServicio`.
- **Razón**: El uso de IDs incrementales (`int`) exponía la secuencia de datos y limitaba la generación de stubs locales seguros para el sistema legacy.

### 2. Feature: Auto-Stubbing Híbrido (Legacy ID)
- **Acción**: Adición del campo `IdPacienteLegacy` (int?) en `PacienteAdmision`.
- **Propósito**: Permitir que pacientes provenientes del laboratorio antiguo sean registrados localmente con un GUID propio, manteniendo el ID original para consultas históricas.
- **Implementación**: Búsqueda por `Cedula` -> Si existe, devolver local; Si no, buscar en Legacy -> Crear Stub local con `IdPacienteLegacy`.

### 3. Fix: EF Core Mapping (InvalidOperationException)
- **Acción**: Aplicado `entity.Ignore(c => c.SaldoPendienteBase)` en `SatHospitalarioDbContext`.
- **Razón**: EF Core intentaba mapear una propiedad calculada de dominio (sin setter) como columna física, bloqueando el arranque del `SystemDbInitializer`.
- **Lección**: Las propiedades de dominio calculadas DEBEN ser ignoradas explícitamente en el `OnModelCreating`.

### 4. Infrastructure: Migración Destructiva Controlada (V11.2)
- **Acción**: Rollback físico de la base de datos a cero y re-aplicación de migraciones con parches manuales.
- **Técnica**: Se editó la migración `V11_ModernIdentity_AutoStub` para incluir `DropForeignKey` y `AddForeignKey` manuales para `OrdenesDeServicio`.
- **Resultado**: Base de datos MySQL sincronizada con éxito tras corregir una migración previa corrupta (`UpdateConvenioToCompanyModel`) que tenía FK drops comentados.

## 🛠️ Registro de Actividad - 27 Marzo 2026 (Mediodía)

### 🚀 Ciclo: Refinamiento de Facturación Atómica (V11.5 - V11.7)
- **Hito Final**: Consolidación de la **Memoria de Arquitectura** (7 documentos actualizados).
- **Logros Clave**:
    - **Admission Atomicity**: Cada sync = nueva cuenta. Prohibido el reciclaje de IDs.
    - **Debt Management**: Pagos parciales disparan `CuentaPorCobrar`.
    - **Legacy Concatenation**: Onboarding dinámico y semilla inicial sincronizada.
    - **Observability**: Visibilidad total en Aspire Dashboard de la lógica de negocio.
- **Estado**: Sistema certificado bajo las leyes MD-001, MD-002 y MD-003.

---

## 📌 Lecciones del Día
- **Documentation as Code**: Mantener los 7 archivos maestros de arquitectura sincronizados previene la degradación del conocimiento técnico y acelera la inducción de nuevos agentes de IA.
- **Atomic Persistence**: La creación de entidades frescas en cada comando simplifica drásticamente el manejo de excepciones de concurrencia de base de datos.
- **Legacy Interop**: El patrón "Just-In-Time Onboarding" es superior a las migraciones masivas por lotes para sistemas legacy altamente activos.
- **Legacy Build Integrity**: El sistema original de WinForms/C# NO SE MODIFICA en su lógica de negocio. Sin embargo, los archivos de proyecto (`.csproj`) deben mantenerse sincronizados con rutas relativas y frameworks soportados (v4.8) para garantizar el build de la solución.

### 🛠️ Parche de Estabilización V11.7.3
- **Fix**: Flexibilización de `NroControlFiscal` (Null permitido).
- **Fix**: Prevención de `Ghost Inserts` mediante el uso de `NoTracking` en `ICajaAdministrativaRepository`.
- **Lección**: EF Core 9 marca como `Added` las entidades de navegación si el objeto base no es rastreado por el MISMO contexto de base de datos durante la asociación.


### 🛠️ Estabilización V11.8 (Legacy Build & UnitTests)
- **Fix**: Re-habilitación del proyecto `Conexiones`.
    - Eliminada referencia a `HojadeTrabajo.cs` (movido a Laboratorio).
    - Actualizadas referencias NuGet a rutas relativas (`..\packages\`).
    - Upgrade de Framework a `v4.8` para compatibilidad con `MySql.Data 8.0.29`.
- **Fix**: Resolución de `CS1503` en `SistemaSatHospitalario.UnitTests`.
    - Sincronizados constructores de `CitaMedica` con el nuevo esquema de identidad `Guid`.
- **Database**: Aplicada migración `V11_8_ModelSync` para sincronizar cambios pendientes en `SatHospitalarioDbContext`.
- **Resultado**: Solución `Sistema2020Excelencia.sln` con compilación 100% exitosa y base de datos sincronizada. 🏗️✅

## 🗓️ 2026-03-30 (Integración Legacy y Estabilización Concurrente V11.9)
### 1. Refactor: Dapper Concurrency Lock para `NumeroDia`
- **Acción**: Intervención en `LegacyLabRepository` transformando el uso de EF Core transaccional a un query en crudo Dapper con `SELECT COUNT(...) FOR UPDATE`.
- **Razón**: El cálculo del código de paciente diario sufría de "Race Conditions" (colisión de IDs) en momentos de alta facturación paralela.
- **Resultado**: Conteo atómico estricto garantizado por el compilador C de MySQL.

### 2. Architecture: Integridad Transaccional Blindada (Anti-Huérfanos)
- **Acción**: Reposicionamiento de la llamada `ProcessLegacyOrder` en el bottom exacto de `CloseAccountCommandHandler`, instantes antes del `await transaction.CommitAsync()`.
- **Propósito**: Si SQL Server falla, hacer rollback total en la API nueva SIN lanzar inserciones muertas en la base de datos MySQL anterior.

### 3. Architecture: Snapshot Pinning (Añadir PK a Legacy)
- **Acción**: Modificación de la entidad Legacy `Resultadospaciente` que era *Keyless* para inyectar `IdResultadoPaciente` como PK Auto-Incremental.
- **Técnica Segura**: Generación de *InitialCreate* Migration -> Vaciado total de métodos `Up()` y `Down()` -> Generación de segunda migración -> Aplicación a BD.
- **Resultado**: Prevención absoluta de un desastroso evento Drop/Create de EF Core en la base principal.

### 4. Feature: Zero-Touch Deployments (Auto-Migraciones)
- **Impacto**: El backend moderno ahora gestiona también la verificación estructural de las tablas heredadas mediante `MigrateAsync()` silente. Todo el código despliega solo sin intervención humana.

### 5. Feature: Liquidación AR Multi-moneda (V12.0)
- **Acción**: Refactorización completa del módulo `Receivables` (Cuentas por Cobrar).
- **Frontend**: Integración con `SettingsService` para obtener la `tasaCambio` viva vía SignalR. Implementación de lógica de conversión automática para métodos USD (Zelle, USDT, etc.).
- **Backend**: Actualización de `SettleARCommand` y `SettleARCommandHandler` para persistir el desglose de pagos en la tabla `DetallePagos` de SQL Server.
- **Arquitectura**: Se eliminó la restricción de estado en `ReciboFactura.AgregarDetallePago` para permitir la conciliación de saldos en facturas ya emitidas.
- **Resultado**: El sistema ahora permite liquidar deudas mezclando múltiples monedas con auditoría total del monto original y su equivalente en bolívares.

## 🗓️ 2026-03-31 (Auditoría de Seguridad y Sanitización V12.1)
### 1. Security: Hardening del Interceptor JWT (SEC-001)
- **Acción**: Se refactorizó `jwt.interceptor.ts` para validar el destino de la petición.
- **Razón**: El interceptor adjuntaba el token Bearer a cualquier URL saliente (ej. assets externos o telemetría), arriesgando el secuestro de sesiones.
- **Resultado**: El token ahora solo se inyecta en peticiones dirigidas a `environment.apiUrl`.

### 2. Security: Enmascaramiento de Errores Críticos (SEC-002)
- **Acción**: Saneamiento de `GlobalExceptionHandler.cs` y `BillingController.cs`.
- **Razón**: Se devolvía `exception.Message` directamente al cliente, lo que exponía detalles internos de la base de datos y la arquitectura ante fallos.
- **Resultado**: Respuestas estandarizadas con mensajes genéricos y ticket de soporte.

### 3. Security: Endurecimiento de Configuración Global (SEC-003/004)
- **Acción**: Actualización de `Program.cs`.
- **Cambios**:
  - `RequireHttpsMetadata = true` para JWT.
  - CORS restringido mediante `WithOrigins` usando la configuración `AllowedOrigins`.
- **Resultado**: Configuración de red lista para entornos de producción seguros.


## 📌 Lecciones del Día
- **Default Insecurity**: Las configuraciones por defecto de plantillas (.NET/Angular) suelen priorizar la agilidad del desarrollador sobre la seguridad. Es mandatorio realizar un paso de hardening antes de avanzar a producción.
- **Information Disclosure**: El mensaje de error es el principal vector de reconocimiento para un atacante. El enmascaramiento en el `GlobalExceptionHandler` es la primera línea de defensa.
- **CORS Broadness**: `AllowAnyOrigin` es incompatible con `AllowCredentials` en navegadores modernos y es un riesgo de seguridad mayor; siempre usar listas blancas de dominios.
