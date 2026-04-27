import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LucideAngularModule } from 'lucide-angular';
import { BillingFacadeService } from '../../../core/services/billing-facade.service';
import { CatalogService, CatalogItem } from '../../../core/services/catalog.service';
import { PdfService } from '../../../core/services/pdf.service';
import { SettingsService } from '../../../core/services/settings.service';
import { environment } from '../../../../environments/environment';
import { firstValueFrom } from 'rxjs';

@Component({
  selector: 'app-sanity-check',
  standalone: true,
  imports: [CommonModule, LucideAngularModule],
  template: `
    <div class="space-y-8 p-6 md:p-8">
        <div class="bg-surface-card p-8 rounded-3xl flex items-center justify-between border border-white/10">
            <div class="flex items-center space-x-6">
                <div class="h-16 w-16 bg-emerald-500/10 text-emerald-500 rounded-2xl flex items-center justify-center">
                    <lucide-icon name="activity" class="w-8 h-8"></lucide-icon>
                </div>
                <div>
                    <h1 class="text-3xl font-black text-white uppercase">Estado de Integridad</h1>
                    <p class="text-xs text-slate-500 uppercase tracking-widest mt-1">Diagnóstico Automático</p>
                </div>
            </div>
            <button (click)="runAllTests()" [disabled]="isRunning()"
                class="bg-emerald-500 hover:bg-emerald-600 text-white px-8 py-4 rounded-2xl text-xs font-black transition-all disabled:opacity-50">
                {{ isRunning() ? 'EJECUTANDO...' : 'INICIAR CHEQUEO' }}
            </button>
        </div>

        <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
            <div *ngFor="let test of tests()" class="bg-surface-card p-6 rounded-3xl border border-white/10">
                <div class="flex items-center justify-between mb-4">
                    <div class="h-12 w-12 bg-slate-500/10 text-slate-500 rounded-xl flex items-center justify-center">
                        <lucide-icon [name]="test.icon" class="w-6 h-6"></lucide-icon>
                    </div>
                    <span class="text-[10px] font-black uppercase px-3 py-1 bg-white/5 rounded-full text-slate-400">
                        {{ test.status }}
                    </span>
                </div>
                <h3 class="text-lg font-black text-white uppercase">{{ test.name }}</h3>
                <p class="text-xs text-slate-500 mt-1">{{ test.description }}</p>
                <div *ngIf="test.message" class="mt-4 p-4 bg-black/20 rounded-xl">
                    <p class="text-[10px] font-mono text-slate-300">{{ test.message }}</p>
                </div>
            </div>
        </div>

        <div class="bg-surface-card rounded-3xl border border-white/10 overflow-hidden">
            <div class="p-6 bg-black/20 border-b border-white/10">
                <h3 class="text-sm font-black text-white uppercase">Consola</h3>
            </div>
            <div class="p-6 bg-black/40 h-64 overflow-y-auto font-mono text-xs space-y-2">
                <div *ngFor="let log of logs()" class="flex space-x-4">
                    <span class="text-slate-600">[{{ log.time | date:'HH:mm:ss' }}]</span>
                    <span [class.text-emerald-400]="log.type === 'info'" 
                          [class.text-rose-400]="log.type === 'error'"
                          [class.text-amber-400]="log.type === 'warn'">
                        {{ log.message }}
                    </span>
                </div>
            </div>
        </div>
    </div>
  `
})
export class SanityCheckComponent {
  private pdfService = inject(PdfService);
  private catalogService = inject(CatalogService);
  private settingsService = inject(SettingsService);
  private http = inject(HttpClient);

  isRunning = signal(false);
  logs = signal<{time: Date, message: string, type: 'info' | 'error' | 'warn'}[]>([]);

  tests = signal([
    { id: 'pdf-receipt', name: 'Recibos PDF', description: 'Pipeline QuestPDF.', icon: 'file-text', status: 'idle', message: '' },
    { id: 'pdf-guarantee', name: 'Garantías', description: 'Documentos legales.', icon: 'shield-check', status: 'idle', message: '' },
    { id: 'suggestions', name: 'Sugerencias', description: 'Motor automático.', icon: 'sparkles', status: 'idle', message: '' },
    { id: 'settings', name: 'Configuración', description: 'Cloud sync.', icon: 'settings', status: 'idle', message: '' },
    { id: 'catalog', name: 'Catálogo', description: 'Precios y servicios.', icon: 'database', status: 'idle', message: '' },
    { id: 'laboratory', name: 'Lab Monitor', description: 'Sync Legacy.', icon: 'flask-conical', status: 'idle', message: '' },
    { id: 'backend', name: 'API Core', description: 'Disponibilidad.', icon: 'server', status: 'idle', message: '' }
  ]);

