import { ServiceCategory } from '../../../core/models/service-category.enum';
import { AtmAmountDirective } from '../../../shared/directives/atm-amount.directive';
import { CurrencyBsPipe } from '../../../shared/pipes/currency-bs.pipe';
import { Component, inject, signal, computed, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject, first, from, of, switchMap, firstValueFrom } from 'rxjs';
import { concatMap, tap, catchError, finalize } from 'rxjs/operators';
import { FacturacionService, DetallePagoDto, CargarServicioACuentaRequest, ReceiptPrintData } from '../../../core/services/facturacion.service';
import { AuthService } from '../../../core/services/auth.service';
import { AppointmentsService, Doctor, ScheduleEntry } from '../../../core/services/appointments.service';
import { SpecialtyService, Especialidad as Specialty } from '../../../core/services/specialty.service';
import { CatalogService } from '../../../core/services/catalog.service';
import { CatalogItem } from '../../../core/models/priced-item.model';
import { BillingFacadeService } from '../../../core/services/billing-facade.service';
import { PatientSelectorComponent } from './components/patient-selector/patient-selector.component';
import { ServiceCatalogComponent } from './components/service-catalog/service-catalog.component';
import { BillingCartComponent } from './components/billing-cart/billing-cart.component';
import { PaymentModuleComponent } from './components/payment-module/payment-module.component';
import { PatientService, PatientRecord } from '../../../core/services/patient.service';
import { CajaService, DailyClosingReport } from '../../../core/services/caja.service';
import { PrintService } from '../../../core/services/print.service';
import { ConveniosService } from '../../../core/services/convenios.service';
import { SettingsService } from '../../../core/services/settings.service';
import { toObservable, toSignal, takeUntilDestroyed } from '@angular/core/rxjs-interop';
import {
  LucideAngularModule,
  CreditCard,
  RefreshCcw,
  Check,
  Plus,
  User,
  Calendar,
  Search,
  Package,
  Clock,
  SearchX,
  Info,
  ChevronRight,
  Trash2,
  X,
  Lock,
  UserPlus,
  Phone,
  Mail,
  Layout,
  ShieldAlert,
  CalendarCheck,
  Edit3
} from 'lucide-angular';

@Component({
  selector: 'app-facturacion',
  standalone: true,
  imports: [
    CommonModule, 
    FormsModule, 
    LucideAngularModule, 
    CurrencyBsPipe, 
    PatientSelectorComponent, 
    ServiceCatalogComponent, 
    BillingCartComponent, 
    PaymentModuleComponent
  ],
  templateUrl: './facturacion.component.html',
  styleUrl: './facturacion.component.css'
})
export class FacturacionComponent {
  readonly icons = {
    CreditCard,
    RefreshCcw,
    Check,
    Plus,
    User,
    Calendar,
    Search,
    Package,
    Clock,
    SearchX,
    Info,
    ChevronRight,
    Trash2,
    X,
    Lock,
    UserPlus,
    Phone,
    Mail,
    Layout,
    ShieldAlert,
    CalendarCheck,
    Edit3
  };
  private facturacionService = inject(FacturacionService);
  private authService = inject(AuthService);
  private appointmentsService = inject(AppointmentsService);
  private specialtyService = inject(SpecialtyService);
  private catalogService = inject(CatalogService);
  private patientService = inject(PatientService);
  private cajaService = inject(CajaService);
  private printService = inject(PrintService);
  private conveniosService = inject(ConveniosService);
  private settingsService = inject(SettingsService);
  public billingFacade = inject(BillingFacadeService);

  // --- Estados de Facturación y Usuario (Senior Design Patterns) ---
  public user = this.authService.currentUser;
  public isAdmin = computed(() => this.authService.isAdministrador());
  public isParticularAssistant = computed(() => this.authService.isParticularAssistant());
  public isInsuranceAssistant = computed(() => this.authService.isInsuranceAssistant());
  
  public isRxAssistant = computed(() => {
    const role = this.user()?.role?.toLowerCase() || '';
    return (role === 'rx' || role === 'farmacia' || role === 'asistente rx') && !this.isAdmin();
  });

  public isHospitalAssistant = computed(() => {
    const role = this.user()?.role?.toLowerCase() || '';
    return role.includes('hospitalario') || this.isAdmin();
  });

  public isEmergencyAssistant = computed(() => {
    const role = this.user()?.role?.toLowerCase() || '';
    return role.includes('emergencia') || this.isAdmin();
  });

  // --- Estados del Wizard (SSoT) ---
  public currentStep = signal<number>(1);
  public maxSteps = 3;

