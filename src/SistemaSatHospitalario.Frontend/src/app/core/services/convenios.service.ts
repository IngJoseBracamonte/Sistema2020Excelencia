import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { SeguroConvenio } from '../models/convenio.model';

@Injectable({ providedIn: 'root' })
export class ConveniosService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/convenios`;

  getAll(): Observable<SeguroConvenio[]> { 
    return this.http.get<SeguroConvenio[]>(this.apiUrl); 
  }
  
  create(convenio: Partial<SeguroConvenio>): Observable<number> { 
    return this.http.post<number>(this.apiUrl, convenio); 
  }
  
  update(convenio: SeguroConvenio): Observable<any> { 
    return this.http.put(this.apiUrl, convenio); 
  }
  
  delete(id: number): Observable<any> { 
    return this.http.delete(`${this.apiUrl}/${id}`); 
  }

  getPrecios(id: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/${id}/precios`);
  }

  updatePrecio(command: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/precios`, command);
  }
}
