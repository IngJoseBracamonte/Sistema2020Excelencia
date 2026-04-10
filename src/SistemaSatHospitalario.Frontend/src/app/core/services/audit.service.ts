import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface PriceAuditLog {
  id: string;
  descripcionServicio: string;
  precioOriginal: number;
  precioModificado: number;
  honorarioAnterior: number;
  nuevoHonorario: number;
  varianzo: number;
  varianzoPorcentual: number;
  usuarioOperador: string;
  autorizadoPor: string;
  fechaModificacion: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuditService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/settings/audit/precios`;

  getPriceAuditLogs(desde?: string, hasta?: string): Observable<PriceAuditLog[]> {
    let params = new HttpParams();
    if (desde) params = params.set('desde', desde);
    if (hasta) params = params.set('hasta', hasta);
    
    return this.http.get<PriceAuditLog[]>(this.apiUrl, { params });
  }
}
