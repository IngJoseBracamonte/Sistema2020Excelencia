import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterLinkActive, Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import {
    LucideAngularModule,
    LayoutDashboard,
    Files,
    Box,
    Users,
    LogOut,
    Settings,
    ClipboardList,
    CreditCard,
    FileText,
    Package,
    Stethoscope,
    Activity,
    Bookmark,
    ChevronDown,
    ShieldCheck,
    BarChart3
} from 'lucide-angular';

@Component({
    selector: 'app-sidebar',
    standalone: true,
    imports: [CommonModule, RouterLink, RouterLinkActive, LucideAngularModule],
    template: `
    <div class="h-screen w-64 glass-sidebar flex flex-col border-r border-glass-border">
      <!-- Logo Section -->
      <div class="p-6 flex items-center space-x-3">
        <div class="w-10 h-10 bg-primary rounded-xl flex items-center justify-center shadow-lg shadow-primary/20">
           <span class="text-white font-bold text-xl">S</span>
        </div>
        <span class="text-xl font-bold text-main tracking-tight uppercase">SAT <span class="text-primary-glow">Hosp</span></span>
      </div>

      <!-- Navigation -->
      <nav class="flex-1 px-4 space-y-1 mt-4 overflow-y-auto custom-scrollbar pb-10">
        <div class="text-[10px] font-black text-muted uppercase tracking-[0.2em] px-3 mb-2 opacity-60">Menu Principal</div>
        
        <a routerLink="/dashboard" routerLinkActive="active-link" class="nav-item">
          <lucide-icon [name]="icons.Dashboard" class="w-5 h-5 mr-3"></lucide-icon>
          Dashboard
        </a>

        <a routerLink="/facturacion" routerLinkActive="active-link" class="nav-item">
          <lucide-icon [name]="icons.Billing" class="w-5 h-5 mr-3"></lucide-icon>
          Facturación
        </a>



        <!-- Admin & Specialized Sections -->
        <div *ngIf="isAdmin() || isRxAssistant()">
          <div class="text-[10px] font-black text-muted uppercase tracking-[0.2em] px-3 mt-6 mb-2 opacity-60">Operativo / Gestión</div>
          
          <!-- Dropdown: Caja y Catálogos -->
          <div *ngIf="isAdmin()" class="space-y-1">
            <button (click)="toggleDropdown('caja')" 
                class="w-full flex items-center justify-between nav-item group"
                [ngClass]="{ 'bg-white/5': dropdownsOpen().caja }">
                <div class="flex items-center">
                    <lucide-icon [name]="icons.Cajas" class="w-5 h-5 mr-3 text-rose-500"></lucide-icon>
                    Caja y Catálogos
                </div>
                <lucide-icon [name]="icons.ChevronDown" class="w-4 h-4 transition-transform duration-300"
                    [class.rotate-180]="dropdownsOpen().caja"></lucide-icon>
            </button>
            <div *ngIf="dropdownsOpen().caja" class="pl-8 space-y-1 animate-fade-in">
                <a routerLink="/cajas" routerLinkActive="active-sublink" class="nav-subitem">Gestión de Cajas</a>
                <a routerLink="/catalog" [routerLinkActiveOptions]="{ exact: true }" routerLinkActive="active-sublink" class="nav-subitem">Catálogo General</a>
            </div>
          </div>

          <!-- Dropdown: Reportes Operativos (Fase 5 Master Move) -->
          <div *ngIf="isAdmin()" class="space-y-1">
            <button (click)="toggleDropdown('reportes')" 
                class="w-full flex items-center justify-between nav-item group"
                [ngClass]="{ 'bg-white/5': dropdownsOpen().reportes }">
                <div class="flex items-center">
                    <lucide-icon [name]="icons.Reportes" class="w-5 h-5 mr-3 text-amber-500"></lucide-icon>
                    Reportes Operativos
                </div>
                <lucide-icon [name]="icons.ChevronDown" class="w-4 h-4 transition-transform duration-300"
                    [class.rotate-180]="dropdownsOpen().reportes"></lucide-icon>
            </button>
            <div *ngIf="dropdownsOpen().reportes" class="pl-8 space-y-1 animate-fade-in">
                <a routerLink="/cxc" routerLinkActive="active-sublink" class="nav-subitem">Cuentas por Cobrar</a>
                <a routerLink="/admin/audit/cuentas" routerLinkActive="active-sublink" class="nav-subitem">Cuentas por Auditar</a>
                <a routerLink="/rx-orders" routerLinkActive="active-sublink" class="nav-subitem">Ordenes Medicas</a>
                <a routerLink="/expedientes" routerLinkActive="active-sublink" class="nav-subitem">Expedientes</a>
            </div>
          </div>

          <!-- Dropdown: Gestión Médica -->
          <div *ngIf="isAdmin()" class="space-y-1">
            <button (click)="toggleDropdown('medica')" 
                class="w-full flex items-center justify-between nav-item group"
                [ngClass]="{ 'bg-white/5': dropdownsOpen().medica }">
                <div class="flex items-center">
                    <lucide-icon [name]="icons.Doctor" class="w-5 h-5 mr-3 text-blue-500"></lucide-icon>
                    Gestión Médica
                </div>
                <lucide-icon [name]="icons.ChevronDown" class="w-4 h-4 transition-transform duration-300"
                    [class.rotate-180]="dropdownsOpen().medica"></lucide-icon>
            </button>
            <div *ngIf="dropdownsOpen().medica" class="pl-8 space-y-1 animate-fade-in">
                <a routerLink="/medicos" routerLinkActive="active-sublink" class="nav-subitem">Médicos Nominal</a>
                <a routerLink="/especialidades" routerLinkActive="active-sublink" class="nav-subitem">Especialidades</a>
                <a routerLink="/admin/reportes/calculo-honorarios" routerLinkActive="active-sublink" class="nav-subitem">Cálculo de Honorarios</a>
            </div>
          </div>

          <!-- Dropdown: Configuración -->
          <div *ngIf="isAdmin()" class="space-y-1">
            <button (click)="toggleDropdown('settings')" 
                class="w-full flex items-center justify-between nav-item group"
                [ngClass]="{ 'bg-white/5': dropdownsOpen().settings }">
                <div class="flex items-center">
                    <lucide-icon [name]="icons.Settings" class="w-5 h-5 mr-3 text-emerald-500"></lucide-icon>
                    Configuración
                </div>
                <lucide-icon [name]="icons.ChevronDown" class="w-4 h-4 transition-transform duration-300"
                    [class.rotate-180]="dropdownsOpen().settings"></lucide-icon>
            </button>
            <div *ngIf="dropdownsOpen().settings" class="pl-8 space-y-1 animate-fade-in">
                <a routerLink="/settings" [queryParams]="{tab: 'general'}" routerLinkActive="active-sublink" class="nav-subitem">General & Finanzas</a>
                <a routerLink="/settings" [queryParams]="{tab: 'convenios'}" routerLinkActive="active-sublink" class="nav-subitem">Convenios</a>
                <a routerLink="/settings" [queryParams]="{tab: 'usuarios'}" routerLinkActive="active-sublink" class="nav-subitem">Gestión de Usuario</a>
                <a routerLink="/settings" [queryParams]="{tab: 'citas'}" routerLinkActive="active-sublink" class="nav-subitem">Gestión de Citas</a>
            </div>
          </div>
        </div>
      </nav>

      <!-- User Profile -->
      <div class="p-4 border-t border-glass-border bg-surface/30">
        <div class="flex items-center p-2 rounded-lg">
          <div class="w-8 h-8 rounded-full bg-surface-card border border-glass-border flex items-center justify-center mr-3">
            <lucide-icon [name]="icons.User" class="w-4 h-4 text-muted"></lucide-icon>
          </div>
          <div class="flex-1 min-w-0">
            <p class="text-xs font-black text-main truncate">{{ auth.currentUser()?.username }}</p>
            <p class="text-[9px] font-bold text-muted truncate uppercase tracking-widest">{{ auth.currentUser()?.role }}</p>
          </div>
          <button (click)="auth.logout()" class="p-2 text-muted hover:text-primary transition-colors">
            <lucide-icon [name]="icons.Logout" class="w-5 h-5"></lucide-icon>
          </button>
        </div>
      </div>
    </div>
  `,
    styles: [`
    .glass-sidebar {
      background: rgba(10, 15, 26, 0.85);
      backdrop-filter: blur(30px);
    }
    .nav-item {
      display: flex;
      align-items: center;
      padding: 0.85rem 1rem;
      border-radius: 1rem;
      color: #94a3b8;
      font-size: 0.75rem;
      font-weight: 800;
      text-transform: uppercase;
      letter-spacing: 0.05em;
      transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
      border: 1px solid transparent;
    }
    .nav-item:hover {
      background: rgba(255, 255, 255, 0.03);
      color: white;
      border-color: rgba(255, 255, 255, 0.05);
    }
    .nav-subitem {
      display: block;
      padding: 0.6rem 0.75rem;
      border-radius: 0.75rem;
      color: #64748b;
      font-size: 0.7rem;
      font-weight: 700;
      text-transform: uppercase;
      letter-spacing: 0.025em;
      transition: all 0.2s;
    }
    .nav-subitem:hover {
      color: white;
      background: rgba(255, 255, 255, 0.02);
    }
    .active-link {
      background: rgba(225, 29, 72, 0.1) !important;
      color: #f43f5e !important;
      border-color: rgba(225, 29, 72, 0.2) !important;
      box-shadow: 0 4px 20px rgba(225, 29, 72, 0.1);
    }
    .active-sublink {
      color: #f43f5e !important;
      background: rgba(225, 29, 72, 0.05) !important;
    }
    @keyframes fadeIn {
        from { opacity: 0; transform: translateY(-4px); }
        to { opacity: 1; transform: translateY(0); }
    }
    .animate-fade-in {
        animation: fadeIn 0.3s ease-out forwards;
    }
  `]
})
export class SidebarComponent implements OnInit {
    auth = inject(AuthService);
    private router = inject(Router);

