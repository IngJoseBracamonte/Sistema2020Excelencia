import { Injectable, inject, signal } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, of, catchError, tap } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface RequisitoCirugia {
  id: string;
  nombre: string;
  descripcion: string;
  esActivo: boolean;
}

export interface OrdenCirugiaRequisito {
  id: string;
  requisitoCirugiaId: string;
  nombre: string;
  descripcion: string;
  cumplido: boolean;
  fechaVerificacion?: string;
  verificadoPor?: string;
}

export interface CirugiaObservacionHistorial {
  id: string;
  observacion: string;
  tipo: string;
  fechaRegistro: string;
  usuarioRegistro: string;
}

export interface CirugiaLog {
  id: string;
  ordenCirugiaId: string;
  usuarioId: string;
  evento: string;
  detalle: string;
  timestamp: string;
}

export interface InsumoCirugiaConsumo {
  insumoId: string;
  insumoNombre: string;
  insumoCodigo: string;
  cantidadEntregada: number;
  cantidadDevuelta: number;
  cantidadConsumida: number;
  precioUnitarioUsd: number;
}

export interface MedicoHonorarioItem {
  id: string;
  medicoId: string;
  medicoNombre: string;
  especialidadId: string;
  especialidadNombre: string;
  montoHonorarioUsd: number;
  esCirujanoPrincipal: boolean;
}

export interface SolicitudInsumoDetalle {
  id: string;
  insumoId: string;
  insumoNombre: string;
  insumoCodigo: string;
  cantidadSolicitada: number;
  estadoSolicitud: string;
  fechaSolicitud: string;
  usuarioSolicitud: string;
}

export interface UbicacionPaciente {
  areaClinicaId?: string;
  areaClinicaNombre: string;
  camaId?: string;
  camaNombre: string;
  camaCodigo: string;
  descripcionCompleta: string;
}

export interface TipoIngresoCobertura {
  tipo: string;
  convenioId?: number;
  convenioNombre?: string;
  esAsegurado: boolean;
}

export interface CirugiaCalendarioItem {
  id: string;
  cuentaServicioId: string;
  pacienteId: string;
  pacienteNombre: string;
  pacienteCedula: string;
  ubicacion: UbicacionPaciente;
  ingresoCobertura: TipoIngresoCobertura;
  descripcionCirugia: string;
  salaQuirofano: string;
  modalidadAnestesia: string;
  esAlquilado: boolean;
  precioDerechoSalaUsd: number;
  precioBaseUsd: number;
  medicoId: string;
  medicoNombre: string;
  fechaHoraProgramada: string;
  estado: string;
  totalRequisitos: number;
  requisitosCumplidos: number;
  estaAptoParaQuirofano: boolean;
}

export interface PacienteQuirurgicoItem {
  id: string;
  cuentaServicioId: string;
  pacienteId: string;
  pacienteNombre: string;
  pacienteCedula: string;
  ubicacion: UbicacionPaciente;
  ingresoCobertura: TipoIngresoCobertura;
  descripcionCirugia: string;
  salaQuirofano: string;
  modalidadAnestesia: string;
  esAlquilado: boolean;
  precioDerechoSalaUsd: number;
  precioBaseUsd: number;
  medicoPrincipalId: string;
  medicoPrincipalNombre: string;
  fechaHoraProgramada: string;
  estado: string;
  fechaCreacion: string;
  usuarioCreacion: string;
  requisitos: OrdenCirugiaRequisito[];
  medicosHonorarios: MedicoHonorarioItem[];
  insumosAsignados: InsumoCirugiaConsumo[];
  solicitudesInsumos: SolicitudInsumoDetalle[];
  logs: CirugiaLog[];
}

export interface OrdenCirugia {
  id: string;
  cuentaServicioId: string;
  pacienteId: string;
  pacienteNombre: string;
  pacienteCedula: string;
  descripcionCirugia: string;
  precioBaseUsd: number;
  medicoId: string;
  medicoNombre: string;
  fechaHoraProgramada: string;
  estado: string;
  motivoCancelacion?: string;
  fechaCreacion: string;
  usuarioCreacion: string;
  salaQuirofano?: string;
  modalidadAnestesia?: string;
  esAlquilado?: boolean;
  precioDerechoSalaUsd?: number;
  requisitos?: OrdenCirugiaRequisito[];
  historialObservaciones?: CirugiaObservacionHistorial[];
}

