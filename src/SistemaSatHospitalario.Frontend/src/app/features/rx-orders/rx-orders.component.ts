import { Component, inject, OnInit, computed, signal } from '@angular/core';
import { NgFor, NgIf, NgClass, CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { AuthService } from '../../core/services/auth.service';
import { SignalrService } from '../../core/services/signalr.service';
import { LucideAngularModule } from 'lucide-angular';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-rx-orders',
  standalone: true,
  imports: [CommonModule, LucideAngularModule],
  templateUrl: './rx-orders.component.html'
})
export class RxOrdersComponent implements OnInit {
  private authService = inject(AuthService);
  private signalRService = inject(SignalrService);
  private route = inject(ActivatedRoute);
  private http = inject(HttpClient);

  // Buffer local para manejar persistencia + live updates
  private localTickets = signal<any[]>([]);

  /**
   * Senior Imaging Strategy (V16.2):
   * Differentiates station identity based on the current URL path.
   */
  public isTomoRoute = computed(() => this.route.snapshot.url[0]?.path === 'tomo-orders');

  // Combinamos los tickets iniciales cargados por API con los que llegan por SignalR
  public incomingTickets = computed(() => {
    const live = this.signalRService.incomingTickets();
    const local = this.localTickets();
    const isTomo = this.isTomoRoute();

    // Filtrar por tipo según ruta
    const combined = [...live, ...local].filter(t => t.tipoServicio === (isTomo ? 'TOMO' : 'RX'));
    
    // Eliminar duplicados por orderId (por si SignalR y API traen lo mismo en el solapamiento)
    return combined.filter((v, i, a) => a.findIndex(t => t.orderId === v.orderId) === i);
  });

  public stationName = computed(() => {
    return this.isTomoRoute() ? 'Tomografía' : 'RX';
  });

  public stationColor = computed(() => {
    return this.isTomoRoute() ? 'sky' : 'rose';
  });

  ngOnInit(): void {
    const token = this.authService.getToken() || '';
    const role = this.authService.currentUser()?.role || '';
    
    // 1. Iniciar conexión SignalR
    this.signalRService.startConnection(token, role);

    // 2. Cargar órdenes pendientes desde el API real (V16.2 Persistence)
    this.loadPendingOrders();
  }

  private loadPendingOrders(): void {
    const type = this.isTomoRoute() ? 'TOMO' : 'RX';
    this.http.get<any[]>(`${environment.apiUrl}/api/Imaging/pending?type=${type}`)
      .subscribe({
        next: (orders) => {
          // Mapeamos al formato TicketUpdate que espera el servicio
          const mapped = orders.map(o => ({
            orderId: o.id,
            status: o.estado,
            patientName: o.pacienteNombre,
            servicioNombre: o.estudio,
            tipoServicio: o.tipoServicio
          }));
          this.localTickets.set(mapped);
        },
        error: (err) => console.error('[IMAGING] Error cargando órdenes:', err)
      });
  }

  confirmarProceso(id: number): void {
    this.http.post(`${environment.apiUrl}/api/Imaging/${id}/complete`, {})
      .subscribe({
        next: () => {
          // Remover localmente para feedback inmediato
          this.localTickets.update(tickets => tickets.filter(t => t.orderId !== id));
          // También limpiar en el signalr service si estaba ahí
          this.signalRService.incomingTickets.update(tickets => tickets.filter(t => t.orderId !== id));
        },
        error: (err) => alert('Error al procesar la orden: ' + err.message)
      });
  }

  logout(): void {
    this.authService.logout();
    this.signalRService.stopConnection();
  }
}
