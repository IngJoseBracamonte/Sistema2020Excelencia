import { Component, inject, OnInit, signal, computed, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { Router, NavigationEnd } from '@angular/router';
import { filter } from 'rxjs';
import { LucideAngularModule } from 'lucide-angular';
import {
  PabellonService,
  PacienteQuirurgicoItem,
  CirugiaCalendarioItem,
  CrearOrdenCirugiaRequest
} from '../../../core/services/pabellon.service';
import { MedicoService } from '../../../core/services/medico.service';
import { PatientService, PatientRecord } from '../../../core/services/patient.service';
import { environment } from '../../../../environments/environment';

import { PabellonCalendarioComponent } from './pabellon-calendario.component';
import { PabellonPacientesListaComponent } from './pabellon-pacientes-lista.component';
import { PanelDetalleCirugiaComponent } from './panel-detalle-cirugia.component';

@Component({
  selector: 'app-pabellon-gestion',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    LucideAngularModule,
    PabellonCalendarioComponent,
    PabellonPacientesListaComponent,
    PanelDetalleCirugiaComponent
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="p-6 space-y-6 bg-gray-950 min-h-screen text-gray-100 font-sans">
      
      <!-- Top Action & Title Bar -->
      <div class="flex flex-col lg:flex-row justify-between items-start lg:items-center gap-4 bg-gray-900/60 p-4 rounded-2xl border border-gray-800/80 backdrop-blur-md shadow-xl">
        <div class="flex items-center gap-3">
          <div class="p-3 bg-sky-500/10 border border-sky-500/20 rounded-2xl text-sky-400">
            <lucide-icon name="stethoscope" class="w-7 h-7"></lucide-icon>
          </div>
          <div>
            <h1 class="text-xl font-bold text-white tracking-tight flex items-center gap-2">
              Pabellón Quirúrgico & Gestión de Cirugías
            </h1>
            <p class="text-xs text-gray-400">Programación operativa, checklist preoperatorio y honorarios médicos</p>
          </div>
        </div>

        <div class="flex flex-wrap items-center gap-3 w-full lg:w-auto">
          <!-- Switcher de Modos de Vista Principal -->
          <div class="bg-gray-950 p-1 rounded-xl border border-gray-800 flex items-center gap-1">
            <button
              (click)="cambiarModoVista('tablero')"
              [class.bg-sky-600]="vistaModo() === 'tablero'"
              [class.text-white]="vistaModo() === 'tablero'"
              [class.text-gray-400]="vistaModo() !== 'tablero'"
              class="px-3 py-1.5 rounded-lg text-xs font-semibold transition flex items-center gap-1.5"
            >
              <lucide-icon name="layout-list" class="w-3.5 h-3.5"></lucide-icon>
              Tablero de Pacientes
            </button>

            <button
              (click)="cambiarModoVista('calendario')"
              [class.bg-sky-600]="vistaModo() === 'calendario'"
              [class.text-white]="vistaModo() === 'calendario'"
              [class.text-gray-400]="vistaModo() !== 'calendario'"
              class="px-3 py-1.5 rounded-lg text-xs font-semibold transition flex items-center gap-1.5"
            >
              <lucide-icon name="calendar-days" class="w-3.5 h-3.5"></lucide-icon>
              Calendario Total
            </button>
          </div>

          <button
            (click)="abrirModalNuevaCirugia()"
            class="bg-sky-600 hover:bg-sky-500 text-white font-semibold text-xs px-4 py-2 rounded-xl shadow-lg shadow-sky-600/20 transition flex items-center gap-2"
          >
            <lucide-icon name="plus" class="w-4 h-4"></lucide-icon>
            Programar Cirugía
          </button>
        </div>
      </div>

      <!-- VISTA 1: TABLERO Y LISTA MAESTRA DE PACIENTES -->
      @if (vistaModo() === 'tablero') {
        <app-pabellon-pacientes-lista
          [pacientes]="pacientesQuirurgicos()"
          (seleccionarPaciente)="onSeleccionarPaciente($event)"
          (nuevaCirugia)="abrirModalNuevaCirugia()"
          (filtrar)="onFiltrarPacientes($event)"
        ></app-pabellon-pacientes-lista>
      }

      <!-- VISTA 2: CALENDARIO QUIRÚRGICO TOTAL -->
      @if (vistaModo() === 'calendario') {
        <app-pabellon-calendario
          [cirugias]="calendarioItems()"
          (seleccionarCirugia)="onSeleccionarDeCalendario($event)"
        ></app-pabellon-calendario>
      }

      <!-- PANEL LATERAL CONTEXTUAL POR ROL (DRAWER) -->
      @if (pacienteSeleccionadoDetalle()) {
        <app-panel-detalle-cirugia
          [paciente]="pacienteSeleccionadoDetalle()"
          (cerrar)="pacienteSeleccionadoDetalle.set(null)"
          (recargar)="recargarDatos()"
        ></app-panel-detalle-cirugia>
      }

      <!-- MODAL: PROGRAMAR NUEVA CIRUGÍA -->
      @if (modalNuevaCirugiaVisible()) {
        <div class="fixed inset-0 bg-black/80 backdrop-blur-sm z-50 flex items-center justify-center p-4">
          <div class="bg-gray-900 border border-gray-800 rounded-2xl shadow-2xl w-full max-w-xl p-6 space-y-4 text-white animate-fade-in">
            <div class="flex justify-between items-center border-b border-gray-800 pb-3">
              <h3 class="text-base font-bold text-sky-400 flex items-center gap-2">
                <lucide-icon name="plus" class="w-5 h-5"></lucide-icon>
                Programar Intervención Quirúrgica
              </h3>
              <button (click)="modalNuevaCirugiaVisible.set(false)" class="text-gray-400 hover:text-white transition">
                <lucide-icon name="x" class="w-5 h-5"></lucide-icon>
              </button>
            </div>

            <div class="space-y-3 text-xs">
              <!-- Descripción de la Cirugía -->
              <div>
                <label class="text-gray-400 mb-1 block font-semibold">Descripción del Procedimiento Quirúrgico *</label>
                <input
                  type="text"
                  [(ngModel)]="nuevaCirugiaForm.descripcionCirugia"
                  placeholder="Ej: Apendicectomía Laparoscópica"
                  class="w-full bg-gray-950 border border-gray-700 rounded-xl px-3 py-2 text-white focus:outline-none focus:border-sky-500"
                />
              </div>

              <!-- Sala / Quirófano y Modalidad de Anestesia -->
              <div class="grid grid-cols-2 gap-3">
                <div>
                  <label class="text-gray-400 mb-1 block font-semibold">Sala / Quirófano *</label>
                  <select [(ngModel)]="nuevaCirugiaForm.salaQuirofano" class="w-full bg-gray-950 border border-gray-700 rounded-xl px-3 py-2 text-white focus:outline-none focus:border-sky-500">
                    <option value="Quirófano 1">Quirófano 1</option>
                    <option value="Quirófano 2">Quirófano 2</option>
                    <option value="Sala de Partos">Sala de Partos</option>
                  </select>
                </div>

                <div>
                  <label class="text-gray-400 mb-1 block font-semibold">Modalidad de Anestesia</label>
                  <select [(ngModel)]="nuevaCirugiaForm.modalidadAnestesia" class="w-full bg-gray-950 border border-gray-700 rounded-xl px-3 py-2 text-white focus:outline-none focus:border-sky-500">
                    <option value="General">General</option>
                    <option value="Epidural / Raquídea">Epidural / Raquídea</option>
                    <option value="Sedación Local">Sedación Local</option>
                    <option value="Bloqueo Regional">Bloqueo Regional</option>
                  </select>
                </div>
              </div>

              <!-- Tarifas Administrativas Iniciales -->
              <div class="grid grid-cols-2 gap-3">
                <div>
                  <label class="text-gray-400 mb-1 block font-semibold">Derecho de Sala (USD $)</label>
                  <input
                    type="number"
                    step="0.01"
                    [(ngModel)]="nuevaCirugiaForm.precioDerechoSalaUsd"
                    class="w-full bg-gray-950 border border-gray-700 rounded-xl px-3 py-2 text-white font-mono focus:outline-none focus:border-sky-500"
                  />
                </div>
                <div>
                  <label class="text-gray-400 mb-1 block font-semibold">Precio Base Cirugía (USD $)</label>
                  <input
                    type="number"
                    step="0.01"
                    [(ngModel)]="nuevaCirugiaForm.precioBaseUsd"
                    class="w-full bg-gray-950 border border-gray-700 rounded-xl px-3 py-2 text-white font-mono focus:outline-none focus:border-sky-500"
                  />
                </div>
              </div>

              <!-- Flag Alquilado -->
              <div class="flex items-center gap-2 pt-1">
                <input
                  type="checkbox"
                  id="nuevoAlquiladoCheck"
                  [(ngModel)]="nuevaCirugiaForm.esAlquilado"
                  class="w-4 h-4 rounded text-sky-600 bg-gray-950 border-gray-700 cursor-pointer"
                />
                <label for="nuevoAlquiladoCheck" class="text-gray-300 font-medium cursor-pointer">
                  Pabellón Alquilado (Médico Externo / Tarifa Solo Sala)
                </label>
              </div>

              <!-- Fecha Programada y Cirujano Principal -->
              <div class="grid grid-cols-2 gap-3">
                <div>
                  <label class="text-gray-400 mb-1 block font-semibold">Fecha y Hora Programada *</label>
                  <input
                    type="datetime-local"
                    [(ngModel)]="nuevaCirugiaForm.fechaHoraProgramada"
                    class="w-full bg-gray-950 border border-gray-700 rounded-xl px-3 py-2 text-white focus:outline-none focus:border-sky-500"
                  />
                </div>
                <div>
                  <label class="text-gray-400 mb-1 block font-semibold">Cirujano Principal *</label>
                  <select [(ngModel)]="nuevaCirugiaForm.medicoId" class="w-full bg-gray-950 border border-gray-700 rounded-xl px-3 py-2 text-white focus:outline-none focus:border-sky-500">
                    <option value="">Seleccione Cirujano</option>
                    @for (med of medicos(); track med.id) {
                      <option [value]="med.id">{{ med.nombre }}</option>
                    }
                  </select>
                </div>
              </div>

              <!-- Buscador de Pacientes DB-Driven -->
              <div>
                <label class="text-gray-400 mb-1 block font-semibold">Paciente (Búsqueda por Cédula o Nombre) *</label>
                <div class="relative">
                  <input
                    type="text"
                    [ngModel]="pacienteSearchQuery()"
                    (ngModelChange)="buscarPacientes($event)"
                    placeholder="Escriba cédula o nombre del paciente..."
                    class="w-full bg-gray-950 border border-gray-700 rounded-xl px-3 py-2 text-white focus:outline-none focus:border-sky-500"
                  />
                  @if (pacientesEncontrados().length > 0) {
                    <div class="absolute left-0 right-0 top-full mt-1 bg-gray-900 border border-gray-700 rounded-xl shadow-2xl z-50 max-h-48 overflow-y-auto divide-y divide-gray-800">
                      @for (p of pacientesEncontrados(); track p.id) {
                        <div (click)="seleccionarPaciente(p)" class="p-3 hover:bg-sky-600/20 cursor-pointer flex justify-between items-center transition">
                          <div>
                            <span class="text-xs font-bold text-white block">{{ p.nombre }} {{ p.apellidos || '' }}</span>
                            <span class="text-[10px] text-gray-400">CI: {{ p.cedula }}</span>
                          </div>
                          <span class="text-[10px] bg-sky-500/20 text-sky-400 px-2 py-0.5 rounded-md font-semibold">Seleccionar</span>
                        </div>
                      }
                    </div>
                  }
                </div>

                @if (pacienteSeleccionado()) {
                  <div class="mt-2 p-2.5 bg-sky-500/10 border border-sky-500/20 rounded-xl flex items-center justify-between text-xs text-sky-300">
                    <span>👤 {{ pacienteSeleccionado()?.nombre }} (CI: {{ pacienteSeleccionado()?.cedula }})</span>
                    <span class="text-[10px] text-gray-400 font-mono">Cuenta: {{ nuevaCirugiaForm.cuentaServicioId ? 'Vinculada' : 'Pendiente' }}</span>
                  </div>
                }
              </div>
            </div>

            <div class="flex justify-end gap-2 pt-3 border-t border-gray-800">
              <button (click)="modalNuevaCirugiaVisible.set(false)" class="bg-gray-800 hover:bg-gray-700 text-gray-300 text-xs px-4 py-2 rounded-xl">
                Cancelar
              </button>
              <button
                (click)="guardarNuevaCirugia()"
                [disabled]="!esFormularioValido()"
                class="bg-sky-600 hover:bg-sky-500 text-white text-xs font-semibold px-4 py-2 rounded-xl disabled:opacity-50 transition shadow-lg shadow-sky-600/20"
              >
                Guardar y Programar
              </button>
            </div>
          </div>
        </div>
      }

    </div>
  `
})
export class PabellonGestionComponent implements OnInit {
  private pabellonService = inject(PabellonService);
  private medicoService = inject(MedicoService);
  private patientService = inject(PatientService);
  private http = inject(HttpClient);
  private router = inject(Router);

  // Estados Reactivos con Signals
  public vistaModo = signal<'tablero' | 'calendario'>('tablero');
  public pacientesQuirurgicos = signal<PacienteQuirurgicoItem[]>([]);
  public calendarioItems = signal<CirugiaCalendarioItem[]>([]);
  public pacienteSeleccionadoDetalle = signal<PacienteQuirurgicoItem | null>(null);

  public medicos = signal<{ id: string; nombre: string; especialidad?: string; activo?: boolean }[]>([]);

  // Búsqueda de Pacientes
  public pacienteSearchQuery = signal<string>('');
  public pacientesEncontrados = signal<PatientRecord[]>([]);
  public pacienteSeleccionado = signal<PatientRecord | null>(null);
  public modalNuevaCirugiaVisible = signal<boolean>(false);

  public nuevaCirugiaForm: CrearOrdenCirugiaRequest = {
    cuentaServicioId: '',
    pacienteId: '',
    descripcionCirugia: '',
    precioBaseUsd: 0,
    medicoId: '',
    fechaHoraProgramada: new Date().toISOString().slice(0, 16),
    salaQuirofano: 'Quirófano 1',
    modalidadAnestesia: 'General',
    esAlquilado: false,
    precioDerechoSalaUsd: 0
  };

  ngOnInit(): void {
    this.sincronizarVistaConRuta();
    this.router.events.pipe(
      filter((e): e is NavigationEnd => e instanceof NavigationEnd)
    ).subscribe(() => {
      this.sincronizarVistaConRuta();
    });

    this.recargarDatos();
    this.cargarMedicos();
  }

  private sincronizarVistaConRuta(): void {
    const url = this.router.url;
    if (url.includes('/calendario')) {
      this.vistaModo.set('calendario');
    } else {
      this.vistaModo.set('tablero');
    }
  }

  public cambiarModoVista(modo: 'tablero' | 'calendario'): void {
    this.vistaModo.set(modo);
    this.router.navigate(['/pabellon', modo]);
  }

  recargarDatos(): void {
    this.pabellonService.getPacientesQuirurgicos().subscribe(items => {
      this.pacientesQuirurgicos.set(items || []);
      // Actualizar detalle si está abierto
      const abierto = this.pacienteSeleccionadoDetalle();
      if (abierto) {
        const actualizado = (items || []).find(i => i.id === abierto.id);
        if (actualizado) {
          this.pacienteSeleccionadoDetalle.set(actualizado);
        }
      }
    });

    this.pabellonService.getCalendario().subscribe(items => {
      this.calendarioItems.set(items || []);
    });
  }

  cargarMedicos(): void {
    this.medicoService.getAll().subscribe({
      next: (res: any[]) => this.medicos.set(res || [])
    });
  }

  onFiltrarPacientes(filtros: { busqueda: string; estado: string }): void {
    this.pabellonService.getPacientesQuirurgicos(filtros.busqueda, filtros.estado).subscribe(items => {
      this.pacientesQuirurgicos.set(items || []);
    });
  }

  onSeleccionarPaciente(p: PacienteQuirurgicoItem): void {
    this.pacienteSeleccionadoDetalle.set(p);
  }

  onSeleccionarDeCalendario(cal: CirugiaCalendarioItem): void {
    const p = this.pacientesQuirurgicos().find(item => item.id === cal.id);
    if (p) {
      this.pacienteSeleccionadoDetalle.set(p);
    } else {
      this.pabellonService.getPacientesQuirurgicos().subscribe(items => {
        this.pacientesQuirurgicos.set(items || []);
        const found = items.find(i => i.id === cal.id);
        if (found) this.pacienteSeleccionadoDetalle.set(found);
      });
    }
  }

  buscarPacientes(term: string): void {
    this.pacienteSearchQuery.set(term);
    if (!term || term.trim().length < 2) {
      this.pacientesEncontrados.set([]);
      return;
    }
    this.patientService.searchPatients(term.trim()).subscribe({
      next: (res) => this.pacientesEncontrados.set(res || []),
      error: () => this.pacientesEncontrados.set([])
    });
  }

  seleccionarPaciente(p: PatientRecord): void {
    this.pacienteSeleccionado.set(p);
    this.nuevaCirugiaForm.pacienteId = p.id;
    this.pacientesEncontrados.set([]);
    this.pacienteSearchQuery.set(`${p.nombre} ${p.apellidos || ''} (${p.cedula})`);

    // Vincular cuenta de hospitalización o emergencia activa
    this.http.get<any[]>(`${environment.apiUrl}/api/Enfermeria/cuentas-activas`).subscribe({
      next: (cuentas) => {
        const cuentaPaciente = (cuentas || []).find(c => c.pacienteId === p.id || c.pacienteCedula === p.cedula);
        if (cuentaPaciente) {
          this.nuevaCirugiaForm.cuentaServicioId = cuentaPaciente.cuentaId || cuentaPaciente.id;
        } else {
          this.nuevaCirugiaForm.cuentaServicioId = p.id;
        }
      },
      error: () => {
        this.nuevaCirugiaForm.cuentaServicioId = p.id;
      }
    });
  }

  abrirModalNuevaCirugia(): void {
    this.pacienteSearchQuery.set('');
    this.pacientesEncontrados.set([]);
    this.pacienteSeleccionado.set(null);
    this.nuevaCirugiaForm = {
      cuentaServicioId: '',
      pacienteId: '',
      descripcionCirugia: '',
      precioBaseUsd: 0,
      medicoId: '',
      fechaHoraProgramada: new Date().toISOString().slice(0, 16),
      salaQuirofano: 'Quirófano 1',
      modalidadAnestesia: 'General',
      esAlquilado: false,
      precioDerechoSalaUsd: 0
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
        this.recargarDatos();
      }
    });
  }
}
