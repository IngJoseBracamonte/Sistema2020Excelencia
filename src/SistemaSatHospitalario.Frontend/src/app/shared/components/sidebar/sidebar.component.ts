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
    ClipboardList
} from 'lucide-angular';

@Component({
    selector: 'app-sidebar',
    standalone: true,
    imports: [CommonModule, RouterLink, RouterLinkActive, LucideAngularModule],
    template: `
    <div class="h-screen w-64 glass-sidebar flex flex-col border-r border-slate-200/50">
      <!-- Logo Section -->
      <div class="p-6 flex items-center space-x-3">
        <div class="w-10 h-10 bg-blue-600 rounded-xl flex items-center justify-center shadow-lg shadow-blue-200">
           <span class="text-white font-bold text-xl">S</span>
        </div>
        <span class="text-xl font-bold text-slate-800 tracking-tight">SAT <span class="text-blue-600">Hosp</span></span>
      </div>

      <!-- Navigation -->
      <nav class="flex-1 px-4 space-y-1 mt-4">
        <div class="text-xs font-semibold text-slate-400 uppercase tracking-wider px-3 mb-2">Menu Principal</div>
        
        <a routerLink="/dashboard" routerLinkActive="active-link" class="nav-item">
          <lucide-icon [name]="icons.Dashboard" class="w-5 h-5 mr-3"></lucide-icon>
          Dashboard
        </a>

        <a routerLink="/facturacion" routerLinkActive="active-link" class="nav-item">
          <lucide-icon [name]="icons.Billing" class="w-5 h-5 mr-3"></lucide-icon>
          Facturación
        </a>

        <!-- Admin Only Sections -->
        <ng-container *ngIf="isAdmin()">
          <div class="text-xs font-semibold text-slate-400 uppercase tracking-wider px-3 mt-6 mb-2">Administración</div>
          
          <a routerLink="/cajas" routerLinkActive="active-link" class="nav-item">
            <lucide-icon [name]="icons.Cajas" class="w-5 h-5 mr-3"></lucide-icon>
            Gestión de Cajas
          </a>

          <a routerLink="/rx-orders" routerLinkActive="active-link" class="nav-item">
            <lucide-icon [name]="icons.Orders" class="w-5 h-5 mr-3"></lucide-icon>
            Ordenes Medicas
          </a>

          <a routerLink="/settings" routerLinkActive="active-link" class="nav-item">
            <lucide-icon [name]="icons.Settings" class="w-5 h-5 mr-3"></lucide-icon>
            Configuración
          </a>
        </ng-container>
      </nav>

      <!-- User Profile -->
      <div class="p-4 border-t border-slate-200/50 bg-slate-50/50">
        <div class="flex items-center p-2 rounded-lg">
          <div class="w-8 h-8 rounded-full bg-slate-200 flex items-center justify-center mr-3">
            <lucide-icon [name]="icons.User" class="w-4 h-4 text-slate-600"></lucide-icon>
          </div>
          <div class="flex-1 min-w-0">
            <p class="text-sm font-medium text-slate-900 truncate">{{ auth.currentUser()?.username }}</p>
            <p class="text-xs text-slate-500 truncate">{{ auth.currentUser()?.role }}</p>
          </div>
          <button (click)="auth.logout()" class="p-2 text-slate-400 hover:text-red-500 transition-colors">
            <lucide-icon [name]="icons.Logout" class="w-5 h-5"></lucide-icon>
          </button>
        </div>
      </div>
    </div>
  `,
    styles: [`
    .glass-sidebar {
      background: rgba(255, 255, 255, 0.7);
      backdrop-filter: blur(10px);
    }
    .nav-item {
      display: flex;
      items: center;
      padding: 0.75rem 0.75rem;
      border-radius: 0.75rem;
      color: #64748b;
      font-size: 0.875rem;
      font-weight: 500;
      transition: all 0.2s cubic-bezier(0.4, 0, 0.2, 1);
    }
    .nav-item:hover {
      background: rgba(235, 245, 255, 0.5);
      color: #2563eb;
    }
    .active-link {
      background: #eff6ff !important;
      color: #2563eb !important;
      font-weight: 600;
      box-shadow: 0 1px 2px rgba(37, 99, 235, 0.05);
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
        Logout: LogOut
    };

    isAdmin(): boolean {
        return this.auth.currentUser()?.role === 'Administrador';
    }
}
