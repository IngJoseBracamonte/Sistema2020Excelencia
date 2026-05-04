import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';
import { SidebarComponent } from '../../components/sidebar/sidebar.component';
import { NotificationCenterComponent } from '../../components/notification-center/notification-center.component';


@Component({
    selector: 'app-user-layout',
    standalone: true,
    imports: [CommonModule, RouterOutlet, SidebarComponent, NotificationCenterComponent],

    template: `
    <div class="flex h-screen bg-surface overflow-hidden">
      <!-- Sidebar fijo -->
      <app-sidebar class="flex-shrink-0 animate-slide-in"></app-sidebar>

      <!-- Main Content Area -->
      <main class="flex-1 flex flex-col min-w-0 overflow-hidden relative">
        <!-- Top Bar / Header -->
        <header class="h-16 flex items-center justify-between px-6 md:px-12 bg-surface/50 backdrop-blur-md border-b border-white/5 z-[200]">
          <div class="flex items-center gap-4">
            <!-- Espacio para título dinámico si se requiere -->
          </div>
          
          <div class="flex items-center gap-6">
            <app-notification-center></app-notification-center>
            
            <!-- Perfil de usuario simplificado -->
            <div class="h-8 w-8 rounded-full bg-indigo-500/20 border border-indigo-500/30 flex items-center justify-center">
              <span class="text-[10px] font-black text-indigo-400">JB</span>
            </div>
          </div>
        </header>
        
        <!-- Vista de contenido con scroll -->
        <div class="flex-1 overflow-y-auto pt-4 px-2 md:px-6 pb-6">

          <div class="max-w-7xl mx-auto animate-fade-in-up">
            <router-outlet></router-outlet>
          </div>
        </div>

        <!-- Pattern decorativo sutil en el fondo -->
        <div class="absolute top-0 right-0 p-32 opacity-[0.03] pointer-events-none -z-10">
           <svg width="400" height="400" viewBox="0 0 100 100">
             <circle cx="50" cy="50" r="40" stroke="currentColor" stroke-width="0.5" fill="none" class="text-blue-500" />
           </svg>
        </div>
      </main>
    </div>
  `,
    styles: [`
    @keyframes slideIn {
      from { transform: translateX(-20px); opacity: 0; }
      to { transform: translateX(0); opacity: 1; }
    }
    @keyframes fadeInUp {
      from { opacity: 0; }
      to { opacity: 1; }
    }
    .animate-slide-in {
      animation: slideIn 0.5s ease-out forwards;
    }
    .animate-fade-in-up {
      animation: fadeInUp 0.5s ease-out 0.2s forwards;
      opacity: 0;
    }
  `]
})
export class UserLayoutComponent { }
