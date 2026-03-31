---
name: memoria-de-arquitectura
description: Mantiene un registro actualizado de la arquitectura, rutas clave, dependencias y decisiones técnicas del proyecto para optimizar el uso de tokens y la precisión del análisis.
---

# Memoria de Arquitectura (v2.0)

## Cuándo usar este skill
- Al iniciar el trabajo en un repositorio o al cambiar de agente (handoff).
- Al crear nuevos módulos, servicios o componentes estructurales.
- Cuando se identifiquen rutas críticas, reglas de negocio o flujos de datos complejos.
- Para registrar métricas de rendimiento de la IA y lecciones aprendidas (Journal).

## Inputs necesarios
1) Estructura de carpetas actual y tecnologías detectadas.
2) Puntos de entrada y archivos de configuración.
3) Decisiones técnicas previas (leídas de los archivos `.md` existentes en `agent/docs/`).

## Workflow
1) **Sincronizar:** Lee los archivos en `agent/docs/` para retomar el contexto sin re-analizar.
2) **Mapear (Macro/Micro):** Diferencia entre la arquitectura global y los flujos atómicos.
3) **Especializar:** Distribuye la información en los archivos correspondientes:
   - `Architecture.md`: Mapa maestro y visión general.
   - `Rules.md`: Leyes inquebrantables, naming y estándares UI (Rose on Slate).
   - `DataFlow.md`: Rutas de datos (e.g., Frontend -> API -> DB).
   - `Parameters.md`: Configuración técnica, endpoints y tokens.
   - `Metrics.md`: Registro de rendimiento del agente (tokens, build success).
   - `Checks.md`: Checklist de validación contextual.
   - `StepJournal.md`: Registro de las últimas acciones atómicas realizadas.
4) **Optimizar por Agente:** 
   - **Flash**: Usa `Rules.md` y `Parameters.md` para ejecución rápida.
   - **Pro High**: Analiza `Architecture.md` y `DataFlow.md` para cambios estructurales.
   - **Pro Low**: Sigue `Checks.md` y `StepJournal.md` para tareas iterativas.

## Context Orchestration
Al crear algo nuevo (clase, servicio, componente), este skill debe orquestar los archivos necesarios:
- **Nueva Clase/Entidad**: Requiere `Architecture.md`, `Rules.md` y una entidad de referencia similar en `src/Domain/`.
- **Nuevo Handler (Command/Query)**: Requiere `DataFlow.md`, `Parameters.md` y un handler existente en `src/Application/`.
- **Cambio en DB/EF Core**: Requiere `Architecture.md` (Mapping rules) y el `DbContext` actual.
- **Componente UI-Angular**: Requiere `Rules.md` (Estilo) y un componente similar en `src/Web/Clients/`.

**Workflow de Contexto**:
1) Identifica el tipo de creación.
2) Lista los archivos de `agent/docs/` y `src/` que DEBEN leerse antes de escribir código.
3) Recomienda el set de archivos al Orquestador para la ejecución final.

## Instrucciones
- **Rich Context**: No te limites a resúmenes. Proporciona detalles técnicos profundos, el "por qué" de las decisiones y el impacto futuro.
- **Exhaustive Mapping**: Mapea cada ruta crítica, cada parámetro y cada flujo de datos sin omitir detalles por brevedad.
- **Deep Technical Detail**: Incluye versiones de tecnología, especificaciones de diseño (HSL, transitions), y diagramas Mermaid si el contexto lo requiere.
- **No redundancy**: Si la información ya está en un archivo especializado, no la repitas en el Index, pero asegúrate de que el enlace sea claro y descriptivo.
- **Micro-journaling**: Actualiza `StepJournal.md` tras cada subtarea importante, documentando no solo qué se hizo, sino qué se aprendió o qué error se evitó.
- **Model Awareness**: Al cambiar de modelo (Flash <-> Pro), invoca este skill para sincronizar y adaptar tu nivel de razonamiento al contexto documentado.
- **Persistencia y Calidad**: Estos archivos son el "cerebro" del proyecto; su calidad determina tu éxito como IA. Mantenlos impecables.

## 🚫 Anti-Patrones y Errores Frecuentes (Checklist de Seguridad)
Antes de dar una tarea por finalizada, revisa que no hayas cometido estos errores documentados en ciclos previos:

1.  **Migración de Identidad Parcial**: Modificar entidades en el Dominio (ej. de `int` a `Guid`) pero olvidar actualizar los Comandos (`IRequest`), DTOs o Interfaces del Frontend. 
    *   *Check*: ¿Todos los campos `PacienteId` e `Id` en el flujo completo usan el mismo tipo de dato?
2.  **La "Falacia del Auto-Stub"**: Diseñar lógica que crea un registro temporal (stub) si no encuentra un ID. Esto causa duplicidad y fragmentación.
    *   *Check*: ¿Estoy forzando la existencia del registro en el Stage 1 (Registro) o estoy creando "basura" en Stage 2 (Carga)?
3.  **Desincronización de Signals**: Cambiar el tipo de retorno en el API pero no actualizar la `Signal` correspondiente en Angular.
    *   *Check*: ¿El tipo de la Signal en el componente coincide exactamente con el JSON devuelto?
4.  **Enmascaramiento de Errores con `.Ignore()`**: Usar configuraciones de Fluent API para ignorar propiedades que fallan durante la validación de EF Core.
    *   *Check*: ¿Estoy resolviendo la causa raíz de la inconsistencia o simplemente ocultando el síntoma?
5.  **Hardcoding de Cuentas Lab**: Intentar mapear IDs de laboratorio sin verificar si el paciente está correctamente sincronizado con el sistema legado.

## Output (formato exacto)
Devuelve siempre:
1) **Estado de Sincronización**: Qué archivos se han actualizado.
2) **Cambios Críticos**: Resumen de nuevas reglas o parámetros detectados.
3) **Paths Actualizados**: Lista de los archivos en `agent/docs/` que han cambiado.