  // --- Estados de Cuenta y Carrito (Delegados al Facade V11.1 Guid) ---
  public pacienteId = signal<string | null>(null);
  public selectedPatientData = signal<PatientRecord | null>(null);
  public pacienteSeleccionado = computed(() => !!this.pacienteId());
  
  public cuentaId = this.billingFacade.cuentaId;
  public carritoLocal = this.billingFacade.carritoLocal;
  public serviciosEnBackend = this.billingFacade.serviciosEnBackend;
  public pagos = this.billingFacade.pagos;
  public serviciosCargados = this.billingFacade.serviciosCargados;
  public totalCargadoUSD = this.billingFacade.totalCargadoUSD;
  public totalCargadoBS = this.billingFacade.totalCargadoBS;

  public tipoIngreso = signal<string>('Particular');
  public convenioId = signal<number | null>(null); 
  public tasaCambioDia = this.billingFacade.tasaCambioDia;
  public editandoTasa = signal<boolean>(false);
  public tasaEditValue = signal<number>(0);

  // --- Catálogos y Búsqueda (Delegados al Facade V7.0) ---
  public especialidades = this.billingFacade.especialidades;
  public medicosFiltrados = this.billingFacade.medicosFiltrados;
  public serviciosFiltradosPorRol = this.billingFacade.serviciosFiltrados;
  public searchTermServicio = this.billingFacade.searchTermServicio;
  public selectedEspecialidad = this.billingFacade.selectedEspecialidad;
  public selectedMedicoId = this.billingFacade.selectedMedicoId;
  public suggestedServiceId = signal<string | null>(null);
  public convenios = signal<any[]>([]);

  constructor() {
    // 1. Sincronización Real-time de Tasa via SignalR
    this.settingsService.tasa$.pipe(takeUntilDestroyed()).subscribe(monto => {
      this.tasaCambioDia.set(monto);
    });

    // 2. Cargar Tasa de Cambio Inicial desde Backend
    this.settingsService.getTasa().pipe(takeUntilDestroyed()).subscribe({
      next: (res) => this.tasaCambioDia.set(res.monto),
      error: () => this.tasaCambioDia.set(36.5) // Fallback
    });

    // 4. Inicialización de Estados Seguros según Rol
    if (this.isInsuranceAssistant()) {
      this.tipoIngreso.set('Seguro');
    } else if (this.isParticularAssistant()) {
      this.tipoIngreso.set('Particular');
    }

    // 5. Cargar Especialidades Dinámicas
    this.specialtyService.getAll().pipe(takeUntilDestroyed()).subscribe(res => {
      this.billingFacade.especialidades.set(res.filter(e => e.activo).map(e => e.nombre));
    });

    // 6. Cargar Convenios Dinámicos (Pachón Pro)
    this.conveniosService.getAll().pipe(takeUntilDestroyed()).subscribe((res: any[]) => {
      this.convenios.set(res);
    });

    // 7. Cargar Catálogo Inicial
    this.refreshCatalog();

    // 8. Reaccionar a cambios en convenio para actualizar precios (Fase 39)
    effect(() => {
      const convId = this.convenioId();
      this.refreshCatalog(convId);
    });
  }

  // --- Handlers de Interfaz (Angular Pro) ---
  onComentarioInput(event: Event, slot: ScheduleEntry) {
    const input = event.target as HTMLInputElement;
    if (input) {
      slot.comentario = input.value;
    }
  }

  // Motor de Búsqueda Dinámica: Stemming de 4 caracteres (Pachón Pro V5.0)
  private getSearchKey(text: string | null | undefined): string {
    if (!text) return '';
    return text.toUpperCase().substring(0, 4);
  }




  // Efectos de Negocio
  // (Sin duplicados de señales)

  // Efecto para Auto-sugerir Servicio según Especialidad
  private _specialtyEffect = effect(() => {
    const esp = this.selectedEspecialidad();
    if (!esp) {
      this.suggestedServiceId.set(null);
      return;
    }

    const searchTerm = this.getSearchKey(esp);
    const service = this.billingFacade.servicesCatalog().find((s: CatalogItem) =>
      s.isConsultation &&
      s.descripcion.toUpperCase().includes(searchTerm)
    );

    if (service) {
      this.suggestedServiceId.set(service.id);
      // Opcional: Si el usuario quiere "todo automático", podríamos hacer auto-scroll o pre-selección
    } else {
      // Fallback a General si no hay específico
      const general = this.billingFacade.servicesCatalog().find((s: CatalogItem) => s.id === 'S001' || s.descripcion.includes('GENERAL'));
      this.suggestedServiceId.set(general?.id || null);
    }
  });

