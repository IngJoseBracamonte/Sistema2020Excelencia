import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { LucideAngularModule } from 'lucide-angular';
import { environment } from '../../../../environments/environment';
import { HONORARIO_CATEGORIAS } from '../../../core/constants/honorario.constants';

@Component({
  selector: 'app-honorario-config',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule],
  template: `
    <div class="p-6 space-y-6">
      <div class="flex items-center justify-between">
        <div>
          <h1 class="text-2xl font-black text-white tracking-tight uppercase">Configuración de Honorarios</h1>
          <p class="text-xs text-muted font-bold uppercase tracking-widest">Asignación de médicos responsables por defecto</p>
        </div>
      </div>

      <div class="grid grid-cols-1 gap-6">
        <div class="bg-surface-card/40 backdrop-blur-xl border border-glass-border rounded-2xl shadow-2xl p-6">
          <div class="overflow-x-auto">
            <table class="w-full text-left border-separate border-spacing-y-2">
              <thead>
                <tr class="text-[10px] font-black text-muted uppercase tracking-[0.2em]">
                  <th class="px-4 py-3">Categoría</th>
                  <th class="px-4 py-3">Médico por Defecto</th>
                  <th class="px-4 py-3">Última Modificación</th>
                  <th class="px-4 py-3">Acciones</th>
                </tr>
              </thead>
              <tbody>
                <tr *ngFor="let cat of categorias()" class="group bg-white/[0.02] hover:bg-white/[0.05] transition-all border border-white/5 rounded-xl">
                  <td class="px-4 py-4">
                    <span class="text-sm font-black text-white uppercase">{{ cat.nombre }}</span>
                  </td>
                  <td class="px-4 py-4">
                    <select [(ngModel)]="cat.medicoId" 
                            class="bg-surface-card border border-glass-border text-white text-xs rounded-lg p-2 w-full focus:outline-none focus:border-primary transition-colors">
                      <option [ngValue]="null">-- Sin asignar --</option>
                      <option *ngFor="let m of medicos()" [value]="m.id">{{ m.nombre }}</option>
                    </select>
                  </td>
                  <td class="px-4 py-4">
                    <div class="flex flex-col">
                      <span class="text-[10px] font-bold text-muted uppercase tracking-wider">{{ cat.usuarioConfiguro || 'N/A' }}</span>
                      <span class="text-[9px] font-medium text-slate-500">{{ (cat.fechaConfiguracion | date:'dd/MM/yyyy HH:mm') || '-' }}</span>
                    </div>
                  </td>
                  <td class="px-4 py-4">
                    <button (click)="guardarConfig(cat)" 
                            class="p-2 bg-primary/10 text-primary hover:bg-primary hover:text-white rounded-lg transition-all">
                      <lucide-icon name="save" class="w-4 h-4"></lucide-icon>
                    </button>
                    <button (click)="limpiarConfig(cat)" 
                            class="p-2 ml-2 bg-rose-500/10 text-rose-500 hover:bg-rose-500 hover:text-white rounded-lg transition-all">
                      <lucide-icon name="trash-2" class="w-4 h-4"></lucide-icon>
                    </button>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>

        <!-- Nueva Sección: Reglas de Mapeo Dinámicas -->
        <div class="bg-surface-card/40 backdrop-blur-xl border border-glass-border rounded-2xl shadow-2xl p-6 border-l-2 border-primary/40">
          <div class="flex items-center justify-between mb-6">
            <div>
              <h2 class="text-sm font-black text-white uppercase tracking-[0.2em]">Reglas de Mapeo Dinámicas</h2>
              <p class="text-[10px] font-bold text-muted uppercase">Configura prefijos para auto-categorización</p>
            </div>
            <button (click)="showRuleForm = !showRuleForm" 
                    class="px-4 py-2 bg-primary/10 text-primary hover:bg-primary hover:text-white rounded-xl text-[10px] font-black uppercase transition-all">
              {{ showRuleForm ? 'Cancelar' : 'Nueva Regla' }}
            </button>
          </div>

          <!-- Formulario Nueva Regla -->
          <div *ngIf="showRuleForm" class="grid grid-cols-1 md:grid-cols-4 gap-4 p-4 bg-white/[0.02] rounded-xl border border-white/5 mb-6 animate-in slide-in-from-top-2">
            <div class="flex flex-col">
              <label class="text-[9px] font-black text-muted uppercase mb-1">Patrón (Prefijo)</label>
              <input [(ngModel)]="newRule.pattern" placeholder="Ej: RX_" 
                     class="bg-surface-card border border-glass-border text-white text-xs rounded-lg p-2 focus:outline-none focus:border-primary">
            </div>
            <div class="flex flex-col">
              <label class="text-[9px] font-black text-muted uppercase mb-1">Categoría</label>
              <select [(ngModel)]="newRule.category" 
                      class="bg-surface-card border border-glass-border text-white text-xs rounded-lg p-2 focus:outline-none focus:border-primary">
                <option *ngFor="let c of catList" [value]="c">{{ c }}</option>
              </select>
            </div>
            <div class="flex flex-col">
              <label class="text-[9px] font-black text-muted uppercase mb-1">Prioridad</label>
              <input type="number" [(ngModel)]="newRule.priority" 
                     class="bg-surface-card border border-glass-border text-white text-xs rounded-lg p-2 focus:outline-none focus:border-primary">
            </div>
            <div class="flex items-end">
              <button (click)="crearRegla()" 
                      class="w-full py-2 bg-primary text-white rounded-lg text-[10px] font-black uppercase shadow-lg shadow-primary/20">
                Guardar Regla
              </button>
            </div>
          </div>

          <div class="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 gap-4">
            <div *ngFor="let rule of rules()" class="group relative p-4 bg-white/[0.02] hover:bg-white/[0.05] border border-white/5 rounded-xl transition-all">
              <div class="flex items-center justify-between mb-2">
                <span class="px-2 py-0.5 bg-primary/20 text-primary rounded text-[8px] font-black uppercase">{{ rule.category }}</span>
                <button (click)="eliminarRegla(rule.id)" class="text-rose-500 opacity-0 group-hover:opacity-100 transition-all">
                  <lucide-icon name="trash-2" class="w-3 h-3"></lucide-icon>
                </button>
              </div>
              <p class="text-sm font-black text-white uppercase">{{ rule.pattern }}</p>
              <div class="mt-2 flex items-center justify-between">
                <span class="text-[8px] font-bold text-muted uppercase">Prioridad: {{ rule.priority }}</span>
                <span class="text-[8px] font-medium text-slate-600">{{ rule.fechaCreacion | date:'dd/MM/yy' }}</span>
              </div>
            </div>
          </div>
        </div>

        <div class="bg-surface-card/40 backdrop-blur-xl border border-glass-border rounded-2xl shadow-2xl p-6 border-t-2 border-amber-500/20">
          <h2 class="text-sm font-black text-amber-500 uppercase tracking-[0.2em] mb-4">Registro de Cambios (Configuración)</h2>
          <div class="space-y-3 max-h-[300px] overflow-y-auto custom-scrollbar">
            <div *ngFor="let log of logs()" class="flex items-center justify-between p-3 bg-white/[0.01] rounded-lg border border-white/5">
              <div class="flex items-center space-x-3">
                <div class="w-8 h-8 rounded-full bg-amber-500/10 flex items-center justify-center">
                  <lucide-icon name="settings" class="w-4 h-4 text-amber-500"></lucide-icon>
                </div>
                <div>
                  <p class="text-[10px] font-black text-white uppercase">{{ log.nombreServicio }}</p>
                  <p class="text-[9px] font-bold text-muted uppercase tracking-widest">{{ log.medicoNuevoNombre || 'Limpiado' }}</p>
                </div>
              </div>
              <div class="text-right">
                <p class="text-[10px] font-black text-muted uppercase">{{ log.usuarioOperador }}</p>
                <p class="text-[9px] font-medium text-slate-600">{{ log.fechaAccion | date:'dd/MM/yyyy HH:mm' }}</p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: []
})
export class HonorarioConfigComponent implements OnInit {
  private http = inject(HttpClient);
  
  categorias = signal<any[]>([
    { nombre: HONORARIO_CATEGORIAS.CONSULTA, medicoId: null },
    { nombre: HONORARIO_CATEGORIAS.RX, medicoId: null },
    { nombre: HONORARIO_CATEGORIAS.INFORME, medicoId: null },
    { nombre: HONORARIO_CATEGORIAS.CITOLOGIA, medicoId: null },
    { nombre: HONORARIO_CATEGORIAS.BIOPSIA, medicoId: null }
  ]);

  medicos = signal<any[]>([]);
  logs = signal<any[]>([]);
  rules = signal<any[]>([]);

  showRuleForm = false;
  catList = Object.values(HONORARIO_CATEGORIAS);
  newRule = {
    pattern: '',
    category: HONORARIO_CATEGORIAS.RX,
    priority: 1,
    matchType: 1 // Contains
  };

  ngOnInit() {
    this.cargarMedicos();
    this.cargarConfigs();
    this.cargarLogs();
    this.cargarRules();
  }

  cargarMedicos() {
    this.http.get<any[]>(`${environment.apiUrl}/api/Medicos`).subscribe(data => {
      this.medicos.set(data.filter(m => m.activo !== false));
    });
  }

  cargarConfigs() {
    this.http.get<any[]>(`${environment.apiUrl}/api/HonorarioConfig`).subscribe(data => {
      const current = this.categorias();
      data.forEach(config => {
        const cat = current.find(c => c.nombre === config.categoriaServicio);
        if (cat) {
          cat.medicoId = config.medicoDefaultId;
          cat.usuarioConfiguro = config.usuarioConfiguro;
          cat.fechaConfiguracion = config.fechaConfiguracion;
        }
      });
      this.categorias.set([...current]);
    });
  }

  cargarLogs() {
    this.http.get<any[]>(`${environment.apiUrl}/api/HonorarioConfig/logs`).subscribe(data => {
      this.logs.set(data);
    });
  }

  guardarConfig(cat: any) {
    this.http.put(`${environment.apiUrl}/api/HonorarioConfig/${cat.nombre}`, {
      medicoId: cat.medicoId,
      observaciones: `Cambiado vía panel de configuración`
    }).subscribe(() => {
      this.cargarConfigs();
      this.cargarLogs();
    });
  }

  limpiarConfig(cat: any) {
    cat.medicoId = null;
    this.guardarConfig(cat);
  }

  cargarRules() {
    this.http.get<any[]>(`${environment.apiUrl}/api/HonorariumRules`).subscribe(data => {
      this.rules.set(data);
    });
  }

  crearRegla() {
    if (!this.newRule.pattern) return;
    this.http.post(`${environment.apiUrl}/api/HonorariumRules`, this.newRule).subscribe(() => {
      this.showRuleForm = false;
      this.newRule.pattern = '';
      this.cargarRules();
    });
  }

  eliminarRegla(id: string) {
    this.http.delete(`${environment.apiUrl}/api/HonorariumRules/${id}`).subscribe(() => {
      this.rules.set(this.rules().filter(r => r.id !== id));
    });
  }
}
