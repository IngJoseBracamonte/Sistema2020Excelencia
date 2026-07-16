# Fase 2: Desarrollo del Frontend (Angular)

En esta fase se crea la interfaz de usuario basándose en los componentes y la navegación en Angular, siguiendo las estrictas directrices de UI (colores corporativos, minimalismo, sin gamificación, ni métricas ficticias, enfocado en diseño de escritorio único).

## Estilo e Identidad Visual (SAT Hosp)
- **Fondo de la Aplicación**: `#f3faff` (Gris/Azul claro clínico y limpio).
- **Color Primario (Énfasis/Acción Crítica)**: `#af101a` (Rojo institucional).
- **Color Secundario (Navegación/Tablas)**: `#005faf` (Azul confianza).
- **Tipografía**: `Inter` para datos e información y `JetBrains Mono` para códigos, lotes, fechas de vencimiento y cantidades numéricas.
- **Bordes y Sombras**: Tonalidades limpias con bordes definidos de `1px` y esquinas suavemente redondeadas (`rounded-md`, `rounded-lg`). Sin sombras pesadas o efectos 3D.
- **Touch Targets / Interactividad**: Botones y campos de entrada con altura de `48px` para evitar clics erróneos.

---

## Componentes y Vistas a Desarrollar

### 1. Modificación de Modelos y Servicios
- **`inventory.model.ts`**:
  - Agregar campos: `reactivosCombinados?: string;`, `indicaciones?: string;`, `fechaVencimiento?: string;`, `ocultoEnTraslados?: boolean;`.
- **`inventory.service.ts`**:
  - `getInsumos(excludeHidden?: boolean)`: Agregar parámetro para filtrar la lista de insumos de envío.
  - `recordPurchase(purchaseDto: RecordPurchase)`: Endpoint `/api/inventory/compras` para guardar compras.
  - `deleteInsumo(id: string)`: Borrado lógico (`/api/inventory/insumos/{id}`).
  - `restoreInsumo(id: string)`: Restaurar insumo (`/api/inventory/insumos/{id}/restaurar`).

### 2. Componente de Farmacia y Compras (`compras.component.ts` / `compras.component.html`)
Un componente minimalista de escritorio con tres pestañas superiores para fácil navegación:

#### Pestaña 1: Registrar Compra / Entrada
- **Campos**:
  - Selector de Sede de Entrada (por defecto "Almacen Principal").
  - Buscador de Insumos con autocompletado interactivo.
  - Campo numérico para Cantidad.
  - Campo numérico para Costo Unitario de Compra (en USD).
- **Acción**: Botón "Añadir al Carrito" (icono `Plus`).
- **Carrito de Compra**: Tabla compacta que lista los ítems agregados (Nombre, Cantidad, Costo Unitario, Total en USD) con botón para remover.
- **Confirmación**: Botón principal "Registrar Entrada de Inventario" con indicador de carga para persistir en backend.

#### Pestaña 2: Aprobar Pedidos Pendientes
- **Objetivo**: Listar solicitudes de traslado inter-sede dirigidas a la sede de Farmacia.
- **Tabla**: Correlativo, Sede Solicitante, Fecha, Ítems Solicitados (Nombre, Cantidad) y Botón "Aprobar y Despachar" (Llama a `DispatchPedido` en backend).

#### Pestaña 3: Gestión del Catálogo (CRUD)
- **Barra de Búsqueda**: Busca en tiempo real por Nombre, Código o Reactivo.
- **Tabla Adaptada (Directriz de Visualización Limpia)**:
  - *Imagen*: Icono genérico minimalista `Package` de Lucide.
  - *Pedir*: Checkbox para selección o indicación rápida de reposición.
  - *Nombre del Producto*: Nombre en texto destacado, y justo debajo en letra pequeña y color gris (`text-slate-500`): Reactivos combinados e indicaciones.
  - *Lote/Vencimiento*: Solo vencimiento formateado (`fechaVencimiento | date: 'dd/MM/yyyy'`) en tipografía `JetBrains Mono`. (Columna Lote omitida según feedback "sin lote").
  - *Código*: Código del insumo en tipografía `JetBrains Mono`.
  - *Unidad de Medida (U.M.)*: Unidad base (e.g. UNIDAD, ML, G).
  - *Existencia*: Cantidad en stock en la sede activa.
  - *Precio/Costo Unitario*: Costo en USD.
  - *Acciones (Barra)*: Botón de Editar (Modal) y Botón de Ocultar/Eliminar (Borrado lógico) o Restaurar.
- **Modal de Creación/Edición**:
  - Formulario con campos: Código, Nombre, U.M., Categoría, Costo Base, Reactivos Combinados, Indicaciones y Fecha Vencimiento.
  - Sin columnas comerciales complejas (IVA, DV%, DCT%).

### 3. Exclusión en Traslados (`pedidos-inter-sede.component.ts`)
- Modificar la carga de insumos en la sección de creación de solicitudes de traslado para enviar `this.inventoryService.getInsumos(true)` (excluyendo ocultos), previniendo que los insumos con `ocultoEnTraslados === true` sean elegibles para traslados.

---

## Entregables de la Fase
- Componente Angular [compras.component.ts](file:///c:/Src/src/Sistema2020Excelencia/src/SistemaSatHospitalario.Frontend/src/app/features/admin/inventory/compras.component.ts) y su plantilla HTML.
- Rutas actualizadas en [app.routes.ts](file:///c:/Src/src/Sistema2020Excelencia/src/SistemaSatHospitalario.Frontend/src/app/app.routes.ts) y enlace en el menú lateral [sidebar.component.html](file:///c:/Src/src/Sistema2020Excelencia/src/SistemaSatHospitalario.Frontend/src/app/shared/components/sidebar/sidebar.component.html).
- Lógica de exclusión de insumos oculta operando en el selector de traslados inter-sede.
