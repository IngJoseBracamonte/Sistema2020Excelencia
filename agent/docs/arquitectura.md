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
- **Ruta Crítica**: `src/SistemaSatHospitalario.Frontend/package.json`

## ⚠️ Decisiones Técnicas Críticas
- **Downgrade EF Core**: Se bajó de v10 a v9.0.2 para mantener compatibilidad con `Pomelo.EntityFrameworkCore.MySql`.
- **Bypass de Migración**: No intentar regenerar la migración inicial de Identity sin borrar primero el registro en `__EFMigrationsHistory`, de lo contrario, EF intentará crear esquemas.
- **SMTP Check**: `EmailService.cs` tiene una guarda que evita envíos si no hay configuración en `appsettings.json`.

## 📂 Paths Maestros (Top 5)
1. [AppHost.cs](file:///c:/Users/J_Bra/OneDrive/src/Sistema2020Excelencia/src/SistemaSatHospitalario.AppHost/AppHost.cs)
2. [DependencyInjection.cs](file:///c:/Users/J_Bra/OneDrive/src/Sistema2020Excelencia/src/SistemaSatHospitalario.Infrastructure/DependencyInjection.cs)
3. [IdentityDbInitializer.cs](file:///c:/Users/J_Bra/OneDrive/src/Sistema2020Excelencia/src/SistemaSatHospitalario.Infrastructure/Identity/Seeds/IdentityDbInitializer.cs)
4. [SatHospitalarioIdentityDbContext.cs](file:///c:/Users/J_Bra/OneDrive/src/Sistema2020Excelencia/src/SistemaSatHospitalario.Infrastructure/Identity/Contexts/SatHospitalarioIdentityDbContext.cs)
5. [Program.cs (WebAPI)](file:///c:/Users/J_Bra/OneDrive/src/Sistema2020Excelencia/src/SistemaSatHospitalario.WebAPI/Program.cs)
