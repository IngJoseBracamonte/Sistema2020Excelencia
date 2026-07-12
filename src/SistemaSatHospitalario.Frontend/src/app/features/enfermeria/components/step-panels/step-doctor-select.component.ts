import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Medico } from '../../../../core/services/medico.service';
import { AreaClinica } from '../../../../core/services/multi-sede.service';

@Component({
  selector: 'app-step-doctor-select',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="space-y-4">
      <div>
        <label class="text-[10px] uppercase font-black tracking-widest text-slate-400 block mb-1">Médico Tratante (Obligatorio)</label>
        <select 
          id="selectMedicoFastCharge"
          [ngModel]="selectedMedicoId"
          (ngModelChange)="medicoSelected.emit($event)"
          class="w-full bg-slate-950/60 border border-slate-700 rounded-lg py-2 px-3 text-xs text-white focus:outline-none focus:border-indigo-500"
        >
          <option [value]="null">-- Seleccione Médico --</option>
          <option *ngFor="let m of medicos" [value]="m.id">{{ m.nombre | uppercase }} - {{ m.especialidad | uppercase }}</option>
        </select>
      </div>

      <div>
        <label class="text-[10px] uppercase font-black tracking-widest text-slate-400 block mb-1">Área Clínica (Ubicación de Atención)</label>
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

      <div class="grid grid-cols-2 gap-4 pt-2">
        <div>
          <label class="text-[9px] uppercase font-black tracking-widest text-slate-500 block mb-0.5">Precio Base USD</label>
          <input 
            type="number"
            [ngModel]="customPrecio"
            (ngModelChange)="precioChange.emit($event)"
            class="w-full bg-slate-950/60 border border-slate-700 rounded-lg py-2 px-3 text-xs text-white focus:outline-none focus:border-indigo-500 font-mono"
            placeholder="0.00"
          />
        </div>
        <div>
          <label class="text-[9px] uppercase font-black tracking-widest text-slate-500 block mb-0.5">Honorario Médico USD</label>
          <input 
            type="number"
            [ngModel]="customHonorario"
            (ngModelChange)="honorarioChange.emit($event)"
            class="w-full bg-slate-950/60 border border-slate-700 rounded-lg py-2 px-3 text-xs text-white focus:outline-none focus:border-indigo-500 font-mono"
            placeholder="0.00"
          />
        </div>
      </div>

      <div class="mt-4 p-3 bg-white/5 rounded-lg border border-white/5 flex items-center justify-between">
        <span class="text-xs font-bold text-slate-400">Total Estimado de Consulta:</span>
        <span class="text-base font-mono font-bold text-emerald-400">\${{ precioFinalCalculado | number:'1.2-2' }}</span>
      </div>
    </div>
  `
})
export class StepDoctorSelectComponent {
  @Input() medicos: Medico[] = [];
  @Input() selectedMedicoId: string | null = null;
  @Input() areasClinicas: AreaClinica[] = [];
  @Input() selectedAreaClinicaId: string | null = null;
  @Input() customPrecio: number | null = null;
  @Input() customHonorario: number | null = null;
  @Input() precioFinalCalculado = 0;

  @Output() medicoSelected = new EventEmitter<string | null>();
  @Output() areaSelected = new EventEmitter<string | null>();
  @Output() precioChange = new EventEmitter<number>();
  @Output() honorarioChange = new EventEmitter<number>();
}
