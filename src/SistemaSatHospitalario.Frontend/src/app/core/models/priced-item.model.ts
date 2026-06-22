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
  honorarioBase?: number;
  HonorarioBase?: number;
  sugerenciasIds?: string[];
  SugerenciasIds?: string[];
  honorariosMedicos?: { medicoId: string; medicoNombre?: string; honorario: number }[];

  // Datos para Citas y Sincronización
  medicoId?: string;
  medicoNombre?: string;
  horaCita?: string;
  comentario?: string;
  detalleId?: string;
  hora?: string;

  constructor(data: any) {
    this.id = data.id || data.Id;
    this.codigo = data.codigo || data.Codigo;
    this.descripcion = data.descripcion || data.Descripcion;
    this.precio = data.precio || data.Precio || 0;
    this.precioBs = data.precioBs ?? data.PrecioBs;
    this.precioUsd = data.precioUsd ?? data.PrecioUsd;
    this.tipo = data.tipo || data.Tipo;
    this.categoryId = data.categoryId ?? data.CategoryId ?? 0;
    this.esLegacy = data.esLegacy ?? data.EsLegacy ?? false;
    this.activo = data.activo ?? data.Activo;
    this.honorarioBase = data.honorarioBase ?? data.HonorarioBase ?? 0;
    this.sugerenciasIds = data.sugerenciasIds ?? data.SugerenciasIds ?? [];
    this.honorariosMedicos = data.honorariosMedicos || data.HonorariosMedicos || [];

    this.medicoId = data.medicoId ?? data.MedicoId;
    this.medicoNombre = data.medicoNombre ?? data.MedicoNombre;
    this.horaCita = data.horaCita ?? data.HoraCita;
    this.comentario = data.comentario ?? data.Comentario;
    this.detalleId = data.detalleId ?? data.DetalleId;
    this.hora = data.hora ?? data.Hora;

    const priceVal = this.precioUsd ?? 0;
    if (data.honorarioUsd !== undefined && data.honorarioUsd !== null) {
      this.honorarioUsd = data.honorarioUsd;
    } else if (data.HonorarioUsd !== undefined && data.HonorarioUsd !== null) {
      this.honorarioUsd = data.HonorarioUsd;
    } else {
      const baseHonorary = this.honorarioBase ?? 0;
      if (this.isConsultation) {
        this.honorarioUsd = baseHonorary;
        this.precioUsd = baseHonorary + priceVal;
        this.precio = baseHonorary + priceVal;
      } else {
        this.honorarioUsd = baseHonorary;
      }
    }
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
    if (t.includes('MEDICINA') || t.includes('MEDICAMENTO') || t.includes('INSUMO')) return false;
    const prefixes = ['CONS', 'MEDI', 'MÉDI', 'OBST', 'GINE'];
    return prefixes.some(p => t.includes(p));
  }
}

export class CatalogItem extends BasePricedItem {
  constructor(data: any) {
    super(data);
  }
}
