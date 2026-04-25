import { Component, inject, OnInit, computed } from '@angular/core';
import { NgFor, NgIf, NgClass, CommonModule } from '@angular/common';
import { AuthService } from '../../core/services/auth.service';
import { SignalrService } from '../../core/services/signalr.service';
import { LucideAngularModule } from 'lucide-angular';

@Component({
  selector: 'app-rx-orders',
  standalone: true,
  imports: [CommonModule, LucideAngularModule],
  templateUrl: './rx-orders.component.html'
})
export class RxOrdersComponent implements OnInit {
  private authService = inject(AuthService);
  private signalRService = inject(SignalrService);

  /**
   * Senior Imaging Strategy (V16.0):
   * Filters incoming tickets based on the user's specialized role (RX vs TOMO).
   * This prevents information leakage between departments while reusing the same UI.
   */
  public filteredTickets = computed(() => {
    const all = this.signalRService.incomingTickets();
    const isRx = this.authService.isRxAssistant();
    const isTomo = this.authService.isTomographyAssistant();
    const isAdmin = this.authService.isAdministrador();

    if (isAdmin) return all;
    if (isRx) return all.filter(t => t.tipoServicio === 'RX');
    if (isTomo) return all.filter(t => t.tipoServicio === 'TOMO');
    
    return []; // No tickets for unauthorized roles
  });

  public incomingTickets = this.filteredTickets;

  public stationName = computed(() => {
    if (this.authService.isTomographyAssistant()) return 'Tomografía';
    if (this.authService.isRxAssistant()) return 'Rayos X';
    return 'Imágenes';
  });

  public stationColor = computed(() => {
    if (this.authService.isTomographyAssistant()) return 'sky';
    return 'rose';
  });

  ngOnInit(): void {
    // Si bien RX es Minimalista, conectamos a SignalR si el operador debe ver colas live
    const token = this.authService.getToken() || '';
    const role = this.authService.currentUser()?.role || '';
    this.signalRService.startConnection(token, role);
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
