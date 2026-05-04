import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { LucideAngularModule } from 'lucide-angular';
import { environment } from '../../../../environments/environment';
import { HONORARIO_CATEGORIAS } from '../../../core/constants/honorario.constants';

@Component({
  selector: 'app-honorario-asignaciones',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule],
  template: `
    <div class="p-6 space-y-6">
      <!-- Header -->
      <div class="flex items-center justify-between">
        <div>
          <h1 class="text-2xl font-black text-white tracking-tight uppercase">Panel de Asignaciones</h1>
          <p class="text-xs text-muted font-bold uppercase tracking-widest">Asignación de médicos a servicios facturados</p>
        </div>

        <div class="flex items-center space-x-4 bg-surface-card/40 backdrop-blur-xl p-2 rounded-2xl border border-glass-border">
          <div class="flex flex-col px-4 border-r border-glass-border">
            <span class="text-[8px] font-black text-muted uppercase tracking-[0.2em]">Estado</span>
            <select [(ngModel)]="filtros.estado" (change)="cargarPendientes()"
                    class="bg-transparent text-xs font-black text-white focus:outline-none uppercase">
              <option value="TODOS">Todos</option>
              <option value="PENDIENTE">Pendientes</option>
              <option value="ASIGNADO">Asignados</option>
            </select>
          </div>
          <button (click)="cargarPendientes()" 
                  class="p-2 bg-primary/10 text-primary hover:bg-primary hover:text-white rounded-xl transition-all">
            <lucide-icon name="refresh-cw" class="w-4 h-4"></lucide-icon>
          </button>
        </div>
      </div>

      <!-- Main Table -->
      <div class="bg-surface-card/40 backdrop-blur-xl border border-glass-border rounded-2xl shadow-2xl p-6">
        <div class="overflow-x-auto">
          <table class="w-full text-left border-separate border-spacing-y-2">
            <thead>
              <tr class="text-[10px] font-black text-muted uppercase tracking-[0.2em]">
                <th class="px-4 py-3">Paciente</th>
                <th class="px-4 py-3">Servicio / Categoría</th>
                <th class="px-4 py-3">Honorario</th>
                <th class="px-4 py-3">Médico Asignado</th>
                <th class="px-4 py-3">Estado</th>
                <th class="px-4 py-3 text-right">Acciones</th>
              </tr>
            </thead>
            <tbody>
              <tr *ngFor="let item of items()" class="group bg-white/[0.02] hover:bg-white/[0.05] transition-all border border-white/5 rounded-xl">
                <td class="px-4 py-4">
                  <div class="flex items-center space-x-3">
                    <div class="w-8 h-8 rounded-full bg-surface-card flex items-center justify-center text-[10px] font-black text-primary border border-glass-border">
                      {{ getInitials(item.pacienteNombre) }}
                    </div>
                    <div class="flex flex-col">
                      <span class="text-sm font-black text-white uppercase truncate max-w-[200px]">{{ item.pacienteNombre }}</span>
                      <span class="text-[9px] font-medium text-slate-500">{{ item.fechaCarga | date:'dd/MM/yyyy HH:mm' }}</span>
                    </div>
                  </div>
                </td>
                <td class="px-4 py-4">
                  <div class="flex flex-col">
                    <span class="text-xs font-bold text-white uppercase">{{ item.descripcion }}</span>
                    <span class="text-[9px] font-black text-muted uppercase tracking-widest">{{ item.tipoServicio }}</span>
                  </div>
                </td>
                <td class="px-4 py-4 font-mono text-xs font-black text-emerald-400">
                  $ {{ item.honorario | number:'1.2-2' }}
                </td>
                <td class="px-4 py-4">
                  <div *ngIf="item.medicoAsignadoId" class="flex items-center space-x-2">
                    <div class="w-6 h-6 rounded-md bg-blue-500/10 flex items-center justify-center">
                      <lucide-icon name="user" class="w-3 h-3 text-blue-500"></lucide-icon>
                    </div>
                    <span class="text-xs font-bold text-white uppercase">{{ item.medicoAsignadoNombre }}</span>
                  </div>
                  <span *ngIf="!item.medicoAsignadoId" class="text-[10px] font-black text-slate-600 uppercase">Sin Médico</span>
                </td>
                <td class="px-4 py-4">
                  <span *ngIf="!item.medicoAsignadoId" class="px-3 py-1 bg-rose-500/5 border border-rose-500/10 text-rose-500/60 rounded-lg text-[8px] font-black uppercase">
                    PENDIENTE
                  </span>
                  <span *ngIf="item.medicoAsignadoId && !item.esAutoAsignado" class="px-3 py-1 bg-emerald-500/5 border border-emerald-500/10 text-emerald-500/60 rounded-lg text-[8px] font-black uppercase">
                    ASIGNADO
                  </span>
                  <span *ngIf="item.medicoAsignadoId && item.esAutoAsignado" class="px-3 py-1 bg-indigo-500/5 border border-indigo-500/10 text-indigo-500/60 rounded-lg text-[8px] font-black uppercase flex items-center w-fit">
                    <lucide-icon name="zap" class="w-2 h-2 mr-1 fill-indigo-500"></lucide-icon>
                    AUTO
                  </span>
                </td>
                <td class="px-4 py-4 text-right">
                  <button (click)="openAsignar(item)" 
                          class="px-4 py-2 bg-primary text-white rounded-xl text-[10px] font-black uppercase tracking-widest hover:scale-105 active:scale-95 transition-all shadow-lg shadow-primary/20">
                    Asignar
                  </button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>

      <!-- Modal de Asignación -->
      <div *ngIf="showModal()" class="fixed inset-0 z-[100] flex items-center justify-center p-4">
        <div class="absolute inset-0 bg-black/60 backdrop-blur-sm" (click)="showModal.set(false)"></div>
        
        <div class="relative w-full max-w-lg bg-surface-card/40 backdrop-blur-xl border border-glass-border rounded-2xl shadow-2xl overflow-hidden animate-in fade-in zoom-in duration-200">
          <div class="p-6 border-b border-glass-border">
            <h3 class="text-lg font-black text-white uppercase">Asignar Médico</h3>
            <p class="text-[10px] font-bold text-muted uppercase tracking-widest">
              {{ selectedItem()?.descripcion }} - $ {{ selectedItem()?.honorario | number:'1.2-2' }}
            </p>
          </div>

          <div class="p-6 max-h-[400px] overflow-y-auto custom-scrollbar space-y-2">
            <div *ngFor="let m of medicos()" 
                 (click)="confirmarAsignacion(m)"
                 class="flex items-center justify-between p-3 bg-white/[0.02] hover:bg-primary/10 border border-white/5 rounded-xl cursor-pointer group transition-all">
              <div class="flex items-center space-x-3">
                <div class="w-10 h-10 rounded-full bg-surface-card flex items-center justify-center border border-glass-border">
                  <lucide-icon name="user" class="w-5 h-5 text-muted group-hover:text-primary"></lucide-icon>
                </div>
                <div>
                  <p class="text-sm font-black text-white uppercase tracking-tight">{{ m.nombre }}</p>
                  <p class="text-[9px] font-bold text-muted uppercase">{{ m.especialidadNombre || 'Sin Especialidad' }}</p>
                </div>
              </div>
              <lucide-icon name="chevron-right" class="w-4 h-4 text-muted group-hover:text-primary opacity-0 group-hover:opacity-100 transition-all"></lucide-icon>
            </div>
          </div>

          <div class="p-4 bg-surface/30 flex justify-end">
            <button (click)="showModal.set(false)" class="px-6 py-2 text-[10px] font-black text-muted uppercase tracking-widest">Cancelar</button>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: []
})
export class HonorarioAsignacionesComponent implements OnInit {
  private http = inject(HttpClient);
  
  items = signal<any[]>([]);
  medicos = signal<any[]>([]);
  filtros = {
    estado: 'PENDIENTE',
    desde: null,
    hasta: null
  };

  showModal = signal(false);
  selectedItem = signal<any>(null);

  ngOnInit() {
    this.cargarPendientes();
    this.cargarMedicos();
  }

  cargarPendientes() {
    this.http.get<any[]>(`${environment.apiUrl}/api/AsignacionHonorarios/pendientes`, {
      params: { estado: this.filtros.estado }
    }).subscribe(data => this.items.set(data));
  }

  cargarMedicos() {
    this.http.get<any[]>(`${environment.apiUrl}/api/Medicos`).subscribe(data => {
      this.medicos.set(data.filter(m => m.activo !== false));
    });
  }

  getInitials(name: string): string {
    return name.split(' ').map(n => n[0]).join('').substring(0, 2).toUpperCase();
  }

  openAsignar(item: any) {
    this.selectedItem.set(item);
    this.showModal.set(true);
  }

  confirmarAsignacion(medico: any) {
    const item = this.selectedItem();
    this.http.post(`${environment.apiUrl}/api/AsignacionHonorarios/asignar`, {
      detalleServicioId: item.detalleId,
      medicoId: medico.id,
      categoriaHonorario: this.mapearCategoria(item.tipoServicio)
    }).subscribe(() => {
      this.showModal.set(false);
      this.cargarPendientes();
    });
  }

  private mapearCategoria(tipo: string): string {
    const t = tipo?.toUpperCase();
    if (t.includes('RX') || t.includes('IMAGEN')) return HONORARIO_CATEGORIAS.RX;
    if (t.includes('INFORME')) return HONORARIO_CATEGORIAS.INFORME;
    if (t.includes('CITO')) return HONORARIO_CATEGORIAS.CITOLOGIA;
    if (t.includes('BIOPSIA')) return HONORARIO_CATEGORIAS.BIOPSIA;
    return HONORARIO_CATEGORIAS.CONSULTA;
  }
}
