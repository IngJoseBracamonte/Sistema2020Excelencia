export abstract class BasePricedItem {
  id: string;
  codigo: string;
  descripcion: string;
  precio: number; // Bs para compatibilidad
  precioBs?: number;
  precioUsd?: number;
  PrecioBs?: number; // PascalCase del backend (mapeo seguro)
  PrecioUsd?: number; // PascalCase del backend (mapeo seguro)
  tipo: string;
  esLegacy: boolean;
  activo?: boolean;

  constructor(data: any) {
    this.id = data.id || data.Id;
    this.codigo = data.codigo || data.Codigo;
    this.descripcion = data.descripcion || data.Descripcion;
    this.precio = data.precio || data.Precio || 0;
    this.precioBs = data.precioBs ?? data.PrecioBs;
    this.precioUsd = data.precioUsd ?? data.PrecioUsd;
    this.tipo = data.tipo || data.Tipo;
    this.esLegacy = data.esLegacy ?? data.EsLegacy ?? false;
    this.activo = data.activo ?? data.Activo;
  }

  /**
   * Calcula el precio en Bs basado en una tasa local si no viene del servidor
   */
  getCalculatedBs(tasa: number): number {
    return (this.precioUsd ?? this.PrecioUsd ?? 0) * tasa;
  }

  get displayPriceBs(): string {
     const val = this.precioBs ?? this.PrecioBs ?? this.precio;
     return val.toLocaleString('es-VE', { minimumFractionDigits: 2 });
  }

  get displayPriceUsd(): string {
     const val = this.precioUsd ?? this.PrecioUsd ?? 0;
     return val.toLocaleString('en-US', { minimumFractionDigits: 2 });
  }
}

export class CatalogItem extends BasePricedItem {
  constructor(data: any) {
    super(data);
  }
}
