---
name: cambio-de-agente
description: Garantiza una transición perfecta y sin pérdida de contexto (95%+ de consistencia) al cambiar entre modelos de IA (Gemini Pro, Claude Sonnet/Opus, etc.), forzando el respeto por la memoria de arquitectura existente.
---

# Cambio de Agente (Handoff / Transition)

## Cuándo usar este skill
- Cuando el usuario inicie una conversación con un modelo de IA nuevo o diferente.
- Cuando se retome el desarrollo tras un cambio de sistema o actualización de LLM.
- Para evitar taxativamente que un nuevo agente reescriba patrones, rediseñe la arquitectura o aplique sus "gustos personales" ignorando la base ya cimentada.

## Inputs necesarios
1) El modelo de IA asumiendo el rol (Gemini 3.1 Pro/Flash, Claude Sonnet/Opus, etc.).
2) Lectura del archivo `agent/docs/arquitectura.md` a través del sistema (o instrucción equivalente de `memoria-de-arquitectura`).
3) El objetivo o Micro-Ciclo inmediato a atacar.

## Workflow
1) **Absorción Estricta**: El nuevo agente DEBE leer la memoria de arquitectura y el `implementation_plan.md` antes de emitir cualquier código.
2) **Alineación de Restricciones**: El nuevo agente auto-declara internamente que:
   - "El sistema Legacy de WinForms/C# NO SE MODIFICA bajo ninguna circunstancia".
   - "Se reutilizarán los Casos de Uso y Entidades de Clean Architecture (CajaDiaria, CuentaServicios, CQRS MediatR) ya establecidas en `src`".
   - "El Frontend seguirá las directivas de `experto-angular` (Standalone, Signals, sin RxJS donde no haga falta)".
3) **Congelamiento Arquitectónico**: Queda prohibido plantear reestructuraciones de carpetas, sugerir cambios de framework, ORMs o lógicas de negocio ajenas al plan establecido.
4) **Ejecución Directa**: Retomar fluidamente el desarrollo del Micro-Ciclo en progreso.

## Instrucciones
- Asume que las decisiones documentadas en la "Memoria de Arquitectura" son **ley irrefutable**.
- Tu objetivo primario de consistencia es >95%. Redactarás código emulando los patrones, naming conventions y dependencias del agente que te precedió.
- Si detectas formas "superiores" de hacer algo, guárdatelas. La prioridad es la tracción, la reducción de tokens y reutilizar los Commands/Entidades de Admisión que ya existen.
- Aborda el trabajo dividiéndolo atómicamente; no gastes tokens alucinando todo un sistema de golpe de forma iterativa fallida.

## Output (formato exacto)
Al iniciar su labor o al invocar esta skill explícitamente, la nueva IA devolverá obligatoriamente esta estructura:
1) **Sincronización [Nombre del IA]**: Confirmación de asimilación del archivo de Arquitectura (Context Check de ~2 líneas).
2) **Reglas Asumidas**: Mención rápida validando "Legacy Intocable / Reuso de Commands / Signals".
3) **Próximo Paso Inmediato**: La ejecución de código o la herramienta requerida para continuar el Micro-Ciclo pendente.
