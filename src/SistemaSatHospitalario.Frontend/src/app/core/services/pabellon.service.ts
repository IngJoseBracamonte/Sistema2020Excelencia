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
  razonCirugia?: string;
  especialidad?: string;
  notasOperatorias?: string;
  requisitosQuirurgicos?: string;
  requisitos?: OrdenCirugiaRequisito[];
  historialObservaciones?: CirugiaObservacionHistorial[];
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

export interface OrdenCirugiaDetalle extends OrdenCirugia {
  logs: CirugiaLog[];
  insumosAsignados: InsumoCirugiaConsumo[];
}

export interface CrearOrdenCirugiaRequest {
  cuentaServicioId: string;
  pacienteId: string;
  descripcionCirugia: string;
  precioBaseUsd: number;
  medicoId: string;
  fechaHoraProgramada: string;
  razonCirugia?: string;
  especialidad?: string;
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

export interface DevolucionMasivaRequest {
  ordenCirugiaId: string;
  cuentaId: string;
  items: { insumoId: string; cantidadUsada: number }[];
}

export interface CargoExtraCirugiaRequest {
  ordenCirugiaId: string;
  cuentaServicioId: string;
  servicioId: string;
  descripcion: string;
  precio: number;
  honorario: number;
  cantidad: number;
  tipoServicio: string;
}

@Injectable({ providedIn: 'root' })
export class PabellonService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api/Pabellon`;

  // Reactive State mediante Angular Signals
  public cirugias = signal<OrdenCirugia[]>([]);
  public requisitosCatalogo = signal<RequisitoCirugia[]>([]);
  public loading = signal<boolean>(false);

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

  devolucionMasiva(req: DevolucionMasivaRequest): Observable<{ success: boolean }> {
    return this.http.post<{ success: boolean }>(`${this.apiUrl}/DevolucionMasiva`, req);
  }

  cargoExtra(req: CargoExtraCirugiaRequest): Observable<{ success: boolean }> {
    return this.http.post<{ success: boolean }>(`${this.apiUrl}/CargoExtra`, req);
  }

  getEstadosCirugia(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/Estados`).pipe(
      catchError(() => of([
        { id: 'Programada', codigo: 'PROG', nombre: 'Programada' },
        { id: 'EnEspera', codigo: 'ESPE', nombre: 'En Espera Pre-Quirúrgica' },
        { id: 'EnCirugia', codigo: 'PROC', nombre: 'En Cirugía / Quirófano' },
        { id: 'Finalizado', codigo: 'COMP', nombre: 'Finalizado / Recuperación' },
        { id: 'Cancelada', codigo: 'CANC', nombre: 'Cancelada' }
      ]))
    );
  }
}
