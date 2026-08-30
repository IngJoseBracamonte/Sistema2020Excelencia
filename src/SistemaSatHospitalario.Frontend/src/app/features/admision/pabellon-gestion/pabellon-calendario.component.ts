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
      <!-- Barra Superior de Encabezado y Filtro de Quirófano -->
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
          <!-- ComboList con Buscador para Quirófano -->
          <div class="flex items-center gap-2 bg-gray-950/90 px-3 py-1.5 rounded-xl border border-gray-800 focus-within:border-sky-500/80 transition shadow-inner">
            <lucide-icon name="door-open" class="w-4 h-4 text-sky-400 shrink-0"></lucide-icon>
            <span class="text-[11px] font-bold text-gray-400 uppercase tracking-wider shrink-0">Quirófano:</span>
            <select
              [ngModel]="salaFiltro()"
              (ngModelChange)="salaFiltro.set($event)"
              class="bg-transparent text-xs font-semibold text-white focus:outline-none cursor-pointer pr-4"
            >
              <option value="" class="bg-gray-900 text-gray-200">Todos los Quirófanos ({{ cirugias().length }})</option>
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

      <!-- Barra de Filtro de Fechas y Navegación Temporal (Parte de abajo del encabezado de Calendario) -->
      <div class="bg-gray-900/60 border border-gray-800/80 rounded-2xl p-3.5 backdrop-blur-md shadow-xl flex flex-wrap items-center justify-between gap-3">
        
        <!-- Accesos Rápidos de Fecha -->
        <div class="flex items-center gap-1.5 bg-gray-950 p-1 rounded-xl border border-gray-800">
          <button
            (click)="setModoFecha('todas')"
            [class.bg-sky-600]="rangoModo() === 'todas'"
            [class.text-white]="rangoModo() === 'todas'"
            [class.text-gray-400]="rangoModo() !== 'todas'"
            class="px-3 py-1.5 rounded-lg text-xs font-semibold transition hover:text-white"
          >
            Todas
          </button>
          <button
            (click)="setModoFecha('hoy')"
            [class.bg-sky-600]="rangoModo() === 'hoy'"
            [class.text-white]="rangoModo() === 'hoy'"
            [class.text-gray-400]="rangoModo() !== 'hoy'"
            class="px-3 py-1.5 rounded-lg text-xs font-semibold transition hover:text-white"
          >
            Hoy
          </button>
          <button
            (click)="setModoFecha('manana')"
            [class.bg-sky-600]="rangoModo() === 'manana'"
            [class.text-white]="rangoModo() === 'manana'"
            [class.text-gray-400]="rangoModo() !== 'manana'"
            class="px-3 py-1.5 rounded-lg text-xs font-semibold transition hover:text-white"
          >
            Mañana
          </button>
          <button
            (click)="setModoFecha('semana')"
            [class.bg-sky-600]="rangoModo() === 'semana'"
            [class.text-white]="rangoModo() === 'semana'"
            [class.text-gray-400]="rangoModo() !== 'semana'"
            class="px-3 py-1.5 rounded-lg text-xs font-semibold transition hover:text-white"
          >
            Esta Semana
          </button>
        </div>

        <!-- Selector de Fecha & Controles de Navegación Día a Día -->
        <div class="flex items-center gap-2">
          <button
            (click)="cambiarDia(-1)"
            title="Día anterior"
            class="p-2 bg-gray-950 hover:bg-gray-800 text-gray-300 rounded-xl border border-gray-800 transition shadow-inner"
          >
            <lucide-icon name="chevron-left" class="w-4 h-4"></lucide-icon>
          </button>

          <div class="flex items-center gap-2 bg-gray-950 px-3 py-1.5 rounded-xl border border-gray-800 focus-within:border-sky-500/80 transition shadow-inner">
            <lucide-icon name="calendar" class="w-4 h-4 text-sky-400 shrink-0"></lucide-icon>
            <span class="text-[11px] font-bold text-gray-400 uppercase tracking-wider shrink-0">Fecha:</span>
            <input
              type="date"
              [ngModel]="fechaFiltro()"
              (ngModelChange)="onFechaInputChange($event)"
              class="bg-transparent text-xs font-semibold text-white focus:outline-none cursor-pointer [color-scheme:dark]"
            />
          </div>

          <button
            (click)="cambiarDia(1)"
            title="Día siguiente"
            class="p-2 bg-gray-950 hover:bg-gray-800 text-gray-300 rounded-xl border border-gray-800 transition shadow-inner"
          >
            <lucide-icon name="chevron-right" class="w-4 h-4"></lucide-icon>
          </button>

          @if (rangoModo() !== 'todas') {
            <button
              (click)="setModoFecha('todas')"
              title="Limpiar filtro de fecha"
              class="px-2.5 py-2 bg-rose-500/10 hover:bg-rose-500/20 text-rose-400 border border-rose-500/20 rounded-xl text-xs font-semibold transition flex items-center gap-1"
            >
              <lucide-icon name="x" class="w-3.5 h-3.5"></lucide-icon>
              Ver Todo
            </button>
          }
        </div>

      </div>

      <!-- Grid de Cirugías en Calendario -->
      <div *ngIf="cirugiasFiltradas().length === 0" class="text-center py-16 text-gray-400 text-xs bg-gray-900/40 rounded-2xl border border-gray-800/60 p-6">
        <lucide-icon name="calendar-x" class="w-10 h-10 mx-auto text-gray-600 mb-3"></lucide-icon>
        <p class="font-semibold text-gray-300">No hay cirugías programadas para este filtro</p>
        <p class="text-gray-500 mt-1">Seleccione otra fecha, quirófano o agregue una nueva intervención desde el tablero de gestión.</p>
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
                  {{ item.salaQuirofano || 'Quirófano' }}
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
  quirofanos = input<AreaClinica[]>([]);
  seleccionarCirugia = output<CirugiaCalendarioItem>();

  salaFiltro = signal<string>('');
  fechaFiltro = signal<string>('');
  rangoModo = signal<'todas' | 'hoy' | 'manana' | 'semana' | 'personalizado'>('todas');

  // Muestra estrictamente los quirófanos asignados al área de Cirugía
  salasDisponibles = computed(() => {
    const listQ = (this.quirofanos() || []).map(q => q.nombre.trim()).filter(Boolean);
    return Array.from(new Set(listQ)).sort((first, second) => first.localeCompare(second));
  });

  cirugiasFiltradas = computed(() => {
    let list = this.cirugias();
    const filtroSala = this.salaFiltro();
    const modo = this.rangoModo();
    const fecha = this.fechaFiltro();

    if (filtroSala) {
      list = list.filter(c => (c.salaQuirofano || '').toLowerCase() === filtroSala.toLowerCase());
    }

    if (modo === 'todas') {
      return list;
    }

    const hoyStr = new Date().toISOString().slice(0, 10);
    const mananaDate = new Date();
    mananaDate.setDate(mananaDate.getDate() + 1);
    const mananaStr = mananaDate.toISOString().slice(0, 10);

    if (modo === 'hoy') {
      return list.filter(c => {
        if (!c.fechaHoraProgramada) return false;
        const d = new Date(c.fechaHoraProgramada).toISOString().slice(0, 10);
        return d === hoyStr;
      });
    }

    if (modo === 'manana') {
      return list.filter(c => {
        if (!c.fechaHoraProgramada) return false;
        const d = new Date(c.fechaHoraProgramada).toISOString().slice(0, 10);
        return d === mananaStr;
      });
    }

    if (modo === 'semana') {
      const hoy = new Date();
      hoy.setHours(0, 0, 0, 0);
      const en7Dias = new Date();
      en7Dias.setDate(en7Dias.getDate() + 7);
      en7Dias.setHours(23, 59, 59, 999);

      return list.filter(c => {
        if (!c.fechaHoraProgramada) return false;
        const d = new Date(c.fechaHoraProgramada);
        return d >= hoy && d <= en7Dias;
      });
    }

    if (modo === 'personalizado' && fecha) {
      return list.filter(c => {
        if (!c.fechaHoraProgramada) return false;
        const d = new Date(c.fechaHoraProgramada).toISOString().slice(0, 10);
        return d === fecha;
      });
    }

    return list;
  });

  setModoFecha(modo: 'todas' | 'hoy' | 'manana' | 'semana') {
    this.rangoModo.set(modo);
    if (modo === 'todas') {
      this.fechaFiltro.set('');
    } else if (modo === 'hoy') {
      this.fechaFiltro.set(new Date().toISOString().slice(0, 10));
    } else if (modo === 'manana') {
      const d = new Date();
      d.setDate(d.getDate() + 1);
      this.fechaFiltro.set(d.toISOString().slice(0, 10));
    } else if (modo === 'semana') {
      this.fechaFiltro.set('');
    }
  }

  onFechaInputChange(fecha: string) {
    this.fechaFiltro.set(fecha);
    this.rangoModo.set(fecha ? 'personalizado' : 'todas');
  }

  cambiarDia(delta: number) {
    let base = this.fechaFiltro() ? new Date(this.fechaFiltro() + 'T00:00:00') : new Date();
    base.setDate(base.getDate() + delta);
    const nuevaFecha = base.toISOString().slice(0, 10);
    this.fechaFiltro.set(nuevaFecha);
    this.rangoModo.set('personalizado');
  }

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
