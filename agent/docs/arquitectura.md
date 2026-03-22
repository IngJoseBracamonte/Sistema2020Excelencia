# Memoria de Arquitectura - SistemaSatHospitalario

## Estado Actual de la Memoria
El sistema es una solución híbrida de facturación y admisión altamente modular (.NET 9 + Angular 19). Se ha evolucionado de una unificación de datos a una plataforma **Multi-Rol con Seguridad Avanzada y Optimización de Rendimiento (Micro-Ciclos 22-29)**.

## Zonas de Interés

### 1. Núcleo de Admisión y Unificación (Micro-Ciclo 21)
Sincronización de identidad entre el sistema Nativo y el legado de Laboratorio (MySQL). 
- **ID Numérico Maestro**: PKs `int` sincronizadas (`IdPersona` / `IDConvenio`).
- **Legacy-First Provisioning**: El ID del legado es la "Fuente de la Verdad" para identidades persistentes.

### 2. Dashboards y Seguridad de Roles (Micro-Ciclos 22-27)
Estructura de visualización dinámica basada en CLAIMS de JWT.
- **Rutas y Menús**: Filtrado en `SidebarComponent` (Angular) mediante señales reactivas según el rol (`Administrador`, `Asistente Particular`, `Asistente Rx`).
- **Filtrado de Backend**: `GetBusinessInsightsQuery` inyecta el `UserRole` desde el controlador para devolver métricas específicas (e.g., RX Throughput vs. Ventas Consolidadas).
- **Ajustes de Sistema**: Centralización de parámetros globales en `SystemSettingsComponent` restringido a Admins.

### 3. Gestión de Cajas y Cierres Automatizados (Micro-Ciclo 28)
Flujo de tesorería "Continuous Flow" para mayor eficiencia operativa.
- **Auto-Apertura**: La entidad `CajaDiaria` se instancia automáticamente en el primer cobro del usuario si no hay una sesión activa.
- **Vinculación de Usuario**: Cada sesión de caja rastrea `UsuarioId` y `NombreUsuario` desde el token JWT.
- **Monitor Administrativo**: Consolidación de ingresos en tiempo real y arqueo por usuario disponible para el rol Administrador.

### 4. Capa de Optimización: Legacy Caching (Micro-Ciclo 29)
Protección de la base de datos MySQL Legacy mediante el patrón **Decorator**.
- **CachedLegacyLabRepository**: Envuelve al repositorio tradicional usando `IMemoryCache`.
- **Estrategia TTL**: 1h para Pacientes (Sliding), 4h para Convenios/Perfiles (Static-ish).
- **Invalidación**: Limpieza reactiva del caché al crear nuevos pacientes para evitar estados inconsistentes.

## Paths Críticos (Contexto Actual)
1. `src/SistemaSatHospitalario.Core.Domain/Entities/Admision/CajaDiaria.cs`: Seguimiento de sesiones por usuario.
2. `src/SistemaSatHospitalario.Core.Application/Queries/Admision/GetBusinessInsightsQueryHandler.cs`: Lógica de métricas multirrol.
3. `src/SistemaSatHospitalario.Infrastructure/Persistence/Legacy/CachedLegacyLabRepository.cs`: Capa de caching transparente.
4. `src/SistemaSatHospitalario.Infrastructure/DependencyInjection.cs`: Configuración del Decorator de Caching.
5. `src/SistemaSatHospitalario.WebAPI/Controllers/Admision/CajaController.cs`: Extracción dinámica de identidad para tesorería.

## Decisiones Técnicas Clave
- **Numeric ID Parity**: Eliminación de tablas de mapeo complejas para joins directos.
- **Role-Based Claim Filtering**: La seguridad y visibilidad se gestionan desde el token JWT, evitando flags en base de datos.
- **Decorator Pattern for Caching**: Permite optimizar el acceso a datos legacy sin contaminar la lógica de negocio central.
- **Automated Cashier Lifecycle**: Reduce errores humanos y fricción operativa al eliminar la apertura manual para los asistentes.

### 5. Independencia de Identidades (Seguridad de Credenciales)
Garantiza que la modernización no rompa el inicio de sesión en el sistema legacy.
- **Doble Persistencia**:
  - **Nativo (SQL Server)**: Usa `Microsoft.AspNetCore.Identity` con Hashing (PBKDF2/SHA256). Esquema `Identity`.
  - **Legacy (MySQL)**: Mantiene su tabla propia de usuarios con claves en texto plano (Plain Text). No es tocada por la App nueva.
- **Mapeo Virtual**: La entidad `UsuarioHospital` posee un campo `LegacyCajeroId` para vincular reportes, pero **NO** sincroniza contraseñas.
- **Aislamiento de Cambios**: Cualquier cambio de clave o creación en el sistema nuevo afecta solo a SQL Server, preservando la operatividad del legado MySQL.

### 6. Simplificación de Autenticación (Micro-Ciclo 30)
Transición de Email a Username como identificador principal de login.
- **Configuración de Identity**: `options.User.RequireUniqueEmail = false` para permitir flexibilidad total.
- **Mapeo de Claims**: `JwtRegisteredClaimNames.UniqueName` reemplaza al Email en el token JWT generado por `JwtAuthService`.
- **Compatibilidad Legacy**: Al usar nombres de usuario simples (e.g., "admin"), el sistema nuevo se alinea con la experiencia de usuario del legacy sin comprometer la seguridad del Hashing moderno.
