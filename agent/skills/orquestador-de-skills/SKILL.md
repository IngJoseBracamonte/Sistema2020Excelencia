---
name: orquestador-de-skills
description: Orquestador maestro que identifica qué skills son necesarias, recomienda el modelo de IA ideal (Gemini Flash, Pro, Claude Sonnet, Opus) y encadena flujos de trabajo de alto nivel.
---

# Orquestador de Skills (v1.0)

## Cuándo usar este skill
- **Al recibir cualquier solicitud del usuario**: Para determinar la estrategia de resolución.
- Para identificar si faltan skills en el repositorio y sugerir su creación.
- Para optimizar el uso de modelos según la complejidad de la tarea.

## Model Recommendation Engine
Recomienda al usuario el modelo ideal basado en estos criterios:

| Modelo | Fortalezas | Recomendado Para... |
| :--- | :--- | :--- |
| **Gemini 2.0 Flash** | Velocidad extrema, baja latencia, eficiente en tareas repetitivas. | Formateo, ejecución de comandos, correcciones menores de UI, logs. |
| **Google Pro IA (Gemini Pro)** | Razonamiento complejo, gran ventana de contexto (1M+ tokens). | Análisis de arquitectura completa, depuración profunda, migraciones masivas. |
| **Claude 3.5 Sonnet** | Excelencia en codificación, diseño UI/UX, lógica precisa. | Implementación de componentes complejos, refactorización crítica, tests. |
| **Claude 3 Opus** | Pensamiento creativo, estrategia de alto nivel, problemas abstractos. | Brainstorming inicial, diseño de sistemas desde cero, resolución de bloqueos. |

## Workflow de Orquestación
1) **Analizar**: Identifica el objetivo, la escala (Micro/Macro) y los riesgos.
2) **Inventario**: Revisa `agent/skills/` para seleccionar herramientas (e.g., `planificacion-pro`, `brainstorming-pro`).
3) **Recomendar Modelo**: Sugiere al usuario el modelo más apto para la fase actual.
4) **Chaining**: Define el orden de ejecución:
   - *Ejemplo*: `memoria-de-arquitectura` (Contexto) -> `planificacion-pro` (Mapa) -> `Claude Sonnet` (Ejecución).
5) **Sugerir Vacíos**: Si detectas una tarea recurrente sin skill, recomienda usar `creador-de-skills`.

## Instrucciones
- **User Choice**: Al final de tu análisis, pregunta siempre: "¿Deseas proceder con este plan y este modelo (recomiendo [Modelo]) o prefieres ajustar algo?".
- **Context Awareness**: Antes de ejecutar cualquier skill técnica, invoca siempre `memoria-de-arquitectura` para asegurar que el contexto de archivos sea el correcto.
- **Dynamic Suggestions**: Si el usuario pide algo "rápido", prioriza Gemini Flash. Si pide "calidad máxima", recomienda Claude Sonnet/Opus.

## Output (formato exacto)
1) **Análisis de la Tarea**: Breve resumen de lo que se entiende.
2) **Skills Seleccionadas**: Lista de skills que se usarán.
3) **Recomendación de Modelo**: Modelo sugerido + Razón.
4) **Próximo Paso**: Acción inmediata a tomar.
