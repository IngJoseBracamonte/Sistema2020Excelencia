import { Component, inject, Input, Output, EventEmitter, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PabellonService, OrdenCirugiaDetalle, InsumoCirugiaConsumo, CirugiaLog } from '../../../core/services/pabellon.service';
import { InventoryService } from '../../../core/services/inventory.service';
import { Insumo } from '../../../core/models/inventory.model';
import { 
  LucideAngularModule, 
  X, 
  RotateCcw, 
  Plus, 
  History, 
  CheckCircle2, 
  AlertCircle, 
  Package, 
  DollarSign,
  FileText,
  Search
} from 'lucide-angular';

@Component({
  selector: 'app-gestion-consumo-modal',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule],
  template: `
    <div class="fixed inset-0 bg-black/70 backdrop-blur-sm z-50 flex items-center justify-center p-4">
      <div class="bg-gray-900 border border-gray-800 rounded-xl shadow-2xl w-full max-w-4xl max-h-[90vh] flex flex-col text-white">
        
        <!-- Header -->
        <div class="px-6 py-4 border-b border-gray-800 flex items-center justify-between bg-gray-950/50 rounded-t-xl">
          <div>
            <h2 class="text-xl font-bold text-white flex items-center gap-2">
              <lucide-icon name="package" class="w-5 h-5 text-sky-400"></lucide-icon>
              Gestión de Consumo y Pabellón
            </h2>
            <p class="text-xs text-gray-400 mt-0.5" *ngIf="detalle()">
              Paciente: <span class="text-gray-200 font-semibold">{{ detalle()?.pacienteNombre }}</span> ({{ detalle()?.pacienteCedula }}) |
              Cirugía: <span class="text-sky-400 font-semibold">{{ detalle()?.descripcionCirugia }}</span>
            </p>
          </div>
          <button (click)="cerrarModal.emit()" class="text-gray-400 hover:text-white p-1 rounded-lg hover:bg-gray-800 transition">
            <lucide-icon name="x" class="w-5 h-5"></lucide-icon>
          </button>
        </div>

        <!-- Navigation Tabs -->
        <div class="flex border-b border-gray-800 bg-gray-950/30 px-6 gap-2 pt-2">
          <button 
            (click)="activeTab.set('devolucion')"
            [class.border-sky-500]="activeTab() === 'devolucion'"
            [class.text-sky-400]="activeTab() === 'devolucion'"
            [class.border-transparent]="activeTab() !== 'devolucion'"
            [class.text-gray-400]="activeTab() !== 'devolucion'"
            class="px-4 py-2 text-sm font-semibold border-b-2 transition flex items-center gap-2">
            <lucide-icon name="rotate-ccw" class="w-4 h-4"></lucide-icon>
            Devolución a Sede Principal
          </button>
          <button 
            (click)="activeTab.set('carrito')"
            [class.border-sky-500]="activeTab() === 'carrito'"
            [class.text-sky-400]="activeTab() === 'carrito'"
            [class.border-transparent]="activeTab() !== 'carrito'"
            [class.text-gray-400]="activeTab() !== 'carrito'"
            class="px-4 py-2 text-sm font-semibold border-b-2 transition flex items-center gap-2">
            <lucide-icon name="plus" class="w-4 h-4"></lucide-icon>
            Anexar Cargo Extra
          </button>
          <button 
            (click)="activeTab.set('logs')"
            [class.border-sky-500]="activeTab() === 'logs'"
            [class.text-sky-400]="activeTab() === 'logs'"
            [class.border-transparent]="activeTab() !== 'logs'"
            [class.text-gray-400]="activeTab() !== 'logs'"
            class="px-4 py-2 text-sm font-semibold border-b-2 transition flex items-center gap-2">
            <lucide-icon name="history" class="w-4 h-4"></lucide-icon>
            Logs de Auditoría ({{ detalle()?.logs?.length || 0 }})
          </button>
        </div>

        <!-- Content Body -->
        <div class="p-6 overflow-y-auto flex-1 space-y-4">

          <!-- TAB 1: Devolución a Sede Principal -->
          <div *ngIf="activeTab() === 'devolucion'" class="space-y-4">
            <div class="bg-sky-950/30 border border-sky-800/40 rounded-lg p-3 text-xs text-sky-300 flex items-start gap-2">
              <lucide-icon name="alert-circle" class="w-4 h-4 mt-0.5 shrink-0"></lucide-icon>
              <span>Especifique la cantidad real consumida en quirófano. Los insumos sobrantes serán reintegrados automáticamente al stock de la Sede Principal y el cargo en cuenta se ajustará en dólares.</span>
            </div>

            <div *ngIf="!detalle()?.insumosAsignados?.length" class="text-center py-8 text-gray-500 text-sm">
              No hay insumos o kits despachados a esta cirugía.
            </div>

            <table *ngIf="detalle()?.insumosAsignados?.length" class="w-full text-left text-xs border-collapse">
              <thead>
                <tr class="border-b border-gray-800 text-gray-400 bg-gray-950/50">
                  <th class="py-2.5 px-3">Insumo</th>
                  <th class="py-2.5 px-3 text-center">Entregado</th>
                  <th class="py-2.5 px-3 text-center">Devuelto Previo</th>
                  <th class="py-2.5 px-3 text-center">Cantidad Usada Real</th>
                  <th class="py-2.5 px-3 text-center">A Devolver</th>
                </tr>
              </thead>
              <tbody class="divide-y divide-gray-800/50">
                <tr *ngFor="let item of itemsDevolucion()" class="hover:bg-gray-800/30">
                  <td class="py-2.5 px-3">
                    <div class="font-medium text-gray-200">{{ item.insumoNombre }}</div>
                    <div class="text-[10px] text-gray-500">{{ item.insumoCodigo }} - $ {{ item.precioUnitarioUsd }} USD</div>
                  </td>
                  <td class="py-2.5 px-3 text-center font-mono text-gray-300">{{ item.cantidadEntregada }}</td>
                  <td class="py-2.5 px-3 text-center font-mono text-amber-400">{{ item.cantidadDevuelta }}</td>
                  <td class="py-2.5 px-3 text-center">
                    <input 
                      type="number" 
                      min="0" 
                      [max]="item.cantidadEntregada - item.cantidadDevuelta"
                      [(ngModel)]="item.cantidadUsada" 
                      class="w-20 bg-gray-950 border border-gray-700 rounded px-2 py-1 text-center text-xs font-mono text-white focus:border-sky-500 focus:outline-none">
                  </td>
                  <td class="py-2.5 px-3 text-center font-mono font-bold" [class.text-emerald-400]="getCantidadADevolver(item) > 0" [class.text-gray-500]="getCantidadADevolver(item) <= 0">
                    {{ getCantidadADevolver(item) }}
                  </td>
                </tr>
              </tbody>
            </table>

            <div class="flex justify-end pt-2">
              <button 
                (click)="ejecutarDevolucionMasiva()"
                [disabled]="procesando()"
                class="bg-emerald-600 hover:bg-emerald-500 text-white font-semibold text-xs px-4 py-2 rounded-lg transition flex items-center gap-2 disabled:opacity-50">
                <lucide-icon name="check-circle-2" class="w-4 h-4"></lucide-icon>
                Confirmar Devolución a Sede Principal
              </button>
            </div>
          </div>

          <!-- TAB 2: Anexar Cargo Extra -->
          <div *ngIf="activeTab() === 'carrito'" class="space-y-4">
            <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
              <!-- Búsqueda de Insumo o Servicio -->
              <div class="space-y-2">
                <label class="text-xs font-semibold text-gray-300">Buscar Insumo o Servicio</label>
                <div class="relative">
                  <input 
                    type="text" 
                    [(ngModel)]="busquedaTermino" 
                    (input)="buscarInsumos()"
                    placeholder="Escriba código o nombre..." 
                    class="w-full bg-gray-950 border border-gray-700 rounded-lg px-3 py-2 text-xs text-white pl-8 focus:border-sky-500 focus:outline-none">
                  <lucide-icon name="search" class="w-4 h-4 text-gray-500 absolute left-2.5 top-2.5"></lucide-icon>
                </div>
                <div *ngIf="insumosEncontrados().length" class="max-h-40 overflow-y-auto border border-gray-800 rounded-lg bg-gray-950 divide-y divide-gray-800">
                  <button 
                    *ngFor="let ins of insumosEncontrados()" 
                    (click)="seleccionarInsumoExtra(ins)"
                    class="w-full text-left p-2 hover:bg-gray-800 text-xs flex justify-between items-center">
                    <div>
                      <div class="font-semibold text-gray-200">{{ ins.nombre }}</div>
                      <div class="text-[10px] text-gray-500">{{ ins.codigo }}</div>
                    </div>
                    <span class="font-mono text-emerald-400">$ {{ ins.costoUnitarioBaseUSD }} USD</span>
                  </button>
                </div>
              </div>

              <!-- Formulario de adición de cargo -->
              <div class="space-y-3 bg-gray-950 p-4 rounded-lg border border-gray-800">
                <h4 class="text-xs font-bold text-sky-400">Detalle del Cargo Extra</h4>
                <div>
                  <label class="text-[11px] text-gray-400">Descripción</label>
                  <input type="text" [(ngModel)]="cargoForm.descripcion" class="w-full bg-gray-900 border border-gray-700 rounded px-2.5 py-1.5 text-xs text-white focus:outline-none">
                </div>
                <div class="grid grid-cols-2 gap-2">
                  <div>
                    <label class="text-[11px] text-gray-400">Precio USD ($)</label>
                    <input type="number" [(ngModel)]="cargoForm.precio" class="w-full bg-gray-900 border border-gray-700 rounded px-2.5 py-1.5 text-xs text-white focus:outline-none">
                  </div>
                  <div>
                    <label class="text-[11px] text-gray-400">Cantidad</label>
                    <input type="number" min="1" [(ngModel)]="cargoForm.cantidad" class="w-full bg-gray-900 border border-gray-700 rounded px-2.5 py-1.5 text-xs text-white focus:outline-none">
                  </div>
                </div>
                <button 
                  (click)="agregarCargoExtra()"
                  [disabled]="!cargoForm.descripcion || cargoForm.cantidad <= 0 || procesando()"
                  class="w-full bg-sky-600 hover:bg-sky-500 text-white font-semibold text-xs py-2 rounded-lg transition flex items-center justify-center gap-2 disabled:opacity-50">
                  <lucide-icon name="plus" class="w-4 h-4"></lucide-icon>
                  Anexar a la Cuenta de Cirugía
                </button>
              </div>
            </div>
          </div>

          <!-- TAB 3: Logs de Auditoría -->
          <div *ngIf="activeTab() === 'logs'" class="space-y-3">
            <div *ngIf="!detalle()?.logs?.length" class="text-center py-8 text-gray-500 text-sm">
              No hay logs de auditoría registrados para esta cirugía.
            </div>
            <div *ngFor="let log of detalle()?.logs" class="bg-gray-950 border border-gray-800 rounded-lg p-3 flex items-start justify-between">
              <div>
                <div class="flex items-center gap-2">
                  <span class="px-2 py-0.5 rounded text-[10px] font-bold uppercase"
                    [class.bg-sky-950]="log.evento === 'Creacion'"
                    [class.text-sky-300]="log.evento === 'Creacion'"
                    [class.bg-emerald-950]="log.evento === 'DevolucionInsumos'"
                    [class.text-emerald-300]="log.evento === 'DevolucionInsumos'"
                    [class.bg-amber-950]="log.evento === 'CargoExtra'"
                    [class.text-amber-300]="log.evento === 'CargoExtra'"
                    [class.bg-purple-950]="log.evento === 'TransicionEstado'"
                    [class.text-purple-300]="log.evento === 'TransicionEstado'">
                    {{ log.evento }}
                  </span>
                  <span class="text-xs font-medium text-gray-300">por {{ log.usuarioId }}</span>
                </div>
                <p class="text-xs text-gray-400 mt-1">{{ log.detalle }}</p>
              </div>
              <span class="text-[10px] text-gray-500 font-mono shrink-0">{{ log.timestamp | date:'short' }}</span>
            </div>
          </div>

        </div>

        <!-- Footer -->
        <div class="px-6 py-3 border-t border-gray-800 bg-gray-950/50 flex justify-end rounded-b-xl">
          <button (click)="cerrarModal.emit()" class="bg-gray-800 hover:bg-gray-700 text-gray-300 text-xs font-semibold px-4 py-2 rounded-lg transition">
            Cerrar
          </button>
        </div>

      </div>
    </div>
  `
})
export class GestionConsumoModalComponent implements OnInit {
  @Input({ required: true }) ordenId!: string;
  @Output() cerrarModal = new EventEmitter<void>();
  @Output() datosActualizados = new EventEmitter<void>();