  // Estado para Flujo Automatizado de Consultas (Step -> Doctor -> Agenda)
  public isPendingConsultation = signal<boolean>(false);

  // Efecto para Auto-abrir Agenda al seleccionar Médico (Restauración de Comportamiento Classic)
  private _autoAgendaEffect = effect(() => {
    const medId = this.selectedMedicoId();

    if (medId) {
      // Pequeño delay para asegurar que el componente esté listo y el usuario vea el cambio
      setTimeout(() => {
        this.abrirModalHorarios();
        this.isPendingConsultation.set(false); // Limpiar flag por si se usó desde servicios
      }, 400);
    }
  });

  // Efecto para Resetear Convenio si no es Seguro (Consistency V2.5)
  private _convenioResetEffect = effect(() => {
    if (this.tipoIngreso() !== 'Seguro') {
      this.convenioId.set(null);
    }
  });

  public showScheduleModal = signal<boolean>(false);
  public scheduleLoading = signal<boolean>(false);
  public availableSlots = signal<ScheduleEntry[]>([]);
  public selectedSlot = signal<string | null>(null);
  public comentarioCita = signal<string | null>(null);
  public fechaCita = signal<string>(new Date().toISOString().split('T')[0]);
  public horaCita = signal<string>('08:00');

  public nombreMedicoSeleccionado = computed(() => {
    const id = this.selectedMedicoId();
    if (!id) return '';
    return this.medicosFiltrados().find(m => m.id === id)?.nombre || '';
  });

  public getHoraRango(hora: string): string {
    if (!hora) return '--:--';

    // Si viene como ISO (2026-03-23T08:00:00), extraer la parte de la hora
    let timePart = hora;
    if (hora.includes('T')) {
      timePart = hora.split('T')[1].substring(0, 5); // "08:00"
    } else if (hora.length > 5 && hora.includes(':')) {
      // Por si viene como "08:00:00"
      timePart = hora.substring(0, 5);
    }

    if (!timePart.includes(':')) return timePart;

    const [h, m] = timePart.split(':');
    const startNum = parseInt(h);
    const endNum = (startNum + 1) % 24;
    const endStr = endNum.toString().padStart(2, '0') + ':' + m;

    return `${timePart} - ${endStr}`;
  }

