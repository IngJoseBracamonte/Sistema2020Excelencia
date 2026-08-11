export interface PagoProveedor {
  id: string;
  ordenCompraId: string;
  numeroFactura: string;
  proveedorNombre: string;
  fechaPago: string;
  montoAbonadoUSD: number;
  tasaCambio: number;
  montoAbonadoBs: number;
  metodoPago: string;
  referencia: string;
  usuarioId: string;
  observaciones?: string;
}

export interface OrdenCompraInventario {
  id: string;
  numeroFactura: string;
  proveedorId?: string;
  proveedorNombre: string;
  fechaEmision: string;
  montoTotalUSD: number;
  montoTotalBs: number;
  totalAbonadoUSD: number;
  saldoPendienteUSD: number;
  estado: 'PorPagar' | 'Pagado' | string;
  observaciones?: string;
  pagos: PagoProveedor[];
}

export interface RegistrarPagoRequest {
  ordenCompraId: string;
  montoAbonadoUSD: number;
  tasaCambio: number;
  metodoPago: string;
  referencia: string;
  observaciones?: string;
}

export interface CatalogoMetodoPagoDto {
  id: string;
  nombre: string;
  monedaId: number; // 1 = USD, 2 = VES / Bs
  activo: boolean;
}

export const MONEDA_USD_ID = 1;
export const MONEDA_BS_ID = 2;
