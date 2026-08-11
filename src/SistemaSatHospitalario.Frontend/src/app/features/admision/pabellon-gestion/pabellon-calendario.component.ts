import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LucideAngularModule } from 'lucide-angular';
import { OrdenCirugia } from '../../../core/services/pabellon.service';

@Component({
  selector: 'app-pabellon-calendario',
  standalone: true,
  imports: [CommonModule, LucideAngularModule],
  template: `
    <div class="space-y-4">
      
      <!-- Pizarra / Grid Horario de Pabellón -->
      <div class="bg-gray-900/60 border border-gray-800 rounded-2xl p-5 backdrop-blur-md shadow-xl">
        <div class="flex justify-between items-center mb-4">
          <div class="flex items-center gap-2">
            <lucide-icon name="grid" class="w-5 h-5 text-sky-400"></lucide-icon>
            <h2 class="text-sm font-bold text-white uppercase tracking-wider">Programación Quirúrgica - Pizarra de Pabellón</h2>
          </div>
          <span class="text-xs text-gray-400">{{ cirugias.length }} cirugías agendadas</span>
        </div>

        <div *ngIf="cirugias.length === 0" class="text-center py-12 text-gray-500 text-xs bg-gray-950/40 rounded-xl border border-gray-800/50">
          No hay cirugías programadas para el criterio de búsqueda seleccionado.
        </div>

        <!-- Grid de Bloques Horarios por Quirófano -->
        <div *ngIf="cirugias.length > 0" class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          <div 
            *ngFor="let c of cirugias"
            (click)="seleccionarCirugia.emit(c)"
            class="bg-gray-950 p-4 rounded-xl border border-gray-800/80 hover:border-sky-500/50 transition cursor-pointer group shadow-md relative overflow-hidden"
          >
            <!-- Badge de Estado con Color Representativo -->
            <div class="flex justify-between items-start mb-3">
              <span class="text-[10px] font-extrabold px-2.5 py-1 rounded-full uppercase tracking-wider" [ngClass]="getEstadoClass(c.estado)">
                {{ c.estado }}
              </span>
              <span class="text-xs font-mono font-bold text-sky-400 bg-sky-500/10 px-2 py-0.5 rounded border border-sky-500/20">
                {{ c.fechaHoraProgramada | date:'hh:mm a' }}
              </span>
            </div>

            <h4 class="text-xs font-bold text-white group-hover:text-sky-400 transition line-clamp-1 mb-1">
              {{ c.descripcionCirugia }}
            </h4>

            <div class="text-[11px] text-gray-300 font-semibold mb-2">
              {{ c.pacienteNombre }} <span class="text-gray-500 font-normal">({{ c.pacienteCedula }})</span>
            </div>

            <div class="text-[11px] text-gray-400 flex items-center gap-1.5 pt-2 border-t border-gray-900">
              <lucide-icon name="user-check" class="w-3.5 h-3.5 text-gray-500"></lucide-icon>
              <span class="truncate">{{ c.medicoNombre }}</span>
            </div>

            <!-- Footer con Conteo de Requisitos -->
            <div class="mt-3 flex justify-between items-center text-[10px] text-gray-400 pt-2 border-t border-gray-900">
              <span>Requisitos: <strong>{{ getRequisitosCumplidos(c) }} / {{ c.requisitos?.length || 0 }}</strong></span>
              <span class="text-sky-400 font-semibold flex items-center gap-1 group-hover:underline">
                Detalle <lucide-icon name="arrow-right" class="w-3 h-3"></lucide-icon>
              </span>
            </div>

          </div>
        </div>

      </div>

    </div>
  `
})
export class PabellonCalendarioComponent {
  @Input() cirugias: OrdenCirugia[] = [];
  @Output() seleccionarCirugia = new EventEmitter<OrdenCirugia>();

  getEstadoClass(estado: string): string {
    const e = (estado || '').toLowerCase();
    if (e === 'programada' || e === 'pendienteejecucion') return 'bg-amber-500/10 text-amber-400 border border-amber-500/20';
    if (e === 'enespera') return 'bg-purple-500/10 text-purple-400 border border-purple-500/20';
    if (e === 'encirugia' || e === 'enproceso') return 'bg-emerald-500/10 text-emerald-400 border border-emerald-500/20 animate-pulse';
    if (e === 'finalizado' || e === 'completada') return 'bg-sky-500/10 text-sky-400 border border-sky-500/20';
    if (e === 'cancelada') return 'bg-rose-500/10 text-rose-400 border border-rose-500/20';
    return 'bg-gray-800 text-gray-300';
  }

  getRequisitosCumplidos(c: OrdenCirugia): number {
    return c.requisitos?.filter(r => r.cumplido).length || 0;
  }
}
