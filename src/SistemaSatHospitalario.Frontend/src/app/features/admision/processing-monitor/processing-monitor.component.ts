import { Component, inject, OnInit, signal, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { LucideAngularModule, Monitor, RefreshCw, Clock, FlaskConical, ClipboardCheck } from 'lucide-angular';
import { environment } from '../../../../environments/environment';
import { SignalrService } from '../../../core/services/signalr.service';
import { AuthService } from '../../../core/services/auth.service';
 
interface MonitoringOrder {
  cuentaId: string;
  legacyOrderId: number;
  pacienteNombre: string;
  pacienteCedula: string;
  fechaCarga: string;
  estado: string;
}
 
@Component({
  selector: 'app-processing-monitor',
  standalone: true,
  imports: [CommonModule, LucideAngularModule],
  template: `
    <div class="space-y-8 animate-fade-in p-6 md:p-8 relative z-10">
        <!-- Header -->
        <div class="bg-surface-card/60 backdrop-blur-2xl p-8 rounded-[2.5rem] shadow-sm border border-glass-border flex items-center justify-between">
            <div class="flex items-center space-x-6">
                <div class="h-16 w-16 bg-blue-500/10 text-blue-500 rounded-2xl flex items-center justify-center border border-blue-500/20">
                    <lucide-icon [name]="icons.Monitor" class="w-8 h-8"></lucide-icon>
                </div>
                <div>
                    <h1 class="text-3xl font-black text-white tracking-tighter uppercase">Monitor de Procesamiento</h1>
                    <p class="text-[10px] font-black text-slate-500 uppercase tracking-[0.3em] mt-1">Órdenes de Laboratorio Legacy en Tiempo Real</p>
                </div>
            </div>
            <div class="flex items-center space-x-4">
                <div class="px-6 py-3 bg-black/20 rounded-2xl border border-white/5">
                    <span class="text-[10px] font-black text-slate-400 uppercase tracking-widest block">Total Pendientes</span>
                    <span class="text-xl font-black text-white">{{ pendingCount() }}</span>
                </div>
                <button (click)="loadOrders()" 
                    class="p-4 bg-surface-card hover:bg-white/5 rounded-2xl border border-glass-border transition-all">
                    <lucide-icon [name]="icons.Refresh" class="w-5 h-5 text-slate-400"></lucide-icon>
                </button>
            </div>
        </div>

        <!-- Orders Grid -->
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
            <div *ngFor="let order of orders()" 
                class="bg-surface-card p-6 rounded-[2rem] border border-glass-border transition-all hover:scale-[1.02] hover:border-primary/30 relative overflow-hidden group">
                
                <!-- Status Badge -->
                <div class="absolute top-6 right-6">
                    <div [ngClass]="{
                        'bg-amber-500/10 text-amber-500 border-amber-500/20': order.estado === 'PENDIENTE',
                        'bg-emerald-500/10 text-emerald-500 border-emerald-500/20': order.estado === 'PROCESADA'
                    }" class="px-4 py-1.5 rounded-full border text-[9px] font-black uppercase tracking-widest shadow-sm">
                        {{ order.estado }}
                    </div>
                </div>

                <!-- Order ID -->
                <div class="h-12 w-12 bg-slate-500/10 text-slate-400 rounded-xl flex items-center justify-center mb-6 border border-white/5">
                    <span class="text-sm font-black">#{{ order.legacyOrderId }}</span>
                </div>

                <div class="space-y-4">
                    <div>
                        <h3 class="text-base font-black text-white uppercase truncate tracking-tight">{{ order.pacienteNombre }}</h3>
                        <p class="text-[10px] text-slate-500 font-bold uppercase tracking-widest">{{ order.pacienteCedula }}</p>
                    </div>

                    <div class="flex items-center text-slate-400">
                        <lucide-icon [name]="icons.Clock" class="w-3.5 h-3.5 mr-2 opacity-50"></lucide-icon>
                        <span class="text-[10px] font-mono">{{ order.fechaCarga | date:'dd/MM/yyyy HH:mm' }}</span>
                    </div>

                    <!-- Progress Bar (Fake Visual) -->
                    <div class="pt-2">
                        <div class="h-1.5 w-full bg-black/40 rounded-full overflow-hidden border border-white/5">
                            <div [ngClass]="{
                                'w-1/2 bg-amber-500': order.estado === 'PENDIENTE',
                                'w-full bg-emerald-500 shadow-[0_0_15px_rgba(16,185,129,0.3)]': order.estado === 'PROCESADA'
                            }" class="h-full transition-all duration-1000 ease-out"></div>
                        </div>
                    </div>
                </div>

                <!-- Icon decoration -->
                <lucide-icon [name]="icons.Lab" 
                    class="absolute -bottom-4 -right-4 w-24 h-24 text-white/[0.02] transform -rotate-12 group-hover:text-primary/[0.05] transition-colors"></lucide-icon>
            </div>
        </div>

        <!-- Empty State -->
        <div *ngIf="orders().length === 0" class="bg-surface-card p-20 rounded-[3rem] border border-glass-border flex flex-col items-center justify-center text-center">
            <div class="h-20 w-20 bg-slate-500/5 rounded-3xl flex items-center justify-center mb-6">
                <lucide-icon [name]="icons.Empty" class="w-10 h-10 text-slate-700"></lucide-icon>
            </div>
            <h2 class="text-xl font-black text-slate-400 uppercase tracking-tight">Sin órdenes para mostrar</h2>
            <p class="text-xs text-slate-600 mt-2">No hay órdenes de laboratorio en el ciclo de monitoreo actual.</p>
        </div>
    </div>
  `,
  styles: [`
    .animate-fade-in { animation: fadeIn 0.5s ease-out; }
    @keyframes fadeIn { from { opacity: 0; transform: translateY(10px); } to { opacity: 1; transform: translateY(0); } }
  `]
})
export class ProcessingMonitorComponent implements OnInit {
  private http = inject(HttpClient);
  private signalR: SignalrService = inject(SignalrService);
  private auth: AuthService = inject(AuthService);
 
  readonly icons = {
    Monitor: Monitor,
    Refresh: RefreshCw,
    Clock: Clock,
    Lab: FlaskConical,
    Empty: ClipboardCheck
  };

  orders = signal<MonitoringOrder[]>([]);
  pendingCount = signal(0);

  ngOnInit() {
    this.loadOrders();
    this.setupSignalR();
  }

  constructor() {
    // Escuchar notificaciones del worker en tiempo real
    effect(() => {
      const nots = this.signalR.incomingNotifications();
      if (nots.length > 0) {
        const last = nots[0];
        if (last.category === 'Laboratory' && last.metadata?.legacyOrderId) {
          // Actualizar el estado localmente sin recargar todo
          this.orders.update(current => {
            const index = current.findIndex(o => o.legacyOrderId === last.metadata.legacyOrderId);
            if (index !== -1) {
              current[index].estado = last.metadata.status;
            }
            return [...current];
          });
          this.updateCounts();
        }
      }
    }, { allowSignalWrites: true });
  }

  setupSignalR() {
    const token = this.auth.getToken() || '';
    const role = this.auth.currentUser()?.role || '';
    this.signalR.startConnection(token, role);
  }

  loadOrders() {
    this.http.get<MonitoringOrder[]>(`${environment.apiUrl}/api/ReciboFactura/MonitoringOrders`)
      .subscribe({
        next: (data) => {
          this.orders.set(data);
          this.updateCounts();
        },
        error: (err) => console.error('Error loading monitoring orders:', err)
      });
  }

  updateCounts() {
    this.pendingCount.set(this.orders().filter(o => o.estado === 'PENDIENTE').length);
  }
}
