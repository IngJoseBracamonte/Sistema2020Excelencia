import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
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
  pacienteId: string; // Identidad Nativa GUID (V11.1)
  cajeroUserId: string;
  tasaCambioDia: number;
  pagosMultidivisa: DetallePagoDto[];
}

export interface CargarServicioACuentaRequest {
  pacienteId: string; // Identidad Nativa GUID (V11.1)
  tipoIngreso: string;
  convenioId?: number; // Referencia Legacy
  servicioId: string; // Guid (Nativo) o stringified int (Legacy)
  descripcion: string;
  precio: number;
  honorario: number;
  cantidad: number;
  tipoServicio: string;
  usuarioCarga: string;
  supervisorKey?: string; // V1.0 Security Matrix
  medicoId?: string;
  horaCita?: string;
  comentario?: string;
  requiereInforme?: boolean;
  medicoInterpreteId?: string;
}

export interface ReservarTurnoRequest {
  medicoId: string;
  horaPautada: string; // ISO
  comentario?: string;
}

export interface BloquearHorarioRequest {
  medicoId: string;
  horaPautada: string; // ISO
  motivo: string;
}

export interface SyncCarritoMasivoRequest {
  pacienteId: string; // Identidad Nativa GUID (V11.1)
  cuentaId?: string;
  idPacienteLegacy?: number; // V11.8 Support
  tipoIngreso: string;
  convenioId?: number;
  usuarioCarga: string;
  supervisorKey?: string; // V1.0 Security Matrix
  items: Array<{
    servicioId: string;
    descripcion: string;
    precio: number;
    honorario: number;
    cantidad: number;
    tipoServicio: string;
    medicoId?: string;
    horaCita?: string;
    comentario?: string;
    requiereInforme?: boolean;
    medicoInterpreteId?: string;
  }>;
}

export interface ReceiptPrintData {
  facturaId: string;
  numeroFactura: string;
  fechaEmision: string;
  pacienteNombre: string;
  pacienteCedula: string;
  tipoIngreso: string;
  detalles: Array<{
    descripcion: string;
    cantidad: number;
    precioUnitarioUsd: number;
    subtotalUsd: number;
    subtotalBs: number;
  }>;
  totalUsd: number;
  tasaCambio: number;
  totalBs: number;
  pagos: Array<{
    metodo: string;
    referencia: string;
    montoOriginal: number;
    montoBs: number;
  }>;
  cajeroNombre: string;
}

@Injectable({
  providedIn: 'root'
})
export class FacturacionService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api/Billing`;

  abrirCuenta(pacienteId: string, usuarioCarga: string, tipoIngreso: string, convenioId?: number): Observable<string> {
    return this.http.post<string>(`${this.apiUrl}/abrir-cuenta`, {
      pacienteId,
      usuarioCarga,
      tipoIngreso,
      convenioId
    });
  }

  cargarServicio(request: CargarServicioACuentaRequest): Observable<{ cuentaId: string; detalleId: string }> {
    return this.http.post<{ cuentaId: string; detalleId: string }>(`${this.apiUrl}/cargar-servicio`, request);
  }

  syncCarritoMasivo(request: SyncCarritoMasivoRequest): Observable<{ cuentaId: string }> {
    return this.http.post<{ cuentaId: string }>(`${this.apiUrl}/sync-carrito`, request);
  }

  registrarReciboFactura(request: RegistrarReciboFacturaRequest): Observable<{ facturaId: string; numeroFactura: string }> {
    return this.http.post<{ facturaId: string; numeroFactura: string }>(`${this.apiUrl}/recibo-factura`, request);
  }

  getFacturaPrintData(facturaId: string): Observable<ReceiptPrintData> {
    return this.http.get<ReceiptPrintData>(`${this.apiUrl}/facturas/${facturaId}/print`);
  }

  getReciboPrintData(reciboId: string): Observable<ReceiptPrintData> {
    return this.http.get<ReceiptPrintData>(`${this.apiUrl}/recibos/${reciboId}/print`);
  }

  getExpedienteFacturacion(pacienteId: string, searchTerm?: string): Observable<any> {
    let url = `${this.apiUrl}/expediente/${pacienteId}`;
    if (searchTerm) {
      url += `?searchTerm=${encodeURIComponent(searchTerm)}`;
    }
    return this.http.get<any>(url);
  }
}