  addLog(message: string, type: 'info' | 'error' | 'warn' = 'info') {
    this.logs.update(l => [...l, { time: new Date(), message, type }]);
  }

  updateTest(id: string, update: Partial<{status: string, message: string}>) {
    this.tests.update(t => t.map(item => item.id === id ? { ...item, ...update } : item));
  }

  async runAllTests() {
    this.isRunning.set(true);
    this.logs.set([]);
    this.tests.update(t => t.map(item => ({ ...item, status: 'idle', message: '' })));
    try {
        await this.testBackend();
        await this.testCatalog();
        await this.testSettings();
        await this.testPdfReceipt();
        await this.testPdfLegal();
        await this.testLaboratory();
        await this.testSuggestions();
    } catch (e) {} finally {
        this.isRunning.set(false);
    }
  }

  private async testBackend() {
    this.updateTest('backend', { status: 'running' });
    try {
        await firstValueFrom(this.settingsService.getConfig());
        this.updateTest('backend', { status: 'success', message: 'Conectado' });
    } catch (e) {
        this.updateTest('backend', { status: 'error', message: 'Error' });
    }
  }

  private async testCatalog() {
    this.updateTest('catalog', { status: 'running' });
    try {
        const catalog = await firstValueFrom(this.catalogService.getUnifiedCatalog());
        this.updateTest('catalog', { status: 'success', message: `${catalog.length} items` });
    } catch (e) {
        this.updateTest('catalog', { status: 'error' });
    }
  }

  private async testSettings() {
    this.updateTest('settings', { status: 'running' });
    try {
        const settings = await firstValueFrom(this.settingsService.getConfig());
        this.updateTest('settings', { status: 'success', message: settings.nombreEmpresa });
    } catch (e) {
        this.updateTest('settings', { status: 'error' });
    }
  }

  private async testPdfReceipt() {
    this.updateTest('pdf-receipt', { status: 'running' });
    try {
        const dummyData: any = { numeroRecibo: 'T-01', pacienteNombre: 'T', pacienteCedula: '0', detalles: [], totalUSD: 0, totalBS: 0, tasaBcv: 0, fechaEmision: new Date(), pagos: [] };
        const blob = await firstValueFrom(this.pdfService.generateRecibo(dummyData));
        this.updateTest('pdf-receipt', { status: 'success', message: `${blob.size} bytes` });
    } catch (e) {
        this.updateTest('pdf-receipt', { status: 'error' });
    }
  }

  private async testPdfLegal() {
    this.updateTest('pdf-guarantee', { status: 'running' });
    try {
        const dummyData: any = { nombreResponsable: 'T', cedulaResponsable: '0', nombrePaciente: 'T', montoTotal: 0, fechaCompromiso: new Date(), fechaVencimiento: new Date() };
        const blob = await firstValueFrom(this.pdfService.generateCompromiso(dummyData));
        this.updateTest('pdf-guarantee', { status: 'success', message: `${blob.size} bytes` });
    } catch (e) {
        this.updateTest('pdf-guarantee', { status: 'error' });
    }
  }

  private async testSuggestions() {
    this.updateTest('suggestions', { status: 'running' });
    try {
        const catalog = await firstValueFrom(this.catalogService.getUnifiedCatalog());
        const has = (catalog as any[]).some((i: any) => i.sugerenciasIds && i.sugerenciasIds.length > 0);
        this.updateTest('suggestions', { status: has ? 'success' : 'warn' });
    } catch (e) {
        this.updateTest('suggestions', { status: 'error' });
    }
  }

  private async testLaboratory() {
    this.updateTest('laboratory', { status: 'running' });
    try {
        const data = await firstValueFrom(this.http.get<any[]>(`${environment.apiUrl}/api/ReciboFactura/MonitoringOrders`));
        this.updateTest('laboratory', { status: 'success', message: `${data.length} órdenes` });
    } catch (e) {
        this.updateTest('laboratory', { status: 'error', message: 'API Off' });
    }
  }
}
