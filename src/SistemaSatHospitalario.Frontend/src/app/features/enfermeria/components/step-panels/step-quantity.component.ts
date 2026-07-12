import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AreaClinica } from '../../../../core/services/multi-sede.service';
import { LucideAngularModule, Plus, Minus } from 'lucide-angular';

@Component({
  selector: 'app-step-quantity',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule],
  template: `
    <div class="space-y-4">
      <div>
        <label class="text-[10px] uppercase font-black tracking-widest text-slate-400 block mb-1">Área Clínica (Ubicación de Carga)</label>
        <select 
          id="selectAreaClinicaFastCharge"
          [ngModel]="selectedAreaClinicaId"
          (ngModelChange)="areaSelected.emit($event)"
          class="w-full bg-slate-950/60 border border-slate-700 rounded-lg py-2 px-3 text-xs text-white focus:outline-none focus:border-indigo-500"
        >
          <option [value]="null">-- Seleccione Ubicación --</option>
          <option *ngFor="let a of areasClinicas" [value]="a.id">{{ a.nombre | uppercase }}</option>
        </select>
      </div>

      <!-- Cantidad Select Panel -->
      <div class="flex flex-col items-center py-4 bg-white/5 border border-white/5 rounded-lg">
        <label class="text-[10px] uppercase font-black tracking-widest text-slate-500 mb-2">Cantidad de Carga</label>
        
        <div class="flex items-center gap-4">
          <button 
            type="button"
            (click)="decrement()"
            class="w-10 h-10 rounded-lg bg-slate-800 border border-slate-700 text-slate-200 hover:bg-slate-700 hover:border-slate-600 active:scale-95 transition-all flex items-center justify-center font-bold text-lg"
          >
            <lucide-icon [name]="icons.Minus" class="w-4 h-4"></lucide-icon>
          </button>
          
          <input 
            type="number"
            id="fastChargeQuantityInput"
            [ngModel]="quantity"
            (ngModelChange)="quantityChange.emit($event)"
            [step]="permiteFraccionamiento ? '0.01' : '1'"
            min="0.01"
            class="w-24 text-center bg-slate-950 border border-slate-800 rounded-lg py-2 text-lg font-mono font-bold text-slate-200 focus:outline-none focus:border-indigo-500"
          />

          <button 
            type="button"
            (click)="increment()"
            class="w-10 h-10 rounded-lg bg-slate-800 border border-slate-700 text-slate-200 hover:bg-slate-700 hover:border-slate-600 active:scale-95 transition-all flex items-center justify-center font-bold text-lg"
          >
            <lucide-icon [name]="icons.Plus" class="w-4 h-4"></lucide-icon>
          </button>
        </div>
        <span *ngIf="unitLabel" class="text-[10px] font-bold text-slate-400 mt-2 uppercase tracking-wide">Unidad: {{ unitLabel }}</span>
      </div>

      <div class="p-3 bg-white/5 rounded-lg border border-white/5 flex items-center justify-between">
        <span class="text-xs font-bold text-slate-400">Total Estimado de Insumos:</span>
        <span class="text-base font-mono font-bold text-emerald-400">\${{ precioFinalCalculado | number:'1.2-2' }}</span>
      </div>
    </div>
  `
})
export class StepQuantityComponent {
  @Input() quantity = 1;
  @Input() unitLabel = 'UD';
  @Input() permiteFraccionamiento = false;
  @Input() areasClinicas: AreaClinica[] = [];
  @Input() selectedAreaClinicaId: string | null = null;
  @Input() precioFinalCalculado = 0;

  @Output() quantityChange = new EventEmitter<number>();
  @Output() areaSelected = new EventEmitter<string | null>();

  readonly icons = { Plus, Minus };

  increment() {
    const step = this.permiteFraccionamiento ? 0.1 : 1;
    this.quantityChange.emit(parseFloat((this.quantity + step).toFixed(2)));
  }

  decrement() {
    const step = this.permiteFraccionamiento ? 0.1 : 1;
    if (this.quantity > step) {
      this.quantityChange.emit(parseFloat((this.quantity - step).toFixed(2)));
    }
  }
}
