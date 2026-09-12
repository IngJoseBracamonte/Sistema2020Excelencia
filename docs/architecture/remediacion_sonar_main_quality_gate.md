# Memoria Técnica de Arquitectura: Remediación Quality Gate en SonarCloud / SonarQube

**Fecha:** 12 de Septiembre de 2026  
**Rama:** `fix/sonar-main-quality-gate`  
**Objetivo:** Cumplimiento total del Quality Gate en SonarCloud para la rama `main`, subsanando Bugs, Vulnerabilidades, Code Smells y normalizando la densidad de duplicación de código (9.6% a < 3.0%).

---

## 1. Executive Summary

El escaneo de calidad estática de SonarCloud sobre la rama `main` presentaba una condición no superada en el Quality Gate derivada principalmente de una densidad artificial de código duplicado (9.6% frente al umbral máximo permitido de 3.0%), vulnerabilidades potenciales de Denegación de Servicio por Expresiones Regulares (ReDoS - `csharpsquid:S6444`), sobre-inclusiones y proyecciones redundantes en Entity Framework Core (`csharpsquid:S9023` y `csharpsquid:S9022`), y omisiones de accesibilidad WCAG en plantillas HTML (`Web:InputWithoutLabelCheck` y `Web:MouseEventWithoutKeyboardEquivalentCheck`). 

La intervención arquitectónica implementó un plan estricto en Clean Architecture y TDD, logrando 0 advertencias y 0 errores en compilación .NET 9 Release, 0 errores en Angular Production Build, y 62/62 pruebas unitarias exitosas.

---

## 2. Diagnóstico y Remediaciones por Capa

### A. Exclusión CPD de Migraciones Autogeneradas (Densidad de Duplicación < 3.0%)
- **Problema:** Las migraciones de Entity Framework Core (`src/SistemaSatHospitalario.Infrastructure/Migrations/20260911160227_NombreDeLaMigracion.cs`) contienen miles de líneas autogeneradas por el CLI con estructuras tabulares repetidas, elevando falsamente la duplicación a 9.6%.
- **Solución:** En `sonar-project.properties`, se añadieron las exclusiones del Copy/Paste Detector (CPD) y del análisis de cobertura:
  ```properties
  sonar.exclusions=**/SistemaLegacy/**,**/Conexiones/**,**/Laboratorio/**,**/Anviz/**,**/OA99/**,**/Conexion.cs,**/e2e/**,**/*.spec.ts,playwright-runner.js,**/Migrations/**
  sonar.coverage.exclusions=**/*.spec.ts,**/e2e/**,playwright-runner.js,**/Migrations/**
  sonar.cpd.exclusions=**/Migrations/**,**/*.spec.ts,**/e2e/**,playwright-runner.js
  ```

### B. Mitigación de Vulnerabilidades ReDoS (`csharpsquid:S6444`)
- **Problema:** En `GetBusinessInsightsQuery.cs` y `SystemDbInitializer.cs`, se realizaban parsing de bitácoras de auditoría de traslados con `Regex.Match` y `Regex.Replace` sin timeout explícito, exponiendo el servicio a bloqueos de CPU ante payloads patológicos.
- **Solución:** Se aplicó timeout de 250 milisegundos (`TimeSpan.FromMilliseconds(250)`) y `RegexOptions.None` en ambas capas (Aplicación e Infraestructura):
  ```csharp
  var match = Regex.Match(text, @"Cama:\s*([0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12})", RegexOptions.None, TimeSpan.FromMilliseconds(250));
  text = Regex.Replace(text, @"AreaDestino:[^,]+,", $"AreaDestino: {sedeNom},", RegexOptions.None, TimeSpan.FromMilliseconds(250));
  ```

### C. Optimización de Consultas EF Core (`csharpsquid:S9023` y `csharpsquid:S9022`)
- **Problema:** En `InventoryController.cs`:
  - L352: `Include(i => i.StocksPorSede)` era redundante y provocaba advertencias/bugs severos de EF Core porque la consulta posteriormente proyectaba explícitamente mediante `.Select(i => new { ... })`.
  - L203 y L226: `Include(s => s.Insumo)` innecesarios en consultas con proyecciones y agrupaciones.
- **Solución:** Se retiraron las cláusulas `.Include(...)` redundantes, delegando la selección de columnas estrictamente a la proyección LINQ SQL optimizada.

### D. Accesibilidad y Estándares WCAG en Frontend Angular
- **Problema:** Formularios y modales en Enfermería, Admisión e Inventario carecían de identificadores o equivalentes de teclado:
  1. `enfermeria.component.html:36`: `(click)` sin handler de teclado `(keydown)`.
  2. `enfermeria.component.html:466`: Selector de búsqueda sin `id` ni `aria-label`.
  3. `cierre-cuenta.component.html:1260`: `<span>` sin asociar a `<select>` mediante `<label for="...">`.
  4. `envios-recepciones.component.html`: Múltiples `<select>`, `<input>` y `<textarea>` sin etiquetas semánticas `<label for>` / `id` / `aria-label`.
  5. `traslados-destino.component.html`: Controles huérfanos sin enlace semántico de accesibilidad.
- **Solución:** Se vincularon todos los controles con pares estrictos `<label for="[id]">` e `id="[id]"` y atributos `aria-label` descriptivos en toda la suite de componentes afectados.

---

## 3. Cobertura y Validación TDD

Se añadió una nueva prueba unitaria determinística en `QueryResilienceNullSafetyTests.cs`:
- `GetBusinessInsights_ShouldProcessAuditLogsWithRegexTimeouts_WithoutThrowing`: Valida la resiliencia del parser de auditoría con timeouts de expresiones regulares, mapeo de camas y sedes en 3FN y ausencia de regresiones.

### Métricas de Validación:
- **Pruebas Unitarias .NET 9:** 62 pasadas / 0 fallidas / 0 omitidas.
- **Compilación .NET 9 WebAPI (Release):** 0 Errores, 0 Advertencias.
- **Compilación Angular Production Build (`npm run build`):** Exitosa en 24.76 segundos, 0 errores.