  // Helpers de Precio para Template (Fix micro-ciclo 34)
  public getFormattedBs(item: any): string {
    const tasa = this.tasaCambioDia();
    const usd = item.precioUsd ?? item.PrecioUsd ?? 0;
    const priceBs = usd > 0 ? (usd * tasa) : (item.precioBs ?? item.PrecioBs ?? item.precio ?? 0);
    return priceBs.toLocaleString('es-VE', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
  }

  public getDisplayPriceUsd(item: any): string {
    const usd = item.precioUsd ?? item.PrecioUsd ?? 0;
    return usd.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
  }

  // Feedback UI (Sistema de Toasts Premium V2.5)
  public isLoading = signal<boolean>(false);
  public actionMessage = signal<string | null>(null);
  public errorMessage = signal<string | null>(null);

  private _toastEffect = effect(() => {
    const action = this.actionMessage();
    const error = this.errorMessage();

    if (action || error) {
      setTimeout(() => {
        this.actionMessage.set(null);
        this.errorMessage.set(null);
      }, 5000); // 5 segundos de permanencia
    }
  });

  // States for Patient Search (Micro-Ciclo 9)
  public busquedaTermino = signal<string>('');
  public resultadosPacientes = signal<PatientRecord[]>([]);
  public buscandoPaciente = signal<boolean>(false);
  public showResultadosBusqueda = signal<boolean>(false);

  // States for New Patient Modal (Micro-Ciclo 29)
  public showRegisterModal = signal<boolean>(false);
  public noResultsFound = signal<boolean>(false);

  public newPatientData: Partial<PatientRecord> = {
    cedula: '',
    nombre: '',
    apellidos: '',
    sexo: 'M',
    fechaNacimiento: new Date().toISOString().split('T')[0],
    correo: '',
    tipoCorreo: '@gmail.com',
    celular: '',
    codigoCelular: '0414',
    telefono: '',
    codigoTelefono: '0212'
  };

  // Listas para Registro de Pacientes (Pachón Pro V2.9)
  public tiposCorreo = ['@gmail.com', '@hotmail.com', '@outlook.com', '@yahoo.com'];
  public codigosCelular = ['0416', '0426', '0414', '0424', '0412', '0422'];
  public codigosTelefono = ['0273', '0251', '0212', '0281', '0241'];



  // Navegación del Wizard
  nextStep() {
    if (this.currentStep() < this.maxSteps) {
      // Paso 1: Convenio (Nuevo Orden)
      if (this.currentStep() === 1 && this.tipoIngreso() === 'Seguro' && !this.convenioId()) {
        this.errorMessage.set("Debe seleccionar un convenio para continuar.");
        return;
      }
      // Paso 2: Estudios (Nuevo Orden)
      if (this.currentStep() === 2 && this.serviciosCargados().length === 0) {
        this.errorMessage.set("Debe añadir al menos un servicio para continuar.");
        return;
      }
      this.errorMessage.set(null);
      this.currentStep.update(s => s + 1);
    }
  }

  prevStep() {
    if (this.currentStep() > 1) {
      this.currentStep.update(s => s - 1);
    }
  }

  goToStep(step: number) {
    if (step < this.currentStep()) {
      this.currentStep.set(step);
    }
  }

  buscarPaciente() {
    const term = this.busquedaTermino().trim();
    if (term.length < 3) return;

    this.buscandoPaciente.set(true);
    this.showResultadosBusqueda.set(true);
    this.noResultsFound.set(false);

    this.patientService.searchPatients(term).subscribe({
      next: (res: PatientRecord[]) => {
        this.resultadosPacientes.set(res);
        this.buscandoPaciente.set(false);
        this.noResultsFound.set(res.length === 0);
      },
      error: () => {
        this.errorMessage.set("Error al buscar pacientes.");
        this.buscandoPaciente.set(false);
        this.noResultsFound.set(true);
      }
    });
  }

  seleccionarPaciente(p: PatientRecord) {
    if (p.id) {
      this.pacienteId.set(p.id);
      this.selectedPatientData.set(p);
      this.noResultsFound.set(false);
    }
    this.showResultadosBusqueda.set(false);
    this.busquedaTermino.set(p.cedula);
    this.actionMessage.set(`Paciente seleccionado: ${p.nombre} ${p.apellidos}`);
  }

  /**
   * Sincroniza los items del carrito local con el backend (Facade Delegation V10.2)
   */
  private async sincronizarCarrito(): Promise<boolean> {
    const pId = this.pacienteId();
    if (!pId) return false;

    this.isLoading.set(true);
    try {
      await firstValueFrom(this.billingFacade.syncCartWithBackend(
        pId, 
        this.tipoIngreso(), 
        this.user()?.username || '', 
        this.convenioId(),
        this.selectedPatientData()?.idPacienteLegacy
      ));
      this.isLoading.set(false);
      return true;
    } catch (err: any) {
      this.errorMessage.set(err?.error?.error || "Error al sincronizar carrito con el servidor.");
      this.isLoading.set(false);
      return false;
    }
  }

  cambiarPaciente() {
    if (this.cuentaId()) {
      this.errorMessage.set('No puede cambiar el paciente después de iniciar la cuenta.');
      return;
    }
    this.pacienteId.set(null);
    this.selectedPatientData.set(null);
    this.busquedaTermino.set('');
    this.showResultadosBusqueda.set(false);
    this.resultadosPacientes.set([]);
    this.noResultsFound.set(false);
  }

  editarTasa() {
    this.tasaEditValue.set(this.tasaCambioDia());
    this.editandoTasa.set(true);
  }

  guardarTasa() {
    const newTasa = this.tasaEditValue();
    if (newTasa <= 0) {
      this.errorMessage.set('La tasa debe ser mayor a 0.');
      return;
    }
    this.settingsService.updateTasa(newTasa).subscribe({
      next: () => {
        this.tasaCambioDia.set(newTasa);
        this.editandoTasa.set(false);
        this.actionMessage.set(`Tasa actualizada a Bs. ${newTasa}`);
      },
      error: () => this.errorMessage.set('Error al actualizar la tasa.')
    });
  }


  abrirRegistroPaciente() {
    const term = this.busquedaTermino();
    // Resetear data y asignar cedula buscada
    this.newPatientData = {
      cedula: term,
      nombre: '',
      apellidos: '',
      sexo: 'M',
      fechaNacimiento: new Date().toISOString().split('T')[0],
      correo: '',
      tipoCorreo: '@gmail.com',
      celular: '',
      codigoCelular: '0414',
      telefono: '',
      codigoTelefono: '0212'
    };
    this.showRegisterModal.set(true);
    this.showResultadosBusqueda.set(false);
    this.noResultsFound.set(false);
  }

  guardarNuevoPaciente() {
    if (!this.newPatientData.cedula || !this.newPatientData.nombre) {
      this.errorMessage.set("Cédula y Nombre son obligatorios.");
      return;
    }

    this.isLoading.set(true);
    this.patientService.createPatient(this.newPatientData).subscribe({
      next: (p: PatientRecord) => {
        this.isLoading.set(false);
        this.showRegisterModal.set(false);
        this.seleccionarPaciente(p);
        this.actionMessage.set("Paciente registrado exitosamente en ambos sistemas.");
      },
      error: (err: any) => {
        this.isLoading.set(false);
        this.errorMessage.set(err.error?.message || "Error al registrar paciente.");
      }
    });
  }

  public refreshCatalog(convenioId?: number | null) {
    this.catalogService.getUnifiedCatalog(convenioId).subscribe({
      next: (items: CatalogItem[]) => {
        this.billingFacade.servicesCatalog.set(items);
        
        // Sincronización de precios si hay items en carrito (Legacy Fix V2.1)
        if (this.carritoLocal().length > 0) {
          this.billingFacade.carritoLocal.update(cart => cart.map(localItem => {
            const updated = items.find(i => i.id === localItem.id);
            return updated ? { ...localItem, precio: updated.precio } : localItem;
          }));
        }
      },
      error: () => {
        this.errorMessage.set("No se pudo cargar el catálogo de servicios.");
      }
    });
  }

  resetCitaSelection() {
    this.selectedMedicoId.set(null);
    this.selectedSlot.set(null);
    this.horaCita.set('08:00');
    this.comentarioCita.set(null);
    this.selectedEspecialidad.set(null);
  }

  public showAdminAppointmentsModal = signal(false); // Fase 10 Panel
  public activeAppointments = signal<any[]>([]); // Fase 10 Info

  abrirModalHorarios() {
    if (!this.selectedMedicoId()) return;

    this.scheduleLoading.set(true);
    this.showScheduleModal.set(true);
    const dateToSearch = this.fechaCita();

    // Ahora pasamos el pacienteId para detectar "Tu Cita" (V4.9)
    this.appointmentsService.getDoctorScheduleWithPatient(
      this.selectedMedicoId()!,
      dateToSearch,
      this.pacienteId() || undefined
    ).subscribe({
      next: (res: any) => {
        this.availableSlots.set(res.turnos);
        this.scheduleLoading.set(false);
      },
      error: () => {
        this.errorMessage.set("No se pudo cargar la agenda del médico.");
        this.scheduleLoading.set(false);
      }
    });
  }

  seleccionarTurno(slot: ScheduleEntry) {
    if (slot.ocupado || slot.bloqueado) {
      this.errorMessage.set("Este horario no está disponible.");
      return;
    }

    if (slot.reservado) {
      this.errorMessage.set("Este horario está siendo facturado por otro usuario.");
      return;
    }

    this.isLoading.set(true);
    // Normalización robusta para evitar desfases de zona horaria o milisegundos
    const d = new Date(slot.hora);
    const horaNormalizada = `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}T${String(d.getHours()).padStart(2, '0')}:${String(d.getMinutes()).padStart(2, '0')}:00`;

    const payload = {
      medicoId: this.selectedMedicoId()!,
      horaPautada: horaNormalizada,
      comentario: (slot.comentario === 'Libre' || slot.comentario === 'Disponible') ? '' : slot.comentario
    };

    console.log("DEBUG: Intentando reservar turno:", payload);

    this.facturacionService.reservarTurno(payload)
      .subscribe({
        next: () => {
          this.selectedSlot.set(this.getHoraRango(slot.hora));
          // Guardar la fecha y hora completa de la reserva
          this.horaCita.set(slot.hora);
          this.comentarioCita.set(slot.comentario === 'Disponible' ? '' : slot.comentario);
          this.showScheduleModal.set(false);
          this.isLoading.set(false);
          this.errorMessage.set(null); // Limpiar error si logró agendar
          this.actionMessage.set("Horario reservado temporalmente para esta facturación.");
        },
        error: (err) => {
          this.errorMessage.set(err.error?.error || "No se pudo reservar el turno. Intente con otro.");
          this.isLoading.set(false);
          // Refrescar agenda por si cambió
          this.abrirModalHorarios();
        }
      });
  }

  bloquearHorario(slot: ScheduleEntry) {
    const motivo = prompt("Motivo del bloqueo (Opcional):", slot.comentario === 'Libre' ? '' : slot.comentario);
    if (motivo === null) return;

    this.isLoading.set(true);
    this.facturacionService.bloquearHorario({
      medicoId: this.selectedMedicoId()!,
      horaPautada: slot.hora,
      motivo: motivo
    }).subscribe({
      next: () => {
        this.isLoading.set(false);
        this.actionMessage.set("Horario bloqueado exitosamente.");
        this.abrirModalHorarios(); // Refrescar
      },
      error: (err) => {
        this.errorMessage.set(err.error?.error || "Error al bloquear horario.");
        this.isLoading.set(false);
      }
    });
  }

  // Método para Filtrado Inverso (Servicio -> Especialidad)
  seleccionarTipoConsulta(s: CatalogItem) {
    if (!s.isConsultation) return;

    const serviceKey = this.getSearchKey(s.descripcion);
    const tipoKey = this.getSearchKey(s.tipo);

    // Detección Dinámica de Especialidad (Pachón Pro V5.0)
    const match = this.especialidades().find(esp => {
      const espKey = this.getSearchKey(esp);
      return s.descripcion.toUpperCase().includes(espKey) || s.tipo.toUpperCase().includes(espKey);
    });

    if (match) {
      this.selectedEspecialidad.set(match);
      this.selectedMedicoId.set(null); // Reset para forzar selección
      this.isPendingConsultation.set(true); // Marcar como pendiente para auto-agenda (UX V3.5)
      this.actionMessage.set(`Especialidad ${match} detectada. Seleccione su médico.`);

      // Feedback visual y automatización de selección (V2.1)
      setTimeout(() => {
        const el = document.getElementById('seccion-medica');
        el?.scrollIntoView({ behavior: 'smooth', block: 'center' });

        // Si solo hay un médico, seleccionarlo automáticamente
        const medicos = this.medicosFiltrados();
        if (medicos.length === 1) {
          this.selectedMedicoId.set(medicos[0].id);
          this.actionMessage.set(`Especialidad ${match} detectada. Médico sugerido: ${medicos[0].nombre}`);
        }
      }, 300); // Un poco más de tiempo para que medicosFiltrados reaccione al cambio de especialidad
    }
  }

  cargarServicio(servId: string) {
    const s = this.billingFacade.servicesCatalog().find((x: CatalogItem) => x.id === servId);
    if (!s) return;

    // Abstracción Senior: Identificación por Categoría de Dominio (V5.2)
    const esConsulta = s.isConsultation;

    // Validación estricta V3.0 (Micro-Ciclo 38): Consulta requiere Médico Y Turno Agendado
    if (esConsulta) {
      if (!this.selectedMedicoId()) {
        this.seleccionarTipoConsulta(s);
        this.errorMessage.set(`Indique el médico para procesar la ${s.descripcion}.`);
        return;
      }

      if (!this.selectedSlot() || !this.horaCita()) {
        this.isPendingConsultation.set(true); // Activar por si acaso el usuario vuelve a darle
        this.abrirModalHorarios();
        this.errorMessage.set(`Por favor, seleccione un horario disponible para el Dr. ${this.nombreMedicoSeleccionado()}.`);
        return;
      }
    }

    let finalDescripcion = s.descripcion;
    // Ya no necesitamos el sufijo manual si el ítem ya es específico, 
    // pero lo mantenemos por compatibilidad con el ítem S001 general
    if (s.id === 'S001' && this.selectedEspecialidad()) {
      const esp = this.selectedEspecialidad()!;
      finalDescripcion = `${finalDescripcion} (${esp})`;
    }

    const pId = this.pacienteId();

    // Si no hay paciente, agregar al carrito local (No bloqueante)
    if (pId === null) {
      const yaEnCarrito = this.carritoLocal().some(x => x.id === s.id);
      if (!yaEnCarrito) {
        this.carritoLocal.update(prev => [...prev, {
          ...s,
          descripcion: finalDescripcion,
          medicoId: esConsulta ? this.selectedMedicoId() : undefined,
          medicoNombre: esConsulta ? this.nombreMedicoSeleccionado() : undefined,
          horaCita: esConsulta ? this.horaCita() : undefined,
          comentario: this.comentarioCita()
        }]);
        this.resetCitaSelection();
        this.actionMessage.set(`Servicio "${finalDescripcion}" añadido al carrito temporal.`);
      } else {
        this.errorMessage.set("Este servicio ya está en el carrito.");
      }
      return;
    }

    // Si hay paciente, cargar directamente al backend
    this.isLoading.set(true);

    // Lógica de Horario Profesional (V2.0 Core Fix)
    // El backend espera 'horaCita' como el ISO que viene del servidor (slot.hora)
    let fullHoraCita: string | undefined = undefined;
    if (esConsulta && this.horaCita()) {
      fullHoraCita = this.horaCita(); // Ya es un ISO string válido desde la reserva
    }

    const payload: CargarServicioACuentaRequest = {
      pacienteId: pId,
      tipoIngreso: this.tipoIngreso(),
      convenioId: this.convenioId() || undefined,
      servicioId: s.id,
      descripcion: finalDescripcion,
      precio: s.precioUsd || s.PrecioUsd || 0,
      cantidad: 1,
      tipoServicio: s.tipo,
      usuarioCarga: this.user()?.username || '',
      medicoId: esConsulta ? this.selectedMedicoId() || undefined : undefined,
      horaCita: fullHoraCita,
      comentario: this.comentarioCita() || undefined
    };

    this.facturacionService.cargarServicio(payload).subscribe({
      next: (res: any) => {
        this.cuentaId.set(res.cuentaId);
        this.serviciosEnBackend.update((prev: any[]) => [...prev, {
          ...s,
          detalleId: res.detalleId, // Guárdalo para eliminación precisa (V4.8)
          medicoId: esConsulta ? this.selectedMedicoId() : undefined, // Guardar ID para limpieza de cita
          hora: this.horaCita(),
          medicoNombre: esConsulta ? this.nombreMedicoSeleccionado() : undefined,
          precio: payload.precio,
          precioBs: s.precioBs,
          precioUsd: s.precioUsd
        }]);
        this.resetCitaSelection();
        this.actionMessage.set("Servicio cargado exitosamente.");
        this.errorMessage.set(null);
        this.isLoading.set(false);
      },
      error: (err: any) => {
        this.errorMessage.set(err.error?.error || "Error al cargar servicio.");
        this.isLoading.set(false);
      }
    });
  }




  async procesarCobro() {
    if (!this.pacienteSeleccionado()) {
      this.errorMessage.set("Debe identificar y seleccionar un beneficiario antes de emitir.");
      return;
    }

    // Validación de Último Recurso (Fase 11): Verificar que todas las consultas tengan médico
    const inconsistente = this.serviciosCargados().find(s => s.isConsultation && (!s.medicoId && !s.MedicoId));
    if (inconsistente) {
      this.errorMessage.set(`La consulta '${inconsistente.descripcion}' no tiene un médico asignado. Elimínela y vuelva a cargarla seleccionando un especialista.`);
      this.isLoading.set(false);
      return;
    }

    this.isLoading.set(true);

    // Si hay items locales, sincronizar primero antes de cerrar (V10.2 Timing Fix)
    if (this.carritoLocal().length > 0) {
      const synced = await this.sincronizarCarrito();
      if (!synced) {
        this.isLoading.set(false);
        return; // Error ya manejado en sincronizarCarrito
      }
      // Delay Estratégico (V10.8 Force Robustness)
      await new Promise(resolve => setTimeout(resolve, 500));
    }

    if (!this.cuentaId()) {
      this.errorMessage.set("No hay una cuenta activa sincronizada.");
      this.isLoading.set(false);
      return;
    }

    this.facturacionService.closeAccount({
      cuentaId: this.cuentaId()!,
      usuarioCajero: this.user()?.username || '',
      usuarioId: this.user()?.id || '',
      tasaCambio: this.tasaCambioDia(),
      pagos: this.pagos()
    }).subscribe({
      next: (res: any) => {
        this.actionMessage.set(`¡Facturación Exitosa! Recibo: ${res.reciboId}`);

        // Confirmación de Impresión No Intrínseca (Requerimiento Pro)
        const deseaImprimir = confirm("¿Desea imprimir el comprobante de pago ahora?");
        if (deseaImprimir) {
          this.imprimirRecibo(res.reciboId);
        }

        this.resetForm();
        this.isLoading.set(false);
      },
      error: (err: any) => {
        this.errorMessage.set(err.error?.error || "Error al procesar cobro.");
        this.isLoading.set(false);
      }
    });
  }

  /**
   * Strategy Pattern: Cierra la cuenta omitiendo deliberadamente la impresión.
   * Ideal para casos donde el comprobante se entregó previamente o no es requerido.
   */
  async omitirComprobante() {
    if (!this.pacienteSeleccionado()) {
      this.errorMessage.set("⚠️ Debe seleccionar un paciente en el Paso 3 antes de cerrar la cuenta.");
      return;
    }

    this.isLoading.set(true);

    // Si hay items locales, sincronizar primero antes de cerrar (V10.2 Timing Fix)
    if (this.carritoLocal().length > 0) {
      const synced = await this.sincronizarCarrito();
      if (!synced) {
        this.isLoading.set(false);
        return; // Error ya manejado en sincronizarCarrito
      }
      // Delay Estratégico (V10.8 Force Robustness)
      await new Promise(resolve => setTimeout(resolve, 500));
    }

    if (!this.cuentaId()) {
      this.errorMessage.set("⚠️ No hay una cuenta activa en el servidor.");
      this.isLoading.set(false);
      return;
    }

    this.facturacionService.closeAccount({
      cuentaId: this.cuentaId()!,
      usuarioCajero: this.user()?.username || '',
      usuarioId: this.user()?.id || '',
      tasaCambio: this.tasaCambioDia(),
      pagos: this.pagos()
    }).subscribe({
      next: (res: any) => {
        this.actionMessage.set(`Cuenta cerrada sin comprobante. Recibo: ${res.reciboId}`);
        this.resetForm();
        this.isLoading.set(false);
      },
      error: (err: any) => {
        this.errorMessage.set(err.error?.error || "Error al cerrar cuenta.");
        this.isLoading.set(false);
      }
    });
  }

  imprimirRecibo(reciboId: string) {
    this.facturacionService.getReceiptPrintData(reciboId).subscribe({
      next: (data: ReceiptPrintData) => {
        const content = this.printService.generateReceiptHtml(data);
        this.printService.print(content, `Recibo ${data.numeroRecibo}`);
      },
      error: () => this.errorMessage.set("Error al generar vista de impresión.")
    });
  }

  quitarServicio(index: number) {
    const backendCount = this.serviciosEnBackend().length;
    const isBackend = index < backendCount;

    this.isLoading.set(true);
    this.billingFacade.removeService(index, isBackend).subscribe({
      next: () => {
        this.actionMessage.set("Servicio removido.");
        this.isLoading.set(false);
      },
      error: (err: any) => {
        this.errorMessage.set(err.error?.error || "Error al remover servicio.");
        this.isLoading.set(false);
      }
    });
  }

  // Métodos de Gestión Administrativa (Fase 10)
  verCitasActivas() {
    if (!this.isAdmin()) return;

    this.isLoading.set(true);
    this.showAdminAppointmentsModal.set(true);
    // Por ahora listamos las del día actual, o todas si no se especifica
    this.facturacionService.getAppointments(this.fechaCita()).subscribe({
      next: (res) => {
        this.activeAppointments.set(res);
        this.isLoading.set(false);
      },
      error: () => {
        this.errorMessage.set("Error al cargar citas activas.");
        this.isLoading.set(false);
      }
    });
  }

  anularCitaAdmin(citaId: string) {
    if (!confirm("¿Está seguro de anular esta cita? Esta acción liberará el horario de forma permanente.")) return;

    this.isLoading.set(true);
    this.facturacionService.cancelAppointment(citaId).subscribe({
      next: () => {
        this.actionMessage.set("Cita anulada exitosamente.");
        this.verCitasActivas(); // Refrescar lista
        this.abrirModalHorarios(); // Refrescar calendario si está abierto
      },
      error: () => {
        this.errorMessage.set("No se pudo anular la cita.");
        this.isLoading.set(false);
      }
    });
  }

  onAdminManage(slot: ScheduleEntry, action: 'Delete' | 'Update') {
    if (!this.isAdmin() || !slot.targetId || !slot.type) return;

    if (action === 'Delete' && !confirm("¿Está seguro de anular administrativamente este turno?")) return;

    this.isLoading.set(true);
    this.appointmentsService.adminManageSchedule({
      action,
      type: slot.type,
      targetId: slot.targetId
    }).subscribe({
      next: () => {
        this.isLoading.set(false);
        this.actionMessage.set(`Acción administrativa realizada con éxito.`);
        this.abrirModalHorarios(); // Refrescar agenda
      },
      error: (err: any) => {
        this.isLoading.set(false);
        this.errorMessage.set(err.error?.error || "Error al realizar acción administrativa.");
      }
    });
  }

  private resetForm() {
    this.pagos.set([]);
    this.serviciosEnBackend.set([]);
    this.carritoLocal.set([]);
    this.cuentaId.set(null);
    this.pacienteId.set(null);
    this.selectedPatientData.set(null);
    this.currentStep.set(1);
  }
}
