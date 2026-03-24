import { Component, inject, signal, computed, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { FacturacionService, DetallePagoDto, CargarServicioACuentaRequest, ReceiptPrintData } from '../../../core/services/facturacion.service';
import { AuthService } from '../../../core/services/auth.service';
import { AppointmentsService, Doctor, ScheduleEntry } from '../../../core/services/appointments.service';
import { SpecialtyService, Especialidad as Specialty } from '../../../core/services/specialty.service';
import { CatalogService } from '../../../core/services/catalog.service';
import { CatalogItem } from '../../../core/models/priced-item.model';
import { PatientService, PatientRecord } from '../../../core/services/patient.service';
import { CajaService, DailyClosingReport } from '../../../core/services/caja.service';
import { PrintService } from '../../../core/services/print.service';
import { toObservable, toSignal } from '@angular/core/rxjs-interop';
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
  UserPlus
} from 'lucide-angular';
import { switchMap, of } from 'rxjs';

@Component({
  selector: 'app-facturacion',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule],
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
    UserPlus
  };
  private facturacionService = inject(FacturacionService);
  private authService = inject(AuthService);
  private appointmentsService = inject(AppointmentsService);
  private specialtyService = inject(SpecialtyService);
  private catalogService = inject(CatalogService);
  private patientService = inject(PatientService);
  private printService = inject(PrintService);

  // Estados de Usuario y Rol
  public user = this.authService.currentUser;
  public isAdmin = computed(() => this.user()?.role === 'Administrador');
  public isParticularAssistant = computed(() => this.user()?.role === 'Asistente Particular');
  public isInsuranceAssistant = computed(() => this.user()?.role === 'Asistente Seguro');
  public isRxAssistant = computed(() => this.user()?.role === 'Asistente Rx');
  public isHospitalAssistant = computed(() => this.user()?.role === 'Asistente Hospitalario');
  public isEmergencyAssistant = computed(() => this.user()?.role === 'Asistente Emergencia');

  // Estados de Facturación
  public pacienteId = signal<number | null>(null);
  public pacienteSeleccionado = computed(() => !!this.pacienteId());
  public cuentaId = signal<string | null>(null);
  public tipoIngreso = signal<string>('Particular');
  public convenioId = signal<number | null>(null); // Se unificó a ID numérico del Legacy
  public tasaCambioDia = signal<number>(45.50);

  // Mapeo de Especialidades para Búsqueda en Catálogo (Fase 30)
  private specialtySearchMap: Record<string, string> = {
    'Ginecologo': 'GINECO',
    'Ginecólogo': 'GINECO',
    'Pediatra': 'PEDIAT',
    'Traumatologo': 'TRAUMA',
    'Traumatólogo': 'TRAUMA',
    'Cardiologo': 'CARDIO',
    'Cardiólogo': 'CARDIO',
    'Medicina General': 'GENERAL',
    'Urologo': 'UROLO',
    'Urólogo': 'UROLO',
    'Oftalmologo': 'OFTALMO',
    'Oftalmólogo': 'OFTALMO',
    'Obstetricia': 'OBSTETRI'
  };

  public suggestedServiceId = signal<string | null>(null);

  // Estado del Wizard (UX Improvement)
  public currentStep = signal<number>(1);
  public maxSteps = 3;

  // Catálogos Reales (Sincronizados con Backend)
  public especialidades = signal<string[]>([]);
  public convenios = signal<any[]>([
    { id: 1, nombre: 'Particular' },
    { id: 2, nombre: 'Seguro Universal' },
    { id: 3, nombre: 'Aseguradora Regional' }
  ]);

  // Filtros UI y Signals Reactivos
  public selectedEspecialidad = signal<string | null>(null);

  public medicosFiltrados = toSignal(
    toObservable(this.selectedEspecialidad).pipe(
      switchMap((esp: string | null) => esp ? this.appointmentsService.getDoctorsBySpecialty(esp) : of([] as Doctor[]))
    ), { initialValue: [] as Doctor[] }
  );

  // Efecto para Auto-sugerir Servicio según Especialidad
  private _specialtyEffect = effect(() => {
    const esp = this.selectedEspecialidad();
    if (!esp) {
      this.suggestedServiceId.set(null);
      return;
    }

    const searchTerm = this.specialtySearchMap[esp] || esp.toUpperCase();
    const service = this.serviciosCatalogo().find(s =>
      s.tipo.toUpperCase().includes('CONSULTA') &&
      s.descripcion.toUpperCase().includes(searchTerm)
    );

    if (service) {
      this.suggestedServiceId.set(service.id);
      // Opcional: Si el usuario quiere "todo automático", podríamos hacer auto-scroll o pre-selección
    } else {
      // Fallback a General si no hay específico
      const general = this.serviciosCatalogo().find(s => s.id === 'S001' || s.descripcion.includes('GENERAL'));
      this.suggestedServiceId.set(general?.id || null);
    }
  });

  public selectedMedicoId = signal<string | null>(null);
  public showScheduleModal = signal<boolean>(false);
  public scheduleLoading = signal<boolean>(false);
  public availableSlots = signal<ScheduleEntry[]>([]);
  public selectedSlot = signal<string | null>(null);
  public comentarioCita = signal<string | null>(null);
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

  // Catálogo Real Filtrado por Rol y Búsqueda (UX Improvement)
  public serviciosCatalogo = signal<CatalogItem[]>([]);
  public serviciosFiltradosPorRol = computed(() => {
    const raw = this.serviciosCatalogo();
    const roleFiltered = this.isAdmin() ? raw : (this.isRxAssistant() ? raw.filter(s => s.tipo === 'RX') : raw);

    // Null check for searchTermServicio
    const term = (this.searchTermServicio() || '').toLowerCase().trim();
    if (!term) return roleFiltered;

    return roleFiltered.filter(s => {
      const desc = (s.descripcion || '').toLowerCase();
      const tipo = (s.tipo || '').toLowerCase();
      return desc.includes(term) || tipo.includes(term);
    });
  });

  // Carrito de Servicios (Servicios ya persistidos en la cuenta)
  public serviciosEnBackend = signal<any[]>([]);
  public carritoLocal = signal<any[]>([]);

  // Vista unificada del Carrito
  public serviciosCargados = computed(() => [...this.serviciosEnBackend(), ...this.carritoLocal()]);

  // Array de Pagos
  public pagos = signal<DetallePagoDto[]>([]);

  // Feedback UI
  public isLoading = signal<boolean>(false);
  public actionMessage = signal<string | null>(null);
  public errorMessage = signal<string | null>(null);

  // States for Service Search (UX Improvement)
  public searchTermServicio = signal<string>('');

  // States for Patient Search (Micro-Ciclo 9)
  public busquedaTermino = signal<string>('');
  public resultadosPacientes = signal<PatientRecord[]>([]);
  public buscandoPaciente = signal<boolean>(false);
  public showResultadosBusqueda = signal<boolean>(false);

  // States for New Patient Modal (Micro-Ciclo 11)
  public showRegisterModal = signal<boolean>(false);
  public newPatientData = {
    cedula: '',
    nombre: '',
    telefono: ''
  };

  // Nuevo Elemento Form (Pagos)
  public currentPago = {
    metodoPago: 'Punto de Venta Bs',
    referenciaBancaria: '',
    montoAbonadoMoneda: 0
  };

  public metodosDisponibles = ['Punto de Venta Bs', 'Pago Móvil', 'Efectivo Divisas', 'Zelle'];

  // Impuestos y Totales (Calculados reactivamente a la tasa)
  public totalCargado = computed(() => {
    const tasa = this.tasaCambioDia();
    return this.serviciosCargados().reduce((acc: number, curr: any) => {
      // Intentar calcular desde USD si existe, si no usar precio base
      const usd = curr.precioUsd ?? curr.PrecioUsd ?? 0;
      const priceBs = usd > 0 ? (usd * tasa) : (curr.precioBs ?? curr.PrecioBs ?? curr.precio);
      return acc + priceBs;
    }, 0);
  });
  public totalFacturadoBase = computed(() => this.pagos().reduce((acc: number, curr: DetallePagoDto) => acc + curr.equivalenteAbonadoBase, 0));

  constructor() {
    // Inicialización de Estados Seguros según Rol
    if (this.isParticularAssistant()) this.tipoIngreso.set('Particular');
    if (this.isInsuranceAssistant()) this.tipoIngreso.set('Seguro');
    if (this.isRxAssistant()) this.tipoIngreso.set('Hospitalizacion');
    if (this.isHospitalAssistant()) this.tipoIngreso.set('Hospitalizacion');
    if (this.isEmergencyAssistant()) this.tipoIngreso.set('Emergencia');

    // Cargar Especialidades Dinámicas
    this.specialtyService.getAll().subscribe(res => {
      this.especialidades.set(res.filter(e => e.activo).map(e => e.nombre));
    });

    // Cargar Catálogo Inicial
    this.refreshCatalog();

    // Reaccionar a cambios en convenio para actualizar precios
    effect(() => {
      const convId = this.convenioId();
      this.refreshCatalog(convId);
    });
  }

  // Navegación del Wizard
  nextStep() {
    if (this.currentStep() < this.maxSteps) {
      // Validación Paso 1: Servicios (Ahora es el primero)
      if (this.currentStep() === 1 && this.serviciosCargados().length === 0) {
        this.errorMessage.set("Debe añadir al menos un servicio para continuar.");
        return;
      }
      // Validación Paso 2: Convenio (Ahora es el segundo)
      if (this.currentStep() === 2 && this.tipoIngreso() === 'Seguro' && !this.convenioId()) {
        this.errorMessage.set("Debe seleccionar un convenio para continuar.");
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
    this.patientService.searchPatients(term).subscribe({
      next: (res: PatientRecord[]) => {
        this.resultadosPacientes.set(res);
        this.buscandoPaciente.set(false);
      },
      error: () => {
        this.errorMessage.set("Error al buscar pacientes.");
        this.buscandoPaciente.set(false);
      }
    });
  }

  seleccionarPaciente(p: PatientRecord) {
    if (p.id) {
      this.pacienteId.set(p.id);

      // Sincronizar Carrito Local al identificar al paciente
      if (this.carritoLocal().length > 0) {
        this.sincronizarCarrito();
      }
    }
    this.showResultadosBusqueda.set(false);
    this.busquedaTermino.set(p.cedula);
    this.actionMessage.set(`Paciente seleccionado: ${p.nombre} ${p.apellidos}`);
  }

  private sincronizarCarrito() {
    const items = [...this.carritoLocal()];
    this.carritoLocal.set([]); // Limpiar local temporalmente mientras se procesa
    this.isLoading.set(true);

    // Cargar cada item en el backend de forma secuencial o paralela según el servicio
    items.forEach(s => {
      this.procesarCargaBackend(s);
    });
  }

  private procesarCargaBackend(s: CatalogItem) {
    const pId = this.pacienteId();
    if (!pId) return;

    const payload: CargarServicioACuentaRequest = {
      pacienteId: pId,
      tipoIngreso: this.tipoIngreso(),
      convenioId: this.convenioId() || undefined,
      servicioId: s.id,
      descripcion: s.descripcion,
      precio: s.precio,
      cantidad: 1,
      tipoServicio: s.tipo,
      usuarioCarga: this.user()?.username || 'admin'
    };

    this.facturacionService.cargarServicio(payload).subscribe({
      next: (res: any) => {
        this.cuentaId.set(res.cuentaId);
        this.serviciosEnBackend.update(prev => [...prev, {
          ...s,
          precio: payload.precio,
          precioBs: s.precioBs,
          precioUsd: s.precioUsd
        }]);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  abrirRegistroPaciente() {
    this.newPatientData.cedula = this.busquedaTermino();
    this.showRegisterModal.set(true);
    this.showResultadosBusqueda.set(false);
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

  private refreshCatalog(convenioId?: number | null) {
    this.catalogService.getUnifiedCatalog(convenioId).subscribe({
      next: (items: CatalogItem[]) => {
        this.serviciosCatalogo.set(items);

        // Actualizar precios del carrito local si ya hay items y cambió el catálogo
        if (this.carritoLocal().length > 0) {
          this.carritoLocal.update(cart => cart.map(localItem => {
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

  abrirModalHorarios() {
    if (!this.selectedMedicoId()) return;

    this.scheduleLoading.set(true);
    this.showScheduleModal.set(true);
    const today = new Date().toISOString().split('T')[0];

    this.appointmentsService.getDoctorSchedule(this.selectedMedicoId()!, today).subscribe({
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
    this.facturacionService.reservarTurno({
      medicoId: this.selectedMedicoId()!,
      horaPautada: slot.hora
    }).subscribe({
      next: () => {
        this.selectedSlot.set(this.getHoraRango(slot.hora));
        this.horaCita.set(slot.hora);
        this.comentarioCita.set(slot.comentario === 'Disponible' ? '' : slot.comentario);
        this.showScheduleModal.set(false);
        this.isLoading.set(false);
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
    if (!this.isAdmin()) return;
    
    const motivo = prompt("Motivo del bloqueo administrativo:", "Reunión/Descanso");
    if (!motivo) return;

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
    if (!s.tipo.toUpperCase().includes('CONSULTA')) return;

    // Buscar si alguna especialidad coincide con la descripción
    const match = Object.entries(this.specialtySearchMap).find(([key, val]) =>
      s.descripcion.toUpperCase().includes(val)
    );

    if (match) {
      this.selectedEspecialidad.set(match[0]);
      this.actionMessage.set(`Filtrando médicos para: ${s.descripcion}`);
    }
  }

  cargarServicio(servId: string) {
    const s = this.serviciosCatalogo().find(x => x.id === servId);
    if (!s) return;

    const esConsulta = s.tipo.toUpperCase().includes('CONSULTA') || s.tipo.toUpperCase().includes('MEDICO') || s.tipo.toUpperCase().includes('MÉDICO');
    const esConsultaEspecializada = s.tipo.toUpperCase().includes('CONSULTA') && s.id !== 'S001';

    if (esConsultaEspecializada && !this.selectedMedicoId()) {
      this.seleccionarTipoConsulta(s);
      this.errorMessage.set(`Por favor, seleccione un médico para la ${s.descripcion}`);
      // Hacemos scroll hacia arriba para que vea el selector de médicos
      window.scrollTo({ top: 100, behavior: 'smooth' });
      return;
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
        this.carritoLocal.update(prev => [...prev, { ...s, descripcion: finalDescripcion }]);
        this.actionMessage.set(`Estudio "${finalDescripcion}" añadido al carrito temporal.`);
      } else {
        this.errorMessage.set("Este servicio ya está en el carrito.");
      }
      return;
    }

    // Si hay paciente, cargar directamente al backend
    this.isLoading.set(true);
    // Lógica de Horario Profesional (ISO)
    let fullHoraCita: string | undefined = undefined;
    if (esConsulta && this.horaCita()) {
      const now = new Date();
      const [hours, minutes] = this.horaCita().split(':');
      now.setHours(parseInt(hours), parseInt(minutes), 0, 0);
      fullHoraCita = now.toISOString();
    }

    const payload: CargarServicioACuentaRequest = {
      pacienteId: pId,
      tipoIngreso: this.tipoIngreso(),
      convenioId: this.convenioId() || undefined,
      servicioId: s.id,
      descripcion: finalDescripcion,
      precio: s.precio,
      cantidad: 1,
      tipoServicio: s.tipo,
      usuarioCarga: this.user()?.username || 'admin',
      medicoId: esConsulta ? this.selectedMedicoId() || undefined : undefined,
      horaCita: fullHoraCita,
      comentario: this.comentarioCita() || undefined
    };

    this.facturacionService.cargarServicio(payload).subscribe({
      next: (res: any) => {
        this.cuentaId.set(res.cuentaId);
        this.serviciosEnBackend.update((prev: any[]) => [...prev, {
          ...s,
          hora: this.horaCita(),
          precio: payload.precio,
          precioBs: s.precioBs,
          precioUsd: s.precioUsd
        }]);
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

  agregarPago() {
    if (this.currentPago.montoAbonadoMoneda <= 0) return;

    let eqBase = 0;
    const bsMethods = ['Punto de Venta Bs', 'Pago Móvil', 'Transferencia Bs'];
    if (bsMethods.includes(this.currentPago.metodoPago)) {
      eqBase = this.currentPago.montoAbonadoMoneda / this.tasaCambioDia();
    } else {
      eqBase = this.currentPago.montoAbonadoMoneda;
    }

    const nuevoPago: DetallePagoDto = {
      metodoPago: this.currentPago.metodoPago,
      referenciaBancaria: this.currentPago.referenciaBancaria || 'NA',
      montoAbonadoMoneda: this.currentPago.montoAbonadoMoneda,
      equivalenteAbonadoBase: parseFloat(eqBase.toFixed(2))
    };

    this.pagos.update((ps: DetallePagoDto[]) => [...ps, nuevoPago]);
    this.currentPago = { metodoPago: 'Punto de Venta Bs', referenciaBancaria: '', montoAbonadoMoneda: 0 };
  }

  removerPago(index: number) {
    this.pagos.update((ps: DetallePagoDto[]) => ps.filter((_, i) => i !== index));
  }


  procesarCobro() {
    if (!this.cuentaId() || this.pagos().length === 0) {
      this.errorMessage.set("No hay una cuenta activa o pagos registrados.");
      return;
    }

    this.isLoading.set(true);
    this.facturacionService.closeAccount({
      cuentaId: this.cuentaId()!,
      usuarioCajero: this.user()?.username || 'admin',
      tasaCambio: this.tasaCambioDia(),
      pagos: this.pagos()
    }).subscribe({
      next: (res: any) => {
        this.actionMessage.set(`¡Facturación Exitosa! Recibo: ${res.reciboId}`);

        this.imprimirRecibo(res.reciboId);

        this.resetForm();
        this.isLoading.set(false);
      },
      error: (err: any) => {
        this.errorMessage.set(err.error?.error || "Error al procesar cobro.");
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
    const servicios = this.serviciosCargados();
    if (index < 0 || index >= servicios.length) return;

    const item = servicios[index];
    const backendCount = this.serviciosEnBackend().length;

    // Si el item está en el backend (ya persistido)
    if (index < backendCount) {
      const cId = this.cuentaId();
      if (!cId) return;

      this.isLoading.set(true);
      this.facturacionService.quitarServicio(cId, item.id).subscribe({
        next: () => {
          this.serviciosEnBackend.update(prev => prev.filter((_, i) => i !== index));
          this.actionMessage.set("Servicio removido de la cuenta.");
          this.isLoading.set(false);
        },
        error: (err: any) => {
          this.errorMessage.set(err.error?.error || "Error al remover servicio.");
          this.isLoading.set(false);
        }
      });
    } else {
      // Si el item está en el carrito local
      const localIndex = index - backendCount;
      this.carritoLocal.update(prev => prev.filter((_, i) => i !== localIndex));
      this.actionMessage.set("Servicio removido del carrito temporal.");
    }
  }

  private resetForm() {
    this.pagos.set([]);
    this.serviciosEnBackend.set([]);
    this.carritoLocal.set([]);
    this.cuentaId.set(null);
    this.pacienteId.set(null);
  }
}
