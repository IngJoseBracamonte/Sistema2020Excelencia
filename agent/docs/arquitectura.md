# Memoria de Arquitectura - Sistema Sat Hospitalario 🏥 🧠

Este documento es el mapa maestro para el agente. Contiene las decisiones técnicas críticas, rutas de archivos y configuraciones que no deben ser ignoradas.

## 🏗️ Estructura General
- **Backend**: ASP.NET Core 9 (WebAPI) con EF Core 9.0.2.
- **Frontend**: Angular 19 (SPA) con orquestación Aspire.
- **Base de Datos**: MySQL (Pomelo) con soporte Multi-DB configurado por estrategia.

## 📍 Puntos de Entrada (Entry Points)
1. **Orquestador**: `src/SistemaSatHospitalario.AppHost/AppHost.cs`
2. **WebAPI**: `src/SistemaSatHospitalario.WebAPI/Program.cs`
3. **DI Infraestructura**: `src/SistemaSatHospitalario.Infrastructure/DependencyInjection.cs`

## 🐳 Zona de Datos (MySQL / Identity)
- **Problemática**: MySQL no soporta esquemas (Schema). Se usaba "Identity" y "System" en SQL Server.
- **Solución Actual**: 
  - Se configuró `b.SchemaBehavior(MySqlSchemaBehavior.Ignore)` en `DependencyInjection.cs`.
  - Se inyectó un **Bypass de Historial** para la migración `20260309004403_InitialIdentityMigration`.
  - Las tablas de Identity (`Roles`, `Usuarios`) fueron creadas manualmente con sintaxis compatible (`char(36)`).
- **Ruta Crítica**: `src/SistemaSatHospitalario.Infrastructure/Identity/Seeds/IdentityDbInitializer.cs`

## 🌐 Zona UI (Frontend)
- **Comando de Inicio**: Se utiliza `npm run start` en lugar de llamada directa al binario `ng.js` para evitar errores de vinculación en Windows.
- **Puntos de Control**: 
  - `serviciosFiltradosPorRol`: Se implementó un guardián de nulidad (`?.toLowerCase()`) para evitar errores de renderizado.
  - **Wizard Flow**: Estandarizado en: `1. Servicios` -> `2. Convenio` -> `3. Paciente/Pago`.
- **Ruta Crítica**: `src/SistemaSatHospitalario.Frontend/src/app/features/admision/facturacion/facturacion.component.ts`

## ⚠️ Decisiones Técnicas Críticas
- **Downgrade EF Core**: Se bajó de v10 a v9.0.2 para mantener compatibilidad con `Pomelo.EntityFrameworkCore.MySql`.
- **Bypass de Migración**: No intentar regenerar la migración inicial de Identity sin borrar primero el registro en `__EFMigrationsHistory`, de lo contrario, EF intentará crear esquemas.
- **SMTP Check**: `EmailService.cs` tiene una guarda que evita envíos si no hay configuración en `appsettings.json`.

## 📂 Paths Maestros (Top 5)
1. [AppHost.cs](file:///c:/Users/J_Bra/OneDrive/src/Sistema2020Excelencia/src/SistemaSatHospitalario.AppHost/AppHost.cs)
2. [DependencyInjection.cs](file:///c:/Users/J_Bra/OneDrive/src/Sistema2020Excelencia/src/SistemaSatHospitalario.Infrastructure/DependencyInjection.cs)
3. [styles.css](file:///c:/Users/J_Bra/OneDrive/src/Sistema2020Excelencia/src/SistemaSatHospitalario.Frontend/src/styles.css) (Design Tokens)
4. [UserLayoutComponent.ts](file:///c:/Users/J_Bra/OneDrive/src/Sistema2020Excelencia/src/SistemaSatHospitalario.Frontend/src/app/shared/layouts/user-layout/user-layout.component.ts)
5. [FacturacionComponent.html](file:///c:/Users/J_Bra/OneDrive/src/Sistema2020Excelencia/src/SistemaSatHospitalario.Frontend/src/app/features/admision/facturacion/facturacion.component.html)

---

## 🎨 Sistema de Diseño (UI/UX) - "Rose on Slate" Premium

Se ha implementado una identidad visual de siguiente generación enfocada en la reducción de fatiga visual y estética de alto valor.

### 1. Paleta de Colores (HSL System)
| Token | Uso | Valor HSL | Color |
| :--- | :--- | :--- | :--- |
| `--primary` | Identidad / Acciones / Rose | `343 85% 55%` | `#f43f5e` |
| `--primary-glow` | Resplandor / Hover | `343 85% 55% / 0.15` | |
| `--surface` | Fondo Principal (Slate 950) | `222 47% 4%` | `#020617` |
| `--surface-card` | Contenedores (Slate 900) | `222 47% 8%` | `#0f172a` |
| `--surface-light` | Elevación (Slate 800) | `222 47% 15%` | `#1e293b` |
| `--text-main` | Títulos / Contenido Crítico | `210 40% 98%` | `#f8fafc` |
| `--text-muted` | Secundaria (60% opacidad) | `215 20% 65%` | `#94a3b8` |

### 2. Estándares Técnicos
- **Tipografía**: `Outfit` para encabezados y `Inter` para datos tabulares.
- **Glassmorphism**: 
  - `backdrop-filter: blur(20px)`
  - Border: `1px solid rgba(255, 255, 255, 0.08)`
- **Jerarquía de Capas (Z-Index Visual)**:
  - Nivel 0: Fondo Base (`--surface`)
  - Nivel 1: Sidebar/Cards (`--surface-card`)
  - Nivel 2: Modals/Dropdowns (`--surface-light`)

### 3. Principios UX Aplicados
- **Escalabilidad**: Uso estricto de variables CSS para facilitar cambios de branding.
- **Feedback Sensorial**: Micro-animaciones de scale (1.02) y glows suaves en estados hover.
- **Foco de Acción**: El color Rose se reserva exclusivamente para elementos interactivos y avisos críticos.
