import { Injectable, inject, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { tap, shareReplay } from 'rxjs/operators';
import { environment } from '../../../environments/environment';

export interface Sede {
  id: string;
  codigo: string;
  nombre: string;
  esPrincipal: boolean;
  activo: boolean;
  areasClinicas?: AreaClinica[];
}

export enum EstadoPedidoInterSede {
  Solicitado = 'Solicitado',
  Aprobado = 'Aprobado',
  Despachado = 'Despachado',
  Recibido = 'Recibido',
  Rechazado = 'Rechazado',
  Cancelado = 'Cancelado',
  Pendiente = 'Pendiente'
}
export interface AreaClinica {
  id: string;
  nombre: string;
  codigo: string;
  sedeId?: string;
  sedeNombre?: string;
  clasificacionId?: string;
  activo?: boolean; // Se agrega para resolver el error en cierre-cuenta
  esSubAreaAlmacenPrincipal?: boolean;
  areaPadreId?: string | null;
  servicioTarifaBaseId?: string | null;
  tarifaBasePrecioUsd?: number;
  tarifaBaseHonorarioUsd?: number;
}

export interface PedidoInterSede {
  id: string;
  correlativo: string;
  sedeSolicitanteId: string;
  sedeSolicitanteNombre: string;
  sedeProveedoraId: string;
  sedeProveedoraNombre: string;
  estado: string;
  fechaCreacion: string;
  fechaDespacho?: string;
  fechaRecepcion?: string;
  usuarioCreador: string;
  usuarioSolicitante?: string;
  usuarioAprobador?: string;
  observaciones: string;
  detalles: PedidoInterSedeDetalle[];
}

export interface PedidoInterSedeDetalle {
  id: string;
  insumoId: string;
  insumoNombre: string;
  insumoCodigo: string;
  cantidadSolicitada: number;
  cantidadDespachada: number;
  cantidadRecibida: number;
  stockDisponibleSedeProveedora?: number;
  observacionesSupervisor?: string;
}

export interface CreatePedidoInterSedeDto {
  sedeSolicitanteId: string;
  sedeProveedoraId: string;
  observaciones: string;
  lineas: { insumoId: string; cantidadSolicitada: number }[];
}
export interface SedeDto {
  id: string;
  nombre: string;
  codigo: string;
  activo: boolean;
}
@Injectable({ providedIn: 'root' })
export class MultiSedeService {
  private http = inject(HttpClient);

  private sedesCache$: Observable<Sede[]> | null = null;
  private areasCache = new Map<string, Observable<AreaClinica[]>>();

  // Sede Contexto Activo (para filtrados de stock reactivos)
  private activeSedeSignal = signal<Sede | null>(null);
  public activeSede = computed(() => this.activeSedeSignal());

  clearCache() {
    this.sedesCache$ = null;
    this.areasCache.clear();
  }

  setSedeActiva(sede: Sede | null) {
    this.activeSedeSignal.set(sede);
    if (sede) {
      localStorage.setItem('active_sede_context', JSON.stringify(sede));
    } else {
      localStorage.removeItem('active_sede_context');
    }
  }

  loadInitialSede(sedes: Sede[]) {
    const cached = localStorage.getItem('active_sede_context');
    if (cached) {
      try {
        const parsed = JSON.parse(cached) as Sede;
        const exists = sedes.find(s => s.id === parsed.id && s.activo);
        if (exists) {
          this.activeSedeSignal.set(exists);
          return;
        }
      } catch { }
    }
    const principal = sedes.find(s => s.esPrincipal && s.activo);
    if (principal) {
      this.setSedeActiva(principal);
    } else if (sedes.length > 0) {
      this.setSedeActiva(sedes[0]);
    }
  }

  // --- API SEDES ---
  getSedes(forceRefresh = false): Observable<Sede[]> {
    if (!this.sedesCache$ || forceRefresh) {
      this.sedesCache$ = this.http.get<Sede[]>(`${environment.apiUrl}/api/Sede`).pipe(
        shareReplay({ bufferSize: 1, refCount: false })
      );
    }
    return this.sedesCache$;
  }

  createSede(dto: { codigo: string; nombre: string; esPrincipal: boolean }): Observable<string> {
    return this.http.post<string>(`${environment.apiUrl}/api/Sede`, dto).pipe(
      tap(() => this.clearCache())
    );
  }

  updateSede(id: string, dto: { id: string; codigo: string; nombre: string; esPrincipal: boolean }): Observable<any> {
    return this.http.put(`${environment.apiUrl}/api/Sede/${id}`, dto).pipe(
      tap(() => this.clearCache())
    );
  }

  deleteSede(id: string): Observable<any> {
    return this.http.delete(`${environment.apiUrl}/api/Sede/${id}`).pipe(
      tap(() => this.clearCache())
    );
  }

  // --- API AREAS CLINICAS ---
  getAreasClinicas(sedeId?: string, forceRefresh = false): Observable<AreaClinica[]> {
    const key = sedeId || 'ALL';
    if (!this.areasCache.has(key) || forceRefresh) {
      let params: any = {};
      if (sedeId) params.sedeId = sedeId;
      const obs = this.http.get<AreaClinica[]>(`${environment.apiUrl}/api/AreaClinica`, { params }).pipe(
        shareReplay({ bufferSize: 1, refCount: false })
      );
      this.areasCache.set(key, obs);
    }
    return this.areasCache.get(key)!;
  }

  createAreaClinica(dto: { sedeId: string; codigo: string; nombre: string }): Observable<string> {
    return this.http.post<string>(`${environment.apiUrl}/api/AreaClinica`, dto).pipe(
      tap(() => this.clearCache())
    );
  }

  updateAreaClinica(id: string, dto: { id: string; sedeId: string; codigo: string; nombre: string }): Observable<any> {
    return this.http.put(`${environment.apiUrl}/api/AreaClinica/${id}`, dto).pipe(
      tap(() => this.clearCache())
    );
  }

  deleteAreaClinica(id: string): Observable<any> {
    return this.http.delete(`${environment.apiUrl}/api/AreaClinica/${id}`).pipe(
      tap(() => this.clearCache())
    );
  }

  // --- API PEDIDOS INTER-SEDE ---
  createPedido(dto: CreatePedidoInterSedeDto): Observable<string> {
    return this.http.post<string>(`${environment.apiUrl}/api/PedidoInterSede`, dto);
  }

  getPedidosPendientes(): Observable<PedidoInterSede[]> {
    return this.http.get<PedidoInterSede[]>(`${environment.apiUrl}/api/PedidoInterSede/pendientes`);
  }

  getPedidosRecibidos(): Observable<PedidoInterSede[]> {
    return this.getPedidosPendientes();
  }

  getPedidosHistorial(): Observable<PedidoInterSede[]> {
    return this.http.get<PedidoInterSede[]>(`${environment.apiUrl}/api/PedidoInterSede/historial`);
  }

  despacharPedido(id: string, cantidadesAprobadas?: { [key: string]: number }, observacionesPorDetalle?: { [key: string]: string }): Observable<any> {
    const payload = {
      cantidadesAprobadas: cantidadesAprobadas || {},
      observacionesPorDetalle: observacionesPorDetalle || {}
    };
    return this.http.put(`${environment.apiUrl}/api/PedidoInterSede/${id}/despachar`, payload);
  }

  aprobarPedido(id: string, cantidadesAprobadas?: { [key: string]: number }, observacionesPorDetalle?: { [key: string]: string }): Observable<any> {
    return this.despacharPedido(id, cantidadesAprobadas, observacionesPorDetalle);
  }

  rechazarPedido(id: string, motivo: string): Observable<any> {
    return this.http.put(`${environment.apiUrl}/api/PedidoInterSede/${id}/rechazar`, { motivo });
  }

  recibirPedido(id: string, discrepancias: { [key: string]: number }): Observable<any> {
    return this.http.put(`${environment.apiUrl}/api/PedidoInterSede/${id}/recibir`, discrepancias);
  }

  // --- MONITOREO Y CREACIÓN DE CAMAS ---
  getCamasMonitoreo(): Observable<any[]> {
    return this.http.get<any[]>(`${environment.apiUrl}/api/AreaClinica/monitoreo`);
  }

  crearCama(dto: { sedeId: string; codigo: string; nombre: string }): Observable<string> {
    return this.http.post<string>(`${environment.apiUrl}/api/AreaClinica`, dto);
  }
}
