import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AuthService } from './auth.service';

export interface BusinessInsights {
  totalVentasHoy: number;
  pacientesAtendidosHoy: number;
  saldoPendienteAR: number;
  turnosPautadosHoy: number;
  ventasPorEspecialidad: Array<{ especialidad: string; monto: number }>;
  ventasPorSeguro: Array<{ seguro: string; monto: number }>;
  
  // Métricas de RX (Micro-Ciclo 22)
  totalOrdenesRxHoy: number;
  ordenesRxProcesadasHoy: number;
  ventasRxHoy: number;

  // Analytics Phase 6
  tendenciaIngresos: Array<{ fecha: string; monto: number }>;
  distribucionPacientes: Array<{ etiqueta: string; valor: number }>;
}

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  private http = inject(HttpClient);
  private auth = inject(AuthService);
  private baseUrl = `${environment.apiUrl}/Dashboard`;

  getInsights(): Observable<BusinessInsights> {
    // El rol se envía para que el Backend filtre la información sensible
    return this.http.get<BusinessInsights>(`${this.baseUrl}/Insights`);
  }
}
