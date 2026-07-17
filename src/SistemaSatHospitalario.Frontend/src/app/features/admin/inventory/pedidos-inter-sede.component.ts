import { Component, inject, OnInit, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MultiSedeService, Sede, PedidoInterSede } from '../../../core/services/multi-sede.service';
import { InventoryService } from '../../../core/services/inventory.service';
import { Insumo } from '../../../core/models/inventory.model';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-pedidos-inter-sede',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="p-6 space-y-6 animate-fade-in text-main relative z-10">

      <!-- Encabezado con patrón global del sistema -->
      <div class="flex flex-col md:flex-row md:items-center justify-between bg-surface-card backdrop-blur-3xl p-5 rounded-[2rem] border border-white/5 relative overflow-hidden shadow-2xl">
        <div class="absolute top-0 right-0 w-64 h-64 bg-primary/5 rounded-full -z-10 blur-3xl"></div>
        <div class="flex items-center space-x-4">
          <div class="h-10 w-10 bg-primary/10 text-primary border border-primary/20 rounded-xl flex items-center justify-center shadow-lg shadow-primary/5">
            <svg class="w-6 h-6 text-primary" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                d="M8 7h12m0 0l-4-4m4 4l-4 4m0 6H4m0 0l4 4m-4-4l4-4" />
            </svg>
          </div>
          <div>
            <h1 class="text-xl sm:text-2xl font-black text-white tracking-tighter uppercase">Pedidos</h1>
            <p class="text-[9px] font-black text-slate-500 uppercase tracking-widest mt-0.5 italic">Gestión de traslados de mercadería y reposición de inventario entre sucursales</p>
          </div>
        </div>
      </div>

      <div class="grid grid-cols-1 lg:grid-cols-12 gap-6">
        <!-- Panel de Creación de Pedido -->
        <div class="lg:col-span-5 xl:col-span-5 glass-card border border-glass-border rounded-2xl p-6 space-y-4 shadow-md">
          <h2 class="text-sm font-black uppercase tracking-widest text-muted">Solicitar Reposición</h2>

          <div class="space-y-3">
            <div>
              <label class="text-[10px] text-muted font-black uppercase tracking-wider">Sede Solicitante (Destino)</label>
              <select [(ngModel)]="newPedido.sedeSolicitanteId" [disabled]="isOnlyEmergency()"
                class="w-full mt-1 bg-surface-card border border-glass-border rounded-xl px-3 py-2 text-sm text-main focus:outline-none focus:border-primary/50 transition-all cursor-pointer disabled:opacity-60 disabled:cursor-not-allowed">
                <option *ngFor="let s of filteredSedes" [value]="s.id">
                  {{ s.nombre }} {{ s.esPrincipal ? '(Principal)' : '' }}
                </option>
              </select>
            </div>

            <div>
              <label class="text-[10px] text-muted font-black uppercase tracking-wider">Agregar Insumo</label>
              <div class="flex gap-2 mt-1">
                <select [(ngModel)]="selectedInsumoId"
                  class="flex-1 bg-surface-card border border-glass-border rounded-xl px-3 py-2 text-sm text-main focus:outline-none focus:border-primary/50 transition-all cursor-pointer">
                  <option *ngFor="let ins of insumos" [value]="ins.id">{{ ins.nombre }}</option>
                </select>
                <input type="number" [(ngModel)]="selectedCantidad" min="1"
                  class="w-16 bg-surface-card border border-glass-border rounded-xl px-2 py-2 text-sm text-main text-center focus:outline-none focus:border-primary/50 transition-all" />
                <button (click)="addLinea()"
                  class="px-3 rounded-xl bg-primary hover:bg-primary/90 text-white text-xs font-black uppercase tracking-widest transition-all shadow-lg shadow-primary/25 active:scale-95 flex items-center justify-center shrink-0">
                  +
                </button>
              </div>
            </div>

            <!-- Listado de Líneas Temporales -->
            <div class="border border-glass-border rounded-xl p-3 space-y-2 bg-surface-card/50" *ngIf="newPedido.lineas.length > 0">
              <div class="flex justify-between items-center text-[10px] text-muted font-black uppercase tracking-widest border-b border-glass-border pb-2">
                <span>Insumo</span>
                <span>Cant.</span>
              </div>
              <div *ngFor="let line of newPedido.lineas; let idx = index" class="flex justify-between items-center text-xs">
                <span class="text-main/80 truncate max-w-[180px] font-medium">{{ getInsumoName(line.insumoId) }}</span>
                <div class="flex items-center gap-2">
                  <span class="font-black text-main">{{ line.cantidadSolicitada }}</span>
                  <button (click)="removeLinea(idx)" class="text-rose-400 hover:text-rose-300 font-black transition-colors">×</button>
                </div>
              </div>
            </div>

            <div>
              <label class="text-[10px] text-muted font-black uppercase tracking-wider">Observaciones</label>
              <textarea [(ngModel)]="newPedido.observaciones" rows="3"
                class="w-full mt-1 bg-surface-card border border-glass-border rounded-xl px-3 py-2 text-sm text-main focus:outline-none focus:border-primary/50 transition-all resize-none">
              </textarea>
            </div>

            <button
              (click)="crearSolicitud()"
              [disabled]="newPedido.lineas.length === 0 || !newPedido.sedeProveedoraId"
              class="w-full h-10 rounded-xl bg-primary hover:bg-primary/90 disabled:opacity-40 font-black text-white transition-all shadow-lg shadow-primary/25 text-xs uppercase tracking-widest active:scale-[0.98]"
            >
              Enviar Requisición
            </button>
          </div>
        </div>

        <!-- Panel de Pedidos Activos/Pendientes -->
        <div class="lg:col-span-7 xl:col-span-7 glass-card border border-glass-border rounded-2xl p-6 space-y-4 shadow-md">
          <h2 class="text-sm font-black uppercase tracking-widest text-muted">Pedidos en Tránsito / Pendientes</h2>

          <div class="space-y-4" *ngIf="pedidos.length > 0; else noPedidos">
            <div
              *ngFor="let ped of pedidos"
              class="border border-glass-border rounded-xl p-4 bg-surface-card/50 hover:border-white/10 hover:bg-surface-card transition-all space-y-3"
            >
              <div class="flex flex-wrap items-center justify-between gap-2 border-b border-glass-border pb-3">
                <div>
                  <span class="text-[10px] font-black tracking-widest uppercase text-primary bg-primary/10 border border-primary/20 px-2 py-0.5 rounded-lg">
                    {{ ped.correlativo }}
                  </span>
                  <span class="text-xs text-muted ml-2 font-bold">
                    De: {{ ped.sedeSolicitanteNombre }} → Para: {{ ped.sedeProveedoraNombre }}
                  </span>
                </div>
                <div class="flex items-center gap-2">
                  <span class="text-[10px] font-black px-2.5 py-1 rounded-full uppercase tracking-widest" [ngClass]="{
                    'bg-amber-500/10 text-amber-400 border border-amber-500/20': ped.estado === 'Solicitado',
                    'bg-hospital-500/10 text-hospital-400 border border-hospital-500/20': ped.estado === 'Despachado',
                    'bg-emerald-500/10 text-emerald-400 border border-emerald-500/20': ped.estado === 'Recibido'
                  }">
                    {{ ped.estado }}
                  </span>
                </div>
              </div>

              <!-- Listado de Detalles de Items -->
              <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div class="space-y-2">
                  <h4 class="text-[10px] font-black uppercase text-muted tracking-widest">Detalles de Insumos</h4>
                  <div *ngFor="let det of ped.detalles" class="text-xs flex justify-between bg-surface-card border border-glass-border p-2 rounded-lg">
                    <span class="text-main/80 font-medium">{{ det.insumoNombre }}</span>
                    <span class="text-main font-black">
                      Sol: {{ det.cantidadSolicitada }} | Desp: {{ det.cantidadDespachada }} | Rec: {{ det.cantidadRecibida }}
                    </span>
                  </div>
                </div>

                <div class="flex flex-col justify-end gap-2" *ngIf="ped.estado !== 'Recibido'">
                  <!-- Acciones del Proveedor (Despacho) -->
                  <div class="flex justify-end gap-2" *ngIf="ped.estado === 'Solicitado' && isSedeProveedora(ped)">
                    <button
                      (click)="despachar(ped.id)"
                      class="px-4 py-2 bg-primary hover:bg-primary/90 text-xs font-black text-white rounded-xl transition-all shadow-lg shadow-primary/25 uppercase tracking-widest active:scale-[0.98]"
                    >
                      Aprobar y Despachar
                    </button>
                  </div>

                  <!-- Acciones del Solicitante (Recepción) -->
                  <div class="flex justify-end gap-2" *ngIf="ped.estado === 'Despachado' && isSedeSolicitante(ped)">
                    <button
                      (click)="abrirRecepcionDialog(ped)"
                      class="px-4 py-2 bg-emerald-600 hover:bg-emerald-500 text-xs font-black text-white rounded-xl transition-all shadow-lg shadow-emerald-500/25 uppercase tracking-widest active:scale-[0.98]"
                    >
                      Confirmar Recepción
                    </button>
                  </div>
                </div>
              </div>
            </div>
          </div>
          <ng-template #noPedidos>
            <div class="text-center py-12">
              <div class="w-12 h-12 bg-surface-card border border-glass-border rounded-2xl flex items-center justify-center mx-auto mb-3">
                <svg class="w-6 h-6 text-muted" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20 13V6a2 2 0 00-2-2H6a2 2 0 00-2 2v7m16 0v5a2 2 0 01-2 2H6a2 2 0 01-2-2v-5m16 0h-2.586a1 1 0 00-.707.293l-2.414 2.414a1 1 0 01-.707.293h-3.172a1 1 0 01-.707-.293l-2.414-2.414A1 1 0 006.586 13H4" />
                </svg>
              </div>
              <p class="text-sm text-muted font-bold uppercase tracking-wider">No hay solicitudes activas ni traslados en curso</p>
            </div>
          </ng-template>
        </div>
      </div>

      <!-- Dialogo de Recepción con Discrepancias -->
      <div *ngIf="showRecepcionDialog" class="fixed inset-0 z-[1000] flex items-center justify-center bg-black/70 backdrop-blur-sm p-4">
        <div class="w-full max-w-md rounded-2xl border border-primary/20 bg-surface-card shadow-2xl shadow-black/50 space-y-4 p-6 animate-scale-in">
          <div class="flex items-center gap-3 pb-2 border-b border-glass-border">
            <div class="w-9 h-9 bg-emerald-500/10 border border-emerald-500/20 rounded-xl flex items-center justify-center">
              <svg class="w-5 h-5 text-emerald-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
              </svg>
            </div>
            <div>
              <h3 class="text-sm font-black text-main uppercase tracking-widest">Confirmar Recepción</h3>
              <p class="text-[10px] text-muted font-bold">Especifique la cantidad real de mercadería recibida en bodega.</p>
            </div>
          </div>

          <div class="space-y-3 max-h-60 overflow-y-auto">
            <div *ngFor="let det of selectedPedidoForRecepcion?.detalles" class="flex items-center justify-between text-sm bg-surface-card/50 border border-glass-border rounded-lg px-3 py-2">
              <span class="text-main font-medium truncate max-w-[200px]">{{ det.insumoNombre }}</span>
              <div class="flex items-center gap-2">
                <span class="text-[10px] text-muted font-black uppercase">(Despachado: {{ det.cantidadDespachada }})</span>
                <input
                  type="number"
                  [(ngModel)]="discrepanciasForm[det.insumoId]"
                  class="w-20 bg-surface-card border border-glass-border rounded-lg px-2 py-1 text-xs text-center text-main focus:outline-none focus:border-primary/50 transition-all"
                />
              </div>
            </div>
          </div>

          <div class="flex justify-end gap-3 pt-2">
            <button (click)="closeRecepcionDialog()"
              class="px-4 py-2 rounded-xl bg-surface-card border border-glass-border hover:bg-white/5 text-xs text-main font-black uppercase tracking-widest transition-all">
              Cancelar
            </button>
            <button (click)="guardarRecepcion()"
              class="px-4 py-2 rounded-xl bg-emerald-600 hover:bg-emerald-500 text-xs text-white font-black uppercase tracking-widest transition-all shadow-lg shadow-emerald-500/25 active:scale-[0.98]">
              Recibir Carga
            </button>
          </div>
        </div>
      </div>
    </div>
  `
})
export class PedidosInterSedeComponent implements OnInit {
  public multiSedeService = inject(MultiSedeService);
  private inventoryService = inject(InventoryService);
  private authService = inject(AuthService);

  public sedes: Sede[] = [];
  public insumos: Insumo[] = [];
  public pedidos: PedidoInterSede[] = [];

  // Form states
  public newPedido = {
    sedeSolicitanteId: '',
    sedeProveedoraId: '',
    observaciones: '',
    lineas: [] as { insumoId: string; cantidadSolicitada: number }[]
  };

  public selectedInsumoId = '';
  public selectedCantidad = 1;

  public showRecepcionDialog = false;
  public selectedPedidoForRecepcion: PedidoInterSede | null = null;
  public discrepanciasForm: { [key: string]: number } = {};

  public isOnlyEmergency = computed(() => {
    const user = this.authService.currentUser();
    const role = user?.role?.toLowerCase() || '';
    const isAdmin = this.authService.isAdmin();
    const isSupervisor = this.authService.isSupervisor();
    return role.includes('emergencia') && !isAdmin && !isSupervisor;
  });

  get filteredSedes(): Sede[] {
    if (this.isOnlyEmergency()) {
      return this.sedes.filter(s => s.codigo?.toUpperCase() === 'EMERGENCIA' || s.id === '10000000-0000-0000-0000-000000000002');
    }
    return this.sedes;
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
        
        // La sede proveedora siempre es fija a la sede principal
        this.newPedido.sedeProveedoraId = principal.id;

        // La sede solicitante preseleccionada según el rol
        if (this.isOnlyEmergency()) {
          const emergencySede = activeSedes.find(s => s.codigo?.toUpperCase() === 'EMERGENCIA' || s.id === '10000000-0000-0000-0000-000000000002');
          if (emergencySede) {
            this.newPedido.sedeSolicitanteId = emergencySede.id;
          } else {
            this.newPedido.sedeSolicitanteId = principal.id;
          }
        } else {
          this.newPedido.sedeSolicitanteId = principal.id;
        }
      }
    });
    this.inventoryService.getInsumos(true).subscribe(res => {
      this.insumos = res;
      if (res.length > 0) this.selectedInsumoId = res[0].id;
    });
    this.loadPedidos();
  }

  loadPedidos() {
    this.multiSedeService.getPedidosPendientes().subscribe(res => {
      this.pedidos = res;
    });
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

  crearSolicitud() {
    if (!this.newPedido.sedeSolicitanteId || !this.newPedido.sedeProveedoraId) return;

    this.multiSedeService.createPedido({
      sedeSolicitanteId: this.newPedido.sedeSolicitanteId,
      sedeProveedoraId: this.newPedido.sedeProveedoraId,
      observaciones: this.newPedido.observaciones,
      lineas: this.newPedido.lineas
    }).subscribe({
      next: () => {
        this.loadPedidos();
        const prevSolicitante = this.newPedido.sedeSolicitanteId;
        const prevProveedora = this.newPedido.sedeProveedoraId;
        this.newPedido = {
          sedeSolicitanteId: prevSolicitante,
          sedeProveedoraId: prevProveedora,
          observaciones: '',
          lineas: []
        };
      },
      error: (err) => alert(err.error?.message || 'Error al enviar la requisición')
    });
  }

  isSedeProveedora(ped: PedidoInterSede): boolean {
    return true;
  }

  isSedeSolicitante(ped: PedidoInterSede): boolean {
    return true;
  }

  despachar(id: string) {
    this.multiSedeService.despacharPedido(id).subscribe({
      next: () => this.loadPedidos(),
      error: (err) => alert(err.error?.message || 'Error al despachar pedido')
    });
  }

  abrirRecepcionDialog(ped: PedidoInterSede) {
    this.selectedPedidoForRecepcion = ped;
    this.discrepanciasForm = {};
    for (const d of ped.detalles) {
      this.discrepanciasForm[d.insumoId] = d.cantidadDespachada;
    }
    this.showRecepcionDialog = true;
  }

  closeRecepcionDialog() {
    this.showRecepcionDialog = false;
    this.selectedPedidoForRecepcion = null;
  }

  guardarRecepcion() {
    if (!this.selectedPedidoForRecepcion) return;

    this.multiSedeService.recibirPedido(this.selectedPedidoForRecepcion.id, this.discrepanciasForm).subscribe({
      next: () => {
        this.loadPedidos();
        this.closeRecepcionDialog();
      },
      error: (err) => alert(err.error?.message || 'Error al guardar recepción')
    });
  }
}
