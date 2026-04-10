import { Injectable, inject, effect } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject, first } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ConfiguracionGeneral, UserDto } from '../models/settings.model';
import { AuthService } from './auth.service';
import * as signalR from '@microsoft/signalr';

@Injectable({ providedIn: 'root' })
export class SettingsService {
  private http = inject(HttpClient);
  private authService = inject(AuthService);
  private apiUrl = `${environment.apiUrl}/global-settings`;
  private hubUrl = `${environment.apiUrl.replace('/api', '')}/hub/tasa`;
  
  private hubConnection!: signalR.HubConnection;
  private _tasaSubject = new BehaviorSubject<number>(0);
  public tasa$ = this._tasaSubject.asObservable();

  constructor() {
    this.refreshTasa();
    this.initSignalR();

    // Re-cargar tasa cuando cambia el usuario (por si acaso el anonimo falló o necesitamos sesión)
    effect(() => {
      const user = this.authService.currentUser();
      if (user) {
        this.refreshTasa();
      }
    });
  }

  refreshTasa() {
    this.getTasa().pipe(first()).subscribe({
      next: (res) => {
        if (res && res.monto > 0) {
          this._tasaSubject.next(res.monto);
          console.log(`[SettingsService] Tasa cargada: ${res.monto}`);
        }
      },
      error: (err) => console.error('[SettingsService] No se pudo cargar la tasa inicial. Es posible que falte autenticación.', err)
    });
  }

  private initSignalR() {
    // Senior Pattern: Flexible URL for SignalR on Proxy Environments
    // No usamos connectionUrl directamente en .withUrl para permitir que SignalR maneje la base
    
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(this.hubUrl, { 
        skipNegotiation: false, 
        transport: signalR.HttpTransportType.WebSockets | signalR.HttpTransportType.LongPolling
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start()
      .then(() => console.log('[SettingsService] SignalR: Conectado al Hub de Tasas'))
      .catch(err => console.error('[SettingsService] Error al iniciar SignalR Tasa:', err));

    this.hubConnection.on('TasaActualizada', (nuevaTasa: number) => {
      this._tasaSubject.next(nuevaTasa);
      console.log(`[SettingsService] Tasa actualizada vía SignalR: ${nuevaTasa}`);
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
