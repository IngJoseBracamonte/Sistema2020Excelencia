import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MedicoService, DoctorHonorariumSummaryDto } from '../../../core/services/medico.service';
import { LucideAngularModule, Stethoscope, Calendar, DollarSign, Activity } from 'lucide-angular';

@Component({
  selector: 'app-admin-honorariums',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule],
  template: `
    <div class="p-8 max-w-[1600px] space-y-8 animate-fade-in">
        <!-- Header Estilo Estación -->
        <div class="flex items-center justify-between">
            <div class="flex items-center gap-6">
                <div class="h-14 w-14 bg-blue-500/10 border border-blue-500/20 rounded-2xl flex items-center justify-center text-blue-500 shadow-lg shadow-blue-500/5">
                    <lucide-icon name="stethoscope" class="w-7 h-7"></lucide-icon>
                </div>
                <div>
                    <h1 class="text-3xl font-black text-white tracking-tight uppercase leading-none">Cálculo de Honorarios</h1>
                    <p class="text-[10px] font-black text-slate-500 uppercase tracking-[0.3em] mt-3 opacity-60">Reporte Administrativo de Pagos Médicos</p>
                </div>
            </div>

            <!-- Filtros de Fecha Consolidados -->
            <div class="flex items-center bg-surface-card/40 backdrop-blur-xl border border-white/5 rounded-2xl p-1.5 gap-2 shadow-2xl">
                <div class="flex items-center px-4 py-2 gap-4">
                    <div class="flex flex-col">
                        <span class="text-[7px] font-black text-blue-500 uppercase tracking-widest mb-0.5">Desde</span>
                        <input type="date" [(ngModel)]="startDate" class="bg-transparent border-none p-0 text-[10px] font-black text-white focus:ring-0 uppercase cursor-pointer">
                    </div>
                    <div class="w-px h-6 bg-white/10"></div>
                    <div class="flex flex-col">
                        <span class="text-[7px] font-black text-blue-500 uppercase tracking-widest mb-0.5">Hasta</span>
                        <input type="date" [(ngModel)]="endDate" class="bg-transparent border-none p-0 text-[10px] font-black text-white focus:ring-0 uppercase cursor-pointer">
                    </div>
                </div>
                <button (click)="calcular()" [disabled]="isLoading()" 
                    class="bg-blue-600 hover:bg-blue-500 text-white px-6 py-3 rounded-xl font-black text-[10px] uppercase tracking-widest transition-all hover:scale-105 active:scale-95 shadow-lg shadow-blue-600/20">
                    {{ isLoading() ? 'Procesando...' : 'Calcular Pagos' }}
                </button>
            </div>
        </div>

        <!-- Dashboard Cards (Resúmenes) -->
        <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
            <div class="bg-surface-card/40 backdrop-blur-xl border border-white/5 rounded-[2rem] p-8 group overflow-hidden relative shadow-2xl">
                <div class="absolute top-0 right-0 w-24 h-24 bg-blue-500/5 rounded-bl-full blur-2xl"></div>
                <div class="relative z-10">
                    <p class="text-[9px] font-black text-slate-500 uppercase tracking-widest mb-2">Total Médicos con Actividad</p>
                    <div class="text-4xl font-black text-white tracking-tighter">{{ data().length }}</div>
                </div>
            </div>
            <div class="bg-surface-card/40 backdrop-blur-xl border border-white/5 rounded-[2rem] p-8 group overflow-hidden relative shadow-2xl">
                <div class="absolute top-0 right-0 w-24 h-24 bg-emerald-500/5 rounded-bl-full blur-2xl"></div>
                <div class="relative z-10">
                    <p class="text-[9px] font-black text-slate-500 uppercase tracking-widest mb-2">Total Honorarios Acumulados</p>
                    <div class="text-4xl font-black text-emerald-400 tracking-tighter font-mono">$ {{ totalAcumulado() | number:'1.2-2' }}</div>
                </div>
            </div>
            <div class="bg-surface-card/40 backdrop-blur-xl border border-white/5 rounded-[2rem] p-8 group overflow-hidden relative shadow-2xl">
                <div class="absolute top-0 right-0 w-24 h-24 bg-rose-500/5 rounded-bl-full blur-2xl"></div>
                <div class="relative z-10">
                    <p class="text-[9px] font-black text-slate-500 uppercase tracking-widest mb-2">Promedio por Servicio</p>
                    <div class="text-4xl font-black text-white tracking-tighter font-mono">$ {{ promedioPorServicio() | number:'1.2-2' }}</div>
                </div>
            </div>
        </div>

        <!-- Tabla de Resultados -->
        <div class="bg-surface-card/40 backdrop-blur-xl border border-white/5 rounded-[2rem] overflow-hidden relative shadow-2xl">
            <div class="overflow-x-auto">
                <table class="w-full text-left border-collapse">
                    <thead>
                        <tr class="bg-white/5 border-b border-white/5">
                            <th class="px-8 py-5 text-[9px] font-black text-slate-500 uppercase tracking-[0.2em]">Médico Profesional</th>
                            <th class="px-8 py-5 text-[9px] font-black text-slate-500 uppercase tracking-[0.2em] text-center">Servicios Realizados</th>
                            <th class="px-8 py-5 text-[9px] font-black text-slate-500 uppercase tracking-[0.2em] text-right">Monto a Pagar (USD)</th>
                        </tr>
                    </thead>
                    <tbody class="divide-y divide-white/5">
                        <ng-container *ngFor="let row of data()">
                            <tr (click)="toggleExpand(row.medicoId)" 
                                class="hover:bg-white/10 transition-all cursor-pointer" 
                                [ngClass]="{'bg-white/10': expandedRow() === row.medicoId}">
                                <td class="px-8 py-4">
                                    <div class="flex items-center gap-4">
                                        <div class="h-10 w-10 bg-slate-800 rounded-xl border border-white/5 flex items-center justify-center text-slate-500 transition-all">
                                            <lucide-icon [name]="expandedRow() === row.medicoId ? 'chevron-down' : 'stethoscope'" class="w-5 h-5"></lucide-icon>
                                        </div>
                                        <div>
                                            <div class="text-[11px] font-black text-white uppercase tracking-tight transition-colors leading-none mb-1">{{ row.medicoNombre }}</div>
                                            <div class="text-[8px] font-mono text-slate-500 tracking-wider">ID: {{ row.medicoId.substring(0,8).toUpperCase() }}</div>
                                        </div>
                                    </div>
                                </td>
                                <td class="px-8 py-4 text-center">
                                    <span class="px-3 py-1 bg-white/5 rounded-full text-[10px] font-black text-white tracking-widest border border-white/5">{{ row.cantidadServicios }}</span>
                                </td>
                                <td class="px-8 py-4 text-right">
                                    <span class="text-sm font-black text-emerald-400 font-mono tracking-tighter">$ {{ row.totalHonorarios | number:'1.2-2' }}</span>
                                </td>
                            </tr>

                            <!-- Breakdown Row -->
                            <tr *ngIf="expandedRow() === row.medicoId" class="bg-white/[0.02] animate-in fade-in slide-in-from-top-2 duration-200">
                                <td colspan="3" class="px-8 py-6">
                                    <div class="grid grid-cols-2 md:grid-cols-5 gap-4">
                                        <div *ngFor="let cat of row.desglose" class="bg-surface-card/40 backdrop-blur-xl border border-glass-border p-4 rounded-2xl flex flex-col items-center justify-center text-center">
                                            <span class="text-[8px] font-black text-slate-500 uppercase tracking-widest mb-1">{{ cat.categoria }}</span>
                                            <span class="text-lg font-black text-white leading-none">{{ cat.cantidad }}</span>
                                            <span class="text-[10px] font-mono text-emerald-400 mt-1 font-bold">$ {{ cat.total | number:'1.2-2' }}</span>
                                        </div>
                                    </div>
                                </td>
                            </tr>
                        </ng-container>

                        <!-- Empty State -->
                        <tr *ngIf="data().length === 0 && !isLoading()">
                            <td colspan="3" class="px-8 py-20 text-center">
                                <div class="opacity-20 flex flex-col items-center">
                                    <lucide-icon name="activity" class="w-12 h-12 mb-4"></lucide-icon>
                                    <p class="text-[10px] font-black uppercase tracking-[0.3em]">No hay actividad en este rango</p>
                                </div>
                            </td>
                        </tr>
                        
                        <!-- Loading State -->
                        <tr *ngIf="isLoading()">
                             <td colspan="3" class="px-8 py-20 text-center">
                                <div class="animate-pulse space-y-4 max-w-sm mx-auto">
                                    <div class="h-4 bg-white/5 rounded w-3/4 mx-auto"></div>
                                    <div class="h-4 bg-white/5 rounded w-1/2 mx-auto"></div>
                                </div>
                             </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>
    </div>
  `,
  styles: [`
    input[type="date"]::-webkit-calendar-picker-indicator {
        filter: invert(1);
        opacity: 0.5;
        cursor: pointer;
    }
  `]
})
export class AdminHonorariumsComponent implements OnInit {
  private medicoService = inject(MedicoService);

