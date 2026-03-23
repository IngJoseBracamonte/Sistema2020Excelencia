import { Component, inject, signal, computed, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { FacturacionService, DetallePagoDto, CargarServicioACuentaRequest, ReceiptPrintData } from '../../../core/services/facturacion.service';
import { AuthService } from '../../../core/services/auth.service';
import { AppointmentsService, Doctor, ScheduleEntry } from '../../../core/services/appointments.service';
import { CatalogService } from '../../../core/services/catalog.service';
import { CatalogItem } from '../../../core/models/priced-item.model';
import { PatientService, PatientRecord } from '../../../core/services/patient.service';
import { CajaService, DailyClosingReport } from '../../../core/services/caja.service';
import { PrintService } from '../../../core/services/print.service';
import { toObservable, toSignal } from '@angular/core/rxjs-interop';
import { switchMap, of } from 'rxjs';

@Component({
  selector: 'app-facturacion',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './facturacion.component.html',
  styleUrl: './facturacion.component.css'
})
export class FacturacionComponent {
  private facturacionService = inject(FacturacionService);
  private authService = inject(AuthService);
  private appointmentsService = inject(AppointmentsService);
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
  
  // Estado del Wizard (UX Improvement)
  public currentStep = signal<number>(1);
  public maxSteps = 3;

  // Catálogos Reales (Sincronizados con Backend)
  public especialidades = ['Ginecologo', 'Pediatra', 'Traumatologo', 'Cardiologo', 'Medicina General'];
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

  public selectedMedicoId = signal<string | null>(null);
  public showScheduleModal = signal<boolean>(false);
  public scheduleLoading = signal<boolean>(false);
  public availableSlots = signal<ScheduleEntry[]>([]);
  public selectedSlot = signal<string | null>(null);
  public horaCita = signal<string>('08:00');

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

  // Impuestos y Totales (Calculados)
  public totalCargado = computed(() => this.serviciosCargados().reduce((acc: number, curr: any) => acc + curr.precio, 0));
  public totalFacturadoBase = computed(() => this.pagos().reduce((acc: number, curr: DetallePagoDto) => acc + curr.equivalenteAbonadoBase, 0));

  constructor() {
    // Inicialización de Estados Seguros según Rol
    if (this.isParticularAssistant()) this.tipoIngreso.set('Particular');
    if (this.isInsuranceAssistant()) this.tipoIngreso.set('Seguro');
    if (this.isRxAssistant()) this.tipoIngreso.set('Particular'); 
    if (this.isHospitalAssistant()) this.tipoIngreso.set('Hospitalizacion');
    if (this.isEmergencyAssistant()) this.tipoIngreso.set('Emergencia');

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

  seleccionarTurno(hora: string) {
    this.selectedSlot.set(hora);
    this.horaCita.set(hora);
    this.showScheduleModal.set(false);
  }

  cargarServicio(servId: string) {
    const s = this.serviciosCatalogo().find(x => x.id === servId);
    if (!s) return;

    const pId = this.pacienteId();
    
    // Si no hay paciente, agregar al carrito local (No bloqueante)
    if (pId === null) {
      const yaEnCarrito = this.carritoLocal().some(x => x.id === s.id);
      if (!yaEnCarrito) {
        this.carritoLocal.update(prev => [...prev, s]);
        this.actionMessage.set(`Estudio "${s.descripcion}" añadido al carrito temporal.`);
      } else {
        this.errorMessage.set("Este servicio ya está en el carrito.");
      }
      return;
    }

    // Si hay paciente, cargar directamente al backend
    this.isLoading.set(true);
    const payload: CargarServicioACuentaRequest = {
      pacienteId: pId,
      tipoIngreso: this.tipoIngreso(),
      convenioId: this.convenioId() || undefined,
      servicioId: s.id,
      descripcion: s.descripcion,
      precio: s.precio,
      cantidad: 1,
      tipoServicio: s.tipo,
      usuarioCarga: this.user()?.username || 'admin',
      medicoId: (s.tipo === 'Medico' || s.tipo === 'CONSULTA' || s.tipo === 'Médico') ? this.selectedMedicoId() || undefined : undefined,
      horaCita: (s.tipo === 'Medico' || s.tipo === 'CONSULTA' || s.tipo === 'Médico') ? this.horaCita() : undefined
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

  private resetForm() {
    this.pagos.set([]);
    this.serviciosEnBackend.set([]);
    this.carritoLocal.set([]);
    this.cuentaId.set(null);
    this.pacienteId.set(null);
  }
}
