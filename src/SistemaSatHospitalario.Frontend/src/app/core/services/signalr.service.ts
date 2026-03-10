import { Injectable, signal } from '@angular/core';
import * as signalR from '@microsoft/signalr';

export interface TicketUpdate {
  orderId: number;
  status: string;
  patientName: string;
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
    this.hubConnection = new signalR.HubConnectionBuilder()
      // URL de Kestrel para Hubs - Configurar el Access_Token factory para WS PWA
      .withUrl('https://localhost:7019/hub/dashboard', {
        accessTokenFactory: () => token
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .then(() => {
        this.connectionStatus.set('Conectado');
        this.addTicketUpdatesListener();
      })
      .catch(err => console.log('Error estableciendo conexión PWA-SignalR: ' + err));

    this.hubConnection.onreconnecting(() => this.connectionStatus.set('Reconectando...'));
    this.hubConnection.onreconnected(() => this.connectionStatus.set('Conectado'));
    this.hubConnection.onclose(() => this.connectionStatus.set('Desconectado'));
  }

  private addTicketUpdatesListener = () => {
    this.hubConnection?.on('ReceiveTicketUpdate', (data: TicketUpdate) => {
      // Angular 19 Signals Update - Inmuatibilidad
      this.incomingTickets.update(tickets => [data, ...tickets]);
    });
  }

  public stopConnection = () => {
    this.hubConnection?.stop();
    this.connectionStatus.set('Desconectado');
  }
}
