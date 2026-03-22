import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface DetallePagoDto {
  metodoPago: string;
  referenciaBancaria: string;
  montoAbonadoMoneda: number;
  equivalenteAbonadoBase: number;
}

export interface RegistrarReciboFacturaRequest {
  cuentaServicioId: string; // Guid
  pacienteId: number; // Unificado con Legacy
  cajeroUserId: string;
  tasaCambioDia: number;
  pagosMultidivisa: DetallePagoDto[];
}

export interface CargarServicioACuentaRequest {
  pacienteId: number; // Unificado con Legacy
  tipoIngreso: string;
  convenioId?: number; // Unificado con Legacy
  servicioId: string; // Guid (Nativo) o stringified int (Legacy)
  descripcion: string;
  precio: number;
  cantidad: number;
  tipoServicio: string;
  usuarioCarga: string;
  medicoId?: string;
  horaCita?: string;
}

export interface ReceiptPrintData {
  id: string;
  numeroRecibo: string;
  fechaEmision: string;
  pacienteNombre: string;
  pacienteCedula: string;
  tipoIngreso: string;
  totalUSD: number;
  totalBS: number;
  tasaBcv: number;
  detalles: Array<{ descripcion: string; cantidad: number; precioUnitario: number; subtotal: number }>;
  pagos: Array<{ metodoPago: string; montoOriginal: number; equivalenteBase: number; referencia: string }>;
}

@Injectable({
  providedIn: 'root'
})
export class FacturacionService {
  private http = inject(HttpClient);
  private billingUrl = `${environment.apiUrl}/Billing`;
  private receiptUrl = `${environment.apiUrl}/ReciboFactura`;

  closeAccount(request: any): Observable<any> {
    // Al cerrar cuenta, se espera cuentaId (Guid)
    return this.http.post<any>(`${this.billingUrl}/CloseAccount`, request);
  }

  cargarServicio(payload: CargarServicioACuentaRequest): Observable<any> {
    return this.http.post<any>(`${this.billingUrl}/CargarServicio`, payload);
  }

  registrarPago(payload: RegistrarReciboFacturaRequest): Observable<any> {
    return this.http.post<any>(`${this.receiptUrl}/RegistrarPagoMultidivisa`, payload);
  }

  getReceiptPrintData(reciboId: string): Observable<ReceiptPrintData> {
    return this.http.get<ReceiptPrintData>(`${this.receiptUrl}/${reciboId}/Print`);
  }
}