  public startDate = signal<string>(new Date().toISOString().split('T')[0]);
  public endDate = signal<string>(new Date().toISOString().split('T')[0]);
  public data = signal<DoctorHonorariumSummaryDto[]>([]);
  public isLoading = signal<boolean>(false);
  public expandedRow = signal<string | null>(null);

  ngOnInit() {
    this.calcular();
  }

  calcular() {
    this.isLoading.set(true);
    this.medicoService.getHonorariumSummary(this.startDate(), this.endDate()).subscribe({
      next: (res) => {
        this.data.set(res);
        this.isLoading.set(false);
      },
      error: (err) => {
        console.error('Error al calcular honorarios:', err);
        this.isLoading.set(false);
      }
    });
  }

  totalAcumulado() {
    return this.data().reduce((acc, curr) => acc + curr.totalHonorarios, 0);
  }

  totalServicios() {
    return this.data().reduce((acc, curr) => acc + curr.cantidadServicios, 0);
  }

  promedioPorServicio() {
    const totalSvc = this.totalServicios();
    return totalSvc > 0 ? this.totalAcumulado() / totalSvc : 0;
  }

  toggleExpand(medicoId: string) {
    if (this.expandedRow() === medicoId) {
      this.expandedRow.set(null);
    } else {
      this.expandedRow.set(medicoId);
    }
  }
}
