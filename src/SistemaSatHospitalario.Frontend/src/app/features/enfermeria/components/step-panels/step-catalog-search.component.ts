import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ServicioCatalogo } from '../../enfermeria.component';
import { LucideAngularModule, Search } from 'lucide-angular';

@Component({
  selector: 'app-step-catalog-search',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule],
  template: `
    <div class="space-y-4">
      <div class="relative">
        <lucide-icon [name]="icons.Search" class="absolute left-3 top-3 text-slate-500 w-4 h-4"></lucide-icon>
        <input 
          type="text"
          id="fastChargeSearchInput"
          [ngModel]="searchTerm"
          (ngModelChange)="searchChange.emit($event)"
          class="w-full bg-slate-950/60 border border-slate-700 rounded-lg py-2.5 pl-10 pr-4 text-xs text-white focus:outline-none focus:border-indigo-500 placeholder-slate-500"
          placeholder="Escriba código o nombre de insumo, medicamento o examen..."
          autocomplete="off"
        />
      </div>

      <!-- Autocomplete Dropdown list -->
      <div *ngIf="filteredServices.length > 0" class="bg-slate-900 border border-slate-800 rounded-lg max-h-[300px] overflow-y-auto divide-y divide-slate-800/60 shadow-xl">
        <div 
          *ngFor="let service of filteredServices"
          (click)="itemSelected.emit(service)"
          class="p-3 hover:bg-white/5 cursor-pointer flex items-center justify-between transition-colors duration-150"
        >
          <div>
            <span class="text-xs font-bold text-slate-200 block">{{ service.descripcion }}</span>
            <span class="text-[10px] text-slate-500">{{ service.codigo }} - {{ service.tipo || 'SERVICIO' }}</span>
          </div>
          <div class="text-right">
            <span class="text-xs font-mono font-bold text-indigo-400 block">\${{ service.precioUsd | number:'1.2-2' }}</span>
            <span *ngIf="(service.honorarioBase ?? 0) > 0" class="text-[9px] text-emerald-400">+\${{ service.honorarioBase }} Hon</span>
          </div>
        </div>
      </div>
    </div>
  `
})
export class StepCatalogSearchComponent {
  @Input() searchTerm = '';
  @Input() filteredServices: ServicioCatalogo[] = [];
  @Output() searchChange = new EventEmitter<string>();
  @Output() itemSelected = new EventEmitter<ServicioCatalogo>();

  readonly icons = { Search };
}
