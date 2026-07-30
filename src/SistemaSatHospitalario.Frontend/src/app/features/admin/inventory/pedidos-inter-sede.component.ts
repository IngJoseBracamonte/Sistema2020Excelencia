import { Component, inject, signal, OnInit, computed, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MultiSedeService, Sede, PedidoInterSede } from '../../../core/services/multi-sede.service';
import { InventoryService } from '../../../core/services/inventory.service';
import { Insumo } from '../../../core/models/inventory.model';
import { AuthService } from '../../../core/services/auth.service';

export type TipoDestinoSolicitud = 'DEPÓSITO_OPERATIVO' | 'GASTO_INTERNO_LABORATORIO';

@Component({
  selector: 'app-pedidos-inter-sede',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="p-6 space-y-6 animate-fade-in text-main relative z-10">

      <!-- Encabezado -->
      <div class="flex flex-col md:flex-row md:items-center justify-between bg-surface-card backdrop-blur-3xl p-5 rounded-[2rem] border border-white/5 relative overflow-hidden shadow-2xl">
        <div class="flex items-center space-x-4">
          <div class="h-10 w-10 bg-primary/10 text-primary border border-primary/20 rounded-xl flex items-center justify-center">
            <svg class="w-6 h-6 text-primary" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7h12m0 0l-4-4m4 4l-4 4m0 6H4m0 0l4 4m-4-4l4-4" />
            </svg>
          </div>
          <div>
            <h1 class="text-xl sm:text-2xl font-black text-white tracking-tighter uppercase">Solicitudes de Despacho y Reposición</h1>
            <p class="text-[9px] font-black text-slate-500 uppercase tracking-widest mt-0.5 italic">
              Control Unidireccional & Segregación de Roles (Enfermería / Supervisor)
            </p>
          </div>
        </div>
      </div>

      <div class="grid grid-cols-1 lg:grid-cols-12 gap-6">
        <!-- Panel de Creación de Solicitud (Enfermería / Asistentes) -->
        <div class="lg:col-span-5 glass-card border border-glass-border rounded-2xl p-6 space-y-4 shadow-md">
          <h2 class="text-sm font-black uppercase tracking-widest text-muted">Nueva Requisición</h2>

          <div class="space-y-4">
            <!-- Sede Proveedora Fija -->
            <div class="flex items-center justify-between bg-surface-card/60 p-3 rounded-xl border border-glass-border">
              <span class="text-[10px] text-muted font-black uppercase">Sede Proveedora</span>
              <span class="bg-primary/10 text-primary border border-primary/20 px-2.5 py-1 rounded-lg font-black text-[10px] uppercase">
                🏢 Almacén Principal
              </span>
            </div>

            <!-- Sede Solicitante (Destino) derivado del Área Activa -->
            <div class="flex items-center justify-between bg-surface-card/60 p-3 rounded-xl border border-glass-border">
              <span class="text-[10px] text-muted font-black uppercase">Área / Sede Solicitante (Destino)</span>
              <span class="bg-indigo-500/10 text-indigo-400 border border-indigo-500/20 px-2.5 py-1 rounded-lg font-black text-[10px] uppercase">
                📍 {{ getSedeNombre(newPedido.sedeSolicitanteId) || areaNombreContext || 'Área de Emergencia' }}
              </span>
            </div>

            <!-- Agregar Insumo con Buscador por Nombre, Código o Principio Activo -->
            <div class="space-y-2 relative">
              <div class="flex justify-between items-center">
                <label class="text-[10px] text-muted font-black uppercase tracking-wider">Buscar Insumo Requerido</label>
                <span *ngIf="selectedInsumoObj" class="text-[10px] font-black uppercase px-2 py-0.5 rounded"
                  [ngClass]="selectedInsumoObj.stockActual > 0 ? 'bg-emerald-500/10 text-emerald-400 border border-emerald-500/20' : 'bg-rose-500/10 text-rose-400 border border-rose-500/20'">
                  Stock Principal: {{ selectedInsumoObj.stockActual }} {{ selectedInsumoObj.unidadMedidaBase }}
                </span>
              </div>

              <div class="flex items-center gap-2 w-full">
                <div class="relative flex-1 min-w-0">
                  <input type="text"
                    [ngModel]="insumoSearchTerm()"
                    (ngModelChange)="onSearchTermChange($event)"
                    (focus)="showInsumoDropdown.set(true)"
                    placeholder="Buscar por nombre, código o principio activo..."
                    class="w-full bg-surface-card border border-glass-border rounded-xl px-3 py-2 text-xs font-bold text-main focus:outline-none focus:border-primary/50 transition-all truncate" />

                  <!-- Autocomplete Dropdown -->
                  <div *ngIf="showInsumoDropdown() && filteredInsumosList().length > 0"
                       class="absolute z-50 left-0 right-0 top-full mt-1 bg-[#0f172a] border border-white/10 rounded-xl shadow-2xl max-h-56 overflow-y-auto custom-scrollbar">
                    <div *ngFor="let ins of filteredInsumosList()"
                         (click)="selectInsumo(ins)"
                         class="p-2.5 hover:bg-white/10 cursor-pointer border-b border-white/5 last:border-b-0 transition-all">
                      <div class="flex justify-between items-center">
                        <span class="text-xs font-bold text-white truncate max-w-[200px]">{{ ins.nombre }}</span>
                        <span class="text-[9px] font-mono text-slate-400 bg-white/5 px-1.5 py-0.5 rounded">{{ ins.codigo }}</span>
                      </div>
                      <div class="flex items-center justify-between text-[10px] text-slate-400 mt-1">
                        <span *ngIf="ins.principiosActivos && ins.principiosActivos.length > 0" class="text-indigo-400 italic truncate max-w-[180px]">
                          🧬 {{ getPAString(ins) }}
                        </span>
                        <span *ngIf="!ins.principiosActivos || ins.principiosActivos.length === 0" class="italic text-slate-500">
                          Sin P.A.
                        </span>
                        <span class="font-black" [ngClass]="ins.stockActual > 0 ? 'text-emerald-400' : 'text-rose-400'">
                          Stock: {{ ins.stockActual }}
                        </span>
                      </div>
                    </div>
                  </div>
                </div>

                <input type="number" [(ngModel)]="selectedCantidad" min="1"
                  class="w-14 shrink-0 bg-surface-card border border-glass-border rounded-xl px-2 py-2 text-xs text-main font-bold text-center focus:outline-none focus:border-primary/50" />

                <button (click)="addLinea()" [disabled]="!selectedInsumoId"
                  class="px-3.5 py-2 shrink-0 rounded-xl bg-primary hover:bg-primary/90 disabled:opacity-40 text-white text-xs font-black uppercase tracking-widest transition-all shadow-lg active:scale-95 flex items-center justify-center">
                  +
                </button>
              </div>
            </div>

            <!-- Tabla Temporal de Ítems Solicitados -->
            <div class="border border-glass-border rounded-xl p-3 space-y-2 bg-surface-card/50" *ngIf="newPedido.lineas.length > 0">
              <div class="flex justify-between items-center text-[10px] text-muted font-black uppercase tracking-widest border-b border-glass-border pb-2">
                <span>Insumo</span>
                <span>Cant.</span>
              </div>
              <div *ngFor="let line of newPedido.lineas; let idx = index" class="flex justify-between items-center text-xs">
                <span class="text-main/90 truncate max-w-[200px] font-medium">{{ getInsumoName(line.insumoId) }}</span>
                <div class="flex items-center gap-2">
                  <span class="font-black text-primary bg-primary/10 px-2 py-0.5 rounded-md">{{ line.cantidadSolicitada }}</span>
                  <button (click)="removeLinea(idx)" class="text-rose-400 hover:text-rose-300 font-black px-1">×</button>
                </div>
              </div>
            </div>

            <button (click)="crearSolicitud()" [disabled]="newPedido.lineas.length === 0"
              class="w-full h-10 rounded-xl bg-primary hover:bg-primary/90 disabled:opacity-40 font-black text-white transition-all shadow-lg shadow-primary/25 text-xs uppercase tracking-widest active:scale-[0.98]">
              Solicitar Reposición de Stock
            </button>
          </div>
        </div>

        <!-- Panel de Solicitudes en Tránsito / Pendientes (Matriz SoD) -->
        <div class="lg:col-span-7 glass-card border border-glass-border rounded-2xl p-6 space-y-4 shadow-md">
          <h2 class="text-sm font-black uppercase tracking-widest text-muted">Bandeja de Requisiciones</h2>

          <div class="space-y-4" *ngIf="pedidos.length > 0; else noPedidos">
            <div *ngFor="let ped of pedidos"
              class="border border-glass-border rounded-xl p-4 bg-surface-card/50 hover:border-white/10 transition-all space-y-3">
              <div class="flex flex-wrap items-center justify-between gap-2 border-b border-glass-border pb-3">
                <div>
                  <span class="text-[10px] font-black tracking-widest uppercase text-primary bg-primary/10 border border-primary/20 px-2 py-0.5 rounded-lg">
                    {{ ped.correlativo }}
                  </span>
                  <span class="text-xs text-muted ml-2 font-bold">
                    Destino: {{ ped.sedeSolicitanteNombre }}
                  </span>
                </div>
                <span class="text-[10px] font-black px-2.5 py-1 rounded-full uppercase tracking-widest" [ngClass]="{
                  'bg-amber-500/10 text-amber-400 border border-amber-500/20': ped.estado === 'Solicitado',
                  'bg-hospital-500/10 text-hospital-400 border border-hospital-500/20': ped.estado === 'Despachado',
                  'bg-emerald-500/10 text-emerald-400 border border-emerald-500/20': ped.estado === 'Recibido',
                  'bg-rose-500/10 text-rose-400 border border-rose-500/20': ped.estado === 'Cancelado'
                }">
                  {{ ped.estado }}
                </span>
              </div>

              <!-- Lista de Detalles con Muestreo de Stock Central y Cantidades Ajustables -->
              <div class="space-y-2">
                <div *ngFor="let det of ped.detalles" class="text-xs flex flex-wrap items-center justify-between bg-surface-card border border-glass-border p-2.5 rounded-lg gap-2">
                  <div>
                    <div class="font-bold text-main/90">{{ det.insumoNombre }}</div>
                    <div class="text-[10px] text-muted">
                      Stock Central Disp: 
                      <span class="font-black" [ngClass]="getStockDisponible(det.insumoId) > 0 ? 'text-emerald-400' : 'text-rose-400'">
                        {{ getStockDisponible(det.insumoId) }}
                      </span>
                    </div>
                  </div>

                  <div class="flex items-center gap-3">
                    <span class="text-[10px] font-black text-muted uppercase">Pedida: {{ det.cantidadSolicitada }}</span>
                    
                    <!-- Edición de Cantidad para Supervisor -->
                    <div *ngIf="ped.estado === 'Solicitado' && isSupervisorOrAdmin()" class="flex items-center gap-1">
                      <span class="text-[10px] font-black text-amber-400 uppercase">Aprobar:</span>
                      <input type="number" 
                        [ngModel]="getCantidadAprobada(ped.id, det.id, det.cantidadSolicitada)"
                        (ngModelChange)="setCantidadAprobada(ped.id, det.id, $event)"
                        min="0" [max]="getStockDisponible(det.insumoId)"
                        class="w-16 bg-black/40 border border-amber-500/30 rounded-lg px-1.5 py-1 text-xs text-amber-300 font-bold text-center focus:outline-none focus:border-amber-400" />
                    </div>

                    <span *ngIf="ped.estado !== 'Solicitado'" class="font-black text-primary">
                      Despachada: {{ det.cantidadDespachada }}
                    </span>
                  </div>
                </div>
              </div>

              <!-- Observaciones -->
              <div *ngIf="ped.observaciones" class="text-[11px] italic text-muted bg-black/20 p-2 rounded-lg border border-glass-border">
                💬 {{ ped.observaciones }}
              </div>

              <!-- Botones de Acción según Matriz de Permisos (SoD) -->
              <div class="flex flex-wrap justify-end gap-2 pt-2 border-t border-glass-border">
                <!-- Acciones del Supervisor / Admin -->
                <ng-container *ngIf="ped.estado === 'Solicitado' && isSupervisorOrAdmin()">
                  <button (click)="openRechazoModal(ped)"
                    class="px-3 py-1.5 bg-rose-500/10 hover:bg-rose-500/20 border border-rose-500/30 text-rose-400 text-xs font-black rounded-xl transition-all uppercase tracking-widest">
                    ✕ Rechazar
                  </button>
                  <button (click)="despacharAjustado(ped.id)"
                    class="px-4 py-1.5 bg-primary hover:bg-primary/90 text-xs font-black text-white rounded-xl transition-all shadow-lg uppercase tracking-widest active:scale-95">
                    ✓ Aprobar y Despachar
                  </button>
                </ng-container>

                <!-- Acciones del Personal Operativo (Enfermería / Receptor) -->
                <ng-container *ngIf="ped.estado === 'Despachado'">
                  <button (click)="confirmarRecepcion(ped.id)"
                    class="px-4 py-1.5 bg-emerald-600 hover:bg-emerald-500 text-xs font-black text-white rounded-xl transition-all shadow-lg uppercase tracking-widest active:scale-95">
                    📥 Confirmar Recepción
                  </button>
                </ng-container>
              </div>
            </div>
          </div>
          <ng-template #noPedidos>
            <div class="text-center py-12 text-muted font-bold text-xs uppercase tracking-wider">
              No hay solicitudes activas actualmente.
            </div>
          </ng-template>
        </div>
      </div>

      <!-- Modal de Rechazo de Requisición -->
      <div *ngIf="showRechazoModal" class="fixed inset-0 bg-black/80 backdrop-blur-md flex items-center justify-center p-4 z-50 animate-fade-in">
        <div class="bg-surface-card border border-glass-border rounded-2xl max-w-md w-full p-6 space-y-4 shadow-2xl">
          <div class="flex justify-between items-center border-b border-glass-border pb-3">
            <h3 class="text-sm font-black uppercase text-rose-400 tracking-wider">Rechazar Requisición {{ selectedPedidoRechazar?.correlativo }}</h3>
            <button (click)="showRechazoModal = false" class="text-muted hover:text-white font-black text-lg">×</button>
          </div>
          <div>
            <label class="text-[10px] text-muted font-black uppercase tracking-wider">Motivo de Rechazo (Auditoría Obligatoria)</label>
            <textarea [(ngModel)]="motivoRechazo" rows="3"
              placeholder="Indique la razón del rechazo..."
              class="w-full mt-1 bg-black/40 border border-glass-border rounded-xl p-3 text-xs text-main focus:outline-none focus:border-rose-500 transition-all resize-none">
            </textarea>
          </div>
          <div class="flex justify-end gap-2">
            <button (click)="showRechazoModal = false"
              class="px-4 py-2 bg-surface-card border border-glass-border text-xs font-bold rounded-xl hover:bg-white/5 transition-all">
              Cancelar
            </button>
            <button (click)="confirmarRechazo()" [disabled]="!motivoRechazo.trim()"
              class="px-4 py-2 bg-rose-600 hover:bg-rose-500 disabled:opacity-40 text-xs font-black text-white rounded-xl transition-all shadow-lg uppercase tracking-widest">
              Confirmar Rechazo
            </button>
          </div>
        </div>
      </div>

    </div>
  `
})
export class PedidosInterSedeComponent implements OnInit {
  @Input() set areaNombre(val: string) {
    if (val) this.areaNombreContext = val;
  }
  @Input() set sedeId(val: string) {
    if (val) {
      this.overrideSedeSolicitanteId = val;
      this.newPedido.sedeSolicitanteId = val;
    }
  }

  public areaNombreContext: string = '';
  public overrideSedeSolicitanteId: string = '';

  public multiSedeService = inject(MultiSedeService);
  private inventoryService = inject(InventoryService);
  private authService = inject(AuthService);

  public sedes: Sede[] = [];
  public insumos: Insumo[] = [];
  public pedidos: PedidoInterSede[] = [];

  public tipoDestino: TipoDestinoSolicitud = 'DEPÓSITO_OPERATIVO';

  public newPedido = {
    sedeSolicitanteId: '',
    sedeProveedoraId: '',
    observaciones: '',
    lineas: [] as { insumoId: string; cantidadSolicitada: number }[]
  };

  public selectedInsumoId = '';
  public selectedCantidad = 1;

  // Autocompletado de Insumos (Buscador por Nombre, Código y Principio Activo)
  public insumoSearchTerm = signal<string>('');
  public showInsumoDropdown = signal<boolean>(false);

  public filteredInsumosList = computed(() => {
    const term = this.insumoSearchTerm().toLowerCase().trim();
    if (!term) return this.insumos.slice(0, 15);
    return this.insumos.filter(i => {
      const matchNombre = (i.nombre || '').toLowerCase().includes(term);
      const matchCodigo = (i.codigo || '').toLowerCase().includes(term);
      const matchPA = i.principiosActivos?.some(pa => 
        (pa.nombre || '').toLowerCase().includes(term) || 
        (pa.concentracion || '').toLowerCase().includes(term)
      );
      return matchNombre || matchCodigo || matchPA;
    }).slice(0, 20);
  });

  public onSearchTermChange(term: string) {
    this.insumoSearchTerm.set(term);
    this.showInsumoDropdown.set(true);
  }

  public selectInsumo(insumo: Insumo) {
    this.selectedInsumoId = insumo.id;
    this.insumoSearchTerm.set(insumo.nombre);
    this.showInsumoDropdown.set(false);
  }

  public getPAString(insumo: Insumo): string {
    if (!insumo.principiosActivos || insumo.principiosActivos.length === 0) return '';
    return insumo.principiosActivos.map(pa => `${pa.nombre} ${pa.concentracion ? '(' + pa.concentracion + ')' : ''}`).join(', ');
  }

  public getSedeNombre(sedeId: string): string {
    const s = this.sedes.find(x => x.id === sedeId);
    return s ? s.nombre : '';
  }

  // Mapa local para edición de cantidades aprobadas: { [pedidoId]: { [detalleId]: cantidad } }
  public cantidadesAprobadasMap: { [pedidoId: string]: { [detalleId: string]: number } } = {};

  // Modal Rechazo
  public showRechazoModal = false;
  public selectedPedidoRechazar: PedidoInterSede | null = null;
  public motivoRechazo = '';

  public isSupervisorOrAdmin = computed(() => {
    return this.authService.isAdmin() || this.authService.isSupervisor();
  });

  public isOnlyEmergency = computed(() => {
    const user = this.authService.currentUser();
    const role = user?.role?.toLowerCase() || '';
    return role.includes('emergencia') && !this.authService.isAdmin() && !this.authService.isSupervisor();
  });

  get filteredSedes(): Sede[] {
    if (this.isOnlyEmergency()) {
      return this.sedes.filter(s => s.codigo?.toUpperCase() === 'EMERGENCIA' || s.id === '10000000-0000-0000-0000-000000000002');
    }
    return this.sedes;
  }

  get selectedInsumoObj(): Insumo | undefined {
    return this.insumos.find(i => i.id === this.selectedInsumoId);
  }

  ngOnInit() {
    this.loadInitialData();
  }

  loadInitialData() {
    this.multiSedeService.getSedes().subscribe(res => {
      const activeSedes = res.filter(s => s.activo);
      this.sedes = activeSedes;
      if (activeSedes.length > 0) {
        const principal = activeSedes.find(s => s.esPrincipal) || activeSedes[0];
        // Sede proveedora FIJA a Almacén Principal
        this.newPedido.sedeProveedoraId = principal.id;
        if (this.overrideSedeSolicitanteId) {
          this.newPedido.sedeSolicitanteId = this.overrideSedeSolicitanteId;
        } else {
          this.newPedido.sedeSolicitanteId = activeSedes.find(s => !s.esPrincipal)?.id || principal.id;
        }
      }
    });

    this.inventoryService.getInsumos(true).subscribe(res => {
      this.insumos = res;
    });

    this.loadPedidos();
  }

  loadPedidos() {
    this.multiSedeService.getPedidosPendientes().subscribe(res => {
      this.pedidos = res;
    });
  }

  onTipoDestinoChange() {
    if (this.tipoDestino === 'GASTO_INTERNO_LABORATORIO') {
      this.newPedido.observaciones = 'Despacho directo para consumo interno de Laboratorio (Gasto operativo).';
    } else {
      this.newPedido.observaciones = '';
    }
  }

  addLinea() {
    if (!this.selectedInsumoId || this.selectedCantidad <= 0) return;
    const exists = this.newPedido.lineas.find(l => l.insumoId === this.selectedInsumoId);
    if (exists) {
      exists.cantidadSolicitada += this.selectedCantidad;
    } else {
      this.newPedido.lineas.push({
        insumoId: this.selectedInsumoId,
        cantidadSolicitada: this.selectedCantidad
      });
    }
  }

  removeLinea(idx: number) {
    this.newPedido.lineas.splice(idx, 1);
  }

  getInsumoName(id: string): string {
    return this.insumos.find(i => i.id === id)?.nombre || 'Insumo Desconocido';
  }

  getStockDisponible(insumoId: string): number {
    return this.insumos.find(i => i.id === insumoId)?.stockActual ?? 0;
  }

  getCantidadAprobada(pedidoId: string, detalleId: string, defaultVal: number): number {
    if (!this.cantidadesAprobadasMap[pedidoId]) {
      this.cantidadesAprobadasMap[pedidoId] = {};
    }
    if (this.cantidadesAprobadasMap[pedidoId][detalleId] === undefined) {
      this.cantidadesAprobadasMap[pedidoId][detalleId] = defaultVal;
    }
    return this.cantidadesAprobadasMap[pedidoId][detalleId];
  }

  setCantidadAprobada(pedidoId: string, detalleId: string, val: number) {
    if (!this.cantidadesAprobadasMap[pedidoId]) {
      this.cantidadesAprobadasMap[pedidoId] = {};
    }
    this.cantidadesAprobadasMap[pedidoId][detalleId] = Math.max(0, val);
  }

  crearSolicitud() {
    if (!this.newPedido.sedeSolicitanteId || !this.newPedido.sedeProveedoraId) return;

    this.multiSedeService.createPedido({
      sedeSolicitanteId: this.newPedido.sedeSolicitanteId,
      sedeProveedoraId: this.newPedido.sedeProveedoraId,
      observaciones: `Solicitud de Reposición desde ${this.areaNombreContext || 'Enfermería'}`,
      lineas: this.newPedido.lineas
    }).subscribe({
      next: () => {
        this.newPedido.lineas = [];
        this.newPedido.observaciones = '';
        this.insumoSearchTerm.set('');
        this.selectedInsumoId = '';
        this.loadPedidos();
      },
      error: (err) => console.error('Error al crear solicitud:', err)
    });
  }

  despacharAjustado(pedidoId: string) {
    const mapaLineas = this.cantidadesAprobadasMap[pedidoId] || {};
    this.multiSedeService.despacharPedido(pedidoId, mapaLineas).subscribe({
      next: () => {
        this.loadPedidos();
        this.inventoryService.getInsumos(true).subscribe(res => this.insumos = res);
      },
      error: (err: any) => alert(err.error?.message || 'Error al despachar el pedido')
    });
  }

  openRechazoModal(ped: PedidoInterSede) {
    this.selectedPedidoRechazar = ped;
    this.motivoRechazo = '';
    this.showRechazoModal = true;
  }

  confirmarRechazo() {
    if (!this.selectedPedidoRechazar || !this.motivoRechazo.trim()) return;

    this.multiSedeService.rechazarPedido(this.selectedPedidoRechazar.id, this.motivoRechazo.trim()).subscribe({
      next: () => {
        this.showRechazoModal = false;
        this.selectedPedidoRechazar = null;
        this.loadPedidos();
      },
      error: (err: any) => alert(err.error?.message || 'Error al rechazar el pedido')
    });
  }

  confirmarRecepcion(pedidoId: string) {
    this.multiSedeService.recibirPedido(pedidoId, {}).subscribe({
      next: () => this.loadPedidos(),
      error: (err: any) => alert(err.error?.message || 'Error al confirmar recepción del pedido')
    });
  }
}
