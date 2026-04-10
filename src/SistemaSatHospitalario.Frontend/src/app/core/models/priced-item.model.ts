export abstract class BasePricedItem {
  id: string;
  codigo: string;
  descripcion: string;
  precio: number; // Bs para compatibilidad
  precioBs?: number;
  precioUsd?: number;
  PrecioBs?: number; // PascalCase del backend (mapeo seguro)
  PrecioUsd?: number; // PascalCase del backend (mapeo seguro)
  honorarioUsd?: number;
  HonorarioUsd?: number;
  tipo: string;
  categoryId: number; // V5.2 structural classification
  esLegacy: boolean;
  activo?: boolean;

  constructor(data: any) {
    this.id = data.id || data.Id;
    this.codigo = data.codigo || data.Codigo;
    this.descripcion = data.descripcion || data.Descripcion;
    this.precio = data.precio || data.Precio || 0;
    this.precioBs = data.precioBs ?? data.PrecioBs;
    this.precioUsd = data.precioUsd ?? data.PrecioUsd;
    this.honorarioUsd = data.honorarioUsd ?? data.HonorarioUsd;
    this.tipo = data.tipo || data.Tipo;
    this.categoryId = data.categoryId ?? data.CategoryId ?? 0;
    this.esLegacy = data.esLegacy ?? data.EsLegacy ?? false;
    this.activo = data.activo ?? data.Activo;
  }

  /**
   * Obtiene el precio formateado en Bs basado en la tasa actual
   */
  getFormattedBs(tasa: number): string {
    const val = (this.precioUsd ?? this.PrecioUsd ?? 0) * tasa;
    const finalVal = val > 0 ? val : (this.precioBs ?? this.PrecioBs ?? this.precio);
    return finalVal.toLocaleString('es-VE', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
  }

  /**
   * Obtiene el valor numérico en Bs basado en la tasa actual
   */
  getRawBs(tasa: number): number {
    const val = (this.precioUsd ?? this.PrecioUsd ?? 0) * tasa;
    return val > 0 ? val : (this.precioBs ?? this.PrecioBs ?? this.precio);
  }

  get priceBs(): number {
    return this.precioBs ?? this.PrecioBs ?? this.precio;
  }

  get displayPriceBs(): string {
     const val = this.precioBs ?? this.PrecioBs ?? this.precio;
     return val.toLocaleString('es-VE', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
  }

  get displayPriceUsd(): string {
     const val = this.precioUsd ?? this.PrecioUsd ?? 0;
     return val.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
  }

  get isConsultation(): boolean {
    if (this.categoryId === 1) return true; // ServiceCategory.Consultation
    if (!this.tipo) return false;
    const t = this.tipo.toUpperCase();
    const prefixes = ['CONS', 'MEDI', 'MÉDI', 'OBST', 'GINE'];
    return prefixes.some(p => t.includes(p));
  }
}

export class CatalogItem extends BasePricedItem {
  constructor(data: any) {
    super(data);
  }
}
