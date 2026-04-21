import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface Doctor {
  id: string;
  nombre: string;
  especialidad: string;
  telefono?: string;
}

export interface ScheduleEntry {
  hora: string;
  ocupado: boolean;
  reservado: boolean;
  bloqueado: boolean;
  comentario: string;
  targetId?: string;
  type?: 'Reserva' | 'Cita' | 'Bloqueo';
}

export interface DoctorScheduleResponse {
  medicoId: string;
  fecha: string;
  turnos: ScheduleEntry[];
}

@Injectable({
  providedIn: 'root'
})
export class AppointmentsService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiUrl}/api/Appointments`;

  getDoctorsBySpecialty(specialty: string): Observable<Doctor[]> {
    return this.http.get<Doctor[]>(`${this.baseUrl}/Doctors/${specialty}`);
  }

  getDoctorSchedule(doctorId: string, date: string): Observable<DoctorScheduleResponse> {
    return this.http.get<DoctorScheduleResponse>(`${this.baseUrl}/Schedule/${doctorId}/${date}`);
  }

  getDoctorScheduleWithPatient(doctorId: string, date: string, pacienteId?: string): Observable<DoctorScheduleResponse> {
    let url = `${this.baseUrl}/Schedule/${doctorId}/${date}`;
    if (pacienteId) url += `?pacienteId=${pacienteId}`;
    return this.http.get<DoctorScheduleResponse>(url);
  }

  adminManageSchedule(data: { action: 'Delete' | 'Update', type: string, targetId: string, newTime?: string }): Observable<any> {
    return this.http.post(`${this.baseUrl}/Admin/Manage`, data);
  }

  getActiveAppointments(fecha?: string, medicoId?: string): Observable<any[]> {
    let url = `${environment.apiUrl}/api/Billing/Appointments`;
    const params = [];
    if (fecha) params.push(`fecha=${fecha}`);
    if (medicoId) params.push(`medicoId=${medicoId}`);
    if (params.length > 0) url += `?${params.join('&')}`;
    return this.http.get<any[]>(url);
  }
}
