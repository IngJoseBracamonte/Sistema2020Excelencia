---
name: documentacion-senior-microsoft
description: Genera documentación técnica de alto impacto para entornos corporativos de nivel Microsoft Senior. Enfocado en arquitectura, precisión técnica y visión estratégica.
---

# Documentación Senior Microsoft

## Cuándo usar este skill
- Cuando se deba entregar un módulo o sistema a un equipo de arquitectura senior o a un cliente corporativo de alto perfil.
- Cuando se necesite documentar decisiones de diseño complejas, patrones de arquitectura o estrategias de escalabilidad.
- Cuando la claridad técnica deba ir acompañada de una presentación premium y estructurada (nivel Microsoft/Fortune 500).
- Cuando se requiera un balance entre visión de negocio y profundidad técnica (Deep Dive).

## Inputs necesarios
1) **Contexto Técnico**: Stack (e.g., .NET Aspire, Angular Signals, MySQL), patrones usados y estado actual.
2) **Alcance**: ¿Es un RFC, una bitácora de cambios (Changelog), una especificación técnica o un manual de arquitectura?
3) **Público Objetivo**: (e.g., Desarrolladores, Arquitectos, Stakeholders).

## Workflow
1) **Brainstorming & Estructura**: Define los pilares de la documentación (Arquitectura, Seguridad, Performance, UX).
2) **Diagramación (Mermaid)**: Crea representaciones visuales del flujo de datos, estados y componentes.
3) **Refinamiento Técnico**: Explica el "Por qué" (Rationale) detrás de cada decisión operativa, no solo el "Qué".
4) **Garantía de Calidad**: Asegura que el tono sea profesional, directo y libre de ambigüedades.

## Instrucciones y Estándares Microsoft
### 1. Arquitectura & Diseño (The Foundation)
- Siempre incluye diagramas **Mermaid** claros (C4 Model simplificado o Diagramas de Clase/Secuencia).
- Explica la adherencia a **Principios SOLID** y patrones específicos (.NET/Angular).
- Detalla la infraestructura: Orquestación (Aspire), Service Discovery y persistencia.

### 2. Angular Pro Standards
- Documenta el uso de **Signals (Reactive programming)** vs Observables.
- Detalla la estrategia de **Standalone Components**, Lazy Loading e inyección de dependencias avanzada (`inject()`).
- Explica la gestión de estado y el ciclo de vida de los componentes críticos.

### 3. Excelencia Operativa & Seguridad
- Documenta estrategias de manejo de errores, resiliencia (Retry policies) y logs inteligentes.
- Detalla la seguridad: JWT, Role-Based Access Control (RBAC), seguridad en base de datos.
- Menciona la optimización de rendimiento: Tree-shaking, Change Detection strategy y query optimization en MySQL (EF Core).

### 4. Tono y Formato
- Usa un lenguaje formal, técnico y conciso.
- Resalta secciones críticas con alertas GitHub (`> [!IMPORTANT]`, `> [!TIP]`).
- Provee ejemplos de código de alta calidad en secciones de 'Implementation Details'.

## Output (Formato Microsoft Executive)
Toda documentación generada con este skill debe seguir esta estructura base:
1.  **Executive Summary**: El valor del cambio/sistema en 3 oraciones.
2.  **Architectural Blueprint**: Diagramas Mermaid y descripción de capas.
3.  **Core Technical Implementation**: Detalles específicos del Stack (Angular/C#/Aspire).
4.  **Security & Reliability analysis**: Cómo garantizamos que el sistema no falle y sea seguro.
5.  **Senior Recommendations**: Próximos pasos y escalabilidad a largo plazo.
