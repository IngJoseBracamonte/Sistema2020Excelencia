import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class PdfService {
  private http = inject(HttpClient);
  private receiptUrl = `${environment.apiUrl}/api/ReciboFactura`;
  private segurosUrl = `${environment.apiUrl}/api/Seguros`;

  generateRecibo(data: any): Observable<Blob> {
    // Pipeline para generar el PDF de facturación (QuestPDF)
    return this.http.post(`${this.receiptUrl}/GeneratePdf`, data, { responseType: 'blob' });
  }

  generateGarantia(data: any): Observable<Blob> {
    // Pipeline para generar el PDF de Garantía Legal
    return this.http.post(`${this.segurosUrl}/garantia-pago`, data, { responseType: 'blob' });
  }

  generateCompromiso(data: any): Observable<Blob> {
    // Pipeline para generar el PDF de Compromiso de Pago
    return this.http.post(`${this.segurosUrl}/compromiso-pago`, data, { responseType: 'blob' });
  }
}
