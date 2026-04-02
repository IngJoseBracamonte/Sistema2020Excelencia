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
}

@Injectable({
  providedIn: 'root'
})
export class MedicoService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/Medicos`;

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
}
