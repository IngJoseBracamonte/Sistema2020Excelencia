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
      <div class="flex items-center justify-between bg-white/60 backdrop-blur-2xl p-8 rounded-[2.5rem] shadow-sm border border-slate-100/50">
        <div class="flex items-center space-x-6">
          <div class="h-16 w-16 bg-slate-800 rounded-3xl shadow-xl flex items-center justify-center text-white">
            <lucide-icon [name]="'Settings'" class="w-8 h-8"></lucide-icon>
          </div>
          <div>
            <h1 class="text-3xl font-black text-slate-800 tracking-tighter uppercase">Configuración del Sistema</h1>
            <p class="text-slate-400 text-xs font-bold uppercase tracking-widest mt-1">Ajustes Globales y Parámetros del Establecimiento</p>
          </div>
        </div>
        <button (click)="saveSettings()" class="flex items-center space-x-2 bg-blue-600 hover:bg-blue-700 text-white px-6 py-3 rounded-2xl shadow-lg transition-all active:scale-95">
          <lucide-icon [name]="'Save'" class="w-4 h-4"></lucide-icon>
          <span class="text-sm font-black uppercase tracking-widest">Guardar Cambios</span>
        </button>
      </div>

      <div class="grid grid-cols-1 lg:grid-cols-3 gap-8">
        <!-- Columna de Navegación de Ajustes -->
        <div class="lg:col-span-1 space-y-4">
          <div class="bg-white rounded-[2rem] p-4 shadow-sm border border-slate-100 italic text-xs font-bold text-slate-400 text-center uppercase tracking-widest">
            Secciones de Ajustes
          </div>
          <button class="w-full text-left p-6 rounded-3xl bg-blue-50 border border-blue-100 text-blue-700 flex items-center space-x-4">
            <lucide-icon [name]="'Database'" class="w-5 h-5"></lucide-icon>
            <span class="font-black text-sm uppercase tracking-tight">General & Moneda</span>
          </button>
        </div>

        <!-- Formulario de Ajustes -->
        <div class="lg:col-span-2 bg-white rounded-[2.5rem] shadow-xl border border-slate-100 p-10">
          <div class="space-y-10">
            <!-- Sección: Moneda y Tasa -->
            <div>
              <h3 class="text-xs font-black text-slate-400 uppercase tracking-[0.2em] mb-6 border-b border-slate-50 pb-4">Parámetros Financieros</h3>
              <div class="grid grid-cols-1 md:grid-cols-2 gap-8">
                <div class="space-y-2">
                  <label class="text-[10px] font-black text-slate-500 uppercase tracking-widest ml-1">Tasa de Cambio del Día (Bs/$)</label>
                  <div class="relative">
                    <input type="number" [(ngModel)]="tasaCambio" class="w-full bg-slate-50 border border-slate-100 p-4 rounded-2xl focus:ring-4 focus:ring-blue-100 transition-all outline-none font-bold text-slate-700" placeholder="0.00">
                    <div class="absolute right-4 top-1/2 -translate-y-1/2">
                       <lucide-icon [name]="'RefreshCw'" class="w-4 h-4 text-slate-300"></lucide-icon>
                    </div>
                  </div>
                </div>
                <div class="space-y-2">
                  <label class="text-[10px] font-black text-slate-500 uppercase tracking-widest ml-1">Impuesto (IVA %)</label>
                  <input type="number" [(ngModel)]="iva" class="w-full bg-slate-50 border border-slate-100 p-4 rounded-2xl focus:ring-4 focus:ring-blue-100 transition-all outline-none font-bold text-slate-700" placeholder="16">
                </div>
              </div>
            </div>

            <!-- Sección: Establecimiento -->
            <div>
              <h3 class="text-xs font-black text-slate-400 uppercase tracking-[0.2em] mb-6 border-b border-slate-50 pb-4">Datos del Establecimiento</h3>
              <div class="space-y-6">
                <div class="space-y-2">
                  <label class="text-[10px] font-black text-slate-500 uppercase tracking-widest ml-1">Nombre Comercial</label>
                  <input type="text" [(ngModel)]="nombreHosp" class="w-full bg-slate-50 border border-slate-100 p-4 rounded-2xl focus:ring-4 focus:ring-blue-100 transition-all outline-none font-bold text-slate-700">
                </div>
                <div class="space-y-2">
                  <label class="text-[10px] font-black text-slate-500 uppercase tracking-widest ml-1">RIF / Identificación Fiscal</label>
                  <input type="text" [(ngModel)]="rif" class="w-full bg-slate-50 border border-slate-100 p-4 rounded-2xl focus:ring-4 focus:ring-blue-100 transition-all outline-none font-bold text-slate-700">
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
      animation: fadeIn 0.8s cubic-bezier(0.4, 0, 0.2, 1);
    }
    @keyframes fadeIn {
      from { opacity: 0; transform: translateY(10px); }
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
