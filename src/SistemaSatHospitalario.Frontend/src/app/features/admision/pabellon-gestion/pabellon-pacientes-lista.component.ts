import { Component, ChangeDetectionStrategy, input, output, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LucideAngularModule } from 'lucide-angular';
import { PacienteQuirurgicoItem } from '../../../core/services/pabellon.service';

@Component({
  selector: 'app-pabellon-pacientes-lista',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="space-y-4">
      <!-- Barra de Herramientas y Filtros -->
      <div class="bg-gray-900/60 border border-gray-800/80 rounded-2xl p-4 backdrop-blur-md shadow-xl flex flex-wrap items-center justify-between gap-4">
        <div class="flex items-center gap-3 flex-1 min-w-[280px]">
          <div class="relative w-full max-w-md">
            <lucide-icon name="search" class="w-4 h-4 text-gray-400 absolute left-3 top-1/2 -translate-y-1/2"></lucide-icon>
            <input
              type="text"
              [(ngModel)]="busquedaTexto"
              (input)="onBusquedaChange()"
              placeholder="Buscar por paciente, cédula o procedimiento..."
              class="w-full bg-gray-950/80 border border-gray-800 rounded-xl pl-9 pr-4 py-2 text-xs text-white placeholder-gray-500 focus:border-sky-500 focus:ring-1 focus:ring-sky-500 transition"
            />
          </div>

          <!-- Filtro por Estado -->
          <div class="flex items-center bg-gray-950/80 p-1 rounded-xl border border-gray-800">
            <button
              (click)="setEstadoFiltro('')"
              [class.bg-sky-600]="estadoFiltro() === ''"
              [class.text-white]="estadoFiltro() === ''"
              [class.text-gray-400]="estadoFiltro() !== ''"
              class="px-2.5 py-1.5 rounded-lg text-xs font-semibold transition"
            >
              Todos
            </button>
            <button
              (click)="setEstadoFiltro('Programada')"
              [class.bg-sky-600]="estadoFiltro() === 'Programada'"
              [class.text-white]="estadoFiltro() === 'Programada'"
              [class.text-gray-400]="estadoFiltro() !== 'Programada'"
              class="px-2.5 py-1.5 rounded-lg text-xs font-semibold transition"
            >
              Programada
            </button>
            <button
              (click)="setEstadoFiltro('EnEspera')"
              [class.bg-sky-600]="estadoFiltro() === 'EnEspera'"
              [class.text-white]="estadoFiltro() === 'EnEspera'"
              [class.text-gray-400]="estadoFiltro() !== 'EnEspera'"
              class="px-2.5 py-1.5 rounded-lg text-xs font-semibold transition"
            >
              En Espera
            </button>
            <button
              (click)="setEstadoFiltro('EnCirugia')"
              [class.bg-sky-600]="estadoFiltro() === 'EnCirugia'"
              [class.text-white]="estadoFiltro() === 'EnCirugia'"
              [class.text-gray-400]="estadoFiltro() !== 'EnCirugia'"
              class="px-2.5 py-1.5 rounded-lg text-xs font-semibold transition"
            >
              En Quirófano
            </button>
            <button
              (click)="setEstadoFiltro('Finalizado')"
              [class.bg-sky-600]="estadoFiltro() === 'Finalizado'"
              [class.text-white]="estadoFiltro() === 'Finalizado'"
              [class.text-gray-400]="estadoFiltro() !== 'Finalizado'"
              class="px-2.5 py-1.5 rounded-lg text-xs font-semibold transition"
            >
              Finalizado
            </button>
          </div>
        </div>

        <!-- Botón Nueva Cirugía -->
        <button
          (click)="nuevaCirugia.emit()"
          class="px-4 py-2 bg-sky-600 hover:bg-sky-500 text-white rounded-xl text-xs font-bold transition flex items-center gap-1.5 shadow-lg shadow-sky-900/30"
        >
          <lucide-icon name="plus" class="w-4 h-4"></lucide-icon>
          Programar Cirugía
        </button>
      </div>

      <!-- Tabla Maestra de Pacientes Quirúrgicos -->
      <div class="bg-gray-900/60 border border-gray-800/80 rounded-2xl overflow-hidden backdrop-blur-md shadow-xl">
        <div class="overflow-x-auto">
          <table class="w-full text-left text-xs text-gray-300">
            <thead class="bg-gray-950/80 text-[10px] text-gray-400 uppercase tracking-wider border-b border-gray-800">
              <tr>
                <th class="p-3.5">Paciente & Cédula</th>
                <th class="p-3.5">Procedimiento / Cirugía</th>
                <th class="p-3.5">Origen & Cama</th>
                <th class="p-3.5">Sala / Pabellón</th>
                <th class="p-3.5">Médico Principal</th>
                <th class="p-3.5">Programación</th>
                <th class="p-3.5">Checklist</th>
                <th class="p-3.5">Estado</th>
                <th class="p-3.5 text-right">Acción</th>
              </tr>
            </thead>
            <tbody class="divide-y divide-gray-800/60">
              @if (pacientes().length === 0) {
                <tr>
                  <td colspan="9" class="text-center py-12 text-gray-500 text-xs">
                    No se encontraron pacientes en espera o en curso quirúrgico.
                  </td>
                </tr>
              }

              @for (p of pacientes(); track p.id) {
                <tr
                  (click)="seleccionarPaciente.emit(p)"
                  class="hover:bg-gray-850/50 transition cursor-pointer group"
                >
                  <td class="p-3.5">
                    <div class="font-bold text-white group-hover:text-sky-400 transition">{{ p.pacienteNombre }}</div>
                    <div class="text-[10px] text-gray-500 font-mono">{{ p.pacienteCedula }}</div>
                  </td>

                  <td class="p-3.5">
                    <div class="font-medium text-gray-200 line-clamp-1">{{ p.descripcionCirugia }}</div>
                    @if (p.esAlquilado) {
                      <span class="text-[9px] font-bold text-amber-400 bg-amber-500/10 px-1.5 py-0.5 rounded border border-amber-500/20 inline-block mt-0.5">
                        Alquilado
                      </span>
                    }
                  </td>

                  <td class="p-3.5">
                    <span class="text-xs text-purple-300 font-semibold bg-purple-500/10 px-2 py-0.5 rounded border border-purple-500/20">
                      {{ p.ubicacion.descripcionCompleta }}
                    </span>
                    <span class="text-[10px] text-gray-500 block mt-0.5">{{ p.ingresoCobertura.tipo }}</span>
                  </td>

                  <td class="p-3.5">
                    <span class="text-xs font-mono font-bold text-gray-300">
                      {{ p.salaQuirofano || 'Quirófano 1' }}
                    </span>
                    <span class="text-[10px] text-gray-500 block mt-0.5">{{ p.modalidadAnestesia || 'General' }}</span>
                  </td>

                  <td class="p-3.5">
                    <div class="font-medium text-gray-300 truncate max-w-[160px]">{{ p.medicoPrincipalNombre }}</div>
                  </td>

                  <td class="p-3.5">
                    <span class="text-xs font-mono font-bold text-sky-400">
                      {{ p.fechaHoraProgramada | date:'dd/MM hh:mm a' }}
                    </span>
                  </td>

                  <td class="p-3.5">
                    @let cumplidos = getRequisitosCumplidos(p);
                    @let total = p.requisitos.length;
                    @let apto = total === 0 || cumplidos === total;
                    <span
                      class="text-[10px] font-bold px-2 py-0.5 rounded-full border inline-flex items-center gap-1"
                      [ngClass]="apto ? 'bg-emerald-500/10 text-emerald-400 border-emerald-500/20' : 'bg-amber-500/10 text-amber-400 border-amber-500/20'"
                    >
                      <lucide-icon [name]="apto ? 'check' : 'clock'" class="w-3 h-3"></lucide-icon>
                      {{ cumplidos }}/{{ total }}
                    </span>
                  </td>

                  <td class="p-3.5">
                    <span class="text-[10px] font-extrabold px-2.5 py-1 rounded-full uppercase" [ngClass]="getEstadoClass(p.estado)">
                      {{ p.estado }}
                    </span>
                  </td>

                  <td class="p-3.5 text-right">
                    <button
                      (click)="seleccionarPaciente.emit(p); $event.stopPropagation()"
                      class="px-2.5 py-1 bg-sky-500/10 hover:bg-sky-500/20 text-sky-400 border border-sky-500/20 rounded-lg text-xs font-semibold transition inline-flex items-center gap-1"
                    >
                      Gestionar <lucide-icon name="chevron-right" class="w-3 h-3"></lucide-icon>
                    </button>
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
export class PabellonPacientesListaComponent {
  pacientes = input<PacienteQuirurgicoItem[]>([]);
  seleccionarPaciente = output<PacienteQuirurgicoItem>();
  nuevaCirugia = output<void>();
  filtrar = output<{ busqueda: string; estado: string }>();

  busquedaTexto = '';
  estadoFiltro = signal<string>('');

  onBusquedaChange() {
    this.filtrar.emit({
      busqueda: this.busquedaTexto,
      estado: this.estadoFiltro()
    });
  }

  setEstadoFiltro(estado: string) {
    this.estadoFiltro.set(estado);
    this.filtrar.emit({
      busqueda: this.busquedaTexto,
      estado: estado
    });
  }

  getRequisitosCumplidos(p: PacienteQuirurgicoItem): number {
    return p.requisitos?.filter(r => r.cumplido).length || 0;
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
