import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { OrdenCompraInventario, PagoProveedor, RegistrarPagoRequest } from '../models/cuentas-por-pagar.model';

@Injectable({
  providedIn: 'root'
})
export class CuentasPorPagarService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiUrl}/api/CuentasPorPagar`;

  getOrdenes(estado?: string, busqueda?: string, fechaDesde?: string, fechaHasta?: string): Observable<OrdenCompraInventario[]> {
    let params = new HttpParams();
    if (estado) params = params.set('estado', estado);
    if (busqueda) params = params.set('busqueda', busqueda);
    if (fechaDesde) params = params.set('fechaDesde', fechaDesde);
    if (fechaHasta) params = params.set('fechaHasta', fechaHasta);

    return this.http.get<OrdenCompraInventario[]>(`${this.baseUrl}/ordenes`, { params });
  }

  getFacturas(estado?: string, busqueda?: string, fechaDesde?: string, fechaHasta?: string): Observable<OrdenCompraInventario[]> {
    return this.getOrdenes(estado, busqueda, fechaDesde, fechaHasta);
  }

  registrarPago(request: RegistrarPagoRequest): Observable<PagoProveedor> {
    return this.http.post<PagoProveedor>(`${this.baseUrl}/registrar-pago`, request);
  }

  getHistorialPagos(busqueda?: string, fechaDesde?: string, fechaHasta?: string): Observable<PagoProveedor[]> {
    let params = new HttpParams();
    if (busqueda) params = params.set('busqueda', busqueda);
    if (fechaDesde) params = params.set('fechaDesde', fechaDesde);
    if (fechaHasta) params = params.set('fechaHasta', fechaHasta);

    return this.http.get<PagoProveedor[]>(`${this.baseUrl}/historial-pagos`, { params });
  }
}
