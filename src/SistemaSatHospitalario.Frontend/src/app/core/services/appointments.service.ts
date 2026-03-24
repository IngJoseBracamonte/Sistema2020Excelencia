import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface Doctor {
  id: string;
  nombre: string;
  especialidad: string;
}

export interface ScheduleEntry {
  hora: string;
  ocupado: boolean;
  reservado: boolean;
  bloqueado: boolean;
  comentario: string;
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
  private baseUrl = `${environment.apiUrl}/Appointments`;

  getDoctorsBySpecialty(specialty: string): Observable<Doctor[]> {
    return this.http.get<Doctor[]>(`${this.baseUrl}/Doctors/${specialty}`);
  }

  getDoctorSchedule(doctorId: string, date: string): Observable<DoctorScheduleResponse> {
    return this.http.get<DoctorScheduleResponse>(`${this.baseUrl}/Schedule/${doctorId}/${date}`);
  }
}
