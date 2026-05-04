import { Component, inject, signal, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LucideAngularModule, Bell, Check, Info, AlertTriangle, X } from 'lucide-angular';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
// Importamos el servicio de SignalR si existe, o usamos uno nuevo.
// Por ahora simularemos la conexión o usaremos HttpClient para pull inicial.

@Component({
  selector: 'app-notification-center',
  standalone: true,
  imports: [CommonModule, LucideAngularModule],
  template: `
    <div class="relative">
      <!-- Icono de Campana con Badge -->
      <button (click)="toggleDropdown()" 
        class="relative p-2 rounded-xl bg-white/5 hover:bg-white/10 text-slate-400 hover:text-white transition-all active:scale-95 group">
        <lucide-icon [name]="icons.Bell" class="w-5 h-5 group-hover:rotate-12 transition-transform"></lucide-icon>
        <span *ngIf="unreadCount() > 0" 
          class="absolute -top-1 -right-1 flex h-4 w-4 items-center justify-center rounded-full bg-rose-500 text-[8px] font-black text-white border-2 border-[#0a0f1a]">
          {{ unreadCount() > 9 ? '9+' : unreadCount() }}
        </span>
      </button>

      <!-- Dropdown Estilo Facebook -->
      <div *ngIf="showDropdown()" 
        class="absolute right-0 mt-4 w-80 max-h-[500px] overflow-hidden bg-surface-card border border-white/10 rounded-3xl shadow-2xl z-[300] animate-scale-in">
        
        <div class="p-4 border-b border-white/5 flex items-center justify-between">
          <h3 class="text-[10px] font-black text-white uppercase tracking-widest">Notificaciones</h3>
          <button (click)="markAllAsRead()" class="text-[8px] font-black text-emerald-500 uppercase tracking-widest hover:underline">Marcar todas como leídas</button>
        </div>

        <div class="overflow-y-auto max-h-[400px] custom-scrollbar">
          <div *ngFor="let n of notifications()" 
            [ngClass]="{'bg-white/5': !n.isRead}"
            (click)="markAsRead(n)"
            class="p-4 border-b border-white/5 hover:bg-white/[0.02] cursor-pointer transition-colors relative group">
            
            <div class="flex gap-4">
              <div [ngClass]="getTypeClass(n.type)" 
                class="h-8 w-8 rounded-lg flex items-center justify-center shrink-0">
                <lucide-icon [name]="getTypeIcon(n.type)" class="w-4 h-4"></lucide-icon>
              </div>
              
              <div class="flex-1 space-y-1">
                <p class="text-[10px] font-black text-white uppercase leading-tight tracking-tight">{{ n.title }}</p>
                <p class="text-[9px] text-slate-400 leading-tight">{{ n.message }}</p>
                <p class="text-[7px] font-bold text-slate-500 uppercase mt-1">{{ formatTime(n.timestamp) }}</p>
              </div>

              <div *ngIf="!n.isRead" class="w-2 h-2 rounded-full bg-emerald-500 mt-2 shrink-0"></div>
            </div>
            
            <a *ngIf="n.actionUrl" [href]="n.actionUrl" 
               class="absolute inset-0 opacity-0 pointer-events-auto"></a>
          </div>

          <div *ngIf="notifications().length === 0" class="p-12 text-center opacity-30">
            <lucide-icon [name]="icons.Bell" class="w-8 h-8 mx-auto mb-3"></lucide-icon>
            <p class="text-[9px] font-black uppercase tracking-widest">No hay notificaciones</p>
          </div>
        </div>

      </div>

      <!-- Overlay para cerrar dropdown -->
      <div *ngIf="showDropdown()" (click)="showDropdown.set(false)" class="fixed inset-0 z-[-1]"></div>
    </div>
  `,
  styles: [`
    .animate-scale-in {
      animation: scaleIn 0.2s cubic-bezier(0.16, 1, 0.3, 1) forwards;
      transform-origin: top right;
    }
    @keyframes scaleIn {
      from { opacity: 0; transform: scale(0.95) translateY(-10px); }
      to { opacity: 1; transform: scale(1) translateY(0); }
    }
  `]
})
export class NotificationCenterComponent implements OnInit {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api/Notifications`; // Por implementar

  public icons = { Bell, Check, Info, AlertTriangle, X };
  public showDropdown = signal(false);
  public notifications = signal<any[]>([]);
  public unreadCount = signal(0);

  ngOnInit() {
    this.loadNotifications();
    // Aquí se integraría SignalR para recibir en tiempo real
  }

  loadNotifications() {
    // Simulamos carga por ahora hasta tener el controlador
    this.http.get<any[]>(`${this.apiUrl}/latest`).subscribe({
      next: (data) => {
        this.notifications.set(data);
        this.updateUnreadCount();
      },
      error: () => {
        // Fallback para demo
        this.notifications.set([]);
      }
    });
  }

  updateUnreadCount() {
    this.unreadCount.set(this.notifications().filter(n => !n.isRead).length);
  }

  toggleDropdown() {
    this.showDropdown.set(!this.showDropdown());
  }

  markAsRead(notification: any) {
    if (notification.isRead) return;
    
    this.http.post(`${this.apiUrl}/${notification.id}/read`, {}).subscribe(() => {
      notification.isRead = true;
      this.updateUnreadCount();
    });
  }

  markAllAsRead() {
    this.http.post(`${this.apiUrl}/read-all`, {}).subscribe(() => {
      this.notifications().forEach(n => n.isRead = true);
      this.updateUnreadCount();
    });
  }

  getTypeClass(type: string) {
    switch (type.toLowerCase()) {
      case 'success': return 'bg-emerald-500/10 text-emerald-500';
      case 'warning': return 'bg-amber-500/10 text-amber-500';
      case 'error': return 'bg-rose-500/10 text-rose-500';
      default: return 'bg-indigo-500/10 text-indigo-500';
    }
  }

  getTypeIcon(type: string) {
    switch (type.toLowerCase()) {
      case 'success': return this.icons.Check;
      case 'warning': return this.icons.AlertTriangle;
      case 'error': return this.icons.X;
      default: return this.icons.Info;
    }
  }

  formatTime(timestamp: string) {
    const date = new Date(timestamp);
    const now = new Date();
    const diff = Math.floor((now.getTime() - date.getTime()) / 1000);

    if (diff < 60) return 'Hace un momento';
    if (diff < 3600) return `Hace ${Math.floor(diff / 60)} min`;
    if (diff < 86400) return `Hace ${Math.floor(diff / 3600)} h`;
    return date.toLocaleDateString();
  }
}
