import { Component, inject, signal, computed, OnInit, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { AuthService } from '../../../core/services/auth.service';
import { SignalrService } from '../../../core/services/signalr.service';
import { environment } from '../../../../environments/environment';
import { 
  LucideAngularModule, 
  Search, 
  RefreshCcw, 
  Check, 
  X, 
  ChevronRight, 
  ChevronDown, 
  AlertCircle,
  Info,
  Calendar,
  Camera,
  Activity,
  User,
  Clock,
  Trash2,
  Tv
} from 'lucide-angular';

export interface ImagingOrder {
  id: number;
  cuentaId: string;
  pacienteId: string;
  pacienteNombre: string;
  estudio: string;
  tipoServicio: string; // RX o TOMO
  estado: string; // Pendiente, Procesado, Anulado
  fechaCreacion: string;
  procesadoPor?: string;
  fechaProcesado?: string;
}

@Component({
  selector: 'app-imaging-monitor',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule],
  templateUrl: './imaging-monitor.component.html',
  styleUrl: './imaging-monitor.component.css'
})
export class ImagingMonitorComponent implements OnInit {
  readonly icons = {
    Search,
    RefreshCcw,
    Check,
    X,
    ChevronRight,
    ChevronDown,
    AlertCircle,
    Info,
    Calendar,
    Camera,
    Activity,
    User,
    Clock,
    Trash2,
    Tv
  };

  private http = inject(HttpClient);
  private authService = inject(AuthService);
  private signalRService = inject(SignalrService);

  // Filters & State Signals
  public orders = signal<ImagingOrder[]>([]);
  public searchTerm = signal<string>('');
  public filterType = signal<string>('ALL'); // ALL, RX, TOMO
  public filterStatus = signal<string>('ALL'); // ALL, Pendiente, Procesado, Anulado
  public startDate = signal<string>(new Date().toLocaleDateString('sv-SE'));
  public endDate = signal<string>(new Date().toLocaleDateString('sv-SE'));
  public isLoading = signal<boolean>(false);
  public actionMessage = signal<string | null>(null);

  // Accordion Logic
  public expandedRows = signal<Set<number>>(new Set<number>());

  // Wow Metrics Computations
  public totalToday = computed(() => this.orders().length);
  
  public pendingCount = computed(() => 
    this.orders().filter(o => o.estado === 'Pendiente').length
  );
  
  public processedCount = computed(() => 
    this.orders().filter(o => o.estado === 'Procesado').length
  );

  public averageResponseTime = computed(() => {
    const processed = this.orders().filter(o => o.estado === 'Procesado' && o.fechaProcesado && o.fechaCreacion);
    if (processed.length === 0) return 0;
    
    const totalMinutes = processed.reduce((acc, curr) => {
      const start = new Date(curr.fechaCreacion).getTime();
      const end = new Date(curr.fechaProcesado!).getTime();
      const diffMin = Math.max(0, (end - start) / 60000);
      return acc + diffMin;
    }, 0);

    return Math.round(totalMinutes / processed.length);
  });

  constructor() {
    // Reactive sync with SignalR: Whenever new tickets are received/updated, trigger refresh
    effect(() => {
      const tickets = this.signalRService.incomingTickets();
      if (tickets && tickets.length > 0) {
        this.refresh();
      }
    });
  }

  ngOnInit() {
    const token = this.authService.getToken() || '';
    const role = this.authService.currentUser()?.role || '';
    
    // Start SignalR Connection for live updates
    this.signalRService.startConnection(token, role);
    
    this.refresh();
  }

  refresh() {
    this.isLoading.set(true);
    const params: any = {
      type: this.filterType(),
      status: this.filterStatus(),
      startDate: this.startDate(),
      endDate: this.endDate()
    };

    if (this.searchTerm().trim()) {
      params.search = this.searchTerm().trim();
    }

    this.http.get<ImagingOrder[]>(`${environment.apiUrl}/api/Imaging/history`, { params })
      .subscribe({
        next: (res) => {
          this.orders.set(res);
          this.isLoading.set(false);
        },
        error: (err) => {
          console.error('[IMAGING-MONITOR] Error fetching history:', err);
          this.isLoading.set(false);
        }
      });
  }

  toggleRow(id: number) {
    const next = new Set(this.expandedRows());
    if (next.has(id)) next.delete(id);
    else next.add(id);
    this.expandedRows.set(next);
  }

  isExpanded(id: number): boolean {
    return this.expandedRows().has(id);
  }

  cancelOrder(order: ImagingOrder) {
    let confirmMessage = `¿Está seguro de que desea anular esta orden pendiente para ${order.pacienteNombre}?`;
    
    if (order.estado === 'Procesado') {
      const formattedDate = new Date(order.fechaProcesado!).toLocaleString('es-ES', {
        day: '2-digit',
        month: '2-digit',
        year: 'numeric',
        hour: '2-digit',
        minute: '2-digit'
      });
      confirmMessage = `ATENCIÓN: Esta orden ya fue procesada/validada por el asistente ${order.procesadoPor || 'desconocido'} el día ${formattedDate}.\n\n¿Está seguro de que desea anularla de todas formas?`;
    }

    if (confirm(confirmMessage)) {
      this.isLoading.set(true);
      this.http.post(`${environment.apiUrl}/api/Imaging/${order.id}/cancel`, {})
        .subscribe({
          next: () => {
            this.actionMessage.set(`La orden de ${order.pacienteNombre} ha sido anulada exitosamente.`);
            this.refresh();
            setTimeout(() => this.actionMessage.set(null), 5000);
          },
          error: (err) => {
            console.error('[IMAGING-MONITOR] Error canceling order:', err);
            alert('Error al anular la orden: ' + (err.error?.Message || err.message));
            this.isLoading.set(false);
          }
        });
    }
  }

  getResponseTime(order: ImagingOrder): number {
    if (!order.fechaProcesado) return 0;
    const start = new Date(order.fechaCreacion).getTime();
    const end = new Date(order.fechaProcesado).getTime();
    return Math.max(0, Math.round((end - start) / 60000));
  }
}
