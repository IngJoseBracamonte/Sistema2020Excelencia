# Paleta de Colores "Excelencia Premium"

Esta paleta utiliza el sistema HSL para asegurar armonía y facilitar la implementación de temas dinámicos.

## Colores Base (Brand)

| Nombre | HSL | Hex Equivalent | Uso |
| :--- | :--- | :--- | :--- |
| **Primary Blue** | `210, 100%, 50%` | `#007BFF` | Botones principales, acentos destacados. |
| **Primary Soft** | `210, 100%, 90%` | `#CCE5FF` | Fondos de componentes activos. |
| **Accent Emerald** | `160, 100%, 40%` | `#00CC88` | Estados de éxito, ahorro, crecimiento. |
| **Alert Ruby** | `0, 100%, 60%` | `#FF3333` | Errores, avisos críticos. |

## Superficies y Fondos (Surface)

| Nombre | Light Mode (HSL) | Dark Mode (HSL) | Uso |
| :--- | :--- | :--- | :--- |
| **Background** | `210, 20%, 98%` | `220, 30%, 5%` | Fondo general de la aplicación. |
| **Surface** | `0, 0%, 100%` | `220, 25%, 10%` | Fondo de cards, modales, tablas. |
| **Glass** | `rgba(255, 255, 255, 0.7)` | `rgba(15, 20, 30, 0.7)` | Paneles con `backdrop-filter`. |

## Tipografía y Texto

| Nombre | HSL | Contraste | Uso |
| :--- | :--- | :--- | :--- |
| **Text High** | `210, 30%, 15%` | AAA | Títulos y cuerpo principal. |
| **Text Medium** | `210, 10%, 45%` | AA | Etiquetas secundarias, descripciones. |
| **Text Low** | `210, 10%, 70%` | - | Placeholders, bordes inactivos. |

## Uso en Angular (SCSS Variables)

```scss
$primary: hsl(210, 100%, 50%);
$surface: hsl(210, 20%, 98%);
$glass: rgba($surface, 0.7);

.premium-card {
  background: $glass;
  backdrop-filter: blur(12px);
  border: 1px solid rgba($primary, 0.1);
  border-radius: 16px;
  box-shadow: 0 10px 30px -10px rgba(0,0,0,0.1);
}
```
