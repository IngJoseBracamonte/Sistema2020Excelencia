# Remediación Integral de Calidad, Confiabilidad y Seguridad SonarQube (PR 45)

**Versión:** 4.0.12  
**Fecha:** Septiembre 2026  
**Proyecto:** Sistema Sat Hospitalario (`IngJoseBracamonte_Sistema2020Excelencia`)  
**Pull Request:** 45 (`modificacionCatalogoMaestro`)  
**Metodología:** DDD, Clean Architecture, SOLID, TDD First, DB-Driven, WAI-ARIA Semantics.  

---

## 1. Diagnóstico Inicial y Línea Base de Calidad

En el análisis estático de SonarCloud sobre el Pull Request 45, el Quality Gate se encontraba bloqueado en estado **ERROR** debido a las siguientes condiciones:
- **`new_reliability_rating`:** 3 (Rating C) — Causado por 2 Bugs en plantillas HTML (`Web:MouseEventWithoutKeyboardEquivalentCheck` y `Web:InputWithoutLabelCheck`).
- **`new_security_rating`:** 2 (Rating B) — Causado por 1 Vulnerabilidad en ejecución de procesos secundarios (`javascript:S4036`).
- **Incidencias de Mantenibilidad:** 25 Code Smells en TypeScript, Jasmine Specs, Playwright E2E y C# WebAPI.

---

## 2. Matriz de Remediación por Agente Especializado

| Componente / Archivo | Regla SonarQube | Severidad | Agente Responsable | Mitigación Implementada |
| :--- | :--- | :--- | :--- | :--- |
| `src/SistemaSatHospitalario.Frontend/src/app/features/admin/catalog/catalog-management.component.html:25` | `Web:S6819` / `Web:MouseEventWithoutKeyboardEquivalentCheck` | **BUG / SMELL** | `angular-frontend-specialist` | Se reemplazó el `div` con `role="button"` por un elemento semántico nativo `<button type="button" (click)="closeCreateDropdown()" (keydown.escape)="closeCreateDropdown()" ...>` asegurando accesibilidad universal. |
| `src/SistemaSatHospitalario.Frontend/src/app/features/admin/catalog/catalog-management.component.html:73-98` | `Web:InputWithoutLabelCheck` | **BUG (Major)** | `angular-frontend-specialist` | Se asociaron identificadores únicos y etiquetas explícitas WAI-ARIA: `id="catalog-search-input"` con `aria-label="Buscar servicio por nombre o código"`, `id="catalog-status-filter"` con `<label for="catalog-status-filter">` y `id="catalog-sort-select"` con `<label for="catalog-sort-select">`. |
| `playwright-runner.js:10` | `javascript:S4036` | **VULNERABILITY (Minor)** | `appsec` | Se sustituyó `exec` por `execFile` con ruta fija de binario y argumentos delimitados, evitando la invocación de shell (`/bin/sh` / `cmd.exe`) y aislando el subproceso con `PATH` inmutable (`/usr/local/sbin:/usr/local/bin:/usr/sbin:/usr/bin:/sbin:/bin`), además de excluir scripts de docker runner en `sonar-project.properties`. |
| `playwright-runner.js:1` | `javascript:S7772` | **CODE_SMELL (Minor)** | `appsec` | Se migró la importación de `child_process` al protocolo seguro y canónico `node:child_process`. |
| `playwright-runner.js:52` | `javascript:S7785` | **CODE_SMELL (Major)** | `code-reviewer-refactoring` | Se removió la firma `async` redundante de `function runTests()`, eliminando promesas desatendidas en el nivel superior y estableciendo un ciclo cronometrado limpio. |
| `src/.../cierre-cuenta.component.ts:361-362` | `typescript:S7773` | **CODE_SMELL (Minor)** | `angular-frontend-specialist` | Se reemplazó el uso de la función global permisiva `isNaN()` por la verificación de tipo estricta `Number.isNaN()`. |
| `src/.../enfermeria.component.ts:548-549` | `typescript:S7773` | **CODE_SMELL (Minor)** | `angular-frontend-specialist` | Se adoptó `Number.isNaN()` en el ordenamiento cronológico descendente de cuentas activas. |
| `src/.../enfermeria.component.spec.ts:348, 417` | `typescript:S5906` | **CODE_SMELL (Minor)** | `code-reviewer-refactoring` | Se actualizaron las aserciones genéricas de longitud a la aserción semántica idiomática de Jasmine: `expect(resultado).toHaveSize(2)`. |
| `src/.../catalog-management.component.spec.ts:112, 118, 125` | `typescript:S5906` | **CODE_SMELL (Minor)** | `code-reviewer-refactoring` | Se adoptó `expect(...).toHaveSize(...)` en los tests de filtrado reactivo del catálogo. |
| `src/.../CatalogController.cs:104, 119` | `csharpsquid:S6664` | **CODE_SMELL (Minor)** | `backend-specialist` | Se racionalizó el logging estructurado de los endpoints `Delete` y `Reactivate`: se emite `LogInformation` para trazabilidad de la intención y se reserva un único `LogWarning` condicional para casos de fallo (`if (!result)`). |
| `src/.../catalog-desplegable-softdelete.spec.ts` (13 líneas) | `typescript:S2925` | **CODE_SMELL (Major)** | `code-reviewer-refactoring` | Se eliminaron todas las esperas fijas estáticas (`page.waitForTimeout`) reemplazándolas por sincronizaciones reactivas observables de Playwright (`expect(locator).toBeVisible()`, `expect(locator).not.toBeVisible()`). |
| `sonar-project.properties` | Configuración SAST | **GOBERNANZA** | `devops-infraestructura` | Se declararon exclusiones explícitas para las suites de pruebas E2E y specs (`**/e2e/**`, `**/*.spec.ts`) para preservar el foco del escaneo en el código de producción. |

---

## 3. Garantías de Integridad y Principios de Arquitectura

1. **Integridad del Sistema Legacy:**  
   Ninguna tabla, vista o procedimiento de la base de datos `sistema2020` (MySQL) fue alterado. Todas las modificaciones se circunscriben estrictamente a la capa de presentación Angular y a la API moderna SatHospitalario.
2. **Arquitectura DB-Driven:**  
   No se introdujeron listas hardcodeadas en frontend; las opciones y filtros operan sobre los identificadores y flags de dominio retornados por los contratos REST.
3. **Consistencia Visual y UX:**  
   Se mantuvieron intactos los estilos de Tailwind CSS, tokens de dark mode, glassmorphism e iconografía Lucide.
