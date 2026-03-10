import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';

export interface AbrirCajaRequest {
  montoInicialDivisa: number;
  montoInicialBs: number;
}

export interface AbrirTurnoRequest {
  cajeroUserId: string;
}

export interface ResumenTurnoDto {
  turnoId: string;
  cajeroUserId: string;
  estado: string;
  recaudadoBase: number;
}

export interface ResumenCajaGlobalDto {
  cajaId: string;
  fechaApertura: string;
  montoInicialBase: number;
  totalRecaudadoBase: number;
  granTotalEnCajaBase: number;
  turnos: ResumenTurnoDto[];
}

@Injectable({
  providedIn: 'root'
})
export class CajaService {
  private http = inject(HttpClient);
  private baseUrl = 'https://localhost:7019/api/Caja'; // Puerto por defecto del backend

  // Signals reactivos para observar el estado local
  public isCajaAbierta = signal<boolean>(false);

  abrirCaja(payload: AbrirCajaRequest): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/Abrir`, payload).pipe(
      tap(() => this.isCajaAbierta.set(true))
    );
  }

  cerrarCaja(): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/Cerrar`, {}).pipe(
      tap(() => {
        this.isCajaAbierta.set(false);
      })
    );
  }

  obtenerResumenDiario(): Observable<ResumenCajaGlobalDto> {
    return this.http.get<ResumenCajaGlobalDto>(`${this.baseUrl}/Resumen`);
  }
}
