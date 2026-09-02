import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';

// DTOs de catálogo (coinciden con los records del backend)
export interface UnidadMedidaDto {
  id: number;
  codigo: string;
  nombre: string;
  simbolo: string;
  esFraccionable: boolean;
}

export interface EstadoCitaMedicaDto {
  id: number;
  codigo: string;
  nombre: string;
}

export interface EstadoCajaDto {
  id: number;
  codigo: string;
  nombre: string;
}

export interface EstadoCuentaDto {
  id: number;
  codigo: string;
  nombre: string;
}

export interface TipoIngresoDto {
  id: number;
  codigo: string;
  nombre: string;
}

export interface EstadoFiscalDto {
  id: number;
  codigo: string;
  nombre: string;
}

export interface MotivoAutorizacionDto {
  id: number;
  nombre: string;
}

export interface CategoriaInsumoDto {
  id: string;
  nombre: string;
  codigo?: string;
}

/**
 * Servicio central de catálogos cacheados (3FN).
 * Consume los endpoints del backend que usan ICatalogLookupService (IMemoryCache).
 * Los signals se actualizan reactivamente al llamar load*().
 */
@Injectable({ providedIn: 'root' })
export class CatalogLookupService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api/Catalog/catalogos`;

  // Signals cacheados (se actualizan al llamar load*())
  public unidadesMedida = signal<UnidadMedidaDto[]>([]);
  public estadosCita = signal<EstadoCitaMedicaDto[]>([]);
  public estadosCaja = signal<EstadoCajaDto[]>([]);
  public estadosCuenta = signal<EstadoCuentaDto[]>([]);
  public tiposIngreso = signal<TipoIngresoDto[]>([]);
  public estadosFiscales = signal<EstadoFiscalDto[]>([]);
  public motivosAutorizacion = signal<MotivoAutorizacionDto[]>([]);
  public categoriasInsumo = signal<CategoriaInsumoDto[]>([]);

  /** Carga todas las unidades de medida activas. */
  loadUnidadesMedida(): Observable<UnidadMedidaDto[]> {
    return this.http.get<UnidadMedidaDto[]>(`${this.apiUrl}/unidades-medida`).pipe(
      tap(data => this.unidadesMedida.set(data))
    );
  }

  /** Carga todos los estados de cita médica activos. */
  loadEstadosCita(): Observable<EstadoCitaMedicaDto[]> {
    return this.http.get<EstadoCitaMedicaDto[]>(`${this.apiUrl}/estados-cita`).pipe(
      tap(data => this.estadosCita.set(data))
    );
  }

  /** Carga todos los estados de caja activos. */
  loadEstadosCaja(): Observable<EstadoCajaDto[]> {
    return this.http.get<EstadoCajaDto[]>(`${this.apiUrl}/estados-caja`).pipe(
      tap(data => this.estadosCaja.set(data))
    );
  }

  /** Carga todos los estados de cuenta activos. */
  loadEstadosCuenta(): Observable<EstadoCuentaDto[]> {
    return this.http.get<EstadoCuentaDto[]>(`${this.apiUrl}/estados-cuenta`).pipe(
      tap(data => this.estadosCuenta.set(data))
    );
  }

  /** Carga todos los tipos de ingreso activos. */
  loadTiposIngreso(): Observable<TipoIngresoDto[]> {
    return this.http.get<TipoIngresoDto[]>(`${this.apiUrl}/tipos-ingreso`).pipe(
      tap(data => this.tiposIngreso.set(data))
    );
  }

  /** Carga todos los estados fiscales activos. */
  loadEstadosFiscales(): Observable<EstadoFiscalDto[]> {
    return this.http.get<EstadoFiscalDto[]>(`${this.apiUrl}/estados-fiscales`).pipe(
      tap(data => this.estadosFiscales.set(data))
    );
  }

  /** Carga todos los motivos de autorización activos. */
  loadMotivosAutorizacion(): Observable<MotivoAutorizacionDto[]> {
    return this.http.get<MotivoAutorizacionDto[]>(`${this.apiUrl}/motivos-autorizacion`).pipe(
      tap(data => this.motivosAutorizacion.set(data))
    );
  }

  /** Carga todas las categorías de insumo activas. */
  loadCategoriasInsumo(): Observable<CategoriaInsumoDto[]> {
    return this.http.get<CategoriaInsumoDto[]>(`${this.apiUrl}/categorias-insumo`).pipe(
      tap(data => this.categoriasInsumo.set(data))
    );
  }

  /** Carga todos los catálogos de una vez (útil para inicialización). */
  loadAll(): void {
    this.loadUnidadesMedida().subscribe();
    this.loadEstadosCita().subscribe();
    this.loadEstadosCaja().subscribe();
    this.loadEstadosCuenta().subscribe();
    this.loadTiposIngreso().subscribe();
    this.loadEstadosFiscales().subscribe();
    this.loadMotivosAutorizacion().subscribe();
    this.loadCategoriasInsumo().subscribe();
  }

  /** Invalida toda la caché del backend (solo Admin). */
  invalidateAll(): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/invalidate`, {});
  }
}
