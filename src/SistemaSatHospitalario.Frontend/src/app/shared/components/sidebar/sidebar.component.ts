import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterLinkActive } from '@angular/router';
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
    Package
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
      <nav class="flex-1 px-4 space-y-1 mt-4">
        <div class="text-[10px] font-black text-muted uppercase tracking-[0.2em] px-3 mb-2 opacity-60">Menu Principal</div>
        
        <a routerLink="/dashboard" routerLinkActive="active-link" class="nav-item">
          <lucide-icon [name]="icons.Dashboard" class="w-5 h-5 mr-3"></lucide-icon>
          Dashboard
        </a>

        <a routerLink="/facturacion" routerLinkActive="active-link" class="nav-item">
          <lucide-icon [name]="icons.Billing" class="w-5 h-5 mr-3"></lucide-icon>
          Facturación
        </a>

        <a routerLink="/expedientes" routerLinkActive="active-link" class="nav-item">
          <lucide-icon [name]="icons.History" class="w-5 h-5 mr-3"></lucide-icon>
          Expedientes
        </a>

        <!-- Admin & Specialized Sections -->
        <div *ngIf="isAdmin() || isRxAssistant()">
          <div class="text-[10px] font-black text-muted uppercase tracking-[0.2em] px-3 mt-6 mb-2 opacity-60">Operativo / Gestión</div>
          
          <a *ngIf="isAdmin()" routerLink="/cajas" routerLinkActive="active-link" class="nav-item">
            <lucide-icon [name]="icons.Cajas" class="w-5 h-5 mr-3"></lucide-icon>
            Gestión de Cajas
          </a>

          <a *ngIf="isAdmin()" routerLink="/catalog" routerLinkActive="active-link" class="nav-item">
            <lucide-icon [name]="icons.Catalog" class="w-5 h-5 mr-3"></lucide-icon>
            Catálogo de Servicios
          </a>

          <a *ngIf="isAdmin()" routerLink="/cxc" routerLinkActive="active-link" class="nav-item">
            <lucide-icon [name]="icons.AR" class="w-5 h-5 mr-3"></lucide-icon>
            Cuentas por Cobrar
          </a>

          <a *ngIf="canSeeOrders()" routerLink="/rx-orders" routerLinkActive="active-link" class="nav-item">
            <lucide-icon [name]="icons.Orders" class="w-5 h-5 mr-3"></lucide-icon>
            Ordenes Medicas
          </a>

          <a *ngIf="isAdmin()" routerLink="/settings" routerLinkActive="active-link" class="nav-item">
            <lucide-icon [name]="icons.Settings" class="w-5 h-5 mr-3"></lucide-icon>
            Configuración
          </a>
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
          <button (click)="auth.logout()" class="p-2 text-muted hover:text-rose-400 transition-colors">
            <lucide-icon [name]="icons.Logout" class="w-5 h-5"></lucide-icon>
          </button>
        </div>
      </div>
    </div>
  `,
    styles: [`
    .glass-sidebar {
      background: rgba(10, 15, 25, 0.4);
      backdrop-filter: blur(20px);
    }
    .nav-item {
      display: flex;
      items: center;
      padding: 0.75rem 0.75rem;
      border-radius: 0.75rem;
      color: var(--text-muted);
      font-size: 0.8125rem;
      font-weight: 600;
      text-transform: uppercase;
      letter-spacing: 0.025em;
      transition: all 0.2s cubic-bezier(0.4, 0, 0.2, 1);
    }
    .nav-item:hover {
      background: rgba(255, 255, 255, 0.05);
      color: var(--text-main);
      transform: translateX(4px);
    }
    .active-link {
      background: var(--primary) !important;
      color: white !important;
      font-weight: 800;
      box-shadow: 0 4px 12px rgba(0, 0, 0, 0.2);
    }
    :host ::ng-deep lucide-icon svg {
      stroke-width: 2px;
    }
  `]
})
export class SidebarComponent {
    auth = inject(AuthService);

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
        Catalog: Package
    };

    isAdmin(): boolean {
        return this.auth.currentUser()?.role === 'Administrador';
    }

    isRxAssistant(): boolean {
        return this.auth.currentUser()?.role === 'Asistente Rx';
    }

    canSeeAdminSection(): boolean {
        return this.isAdmin();
    }

    canSeeOrders(): boolean {
        return this.isAdmin() || this.isRxAssistant();
    }
}
