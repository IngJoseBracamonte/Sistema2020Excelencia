import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface AbrirCajaRequest {
  montoInicialDivisa: number;
  montoInicialBs: number;
}

export interface ResumenTurnoDto {
  turnoId: string;
  cajeroUserId: string;
  estado: string;
  recaudadoBase: number;
}

export interface ResumenCajaGlobalDto {
  cajaId: string;
  fechaApertura: string;
  montoInicialBase: number;
  totalRecaudadoBase: number;
  granTotalEnCajaBase: number;
  turnos: ResumenTurnoDto[];
}

export interface PaymentMethodSummary {
  metodo: string;
  montoMonedaOriginal: number;
  montoEquivalenteBase: number;
  conteo: number;
}

export interface DailyClosingReport {
  fecha: string;
  usuario: string;
  totalOrdenes: number;
  totalVendidoUSD: number;
  totalRecaudadoBase: number;
  desgloseMetodos: PaymentMethodSummary[];
}

export interface CajaDetailDto {
  id: string;
  usuario: string;
  apertura: string;
  cierre: string | null;
  montoInicialDivisa: number;
  montoInicialBs: number;
  estado: string;
}

export interface CajaSummaryDto {
  granTotalDivisa: number;
  granTotalBs: number;
  cierres: CajaDetailDto[];
}

@Injectable({
  providedIn: 'root'
})
export class CajaService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiUrl}/api/Caja`;

  // Signal reactivo para observar el estado local
  public isCajaAbierta = signal<boolean>(false);

  abrirCaja(payload: AbrirCajaRequest): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/Abrir`, payload).pipe(
      tap(() => this.isCajaAbierta.set(true))
    );
  }

  cerrarCaja(): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/Cerrar`, {}).pipe(
      tap(() => this.isCajaAbierta.set(false))
    );
  }

  obtenerResumenDiario(): Observable<ResumenCajaGlobalDto> {
    return this.http.get<ResumenCajaGlobalDto>(`${this.baseUrl}/Resumen`);
  }

  getPersonalReport(userId?: string): Observable<DailyClosingReport> {
    const url = userId ? `${this.baseUrl}/PersonalReport?userId=${userId}` : `${this.baseUrl}/PersonalReport`;
    return this.http.get<DailyClosingReport>(url);
  }

  // Monitor Administrativo (Micro-Ciclo 28)
  obtenerHistorial(desde?: string, hasta?: string, usuarioId?: string): Observable<CajaSummaryDto> {
    let params = [];
    if (desde) params.push(`desde=${desde}`);
    if (hasta) params.push(`hasta=${hasta}`);
    if (usuarioId) params.push(`usuarioId=${usuarioId}`);
    
    const query = params.length > 0 ? `?${params.join('&')}` : '';
    return this.http.get<CajaSummaryDto>(`${this.baseUrl}/Historial${query}`);
  }
}
