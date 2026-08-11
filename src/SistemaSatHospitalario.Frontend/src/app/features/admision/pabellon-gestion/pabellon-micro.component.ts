import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LucideAngularModule } from 'lucide-angular';
import { OrdenCirugia } from '../../../core/services/pabellon.service';

@Component({
  selector: 'app-pabellon-micro',
  standalone: true,
  imports: [CommonModule, LucideAngularModule],
  template: `
    <div class="space-y-4">
      
      <div *ngIf="cirugias.length === 0" class="text-center py-12 text-gray-500 text-xs bg-gray-900/40 rounded-xl border border-gray-800/80">
        No hay cirugías registradas para el filtro seleccionado.
      </div>

      <!-- Tarjetas Operativas Micro -->
      <div *ngFor="let c of cirugias" class="bg-gray-900/70 border border-gray-800/80 rounded-2xl p-5 backdrop-blur-md shadow-xl transition hover:border-gray-700/80">
        
        <!-- Fila Superior: Datos de Paciente y Estado -->
        <div class="flex flex-col lg:flex-row lg:items-center justify-between gap-4 pb-4 border-b border-gray-800/80">
          
          <div class="flex items-start gap-3">
            <div class="p-3 bg-sky-500/10 border border-sky-500/20 rounded-xl text-sky-400 mt-0.5">
              <lucide-icon name="heart-pulse" class="w-6 h-6"></lucide-icon>
            </div>
            <div>
              <div class="flex items-center gap-2">
                <h3 class="text-base font-bold text-white">{{ c.pacienteNombre }}</h3>
                <span class="text-xs text-gray-400 font-mono">({{ c.pacienteCedula }})</span>
              </div>
              <p class="text-xs text-sky-400 font-semibold mt-0.5">{{ c.descripcionCirugia }}</p>
              <div class="flex items-center gap-4 text-[11px] text-gray-400 mt-1">
                <span>Dr. <strong>{{ c.medicoNombre }}</strong></span>
                <span>Programado: <strong class="text-white">{{ c.fechaHoraProgramada | date:'dd/MM/yyyy hh:mm a' }}</strong></span>
              </div>
            </div>
          </div>

          <!-- Acciones de Estado -->
          <div class="flex flex-wrap items-center gap-2">
            <!-- Badge Estado -->
            <span class="text-xs font-bold px-3 py-1.5 rounded-xl uppercase tracking-wider" [ngClass]="getEstadoClass(c.estado)">
              {{ c.estado }}
            </span>

            <!-- Transición a En Espera -->
            <button 
              *ngIf="c.estado === 'Programada' || c.estado === 'PendienteEjecucion'"
              (click)="cambiarEstado.emit({ cirugiaId: c.id, nuevoEstado: 'EnEspera' })"
              class="bg-purple-600/20 hover:bg-purple-600/30 text-purple-300 border border-purple-500/30 text-xs font-semibold px-3 py-1.5 rounded-xl transition flex items-center gap-1.5"
            >
              <lucide-icon name="clock" class="w-3.5 h-3.5"></lucide-icon>
              Ingresar a Espera
            </button>

            <!-- Transición a En Cirugía -->
            <button 
              *ngIf="c.estado === 'Programada' || c.estado === 'PendienteEjecucion' || c.estado === 'EnEspera'"
              (click)="cambiarEstado.emit({ cirugiaId: c.id, nuevoEstado: 'EnCirugia' })"
              class="bg-emerald-600 hover:bg-emerald-500 text-white text-xs font-semibold px-3 py-1.5 rounded-xl shadow-lg shadow-emerald-600/20 transition flex items-center gap-1.5"
            >
              <lucide-icon name="play" class="w-3.5 h-3.5"></lucide-icon>
              Iniciar Cirugía
            </button>

            <!-- Transición a Finalizado -->
            <button 
              *ngIf="c.estado === 'EnCirugia' || c.estado === 'EnProceso'"
              (click)="cambiarEstado.emit({ cirugiaId: c.id, nuevoEstado: 'Finalizado' })"
              class="bg-sky-600 hover:bg-sky-500 text-white text-xs font-semibold px-3 py-1.5 rounded-xl shadow-lg shadow-sky-600/20 transition flex items-center gap-1.5"
            >
              <lucide-icon name="check-circle-2" class="w-3.5 h-3.5"></lucide-icon>
              Finalizar Cirugía
            </button>

            <!-- Botón Reprogramar -->
            <button 
              *ngIf="c.estado !== 'Finalizado' && c.estado !== 'Completada' && c.estado !== 'Cancelada'"
              (click)="abrirReprogramar.emit(c)"
              class="bg-amber-500/10 hover:bg-amber-500/20 text-amber-400 border border-amber-500/20 text-xs font-semibold px-3 py-1.5 rounded-xl transition flex items-center gap-1.5"
            >
              <lucide-icon name="calendar-clock" class="w-3.5 h-3.5"></lucide-icon>
              Reprogramar
            </button>

            <!-- Botón Ver Historial -->
            <button 
              (click)="abrirHistorial.emit(c)"
              class="bg-gray-800 hover:bg-gray-700 text-gray-300 text-xs font-semibold px-3 py-1.5 rounded-xl transition flex items-center gap-1.5"
            >
              <lucide-icon name="file-text" class="w-3.5 h-3.5"></lucide-icon>
              Historial
            </button>
          </div>

        </div>

        <!-- Fila Inferior: Checklist Requisitos DB-Driven -->
        <div class="mt-4 pt-3">
          <div class="flex justify-between items-center mb-2">
            <span class="text-xs font-bold text-gray-300 uppercase tracking-wider flex items-center gap-1.5">
              <lucide-icon name="check-square" class="w-4 h-4 text-sky-400"></lucide-icon>
              Checklist de Requisitos Quirúrgicos (DB-Driven)
            </span>
            <span class="text-[11px] text-gray-400">
              Cumplidos: <strong class="text-sky-400">{{ getRequisitosCumplidos(c) }} / {{ c.requisitos?.length || 0 }}</strong>
            </span>
          </div>

          <div *ngIf="!c.requisitos || c.requisitos.length === 0" class="text-[11px] text-gray-500 italic">
            Sin requisitos quirúrgicos asignados.
          </div>

          <div *ngIf="c.requisitos && c.requisitos.length > 0" class="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 gap-2 mt-2">
            <div 
              *ngFor="let req of c.requisitos"
              (click)="toggleReq.emit({ ordenId: c.id, requisitoId: req.requisitoCirugiaId, cumplido: !req.cumplido })"
              class="p-2.5 rounded-xl border transition cursor-pointer flex items-center justify-between gap-2"
              [ngClass]="req.cumplido ? 'bg-emerald-500/10 border-emerald-500/30 text-emerald-300' : 'bg-gray-950/60 border-gray-800 text-gray-400 hover:border-gray-700'"
            >
              <div class="flex items-center gap-2 min-w-0">
                <input 
                  type="checkbox" 
                  [checked]="req.cumplido" 
                  class="rounded bg-gray-900 border-gray-700 text-sky-500 focus:ring-sky-500 h-4 w-4 pointer-events-none"
                />
                <span class="text-xs font-semibold truncate">{{ req.nombre }}</span>
              </div>

              <span *ngIf="req.cumplido" class="text-[10px] text-emerald-400 font-bold bg-emerald-500/20 px-1.5 py-0.5 rounded">OK</span>
              <span *ngIf="!req.cumplido" class="text-[10px] text-gray-500 font-medium">Pendiente</span>
            </div>
          </div>
        </div>

      </div>

    </div>
  `
})
export class PabellonMicroComponent {
  @Input() cirugias: OrdenCirugia[] = [];
  @Output() cambiarEstado = new EventEmitter<{ cirugiaId: string; nuevoEstado: string }>();
  @Output() abrirReprogramar = new EventEmitter<OrdenCirugia>();
  @Output() abrirHistorial = new EventEmitter<OrdenCirugia>();
  @Output() toggleReq = new EventEmitter<{ ordenId: string; requisitoId: string; cumplido: boolean }>();

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
