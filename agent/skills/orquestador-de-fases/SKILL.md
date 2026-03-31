---
name: orquestador-de-fases
description: Orquestador de ciclo de vida del proyecto. Gestiona la creación de módulos y funcionalidades a través de fases estructuradas (Arquitectura, Dominio, Infraestructura, Frontend, etc.).
---

# Orquestador de Fases (v1.0)

## Propósito
Este skill guía la implementación técnica de una nueva funcionalidad o módulo, asegurando que se sigan los estándares arquitectónicos del proyecto y se mantenga un registro de calidad.

## Workflow de Fases

### Fase 1: Pre-Definición y Arquitectura
- **Acción**: El usuario asigna la arquitectura (Clean Architecture, DDD, etc.).
- **Naming**: Se definen los nombres de los proyectos siguiendo el estándar (e.g., `Core.Domain`, `Infrastructure`).
- **Check**: ¿La arquitectura es compatible con el sistema actual?

### Fase 2: Estructura de Carpetas
- **Acción**: Creación de los directorios base y archivos de proyecto (`.csproj`).
- **Tool**: Usa comandos shell para crear la estructura.

### Fase 3: Capa de Dominio (Domain)
- **Acción**: Creación de Entidades, Value Objects e Interfaces de repositorio.
- **Contexto**: Invoca `memoria-de-arquitectura` para asegurar la consistencia del modelo.

### Fase 4: Infraestructura y Persistencia
- **Acción**: Implementación de Repositorios, DbContext, Mapeos y adaptadores externos.

### Fase 5: Lógica de Procesos (Application)
- **Acción**: Implementación de Handlers (MediatR), Comandos, Consultas y Validaciones de negocio.

### Fase 6: Reglas y Pruebas (QA)
- **Acción**: Creación de Unit Tests y verificación de reglas de negocio críticas.

### Fase 7: Diseño y Reglas del Frontend
- **Acción**: Creación de componentes Angular (Smart/Dumb), Signals y servicios de UI.
- **Reglas**: Definición de comportamientos y validaciones en el cliente.

### Fase 8: Gestión de Bugs
- **Acción**: Durante el diseño, se detectan errores que se guardan en `agent/docs/Bugs.md`.
- **Regla Crítica**: Si se detecta un bug, **DEBES PREGUNTAR AL USUARIO**:
  1. ¿Deseas solucionarlo inmediatamente?
  2. ¿Deseas guardarlo en el log de bugs para después?

### Fase 9: Producción y Entrega
- **Acción**: Verificación final, logs de telemetría y checklist de despliegue.

## Instrucciones de Orquestación
1. **Estado Actual**: Al iniciar, identifica en qué fase se encuentra el proyecto.
2. **Recomendación de Modelo**: Invoca al `orquestador-de-skills` para recomendar el modelo de IA ideal para la fase actual.
3. **Persistencia**: Actualiza el `StepJournal.md` al finalizar cada fase.
4. **Bug Tracking**: No avances a la siguiente fase si hay bugs críticos sin decisión del usuario.

## Output (formato exacto)
1) **Fase Actual**: Nombre y progreso de la fase.
2) **Acciones Realizadas**: Lista de tareas completadas.
3) **Bugs Detectados**: Lista de errores (si los hay) y estado (Pendiente/Corregido).
4) **Próxima Fase**: Descripción de lo que sigue.
