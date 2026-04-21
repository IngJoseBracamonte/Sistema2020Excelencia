import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CatalogItem } from '../models/priced-item.model';
export { CatalogItem };

@Injectable({
  providedIn: 'root'
})
export class CatalogService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api/Catalog`;

  // convenioId ahora es numérico para coincidir con Legacy
  getUnifiedCatalog(convenioId?: number | null): Observable<CatalogItem[]> {
    const url = convenioId ? `${this.apiUrl}/unified?convenioId=${convenioId}` : `${this.apiUrl}/unified`;
    return this.http.get<any[]>(url).pipe(
      map(items => items.map(i => new CatalogItem(i)))
    );
  }

  createItem(item: Partial<CatalogItem>): Observable<string> {
    return this.http.post<string>(this.apiUrl, {
      descripcion: item.descripcion,
      codigo: item.codigo,
      precioUsd: item.precioUsd,
      tipo: item.tipo,
      honorarioBase: item.honorarioBase ?? 0,
      sugerenciasIds: item.sugerenciasIds ?? [],
      activo: item.activo ?? true
    });
  }

  updateItem(item: CatalogItem): Observable<boolean> {
    return this.http.put<boolean>(this.apiUrl, {
      id: item.id,
      descripcion: item.descripcion,
      codigo: item.codigo,
      precioUsd: item.precioUsd,
      tipo: item.tipo,
      honorarioBase: item.honorarioBase ?? 0,
      sugerenciasIds: item.sugerenciasIds ?? [],
      activo: item.activo ?? true
    });
  }

  deleteItem(id: string): Observable<boolean> {
    return this.http.delete<boolean>(`${this.apiUrl}/${id}`);
  }
}
