import { Component, ChangeDetectionStrategy, input, output, signal, inject, OnInit, effect, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LucideAngularModule } from 'lucide-angular';
import {
  PabellonService,
  PacienteQuirurgicoItem,
  MedicoHonorarioItem,
  OrdenCirugiaRequisito,
  RequisitoCirugia
} from '../../../core/services/pabellon.service';
import { MedicoService, Medico } from '../../../core/services/medico.service';
import { InventoryService } from '../../../core/services/inventory.service';
import { MultiSedeService, Sede, AreaClinica } from '../../../core/services/multi-sede.service';
import { Insumo } from '../../../core/models/inventory.model';

@Component({
  selector: 'app-panel-detalle-cirugia',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    @if (paciente()) {
      <div class="fixed inset-0 z-[500] overflow-hidden bg-black/60 backdrop-blur-sm flex justify-end animate-fade-in">
        <div class="w-full max-w-2xl bg-gray-950 border-l border-gray-800 shadow-2xl h-full flex flex-col justify-between overflow-y-auto">
          
          <!-- Encabezado del Panel -->
          <div class="pt-8 pb-6 px-7 border-b border-gray-800/80 bg-gray-900/95 sticky top-0 z-20 backdrop-blur-md">
            <div class="flex justify-between items-start gap-4">
              <div class="space-y-1.5 flex-1 min-w-0">
                <div class="flex items-center gap-2 flex-wrap mb-1">
                  <span class="text-xs font-mono font-bold text-sky-400 bg-sky-500/10 px-2.5 py-0.5 rounded-lg border border-sky-500/20">
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
                <h3 class="text-base font-bold text-white leading-tight truncate">{{ paciente()!.descripcionCirugia }}</h3>
                <p class="text-xs text-gray-400">
                  Paciente: <strong class="text-gray-200">{{ paciente()!.pacienteNombre }}</strong> ({{ paciente()!.pacienteCedula }}) | 
                  Ubicación: <strong class="text-sky-300">{{ paciente()!.ubicacion.descripcionCompleta || paciente()!.salaQuirofano || 'Sin Asignar' }}</strong>
                </p>
              </div>

              <button (click)="cerrar.emit()" title="Cerrar Panel" class="p-2 text-gray-400 hover:text-white rounded-xl bg-gray-800/60 hover:bg-gray-800 border border-gray-700/60 transition shadow-sm flex items-center justify-center shrink-0">
                <lucide-icon name="x" class="w-5 h-5"></lucide-icon>
              </button>
            </div>

            <!-- Selector de Pestañas por Rol -->
            <div class="flex items-center gap-1 mt-4 bg-gray-950 p-1 rounded-xl border border-gray-800/80 text-xs overflow-x-auto">
              <button
                (click)="seleccionarTab('clinica')"
                [class.bg-sky-600]="tabActivo() === 'clinica'"
                [class.text-white]="tabActivo() === 'clinica'"
                [class.text-gray-400]="tabActivo() !== 'clinica'"
                class="flex-1 py-1.5 px-2 rounded-lg font-semibold transition flex items-center justify-center gap-1.5 whitespace-nowrap"
              >
                <lucide-icon name="clipboard-check" class="w-3.5 h-3.5"></lucide-icon>
                Checklist Preop
              </button>
              <button
                (click)="seleccionarTab('insumos')"
                [class.bg-sky-600]="tabActivo() === 'insumos'"
                [class.text-white]="tabActivo() === 'insumos'"
                [class.text-gray-400]="tabActivo() !== 'insumos'"
                class="flex-1 py-1.5 px-2 rounded-lg font-semibold transition flex items-center justify-center gap-1.5 whitespace-nowrap"
              >
                <lucide-icon name="boxes" class="w-3.5 h-3.5"></lucide-icon>
                Insumos & Kits
              </button>
              <button
                (click)="seleccionarTab('honorarios')"
                [class.bg-sky-600]="tabActivo() === 'honorarios'"
                [class.text-white]="tabActivo() === 'honorarios'"
                [class.text-gray-400]="tabActivo() !== 'honorarios'"
                class="flex-1 py-1.5 px-2 rounded-lg font-semibold transition flex items-center justify-center gap-1.5 whitespace-nowrap"
              >
                <lucide-icon name="dollar-sign" class="w-3.5 h-3.5"></lucide-icon>
                Honorarios & Precios
              </button>
              <button
                (click)="seleccionarTab('reprogramar')"
                [class.bg-sky-600]="tabActivo() === 'reprogramar'"
                [class.text-white]="tabActivo() === 'reprogramar'"
                [class.text-gray-400]="tabActivo() !== 'reprogramar'"
                class="flex-1 py-1.5 px-2 rounded-lg font-semibold transition flex items-center justify-center gap-1.5 whitespace-nowrap"
              >
                <lucide-icon name="calendar" class="w-3.5 h-3.5"></lucide-icon>
                Reasignar Fecha/Hora
              </button>
              <button
                (click)="seleccionarTab('logs')"
                [class.bg-sky-600]="tabActivo() === 'logs'"
                [class.text-white]="tabActivo() === 'logs'"
                [class.text-gray-400]="tabActivo() !== 'logs'"
                class="flex-1 py-1.5 px-2 rounded-lg font-semibold transition flex items-center justify-center gap-1.5 whitespace-nowrap"
              >
                <lucide-icon name="history" class="w-3.5 h-3.5"></lucide-icon>
                Auditoría
              </button>
            </div>
          </div>

          <!-- Contenido de las Pestañas -->
          <div class="p-5 flex-1 space-y-4 overflow-y-auto max-h-[calc(100vh-210px)] pr-2">
            
            <!-- PESTAÑA 1: FICHA CLÍNICA & CHECKLIST PREOPERATORIO -->
            @if (tabActivo() === 'clinica') {
              <div class="space-y-4 animate-fade-in">
                <!-- Indicador de Aptitud -->
                @if (paciente()!.requisitos.length > 0) {
                  <div class="p-3.5 rounded-xl border flex items-center justify-between shadow-sm" [ngClass]="esApto() ? 'bg-emerald-950/40 border-emerald-500/40 text-emerald-300' : 'bg-amber-950/40 border-amber-500/40 text-amber-300'">
                    <div class="flex items-center gap-2.5">
                      <lucide-icon [name]="esApto() ? 'check-circle-2' : 'alert-triangle'" class="w-5 h-5" [ngClass]="esApto() ? 'text-emerald-400' : 'text-amber-400'"></lucide-icon>
                      <div>
                        <h4 class="text-xs font-bold">{{ esApto() ? 'Paciente Apto para Ingresar a Quirófano' : 'Requisitos Quirúrgicos Pendientes' }}</h4>
                        <p class="text-[11px] opacity-80">{{ getRequisitosCumplidos() }} de {{ paciente()!.requisitos.length }} requisitos completados.</p>
                      </div>
                    </div>
                    <span class="text-xs font-mono font-bold px-2 py-1 rounded bg-black/40 border border-white/10">{{ getRequisitosCumplidos() }}/{{ paciente()!.requisitos.length }}</span>
                  </div>
                } @else {
                  <div class="p-3.5 rounded-xl border bg-gray-900/80 border-gray-800 text-gray-400 flex items-center justify-between shadow-sm">
                    <div class="flex items-center gap-2.5">
                      <lucide-icon name="alert-circle" class="w-5 h-5 text-gray-500"></lucide-icon>
                      <div>
                        <h4 class="text-xs font-bold text-gray-300">Sin Requisitos Preoperatorios</h4>
                        <p class="text-[11px] text-gray-500">Agregue los requisitos para validar la aptitud prequirúrgica.</p>
                      </div>
                    </div>
                    <span class="text-xs font-mono font-bold text-gray-500 px-2 py-1 rounded bg-black/40 border border-gray-800">0/0</span>
                  </div>
                }

                <!-- Lista de Requisitos / Checklist Interactivo -->
                <div class="bg-gray-900/60 p-4 rounded-xl border border-gray-800 space-y-2.5 max-h-80 overflow-y-auto pr-1">
                  <div class="flex justify-between items-center mb-2">
                    <h4 class="text-xs font-bold text-gray-300 uppercase tracking-wider">Checklist Preoperatorio DB-Driven</h4>
                    <div class="flex items-center gap-1.5">
                      @if (paciente()!.requisitos.length > 1) {
                        <button
                          (click)="eliminarTodosLosRequisitos()"
                          title="Remover todos los requisitos preoperatorios de esta cirugía"
                          class="px-2.5 py-1 bg-rose-500/10 hover:bg-rose-500/20 text-rose-400 hover:text-rose-300 border border-rose-500/30 rounded-lg text-xs font-semibold transition flex items-center gap-1 shadow-sm"
                        >
                          <lucide-icon name="trash-2" class="w-3.5 h-3.5"></lucide-icon>
                          Eliminar Todos
                        </button>
                      }
                      <button
                        (click)="mostrarNuevoRequisito.set(true)"
                        class="px-2.5 py-1 bg-sky-600 hover:bg-sky-500 text-white rounded-lg text-xs font-bold transition flex items-center gap-1 shadow-sm"
                      >
                        <lucide-icon name="plus" class="w-3.5 h-3.5"></lucide-icon>
                        Agregar Preoperatorio
                      </button>
                    </div>
                  </div>

                  <!-- Formulario Inline: Agregar Requisito Preoperatorio -->
                  @if (mostrarNuevoRequisito()) {
                    <div class="p-3.5 bg-gray-950 rounded-xl border border-sky-500/40 space-y-2.5 animate-fade-in shadow-lg">
                      <div class="flex justify-between items-center">
                        <span class="text-xs font-bold text-sky-400">Nuevo Requisito Preoperatorio</span>
                        @if (catalogoRequisitos().length > 0) {
                          <div class="flex items-center gap-1.5">
                            <span class="text-[11px] text-gray-400">Del Catálogo:</span>
                            <select
                              (change)="seleccionarDelCatalogo($event)"
                              class="bg-gray-900 border border-gray-700 rounded-lg px-2 py-1 text-xs text-white focus:outline-none focus:border-sky-500"
                            >
                              <option value="">-- Seleccionar --</option>
                              @for (r of catalogoRequisitos(); track r.id) {
                                <option [value]="r.id">{{ r.nombre }}</option>
                              }
                            </select>
                          </div>
                        }
                      </div>
                      <div>
                        <label class="text-[10px] text-gray-400 font-semibold block mb-0.5">Nombre del Requisito *</label>
                        <input
                          type="text"
                          [(ngModel)]="nuevoRequisitoNombre"
                          placeholder="Ej: Radiografía de Tórax PA"
                          class="w-full bg-gray-900 border border-gray-700 rounded-lg p-2 text-xs text-white focus:outline-none focus:border-sky-500"
                        />
                      </div>
                      <div>
                        <label class="text-[10px] text-gray-400 font-semibold block mb-0.5">Descripción / Observación (Opcional)</label>
                        <input
                          type="text"
                          [(ngModel)]="nuevoRequisitoDescripcion"
                          placeholder="Ej: Informe radiológico y firma de médico tratante"
                          class="w-full bg-gray-900 border border-gray-700 rounded-lg p-2 text-xs text-white focus:outline-none focus:border-sky-500"
                        />
                      </div>
                      <div class="flex justify-end gap-2 pt-1">
                        <button (click)="mostrarNuevoRequisito.set(false)" class="px-3 py-1.5 bg-gray-800 hover:bg-gray-700 text-gray-300 rounded-lg text-xs font-semibold">Cancelar</button>
                        <button (click)="guardarNuevoRequisito()" [disabled]="!nuevoRequisitoNombre.trim()" class="px-3 py-1.5 bg-sky-600 hover:bg-sky-500 disabled:opacity-50 text-white rounded-lg text-xs font-bold">Guardar Requisito</button>
                      </div>
                    </div>
                  }

                  @if (paciente()!.requisitos.length === 0 && !mostrarNuevoRequisito()) {
                    <div class="text-center py-6 text-gray-500 text-xs border border-dashed border-gray-800 rounded-xl bg-gray-950/40 p-4 space-y-2">
                      <lucide-icon name="clipboard-list" class="w-7 h-7 mx-auto text-gray-600"></lucide-icon>
                      <p class="font-medium text-gray-400">No hay requisitos preoperatorios asignados a esta cirugía.</p>
                      <p class="text-[11px] text-gray-500">Agregue requisitos individuales o cargue los preoperatorios estándar con 1 clic.</p>
                      <div class="flex justify-center gap-2 pt-1">
                        <button
                          (click)="mostrarNuevoRequisito.set(true)"
                          class="px-3 py-1.5 bg-sky-600/20 hover:bg-sky-600/30 text-sky-400 border border-sky-500/30 rounded-lg text-xs font-semibold inline-flex items-center gap-1.5 transition"
                        >
                          <lucide-icon name="plus" class="w-3.5 h-3.5"></lucide-icon>
                          Nuevo Requisito
                        </button>
                        <button
                          (click)="cargarRequisitosPredeterminados()"
                          class="px-3 py-1.5 bg-indigo-600/20 hover:bg-indigo-600/30 text-indigo-400 border border-indigo-500/30 rounded-lg text-xs font-semibold inline-flex items-center gap-1.5 transition"
                        >
                          <lucide-icon name="list-plus" class="w-3.5 h-3.5"></lucide-icon>
                          Cargar Estándar
                        </button>
                      </div>
                    </div>
                  }

                  @for (req of paciente()!.requisitos; track req.id) {
                    <div class="flex items-center justify-between p-3 rounded-xl bg-gray-950 border border-gray-800/90 hover:border-gray-700 transition group shadow-sm">
                      <label class="flex items-start gap-3 cursor-pointer flex-1 min-w-0 select-none">
                        <input
                          type="checkbox"
                          [checked]="req.cumplido"
                          (change)="toggleRequisito(req)"
                          class="mt-0.5 w-4 h-4 rounded text-sky-500 bg-gray-900 border-gray-700 focus:ring-sky-500 cursor-pointer shrink-0"
                        />
                        <div class="space-y-0.5 flex-1 min-w-0">
                          <span class="text-xs font-semibold text-gray-100 block transition" [class.line-through]="req.cumplido" [class.text-gray-400]="req.cumplido">
                            {{ req.nombre || 'Requisito Quirúrgico' }}
                          </span>
                          @if (req.descripcion) {
                            <span class="text-[11px] text-gray-400 block leading-tight">
                              {{ req.descripcion }}
                            </span>
                          }
                        </div>
                      </label>
                      <div class="flex items-center gap-2 shrink-0 ml-2">
                        @if (req.cumplido) {
                          <span class="text-[10px] font-mono text-emerald-400 bg-emerald-500/10 px-2 py-0.5 rounded border border-emerald-500/20">
                            {{ req.verificadoPor ? 'Por: ' + req.verificadoPor : 'Completado' }}
                          </span>
                        }
                        <button (click)="eliminarRequisito(req)" title="Remover requisito" class="text-gray-500 hover:text-rose-400 p-1 rounded hover:bg-rose-500/10 transition">
                          <lucide-icon name="trash-2" class="w-3.5 h-3.5"></lucide-icon>
                        </button>
                      </div>
                    </div>
                  }
                </div>

                <!-- Acciones de Transición de Estado -->
                <div class="bg-gray-900/60 p-4 rounded-xl border border-gray-800 space-y-3">
                  <div class="flex items-center justify-between">
                    <h4 class="text-xs font-bold text-gray-300 uppercase tracking-wider">Flujo de Estado Quirúrgico</h4>
                    <span class="text-[10px] px-2 py-0.5 rounded-full font-bold uppercase" [ngClass]="getEstadoClass(paciente()!.estado)">
                      {{ paciente()!.estado }}
                    </span>
                  </div>
                  <div class="grid grid-cols-2 gap-2.5">
                    <button
                      (click)="abrirModalTraslado()"
                      class="px-3 py-2.5 bg-indigo-600/20 hover:bg-indigo-600/40 text-indigo-300 border border-indigo-500/30 rounded-xl text-xs font-bold transition flex items-center justify-center gap-1.5 shadow-sm"
                    >
                      <lucide-icon name="arrow-right-left" class="w-4 h-4 text-indigo-400"></lucide-icon>
                      Traslado (UCI / Hosp)
                    </button>
                    @if (paciente()!.estado !== 'EnCirugia') {
                      <button
                        (click)="cambiarEstado('EnCirugia')"
                        class="px-3 py-2.5 bg-emerald-600/20 hover:bg-emerald-600/40 text-emerald-300 border border-emerald-500/30 rounded-xl text-xs font-bold transition flex items-center justify-center gap-1.5 shadow-sm"
                      >
                        <lucide-icon name="activity" class="w-4 h-4 text-emerald-400"></lucide-icon>
                        Ingresar a Quirófano
                      </button>
                    } @else {
                      <button
                        (click)="cambiarEstado('Finalizado')"
                        class="px-3 py-2.5 bg-purple-600/20 hover:bg-purple-600/40 text-purple-300 border border-purple-500/30 rounded-xl text-xs font-bold transition flex items-center justify-center gap-1.5 shadow-sm"
                      >
                        <lucide-icon name="check-circle-2" class="w-4 h-4 text-purple-400"></lucide-icon>
                        Finalizar Cirugía
                      </button>
                    }
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

                  @if (paciente()!.insumosAsignados.length === 0) {
                    <p class="text-xs text-gray-500 text-center py-4 bg-gray-950 rounded-xl border border-gray-800">
                      No se han despachado kits o insumos iniciales para esta cirugía.
                    </p>
                  } @else {
                    <div class="space-y-2">
                      @for (ins of paciente()!.insumosAsignados; track ins.insumoId) {
                        <div class="p-3 bg-gray-950 rounded-xl border border-gray-800 flex items-center justify-between gap-3">
                          <div class="space-y-0.5">
                            <span class="text-xs font-bold text-white block">{{ ins.insumoNombre }}</span>
                            <span class="text-[10px] text-gray-400 font-mono">Cód: {{ ins.insumoCodigo }} | Unit: \${{ ins.precioUnitarioUsd | number:'1.2-2' }}</span>
                          </div>

                          <div class="flex items-center gap-3">
                            <div class="text-right text-[11px] font-mono">
                              <span class="text-sky-400 block">Entregado: {{ ins.cantidadEntregada }}</span>
                              <span class="text-amber-400 block">Devuelto: {{ ins.cantidadDevuelta }}</span>
                              <span class="text-emerald-400 font-bold block">Consumido: {{ ins.cantidadConsumida }}</span>
                            </div>

                            @if (ins.cantidadConsumida > 0) {
                              <button
                                (click)="abrirDevolucionModal(ins)"
                                class="px-2.5 py-1.5 bg-amber-500/10 hover:bg-amber-500/20 text-amber-300 border border-amber-500/30 rounded-lg text-xs font-semibold transition"
                              >
                                Devolver
                              </button>
                            }
                          </div>
                        </div>
                      }
                    </div>
                  }
                </div>

                <!-- Solicitudes Ad-Hoc Extras en Cirugía -->
                <div class="bg-gray-900/60 p-4 rounded-xl border border-gray-800 space-y-3">
                  <div class="flex justify-between items-center">
                    <h4 class="text-xs font-bold text-gray-300 uppercase tracking-wider">Solicitudes Ad-Hoc Extra</h4>
                    <button
                      (click)="mostrarSolicitudAdHoc.set(true)"
                      class="px-2.5 py-1 bg-purple-600 hover:bg-purple-500 text-white rounded-lg text-xs font-bold transition flex items-center gap-1"
                    >
                      <lucide-icon name="plus" class="w-3.5 h-3.5"></lucide-icon>
                      Solicitud Extra
                    </button>
                  </div>

                  @if (mostrarSolicitudAdHoc()) {
                    <div class="p-3 bg-gray-950 rounded-xl border border-purple-500/30 space-y-3 animate-fade-in">
                      <h5 class="text-xs font-bold text-purple-400">Nueva Solicitud Urgente al Almacén Central</h5>
                      <div class="grid grid-cols-3 gap-2">
                        <div class="col-span-2">
                          <label class="text-[10px] text-gray-400 font-semibold block mb-0.5">Insumo / Medicamento</label>
                          <select [(ngModel)]="insumoAdHocId" class="w-full bg-gray-900 border border-gray-700 rounded-lg p-2 text-xs text-white focus:outline-none focus:border-purple-500">
                            @for (i of catalogoInsumos(); track i.id) {
                              <option [value]="i.id">{{ i.nombre }} (Stock: {{ i.stockActual }})</option>
                            }
                          </select>
                        </div>
                        <div>
                          <label class="text-[10px] text-gray-400 font-semibold block mb-0.5">Cantidad</label>
                          <input
                            type="number"
                            min="1"
                            [(ngModel)]="cantidadAdHoc"
                            class="w-full bg-gray-900 border border-gray-700 rounded-lg p-2 text-xs text-white font-mono focus:outline-none focus:border-purple-500"
                          />
                        </div>
                      </div>
                      <div>
                        <label class="text-[10px] text-gray-400 font-semibold block mb-0.5">Motivo / Justificación Médica</label>
                        <input
                          type="text"
                          [(ngModel)]="observacionAdHoc"
                          placeholder="Ej: Sangrado imprevisto, reposición de compresa..."
                          class="w-full bg-gray-900 border border-gray-700 rounded-lg p-2 text-xs text-white focus:outline-none focus:border-purple-500"
                        />
                      </div>
                      <div class="flex justify-end gap-2">
                        <button (click)="mostrarSolicitudAdHoc.set(false)" class="px-3 py-1 bg-gray-800 text-gray-300 rounded-lg text-xs">Cancelar</button>
                        <button (click)="enviarSolicitudAdHoc()" class="px-3 py-1 bg-purple-600 hover:bg-purple-500 text-white rounded-lg text-xs font-bold">Solicitar</button>
                      </div>
                    </div>
                  }

                  @if (paciente()!.solicitudesInsumos.length === 0) {
                    <p class="text-xs text-gray-500 text-center py-4 bg-gray-950 rounded-xl border border-gray-800">
                      No hay solicitudes ad-hoc registradas para esta cirugía.
                    </p>
                  } @else {
                    <div class="space-y-2">
                      @for (sol of paciente()!.solicitudesInsumos; track sol.id) {
                        <div class="p-3 bg-gray-950 rounded-xl border border-gray-800 flex items-center justify-between">
                          <div>
                            <span class="text-xs font-bold text-white block">{{ sol.insumoNombre }}</span>
                            <span class="text-[10px] text-gray-400 font-mono">Cant: {{ sol.cantidadSolicitada }} | Solicitado por: {{ sol.usuarioSolicitud }}</span>
                          </div>

                          <div class="flex items-center gap-2">
                            <span class="text-[10px] font-bold px-2 py-0.5 rounded-full" [ngClass]="sol.estadoSolicitud === 'Despachado' ? 'bg-emerald-500/10 text-emerald-400' : 'bg-amber-500/10 text-amber-400'">
                              {{ sol.estadoSolicitud }}
                            </span>
                            @if (sol.estadoSolicitud === 'Pendiente') {
                              <button
                                (click)="despacharSolicitud(sol.id)"
                                class="px-2.5 py-1 bg-emerald-600 hover:bg-emerald-500 text-white rounded-lg text-xs font-bold transition"
                              >
                                Despachar
                              </button>
                            }
                          </div>
                        </div>
                      }
                    </div>
                  }
                </div>
              </div>
            }

            <!-- PESTAÑA 3: HONORARIOS MÉDICOS & PRECIOS ADMINISTRATIVOS -->
            @if (tabActivo() === 'honorarios') {
              <div class="space-y-4 animate-fade-in">
                <div class="bg-gray-900/60 p-4 rounded-xl border border-gray-800 space-y-3">
                  <h4 class="text-xs font-bold text-gray-300 uppercase tracking-wider">Tarifas Administrativas de la Cirugía</h4>
                  
                  <div class="grid grid-cols-2 gap-3">
                    <div>
                      <label class="text-[10px] text-gray-400 font-semibold block mb-1">Precio Base Cirugía (USD $)</label>
                      <input
                        type="number"
                        step="0.01"
                        [(ngModel)]="precioBaseCirugia"
                        class="w-full bg-gray-950 border border-gray-700 rounded-xl p-2.5 text-xs text-white font-mono focus:outline-none focus:border-sky-500"
                      />
                    </div>
                    <div>
                      <label class="text-[10px] text-gray-400 font-semibold block mb-1">Derecho de Sala (USD $)</label>
                      <input
                        type="number"
                        step="0.01"
                        [(ngModel)]="precioDerechoSala"
                        class="w-full bg-gray-950 border border-gray-700 rounded-xl p-2.5 text-xs text-white font-mono focus:outline-none focus:border-sky-500"
                      />
                    </div>
                  </div>

                  <div class="pt-1">
                    <label class="flex items-center gap-2 cursor-pointer text-xs text-gray-300">
                      <input
                        type="checkbox"
                        [(ngModel)]="esAlquiladoPabellon"
                        class="w-4 h-4 rounded text-sky-600 bg-gray-900 border-gray-700 focus:ring-sky-500"
                      />
                      <span>Pabellón Alquilado (Cirujano Externo con Honorario Libre)</span>
                    </label>
                  </div>
                </div>

                <!-- Lista de N Médicos y Honorarios -->
                <div class="bg-gray-900/60 p-4 rounded-xl border border-gray-800 space-y-3">
                  <div class="flex justify-between items-center">
                    <h4 class="text-xs font-bold text-gray-300 uppercase tracking-wider">Equipo Médico Quirúrgico (N Médicos)</h4>
                    <button
                      (click)="agregarFilaMedico()"
                      class="px-2.5 py-1 bg-sky-600 hover:bg-sky-500 text-white rounded-lg text-xs font-bold transition flex items-center gap-1"
                    >
                      <lucide-icon name="user-plus" class="w-3.5 h-3.5"></lucide-icon>
                      Agregar Médico
                    </button>
                  </div>

                  <div class="space-y-2.5">
                    @for (m of medicosHonorariosEdit(); track $index; let idx = $index) {
                      <div class="p-3 bg-gray-950 rounded-xl border border-gray-800 grid grid-cols-12 gap-2 items-center">
                        <div class="col-span-4">
                          <label class="text-[10px] text-gray-400 block mb-0.5">Médico</label>
                          <select
                            [(ngModel)]="m.medicoId"
                            (change)="onMedicoSelected(m)"
                            class="w-full bg-gray-900 border border-gray-700 rounded-lg p-1.5 text-xs text-white focus:outline-none focus:border-sky-500"
                          >
                            @for (med of catalogoMedicos(); track med.id) {
                              <option [value]="med.id">{{ med.nombre }}</option>
                            }
                          </select>
                        </div>

                        <div class="col-span-3">
                          <label class="text-[10px] text-gray-400 block mb-0.5">Especialidad</label>
                          <span class="text-xs text-gray-300 block truncate py-1.5">{{ m.especialidadNombre || 'General' }}</span>
                        </div>

                        <div class="col-span-3">
                          <label class="text-[10px] text-gray-400 block mb-0.5">Honorario (USD $)</label>
                          <input
                            type="number"
                            step="0.01"
                            [(ngModel)]="m.montoHonorarioUsd"
                            class="w-full bg-gray-900 border border-gray-700 rounded-lg p-1.5 text-xs text-white font-mono focus:outline-none focus:border-sky-500"
                          />
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

            <!-- PESTAÑA 4: REASIGNAR FECHA Y HORA (REPROGRAMACIÓN) -->
            @if (tabActivo() === 'reprogramar') {
              <div class="space-y-4 animate-fade-in">
                <div class="bg-gray-900/60 p-5 rounded-2xl border border-gray-800 space-y-4">
                  <div class="flex items-center justify-between border-b border-gray-800/80 pb-3">
                    <div class="flex items-center gap-2.5">
                      <div class="w-8 h-8 rounded-xl bg-sky-500/10 border border-sky-500/20 flex items-center justify-center text-sky-400">
                        <lucide-icon name="calendar" class="w-4 h-4"></lucide-icon>
                      </div>
                      <div>
                        <h4 class="text-xs font-bold text-white uppercase tracking-wider">Reasignar Fecha y Hora Quirúrgica</h4>
                        <p class="text-[11px] text-gray-400">Reprogramación de orden quirúrgica con registro inmutable en auditoría.</p>
                      </div>
                    </div>
                  </div>

                  <!-- Resumen de Programación Actual -->
                  <div class="grid grid-cols-2 gap-3 p-3.5 bg-gray-950/80 rounded-xl border border-gray-800/80 text-xs">
                    <div>
                      <span class="text-[10px] text-gray-500 block font-semibold">FECHA / HORA ACTUAL</span>
                      <span class="text-white font-mono font-bold text-xs mt-0.5 block">
                        {{ paciente()!.fechaHoraProgramada | date:'dd/MM/yyyy hh:mm a' }}
                      </span>
                    </div>
                    <div>
                      <span class="text-[10px] text-gray-500 block font-semibold">SALA / QUIRÓFANO</span>
                      <span class="text-sky-400 font-bold text-xs mt-0.5 block">
                        {{ paciente()!.salaQuirofano || 'Quirófano 1' }}
                      </span>
                    </div>
                  </div>

                  <!-- Formulario de Nueva Fecha y Motivo -->
                  <div class="space-y-3.5 text-xs">
                    <div>
                      <label class="text-gray-300 font-semibold block mb-1.5 flex items-center gap-1.5">
                        <lucide-icon name="clock" class="w-3.5 h-3.5 text-sky-400"></lucide-icon>
                        Nueva Fecha y Hora Programada *
                      </label>
                      <input
                        type="datetime-local"
                        [ngModel]="nuevaFechaHoraReprogramar()"
                        (ngModelChange)="nuevaFechaHoraReprogramar.set($event)"
                        class="w-full bg-gray-950 border border-gray-700 rounded-xl px-3 py-2.5 text-white font-mono focus:outline-none focus:border-sky-500"
                      />
                    </div>

                    <div>
                      <label class="text-gray-300 font-semibold block mb-1.5 flex items-center gap-1.5">
                        <lucide-icon name="file-text" class="w-3.5 h-3.5 text-amber-400"></lucide-icon>
                        Motivo / Justificación de la Reasignación *
                      </label>
                      <textarea
                        rows="3"
                        [ngModel]="motivoReprogramacion()"
                        (ngModelChange)="motivoReprogramacion.set($event)"
                        placeholder="Describa el motivo médico, administrativo o técnico para la reprogramación..."
                        class="w-full bg-gray-950 border border-gray-700 rounded-xl p-3 text-white focus:outline-none focus:border-sky-500 resize-none text-xs"
                      ></textarea>
                    </div>

                    <div class="pt-2 flex justify-end">
                      <button
                        (click)="guardarReprogramacion()"
                        [disabled]="guardandoReprogramacion() || !motivoReprogramacion().trim() || !nuevaFechaHoraReprogramar()"
                        class="px-5 py-2.5 bg-sky-600 hover:bg-sky-500 disabled:opacity-40 text-white rounded-xl text-xs font-bold transition flex items-center gap-2 shadow-lg shadow-sky-900/20"
                      >
                        <lucide-icon name="calendar-check" class="w-4 h-4"></lucide-icon>
                        {{ guardandoReprogramacion() ? 'Reasignando...' : 'Confirmar Reasignación de Horario' }}
                      </button>
                    </div>
                  </div>
                </div>
              </div>
            }

            <!-- PESTAÑA 5: AUDITORÍA Y LOGS -->
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
          <div class="p-4 border-t border-gray-800 bg-gray-900/80 flex items-center justify-between">
            <button
              (click)="abrirModalCancelacion()"
              [disabled]="paciente()!.estado === 'Cancelada'"
              class="px-4 py-2 bg-rose-500/10 hover:bg-rose-500/20 text-rose-400 hover:text-rose-300 border border-rose-500/30 rounded-xl text-xs font-semibold transition flex items-center gap-1.5 disabled:opacity-40"
            >
              <lucide-icon name="x-circle" class="w-4 h-4"></lucide-icon>
              Cancelar Cirugía
            </button>
            <button (click)="cerrar.emit()" class="px-4 py-2 bg-gray-800 hover:bg-gray-700 text-gray-300 rounded-xl text-xs font-semibold transition">
              Cerrar Panel
            </button>
          </div>

        </div>
      </div>

      <!-- MODAL DE TRASLADO CON ÁREA Y CAMA/QUIRÓFANO FILTRADO -->
      @if (modalTrasladoVisible()) {
        <div class="fixed inset-0 bg-black/80 backdrop-blur-sm z-[550] flex items-center justify-center p-4">
          <div class="bg-gray-900 border border-gray-800 rounded-2xl shadow-2xl w-full max-w-md p-5 space-y-4 text-white animate-fade-in">
            <div class="flex justify-between items-center border-b border-gray-800 pb-3">
              <h3 class="text-sm font-bold text-indigo-400 flex items-center gap-2">
                <lucide-icon name="arrow-right-left" class="w-4 h-4"></lucide-icon>
                Traslado de Paciente Quirúrgico
              </h3>
              <button (click)="modalTrasladoVisible.set(false)" class="text-gray-400 hover:text-white transition">
                <lucide-icon name="x" class="w-4 h-4"></lucide-icon>
              </button>
            </div>

            <div class="space-y-3.5 text-xs">
              <!-- Información actual del paciente -->
              <div class="p-2.5 bg-gray-950 rounded-xl border border-gray-800 space-y-1">
                <p class="text-gray-400 text-[11px]">Paciente: <strong class="text-white">{{ paciente()?.pacienteNombre }}</strong></p>
                <p class="text-gray-400 text-[11px]">Ubicación Actual: <strong class="text-sky-400">{{ paciente()?.ubicacion?.descripcionCompleta || paciente()?.salaQuirofano || 'Sin Asignar' }}</strong></p>
              </div>

              <!-- Selector de Área / Sede Destino -->
              <div>
                <label class="text-gray-300 font-semibold block mb-1">Área / Sede Destino *</label>
                <select
                  [ngModel]="trasladoSedeId()"
                  (ngModelChange)="onSedeTrasladoChange($event)"
                  class="w-full bg-gray-950 border border-gray-700 rounded-xl px-3 py-2 text-white focus:outline-none focus:border-indigo-500 font-medium"
                >
                  <option value="" disabled>-- Seleccione Área / Sede --</option>
                  @for (sede of sedesDisponibles(); track sede.id) {
                    <option [value]="sede.id">{{ sede.nombre }} ({{ sede.codigo }})</option>
                  }
                </select>
              </div>

              <!-- Selector de Cama / Quirófano Filtrado por Sede -->
              <div>
                <label class="text-gray-300 font-semibold block mb-1">
                  {{ esSedeCirugia() ? 'Quirófano / Sala Quirúrgica *' : 'Habitación / Cama Destino *' }}
                </label>
                <select
                  [(ngModel)]="trasladoCamaId"
                  class="w-full bg-gray-950 border border-gray-700 rounded-xl px-3 py-2 text-white focus:outline-none focus:border-indigo-500 font-medium"
                >
                  <option value="" disabled>-- Seleccione {{ esSedeCirugia() ? 'Quirófano / Sala' : 'Cama' }} --</option>
                  @for (cama of camasFiltradasPorSede(); track cama.id) {
                    <option [value]="cama.id">{{ cama.nombre }} ({{ cama.codigo }})</option>
                  }
                </select>
                @if (camasFiltradasPorSede().length === 0 && trasladoSedeId()) {
                  <p class="text-[11px] text-amber-400 mt-1">No hay camas o quirófanos activos registrados en esta área.</p>
                }
              </div>

              <!-- Observación de Traslado -->
              <div>
                <label class="text-gray-300 font-semibold block mb-1">Observación / Motivo (Opcional)</label>
                <input
                  type="text"
                  [(ngModel)]="trasladoObservacion"
                  placeholder="Ej: Traslado a recuperación post-operatoria..."
                  class="w-full bg-gray-950 border border-gray-700 rounded-xl px-3 py-2 text-white focus:outline-none focus:border-indigo-500"
                />
              </div>
            </div>

            <div class="flex justify-end gap-2.5 pt-2 border-t border-gray-800">
              <button
                (click)="modalTrasladoVisible.set(false)"
                class="px-4 py-2 bg-gray-800 hover:bg-gray-700 text-gray-300 rounded-xl text-xs font-semibold transition"
              >
                Cancelar
              </button>
              <button
                (click)="confirmarTraslado()"
                [disabled]="guardandoTraslado() || !trasladoSedeId() || !trasladoCamaId()"
                class="px-4 py-2 bg-indigo-600 hover:bg-indigo-500 disabled:opacity-40 text-white rounded-xl text-xs font-bold transition flex items-center gap-1.5 shadow-lg shadow-indigo-900/20"
              >
                @if (guardandoTraslado()) {
                  <lucide-icon name="loader-2" class="w-3.5 h-3.5 animate-spin"></lucide-icon>
                  Procesando...
                } @else {
                  <lucide-icon name="check" class="w-3.5 h-3.5"></lucide-icon>
                  Confirmar Traslado
                }
              </button>
            </div>
          </div>
        </div>
      }
    }
  `
})
export class PanelDetalleCirugiaComponent implements OnInit {
  private pabellonService = inject(PabellonService);
  private medicoService = inject(MedicoService);
  private inventoryService = inject(InventoryService);
  private multiSedeService = inject(MultiSedeService);

  paciente = input<PacienteQuirurgicoItem | null>(null);
  cerrar = output<void>();
  recargar = output<void>();

  tabActivo = signal<'clinica' | 'insumos' | 'honorarios' | 'reprogramar' | 'logs'>('clinica');

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

  // Gestión de Preoperatorios Interactivos
  catalogoRequisitos = signal<RequisitoCirugia[]>([]);
  mostrarNuevoRequisito = signal<boolean>(false);
  nuevoRequisitoNombre = '';
  nuevoRequisitoDescripcion = '';

  // Gestión de Reasignación de Horario (Reprogramación)
  nuevaFechaHoraReprogramar = signal<string>('');
  motivoReprogramacion = signal<string>('');
  guardandoReprogramacion = signal<boolean>(false);

  // Gestión de Traslados Reactivos Filtrados
  sedesDisponibles = signal<Sede[]>([]);
  todasLasAreas = signal<AreaClinica[]>([]);
  modalTrasladoVisible = signal<boolean>(false);
  guardandoTraslado = signal<boolean>(false);
  trasladoSedeId = signal<string>('');
  trasladoCamaId = signal<string>('');
  trasladoObservacion = '';

  camasFiltradasPorSede = computed(() => {
    const sedeId = this.trasladoSedeId();
    if (!sedeId) return [];

    const todas = this.todasLasAreas();
    const sedeSeleccionada = this.sedesDisponibles().find(s => s.id === sedeId);
    const esCirugia = sedeSeleccionada && (sedeSeleccionada.codigo === 'CIR' || sedeSeleccionada.nombre.toLowerCase().includes('cirug'));

    return todas.filter(a => {
      if (a.sedeId && a.sedeId === sedeId) {
        return true;
      }
      if (esCirugia && (a.nombre.toLowerCase().includes('quiróf') || a.nombre.toLowerCase().includes('quirof') || a.codigo?.toLowerCase().includes('qx') || a.nombre.toLowerCase().includes('parto'))) {
        return true;
      }
      return false;
    });
  });

  esSedeCirugia = computed(() => {
    const sedeId = this.trasladoSedeId();
    if (!sedeId) return false;
    const sede = this.sedesDisponibles().find(s => s.id === sedeId);
    return sede ? (sede.codigo === 'CIR' || sede.nombre.toLowerCase().includes('cirug')) : false;
  });

  // Solicitud Ad-Hoc
  mostrarSolicitudAdHoc = signal<boolean>(false);
  insumoAdHocId = '';
  cantidadAdHoc = 1;
  observacionAdHoc = '';

  constructor() {
    effect(() => {
      const p = this.paciente();
      if (p) {
        this.sincronizarDatos();
      }
    });
  }

  ngOnInit() {
    this.cargarCatalogos();
    this.sincronizarDatos();
  }

  cargarCatalogos() {
    this.medicoService.getAll().subscribe(meds => {
      this.catalogoMedicos.set(meds || []);
      this.sincronizarDatos();
    });

    this.multiSedeService.getSedes().subscribe(sedes => {
      const activas = (sedes || []).filter(s => s.activo);
      this.sedesDisponibles.set(activas);
    });

    this.multiSedeService.getAreasClinicas().subscribe(areas => {
      this.todasLasAreas.set(areas || []);
    });

    this.pabellonService.getRequisitosCatalogo().subscribe(reqs => {
      this.catalogoRequisitos.set(reqs || []);
    });
  }

  seleccionarTab(tab: 'clinica' | 'insumos' | 'honorarios' | 'reprogramar' | 'logs') {
    this.tabActivo.set(tab);
    if (tab === 'insumos') {
      this.cargarInsumosSiEsNecesario();
    }
  }

  cargarInsumosSiEsNecesario() {
    if (this.catalogoInsumos().length === 0) {
      this.inventoryService.getInsumos().subscribe(ins => {
        this.catalogoInsumos.set(ins || []);
        if (ins && ins.length > 0) {
          this.insumoAdHocId = ins[0].id;
        }
      });
    }
  }

  abrirModalTraslado() {
    this.trasladoObservacion = '';
    const sedes = this.sedesDisponibles();
    const p = this.paciente();
    if (sedes.length > 0) {
      if (p?.estado === 'EnCirugia' || p?.estado === 'Finalizado') {
        // En estado quirúrgico avanzado, sugerir por defecto hospitalización o UCI
        const sedeDestino = sedes.find(s => s.codigo === 'HOSP' || s.codigo === 'UCI' || s.nombre.toLowerCase().includes('hosp') || s.nombre.toLowerCase().includes('intensiv'));
        this.trasladoSedeId.set(sedeDestino ? sedeDestino.id : sedes[0].id);
      } else {
        // En estado preoperatorio, sugerir el área quirúrgica/quirófano
        const sedeCirugia = sedes.find(s => s.codigo === 'CIR' || s.nombre.toLowerCase().includes('cirug'));
        this.trasladoSedeId.set(sedeCirugia ? sedeCirugia.id : sedes[0].id);
      }
      
      const camas = this.camasFiltradasPorSede();
      if (camas.length > 0) {
        this.trasladoCamaId.set(camas[0].id);
      } else {
        this.trasladoCamaId.set('');
      }
    }
    this.modalTrasladoVisible.set(true);
  }

  onSedeTrasladoChange(sedeId: string) {
    this.trasladoSedeId.set(sedeId);
    const camas = this.camasFiltradasPorSede();
    if (camas.length > 0) {
      this.trasladoCamaId.set(camas[0].id);
    } else {
      this.trasladoCamaId.set('');
    }
  }

  confirmarTraslado() {
    const p = this.paciente();
    const sedeId = this.trasladoSedeId();
    const camaId = this.trasladoCamaId();
    if (!p || !sedeId || !camaId) return;

    this.guardandoTraslado.set(true);
    this.pabellonService.trasladarPaciente({
      ordenCirugiaId: p.id,
      sedeDestinoId: sedeId,
      areaClinicaCamaId: camaId,
      observacion: this.trasladoObservacion.trim()
    }).subscribe({
      next: () => {
        this.guardandoTraslado.set(false);
        this.modalTrasladoVisible.set(false);
        this.recargar.emit();
      },
      error: (err) => {
        this.guardandoTraslado.set(false);
        console.error('[PABELLON] Error en traslado:', err);
        alert(err?.error?.message || 'Error al ejecutar el traslado.');
      }
    });
  }

  sincronizarDatos() {
    const p = this.paciente();
    if (!p) return;

    this.precioDerechoSala = p.precioDerechoSalaUsd || 0;
    this.precioBaseCirugia = p.precioBaseUsd || 0;
    this.esAlquiladoPabellon = p.esAlquilado || false;

    if (p.fechaHoraProgramada) {
      try {
        const d = new Date(p.fechaHoraProgramada);
        // Formato YYYY-MM-DDTHH:mm para input datetime-local
        const y = d.getFullYear();
        const m = String(d.getMonth() + 1).padStart(2, '0');
        const day = String(d.getDate()).padStart(2, '0');
        const h = String(d.getHours()).padStart(2, '0');
        const min = String(d.getMinutes()).padStart(2, '0');
        this.nuevaFechaHoraReprogramar.set(`${y}-${m}-${day}T${h}:${min}`);
      } catch {
        this.nuevaFechaHoraReprogramar.set('');
      }
    }

    let lista = (p.medicosHonorarios || []).map(m => ({
      medicoId: m.medicoId,
      especialidadId: m.especialidadId,
      especialidadNombre: m.especialidadNombre,
      montoHonorarioUsd: m.montoHonorarioUsd,
      esCirujanoPrincipal: m.esCirujanoPrincipal
    }));

    // Fallback: Si la lista de honorarios aún está vacía pero la orden tiene MedicoPrincipalId, asignar el Cirujano Principal
    if (lista.length === 0 && p.medicoPrincipalId) {
      const med = this.catalogoMedicos().find(m => m.id === p.medicoPrincipalId);
      lista = [{
        medicoId: p.medicoPrincipalId,
        especialidadId: med?.especialidadId || '00000000-0000-0000-0000-000000000000',
        especialidadNombre: med?.especialidad || 'Cirugía General',
        montoHonorarioUsd: p.precioBaseUsd || 0,
        esCirujanoPrincipal: true
      }];
    }

    this.medicosHonorariosEdit.set(lista);
  }

  guardarNuevoRequisito() {
    const p = this.paciente();
    if (!p || !this.nuevoRequisitoNombre.trim()) return;

    this.pabellonService.addRequisitoAOrden(p.id, this.nuevoRequisitoNombre.trim(), this.nuevoRequisitoDescripcion.trim()).subscribe({
      next: () => {
        this.mostrarNuevoRequisito.set(false);
        this.nuevoRequisitoNombre = '';
        this.nuevoRequisitoDescripcion = '';
        this.recargar.emit();
      },
      error: (err) => console.error('[PABELLON] Error al agregar preoperatorio:', err)
    });
  }

  seleccionarDelCatalogo(event: Event) {
    const id = (event.target as HTMLSelectElement).value;
    const req = this.catalogoRequisitos().find(r => r.id === id);
    if (req) {
      this.nuevoRequisitoNombre = req.nombre;
      this.nuevoRequisitoDescripcion = req.descripcion || '';
    }
  }

  cargarRequisitosPredeterminados() {
    const p = this.paciente();
    if (!p) return;

    const defaults = [
      { nombre: 'Evaluación Cardiovascular y EKG', desc: 'Informe cardiológico vigente y apto quirúrgico firmado' },
      { nombre: 'Ayuno Completo (>= 8 horas)', desc: 'Verificación estricta de ingesta de alimentos y líquidos' },
      { nombre: 'Laboratorios (Hematología, PT, PTT, Glicemia)', desc: 'Perfil preoperatorio completo emitido en los últimos 15 días' },
      { nombre: 'Consentimiento Informado Firmado', desc: 'Documento legal de autorización quirúrgica y anestésica firmado por paciente o familiar' },
      { nombre: 'Reserva de Hemoderivados / Tipaje Sanguíneo', desc: 'Disponibilidad de concentrado globular o plasma en banco de sangre si aplica' }
    ];

    let count = 0;
    defaults.forEach(d => {
      this.pabellonService.addRequisitoAOrden(p.id, d.nombre, d.desc).subscribe({
        next: () => {
          count++;
          if (count === defaults.length) {
            this.recargar.emit();
          }
        },
        error: (err) => console.error('[PABELLON] Error al cargar preoperatorio estándar:', err)
      });
    });
  }

  eliminarRequisito(req: OrdenCirugiaRequisito) {
    const p = this.paciente();
    if (!p) return;
    if (!confirm(`¿Desea remover el requisito preoperatorio '${req.nombre}' de esta cirugía?`)) return;

    this.pabellonService.removeRequisitoDeOrden(p.id, req.requisitoCirugiaId || req.id).subscribe({
      next: () => {
        this.recargar.emit();
      },
      error: (err) => console.error('[PABELLON] Error al remover preoperatorio:', err)
    });
  }

  eliminarTodosLosRequisitos() {
    const p = this.paciente();
    if (!p || p.requisitos.length <= 1) return;
    if (!confirm(`¿Está seguro de eliminar TODOS los ${p.requisitos.length} requisitos preoperatorios de esta cirugía?`)) return;

    this.pabellonService.clearRequisitosDeOrden(p.id).subscribe({
      next: () => {
        this.recargar.emit();
      },
      error: (err) => console.error('[PABELLON] Error al eliminar todos los preoperatorios:', err)
    });
  }

  esApto(): boolean {
    const p = this.paciente();
    if (!p || p.requisitos.length === 0) return false;
    return p.requisitos.every(r => r.cumplido);
  }

  getRequisitosCumplidos(): number {
    return this.paciente()?.requisitos.filter(r => r.cumplido).length || 0;
  }

  toggleRequisito(req: OrdenCirugiaRequisito) {
    const p = this.paciente();
    if (!p) return;

    const nuevoEstado = !req.cumplido;
    // Mutación optimista en 0 ms sin bloqueo de interfaz
    req.cumplido = nuevoEstado;
    if (nuevoEstado) {
      req.verificadoPor = 'admin';
    }

    this.pabellonService.toggleRequisito(p.id, req.requisitoCirugiaId, nuevoEstado).subscribe({
      next: () => {
        // Guardado con éxito en segundo plano
      },
      error: (err) => {
        // Revertir ante falla
        req.cumplido = !nuevoEstado;
        console.error('[PABELLON] Error al marcar preoperatorio:', err);
        alert(err?.error?.message || 'Error al actualizar requisito preoperatorio.');
      }
    });
  }

  guardarReprogramacion() {
    const p = this.paciente();
    const fecha = this.nuevaFechaHoraReprogramar();
    const motivo = this.motivoReprogramacion().trim();
    if (!p || !fecha || !motivo) {
      alert('Por favor seleccione la nueva fecha/hora e ingrese el motivo de la reprogramación.');
      return;
    }

    this.guardandoReprogramacion.set(true);
    this.pabellonService.reprogramarCirugia({
      ordenCirugiaId: p.id,
      nuevaFechaHora: new Date(fecha).toISOString(),
      motivo: motivo
    }).subscribe({
      next: () => {
        this.guardandoReprogramacion.set(false);
        this.motivoReprogramacion.set('');
        alert('Fecha y hora quirúrgica reasignada exitosamente.');
        this.recargar.emit();
      },
      error: (err) => {
        this.guardandoReprogramacion.set(false);
        console.error('[PABELLON] Error al reprogramar:', err);
        alert(err?.error?.message || 'Error al reprogramar la fecha y hora de la cirugía.');
      }
    });
  }

  cambiarEstado(nuevoEstado: string) {
    const p = this.paciente();
    if (!p) return;

    this.pabellonService.cambiarEstado({
      ordenCirugiaId: p.id,
      nuevoEstado
    }).subscribe({
      next: () => {
        this.recargar.emit();
      },
      error: (err) => {
        console.error('[PABELLON] Error al cambiar estado:', err);
        alert(err?.error?.message || 'Error al cambiar estado quirúrgico.');
      }
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
    }).subscribe({
      next: () => {
        this.recargar.emit();
      },
      error: (err) => {
        console.error('[PABELLON] Error al cancelar cirugía:', err);
        alert(err?.error?.message || 'Error al cancelar la cirugía.');
      }
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
