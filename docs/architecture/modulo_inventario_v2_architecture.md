# Memoria de Arquitectura — Módulo de Inventario v2 (Motor Transaccional Puro)

**Fecha de Implementación**: 2026-07-28  
**Stack de Tecnología**: .NET 9 WebAPI (CQRS MediatR) + Angular 19+ (Standalone Components + Signals)

---

## 1. Principios de Diseño & Invariantes

- **Motor Transaccional Puro**: Se eliminaron completamente las fechas de vencimiento, gestión de lotes, gráficos BI, dashboards de métricas y placeholders/fotografías de productos.
- **Persistencia Monetaria**: Operaciones base persistidas exclusivamente en Dólares ($ USD).
- **Control Flow & Navegación Pure**: Toda la interacción ocurre mediante el `router-outlet` del layout principal sin desplegar modales primarios de edición.
- **Relación N:M Principios Activos**: Cada insumo o medicamento puede asociarse con N principios activos indicando su concentración individual (ej: Ibuprofeno 400mg + Clorfeniramina 4mg).
- **Borrado Lógico (Soft Delete)**: `Insumo.IsDeleted = true` y `FechaInactivacion` para proteger la integridad referencial histórica en facturación y recetas.

---

## 2. Segregación de Rutas (Sidebar)

El Sidebar simplifica la navegación bajo la categoría **Inventario**:

| Ruta Frontend | Componente | Descripción / Responsabilidad |
|---|---|---|
| `/inventario/compras` | `ComprasComponent` | Registro e ingreso de stock central al Almacén Principal sin vencimiento. |
| `/inventario/pedidos` | `PedidosAprobacionComponent` | Panel del Supervisor para evaluar requisiciones. Permite despacho parcial con **observación obligatoria por ajuste**. |
| `/inventario/catalogo` | `CatalogoComponent` | CRUD de Insumos y gestión de la relación N:M de Principios Activos. |
| `/inventario/descarte` | `DescarteComponent` | Bajas manuales de stock por merma o deterioro con justificación requerida para auditoría. |

---

## 3. Especificación CQRS Backend (.NET 9)

- `RegistrarCompraCommand`: Incrementa stock atómicamente en Sede Principal y actualiza costo unitario USD.
- `RegistrarDescarteCommand`: Valida stock central, aplica descuento directo y registra movimiento tipo `Descarte` con motivo de auditoría.
- `DispatchPedidoInterSedeCommand`: Admite `CantidadesAprobadas` y `ObservacionesPorDetalle`. Valida que si `CantidadAEnviar < CantidadSolicitada`, la observación por detalle es requerida.

---

## 4. UI/UX Cyber-Medical Glassmorphism

- Paleta HSL tailor-made `#0B0F19` con tarjetas glassmorphic `bg-surface-card/60 backdrop-blur-2xl`.
- Badges dinámicos de principios activos 🧬 en color Índigo (`bg-indigo-500/10 text-indigo-300`).
- Badges de Stock Central con discriminación de disponibilidad (Emerald/Rose).
