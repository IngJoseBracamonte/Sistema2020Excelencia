import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';

export interface ExpedienteFacturacionRow {
  id: string;
  fecha: string;
  pacienteNombre: string;
  pacienteCedula: string;
  pacienteTelefono: string;
  estudio: string;
  tipoIngreso: string;
  seguroNombre: string;
  metodoPago: string;
  montoUSD: number;
  facturadoPor: string;
  estado: string;
  tipoServicio: string;
}

export interface ControlCitaRow {
  id: string;
  hora: string;
  pacienteNombre: string;
  pacienteCedula: string;
  pacienteTelefono: string;
  pacienteEdad?: number;
  especialidad: string;
  medico: string;
  formaPago: string;
  montoUSD: number;
  estado: string;
  observaciones: string;
  turno: number;
  cuentaServicioId: string;
}

@Injectable({
  providedIn: 'root'
})
export class ExpedienteService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api/Expediente`;

  getBillingReport(startDate?: string, endDate?: string, searchTerm?: string): Observable<ExpedienteFacturacionRow[]> {
    let url = `${this.apiUrl}/billing?`;
    if (startDate) url += `startDate=${startDate}&`;
    if (endDate) url += `endDate=${endDate}&`;
    if (searchTerm) url += `searchTerm=${searchTerm}&`;
    
    return this.http.get<ExpedienteFacturacionRow[]>(url);
  }

  getControlCitas(date?: string): Observable<ControlCitaRow[]> {
    let url = `${this.apiUrl}/citas`;
    if (date) url += `?date=${date}`;
    return this.http.get<ControlCitaRow[]>(url);
  }

  markAtendida(id: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/citas/${id}/atender`, {});
  }

  cancelCita(id: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/citas/${id}/cancelar`, {});
  }
}
