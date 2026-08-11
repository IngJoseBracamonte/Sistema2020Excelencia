import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LucideAngularModule } from 'lucide-angular';
import { CirugiaObservacionHistorial } from '../../../core/services/pabellon.service';

@Component({
  selector: 'app-historial-observaciones-modal',
  standalone: true,
  imports: [CommonModule, LucideAngularModule],
  template: `
    <div class="fixed inset-0 bg-black/80 backdrop-blur-sm z-50 flex items-center justify-center p-4">
      <div class="bg-gray-900 border border-gray-800 rounded-2xl w-full max-w-xl overflow-hidden shadow-2xl animate-fade-in">
        
        <!-- Header -->
        <div class="bg-gray-950 p-5 border-b border-gray-800 flex justify-between items-center">
          <div class="flex items-center gap-3">
            <div class="p-2.5 bg-sky-500/10 border border-sky-500/20 rounded-xl text-sky-400">
              <lucide-icon name="file-text" class="w-5 h-5"></lucide-icon>
            </div>
            <div>
              <h3 class="text-base font-bold text-white">Historial de Observaciones Quirúrgicas</h3>
              <p class="text-xs text-gray-400">{{ pacienteNombre }} • Trazabilidad inmutable</p>
            </div>
          </div>
          <button (click)="cerrar.emit()" class="text-gray-400 hover:text-white p-1 rounded-lg hover:bg-gray-800 transition">
            <lucide-icon name="x" class="w-5 h-5"></lucide-icon>
          </button>
        </div>

        <!-- Body / Timeline -->
        <div class="p-6 max-h-[60vh] overflow-y-auto space-y-4">
          <div *ngIf="!historial || historial.length === 0" class="text-center py-8 text-gray-500 text-xs">
            No existen observaciones ni reprogramaciones registradas para esta cirugía.
          </div>

          <div *ngFor="let item of historial" class="relative pl-6 border-l border-gray-800 pb-4 last:pb-0">
            <div class="absolute -left-1.5 top-0 w-3 h-3 rounded-full bg-sky-500 ring-4 ring-gray-900"></div>
            
            <div class="bg-gray-950/70 p-3.5 rounded-xl border border-gray-800/80 space-y-1.5">
              <div class="flex justify-between items-center text-[11px]">
                <span class="font-bold text-sky-400 uppercase tracking-wider">{{ item.tipo }}</span>
                <span class="text-gray-400">{{ item.fechaRegistro | date:'dd/MM/yyyy hh:mm a' }}</span>
              </div>
              <p class="text-xs text-gray-200 leading-relaxed">{{ item.observacion }}</p>
              <div class="text-[10px] text-gray-500 flex items-center gap-1 pt-1">
                <lucide-icon name="user" class="w-3 h-3"></lucide-icon>
                <span>Registrado por: <strong>{{ item.usuarioRegistro }}</strong></span>
              </div>
            </div>
          </div>
        </div>

        <!-- Footer -->
        <div class="p-4 bg-gray-950 border-t border-gray-800 flex justify-end">
          <button 
            (click)="cerrar.emit()" 
            class="px-4 py-2 rounded-xl text-xs font-semibold bg-gray-800 hover:bg-gray-700 text-white transition"
          >
            Cerrar
          </button>
        </div>

      </div>
    </div>
  `
})
export class HistorialObservacionesModalComponent {
  @Input() pacienteNombre: string = '';
  @Input() historial: CirugiaObservacionHistorial[] = [];
  @Output() cerrar = new EventEmitter<void>();
}
