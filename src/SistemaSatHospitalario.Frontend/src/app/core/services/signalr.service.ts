import { Injectable, signal } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { environment } from '../../../environments/environment';

export interface TicketUpdate {
  orderId: number;
  status: string;
  patientName: string;
  servicioNombre: string;
  pacienteId?: number;
  tipoServicio: string; // RX o TOMO
}

@Injectable({
  providedIn: 'root'
})
export class SignalrService {
  private hubConnection: signalR.HubConnection | undefined;
  private notificationConnection: signalR.HubConnection | undefined;

  // Array Signal Reactivo
  public incomingTickets = signal<TicketUpdate[]>([]);
  public incomingNotifications = signal<any[]>([]);
  public connectionStatus = signal<string>('Desconectado');

  constructor() { }

  public startConnection = (token: string, role?: string) => {
    // 1. Dashboard Hub
    if (!this.hubConnection || this.hubConnection.state === signalR.HubConnectionState.Disconnected) {
      this.hubConnection = new signalR.HubConnectionBuilder()
        .withUrl(`${environment.apiUrl.replace('/api', '')}/hub/dashboard`, {
          accessTokenFactory: () => token
        })
        .withAutomaticReconnect()
        .build();

      this.hubConnection.start()
        .then(() => {
          this.connectionStatus.set('Conectado');
          this.addTicketUpdatesListener();
        })
        .catch(err => console.error('[SIGNALR DASHBOARD] Error: ' + err));
    }

    // 2. Notification Hub
    if (!this.notificationConnection || this.notificationConnection.state === signalR.HubConnectionState.Disconnected) {
      this.notificationConnection = new signalR.HubConnectionBuilder()
        .withUrl(`${environment.apiUrl.replace('/api', '')}/hub/notifications`, {
          accessTokenFactory: () => token
        })
        .withAutomaticReconnect()
        .build();

      this.notificationConnection.start()
        .then(() => {
          this.addNotificationListener();
          // Join Admin group if role is admin
          if (role?.toLowerCase().includes('admin')) {
            this.notificationConnection?.invoke('JoinGroup', 'Admin');
          }
          // Todos los que ven el monitor se unen a este grupo
          this.notificationConnection?.invoke('JoinGroup', 'ProcessingOrders');
        })
        .catch(err => console.error('[SIGNALR NOTIFY] Error: ' + err));
    }
  }

  private addTicketUpdatesListener = () => {
    this.hubConnection?.on('ReceiveTicketUpdate', (data: TicketUpdate) => {
      this.incomingTickets.update(tickets => [data, ...tickets]);
    });
  }

  private addNotificationListener = () => {
    this.notificationConnection?.on('ReceiveNotification', (data: any) => {
      this.incomingNotifications.update(nots => [data, ...nots]);
      console.log('[SIGNALR NOTIFY] Nueva Notificación:', data);
    });
  }

  public stopConnection = () => {
    this.hubConnection?.stop();
    this.notificationConnection?.stop();
    this.connectionStatus.set('Desconectado');
  }
}
