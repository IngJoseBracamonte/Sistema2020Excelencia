import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { tap, catchError } from 'rxjs/operators';
import { Observable, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';
import { StorageService } from './storage.service';
import { UserRole, RoleGroups } from '../models/user-role.enum';

export interface LoginRequest {
  username: string;
  password: string;
}

export interface AuthResponse {
  token: string;
  username: string;
  role: string;
  id: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private http = inject(HttpClient);
  private router = inject(Router);
  private storage = inject(StorageService);
  private apiUrl = `${environment.apiUrl}/Auth/Login`;

  // Estado reactivo (Signal)
  public currentUser = signal<AuthResponse | null>(this.getUserFromSession());

  // Helpers de Roles Normalizados (Senior Standardized)
  private hasRole(role: UserRole | string): boolean {
    const currentRole = this.currentUser()?.role;
    return currentRole?.toLowerCase() === role?.toLowerCase();
  }

  private isInGroup(roles: string[]): boolean {
    const currentRole = this.currentUser()?.role || '';
    return roles.some(r => r.toLowerCase() === currentRole.toLowerCase());
  }

  public isAdministrador = (): boolean => 
    this.isInGroup(RoleGroups.Administrative);
  
  public isParticularAssistant = (): boolean => 
    this.hasRole(UserRole.AsistenteParticular) || this.isAdministrador();

  public isInsuranceAssistant = (): boolean => 
    this.hasRole(UserRole.AsistenteSeguro) || this.hasRole(UserRole.AsistenteSeguros) || this.isAdministrador();

  public isSupervisor = (): boolean => 
    this.hasRole(UserRole.Supervisor) || this.isAdministrador();

  public isCajero = (): boolean => this.isParticularAssistant() || this.isInsuranceAssistant();
  
  public isMedico = (): boolean => this.hasRole(UserRole.Medico);
  
  public isFarmacia = (): boolean => {
    const role = this.currentUser()?.role?.toLowerCase() || '';
    return role === 'rx' || role === 'farmacia' || role === 'asistente rx' || this.isAdministrador();
  };

  // Recupera la sesión persistida (Abstracted via StorageService)
  private getUserFromSession(): AuthResponse | null {
    const data = this.storage.getAuthData();
    if (data.token && data.username && data.role && data.id) {
      return { 
        token: data.token, 
        username: data.username, 
        role: data.role, 
        id: data.id 
      };
    }
    return null;
  }

  login(credentials: LoginRequest): Observable<AuthResponse> {
    return this.http.post<any>(this.apiUrl, credentials).pipe(
      tap(response => {
        const id = response.userId || response.id;
        
        // Almacenar usando el nuevo StorageService
        this.storage.saveAuthData(response.token, response.username, response.role, id);

        const authResp: AuthResponse = {
          token: response.token,
          username: response.username,
          role: response.role,
          id: id
        };

        // Actualizar UI reactiva
        this.currentUser.set(authResp);
      }),
      catchError(err => throwError(() => new Error('Credenciales inválidas o Error en el Servidor')))
    );
  }

  logout(): void {
    this.storage.clearAuthData();
    this.currentUser.set(null);
    this.router.navigate(['/login']);
  }

  getToken(): string | null {
    return this.currentUser()?.token || null;
  }

  isAuthenticated(): boolean {
    return !!this.getToken();
  }
}
