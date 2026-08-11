import { Component, inject, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PabellonService, OrdenCirugia, CrearOrdenCirugiaRequest } from '../../../core/services/pabellon.service';
import { MedicoService } from '../../../core/services/medico.service';
import { GestionConsumoModalComponent } from './gestion-consumo-modal.component';
import { PabellonCalendarioComponent } from './pabellon-calendario.component';
import { PabellonMicroComponent } from './pabellon-micro.component';
import { ReprogramarModalComponent } from './reprogramar-modal.component';
import { HistorialObservacionesModalComponent } from './historial-observaciones-modal.component';
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
  Stethoscope,
  Grid,
  FileText,
  HeartPulse,
  Info
} from 'lucide-angular';

@Component({
  selector: 'app-pabellon-gestion',
  standalone: true,
  imports: [
    CommonModule, 
    FormsModule, 
    LucideAngularModule, 
    GestionConsumoModalComponent,
    PabellonCalendarioComponent,
    PabellonMicroComponent,
    ReprogramarModalComponent,
    HistorialObservacionesModalComponent
  ],
  template: `
    <div class="p-6 space-y-6 bg-gray-950 min-h-screen text-gray-100 font-sans">
      
      <!-- Top Action & Title Bar -->
      <div class="flex flex-col lg:flex-row justify-between items-start lg:items-center gap-4 bg-gray-900/60 p-4 rounded-xl border border-gray-800/80 backdrop-blur-md shadow-xl">
        <div class="flex items-center gap-3">
          <div class="p-3 bg-sky-500/10 border border-sky-500/20 rounded-xl text-sky-400">
            <lucide-icon name="stethoscope" class="w-7 h-7"></lucide-icon>
          </div>
          <div>
            <h1 class="text-2xl font-bold text-white tracking-tight flex items-center gap-2">
              Pizarra Digital Quirúrgica - Sat Hospitalario
            </h1>
            <p class="text-xs text-gray-400">Programación operativa de pabellón, trazabilidad de cirugías e insumos en tiempo real</p>
          </div>
        </div>

        <div class="flex flex-wrap items-center gap-3 w-full lg:w-auto">
          <!-- Switcher de Modos de Vista -->
          <div class="bg-gray-950 p-1 rounded-xl border border-gray-800 flex items-center gap-1">
            <button (click)="vistaModo.set('calendario')" [class.bg-sky-600]="vistaModo() === 'calendario'" [class.text-white]="vistaModo() === 'calendario'" [class.text-gray-400]="vistaModo() !== 'calendario'" class="px-3 py-1.5 rounded-lg text-xs font-semibold transition flex items-center gap-1.5">
              <lucide-icon name="grid" class="w-3.5 h-3.5"></lucide-icon>
              Pizarra Horaria
            </button>
            <button (click)="vistaModo.set('lista')" [class.bg-sky-600]="vistaModo() === 'lista'" [class.text-white]="vistaModo() === 'lista'" [class.text-gray-400]="vistaModo() !== 'lista'" class="px-3 py-1.5 rounded-lg text-xs font-semibold transition flex items-center gap-1.5">
              <lucide-icon name="list" class="w-3.5 h-3.5"></lucide-icon>
              Lista Micro
            </button>
          </div>

          <button (click)="abrirModalNuevaCirugia()" class="bg-sky-600 hover:bg-sky-500 text-white font-semibold text-xs px-4 py-2 rounded-xl shadow-lg shadow-sky-600/20 transition flex items-center gap-2">
            <lucide-icon name="plus" class="w-4 h-4"></lucide-icon>
            Programar Cirugía
          </button>
        </div>
      </div>

      <!-- Combined Filter & Search Bar -->
      <div class="flex flex-wrap items-center justify-between gap-3 bg-gray-900/40 p-3 rounded-xl border border-gray-800/80 backdrop-blur-sm">
        
        <div class="flex flex-wrap items-center gap-3 flex-1 min-w-[280px]">
          <!-- Input Búsqueda -->
          <div class="relative flex-1 min-w-[220px]">
            <lucide-icon name="search" class="w-4 h-4 text-gray-500 absolute left-3 top-1/2 -translate-y-1/2 pointer-events-none"></lucide-icon>
            <input type="text" [ngModel]="filtroTexto()" (ngModelChange)="filtroTexto.set($event)" placeholder="Buscar paciente, cédula, cirugía, doctor o especialidad..." class="w-full bg-gray-950 border border-gray-800 rounded-xl pl-9 pr-4 py-2 text-xs text-white placeholder-gray-500 focus:border-sky-500 focus:outline-none focus:ring-1 focus:ring-sky-500 transition">
          </div>

          <!-- Filtro de Estado -->
          <div class="relative min-w-[160px]">
            <lucide-icon name="filter" class="w-3.5 h-3.5 text-gray-500 absolute left-3 top-1/2 -translate-y-1/2 pointer-events-none"></lucide-icon>
            <select [ngModel]="filtroEstado()" (ngModelChange)="onFiltroEstadoChange($event)" class="w-full bg-gray-950 border border-gray-800 rounded-xl pl-8 pr-4 py-2 text-xs text-white focus:border-sky-500 focus:outline-none focus:ring-1 focus:ring-sky-500 appearance-none transition">
              <option value="">Todos los Estados</option>
              <option *ngFor="let est of estadosCirugiaCatalog()" [value]="est.id">
                <ng-container *ngIf="est.codigo">[{{ est.codigo }}] </ng-container>{{ est.nombre }}
              </option>
            </select>
          </div>

          <!-- Filtro por Médico -->
          <div class="relative min-w-[160px]">
            <lucide-icon name="user-check" class="w-3.5 h-3.5 text-gray-500 absolute left-3 top-1/2 -translate-y-1/2 pointer-events-none"></lucide-icon>
            <select [ngModel]="filtroMedicoId()" (ngModelChange)="filtroMedicoId.set($event)" class="w-full bg-gray-950 border border-gray-800 rounded-xl pl-8 pr-4 py-2 text-xs text-white focus:border-sky-500 focus:outline-none focus:ring-1 focus:ring-sky-500 appearance-none transition">
              <option value="">Todos los Médicos</option>
              <option *ngFor="let med of medicos()" [value]="med.id">{{ med.nombre }}</option>
            </select>
          </div>
        </div>

        <button (click)="cargarOrdenes()" class="p-2 bg-gray-800 hover:bg-gray-700 text-gray-300 rounded-xl transition" title="Refrescar Lista">
          <lucide-icon name="refresh-ccw" class="w-4 h-4"></lucide-icon>
        </button>
      </div>

      <!-- VISTA 1: CALENDARIO / PIZARRA HORARIA -->
      <div *ngIf="vistaModo() === 'calendario'">
        <app-pabellon-calendario
          [cirugias]="ordenesFiltradas()"
          (seleccionarCirugia)="ordenSeleccionada.set($event)"
        ></app-pabellon-calendario>
      </div>

      <!-- VISTA 2: LISTA MICRO CON CHECKLIST DE REQUISITOS DB-DRIVEN -->
      <div *ngIf="vistaModo() === 'lista'">
        <app-pabellon-micro
          [cirugias]="ordenesFiltradas()"
          (cambiarEstado)="onCambiarEstado($event)"
          (abrirReprogramar)="modalReprogramarCirugia.set($event)"
          (abrirHistorial)="modalHistorialCirugia.set($event)"
          (toggleReq)="onToggleRequisito($event)"
        ></app-pabellon-micro>
      </div>

      <!-- MODAL REPROGRAMAR CIRUGÍA -->
      <app-reprogramar-modal
        *ngIf="modalReprogramarCirugia()"
        [cirugia]="modalReprogramarCirugia()!"
        (cerrar)="modalReprogramarCirugia.set(null)"
        (confirmar)="onConfirmarReprogramacion($event)"
      ></app-reprogramar-modal>

      <!-- MODAL HISTORIAL DE OBSERVACIONES -->
      <app-historial-observaciones-modal
        *ngIf="modalHistorialCirugia()"
        [pacienteNombre]="modalHistorialCirugia()!.pacienteNombre"
        [historial]="modalHistorialCirugia()!.historialObservaciones || []"
        (cerrar)="modalHistorialCirugia.set(null)"
      ></app-historial-observaciones-modal>

      <!-- Modal Nueva Cirugía -->
      <div *ngIf="modalNuevaCirugiaVisible()" class="fixed inset-0 bg-black/80 backdrop-blur-sm z-50 flex items-center justify-center p-4">
        <div class="bg-gray-900 border border-gray-800 rounded-2xl shadow-2xl w-full max-w-xl p-6 space-y-4 text-white">
          <div class="flex justify-between items-center border-b border-gray-800 pb-3">
            <h3 class="text-base font-bold text-sky-400 flex items-center gap-2">
              <lucide-icon name="plus" class="w-5 h-5"></lucide-icon>
              Programar Nueva Cirugía
            </h3>
            <button (click)="modalNuevaCirugiaVisible.set(false)" class="text-gray-400 hover:text-white transition">
              <lucide-icon name="x-circle" class="w-5 h-5"></lucide-icon>
            </button>
          </div>

          <div class="space-y-3 text-xs">
            <div>
              <label class="text-gray-400 mb-1 block">Descripción del Procedimiento Quirúrgico *</label>
              <input type="text" [(ngModel)]="nuevaCirugiaForm.descripcionCirugia" placeholder="Ej: Colecistectomía Laparoscópica" class="w-full bg-gray-950 border border-gray-700 rounded-xl px-3 py-2 text-white focus:outline-none focus:border-sky-500">
            </div>

            <div class="grid grid-cols-2 gap-3">
              <div>
                <label class="text-gray-400 mb-1 block">Especialidad Quirúrgica</label>
                <input type="text" [(ngModel)]="nuevaCirugiaForm.especialidad" placeholder="Ej: Cirugía General" class="w-full bg-gray-950 border border-gray-700 rounded-xl px-3 py-2 text-white focus:outline-none focus:border-sky-500">
              </div>
              <div>
                <label class="text-gray-400 mb-1 block">Precio Base USD ($)</label>
                <input type="number" [(ngModel)]="nuevaCirugiaForm.precioBaseUsd" class="w-full bg-gray-950 border border-gray-700 rounded-xl px-3 py-2 text-white focus:outline-none focus:border-sky-500">
              </div>
            </div>

            <div>
              <label class="text-gray-400 mb-1 block">Razón / Motivo de la Cirugía *</label>
              <textarea [(ngModel)]="nuevaCirugiaForm.razonCirugia" rows="2" placeholder="Indicación clínica o diagnóstico justificante..." class="w-full bg-gray-950 border border-gray-700 rounded-xl px-3 py-2 text-white focus:outline-none focus:border-sky-500"></textarea>
            </div>

            <div class="grid grid-cols-2 gap-3">
              <div>
                <label class="text-gray-400 mb-1 block">Fecha y Hora Programada *</label>
                <input type="datetime-local" [(ngModel)]="nuevaCirugiaForm.fechaHoraProgramada" class="w-full bg-gray-950 border border-gray-700 rounded-xl px-3 py-2 text-white focus:outline-none focus:border-sky-500">
              </div>
              <div>
                <label class="text-gray-400 mb-1 block">Médico Cirujano *</label>
                <select [(ngModel)]="nuevaCirugiaForm.medicoId" class="w-full bg-gray-950 border border-gray-700 rounded-xl px-3 py-2 text-white focus:outline-none focus:border-sky-500">
                  <option value="">Seleccione Cirujano</option>
                  <option *ngFor="let med of medicos()" [value]="med.id">{{ med.nombre }}</option>
                </select>
              </div>
            </div>

            <div class="grid grid-cols-2 gap-3">
              <div>
                <label class="text-gray-400 mb-1 block">ID Cuenta Servicio *</label>
                <input type="text" [(ngModel)]="nuevaCirugiaForm.cuentaServicioId" placeholder="Guid cuenta" class="w-full bg-gray-950 border border-gray-700 rounded-xl px-3 py-2 text-white focus:outline-none focus:border-sky-500">
              </div>
              <div>
                <label class="text-gray-400 mb-1 block">ID Paciente *</label>
                <input type="text" [(ngModel)]="nuevaCirugiaForm.pacienteId" placeholder="Guid paciente" class="w-full bg-gray-950 border border-gray-700 rounded-xl px-3 py-2 text-white focus:outline-none focus:border-sky-500">
              </div>
            </div>
          </div>

          <div class="flex justify-end gap-2 pt-3 border-t border-gray-800">
            <button (click)="modalNuevaCirugiaVisible.set(false)" class="bg-gray-800 hover:bg-gray-700 text-gray-300 text-xs px-4 py-2 rounded-xl">Cancelar</button>
            <button (click)="guardarNuevaCirugia()" [disabled]="!esFormularioValido()" class="bg-sky-600 hover:bg-sky-500 text-white text-xs font-semibold px-4 py-2 rounded-xl disabled:opacity-50 transition shadow-lg shadow-sky-600/20">
              Guardar y Programar
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
  public medicos = signal<{ id: string; nombre: string; especialidad?: string; activo?: boolean }[]>([]);
  public filtroTexto = signal<string>('');
  public filtroEstado = signal<string>('');
  public filtroMedicoId = signal<string>('');
  public ordenSeleccionada = signal<OrdenCirugia | null>(null);

  public modalConsumoOrdenId = signal<string | null>(null);
  public modalNuevaCirugiaVisible = signal<boolean>(false);
  public modalReprogramarCirugia = signal<OrdenCirugia | null>(null);
  public modalHistorialCirugia = signal<OrdenCirugia | null>(null);

  public nuevaCirugiaForm: CrearOrdenCirugiaRequest = {
    cuentaServicioId: '',
    pacienteId: '',
    descripcionCirugia: '',
    precioBaseUsd: 0,
    medicoId: '',
    fechaHoraProgramada: new Date().toISOString().slice(0, 16),
    especialidad: '',
    razonCirugia: ''
  };

  public estadosCirugiaCatalog = signal<any[]>([]);

  ngOnInit(): void {
    this.pabellonService.getEstadosCirugia().subscribe({
      next: (estados) => this.estadosCirugiaCatalog.set(estados || [])
    });
    this.cargarOrdenes();
    this.cargarMedicos();
  }

  onFiltroEstadoChange(val: string): void {
    this.filtroEstado.set(val);
    this.cargarOrdenes();
  }

  cargarOrdenes(): void {
    this.pabellonService.getOrdenes(undefined, undefined, this.filtroEstado() || undefined).subscribe({
      next: (res) => {
        const list = res || [];
        this.ordenes.set(list);
        if (list.length > 0 && !this.ordenSeleccionada()) {
          this.ordenSeleccionada.set(list[0]);
        }
      }
    });
  }

  cargarMedicos(): void {
    this.medicoService.getAll().subscribe({
      next: (res: any[]) => this.medicos.set(res || [])
    });
  }

  public ordenesFiltradas = computed(() => {
    let list = this.ordenes();
    const txt = this.filtroTexto().trim().toLowerCase();
    const medId = this.filtroMedicoId();

    if (medId) {
      list = list.filter(o => o.medicoId === medId);
    }

    if (txt) {
      list = list.filter(o =>
        (o.pacienteNombre || '').toLowerCase().includes(txt) ||
        (o.pacienteCedula || '').toLowerCase().includes(txt) ||
        (o.descripcionCirugia || '').toLowerCase().includes(txt) ||
        (o.medicoNombre || '').toLowerCase().includes(txt)
      );
    }

    return list;
  });

  onCambiarEstado(event: { cirugiaId: string; nuevoEstado: string }): void {
    this.pabellonService.cambiarEstado({
      ordenCirugiaId: event.cirugiaId,
      nuevoEstado: event.nuevoEstado
    }).subscribe({
      next: () => this.cargarOrdenes()
    });
  }

  onConfirmarReprogramacion(event: { nuevaFechaHora: string; motivo: string }): void {
    const target = this.modalReprogramarCirugia();
    if (!target) return;

    this.pabellonService.reprogramarCirugia({
      ordenCirugiaId: target.id,
      nuevaFechaHora: event.nuevaFechaHora,
      motivo: event.motivo
    }).subscribe({
      next: () => {
        this.modalReprogramarCirugia.set(null);
        this.cargarOrdenes();
      }
    });
  }

  onToggleRequisito(event: { ordenId: string; requisitoId: string; cumplido: boolean }): void {
    this.pabellonService.toggleRequisito(event.ordenId, event.requisitoId, event.cumplido).subscribe({
      next: () => this.cargarOrdenes()
    });
  }

  abrirModalNuevaCirugia(): void {
    this.nuevaCirugiaForm = {
      cuentaServicioId: '',
      pacienteId: '',
      descripcionCirugia: '',
      precioBaseUsd: 0,
      medicoId: '',
      fechaHoraProgramada: new Date().toISOString().slice(0, 16),
      especialidad: '',
      razonCirugia: ''
    };
    this.modalNuevaCirugiaVisible.set(true);
  }

  esFormularioValido(): boolean {
    return !!(
      this.nuevaCirugiaForm.descripcionCirugia.trim() &&
      this.nuevaCirugiaForm.medicoId &&
      this.nuevaCirugiaForm.cuentaServicioId.trim() &&
      this.nuevaCirugiaForm.pacienteId.trim() &&
      this.nuevaCirugiaForm.fechaHoraProgramada
    );
  }

  guardarNuevaCirugia(): void {
    if (!this.esFormularioValido()) return;

    this.pabellonService.crearOrden(this.nuevaCirugiaForm).subscribe({
      next: () => {
        this.modalNuevaCirugiaVisible.set(false);
        this.cargarOrdenes();
      }
    });
  }
}
