import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AuthService } from './auth.service';

export interface RevenueTrendPoint {
  fecha: string;
  monto: number;
  transacciones: number;
}

export interface CategoryPurchase {
  categoria: string;
  monto: number;
}

export interface ServiceBreakdown {
  servicio: string;
  ordenes: number;
  monto: number;
  porcentaje: number;
}

export interface OriginBreakdown {
  origen: string;
  cantidad: number;
  porcentaje: number;
}

export interface DashboardAuditEntry {
  modulo: string;
  timestamp: string;
  descripcion: string;
  usuario: string;
  tipoEvento: string;
}

export interface BusinessInsights {
  totalVentasHoy: number;
  pacientesAtendidosHoy: number;
  saldoPendienteAR: number;
  turnosPautadosHoy: number;
  ventasPorEspecialidad: Array<{ especialidad: string; monto: number }>;
  ventasPorSeguro: Array<{ seguro: string; monto: number }>;
  
  // Dashboard Redesign Panel KPIs
  kpiTotalOrdenes: number;
  kpiPacientesAtendidos: number;
  kpiTicketPromedio: number;

  // Panel Inferior Central
  totalComprasInventario: number;
  comprasPorCategoria: CategoryPurchase[];

  // Panel Izquierdo
  desglosePorServicio: ServiceBreakdown[];
  desglosePorOrigen: OriginBreakdown[];

  // Panel Derecho
  historialAuditoria: DashboardAuditEntry[];

  // Métricas de RX (Micro-Ciclo 22)
  totalOrdenesRxHoy: number;
  ordenesRxProcesadasHoy: number;
  ventasRxHoy: number;

  // Analytics Phase 6
  tendenciaIngresos: RevenueTrendPoint[];
  distribucionPacientes: Array<{ etiqueta: string; valor: number }>;
}

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  private http = inject(HttpClient);
  private auth = inject(AuthService);
  private baseUrl = `${environment.apiUrl}/api/Dashboard`;

  getInsights(): Observable<BusinessInsights> {
    // El rol se envía para que el Backend filtre la información sensible
    return this.http.get<BusinessInsights>(`${this.baseUrl}/Insights`);
  }
}
