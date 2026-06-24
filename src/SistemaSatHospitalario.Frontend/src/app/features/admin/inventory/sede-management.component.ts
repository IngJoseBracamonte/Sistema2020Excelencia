import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MultiSedeService, Sede, AreaClinica } from '../../../core/services/multi-sede.service';

@Component({
  selector: 'app-sede-management',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="space-y-6">
      <!-- Encabezado con estética Glassmorphic -->
      <div class="relative overflow-hidden rounded-2xl border border-white/10 bg-surface/50 backdrop-blur-xl p-8 shadow-2xl">
        <div class="relative z-10 flex flex-col md:flex-row md:items-center md:justify-between gap-4">
          <div>
            <h1 class="text-3xl font-black tracking-tight text-foreground">Gestión de Sedes y Áreas Clínicas</h1>
            <p class="text-sm text-muted-foreground mt-1">Configuración del mapeo de sucursales físicas y departamentos del hospital.</p>
          </div>
          <button 
            (click)="openCreateSedeModal()"
            class="h-11 px-6 rounded-xl bg-gradient-to-r from-blue-600 to-indigo-600 hover:from-blue-500 hover:to-indigo-500 font-semibold text-white transition-all shadow-lg hover:shadow-indigo-500/20 active:scale-[0.98] text-sm flex items-center justify-center gap-2"
          >
            <span>+ Nueva Sede</span>
          </button>
        </div>
      </div>

      <!-- Lista de Sedes con Grid Premium -->
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <div 
          *ngFor="let sede of sedes" 
          class="rounded-xl border border-white/5 bg-surface/30 p-6 flex flex-col justify-between gap-4 shadow-md hover:border-white/10 transition-all"
        >
          <div>
            <div class="flex items-center justify-between">
              <div class="flex items-center gap-2">
                <span class="text-xs font-black tracking-wider uppercase bg-white/5 text-white/60 px-2 py-1 rounded">
                  {{ sede.codigo }}
                </span>
                <span 
                  *ngIf="sede.esPrincipal"
                  class="text-[10px] font-black uppercase bg-indigo-500/20 text-indigo-400 border border-indigo-500/30 px-2 py-0.5 rounded-full"
                >
                  Principal
                </span>
              </div>
              <div class="flex gap-2">
                <button (click)="editSede(sede)" class="text-xs text-blue-400 hover:text-blue-300">Editar</button>
                <button (click)="deleteSede(sede.id)" class="text-xs text-red-400 hover:text-red-300">Desactivar</button>
              </div>
            </div>

            <h3 class="text-xl font-bold text-foreground mt-2">{{ sede.nombre }}</h3>

            <!-- Áreas Clínicas de esta Sede -->
            <div class="mt-4 border-t border-white/5 pt-4">
              <div class="flex justify-between items-center mb-2">
                <h4 class="text-xs font-bold uppercase tracking-wider text-muted-foreground">Áreas Clínicas</h4>
                <button (click)="openCreateAreaModal(sede.id)" class="text-[10px] font-bold text-indigo-400 hover:underline">+ Agregar Área</button>
              </div>
              <ul class="space-y-1" *ngIf="sede.areasClinicas && sede.areasClinicas.length > 0; else noAreas">
                <li 
                  *ngFor="let area of sede.areasClinicas"
                  class="text-xs flex items-center justify-between py-1 px-2 rounded bg-white/[0.02]"
                >
                  <span class="text-foreground/80 font-medium">[{{ area.codigo }}] {{ area.nombre }}</span>
                  <button (click)="deleteArea(area.id)" class="text-[10px] text-red-400/70 hover:text-red-400">Eliminar</button>
                </li>
              </ul>
              <ng-template #noAreas>
                <p class="text-[11px] text-muted-foreground italic">Sin áreas configuradas en esta sede.</p>
              </ng-template>
            </div>
          </div>
        </div>
      </div>

      <!-- Modales de Creación/Edición Simplificados -->
      <div *ngIf="showSedeModal" class="fixed inset-0 z-[1000] flex items-center justify-center bg-black/60 backdrop-blur-sm p-4">
        <div class="w-full max-w-md rounded-2xl border border-white/10 bg-surface p-6 shadow-2xl space-y-4">
          <h3 class="text-lg font-bold text-foreground">{{ isEditing ? 'Editar Sede' : 'Nueva Sede' }}</h3>
          <div class="space-y-3">
            <div>
              <label class="text-xs text-muted-foreground font-semibold">Código</label>
              <input [(ngModel)]="sedeForm.codigo" class="w-full mt-1 bg-white/5 border border-white/10 rounded-lg px-3 py-2 text-sm text-foreground focus:outline-none focus:border-indigo-500" />
            </div>
            <div>
              <label class="text-xs text-muted-foreground font-semibold">Nombre</label>
              <input [(ngModel)]="sedeForm.nombre" class="w-full mt-1 bg-white/5 border border-white/10 rounded-lg px-3 py-2 text-sm text-foreground focus:outline-none focus:border-indigo-500" />
            </div>
            <div class="flex items-center gap-2 pt-2">
              <input type="checkbox" id="esPrincipal" [(ngModel)]="sedeForm.esPrincipal" class="rounded border-white/10 bg-white/5 text-indigo-600 focus:ring-indigo-500" />
              <label for="esPrincipal" class="text-xs text-foreground font-medium">¿Establecer como Sede Principal?</label>
            </div>
          </div>
          <div class="flex justify-end gap-3 pt-2">
            <button (click)="closeSedeModal()" class="px-4 py-2 rounded-lg bg-white/5 hover:bg-white/10 text-xs text-foreground font-medium transition-colors">Cancelar</button>
            <button (click)="saveSede()" class="px-4 py-2 rounded-lg bg-indigo-600 hover:bg-indigo-500 text-xs text-white font-semibold transition-colors">Guardar</button>
          </div>
        </div>
      </div>

      <!-- Modal Crear Área Clínica -->
      <div *ngIf="showAreaModal" class="fixed inset-0 z-[1000] flex items-center justify-center bg-black/60 backdrop-blur-sm p-4">
        <div class="w-full max-w-md rounded-2xl border border-white/10 bg-surface p-6 shadow-2xl space-y-4">
          <h3 class="text-lg font-bold text-foreground">Agregar Área Clínica</h3>
          <div class="space-y-3">
            <div>
              <label class="text-xs text-muted-foreground font-semibold">Código</label>
              <input [(ngModel)]="areaForm.codigo" class="w-full mt-1 bg-white/5 border border-white/10 rounded-lg px-3 py-2 text-sm text-foreground focus:outline-none focus:border-indigo-500" />
            </div>
            <div>
              <label class="text-xs text-muted-foreground font-semibold">Nombre del Área</label>
              <input [(ngModel)]="areaForm.nombre" class="w-full mt-1 bg-white/5 border border-white/10 rounded-lg px-3 py-2 text-sm text-foreground focus:outline-none focus:border-indigo-500" />
            </div>
          </div>
          <div class="flex justify-end gap-3 pt-2">
            <button (click)="closeAreaModal()" class="px-4 py-2 rounded-lg bg-white/5 hover:bg-white/10 text-xs text-foreground font-medium transition-colors">Cancelar</button>
            <button (click)="saveAreaClinica()" class="px-4 py-2 rounded-lg bg-indigo-600 hover:bg-indigo-500 text-xs text-white font-semibold transition-colors">Agregar</button>
          </div>
        </div>
      </div>
    </div>
  `
})
export class SedeManagementComponent implements OnInit {
  private multiSedeService = inject(MultiSedeService);

  public sedes: Sede[] = [];

  // Form states
  public showSedeModal = false;
  public showAreaModal = false;
  public isEditing = false;
  
  public sedeForm = { id: '', codigo: '', nombre: '', esPrincipal: false };
  public areaForm = { sedeId: '', codigo: '', nombre: '' };

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    this.multiSedeService.getSedes().subscribe({
      next: (res) => this.sedes = res,
      error: (err) => console.error(err)
    });
  }

  // --- SEDE METHODS ---
  openCreateSedeModal() {
    this.isEditing = false;
    this.sedeForm = { id: '', codigo: '', nombre: '', esPrincipal: false };
    this.showSedeModal = true;
  }

  editSede(sede: Sede) {
    this.isEditing = true;
    this.sedeForm = { id: sede.id, codigo: sede.codigo, nombre: sede.nombre, esPrincipal: sede.esPrincipal };
    this.showSedeModal = true;
  }

  closeSedeModal() {
    this.showSedeModal = false;
  }

  saveSede() {
    const action = this.isEditing 
      ? this.multiSedeService.updateSede(this.sedeForm.id, this.sedeForm)
      : this.multiSedeService.createSede(this.sedeForm);

    action.subscribe({
      next: () => {
        this.loadData();
        this.closeSedeModal();
      },
      error: (err) => alert(err.error?.message || 'Error al procesar la sede')
    });
  }

  deleteSede(id: string) {
    if (confirm('¿Está seguro de desactivar esta sede? Se desactivarán las áreas clínicas asociadas.')) {
      this.multiSedeService.deleteSede(id).subscribe(() => this.loadData());
    }
  }

  // --- AREA METHODS ---
  openCreateAreaModal(sedeId: string) {
    this.areaForm = { sedeId, codigo: '', nombre: '' };
    this.showAreaModal = true;
  }

  closeAreaModal() {
    this.showAreaModal = false;
  }

  saveAreaClinica() {
    this.multiSedeService.createAreaClinica(this.areaForm).subscribe({
      next: () => {
        this.loadData();
        this.closeAreaModal();
      },
      error: (err) => alert(err.error?.message || 'Error al agregar el área')
    });
  }

  deleteArea(id: string) {
    if (confirm('¿Desea eliminar esta área clínica?')) {
      this.multiSedeService.deleteAreaClinica(id).subscribe(() => this.loadData());
    }
  }
}
