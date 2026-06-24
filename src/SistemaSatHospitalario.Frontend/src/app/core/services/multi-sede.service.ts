import { Injectable, inject, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { environment } from '../../../environments/environment';

export interface Sede {
  id: string;
  codigo: string;
  nombre: string;
  esPrincipal: boolean;
  activo: boolean;
  areasClinicas?: AreaClinica[];
}

export interface AreaClinica {
  id: string;
  sedeId: string;
  codigo: string;
  nombre: string;
  activo: boolean;
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
}

export interface CreatePedidoInterSedeDto {
  sedeSolicitanteId: string;
  sedeProveedoraId: string;
  observaciones: string;
  lineas: { insumoId: string; cantidadSolicitada: number }[];
}

@Injectable({ providedIn: 'root' })
export class MultiSedeService {
  private http = inject(HttpClient);
  
  // Sede Contexto Activo (para filtrados de stock reactivos)
  private activeSedeSignal = signal<Sede | null>(null);
  public activeSede = computed(() => this.activeSedeSignal());

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
      } catch {}
    }
    const principal = sedes.find(s => s.esPrincipal && s.activo);
    if (principal) {
      this.setSedeActiva(principal);
    } else if (sedes.length > 0) {
      this.setSedeActiva(sedes[0]);
    }
  }

  // --- API SEDES ---
  getSedes(): Observable<Sede[]> {
    return this.http.get<Sede[]>(`${environment.apiUrl}/api/Sede`);
  }

  createSede(dto: { codigo: string; nombre: string; esPrincipal: boolean }): Observable<string> {
    return this.http.post<string>(`${environment.apiUrl}/api/Sede`, dto);
  }

  updateSede(id: string, dto: { id: string; codigo: string; nombre: string; esPrincipal: boolean }): Observable<any> {
    return this.http.put(`${environment.apiUrl}/api/Sede/${id}`, dto);
  }

  deleteSede(id: string): Observable<any> {
    return this.http.delete(`${environment.apiUrl}/api/Sede/${id}`);
  }

  // --- API AREAS CLINICAS ---
  createAreaClinica(dto: { sedeId: string; codigo: string; nombre: string }): Observable<string> {
    return this.http.post<string>(`${environment.apiUrl}/api/AreaClinica`, dto);
  }

  updateAreaClinica(id: string, dto: { id: string; sedeId: string; codigo: string; nombre: string }): Observable<any> {
    return this.http.put(`${environment.apiUrl}/api/AreaClinica/${id}`, dto);
  }

  deleteAreaClinica(id: string): Observable<any> {
    return this.http.delete(`${environment.apiUrl}/api/AreaClinica/${id}`);
  }

  // --- API PEDIDOS INTER-SEDE ---
  createPedido(dto: CreatePedidoInterSedeDto): Observable<string> {
    return this.http.post<string>(`${environment.apiUrl}/api/PedidoInterSede`, dto);
  }

  getPedidosPendientes(): Observable<PedidoInterSede[]> {
    return this.http.get<PedidoInterSede[]>(`${environment.apiUrl}/api/PedidoInterSede/pendientes`);
  }

  despacharPedido(id: string): Observable<any> {
    return this.http.put(`${environment.apiUrl}/api/PedidoInterSede/${id}/despachar`, {});
  }

  recibirPedido(id: string, discrepancias: { [key: string]: number }): Observable<any> {
    return this.http.put(`${environment.apiUrl}/api/PedidoInterSede/${id}/recibir`, discrepancias);
  }
}
