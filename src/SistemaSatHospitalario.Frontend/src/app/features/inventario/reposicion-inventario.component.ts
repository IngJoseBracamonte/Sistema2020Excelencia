import { Component, ChangeDetectionStrategy, inject, signal, computed, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LucideAngularModule } from 'lucide-angular';
import { InventoryService } from '../../core/services/inventory.service';
import { Insumo, TransferenciaReposicionItem } from '../../core/models/inventory.model';
import { AreaClinica, MultiSedeService, Sede } from '../../core/services/multi-sede.service';

@Component({
  selector: 'app-reposicion-inventario',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="space-y-6 animate-fade-in">
      <!-- Encabezado de la Sección -->
      <div class="bg-gray-900/60 border border-gray-800/80 rounded-2xl p-5 backdrop-blur-md shadow-xl flex flex-wrap items-center justify-between gap-4">
        <div class="flex items-center gap-3">
          <div class="p-3 bg-emerald-500/10 border border-emerald-500/20 rounded-2xl text-emerald-400">
            <lucide-icon name="repeat" class="w-6 h-6"></lucide-icon>
          </div>
          <div>
            <h2 class="text-base font-bold text-white uppercase tracking-wider">Apartado de Reposición e Intercambio de Insumos</h2>
            <p class="text-xs text-gray-400">Gestión de reposiciones, devoluciones de insumos y cambios de talla entre sedes y sub-áreas sin desfase de inventario</p>
          </div>
        </div>

        <button
          (click)="cargarHistorial()"
          class="px-3.5 py-2 bg-gray-800 hover:bg-gray-700 text-gray-300 rounded-xl text-xs font-semibold transition flex items-center gap-1.5"
        >
          <lucide-icon name="refresh-cw" class="w-3.5 h-3.5"></lucide-icon>
          Actualizar Historial
        </button>
      </div>

      <!-- Formulario de Reposición / Intercambio Multi-Sede -->
      <div class="bg-gray-900/60 border border-gray-800/80 rounded-2xl p-6 backdrop-blur-md shadow-xl space-y-4">
        <h3 class="text-xs font-bold text-gray-300 uppercase tracking-wider flex items-center gap-2">
          <lucide-icon name="arrow-left-right" class="w-4 h-4 text-sky-400"></lucide-icon>
          Registrar Operación de Reposición o Cambio de Insumo
        </h3>

        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
          <!-- Insumo / Medicamento (búsqueda por código o nombre, compatible con lector de barras) -->
          <div class="lg:col-span-2 relative">
            <label class="text-[11px] font-semibold text-gray-400 block mb-1.5">Insumo o Medicamento</label>
            <input
              type="text"
              [ngModel]="insumoBusqueda()"
              (ngModelChange)="onBusquedaInsumoChange($event)"
              (focus)="mostrarSugerenciasInsumo.set(true)"
              (blur)="onBlurBusquedaInsumo()"
              placeholder="Escanee código o escriba el nombre del insumo..."
              class="w-full bg-gray-950 border border-gray-800 rounded-xl p-2.5 text-xs text-white placeholder-gray-500 focus:border-sky-500 focus:ring-1 focus:ring-sky-500 transition"
              autocomplete="off"
            />
            @if (mostrarSugerenciasInsumo() && sugerenciasInsumo().length > 0) {
              <div class="absolute z-50 left-0 right-0 mt-1 bg-gray-900 border border-gray-700 rounded-xl shadow-2xl max-h-60 overflow-y-auto">
                @for (ins of sugerenciasInsumo(); track ins.id) {
                  <button
                    type="button"
                    (mousedown)="seleccionarInsumo(ins)"
                    class="w-full text-left px-3 py-2 text-xs hover:bg-sky-500/10 border-b border-gray-800/60 last:border-0 transition"
                  >
                    <span class="font-bold text-white block">{{ ins.nombre }}</span>
                    <span class="text-[10px] text-gray-400 font-mono">{{ ins.codigo }}</span>
                  </button>
                }
              </div>
            }
            @if (mostrarSugerenciasInsumo() && insumoBusqueda().length > 0 && sugerenciasInsumo().length === 0) {
              <div class="absolute z-50 left-0 right-0 mt-1 bg-gray-900 border border-gray-700 rounded-xl shadow-2xl p-3 text-xs text-gray-500">
                No se encontraron insumos con ese código o nombre.
              </div>
            }
          </div>

          <!-- Sede / Sub-Área Origen -->
          <div>
            <label class="text-[11px] font-semibold text-gray-400 block mb-1.5">Sede / Sub-Área Origen</label>
            <select
              [(ngModel)]="sedeOrigenId"
              class="w-full bg-gray-950 border border-gray-800 rounded-xl p-2.5 text-xs text-white focus:border-sky-500 focus:ring-1 focus:ring-sky-500 transition"
            >
              @for (s of sedes(); track s.id) {
                <option [value]="s.id">{{ etiquetaAreaYSubAreas(s) }}</option>
              }
            </select>
          </div>

          <!-- Sede / Sub-Área Destino -->
          <div>
            <label class="text-[11px] font-semibold text-gray-400 block mb-1.5">Sede / Sub-Área Destino</label>
            <select
              [(ngModel)]="sedeDestinoId"
              class="w-full bg-gray-950 border border-gray-800 rounded-xl p-2.5 text-xs text-white focus:border-sky-500 focus:ring-1 focus:ring-sky-500 transition"
            >
              @for (s of sedes(); track s.id) {
                <option [value]="s.id">{{ etiquetaAreaYSubAreas(s) }}</option>
              }
            </select>
          </div>

          <!-- Motivo DB-Driven -->
          <div>
            <label class="text-[11px] font-semibold text-gray-400 block mb-1.5">Motivo de la Operación</label>
            <select
              [(ngModel)]="motivoOperacion"
              class="w-full bg-gray-950 border border-gray-800 rounded-xl p-2.5 text-xs text-white focus:border-sky-500 focus:ring-1 focus:ring-sky-500 transition"
            >
              <option value="Devolución por Talla / Calibre Incorrecto">Devolución por Talla / Calibre</option>
              <option value="Reposición por Insumo Dañado o Vencido">Reposición por Insumo Dañado</option>
              <option value="Transferencia Inter-Sede Operativa">Transferencia Inter-Sede</option>
              <option value="Reingreso Sobrante de Quirófano">Reingreso de Quirófano</option>
              <option value="Ajuste de Auditoría de Inventario">Ajuste de Auditoría</option>
            </select>
          </div>

          <!-- Cantidad -->
          <div>
            <label class="text-[11px] font-semibold text-gray-400 block mb-1.5">Cantidad</label>
            <input
              type="number"
              min="1"
              [(ngModel)]="cantidadTransferencia"
              class="w-full bg-gray-950 border border-gray-800 rounded-xl p-2.5 text-xs text-white font-mono focus:border-sky-500 focus:ring-1 focus:ring-sky-500 transition"
            />
          </div>

          <!-- Observaciones -->
          <div class="lg:col-span-2">
            <label class="text-[11px] font-semibold text-gray-400 block mb-1.5">Observaciones de Auditoría</label>
            <input
              type="text"
              [(ngModel)]="observacionesOperacion"
              placeholder="Ej. Cambio de guantes talla 7.5 por talla 8.0 devueltos en buenas condiciones"
              class="w-full bg-gray-950 border border-gray-800 rounded-xl p-2.5 text-xs text-white placeholder-gray-500 focus:border-sky-500 focus:ring-1 focus:ring-sky-500 transition"
            />
          </div>
        </div>

        <!-- Alerta de Validación / Mensaje -->
        @if (mensajeFeedback()) {
          <div
            class="p-3 rounded-xl border text-xs font-semibold flex items-center gap-2 animate-fade-in"
            [ngClass]="esErrorFeedback() ? 'bg-rose-500/10 border-rose-500/20 text-rose-300' : 'bg-emerald-500/10 border-emerald-500/20 text-emerald-300'"
          >
            <lucide-icon [name]="esErrorFeedback() ? 'alert-octagon' : 'check-circle-2'" class="w-4 h-4"></lucide-icon>
            <span>{{ mensajeFeedback() }}</span>
          </div>
        }

        <!-- Botón de Ejecución -->
        <div class="flex justify-end pt-2">
          <button
            (click)="ejecutarReposicion()"
            [disabled]="procesando()"
            class="px-5 py-2.5 bg-emerald-600 hover:bg-emerald-500 text-white rounded-xl text-xs font-bold transition flex items-center gap-2 shadow-lg shadow-emerald-900/30 disabled:opacity-50"
          >
            <lucide-icon name="check-check" class="w-4 h-4"></lucide-icon>
            {{ procesando() ? 'Procesando Transferencia...' : 'Confirmar y Actualizar Stock' }}
          </button>
        </div>
      </div>

      <!-- Tabla de Historial de Reposiciones y Transferencias -->
      <div class="bg-gray-900/60 border border-gray-800/80 rounded-2xl overflow-hidden backdrop-blur-md shadow-xl">
        <div class="p-4 border-b border-gray-800 flex justify-between items-center bg-gray-950/60">
          <div class="flex items-center gap-2">
            <lucide-icon name="history" class="w-4 h-4 text-sky-400"></lucide-icon>
            <h3 class="text-xs font-bold text-white uppercase tracking-wider">Historial de Reposiciones y Transferencias</h3>
          </div>
          <span class="text-xs font-mono font-bold text-gray-400">{{ historial().length }} Registros</span>
        </div>

        <div class="overflow-x-auto">
          <table class="w-full text-left text-xs text-gray-300">
            <thead class="bg-gray-950/80 text-[10px] text-gray-400 uppercase tracking-wider border-b border-gray-800">
              <tr>
                <th class="p-3.5">Fecha & Hora</th>
                <th class="p-3.5">Insumo</th>
                <th class="p-3.5">Origen (Descuento)</th>
                <th class="p-3.5">Destino (Ingreso)</th>
                <th class="p-3.5 text-center">Cantidad</th>
                <th class="p-3.5">Motivo</th>
                <th class="p-3.5">Usuario Auditor</th>
                <th class="p-3.5">Detalles</th>
              </tr>
            </thead>
            <tbody class="divide-y divide-gray-800/60">
              @if (historial().length === 0) {
                <tr>
                  <td colspan="8" class="text-center py-10 text-gray-500 text-xs">
                    No se han registrado reposiciones o transferencias de insumos en el período.
                  </td>
                </tr>
              }

              @for (h of historial(); track h.id) {
                <tr class="hover:bg-gray-850/40 transition">
                  <td class="p-3.5 font-mono text-[11px] text-gray-400">
                    {{ h.fechaTransferencia | date:'dd/MM/yyyy hh:mm a' }}
                  </td>
                  <td class="p-3.5">
                    <span class="font-bold text-white block">{{ h.insumoNombre }}</span>
                    <span class="text-[10px] text-gray-500 font-mono">{{ h.insumoCodigo }}</span>
                  </td>
                  <td class="p-3.5">
                    <span class="text-rose-400 font-semibold bg-rose-500/10 px-2 py-0.5 rounded border border-rose-500/20">
                      {{ h.sedeOrigenNombre }}
                    </span>
                  </td>
                  <td class="p-3.5">
                    <span class="text-emerald-400 font-semibold bg-emerald-500/10 px-2 py-0.5 rounded border border-emerald-500/20">
                      {{ h.sedeDestinoNombre }}
                    </span>
                  </td>
                  <td class="p-3.5 text-center font-mono font-bold text-sky-400">
                    {{ h.cantidad }}
                  </td>
                  <td class="p-3.5">
                    <span class="text-[10px] font-bold text-amber-300 bg-amber-500/10 px-2 py-0.5 rounded border border-amber-500/20">
                      {{ h.motivo }}
                    </span>
                  </td>
                  <td class="p-3.5 text-gray-400">
                    {{ h.usuarioId }}
                  </td>
                  <td class="p-3.5 text-gray-400 text-[11px]">
                    {{ h.observaciones || '-' }}
                  </td>
                </tr>
              }
            </tbody>
          </table>
        </div>
      </div>
    </div>
  `
})
export class ReposicionInventarioComponent implements OnInit {
  private inventoryService = inject(InventoryService);
  private multiSedeService = inject(MultiSedeService);

  catalogoInsumos = signal<Insumo[]>([]);
  sedes = signal<Sede[]>([]);
  subAreas = signal<AreaClinica[]>([]);
  historial = signal<TransferenciaReposicionItem[]>([]);

  insumoSeleccionadoId = '';
  sedeOrigenId = '';
  sedeDestinoId = '';
  motivoOperacion = 'Devolución por Talla / Calibre Incorrecto';
  cantidadTransferencia = 1;
  observacionesOperacion = '';

  procesando = signal<boolean>(false);
  mensajeFeedback = signal<string>('');
  esErrorFeedback = signal<boolean>(false);

  // Búsqueda de insumo por código o nombre (compatible con lector de código de barras)
  insumoBusqueda = signal<string>('');
  mostrarSugerenciasInsumo = signal<boolean>(false);

  sugerenciasInsumo = computed(() => {
    const term = this.insumoBusqueda().trim().toLowerCase();
    const catalogo = this.catalogoInsumos();
    if (!term) return catalogo.slice(0, 50);
    return catalogo
      .filter(i =>
        i.nombre.toLowerCase().includes(term) ||
        (i.codigo && i.codigo.toLowerCase().includes(term))
      )
      .slice(0, 50);
  });

  onBusquedaInsumoChange(valor: string) {
    this.insumoBusqueda.set(valor);
    this.mostrarSugerenciasInsumo.set(true);

    // Lector de código de barras: coincidencia exacta por código → selección automática
    const term = valor.trim().toLowerCase();
    if (term.length >= 4) {
      const exacto = this.catalogoInsumos().find(i => i.codigo && i.codigo.toLowerCase() === term);
      if (exacto) {
        this.seleccionarInsumo(exacto);
      }
    }
  }

  seleccionarInsumo(ins: Insumo) {
    this.insumoSeleccionadoId = ins.id;
    this.insumoBusqueda.set(`${ins.nombre} (${ins.codigo})`);
    this.mostrarSugerenciasInsumo.set(false);
  }

  onBlurBusquedaInsumo() {
    // Pequeño delay para permitir el click en la sugerencia antes de cerrar
    setTimeout(() => this.mostrarSugerenciasInsumo.set(false), 200);
  }

  ngOnInit() {
    this.cargarDatosMaestros();
    this.cargarHistorial();
  }

  cargarDatosMaestros() {
    this.inventoryService.getInsumos().subscribe(ins => {
      this.catalogoInsumos.set(ins || []);
      if (ins && ins.length > 0) {
        this.insumoSeleccionadoId = ins[0].id;
      }
    });

    this.multiSedeService.getSedes().subscribe(sedes => {
      this.sedes.set(sedes || []);
      if (sedes && sedes.length >= 2) {
        this.sedeOrigenId = sedes[0].id;
        this.sedeDestinoId = sedes[1].id;
      } else if (sedes && sedes.length === 1) {
        this.sedeOrigenId = sedes[0].id;
        this.sedeDestinoId = sedes[0].id;
      }
    });

    this.multiSedeService.getAreasClinicas().subscribe(areas => {
      this.subAreas.set((areas || []).filter(area => area.activo !== false));
    });
  }

  etiquetaAreaYSubAreas(sede: Sede): string {
    const subAreas = this.subAreas()
      .filter(area => area.sedeId === sede.id || area.sedeNombre === sede.nombre)
      .map(area => area.nombre);
    const area = sede.esPrincipal ? `${sede.nombre} (Almacén Principal)` : sede.nombre;

    return subAreas.length > 0
      ? `Área: ${area} | Subáreas: ${subAreas.join(', ')}`
      : `Área: ${area}`;
  }

  cargarHistorial() {
    this.inventoryService.getReposicionesHistorial().subscribe(items => {
      this.historial.set(items || []);
    });
  }

  ejecutarReposicion() {
    this.mensajeFeedback.set('');
    this.esErrorFeedback.set(false);

    if (!this.insumoSeleccionadoId) {
      this.mostrarMensaje('Debe seleccionar un insumo o medicamento.', true);
      return;
    }

    if (!this.sedeOrigenId || !this.sedeDestinoId) {
      this.mostrarMensaje('Debe seleccionar sede de origen y sede de destino.', true);
      return;
    }

    if (this.sedeOrigenId === this.sedeDestinoId) {
      this.mostrarMensaje('La sede de origen y la de destino deben ser distintas para realizar una transferencia.', true);
      return;
    }

    if (this.cantidadTransferencia <= 0) {
      this.mostrarMensaje('La cantidad a transferir debe ser mayor a cero.', true);
      return;
    }

    this.procesando.set(true);
    this.inventoryService.procesarReposicion({
      insumoId: this.insumoSeleccionadoId,
      sedeOrigenId: this.sedeOrigenId,
      sedeDestinoId: this.sedeDestinoId,
      cantidad: this.cantidadTransferencia,
      motivo: this.motivoOperacion,
      observaciones: this.observacionesOperacion
    }).subscribe({
      next: () => {
        this.procesando.set(false);
        this.mostrarMensaje('Reposición / Transferencia procesada exitosamente. Stock actualizado en ambas sedes.', false);
        this.observacionesOperacion = '';
        this.cantidadTransferencia = 1;
        this.cargarHistorial();
      },
      error: (err) => {
        this.procesando.set(false);
        this.mostrarMensaje(err.error?.message || 'Error al procesar la reposición.', true);
      }
    });
  }

  private mostrarMensaje(msg: string, esError: boolean) {
    this.mensajeFeedback.set(msg);
    this.esErrorFeedback.set(esError);
  }
}
