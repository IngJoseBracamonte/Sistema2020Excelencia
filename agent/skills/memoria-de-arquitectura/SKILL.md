---
name: memoria-de-arquitectura
description: Mantiene un registro actualizado de la arquitectura, rutas clave, dependencias y decisiones técnicas del proyecto para optimizar el uso de tokens y la precisión del análisis.
---

# Memoria de Arquitectura

## Cuándo usar este skill
- Al iniciar el trabajo en un repositorio desconocido.
- Al crear nuevos módulos, servicios o componentes estructurales.
- Cuando se identifiquen rutas de archivos críticas que se consultan frecuentemente.
- Cuando haya cambios en la lógica de negocio central que afecten a múltiples archivos.

## Inputs necesarios
1) Estructura de carpetas actual (vía `list_dir`).
2) Tecnologías y frameworks detectados.
3) Puntos de entrada (Entry points) y archivos de configuración.
4) Flujos de datos principales (e.g., UI -> Service -> Repository).

## Workflow
1) **Explorar:** Escanea el proyecto para identificar la arquitectura (Capa, Monolito, Hexagonal, etc.).
2) **Mapear:** Identifica los "Paths Maestros" (archivos que definen la lógica central).
3) **Documentar:** Crea o actualiza el archivo `agent/docs/arquitectura.md`.
4) **Sintetizar:** Resume las decisiones técnicas para que sean legibles en un solo vistazo.

## Instrucciones
- Mantén el archivo de memoria corto y técnico. No es una guía de usuario, es un mapa para el agente.
- Usa rutas absolutas o relativas consistentes.
- Agrupa por "Zonas de Interés" (e.g., Datos, UI, Lógica de Negocio).
- Indica dependencias críticas que suelen causar errores si se ignoran.
- Si detectas un cambio importante en el proyecto, actualiza la memoria inmediatamente.

## Output (formato exacto)
Devuelve siempre:
1) **Estado Actual de la Memoria**: Resumen de lo que se sabe.
2) **Archivo Creado/Actualizado**: Ruta al archivo `agent/docs/arquitectura.md`.
3) **Paths Críticos**: Lista de los 5 archivos más importantes del contexto actual.
