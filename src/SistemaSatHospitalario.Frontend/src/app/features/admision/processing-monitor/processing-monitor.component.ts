import { Component, inject, OnInit, signal, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { LucideAngularModule, Monitor, RefreshCw, Clock, FlaskConical, ClipboardCheck, Printer, X, Check, AlertTriangle } from 'lucide-angular';
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

                    <!-- Acciones del Monitor (Hoja de Trabajo y Completar) -->
                    <div class="pt-4 border-t border-white/5 flex items-center justify-between gap-3 relative z-20">
                        <button (click)="showWorklist(order.cuentaId)" 
                            class="flex-1 py-2 bg-white/5 hover:bg-white/10 rounded-xl text-xs font-bold text-slate-300 hover:text-white transition-all flex items-center justify-center space-x-2 border border-white/5">
                            <lucide-icon [name]="icons.Printer" class="w-3.5 h-3.5"></lucide-icon>
                            <span>Hoja de Trabajo</span>
                        </button>
                        <button *ngIf="isLabAssistant() && order.estado === 'PENDIENTE'"
                            (click)="completeOrder(order.cuentaId)"
                            class="p-2 bg-emerald-600/20 hover:bg-emerald-600/30 text-emerald-400 rounded-xl border border-emerald-500/20 transition-all"
                            title="Marcar como Procesada">
                            <lucide-icon [name]="icons.Check" class="w-4 h-4"></lucide-icon>
                        </button>
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

        <!-- Dialogo de Hoja de Trabajo (Worklist de Conciliación) -->
        <div *ngIf="showWorklistDialog()" class="fixed inset-0 z-[1000] flex items-center justify-center bg-black/70 backdrop-blur-sm p-4">
            <div class="w-full max-w-2xl rounded-[2rem] border border-white/10 bg-surface p-8 shadow-2xl space-y-6 relative max-h-[90vh] overflow-y-auto">
                <button (click)="closeWorklistDialog()" class="absolute top-6 right-6 text-slate-400 hover:text-white transition-colors">
                    <lucide-icon [name]="icons.X" class="w-5 h-5"></lucide-icon>
                </button>

                <div class="flex items-center space-x-4 border-b border-white/5 pb-4">
                    <div class="h-12 w-12 bg-emerald-500/10 text-emerald-500 rounded-xl flex items-center justify-center border border-emerald-500/20">
                        <lucide-icon [name]="icons.Lab" class="w-6 h-6"></lucide-icon>
                    </div>
                    <div>
                        <h2 class="text-xl font-black text-white uppercase tracking-tight">Hoja de Trabajo - Control</h2>
                        <p class="text-[10px] font-black text-slate-500 uppercase tracking-widest mt-0.5 font-mono">Conciliación Física vs Digital (Doble Check)</p>
                    </div>
                </div>

                <!-- Box estilo retro-premium de la Imagen -->
                <div class="bg-black/40 border border-white/10 rounded-2xl p-6 font-mono text-xs md:text-sm text-slate-300 shadow-inner relative overflow-hidden">
                    <div class="text-center font-bold border-b border-white/10 pb-3 mb-4 tracking-wider text-white">
                        ---------------------------------------------------------<br>
                        |               HOJA DE TRABAJO - CONTROL               |<br>
                        | Paciente: {{ selectedWorklist()?.pacienteNombre }} | Cuenta: #{{ selectedWorklist()?.legacyOrderId }} |<br>
                        ---------------------------------------------------------
                    </div>

                    <div class="space-y-2 mb-4">
                        <div *ngFor="let item of selectedWorklist()?.items; let i = index" 
                             [ngClass]="{'text-red-400 line-through opacity-70': item.esAnulado}"
                             class="flex items-start justify-between py-1">
                            <div class="flex items-center space-x-3">
                                <span>[{{ item.tieneResultados ? '✓' : ' ' }}] [ ]</span>
                                <span>{{ (i+1).toString().padStart(3, '0') }} - {{ item.nombre }}</span>
                            </div>
                            <span class="font-bold text-[10px] tracking-wider uppercase px-2 py-0.5 rounded"
                                  [ngClass]="{
                                      'bg-emerald-500/10 text-emerald-400 border border-emerald-500/20': !item.esAnulado,
                                      'bg-red-500/10 text-red-400 border border-red-500/20': item.esAnulado
                                  }">
                                ({{ item.estado }})
                            </span>
                        </div>
                    </div>

                    <!-- Nota al pie si hay anulados -->
                    <div *ngIf="selectedWorklist()?.tieneAnulados" class="border-t border-dashed border-red-500/30 pt-3 mt-3 text-red-400 text-xs space-y-1">
                        <div class="font-bold">---------------------------------------------------------</div>
                        <div class="flex items-start space-x-2">
                            <lucide-icon [name]="icons.Alert" class="w-4 h-4 mt-0.5 flex-shrink-0"></lucide-icon>
                            <div>
                                <span class="font-bold">*Nota:</span> Exámenes marcados con [X] fueron anulados en la cuenta por Administración.<br>
                                <span class="font-bold">NO ingresar estos códigos en el sistema de Laboratorio Legacy.</span>
                            </div>
                        </div>
                        <div class="font-bold">---------------------------------------------------------</div>
                    </div>
                </div>

                <!-- Footer del Dialog con Acciones -->
                <div class="flex flex-wrap items-center justify-between gap-4 pt-2">
                    <button (click)="printWorklist()" 
                            class="px-5 py-2.5 bg-white/5 hover:bg-white/10 text-xs font-bold text-white rounded-xl transition-all flex items-center space-x-2 border border-white/5">
                        <lucide-icon [name]="icons.Printer" class="w-4 h-4"></lucide-icon>
                        <span>Imprimir Hoja</span>
                    </button>

                    <div class="flex items-center space-x-3">
                        <button (click)="closeWorklistDialog()" 
                                class="px-5 py-2.5 bg-slate-800 hover:bg-slate-700 text-xs font-bold text-slate-300 rounded-xl transition-all">
                            Cerrar
                        </button>
                        
                        <!-- Botón exclusivo para Asistente de Laboratorio -->
                        <button *ngIf="isLabAssistant() && selectedWorklist()?.legacyOrderId && selectedWorklist()?.items?.length > 0"
                                [disabled]="selectedWorklist()?.estado === 'PROCESADA'"
                                (click)="completeOrder(selectedWorklist().cuentaId)"
                                class="px-5 py-2.5 bg-gradient-to-r from-emerald-600 to-teal-600 hover:from-emerald-500 hover:to-teal-500 disabled:opacity-40 text-xs font-bold text-white rounded-xl transition-all flex items-center space-x-2 shadow-md">
                            <lucide-icon [name]="icons.Check" class="w-4 h-4"></lucide-icon>
                            <span>Marcar como Procesada</span>
                        </button>
                    </div>
                </div>
            </div>
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
    Empty: ClipboardCheck,
    Printer: Printer,
    X: X,
    Check: Check,
    Alert: AlertTriangle
  };

  public isLabAssistant = this.auth.isLabAssistant;

  orders = signal<MonitoringOrder[]>([]);
  pendingCount = signal(0);

  selectedWorklist = signal<any>(null);
  showWorklistDialog = signal(false);

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

  showWorklist(cuentaId: string) {
    this.http.get<any>(`${environment.apiUrl}/api/ReciboFactura/Worklist/${cuentaId}`).subscribe({
      next: (data) => {
        this.selectedWorklist.set(data);
        this.showWorklistDialog.set(true);
      },
      error: (err) => console.error('Error loading worklist:', err)
    });
  }

  closeWorklistDialog() {
    this.showWorklistDialog.set(false);
    this.selectedWorklist.set(null);
  }

  completeOrder(cuentaId: string) {
    this.http.post<any>(`${environment.apiUrl}/api/ReciboFactura/CompleteOrder/${cuentaId}`, {}).subscribe({
      next: () => {
        this.loadOrders();
        this.closeWorklistDialog();
      },
      error: (err) => alert(err.error?.message || 'Error al completar la orden')
    });
  }

  printWorklist() {
    const listData = this.selectedWorklist();
    if (!listData) return;

    // Construcción del documento de impresión con formato exacto de Hoja de Trabajo
    const printWindow = window.open('', '', 'height=600,width=800');
    if (printWindow) {
      printWindow.document.write('<html><head><title>Hoja de Trabajo - Control</title>');
      printWindow.document.write('<style>');
      printWindow.document.write('body { font-family: "Courier New", Courier, monospace; padding: 40px; background: white; color: black; line-height: 1.4; }');
      printWindow.document.write('.text-center { text-align: center; }');
      printWindow.document.write('.font-bold { font-weight: bold; }');
      printWindow.document.write('.item-row { display: flex; justify-content: space-between; margin-bottom: 6px; font-size: 14px; }');
      printWindow.document.write('.line-through { text-decoration: line-through; color: #555; }');
      printWindow.document.write('.footer-note { border-top: 1px dashed black; margin-top: 20px; padding-top: 10px; font-size: 13px; font-weight: bold; }');
      printWindow.document.write('</style></head><body>');
      
      printWindow.document.write('<pre class="font-bold">');
      printWindow.document.write('+-------------------------------------------------------+\n');
      printWindow.document.write('|               HOJA DE TRABAJO - CONTROL               |\n');
      printWindow.document.write(`| Paciente: ${listData.pacienteNombre.padEnd(44)}|\n`);
      printWindow.document.write(`| Cuenta: #${listData.legacyOrderId.toString().padEnd(44)}|\n`);
      printWindow.document.write('+-------------------------------------------------------+\n');
      printWindow.document.write('</pre>');

      printWindow.document.write('<div style="margin-top: 15px; margin-bottom: 15px;">');
      listData.items.forEach((item: any, index: number) => {
        const itemNum = (index + 1).toString().padStart(3, '0');
        const checkMark = item.tieneResultados ? '✓' : ' ';
        const isAnuladoStyle = item.esAnulado ? 'class="line-through"' : '';
        printWindow.document.write(`
          <div class="item-row">
            <span ${isAnuladoStyle}>[${checkMark}] [ ]  ${itemNum} - ${item.nombre}</span>
            <span>(${item.estado})</span>
          </div>
        `);
      });
      printWindow.document.write('</div>');

      if (listData.tieneAnulados) {
        printWindow.document.write('<div class="footer-note">');
        printWindow.document.write('*Nota: Los exámenes tachados fueron anulados en la cuenta por Administración.<br>');
        printWindow.document.write('NO ingresar estos códigos en el sistema de Laboratorio Legacy.');
        printWindow.document.write('</div>');
      }

      printWindow.document.write('</body></html>');
      printWindow.document.close();
      printWindow.focus();
      printWindow.print();
      printWindow.close();
    }
  }
}

