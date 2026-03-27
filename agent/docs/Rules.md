# ⚖️ Leyes y Estándares de Ingeniería (Rules.md)

Este documento define las directrices inquebrantables de desarrollo para garantizar la consistencia, calidad y mantenibilidad del sistema.

## 🏛️ Leyes del Proyecto (Unbreakable Laws)
1. **Legacy Intocable**: El sistema original de WinForms/C# NO SE MODIFICA. Cualquier discrepancia o error en el Legacy se reporta pero no se intenta solventar en este repositorio.
2. **EF Core 9 Control**: Mantener `Microsoft.EntityFrameworkCore` en la versión `9.0.2`. El uso de v10 está prohibido por incompatibilidades con el proveedor Pomelo MySQL.
3. **Bypass de Migración**: No regenerar la migración `20260309004403_InitialIdentityMigration`. Si falla la sincronización, limpiar manualmente `__EFMigrationsHistory` antes de reintentar.
45. **Identidad por GUID**: Todos los nuevos registros de pacientes y entidades transaccionales DEBEN usar `Guid` como clave primaria. El uso de `int` para PKs queda prohibido para nuevas entidades.
6. **Legacy Stubbing (Stage 1 Only)**: La creación de stubs locales basados en Legacy es responsabilidad EXCLUSIVA del flujo de Búsqueda/Sincronización (`PatientService`). Queda prohibida la creación automática de stubs en handlers de facturación o agenda para evitar duplicidad.
7. **Tipado Estricto de Identidad (V11.1)**: Todos los `Commands` (IRequest), `Queries` y sus respectivos Handlers DEBEN recibir `Guid` para `PacienteId`. El frontend debe enviar siempre el GUID nativo obtenido tras la identificación.
7. **Mapeo Seguro en DbContext**: Propiedades calculadas en el dominio (sin setters) DEBEN ignorarse explícitamente en el `DbContext` con `.Ignore()` para evitar fallos de inicialización de EF Core.
9. **Transición MySQL Destructiva**: Cambios de tipo `int` -> `Guid` en PK/FK requieren la eliminación manual de restricciones en la migración ANTES de alterar las columnas, debido a las restricciones de Pomelo MySQL.
10. **Checklist de Anti-Patrones (Obligatorio)**: Antes de finalizar cualquier tarea de identidad, revisar la sección '🚫 Anti-Patrones' en `architecture_memory.md`.

## 💻 Patrones de Código (Engineering Patterns)
### Backend (WebAPI / Core)
- **CQRS con MediatR**: No se inyectan servicios de aplicación en los controladores. Se usan estrictamente `Commands` y `Queries`.
- **Dependency Isolation**: La infraestructura no debe exponer sus modelos internos a la API; se deben usar DTOs o resultados de autenticación (`JwtAuthResult`).
- **Result Pattern**: Los comandos deben devolver un objeto de resultado que maneje el éxito/error, evitando el uso de excepciones para el flujo de control.

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
## 🔄 Flujo de Registro de Pacientes (V11.2)

1. **Entrada**: Cédula/Pasaporte.
2. **Local Lookup**: `_context.PacientesAdmision.FirstOrDefault(p => p.Cedula == input)`.
3. **Legacy Lookup (Si falla Local)**: `_legacyRepo.ObtenerPorCedula(input)`.
4. **Auto-Stubbing**:
   - Si se encuentra en Legacy: `new PacienteAdmision(data.Nombre, data.Cedula, ...) { IdPacienteLegacy = data.Id }`.
   - Si no se encuentra en ninguno: Registro manual (Nuevo GUID, sin IdLegacy).
5. **Persistencia**: Se guarda en local y se asocia el GUID a la `CuentaServicio` activa.
