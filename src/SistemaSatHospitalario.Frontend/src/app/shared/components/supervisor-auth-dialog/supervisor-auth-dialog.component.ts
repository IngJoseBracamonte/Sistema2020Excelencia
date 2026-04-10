import { Component, EventEmitter, Output, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LucideAngularModule } from 'lucide-angular';

/**
 * SupervisorAuthDialogComponent (Fase 1 - Security Matrix)
 * Diálogo modal para validación de Clave de Supervisor.
 * Diseño premium con glassmorphism y acento rose-500.
 */
@Component({
  selector: 'app-supervisor-auth-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule],
  template: `
    <div *ngIf="isOpen()" class="fixed inset-0 z-[100] flex items-center justify-center p-4 bg-black/60 backdrop-blur-sm animate-in fade-in duration-300">
      <div class="bg-surface-card w-full max-w-md rounded-[2.5rem] p-8 shadow-2xl border border-white/10 relative overflow-hidden transform transition-all animate-in zoom-in-95 duration-300">
        <!-- Decoración -->
        <div class="absolute top-0 right-0 w-32 h-32 bg-rose-500/10 rounded-bl-full -z-10 blur-3xl"></div>
        
        <div class="flex flex-col items-center text-center space-y-6">
          <div class="p-5 bg-rose-500/10 rounded-full">
            <lucide-icon name="shield-lock" class="w-12 h-12 text-rose-500"></lucide-icon>
          </div>
          
          <div class="space-y-2">
            <h3 class="text-xl font-black text-white uppercase tracking-wider">Autorización Requerida</h3>
            <p class="text-xs text-slate-400 font-medium">Se ha detectado una modificación de precio. Por favor, ingrese la Clave de Supervisor para continuar.</p>
          </div>

          <div class="w-full space-y-4">
            <div class="relative group">
              <lucide-icon name="key" class="w-5 h-5 absolute left-5 top-1/2 -translate-y-1/2 text-slate-500 group-focus-within:text-rose-500 transition-colors"></lucide-icon>
              <input 
                type="password" 
                [(ngModel)]="key" 
                (keyup.enter)="confirm()"
                placeholder="CLAVE DE SUPERVISOR"
                class="w-full bg-black/40 border border-white/5 focus:border-rose-500/50 rounded-2xl py-5 pl-14 pr-6 text-sm font-bold text-white placeholder:text-slate-600 focus:ring-4 focus:ring-rose-500/10 transition-all outline-none tracking-[0.3em] uppercase"
                autofocus
              />
            </div>

            <p *ngIf="error()" class="text-[10px] text-rose-500 font-bold uppercase tracking-widest animate-pulse">
              Clave incorrecta o campos vacíos
            </p>
          </div>

          <div class="grid grid-cols-2 gap-4 w-full pt-4">
            <button (click)="close()" 
              class="py-4 bg-white/5 hover:bg-white/10 text-slate-400 hover:text-white rounded-2xl font-black text-[10px] uppercase tracking-widest transition-all">
              Cancelar
            </button>
            <button (click)="confirm()" 
              class="py-4 bg-rose-500 hover:bg-rose-600 text-white rounded-2xl font-black text-[10px] uppercase tracking-widest shadow-lg shadow-rose-500/20 transition-all flex items-center justify-center">
              Confirmar
              <lucide-icon name="arrow-right" class="w-4 h-4 ml-2"></lucide-icon>
            </button>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    :host { display: contents; }
  `]
})
export class SupervisorAuthDialogComponent {
  public isOpen = signal(false);
  public key = '';
  public error = signal(false);

  @Output() onConfirm = new EventEmitter<string>();
  @Output() onCancel = new EventEmitter<void>();

  public open(): void {
    this.key = '';
    this.error.set(false);
    this.isOpen.set(true);
  }

  public close(): void {
    this.isOpen.set(false);
    this.onCancel.emit();
  }

  public confirm(): void {
    if (!this.key || this.key.length < 4) {
      this.error.set(true);
      return;
    }
    this.onConfirm.emit(this.key);
    this.isOpen.set(false);
  }
}
