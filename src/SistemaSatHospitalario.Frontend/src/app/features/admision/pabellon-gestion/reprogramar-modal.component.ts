import { Component, EventEmitter, Input, OnInit, Output, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LucideAngularModule } from 'lucide-angular';
import { OrdenCirugia } from '../../../core/services/pabellon.service';

@Component({
  selector: 'app-reprogramar-modal',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule],
  template: `
    <div class="fixed inset-0 bg-black/80 backdrop-blur-sm z-[500] flex items-center justify-center p-4">
      <div class="bg-gray-900 border border-gray-800 rounded-2xl w-full max-w-lg overflow-hidden shadow-2xl animate-fade-in">
        
        <!-- Header -->
        <div class="bg-gray-950 p-5 border-b border-gray-800 flex justify-between items-center">
          <div class="flex items-center gap-3">
            <div class="p-2.5 bg-amber-500/10 border border-amber-500/20 rounded-xl text-amber-400">
              <lucide-icon name="calendar-clock" class="w-5 h-5"></lucide-icon>
            </div>
            <div>
              <h3 class="text-base font-bold text-white">Reprogramar Cirugía</h3>
              <p class="text-xs text-gray-400">{{ cirugia.pacienteNombre }} • {{ cirugia.descripcionCirugia }}</p>
            </div>
          </div>
          <button (click)="cerrar.emit()" class="text-gray-400 hover:text-white p-1 rounded-lg hover:bg-gray-800 transition">
            <lucide-icon name="x" class="w-5 h-5"></lucide-icon>
          </button>
        </div>

        <!-- Body -->
        <div class="p-6 space-y-5">
          <!-- Fecha y Hora Actual -->
          <div class="bg-gray-950/60 p-3.5 rounded-xl border border-gray-800 flex justify-between items-center text-xs">
            <span class="text-gray-400">Programación Actual:</span>
            <span class="font-bold text-amber-400">{{ cirugia.fechaHoraProgramada | date:'dd/MM/yyyy hh:mm a' }}</span>
          </div>

          <!-- Nueva Fecha y Hora -->
          <div>
            <label class="block text-xs font-semibold text-gray-300 mb-1.5">Nueva Fecha y Hora *</label>
            <input 
              type="datetime-local" 
              [ngModel]="nuevaFecha()" 
              (ngModelChange)="nuevaFecha.set($event)" 
              class="w-full bg-gray-950 border border-gray-800 rounded-xl px-4 py-2.5 text-xs text-white focus:border-amber-500 focus:outline-none transition"
            />
          </div>

          <!-- Motivo / Observaciones (Obligatorio) -->
          <div>
            <label class="block text-xs font-semibold text-gray-300 mb-1.5">Motivo / Observaciones de Reprogramación *</label>
            <textarea 
              rows="3"
              [ngModel]="motivo()" 
              (ngModelChange)="motivo.set($event)" 
              placeholder="Ingrese detalladamente la razón médica u operativa de la reprogramación..."
              class="w-full bg-gray-950 border border-gray-800 rounded-xl p-3 text-xs text-white placeholder-gray-500 focus:border-amber-500 focus:outline-none transition"
            ></textarea>
            <p class="text-[10px] text-amber-400/80 mt-1 flex items-center gap-1">
              <lucide-icon name="alert-circle" class="w-3 h-3"></lucide-icon>
              La captura de observaciones es obligatoria por normas de auditoría médica.
            </p>
          </div>
        </div>

        <!-- Footer -->
        <div class="p-4 bg-gray-950 border-t border-gray-800 flex justify-end gap-3">
          <button 
            (click)="cerrar.emit()" 
            class="px-4 py-2 rounded-xl text-xs font-semibold text-gray-400 hover:text-white hover:bg-gray-800 transition"
          >
            Cancelar
          </button>

          <button 
            (click)="confirmarReprogramacion()" 
            [disabled]="!nuevaFecha() || !motivo().trim() || enviando()"
            class="px-5 py-2 rounded-xl text-xs font-semibold bg-amber-600 hover:bg-amber-500 disabled:opacity-40 disabled:cursor-not-allowed text-white shadow-lg shadow-amber-600/20 transition flex items-center gap-2"
          >
            <lucide-icon name="check" class="w-4 h-4"></lucide-icon>
            Confirmar Reprogramación
          </button>
        </div>

      </div>
    </div>
  `
})
export class ReprogramarModalComponent implements OnInit {
  @Input({ required: true }) cirugia!: OrdenCirugia;
  @Output() cerrar = new EventEmitter<void>();
  @Output() confirmar = new EventEmitter<{ nuevaFechaHora: string; motivo: string }>();

  public nuevaFecha = signal<string>('');
  public motivo = signal<string>('');
  public enviando = signal<boolean>(false);

  ngOnInit(): void {
    if (this.cirugia?.fechaHoraProgramada) {
      // Formatear fecha para el input datetime-local
      const date = new Date(this.cirugia.fechaHoraProgramada);
      const iso = new Date(date.getTime() - (date.getTimezoneOffset() * 60000)).toISOString().slice(0, 16);
      this.nuevaFecha.set(iso);
    }
  }

  confirmarReprogramacion(): void {
    if (!this.nuevaFecha() || !this.motivo().trim()) return;
    this.enviando.set(true);
    this.confirmar.emit({
      nuevaFechaHora: this.nuevaFecha(),
      motivo: this.motivo().trim()
    });
  }
}
