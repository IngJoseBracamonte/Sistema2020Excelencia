import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface PatientRecord {
  // Identidad GUID del sistema nuevo (V11.1)
  id: string;
  // Referencia histórica legado
  idPacienteLegacy?: number;
  cedula: string;
  nombre: string;
  apellidos?: string;
  sexo?: string;
  fechaNacimiento?: string;
  correo?: string;
  tipoCorreo?: string;
  celular?: string;
  codigoCelular?: string;
  telefono?: string;
  codigoTelefono?: string;
  source?: string;
  esLegacy?: boolean;
  activo?: boolean;
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

  // patientId ahora es GUID (V11.1 Identity Synchronization)
  getHistory(patientId: string): Observable<PatientHistory[]> {
    return this.http.get<PatientHistory[]>(`${this.apiUrl}/${patientId}/history`);
  }
}
