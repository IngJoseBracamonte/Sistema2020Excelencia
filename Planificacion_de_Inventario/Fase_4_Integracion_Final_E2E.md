# Fase 4: Integración Final (E2E / Funcional)

En esta fase se conecta la interfaz de usuario en Angular con la lógica real expuesta en .NET Core para realizar pruebas funcionales de extremo a extremo que validen el correcto funcionamiento de todo el circuito de compras, traslados y consumos clínicos.

---

## Flujo del Circuito Completo de Pruebas (Paso a Paso)

### Paso 1: Inicialización y Creación de Insumo con Reactivos
1. Ir a la pestaña **Gestión del Catálogo (CRUD)** en el módulo de Farmacia.
2. Hacer clic en **Crear Insumo** y registrar:
   - Código: `IBUP-CAF-500`
   - Nombre: `Ibuprofeno + Cafeína 500mg`
   - Categoría: `MEDICAMENTO`
   - Unidad de Medida: `UNIDAD`
   - Costo Base Inicial: `1.00` USD
   - Reactivos Combinados: `Ibuprofeno, Cafeina`
   - Indicaciones: `Tomar 1 tableta cada 8 horas con alimentos.`
   - Fecha de Vencimiento: `31/12/2028`
3. Guardar el insumo y confirmar que aparece listado en la tabla del catálogo.

### Paso 2: Validación de la Búsqueda Avanzada por Reactivos
1. En la barra de búsqueda superior de la Gestión del Catálogo, escribir `Cafeina`.
2. Verificar que la tabla se filtra y muestra `Ibuprofeno + Cafeína 500mg` ya que contiene "Cafeina" en sus reactivos combinados.
3. Repetir la búsqueda con `IBUP-CAF` y confirmar que también se encuentra.

### Paso 3: Validación del Ocultamiento (Soft Delete) en Traslados
1. En la Gestión del Catálogo, hacer clic en el botón de **Eliminar/Ocultar** para el ítem `IBUP-CAF-500`.
2. Verificar que su estado visual cambia a "Oculto para Envíos" (y el botón cambia a **Restaurar**).
3. Ir al módulo de **Pedidos Inter-Sede** (Traslados).
4. Hacer clic en **Crear Solicitud** e intentar buscar `IBUP-CAF-500` o `Ibuprofeno` en el selector de insumos.
5. **Resultado esperado**: El producto no debe figurar en el dropdown porque está en borrado lógico.

### Paso 4: Validación de la Restauración del Insumo
1. Regresar a la pestaña de **Gestión del Catálogo** en Farmacia y hacer clic en **Restaurar** sobre el ítem `IBUP-CAF-500`.
2. El indicador visual de "Oculto para Envíos" debe desaparecer.
3. Regresar al selector de **Pedidos Inter-Sede** y confirmar que el insumo ya es visible y elegible para traslados nuevamente.

### Paso 5: Registro de Compra e Ingreso de Inventario
1. Ir a la pestaña **Registrar Compra / Entrada**.
2. Seleccionar la sede `Almacen Principal`.
3. Buscar y seleccionar `Ibuprofeno + Cafeína 500mg`.
4. Ingresar una cantidad de `100` unidades y un precio de costo unitario de `2.50` USD.
5. Hacer clic en **Añadir al Carrito**.
6. Hacer clic en **Registrar Entrada de Inventario**.
7. **Verificación**:
   - El stock actual del insumo en el Almacen Principal debe marcar `100`.
   - El precio de costo unitario del insumo en el catálogo debe haberse actualizado de `1.00` USD a `2.50` USD.

### Paso 6: Solicitud de Traslado Inter-Sede
1. Ir al módulo de **Pedidos Inter-Sede**.
2. Crear un pedido seleccionando como Sede Solicitante: `Sede Hospitalaria` (la sede secundaria) y Sede Proveedora: `Almacen Principal`.
3. Agregar el insumo `Ibuprofeno + Cafeína 500mg` con cantidad solicitada `20`.
4. Enviar la solicitud.

### Paso 7: Aprobación y Despacho en Farmacia
1. Como Farmaceuta, ir a la pestaña **Aprobar Pedidos Pendientes** de Farmacia.
2. Identificar el pedido pendiente y hacer clic en **Aprobar y Despachar**.
3. **Verificación**: El stock en `Almacen Principal` disminuye de `100` a `80`. El pedido pasa al estado "Despachado".

### Paso 8: Recepción de Mercancía en Sede Destino
1. Ir al módulo de **Pedidos Inter-Sede** desde la perspectiva de la `Sede Hospitalaria`.
2. Seleccionar el pedido despachado y confirmar la recepción de las `20` unidades.
3. **Verificación**: El stock actual del insumo en `Sede Hospitalaria` aumenta de `0` a `20`.

### Paso 9: Aplicación de Dosis y Suministro al Paciente
1. Ir al módulo de **Enfermería** y seleccionar la sede `Sede Hospitalaria`.
2. Seleccionar un paciente admitido en **Emergencia** o **Hospitalizacion**.
3. Ir a la pestaña de **Cargar Cargos / Fast Charge**.
4. Buscar y seleccionar el servicio o insumo `Ibuprofeno + Cafeína 500mg`.
5. Seleccionar la cantidad de `1` unidad y hacer clic en **Cargar Servicio**.
6. **Verificación final**: El stock del insumo en la `Sede Hospitalaria` debe decrementarse a `19`. El consumo debe registrarse en el historial clínico del paciente en USD.

---

## Entregables de la Fase
- Walkthrough funcional completo documentado en `walkthrough.md`.
- Verificación visual mediante captura de pantalla de la interfaz de compras y catálogo.
- Pruebas E2E de Playwright ejecutadas exitosamente.
