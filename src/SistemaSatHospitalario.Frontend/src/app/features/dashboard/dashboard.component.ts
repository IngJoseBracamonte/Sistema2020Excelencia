import { Component, effect, inject, OnInit, OnDestroy } from '@angular/core';
import { NgFor, NgIf, NgClass } from '@angular/common';
import { RouterLink } from '@angular/router';
import { SignalrService, TicketUpdate } from '../../core/services/signalr.service';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [NgFor, NgIf, NgClass, RouterLink],
  templateUrl: './dashboard.component.html'
})
export class DashboardComponent implements OnInit, OnDestroy {
  public signalRService = inject(SignalrService);
  public authService = inject(AuthService);

  // Derivando el Token Reactivamente
  private jwtToken = this.authService.getToken() || '';

  // Reactividad Directa
  public userName = this.authService.currentUser()?.username || 'Usuario';
  public tickets = this.signalRService.incomingTickets;

  constructor() {
    // Escucha cambios en tiempo real automáticamente con Effect si fuese necesario
  }

  ngOnInit(): void {
    // Iniciar conexión WebSockets
    this.signalRService.startConnection(this.jwtToken);
  }

  ngOnDestroy(): void {
    // Prevenir fugas de memoria y sockets fantasma (Mitigación del riesgo #2)
    this.signalRService.stopConnection();
  }

  logout(): void {
    this.authService.logout();
  }

  getStatusColor(status: string): string {
    switch (status?.toLowerCase()) {
      case 'completado': return 'bg-emerald-100 text-emerald-800 border-emerald-200';
      case 'en proceso': return 'bg-amber-100 text-amber-800 border-amber-200';
      case 'pendiente': return 'bg-slate-100 text-slate-800 border-slate-200';
      default: return 'bg-hospital-100 text-hospital-800 border-hospital-200';
    }
  }
}
