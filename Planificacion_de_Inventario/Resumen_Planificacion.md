# Planificación del Inventario Clínico, Compras y Farmacia

Este directorio contiene las especificaciones detalladas por fases para el desarrollo ordenado de la funcionalidad de compras, administración de catálogo de insumos y descuento de stock clínico en múltiples sedes.

## Estructura de Fases

1. **Fase 1: Pruebas Unitarias del Backend (Casos de Uso y Lógica)**
   - Define las especificaciones de comportamiento del backend usando XUnit antes del desarrollo de lógica.
   - Detalle en: [Fase_1_Pruebas_Unitarias_Backend.md](file:///c:/Src/src/Sistema2020Excelencia/Planificacion_de_Inventario/Fase_1_Pruebas_Unitarias_Backend.md)
   
2. **Fase 2: Desarrollo del Frontend (Angular)**
   - Construcción de las pantallas de Farmacia y Compras, diseño minimalista de la tabla y flujos de aprobación.
   - Detalle en: [Fase_2_Desarrollo_Frontend_Angular.md](file:///c:/Src/src/Sistema2020Excelencia/Planificacion_de_Inventario/Fase_2_Desarrollo_Frontend_Angular.md)
   
3. **Fase 3: Desarrollo del Backend (.NET)**
   - Implementación de la persistencia de datos (reactivos combinados, vencimiento, borrado lógico), endpoints y ajuste de deducción multisede.
   - Detalle en: [Fase_3_Desarrollo_Backend_Net.md](file:///c:/Src/src/Sistema2020Excelencia/Planificacion_de_Inventario/Fase_3_Desarrollo_Backend_Net.md)
   
4. **Fase 4: Integración Final (E2E / Funcional)**
   - Verificación del circuito completo de extremo a extremo: Compra $\rightarrow$ Traslado $\rightarrow$ Despacho $\rightarrow$ Recepción $\rightarrow$ Consumo clínico.
   - Detalle en: [Fase_4_Integracion_Final_E2E.md](file:///c:/Src/src/Sistema2020Excelencia/Planificacion_de_Inventario/Fase_4_Integracion_Final_E2E.md)

---

## Directrices Generales de Calidad
- **Sin Lote**: Se omite el campo Lote a nivel de la entidad Insumo, conservando únicamente Código, Nombre, U.M., Reactivos Combinados, Indicaciones y Vencimiento.
- **Sin Datos Ficticios**: Se prohíbe añadir dashboards de analítica, gráficos o contadores que desvíen la atención del alcance funcional.
- **Interfaz de Escritorio**: El diseño en Angular es prioritario para vistas de escritorio fijos.
