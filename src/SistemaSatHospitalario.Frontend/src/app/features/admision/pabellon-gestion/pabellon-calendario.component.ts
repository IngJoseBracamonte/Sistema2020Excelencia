import { Component, ChangeDetectionStrategy, input, output, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LucideAngularModule } from 'lucide-angular';
import { CirugiaCalendarioItem } from '../../../core/services/pabellon.service';
import { AreaClinica } from '../../../core/services/multi-sede.service';

@Component({
  selector: 'app-pabellon-calendario',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="space-y-4">
      <!-- Barra de Filtros del Calendario -->
      <div class="bg-gray-900/60 border border-gray-800/80 rounded-2xl p-4 backdrop-blur-md shadow-xl flex flex-wrap items-center justify-between gap-4">
        <div class="flex items-center gap-3">
          <div class="p-2.5 bg-sky-500/10 border border-sky-500/20 rounded-xl text-sky-400">
            <lucide-icon name="calendar-days" class="w-5 h-5"></lucide-icon>
          </div>
          <div>
            <h2 class="text-sm font-bold text-white uppercase tracking-wider">Calendario Quirúrgico Total</h2>
            <p class="text-xs text-gray-400">Programación de intervenciones, ocupación de quirófanos y salas clínicas</p>
          </div>
        </div>

        <div class="flex flex-wrap items-center gap-3 w-full sm:w-auto">
          <!-- ComboList con Buscador para Quirófano / Habitación -->
          <div class="flex items-center gap-2 bg-gray-950/90 px-3 py-1.5 rounded-xl border border-gray-800 focus-within:border-sky-500/80 transition shadow-inner">
            <lucide-icon name="door-open" class="w-4 h-4 text-sky-400 shrink-0"></lucide-icon>
            <span class="text-[11px] font-bold text-gray-400 uppercase tracking-wider shrink-0">Habitación / Quirófano:</span>
            <select
              [ngModel]="salaFiltro()"
              (ngModelChange)="salaFiltro.set($event)"
              class="bg-transparent text-xs font-semibold text-white focus:outline-none cursor-pointer pr-4"
            >
              <option value="" class="bg-gray-900 text-gray-200">Todas las Habitaciones / Salas ({{ cirugias().length }})</option>
              @for (sala of salasDisponibles(); track sala) {
                <option [value]="sala" class="bg-gray-900 text-white">{{ sala }}</option>
              }
            </select>
          </div>

          <!-- Totalizador -->
          <span class="text-xs font-mono font-bold text-sky-400 bg-sky-500/10 px-3 py-2 rounded-xl border border-sky-500/20 shadow-sm">
            {{ cirugiasFiltradas().length }} Agendadas
          </span>
        </div>
      </div>

      <!-- Grid de Cirugías en Calendario -->
      <div *ngIf="cirugiasFiltradas().length === 0" class="text-center py-16 text-gray-400 text-xs bg-gray-900/40 rounded-2xl border border-gray-800/60 p-6">
        <lucide-icon name="calendar-x" class="w-10 h-10 mx-auto text-gray-600 mb-3"></lucide-icon>
        <p class="font-semibold text-gray-300">No hay cirugías programadas para este filtro</p>
        <p class="text-gray-500 mt-1">Seleccione otra habitación o quirófano, o agregue una nueva intervención desde el tablero de gestión.</p>
      </div>

      <div *ngIf="cirugiasFiltradas().length > 0" class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-4">
        @for (item of cirugiasFiltradas(); track item.id) {
          <div
            (click)="seleccionarCirugia.emit(item)"
            class="bg-gray-900/70 p-4 rounded-2xl border border-gray-800 hover:border-sky-500/50 transition cursor-pointer group shadow-xl relative overflow-hidden flex flex-col justify-between"
          >
            <!-- Barra Superior: Sala y Horario -->
            <div>
              <div class="flex justify-between items-start mb-2.5">
                <span class="text-[10px] font-extrabold px-2.5 py-1 rounded-full uppercase tracking-wider" [ngClass]="getEstadoClass(item.estado)">
                  {{ item.estado }}
                </span>
                <span class="text-xs font-mono font-bold text-sky-300 bg-sky-500/10 px-2 py-0.5 rounded-lg border border-sky-500/20">
                  {{ item.fechaHoraProgramada | date:'shortTime' }}
                </span>
              </div>

              <!-- Indicadores de Alquiler y Sala -->
              <div class="flex flex-wrap items-center gap-1.5 mb-2">
                <span class="text-[10px] font-semibold text-gray-300 bg-gray-950 px-2 py-0.5 rounded border border-gray-800 flex items-center gap-1">
                  <lucide-icon name="door-open" class="w-3 h-3 text-sky-400"></lucide-icon>
                  {{ item.salaQuirofano || 'Sala Clínica' }}
                </span>
                @if (item.esAlquilado) {
                  <span class="text-[10px] font-bold text-amber-400 bg-amber-500/10 px-2 py-0.5 rounded border border-amber-500/20 flex items-center gap-1">
                    <lucide-icon name="badge-dollar-sign" class="w-3 h-3 text-amber-400"></lucide-icon>
                    Pabellón Alquilado
                  </span>
                }
              </div>

              <!-- Procedimiento -->
              <h4 class="text-xs font-bold text-white group-hover:text-sky-400 transition line-clamp-2 mb-1.5">
                {{ item.descripcionCirugia }}
              </h4>

              <!-- Paciente & Ubicación -->
              <div class="text-[11px] text-gray-300 font-medium mb-1">
                {{ item.pacienteNombre }} <span class="text-gray-500 font-normal">({{ item.pacienteCedula }})</span>
              </div>
              <div class="text-[10px] text-gray-400 flex items-center gap-1 mb-2">
                <lucide-icon name="bed" class="w-3 h-3 text-purple-400"></lucide-icon>
                <span>Origen: <strong>{{ item.ubicacion.descripcionCompleta }}</strong> ({{ item.ingresoCobertura.tipo }})</span>
              </div>
            </div>

            <!-- Barra Inferior: Médico y Checklist -->
            <div class="pt-2.5 border-t border-gray-800/80 mt-2 space-y-2">
              <div class="text-[11px] text-gray-300 flex items-center gap-1.5 truncate">
                <lucide-icon name="stethoscope" class="w-3.5 h-3.5 text-sky-400 shrink-0"></lucide-icon>
                <span class="truncate">{{ item.medicoNombre }}</span>
              </div>

              <div class="flex justify-between items-center text-[10px] pt-1">
                <span [class.text-emerald-400]="item.estaAptoParaQuirofano" [class.text-amber-400]="!item.estaAptoParaQuirofano" class="font-semibold flex items-center gap-1">
                  <lucide-icon [name]="item.estaAptoParaQuirofano ? 'check-circle-2' : 'alert-circle'" class="w-3 h-3"></lucide-icon>
                  Checklist: {{ item.requisitosCumplidos }}/{{ item.totalRequisitos }}
                </span>
                <span class="text-sky-400 font-bold flex items-center gap-0.5 group-hover:translate-x-0.5 transition">
                  Detalle <lucide-icon name="chevron-right" class="w-3 h-3"></lucide-icon>
                </span>
              </div>
            </div>
          </div>
        }
      </div>
    </div>
  `
})
export class PabellonCalendarioComponent {
  cirugias = input<CirugiaCalendarioItem[]>([]);
  habitaciones = input<AreaClinica[]>([]);
  seleccionarCirugia = output<CirugiaCalendarioItem>();

  salaFiltro = signal<string>('');

  // Unifica las habitaciones de la BD con las salas registradas en cirugías
  salasDisponibles = computed(() => {
    const listHab = (this.habitaciones() || []).map(h => h.nombre.trim()).filter(Boolean);
    const listCirugias = (this.cirugias() || []).map(c => (c.salaQuirofano || '').trim()).filter(Boolean);
    const combinadas = Array.from(new Set([...listHab, ...listCirugias]));
    return combinadas.sort();
  });

  cirugiasFiltradas = computed(() => {
    const list = this.cirugias();
    const filtro = this.salaFiltro();
    if (!filtro) return list;
    return list.filter(c => (c.salaQuirofano || '').toLowerCase() === filtro.toLowerCase());
  });

  getEstadoClass(estado: string): string {
    const e = (estado || '').toLowerCase();
    if (e === 'programada') return 'bg-amber-500/10 text-amber-400 border border-amber-500/20';
    if (e === 'enespera') return 'bg-purple-500/10 text-purple-400 border border-purple-500/20';
    if (e === 'encirugia') return 'bg-emerald-500/10 text-emerald-400 border border-emerald-500/20 animate-pulse';
    if (e === 'finalizado') return 'bg-sky-500/10 text-sky-400 border border-sky-500/20';
    if (e === 'cancelada') return 'bg-rose-500/10 text-rose-400 border border-rose-500/20';
    return 'bg-gray-800 text-gray-300';
  }
}
