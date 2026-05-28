import { environment } from '../../../../environments/environment';
import { ServiceCategory } from '../../../core/models/service-category.enum';
import { AtmAmountDirective } from '../../../shared/directives/atm-amount.directive';
import { CurrencyBsPipe } from '../../../shared/pipes/currency-bs.pipe';
import { Component, inject, signal, computed, effect } from '@angular/core';
import { PermissionService } from '../../../core/services/permission.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject, first, from, of, switchMap, firstValueFrom } from 'rxjs';
import { concatMap, tap, catchError, finalize } from 'rxjs/operators';
import { FacturacionService, DetallePagoDto, CargarServicioACuentaRequest, ReceiptPrintData } from '../../../core/services/facturacion.service';
import { AuthService } from '../../../core/services/auth.service';
import { AppointmentsService, Doctor, ScheduleEntry } from '../../../core/services/appointments.service';
import { SpecialtyService, Especialidad as Specialty } from '../../../core/services/specialty.service';
import { ActivatedRoute } from '@angular/router';
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
import { SupervisorAuthDialogComponent } from '../../../shared/components/supervisor-auth-dialog/supervisor-auth-dialog.component';
import { FilterByDayPipe } from '../../../shared/pipes/filter-by-day.pipe';
import { ViewChild } from '@angular/core';
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
  Shield,
  CalendarCheck,
  Edit3,
  Sparkles
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
    PaymentModuleComponent,
    SupervisorAuthDialogComponent,
    FilterByDayPipe
  ],
  templateUrl: './facturacion.component.html',
  styleUrl: './facturacion.component.css'
})
export class FacturacionComponent {
  public systemVersion = environment.systemVersion;
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
    Shield,
    CalendarCheck,
    Edit3,
    Sparkles
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
  public permissionService = inject(PermissionService);
  private route = inject(ActivatedRoute);

  @ViewChild('supervisorDialog') supervisorDialog!: SupervisorAuthDialogComponent;
  @ViewChild('billingCart') billingCart!: BillingCartComponent;

  // --- Estados de Facturación y Usuario (Senior Design Patterns) ---
  public user = this.authService.currentUser;
  public isAdmin = this.authService.isAdmin;
  public isParticularAssistant = this.authService.isParticularAssistant;
  public isInsuranceAssistant = this.authService.isInsuranceAssistant;
  public isRxAssistant = this.authService.isRxAssistant;
  public isHospitalAssistant = this.authService.isHospitalAssistant;
  public isEmergencyAssistant = this.authService.isEmergencyAssistant;

  // --- Estados del Wizard (SSoT) ---
  public currentStep = signal<number>(1);
  public maxSteps = computed(() => this.tipoIngreso() === 'Particular' ? 2 : 3);

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
  public suggestedServices = signal<CatalogItem[]>([]);
  public convenios = signal<any[]>([]);

  // --- Sugerencias Dinámicas (Fase 12.5 Refactored) ---
  public showSuggestionModal = signal<boolean>(false);
  public activeSuggestions = signal<CatalogItem[]>([]);
  public selectedSuggestions = signal<Record<string, boolean>>({});

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

    // 4. Inicialización Reactiva de Estados según QueryParams (Fase 41 - Ultra Robust)
    this.route.queryParams.pipe(takeUntilDestroyed()).subscribe(params => {
      const typeParam = params['type'];
      const oldType = this.tipoIngreso();
      
      if (typeParam) {
        this.tipoIngreso.set(typeParam);
      } else if (this.isInsuranceAssistant()) {
        this.tipoIngreso.set('Seguro');
      } else if (this.isParticularAssistant()) {
        this.tipoIngreso.set('Particular');
      }

      // Si el tipo cambió o es la primera carga, reseteamos al paso 1 del flujo correspondiente
      if (typeParam !== oldType || !this.pacienteId()) {
          this.currentStep.set(1);
          this.billingFacade.resetCart();
          
          // Forzar recarga de catálogo con el nuevo contexto
          const currentConv = typeParam === 'Seguro' ? this.convenioId() : null;
          this.refreshCatalog(currentConv);
      }
    });

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

