import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { LucideAngularModule, Search, AlertCircle, RefreshCcw, FileText, Check, Link } from 'lucide-angular';

export interface UnprocessedReportOrder {
  id: number;
  cuentaId: string;
  pacienteId: string;
  pacienteNombre: string;
  pacienteCedula?: string;
  estudio: string;
  tipoServicio: string;
  estado: string;
  fechaCreacion: string;
  linkInforme?: string;
  observacionesMedico?: string;
  medicoInterpreteId?: string;
  requiereInforme: boolean;
}

@Component({
  selector: 'app-unprocessed-imaging-reports',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule],
  template: `
    <div class="p-6 space-y-6 bg-slate-950 min-h-screen text-slate-100">
      <!-- Header -->
      <div class="flex items-center justify-between">
        <div>
          <h1 class="text-xl font-bold text-slate-100 flex items-center gap-2">
            <lucide-icon [img]="icons.FileText" [size]="24" class="text-purple-400" />
            Auditoría de Informes Médicos de Imagenología
          </h1>
          <p class="text-xs text-slate-400">Control y regularización de estudios de RX/Tomografía pendientes de informe o vinculación PACS</p>
        </div>
        <button (click)="loadOrders()" class="px-4 py-2 bg-slate-800 hover:bg-slate-700 border border-slate-700 rounded-xl text-xs font-semibold flex items-center gap-2 transition-colors">
          <lucide-icon [img]="icons.RefreshCcw" [size]="16" />
          Actualizar Lista
        </button>
      </div>

      <!-- Main Table -->
      <div class="bg-slate-900 border border-slate-800 rounded-2xl overflow-hidden shadow-xl">
        <table class="w-full text-left border-collapse">
          <thead>
            <tr class="text-[11px] font-bold uppercase tracking-wider text-slate-400 bg-slate-800/50 border-b border-slate-800">
              <th class="p-4">Paciente</th>
              <th class="p-4">Estudio</th>
              <th class="p-4">Tipo</th>
              <th class="p-4">Fecha Carga</th>
              <th class="p-4">Estado</th>
              <th class="p-4">Informe PACS / Enlace</th>
              <th class="p-4 text-right">Acción</th>
            </tr>
          </thead>
          <tbody class="divide-y divide-slate-800/60 text-xs">
            <tr *ngFor="let order of orders()" class="hover:bg-slate-800/30 transition-colors">
              <td class="p-4 font-semibold text-slate-200">
                <div>{{ order.pacienteNombre }}</div>
                <div class="text-[10px] text-slate-500 font-mono">{{ order.pacienteCedula || 'N/A' }}</div>
              </td>
              <td class="p-4 font-medium text-slate-300">{{ order.estudio }}</td>
              <td class="p-4">
                <span class="px-2.5 py-1 rounded-lg text-[10px] font-bold uppercase"
                      [ngClass]="order.tipoServicio === 'RX' ? 'bg-rose-500/20 text-rose-300' : 'bg-sky-500/20 text-sky-300'">
                  {{ order.tipoServicio }}
                </span>
              </td>
              <td class="p-4 text-slate-400">{{ order.fechaCreacion | date:'dd/MM/yyyy HH:mm' }}</td>
              <td class="p-4">
                <span class="px-2.5 py-1 rounded-full text-[10px] font-bold uppercase"
                      [ngClass]="order.estado === 'Procesado' ? 'bg-emerald-500/20 text-emerald-300' : 'bg-amber-500/20 text-amber-300'">
                  {{ order.estado }}
                </span>
              </td>
              <td class="p-4">
                <input type="text" [(ngModel)]="order.linkInforme" placeholder="Ej: https://pacs.hospital.com/123"
                       class="w-full px-3 py-1.5 bg-slate-950 border border-slate-700 rounded-lg text-slate-100 placeholder-slate-600 text-xs font-mono focus:border-purple-500 focus:ring-1 focus:ring-purple-500 outline-none" />
              </td>
              <td class="p-4 text-right">
                <button (click)="saveReportLink(order)" [disabled]="isSaving(order.id)"
                        class="px-3 py-1.5 bg-purple-600 hover:bg-purple-500 disabled:opacity-50 text-white rounded-lg text-xs font-semibold flex items-center gap-1.5 ml-auto transition-all">
                  <lucide-icon [img]="icons.Check" [size]="14" />
                  Guardar
                </button>
              </td>
            </tr>
            <tr *ngIf="orders().length === 0 && !isLoading()">
              <td colspan="7" class="p-8 text-center text-slate-500">
                No hay estudios de imagenología pendientes de informe o adjunción.
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  `
})
export class UnprocessedImagingReportsComponent implements OnInit {
  protected readonly icons = { Search, AlertCircle, RefreshCcw, FileText, Check, Link };
  
  private http = inject(HttpClient);
  
  public orders = signal<UnprocessedReportOrder[]>([]);
  public isLoading = signal<boolean>(false);
  public savingIds = signal<Set<number>>(new Set());

  ngOnInit(): void {
    this.loadOrders();
  }

  public loadOrders(): void {
    this.isLoading.set(true);
    this.http.get<UnprocessedReportOrder[]>(`${environment.apiUrl}/api/Imaging/unprocessed-reports`).subscribe({
      next: (res) => {
        this.orders.set(res || []);
        this.isLoading.set(false);
      },
      error: (err) => {
        console.error('Error loading unprocessed reports:', err);
        this.isLoading.set(false);
      }
    });
  }

  public isSaving(id: number): boolean {
    return this.savingIds().has(id);
  }

  public saveReportLink(order: UnprocessedReportOrder): void {
    if (!order.linkInforme) {
      alert('Por favor ingrese el enlace al informe PACS.');
      return;
    }

    const nextSaving = new Set(this.savingIds());
    nextSaving.add(order.id);
    this.savingIds.set(nextSaving);

    this.http.post(`${environment.apiUrl}/api/Imaging/${order.id}/complete`, {
      linkInforme: order.linkInforme,
      observacionesMedico: order.observacionesMedico || 'Informe adjuntado por auditoría administrativa'
    }).subscribe({
      next: () => {
        alert(`Informe vinculado para la orden de ${order.pacienteNombre}.`);
        const updatedSaving = new Set(this.savingIds());
        updatedSaving.delete(order.id);
        this.savingIds.set(updatedSaving);
        this.loadOrders();
      },
      error: (err) => {
        alert('Error al vincular el informe: ' + (err.error?.message || err.message));
        const updatedSaving = new Set(this.savingIds());
        updatedSaving.delete(order.id);
        this.savingIds.set(updatedSaving);
      }
    });
  }
}
