---
name: creador-de-skills
description: Experto en diseñar Skills predecibles, reutilizables y fáciles de mantener para el entorno de Antigravity.
---

# Creador de Skills

## Cuándo usar este skill
- Cuando el usuario pida crear un skill nuevo.
- Cuando el usuario repita un proceso que pueda automatizarse.
- Cuando se necesite un estándar de formato para tareas recurrentes.
- Cuando haya que convertir un procedimiento largo en una herramienta ejecutable.

## Inputs necesarios
1) Nombre o tema del skill.
2) Objetivo o descripción de lo que debe lograr.
3) Pasos o workflow (si se conocen).
4) Restricciones o formato de salida.

## Workflow
1) **Planificar**: Define el nombre corto, descripción operativa y estructura necesaria.
2) **Validar**: Asegura que los triggers sean claros y los pasos lógicos.
3) **Escribir SKILL.md**: Aplica el frontmatter YAML y las secciones obligatorias.
4) **Generar Recursos**: Crea scripts o plantillas de apoyo si aportan valor real.

## Instrucciones
- Sigue fielmente la estructura: Carpeta -> SKILL.md -> Recursos.
- Mantén el lenguaje en español, profesional y operativo.
- Evita el relleno; cada línea del skill debe ser una instrucción accionable.
- Define niveles de libertad adecuados (Alta para creatividad, Baja para técnico).

## Output (formato exacto)
Devuelve siempre:
1) **Carpeta**: `agent/skills/<nombre-del-skill>/`
2) **SKILL.md**: Contenido completo con YAML.
3) **Recursos**: Lista de archivos adicionales creados.
