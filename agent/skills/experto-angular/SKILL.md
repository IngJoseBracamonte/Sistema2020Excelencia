---
name: experto-angular
description: Aplica las mejores prácticas de Angular v14+ (Standalone components, Signals, Lazy Loading, Clean Architecture en Frontend) para desarrollar aplicaciones escalables.
---

# Experto Angular

## Cuándo usar este skill
- Cuando se requiera crear o refactorizar componentes, servicios, pipes o directivas en Angular.
- Al implementar flujos completos de vistas (rutas lazy-loaded, features).
- Cuando el usuario pida aplicar arquitectura escalable en el frontend (`features`, `core`, `shared`).
- Para la optimización de performance (Change Detection OnPush, RxJS, Signals).

## Inputs necesarios
1) Requerimiento funcional o elemento a crear/modificar.
2) Versión de Angular objetivo (asume v14+ / v19 por defecto).
3) Stack UI (e.g., TailwindCSS, SCSS).

## Workflow
1) **Planificar**: Define qué piezas son necesarias (Componentes Dumb/Smart, Servicios de estado/API).
2) **Ubicar**: Determina la ruta correcta según Clean Architecture (e.g., `src/app/features/...`).
3) **Implementar**: Escribe el código TypeScript, el template HTML y los estilos.
4) **Validar**: Asegura que el código siga las reglas de modernidad (Standalone, Signals, Inject).

## Instrucciones
- Prioriza SIEMPRE crear componentes **Standalone** (`standalone: true`). Evita usar `NgModules` a menos que sea estrictamente necesario o para legado.
- Usa la función `inject()` para la inyección de dependencias en lugar de inyectar a través del `constructor`.
- Para el manejo de estado reactivo local, utiliza las nuevas primitivas **Signals** (`signal`, `computed`, `effect`) en lugar de `BehaviorSubject`.
- Resuelve llamadas asíncronas complejas o eventos con base temporal mediante **RxJS**. Suscríbete y desuscríbete correctamente (usa `takeUntilDestroyed` o el pipe `async`).
- Establece `changeDetection: ChangeDetectionStrategy.OnPush` por defecto.
- Escribe código limpio, autoexplicativo y aprovecha características modernas de TypeScript y las nuevas "Control Flow Syntax" de Angular (e.g., `@if`, `@for` en templates).

## Output (formato exacto)
Devuelve tu repuesta en la siguiente estructura:
1) **Análisis Angular**: Decisión de arquitectura local (Control Flow, State Management escogido, tipo de componente).
2) **Ruta Recomendada**: La ubicación óptima del componente (Ej. `src/app/shared/components/...`).
3) **Código TypeScript**: El componente `.ts`.
4) **Template HTML / Estilos**: El código relacionado de la vista (puede ir embebido si es muy corto).
