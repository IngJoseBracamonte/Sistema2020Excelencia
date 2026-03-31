# ⚙️ Catálogo de Parámetros Técnicos (Parameters.md)

Este archivo centraliza todas las constantes, configuraciones y variables críticas para la operación del Sistema Sat Hospitalario.

## 🔗 Endpoints y Orquestación (Aspire/Env)
| Variable | Descripción | Valor / Default |
| :--- | :--- | :--- |
| `OTEL_EXPORTER_OTLP_ENDPOINT` | Endpoint del Colector OTel | `http://localhost:18889` |
| `ASPNETCORE_URLS` | Puertos de la API | `https://localhost:17222`, `http://localhost:17088` (Ejemplo) |
| `ASPIRE_DASHBOARD_URL` | Acceso al Panel Central | `https://localhost:17196` |
| `FRONTEND_PORT` | Puerto de Angular 19 | `4200` |

## 🗄️ Diccionario de Bases de Datos (MySQL)
El sistema utiliza una estrategia Multi-DB sobre MySQL 8.0, orquestada por Aspire como recursos externos.

- **`mysql-system`**: Almacena el núcleo del sistema hospitalario (Médicos, Especialidades, Admisiones, Facturación).
- **`mysql-identity`**: Gestión de usuarios, roles y autenticación (ASP.NET Core Identity).
- **`LegacyConnection`**: Acceso a la base de datos `sistema2020` MySQL de WinForms.

### Parámetros Críticos de Comando (DTOs)
- **`IdPacienteLegacy`**: (string?) Identificador numérico del sistema anterior. Se usa para "Onboarding JIT" en `SyncCarritoCommand`.
- **`IdCuenta`**: (Guid) Identificador nativo de la admisión atómica.
- **`MontoPagado`**: (decimal) Base para el cálculo del balance y generación automática de deuda.

### Convenciones de Tablas
- **Prefijos**: `AspNet*` para tablas de Identity.
- **Sufijos**: No se usan.
- **Boilerplate**: Todas las entidades de sistema deben heredar de `BaseEntity` (ID Guid, CreateDate, ModifiedDate, IsDeleted).

## 🔐 Parámetros de Seguridad
- **JWT Issuer**: `https://localhost:17222/api/auth`
- **JWT Audience**: `SistemaSatHospitalarioClient`
- **Secret Injection (Parameter System)**: Los secretos (JWT, SMTP, DB) se inyectan mediante el sistema de parámetros de Aspire (`AddParameter`).
- **Dev Secrets**: Se recomienda `dotnet user-secrets` en el proyecto AppHost para desarrollo local.
- **Prod Secrets**: Deben proveerse vía `ASPNETCORE_` environment variables o un Secret Store externo. El orquestador no contiene valores por defecto.
- **AllowedOrigins**: Lista de orígenes permitidos (CORS) configurados en `Program.cs`. Default: `https://localhost:4200`.
- **Expiration**: 8 horas por defecto.
- **Email**: Puerto 587 (TLS/SSL) para `SmtpClient`.
- **Information Disclosure**: El mensaje de error es el principal vector de reconocimiento para un atacante. El enmascaramiento en el `GlobalExceptionHandler` es la primera línea de defensa.
- **CORS Broadness**: `AllowAnyOrigin` es incompatible con `AllowCredentials` en navegadores modernos y es un riesgo de seguridad mayor; siempre usar listas blancas de dominios.


## 🎨 Tokens de Diseño (HSL Full List)
- **Principal (Rose)**: `--primary: 343 85% 55%`
- **Fondo (Slate 950)**: `--surface: 222 47% 4%`
- **Contenedor (Slate 900)**: `--surface-card: 222 47% 8%`
- **Elevado (Slate 800)**: `--surface-light: 222 47% 15%`
- **Borde Primario**: `rgba(244, 63, 94, 0.2)`
