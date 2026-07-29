import { Component, inject, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PabellonService, OrdenCirugia, CrearOrdenCirugiaRequest } from '../../../core/services/pabellon.service';
import { MedicoService } from '../../../core/services/medico.service';
import { PatientService } from '../../../core/services/patient.service';
import { FacturacionService } from '../../../core/services/facturacion.service';
import { GestionConsumoModalComponent } from './gestion-consumo-modal.component';
import { 
  LucideAngularModule, 
  Calendar, 
  List, 
  Plus, 
  Activity, 
  Clock, 
  User, 
  UserCheck, 
  CheckCircle2, 
  XCircle, 
  Play, 
  Package, 
  Search, 
  Filter, 
  RefreshCcw,
  Sparkles,
  AlertTriangle,
  Stethoscope
} from 'lucide-angular';

@Component({
  selector: 'app-pabellon-gestion',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule, GestionConsumoModalComponent],
  template: `
    <div class="p-6 space-y-6 bg-gray-950 min-h-screen text-gray-100">
      
      <!-- Top Action Bar -->
      <div class="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4 bg-gray-900/60 p-4 rounded-xl border border-gray-800 backdrop-blur-md">
        <div>
          <h1 class="text-2xl font-bold text-white flex items-center gap-2">
            <lucide-icon name="stethoscope" class="w-6 h-6 text-sky-400"></lucide-icon>
            Módulo de Cirugías (Pabellón)
          </h1>
          <p class="text-xs text-gray-400">Gestión operativa de cirugías, vista dual, consumo de insumos y auditoría</p>
        </div>

        <div class="flex items-center gap-3 w-full sm:w-auto">
          <!-- View Toggle Switcher -->
          <div class="bg-gray-950 p-1 rounded-lg border border-gray-800 flex items-center gap-1">
            <button 
              (click)="vistaModo.set('calendario')" 
              [class.bg-sky-600]="vistaModo() === 'calendario'"
              [class.text-white]="vistaModo() === 'calendario'"
              [class.text-gray-400]="vistaModo() !== 'calendario'"
              class="px-3 py-1.5 rounded-md text-xs font-semibold transition flex items-center gap-1.5">
              <lucide-icon name="calendar" class="w-3.5 h-3.5"></lucide-icon>
              Calendario
            </button>
            <button 
              (click)="vistaModo.set('lista')" 
              [class.bg-sky-600]="vistaModo() === 'lista'"
              [class.text-white]="vistaModo() === 'lista'"
              [class.text-gray-400]="vistaModo() !== 'lista'"
              class="px-3 py-1.5 rounded-md text-xs font-semibold transition flex items-center gap-1.5">
              <lucide-icon name="list" class="w-3.5 h-3.5"></lucide-icon>
              Lista
            </button>
          </div>

          <button 
            (click)="abrirModalNuevaCirugia()"
            class="bg-sky-600 hover:bg-sky-500 text-white font-semibold text-xs px-4 py-2 rounded-lg shadow-lg shadow-sky-600/20 transition flex items-center gap-2">
            <lucide-icon name="plus" class="w-4 h-4"></lucide-icon>
            Programar Cirugía
          </button>
        </div>
      </div>

      <!-- Filter Controls -->
      <div class="flex flex-wrap items-center gap-3 bg-gray-900/40 p-3 rounded-lg border border-gray-800">
        <div class="relative flex-1 min-w-[200px]">
          <input 
            type="text" 
            [ngModel]="filtroTexto()" 
            (ngModelChange)="filtroTexto.set($event)"
            placeholder="Buscar por paciente, cédula o cirugía..." 
            class="w-full bg-gray-950 border border-gray-700 rounded-lg px-3 py-1.5 text-xs text-white pl-8 focus:border-sky-500 focus:outline-none">
          <lucide-icon name="search" class="w-3.5 h-3.5 text-gray-500 absolute left-2.5 top-2.5"></lucide-icon>
        </div>

        <select 
          [ngModel]="filtroEstado()" 
          (ngModelChange)="onFiltroEstadoChange($event)"
          class="bg-gray-950 border border-gray-700 rounded-lg px-3 py-1.5 text-xs text-white focus:border-sky-500 focus:outline-none">
          <option value="">Todos los Estados</option>
          <option value="PendienteEjecucion">Pendiente de Ejecución</option>
          <option value="EnProceso">En Proceso</option>
          <option value="Completada">Completada</option>
          <option value="Cancelada">Cancelada</option>
        </select>

        <button 
          (click)="cargarOrdenes()" 
          class="p-1.5 bg-gray-800 hover:bg-gray-700 text-gray-300 rounded-lg transition">
          <lucide-icon name="refresh-ccw" class="w-4 h-4"></lucide-icon>
        </button>
      </div>

      <!-- VISTA DUAL 1: CALENDARIO -->
      <div *ngIf="vistaModo() === 'calendario'" class="space-y-4">
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          <div 
            *ngFor="let orden of ordenesFiltradas()" 
            class="bg-gray-900 border rounded-xl p-4 shadow-lg flex flex-col justify-between transition hover:border-gray-700"
            [class.border-amber-500]="orden.estado === 'PendienteEjecucion'"
            [class.border-sky-500]="orden.estado === 'EnProceso'"
            [class.border-emerald-500]="orden.estado === 'Completada'"
            [class.border-red-500]="orden.estado === 'Cancelada'">

            <div>
              <!-- Header Badges -->
              <div class="flex items-center justify-between gap-2 mb-2">
                <span class="px-2 py-0.5 rounded text-[10px] font-bold uppercase"
                  [class.bg-amber-950]="orden.estado === 'PendienteEjecucion'"
                  [class.text-amber-400]="orden.estado === 'PendienteEjecucion'"
                  [class.bg-sky-950]="orden.estado === 'EnProceso'"
                  [class.text-sky-400]="orden.estado === 'EnProceso'"
                  [class.bg-emerald-950]="orden.estado === 'Completada'"
                  [class.text-emerald-400]="orden.estado === 'Completada'"
                  [class.bg-red-950]="orden.estado === 'Cancelada'"
                  [class.text-red-400]="orden.estado === 'Cancelada'">
                  {{ orden.estado }}
                </span>
                <span class="text-[11px] font-mono text-gray-400 flex items-center gap-1">
                  <lucide-icon name="clock" class="w-3 h-3 text-gray-500"></lucide-icon>
                  {{ orden.fechaHoraProgramada | date:'shortTime' }} ({{ orden.fechaHoraProgramada | date:'dd/MM' }})
                </span>
              </div>

              <!-- Body Info -->
              <h3 class="text-sm font-bold text-white mb-1 line-clamp-1">{{ orden.descripcionCirugia }}</h3>
              <div class="text-xs text-gray-300 font-medium mb-1">
                Paciente: <span class="text-sky-300 font-semibold">{{ orden.pacienteNombre }}</span>
                <span class="text-gray-500 text-[10px]"> ({{ orden.pacienteCedula }})</span>
              </div>
              <div class="text-xs text-gray-400 flex items-center gap-1 mb-3">
                <lucide-icon name="user-check" class="w-3.5 h-3.5 text-gray-500"></lucide-icon>
                Cirujano: {{ orden.medicoNombre }}
              </div>
              <div class="text-xs font-mono text-emerald-400 font-semibold mb-3">
                Precio Base: $ {{ orden.precioBaseUsd }} USD
              </div>
            </div>

            <!-- Actions Footer -->
            <div class="pt-3 border-t border-gray-800/80 flex items-center justify-between gap-2">
              <button 
                (click)="abrirModalConsumo(orden.id)"
                class="bg-gray-800 hover:bg-gray-700 text-sky-400 font-semibold text-xs px-3 py-1.5 rounded-lg transition flex items-center gap-1.5">
                <lucide-icon name="package" class="w-3.5 h-3.5"></lucide-icon>
                Consumo & Logs
              </button>

              <!-- State Transition Actions -->
              <div class="flex items-center gap-1">
                <button 
                  *ngIf="orden.estado === 'PendienteEjecucion'"
                  (click)="cambiarEstado(orden.id, 'EnProceso')"
                  title="Iniciar Cirugía"
                  class="bg-sky-600 hover:bg-sky-500 text-white p-1.5 rounded-lg transition">
                  <lucide-icon name="play" class="w-3.5 h-3.5"></lucide-icon>
                </button>

                <button 
                  *ngIf="orden.estado === 'EnProceso'"
                  (click)="cambiarEstado(orden.id, 'Completada')"
                  title="Completar Cirugía"
                  class="bg-emerald-600 hover:bg-emerald-500 text-white p-1.5 rounded-lg transition">
                  <lucide-icon name="check-circle-2" class="w-3.5 h-3.5"></lucide-icon>
                </button>

                <button 
                  *ngIf="orden.estado !== 'Completada' && orden.estado !== 'Cancelada'"
                  (click)="solicitarCancelacion(orden)"
                  title="Cancelar Cirugía"
                  class="bg-red-950 hover:bg-red-900 text-red-400 p-1.5 rounded-lg transition">
                  <lucide-icon name="x-circle" class="w-3.5 h-3.5"></lucide-icon>
                </button>
              </div>
            </div>

          </div>
        </div>
      </div>

      <!-- VISTA DUAL 2: LISTA -->
      <div *ngIf="vistaModo() === 'lista'" class="bg-gray-900 border border-gray-800 rounded-xl overflow-hidden shadow-lg">
        <table class="w-full text-left text-xs border-collapse">
          <thead>
            <tr class="border-b border-gray-800 text-gray-400 bg-gray-950/60 uppercase text-[10px] tracking-wider">
              <th class="py-3 px-4">Fecha / Hora</th>
              <th class="py-3 px-4">Cirugía</th>
              <th class="py-3 px-4">Paciente</th>
              <th class="py-3 px-4">Médico Cirujano</th>
              <th class="py-3 px-4 text-center">Estado</th>
              <th class="py-3 px-4 text-right">Precio Base ($)</th>
              <th class="py-3 px-4 text-center">Acciones</th>
            </tr>
          </thead>
          <tbody class="divide-y divide-gray-800/60">
            <tr *ngFor="let orden of ordenesFiltradas()" class="hover:bg-gray-800/30 transition">
              <td class="py-3 px-4 font-mono text-gray-300">
                {{ orden.fechaHoraProgramada | date:'short' }}
              </td>
              <td class="py-3 px-4 font-bold text-white">
                {{ orden.descripcionCirugia }}
              </td>
              <td class="py-3 px-4">
                <div class="text-gray-200 font-semibold">{{ orden.pacienteNombre }}</div>
                <div class="text-[10px] text-gray-500">{{ orden.pacienteCedula }}</div>
              </td>
              <td class="py-3 px-4 text-gray-300">
                {{ orden.medicoNombre }}
              </td>
              <td class="py-3 px-4 text-center">
                <span class="px-2 py-0.5 rounded text-[10px] font-bold uppercase"
                  [class.bg-amber-950]="orden.estado === 'PendienteEjecucion'"
                  [class.text-amber-400]="orden.estado === 'PendienteEjecucion'"
                  [class.bg-sky-950]="orden.estado === 'EnProceso'"
                  [class.text-sky-400]="orden.estado === 'EnProceso'"
                  [class.bg-emerald-950]="orden.estado === 'Completada'"
                  [class.text-emerald-400]="orden.estado === 'Completada'"
                  [class.bg-red-950]="orden.estado === 'Cancelada'"
                  [class.text-red-400]="orden.estado === 'Cancelada'">
                  {{ orden.estado }}
                </span>
              </td>
              <td class="py-3 px-4 text-right font-mono text-emerald-400 font-bold">
                $ {{ orden.precioBaseUsd }} USD
              </td>
              <td class="py-3 px-4 text-center">
                <button 
                  (click)="abrirModalConsumo(orden.id)"
                  class="bg-gray-800 hover:bg-gray-700 text-sky-400 font-semibold text-xs px-2.5 py-1 rounded transition">
                  Gestionar
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Modal de Gestión de Consumo y Devolución -->
      <app-gestion-consumo-modal 
        *ngIf="modalConsumoOrdenId()" 
        [ordenId]="modalConsumoOrdenId()!" 
        (cerrarModal)="modalConsumoOrdenId.set(null)"
        (datosActualizados)="cargarOrdenes()">
      </app-gestion-consumo-modal>

      <!-- Modal Programar Nueva Cirugía -->
      <div *ngIf="modalNuevaCirugiaVisible()" class="fixed inset-0 bg-black/70 backdrop-blur-sm z-50 flex items-center justify-center p-4">
        <div class="bg-gray-900 border border-gray-800 rounded-xl shadow-2xl w-full max-w-lg p-6 space-y-4 text-white">
          <div class="flex justify-between items-center border-b border-gray-800 pb-3">
            <h3 class="text-lg font-bold text-white flex items-center gap-2">
              <lucide-icon name="plus" class="w-5 h-5 text-sky-400"></lucide-icon>
              Programar Nueva Cirugía
            </h3>
            <button (click)="modalNuevaCirugiaVisible.set(false)" class="text-gray-400 hover:text-white">
              <lucide-icon name="x-circle" class="w-5 h-5"></lucide-icon>
            </button>
          </div>

          <div class="space-y-3 text-xs">
            <div>
              <label class="text-gray-400 mb-1 block">Descripción de la Cirugía *</label>
              <input type="text" [(ngModel)]="nuevaCirugiaForm.descripcionCirugia" class="w-full bg-gray-950 border border-gray-700 rounded px-3 py-2 text-white focus:outline-none focus:border-sky-500">
            </div>

            <div class="grid grid-cols-2 gap-3">
              <div>
                <label class="text-gray-400 mb-1 block">Precio Base USD ($) *</label>
                <input type="number" [(ngModel)]="nuevaCirugiaForm.precioBaseUsd" class="w-full bg-gray-950 border border-gray-700 rounded px-3 py-2 text-white focus:outline-none focus:border-sky-500">
              </div>
              <div>
                <label class="text-gray-400 mb-1 block">Fecha y Hora Programada *</label>
                <input type="datetime-local" [(ngModel)]="nuevaCirugiaForm.fechaHoraProgramada" class="w-full bg-gray-950 border border-gray-700 rounded px-3 py-2 text-white focus:outline-none focus:border-sky-500">
              </div>
            </div>

            <div>
              <label class="text-gray-400 mb-1 block">Médico Cirujano *</label>
              <select [(ngModel)]="nuevaCirugiaForm.medicoId" class="w-full bg-gray-950 border border-gray-700 rounded px-3 py-2 text-white focus:outline-none focus:border-sky-500">
                <option value="">Seleccione un Médico</option>
                <option *ngFor="let med of medicos()" [value]="med.id">{{ med.nombre }}</option>
              </select>
            </div>

            <div>
              <label class="text-gray-400 mb-1 block">ID Cuenta de Servicio del Paciente *</label>
              <input type="text" [(ngModel)]="nuevaCirugiaForm.cuentaServicioId" placeholder="Guid de la cuenta abierta" class="w-full bg-gray-950 border border-gray-700 rounded px-3 py-2 text-white focus:outline-none focus:border-sky-500">
            </div>
            <div>
              <label class="text-gray-400 mb-1 block">ID Paciente *</label>
              <input type="text" [(ngModel)]="nuevaCirugiaForm.pacienteId" placeholder="Guid del paciente" class="w-full bg-gray-950 border border-gray-700 rounded px-3 py-2 text-white focus:outline-none focus:border-sky-500">
            </div>
          </div>

          <div class="flex justify-end gap-2 pt-3 border-t border-gray-800">
            <button (click)="modalNuevaCirugiaVisible.set(false)" class="bg-gray-800 hover:bg-gray-700 text-gray-300 text-xs px-4 py-2 rounded-lg">Cancelar</button>
            <button 
              (click)="guardarNuevaCirugia()"
              [disabled]="!esFormularioValido()"
              class="bg-sky-600 hover:bg-sky-500 text-white text-xs font-semibold px-4 py-2 rounded-lg disabled:opacity-50">
              Guardar y Programar
            </button>
          </div>
        </div>
      </div>

      <!-- Modal Confirmar Cancelación Cirugía -->
      <div *ngIf="modalCancelacionOrden()" class="fixed inset-0 bg-black/70 backdrop-blur-sm z-50 flex items-center justify-center p-4">
        <div class="bg-gray-900 border border-gray-800 rounded-xl shadow-2xl w-full max-w-md p-6 space-y-4 text-white">
          <div class="flex justify-between items-center border-b border-gray-800 pb-3">
            <h3 class="text-base font-bold text-red-400 flex items-center gap-2">
              <lucide-icon name="alert-triangle" class="w-5 h-5"></lucide-icon>
              Cancelar Cirugía
            </h3>
            <button (click)="modalCancelacionOrden.set(null)" class="text-gray-400 hover:text-white">
              <lucide-icon name="x-circle" class="w-5 h-5"></lucide-icon>
            </button>
          </div>

          <p class="text-xs text-gray-300">
            Está a punto de cancelar la cirugía <strong class="text-white">'{{ modalCancelacionOrden()?.descripcionCirugia }}'</strong>. Por favor especifique el motivo de auditoría:
          </p>

          <div>
            <label class="text-[11px] text-gray-400 mb-1 block">Motivo de Cancelación *</label>
            <textarea 
              [ngModel]="motivoCancelacionTexto()" 
              (ngModelChange)="motivoCancelacionTexto.set($event)"
              rows="3" 
              placeholder="Ingrese la razón justificada..."
              class="w-full bg-gray-950 border border-gray-700 rounded px-3 py-2 text-xs text-white focus:outline-none focus:border-red-500"></textarea>
          </div>

          <div class="flex justify-end gap-2 pt-2 border-t border-gray-800">
            <button (click)="modalCancelacionOrden.set(null)" class="bg-gray-800 hover:bg-gray-700 text-gray-300 text-xs px-4 py-2 rounded-lg">Volver</button>
            <button 
              (click)="confirmarCancelacion()"
              [disabled]="!motivoCancelacionTexto().trim()"
              class="bg-red-600 hover:bg-red-500 text-white text-xs font-semibold px-4 py-2 rounded-lg disabled:opacity-50 transition">
              Confirmar Cancelación
            </button>
          </div>
        </div>
      </div>

    </div>
  `
})
export class PabellonGestionComponent implements OnInit {
  private pabellonService = inject(PabellonService);
  private medicoService = inject(MedicoService);

