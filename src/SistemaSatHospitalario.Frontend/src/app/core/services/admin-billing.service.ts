import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface CuentaAdministrativaDetailDto {
  id: string;
  servicioId: string;
  descripcion: string;
  precio: number;
  honorario: number;
  cantidad: number;
  tipoServicio: string;
  fechaCarga: string;
  legacyMappingId?: string;
}

export interface CuentaAdministrativaDto {
  cuentaId: string;
  pacienteId: string;
  pacienteNombre: string;
  pacienteCedula: string;
  fechaCarga: string;
  fechaCierre?: string;
  estado: string;
  tipoIngreso: string;
  convenioId?: number;
  seguroNombre?: string;
  total: number;
  reciboId?: string;
  numeroRecibo?: string;
  detalles: CuentaAdministrativaDetailDto[];
}

export interface DetallePrecioCorreccionDto {
  detalleId: string;
  nuevoPrecio: number;
  nuevoHonorario: number;
  nuevaCantidad?: number;
}

export interface UpdateCuentaAdministrativaCommand {
  cuentaId: string;
  nuevoPacienteId?: string;
  nuevoTipoIngreso?: string;
  nuevoConvenioId?: number;
  correccionesPrecios?: DetallePrecioCorreccionDto[];
}

export interface HistorialModificacionCuentaDto {
  id: string;
  cuentaServicioId: string;
  fechaModificacion: string;
  usuario: string;
  pacienteAnteriorId?: string;
  pacienteAnteriorNombre?: string;
  pacienteNuevoId?: string;
  pacienteNuevoNombre?: string;
  tipoIngresoAnterior?: string;
  tipoIngresoNuevo?: string;
  convenioAnteriorId?: number;
  convenioAnteriorNombre?: string;
  convenioNuevoId?: number;
  convenioNuevoNombre?: string;
  totalAnteriorUSD: number;
  totalNuevoUSD: number;
  reciboTotalAnteriorUSD: number;
  reciboTotalNuevoUSD: number;
  reciboVueltoAnteriorUSD: number;
  reciboVueltoNuevoUSD: number;
  reciboPagadoUSD: number;
  cxcSaldoAnteriorUSD: number;
  cxcSaldoNuevoUSD: number;
  detalleServiciosCambiosJson?: string;
}

@Injectable({
  providedIn: 'root'
})
export class AdminBillingService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api/Billing`;

  getCuentasAdministrativas(searchTerm?: string, tipoIngreso?: string, estado?: string): Observable<CuentaAdministrativaDto[]> {
    let params = new HttpParams();
    if (searchTerm) params = params.set('searchTerm', searchTerm);
    if (tipoIngreso) params = params.set('tipoIngreso', tipoIngreso);
    if (estado) params = params.set('estado', estado);
    
    return this.http.get<CuentaAdministrativaDto[]>(`${this.apiUrl}/cuentas-administrativas`, { params });
  }

  updateCuentaAdministrativa(command: UpdateCuentaAdministrativaCommand): Observable<any> {
    return this.http.post(`${this.apiUrl}/update-cuenta-administrativa`, command);
  }

  getHistorialModificaciones(cuentaId: string): Observable<HistorialModificacionCuentaDto[]> {
    return this.http.get<HistorialModificacionCuentaDto[]>(`${this.apiUrl}/cuenta-historial/${cuentaId}`);
  }
}
