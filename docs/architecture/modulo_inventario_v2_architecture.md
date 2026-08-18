# Memoria de Arquitectura — Módulo de Inventario v4.1.0 (Motor Transaccional & Auditoría Consolidada)

**Versión**: v4.1.0  
**Fecha de Actualización**: 2026-08-09  
**Stack de Tecnología**: .NET 9 WebAPI (CQRS MediatR) + Angular 19+ (Standalone Components + Signals)

---

## 1. Principios de Diseño & Invariantes

- **Motor Transaccional Puro**: Se eliminaron completamente las fechas de vencimiento, gestión de lotes, gráficos BI, dashboards de métricas y placeholders/fotografías de productos.
- **Persistencia Monetaria**: Operaciones base persistidas exclusivamente en Dólares ($ USD).
- **Control Flow & Navegación Pure**: Toda la interacción ocurre mediante el `router-outlet` del layout principal sin desplegar modales primarios de edición.
- **Relación N:M Principios Activos**: Cada insumo o medicamento puede asociarse con N principios activos indicando su concentración individual (ej: Ibuprofeno 400mg + Clorfeniramina 4mg).
- **Segregación de Funciones Estricta (SoD)**: El personal operativo (Enfermería / Asistentes) en `/enfermeria` únicamente puede crear requisiciones y confirmar recepción de pedidos. Se deshabilitan totalmente los botones y controles de aprobación en Enfermería (`[readOnlyApprovals]="true"`), y se oculta completamente la visibilidad del Stock disponible del Almacén Principal (`!readOnlyApprovals`) para mantener un formulario limpio de requisición sin especulaciones. La evaluación, ajuste de cantidades y despacho queda centralizado exclusivamente en `/inventario/envios-recepciones` para supervisores.
- **Flujo de Envío Directo a Sub-Áreas (Salida Definitiva de Almacén)**: Las sub-áreas como Laboratorio, Farmacia, Mantenimiento, etc. que operan con dinámicas externas, junto con la sede de **Hospitalización**, son los únicos destinos permitidos en el selector de Despacho Directo (`EnviosRecepcionesComponent`). Al ejecutar un "Envío a Sub-Área", el sistema descuenta de forma única y atómica el stock físico del Almacén Principal (`SedeId_Principal`) y genera un registro inmutable en `MovimientoInsumo` de tipo `"EnvioSubArea"`, capturando timestamp, insumo, cantidad, sub-área/sede de destino, usuario y motivo.
- **Simplificación de Formulario & Restricción a Áreas Clínicas**: La Sede Solicitante en Enfermería se sincroniza automáticamente con el área clínica activa (**EMERGENCIA**, **HOSPITALIZACIÓN**, **UCI**, **CIRUGÍA**), dirigiendo la reposición unidireccional directamente hacia el Almacén Principal.
- **Módulo Consolidado de Historiales (6 Dimensiones de Trazabilidad)**: Ubicado en `/inventario/historiales`. Integra en una sola interfaz reactiva 6 pestañas de auditoría: 1) Historial de Ingreso de Medicamentos, 2) Historial de Compras, 3) Historial de Aprobación de Pedidos, 4) Historial de Envíos a Subáreas, 5) Historial de Descartes y 6) Historial de Cuentas por Pagar. Permite filtrado universal por rango de fechas (`Fecha Desde` / `Fecha Hasta`) y por texto reactivo.
- **Módulo de Cuentas por Pagar de Inventario (Proveedores)**: Ubicado en `/inventario/cuentas-por-pagar`. Permite gestionar las facturas de proveedores (`OrdenCompraInventario`) y el registro atómico de abonos/pagos (`PagoProveedor`) en $ USD y su conversión oficial a Bs. Valida que ningún abono supere el `SaldoPendienteUSD` actual y transiciona automáticamente la compra a estado **Pagado** al alcanzar el 100% saldado.
- **Presentación Comercial Descriptiva & Cantidad Kárdex Directa**: En el módulo de Compras y Recepción (`/inventario/compras`), el campo `PresentacionCompra` (`string`, ej: '10 Viales x 10 mL') es puramente informativo. La cantidad ingresada por el usuario (`Cantidad`, `decimal`) representa directamente las unidades físicas totales a sumar al Kárdex de Almacén Principal, prescindiendo de multiplicadores automáticos, fórmulas o conversiones de dosis.

---

## 2. Segregación de Rutas (Sidebar)

El Sidebar simplifica la navegación bajo la categoría **Inventario**:

