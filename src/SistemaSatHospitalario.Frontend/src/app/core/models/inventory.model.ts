export interface CategoriaInsumo {
  id: string;
  nombre: string;
  codigo?: string;
  activo: boolean;
  fechaCreacion?: string;
}

export interface PrincipioActivo {
  id: string;
  nombre: string;
  activo: boolean;
}

export interface InsumoPrincipioActivo {
  id: string;
  insumoId: string;
  principioActivoId: string;
  nombre?: string;
  concentracion: string;
}

export interface Insumo {
  id: string;
  codigo: string;
  nombre: string;
  stockActual: number;
  unidadMedidaBase: string;
  costoUnitarioBaseUSD: number;
  permiteFraccionamiento?: boolean;
  categoria?: string;
  isDeleted?: boolean;
  fechaInactivacion?: string;
  ocultoEnTraslados?: boolean;
  principiosActivos?: InsumoPrincipioActivo[];
  reactivosCombinados?: string;
  indicaciones?: string;
  fechaVencimiento?: string;
}

export type TipoMovimientoInsumo = 
  | 'Ingreso'             // Compra a Proveedor -> Almacén Principal
  | 'Transferencia'       // Almacén Principal -> Emergencia / Hospitalización (Crea Stock Local)
  | 'ConsumoInterno'      // Almacén Principal -> Laboratorio (Nota de Entrega / Descarte Inmediato)
  | 'DespachoQuirurgico'  // Almacén Principal -> Quirófano (Tránsito / Kit)
  | 'DevolucionQuirurgica'// Quirófano -> Almacén Principal (Reingreso)
  | 'Descarte'            // Deterioro o Merma
  | 'AjusteCierre';       // Auditoría Física

export interface MovimientoInsumo {
  id: string;
  insumoId: string;
  insumo?: Insumo;
  sedeId: string;
  tipoMovimiento: TipoMovimientoInsumo | string;
  cantidadBase: number;
  unidadMedidaOriginal: string;
  cantidadOriginal: number;
  usuario: string;
  fecha: string;
  motivo: string;
}

export interface CierreDetalleInput {
  insumoId: string;
  stockReal: number;
}

export interface CierreInventario {
  id: string;
  fechaCierre: string;
  usuario: string;
  observaciones: string;
  detalles?: CierreInventarioDetalle[];
}

export interface CierreInventarioDetalle {
  id: string;
  cierreInventarioId: string;
  insumoId: string;
  insumo?: Insumo;
  stockTeoricoBase: number;
  stockRealBase: number;
  costoBaseUSD: number;
}

export interface ServicioInsumoReceta {
  id: string;
  servicioClinicoId: string;
  servicioCodigo: string;
  servicioClinico?: any;
  insumoId: string;
  insumo?: Insumo;
  cantidad: number;
  unidadMedidaConsumo: string;
}

export interface CreateInsumo {
  codigo: string;
  nombre: string;
  stockInicial: number;
  unidadMedidaBase: string;
  costoUnitarioBaseUSD: number;
  permiteFraccionamiento?: boolean;
  categoria?: string;
  reactivosCombinados?: string;
  indicaciones?: string;
  fechaVencimiento?: string;
}

export interface UpdateInsumo {
  nombre: string;
  unidadMedidaBase?: string;
  costoUnitarioBaseUSD: number;
  permiteFraccionamiento?: boolean;
  categoria?: string;
  reactivosCombinados?: string;
  indicaciones?: string;
  fechaVencimiento?: string;
}

export interface RecordMovement {
  insumoId: string;
  sedeId: string;
  tipoMovimiento: string;
  cantidadOriginal: number;
  unidadMedidaOriginal: string;
  usuario?: string;
  motivo: string;
}

export interface DescarteRequest {
  insumoId: string;
  sedeId?: string;
  cantidad: number;
  motivo: string;
}

export interface PerformClosing {
  sedeId: string;
  usuario?: string;
  observaciones: string;
  detalles: CierreDetalleInput[];
}

export interface CreateRecipe {
  servicioClinicoId: string;
  insumoId: string;
  cantidad: number;
  unidadMedidaConsumo: string;
}

export interface Proveedor {
  id: string;
  rif: string;
  razonSocial: string;
  direccion?: string;
  telefono?: string;
  activo?: boolean;
  fechaRegistro?: string;
}

export interface PurchaseItem {
  insumoId: string;
  cantidad: number;
  precioCostoUSD: number;
  presentacionCompra?: string;
}

export interface RecordPurchase {
  sedeId?: string;
  proveedorId?: string;
  proveedorNombre?: string;
  numeroFactura?: string;
  tasaCambio?: number;
  items: PurchaseItem[];
}

export interface TransferenciaReposicionItem {
  id: string;
  insumoId: string;
  insumoNombre: string;
  insumoCodigo: string;
  sedeOrigenId: string;
  sedeOrigenNombre: string;
  sedeDestinoId: string;
  sedeDestinoNombre: string;
  cantidad: number;
  motivo: string;
  fechaTransferencia: string;
  usuarioId: string;
  observaciones?: string;
}

export interface ProcesarReposicionRequest {
  insumoId: string;
  sedeOrigenId: string;
  sedeDestinoId: string;
  cantidad: number;
  motivo?: string;
  observaciones?: string;
}

