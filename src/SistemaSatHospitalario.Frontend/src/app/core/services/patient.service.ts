import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface PatientRecord {
  // Id numérico igual a IdPersona del Sistema Legacy
  id?: number;
  cedula: string;
  nombre: string;
  apellidos: string;
  sexo: string;
  correo: string;
  celular: string;
  source: string;
  esLegacy: boolean;
}

export interface HistoryServiceDetail {
  descripcion: string;
  precio: number;
  cantidad: number;
  tipoServicio: string;
}

export interface PatientHistory {
  cuentaId: string;
  fechaCreacion: string;
  fechaCierre: string;
  estado: string;
  tipoIngreso: string;
  total: number;
  servicios: HistoryServiceDetail[];
}

@Injectable({
  providedIn: 'root'
})
export class PatientService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/Patient`;

  searchPatients(term: string): Observable<PatientRecord[]> {
    return this.http.get<PatientRecord[]>(`${this.apiUrl}/search?term=${term}`);
  }

  createPatient(patient: Partial<PatientRecord>): Observable<PatientRecord> {
    return this.http.post<PatientRecord>(this.apiUrl, patient);
  }

  // patientId ahora es numérico para coincidir con Legacy
  getHistory(patientId: number): Observable<PatientHistory[]> {
    return this.http.get<PatientHistory[]>(`${this.apiUrl}/${patientId}/history`);
  }
}
