import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { LucideAngularModule, BarChart3, TrendingUp, AlertCircle, ShieldCheck, Download, Filter } from 'lucide-angular';

@Component({
  selector: 'app-management-insurance-dashboard',
  standalone: true,
  imports: [CommonModule, LucideAngularModule],
  template: `
<div class="p-8 max-w-[1700px] space-y-8 animate-fade-in relative z-10">
    <!-- Header Premium -->
    <div class="flex items-center justify-between">
        <div class="flex items-center gap-6">
            <div class="h-14 w-14 bg-indigo-500/10 border border-indigo-500/20 rounded-2xl flex items-center justify-center text-indigo-500 shadow-xl shadow-indigo-500/5">
                <lucide-icon [name]="icons.BarChart3" class="w-7 h-7"></lucide-icon>
            </div>
            <div>
                <h1 class="text-3xl font-black text-white tracking-tighter uppercase leading-none">Consolidado de Seguros</h1>
                <p class="text-[9px] font-black text-slate-500 uppercase tracking-[0.3em] mt-2 italic">Reporte Gerencial de Cuentas por Cobrar</p>
            </div>
        </div>
        
        <div class="flex items-center gap-3">
            <button class="px-6 py-3 bg-white/5 border border-white/10 rounded-xl text-[10px] font-black text-white uppercase tracking-widest hover:bg-white/10 transition-all flex items-center gap-2">
                <lucide-icon [name]="icons.Download" class="w-4 h-4"></lucide-icon> Exportar PDF
            </button>
            <button class="px-6 py-3 bg-emerald-500 text-white rounded-xl text-[10px] font-black uppercase tracking-widest shadow-lg shadow-emerald-500/20 hover:scale-105 active:scale-95 transition-all flex items-center gap-2">
                <lucide-icon [name]="icons.Filter" class="w-4 h-4"></lucide-icon> Ajustar Período
            </button>
        </div>
    </div>

    <!-- Key Performance Indicators (KPIs) -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-6">
        <div *ngFor="let kpi of kpis()" class="p-6 bg-white/[0.02] border border-white/5 rounded-[2rem] space-y-4">
            <div class="flex items-center justify-between">
                <span class="text-[9px] font-black text-slate-500 uppercase tracking-widest">{{ kpi.title }}</span>
                <div [ngClass]="kpi.color" class="p-2 rounded-lg bg-current/10">
                    <lucide-icon [name]="kpi.icon" class="w-4 h-4"></lucide-icon>
                </div>
            </div>
            <div class="flex items-end justify-between">
                <h2 class="text-2xl font-black text-white tracking-tighter">{{ kpi.value }}</h2>
                <span class="text-[8px] font-bold text-emerald-500 bg-emerald-500/10 px-2 py-0.5 rounded-full">{{ kpi.trend }}</span>
            </div>
        </div>
    </div>

    <!-- Main Analytics Section -->
    <div class="grid grid-cols-1 lg:grid-cols-3 gap-8">
        <!-- Deuda por Seguro (Tabla Consolidada) -->
        <div class="lg:col-span-2 bg-white/[0.02] border border-white/5 rounded-[2.5rem] overflow-hidden flex flex-col">
            <div class="p-8 border-b border-white/5">
                <h3 class="text-xs font-black text-white uppercase tracking-widest">Resumen por Institución / Seguro</h3>
            </div>
            <div class="flex-1 overflow-x-auto">
                <table class="w-full text-left">
                    <thead>
                        <tr class="bg-white/[0.01]">
                            <th class="px-8 py-4 text-[8px] font-black text-slate-500 uppercase tracking-widest">Institución</th>
                            <th class="px-8 py-4 text-[8px] font-black text-slate-500 uppercase tracking-widest text-center">Pacientes</th>
                            <th class="px-8 py-4 text-[8px] font-black text-slate-500 uppercase tracking-widest text-right">Total Pendiente</th>
                            <th class="px-8 py-4 text-[8px] font-black text-slate-500 uppercase tracking-widest text-center">Status</th>
                        </tr>
                    </thead>
                    <tbody class="divide-y divide-white/5">
                        <tr *ngFor="let s of seguros()" class="hover:bg-white/[0.04] transition-all group cursor-pointer">
                            <td class="px-8 py-6">
                                <div class="flex items-center gap-4">
                                    <div class="h-10 w-10 rounded-full bg-surface-card border border-white/10 flex items-center justify-center text-xs font-black text-white group-hover:border-indigo-500/50 transition-colors">
                                        {{ s.nombre.substring(0, 2) }}
                                    </div>
                                    <span class="text-[10px] font-black text-white uppercase tracking-tight">{{ s.nombre }}</span>
                                </div>
                            </td>
                            <td class="px-8 py-6 text-center">
                                <span class="text-xs font-black text-slate-400">{{ s.pacientes }}</span>
                            </td>
                            <td class="px-8 py-6 text-right">
                                <span class="text-sm font-black font-mono text-emerald-400 tracking-tighter">$ {{ s.monto | number:'1.2-2' }}</span>
                            </td>
                            <td class="px-8 py-6 text-center">
                                <span [ngClass]="s.monto > 5000 ? 'text-rose-500 bg-rose-500/10' : 'text-emerald-500 bg-emerald-500/10'" 
                                    class="text-[7px] font-black uppercase px-3 py-1 rounded-full">
                                    {{ s.monto > 5000 ? 'Alta Morosidad' : 'Estable' }}
                                </span>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>

        <!-- Alertas de Morosidad y Resumen de Estado -->
        <div class="space-y-6">
            <div class="p-8 bg-indigo-500 text-white rounded-[2.5rem] relative overflow-hidden shadow-2xl shadow-indigo-500/20">
                <lucide-icon [name]="icons.TrendingUp" class="w-32 h-32 absolute -right-8 -bottom-8 opacity-20"></lucide-icon>
                <h3 class="text-xl font-black uppercase tracking-tighter mb-4">Meta Mensual</h3>
                <p class="text-[10px] font-bold opacity-80 uppercase leading-relaxed mb-8">Se ha recuperado el 65% de la deuda proyectada para este período.</p>
                <div class="h-2 w-full bg-white/20 rounded-full overflow-hidden">
                    <div class="h-full bg-white w-[65%]"></div>
                </div>
            </div>

            <div class="p-8 bg-white/[0.02] border border-white/5 rounded-[2.5rem] space-y-6">
                <h3 class="text-xs font-black text-white uppercase tracking-widest">Compromisos Vencidos</h3>
                <div class="space-y-4">
                    <div *ngFor="let item of [1,2,3]" class="flex items-center gap-4 p-4 bg-white/5 rounded-2xl border border-white/5">
                        <div class="h-8 w-8 bg-rose-500/10 text-rose-500 rounded-lg flex items-center justify-center">
                            <lucide-icon [name]="icons.AlertCircle" class="w-4 h-4"></lucide-icon>
                        </div>
                        <div class="flex-1 min-w-0">
                            <p class="text-[9px] font-black text-white uppercase truncate">Sura Seguros - Cta #8822</p>
                            <p class="text-[7px] font-bold text-slate-500 uppercase mt-0.5">Hace 45 días • $ 1,200.00</p>
                        </div>
                    </div>
                </div>
                <button class="w-full py-4 text-[8px] font-black text-slate-500 uppercase tracking-[0.2em] hover:text-white transition-colors">Ver todas las alertas</button>
            </div>
        </div>
    </div>
</div>
  `,
  styles: [`
    :host { display: block; }
  `]
})
export class ManagementInsuranceDashboardComponent implements OnInit {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api/Seguros`;

  public icons = { BarChart3, TrendingUp, AlertCircle, ShieldCheck, Download, Filter };

  public kpis = signal([
    { title: 'Total Pendiente', value: '$ 0.00', trend: '...', icon: this.icons.TrendingUp, color: 'text-emerald-500' },
    { title: 'Pacientes Activos', value: '0', trend: '...', icon: this.icons.ShieldCheck, color: 'text-sky-500' },
    { title: 'Morosidad Crítica', value: '0 Cuentas', trend: '...', icon: this.icons.AlertCircle, color: 'text-rose-500' }
  ]);

  public seguros = signal<any[]>([]);

  ngOnInit() {
    this.loadConsolidado();
  }

  loadConsolidado() {
    this.http.get<any[]>(`${this.apiUrl}/consolidado-gerencia`).subscribe({
      next: (data) => {
        this.seguros.set(data);
        
        const total = data.reduce((acc, curr) => acc + curr.monto, 0);
        const pacientes = data.reduce((acc, curr) => acc + curr.pacientes, 0);
        const criticos = data.reduce((acc, curr) => acc + curr.criticos, 0);

        this.kpis.set([
          { title: 'Total Pendiente', value: `$ ${total.toLocaleString()}`, trend: 'Actualizado', icon: this.icons.TrendingUp, color: 'text-emerald-500' },
          { title: 'Pacientes Activos', value: `${pacientes}`, trend: 'Global', icon: this.icons.ShieldCheck, color: 'text-sky-500' },
          { title: 'Morosidad Crítica', value: `${criticos} Cuentas`, trend: 'Alertas', icon: this.icons.AlertCircle, color: 'text-rose-500' }
        ]);
      }
    });
  }
}

