# Arquitectura: Módulo de Quirófanos, Entidad CompromisoPago y Atajo Alt+K

**Fecha**: 2026-07-29  
**Sistema**: Sistema Sat Hospitalario v2.0  
**Patrones aplicados**: Clean Architecture, CQRS, Signals (Angular 19+), Standalone Components, Direct Connect DB Tests.

---

## 1. Atajo Global de Búsqueda (`Alt + K`)
- **Directiva**: `SearchFocusDirective` (`src/SistemaSatHospitalario.Frontend/src/app/shared/directives/search-focus.directive.ts`)
- **Propósito**: Resolver de forma universal la interceptación del modificador `Ctrl + K` por Google Chrome.
- **Funcionamiento**: Escucha eventos `window:keydown`, valida si la combinación es `Alt + K` o `Ctrl + Shift + K`, invoca `event.preventDefault()` y `event.stopPropagation()`, y asigna el foco + selección al `<input>` decorado.

---

## 2. Refactorización del Módulo de Quirófanos (`PabellonGestionComponent`)
- **Componentes**: `PabellonGestionComponent` y `GestionConsumoModalComponent`.
- **Tipo de Médicos**: Eliminación de `any[]` a favor de la interfaz explícita `MedicoResumen`.
- **Signals**: Reactividad con `filtroTexto` y `filtroEstado` mediante `signal<string>('')`.
- **Invariante de Dominio (Guard)**: El modal de insumos quirúrgicos restringe la carga de cargos extras y devoluciones si la cirugía no se encuentra en estado `EnProceso`.
- **Auditoría**: Se reemplazó el `prompt()` nativo por un modal Tailwind dedicado para auditar la razón justificada de cancelación.

---

## 3. Entidad de Dominio `CompromisoPago`
- **Ubicación**: `SistemaSatHospitalario.Core.Domain/Entities/Admision/CompromisoPago.cs`
- **Atributos**: `Id`, `CuentaPorCobrarId`, `Omitido`, `Observacion`, `UsuarioCreacion`, `FechaCreacion`.
- **Encapsulamiento**: Método `Omitir(observacion)` que valida que la observación de omisión no sea nula ni vacía.
- **DbContext**: Registrado como `DbSet<CompromisoPago> CompromisosPago` en `SatHospitalarioDbContext`.

---

## 4. Pruebas E2E Directas a BD
- **Suite**: `AdmisionQuirofanoE2EDbDirectTests.cs` (100% Pasadas: 16/16).
- **Entidades Utilizadas**:
  - `CuentaServicios` y `DetalleServicioCuenta` para la cuenta del paciente.
  - `StockSede` para la visibilidad y actualización atómica del inventario en la Sede Principal.
  - `GarantiaItem` y `CompromisoPago` para garantías de admisión.
  - `OrdenCirugia`, `InsumoCirugiaPaciente` y `CirugiaLog` para el ciclo de quirófano.
  - `Sistema2020LegacyDbContext` + Dapper para el Monitor de Laboratorio (Legacy DB).
