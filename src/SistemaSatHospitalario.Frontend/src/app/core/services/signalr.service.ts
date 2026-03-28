import { Injectable, signal } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { environment } from '../../../environments/environment';

export interface TicketUpdate {
  orderId: number;
  status: string;
  patientName: string;
  servicioNombre: string;
  pacienteId: number;
}

@Injectable({
  providedIn: 'root'
})
export class SignalrService {
  private hubConnection: signalR.HubConnection | undefined;

  // Array Signal Reactivo
  public incomingTickets = signal<TicketUpdate[]>([]);
  public connectionStatus = signal<string>('Desconectado');

  constructor() { }

  public startConnection = (token: string) => {
    // Evitar múltiples llamadas al inicializador (Senior Patterns V11.0)
    if (this.hubConnection && this.hubConnection.state !== signalR.HubConnectionState.Disconnected) {
      console.log(`[SIGNALR DEBUG] Ignorando inicio: Estado actual es ${this.hubConnection.state}`);
      return;
    }

    this.hubConnection = new signalR.HubConnectionBuilder()
      // URL de Kestrel para Hubs - Configurar el Access_Token factory para WS PWA
      .withUrl(`${environment.apiUrl.replace('/api', '')}/hub/dashboard`, {
        accessTokenFactory: () => token
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .then(() => {
        this.connectionStatus.set('Conectado');
        this.addTicketUpdatesListener();
        console.log('[SIGNALR DEBUG] Conexión establecida exitosamente.');
      })
      .catch(err => console.error('[SIGNALR DEBUG] Error estableciendo conexión: ' + err));

    this.hubConnection.onreconnecting(() => this.connectionStatus.set('Reconectando...'));
    this.hubConnection.onreconnected(() => this.connectionStatus.set('Conectado'));
    this.hubConnection.onclose(() => this.connectionStatus.set('Desconectado'));
  }

  private addTicketUpdatesListener = () => {
    this.incomingTickets.set([]); // Limpiar previas (Reset V2.5)
    this.hubConnection?.on('ReceiveTicketUpdate', (data: TicketUpdate) => {
      // Angular 19 Signals Update - Inmuatibilidad
      this.incomingTickets.update(tickets => [data, ...tickets]);
    });
  }

  public stopConnection = () => {
    if (this.hubConnection && this.hubConnection.state !== signalR.HubConnectionState.Disconnected) {
      this.hubConnection.stop()
        .then(() => {
          this.connectionStatus.set('Desconectado');
          console.log('[SIGNALR DEBUG] Conexión cerrada intencionalmente.');
        })
        .catch(err => console.error('[SIGNALR DEBUG] Error cerrando conexión: ' + err));
    }
  }
}
