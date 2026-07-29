import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
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

  getInsumos(excludeHidden?: boolean, search?: string): Observable<Insumo[]> {
    let params: any = {};
    if (excludeHidden !== undefined) {
      params.excludeHidden = excludeHidden.toString();
    }
    if (search) {
      params.search = search;
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

  recordMovement(dto: RecordMovement): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/movimientos`, dto);
  }

  getMovements(): Observable<MovimientoInsumo[]> {
    return this.http.get<MovimientoInsumo[]>(`${this.apiUrl}/movimientos`);
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
