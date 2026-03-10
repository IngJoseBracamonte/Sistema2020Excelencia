import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface DetallePagoDto {
  metodoPago: string;
  referenciaBancaria: string;
  montoAbonadoMoneda: number;
  equivalenteAbonadoBase: number;
}

export interface RegistrarReciboFacturaRequest {
  cuentaServicioId: string;
  cajeroUserId: string;
  tasaCambioDia: number;
  pagosMultidivisa: DetallePagoDto[];
}

export interface CargarServicioACuentaRequest {
  pacienteId: string;
  tipoIngreso: string;
  convenioId?: string;
  servicioId: string;
  descripcion: string;
  precio: number;
  cantidad: number;
  tipoServicio: string;
  usuarioCarga: string;
  medicoId?: string;
  horaCita?: string;
}

@Injectable({
  providedIn: 'root'
})
export class FacturacionService {
  private http = inject(HttpClient);
  private baseUrl = 'https://localhost:7019/api';

  registrarPagoMultidivisa(payload: RegistrarReciboFacturaRequest): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/ReciboFactura/RegistrarPagoMultidivisa`, payload);
  }

  cargarServicio(payload: CargarServicioACuentaRequest): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/Billing/CargarServicio`, payload);
  }
}
