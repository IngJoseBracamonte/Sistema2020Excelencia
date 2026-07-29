# Planificación del Módulo de Inventario v2 — Motor Transaccional Puro

Este documento sirve como la especifición actualizada y arquitectura de referencia para el módulo de Inventario v2 en SistemaSatHospitalario.

---

## 1. Alcance Optimizado del Módulo

Se han eliminado completamente:
- Fechas de vencimiento.
- Lotes / caducidad.
- Gráficos, métricas tipo BI y dashboards redundantes.
- Fotos / imágenes de ítems.
- Modales principales para formularios (Toda navegación ocurre vía `router-outlet`).

---

## 2. Estructura de Navegación Sidebar

Grupo **Inventario**:
1. `[route: /inventario/compras]` — Compras & Entradas: Registro atómico de stock central y precio costo USD.
2. `[route: /inventario/pedidos]` — Aprobación de Pedidos: Vista del supervisor para aprobar/despachar requisiciones con regla de observación obligatoria por ajuste.
3. `[route: /inventario/catalogo]` — Catálogo & Principios Activos (CRUD): Administración de productos, Soft Delete (`IsDeleted = true`) y vinculación N:M de Principios Activos.
4. `[route: /inventario/descarte]` — Descarte & Bajas: Disminución manual por merma con justificación/motivo de auditoría obligatorio.

---

## 3. Entidades de Dominio

### `Insumo`
- Propiedades: `Id`, `Codigo`, `Nombre`, `UnidadMedidaBase`, `CostoUnitarioBaseUSD`, `PermiteFraccionamiento`, `Categoria`, `IsDeleted`, `FechaInactivacion`, `OcultoEnTraslados`.
- Colección N:M: `PrincipiosActivos` (`ICollection<InsumoPrincipioActivo>`).

### `PrincipioActivo`
- Propiedades: `Id`, `Nombre`, `Activo`.

### `InsumoPrincipioActivo` (Tabla Puente N:M)
- Propiedades: `Id`, `InsumoId`, `PrincipioActivoId`, `Concentracion` (Ej: "400mg", "4mg/5ml").

---

## 4. Reglas de Negocio Estrictas

1. **Unidireccionalidad**: Toda reposición de stock proviene del Almacén Central (Sede Principal).
2. **Despacho Parcial o Total**: Si la `Cantidad a Enviar` < `Cantidad Solicitada`, la `ObservacionDespacho` es **OBLIGATORIA** para auditoría por ítem. Si coinciden, la observación permanece opcional.
3. **Descarte Manual**: Requiere motivo explicativo obligatorio.
4. **Soft Delete**: Insumos inhabilitados pasan a `IsDeleted = true` conservando historial e integridad transaccional.
