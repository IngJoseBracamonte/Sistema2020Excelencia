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
  CreateRecipe 
} from '../models/inventory.model';

@Injectable({ providedIn: 'root' })
export class InventoryService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api/inventory`;

  getInsumos(): Observable<Insumo[]> {
    return this.http.get<Insumo[]>(`${this.apiUrl}/insumos`);
  }

  getStockPorSede(sedeId: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/stock-por-sede`, { params: { sedeId } });
  }

  createInsumo(dto: CreateInsumo): Observable<Insumo> {
    return this.http.post<Insumo>(`${this.apiUrl}/insumos`, dto);
  }

  updateInsumo(id: string, dto: UpdateInsumo): Observable<Insumo> {
    return this.http.put<Insumo>(`${this.apiUrl}/insumos/${id}`, dto);
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
    return this.http.delete<any>(`${`${this.apiUrl}/recetas`}/${id}`);
  }
}
