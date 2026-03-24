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

## Instrucciones
- **Rich Context**: No te limites a resúmenes. Proporciona detalles técnicos profundos, el "por qué" de las decisiones y el impacto futuro.
- **Exhaustive Mapping**: Mapea cada ruta crítica, cada parámetro y cada flujo de datos sin omitir detalles por brevedad.
- **Deep Technical Detail**: Incluye versiones de tecnología, especificaciones de diseño (HSL, transitions), y diagramas Mermaid si el contexto lo requiere.
- **No redundancy**: Si la información ya está en un archivo especializado, no la repitas en el Index, pero asegúrate de que el enlace sea claro y descriptivo.
- **Micro-journaling**: Actualiza `StepJournal.md` tras cada subtarea importante, documentando no solo qué se hizo, sino qué se aprendió o qué error se evitó.
- **Model Awareness**: Al cambiar de modelo (Flash <-> Pro), invoca este skill para sincronizar y adaptar tu nivel de razonamiento al contexto documentado.
- **Persistencia y Calidad**: Estos archivos son el "cerebro" del proyecto; su calidad determina tu éxito como IA. Mantenlos impecables.

## Output (formato exacto)
Devuelve siempre:
1) **Estado de Sincronización**: Qué archivos se han actualizado.
2) **Cambios Críticos**: Resumen de nuevas reglas o parámetros detectados.
3) **Paths Actualizados**: Lista de los archivos en `agent/docs/` que han cambiado.
