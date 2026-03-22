import { Component, inject, signal, computed, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { FacturacionService, DetallePagoDto, CargarServicioACuentaRequest, ReceiptPrintData } from '../../../core/services/facturacion.service';
import { AuthService } from '../../../core/services/auth.service';
import { AppointmentsService, Doctor, ScheduleEntry } from '../../../core/services/appointments.service';
import { CatalogService, CatalogItem } from '../../../core/services/catalog.service';
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
  private cajaService = inject(CajaService);
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
  public pacienteId = signal<number | null>(null); // Se unificó a ID numérico del Legacy
  public cuentaId = signal<string | null>(null);
  public tipoIngreso = signal<string>('Particular');
  public convenioId = signal<number | null>(null); // Se unificó a ID numérico del Legacy
  public tasaCambioDia = signal<number>(45.50);

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

  // Catálogo Real Filtrado por Rol (Micro-Ciclo 10)
  public serviciosCatalogo = signal<CatalogItem[]>([]);
  public serviciosFiltradosPorRol = computed(() => {
    const raw = this.serviciosCatalogo();
    if (this.isAdmin()) return raw;
    if (this.isRxAssistant()) return raw.filter((s: CatalogItem) => s.tipo === 'RX');
    return raw;
  });

  // Carrito de Servicios (Servicios ya cargados en la cuenta)
  public serviciosCargados = signal<any[]>([]);

  // Array de Pagos
  public pagos = signal<DetallePagoDto[]>([]);

  // Feedback UI
  public isLoading = signal<boolean>(false);
  public actionMessage = signal<string | null>(null);
  public errorMessage = signal<string | null>(null);

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

  // States for Cash Closing (Micro-Ciclo 12)
  public showClosingModal = signal<boolean>(false);
  public closingReport = signal<DailyClosingReport | null>(null);

  // Nuevo Elemento Form (Pagos)
  public currentPago = {
    metodoPago: 'Punto de Venta Bs',
    referenciaBancaria: '',
    montoAbonadoMoneda: 0
  };

  public metodosDisponibles = ['Punto de Venta Bs', 'Pago Móvil', 'Efectivo Divisas', 'Zelle'];

  // Cierre de Caja Local (Sesión actual)
  public serviciosFacturadosHoy = signal<any[]>([]);
  public totalCierreCaja = computed(() => this.serviciosFacturadosHoy().reduce((acc: number, curr: any) => acc + curr.precio, 0));

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
    }
    this.showResultadosBusqueda.set(false);
    this.busquedaTermino.set(p.cedula);
    this.actionMessage.set(`Paciente seleccionado: ${p.nombre} ${p.apellidos}`);
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

  generarCierreTurno() {
    this.isLoading.set(true);
    const currentUser = this.user()?.username || "Asistente";
    this.cajaService.getPersonalReport(currentUser).subscribe({
      next: (report: DailyClosingReport) => {
        this.isLoading.set(false);
        this.closingReport.set(report);
        this.showClosingModal.set(true);
      },
      error: (err: any) => {
        this.isLoading.set(false);
        this.errorMessage.set("No se pudo generar el reporte de cierre.");
      }
    });
  }

  private refreshCatalog(convenioId?: number | null) {
    this.catalogService.getUnifiedCatalog(convenioId).subscribe({
      next: (items: CatalogItem[]) => {
        this.serviciosCatalogo.set(items);
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
    const pId = this.pacienteId();
    if (!s || pId === null) {
      this.errorMessage.set("Seleccione un paciente antes de cargar servicios.");
      return;
    }

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
        const servicioConPrecioAjustado = { ...s, hora: this.horaCita(), precio: payload.precio };
        this.serviciosCargados.update((prev: any[]) => [...prev, servicioConPrecioAjustado]);
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

  public totalCargado = computed(() => this.serviciosCargados().reduce((acc: number, curr: any) => acc + curr.precio, 0));
  public totalFacturadoBase = computed(() => this.pagos().reduce((acc: number, curr: DetallePagoDto) => acc + curr.equivalenteAbonadoBase, 0));

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
        this.serviciosFacturadosHoy.update((prev: any[]) => [...prev, ...this.serviciosCargados()]);
        
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
    this.serviciosCargados.set([]);
    this.cuentaId.set(null);
    this.pacienteId.set(null);
  }
}
