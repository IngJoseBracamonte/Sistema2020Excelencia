import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LucideAngularModule, Settings, Save, RefreshCw, Database } from 'lucide-angular';

@Component({
  selector: 'app-system-settings',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule],
  template: `
    <div class="p-8 space-y-8 animate-fade-in relative z-10">
      <!-- Header Section (V2.0 Midnight Blue Legítimo) -->
      <div class="flex flex-col md:flex-row md:items-center justify-between bg-surface-card backdrop-blur-3xl p-8 rounded-[3rem] border border-white/5 relative overflow-hidden shadow-2xl">
        <div class="absolute top-0 right-0 w-64 h-64 bg-rose-500/5 rounded-full -z-10 blur-3xl"></div>
        <div class="flex items-center space-x-6">
          <div class="h-16 w-16 bg-rose-500/10 text-rose-500 border border-rose-500/20 rounded-2xl flex items-center justify-center shadow-lg shadow-rose-500/5">
            <lucide-icon [name]="icons.Settings" class="w-8 h-8"></lucide-icon>
          </div>
          <div>
            <h1 class="text-3xl font-black text-white tracking-tighter uppercase">Configuración del Sistema</h1>
            <p class="text-slate-500 text-[10px] font-black uppercase tracking-[0.2em] mt-1 italic">Parámetros Maestros & Establecimiento</p>
          </div>
        </div>
        <button (click)="saveSettings()" class="mt-4 md:mt-0 bg-rose-500 hover:bg-rose-600 text-white px-8 py-4 rounded-2xl text-[10px] font-black uppercase tracking-widest transition-all shadow-xl shadow-rose-500/20 flex items-center group active:scale-95">
          <lucide-icon [name]="icons.Save" class="w-4 h-4 mr-3"></lucide-icon>
          Guardar Cambios
        </button>
      </div>

      <div class="grid grid-cols-1 lg:grid-cols-4 gap-8">
        <!-- Navigation Menu (V2.0 Card Style) -->
        <div class="lg:col-span-1 space-y-4">
          <div class="bg-surface-card p-4 rounded-[1.5rem] border border-white/5 italic text-[8px] font-black text-slate-500 text-center uppercase tracking-widest">
            Secciones de Ajustes
          </div>
          <button class="w-full text-left p-6 rounded-[1.5rem] bg-rose-500/10 border border-rose-500/20 text-rose-500 flex items-center space-x-4 shadow-lg shadow-rose-500/5 transition-all">
            <lucide-icon [name]="icons.Database" class="w-5 h-5"></lucide-icon>
            <span class="font-black text-xs uppercase tracking-tight">General & Moneda</span>
          </button>
        </div>

        <!-- Form Context (Premium Card) -->
        <div class="lg:col-span-3 bg-surface-card rounded-[2.5rem] shadow-2xl border border-white/5 p-10 relative overflow-hidden">
          <div class="space-y-12">
            <!-- Section: Currency & Tax -->
            <div>
              <h3 class="text-[10px] font-black text-slate-500 uppercase tracking-[0.3em] mb-8 flex items-center">
                <span class="h-1.5 w-1.5 rounded-full bg-rose-500 mr-3 animate-pulse"></span>
                Parámetros Financieros
              </h3>
              <div class="grid grid-cols-1 md:grid-cols-2 gap-8">
                <div class="space-y-3">
                  <label class="text-[9px] font-black text-slate-500 uppercase tracking-widest ml-1">Tasa de Cambio del Día (BS/$)</label>
                  <div class="relative">
                    <input type="number" [(ngModel)]="tasaCambio" class="w-full bg-black/20 border border-white/5 p-5 rounded-2xl focus:ring-4 focus:ring-rose-500/10 focus:border-rose-500 transition-all outline-none font-black text-white text-sm" placeholder="0.00">
                    <div class="absolute right-5 top-1/2 -translate-y-1/2">
                       <lucide-icon [name]="icons.RefreshCw" class="w-4 h-4 text-slate-600"></lucide-icon>
                    </div>
                  </div>
                </div>
                <div class="space-y-3">
                  <label class="text-[9px] font-black text-slate-500 uppercase tracking-widest ml-1">Impuesto (IVA %)</label>
                  <input type="number" [(ngModel)]="iva" class="w-full bg-black/20 border border-white/5 p-5 rounded-2xl focus:ring-4 focus:ring-rose-500/10 focus:border-rose-500 transition-all outline-none font-black text-white text-sm" placeholder="16">
                </div>
              </div>
            </div>

            <!-- Section: Establishment -->
            <div>
              <h3 class="text-[10px] font-black text-slate-500 uppercase tracking-[0.3em] mb-8 flex items-center">
                <span class="h-1.5 w-1.5 rounded-full bg-emerald-500 mr-3 animate-pulse"></span>
                Datos del Establecimiento
              </h3>
              <div class="space-y-8">
                <div class="space-y-3">
                  <label class="text-[9px] font-black text-slate-500 uppercase tracking-widest ml-1">Nombre Comercial</label>
                  <input type="text" [(ngModel)]="nombreHosp" class="w-full bg-black/20 border border-white/5 p-5 rounded-2xl focus:ring-4 focus:ring-rose-500/10 focus:border-rose-500 transition-all outline-none font-black text-white text-sm uppercase">
                </div>
                <div class="space-y-3">
                  <label class="text-[9px] font-black text-slate-500 uppercase tracking-widest ml-1">RIF / Identificación Fiscal</label>
                  <input type="text" [(ngModel)]="rif" class="w-full bg-black/20 border border-white/5 p-5 rounded-2xl focus:ring-4 focus:ring-rose-500/10 focus:border-rose-500 transition-all outline-none font-black text-white text-sm uppercase">
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .animate-fade-in {
      animation: fadeIn 0.6s cubic-bezier(0.4, 0, 0.2, 1);
    }
    @keyframes fadeIn {
      from { opacity: 0; transform: translateY(15px); }
      to { opacity: 1; transform: translateY(0); }
    }
  `]
})
export class SystemSettingsComponent {
  public tasaCambio = signal<number>(36.50);
  public iva = signal<number>(16);
  public nombreHosp = signal<string>('SAT HOSPITALARIO - EXCELENCIA');
  public rif = signal<string>('J-12345678-9');

  public readonly icons = {
    Settings,
    Save,
    RefreshCw,
    Database
  };

  saveSettings() {
    alert('Configuración guardada exitosamente en la base de datos maestra.');
    // En una fase posterior llamaríamos al SettingsService
  }
}