| Ruta Frontend | Componente | Descripción / Responsabilidad |
|---|---|---|
| `/inventario/stock` | `StockMultisedeComponent` | Control de existencias por Sede / Consolidado Global y pestaña de Kárdex de Movimientos Diario. |
| `/inventario/reposicion` | `ReposicionInventarioComponent` | Gestión de reposiciones, devoluciones de insumos y cambios de talla/calibre entre sedes sin desfase de stock. |
| `/inventario/compras` | `ComprasComponent` | Registro e ingreso de stock central al Almacén Principal sin vencimiento. |
| `/inventario/cuentas-por-pagar` | `CuentasPorPagarComponent` | Gestión de compras a proveedores, registro de abonos en $ USD / Bs, cálculo de saldos y auditoría de pagos. |
| `/inventario/envios-recepciones` | `EnviosRecepcionesComponent` | Pestaña 1 (Aprobación de Requisiciones Inter-Sede) + Pestaña 2 (Despacho Directo a Sub-Áreas como Salida Definitiva de Almacén Principal). |
| `/inventario/historiales` | `HistorialesComponent` | Consola de auditoría de 6 dimensiones (Ingresos, Compras, Pedidos, Envíos a Subáreas, Descartes, Cuentas por Pagar). |
| `/inventario/catalogo` | `CatalogoComponent` | CRUD de Insumos y gestión de la relación N:M de Principios Activos. |
| `/inventario/descarte` | `DescarteComponent` | Bajas manuales de stock por merma o deterioro con justificación requerida para auditoría. |
| `/inventario/sedes-areas` | `SedeManagementComponent` | Administración y creación de sucursales físicas (Sedes) y departamentos u áreas clínicas. |

---

## 3. Especificación CQRS Backend (.NET 9)

- `ProcesarReposicionStockCommand`: Ejecuta transferencias atómicas de insumos entre sedes (descuento en origen, incremento en destino) y registra auditoría inmutable en `TransferenciaReposicionStock`.
- `GetReposicionesHistorialQuery`: Consulta el historial de transferencias y reposiciones con filtros por sede, insumo, fechas y motivo.
- `RegistrarCompraCommand`: Incrementa stock atómicamente en Sede Principal y actualiza costo unitario USD.
- `RegistrarDescarteCommand`: Valida stock central, aplica descuento directo y registra movimiento tipo `Descarte` con motivo de auditoría.
- `DispatchPedidoInterSedeCommand`: Admite `CantidadesAprobadas` y `ObservacionesPorDetalle`. Al ser aprobado por el Supervisor, ejecuta de forma atómica e inmediata la salida del Almacén Principal (`TransferenciaSalida`) y la recepción/sumado directo en la Sede Solicitante (`TransferenciaEntrada`), cambiando la solicitud a estado `Recibido` sin requerir confirmación manual posterior de Enfermería.
- `ReceivePedidoInterSedeCommand`: Al confirmar recepción desde la sede solicitante, incrementa el stock en `StocksSedes` para la sub-sede destino y registra `TransferenciaEntrada`.
- `GetPedidosInterSedeHistorialQuery`: Obtiene el historial completo de solicitudes filtrable por Sede y Rango de Fechas.
- `GetKardex`: Endpoint transaccional que computa Balance Inicial, Entradas, Salidas y Balance Final por Insumo/Sede entre Fechas.

---

## 4. Infraestructura & Auto-Generación SSL Interna en Docker (Nginx Alpine)

- **Entrypoint Autónomo (`docker-entrypoint.sh`)**: El contenedor Nginx (`sat-frontend`) en su rutina de arranque evalúa la presencia de certificados válidos en `/etc/nginx/ssl` (volumen montado `:ro`). Si no existen certificados provistos por el host, genera internamente mediante `openssl` llaves autofirmadas RSA 2048-bit válidas por 10 años (`CN=localhost`) en `/etc/ssl/certs/selfsigned.crt` y reconfigura `/etc/nginx/conf.d/default.conf` con `sed`.
- **Cero Intervención Manual**: Elimina la dependencia de scripts manuales de generación SSL en Windows, asegurando un arranque limpio con `docker-compose up -d --build frontend` sin errores `BIO_new_file() failed`.

---

## 5. UI/UX Cyber-Medical Glassmorphism

- Paleta HSL tailor-made `#0B0F19` con tarjetas glassmorphic `bg-surface-card/60 backdrop-blur-2xl`.
- Badges dinámicos de principios activos 🧬 en color Índigo (`bg-indigo-500/10 text-indigo-300`).
- Badges de Stock Central con discriminación de disponibilidad (Emerald/Rose).
- Tableros de auditoría de Kárdex y Requisiciones con discriminación por colores de estado.


