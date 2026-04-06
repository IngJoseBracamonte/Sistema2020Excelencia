import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface PendingAR {
  id: string;
  cuentaId: string;
  pacienteNombre: string;
  pacienteCedula: string;
  tipoIngreso: string;
  seguroNombre: string;
  montoTotal: number;
  saldoPendiente: number;
  fechaEmision: string;
  estado: string;
}

export interface SettleARRequest {
  arId: string;
  payments: PaymentItemDto[];
  observaciones: string;
}

export interface PaymentItemDto {
  method: string;
  amount: number; // Monto base en USD ($)
  amountMoneda: number; // Monto en moneda original ($ o Bs.)
  tasaAplicada: number; // Tasa de cambio usada al registrar
  reference: string;
}

@Injectable({
  providedIn: 'root'
})
export class ReceivablesService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiUrl}/Receivables`;

  getPending(searchTerm?: string, estado?: string, startDate?: string, endDate?: string): Observable<PendingAR[]> {
    let params = new HttpParams();
    if (searchTerm) params = params.set('searchTerm', searchTerm);
    
    if (estado && estado.trim() !== '' && estado.toLowerCase() !== 'todas') {
      params = params.set('estado', estado);
    }
    
    if (startDate) params = params.set('startDate', startDate);
    if (endDate) params = params.set('endDate', endDate);
    
    return this.http.get<PendingAR[]>(`${this.baseUrl}/Pending`, { params });
  }

  settle(request: SettleARRequest): Observable<any> {
    return this.http.post(`${this.baseUrl}/Settle`, request);
  }
}
