import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ConfiguracionGeneral, UserDto } from '../models/settings.model';

@Injectable({ providedIn: 'root' })
export class SettingsService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/settings`;

  getConfig(): Observable<ConfiguracionGeneral> { 
    return this.http.get<ConfiguracionGeneral>(`${this.apiUrl}/config`); 
  }
  
  updateConfig(config: ConfiguracionGeneral): Observable<any> { 
    return this.http.post(`${this.apiUrl}/config`, config); 
  }
  
  updateTasa(monto: number): Observable<any> { 
    return this.http.post(`${this.apiUrl}/tasa`, { monto }); 
  }
  
  getUsers(): Observable<UserDto[]> { 
    return this.http.get<UserDto[]>(`${this.apiUrl}/users`); 
  }
  
  createUser(user: any): Observable<any> { 
    return this.http.post(`${this.apiUrl}/users`, user); 
  }
  
  getRoles(): Observable<string[]> { 
    return this.http.get<string[]>(`${this.apiUrl}/roles`); 
  }
  
  updateUserRoles(userId: string, roles: string[]): Observable<any> { 
    return this.http.post(`${this.apiUrl}/users/roles`, { userId, roles }); 
  }
}
