import { Component, inject, signal, OnInit, computed } from '@angular/core';
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
            <p class="text-[9px] font-black text-slate-500 uppercase tracking-widest mt-0.5 italic">Requisiciones de insumos hacia el Almacén Principal / Farmacia Central</p>
          </div>
        </div>
      </div>

      <div class="grid grid-cols-1 lg:grid-cols-12 gap-6">
        <!-- Panel de Creación de Solicitud -->
        <div class="lg:col-span-5 glass-card border border-glass-border rounded-2xl p-6 space-y-4 shadow-md">
          <h2 class="text-sm font-black uppercase tracking-widest text-muted">Nueva Requisición</h2>

          <div class="space-y-4">
            <!-- Proveedor Fijo -->
            <div class="flex items-center justify-between bg-surface-card/60 p-3 rounded-xl border border-glass-border">
              <span class="text-[10px] text-muted font-black uppercase">Sede Proveedora</span>
              <span class="bg-primary/10 text-primary border border-primary/20 px-2.5 py-1 rounded-lg font-black text-[10px] uppercase">
                🏢 Almacén Principal
              </span>
            </div>

            <!-- Tipo de Destino / Propósito -->
            <div>
              <label class="text-[10px] text-muted font-black uppercase tracking-wider">Tipo de Destino</label>
              <select [(ngModel)]="tipoDestino" (change)="onTipoDestinoChange()"
                class="w-full mt-1 bg-surface-card border border-glass-border rounded-xl px-3 py-2 text-xs font-bold text-main focus:outline-none focus:border-primary/50 transition-all cursor-pointer">
                <option value="DEPÓSITO_OPERATIVO">Depósito con Stock Local (Emergencia / Hospitalización)</option>
                <option value="GASTO_INTERNO_LABORATORIO">Consumo Directo / Nota de Entrega (Laboratorio / Mantenimiento)</option>
              </select>
            </div>

            <!-- Seleccionar Sede o Área Solicitante -->
            <div>
              <label class="text-[10px] text-muted font-black uppercase tracking-wider">Área / Sede Solicitante</label>
              <select [(ngModel)]="newPedido.sedeSolicitanteId" [disabled]="isOnlyEmergency()"
                class="w-full mt-1 bg-surface-card border border-glass-border rounded-xl px-3 py-2 text-xs font-bold text-main focus:outline-none focus:border-primary/50 transition-all cursor-pointer disabled:opacity-60">
                <option *ngFor="let s of filteredSedes" [value]="s.id">{{ s.nombre }}</option>
              </select>
            </div>

            <!-- Agregar Insumo con Buscador -->
            <div class="space-y-2">
              <label class="text-[10px] text-muted font-black uppercase tracking-wider">Insumo Requerido</label>
              <div class="flex gap-2">
                <select [(ngModel)]="selectedInsumoId"
                  class="flex-1 bg-surface-card border border-glass-border rounded-xl px-3 py-2 text-xs font-bold text-main focus:outline-none focus:border-primary/50 transition-all">
                  <option *ngFor="let ins of insumos" [value]="ins.id">{{ ins.nombre }} (Cód: {{ ins.codigo }})</option>
                </select>
                <input type="number" [(ngModel)]="selectedCantidad" min="1"
                  class="w-16 bg-surface-card border border-glass-border rounded-xl px-2 py-2 text-xs text-main font-bold text-center focus:outline-none focus:border-primary/50" />
                <button (click)="addLinea()"
                  class="px-3 rounded-xl bg-primary hover:bg-primary/90 text-white text-xs font-black uppercase tracking-widest transition-all shadow-lg active:scale-95 shrink-0">
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

            <div>
              <label class="text-[10px] text-muted font-black uppercase tracking-wider">Observaciones / Justificación</label>
              <textarea [(ngModel)]="newPedido.observaciones" rows="2"
                placeholder="Ej: Insumos para pruebas de laboratorio rutinarias..."
                class="w-full mt-1 bg-surface-card border border-glass-border rounded-xl px-3 py-2 text-xs text-main focus:outline-none focus:border-primary/50 transition-all resize-none">
              </textarea>
            </div>

            <button (click)="crearSolicitud()" [disabled]="newPedido.lineas.length === 0"
              class="w-full h-10 rounded-xl bg-primary hover:bg-primary/90 disabled:opacity-40 font-black text-white transition-all shadow-lg shadow-primary/25 text-xs uppercase tracking-widest active:scale-[0.98]">
              {{ tipoDestino === 'GASTO_INTERNO_LABORATORIO' ? 'Solicitar Nota de Entrega Directa' : 'Solicitar Reposición de Stock' }}
            </button>
          </div>
        </div>

        <!-- Panel de Solicitudes en Tránsito / Pendientes -->
        <div class="lg:col-span-7 glass-card border border-glass-border rounded-2xl p-6 space-y-4 shadow-md">
          <h2 class="text-sm font-black uppercase tracking-widest text-muted">Estado de Solicitudes</h2>

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
                  'bg-emerald-500/10 text-emerald-400 border border-emerald-500/20': ped.estado === 'Recibido'
                }">
                  {{ ped.estado }}
                </span>
              </div>

              <!-- Ítems -->
              <div class="space-y-1">
                <div *ngFor="let det of ped.detalles" class="text-xs flex justify-between bg-surface-card border border-glass-border p-2 rounded-lg">
                  <span class="text-main/80 font-medium">{{ det.insumoNombre }}</span>
                  <span class="text-main font-black">Cant: {{ det.cantidadSolicitada }}</span>
                </div>
              </div>

              <!-- Acciones según perfil -->
              <div class="flex justify-end pt-2" *ngIf="ped.estado === 'Solicitado'">
                <button (click)="despachar(ped.id)"
                  class="px-4 py-2 bg-primary hover:bg-primary/90 text-xs font-black text-white rounded-xl transition-all shadow-lg uppercase tracking-widest">
                  Aprobar y Despachar
                </button>
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

  public tipoDestino: TipoDestinoSolicitud = 'DEPÓSITO_OPERATIVO';

  public newPedido = {
    sedeSolicitanteId: '',
    sedeProveedoraId: '',
    observaciones: '',
    lineas: [] as { insumoId: string; cantidadSolicitada: number }[]
  };

  public selectedInsumoId = '';
  public selectedCantidad = 1;

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
        this.newPedido.sedeSolicitanteId = activeSedes.find(s => !s.esPrincipal)?.id || principal.id;
      }
    });

    this.inventoryService.getInsumos(true).subscribe(res => {
      this.insumos = res;
      if (res.length > 0) this.selectedInsumoId = res[0].id;
    });

    this.loadPedidos();
  }

  loadPedidos() {
    this.multiSedeService.getPedidosPendientes().subscribe(res => this.pedidos = res);
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

  crearSolicitud() {
    if (!this.newPedido.sedeSolicitanteId || !this.newPedido.sedeProveedoraId) return;

    this.multiSedeService.createPedido({
      sedeSolicitanteId: this.newPedido.sedeSolicitanteId,
      sedeProveedoraId: this.newPedido.sedeProveedoraId,
      observaciones: `[${this.tipoDestino}] ${this.newPedido.observaciones}`,
      lineas: this.newPedido.lineas
    }).subscribe({
      next: () => {
        this.loadPedidos();
        this.newPedido.lineas = [];
        this.newPedido.observaciones = '';
      },
      error: (err: any) => alert(err.error?.message || 'Error al enviar la requisición')
    });
  }

  despachar(id: string) {
    this.multiSedeService.despacharPedido(id).subscribe({
      next: () => this.loadPedidos(),
      error: (err: any) => alert(err.error?.message || 'Error al despachar pedido')
    });
  }
}