  private pabellonService = inject(PabellonService);
  private inventoryService = inject(InventoryService);

  public detalle = signal<OrdenCirugiaDetalle | null>(null);
  public activeTab = signal<'devolucion' | 'carrito' | 'logs'>('devolucion');
  public itemsDevolucion = signal<(InsumoCirugiaConsumo & { cantidadUsada: number })[]>([]);
  public procesando = signal<boolean>(false);

  // Búsqueda y formulario para cargo extra
  public busquedaTermino = '';
  public insumosEncontrados = signal<Insumo[]>([]);
  public cargoForm = {
    servicioId: '00000000-0000-0000-0000-000000000000',
    descripcion: '',
    precio: 0,
    honorario: 0,
    cantidad: 1,
    tipoServicio: 'Insumo'
  };

  ngOnInit(): void {
    this.cargarDetalle();
  }

  cargarDetalle(): void {
    this.pabellonService.getOrdenDetalle(this.ordenId).subscribe({
      next: (res) => {
        this.detalle.set(res);
        const mapped = (res.insumosAsignados || []).map(i => ({
          ...i,
          cantidadUsada: i.cantidadConsumida
        }));
        this.itemsDevolucion.set(mapped);
      }
    });
  }

  getCantidadADevolver(item: InsumoCirugiaConsumo & { cantidadUsada: number }): number {
    const disponible = item.cantidadEntregada - item.cantidadDevuelta;
    return Math.max(0, disponible - item.cantidadUsada);
  }

