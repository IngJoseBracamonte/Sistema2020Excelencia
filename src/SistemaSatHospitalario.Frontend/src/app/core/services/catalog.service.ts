import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface CatalogItem {
  id: string; // Puede ser Guid (Nativo) o stringified int (Legacy)
  codigo: string;
  descripcion: string;
  precio: number;
  tipo: string;
  esLegacy: boolean;
  activo?: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class CatalogService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/Catalog`;

  // convenioId ahora es numérico para coincidir con Legacy
  getUnifiedCatalog(convenioId?: number | null): Observable<CatalogItem[]> {
    const url = convenioId ? `${this.apiUrl}/unified?convenioId=${convenioId}` : `${this.apiUrl}/unified`;
    return this.http.get<CatalogItem[]>(url);
  }

  createItem(item: Partial<CatalogItem>): Observable<string> {
    return this.http.post<string>(this.apiUrl, {
      descripcion: item.descripcion,
      codigo: item.codigo,
      precioBase: item.precio,
      tipo: item.tipo,
      activo: item.activo ?? true
    });
  }

  updateItem(item: CatalogItem): Observable<boolean> {
    return this.http.put<boolean>(this.apiUrl, {
      id: item.id,
      descripcion: item.descripcion,
      codigo: item.codigo,
      precioBase: item.precio,
      tipo: item.tipo,
      activo: item.activo ?? true
    });
  }
}
