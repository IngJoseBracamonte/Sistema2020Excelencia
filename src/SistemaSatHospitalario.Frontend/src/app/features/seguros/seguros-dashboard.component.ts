import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { LucideAngularModule, Shield, Download, Calendar, Search, RefreshCcw, ChevronRight, Check } from 'lucide-angular';

@Component({
  selector: 'app-seguros-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule],
  template: `
<div class="p-8 max-w-[1700px] space-y-6 animate-fade-in relative z-10">
    <!-- Header Estilo "Clear" Moderno -->
    <div class="px-0 pb-2 animate-fade-in relative">
        <div class="flex items-center justify-between p-6 bg-white/[0.02] border border-white/5 rounded-3xl transition-all">
            <div class="flex items-center gap-6">
                <!-- Icono Estilo Estación Claro -->
                <div class="h-14 w-14 bg-emerald-500/5 border border-emerald-500/20 rounded-xl flex items-center justify-center text-emerald-500 shadow-xl shadow-emerald-500/10">
                    <lucide-icon [name]="icons.Shield" class="w-7 h-7"></lucide-icon>
                </div>
                <div>
                    <h1 class="text-2xl font-black text-white/90 tracking-tight uppercase leading-none">Gestión de Seguros</h1>
                    <p class="text-[8px] font-black text-slate-500 uppercase tracking-[0.2em] mt-2 italic">Pacientes Ingresados y Compromisos de Pago</p>
                </div>
            </div>
        </div>
    </div>

    <!-- Barra de Filtros Consolidada (Estilo CxC) -->
    <div class="px-0 animate-fade-in-up">
        <div class="flex flex-col md:flex-row items-stretch bg-white/[0.02] border border-white/5 rounded-[2rem] p-2 gap-2 relative overflow-hidden group">
            <!-- Buscador Minimalista -->
            <div class="relative flex-[1.5] min-w-[250px]">
                <input type="text" [ngModel]="nombreFiltro()" (ngModelChange)="nombreFiltro.set($event)"
                    (keyup.enter)="loadPacientes()" placeholder="Buscar por Nombre o Cédula..."
                    class="w-full pl-20 pr-4 py-3 bg-transparent border-none focus:ring-0 font-bold text-sm tracking-tight text-white placeholder:text-slate-600 uppercase">
                <lucide-icon [name]="icons.Search"
                    class="w-5 h-5 absolute left-4 top-1/2 -translate-y-1/2 text-slate-500 group-focus-within:text-emerald-500 transition-colors"></lucide-icon>
            </div>

            <div class="hidden md:block w-px bg-white/5 my-2"></div>

            <!-- Selectores de Fecha -->
            <div class="flex items-center gap-4 px-4 bg-white/5 rounded-2xl md:bg-transparent md:rounded-none flex-1 justify-around">
                <div class="flex flex-col">
                    <span class="text-[7px] font-black text-emerald-500 uppercase tracking-widest mb-0.5">Desde</span>
                    <input type="date" [ngModel]="fechaDesde()" (ngModelChange)="fechaDesde.set($event); loadPacientes()"
                        class="bg-transparent border-none p-0 text-[10px] font-black text-white focus:ring-0 uppercase cursor-pointer">
                </div>
                <div class="h-6 w-px bg-white/10 mx-2"></div>
                <div class="flex flex-col">
                    <span class="text-[7px] font-black text-emerald-500 uppercase tracking-widest mb-0.5">Hasta</span>
                    <input type="date" [ngModel]="fechaHasta()" (ngModelChange)="fechaHasta.set($event); loadPacientes()"
                        class="bg-transparent border-none p-0 text-[10px] font-black text-white focus:ring-0 uppercase cursor-pointer">
                </div>
            </div>

            <div class="hidden md:block w-px bg-white/5 my-2"></div>

            <!-- Selector de Estado -->
            <div class="relative flex-1">
                <select [ngModel]="estadoFiltro()" (ngModelChange)="estadoFiltro.set($event); loadPacientes()"
                    class="w-full h-full pl-6 pr-10 bg-transparent border-none text-[10px] font-black uppercase tracking-widest text-white outline-none appearance-none cursor-pointer focus:ring-0">
                    <option value="Todos">TODOS LOS INGRESADOS</option>
                    <option value="Pendiente">SIN COMPROMISO DE PAGO</option>
                    <option value="Generado">CON COMPROMISO DE PAGO</option>
                </select>
                <lucide-icon [name]="icons.ChevronRight"
                    class="w-3 h-3 absolute right-4 top-1/2 -translate-y-1/2 text-slate-500 rotate-90 pointer-events-none"></lucide-icon>
            </div>

            <button (click)="loadPacientes()"
                class="bg-emerald-500 hover:bg-emerald-600 text-white px-8 py-3 rounded-2xl font-black text-[10px] uppercase tracking-widest transition-all active:scale-95 shadow-lg shadow-emerald-500/20">
                <span *ngIf="!isLoading()">FILTRAR</span>
                <lucide-icon *ngIf="isLoading()" [name]="icons.RefreshCcw" class="w-4 h-4 animate-spin mx-auto"></lucide-icon>
            </button>
        </div>
    </div>

    <!-- Listado: Vista de Tabla Premium -->
    <div class="bg-white/[0.02] border border-white/5 rounded-3xl overflow-hidden shadow-2xl animate-fade-in-up">
        <div class="overflow-x-auto">
            <table class="w-full text-left border-collapse">
                <thead>
                    <tr class="border-b border-white/5">
                        <th class="px-6 py-4 text-[9px] font-black text-slate-500 uppercase tracking-[0.2em]">Fecha Ingreso</th>
                        <th class="px-6 py-4 text-[9px] font-black text-slate-500 uppercase tracking-[0.2em]">Paciente</th>
                        <th class="px-6 py-4 text-[9px] font-black text-slate-500 uppercase tracking-[0.2em] text-center">Estado Compromiso</th>
                        <th class="px-6 py-4 text-[9px] font-black text-slate-500 uppercase tracking-[0.2em] text-right">Monto ($)</th>
                        <th class="px-6 py-4 text-[9px] font-black text-slate-500 uppercase tracking-[0.2em] text-center">Acción</th>
                    </tr>
                </thead>
                <tbody class="divide-y divide-white/5">
                    <tr *ngFor="let p of pacientes()" class="hover:bg-white/[0.04] transition-all group/row border-l-2 border-transparent">
                        <td class="px-6 py-4">
                            <div class="flex items-center gap-3">
                                <span class="text-xs font-black text-white leading-none">{{ p.fechaCreacion | date:'dd' }}</span>
                                <span class="text-[8px] font-black uppercase text-emerald-500 tracking-widest">{{ p.fechaCreacion | date:'MMM' }}</span>
                            </div>
                        </td>
                        <td class="px-6 py-4">
                            <div class="flex flex-col">
                                <h4 class="text-[11px] font-black text-white uppercase tracking-tight leading-tight group-hover/row:text-emerald-500 transition-colors">
                                    {{ p.pacienteNombre }}
                                </h4>
                                <p class="text-[9px] font-mono text-slate-600 uppercase tracking-widest mt-1">{{ p.pacienteCedula }}</p>
                                <span class="text-[8px] text-slate-500 font-bold uppercase mt-1">{{ p.seguroNombre }}</span>
                            </div>
                        </td>
                        <td class="px-6 py-4 text-center">
                            <span *ngIf="p.compromisoGenerado" class="inline-flex items-center gap-2 px-3 py-1 bg-emerald-500/5 border border-emerald-500/10 text-emerald-500/60 rounded-lg text-[8px] font-black uppercase tracking-widest">
                                GENERADO
                            </span>
                            <span *ngIf="!p.compromisoGenerado" class="inline-flex items-center gap-2 px-3 py-1 bg-rose-500/5 border border-rose-500/10 text-rose-500/60 rounded-lg text-[8px] font-black uppercase tracking-widest">
                                PENDIENTE
                            </span>
                        </td>
                        <td class="px-6 py-4 text-right">
                            <span class="text-sm font-black font-mono tracking-tighter text-emerald-400">$ {{ p.montoTotalBase | number:'1.2-2' }}</span>
                        </td>
                        <td class="px-6 py-4 text-center">
                            <div class="flex items-center justify-center gap-2">
                                <button (click)="openCompromiso(p, true)" 
                                    class="inline-flex items-center gap-2 px-4 py-2 bg-indigo-500/10 text-indigo-500 border border-indigo-500/20 rounded-xl text-[8px] font-black uppercase tracking-widest transition-all hover:bg-indigo-500 hover:text-white hover:scale-105 active:scale-95 group/btn shadow-lg shadow-indigo-500/5">
                                    Emitir Garantía
                                    <lucide-icon [name]="icons.Shield" class="w-3 h-3 group-hover/btn:scale-110 transition-transform"></lucide-icon>
                                </button>
                                
                                <button (click)="openCompromiso(p, false)" 
                                    [class]="p.compromisoGenerado ? 'bg-emerald-500 text-white' : 'bg-emerald-500/10 text-emerald-500 hover:bg-emerald-500 hover:text-white'"
                                    class="inline-flex items-center gap-2 px-4 py-2 border border-emerald-500/20 rounded-xl text-[8px] font-black uppercase tracking-widest transition-all hover:scale-105 active:scale-95 group/btn shadow-lg shadow-emerald-500/5">
                                    {{ p.compromisoGenerado ? 'Re-emitir Compromiso' : 'Generar Compromiso' }}
                                    <lucide-icon [name]="icons.Download" class="w-3 h-3 group-hover/btn:translate-y-0.5 transition-transform"></lucide-icon>
                                </button>
                            </div>
                        </td>
                    </tr>
                    <tr *ngIf="pacientes().length === 0">
                        <td colspan="5" class="px-6 py-16 text-center opacity-40">
                            <lucide-icon [name]="icons.Shield" class="w-8 h-8 text-slate-500 mx-auto mb-4"></lucide-icon>
                            <p class="text-[9px] font-black uppercase tracking-[0.3em] text-slate-500">No hay registros bajo este filtro</p>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>

    <!-- Modal Compromiso de Pago -->
    <div *ngIf="showModal()" class="fixed inset-0 z-[200] flex items-center justify-center p-4 bg-black/80 backdrop-blur-sm">
        <div class="bg-surface-card border border-white/5 rounded-[2.5rem] w-full max-w-2xl p-8 shadow-2xl relative overflow-hidden">
            <h2 class="text-2xl font-black text-white uppercase tracking-tighter mb-6">{{ isGarantia() ? 'Emitir Garantía de Pago' : 'Generar Compromiso de Pago' }}</h2>
            
            <div class="grid grid-cols-2 gap-4 mb-6">
                <div class="space-y-2">
                    <label class="text-[9px] font-black text-slate-500 uppercase tracking-widest">Nombre del Responsable / Garante</label>
                    <input type="text" [(ngModel)]="compromisoData.nombreResponsable" class="w-full bg-black/40 border border-white/5 p-4 rounded-xl text-white font-black text-xs outline-none">
                </div>
                <div class="space-y-2">
                    <label class="text-[9px] font-black text-slate-500 uppercase tracking-widest">Cédula del Responsable</label>
                    <input type="text" [(ngModel)]="compromisoData.cedulaResponsable" class="w-full bg-black/40 border border-white/5 p-4 rounded-xl text-white font-black text-xs outline-none">
                </div>
                <div class="space-y-2">
                    <label class="text-[9px] font-black text-slate-500 uppercase tracking-widest">Relación con Paciente</label>
                    <input type="text" [(ngModel)]="compromisoData.relacionResponsable" placeholder="Ej: Padre" class="w-full bg-black/40 border border-white/5 p-4 rounded-xl text-white font-black text-xs outline-none">
                </div>
                <div class="space-y-2">
                    <label class="text-[9px] font-black text-slate-500 uppercase tracking-widest">Teléfono Responsable</label>
                    <input type="text" [(ngModel)]="compromisoData.telefonoResponsable" class="w-full bg-black/40 border border-white/5 p-4 rounded-xl text-white font-black text-xs outline-none">
                </div>
                <div class="space-y-2 col-span-2">
                    <label class="text-[9px] font-black text-slate-500 uppercase tracking-widest">Dirección Responsable</label>
                    <input type="text" [(ngModel)]="compromisoData.direccionResponsable" class="w-full bg-black/40 border border-white/5 p-4 rounded-xl text-white font-black text-xs outline-none">
                </div>
                <div class="space-y-2">
                    <label class="text-[9px] font-black text-slate-500 uppercase tracking-widest">Días para Liquidar</label>
                    <input type="number" [(ngModel)]="compromisoData.diasLiquidar" class="w-full bg-black/40 border border-white/5 p-4 rounded-xl text-white font-black text-xs outline-none">
                </div>
                <div class="space-y-2">
                    <label class="text-[9px] font-black text-slate-500 uppercase tracking-widest">Cuotas</label>
                    <input type="number" [(ngModel)]="compromisoData.cuotas" class="w-full bg-black/40 border border-white/5 p-4 rounded-xl text-white font-black text-xs outline-none">
                </div>
            </div>

            <div class="flex justify-end gap-4">
                <button (click)="showModal.set(false)" class="px-6 py-4 rounded-xl border border-white/10 text-white font-black text-[10px] uppercase tracking-widest hover:bg-white/5 transition-all">Cancelar</button>
                <button (click)="generarPdf()" [class]="isGarantia() ? 'bg-indigo-500' : 'bg-emerald-500'" class="px-6 py-4 rounded-xl text-white font-black text-[10px] uppercase tracking-widest transition-all flex items-center shadow-lg shadow-emerald-500/20" [disabled]="isGenerating()">
                    <lucide-icon [name]="icons.Download" class="w-4 h-4 mr-2"></lucide-icon> {{ isGenerating() ? 'Generando...' : (isGarantia() ? 'Descargar Garantía' : 'Descargar Compromiso') }}
                </button>
            </div>
        </div>
    </div>
</div>
  `
})
export class SegurosDashboardComponent implements OnInit {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api/Seguros`;

  public icons = { Shield, Download, Calendar, Search, RefreshCcw, ChevronRight, Check };
  public isLoading = signal(false);
  public pacientes = signal<any[]>([]);
  public fechaDesde = signal<string>(new Date().toISOString().split('T')[0]);
  public fechaHasta = signal<string>(new Date().toISOString().split('T')[0]);
  public nombreFiltro = signal<string>('');
  public estadoFiltro = signal<string>('Todos'); // Todos, Pendiente, Generado
  
  public showModal = signal(false);
  public isGarantia = signal(false);
  public isGenerating = signal(false);
  public compromisoData: any = {};

  ngOnInit() {
    this.loadPacientes();
  }

  loadPacientes() {
    this.isLoading.set(true);
    let url = `${this.apiUrl}/ingresados?desde=${this.fechaDesde()}&hasta=${this.fechaHasta()}`;
    
    if (this.nombreFiltro()) {
      url += `&nombre=${this.nombreFiltro()}`;
    }
    
    if (this.estadoFiltro() === 'Pendiente') {
      url += `&conCompromiso=false`;
    } else if (this.estadoFiltro() === 'Generado') {
      url += `&conCompromiso=true`;
    }

    this.http.get<any[]>(url).subscribe({
      next: (data) => {
        this.pacientes.set(data);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  openCompromiso(paciente: any, garantia: boolean = false) {
    const today = new Date();
    const futureDate = new Date();
    futureDate.setDate(futureDate.getDate() + 30); // Por defecto 30 días

    this.isGarantia.set(garantia);
    this.compromisoData = {
      cuentaPorCobrarId: paciente.id,
      nombreResponsable: paciente.pacienteNombre,
      relacionResponsable: 'Titular',
      cedulaResponsable: paciente.pacienteCedula,
      direccionResponsable: 'No especificada',
      telefonoResponsable: 'No especificado',
      conceptos: 'Servicios Médicos Hospitalarios',
      nombrePaciente: paciente.pacienteNombre,
      edadPaciente: 0,
      cedulaPaciente: paciente.pacienteCedula,
      montoTotal: paciente.montoTotalBase,
      diasLiquidar: 30,
      cuotas: 1,
      fechaCompromiso: today.toISOString().split('T')[0],
      fechaVencimiento: futureDate.toISOString().split('T')[0]
    };
    this.showModal.set(true);
  }

  generarPdf() {
    this.isGenerating.set(true);
    
    // Update future date based on diasLiquidar
    const futureDate = new Date(this.compromisoData.fechaCompromiso);
    futureDate.setDate(futureDate.getDate() + this.compromisoData.diasLiquidar);
    this.compromisoData.fechaVencimiento = futureDate.toISOString();

    const endpoint = this.isGarantia() ? 'garantia-pago' : 'compromiso-pago';

    this.http.post(`${this.apiUrl}/${endpoint}`, this.compromisoData, { responseType: 'blob' }).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        window.open(url, '_blank');
        this.isGenerating.set(false);
        this.showModal.set(false);
        this.loadPacientes(); // Refresh list to update status if needed
      },
      error: () => {
        alert('Error al generar PDF');
        this.isGenerating.set(false);
      }
    });
  }
}
