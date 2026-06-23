export interface Insumo {
  id: string;
  codigo: string;
  nombre: string;
  stockActual: number;
  unidadMedidaBase: string;
  costoUnitarioBaseUSD: number;
}

export interface MovimientoInsumo {
  id: string;
  insumoId: string;
  insumo?: Insumo;
  tipoMovimiento: string; // 'Ingreso', 'Descarte', 'Consumo', 'AjusteCierre'
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
}

export interface UpdateInsumo {
  nombre: string;
  costoUnitarioBaseUSD: number;
}

export interface RecordMovement {
  insumoId: string;
  tipoMovimiento: string;
  cantidadOriginal: number;
  unidadMedidaOriginal: string;
  usuario?: string;
  motivo: string;
}

export interface PerformClosing {
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
