export abstract class BasePricedItem {
  id: string;
  codigo: string;
  descripcion: string;
  precio: number; // Bs para compatibilidad
  precioBs?: number;
  precioUsd?: number;
  honorarioUsd?: number;
  tipo: string;
  editorType?: string;
  categoryId: number; // V5.2 structural classification
  esLegacy: boolean;
  activo?: boolean;
  honorarioBase?: number;
  requiereInventario?: boolean;
  sugerenciasIds?: string[];
  honorariosMedicos?: { medicoId: string; medicoNombre?: string; honorario: number }[];

  // Campos generales de precio base
  precioBaseUsd?: number;

  // Campos quirúrgicos (CIRUGIA)
  complejidad?: string;
  duracionEstimadaMinutos?: number;
  requiereAnestesia?: boolean;
  tipoAnestesia?: string;
  clasificacionRiesgo?: string;
  notasPreoperatorias?: string;
  notasPostoperatorias?: string;
  protocoloQuirurgico?: string;
  indicaciones?: string;
  contraindicaciones?: string;
  equipoQuirurgico?: { id: string; nombre: string; codigo: string; cantidad: number }[];
  honorariosEquipo?: { rol: string; honorarioUsd: number }[];

  // Campos de Tomografía (TOMOGRAFIA)
  requiereContraste?: boolean;
  protocoloTecnico?: string;

  // Datos para Citas y Sincronización
  medicoId?: string;
  medicoNombre?: string;
  horaCita?: string;
  comentario?: string;
  detalleId?: string;
  hora?: string;

  // Getters delegados para compatibilidad de migración (PascalCase -> camelCase)
  get SugerenciasIds(): string[] | undefined { return this.sugerenciasIds; }
  get PrecioBs(): number | undefined { return this.precioBs; }
  get PrecioUsd(): number | undefined { return this.precioUsd; }
  get HonorarioUsd(): number | undefined { return this.honorarioUsd; }
  get HonorarioBase(): number | undefined { return this.honorarioBase; }
  get RequiereInventario(): boolean | undefined { return this.requiereInventario; }
  get PrecioBaseUsd(): number | undefined { return this.precioBaseUsd; }
  get Complejidad(): string | undefined { return this.complejidad; }
  get DuracionEstimadaMinutos(): number | undefined { return this.duracionEstimadaMinutos; }
  get RequiereAnestesia(): boolean | undefined { return this.requiereAnestesia; }
  get TipoAnestesia(): string | undefined { return this.tipoAnestesia; }
  get ClasificacionRiesgo(): string | undefined { return this.clasificacionRiesgo; }
  get NotasPreoperatorias(): string | undefined { return this.notasPreoperatorias; }
  get NotasPostoperatorias(): string | undefined { return this.notasPostoperatorias; }
  get ProtocoloQuirurgico(): string | undefined { return this.protocoloQuirurgico; }
  get Indicaciones(): string | undefined { return this.indicaciones; }
  get Contraindicaciones(): string | undefined { return this.contraindicaciones; }
  get EquipoQuirurgico(): any[] | undefined { return this.equipoQuirurgico; }
  get HonorariosEquipo(): any[] | undefined { return this.honorariosEquipo; }
  get RequiereContraste(): boolean | undefined { return this.requiereContraste; }
  get ProtocoloTecnico(): string | undefined { return this.protocoloTecnico; }

  constructor(data: any) {
    if (!data) data = {};
    this.id = data.id || data.Id || '';
    this.codigo = data.codigo || data.Codigo || '';
    this.descripcion = data.descripcion || data.Descripcion || '';
    this.precio = data.precio ?? data.Precio ?? 0;
    this.precioBs = data.precioBs ?? data.PrecioBs;
    this.precioUsd = data.precioUsd ?? data.PrecioUsd;
    this.tipo = data.tipo || data.Tipo || '';
    this.editorType = data.editorType || data.EditorType || data.tipo || data.Tipo || 'PROCEDIMIENTO';
    this.categoryId = data.categoryId ?? data.CategoryId ?? 0;
    this.esLegacy = data.esLegacy ?? data.EsLegacy ?? false;
    this.activo = data.activo ?? data.Activo ?? true;
    this.honorarioBase = data.honorarioBase ?? data.HonorarioBase ?? 0;
    this.requiereInventario = data.requiereInventario ?? data.RequiereInventario ?? false;
    this.sugerenciasIds = data.sugerenciasIds ?? data.SugerenciasIds ?? [];
    this.honorariosMedicos = data.honorariosMedicos || data.HonorariosMedicos || [];

    // Precio base USD (campo específico de servicios con precio base distinto al honorario)
    this.precioBaseUsd = data.precioBaseUsd ?? data.PrecioBaseUsd;

    // Campos quirúrgicos
    this.complejidad = data.complejidad ?? data.Complejidad;
    this.duracionEstimadaMinutos = data.duracionEstimadaMinutos ?? data.DuracionEstimadaMinutos;
    this.requiereAnestesia = data.requiereAnestesia ?? data.RequiereAnestesia;
    this.tipoAnestesia = data.tipoAnestesia ?? data.TipoAnestesia;
    this.clasificacionRiesgo = data.clasificacionRiesgo ?? data.ClasificacionRiesgo;
    this.notasPreoperatorias = data.notasPreoperatorias ?? data.NotasPreoperatorias;
    this.notasPostoperatorias = data.notasPostoperatorias ?? data.NotasPostoperatorias;
    this.protocoloQuirurgico = data.protocoloQuirurgico ?? data.ProtocoloQuirurgico;
    this.indicaciones = data.indicaciones ?? data.Indicaciones;
    this.contraindicaciones = data.contraindicaciones ?? data.Contraindicaciones;
    this.equipoQuirurgico = data.equipoQuirurgico ?? data.EquipoQuirurgico ?? [];
    this.honorariosEquipo = data.honorariosEquipo ?? data.HonorariosEquipo ?? [];

    // Campos de tomografía
    this.requiereContraste = data.requiereContraste ?? data.RequiereContraste;
    this.protocoloTecnico = data.protocoloTecnico ?? data.ProtocoloTecnico;

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
    const val = (this.precioUsd ?? 0) * tasa;
    const finalVal = val > 0 ? val : (this.precioBs ?? this.precio);
    return finalVal.toLocaleString('es-VE', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
  }

  /**
   * Obtiene el valor numérico en Bs basado en la tasa actual
   */
  getRawBs(tasa: number): number {
    const val = (this.precioUsd ?? 0) * tasa;
    return val > 0 ? val : (this.precioBs ?? this.precio);
  }

  get priceBs(): number {
    return this.precioBs ?? this.precio;
  }

  get displayPriceBs(): string {
     const val = this.precioBs ?? this.precio;
     return val.toLocaleString('es-VE', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
  }

  get displayPriceUsd(): string {
     const val = this.precioUsd ?? 0;
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