  ejecutarDevolucionMasiva(): void {
    const det = this.detalle();
    if (!det) return;

    this.procesando.set(true);
    const payload = {
      ordenCirugiaId: det.id,
      cuentaId: det.cuentaServicioId,
      items: this.itemsDevolucion().map(i => ({
        insumoId: i.insumoId,
        cantidadUsada: i.cantidadUsada
      }))
    };

    this.pabellonService.devolucionMasiva(payload).subscribe({
      next: () => {
        this.procesando.set(false);
        this.cargarDetalle();
        this.datosActualizados.emit();
      },
      error: () => this.procesando.set(false)
    });
  }

  buscarInsumos(): void {
    if (!this.busquedaTermino.trim()) {
      this.insumosEncontrados.set([]);
      return;
    }
    this.inventoryService.getInsumos(true, this.busquedaTermino).subscribe({
      next: (res) => this.insumosEncontrados.set(res || [])
    });
  }

  seleccionarInsumoExtra(insumo: Insumo): void {
    this.cargoForm.servicioId = insumo.id;
    this.cargoForm.descripcion = insumo.nombre;
    this.cargoForm.precio = insumo.costoUnitarioBaseUSD;
    this.cargoForm.tipoServicio = 'Insumo';
    this.insumosEncontrados.set([]);
    this.busquedaTermino = '';
  }

  agregarCargoExtra(): void {
    const det = this.detalle();
    if (!det) return;

    this.procesando.set(true);
    this.pabellonService.cargoExtra({
      ordenCirugiaId: det.id,
      cuentaServicioId: det.cuentaServicioId,
      servicioId: this.cargoForm.servicioId,
      descripcion: this.cargoForm.descripcion,
      precio: this.cargoForm.precio,
      honorario: this.cargoForm.honorario,
      cantidad: this.cargoForm.cantidad,
      tipoServicio: this.cargoForm.tipoServicio
    }).subscribe({
      next: () => {
        this.procesando.set(false);
        this.cargoForm = {
          servicioId: '00000000-0000-0000-0000-000000000000',
          descripcion: '',
          precio: 0,
          honorario: 0,
          cantidad: 1,
          tipoServicio: 'Insumo'
        };
        this.cargarDetalle();
        this.datosActualizados.emit();
      },
      error: () => this.procesando.set(false)
    });
  }
}
