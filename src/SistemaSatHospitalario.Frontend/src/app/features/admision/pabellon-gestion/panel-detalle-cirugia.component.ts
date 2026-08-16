import { Component, ChangeDetectionStrategy, input, output, signal, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LucideAngularModule } from 'lucide-angular';
import {
  PabellonService,
  PacienteQuirurgicoItem,
  MedicoHonorarioItem,
  OrdenCirugiaRequisito
} from '../../../core/services/pabellon.service';
import { MedicoService, Medico } from '../../../core/services/medico.service';
import { InventoryService } from '../../../core/services/inventory.service';
import { Insumo } from '../../../core/models/inventory.model';

@Component({
  selector: 'app-panel-detalle-cirugia',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    @if (paciente()) {
      <div class="fixed inset-0 z-50 overflow-hidden bg-black/60 backdrop-blur-sm flex justify-end animate-fade-in">
        <div class="w-full max-w-2xl bg-gray-950 border-l border-gray-800 shadow-2xl h-full flex flex-col justify-between overflow-y-auto">
          
          <!-- Encabezado del Panel -->
          <div class="p-5 border-b border-gray-800/80 bg-gray-900/60 sticky top-0 z-10 backdrop-blur-md">
            <div class="flex justify-between items-start">
              <div>
                <div class="flex items-center gap-2 mb-1">
                  <span class="text-xs font-mono font-bold text-sky-400 bg-sky-500/10 px-2 py-0.5 rounded border border-sky-500/20">
                    {{ paciente()!.salaQuirofano || 'Quirófano 1' }}
                  </span>
                  <span class="text-[10px] font-extrabold px-2.5 py-0.5 rounded-full uppercase" [ngClass]="getEstadoClass(paciente()!.estado)">
                    {{ paciente()!.estado }}
                  </span>
                  @if (paciente()!.esAlquilado) {
                    <span class="text-[10px] font-bold text-amber-400 bg-amber-500/10 px-2 py-0.5 rounded border border-amber-500/20">
                      Pabellón Alquilado
                    </span>
                  }
                </div>
                <h3 class="text-base font-bold text-white">{{ paciente()!.descripcionCirugia }}</h3>
                <p class="text-xs text-gray-400">
                  Paciente: <strong class="text-gray-200">{{ paciente()!.pacienteNombre }}</strong> ({{ paciente()!.pacienteCedula }}) | Ubicación: <strong class="text-purple-300">{{ paciente()!.ubicacion.descripcionCompleta }}</strong>
                </p>
              </div>

              <button (click)="cerrar.emit()" class="p-1.5 text-gray-400 hover:text-white rounded-lg hover:bg-gray-800 transition">
                <lucide-icon name="x" class="w-5 h-5"></lucide-icon>
              </button>
            </div>

            <!-- Selector de Pestañas por Rol -->
            <div class="flex items-center gap-1 mt-4 bg-gray-950 p-1 rounded-xl border border-gray-800/80 text-xs">
              <button
                (click)="tabActivo.set('clinica')"
                [class.bg-sky-600]="tabActivo() === 'clinica'"
                [class.text-white]="tabActivo() === 'clinica'"
                [class.text-gray-400]="tabActivo() !== 'clinica'"
                class="flex-1 py-1.5 rounded-lg font-semibold transition flex items-center justify-center gap-1.5"
              >
                <lucide-icon name="clipboard-check" class="w-3.5 h-3.5"></lucide-icon>
                Checklist Preop
              </button>
              <button
                (click)="tabActivo.set('insumos')"
                [class.bg-sky-600]="tabActivo() === 'insumos'"
                [class.text-white]="tabActivo() === 'insumos'"
                [class.text-gray-400]="tabActivo() !== 'insumos'"
                class="flex-1 py-1.5 rounded-lg font-semibold transition flex items-center justify-center gap-1.5"
              >
                <lucide-icon name="boxes" class="w-3.5 h-3.5"></lucide-icon>
                Insumos & Kits
              </button>
              <button
                (click)="tabActivo.set('honorarios')"
                [class.bg-sky-600]="tabActivo() === 'honorarios'"
                [class.text-white]="tabActivo() === 'honorarios'"
                [class.text-gray-400]="tabActivo() !== 'honorarios'"
                class="flex-1 py-1.5 rounded-lg font-semibold transition flex items-center justify-center gap-1.5"
              >
                <lucide-icon name="dollar-sign" class="w-3.5 h-3.5"></lucide-icon>
                Honorarios & Precios
              </button>
              <button
                (click)="tabActivo.set('logs')"
                [class.bg-sky-600]="tabActivo() === 'logs'"
                [class.text-white]="tabActivo() === 'logs'"
                [class.text-gray-400]="tabActivo() !== 'logs'"
                class="flex-1 py-1.5 rounded-lg font-semibold transition flex items-center justify-center gap-1.5"
              >
                <lucide-icon name="history" class="w-3.5 h-3.5"></lucide-icon>
                Auditoría
              </button>
            </div>
          </div>

          <!-- Contenido de las Pestañas -->
          <div class="p-5 flex-1 space-y-4">
            
            <!-- PESTAÑA 1: FICHA CLÍNICA & CHECKLIST PREOPERATORIO -->
            @if (tabActivo() === 'clinica') {
              <div class="space-y-4 animate-fade-in">
                <!-- Indicador de Aptitud -->
                <div class="p-3.5 rounded-xl border flex items-center justify-between" [ngClass]="esApto() ? 'bg-emerald-500/10 border-emerald-500/20 text-emerald-300' : 'bg-amber-500/10 border-amber-500/20 text-amber-300'">
                  <div class="flex items-center gap-2.5">
                    <lucide-icon [name]="esApto() ? 'check-circle-2' : 'alert-triangle'" class="w-5 h-5"></lucide-icon>
                    <div>
                      <h4 class="text-xs font-bold">{{ esApto() ? 'Paciente Apto para Ingresar a Quirófano' : 'Requisitos Quirúrgicos Pendientes' }}</h4>
                      <p class="text-[11px] opacity-80">{{ getRequisitosCumplidos() }} de {{ paciente()!.requisitos.length }} requisitos completados.</p>
                    </div>
                  </div>
                  <span class="text-xs font-mono font-bold">{{ getRequisitosCumplidos() }}/{{ paciente()!.requisitos.length }}</span>
                </div>

                <!-- Lista de Requisitos / Checklist Interactivo -->
                <div class="bg-gray-900/60 p-4 rounded-xl border border-gray-800 space-y-2.5">
                  <h4 class="text-xs font-bold text-gray-300 uppercase tracking-wider mb-2">Checklist Preoperatorio DB-Driven</h4>
                  @for (req of paciente()!.requisitos; track req.id) {
                    <div class="flex items-center justify-between p-2.5 rounded-lg bg-gray-950/80 border border-gray-800 hover:border-gray-700 transition">
                      <div class="flex items-center gap-2.5">
                        <input
                          type="checkbox"
                          [checked]="req.cumplido"
                          (change)="toggleRequisito(req)"
                          class="w-4 h-4 rounded text-sky-600 bg-gray-900 border-gray-700 focus:ring-sky-500 cursor-pointer"
                        />
                        <div>
                          <span class="text-xs font-medium text-white block" [class.line-through]="req.cumplido">{{ req.nombre }}</span>
                          @if (req.descripcion) {
                            <span class="text-[10px] text-gray-400 block">{{ req.descripcion }}</span>
                          }
                        </div>
                      </div>
                      @if (req.cumplido && req.verificadoPor) {
                        <span class="text-[10px] text-emerald-400 bg-emerald-500/10 px-2 py-0.5 rounded border border-emerald-500/20 font-mono">
                          Por: {{ req.verificadoPor }}
                        </span>
                      }
                    </div>
                  }
                </div>

                <!-- Acciones de Transición de Estado -->
                <div class="bg-gray-900/60 p-4 rounded-xl border border-gray-800 space-y-3">
                  <h4 class="text-xs font-bold text-gray-300 uppercase tracking-wider">Flujo de Estado Quirúrgico</h4>
                  <div class="grid grid-cols-2 gap-2">
                    <button
                      (click)="cambiarEstado('EnEspera')"
                      [disabled]="paciente()!.estado === 'EnEspera'"
                      class="px-3 py-2 bg-purple-600/20 hover:bg-purple-600/40 text-purple-300 border border-purple-500/30 rounded-xl text-xs font-bold transition disabled:opacity-40"
                    >
                      Mover a Espera
                    </button>
                    <button
                      (click)="cambiarEstado('EnCirugia')"
                      [disabled]="paciente()!.estado === 'EnCirugia' || !esApto()"
                      class="px-3 py-2 bg-emerald-600/20 hover:bg-emerald-600/40 text-emerald-300 border border-emerald-500/30 rounded-xl text-xs font-bold transition disabled:opacity-40"
                    >
                      Ingresar a Quirófano
                    </button>
                    <button
                      (click)="cambiarEstado('Finalizado')"
                      [disabled]="paciente()!.estado === 'Finalizado'"
                      class="px-3 py-2 bg-sky-600/20 hover:bg-sky-600/40 text-sky-300 border border-sky-500/30 rounded-xl text-xs font-bold transition disabled:opacity-40"
                    >
                      Finalizar / Recuperación
                    </button>
                    <button
                      (click)="abrirModalCancelacion()"
                      class="px-3 py-2 bg-rose-600/20 hover:bg-rose-600/40 text-rose-300 border border-rose-500/30 rounded-xl text-xs font-bold transition"
                    >
                      Cancelar Cirugía
                    </button>
                  </div>
                </div>
              </div>
            }

            <!-- PESTAÑA 2: INSUMOS, KITS & SOLICITUDES AD-HOC -->
            @if (tabActivo() === 'insumos') {
              <div class="space-y-4 animate-fade-in">
                <!-- Insumos Asignados a la Cuenta -->
                <div class="bg-gray-900/60 p-4 rounded-xl border border-gray-800 space-y-3">
                  <div class="flex justify-between items-center">
                    <h4 class="text-xs font-bold text-gray-300 uppercase tracking-wider">Insumos y Medicamentos Asignados</h4>
                    <span class="text-xs font-mono font-bold text-sky-400">{{ paciente()!.insumosAsignados.length }} Ítems</span>
                  </div>

                  <div class="space-y-2">
                    @for (ins of paciente()!.insumosAsignados; track ins.insumoId) {
                      <div class="p-3 bg-gray-950 rounded-xl border border-gray-800/80 flex justify-between items-center">
                        <div>
                          <span class="text-xs font-bold text-white block">{{ ins.insumoNombre }}</span>
                          <span class="text-[10px] text-gray-400 font-mono">Entregado: {{ ins.cantidadEntregada }} | Devuelto: {{ ins.cantidadDevuelta }} | Consumido: <strong class="text-sky-300">{{ ins.cantidadConsumida }}</strong></span>
                        </div>

                        <!-- Botón de Devolución Rápida de Sobrante -->
                        <button
                          (click)="abrirDevolucionModal(ins)"
                          [disabled]="ins.cantidadConsumida <= 0"
                          class="px-2.5 py-1 bg-amber-500/10 hover:bg-amber-500/20 text-amber-300 border border-amber-500/20 rounded-lg text-xs font-semibold transition disabled:opacity-40 flex items-center gap-1"
                        >
                          <lucide-icon name="undo-2" class="w-3 h-3"></lucide-icon>
                          Devolver
                        </button>
                      </div>
                    }
                  </div>
                </div>

                <!-- Solicitudes Ad-Hoc Urgentes desde Quirófano -->
                <div class="bg-gray-900/60 p-4 rounded-xl border border-gray-800 space-y-3">
                  <div class="flex justify-between items-center">
                    <h4 class="text-xs font-bold text-gray-300 uppercase tracking-wider">Peticiones Ad-Hoc de Urgencia</h4>
                    <button
                      (click)="mostrarSolicitudAdHoc.set(true)"
                      class="px-2.5 py-1 bg-sky-600 hover:bg-sky-500 text-white rounded-lg text-xs font-bold transition flex items-center gap-1"
                    >
                      <lucide-icon name="plus" class="w-3 h-3"></lucide-icon>
                      Solicitar Insumo
                    </button>
                  </div>

                  <!-- Formulario de Petición Ad-Hoc -->
                  @if (mostrarSolicitudAdHoc()) {
                    <div class="p-3 bg-gray-950 rounded-xl border border-sky-500/30 space-y-3 animate-fade-in">
                      <div>
                        <label class="text-[11px] text-gray-400 block mb-1">Insumo / Medicamento</label>
                        <select [(ngModel)]="insumoAdHocId" class="w-full bg-gray-900 border border-gray-700 rounded-lg p-2 text-xs text-white">
                          @for (cat of catalogoInsumos(); track cat.id) {
                            <option [value]="cat.id">{{ cat.nombre }} ({{ cat.codigo }})</option>
                          }
                        </select>
                      </div>

                      <div class="grid grid-cols-2 gap-2">
                        <div>
                          <label class="text-[11px] text-gray-400 block mb-1">Cantidad</label>
                          <input type="number" min="1" [(ngModel)]="cantidadAdHoc" class="w-full bg-gray-900 border border-gray-700 rounded-lg p-2 text-xs text-white font-mono" />
                        </div>
                        <div>
                          <label class="text-[11px] text-gray-400 block mb-1">Observaciones</label>
                          <input type="text" [(ngModel)]="observacionAdHoc" placeholder="Motivo de urgencia" class="w-full bg-gray-900 border border-gray-700 rounded-lg p-2 text-xs text-white" />
                        </div>
                      </div>

                      <div class="flex justify-end gap-2 pt-1">
                        <button (click)="mostrarSolicitudAdHoc.set(false)" class="px-3 py-1.5 bg-gray-800 text-gray-300 rounded-lg text-xs">Cancelar</button>
                        <button (click)="enviarSolicitudAdHoc()" class="px-3 py-1.5 bg-sky-600 hover:bg-sky-500 text-white rounded-lg text-xs font-bold">Enviar a Almacén Central</button>
                      </div>
                    </div>
                  }

                  <!-- Listado de Peticiones Ad-Hoc -->
                  <div class="space-y-2">
                    @for (sol of paciente()!.solicitudesInsumos; track sol.id) {
                      <div class="p-3 bg-gray-950 rounded-xl border border-gray-800 flex justify-between items-center">
                        <div>
                          <div class="flex items-center gap-2">
                            <span class="text-xs font-bold text-white">{{ sol.insumoNombre }}</span>
                            <span class="text-[10px] font-extrabold px-2 py-0.5 rounded-full uppercase" [ngClass]="sol.estadoSolicitud === 'Despachado' ? 'bg-emerald-500/10 text-emerald-400 border border-emerald-500/20' : 'bg-amber-500/10 text-amber-400 border border-amber-500/20'">
                              {{ sol.estadoSolicitud }}
                            </span>
                          </div>
                          <span class="text-[10px] text-gray-400 font-mono">Cantidad: {{ sol.cantidadSolicitada }} | Solicitado por: {{ sol.usuarioSolicitud }} ({{ sol.fechaSolicitud | date:'shortTime' }})</span>
                        </div>

                        @if (sol.estadoSolicitud === 'Pendiente') {
                          <button
                            (click)="despacharSolicitud(sol.id)"
                            class="px-2.5 py-1 bg-emerald-600 hover:bg-emerald-500 text-white rounded-lg text-xs font-bold transition"
                          >
                            Despachar
                          </button>
                        }
                      </div>
                    }
                  </div>
                </div>
              </div>
            }

            <!-- PESTAÑA 3: HONORARIOS MÉDICOS & PRECIOS (ADMINISTRACIÓN / CAJA) -->
            @if (tabActivo() === 'honorarios') {
              <div class="space-y-4 animate-fade-in">
                <!-- Estructura de Precios Base y Derecho de Sala -->
                <div class="bg-gray-900/60 p-4 rounded-xl border border-gray-800 space-y-3">
                  <h4 class="text-xs font-bold text-gray-300 uppercase tracking-wider">Tarifas Administrativas de Quirófano</h4>
                  <div class="grid grid-cols-2 gap-3">
                    <div>
                      <label class="text-[11px] text-gray-400 block mb-1">Derecho de Sala (USD $)</label>
                      <input type="number" step="0.01" [(ngModel)]="precioDerechoSala" class="w-full bg-gray-950 border border-gray-700 rounded-lg p-2 text-xs text-white font-mono" />
                    </div>
                    <div>
                      <label class="text-[11px] text-gray-400 block mb-1">Precio Base Cirugía (USD $)</label>
                      <input type="number" step="0.01" [(ngModel)]="precioBaseCirugia" class="w-full bg-gray-950 border border-gray-700 rounded-lg p-2 text-xs text-white font-mono" />
                    </div>
                  </div>

                  <div class="flex items-center gap-2 pt-1">
                    <input type="checkbox" id="esAlquiladoCheck" [(ngModel)]="esAlquiladoPabellon" class="w-4 h-4 rounded text-sky-600 bg-gray-900 border-gray-700 cursor-pointer" />
                    <label for="esAlquiladoCheck" class="text-xs text-gray-300 font-medium cursor-pointer">
                      Pabellón Alquilado (Médicos Externos / Tarifa Especial de Sala)
                    </label>
                  </div>
                </div>

                <!-- Equipo de N Médicos Participantes con Especialidades DB-Driven -->
                <div class="bg-gray-900/60 p-4 rounded-xl border border-gray-800 space-y-3">
                  <div class="flex justify-between items-center">
                    <h4 class="text-xs font-bold text-gray-300 uppercase tracking-wider">Equipo Médico & Honorarios Individuales</h4>
                    <button
                      (click)="agregarFilaMedico()"
                      class="px-2.5 py-1 bg-sky-600 hover:bg-sky-500 text-white rounded-lg text-xs font-bold transition flex items-center gap-1"
                    >
                      <lucide-icon name="user-plus" class="w-3 h-3"></lucide-icon>
                      Agregar Médico
                    </button>
                  </div>

                  <div class="space-y-2">
                    @for (m of medicosHonorariosEdit(); track $index; let idx = $index) {
                      <div class="p-3 bg-gray-950 rounded-xl border border-gray-800 grid grid-cols-12 gap-2 items-center">
                        <div class="col-span-4">
                          <label class="text-[10px] text-gray-400 block mb-0.5">Médico</label>
                          <select [(ngModel)]="m.medicoId" (change)="onMedicoSelected(m)" class="w-full bg-gray-900 border border-gray-700 rounded-lg p-1.5 text-xs text-white">
                            @for (med of catalogoMedicos(); track med.id) {
                              <option [value]="med.id">{{ med.nombre }}</option>
                            }
                          </select>
                        </div>

                        <div class="col-span-3">
                          <label class="text-[10px] text-gray-400 block mb-0.5">Especialidad (DB)</label>
                          <span class="text-xs text-sky-400 font-medium truncate block p-1.5 bg-gray-900/60 rounded border border-gray-800">
                            {{ m.especialidadNombre || 'General' }}
                          </span>
                        </div>

                        <div class="col-span-3">
                          <label class="text-[10px] text-gray-400 block mb-0.5">Honorario ($)</label>
                          <input type="number" step="0.01" [(ngModel)]="m.montoHonorarioUsd" class="w-full bg-gray-900 border border-gray-700 rounded-lg p-1.5 text-xs text-white font-mono" />
                        </div>

                        <div class="col-span-2 flex items-center justify-end gap-2 pt-3">
                          <button
                            (click)="m.esCirujanoPrincipal = !m.esCirujanoPrincipal"
                            [title]="m.esCirujanoPrincipal ? 'Cirujano Principal' : 'Médico Asistente'"
                            class="p-1.5 rounded-lg border transition"
                            [ngClass]="m.esCirujanoPrincipal ? 'bg-amber-500/20 text-amber-400 border-amber-500/30' : 'bg-gray-800 text-gray-400 border-gray-700'"
                          >
                            <lucide-icon name="crown" class="w-3.5 h-3.5"></lucide-icon>
                          </button>
                          <button (click)="eliminarFilaMedico(idx)" class="p-1.5 text-rose-400 hover:bg-rose-500/10 rounded-lg transition">
                            <lucide-icon name="trash-2" class="w-3.5 h-3.5"></lucide-icon>
                          </button>
                        </div>
                      </div>
                    }
                  </div>

                  <div class="pt-2 flex justify-end">
                    <button
                      (click)="guardarHonorariosYPrecios()"
                      class="px-4 py-2 bg-emerald-600 hover:bg-emerald-500 text-white rounded-xl text-xs font-bold transition flex items-center gap-1.5"
                    >
                      <lucide-icon name="save" class="w-4 h-4"></lucide-icon>
                      Guardar Honorarios y Precios
                    </button>
                  </div>
                </div>
              </div>
            }

            <!-- PESTAÑA 4: AUDITORÍA Y LOGS -->
            @if (tabActivo() === 'logs') {
              <div class="bg-gray-900/60 p-4 rounded-xl border border-gray-800 space-y-3 animate-fade-in">
                <h4 class="text-xs font-bold text-gray-300 uppercase tracking-wider">Trazabilidad de Eventos Quirúrgicos</h4>
                <div class="space-y-2">
                  @for (log of paciente()!.logs; track log.id) {
                    <div class="p-3 bg-gray-950 rounded-xl border border-gray-800/80 space-y-1">
                      <div class="flex justify-between items-center">
                        <span class="text-xs font-bold text-sky-400">{{ log.evento }}</span>
                        <span class="text-[10px] text-gray-500 font-mono">{{ log.timestamp | date:'short' }}</span>
                      </div>
                      <p class="text-xs text-gray-300">{{ log.detalle }}</p>
                      <span class="text-[10px] text-gray-500 block">Usuario: {{ log.usuarioId }}</span>
                    </div>
                  }
                </div>
              </div>
            }

          </div>

          <!-- Pie del Drawer -->
          <div class="p-4 border-t border-gray-800 bg-gray-900/80 flex justify-end">
            <button (click)="cerrar.emit()" class="px-4 py-2 bg-gray-800 hover:bg-gray-700 text-gray-300 rounded-xl text-xs font-semibold transition">
              Cerrar Panel
            </button>
          </div>

        </div>
      </div>
    }
  `
})
export class PanelDetalleCirugiaComponent implements OnInit {
  private pabellonService = inject(PabellonService);
  private medicoService = inject(MedicoService);
  private inventoryService = inject(InventoryService);

  paciente = input<PacienteQuirurgicoItem | null>(null);
  cerrar = output<void>();
  recargar = output<void>();

  tabActivo = signal<'clinica' | 'insumos' | 'honorarios' | 'logs'>('clinica');

  // Catálogos reactivos
  catalogoMedicos = signal<Medico[]>([]);
  catalogoInsumos = signal<Insumo[]>([]);

  // Estados de Honorarios & Tarifas
  precioDerechoSala = 0;
  precioBaseCirugia = 0;
  esAlquiladoPabellon = false;
  medicosHonorariosEdit = signal<{
    medicoId: string;
    especialidadId: string;
    especialidadNombre: string;
    montoHonorarioUsd: number;
    esCirujanoPrincipal: boolean;
  }[]>([]);

  // Solicitud Ad-Hoc
  mostrarSolicitudAdHoc = signal<boolean>(false);
  insumoAdHocId = '';
  cantidadAdHoc = 1;
  observacionAdHoc = '';

  ngOnInit() {
    this.cargarCatalogos();
    this.sincronizarDatos();
  }

  cargarCatalogos() {
    this.medicoService.getAll().subscribe(meds => {
      this.catalogoMedicos.set(meds || []);
    });

    this.inventoryService.getInsumos().subscribe(ins => {
      this.catalogoInsumos.set(ins || []);
      if (ins && ins.length > 0) {
        this.insumoAdHocId = ins[0].id;
      }
    });
  }

  sincronizarDatos() {
    const p = this.paciente();
    if (!p) return;

    this.precioDerechoSala = p.precioDerechoSalaUsd || 0;
    this.precioBaseCirugia = p.precioBaseUsd || 0;
    this.esAlquiladoPabellon = p.esAlquilado || false;

    const lista = (p.medicosHonorarios || []).map(m => ({
      medicoId: m.medicoId,
      especialidadId: m.especialidadId,
      especialidadNombre: m.especialidadNombre,
      montoHonorarioUsd: m.montoHonorarioUsd,
      esCirujanoPrincipal: m.esCirujanoPrincipal
    }));

    this.medicosHonorariosEdit.set(lista);
  }

  esApto(): boolean {
    const p = this.paciente();
    if (!p || p.requisitos.length === 0) return true;
    return p.requisitos.every(r => r.cumplido);
  }

  getRequisitosCumplidos(): number {
    return this.paciente()?.requisitos.filter(r => r.cumplido).length || 0;
  }

  toggleRequisito(req: OrdenCirugiaRequisito) {
    const p = this.paciente();
    if (!p) return;

    const nuevoEstado = !req.cumplido;
    this.pabellonService.toggleRequisito(p.id, req.requisitoCirugiaId, nuevoEstado).subscribe(() => {
      req.cumplido = nuevoEstado;
      this.recargar.emit();
    });
  }

  cambiarEstado(nuevoEstado: string) {
    const p = this.paciente();
    if (!p) return;

    this.pabellonService.cambiarEstado({
      ordenCirugiaId: p.id,
      nuevoEstado
    }).subscribe(() => {
      this.recargar.emit();
    });
  }

  abrirModalCancelacion() {
    const motivo = prompt('Ingrese el motivo de cancelación de la cirugía:');
    if (!motivo) return;

    const p = this.paciente();
    if (!p) return;

    this.pabellonService.cambiarEstado({
      ordenCirugiaId: p.id,
      nuevoEstado: 'Cancelada',
      motivoCancelacion: motivo
    }).subscribe(() => {
      this.recargar.emit();
    });
  }

  abrirDevolucionModal(ins: any) {
    const cantStr = prompt(`Cantidad a devolver de ${ins.insumoNombre} (Máx: ${ins.cantidadConsumida}):`, '1');
    if (!cantStr) return;
    const cant = parseFloat(cantStr);
    if (isNaN(cant) || cant <= 0 || cant > ins.cantidadConsumida) {
      alert('Cantidad inválida.');
      return;
    }

    const p = this.paciente();
    if (!p) return;

    this.pabellonService.devolucionInsumo({
      cuentaServicioId: p.cuentaServicioId,
      insumoId: ins.insumoId,
      cantidadDevuelta: cant,
      motivo: 'Devolución de sobrante'
    }).subscribe(() => {
      this.recargar.emit();
    });
  }

  enviarSolicitudAdHoc() {
    const p = this.paciente();
    if (!p || !this.insumoAdHocId || this.cantidadAdHoc <= 0) return;

    this.pabellonService.solicitarInsumoExtra({
      ordenCirugiaId: p.id,
      insumoId: this.insumoAdHocId,
      cantidad: this.cantidadAdHoc,
      observaciones: this.observacionAdHoc
    }).subscribe(() => {
      this.mostrarSolicitudAdHoc.set(false);
      this.observacionAdHoc = '';
      this.recargar.emit();
    });
  }

  despacharSolicitud(solicitudId: string) {
    this.pabellonService.despacharSolicitudExtra(solicitudId).subscribe(() => {
      this.recargar.emit();
    });
  }

  agregarFilaMedico() {
    const meds = this.catalogoMedicos();
    if (meds.length === 0) return;

    const primero = meds[0];
    const actual = this.medicosHonorariosEdit();
    this.medicosHonorariosEdit.set([
      ...actual,
      {
        medicoId: primero.id || '',
        especialidadId: primero.especialidadId || '',
        especialidadNombre: primero.especialidad || 'General',
        montoHonorarioUsd: primero.honorarioBase || 0,
        esCirujanoPrincipal: actual.length === 0
      }
    ]);
  }

  onMedicoSelected(item: any) {
    const medico = this.catalogoMedicos().find(m => m.id === item.medicoId);
    if (medico) {
      item.especialidadId = medico.especialidadId || '';
      item.especialidadNombre = medico.especialidad || 'General';
      if (!item.montoHonorarioUsd || item.montoHonorarioUsd === 0) {
        item.montoHonorarioUsd = medico.honorarioBase || 0;
      }
    }
  }

  eliminarFilaMedico(index: number) {
    const actual = this.medicosHonorariosEdit().filter((_, i) => i !== index);
    this.medicosHonorariosEdit.set(actual);
  }

  guardarHonorariosYPrecios() {
    const p = this.paciente();
    if (!p) return;

    this.pabellonService.actualizarHonorariosYPrecios({
      ordenCirugiaId: p.id,
      precioDerechoSalaUsd: this.precioDerechoSala,
      precioBaseUsd: this.precioBaseCirugia,
      esAlquilado: this.esAlquiladoPabellon,
      medicos: this.medicosHonorariosEdit()
    }).subscribe(() => {
      this.recargar.emit();
    });
  }

  getEstadoClass(estado: string): string {
    const e = (estado || '').toLowerCase();
    if (e === 'programada') return 'bg-amber-500/10 text-amber-400 border border-amber-500/20';
    if (e === 'enespera') return 'bg-purple-500/10 text-purple-400 border border-purple-500/20';
    if (e === 'encirugia') return 'bg-emerald-500/10 text-emerald-400 border border-emerald-500/20 animate-pulse';
    if (e === 'finalizado') return 'bg-sky-500/10 text-sky-400 border border-sky-500/20';
    if (e === 'cancelada') return 'bg-rose-500/10 text-rose-400 border border-rose-500/20';
    return 'bg-gray-800 text-gray-300';
  }
}
