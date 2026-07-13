# 🩻 Especificación de Arquitectura: Módulo de Rayos X (RX) y Tomografía

Este documento detalla la arquitectura técnica, las reglas de negocio y los flujos de carga para estudios de **Rayos X (RX)** y **Tomografía** dentro del Sistema Sat Hospitalario.

---

## 🏗️ 1. Concepto y Reglas Operativas

Los estudios de imagenología (Radiografía, Tomografía Axial Computarizada, Ecografías) operan bajo el modo de Stepper `'lab-rx'`. A diferencia de los insumos médicos, no permiten fraccionamiento de cantidades (se asume cantidad fija de 1) y pueden opcionalmente devengar honorarios médicos de especialistas si se asocia un radiólogo para la lectura del estudio.

```mermaid
graph TD
    A[Búsqueda de Estudio RX/Tomo] --> B[Stepper Paso 2: Ajuste Tarifa]
    B --> C{¿Ubicación es Emergencia?}
    C -- Sí --> D[Aplicar Excepción Emergencia: Honorario = 0]
    C -- No --> E{¿Tiene Honorario Base?}
    E -- Sí --> F[Mostrar Selector de Médico Especialista]
    E -- No --> G[Monto Base + Honorario (si aplica)]
    D --> H[Verificar y Confirmar en Paso 3]
    F --> H
    G --> H
```

### Reglas Críticas de Negocio
1. **Fórmula de Cálculo**: Los estudios de RX y Tomografías calculan su precio final bajo la regla unificada:
   $$\text{Precio Final} = \text{Precio Base} + \text{Honorarios (si aplica)}$$
2. **Unidad Fija Estricta (Cantidad = 1)**: Los estudios de RX y Tomografía no poseen selector de cantidad en el Paso 2 de Enfermería ni en el carrito. La cantidad final enviada al backend se fuerza a `1` (Ley de Carga de Imagenología).

2. **Excepción de Honorarios en Emergencias**:
   * Por políticas financieras del hospital, si el **Área Clínica** de origen seleccionada es **Emergencia (EMG)** y el estudio cargado es de tipo **RX**:
     * Se anula de forma automática el honorario médico (se establece a $0.00 USD).
     * No se requiere la selección de un médico responsable en el Stepper.
   * Si es para Hospitalización o consulta externa, el selector de médico se activa de forma mandatoria si el servicio en catálogo tiene un `honorarioBase > 0`.

---

## 💾 2. Persistencia y Mapeo en Catálogo (MySQL)

### Identificación de Categorías
Los estudios de imagenología se identifican en base a su `categoryId` en la tabla `ServicioCatalogo`:
*   `CategoryId = 3`: Radiología (Rayos X)
*   `CategoryId = 6`: Imagenología (Ecografía, Tomografía, Resonancia)

### Regla de Mapeo de Clasificación (Efectiva en Frontend)
```typescript
{
  classification: ITEM_CLASSIFICATIONS.RX,
  categoryIds: [CATEGORY_IDS.RADIOLOGIA, CATEGORY_IDS.IMAGENOLOGIA],
  keywords: ['RX', 'RAYOS', 'RADIOLOGIA', 'TOMOGRAFIA', 'RADIOGRAF', 'ECO', 'TOMOGRAF']
}
```

---

## 🧠 3. Lógica de Backend y Excepciones (C#)

### Validación del Payload en `CargarServicioACuentaCommand`
Cuando la API procesa un cargo de RX:
1. **Clasificación**: Detecta la categoría mediante `baseService.Category`.
2. **Forzar Cantidad**: Ignora el valor de `request.Cantidad` enviado por red y establece `finalCantidad = 1m`.
3. **Validación de Precios**:
   * Si el precio base del estudio fue editado en el frontend, valida que el cambio no supere el límite permitido de descuento sin clave de supervisor.
   * Si el área de carga es Emergencia, sobrescribe el honorario a $0.00 de forma defensiva antes de guardar en la tabla `DetalleCuentaServicios`.
