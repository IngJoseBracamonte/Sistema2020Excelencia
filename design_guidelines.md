# Guías de Diseño y Estética Frontend Premium

Este documento establece las directrices y estándares visuales indispensables para garantizar que cualquier desarrollo frontend mantenga una interfaz de usuario (UI/UX) moderna, consistente y de calidad profesional ("Premium").

---

## 1. Sistema de Color e Identidad Visual
*   **Prohibición de Colores Puros**: Jamás se deben usar colores HTML primarios directos (como `#FF0000` para rojo o `#0000FF` para azul).
*   **Paleta Armónica (HSL/RGB)**: Definir y utilizar tokens semánticos consistentes:
    *   **Fondos Oscuros Sleek**: Tonos oscuros profundos y azulados (ej: `#0B132B`, `#0F172A` o gradientes usando opacidad sobre negro).
    *   **Bordes de Cristal (Glassmorphism)**: Bordes ultra-delgados con opacidades sutiles (ej: `border: 1px solid rgba(255, 255, 255, 0.08)` o `backdrop-filter: blur(12px)`).
    *   **Colores de Acento Vibrantes**: Tonos curados de esmeralda, cian, oro o rosa neón para estados, botones principales y badges (ej: `rose-500`, `emerald-500` con efectos de resplandor sutil).

---

## 2. Tipografía y Jerarquía Visual
*   **Fuentes Modernas**: Evitar las tipografías nativas del navegador. Utilizar fuentes cargadas desde Google Fonts (ej: *Inter*, *Outfit*, o *Roboto*).
*   **Jerarquía de Textos Clara**:
    *   **Labels/Etiquetas**: Tamaño muy pequeño (8px - 10px), en mayúsculas (`uppercase`), negrita extrema (`font-black`), espaciado entre letras (`tracking-widest`) y colores grisáceos/slate semitransparentes.
    *   **Entradas/Datos**: Monoespaciados o fuentes sans-serif muy legibles, con alto contraste para destacar la legibilidad del dato ingresado.

---

## 3. Componentes Interactivos y Dinámicos
*   **Micro-animaciones**: Cada interacción (hover, focus, click) debe responder con transiciones suaves (`transition-all duration-200 ease-in-out`).
*   **Estados Activos (Hover/Focus)**:
    *   Los campos de entrada (`input`) deben reaccionar con sombras de anillo difuminadas (`focus:ring-4 focus:ring-primary/20`) y cambios en el color del borde.
    *   Los botones deben elevarse o cambiar levemente su escala (`hover:scale-[1.02]`) al pasar el cursor.
*   **Insignias de Estado**: Uso de badges redondeados, semitransparentes y de colores semánticos para clasificar estados, triajes o alertas sin saturar el espacio de trabajo.

---

## 4. Estructura y Responsividad
*   **HTML5 Semántico**: Uso riguroso de etiquetas estructurales (`<header>`, `<main>`, `<section>`, `<footer>`).
*   **Consistencia en Espaciados**: Uso continuo de Flexbox y CSS Grid con espaciados unificados (ej: múltiplos de 4px o 8px utilizando `gap-4`, `space-y-4`).
*   **Sin Placeholders Genéricos**: Utilizar bibliotecas de iconos modernas (como **Lucide Icons**) y recursos vectoriales limpios en lugar de marcadores de posición temporales.