    public dropdownsOpen = signal({
        caja: false,
        reportes: false,
        medica: false,
        settings: false
    });

    ngOnInit() {
        const url = this.router.url;
        if (url.includes('/cajas') || url.includes('/catalog')) {
            this.dropdownsOpen.set({ ...this.dropdownsOpen(), caja: true });
        }
        if (url.includes('/admin/audit') || url.includes('/cxc') || url.includes('/rx-orders') || url.includes('/expedientes')) {
            this.dropdownsOpen.set({ ...this.dropdownsOpen(), reportes: true });
        }
        if (url.includes('/medicos') || url.includes('/especialidades')) {
            this.dropdownsOpen.set({ ...this.dropdownsOpen(), medica: true });
        }
        if (url.includes('/settings')) {
            this.dropdownsOpen.set({ ...this.dropdownsOpen(), settings: true });
        }
    }

    readonly icons = {
        Dashboard: LayoutDashboard,
        Billing: Files,
        Cajas: Box,
        Orders: ClipboardList,
        Settings: Settings,
        User: Users,
        Logout: LogOut,
        AR: CreditCard,
        History: FileText,
        Catalog: Package,
        Doctor: Stethoscope,
        RX: Activity,
        Category: Bookmark,
        ChevronDown: ChevronDown,
        Audit: ShieldCheck,
        Reportes: BarChart3
    };

    toggleDropdown(key: 'caja' | 'medica' | 'settings' | 'reportes') {
        this.dropdownsOpen.update(prev => ({
            ...prev,
            [key]: !prev[key as keyof typeof prev]
        }));
    }

    isAdmin(): boolean {
        const role = this.auth.currentUser()?.role?.toLowerCase();
        return role === 'administrador' || role === 'admin';
    }

    isRxAssistant(): boolean {
        return this.auth.currentUser()?.role?.toLowerCase().includes('rx') || false;
    }

    canSeeAdminSection(): boolean {
        return this.isAdmin();
    }

    canSeeOrders(): boolean {
        return this.isAdmin() || this.isRxAssistant();
    }
}
