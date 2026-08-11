import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of, catchError } from 'rxjs';
import { environment } from '../../../environments/environment';
import { 
  Insumo, 
  MovimientoInsumo, 
  ServicioInsumoReceta, 
  CreateInsumo, 
  UpdateInsumo, 
  RecordMovement, 
  PerformClosing, 
  CreateRecipe,
  RecordPurchase,
  PrincipioActivo,
  DescarteRequest
} from '../models/inventory.model';

@Injectable({ providedIn: 'root' })
export class InventoryService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api/inventory`;

  getInsumos(excludeHidden?: boolean, search?: string, sedeId?: string): Observable<Insumo[]> {
    let params: any = {};
    if (excludeHidden !== undefined) {
      params.excludeHidden = excludeHidden.toString();
    }
    if (search) {
      params.search = search;
    }
    if (sedeId) {
      params.sedeId = sedeId;
    }
    return this.http.get<Insumo[]>(`${this.apiUrl}/insumos`, { params });
  }

  getInsumoById(id: string): Observable<Insumo> {
    return this.http.get<Insumo>(`${this.apiUrl}/insumos/${id}`);
  }

  getStockPorSede(sedeId: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/stock-por-sede`, { params: { sedeId } });
  }

  getStockSedeById(sedeId: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/stock-sede/${sedeId}`);
  }

  getStockConsolidado(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/stock-consolidado`);
  }

  createInsumo(dto: CreateInsumo): Observable<Insumo> {
    return this.http.post<Insumo>(`${this.apiUrl}/insumos`, dto);
  }

  updateInsumo(id: string, dto: UpdateInsumo): Observable<Insumo> {
    return this.http.put<Insumo>(`${this.apiUrl}/insumos/${id}`, dto);
  }

  deleteInsumo(id: string): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/insumos/${id}`);
  }

  restoreInsumo(id: string): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/insumos/${id}/restaurar`, {});
  }

  // --- Principios Activos ---

  getPrincipiosActivos(): Observable<PrincipioActivo[]> {
    return this.http.get<PrincipioActivo[]>(`${this.apiUrl}/principios-activos`);
  }

  createPrincipioActivo(nombre: string): Observable<PrincipioActivo> {
    return this.http.post<PrincipioActivo>(`${this.apiUrl}/principios-activos`, { nombre });
  }

  vincularPrincipioActivo(insumoId: string, principioActivoId: string, concentracion: string): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/insumos/${insumoId}/principios-activos`, {
      principioActivoId,
      concentracion
    });
  }

  desvincularPrincipioActivo(insumoId: string, paId: string): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/insumos/${insumoId}/principios-activos/${paId}`);
  }

  // --- Operaciones ---

  recordPurchase(dto: RecordPurchase): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/compras`, dto);
  }

  registrarDescarte(dto: DescarteRequest): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/descarte`, dto);
  }

  recordMovement(dto: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/movimiento`, dto);
  }

  enviarASubArea(dto: { insumoId: string; areaClinicaId?: string; nombreSubArea: string; cantidad: number; motivo: string }): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/envio-subarea`, dto);
  }

  getHistorialMovimientos(tipoMovimiento?: string, fechaDesde?: string, fechaHasta?: string, search?: string): Observable<any[]> {
    let params: any = {};
    if (tipoMovimiento) params.tipoMovimiento = tipoMovimiento;
    if (fechaDesde) params.fechaDesde = fechaDesde;
    if (fechaHasta) params.fechaHasta = fechaHasta;
    if (search) params.search = search;
    return this.http.get<any[]>(`${this.apiUrl}/movimientos`, { params });
  }

  getMovements(): Observable<any[]> {
    return this.getHistorialMovimientos();
  }

  getMotivosDescarte(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/motivos-descarte`).pipe(
      // Fallback defensivo si la BD no retorna registros aún
      catchError(() => of([
        { id: '10000000-0000-0000-0000-000000000101', codigo: 'VENC', nombre: 'Vencimiento / Caducidad' },
        { id: '10000000-0000-0000-0000-000000000102', codigo: 'DANO', nombre: 'Avería / Rotura / Daño Físico' },
        { id: '10000000-0000-0000-0000-000000000103', codigo: 'DETE', nombre: 'Deterioro / Contaminación' },
        { id: '10000000-0000-0000-0000-000000000104', codigo: 'AUDI', nombre: 'Ajuste de Auditoría' }
      ]))
    );
  }

  getTiposMovimiento(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/tipos-movimiento`).pipe(
      catchError(() => of([
        { id: 'ING', nombre: 'Ingreso / Compra Insumo' },
        { id: 'ENV', nombre: 'Envío Directo a Sub-Área' },
        { id: 'DES', nombre: 'Descarte / Baja de Inventario' },
        { id: 'AJU', nombre: 'Ajuste Kárdex Auditoría' }
      ]))
    );
  }

  getKardex(sedeId?: string, insumoId?: string, fechaDesde?: string, fechaHasta?: string): Observable<any> {
    let params: any = {};
    if (sedeId && sedeId !== 'TODAS') params.sedeId = sedeId;
    if (insumoId && insumoId !== 'TODOS') params.insumoId = insumoId;
    if (fechaDesde) params.fechaDesde = fechaDesde;
    if (fechaHasta) params.fechaHasta = fechaHasta;
    return this.http.get<any>(`${this.apiUrl}/kardex`, { params });
  }

  performClosing(dto: PerformClosing): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/cierre`, dto);
  }

  getRecetas(): Observable<ServicioInsumoReceta[]> {
    return this.http.get<ServicioInsumoReceta[]>(`${this.apiUrl}/recetas`);
  }

  createOrUpdateRecipe(dto: CreateRecipe): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/recetas`, dto);
  }

  deleteRecipe(id: string): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/recetas/${id}`);
  }
}
