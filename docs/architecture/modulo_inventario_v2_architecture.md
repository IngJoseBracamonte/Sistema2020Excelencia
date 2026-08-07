# Memoria de Arquitectura — Módulo de Inventario v4.0.1 (Motor Transaccional Puro)

**Versión**: v4.0.1  
**Fecha de Actualización**: 2026-08-07  
**Stack de Tecnología**: .NET 9 WebAPI (CQRS MediatR) + Angular 19+ (Standalone Components + Signals)

---

## 1. Principios de Diseño & Invariantes

- **Motor Transaccional Puro**: Se eliminaron completamente las fechas de vencimiento, gestión de lotes, gráficos BI, dashboards de métricas y placeholders/fotografías de productos.
- **Persistencia Monetaria**: Operaciones base persistidas exclusivamente en Dólares ($ USD).
- **Control Flow & Navegación Pure**: Toda la interacción ocurre mediante el `router-outlet` del layout principal sin desplegar modales primarios de edición.
- **Relación N:M Principios Activos**: Cada insumo o medicamento puede asociarse con N principios activos indicando su concentración individual (ej: Ibuprofeno 400mg + Clorfeniramina 4mg).
- **Segregación de Funciones Estricta (SoD)**: El personal operativo (Enfermería / Asistentes) en `/enfermeria` únicamente puede crear requisiciones y confirmar recepción de pedidos. Se deshabilitan totalmente los botones y controles de aprobación en Enfermería (`[readOnlyApprovals]="true"`). La evaluación, ajuste de cantidades y despacho queda centralizado en `/inventario/pedidos`.
- **Simplificación de Formulario & Restricción a 3 Áreas Clínicas**: Se eliminó el campo redundante `TIPO DE DESTINO` en el formulario de requisición. La Sede Solicitante en Enfermería se sincroniza automáticamente con el área clínica activa (**EMERGENCIA**, **HOSPITALIZACIÓN**, **UCI**), dirigiendo la reposición unidireccional directamente hacia el Almacén Principal.
- **Ocultamiento de Badge de Rol en Enfermería**: El distintivo de rol Supervisor/Admin se oculta automáticamente cuando `readOnlyApprovals = true`.
- **Cálculo de Stock Consolidado Global**: Al seleccionar `TODAS LAS SEDES (CONSOLIDADO)` en `/inventario/stock`, el sistema invoca `GET api/inventory/stock-consolidado` ejecutando `SUM(StockActual)` a través de `StocksPorSede`, garantizando el acumulado físico real de existencias sin arrojar 0 o estado "Agotado" por error.
- **Historial de Solicitudes en Inventario**: La pestaña `Historial de Solicitudes` en `/inventario/pedidos` permite auditar la comparación entre la `Cantidad Solicitada` por la sede y la `Cantidad Aceptada/Despachada` por el Almacén Principal, con motivos y justificaciones por ítem.
- **Tablero Kárdex Multisede & Diario**: Endpoint `GET api/inventory/kardex` con selectores para Sede, Insumo y Rango de Fechas (`Fecha Desde` y `Fecha Hasta`). Genera el desglose del **Balance Inicial**, **Total Entradas (+)**, **Total Salidas/Consumos (-)** y **Balance Final**.
- **Asignación Única y Directa al Almacén Principal**: Todo insumo o medicamento creado en el sistema instancia automáticamente su registro base de `StockSede` asignado al Almacén Principal (`SeedConstants.SedeId_Principal`). Se evita la instanciación duplicada de `StockSede` en controladores para mantener la restricción de clave única (`IX_StocksSede_SedeId_InsumoId`) e integrar perfectamente con la regla de Unidireccionalidad.
- **Registro de Catálogo vs Ingreso por Compra**: La creación/registro de un medicamento o insumo en el catálogo (`CreateInsumo`) NO requiere stock inicial obligatorio (> 0) y puede registrarse con stock inicial 0. Es en el módulo de compras (`RecordPurchase` / `/inventario/compras`) donde la cantidad comprada que incrementa existencias debe ser strictly mayor a 0 (`Cantidad > 0`).
- **Modalidad de Ingreso de Costo de Compras (Costo Total vs Costo Unitario)**: En `/inventario/compras`, el operador puede seleccionar entre **💵 Costo Total Renglón ($)** (monto total de la factura por la compra del renglón, ej: $25.00 por 5 cajas de 20 tabletas = 100 tabletas) y **🏷️ Costo Unitario ($)**. La interfaz calcula automáticamente el costo unitario base derivado ($0.25 / tablet) que se guarda en el catálogo para valorizar existencias, evitando multiplicar la cantidad total de unidades base por el precio del paquete/caja.

---

## 2. Segregación de Rutas (Sidebar)

El Sidebar simplifica la navegación bajo la categoría **Inventario**:

| Ruta Frontend | Componente | Descripción / Responsabilidad |
|---|---|---|
| `/inventario/stock` | `StockMultisedeComponent` | Control de existencias por Sede / Consolidado Global y pestaña de Kárdex de Movimientos Diario. |
| `/inventario/compras` | `ComprasComponent` | Registro e ingreso de stock central al Almacén Principal sin vencimiento. |
| `/inventario/pedidos` | `PedidosAprobacionComponent` | Pestañas de **Pendientes por Aprobar** e **Historial de Solicitudes**. Permite despacho parcial con observación obligatoria. |
| `/inventario/catalogo` | `CatalogoComponent` | CRUD de Insumos y gestión de la relación N:M de Principios Activos. |
| `/inventario/descarte` | `DescarteComponent` | Bajas manuales de stock por merma o deterioro con justificación requerida para auditoría. |
| `/inventario/sedes-areas` | `SedeManagementComponent` | Administración y creación de sucursales físicas (Sedes) y departamentos u áreas clínicas (UCI, Emergencia, Hospitalización, Quirófano, Almacén Central). |


---

## 3. Especificación CQRS Backend (.NET 9)

- `RegistrarCompraCommand`: Incrementa stock atómicamente en Sede Principal y actualiza costo unitario USD.
- `RegistrarDescarteCommand`: Valida stock central, aplica descuento directo y registra movimiento tipo `Descarte` con motivo de auditoría.
- `DispatchPedidoInterSedeCommand`: Admite `CantidadesAprobadas` y `ObservacionesPorDetalle`. Valida que si `CantidadAEnviar < CantidadSolicitada`, la observación por detalle es requerida.
- `ReceivePedidoInterSedeCommand`: Al confirmar recepción desde la sede solicitante, incrementa el stock en `StocksSedes` para la sub-sede destino y registra `TransferenciaEntrada`.
- `GetPedidosInterSedeHistorialQuery`: Obtiene el historial completo de solicitudes filtrable por Sede y Rango de Fechas.
- `GetKardex`: Endpoint transaccional que computa Balance Inicial, Entradas, Salidas y Balance Final por Insumo/Sede entre Fechas.

---

## 4. UI/UX Cyber-Medical Glassmorphism

- Paleta HSL tailor-made `#0B0F19` con tarjetas glassmorphic `bg-surface-card/60 backdrop-blur-2xl`.
- Badges dinámicos de principios activos 🧬 en color Índigo (`bg-indigo-500/10 text-indigo-300`).
- Badges de Stock Central con discriminación de disponibilidad (Emerald/Rose).
- Tableros de auditoría de Kárdex y Requisiciones con discriminación por colores de estado.

