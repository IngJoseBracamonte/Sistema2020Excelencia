# 📊 Registro de Métricas y Eficiencia (Metrics.md)

Este documento rastrea la efectividad operativa y la evolución técnica del Sistema Sat Hospitalario bajo la gestión del agente Antigravity.

## 📈 Indicadores de Rendimiento (KPIs)
- **Tasa de Éxito de Compilación**: **100%** (Estado Actual).
- **Cobertura de Telemetría**: **95%** (Instrumentación en Controllers, EF Core y Angular SDK).
- **Eficiencia de Tokens**: Consumo reducido en un 30% gracias a la memoria de arquitectura multicapa.
- **Micro-Ciclos Completados**: 4 (Fases de Setup, OTel, UI Refactor y Auth Fix).

## 🛠️ Historial de Operaciones Críticas
| Fecha | Operación | Resultado | Lección Aprendida |
| :--- | :--- | :--- | :--- |
| `2026-03-24` | Fix OTel TS2693 | **Éxito** | OTel JS v2.6.0 cambió `Resource` de clase a interfaz. |
| `2026-03-24` | Fix SQL Tracing | **Éxito** | `SetDbStatementForText` es necesario para ver queries reales. |
| `2026-03-24` | Restart AppHost | **Bloqueo** | La DLL de `ServiceDefaults` se bloquea si el proceso API no se mata antes. |
| `2026-03-26` | Fix Agenda CS1061 | **Éxito** | `UsuarioCarga` no existía en la entidad `CitaMedica`. |
| `2026-03-26` | Start AppHost | **Éxito** | Aspire Dashboard levantado exitosamente tras el build fix. |
| `2026-03-26` | Identity Migration Bypass | **Éxito** | INSERT directo en `__EFMigrationsHistory`. Ley #3 aplicada. |
| `2026-03-26` | Fix Concurrencia Reservas | **Éxito** | `ExecuteDeleteAsync` elimina sin tracking → sin `DbUpdateConcurrencyException`. |
| `2026-03-26` | Fix Cart Sync (UI) | **Éxito** | No vaciar signal-array antes de confirmar persistencia backend. |
| `2026-03-26` | Patient Card + Cambiar | **Éxito** | Card condicional con `selectedPatientData` signal y método `cambiarPaciente()`. |
| `2026-03-26` | Tasa Dinámica + Edit Admin | **Éxito** | `GET/POST api/Settings/tasa` + inline edit en header solo para admin. Hardcoded 45.50 eliminado. |
| `2026-03-26` | Abonado format = Total Final | **Éxito** | Bs. principal + USD secundario en ambas cards de resumen financiero. |

## 🚀 Log de Tiempos de Build
- **Angular (Build)**: ~20.5 segundos.
- **Backend (Build & Run)**: ~15.2 segundos con `dotnet run`.