    // 7.1. Asegurar que los Métodos de Pago estén disponibles (Resilience Fix)
    this.billingFacade.reloadPaymentCatalogIfEmpty();

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
      return;
    }

    const searchTerm = this.getSearchKey(esp);
    const matches = this.billingFacade.servicesCatalog().filter((s: CatalogItem) =>
      s.isConsultation &&
      s.descripcion.toUpperCase().includes(searchTerm)
    );

    if (matches.length > 0) {
      this.suggestedServices.set(matches);
    } else {
      // Fallback a General si no hay específico
      const general = this.billingFacade.servicesCatalog().filter((s: CatalogItem) => 
        s.id === 'S001' || s.descripcion.toUpperCase().includes('GENERAL')
      );
      this.suggestedServices.set(general);
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
  public medicoWorkingHours = signal<any[]>([]);
  public selectedSlot = signal<string | null>(null);
  public comentarioCita = signal<string | null>(null);
  public fechaCita = signal<string>(new Date().toISOString().split('T')[0]);
  public horaCita = signal<string>('08:00');

  public nombreMedicoSeleccionado = computed(() => {
    const id = this.selectedMedicoId();
    if (!id) return '';
    const m = this.medicosFiltrados().find(x => (x.id || (x as any).Id) === id);
    return m ? (m.nombre || (m as any).Nombre || '') : '';
  });

  public telefonoMedicoSeleccionado = computed(() => {
    const id = this.selectedMedicoId();
    if (!id) return '';
    const m = this.medicosFiltrados().find(x => (x.id || (x as any).Id) === id);
    return m ? (m.telefono || (m as any).Telefono || '') : '';
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

    const [h, m] = timePart.split(':').map(Number);
    const endH = m >= 30 ? (h + 1) % 24 : h;
    const endM = m >= 30 ? '00' : '30';
    const endStr = String(endH).padStart(2, '0') + ':' + endM;

    return `${timePart} - ${endStr}`;
  }

  public getDayFromDate(dateStr: string): number {
    const d = new Date(dateStr + 'T00:00:00');
    return d.getDay(); // 0-6 (Dom-Sab)
  }

  public getDayName(day: number): string {
    const days = ['Domingo', 'Lunes', 'Martes', 'Miércoles', 'Jueves', 'Viernes', 'Sábado'];
    return days[day];
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

  // V12.1: Post-Billing Success States
  public billingSuccess = signal(false);
  public lastBillResult = signal<any>(null);
  public isInsuranceFlow = signal(false);
  public isGeneratingPdf = signal(false);

  // Metadatos para Documentos Detallados (V12.1)
  public docMetadata = signal({
    quienAutorizo: '',
    doctorProcedimiento: '',
    informacionAdicional: '',
    diasLiquidar: 1,
    cuotas: 1,
    montoGarantia: 0,
    descripcionGarantia: ''
  });

  public anexarGarantia = signal<boolean>(true);
  public garantiasItems = signal<{descripcion: string, valorEstimado: number}[]>([]);

  public totalMontoGarantias = computed(() => {
    return this.garantiasItems().reduce((acc, item) => acc + (item.valorEstimado || 0), 0);
  });

  public addGarantiaItem(): void {
    this.garantiasItems.update(items => [...items, { descripcion: '', valorEstimado: 0 }]);
  }

  public removeGarantiaItem(index: number): void {
    this.garantiasItems.update(items => items.filter((_, i) => i !== index));
  }

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

  public newPatientData: any = {
    cedula: '',
    nombre: '',
    apellidos: '',
    correo: '',
    celular: '',
    telefono: '',
    direccion: '', // V12.1 Premium Requirement
    fechaNacimiento: new Date().toISOString().split('T')[0],
    sexo: 'ND',
    tipoCorreo: '@gmail.com',
    codigoCelular: '0414',
    codigoTelefono: '0274'
  };

  // Listas para Registro de Pacientes (Pachón Pro V2.9)
  public tiposCorreo = ['@gmail.com', '@hotmail.com', '@outlook.com', '@yahoo.com'];
  public codigosCelular = ['0416', '0426', '0414', '0424', '0412', '0422'];
  public codigosTelefono = ['0273', '0251', '0212', '0281', '0241'];

  // --- Gestión de Seguridad de Precios (Fase 1) ---
  private pendingEditInfo: { index: number, isBackend: boolean } | null = null;

  handleEditarPrecio(event: { index: number, isBackend: boolean }) {
    this.pendingEditInfo = event;
    const isPrivileged = this.authService.isSupervisor() || this.authService.isAdministrador();

    if (isPrivileged) {
      this.solicitarNuevoPrecio();
    } else {
      this.supervisorDialog.open();
    }
  }

  onSupervisorAuthorized(key: string) {
    this.billingFacade.currentSupervisorKey.set(key);
    this.solicitarNuevoPrecio();
  }

  private solicitarNuevoPrecio() {
    if (!this.pendingEditInfo) return;
    const { index, isBackend } = this.pendingEditInfo;

    // UI REFINEMENT (Phase 9): Passing both price and honorary to the inline editor
    const item = isBackend ? this.serviciosEnBackend()[index] : this.carritoLocal()[index];
    const currentPrice = item.precioUsd || item.PrecioUsd || item.precio || 0;
    const currentHonorary = item.honorarioUsd ?? item.HonorarioUsd ?? item.honorario ?? item.Honorario ?? item.honorarioBase ?? item.HonorarioBase ?? 0;
    
    const isConsult = item.isConsultation;
    
    const inputPrice = isConsult ? Math.max(0, currentPrice - currentHonorary) : currentPrice;
    const inputHonorary = currentHonorary;

    // Delegar al componente cart que inicie el modo edición
    this.billingCart.startEdit(index, isBackend, inputPrice, inputHonorary);
    this.pendingEditInfo = null;
  }

  onPrecioCambiado(event: { index: number, isBackend: boolean, newPrice: number, newHonorary: number }) {
    const { index, isBackend, newPrice, newHonorary } = event;

    if (isNaN(newPrice) || newPrice < 0) {
      this.errorMessage.set("Ingrese un precio válido (mayor o igual a cero).");
      return;
    }

    if (isBackend) {
      this.errorMessage.set("Solo se permite editar precios de servicios nuevos antes de sincronizar.");
    } else {
      this.billingFacade.carritoLocal.update(cart => {
        const newCart = [...cart];
        const item = newCart[index];
        const isConsult = item.isConsultation;
        const finalPrice = isConsult ? (newPrice + newHonorary) : newPrice;
        const finalHonorary = newHonorary;
        const baseHon = isConsult ? newHonorary : item.honorarioBase;

        newCart[index] = new CatalogItem({ 
          ...item, 
          precioUsd: finalPrice, 
          precio: finalPrice, 
          honorarioUsd: finalHonorary, 
          honorarioBase: baseHon 
        });
        return newCart;
      });
      this.actionMessage.set("Precio y Honorario modificados exitosamente.");
    }
  }



  // Navegación del Wizard
  nextStep() {
    if (this.currentStep() < this.maxSteps()) {
      // Validaciones para flujo de 3 pasos (Seguros)
      if (this.maxSteps() === 3) {
        if (this.currentStep() === 1 && this.tipoIngreso() === 'Seguro' && !this.convenioId()) {
          this.errorMessage.set("Debe seleccionar un convenio para continuar.");
          return;
        }
        if (this.currentStep() === 2 && this.serviciosCargados().length === 0) {
          this.errorMessage.set("Debe añadir al menos un servicio para continuar.");
          return;
        }
      } 
      // Validaciones para flujo de 2 pasos (Particular)
      else if (this.maxSteps() === 2) {
        if (this.currentStep() === 1 && this.serviciosCargados().length === 0) {
          this.errorMessage.set("Debe añadir al menos un servicio para continuar.");
          return;
        }
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

  seleccionarPaciente(p: PatientRecord | any) {
    const pId = p.id || p.Id;
    if (pId) {
      this.pacienteId.set(pId);
      this.selectedPatientData.set(p);
      this.noResultsFound.set(false);
    }
    this.showResultadosBusqueda.set(false);
    this.busquedaTermino.set(p.cedula || p.Cedula);
    this.actionMessage.set(`Paciente seleccionado: ${p.nombre || p.Nombre || ''} ${p.apellidos || p.Apellidos || ''}`.trim());
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
        this.selectedPatientData()?.idPacienteLegacy ?? (this.selectedPatientData() as any)?.IdPacienteLegacy
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
            return updated ? new CatalogItem({ ...localItem, precio: updated.precio }) : localItem;
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

    // Cargar horario de referencia (V5.1) y sincronizar agenda
    this.settingsService.getMedicosHorarios().subscribe(res => {
      const match = res.find(x => (x.medicoId || x.MedicoId || '').toString().toLowerCase() === this.selectedMedicoId()?.toLowerCase());
      const schedules = match ? (match.horarios || match.Horarios || []) : [];
      this.medicoWorkingHours.set(schedules);

      const dayOfWeek = this.getDayFromDate(dateToSearch);
      const daySchedules = schedules.filter((h: any) => h.diaSemana === dayOfWeek);

      this.appointmentsService.getDoctorScheduleWithPatient(
        this.selectedMedicoId()!,
        dateToSearch,
        this.pacienteId() || undefined
      ).subscribe({
        next: (res: any) => {
          if (daySchedules.length > 0) {
            // Filtrar slots que caen dentro de los rangos configurados (V6.0)
            const filteredSlots = res.turnos.filter((slot: any) => {
              // Extraer solo la hora HH:mm de la fecha ISO de forma manual (V6.1 Consistency Fix)
              let slotTime = slot.hora;
              if (slotTime.includes('T')) {
                slotTime = slotTime.split('T')[1].substring(0, 5);
              } else if (slotTime.length > 5 && slotTime.includes(':')) {
                slotTime = slotTime.substring(0, 5);
              }
              return daySchedules.some((h: any) => slotTime >= h.inicio && slotTime < h.fin);
            });
            this.availableSlots.set(filteredSlots);
          } else {
            // [V7.1 Fix] Si no hay horarios específicos definidos para este día,
            // respetamos el fallback automático del servidor (usualmente 8 AM - 6:30 PM).
            this.availableSlots.set(res.turnos);
          }
          this.scheduleLoading.set(false);
        },
        error: () => {
          this.errorMessage.set("No se pudo cargar la agenda del médico.");
          this.scheduleLoading.set(false);
        }
      });
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
          const horaFormateada = this.getHoraRango(slot.hora);
          console.log("DEBUG-TURNO: Reservado temporalmente:", horaFormateada);
          this.selectedSlot.set(horaFormateada);
          this.horaCita.set(horaNormalizada);
          this.comentarioCita.set(slot.comentario === 'Disponible' ? '' : slot.comentario);
          this.showScheduleModal.set(false);
          this.isLoading.set(false);
          this.errorMessage.set(null);
          this.actionMessage.set(`Horario ${horaFormateada} reservado temporalmente.`);

          // [V12.5 Auto-Add] Intentar cargar el servicio de consulta automáticamente
          console.log("DEBUG-TURNO: suggestedServices count:", this.suggestedServices().length);
          console.log("DEBUG-TURNO: suggestedServices detail:", JSON.stringify(this.suggestedServices().map(s => ({ id: s.id, codigo: s.codigo, descripcion: s.descripcion, isConsultation: s.isConsultation }))));
          const matchingConsultation = this.suggestedServices().find(s => s.isConsultation);
          if (matchingConsultation) {
            console.log("DEBUG-TURNO: Found matchingConsultation, calling cargarServicio with ID:", matchingConsultation.id);
            this.cargarServicio(matchingConsultation.id);
          } else {
            console.log("DEBUG-TURNO: No matchingConsultation found in suggestedServices.");
          }
        },
        error: (err) => {
          console.log("DEBUG-TURNO: Error reserving slot:", JSON.stringify(err));
          this.errorMessage.set(err.error?.error || "No se pudo reservar el turno. Intente con otro.");
          this.isLoading.set(false);
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

        const medicos = this.medicosFiltrados();
        if (medicos.length === 1) {
          const m = medicos[0];
          const mId = m.id || (m as any).Id;
          const mNombre = m.nombre || (m as any).Nombre;
          this.selectedMedicoId.set(mId);
          this.actionMessage.set(`Especialidad ${match} detectada. Médico sugerido: ${mNombre}`);
        }
      }, 300); // Un poco más de tiempo para que medicosFiltrados reaccione al cambio de especialidad
    }
  }

  cargarServicio(servId: string) {
    this.cargarServicioAsync(servId).catch(() => {});
  }

  async cargarServicioAsync(servId: string, skipSuggestions = false): Promise<void> {
    console.log("DEBUG-CARGAR: cargarServicioAsync called with ID:", servId, "skipSuggestions:", skipSuggestions);
    const s = this.billingFacade.servicesCatalog().find((x: CatalogItem) => x.id === servId);
    if (!s) {
      console.log("DEBUG-CARGAR: Service not found in servicesCatalog. ID:", servId);
      return;
    }
    console.log("DEBUG-CARGAR: Found service in catalog:", s.codigo, s.descripcion);

    // Abstracción Senior: Identificación por Categoría de Dominio (V5.2)
    const esConsulta = s.isConsultation;

    // Validación estricta V3.0 (Micro-Ciclo 38): Consulta requiere Médico Y Turno Agendado
    if (esConsulta) {
      if (!this.selectedMedicoId()) {
        this.seleccionarTipoConsulta(s);
        this.errorMessage.set(`Indique el médico para procesar la ${s.descripcion}.`);
        throw new Error("Médico requerido");
      }

      if (!this.selectedSlot() || !this.horaCita()) {
        this.isPendingConsultation.set(true); // Activar por si acaso el usuario vuelve a darle
        this.abrirModalHorarios();
        this.errorMessage.set(`Por favor, seleccione un horario disponible para el Dr. ${this.nombreMedicoSeleccionado()}.`);
        throw new Error("Horario requerido");
      }
    }

    let finalDescripcion = s.descripcion;
    // Ya no necesitamos el sufijo manual si el ítem ya es específico, 
    // pero lo mantenemos por compatibilidad con el ítem S001 general
    if (s.id === 'S001' && this.selectedEspecialidad()) {
      const esp = this.selectedEspecialidad()!;
      finalDescripcion = `${finalDescripcion} (${esp})`;
    }

    const selectedDoctor = esConsulta ? this.medicosFiltrados().find(m => (m.id || (m as any).Id) === this.selectedMedicoId()) : null;
    const doctorHonorary = selectedDoctor ? (selectedDoctor.honorarioBase ?? (selectedDoctor as any).HonorarioBase ?? 0) : 0;
    const precioBase = (s.precioUsd ?? 0) - (s.honorarioUsd ?? 0);
    const finalPrecio = esConsulta ? (precioBase + doctorHonorary) : (s.precioUsd ?? 0);
    const finalHonorary = esConsulta ? doctorHonorary : (s.honorarioUsd ?? 0);

    const pId = this.pacienteId();

    // Si no hay paciente, agregar al carrito local (No bloqueante)
    if (pId === null) {
      const yaEnCarrito = this.carritoLocal().some(x => x.id === s.id);
      if (!yaEnCarrito) {
        this.carritoLocal.update(prev => [...prev, new CatalogItem({
          ...s,
          precioUsd: finalPrecio,
          precio: finalPrecio,
          precioBs: finalPrecio * this.tasaCambioDia(),
          honorarioUsd: finalHonorary,
          honorarioBase: esConsulta ? doctorHonorary : s.honorarioBase,
          descripcion: finalDescripcion,
          medicoId: esConsulta ? this.selectedMedicoId() : undefined,
          medicoNombre: esConsulta ? this.nombreMedicoSeleccionado() : undefined,
          horaCita: esConsulta ? this.horaCita() : undefined,
          comentario: this.comentarioCita()
        })]);
        this.resetCitaSelection();
        this.actionMessage.set(`Servicio "${finalDescripcion}" añadido al carrito temporal.`);
        
        // Activar sugerencias también en el carrito local (V11.17)
        if (!skipSuggestions) {
          this.triggerSuggestion(s);
        }
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
      precio: finalPrecio,
      honorario: finalHonorary,
      cantidad: 1,
      tipoServicio: s.tipo,
      usuarioCarga: this.user()?.username || '',
      medicoId: esConsulta ? this.selectedMedicoId() || undefined : undefined,
      horaCita: fullHoraCita,
      comentario: this.comentarioCita() || undefined
    };

    const idempotencyKey = (typeof crypto !== 'undefined' && crypto.randomUUID)
      ? crypto.randomUUID()
      : 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, c => {
          const r = Math.random() * 16 | 0;
          return (c === 'x' ? r : (r & 0x3 | 0x8)).toString(16);
        });

    try {
      const res: any = await firstValueFrom(this.facturacionService.cargarServicio(payload, idempotencyKey));
      this.cuentaId.set(res.cuentaId);
      this.serviciosEnBackend.update((prev: any[]) => [...prev, new CatalogItem({
        ...s,
        detalleId: res.detalleId, // Guárdalo para eliminación precisa (V4.8)
        medicoId: esConsulta ? this.selectedMedicoId() : undefined, // Guardar ID para limpieza de cita
        hora: this.horaCita(),
        medicoNombre: esConsulta ? this.nombreMedicoSeleccionado() : undefined,
        precio: payload.precio,
        precioBs: payload.precio * this.tasaCambioDia(),
        precioUsd: payload.precio,
        honorarioUsd: payload.honorario,
        honorarioBase: esConsulta ? doctorHonorary : s.honorarioBase
      })]);
      this.resetCitaSelection();
      this.actionMessage.set("Servicio cargado exitosamente.");

      // AUTO-SUGERENCIA: Resaltar servicios relacionados (ej: Informes de Tomografía/RX)
      if (!skipSuggestions) {
        this.triggerSuggestion(s);
      }
      
      this.errorMessage.set(null);
    } catch (err: any) {
      this.errorMessage.set(err.error?.error || "Error al cargar servicio.");
      throw err;
    } finally {
      this.isLoading.set(false);
    }
  }

  private triggerSuggestion(s: CatalogItem) {
    console.log("DEBUG-SUGGESTION: triggerSuggestion called for:", s.codigo, s.descripcion);
    const sugIds = s.sugerenciasIds || s.SugerenciasIds || [];
    console.log("DEBUG-SUGGESTION: sugIds for item:", JSON.stringify(sugIds));
    if (sugIds.length > 0) {
      console.log("DEBUG-SUGGESTION: servicesCatalog count:", this.billingFacade.servicesCatalog().length);
      const matches = this.billingFacade.servicesCatalog().filter(item => 
        sugIds.includes(item.id)
      );
      console.log("DEBUG-SUGGESTION: matches found in catalog:", JSON.stringify(matches.map(m => ({ id: m.id, codigo: m.codigo, descripcion: m.descripcion }))));
      
      // Filtrar las que ya están en el carrito (V12.5 Robustness)
      const serviciosCargadosIds = this.serviciosCargados().map(c => c.id);
      console.log("DEBUG-SUGGESTION: serviciosCargados ids:", JSON.stringify(serviciosCargadosIds));
      const filteredMatches = matches.filter(m => !this.serviciosCargados().some(c => c.id === m.id));
      console.log("DEBUG-SUGGESTION: filteredMatches:", JSON.stringify(filteredMatches.map(m => ({ id: m.id, codigo: m.codigo, descripcion: m.descripcion }))));
      
      if (filteredMatches.length > 0) {
        this.activeSuggestions.set(filteredMatches);
        // Seleccionar todas por defecto
        const initialSelected: Record<string, boolean> = {};
        filteredMatches.forEach(m => {
          initialSelected[m.id] = true;
        });
        this.selectedSuggestions.set(initialSelected);
        this.showSuggestionModal.set(true);
        console.log("DEBUG-SUGGESTION: showSuggestionModal set to true. Active suggestions:", JSON.stringify(this.activeSuggestions()));
      } else {
        console.log("DEBUG-SUGGESTION: No new suggested matches to display.");
      }
    } else {
      console.log("DEBUG-SUGGESTION: sugIds is empty.");
    }
  }

  public toggleSuggestionSelection(id: string) {
    this.selectedSuggestions.update(prev => ({
      ...prev,
      [id]: !prev[id]
    }));
  }

  public async aceptarSugerencias() {
    const selectedIds = Object.entries(this.selectedSuggestions())
      .filter(([_, selected]) => selected)
      .map(([id, _]) => id);

    this.showSuggestionModal.set(false);

    if (selectedIds.length > 0) {
      this.isLoading.set(true);
      for (const id of selectedIds) {
        try {
          await this.cargarServicioAsync(id, true);
        } catch (e) {
          console.error("Error al cargar sugerencia:", id, e);
        }
      }
      this.isLoading.set(false);
      this.activeSuggestions.set([]);
      this.selectedSuggestions.set({});
    }
  }

  public rechazarSugerencia() {
    this.showSuggestionModal.set(false);
    this.activeSuggestions.set([]);
    this.selectedSuggestions.set({});
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
        const p: any = this.selectedPatientData();
        const pNombre = p ? (p.nombre || p.Nombre || '') : '';
        const pApellidos = p ? (p.apellidos || p.Apellidos || '') : '';
        const pacienteNombre = p ? `${pNombre} ${pApellidos}`.trim() : '';
        this.actionMessage.set(`¡Facturación Exitosa! Paciente: ${pacienteNombre}.`);

        // Automatización Total (V12.4): Abrir PDF directamente
        const reciboId = res.reciboId || res.id;
        if (reciboId) {
          const downloadUrl = `${environment.apiUrl}/api/ReciboFactura/${reciboId}/Download`;
          window.open(downloadUrl, '_blank');
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
   * @param autoPrintCompromiso Si es true y es flujo de seguro, genera el PDF automáticamente al finalizar.
   * [Fase 12.1 Refinement]
   */
  async omitirComprobante(autoPrintCompromiso: boolean = false) {
    // Si ya fue exitoso y el usuario hace clic otra vez en "Compromiso de Pago", simplemente re-imprimir (V12.6 Fix: Particular Flow)
    if (this.billingSuccess() && autoPrintCompromiso) {
       this.imprimirCompromiso();
       return;
    }

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
        const p: any = this.selectedPatientData();
        const pNombre = p ? (p.nombre || p.Nombre || '') : '';
        const pApellidos = p ? (p.apellidos || p.Apellidos || '') : '';
        const pacienteNombre = p ? `${pNombre} ${pApellidos}`.trim() : '';
        
        // V12.1 Integration: Capturar resultado para panel de éxito
        this.lastBillResult.set(res);
        this.isInsuranceFlow.set(this.tipoIngreso() === 'Seguro');
        this.billingSuccess.set(true);

        if (autoPrintCompromiso) {
           this.imprimirCompromiso();
        }

        this.actionMessage.set(`Ha agregado satisfactoriamente a: ${pacienteNombre}`);
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

  quitarServicio(event: { index: number, isBackend: boolean }) {
    const { index, isBackend } = event;

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
    this.billingSuccess.set(false); // V12.1

    this.docMetadata.set({
      quienAutorizo: '',
      doctorProcedimiento: '',
      informacionAdicional: '',
      diasLiquidar: 1,
      cuotas: 1,
      montoGarantia: 0,
      descripcionGarantia: ''
    });
    this.anexarGarantia.set(true);

    // Recargar métodos de pago si se perdieron durante la sesión
    this.billingFacade.reloadPaymentCatalogIfEmpty();
  }

  // --- Lógica de Gestión de Documentos Post-Facturación (V12.1) ---
  private calcularEdad(fechaNacimiento: string): number {
    if (!fechaNacimiento) return 0;
    const today = new Date();
    const birth = new Date(fechaNacimiento);
    let age = today.getFullYear() - birth.getFullYear();
    const m = today.getMonth() - birth.getMonth();
    if (m < 0 || (m === 0 && today.getDate() < birth.getDate())) {
      age--;
    }
    return age;
  }

  imprimirCompromiso() {
    const res = this.lastBillResult();
    const p: any = this.selectedPatientData();
    if (!res || !p) return;

    const nombre = p.nombre || p.Nombre || '';
    const apellidos = p.apellidos || p.Apellidos || '';
    const cedula = p.cedula || p.Cedula || '';
    const direccion = p.direccion || p.Direccion || '';
    const celular = p.celular || p.Celular || '';
    const telefono = p.telefono || p.Telefono || '';
    const fechaNacimiento = p.fechaNacimiento || p.FechaNacimiento || '';

    this.isGeneratingPdf.set(true);
    const dto = {
      cuentaPorCobrarId: res.cuentaPorCobrarId,
      nombreResponsable: `${nombre} ${apellidos}`.trim(),
      relacionResponsable: 'Titular',
      cedulaResponsable: cedula,
      direccionResponsable: direccion || 'No especificada',
      telefonoResponsable: celular || telefono || 'No especificado', // Fix: Use celular as primary
      conceptos: this.serviciosCargados().map(s => s.descripcion || s.Descripcion).join(', '),
      nombrePaciente: `${nombre} ${apellidos}`.trim(),
      edadPaciente: this.calcularEdad(fechaNacimiento || ''),
      cedulaPaciente: cedula,
      direccionPaciente: direccion,
      telefonoPaciente: celular || telefono,
      montoTotal: this.totalCargadoUSD(), // Use cart total
      diasLiquidar: this.docMetadata().diasLiquidar,
      cuotas: this.docMetadata().cuotas,
      montoGarantia: this.tipoIngreso() === 'Particular' ? this.totalMontoGarantias() : 0,
      descripcionGarantia: this.tipoIngreso() === 'Particular' ? this.garantiasItems().map(i => i.descripcion).join(', ') : '',
      garantiasItems: this.tipoIngreso() === 'Particular' ? this.garantiasItems() : [],
      quienAutorizo: this.docMetadata().quienAutorizo,
      doctorProcedimiento: this.docMetadata().doctorProcedimiento,
      informacionAdicional: this.docMetadata().informacionAdicional,
      esPagoCompletado: this.lastBillResult()?.totalPagado >= this.lastBillResult()?.montoTotal,
      fechaCompromiso: new Date().toISOString(),
      fechaVencimiento: new Date(Date.now() + (this.docMetadata().diasLiquidar * 24 * 60 * 60 * 1000)).toISOString(),
      anexarGarantia: this.tipoIngreso() === 'Particular' && this.anexarGarantia()
    };

    this.facturacionService.generarCompromisoPdf(dto).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        window.open(url, '_blank');
        this.isGeneratingPdf.set(false);
      },
      error: () => {
        this.errorMessage.set('Error al generar compromiso de pago');
        this.isGeneratingPdf.set(false);
      }
    });
  }

  imprimirGarantia() {
      // Similar a compromiso pero llamando a generarGarantiaPdf
      const res = this.lastBillResult();
      const p: any = this.selectedPatientData();
      if (!res || !p) return;

      const nombre = p.nombre || p.Nombre || '';
      const apellidos = p.apellidos || p.Apellidos || '';
      const cedula = p.cedula || p.Cedula || '';
      const direccion = p.direccion || p.Direccion || '';
      const celular = p.celular || p.Celular || '';
      const telefono = p.telefono || p.Telefono || '';
      const fechaNacimiento = p.fechaNacimiento || p.FechaNacimiento || '';
  
      this.isGeneratingPdf.set(true);
      const dto = {
        cuentaPorCobrarId: res.cuentaPorCobrarId,
        nombreResponsable: `${nombre} ${apellidos}`.trim(),
        relacionResponsable: 'Titular',
        cedulaResponsable: cedula,
        direccionResponsable: direccion || 'No especificada',
        telefonoResponsable: celular || telefono || 'No especificado',
        conceptos: this.serviciosCargados().map(s => s.descripcion || s.Descripcion).join(', '),
        nombrePaciente: `${nombre} ${apellidos}`.trim(),
        edadPaciente: this.calcularEdad(fechaNacimiento || ''),
        cedulaPaciente: cedula,
        direccionPaciente: direccion,
        telefonoPaciente: celular || telefono,
        montoTotal: this.totalCargadoUSD(),
        diasLiquidar: this.docMetadata().diasLiquidar,
        cuotas: this.docMetadata().cuotas,
        montoGarantia: this.totalMontoGarantias(),
        descripcionGarantia: this.garantiasItems().map(i => i.descripcion).join(', '),
        garantiasItems: this.garantiasItems(),
        quienAutorizo: this.docMetadata().quienAutorizo,
        doctorProcedimiento: this.docMetadata().doctorProcedimiento,
        informacionAdicional: this.docMetadata().informacionAdicional,
        esPagoCompletado: this.lastBillResult()?.totalPagado >= this.lastBillResult()?.montoTotal,
        fechaCompromiso: new Date().toISOString(),
        fechaVencimiento: new Date(Date.now() + (this.docMetadata().diasLiquidar * 24 * 60 * 60 * 1000)).toISOString()
      };
  
      this.facturacionService.generarGarantiaPdf(dto).subscribe({
        next: (blob) => {
          const url = window.URL.createObjectURL(blob);
          window.open(url, '_blank');
          this.isGeneratingPdf.set(false);
        },
        error: () => {
          this.errorMessage.set('Error al generar garantía de pago');
          this.isGeneratingPdf.set(false);
        }
      });
  }

  finalizarFlujoFacturacion() {
    const res = this.lastBillResult();
    const meta = this.docMetadata();
    if (res && res.cuentaPorCobrarId) {
      this.facturacionService.updateARMetadata({
        cuentaPorCobrarId: res.cuentaPorCobrarId,
        quienAutorizo: meta.quienAutorizo || null,
        doctorProcedimiento: meta.doctorProcedimiento || null,
        informacionAdicional: meta.informacionAdicional || null
      }).subscribe({
        next: () => console.log('Metadata de documentos guardada exitosamente.'),
        error: (err) => console.error('Error al guardar metadata:', err)
      });
    }

    this.resetForm();
    this.billingSuccess.set(false);
    this.lastBillResult.set(null);
  }
}
