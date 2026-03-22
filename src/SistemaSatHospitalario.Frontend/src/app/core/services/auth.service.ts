import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { tap, catchError } from 'rxjs/operators';
import { Observable, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';

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
  private apiUrl = `${environment.apiUrl}/Auth/Login`;

  // Estado reactivo (Signal)
  public currentUser = signal<AuthResponse | null>(this.getUserFromStorage());

  // Recupera la sesión persistida previamente (PWA LocalStorage)
  private getUserFromStorage(): AuthResponse | null {
    const token = localStorage.getItem('jwt_token');
    const username = localStorage.getItem('username');
    const role = localStorage.getItem('user_role');
    const id = localStorage.getItem('user_id');

    if (token && username && role && id) {
      return { token, username, role, id };
    }
    return null;
  }

  login(credentials: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(this.apiUrl, credentials).pipe(
      tap(response => {
        // Almacenar en local storage (PWA friendly offline checks in future)
        localStorage.setItem('jwt_token', response.token);
        localStorage.setItem('username', response.username);
        localStorage.setItem('user_role', response.role);
        localStorage.setItem('user_id', response.id);

        // Actualizar UI reactiva
        this.currentUser.set(response);
      }),
      catchError(err => throwError(() => new Error('Credenciales inválidas o Error en el Servidor')))
    );
  }

  logout(): void {
    localStorage.removeItem('jwt_token');
    localStorage.removeItem('username');
    localStorage.removeItem('user_role');
    localStorage.removeItem('user_id');

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
