import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { MultiSedeService, Sede, AreaClinica } from '../../../core/services/multi-sede.service';
import { environment } from '../../../../environments/environment';

interface SedeConAreas extends Sede {
  todasLasAreas?: AreaClinica[];
}

@Component({
  selector: 'app-sede-management',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="p-6 space-y-6 animate-fade-in text-main">

      <!-- Encabezado -->
      <div class="flex flex-col md:flex-row md:items-center md:justify-between space-y-4 md:space-y-0">
        <div class="flex items-center space-x-3">
          <div class="w-12 h-12 bg-primary/10 border border-primary/20 rounded-2xl flex items-center justify-center shadow-lg shadow-primary/5">
            <svg class="w-6 h-6 text-primary" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4" />
            </svg>
          </div>
          <div>
            <h1 class="text-2xl font-black tracking-tight uppercase">Gestión de Sedes y Áreas Clínicas</h1>
            <p class="text-xs text-muted font-bold uppercase tracking-wider">Configuración del mapeo de sucursales físicas y departamentos del hospital</p>
          </div>
        </div>
        <div class="flex items-center gap-3">
          <!-- Toggle: Mostrar inactivas -->
          <label class="flex items-center gap-2 cursor-pointer bg-surface-card border border-glass-border px-4 py-2.5 rounded-xl hover:border-white/10 transition-all select-none">
            <input type="checkbox"
              id="chkMostrarInactivas"
              [(ngModel)]="mostrarInactivas"
              class="w-4 h-4 rounded border-glass-border bg-surface text-primary focus:ring-primary cursor-pointer" />
            <span class="text-[10px] font-black uppercase tracking-wider"
              [class.text-primary]="mostrarInactivas"
              [class.text-muted]="!mostrarInactivas">
              Ver Desactivadas
            </span>
          </label>
          <button
            (click)="openCreateSedeModal()"
            class="h-11 px-6 rounded-xl bg-primary hover:bg-primary/90 font-black text-white transition-all shadow-lg shadow-primary/25 active:scale-[0.98] text-xs uppercase tracking-widest flex items-center justify-center gap-2"
          >
            <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4" />
            </svg>
            Nueva Sede
          </button>
        </div>
      </div>

      <!-- Lista de Sedes -->
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <div
          *ngFor="let sede of sedes"
          class="glass-card border border-glass-border rounded-2xl p-6 flex flex-col justify-between gap-4 shadow-md hover:border-white/10 transition-all"
        >
          <div>
            <div class="flex items-center justify-between">
              <div class="flex items-center gap-2">
                <span class="text-[10px] font-black tracking-widest uppercase bg-surface-card border border-glass-border text-muted px-2 py-1 rounded-lg">
                  {{ sede.codigo }}
                </span>
                <span
                  *ngIf="sede.esPrincipal"
                  class="text-[10px] font-black uppercase bg-primary/10 text-primary border border-primary/20 px-2 py-0.5 rounded-full"
                >
                  Principal
                </span>
              </div>
              <div class="flex gap-3">
                <button (click)="editSede(sede)"
                  class="text-[10px] font-black uppercase tracking-wider text-muted hover:text-main transition-colors">
                  Editar
                </button>
                <button (click)="deleteSede(sede.id)"
                  class="text-[10px] font-black uppercase tracking-wider text-rose-400/70 hover:text-rose-400 transition-colors">
                  Desactivar
                </button>
              </div>
            </div>

            <h3 class="text-lg font-black text-main mt-3 uppercase tracking-tight">{{ sede.nombre }}</h3>

            <!-- Áreas Clínicas por Sede -->
            <div class="mt-4 border-t border-glass-border pt-4">
              <div class="flex justify-between items-center mb-3">
                <h4 class="text-[10px] font-black uppercase tracking-widest text-muted">
                  {{ sede.codigo === 'CIRUGIA' || sede.nombre.toLowerCase().includes('cirug') ? 'Quirófanos' : 'Áreas Clínicas' }}
                </h4>
                <button (click)="openCreateAreaModal(sede.id)"
                  class="text-[10px] font-black uppercase tracking-wider text-primary hover:text-primary/70 transition-colors">
                  {{ sede.codigo === 'CIRUGIA' || sede.nombre.toLowerCase().includes('cirug') ? '+ Anexar Quirófano' : '+ Agregar Área' }}
                </button>
              </div>

              <ng-container *ngIf="getAreasParaSede(sede.id) as areas">
                <!-- Áreas ACTIVAS -->
                <ul class="space-y-1" *ngIf="getAreasActivas(areas).length > 0">
                  <li
                    *ngFor="let area of getAreasActivas(areas)"
                    class="text-xs flex items-center justify-between py-1.5 px-3 rounded-lg bg-surface-card border border-glass-border hover:border-white/10 transition-all"
                  >
                    <span class="text-main/80 font-bold">[{{ area.codigo }}] {{ area.nombre }}</span>
                    <button (click)="deleteArea(area.id)"
                      class="text-[10px] font-black uppercase tracking-wider text-rose-400/60 hover:text-rose-400 transition-colors">
                      Eliminar
                    </button>
                  </li>
                </ul>

                <!-- Áreas INACTIVAS (solo si toggle activo) -->
                <ng-container *ngIf="mostrarInactivas && getAreasInactivas(areas).length > 0">
                  <div class="mt-2 pt-2 border-t border-dashed border-glass-border/50">
                    <p class="text-[9px] font-black uppercase tracking-widest text-muted/50 mb-1.5">Desactivadas</p>
                    <ul class="space-y-1">
                      <li
                        *ngFor="let area of getAreasInactivas(areas)"
                        class="text-xs flex items-center justify-between py-1.5 px-3 rounded-lg bg-surface border border-dashed border-glass-border/30 opacity-60 hover:opacity-90 transition-all"
                      >
                        <span class="text-muted font-bold line-through">[{{ area.codigo }}] {{ area.nombre }}</span>
                        <button (click)="activarArea(area.id)"
                          class="text-[10px] font-black uppercase tracking-wider text-emerald-400/70 hover:text-emerald-400 transition-colors no-underline">
                          Activar
                        </button>
                      </li>
                    </ul>
                  </div>
                </ng-container>

                <p *ngIf="getAreasActivas(areas).length === 0 && (!mostrarInactivas || getAreasInactivas(areas).length === 0)"
                  class="text-[11px] text-muted font-bold italic">
                  {{ sede.codigo === 'CIRUGIA' || sede.nombre.toLowerCase().includes('cirug') ? 'Sin quirófanos configurados.' : 'Sin áreas configuradas.' }}
                </p>
              </ng-container>
            </div>
          </div>
        </div>
      </div>

      <!-- Modal Crear/Editar Sede -->
      <div *ngIf="showSedeModal" class="fixed inset-0 z-[1000] flex items-center justify-center bg-black/70 backdrop-blur-sm p-4">
        <div class="w-full max-w-md rounded-2xl border border-primary/20 bg-surface-card shadow-2xl shadow-black/50 space-y-4 p-6 animate-scale-in">
          <div class="flex items-center gap-3 pb-2 border-b border-glass-border">
            <div class="w-9 h-9 bg-primary/10 border border-primary/20 rounded-xl flex items-center justify-center">
              <svg class="w-5 h-5 text-primary" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                  d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4" />
              </svg>
            </div>
            <h3 class="text-sm font-black text-main uppercase tracking-widest">{{ isEditing ? 'Editar Sede' : 'Nueva Sede' }}</h3>
          </div>
          <div class="space-y-3">
            <div>
              <label class="text-[10px] text-muted font-black uppercase tracking-wider">Código</label>
              <input [(ngModel)]="sedeForm.codigo"
                class="w-full mt-1 bg-surface border border-glass-border rounded-xl px-3 py-2 text-sm text-main focus:outline-none focus:border-primary/50 transition-all" />
            </div>
            <div>
              <label class="text-[10px] text-muted font-black uppercase tracking-wider">Nombre</label>
              <input [(ngModel)]="sedeForm.nombre"
                class="w-full mt-1 bg-surface border border-glass-border rounded-xl px-3 py-2 text-sm text-main focus:outline-none focus:border-primary/50 transition-all" />
            </div>
            <div class="flex items-center gap-3 pt-2 bg-surface-card/50 border border-glass-border rounded-xl px-3 py-2.5">
              <input type="checkbox" id="esPrincipal" [(ngModel)]="sedeForm.esPrincipal"
                class="rounded border-glass-border bg-surface text-primary focus:ring-primary w-4 h-4 cursor-pointer" />
              <label for="esPrincipal" class="text-xs text-main font-bold cursor-pointer">¿Establecer como Sede Principal?</label>
            </div>
          </div>
          <div class="flex justify-end gap-3 pt-2">
            <button (click)="closeSedeModal()"
              class="px-4 py-2 rounded-xl bg-surface border border-glass-border hover:bg-white/5 text-xs text-main font-black uppercase tracking-widest transition-all">
              Cancelar
            </button>
            <button (click)="saveSede()"
              class="px-4 py-2 rounded-xl bg-primary hover:bg-primary/90 text-xs text-white font-black uppercase tracking-widest transition-all shadow-lg shadow-primary/25 active:scale-[0.98]">
              Guardar
            </button>
          </div>
        </div>
      </div>

      <!-- Modal Crear Área Clínica -->
      <div *ngIf="showAreaModal" class="fixed inset-0 z-[1000] flex items-center justify-center bg-black/70 backdrop-blur-sm p-4">
        <div class="w-full max-w-md rounded-2xl border border-primary/20 bg-surface-card shadow-2xl shadow-black/50 space-y-4 p-6 animate-scale-in">
          <div class="flex items-center gap-3 pb-2 border-b border-glass-border">
            <div class="w-9 h-9 bg-primary/10 border border-primary/20 rounded-xl flex items-center justify-center">
              <svg class="w-5 h-5 text-primary" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4" />
              </svg>
            </div>
            <h3 class="text-sm font-black text-main uppercase tracking-widest">Agregar Área Clínica</h3>
          </div>
          <div class="space-y-3">
            <div>
              <label class="text-[10px] text-muted font-black uppercase tracking-wider">Código</label>
              <input [(ngModel)]="areaForm.codigo"
                class="w-full mt-1 bg-surface border border-glass-border rounded-xl px-3 py-2 text-sm text-main focus:outline-none focus:border-primary/50 transition-all" />
            </div>
            <div>
              <label class="text-[10px] text-muted font-black uppercase tracking-wider">Nombre del Área</label>
              <input [(ngModel)]="areaForm.nombre"
                class="w-full mt-1 bg-surface border border-glass-border rounded-xl px-3 py-2 text-sm text-main focus:outline-none focus:border-primary/50 transition-all" />
            </div>
          </div>
          <div class="flex justify-end gap-3 pt-2">
            <button (click)="closeAreaModal()"
              class="px-4 py-2 rounded-xl bg-surface border border-glass-border hover:bg-white/5 text-xs text-main font-black uppercase tracking-widest transition-all">
              Cancelar
            </button>
            <button (click)="saveAreaClinica()"
              class="px-4 py-2 rounded-xl bg-primary hover:bg-primary/90 text-xs text-white font-black uppercase tracking-widest transition-all shadow-lg shadow-primary/25 active:scale-[0.98]">
              Agregar
            </button>
          </div>
        </div>
      </div>
    </div>
  `
})
export class SedeManagementComponent implements OnInit {
  private multiSedeService = inject(MultiSedeService);
  private http = inject(HttpClient);

  public sedes: Sede[] = [];
  // Mapa sedeId -> todas las áreas (activas + inactivas)
  public areasMap: Map<string, AreaClinica[]> = new Map();

  public mostrarInactivas = false;

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
      next: (res) => {
        this.sedes = res;
        this.loadAllAreas();
      },
      error: (err) => console.error(err)
    });
  }

  loadAllAreas() {
    // Carga todas las áreas (activas + inactivas) para todas las sedes
    this.http.get<AreaClinica[]>(`${environment.apiUrl}/api/AreaClinica`, {
      params: { includeInactive: 'true' }
    }).subscribe({
      next: (areas) => {
        this.areasMap.clear();
        for (const area of areas) {
          const sedeId = area.sedeId ?? '';
          if (!this.areasMap.has(sedeId)) this.areasMap.set(sedeId, []);
          this.areasMap.get(sedeId)!.push(area);
        }
      },
      error: (err) => console.error('[AREAS] Error cargando áreas:', err)
    });
  }

  getAreasParaSede(sedeId: string): AreaClinica[] {
    return this.areasMap.get(sedeId) ?? [];
  }

  getAreasActivas(areas: AreaClinica[]): AreaClinica[] {
    return areas.filter(a => a.activo !== false);
  }

  getAreasInactivas(areas: AreaClinica[]): AreaClinica[] {
    return areas.filter(a => a.activo === false);
  }

  onToggleInactivas(value: boolean) {
    this.mostrarInactivas = value;
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
    if (confirm('¿Está seguro de desactivar esta sede?')) {
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
        this.multiSedeService.clearCache();
        this.loadData();
        this.closeAreaModal();
      },
      error: (err) => alert(err.error?.message || 'Error al agregar el área')
    });
  }

  deleteArea(id: string) {
    if (confirm('¿Desea desactivar esta área clínica?')) {
      this.multiSedeService.deleteAreaClinica(id).subscribe({
        next: () => {
          this.multiSedeService.clearCache();
          this.loadData();
        },
        error: (err) => alert(err.error?.message || 'Error al desactivar el área')
      });
    }
  }

  activarArea(id: string) {
    this.multiSedeService.activarAreaClinica(id).subscribe({
      next: () => {
        this.multiSedeService.clearCache();
        this.loadData();
      },
      error: (err) => alert(err.error?.message || 'Error al activar el área')
    });
  }
}
