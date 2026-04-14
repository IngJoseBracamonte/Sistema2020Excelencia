import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface Especialidad {
  id: string;
  nombre: string;
  activo: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class SpecialtyService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiUrl}/api/Especialidades`;

  getAll(): Observable<Especialidad[]> {
    return this.http.get<Especialidad[]>(this.baseUrl);
  }

  create(especialidad: Partial<Especialidad>): Observable<string> {
    return this.http.post<string>(this.baseUrl, especialidad);
  }

  update(id: string, especialidad: Partial<Especialidad>): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, especialidad);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
