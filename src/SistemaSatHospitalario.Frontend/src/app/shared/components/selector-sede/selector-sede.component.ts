import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MultiSedeService, Sede } from '../../../core/services/multi-sede.service';

@Component({
  selector: 'app-selector-sede',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="flex items-center gap-2" *ngIf="sedes.length > 0">
      <label class="text-xs text-muted-foreground font-medium hidden md:inline">Sede Activa:</label>
      <select 
        [ngModel]="multiSedeService.activeSede()" 
        (ngModelChange)="onSedeChange($event)"
        class="bg-surface/80 border border-white/10 rounded-lg text-xs py-1.5 px-3 focus:outline-none focus:border-indigo-500 transition-colors cursor-pointer text-foreground"
      >
        <option *ngFor="let sede of sedes" [ngValue]="sede">
          {{ sede.nombre }} {{ sede.esPrincipal ? '(Principal)' : '' }}
        </option>
      </select>
    </div>
  `
})
export class SelectorSedeComponent implements OnInit {
  public multiSedeService = inject(MultiSedeService);
  public sedes: Sede[] = [];

  ngOnInit() {
    this.multiSedeService.getSedes().subscribe({
      next: (res) => {
        this.sedes = res.filter(s => s.activo);
        this.multiSedeService.loadInitialSede(this.sedes);
      },
      error: (err) => console.error('[SelectorSede] No se pudieron cargar las sedes', err)
    });
  }

  onSedeChange(sede: Sede) {
    this.multiSedeService.setSedeActiva(sede);
  }
}
