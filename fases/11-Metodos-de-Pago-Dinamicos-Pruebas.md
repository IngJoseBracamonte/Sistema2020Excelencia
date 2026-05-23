# Planificación de Pruebas: Métodos de Pago Dinámicos y CRUD de Monedas

Este documento establece la estrategia y los casos de prueba detallados para validar la correcta persistencia por monedas, el recálculo automático en base a grupos en servidor y cliente, y el panel administrativo CRUD.

---

## 1. Pruebas Unitarias Automatizadas (Backend)

### Cobertura de Lógica CRUD (`CatalogoMetodoPago`)
1. **`Debe_CrearMetodoPagoExitosamente_Cuando_DatosSonValidos`**:
   - Envía un comando válido.
   - Verifica que se inserte en base de datos, con el `GrupoMoneda` correcto y que `EsUSD` se calcule correctamente como `true` si es Grupo 1 o `false` si es Grupo 2.
2. **`Debe_FallarCreacion_Cuando_ValorInternoYaExiste`**:
   - Intenta insertar un método con un `Valor` que ya existe en el catálogo (ej: `"Zelle"`).
   - Verifica que lance una excepción de clave única o conflicto.
3. **`Debe_DesactivarEnLugarDeBorrarFisico_Cuando_MetodoTieneTransacciones`**:
   - Intenta borrar un método de pago que ha sido utilizado en un `DetallePago` existente.
   - Verifica que el handler no lo borre físicamente, sino que establezca `Activo = false` en la entidad.
4. **`Debe_BorrarFisicamente_Cuando_MetodoNoTieneTransacciones`**:
   - Intenta borrar un método recién creado sin transacciones.
   - Verifica que sea eliminado por completo de la tabla `CatalogoMetodosPago`.

### Cobertura de Motor de Conversión & Tasa de Cambio (`DetallePago`)
1. **`Debe_CalcularEquivalenteDirecto_Cuando_GrupoMonedaEs1`**:
   - Registra un pago del Grupo 1 (ej: USD, Zelle).
   - Verifica que `EquivalenteAbonadoBase` sea idéntico a `MontoAbonadoMoneda` y `TasaCambioAplicada = 1.0000`.
2. **`Debe_CalcularEquivalenteDivididoPorTasa_Cuando_GrupoMonedaEs2`**:
   - Registra un pago del Grupo 2 (ej: Bolívares).
   - Verifica que `EquivalenteAbonadoBase = MontoAbonadoMoneda / TasaCambioDia` y `TasaCambioAplicada = TasaCambioDia`.
3. **`Debe_FallarRegistro_Cuando_TasaCambioEsCeroYMonedaEsGrupo2`**:
   - Intenta registrar un pago en bolívares cuando la tasa registrada del día es `0` o negativa.
   - Verifica que el backend arroje una excepción controlada previniendo la división por cero.

---

## 2. Pruebas Manuales y E2E (Frontend + Backend)

### Flujo Administrativo (CRUD)
1. **Ingreso y Tabla de Listado**:
   - Ir a *Configuraciones -> Métodos de Pago*.
   - Verificar que se carguen todos los métodos de pago sembrados (activos e inactivos) ordenados por su columna `Orden`.
2. **Creación**:
   - Hacer clic en "Nuevo Método".
   - Rellenar: Nombre = "EFECTIVO EUROS", Valor = "Euro Efectivo", Grupo = "1 - Dólar (USD / Directo)", Orden = 9, Activo = Sí.
   - Guardar. Comprobar que aparece en la tabla.
3. **Edición y Desactivación**:
   - Editar "Efectivo BS". Cambiar su orden a 10. Desactivarlo.
   - Guardar.
   - Ir a la pantalla de Admisión/Facturación y comprobar que "Efectivo BS" ya **no** se despliega en las opciones de cobro.
   - Volver a activarlo en configuraciones y comprobar que vuelve a estar disponible.

### Flujo de Facturación y Conversión de Pagos (Multi-Moneda)
1. **Cobro Mixto**:
   - Crear una cuenta de servicios por un total de `$110.00 USD`.
   - Registrar abono 1: Zelle (Grupo 1) -> Monto moneda: `$10.00`, equivalente: `$10.00` (Tasa aplicada = 1.00).
   - Registrar abono 2: Pago Móvil (Grupo 2) -> Monto moneda: `5000.00 Bs`. Con tasa de cambio a `50.00 Bs/$`, el equivalente calculado debe ser `$100.00`.
   - Confirmar el cobro.
2. **Auditoría en Base de Datos**:
   - Consultar la tabla `DetallesPago` para este recibo.
   - Verificar las 2 filas de pago:
     - Fila 1: `MetodoPago = "Zelle"`, `MontoAbonadoMoneda = 10.00`, `EquivalenteAbonadoBase = 10.00`, `TasaCambioAplicada = 1.0000`.
     - Fila 2: `MetodoPago = "Pago Movil"`, `MontoAbonadoMoneda = 5000.00`, `EquivalenteAbonadoBase = 100.00`, `TasaCambioAplicada = 50.0000`.
     - Ambas filas deben tener registrado el `UsuarioCarga` correspondiente y la `FechaPago` con la hora exacta del servidor.

### Flujo de Cierre de Caja
1. **Arqueo y Declaración**:
   - Ir a la pantalla de Cierre de Caja del cajero.
   - Verificar que los montos declarados por el cajero se calculen en caliente multiplicando o dividiendo de forma correcta según el grupo de moneda del catálogo y que la diferencia neta en USD base se actualice en tiempo real.
