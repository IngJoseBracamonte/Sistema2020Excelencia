import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface Medico {
  id?: string;
  nombre: string;
  especialidadId?: string;
  especialidad?: string;
  activo: boolean;
  honorarioBase?: number; // V1.0 Security Matrix
  telefono?: string;
}

export interface DoctorHonorariaDto {
  medicoId: string;
  nombre: string;
  especialidad: string;
  honorarioBase: number;
  totalConsultasMes: number;
  activo: boolean;
}

export interface DoctorHonorariumSummaryDto {
  medicoId: string;
  medicoNombre: string;
  cantidadServicios: number;
  totalHonorarios: number;
  desglose: HonorarioDesgloseCategoriaDto[];
}

export interface HonorarioDesgloseCategoriaDto {
  categoria: string;
  cantidad: number;
  total: number;
}

@Injectable({
  providedIn: 'root'
})
export class MedicoService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api/Medicos`;

  getAll(): Observable<Medico[]> {
    return this.http.get<Medico[]>(this.apiUrl);
  }

  create(medico: Partial<Medico>): Observable<string> {
    return this.http.post<string>(this.apiUrl, medico);
  }

  update(medico: Medico): Observable<boolean> {
    return this.http.put<boolean>(this.apiUrl, medico);
  }

  delete(id: string): Observable<boolean> {
    return this.http.delete<boolean>(`${this.apiUrl}/${id}`);
  }

  getHonorariaReport(): Observable<DoctorHonorariaDto[]> {
    return this.http.get<DoctorHonorariaDto[]>(`${this.apiUrl}/reporte/honorarios`);
  }

  getHonorariumSummary(startDate: string, endDate: string): Observable<DoctorHonorariumSummaryDto[]> {
    return this.http.get<DoctorHonorariumSummaryDto[]>(`${this.apiUrl}/reporte/calculo-honorarios?startDate=${startDate}&endDate=${endDate}`);
  }
}
