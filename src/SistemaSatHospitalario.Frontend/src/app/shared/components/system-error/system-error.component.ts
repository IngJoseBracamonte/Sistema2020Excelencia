import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { LucideAngularModule, RefreshCw, Home } from 'lucide-angular';
import { BaseErrorComponent, ErrorUIConfig } from './system-error-base';

@Component({
  selector: 'app-system-error',
  standalone: true,
  imports: [CommonModule, LucideAngularModule],
  template: `
    <div class="fixed inset-0 bg-[#050505] backdrop-blur-3xl z-[9999] flex items-center justify-center p-6 animate-fade-in">
        <div class="max-w-2xl w-full text-center space-y-12 relative">
            <!-- Glow dinámico basado en el tipo de error -->
            <div class="absolute -top-40 left-1/2 -translate-x-1/2 w-96 h-96 rounded-full blur-[120px] animate-pulse opacity-20"
                 [ngClass]="currentConfig.glowClass"></div>
            
            <div class="relative inline-block">
                <div class="h-32 w-32 border rounded-[2.5rem] flex items-center justify-center mx-auto shadow-2xl transition-transform duration-500 hover:rotate-0 rotate-6"
                     [ngClass]="currentConfig.iconContainerClass">
                    <lucide-icon [name]="currentConfig.icon" class="w-16 h-16 animate-bounce" [ngClass]="currentConfig.iconClass"></lucide-icon>
                </div>
                <div class="absolute -bottom-2 -right-2 text-white px-4 py-1 rounded-full text-[10px] font-black uppercase tracking-widest shadow-xl"
                     [ngClass]="currentConfig.badgeClass">
                    Error {{ errorCode }}
                </div>
            </div>

            <div class="space-y-4">
                <h1 class="text-6xl font-black text-white tracking-tighter uppercase leading-none">
                    {{ currentConfig.title }} <span class="text-transparent bg-clip-text" [ngClass]="currentConfig.gradientClass">{{ currentConfig.highlight }}</span>
                </h1>
                <p class="text-slate-400 font-medium text-lg max-w-lg mx-auto">
                    {{ currentConfig.message }}
                </p>
            </div>

            <div class="flex flex-col md:flex-row items-center justify-center gap-6 pt-8">
                <button (click)="retry()" class="px-10 py-5 bg-white text-black font-black uppercase tracking-widest rounded-2xl hover:bg-opacity-90 transition-all active:scale-95 flex items-center gap-3 group shadow-2xl shadow-white/5">
                    <lucide-icon [name]="icons.RefreshCw" class="w-5 h-5 group-hover:rotate-180 transition-transform duration-700"></lucide-icon>
                    Reintentar ahora
                </button>
                <button (click)="goHome()" class="px-10 py-5 bg-white/5 text-white font-black uppercase tracking-widest rounded-2xl border border-white/10 hover:bg-white/10 transition-all active:scale-95 flex items-center gap-3">
                    <lucide-icon [name]="icons.Home" class="w-5 h-5"></lucide-icon>
                    Panel Principal
                </button>
            </div>

            <div class="pt-12 border-t border-white/5">
                <p class="text-[9px] font-black text-slate-700 uppercase tracking-[0.5em]">Sistema de Diagnóstico Activo | Telemetría en Español</p>
            </div>
        </div>
    </div>
  `,
  styles: [`
    :host { display: block; height: 100vh; overflow: hidden; }
    .animate-fade-in { animation: fadeIn 0.4s ease-out forwards; }
    @keyframes fadeIn { from { opacity: 0; transform: translateY(10px); } to { opacity: 1; transform: translateY(0); } }
  `]
})
export class SystemErrorComponent extends BaseErrorComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  
  errorCode: string = '500';
  currentConfig!: ErrorUIConfig;
  
  readonly icons = { RefreshCw, Home };

  ngOnInit() {
    this.route.params.subscribe(params => {
      this.errorCode = params['code'] || '500';
      this.currentConfig = this.getErrorConfig(this.errorCode);
    });
  }

  retry() {
    window.location.reload();
  }

  goHome() {
    this.router.navigate(['/']);
  }
}
