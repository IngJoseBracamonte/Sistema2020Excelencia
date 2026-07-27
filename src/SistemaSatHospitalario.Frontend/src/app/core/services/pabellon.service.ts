import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

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
}

export interface CambiarEstadoCirugiaRequest {
  ordenCirugiaId: string;
  nuevoEstado: string;
  motivoCancelacion?: string;
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

  getOrdenes(fechaInicio?: string, fechaFin?: string, estado?: string): Observable<OrdenCirugia[]> {
    let params = new HttpParams();
    if (fechaInicio) params = params.set('fechaInicio', fechaInicio);
    if (fechaFin) params = params.set('fechaFin', fechaFin);
    if (estado) params = params.set('estado', estado);
    return this.http.get<OrdenCirugia[]>(`${this.apiUrl}/Ordenes`, { params });
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

  devolucionMasiva(req: DevolucionMasivaRequest): Observable<{ success: boolean }> {
    return this.http.post<{ success: boolean }>(`${this.apiUrl}/DevolucionMasiva`, req);
  }

  cargoExtra(req: CargoExtraCirugiaRequest): Observable<{ success: boolean }> {
    return this.http.post<{ success: boolean }>(`${this.apiUrl}/CargoExtra`, req);
  }
}
