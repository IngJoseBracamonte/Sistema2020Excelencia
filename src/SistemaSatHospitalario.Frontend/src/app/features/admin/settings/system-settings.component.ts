import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { LucideAngularModule, Settings, Save, RefreshCw, Database, Users, Shield, Plus, Trash2, Edit } from 'lucide-angular';
import { SettingsService } from '../../../core/services/settings.service';
import { ConveniosService } from '../../../core/services/convenios.service';
import { ConfiguracionGeneral, UserDto } from '../../../core/models/settings.model';
import { SeguroConvenio } from '../../../core/models/convenio.model';

@Component({
  selector: 'app-system-settings',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule],
  template: `
    <div class="p-8 space-y-8 animate-fade-in relative z-10">
      <!-- Header Area -->
      <div class="flex flex-col md:flex-row md:items-center justify-between bg-surface-card backdrop-blur-3xl p-10 rounded-[4rem] border border-white/5 relative overflow-hidden shadow-2xl">
        <div class="absolute top-0 right-0 w-96 h-96 bg-rose-500/10 rounded-full -z-10 blur-[100px] animate-pulse"></div>
        <div class="flex items-center space-x-8">
          <div class="h-20 w-20 bg-rose-500/10 text-rose-500 border border-rose-500/20 rounded-[2rem] flex items-center justify-center shadow-2xl shadow-rose-500/10 ring-1 ring-white/10">
            <lucide-icon [name]="icons.Settings" class="w-10 h-10"></lucide-icon>
          </div>
          <div>
            <h1 class="text-4xl font-black text-white tracking-tighter uppercase leading-none">Ajustes Maestros</h1>
            <p class="text-slate-500 text-[10px] font-black uppercase tracking-[0.4em] mt-3 italic opacity-60">Hospital Pachón Pro Ecosystem • v2.5</p>
          </div>
        </div>
      </div>

      <div class="grid grid-cols-1 lg:grid-cols-4 gap-10">
        <!-- Sidebar Navigation -->
        <div class="lg:col-span-1 space-y-4">
          <div class="bg-surface-card/50 p-4 rounded-[2rem] border border-white/5 text-center">
             <span class="text-[8px] font-black text-slate-600 uppercase tracking-widest italic">Categorías de Control</span>
          </div>
          
          <button 
            (click)="activeTab = 'general'" 
            [ngClass]="activeTab === 'general' ? 'bg-rose-500/10 border-rose-500/50' : 'border-white/5'"
            class="w-full text-left p-8 rounded-[2rem] border flex items-center space-x-5 transition-all hover:bg-white/5 group relative overflow-hidden">
            <div *ngIf="activeTab === 'general'" class="absolute inset-y-0 left-0 w-1 bg-rose-500 rounded-full"></div>
            <lucide-icon [name]="icons.Database" class="w-6 h-6 text-rose-500"></lucide-icon>
            <span class="font-black text-xs uppercase tracking-tight text-slate-300">General & Finanzas</span>
          </button>
          
          <button 
            (click)="activeTab = 'convenios'" 
            [ngClass]="activeTab === 'convenios' ? 'bg-emerald-500/10 border-emerald-500/50' : 'border-white/5'"
            class="w-full text-left p-8 rounded-[2rem] border flex items-center space-x-5 transition-all hover:bg-white/5 group relative overflow-hidden">
            <div *ngIf="activeTab === 'convenios'" class="absolute inset-y-0 left-0 w-1 bg-emerald-500 rounded-full"></div>
            <lucide-icon [name]="icons.Shield" class="w-6 h-6 text-emerald-500"></lucide-icon>
            <span class="font-black text-xs uppercase tracking-tight text-slate-300">Convenios CRUD</span>
          </button>

          <button 
            (click)="activeTab = 'usuarios'" 
            [ngClass]="activeTab === 'usuarios' ? 'bg-blue-500/10 border-blue-500/50' : 'border-white/5'"
            class="w-full text-left p-8 rounded-[2rem] border flex items-center space-x-5 transition-all hover:bg-white/5 group relative overflow-hidden">
            <div *ngIf="activeTab === 'usuarios'" class="absolute inset-y-0 left-0 w-1 bg-blue-500 rounded-full"></div>
            <lucide-icon [name]="icons.Users" class="w-6 h-6 text-blue-500"></lucide-icon>
            <span class="font-black text-xs uppercase tracking-tight text-slate-300">Accesos & Roles</span>
          </button>
        </div>

        <!-- Content Area -->
        <div class="lg:col-span-3 bg-surface-card rounded-[3rem] shadow-2xl border border-white/5 p-12 min-h-[700px] relative overflow-hidden">
          
          <!-- TAB: GENERAL -->
          <div *ngIf="activeTab === 'general'" class="space-y-12 animate-in fade-in slide-in-from-top-4 duration-700">
            <div class="flex justify-between items-end border-b border-white/5 pb-8">
               <div>
                 <h2 class="text-2xl font-black text-white uppercase tracking-tighter">Identidad & Moneda</h2>
                 <p class="text-[10px] text-slate-500 font-bold uppercase tracking-widest mt-1">Configuración base de la institución</p>
               </div>
               <button 
                 (click)="saveGeneral()" 
                 class="bg-rose-500 hover:bg-rose-600 text-white px-10 py-5 rounded-2xl text-[10px] font-black uppercase tracking-widest shadow-2xl shadow-rose-500/20 active:scale-95 transition-all flex items-center">
                 <lucide-icon [name]="icons.Save" class="w-4 h-4 mr-3"></lucide-icon> Guardar Cambios
               </button>
            </div>
            
            <div class="grid grid-cols-1 md:grid-cols-2 gap-10">
              <div class="space-y-4">
                <label class="text-[10px] font-black text-slate-500 uppercase tracking-widest ml-2">Nombre de la Empresa</label>
                <input type="text" [(ngModel)]="configData.nombreEmpresa" class="w-full bg-black/30 border border-white/10 p-6 rounded-2xl text-white font-black text-sm focus:border-rose-500 focus:ring-4 focus:ring-rose-500/5 transition-all outline-none uppercase">
              </div>
              <div class="space-y-4">
                <label class="text-[10px] font-black text-slate-500 uppercase tracking-widest ml-2">RIF / Identificación</label>
                <input type="text" [(ngModel)]="configData.rif" class="w-full bg-black/30 border border-white/10 p-6 rounded-2xl text-white font-black text-sm focus:border-rose-500 focus:ring-4 focus:ring-rose-500/5 transition-all outline-none uppercase">
              </div>
              <div class="space-y-4">
                <label class="text-[10px] font-black text-slate-500 uppercase tracking-widest ml-2">Tasa de Cambio Oficial</label>
                <div class="relative">
                   <input type="number" [(ngModel)]="nuevaTasa" class="w-full bg-black/30 border border-white/10 p-6 rounded-2xl text-white font-black text-sm focus:border-rose-500 transition-all outline-none">
                   <span class="absolute right-6 top-1/2 -translate-y-1/2 text-slate-600 font-black text-[10px] uppercase">BS / $</span>
                </div>
              </div>
              <div class="space-y-4">
                <label class="text-[10px] font-black text-slate-500 uppercase tracking-widest ml-2">IVA Aplicable (%)</label>
                <input type="number" [(ngModel)]="configData.iva" class="w-full bg-black/30 border border-white/10 p-6 rounded-2xl text-white font-black text-sm focus:border-rose-500 transition-all outline-none">
              </div>
            </div>
          </div>

          <!-- TAB: CONVENIOS -->
          <div *ngIf="activeTab === 'convenios'" class="space-y-10 animate-in fade-in slide-in-from-right-4 duration-700">
             <div class="flex justify-between items-end border-b border-white/5 pb-8">
                <div>
                  <h2 class="text-2xl font-black text-white uppercase tracking-tighter">Gestión de Convenios</h2>
                  <p class="text-[10px] text-slate-500 font-bold uppercase tracking-widest mt-1">Seguros y Acuerdos Comerciales</p>
                </div>
                <button 
                  (click)="showConvenioModal = true" 
                  class="bg-emerald-500 hover:bg-emerald-600 text-white px-8 py-4 rounded-2xl text-[10px] font-black uppercase tracking-widest shadow-2xl shadow-emerald-500/20 active:scale-95 transition-all flex items-center">
                  <lucide-icon [name]="icons.Plus" class="w-4 h-4 mr-3"></lucide-icon> Nuevo Convenio
                </button>
             </div>

             <div class="grid grid-cols-1 gap-4">
                <div *ngFor="let c of convenios()" class="bg-black/20 p-8 rounded-[2rem] border border-white/5 hover:border-emerald-500/30 transition-all flex items-center justify-between group">
                  <div class="flex items-center space-x-6">
                    <div class="h-14 w-14 bg-emerald-500/10 text-emerald-500 rounded-2xl flex items-center justify-center font-black text-lg border border-emerald-500/20">
                      {{c.nombre.substring(0,1)}}
                    </div>
                    <div>
                      <h4 class="text-sm font-black text-white uppercase">{{c.nombre}}</h4>
                      <p class="text-[9px] text-slate-500 font-bold uppercase tracking-tighter mt-1">ID Legado: <span class="text-rose-500">{{c.id}}</span> • Cobertura: <span class="text-emerald-500">{{c.porcentajeCobertura}}%</span></p>
                    </div>
                  </div>
                  <div class="flex items-center space-x-3 opacity-0 group-hover:opacity-100 transition-opacity">
                    <button (click)="deleteConvenio(c.id)" class="h-10 w-10 bg-rose-500/10 text-rose-500 rounded-xl flex items-center justify-center hover:bg-rose-500 hover:text-white transition-all">
                      <lucide-icon [name]="icons.Trash2" class="w-4 h-4"></lucide-icon>
                    </button>
                  </div>
                </div>
             </div>
          </div>

          <!-- TAB: ACCESOS -->
          <div *ngIf="activeTab === 'usuarios'" class="space-y-10 animate-in fade-in slide-in-from-bottom-4 duration-700">
             <div class="flex justify-between items-end border-b border-white/5 pb-8">
                <div>
                  <h2 class="text-2xl font-black text-white uppercase tracking-tighter">Accesos del Sistema</h2>
                  <p class="text-[10px] text-slate-500 font-bold uppercase tracking-widest mt-1">Control de Usuarios y Roles de Seguridad</p>
                </div>
                <button 
                  (click)="showUserModal = true" 
                  class="bg-blue-500 hover:bg-blue-600 text-white px-8 py-4 rounded-2xl text-[10px] font-black uppercase tracking-widest shadow-2xl shadow-blue-500/20 active:scale-95 transition-all flex items-center">
                  <lucide-icon [name]="icons.Plus" class="w-4 h-4 mr-3"></lucide-icon> Nuevo Usuario
                </button>
             </div>

             <div class="grid grid-cols-1 md:grid-cols-2 gap-8">
                <div *ngFor="let u of users()" class="bg-white/5 p-8 rounded-[2.5rem] border border-white/5 hover:bg-white/10 transition-all flex flex-col space-y-6">
                  <div class="flex items-center space-x-5">
                    <div class="h-14 w-14 bg-gradient-to-br from-blue-500 to-indigo-600 text-white rounded-full flex items-center justify-center font-black text-xl shadow-xl shadow-blue-500/20">
                      {{u.username.substring(0,1)}}
                    </div>
                    <div>
                      <h4 class="text-sm font-black text-white uppercase tracking-tight">{{u.username}}</h4>
                      <p class="text-[9px] text-slate-500 font-bold uppercase tracking-widest">{{u.email}}</p>
                    </div>
                  </div>
                  <div class="flex flex-wrap gap-2">
                    <span *ngFor="let r of u.roles" class="text-[8px] font-black bg-blue-500/10 text-blue-400 px-3 py-1.5 rounded-lg uppercase border border-blue-500/20 tracking-tighter">
                      {{r}}
                    </span>
                  </div>
                </div>
             </div>
          </div>

        </div>
      </div>
    </div>

    <!-- Modal Nuevo Convenio -->
    <div *ngIf="showConvenioModal" class="fixed inset-0 bg-black/95 backdrop-blur-xl z-[100] flex items-center justify-center p-8 animate-in fade-in duration-300">
       <div class="bg-surface-card border border-white/10 p-12 rounded-[4rem] w-full max-w-xl space-y-10 shadow-3xl animate-in zoom-in-95 duration-500 relative overflow-hidden">
         <div class="absolute -top-20 -left-20 w-64 h-64 bg-emerald-500/10 rounded-full blur-[80px]"></div>
         <div>
           <h2 class="text-3xl font-black text-white uppercase tracking-tighter">Nuevo Convenio</h2>
           <p class="text-slate-500 text-[10px] font-black uppercase tracking-widest mt-2">Sincronización Legacy Pachón Pro</p>
         </div>
         
         <div class="space-y-6">
           <div class="space-y-2">
             <label class="text-[9px] font-black text-slate-600 uppercase tracking-widest ml-1">ID Único</label>
             <input type="number" [(ngModel)]="newC.id" class="w-full bg-black/40 border border-white/5 p-5 rounded-2xl text-white outline-none focus:border-emerald-500 transition-all font-black">
           </div>
           <div class="space-y-2">
             <label class="text-[9px] font-black text-slate-600 uppercase tracking-widest ml-1">Nombre</label>
             <input type="text" [(ngModel)]="newC.nombre" class="w-full bg-black/40 border border-white/5 p-5 rounded-2xl text-white outline-none focus:border-emerald-500 transition-all font-black uppercase">
           </div>
           <div class="space-y-2">
             <label class="text-[9px] font-black text-slate-600 uppercase tracking-widest ml-1">Cobertura (%)</label>
             <input type="number" [(ngModel)]="newC.porcentajeCobertura" class="w-full bg-black/40 border border-white/5 p-5 rounded-2xl text-white outline-none focus:border-emerald-500 transition-all font-black">
           </div>
         </div>

         <div class="flex space-x-5">
           <button (click)="saveConvenio()" class="flex-1 bg-emerald-500 hover:bg-emerald-600 text-white font-black uppercase py-6 rounded-[2rem] shadow-2xl shadow-emerald-500/20 active:scale-95 transition-all text-xs">Crear</button>
           <button (click)="showConvenioModal = false" class="flex-1 bg-white/5 hover:bg-white/10 text-slate-400 font-black uppercase py-6 rounded-[2rem] active:scale-95 transition-all text-xs">Cancelar</button>
         </div>
       </div>
    </div>

    <!-- Modal Nuevo Usuario -->
    <div *ngIf="showUserModal" class="fixed inset-0 bg-black/95 backdrop-blur-xl z-[100] flex items-center justify-center p-8 animate-in fade-in duration-300">
       <div class="bg-surface-card border border-white/10 p-12 rounded-[4rem] w-full max-w-xl space-y-10 shadow-3xl animate-in zoom-in-95 duration-500 relative overflow-hidden">
         <div class="absolute -top-20 -left-20 w-64 h-64 bg-blue-500/10 rounded-full blur-[80px]"></div>
         <div>
           <h2 class="text-3xl font-black text-white uppercase tracking-tighter">Nuevo Usuario</h2>
           <p class="text-slate-500 text-[10px] font-black uppercase tracking-widest mt-2">Control de Identidad Pachón Pro</p>
         </div>
         
         <div class="space-y-6">
           <div class="grid grid-cols-2 gap-4">
               <div class="space-y-2">
                 <label class="text-[9px] font-black text-slate-600 uppercase tracking-widest ml-1">Username</label>
                 <input type="text" [(ngModel)]="newU.username" class="w-full bg-black/40 border border-white/5 p-5 rounded-2xl text-white outline-none focus:border-blue-500 transition-all font-black">
               </div>
               <div class="space-y-2">
                 <label class="text-[9px] font-black text-slate-600 uppercase tracking-widest ml-1">Email</label>
                 <input type="email" [(ngModel)]="newU.email" class="w-full bg-black/40 border border-white/5 p-5 rounded-2xl text-white outline-none focus:border-blue-500 transition-all font-black">
               </div>
           </div>
           <div class="space-y-2">
             <label class="text-[9px] font-black text-slate-600 uppercase tracking-widest ml-1">Password</label>
             <input type="password" [(ngModel)]="newU.password" class="w-full bg-black/40 border border-white/5 p-5 rounded-2xl text-white outline-none focus:border-blue-500 transition-all font-black">
           </div>
           <div class="space-y-2">
             <label class="text-[9px] font-black text-slate-600 uppercase tracking-widest ml-1">Roles Asignados</label>
             <div class="flex flex-wrap gap-2">
               <button *ngFor="let r of roles()" 
                 (click)="toggleRole(r)"
                 [ngClass]="newU.roles.includes(r) ? 'bg-blue-500 text-white' : 'bg-white/5 text-slate-400'"
                 class="px-4 py-2 rounded-xl text-[8px] font-black uppercase transition-all">
                 {{r}}
               </button>
             </div>
           </div>
         </div>

         <div class="flex space-x-5">
           <button (click)="saveUser()" class="flex-1 bg-blue-500 hover:bg-blue-600 text-white font-black uppercase py-6 rounded-[2rem] shadow-2xl shadow-blue-500/20 active:scale-95 transition-all text-xs">Registrar</button>
           <button (click)="showUserModal = false" class="flex-1 bg-white/5 hover:bg-white/10 text-slate-400 font-black uppercase py-6 rounded-[2rem] active:scale-95 transition-all text-xs">Cancelar</button>
         </div>
       </div>
    </div>
  `,
  styles: [`
    .animate-fade-in { animation: fadeIn 0.8s cubic-bezier(0.4, 0, 0.2, 1); }
    @keyframes fadeIn { from { opacity: 0; transform: translateY(25px); } to { opacity: 1; transform: translateY(0); } }
  `]
})
export class SystemSettingsComponent implements OnInit {
  private settingsService = inject(SettingsService);
  private conveniosService = inject(ConveniosService);
  private route = inject(ActivatedRoute);