  public vistaModo = signal<'calendario' | 'lista'>('calendario');
  public ordenes = signal<OrdenCirugia[]>([]);
  public medicos = signal<{ id: string; nombre: string; especialidad?: string }[]>([]);
  public filtroTexto = signal<string>('');
  public filtroEstado = signal<string>('');

  public modalConsumoOrdenId = signal<string | null>(null);
  public modalNuevaCirugiaVisible = signal<boolean>(false);
  public modalCancelacionOrden = signal<OrdenCirugia | null>(null);
  public motivoCancelacionTexto = signal<string>('');

  public nuevaCirugiaForm: CrearOrdenCirugiaRequest = {
    cuentaServicioId: '',
    pacienteId: '',
    descripcionCirugia: '',
    precioBaseUsd: 0,
    medicoId: '',
    fechaHoraProgramada: new Date().toISOString().slice(0, 16)
  };

  ngOnInit(): void {
    this.cargarOrdenes();
    this.cargarMedicos();
  }

  onFiltroEstadoChange(val: string): void {
    this.filtroEstado.set(val);
    this.cargarOrdenes();
  }

  cargarOrdenes(): void {
    this.pabellonService.getOrdenes(undefined, undefined, this.filtroEstado() || undefined).subscribe({
      next: (res) => this.ordenes.set(res || [])
    });
  }

