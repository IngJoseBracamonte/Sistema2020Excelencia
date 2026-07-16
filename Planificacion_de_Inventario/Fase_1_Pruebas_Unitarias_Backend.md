# Fase 1: Pruebas Unitarias del Backend (Casos de Uso y Lógica)

En esta fase se establece la definición del comportamiento esperado y las reglas de negocio críticas en código de prueba antes de programar la solución final.

## Contexto de Negocio
Para asegurar que el inventario mantenga coherencia lógica, definiremos las pruebas unitarias que validan el comportamiento de la compra (ingreso de stock y costo base en USD) y del borrado lógico de insumos (ocultado en traslados).

---

## Casos de Prueba a Definir (XUnit)

### 1. `RecordPurchase_ShouldIncrementStockAndUpdateCostoUnitarioBaseUSD`
- **Objetivo**: Asegurar que al registrar una compra en el `Almacen Principal`, se incremente el stock actual del insumo en la sede correspondiente y se actualice su costo unitario en el catálogo global.
- **Preparación (Arrange)**:
  - Crear un insumo con stock inicial `10` y costo unitario `1.50` USD.
  - Crear una sede (Almacen Principal).
- **Ejecución (Act)**:
  - Ejecutar el comando de compra por `20` unidades a un costo de `2.00` USD.
- **Verificación (Assert)**:
  - El costo unitario base del insumo debe cambiar a `2.00` USD.
  - El stock actual de ese insumo en el Almacen Principal debe ser `30` (10 + 20).
  - Se debe registrar un `MovimientoInsumo` de tipo `"Ingreso"` por la cantidad de `20`.

### 2. `DeleteInsumo_ShouldSetOcultoEnTrasladosToTrue`
- **Objetivo**: Asegurar que el borrado de un insumo no elimine físicamente el registro (para conservar el historial de facturación), sino que lo oculte de futuros traslados.
- **Preparación (Arrange)**:
  - Crear un insumo activo.
- **Ejecución (Act)**:
  - Ejecutar la acción de borrado/eliminación.
- **Verificación (Assert)**:
  - La propiedad `OcultoEnTraslados` del insumo debe ser `true`.
  - El insumo debe seguir existiendo en la base de datos.

### 3. `RestoreInsumo_ShouldSetOcultoEnTrasladosToFalse`
- **Objetivo**: Validar la opción de reactivar un insumo para traslados desde el catálogo de compras.
- **Preparación (Arrange)**:
  - Crear un insumo con `OcultoEnTraslados = true`.
- **Ejecución (Act)**:
  - Ejecutar el comando para restaurar/reactivar el insumo.
- **Verificación (Assert)**:
  - La propiedad `OcultoEnTraslados` debe volver a ser `false`.

---

## Entregables de la Fase
- Archivo de pruebas unitarias [ComprasTests.cs](file:///c:/Src/src/Sistema2020Excelencia/src/SistemaSatHospitalario.UnitTests/Application/ComprasTests.cs) en el proyecto de pruebas unitarias.
- El proyecto debe compilar correctamente, y los nuevos tests deben marcarse como fallidos (*red stage*) inicialmente hasta que se desarrolle el backend.