export interface OrdenCirugiaDetalle extends OrdenCirugia {
  logs: CirugiaLog[];
  insumosAsignados: InsumoCirugiaConsumo[];
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

export interface CrearOrdenCirugiaRequest {
  cuentaServicioId: string;
  pacienteId: string;
  descripcionCirugia: string;
  precioBaseUsd: number;
  medicoId: string;
  fechaHoraProgramada: string;
  salaQuirofano?: string;
  modalidadAnestesia?: string;
  esAlquilado?: boolean;
  precioDerechoSalaUsd?: number;
}

export interface CambiarEstadoCirugiaRequest {
  ordenCirugiaId: string;
  nuevoEstado: string;
  motivoCancelacion?: string;
}

export interface ReprogramarCirugiaRequest {
  ordenCirugiaId: string;
  nuevaFechaHora: string;
  motivo: string;
}

export interface AsignarKitRequest {
  ordenCirugiaId: string;
  cuentaServicioId: string;
  sedeDespachoId?: string;
  insumosPersonalizados: { insumoId: string; cantidad: number }[];
}

export interface DevolucionInsumoRequest {
  cuentaServicioId: string;
  insumoId: string;
  cantidadDevuelta: number;
  sedeReingresoId?: string;
  motivo?: string;
}

export interface SolicitarInsumoExtraRequest {
  ordenCirugiaId: string;
  insumoId: string;
  cantidad: number;
  almacenOrigenId?: string;
  observaciones?: string;
}

export interface ActualizarHonorariosYPreciosRequest {
  ordenCirugiaId: string;
  precioDerechoSalaUsd: number;
  precioBaseUsd: number;
  esAlquilado: boolean;
  medicos: {
    medicoId: string;
    especialidadId?: string;
    montoHonorarioUsd: number;
    esCirujanoPrincipal: boolean;
  }[];
}

@Injectable({ providedIn: 'root' })
export class PabellonService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api/Pabellon`;
  private inventarioUrl = `${environment.apiUrl}/api/Inventario/Reposicion`;

  // Reactive State mediante Signals
  public calendarioItems = signal<CirugiaCalendarioItem[]>([]);
  public pacientesQuirurgicos = signal<PacienteQuirurgicoItem[]>([]);
  public cirugias = signal<OrdenCirugia[]>([]);
  public requisitosCatalogo = signal<RequisitoCirugia[]>([]);
  public reposicionesHistorial = signal<TransferenciaReposicionItem[]>([]);
  public loading = signal<boolean>(false);

  getCalendario(fechaInicio?: string, fechaFin?: string, salaQuirofano?: string, estado?: string): Observable<CirugiaCalendarioItem[]> {
    this.loading.set(true);
    let params = new HttpParams();
    if (fechaInicio) params = params.set('fechaInicio', fechaInicio);
    if (fechaFin) params = params.set('fechaFin', fechaFin);
    if (salaQuirofano) params = params.set('salaQuirofano', salaQuirofano);
    if (estado) params = params.set('estado', estado);

    return this.http.get<CirugiaCalendarioItem[]>(`${this.apiUrl}/Calendario`, { params }).pipe(
      tap(items => {
        this.calendarioItems.set(items || []);
        this.loading.set(false);
      }),
      catchError(err => {
        this.loading.set(false);
        throw err;
      })
    );
  }

  getPacientesQuirurgicos(busqueda?: string, estado?: string): Observable<PacienteQuirurgicoItem[]> {
    this.loading.set(true);
    let params = new HttpParams();
    if (busqueda) params = params.set('busqueda', busqueda);
    if (estado) params = params.set('estado', estado);

    return this.http.get<PacienteQuirurgicoItem[]>(`${this.apiUrl}/Pacientes`, { params }).pipe(
      tap(pacientes => {
        this.pacientesQuirurgicos.set(pacientes || []);
        this.loading.set(false);
      }),
      catchError(err => {
        this.loading.set(false);
        throw err;
      })
    );
  }

  getOrdenes(fechaInicio?: string, fechaFin?: string, estado?: string): Observable<OrdenCirugia[]> {
    this.loading.set(true);
    let params = new HttpParams();
    if (fechaInicio) params = params.set('fechaInicio', fechaInicio);
    if (fechaFin) params = params.set('fechaFin', fechaFin);
    if (estado) params = params.set('estado', estado);

    return this.http.get<OrdenCirugia[]>(`${this.apiUrl}/Ordenes`, { params }).pipe(
      tap(data => {
        this.cirugias.set(data || []);
        this.loading.set(false);
      }),
      catchError(err => {
        this.loading.set(false);
        throw err;
      })
    );
  }

  getOrdenDetalle(id: string): Observable<OrdenCirugiaDetalle> {
    return this.http.get<OrdenCirugiaDetalle>(`${this.apiUrl}/Ordenes/${id}`);
  }

  crearOrden(req: CrearOrdenCirugiaRequest): Observable<{ id: string }> {
    return this.http.post<{ id: string }>(`${this.apiUrl}/Ordenes`, req);
  }

  cambiarEstado(req: CambiarEstadoCirugiaRequest): Observable<{ success: boolean }> {
    return this.http.post<{ success: boolean }>(`${this.apiUrl}/Ordenes/Estado`, req);
  }

  reprogramarCirugia(req: ReprogramarCirugiaRequest): Observable<{ success: boolean }> {
    return this.http.post<{ success: boolean }>(`${this.apiUrl}/Reprogramar`, req);
  }

  asignarKit(req: AsignarKitRequest): Observable<{ success: boolean }> {
    return this.http.post<{ success: boolean }>(`${this.apiUrl}/AsignarKit`, req);
  }

  devolucionInsumo(req: DevolucionInsumoRequest): Observable<{ success: boolean }> {
    return this.http.post<{ success: boolean }>(`${this.apiUrl}/DevolucionInsumo`, req);
  }

  solicitarInsumoExtra(req: SolicitarInsumoExtraRequest): Observable<{ id: string }> {
    return this.http.post<{ id: string }>(`${this.apiUrl}/SolicitarInsumoExtra`, req);
  }

  despacharSolicitudExtra(solicitudId: string): Observable<{ success: boolean }> {
    return this.http.post<{ success: boolean }>(`${this.apiUrl}/DespacharSolicitudExtra`, { solicitudId });
  }

  actualizarHonorariosYPrecios(req: ActualizarHonorariosYPreciosRequest): Observable<{ success: boolean }> {
    return this.http.post<{ success: boolean }>(`${this.apiUrl}/ActualizarHonorariosYPrecios`, req);
  }

  getRequisitosCatalogo(soloActivos: boolean = true): Observable<RequisitoCirugia[]> {
    return this.http.get<RequisitoCirugia[]>(`${this.apiUrl}/RequisitosCatalogo`, {
      params: new HttpParams().set('soloActivos', soloActivos)
    }).pipe(
      tap(reqs => this.requisitosCatalogo.set(reqs || []))
    );
  }

  crearRequisitoCatalogo(nombre: string, descripcion: string): Observable<{ id: string }> {
    return this.http.post<{ id: string }>(`${this.apiUrl}/RequisitosCatalogo`, { nombre, descripcion, esActivo: true });
  }

  toggleRequisito(ordenId: string, requisitoId: string, cumplido: boolean): Observable<{ success: boolean }> {
    return this.http.patch<{ success: boolean }>(`${this.apiUrl}/Ordenes/${ordenId}/Requisitos/${requisitoId}`, { cumplido });
  }

  devolucionMasiva(payload: { ordenCirugiaId: string; cuentaId?: string; items: { insumoId: string; cantidadUsada: number }[] }): Observable<{ success: boolean }> {
    return this.http.post<{ success: boolean }>(`${this.apiUrl}/DevolucionMasiva`, payload);
  }

  cargoExtra(payload: any): Observable<{ success: boolean }> {
    return this.http.post<{ success: boolean }>(`${this.apiUrl}/CargoExtra`, payload);
  }

  // --- Supervisor de Inventario: Reposición e Intercambio de Insumos ---
  procesarReposicion(req: {
    insumoId: string;
    sedeOrigenId: string;
    sedeDestinoId: string;
    cantidad: number;
    motivo: string;
    observaciones?: string;
  }): Observable<{ success: boolean }> {
    return this.http.post<{ success: boolean }>(this.inventarioUrl, req);
  }

  getReposicionesHistorial(params?: {
    sedeId?: string;
    insumoId?: string;
    fechaDesde?: string;
    fechaHasta?: string;
    motivo?: string;
  }): Observable<TransferenciaReposicionItem[]> {
    let httpParams = new HttpParams();
    if (params?.sedeId) httpParams = httpParams.set('sedeId', params.sedeId);
    if (params?.insumoId) httpParams = httpParams.set('insumoId', params.insumoId);
    if (params?.fechaDesde) httpParams = httpParams.set('fechaDesde', params.fechaDesde);
    if (params?.fechaHasta) httpParams = httpParams.set('fechaHasta', params.fechaHasta);
    if (params?.motivo) httpParams = httpParams.set('motivo', params.motivo);

    return this.http.get<TransferenciaReposicionItem[]>(`${this.inventarioUrl}/Historial`, { params: httpParams }).pipe(
      tap(items => this.reposicionesHistorial.set(items || []))
    );
  }

  getEstadosCirugia(): Observable<any[]> {
    return of([
      { id: 'Programada', codigo: 'PROG', nombre: 'Programada' },
      { id: 'EnEspera', codigo: 'ESPE', nombre: 'En Espera Pre-Quirúrgica' },
      { id: 'EnCirugia', codigo: 'PROC', nombre: 'En Cirugía / Quirófano' },
      { id: 'Finalizado', codigo: 'COMP', nombre: 'Finalizado / Recuperación' },
      { id: 'Cancelada', codigo: 'CANC', nombre: 'Cancelada' }
    ]);
  }
}