  cargarMedicos(): void {
    this.medicoService.getAll().subscribe({
      next: (res: any[]) => this.medicos.set(res || [])
    });
  }

  public ordenesFiltradas = computed(() => {
    const list = this.ordenes();
    const txt = this.filtroTexto().trim().toLowerCase();
    if (!txt) return list;
    return list.filter(o => 
      o.pacienteNombre?.toLowerCase().includes(txt) ||
      o.pacienteCedula?.toLowerCase().includes(txt) ||
      o.descripcionCirugia?.toLowerCase().includes(txt)
    );
  });

  abrirModalConsumo(id: string): void {
    this.modalConsumoOrdenId.set(id);
  }

  abrirModalNuevaCirugia(): void {
    this.nuevaCirugiaForm = {
      cuentaServicioId: '',
      pacienteId: '',
      descripcionCirugia: '',
      precioBaseUsd: 0,
      medicoId: '',
      fechaHoraProgramada: new Date().toISOString().slice(0, 16)
    };
    this.modalNuevaCirugiaVisible.set(true);
  }

  esFormularioValido(): boolean {
    return !!(
      this.nuevaCirugiaForm.descripcionCirugia &&
      this.nuevaCirugiaForm.precioBaseUsd >= 0 &&
      this.nuevaCirugiaForm.medicoId &&
      this.nuevaCirugiaForm.cuentaServicioId &&
      this.nuevaCirugiaForm.pacienteId
    );
  }

  guardarNuevaCirugia(): void {
    this.pabellonService.crearOrden(this.nuevaCirugiaForm).subscribe({
      next: () => {
        this.modalNuevaCirugiaVisible.set(false);
        this.cargarOrdenes();
      }
    });
  }

  cambiarEstado(ordenId: string, nuevoEstado: string, motivo?: string): void {
    this.pabellonService.cambiarEstado({
      ordenCirugiaId: ordenId,
      nuevoEstado,
      motivoCancelacion: motivo
    }).subscribe({
      next: () => this.cargarOrdenes()
    });
  }

  solicitarCancelacion(orden: OrdenCirugia): void {
    this.motivoCancelacionTexto.set('');
    this.modalCancelacionOrden.set(orden);
  }

  confirmarCancelacion(): void {
    const orden = this.modalCancelacionOrden();
    const motivo = this.motivoCancelacionTexto().trim();
    if (orden && motivo) {
      this.cambiarEstado(orden.id, 'Cancelada', motivo);
      this.modalCancelacionOrden.set(null);
    }
  }
}
