import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { TicketError, ResolveTicketRequest } from '../models/ticket.model';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class TicketService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api/tickets`;

  getTickets(resueltos?: boolean): Observable<TicketError[]> {
    let url = this.apiUrl;
    if (resueltos !== undefined) {
      url += `?resueltos=${resueltos}`;
    }
    return this.http.get<TicketError[]>(url);
  }

  resolveTicket(id: string, request: ResolveTicketRequest): Observable<{ message: string }> {
    return this.http.put<{ message: string }>(`${this.apiUrl}/${id}/resolve`, request);
  }
}