  public activeTab = 'general';
  public configData: ConfiguracionGeneral = { nombreEmpresa: '', rif: '', iva: 16 };
  public nuevaTasa = 36.50;
  public convenios = signal<SeguroConvenio[]>([]);
  public users = signal<UserDto[]>([]);
  public roles = signal<string[]>([]);

  public showConvenioModal = false;
  public showUserModal = false;
  public newC: Partial<SeguroConvenio> = { id: 0, nombre: '', porcentajeCobertura: 100 };
  public newU: any = { username: '', email: '', password: '', roles: [] };

  public readonly icons = { Settings, Save, RefreshCw, Database, Users, Shield, Plus, Trash2, Edit };

  ngOnInit() { 
    this.loadData(); 
    this.route.queryParams.subscribe(params => {
      if (params['tab']) {
        this.activeTab = params['tab'];
      }
    });
  }

  loadData() {
    this.settingsService.getConfig().subscribe(data => { if (data) this.configData = data; });
    this.conveniosService.getAll().subscribe(data => this.convenios.set(data));
    this.settingsService.getUsers().subscribe(data => this.users.set(data));
    this.settingsService.getRoles().subscribe(data => this.roles.set(data));
  }

  saveGeneral() {
    this.settingsService.updateConfig(this.configData).subscribe(() => {
      this.settingsService.updateTasa(this.nuevaTasa).subscribe(() => {
        alert('Ajustes guardados Pachón Pro.');
      });
    });
  }

  saveConvenio() {
    this.conveniosService.create(this.newC).subscribe(() => {
      this.showConvenioModal = false;
      this.loadData();
      this.newC = { id: 0, nombre: '', porcentajeCobertura: 100 };
    });
  }

  saveUser() {
    this.settingsService.createUser(this.newU).subscribe(() => {
      this.showUserModal = false;
      this.loadData();
      this.newU = { username: '', email: '', password: '', roles: [] };
      alert('Usuario creado Pachón Pro.');
    });
  }

  toggleRole(role: string) {
    const idx = this.newU.roles.indexOf(role);
    if (idx > -1) {
      this.newU.roles.splice(idx, 1);
    } else {
      this.newU.roles.push(role);
    }
  }

  deleteConvenio(id: number) {
    if (confirm('¿Eliminar convenio?')) {
      this.conveniosService.delete(id).subscribe(() => this.loadData());
    }
  }
}
