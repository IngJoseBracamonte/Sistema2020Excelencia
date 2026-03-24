# 🔄 Arquitectura de Flujo de Datos (DataFlow.md)

Este documento mapea la vida de la información a través del Sistema Sat Hospitalario, desde la interacción del usuario hasta la persistencia y observabilidad.

## 🏎️ Flujos de Lógica de Negocio (Workflows)
### 1. Inicio de Sesión (Auth Flow)
- **Input**: `LoginCommand` (Username, Password).
- **Proceso**: 
  - `AuthController` capta el comando.
  - `Manual Activity` iniciada en `DiagnosticsConfig.ActivitySource`.
  - `MediatR` despacha el comando al `Handler` correspondiente.
  - `IdentityDbContext` valida contra MySQL `mysql-identity`.
- **Output**: `JwtAuthResult` con token y expiración.
- **Side Effect**: Incrementar `auth.login_attempts` en el `Meter`.

### 2. Proceso de Admisión y Facturación (Wizard Flow)
Este flujo es secuencial y debe respetarse para garantizar la integridad de los datos:
1. **Paso 1: Selección de Servicios**:
   - Carga de catálogo filtrado por rol y disponibilidad.
   - Guardián de nulidad en `serviciosFiltradosPorRol`.
2. **Paso 2: Selección de Convenio**:
   - Selección del convenio (Asterisco, Descuento, etc.).
   - Mapeo de `Convenios` en la capa de persistencia `mysql-system`.
3. **Paso 3: Identificación Paciente / Pago**:
   - Búsqueda de paciente existente o creación (Solo Admin).
   - Generación de factura y recibo.
   - Cálculo automático de tasas y USD en `FacturaConvenioAsterisco`.

## 📡 Arquitectura de Telemetría (Observability Flow)
El sistema utiliza un pipeline de OpenTelemetry distribuido:

1. **Recolección en Origen**:
   - **Frontend**: `telemetry.service.ts` usa OTel JS SDK para capturar clics, navegación y fallos.
   - **Backend**: `Extensions.cs` instrumenta ASP.NET Core, EF Core y HttpClient.
2. **Exportación**:
   - Ambas capas envían datos vía **OTLP HTTP/gRPC** al endpoint orquestado.
   - Endpoint: `http://localhost:18889` (Aspire Collector).
3. **Persistencia y Visualización**:
   - El **Aspire Dashboard** recibe y procesa las trazas, métricas y logs.
   - Visualización en tiempo real en `https://localhost:17196`.

## 🏗️ Mapeo de Capas (Macro-Cycle)
- **UI (Angular)** -> **API (Controllers)** -> **Application (MediatR)** -> **Domain (Entidades)** -> **Infrastructure (EF Core)** -> **Datos (MySQL)**.
- **Relación Crítica**: Los `Commands` deben ser inmutables y portar solo la información necesaria para el cambio de estado.
