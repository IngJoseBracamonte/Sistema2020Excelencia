import { Component, inject, OnInit } from '@angular/core';
import { NgFor, NgIf } from '@angular/common';
import { AuthService } from '../../core/services/auth.service';
import { SignalrService } from '../../core/services/signalr.service';

@Component({
  selector: 'app-rx-orders',
  standalone: true,
  imports: [NgFor, NgIf],
  templateUrl: './rx-orders.component.html'
})
export class RxOrdersComponent implements OnInit {
  private authService = inject(AuthService);
  private signalRService = inject(SignalrService);

  public incomingTickets = this.signalRService.incomingTickets;

  ngOnInit(): void {
    // Si bien RX es Minimalista, conectamos a SignalR si el operador debe ver colas live
    this.signalRService.startConnection(this.authService.getToken() || '');
  }

  logout(): void {
    this.authService.logout();
    this.signalRService.stopConnection();
  }

  confirmarProceso(id: number): void {
    alert(`Procesamiento Exitoso de Rayos X: #${id}`);
    // Aca llamaríamos al WebAPI => C# MediatoR => "ConfirmarOrdenRXCommand"
  }
}
