import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LucideAngularModule } from 'lucide-angular';

@Component({
  selector: 'app-github-test',
  standalone: true,
  imports: [CommonModule, LucideAngularModule],
  template: `
    <div class="min-h-screen bg-slate-950 p-8 flex items-center justify-center">
      <div class="max-w-2xl w-full bg-slate-900/50 backdrop-blur-xl border border-blue-500/30 rounded-[40px] p-12 text-center shadow-2xl shadow-blue-500/10">
        <div class="mb-8 flex justify-center">
          <div class="p-6 bg-blue-500/20 rounded-full border border-blue-500/30 animate-pulse">
            <i-lucide name="github" class="w-16 h-16 text-blue-400"></i-lucide>
          </div>
        </div>
        <h1 class="text-4xl font-black text-white mb-6 tracking-tight leading-tight">
          ¡CONEXIÓN EXITOSA!
        </h1>
        <p class="text-xl text-blue-200/70 font-medium mb-8 leading-relaxed">
          Esto ha sido subido desde <span class="text-blue-400 font-bold">GitHub por PUSH</span>. 
          La automatización de despliegue continuo está funcionando correctamente.
          <br>
          <span class="text-sm opacity-50 font-mono mt-4 block">Build Artifact: v1.2.33</span>
        </p>
        <div class="flex items-center justify-center gap-3 py-4 px-6 bg-blue-500/10 rounded-2xl border border-blue-500/20 w-fit mx-auto">
          <div class="w-2 h-2 bg-green-500 rounded-full animate-ping"></div>
          <span class="text-green-400 text-xs font-black uppercase tracking-widest">Sistema Sincronizado</span>
        </div>
      </div>
    </div>
  `
})
export class GithubTestComponent {}
