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

export interface DailyBilledPatient {
  cuentaId: string;
  pacienteId: string;
  cedula: string;
  nombre: string;
  apellidos: string;
  tipoIngreso: string;
  montoTotalUsd: number;
  totalFacturado?: number;
  cuentasCerradas?: number;
  fechaIngreso: string;
  fechaCierre?: string;
  estadoCuenta: string;
}

export interface ReceiptPrintData {
  id?: string;
  reciboId?: string;
  numeroRecibo?: string;
  facturaId?: string;
  numeroFactura?: string;
  fechaEmision: string;
  pacienteNombre: string;
  pacienteCedula: string;
  tipoIngreso: string;
  detalles: Array<{
    descripcion: string;
    cantidad: number;
    precioUnitarioUsd?: number;
    precioUnitario?: number;
    subtotalUsd?: number;
    subtotalBs?: number;
    subtotal?: number;
  }>;
  totalUsd?: number;
  totalUSD?: number;
  tasaCambio?: number;
  tasaBcv?: number;
  totalBs?: number;
  totalBS?: number;
  pagos: Array<{
    metodo?: string;
    metodoPago?: string;
    referencia: string;
    montoOriginal: number;
    montoBs?: number;
    equivalenteBase?: number;
  }>;
  cajeroNombre?: string;
}

export interface CloseAccountRequest {
  cuentaId: string;
  tasaCambio: number;
  usuarioCajero: string;
  usuarioId: string;
  pagos: DetallePagoDto[];
  consolidar?: boolean;
  destinoPaciente?: string;
  personalRelevo?: string;
  mantenerCuentaAbierta?: boolean;
  cerrarConSaldoPendiente?: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class FacturacionService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api/Billing`;

  abrirCuenta(pacienteId: string, tipoIngreso: string, convenioId?: number | null, camaId?: string | number | null): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/abrir-cuenta`, {
      pacienteId,
      tipoIngreso,
      convenioId: convenioId ?? null,
      camaId: camaId ?? null
    });
  }

  closeAccount(request: CloseAccountRequest): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/CloseAccount`, request);
  }

  quitarServicio(cuentaId: string, detalleId: string, medicoId?: string, horaCita?: string): Observable<any> {
    let url = `${this.apiUrl}/RemoveServicio?cuentaId=${cuentaId}&detalleId=${detalleId}`;
    if (medicoId) url += `&medicoId=${medicoId}`;
    if (horaCita) url += `&horaCita=${encodeURIComponent(horaCita)}`;
    return this.http.delete<any>(url);
  }

  liberarTurno(medicoId: string, horaPautada: string): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/LiberarTurno?medicoId=${medicoId}&horaPautada=${encodeURIComponent(horaPautada)}`);
  }

  getOpenAccount(pacienteId: string, tipoIngreso?: string): Observable<any> {
    let url = `${this.apiUrl}/OpenAccount/${pacienteId}`;
    if (tipoIngreso) {
      url += `?tipoIngreso=${encodeURIComponent(tipoIngreso)}`;
    }
    return this.http.get<any>(url);
  }

  syncBulk(payload: any, idempotencyKey?: string): Observable<any> {
    let headers = new HttpHeaders();
    if (idempotencyKey) {
      headers = headers.set('X-Idempotency-Key', idempotencyKey);
    }
    return this.http.post<any>(`${this.apiUrl}/SincronizarCarrito`, payload, { headers });
  }

  reservarTurno(payload: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/ReservarTurno`, payload);
  }

  bloquearHorario(payload: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/BloquearHorario`, payload);
  }

  getAppointments(fecha?: string): Observable<any> {
    let url = `${this.apiUrl}/Appointments`;
    if (fecha) url += `?fecha=${encodeURIComponent(fecha)}`;
    return this.http.get<any>(url);
  }

  cancelAppointment(appointmentId: string): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/CancelAppointment/${appointmentId}`, {});
  }

  getDailyBilledPatients(fecha?: string): Observable<any> {
    let url = `${this.apiUrl}/DailyBilledPatients`;
    if (fecha) url += `?fecha=${encodeURIComponent(fecha)}`;
    return this.http.get<any>(url);
  }

  updateARMetadata(dto: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/UpdateARMetadata`, dto);
  }

  getGarantiasItems(arId: string): Observable<any> {
    return this.http.get<any>(`${environment.apiUrl}/api/Seguros/garantias-items/${arId}`);
  }

  guardarGarantiasItems(cuentaPorCobrarId: string, items: any[]): Observable<any> {
    return this.http.post<any>(`${environment.apiUrl}/api/Seguros/garantias-items`, { cuentaPorCobrarId, items });
  }

  generarCompromisoPdf(dto: any): Observable<Blob> {
    return this.http.post(`${environment.apiUrl}/api/Seguros/compromiso-pago`, dto, { responseType: 'blob' });
  }

  generarConformidadPdf(dto: any): Observable<Blob> {
    return this.http.post(`${environment.apiUrl}/api/Seguros/conformidad-servicios`, dto, { responseType: 'blob' });
  }

  generarGarantiaPdf(dto: any): Observable<Blob> {
    return this.http.post(`${environment.apiUrl}/api/Seguros/garantia-prendaria`, dto, { responseType: 'blob' });
  }

  cargarServicio(request: CargarServicioACuentaRequest, idempotencyKey?: string): Observable<{ cuentaId: string; detalleId: string }> {
    let headers = new HttpHeaders();
    if (idempotencyKey) {
      headers = headers.set('Idempotency-Key', idempotencyKey);
    }
    return this.http.post<{ cuentaId: string; detalleId: string }>(`${this.apiUrl}/cargar-servicio`, request, { headers });
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

  getReceiptPrintData(reciboId: string): Observable<ReceiptPrintData> {
    return this.getReciboPrintData(reciboId);
  }

  getExpedienteFacturacion(pacienteId: string, searchTerm?: string): Observable<any> {
    let url = `${this.apiUrl}/expediente/${pacienteId}`;
    if (searchTerm) {
      url += `?searchTerm=${encodeURIComponent(searchTerm)}`;
    }
    return this.http.get<any>(url);
  }
}


