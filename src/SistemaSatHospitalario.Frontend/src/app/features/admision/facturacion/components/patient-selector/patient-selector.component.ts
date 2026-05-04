import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LucideAngularModule, Search, ChevronRight, User, Edit3 } from 'lucide-angular';
import { PatientRecord } from '../../../../../core/services/patient.service';

/**
 * PatientSelectorComponent (Pachón Pro V7.0 - SOLID SRP)
 * Encapsula la búsqueda e identificación de beneficiarios.
 * Proporciona una interfaz limpia para la selección de pacientes.
 */
@Component({
  selector: 'app-patient-selector',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule],
  template: `
    <div class="glass-panel rounded-[2rem] shadow-xl border-white/40 p-6 space-y-6 relative z-[90]">
        <div class="flex items-center justify-between">
            <label class="block text-[10px] font-black text-slate-500 uppercase tracking-[0.2em] ml-2">Identificar Beneficiario</label>
            <div class="flex items-center space-x-3">
                <span *ngIf="hasItems && !selectedPatient" 
                      class="text-[9px] bg-rose-500/10 text-rose-500 px-4 py-1.5 rounded-full font-black border border-rose-500/20 animate-pulse tracking-widest uppercase">
                      SINCRONIZACIÓN PENDIENTE
                </span>
                <span *ngIf="selectedPatient" 
                      class="text-[9px] bg-emerald-500 text-white px-4 py-1.5 rounded-full font-black shadow-lg shadow-emerald-500/20 animate-scale-in tracking-widest uppercase">
                      VERIFICADO
                </span>
            </div>
        </div>

        <!-- Estado: Búsqueda -->
        <div *ngIf="!selectedPatient" class="relative">
            <div class="flex space-x-4">
                <div class="relative flex-1">
                    <input type="text" [(ngModel)]="searchTerm" (keyup.enter)="search.emit(searchTerm)" [disabled]="disabled"
                        class="w-full px-6 py-5 bg-surface-card border border-glass-border rounded-[1.5rem] focus:ring-4 focus:ring-primary/10 focus:border-primary transition-all font-black text-main tracking-tight placeholder:text-slate-600 shadow-inner"
                        placeholder="Ingrese Cédula o Apellidos...">
                    <div *ngIf="searching" class="absolute right-6 top-1/2 -translate-y-1/2">
                        <svg class="w-6 h-6 animate-spin text-primary" fill="none" stroke="currentColor" viewBox="0 0 24 24"><circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle><path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4"></path></svg>
                    </div>
                </div>
                <button (click)="search.emit(searchTerm)" [disabled]="disabled || searching"
                    class="px-8 bg-black hover:bg-slate-900 text-white rounded-[1.5rem] font-black shadow-2xl transition-all flex items-center active:scale-95 disabled:bg-slate-800 uppercase text-[10px] tracking-widest">
                    <lucide-icon *ngIf="!searching" name="search" class="w-5 h-5 mr-3"></lucide-icon>
                    BUSCAR
                </button>
            </div>
            
            <!-- Resultados -->
            <div *ngIf="showResults && results.length > 0" 
                class="absolute z-[80] w-full mt-4 bg-surface/95 backdrop-blur-3xl rounded-[2rem] shadow-2xl border border-glass-border overflow-hidden animate-scale-in ring-1 ring-slate-100">
                <button *ngFor="let p of results" (click)="select.emit(p)"
                    class="w-full p-6 flex items-center justify-between hover:bg-white/5 transition-all border-b border-white/5 last:border-none group text-left">
                    <div class="flex flex-col flex-1">
                        <span class="text-sm font-black text-main group-hover:text-primary transition-colors">{{ p.nombre }} {{ p.apellidos }}</span>
                        <div class="flex items-center space-x-3 mt-1">
                            <span class="text-[9px] font-black text-slate-400 uppercase tracking-widest">{{ p.cedula }}</span>
                        </div>
                    </div>
                    <lucide-icon [name]="icons.ChevronRight" class="w-4 h-4"></lucide-icon>
                </button>
            </div>

            <!-- No Resultados -->
            <div *ngIf="showResults && results.length === 0 && !searching" 
                class="absolute z-[80] w-full mt-4 bg-surface/90 backdrop-blur-3xl rounded-[2.5rem] shadow-2xl border border-rose-500/20 p-10 animate-scale-in text-center group">
                 <h4 class="text-xl font-black text-white tracking-tight uppercase mb-2">Beneficiario no encontrado</h4>
                 <button (click)="register.emit(searchTerm)" 
                     class="w-full py-5 bg-rose-500 hover:bg-rose-600 text-white rounded-2xl font-black text-[10px] uppercase tracking-[0.2em] shadow-xl shadow-rose-500/20 transition-all active:scale-95 flex items-center justify-center">
                     REGISTRAR NUEVO PACIENTE
                 </button>
             </div>
         </div>

        <!-- Estado: Verificado -->
        <div *ngIf="selectedPatient" class="animate-scale-in">
            <div class="flex items-center justify-between p-6 bg-black/20 rounded-[2rem] border border-emerald-500/15 group hover:border-emerald-500/30 transition-all">
                <div class="flex items-center space-x-5">
                    <div class="h-14 w-14 bg-emerald-500/10 text-emerald-500 rounded-2xl flex items-center justify-center border border-emerald-500/20">
                        <lucide-icon name="user" class="w-7 h-7"></lucide-icon>
                    </div>
                    <div>
                        <h4 class="text-base font-black text-white uppercase">{{ selectedPatient.nombre }} {{ selectedPatient.apellidos }}</h4>
                        <p class="text-[9px] font-black text-slate-400">{{ selectedPatient.cedula }}</p>
                    </div>
                </div>
                <button (click)="change.emit()" [disabled]="disabled"
                    class="flex items-center space-x-2 px-5 py-3 bg-white/5 hover:bg-primary/10 text-slate-400 hover:text-primary border border-white/5 hover:border-primary/20 rounded-xl transition-all">
                    <lucide-icon name="edit-3" class="w-4 h-4"></lucide-icon>
                    <span class="text-[9px] font-black uppercase">Cambiar</span>
                </button>
            </div>
        </div>
    </div>
  `,
  styleUrls: []
})
export class PatientSelectorComponent {
  @Input() hasItems = false;
  @Input() selectedPatient: PatientRecord | null = null;
  @Input() searching = false;
  @Input() results: PatientRecord[] = [];
  @Input() showResults = false;
  @Input() disabled = false;

  @Output() search = new EventEmitter<string>();
  @Output() select = new EventEmitter<PatientRecord>();
  @Output() change = new EventEmitter<void>();
  @Output() register = new EventEmitter<string>();

  public searchTerm = '';
  readonly icons = { Search, ChevronRight, User, Edit3 };
}
