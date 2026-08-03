import { Component, inject, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PabellonService, OrdenCirugia, CrearOrdenCirugiaRequest } from '../../../core/services/pabellon.service';
import { MedicoService } from '../../../core/services/medico.service';
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
  Stethoscope,
  Grid,
  FileText,
  HeartPulse,
  Info
} from 'lucide-angular';

@Component({
  selector: 'app-pabellon-gestion',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule, GestionConsumoModalComponent],
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
            <button (click)="vistaModo.set('pizarra')" [class.bg-sky-600]="vistaModo() === 'pizarra'" [class.text-white]="vistaModo() === 'pizarra'" [class.text-gray-400]="vistaModo() !== 'pizarra'" class="px-3 py-1.5 rounded-lg text-xs font-semibold transition flex items-center gap-1.5">
              <lucide-icon name="grid" class="w-3.5 h-3.5"></lucide-icon>
              Pizarra Horaria
            </button>
            <button (click)="vistaModo.set('calendario')" [class.bg-sky-600]="vistaModo() === 'calendario'" [class.text-white]="vistaModo() === 'calendario'" [class.text-gray-400]="vistaModo() !== 'calendario'" class="px-3 py-1.5 rounded-lg text-xs font-semibold transition flex items-center gap-1.5">
              <lucide-icon name="calendar" class="w-3.5 h-3.5"></lucide-icon>
              Tarjetas
            </button>
            <button (click)="vistaModo.set('lista')" [class.bg-sky-600]="vistaModo() === 'lista'" [class.text-white]="vistaModo() === 'lista'" [class.text-gray-400]="vistaModo() !== 'lista'" class="px-3 py-1.5 rounded-lg text-xs font-semibold transition flex items-center gap-1.5">
              <lucide-icon name="list" class="w-3.5 h-3.5"></lucide-icon>
              Lista
            </button>
          </div>

          <button (click)="abrirModalNuevaCirugia()" class="bg-sky-600 hover:bg-sky-500 text-white font-semibold text-xs px-4 py-2 rounded-xl shadow-lg shadow-sky-600/20 transition flex items-center gap-2">
            <lucide-icon name="plus" class="w-4 h-4"></lucide-icon>
            Programar Cirugía
          </button>
        </div>
      </div>

      <!-- Combined Filter & Search Bar with Aligned Icons -->
      <div class="flex flex-wrap items-center justify-between gap-3 bg-gray-900/40 p-3 rounded-xl border border-gray-800/80 backdrop-blur-sm">
        
        <div class="flex flex-wrap items-center gap-3 flex-1 min-w-[280px]">
          <!-- Input Búsqueda con Icono Perfectamente Alineado -->
          <div class="relative flex-1 min-w-[220px]">
            <lucide-icon name="search" class="w-4 h-4 text-gray-500 absolute left-3 top-1/2 -translate-y-1/2 pointer-events-none"></lucide-icon>
            <input type="text" [ngModel]="filtroTexto()" (ngModelChange)="filtroTexto.set($event)" placeholder="Buscar paciente, cédula, cirugía, doctor o especialidad..." class="w-full bg-gray-950 border border-gray-800 rounded-xl pl-9 pr-4 py-2 text-xs text-white placeholder-gray-500 focus:border-sky-500 focus:outline-none focus:ring-1 focus:ring-sky-500 transition">
          </div>

          <!-- Filtro de Estado -->
          <div class="relative min-w-[160px]">
            <lucide-icon name="filter" class="w-3.5 h-3.5 text-gray-500 absolute left-3 top-1/2 -translate-y-1/2 pointer-events-none"></lucide-icon>
            <select [ngModel]="filtroEstado()" (ngModelChange)="onFiltroEstadoChange($event)" class="w-full bg-gray-950 border border-gray-800 rounded-xl pl-8 pr-4 py-2 text-xs text-white focus:border-sky-500 focus:outline-none focus:ring-1 focus:ring-sky-500 appearance-none transition">
              <option value="">Todos los Estados</option>
              <option value="Programado">Programado</option>
              <option value="PendienteEjecucion">Pendiente de Ejecución</option>
              <option value="EnProceso">En Proceso</option>
              <option value="Completada">Completada</option>
              <option value="Cancelada">Cancelada</option>
            </select>
          </div>

          <!-- Filtro por Médico -->
          <div class="relative min-w-[160px]">
            <lucide-icon name="user-check" class="w-3.5 h-3.5 text-gray-500 absolute left-3 top-1/2 -translate-y-1/2 pointer-events-none"></lucide-icon>
            <select [ngModel]="filtroMedicoId()" (ngModelChange)="filtroMedicoId.set($event)" class="w-full bg-gray-950 border border-gray-800 rounded-xl pl-8 pr-4 py-2 text-xs text-white focus:border-sky-500 focus:outline-none focus:ring-1 focus:ring-sky-500 appearance-none transition">
              <option value="">Todos los Cirujanos</option>
              <option *ngFor="let med of medicos()" [value]="med.id">{{ med.nombre }}</option>
            </select>
          </div>
        </div>

        <div class="flex items-center gap-2">
          <button (click)="cargarOrdenes()" title="Recargar datos" class="p-2 bg-gray-800/80 hover:bg-gray-700 text-gray-300 rounded-xl border border-gray-700/60 transition">
            <lucide-icon name="refresh-ccw" class="w-4 h-4"></lucide-icon>
          </button>
        </div>
      </div>

      <!-- MAIN CONTAINER: PIZARRA + PANEL DE DETALLES -->
      <div class="grid grid-cols-1 lg:grid-cols-12 gap-6">

        <!-- CONTENT AREA (MODO PIZARRA, CALENDARIO O LISTA) -->
        <div [class]="ordenSeleccionada() ? 'lg:col-span-8 space-y-4' : 'lg:col-span-12 space-y-4'">

          <!-- VISTA 1: PIZARRA DIGITAL QUIRÚRGICA (SCHEDULE BOARD GRID) -->
          <div *ngIf="vistaModo() === 'pizarra'" class="bg-gray-900/70 border border-gray-800 rounded-2xl p-4 backdrop-blur-md shadow-2xl space-y-4">
            
            <!-- Header Rango de Fecha -->
            <div class="flex justify-between items-center bg-gray-950/80 px-4 py-2.5 rounded-xl border border-gray-800">
              <div class="flex items-center gap-2 text-xs font-semibold text-sky-400">
                <lucide-icon name="calendar" class="w-4 h-4"></lucide-icon>
                <span>{{ rangoFechasSemanaTexto() }}</span>
              </div>

              <div class="flex items-center gap-2">
                <span class="text-[11px] text-gray-400 font-mono">
                  {{ ordenesFiltradas().length }} cirugías en pantalla
                </span>
              </div>
            </div>

            <!-- Schedule Grid Matrix -->
            <div class="overflow-x-auto pb-2">
              <div class="min-w-[900px]">
                
                <!-- Headers de Días -->
                <div class="grid grid-cols-8 gap-2 mb-2 text-center text-xs font-semibold text-gray-400 bg-gray-950/60 py-2.5 px-1 rounded-xl border border-gray-800/60">
                  <div class="text-[11px] text-gray-500 font-mono flex items-center justify-center gap-1">
                    <lucide-icon name="clock" class="w-3.5 h-3.5"></lucide-icon> Hora
                  </div>
                  <div *ngFor="let dia of diasSemana()" class="flex flex-col items-center">
                    <span class="text-white font-bold text-xs">{{ dia.nombre }}</span>
                    <span class="text-[10px] text-gray-500 font-mono">{{ dia.fechaStr }}</span>
                  </div>
                </div>

                <!-- Filas Horarias (07:00 a 18:00) -->
                <div class="space-y-2">
                  <div *ngFor="let hora of franjasHorarias" class="grid grid-cols-8 gap-2 items-stretch min-h-[90px]">
                    
                    <!-- Columna de Hora -->
                    <div class="bg-gray-950/70 border border-gray-800/60 rounded-xl flex items-center justify-center text-xs font-mono font-bold text-gray-400">
                      {{ hora }}
                    </div>

                    <!-- 7 Columnas de Días -->
                    <div *ngFor="let dia of diasSemana()" class="bg-gray-950/30 border border-gray-800/40 rounded-xl p-1.5 flex flex-col gap-1.5 relative hover:border-gray-700/60 transition">
                      
                      <!-- Tarjetas de Cirugías en este Slot -->
                      <div *ngFor="let orden of getCirugiasParaSlot(dia.fechaISO, hora)" (click)="seleccionarCirugia(orden)" [ngClass]="getCardBorderClass(orden)" class="p-2.5 rounded-xl border backdrop-blur-sm cursor-pointer shadow-md transition transform hover:-translate-y-0.5 flex flex-col justify-between space-y-1.5 group">
                        
                        <!-- Header Tarjeta: Doctor & Estado -->
                        <div class="flex items-center justify-between gap-1">
                          <span class="text-[11px] font-extrabold text-white truncate group-hover:text-sky-300 transition">
                            Dr. {{ obtenerPrimerNombreDoctor(orden.medicoNombre) }}
                          </span>
                          <span [ngClass]="getBadgeClass(orden.estado)" class="px-1.5 py-0.5 rounded text-[9px] font-bold uppercase tracking-wider">
                            {{ obtenerBadgeEstado(orden.estado) }}
                          </span>
                        </div>

                        <!-- Item Cirugía (Nombre Principal) -->
                        <div class="text-xs font-bold text-amber-200/90 line-clamp-2 leading-tight">
                          {{ orden.descripcionCirugia }}
                        </div>

                        <!-- Especialidad -->
                        <div *ngIf="orden.especialidad" class="text-[10px] text-gray-400 font-medium">
                          {{ orden.especialidad }}
                        </div>

                        <!-- Paciente -->
                        <div class="text-[10px] text-sky-200/80 truncate font-semibold">
                          Pac: {{ orden.pacienteNombre }}
                        </div>

                      </div>

                    </div>

                  </div>
                </div>

              </div>
            </div>

          </div>

          <!-- VISTA 2: CALENDARIO EN TARJETAS (CARDS GRID) -->
          <div *ngIf="vistaModo() === 'calendario'" class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
            <div *ngFor="let orden of ordenesFiltradas()" (click)="seleccionarCirugia(orden)" [ngClass]="getCardBorderClass(orden)" class="bg-gray-900/80 border rounded-2xl p-4 shadow-xl flex flex-col justify-between transition cursor-pointer hover:border-gray-700">

              <div>
                <div class="flex items-center justify-between gap-2 mb-2">
                  <span [ngClass]="getBadgeClass(orden.estado)" class="px-2 py-0.5 rounded-md text-[10px] font-bold uppercase">
                    {{ obtenerBadgeEstado(orden.estado) }}
                  </span>
                  <span class="text-[11px] font-mono text-gray-400 flex items-center gap-1">
                    <lucide-icon name="clock" class="w-3 h-3 text-gray-500"></lucide-icon>
                    {{ orden.fechaHoraProgramada | date:'shortTime' }} ({{ orden.fechaHoraProgramada | date:'dd/MM' }})
                  </span>
                </div>

                <h3 class="text-sm font-bold text-white mb-1 line-clamp-1">{{ orden.descripcionCirugia }}</h3>
                <div *ngIf="orden.especialidad" class="text-xs text-sky-400 font-semibold mb-2">
                  {{ orden.especialidad }}
                </div>

                <div class="text-xs text-gray-300 mb-1">
                  Doctor: <span class="text-white font-bold">{{ orden.medicoNombre }}</span>
                </div>

                <div class="text-xs text-gray-300 mb-2">
                  Paciente: <span class="text-sky-300 font-semibold">{{ orden.pacienteNombre }}</span>
                  <span class="text-gray-500 text-[10px]"> ({{ orden.pacienteCedula }})</span>
                </div>
              </div>

              <div class="pt-3 border-t border-gray-800/80 flex items-center justify-between gap-2">
                <span class="text-xs font-mono text-emerald-400 font-bold">$ {{ orden.precioBaseUsd }} USD</span>
                <button (click)="abrirModalConsumo(orden.id); $event.stopPropagation()" class="bg-gray-800 hover:bg-gray-700 text-sky-400 font-semibold text-xs px-3 py-1.5 rounded-lg transition flex items-center gap-1.5">
                  <lucide-icon name="package" class="w-3.5 h-3.5"></lucide-icon>
                  Consumo & Logs
                </button>
              </div>

            </div>
          </div>

          <!-- VISTA 3: TABLA DE LISTA DE CIRUGÍAS -->
          <div *ngIf="vistaModo() === 'lista'" class="bg-gray-900/80 border border-gray-800 rounded-2xl overflow-hidden shadow-xl">
            <table class="w-full text-left text-xs border-collapse">
              <thead>
                <tr class="border-b border-gray-800 text-gray-400 bg-gray-950/80 uppercase text-[10px] tracking-wider">
                  <th class="py-3 px-4">Fecha / Hora</th>
                  <th class="py-3 px-4">Cirugía</th>
                  <th class="py-3 px-4">Paciente</th>
                  <th class="py-3 px-4">Médico Cirujano</th>
                  <th class="py-3 px-4 text-center">Estado</th>
                  <th class="py-3 px-4 text-right">Precio ($)</th>
                  <th class="py-3 px-4 text-center">Acción</th>
                </tr>
              </thead>
              <tbody class="divide-y divide-gray-800/60">
                <tr *ngFor="let orden of ordenesFiltradas()" (click)="seleccionarCirugia(orden)" [ngClass]="{'bg-sky-950/30': ordenSeleccionada()?.id === orden.id}" class="hover:bg-gray-800/40 cursor-pointer transition">
                  <td class="py-3 px-4 font-mono text-gray-300">
                    {{ orden.fechaHoraProgramada | date:'dd/MM/yyyy HH:mm' }}
                  </td>
                  <td class="py-3 px-4 font-bold text-white">
                    {{ orden.descripcionCirugia }}
                    <div *ngIf="orden.especialidad" class="text-[10px] text-gray-400 font-normal">{{ orden.especialidad }}</div>
                  </td>
                  <td class="py-3 px-4">
                    <div class="text-gray-200 font-semibold">{{ orden.pacienteNombre }}</div>
                    <div class="text-[10px] text-gray-500">{{ orden.pacienteCedula }}</div>
                  </td>
                  <td class="py-3 px-4 text-gray-300 font-medium">
                    {{ orden.medicoNombre }}
                  </td>
                  <td class="py-3 px-4 text-center">
                    <span [ngClass]="getBadgeClass(orden.estado)" class="px-2 py-0.5 rounded-md text-[10px] font-bold uppercase">
                      {{ obtenerBadgeEstado(orden.estado) }}
                    </span>
                  </td>
                  <td class="py-3 px-4 text-right font-mono text-emerald-400 font-bold">
                    $ {{ orden.precioBaseUsd }}
                  </td>
                  <td class="py-3 px-4 text-center">
                    <button (click)="abrirModalConsumo(orden.id); $event.stopPropagation()" class="bg-gray-800 hover:bg-gray-700 text-sky-400 font-semibold text-[11px] px-2.5 py-1 rounded-lg transition">
                      Consumos
                    </button>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>

        </div>

        <!-- RIGHT SIDE PANEL: OBSERVACIONES Y DETALLE QUIRÚRGICO -->
        <div *ngIf="ordenSeleccionada()" class="lg:col-span-4 bg-gray-900/90 border border-gray-800 rounded-2xl p-5 backdrop-blur-md shadow-2xl space-y-5 sticky top-6 self-start">
          
          <!-- Panel Header -->
          <div class="flex items-center justify-between border-b border-gray-800 pb-3">
            <h2 class="text-base font-bold text-white flex items-center gap-2">
              <lucide-icon name="file-text" class="w-5 h-5 text-sky-400"></lucide-icon>
              Observaciones & Detalle
            </h2>
            <button (click)="ordenSeleccionada.set(null)" class="text-gray-400 hover:text-white transition">
              <lucide-icon name="x-circle" class="w-5 h-5"></lucide-icon>
            </button>
          </div>

          <div class="space-y-4">
            
            <!-- Item de la Cirugía -->
            <div class="bg-gray-950/80 p-3.5 rounded-xl border border-gray-800/80">
              <div class="text-[10px] text-gray-500 uppercase font-bold tracking-wider mb-1">Item de la Cirugía</div>
              <div class="text-base font-extrabold text-sky-300 leading-snug">{{ ordenSeleccionada()?.descripcionCirugia }}</div>
              <div *ngIf="ordenSeleccionada()?.especialidad" class="mt-1 inline-block px-2 py-0.5 bg-sky-950 text-sky-400 border border-sky-800 rounded text-[10px] font-semibold">
                {{ ordenSeleccionada()?.especialidad }}
              </div>
            </div>

            <!-- Cuándo fue Programada -->
            <div class="bg-gray-950/60 p-3 rounded-xl border border-gray-800/60 space-y-1">
              <div class="text-[10px] text-gray-500 uppercase font-bold tracking-wider flex items-center gap-1">
                <lucide-icon name="clock" class="w-3 h-3 text-sky-400"></lucide-icon>
                Cuándo fue Programada
              </div>
              <div class="text-xs font-mono font-bold text-white">
                {{ ordenSeleccionada()?.fechaHoraProgramada | date:'fullDate' }}
              </div>
              <div class="text-xs font-mono text-sky-400 font-semibold">
                Hora: {{ ordenSeleccionada()?.fechaHoraProgramada | date:'hh:mm a' }}
              </div>
            </div>

            <!-- Razón de la Cirugía -->
            <div class="bg-gray-950/60 p-3 rounded-xl border border-gray-800/60 space-y-1">
              <div class="text-[10px] text-gray-500 uppercase font-bold tracking-wider flex items-center gap-1">
                <lucide-icon name="heart-pulse" class="w-3 h-3 text-amber-400"></lucide-icon>
                Razón de la Cirugía / Diagnóstico
              </div>
              <div class="text-xs text-gray-200 font-medium italic">
                "{{ ordenSeleccionada()?.razonCirugia || ordenSeleccionada()?.descripcionCirugia || 'Procedimiento programado según criterio médico' }}"
              </div>
            </div>

            <!-- Doctor Cirujano & Paciente -->
            <div class="grid grid-cols-2 gap-3">
              <div class="bg-gray-950/60 p-3 rounded-xl border border-gray-800/60 space-y-1">
                <div class="text-[10px] text-gray-500 uppercase font-bold tracking-wider flex items-center gap-1">
                  <lucide-icon name="user-check" class="w-3 h-3 text-emerald-400"></lucide-icon> Doctor Cirujano
                </div>
                <div class="text-xs font-bold text-white truncate">{{ ordenSeleccionada()?.medicoNombre }}</div>
              </div>

              <div class="bg-gray-950/60 p-3 rounded-xl border border-gray-800/60 space-y-1">
                <div class="text-[10px] text-gray-500 uppercase font-bold tracking-wider flex items-center gap-1">
                  <lucide-icon name="user" class="w-3 h-3 text-sky-400"></lucide-icon> Paciente
                </div>
                <div class="text-xs font-bold text-white truncate">{{ ordenSeleccionada()?.pacienteNombre }}</div>
                <div class="text-[10px] text-gray-500 font-mono">{{ ordenSeleccionada()?.pacienteCedula }}</div>
              </div>
            </div>

            <!-- Notas Operativas & Requisitos Quirúrgicos -->
            <div class="bg-gray-950/40 p-3 rounded-xl border border-gray-800/60 space-y-2">
              <div class="text-[10px] text-gray-500 uppercase font-bold tracking-wider flex items-center gap-1">
                <lucide-icon name="info" class="w-3 h-3 text-purple-400"></lucide-icon> Requisitos Quirúrgicos & Notas
              </div>
              <ul class="text-[11px] text-gray-300 space-y-1 list-disc list-inside">
                <li>{{ ordenSeleccionada()?.notasOperatorias || 'Preparación estándar de quirófano en ayunas.' }}</li>
                <li>{{ ordenSeleccionada()?.requisitosQuirurgicos || 'Monitorización continua y antibiótico profiláctico.' }}</li>
              </ul>
            </div>

            <!-- Precio & Estado Badges -->
            <div class="flex items-center justify-between pt-2">
              <div>
                <span class="text-[10px] text-gray-500 uppercase font-bold block">Estado Actual</span>
                <span [ngClass]="getBadgeClass(ordenSeleccionada()?.estado)" class="px-2.5 py-1 rounded-md text-xs font-bold uppercase tracking-wider inline-block mt-0.5">
                  {{ obtenerBadgeEstado(ordenSeleccionada()?.estado) }}
                </span>
              </div>

              <div class="text-right">
                <span class="text-[10px] text-gray-500 uppercase font-bold block">Precio Base</span>
                <span class="text-sm font-mono text-emerald-400 font-extrabold">$ {{ ordenSeleccionada()?.precioBaseUsd }} USD</span>
              </div>
            </div>

            <!-- Panel Actions -->
            <div class="pt-3 border-t border-gray-800 space-y-2">
              <button (click)="abrirModalConsumo(ordenSeleccionada()!.id)" class="w-full bg-sky-600 hover:bg-sky-500 text-white font-semibold text-xs py-2 rounded-xl transition flex items-center justify-center gap-2 shadow-lg shadow-sky-600/20">
                <lucide-icon name="package" class="w-4 h-4"></lucide-icon>
                Gestionar Consumos e Insumos
              </button>

              <div class="grid grid-cols-2 gap-2">
                <button *ngIf="ordenSeleccionada()?.estado === 'Programado' || ordenSeleccionada()?.estado === 'PendienteEjecucion'" (click)="cambiarEstado(ordenSeleccionada()!.id, 'EnProceso')" class="w-full bg-emerald-600 hover:bg-emerald-500 text-white font-semibold text-xs py-2 rounded-xl transition flex items-center justify-center gap-1.5">
                  <lucide-icon name="play" class="w-3.5 h-3.5"></lucide-icon>
                  Iniciar Cirugía
                </button>

                <button *ngIf="ordenSeleccionada()?.estado === 'EnProceso'" (click)="cambiarEstado(ordenSeleccionada()!.id, 'Completada')" class="w-full bg-emerald-600 hover:bg-emerald-500 text-white font-semibold text-xs py-2 rounded-xl transition flex items-center justify-center gap-1.5">
                  <lucide-icon name="check-circle-2" class="w-3.5 h-3.5"></lucide-icon>
                  Completar
                </button>

                <button *ngIf="ordenSeleccionada()?.estado !== 'Completada' && ordenSeleccionada()?.estado !== 'Cancelada'" (click)="solicitarCancelacion(ordenSeleccionada()!)" class="w-full bg-red-950 hover:bg-red-900 text-red-400 font-semibold text-xs py-2 rounded-xl transition flex items-center justify-center gap-1.5 border border-red-800/50">
                  <lucide-icon name="x-circle" class="w-3.5 h-3.5"></lucide-icon>
                  Cancelar
                </button>
              </div>
            </div>

          </div>

        </div>

      </div>

      <!-- Modal Consumo e Insumos -->
      <app-gestion-consumo-modal *ngIf="modalConsumoOrdenId()" [ordenId]="modalConsumoOrdenId()!" (cerrarModal)="modalConsumoOrdenId.set(null)" (datosActualizados)="cargarOrdenes()"></app-gestion-consumo-modal>

      <!-- Modal Programar Nueva Cirugía -->
      <div *ngIf="modalNuevaCirugiaVisible()" class="fixed inset-0 bg-black/70 backdrop-blur-sm z-50 flex items-center justify-center p-4">
        <div class="bg-gray-900 border border-gray-800 rounded-2xl shadow-2xl w-full max-w-lg p-6 space-y-4 text-white">
          <div class="flex justify-between items-center border-b border-gray-800 pb-3">
            <h3 class="text-lg font-bold text-white flex items-center gap-2">
              <lucide-icon name="plus" class="w-5 h-5 text-sky-400"></lucide-icon>
              Programar Nueva Cirugía
            </h3>
            <button (click)="modalNuevaCirugiaVisible.set(false)" class="text-gray-400 hover:text-white transition">
              <lucide-icon name="x-circle" class="w-5 h-5"></lucide-icon>
            </button>
          </div>

          <div class="space-y-3 text-xs">
            <div>
              <label class="text-gray-400 mb-1 block">Item / Descripción de la Cirugía *</label>
              <input type="text" [(ngModel)]="nuevaCirugiaForm.descripcionCirugia" placeholder="Ej: Hernia Inguinal, Colecistectomía..." class="w-full bg-gray-950 border border-gray-700 rounded-xl px-3 py-2 text-white focus:outline-none focus:border-sky-500">
            </div>

            <div class="grid grid-cols-2 gap-3">
              <div>
                <label class="text-gray-400 mb-1 block">Especialidad Quirúrgica</label>
                <input type="text" [(ngModel)]="nuevaCirugiaForm.especialidad" placeholder="Ej: Ginecología, Traumatología..." class="w-full bg-gray-950 border border-gray-700 rounded-xl px-3 py-2 text-white focus:outline-none focus:border-sky-500">
              </div>
              <div>
                <label class="text-gray-400 mb-1 block">Precio Base USD ($) *</label>
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

      <!-- Modal Cancelación -->
      <div *ngIf="modalCancelacionOrden()" class="fixed inset-0 bg-black/70 backdrop-blur-sm z-50 flex items-center justify-center p-4">
        <div class="bg-gray-900 border border-gray-800 rounded-2xl shadow-2xl w-full max-w-md p-6 space-y-4 text-white">
          <div class="flex justify-between items-center border-b border-gray-800 pb-3">
            <h3 class="text-base font-bold text-red-400 flex items-center gap-2">
              <lucide-icon name="alert-triangle" class="w-5 h-5"></lucide-icon>
              Cancelar Cirugía
            </h3>
            <button (click)="modalCancelacionOrden.set(null)" class="text-gray-400 hover:text-white transition">
              <lucide-icon name="x-circle" class="w-5 h-5"></lucide-icon>
            </button>
          </div>

          <p class="text-xs text-gray-300">
            Motivo de auditoría para cancelar la cirugía <strong class="text-white">'{{ modalCancelacionOrden()?.descripcionCirugia }}'</strong>:
          </p>

          <div>
            <textarea [ngModel]="motivoCancelacionTexto()" (ngModelChange)="motivoCancelacionTexto.set($event)" rows="3" placeholder="Razón de cancelación..." class="w-full bg-gray-950 border border-gray-700 rounded-xl px-3 py-2 text-xs text-white focus:outline-none focus:border-red-500"></textarea>
          </div>

          <div class="flex justify-end gap-2 pt-2 border-t border-gray-800">
            <button (click)="modalCancelacionOrden.set(null)" class="bg-gray-800 hover:bg-gray-700 text-gray-300 text-xs px-4 py-2 rounded-xl">Volver</button>
            <button (click)="confirmarCancelacion()" [disabled]="!motivoCancelacionTexto().trim()" class="bg-red-600 hover:bg-red-500 text-white text-xs font-semibold px-4 py-2 rounded-xl disabled:opacity-50 transition">
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

  public vistaModo = signal<'pizarra' | 'calendario' | 'lista'>('pizarra');
  public ordenes = signal<OrdenCirugia[]>([]);
  public medicos = signal<{ id: string; nombre: string; especialidad?: string; activo?: boolean }[]>([]);
  public filtroTexto = signal<string>('');
  public filtroEstado = signal<string>('');
  public filtroMedicoId = signal<string>('');
  public ordenSeleccionada = signal<OrdenCirugia | null>(null);

  public modalConsumoOrdenId = signal<string | null>(null);
  public modalNuevaCirugiaVisible = signal<boolean>(false);
  public modalCancelacionOrden = signal<OrdenCirugia | null>(null);
  public motivoCancelacionTexto = signal<string>('');

  public franjasHorarias: string[] = [
    '07:00', '08:00', '09:00', '10:00', '11:00', '12:00',
    '13:00', '14:00', '15:00', '16:00', '17:00', '18:00'
  ];

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

    if (!txt) return list;

    return list.filter(o => 
      o.pacienteNombre?.toLowerCase().includes(txt) ||
      o.pacienteCedula?.toLowerCase().includes(txt) ||
      o.descripcionCirugia?.toLowerCase().includes(txt) ||
      o.medicoNombre?.toLowerCase().includes(txt) ||
      o.especialidad?.toLowerCase().includes(txt) ||
      o.razonCirugia?.toLowerCase().includes(txt)
    );
  });

  public diasSemana = computed(() => {
    const list = this.ordenesFiltradas();
    let refDate = new Date();
    if (list.length > 0 && list[0].fechaHoraProgramada) {
      const d = new Date(list[0].fechaHoraProgramada);
      if (!isNaN(d.getTime())) refDate = d;
    }

    const curr = new Date(refDate);
    const firstDay = curr.getDate() - curr.getDay() + 1; // Lunes

    const nombresDias = ['Lunes', 'Martes', 'Miércoles', 'Jueves', 'Viernes', 'Sábado', 'Domingo'];
    const result: { nombre: string; fechaStr: string; fechaISO: string }[] = [];

    for (let i = 0; i < 7; i++) {
      const day = new Date(curr.setDate(firstDay + i));
      const dayNum = day.getDate();
      const monthNum = day.getMonth() + 1;
      const iso = day.toISOString().slice(0, 10);
      result.push({
        nombre: `${nombresDias[i]} ${dayNum}`,
        fechaStr: `${dayNum}/${monthNum < 10 ? '0' + monthNum : monthNum}`,
        fechaISO: iso
      });
    }

    return result;
  });

  public rangoFechasSemanaTexto = computed(() => {
    const dias = this.diasSemana();
    if (dias.length === 0) return 'Semana Actual';
    return `Semana: ${dias[0].nombre} al ${dias[6].nombre}`;
  });

  getCirugiasParaSlot(fechaISO: string, hora: string): OrdenCirugia[] {
    const horaNum = parseInt(hora.split(':')[0], 10);
    return this.ordenesFiltradas().filter(o => {
      if (!o.fechaHoraProgramada) return false;
      const f = new Date(o.fechaHoraProgramada);
      if (isNaN(f.getTime())) return false;
      const iso = f.toISOString().slice(0, 10);
      const h = f.getHours();
      return iso === fechaISO && h === horaNum;
    });
  }

  seleccionarCirugia(orden: OrdenCirugia): void {
    this.ordenSeleccionada.set(orden);
  }

  obtenerPrimerNombreDoctor(nombreCompleto: string): string {
    if (!nombreCompleto) return 'Dr. Indefinido';
    const partes = nombreCompleto.replace(/^Dr\.\s*/i, '').trim().split(' ');
    return partes[0] || nombreCompleto;
  }

  obtenerBadgeEstado(estado?: string): string {
    if (!estado) return 'Programado';
    if (estado === 'PendienteEjecucion') return 'Programado';
    return estado;
  }

  getCardBorderClass(orden: OrdenCirugia): string {
    const isSelected = this.ordenSeleccionada()?.id === orden.id;
    let base = isSelected ? 'ring-2 ring-sky-400 ' : '';
    if (orden.estado === 'Programado' || orden.estado === 'PendienteEjecucion') {
      return base + 'border-amber-500/50 bg-amber-950/20';
    }
    if (orden.estado === 'EnProceso') {
      return base + 'border-sky-500/60 bg-sky-950/30';
    }
    if (orden.estado === 'Completada') {
      return base + 'border-emerald-500/50 bg-emerald-950/20';
    }
    if (orden.estado === 'Cancelada') {
      return base + 'border-red-500/50 bg-red-950/20';
    }
    return base + 'border-gray-800 bg-gray-900/50';
  }

  getBadgeClass(estado?: string): string {
    if (estado === 'Programado' || estado === 'PendienteEjecucion') {
      return 'bg-amber-500/20 text-amber-300 border-amber-500/30';
    }
    if (estado === 'EnProceso') {
      return 'bg-sky-500/20 text-sky-300 border-sky-500/30';
    }
    if (estado === 'Completada') {
      return 'bg-emerald-500/20 text-emerald-300 border-emerald-500/30';
    }
    if (estado === 'Cancelada') {
      return 'bg-red-500/20 text-red-300 border-red-500/30';
    }
    return 'bg-gray-800 text-gray-300';
  }

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
      fechaHoraProgramada: new Date().toISOString().slice(0, 16),
      especialidad: '',
      razonCirugia: ''
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
      next: () => {
        this.cargarOrdenes();
        if (this.ordenSeleccionada()?.id === ordenId) {
          const current = this.ordenSeleccionada();
          if (current) {
            this.ordenSeleccionada.set({ ...current, estado: nuevoEstado });
          }
        }
      }
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
