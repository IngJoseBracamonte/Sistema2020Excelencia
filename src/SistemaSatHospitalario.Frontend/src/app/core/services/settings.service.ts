import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject, first } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ConfiguracionGeneral, UserDto } from '../models/settings.model';
import * as signalR from '@microsoft/signalr';

@Injectable({ providedIn: 'root' })
export class SettingsService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/global-settings`;
  private hubUrl = `${environment.apiUrl.replace('/api', '')}/hub/tasa`;
  
  private hubConnection!: signalR.HubConnection;
  private _tasaSubject = new BehaviorSubject<number>(0);
  public tasa$ = this._tasaSubject.asObservable();

  constructor() {
    this.initSignalR();
    // Carga inicial
    this.getTasa().pipe(first()).subscribe(res => this._tasaSubject.next(res.monto));
  }

  private initSignalR() {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(this.hubUrl, {
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start()
      .then(() => console.log('SignalR: Conectado al Hub de Tasas'))
      .catch(err => console.error('Error al iniciar SignalR Tasa:', err));

    this.hubConnection.on('TasaActualizada', (nuevaTasa: number) => {
      this._tasaSubject.next(nuevaTasa);
    });
  }

  getConfig(): Observable<ConfiguracionGeneral> { 
    return this.http.get<ConfiguracionGeneral>(`${this.apiUrl}/config`); 
  }
  
  updateConfig(config: ConfiguracionGeneral): Observable<any> { 
    return this.http.post(`${this.apiUrl}/config`, config); 
  }
  
  getTasa(): Observable<{monto: number}> {
    return this.http.get<{monto: number}>(`${this.apiUrl}/tasa`);
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
