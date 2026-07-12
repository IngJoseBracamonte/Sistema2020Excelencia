import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ServicioCatalogo } from '../../enfermeria.component';
import { LucideAngularModule, Sparkles, CheckCircle } from 'lucide-angular';

@Component({
  selector: 'app-step-confirm',
  standalone: true,
  imports: [CommonModule, LucideAngularModule],
  template: `
    <div class="space-y-4">
      <div class="p-4 bg-white/5 border border-white/5 rounded-lg space-y-2">
        <h4 class="text-xs font-black uppercase tracking-widest text-slate-400">Resumen de Carga</h4>
        
        <div class="grid grid-cols-2 gap-y-2 text-xs">
          <span class="text-slate-500">Paciente:</span>
          <span class="font-bold text-slate-200 truncate">{{ patientName }}</span>

          <span class="text-slate-500">Cédula:</span>
          <span class="font-mono text-slate-300">{{ patientCedula }}</span>

          <span class="text-slate-500">Item Principal:</span>
          <span class="font-bold text-slate-200 truncate">{{ itemDescription }}</span>

          <ng-container *ngIf="classification !== 'RX' && classification !== 'Consulta' && classification !== 'Laboratorio'">
            <span class="text-slate-500">Cantidad:</span>
            <span class="font-bold text-indigo-400">{{ quantity }} {{ unitLabel }}</span>
          </ng-container>

          <span *ngIf="medicoNombre" class="text-slate-500">Médico Tratante:</span>
          <span *ngIf="medicoNombre" class="font-bold text-slate-200 truncate">{{ medicoNombre }}</span>

          <span *ngIf="areaClinicaNombre" class="text-slate-500">Área Destino:</span>
          <span *ngIf="areaClinicaNombre" class="font-bold text-slate-200 truncate">{{ areaClinicaNombre }}</span>

          <span class="text-slate-500">Total Unitario:</span>
          <span class="font-mono font-bold text-emerald-400">\${{ precioFinalCalculado | number:'1.2-2' }}</span>
        </div>
      </div>

      <!-- Sugerencias Dinámicas desde la Base de Datos -->
      <div *ngIf="activeSuggestions.length > 0" class="p-3 bg-indigo-950/20 border border-indigo-500/20 rounded-lg space-y-3">
        <div class="flex items-center gap-1.5 text-xs font-black uppercase tracking-widest text-indigo-400">
          <lucide-icon [name]="icons.Sparkles" class="w-4 h-4"></lucide-icon>
          <span>Sugerencias de Facturación Recomendadas</span>
        </div>
        
        <div class="space-y-2">
          <div 
            *ngFor="let sug of activeSuggestions"
            (click)="onToggle(sug.id)"
            class="flex items-center justify-between p-2.5 bg-slate-900/60 border border-slate-800 rounded-lg cursor-pointer hover:border-slate-700 transition-all select-none"
          >
            <div class="flex items-center gap-2">
              <input 
                type="checkbox"
                [checked]="selectedSuggestions[sug.id]"
                (click)="$event.stopPropagation(); onToggle(sug.id)"
                class="rounded border-slate-700 text-indigo-500 focus:ring-indigo-500 bg-slate-950 w-4 h-4"
              />
              <div>
                <span class="text-xs font-bold text-slate-200 block leading-tight">{{ sug.descripcion }}</span>
                <span class="text-[9px] text-slate-500 uppercase tracking-wider">{{ sug.codigo }}</span>
              </div>
            </div>
            <span class="text-xs font-mono font-bold text-indigo-400">\${{ sug.precioUsd | number:'1.2-2' }}</span>
          </div>
        </div>
        <p class="text-[9px] text-indigo-400/80 leading-normal">
          * Estos servicios se configuran en el Maestro de Servicios y se agregarán de forma automática al confirmar el estudio principal si se mantienen seleccionados.
        </p>
      </div>
    </div>
  `
})
export class StepConfirmComponent {
  @Input() patientName = '';
  @Input() patientCedula = '';
  @Input() itemDescription = '';
  @Input() quantity = 1;
  @Input() unitLabel = 'UD';
  @Input() medicoNombre: string | null = null;
  @Input() areaClinicaNombre: string | null = null;
  @Input() precioFinalCalculado = 0;
  @Input() classification: string | null = null;

  // Sugerencias
  @Input() activeSuggestions: ServicioCatalogo[] = [];
  @Input() selectedSuggestions: Record<string, boolean> = {};

  @Output() toggleSuggestion = new EventEmitter<string>();

  readonly icons = { Sparkles, CheckCircle };

  onToggle(id: string | number) {
    this.toggleSuggestion.emit(String(id));
  }
}
