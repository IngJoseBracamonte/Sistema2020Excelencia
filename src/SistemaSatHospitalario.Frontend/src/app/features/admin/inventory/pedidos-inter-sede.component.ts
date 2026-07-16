import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MultiSedeService, Sede, PedidoInterSede } from '../../../core/services/multi-sede.service';
import { InventoryService } from '../../../core/services/inventory.service';
import { Insumo } from '../../../core/models/inventory.model';

@Component({
  selector: 'app-pedidos-inter-sede',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="space-y-6">
      <div class="rounded-2xl border border-white/10 bg-surface/50 backdrop-blur-xl p-8 shadow-2xl">
        <h1 class="text-3xl font-black tracking-tight text-foreground">Pedidos Inter-Sede</h1>
        <p class="text-sm text-muted-foreground mt-1">Gestión de traslados de mercadería y reposición de inventario entre sucursales.</p>
      </div>

      <div class="grid grid-cols-1 xl:grid-cols-3 gap-6">
        <!-- Panel de Creación de Pedido -->
        <div class="xl:col-span-1 rounded-xl border border-white/5 bg-surface/30 p-6 space-y-4 shadow-md">
          <h2 class="text-lg font-bold text-foreground">Solicitar Reposición</h2>
          
          <div class="space-y-3">
            <div>
              <label class="text-xs text-muted-foreground font-semibold">Sede Solicitante (Destino)</label>
              <select [(ngModel)]="newPedido.sedeSolicitanteId" class="w-full mt-1 bg-white/5 border border-white/10 rounded-lg px-3 py-2 text-sm text-foreground focus:outline-none focus:border-indigo-500">
                <option *ngFor="let s of sedes" [value]="s.id">
                  {{ s.nombre }} {{ s.esPrincipal ? '(Principal)' : '' }}
                </option>
              </select>
            </div>

            <div>
              <label class="text-xs text-muted-foreground font-semibold">Sede Proveedora (Origen)</label>
              <select [(ngModel)]="newPedido.sedeProveedoraId" class="w-full mt-1 bg-white/5 border border-white/10 rounded-lg px-3 py-2 text-sm text-foreground focus:outline-none focus:border-indigo-500">
                <option *ngFor="let s of sedes" [value]="s.id" [disabled]="s.id === newPedido.sedeSolicitanteId">
                  {{ s.nombre }} {{ s.esPrincipal ? '(Principal)' : '' }}
                </option>
              </select>
            </div>

            <div>
              <label class="text-xs text-muted-foreground font-semibold">Agregar Insumo</label>
              <div class="flex gap-2 mt-1">
                <select [(ngModel)]="selectedInsumoId" class="flex-1 bg-white/5 border border-white/10 rounded-lg px-3 py-2 text-sm text-foreground focus:outline-none focus:border-indigo-500">
                  <option *ngFor="let ins of insumos" [value]="ins.id">{{ ins.nombre }}</option>
                </select>
                <input type="number" [(ngModel)]="selectedCantidad" min="1" class="w-20 bg-white/5 border border-white/10 rounded-lg px-3 py-2 text-sm text-foreground text-center focus:outline-none focus:border-indigo-500" />
                <button (click)="addLinea()" class="px-3 rounded-lg bg-indigo-600 hover:bg-indigo-500 text-white text-xs font-bold transition-all">+</button>
              </div>
            </div>

            <!-- Listado de Líneas Temporales -->
            <div class="border border-white/5 rounded-lg p-3 space-y-2 bg-white/[0.01]" *ngIf="newPedido.lineas.length > 0">
              <div class="flex justify-between items-center text-xs text-muted-foreground border-b border-white/5 pb-2">
                <span>Insumo</span>
                <span>Cant.</span>
              </div>
              <div *ngFor="let line of newPedido.lineas; let idx = index" class="flex justify-between items-center text-xs">
                <span class="text-foreground/80 truncate max-w-[150px]">{{ getInsumoName(line.insumoId) }}</span>
                <div class="flex items-center gap-2">
                  <span class="font-bold text-foreground">{{ line.cantidadSolicitada }}</span>
                  <button (click)="removeLinea(idx)" class="text-red-400 hover:text-red-300">x</button>
                </div>
              </div>
            </div>

            <div>
              <label class="text-xs text-muted-foreground font-semibold">Observaciones</label>
              <textarea [(ngModel)]="newPedido.observaciones" rows="3" class="w-full mt-1 bg-white/5 border border-white/10 rounded-lg px-3 py-2 text-sm text-foreground focus:outline-none focus:border-indigo-500"></textarea>
            </div>

            <button 
              (click)="crearSolicitud()"
              [disabled]="newPedido.lineas.length === 0 || !newPedido.sedeProveedoraId"
              class="w-full h-10 rounded-lg bg-gradient-to-r from-blue-600 to-indigo-600 hover:from-blue-500 hover:to-indigo-500 disabled:opacity-40 font-semibold text-white transition-all shadow-md text-xs"
            >
              Enviar Requisición
            </button>
          </div>
        </div>

        <!-- Panel de Pedidos Activos/Pendientes -->
        <div class="xl:col-span-2 rounded-xl border border-white/5 bg-surface/30 p-6 space-y-4 shadow-md">
          <h2 class="text-lg font-bold text-foreground">Pedidos en Tránsito / Pendientes</h2>

          <div class="space-y-4" *ngIf="pedidos.length > 0; else noPedidos">
            <div 
              *ngFor="let ped of pedidos" 
              class="border border-white/5 rounded-xl p-4 bg-white/[0.01] hover:border-white/10 transition-all space-y-3"
            >
              <div class="flex flex-wrap items-center justify-between gap-2 border-b border-white/5 pb-3">
                <div>
                  <span class="text-xs font-black tracking-wider uppercase text-indigo-400 bg-indigo-500/10 px-2 py-0.5 rounded">
                    {{ ped.correlativo }}
                  </span>
                  <span class="text-xs text-muted-foreground ml-2">
                    De: {{ ped.sedeSolicitanteNombre }} -> Para: {{ ped.sedeProveedoraNombre }}
                  </span>
                </div>
                <div class="flex items-center gap-2">
                  <span class="text-xs font-bold px-2.5 py-1 rounded-full uppercase tracking-wider text-[10px]" [ngClass]="{
                    'bg-yellow-500/20 text-yellow-400': ped.estado === 'Solicitado',
                    'bg-blue-500/20 text-blue-400': ped.estado === 'Despachado',
                    'bg-green-500/20 text-green-400': ped.estado === 'Recibido'
                  }">
                    {{ ped.estado }}
                  </span>
                </div>
              </div>

              <!-- Listado de Detalles de Items -->
              <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div class="space-y-2">
                  <h4 class="text-xs font-bold uppercase text-muted-foreground tracking-wider">Detalles de Insumos</h4>
                  <div *ngFor="let det of ped.detalles" class="text-xs flex justify-between bg-white/[0.01] p-2 rounded">
                    <span class="text-foreground/80 font-medium">{{ det.insumoNombre }}</span>
                    <span class="text-foreground font-black">
                      Sol: {{ det.cantidadSolicitada }} | Desp: {{ det.cantidadDespachada }} | Rec: {{ det.cantidadRecibida }}
                    </span>
                  </div>
                </div>

                <div class="flex flex-col justify-end gap-2" *ngIf="ped.estado !== 'Recibido'">
                  <!-- Acciones del Proveedor (Despacho) -->
                  <div class="flex justify-end gap-2" *ngIf="ped.estado === 'Solicitado' && isSedeProveedora(ped)">
                    <button 
                      (click)="despachar(ped.id)" 
                      class="px-4 py-2 bg-blue-600 hover:bg-blue-500 text-xs font-bold text-white rounded-lg transition-colors"
                    >
                      Aprobar y Despachar
                    </button>
                  </div>

                  <!-- Acciones del Solicitante (Recepción) -->
                  <div class="flex justify-end gap-2" *ngIf="ped.estado === 'Despachado' && isSedeSolicitante(ped)">
                    <button 
                      (click)="abrirRecepcionDialog(ped)" 
                      class="px-4 py-2 bg-green-600 hover:bg-green-500 text-xs font-bold text-white rounded-lg transition-colors"
                    >
                      Confirmar Recepción
                    </button>
                  </div>
                </div>
              </div>
            </div>
          </div>
          <ng-template #noPedidos>
            <p class="text-sm text-muted-foreground italic text-center py-8">No hay solicitudes activas ni traslados en curso.</p>
          </ng-template>
        </div>
      </div>

      <!-- Dialogo de Recepción con Discrepancias -->
      <div *ngIf="showRecepcionDialog" class="fixed inset-0 z-[1000] flex items-center justify-center bg-black/60 backdrop-blur-sm p-4">
        <div class="w-full max-w-md rounded-2xl border border-white/10 bg-surface p-6 shadow-2xl space-y-4">
          <h3 class="text-lg font-bold text-foreground">Confirmar Recepción de Pedido</h3>
          <p class="text-xs text-muted-foreground">Especifique la cantidad real de mercadería recibida física en bodega (para control de mermas).</p>
          
          <div class="space-y-3 max-h-60 overflow-y-auto">
            <div *ngFor="let det of selectedPedidoForRecepcion?.detalles" class="flex items-center justify-between text-sm">
              <span class="truncate max-w-[200px]">{{ det.insumoNombre }}</span>
              <div class="flex items-center gap-2">
                <span class="text-xs text-muted-foreground">(Despachado: {{ det.cantidadDespachada }})</span>
                <input 
                  type="number" 
                  [(ngModel)]="discrepanciasForm[det.insumoId]"
                  class="w-20 bg-white/5 border border-white/10 rounded-lg px-2 py-1 text-xs text-center text-foreground focus:outline-none focus:border-indigo-500" 
                />
              </div>
            </div>
          </div>

          <div class="flex justify-end gap-3 pt-2">
            <button (click)="closeRecepcionDialog()" class="px-4 py-2 rounded-lg bg-white/5 hover:bg-white/10 text-xs text-foreground font-medium transition-colors">Cancelar</button>
            <button (click)="guardarRecepcion()" class="px-4 py-2 rounded-lg bg-green-600 hover:bg-green-500 text-xs text-white font-semibold transition-colors">Recibir Carga</button>
          </div>
        </div>
      </div>
    </div>
  `
})
export class PedidosInterSedeComponent implements OnInit {
  public multiSedeService = inject(MultiSedeService);
  private inventoryService = inject(InventoryService);

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

  ngOnInit() {
    this.loadInitialData();
  }

  loadInitialData() {
    this.multiSedeService.getSedes().subscribe(res => {
      const activeSedes = res.filter(s => s.activo);
      this.sedes = activeSedes;
      if (activeSedes.length > 0) {
        const principal = activeSedes.find(s => s.esPrincipal) || activeSedes[0];
        this.newPedido.sedeSolicitanteId = principal.id;
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
        this.newPedido = {
          sedeSolicitanteId: prevSolicitante,
          sedeProveedoraId: '',
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
