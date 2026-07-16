import { Component, inject, signal, computed, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AdminBillingService, CuentaAdministrativaDto, CuentaAdministrativaDetailDto } from '../../../core/services/admin-billing.service';
import { FacturacionService, DetallePagoDto, ReceiptPrintData } from '../../../core/services/facturacion.service';
import { PatientService, PatientRecord } from '../../../core/services/patient.service';
import { AuthService } from '../../../core/services/auth.service';
import { PrintService } from '../../../core/services/print.service';
import { ConveniosService } from '../../../core/services/convenios.service';
import { SettingsService } from '../../../core/services/settings.service';
import { environment } from '../../../../environments/environment';
import { Subscription } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { MedicoService, Medico } from '../../../core/services/medico.service';
import { MultiSedeService, AreaClinica } from '../../../core/services/multi-sede.service';
// Wizard components shared with Enfermeria
import { DynamicStepperComponent } from '../../enfermeria/components/dynamic-stepper/dynamic-stepper.component';
import { NursingCartComponent } from '../../enfermeria/components/nursing-cart/nursing-cart.component';
import { StepCatalogSearchComponent } from '../../enfermeria/components/step-panels/step-catalog-search.component';
import { StepDoctorSelectComponent } from '../../enfermeria/components/step-panels/step-doctor-select.component';
import { StepLabRxPriceComponent } from '../../enfermeria/components/step-panels/step-lab-rx-price.component';
import { StepQuantityComponent } from '../../enfermeria/components/step-panels/step-quantity.component';
import { StepConfirmComponent } from '../../enfermeria/components/step-panels/step-confirm.component';
import {
  ServicioCatalogo,
  CartItem,
  StepperMode,
  ItemClassification,
  ITEM_CLASSIFICATIONS,
  classifyService
} from '../../enfermeria/enfermeria.component';
import {
  LucideAngularModule,
  Search,
  RefreshCcw,
  Check,
  X,
  ChevronRight,
  ChevronDown,
  Info,
  Calendar,
  User,
  Clock,
  Plus,
  Trash2,
  Edit,
  DollarSign,
  CreditCard,
  ArrowLeft,
  Activity,
  FileText,
  UserPlus
} from 'lucide-angular';

@Component({
  selector: 'app-cierre-cuenta',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    LucideAngularModule,
    DynamicStepperComponent,
    NursingCartComponent,
    StepCatalogSearchComponent,
    StepDoctorSelectComponent,
    StepLabRxPriceComponent,
    StepQuantityComponent,
    StepConfirmComponent
  ],
  templateUrl: './cierre-cuenta.component.html',
  styleUrl: './cierre-cuenta.component.css'
})
export class CierreCuentaComponent implements OnInit, OnDestroy {
  // Services
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly adminBillingService = inject(AdminBillingService);
  private readonly facturacionService = inject(FacturacionService);
  private readonly patientService = inject(PatientService);
  private readonly authService = inject(AuthService);
  private readonly printService = inject(PrintService);
  private readonly conveniosService = inject(ConveniosService);
  private readonly settingsService = inject(SettingsService);
  private readonly http = inject(HttpClient);
  private readonly medicoService = inject(MedicoService);
  private readonly multiSedeService = inject(MultiSedeService);

  readonly icons = {
    Search,
    RefreshCcw,
    Check,
    X,
    ChevronRight,
    ChevronDown,
    Info,
    Calendar,
    User,
    Clock,
    Plus,
    Trash2,
    Edit,
    DollarSign,
    CreditCard,
    ArrowLeft,
    Activity,
    FileText,
    UserPlus
  };

  // State Signals
  public type = signal<string>('Hospitalizacion'); // 'Hospitalizacion' or 'Emergencia'
  public accounts = signal<CuentaAdministrativaDto[]>([]);
  public patientDetailsMap = signal<Record<string, PatientRecord>>({});
  public searchTerm = signal<string>('');
  public isLoading = signal<boolean>(false);
  public actionMessage = signal<string | null>(null);
  public errorMessage = signal<string | null>(null);

  // Ingresar Paciente Modal State
  public showIngresoModal = signal<boolean>(false);
  public searchIngresoTerm = signal<string>('');
  public patientsEncontrados = signal<PatientRecord[]>([]);
  public isSearchingPatient = signal<boolean>(false);
  public selectedPatientForIngreso = signal<PatientRecord | null>(null);
  public convenioIngresoId = signal<number | null>(null);
  public showNewPatientForm = signal<boolean>(false);
  public selectedCamaId = signal<string | null>(null);
  public camasDisponibles = signal<any[]>([]);

  // Step admission & Triage Signals
  public ingresoStep = signal<number>(1);
  public triageSelectedPatientId = signal<string | null>(null);

  // Triage & Assessment Inputs
  public triageMotivoConsulta = signal<string>('');
  public triageTensionArterial = signal<string>('');
  public triageFrecuenciaCardiaca = signal<number>(80);
  public triageFrecuenciaRespiratoria = signal<number>(18);
  public triageTemperatura = signal<number>(37.0);
  public triageSaturacionO2 = signal<number>(98);
  public triageGlicemiaCapilar = signal<number | null>(null);
  public triageClasificacion = signal<string>('Nivel III (Amarillo)');
  
  public triageEstadoConciencia = signal<string>('Alerta');
  public triageGlasgowOcular = signal<number>(4);
  public triageGlasgowVerbal = signal<number>(5);
  public triageGlasgowMotor = signal<number>(6);
  public triageGlasgowTotal = computed(() => Number(this.triageGlasgowOcular()) + Number(this.triageGlasgowVerbal()) + Number(this.triageGlasgowMotor()));
  
  public triageViaAerea = signal<string>('Permeable');
  public triageVentilacion = signal<string>('Normal');
  public triagePulso = signal<string>('Rítmico');
  public triagePielMucosas = signal<string>('Normocoloreada');
  public triageLlenadoCapilar = signal<string>('< 2 segundos');
  public triagePupilas = signal<string>('Isocóricas');
  
  public triageAlergiasNinguna = signal<boolean>(true);
  public triageAlergiasEspecificar = signal<string>('');
  public triageAccesosVenososTrae = signal<boolean>(false);
  public triageAccesosVenososLugar = signal<string>('');
  public triagePertenencias = signal<string>('Entregadas a familiar');
  public triageAntecedenteHTA = signal<boolean>(false);
  public triageAntecedenteDiabetes = signal<boolean>(false);
  public triageAntecedenteCardiopatia = signal<boolean>(false);

  public newPatientData = {
    cedula: '',
    nombre: '',
    apellidos: '',
    correo: '',
    celular: '',
    telefono: '',
    direccion: '',
    fechaNacimiento: new Date().toISOString().split('T')[0],
    sexo: 'ND',
    tipoCorreo: '@gmail.com',
    codigoCelular: '0414',
    codigoTelefono: '0274'
  };

  get fechaNacimientoFormatted(): string {
    const raw = this.newPatientData.fechaNacimiento;
    if (!raw) return '';
    const datePart = raw.split('T')[0];
    const parts = datePart.split('-');
    if (parts.length === 3 && parts[0].length === 4) {
      return `${parts[2]}-${parts[1]}-${parts[0]}`;
    }
    return raw;
  }

  set fechaNacimientoFormatted(val: string) {
    if (!val) {
      this.newPatientData.fechaNacimiento = '';
      return;
    }
    const cleaned = val.replace(/\//g, '-').trim();
    const parts = cleaned.split('-');
    if (parts.length === 3) {
      if (parts[0].length === 2 && parts[2].length === 4) {
        const dd = parts[0].padStart(2, '0');
        const mm = parts[1].padStart(2, '0');
        const yyyy = parts[2];
        this.newPatientData.fechaNacimiento = `${yyyy}-${mm}-${dd}`;
        return;
      } else if (parts[0].length === 4) {
        this.newPatientData.fechaNacimiento = cleaned;
        return;
      }
    }
    this.newPatientData.fechaNacimiento = val;
  }

  public codigosCelular = ['0416', '0426', '0414', '0424', '0412', '0422'];
  public codigosTelefonoCombinados = ['0274', '0273', '0251', '0212', '0281', '0241', '0416', '0426', '0414', '0424', '0412', '0422'];

  // Selected Account State
  public selectedAccount = signal<CuentaAdministrativaDto | null>(null);
  public estadoFiltro = signal<'Abierta' | 'Facturada'>('Abierta');
  public fechaEgreso = signal<string>(new Date().toISOString().split('T')[0]);
  public horaEgreso = signal<string>(new Date().toTimeString().substring(0, 5));
  public diagnostico = signal<string>('Diagnóstico General / Control Médico');
  public destinoPaciente = signal<string>('Alta Médica');
  public personalRelevo = signal<string>('');
  public mostrarSeccionPago = signal<boolean>(false);
  public mostrarDetalleItems = signal<boolean>(false);

  // Billing Fields
  public tasaCambioDia = signal<number>(36.5);
  public editandoTasa = signal<boolean>(false);
  public tasaEditValue = signal<number>(36.5);

  // Payments List
  public pagos = signal<DetallePagoDto[]>([]);
  public nuevoPagoMetodo = signal<string>('Particular - Tarjeta de Crédito/Débito');
  public nuevoPagoReferencia = signal<string>('');
  public nuevoPagoMontoMoneda = signal<number>(0);

  public metodosPago = [
    { label: '💳 Tarjeta de Crédito/Débito', value: 'Particular - Tarjeta de Crédito/Débito', moneda: 'USD' },
    { label: '💵 Efectivo Divisas', value: 'Particular - Efectivo Divisas', moneda: 'USD' },
    { label: '⚡ Transferencia Nacional (Bs)', value: 'Particular - Transferencia SPEI', moneda: 'VES' },
    { label: '🛡️ Seguro Médico (Convenio)', value: 'Seguro Médico (Convenio)', moneda: 'USD' }
  ];

  // Convenios Catalog
  public convenios = signal<any[]>([]);
  public convenioSeleccionadoId = signal<number | null>(null);
  public seguroCoberturaPorcentaje = signal<number>(80); // Default 80% coverage

  // Roles Helpers for Template (V17.0 Standards)
  public canCollectAndClose = computed(() => {
    return this.authService.isCajero() || this.authService.isAdmin();
  });

  public isEmergencyNurse = computed(() => {
    return this.authService.isEmergencyAssistant() && !this.authService.isCajero();
  });

  public isAdmin = computed(() => {
    return this.authService.isAdmin();
  });

  // Clinical Item Loading — Wizard (shared with Enfermeria)
  public servicesCatalog = signal<ServicioCatalogo[]>([]);
  public fastChargeSearchTerm = signal<string>('');
  public filteredServices = signal<ServicioCatalogo[]>([]);
  public selectedService = signal<ServicioCatalogo | null>(null);
  public fastChargeQuantity = 1;
  public isSavingFastCharge = signal<boolean>(false);
  public triageHistoryMap = signal<Record<string, any[]>>({});
  // Medicos Catalog
  public medicos = signal<Medico[]>([]);
  public medicoTratanteId = signal<string | null>(null);
  // Wizard signals
  public selectedMedicoId = signal<string | null>(null);
  public selectedAreaClinicaId = signal<string | null>(null);
  public areasClinicas = signal<AreaClinica[]>([]);
  public customPrecio = signal<number | null>(null);
  public customHonorario = signal<number | null>(null);
  public activeSuggestions = signal<ServicioCatalogo[]>([]);
  public selectedSuggestions = signal<Record<string, boolean>>({});
  public currentStep = signal<number>(1);
  public activeStepperMode = signal<StepperMode>('catalog');
  public cartItems = signal<CartItem[]>([]);

  // Computed: cart total
  public cartTotalUSD = computed(() =>
    this.cartItems().reduce((acc, item) => acc + (item.precioBase + item.honorario) * item.cantidad, 0)
  );
  public cartItemCount = computed(() => this.cartItems().length);

  // Computed: item classification delegated to shared rules engine
  public itemClassification = computed<ItemClassification>(() => classifyService(this.selectedService()));

  // Computed: medicos filtered by service specialty
  public medicosFiltrados = computed(() => {
    const service = this.selectedService();
    const allMedicos = this.medicos();
    if (!service) return allMedicos;
    const descUpper = (service.descripcion || '').toUpperCase();
    const tipoUpper = (service.tipo || '').toUpperCase();
    const filtered = allMedicos.filter(m => {
      if (!m.especialidad) return false;
      const espClean = m.especialidad.trim().toUpperCase();
      const prefix = espClean.substring(0, 5);
      if (prefix.length < 3) return false;
      return descUpper.includes(prefix) || tipoUpper.includes(prefix);
    });
    return filtered.length > 0 ? filtered : allMedicos;
  });

  // Computed: precio final (base + honorario) por item seleccionado
  public precioFinalCalculado = computed<number>(() => {
    const s = this.selectedService();
    if (!s) return 0;
    const classification = this.itemClassification();
    const customPriceVal = this.customPrecio();
    const pureBasePrice = s.precioUsd ?? 0;
    const basePrice = customPriceVal ?? pureBasePrice;
    if (classification === ITEM_CLASSIFICATIONS.CONSULTA) {
      const customHonoraryVal = this.customHonorario();
      const honorario = customHonoraryVal ?? s.honorarioBase ?? 0;
      return basePrice + honorario;
    }
    if (classification === ITEM_CLASSIFICATIONS.LABORATORIO || classification === ITEM_CLASSIFICATIONS.RX) {
      return basePrice;
    }
    const qty = this.fastChargeQuantity || 1;
    return basePrice * qty;
  });

  private paramSub!: Subscription;

  // Computed Properties for the Patient Directory (Antesala)
  public filteredAccounts = computed(() => {
    const term = this.searchTerm().toLowerCase().trim();
    const list = this.accounts();
    if (!term) return list;
    return list.filter(acc => 
      acc.pacienteNombre.toLowerCase().includes(term) || 
      acc.pacienteCedula.toLowerCase().includes(term)
    );
  });

  public enrichedAccounts = computed(() => {
    const list = this.filteredAccounts();
    return list.map((acc, index) => {
      // Determinamos iniciales del paciente
      const names = acc.pacienteNombre.trim().split(/\s+/);
      const initials = names.map(n => n.charAt(0)).join('').substring(0, 2).toUpperCase();
      
      // Estado clínico, habitación/cama deterministas a partir de la cédula para consistencia visual
      const seed = acc.pacienteCedula.split('').reduce((sum, char) => sum + char.charCodeAt(0), 0) || index;
      const statuses = ['Crítico', 'Estable', 'Observación'];
      const status = statuses[seed % statuses.length];
      
      const roomType = this.type() === 'Hospitalizacion' ? 'Hab.' : 'Box';
      const room = acc.areaClinicaNombre || `${roomType} ${100 + (seed % 15)}${String.fromCharCode(65 + (seed % 3))}`;
      
      let statusClass = 'text-rose-500 bg-rose-500/10 border border-rose-500/20';
      if (status === 'Estable') {
        statusClass = 'text-emerald-400 bg-emerald-500/10 border border-emerald-500/20';
      } else if (status === 'Observación') {
        statusClass = 'text-amber-500 bg-amber-500/10 border border-amber-500/20';
      }

      return {
        ...acc,
        initials,
        status,
        room,
        statusClass
      };
    });
  });

  public capacidadUnidad = computed(() => {
    const activeCount = this.accounts().length;
    return activeCount === 0 ? 0 : Math.min(95, 60 + activeCount * 4);
  });

  public estanciaPromedio = computed(() => {
    const list = this.accounts();
    if (list.length === 0) return 0;
    let totalDays = 0;
    const now = new Date();
    list.forEach(acc => {
      const ingreso = new Date(acc.fechaCarga);
      const diff = now.getTime() - ingreso.getTime();
      const days = Math.ceil(diff / (1000 * 60 * 60 * 24));
      totalDays += days <= 0 ? 1 : days;
    });
    return Math.round((totalDays / list.length) * 10) / 10;
  });

  public calcularEdad(fechaNacimiento: string | null | undefined): string {
    if (!fechaNacimiento) return '—';
    const nacimiento = new Date(fechaNacimiento);
    if (isNaN(nacimiento.getTime())) return '—';
    
    const hoy = new Date();
    let años = hoy.getFullYear() - nacimiento.getFullYear();
    let meses = hoy.getMonth() - nacimiento.getMonth();
    let dias = hoy.getDate() - nacimiento.getDate();
    
    if (dias < 0) {
      meses--;
      const mesAnterior = new Date(hoy.getFullYear(), hoy.getMonth(), 0);
      dias += mesAnterior.getDate();
    }
    if (meses < 0) {
      años--;
      meses += 12;
    }
    
    if (años > 0) return `${años} ${años === 1 ? 'año' : 'años'}`;
    if (meses > 0) return `${meses} ${meses === 1 ? 'mes' : 'meses'}`;
    return `${dias} ${dias === 1 ? 'día' : 'días'}`;
  }

  public alertasCriticosCount = computed(() => {
    return this.enrichedAccounts().filter(a => a.status === 'Crítico').length;
  });

  public recentActivities = computed(() => {
    const list = this.accounts();
    const triageMap = this.triageHistoryMap();
    const activities: any[] = [];

    list.forEach(acc => {
      const name = acc.pacienteNombre;
      const cedula = acc.pacienteCedula;
      const cId = acc.cuentaId;

      // 1. Evento de Admisión
      if (acc.fechaCarga) {
        const date = new Date(acc.fechaCarga);
        activities.push({
          timestamp: date,
          time: this.formatTime(date),
          source: 'SISTEMA',
          message: `Paciente ${name} (Cédula: ${cedula}) ingresado a la sección de ${acc.tipoIngreso}.`,
          badge: 'Ingreso',
          badgeClass: 'bg-blue-500/10 text-blue-400 border border-blue-500/20'
        });
      }

      // 2. Eventos de Servicios/Medicamentos Cargados
      if (acc.detalles && acc.detalles.length > 0) {
        acc.detalles.forEach(item => {
          const date = item.fechaCarga ? new Date(item.fechaCarga) : new Date();
          activities.push({
            timestamp: date,
            time: this.formatTime(date),
            source: 'ENFERMERÍA',
            message: `Servicio de ${item.descripcion} (Cantidad: ${item.cantidad}) cargado a la cuenta de ${name}.`,
            badge: 'Servicio',
            badgeClass: 'bg-indigo-500/10 text-indigo-400 border border-indigo-500/20'
          });
        });
      }

      // 3. Eventos de Triage e Historial Clínico Real
      const triages = triageMap[cId] || [];
      triages.forEach(tr => {
        const rawDate = tr.fechaRegistro || tr.FechaRegistro || acc.fechaCarga;
        const date = new Date(rawDate);
        const user = tr.usuarioRegistro || tr.UsuarioRegistro || 'sistema';
        
        // Registro de Triage Genuino
        activities.push({
          timestamp: date,
          time: this.formatTime(date),
          source: 'ENFERMERÍA',
          message: `Valoración física y triage inicial registrados por ${user} para ${name}.`,
          badge: 'Triage',
          badgeClass: 'bg-emerald-500/10 text-emerald-400 border border-emerald-500/20'
        });

        // Alerta de Frecuencia Cardiaca
        const fc = tr.frecuenciaCardiaca !== undefined ? tr.frecuenciaCardiaca : tr.FrecuenciaCardiaca;
        if (fc !== undefined && fc > 0) {
          if (fc > 100 || fc < 60) {
            activities.push({
              timestamp: new Date(date.getTime() + 1000), // Desfasado para ordenación
              time: this.formatTime(date),
              source: 'SISTEMA',
              message: `ALERTA: Frecuencia cardíaca fuera de rango (${fc} lpm) para paciente ${name} (ID: ${cedula}).`,
              badge: 'Alerta',
              badgeClass: 'bg-rose-500/10 text-rose-500 border border-rose-500/20'
            });
          }
        }

        // Alerta de Temperatura
        const temp = tr.temperatura !== undefined ? tr.temperatura : tr.Temperatura;
        if (temp !== undefined && temp > 0) {
          if (temp > 37.8 || temp < 35.5) {
            activities.push({
              timestamp: new Date(date.getTime() + 2000),
              time: this.formatTime(date),
              source: 'SISTEMA',
              message: `ALERTA: Temperatura corporal fuera de rango (${temp} °C) para paciente ${name} (ID: ${cedula}).`,
              badge: 'Alerta',
              badgeClass: 'bg-rose-500/10 text-rose-500 border border-rose-500/20'
            });
          }
        }

        // Alerta de Saturación de Oxígeno
        const sat = tr.saturacionO2 !== undefined ? tr.saturacionO2 : tr.SaturacionO2;
        if (sat !== undefined && sat > 0 && sat < 95) {
          activities.push({
            timestamp: new Date(date.getTime() + 3000),
            time: this.formatTime(date),
            source: 'SISTEMA',
            message: `ALERTA: Saturación de O2 baja (${sat}%) para paciente ${name} (ID: ${cedula}).`,
            badge: 'Alerta',
            badgeClass: 'bg-rose-500/10 text-rose-500 border border-rose-500/20'
          });
        }

        // Alerta de Escala de Glasgow
        const glasgow = tr.glasgowTotal !== undefined ? tr.glasgowTotal : tr.GlasgowTotal;
        if (glasgow !== undefined && glasgow > 0 && glasgow < 15) {
          activities.push({
            timestamp: new Date(date.getTime() + 4000),
            time: this.formatTime(date),
            source: 'SISTEMA',
            message: `ALERTA: Compromiso neurológico detectado (Glasgow: ${glasgow}/15) para paciente ${name} (ID: ${cedula}).`,
            badge: 'Alerta',
            badgeClass: 'bg-rose-500/10 text-rose-500 border border-rose-500/20'
          });
        }
      });
    });

    // Ordenar por fecha decreciente
    activities.sort((a, b) => b.timestamp.getTime() - a.timestamp.getTime());

    // Si está vacío, mostrar mensaje de información por defecto
    if (activities.length === 0) {
      activities.push({
        timestamp: new Date(),
        time: this.formatTime(new Date()),
        source: 'SISTEMA',
        message: `No hay pacientes activos en la sección de ${this.type()} en este momento.`,
        badge: 'Info',
        badgeClass: 'bg-blue-500/10 text-blue-400 border border-blue-500/20'
      });
    }

    return activities;
  });

  private formatTime(date: Date): string {
    return date.toLocaleTimeString('en-US', { hour: '2-digit', minute: '2-digit', second: '2-digit' });
  }

  // Calculations
  public subtotalServicios = computed(() => {
    const acc = this.selectedAccount();
    if (!acc) return 0;
    return acc.total;
  });

  public totalGeneral = computed(() => {
    return this.subtotalServicios();
  });

  public coberturaSeguro = computed(() => {
    const isInsurance = this.nuevoPagoMetodo() === 'Seguro Médico (Convenio)';
    if (!isInsurance) return 0;
    const total = this.totalGeneral();
    const pct = this.seguroCoberturaPorcentaje() / 100;
    return Math.round(total * pct * 100) / 100;
  });

  public totalAPagarPaciente = computed(() => {
    const total = this.totalGeneral();
    const cob = this.coberturaSeguro();
    // Restamos la cobertura del seguro
    const neto = total - cob;
    // Restamos cualquier abono que ya haya ingresado en el listado de pagos locales
    const abonos = this.pagos().reduce((acc, p) => acc + p.equivalenteAbonadoBase, 0);
    return Math.max(0, neto - abonos);
  });

  // Calculate Estancia Days
  public diasEstancia = computed(() => {
    const acc = this.selectedAccount();
    if (!acc) return 1;
    const ingreso = new Date(acc.fechaCarga);
    const egreso = new Date(`${this.fechaEgreso()}T${this.horaEgreso()}:00`);
    const diff = egreso.getTime() - ingreso.getTime();
    const days = Math.ceil(diff / (1000 * 60 * 60 * 24));
    return days <= 0 ? 1 : days;
  });

  ngOnInit() {
    // 1. Cargar Tasa de Cambio Inicial
    this.settingsService.getTasa().subscribe({
      next: (res) => this.tasaCambioDia.set(res.monto),
      error: () => this.tasaCambioDia.set(36.5)
    });

    // 2. Cargar Convenios para dropdowns
    this.conveniosService.getAll().subscribe({
      next: (res) => this.convenios.set(res),
      error: (err) => console.error('[CIERRE-CUENTA] Error convenios:', err)
    });

    // 3. Suscribirse a los parámetros de la ruta para detectar el tipo de ingreso
    this.paramSub = this.route.params.subscribe(params => {
      const routeType = params['type'];
      if (routeType === 'Hospitalizacion' || routeType === 'Emergencia') {
        this.type.set(routeType);
      } else {
        this.type.set('Hospitalizacion'); // Default
      }
      this.selectedAccount.set(null); // Resetear selección al cambiar de área
      this.loadOpenAccounts();
    });

    // 3.1 Suscribirse a los queryParams para auto-seleccionar paciente
    this.route.queryParams.subscribe(queryParams => {
      const cedula = queryParams['cedula'];
      if (cedula) {
        this.searchTerm.set(cedula);
      }
    });

    // 4. Cargar catálogo unificado de servicios/medicamentos
    this.http.get<any[]>(`${environment.apiUrl}/api/Catalog/unified`)
      .subscribe({
        next: (res) => {
          this.servicesCatalog.set(res);
        },
        error: (err) => console.error('[CIERRE-CUENTA] Error al cargar catálogo unificado:', err)
      });

    // 5. Cargar catálogo de médicos activos
    this.medicoService.getAll().subscribe({
      next: (res) => this.medicos.set(res.filter(m => m.activo)),
      error: (err) => console.error('[CIERRE-CUENTA] Error al cargar médicos:', err)
    });

    // 6. Cargar áreas clínicas (camas)
    this.loadCamasDisponibles();
  }

  public loadCamasDisponibles() {
    this.http.get<any[]>(`${environment.apiUrl}/api/AreaClinica/monitoreo`).subscribe({
      next: (res) => {
        // Almacenar todas las camas y filtrar las disponibles
        const libres = res.filter((c: any) => c.estado === 'Disponible');
        this.camasDisponibles.set(libres);
      },
      error: (err) => console.error('[CIERRE-CUENTA] Error al cargar áreas clínicas/camas:', err)
    });
  }

  ngOnDestroy() {
    if (this.paramSub) {
      this.paramSub.unsubscribe();
    }
  }

  // Cargar cuentas abiertas del tipo actual
  public loadOpenAccounts() {
    this.isLoading.set(true);
    this.adminBillingService.getCuentasAdministrativas(undefined, this.type(), this.estadoFiltro()).subscribe({
      next: (res) => {
        this.accounts.set(res);
        this.isLoading.set(false);

        // Auto re-seleccionar la cuenta activa actual para ver cambios (como nuevos items cargados)
        const currentSel = this.selectedAccount();
        if (currentSel) {
          const updated = res.find(acc => acc.cuentaId === currentSel.cuentaId);
          if (updated) {
            this.selectedAccount.set(updated);
          } else {
            this.selectedAccount.set(null); // Si ya se cerró/no está, deseleccionar
          }
        } else {
          // Auto-seleccionar cuenta basada en queryParam de cédula
          const cedulaParam = this.route.snapshot.queryParams['cedula'];
          if (cedulaParam) {
            const matchedAcc = res.find(acc => acc.pacienteCedula === cedulaParam);
            if (matchedAcc) {
              this.selectAccount(matchedAcc);
            }
          }
        }

        // Cargar detalles de pacientes y triage en paralelo
        res.forEach(acc => {
          this.patientService.searchPatients(acc.pacienteCedula).subscribe({
            next: (patients) => {
              const match = patients.find(p => p.id === acc.pacienteId);
              if (match) {
                this.patientDetailsMap.update(map => {
                  map[acc.pacienteId] = match;
                  return { ...map };
                });
              }
            }
          });

          // Cargar triage del backend para alertas reales
          this.http.get<any[]>(`${environment.apiUrl}/api/Enfermeria/TriageHistorial/${acc.cuentaId}`).subscribe({
            next: (history) => {
              this.triageHistoryMap.update(map => {
                map[acc.cuentaId] = history;
                return { ...map };
              });
            },
            error: (err) => console.error(`[CIERRE-CUENTA] Error al cargar triage de cuenta ${acc.cuentaId}:`, err)
          });
        });
      },
      error: (err) => {
        console.error('[CIERRE-CUENTA] Error al cargar cuentas:', err);
        this.errorMessage.set('Error al cargar la lista de pacientes activos.');
        this.isLoading.set(false);
      }
    });
  }

  // Seleccionar un paciente para ir al Cierre de Cuenta
  public selectAccount(acc: CuentaAdministrativaDto) {
    this.selectedAccount.set(acc);
    this.fechaEgreso.set(new Date().toISOString().split('T')[0]);
    this.horaEgreso.set(new Date().toTimeString().substring(0, 5));
    this.pagos.set([]); // Limpiar pagos temporales
    this.diagnostico.set('Diagnóstico General / Control Médico');
    this.destinoPaciente.set('Alta Médica');
    this.personalRelevo.set('');
    this.medicoTratanteId.set(null); // Resetear selección de médico
    this.mostrarSeccionPago.set(false);
    this.mostrarDetalleItems.set(false);
    
    // Si la cuenta ya tiene convenio pre-configurado
    if (acc.convenioId) {
      this.convenioSeleccionadoId.set(acc.convenioId);
      this.nuevoPagoMetodo.set('Seguro Médico (Convenio)');
    } else {
      this.convenioSeleccionadoId.set(null);
      this.nuevoPagoMetodo.set('Particular - Tarjeta de Crédito/Débito');
    }
  }

  // Volver a la Antesala (Directorio de Pacientes)
  public deselectAccount() {
    this.selectedAccount.set(null);
    this.mostrarSeccionPago.set(false);
    this.mostrarDetalleItems.set(false);
    this.loadOpenAccounts();
  }

  // Mock Blood Type helper based on patient ID to visual match the reference mockup
  public getMockBloodType(pacienteId: string): string {
    const types = ['O+', 'A-', 'AB+', 'B+', 'O-', 'A+'];
    const idx = pacienteId.split('').reduce((acc, char) => acc + char.charCodeAt(0), 0) % types.length;
    return types[idx];
  }

  // Helper calculation of age
  public getPatientAge(fechaNacStr?: string): string {
    if (!fechaNacStr) return '42 años'; // Fallback
    const birth = new Date(fechaNacStr);
    const ageDifMs = Date.now() - birth.getTime();
    const ageDate = new Date(ageDifMs);
    const age = Math.abs(ageDate.getUTCFullYear() - 1970);
    const dateStr = birth.toLocaleDateString('es-ES', { day: '2-digit', month: '2-digit', year: '2-digit' });
    return `${age} años (FN: ${dateStr})`;
  }

  // Tasa de Cambio Handlers
  public editarTasa() {
    this.tasaEditValue.set(this.tasaCambioDia());
    this.editandoTasa.set(true);
  }

  public guardarTasa() {
    if (this.tasaEditValue() <= 0) {
      this.errorMessage.set('La tasa de cambio debe ser mayor a cero.');
      return;
    }
    this.settingsService.updateTasa(this.tasaEditValue()).subscribe({
      next: () => {
        this.tasaCambioDia.set(this.tasaEditValue());
        this.editandoTasa.set(false);
        this.actionMessage.set('Tasa de cambio actualizada con éxito.');
        setTimeout(() => this.actionMessage.set(null), 3000);
      },
      error: () => this.errorMessage.set('Error al actualizar la tasa de cambio.')
    });
  }

  // Payments Management
  public agregarPago() {
    const monto = this.nuevoPagoMontoMoneda();
    if (monto <= 0) {
      this.errorMessage.set('Ingrese un monto de pago mayor a cero.');
      return;
    }

    const metodoInfo = this.metodosPago.find(m => m.value === this.nuevoPagoMetodo());
    const esBs = metodoInfo?.moneda === 'VES';
    const tasa = this.tasaCambioDia();
    const equiv = esBs ? Math.round(monto / tasa * 100) / 100 : monto;

    const nuevoPago: DetallePagoDto = {
      metodoPago: this.nuevoPagoMetodo(),
      referenciaBancaria: this.nuevoPagoReferencia().trim() || 'EFECTIVO/SIN REF',
      montoAbonadoMoneda: monto,
      equivalenteAbonadoBase: equiv
    };

    this.pagos.update(list => [...list, nuevoPago]);
    this.nuevoPagoReferencia.set('');
    this.nuevoPagoMontoMoneda.set(0);
    this.errorMessage.set(null);
  }

  public eliminarPago(index: number) {
    this.pagos.update(list => list.filter((_, i) => i !== index));
  }

  // Administracion de Detalles de la Cuenta
  public selectedDetailToEdit = signal<CuentaAdministrativaDetailDto | null>(null);
  public editPrecio = 0;
  public editHonorario = 0;
  public editCantidad = 1;

  public iniciarEdicionDetalle(item: CuentaAdministrativaDetailDto) {
    this.selectedDetailToEdit.set(item);
    this.editPrecio = item.precio;
    this.editHonorario = item.honorario;
    this.editCantidad = item.cantidad;
  }

  public cancelarEdicionDetalle() {
    this.selectedDetailToEdit.set(null);
  }

  public guardarEdicionDetalle() {
    const detail = this.selectedDetailToEdit();
    const account = this.selectedAccount();
    if (!detail || !account) return;

    this.isLoading.set(true);
    this.errorMessage.set(null);

    const command = {
      cuentaId: account.cuentaId,
      correccionesPrecios: [
        {
          detalleId: detail.id,
          nuevoPrecio: Number(this.editPrecio),
          nuevoHonorario: Number(this.editHonorario),
          nuevaCantidad: Number(this.editCantidad)
        }
      ]
    };

    this.adminBillingService.updateCuentaAdministrativa(command as any).subscribe({
      next: () => {
        this.actionMessage.set('Servicio modificado administrativamente con éxito.');
        this.selectedDetailToEdit.set(null);
        this.isLoading.set(false);
        this.loadOpenAccounts(); // Refrescar cuenta para ver cambios
        setTimeout(() => this.actionMessage.set(null), 3000);
      },
      error: (err: any) => {
        console.error('[CIERRE-CUENTA] Error al modificar servicio:', err);
        this.errorMessage.set(err.error?.Error || err.error?.error || 'Error al modificar el servicio.');
        this.isLoading.set(false);
      }
    });
  }

  public anularDetalleServicio(item: CuentaAdministrativaDetailDto) {
    const account = this.selectedAccount();
    if (!account) return;

    const confirmDel = confirm(`¿Está seguro de que desea anular el servicio "${item.descripcion}" de la cuenta?`);
    if (!confirmDel) return;

    this.isLoading.set(true);
    this.errorMessage.set(null);

    this.facturacionService.quitarServicio(account.cuentaId, item.id).subscribe({
      next: () => {
        this.actionMessage.set('Servicio anulado y removido de la cuenta con éxito.');
        this.isLoading.set(false);
        this.loadOpenAccounts(); // Refrescar cuenta para ver cambios
        setTimeout(() => this.actionMessage.set(null), 3000);
      },
      error: (err: any) => {
        console.error('[CIERRE-CUENTA] Error al anular servicio:', err);
        this.errorMessage.set(err.error?.Error || err.error?.error || 'Error al anular el servicio.');
        this.isLoading.set(false);
      }
    });
  }

  public toggleCortesiaDetalle(item: CuentaAdministrativaDetailDto, incluido: boolean) {
    this.isLoading.set(true);
    this.errorMessage.set(null);
    this.adminBillingService.actualizarCortesiaDetalle(item.id, incluido).subscribe({
      next: () => {
        this.actionMessage.set(`Servicio "${item.descripcion}" actualizado: Cortesía ${incluido ? 'ACTIVADA' : 'DESACTIVADA'}.`);
        this.isLoading.set(false);
        this.loadOpenAccounts();
        setTimeout(() => this.actionMessage.set(null), 3000);
      },
      error: (err: any) => {
        console.error('[CIERRE-CUENTA] Error al cambiar cortesía:', err);
        this.errorMessage.set(err.error?.Error || err.error?.error || 'Error al actualizar cortesía.');
        this.isLoading.set(false);
      }
    });
  }

  public revertirCheckOutActual() {
    const account = this.selectedAccount();
    if (!account) return;

    const confirmRev = confirm(`¿Está seguro de que desea revertir el Check-Out / Alta de la cuenta de "${account.pacienteNombre}"? Esto reabrirá la cuenta y restablecerá la cama como ocupada.`);
    if (!confirmRev) return;

    this.isLoading.set(true);
    this.errorMessage.set(null);
    this.adminBillingService.revertirCheckOut(account.cuentaId).subscribe({
      next: () => {
        this.actionMessage.set('Check-Out revertido con éxito. Cuenta reabierta y cama restablecida.');
        this.isLoading.set(false);
        this.estadoFiltro.set('Abierta'); // Cambiar filtro para mostrar la cuenta abierta
        this.loadOpenAccounts();
        this.deselectAccount();
        setTimeout(() => this.actionMessage.set(null), 3000);
      },
      error: (err: any) => {
        console.error('[CIERRE-CUENTA] Error al revertir check-out:', err);
        this.errorMessage.set(err.error?.Error || err.error?.error || 'Error al revertir check-out.');
        this.isLoading.set(false);
      }
    });
  }

  public devolverInsumoQuirofano(item: CuentaAdministrativaDetailDto) {
    const account = this.selectedAccount();
    if (!account) return;

    const cantStr = prompt(`Ingrese la cantidad a devolver para "${item.descripcion}" (Máximo asignado: ${item.cantidad}):`, '1');
    if (cantStr === null) return;
    const cant = Number(cantStr);
    if (isNaN(cant) || cant <= 0 || cant > item.cantidad) {
      alert('Cantidad inválida.');
      return;
    }

    this.isLoading.set(true);
    this.errorMessage.set(null);
    this.adminBillingService.devolverInsumoCirugia(account.cuentaId, item.servicioId, cant).subscribe({
      next: () => {
        this.actionMessage.set(`Se devolvieron ${cant} unidades de "${item.descripcion}" con éxito.`);
        this.isLoading.set(false);
        this.loadOpenAccounts();
        setTimeout(() => this.actionMessage.set(null), 3000);
      },
      error: (err: any) => {
        console.error('[CIERRE-CUENTA] Error al devolver insumo:', err);
        this.errorMessage.set(err.error?.Error || err.error?.error || 'Error al procesar devolución de Quirófano.');
        this.isLoading.set(false);
      }
    });
  }

  // Print Emergency Release Report
  public imprimirReporteEgreso() {
    const acc = this.selectedAccount();
    if (!acc) return;

    const triages = this.triageHistoryMap()[acc.cuentaId] || [];
    const patientData = {
      diagnostico: this.diagnostico()
    };
    const loggedInUser = this.authService.currentUser()?.username || 'user_emergencia';

    const content = this.printService.generateDischargeReportHtml(
      patientData,
      acc,
      triages,
      this.destinoPaciente(),
      this.personalRelevo(),
      loggedInUser
    );

    this.printService.print(content, `Reporte_Egreso_${acc.pacienteCedula}`);
  }

  // Close Account Execution
  public procesarCierre() {
    const acc = this.selectedAccount();
    if (!acc) return;

    // Si es seguro convenio, inyectamos el pago del seguro
    const finalPayments = [...this.pagos()];
    if (this.nuevoPagoMetodo() === 'Seguro Médico (Convenio)') {
      const cob = this.coberturaSeguro();
      if (cob > 0) {
        finalPayments.push({
          metodoPago: `Seguro Médico (${acc.seguroNombre || 'Convenio'})`,
          referenciaBancaria: `COV-${acc.pacienteCedula}`,
          montoAbonadoMoneda: cob,
          equivalenteAbonadoBase: cob
        });
      }
    }

    // Si queda saldo pendiente por pagar por el paciente, alertamos o permitimos cerrarlo como Cuenta por Cobrar (solo para Hospitalización)
    const pendiente = this.totalGeneral() - finalPayments.reduce((acc, p) => acc + p.equivalenteAbonadoBase, 0);

    if (this.type() !== 'Emergencia' && pendiente > 0.01) {
      const confirmAR = confirm(`Queda un saldo pendiente de $${pendiente.toFixed(2)} USD. La cuenta se cerrará y la diferencia se registrará en Cuentas por Cobrar (CxC). ¿Desea continuar?`);
      if (!confirmAR) return;
    }

    this.isLoading.set(true);

    const currentUser = this.authService.currentUser();
    const request = {
      cuentaId: acc.cuentaId,
      usuarioCajero: currentUser?.username || 'admin',
      usuarioId: currentUser?.id || '',
      tasaCambio: this.tasaCambioDia(),
      destinoPaciente: this.destinoPaciente(),
      personalRelevo: this.personalRelevo(),
      pagos: finalPayments
    };

    this.facturacionService.closeAccount(request).subscribe({
      next: (res: any) => {
        const dest = this.type() === 'Emergencia' ? ` con destino a ${this.destinoPaciente().toUpperCase()}` : '';
        this.actionMessage.set(`¡Cuenta de ${acc.pacienteNombre} cerrada y facturada con éxito${dest}!`);
        this.isLoading.set(false);
        
        // Descarga/Impresión del recibo generado
        const reciboId = res.reciboId || res.id;
        if (reciboId) {
          const downloadUrl = `${environment.apiUrl}/api/ReciboFactura/${reciboId}/Download`;
          window.open(downloadUrl, '_blank');
        }

        setTimeout(() => {
          this.actionMessage.set(null);
          this.deselectAccount();
        }, 3000);
      },
      error: (err) => {
        console.error('[CIERRE-CUENTA] Error al cerrar cuenta:', err);
        this.errorMessage.set(err.error?.Error || err.error?.error || 'Error al procesar el cierre de la cuenta.');
        this.isLoading.set(false);
      }
    });
  }

  // --- Ingresar Paciente / Abrir Cuenta Clinica Flow ---
  public abrirModalIngreso() {
    this.searchIngresoTerm.set('');
    this.patientsEncontrados.set([]);
    this.selectedPatientForIngreso.set(null);
    this.convenioIngresoId.set(null);
    this.showNewPatientForm.set(false);
    this.selectedCamaId.set(null);
    this.loadCamasDisponibles();
    this.newPatientData = {
      cedula: '',
      nombre: '',
      apellidos: '',
      correo: '',
      celular: '',
      telefono: '',
      direccion: '',
      fechaNacimiento: new Date().toISOString().split('T')[0],
      sexo: 'ND',
      tipoCorreo: '@gmail.com',
      codigoCelular: '0414',
      codigoTelefono: '0274'
    };
    this.errorMessage.set(null);
    
    // Reset triage signals
    this.ingresoStep.set(1);
    this.triageSelectedPatientId.set(null);
    this.triageMotivoConsulta.set('');
    this.triageTensionArterial.set('');
    this.triageFrecuenciaCardiaca.set(80);
    this.triageFrecuenciaRespiratoria.set(18);
    this.triageTemperatura.set(37.0);
    this.triageSaturacionO2.set(98);
    this.triageGlicemiaCapilar.set(null);
    this.triageClasificacion.set('Nivel III (Amarillo)');
    this.triageEstadoConciencia.set('Alerta');
    this.triageGlasgowOcular.set(4);
    this.triageGlasgowVerbal.set(5);
    this.triageGlasgowMotor.set(6);
    this.triageViaAerea.set('Permeable');
    this.triageVentilacion.set('Normal');
    this.triagePulso.set('Rítmico');
    this.triagePielMucosas.set('Normocoloreada');
    this.triageLlenadoCapilar.set('< 2 segundos');
    this.triagePupilas.set('Isocóricas');
    this.triageAlergiasNinguna.set(true);
    this.triageAlergiasEspecificar.set('');
    this.triageAccesosVenososTrae.set(false);
    this.triageAccesosVenososLugar.set('');
    this.triagePertenencias.set('Entregadas a familiar');
    this.triageAntecedenteHTA.set(false);
    this.triageAntecedenteDiabetes.set(false);
    this.triageAntecedenteCardiopatia.set(false);

    this.showIngresoModal.set(true);
  }

  public buscarPacienteIngreso() {
    const term = this.searchIngresoTerm().trim();
    if (term.length >= 3) {
      this.isSearchingPatient.set(true);
      this.patientService.searchPatients(term).subscribe({
        next: (res) => {
          this.patientsEncontrados.set(res);
          this.isSearchingPatient.set(false);
        },
        error: () => {
          this.isSearchingPatient.set(false);
        }
      });
    }
  }

  public seleccionarPacienteIngreso(p: PatientRecord) {
    this.selectedPatientForIngreso.set(p);
    this.patientsEncontrados.set([]);
  }

  public deseleccionarPacienteIngreso() {
    this.selectedPatientForIngreso.set(null);
  }

  public procesarIngreso() {
    this.errorMessage.set(null);

    if (this.showNewPatientForm()) {
      // Registrar nuevo paciente primero
      if (!this.newPatientData.cedula || 
          !this.newPatientData.nombre || 
          !this.newPatientData.apellidos || 
          !this.newPatientData.fechaNacimiento || 
          !this.newPatientData.celular || 
          !this.newPatientData.direccion) {
        this.errorMessage.set("Todos los campos marcados con (*) son obligatorios: Cédula, Nombres, Apellidos, Fecha de Nacimiento, Celular y Dirección.");
        return;
      }

      this.isLoading.set(true);
      this.patientService.createPatient(this.newPatientData).subscribe({
        next: (p: PatientRecord) => {
          if (this.type() === 'Emergencia') {
            this.isLoading.set(false);
            this.triageSelectedPatientId.set(p.id);
            this.ingresoStep.set(2);
          } else {
            this.abrirCuentaParaPaciente(p.id);
          }
        },
        error: (err: any) => {
          this.isLoading.set(false);
          this.errorMessage.set(err.error?.message || err.error?.error || "Error al registrar nuevo paciente.");
        }
      });
    } else {
      // Usar paciente existente
      const patient = this.selectedPatientForIngreso();
      if (!patient) {
        this.errorMessage.set("Por favor busque y seleccione un paciente o complete el formulario de nuevo registro.");
        return;
      }

      if (this.type() === 'Emergencia') {
        this.triageSelectedPatientId.set(patient.id);
        this.ingresoStep.set(2);
      } else {
        this.isLoading.set(true);
        this.abrirCuentaParaPaciente(patient.id);
      }
    }
  }

  private abrirCuentaParaPaciente(pacienteId: string) {
    this.facturacionService.abrirCuenta(pacienteId, this.type(), this.convenioIngresoId(), this.selectedCamaId()).subscribe({
      next: () => {
        this.isLoading.set(false);
        this.showIngresoModal.set(false);
        this.actionMessage.set(`Paciente ingresado exitosamente a la sección de ${this.type()}.`);
        setTimeout(() => this.actionMessage.set(null), 5000);
        this.loadOpenAccounts(); // Refrescar lista de activos
      },
      error: (err: any) => {
        this.isLoading.set(false);
        this.errorMessage.set(err.error?.Error || err.error?.message || "Error al abrir la cuenta clínica.");
      }
    });
  }

  public procesarIngresoEmergenciaCompleto() {
    const pacienteId = this.triageSelectedPatientId();
    if (!pacienteId) {
      this.errorMessage.set("Error: No se ha seleccionado un paciente válido.");
      return;
    }

    if (!this.triageMotivoConsulta().trim()) {
      this.errorMessage.set("El Motivo de Consulta es obligatorio para el triage de Emergencia.");
      return;
    }

    this.isLoading.set(true);

    // 1. Abrir la cuenta clínica
    this.facturacionService.abrirCuenta(pacienteId, 'Emergencia', this.convenioIngresoId(), this.selectedCamaId()).subscribe({
      next: (res: any) => {
        const cuentaId = res.cuentaId || res.id;
        if (!cuentaId) {
          this.isLoading.set(false);
          this.errorMessage.set("Error: No se pudo obtener el ID de la cuenta creada.");
          return;
        }

        // 2. Preparar el payload del triage
        const antecedenteParts: string[] = [];
        if (this.triageAntecedenteHTA()) antecedenteParts.push('HTA');
        if (this.triageAntecedenteDiabetes()) antecedenteParts.push('Diabetes');
        if (this.triageAntecedenteCardiopatia()) antecedenteParts.push('Cardiopatía');
        const antecedentes = antecedenteParts.join(', ') || 'Ninguno';

        const rawMotivo = this.triageMotivoConsulta().trim();
        const clasificacion = this.triageClasificacion();
        const motivoConTriaje = `[CLASIFICACIÓN TRIAJE: ${clasificacion.toUpperCase()}] ${rawMotivo}`;

        const currentUser = this.authService.currentUser();

        const triagePayload = {
          cuentaServicioId: cuentaId,
          motivoConsulta: motivoConTriaje,
          tensionArterial: this.triageTensionArterial().trim() || 'N/A',
          frecuenciaCardiaca: Number(this.triageFrecuenciaCardiaca()) || 0,
          frecuenciaRespiratoria: Number(this.triageFrecuenciaRespiratoria()) || 0,
          temperatura: Number(this.triageTemperatura()) || 37.0,
          saturacionO2: Number(this.triageSaturacionO2()) || 98,
          glicemiaCapilar: this.triageGlicemiaCapilar() ? Number(this.triageGlicemiaCapilar()) : null,
          estadoConciencia: this.triageEstadoConciencia(),
          glasgowOcular: Number(this.triageGlasgowOcular()) || 4,
          glasgowVerbal: Number(this.triageGlasgowVerbal()) || 5,
          glasgowMotor: Number(this.triageGlasgowMotor()) || 6,
          glasgowTotal: Number(this.triageGlasgowTotal()) || 15,
          viaAerea: this.triageViaAerea(),
          ventilacion: this.triageVentilacion(),
          pulso: this.triagePulso(),
          pielMucosas: this.triagePielMucosas().trim() || 'Normocoloreada',
          llenadoCapilar: this.triageLlenadoCapilar(),
          pupilas: this.triagePupilas(),
          alergias: this.triageAlergiasNinguna() ? 'Ninguna' : (this.triageAlergiasEspecificar().trim() || 'Ninguna'),
          accesosVenosos: this.triageAccesosVenososTrae() ? `Sí (${this.triageAccesosVenososLugar().trim() || 'No especificado'})` : 'No',
          pertenencias: this.triagePertenencias(),
          antecedentesMedicos: antecedentes,
          usuarioRegistro: currentUser?.username || 'admin',
          registrarConstantesVitales: true,
          registrarValoracionFisica: true,
          registrarAntecedentes: true,
          registrarEstadoActual: true,
          descripcionRapida: rawMotivo,
          descripcionDetallada: `Ingreso inicial por: ${rawMotivo}. Clasificación: ${clasificacion}`
        };

        // 3. Registrar el Triage y Valoración Física
        this.http.post(`${environment.apiUrl}/api/Enfermeria/Triage`, triagePayload).subscribe({
          next: () => {
            this.isLoading.set(false);
            this.showIngresoModal.set(false);
            this.actionMessage.set(`Paciente ingresado y triage registrado exitosamente en Emergencia.`);
            setTimeout(() => this.actionMessage.set(null), 5000);
            this.loadOpenAccounts(); // Refrescar lista de activos
          },
          error: (err: any) => {
            this.isLoading.set(false);
            // Mostramos éxito del ingreso pero advertimos sobre el triage
            this.showIngresoModal.set(false);
            this.actionMessage.set(`Paciente ingresado, pero hubo un error al registrar el triage: ${err.error?.Error || err.message}`);
            setTimeout(() => this.actionMessage.set(null), 8000);
            this.loadOpenAccounts();
          }
        });
      },
      error: (err: any) => {
        this.isLoading.set(false);
        this.errorMessage.set(err.error?.Error || err.error?.message || "Error al abrir la cuenta clínica.");
      }
    });
  }

  // --- Wizard: Fast Charge (shared logic with Enfermeria) ---
  public onFastChargeSearchChange(val: string): void {
    this.fastChargeSearchTerm.set(val);
    const term = val.trim().toLowerCase();
    if (term.length >= 1) {
      const filtered = this.servicesCatalog().filter(s =>
        s.descripcion.toLowerCase().includes(term) ||
        s.codigo.toLowerCase().includes(term)
      );
      this.filteredServices.set(filtered);
    } else {
      this.filteredServices.set([]);
    }
  }

  public selectCatalogService(service: ServicioCatalogo): void {
    this.selectedService.set(service);
    this.fastChargeSearchTerm.set(service.descripcion);
    this.filteredServices.set([]);
    this.fastChargeQuantity = 1;
    this.selectedMedicoId.set(null);
    this.customPrecio.set(service.precioUsd ?? 0);
    this.customHonorario.set(service.honorarioBase ?? 0);
    const classification = classifyService(service);
    this.activeStepperMode.set(this.mapClassificationToMode(classification));
    const sugIds = service.sugerenciasIds || service.SugerenciasIds || [];
    const suggestions = this.servicesCatalog().filter(item => sugIds.includes(String(item.id)));
    this.activeSuggestions.set(suggestions);
    const initialSelection: Record<string, boolean> = {};
    suggestions.forEach(s => initialSelection[s.id] = true);
    this.selectedSuggestions.set(initialSelection);
    this.currentStep.set(2);
  }

  public mapClassificationToMode(c: ItemClassification): StepperMode {
    switch (c) {
      case ITEM_CLASSIFICATIONS.CONSULTA: return 'consulta';
      case ITEM_CLASSIFICATIONS.LABORATORIO:
      case ITEM_CLASSIFICATIONS.RX: return 'lab-rx';
      case ITEM_CLASSIFICATIONS.MEDICAMENTO: return 'medicamento';
      default: return 'medicamento';
    }
  }

  public toggleSuggestionSelection(id: string): void {
    this.selectedSuggestions.update(prev => ({ ...prev, [id]: !prev[id] }));
  }

  public resetCurrentItemSelection(): void {
    this.selectedService.set(null);
    this.fastChargeSearchTerm.set('');
    this.fastChargeQuantity = 1;
    this.selectedMedicoId.set(null);
    this.selectedAreaClinicaId.set(null);
    this.customPrecio.set(null);
    this.customHonorario.set(null);
    this.activeSuggestions.set([]);
    this.selectedSuggestions.set({});
    this.currentStep.set(1);
    this.activeStepperMode.set('catalog');
  }

  public onMedicoSelected(medicoId: string | null): void {
    this.selectedMedicoId.set(medicoId);
    if (!medicoId) return;
    const doctor = this.medicos().find(m => m.id === medicoId);
    if (doctor) {
      const service = this.selectedService();
      const doctorHonorary = doctor.honorarioBase ?? (service?.honorarioBase ?? 0);
      this.customHonorario.set(doctorHonorary);
    }
  }

  public onAreaSelected(areaId: string | null): void {
    this.selectedAreaClinicaId.set(areaId);
    if (this.itemClassification() === 'RX' && this.getAreaClinicaNombre(areaId) === 'EMERGENCIA') {
      this.selectedMedicoId.set(null);
      this.customHonorario.set(null);
    }
  }

  public getMedicoNombre(medicoId: string | null): string {
    if (!medicoId) return '';
    const m = this.medicos().find(x => x.id === medicoId);
    return m ? m.nombre.toUpperCase() : 'DESCONOCIDO';
  }

  public getAreaClinicaNombre(areaId: string | null): string {
    if (!areaId) return '';
    const a = this.areasClinicas().find(x => x.id === areaId);
    return a ? a.nombre.toUpperCase() : 'DESCONOCIDA';
  }

  public obtenerNombreMedico(id: string | null): string {
    if (!id) return '--- SIN ASIGNAR ---';
    const medico = this.medicos().find(m => m.id === id);
    return medico ? medico.nombre : '--- SIN ASIGNAR ---';
  }

  public addCurrentItemToCart(): void {
    const active = this.selectedAccount();
    const service = this.selectedService();
    if (!active || !service) return;

    const getUuid = () => {
      if (typeof crypto !== 'undefined') {
        if (crypto.randomUUID) return crypto.randomUUID();
        const array = new Uint32Array(1);
        crypto.getRandomValues(array);
        return 'id_' + array[0].toString(36) + Date.now().toString(36);
      }
      return 'id_' + Date.now().toString(36);
    };

    const classification = this.itemClassification();
    const isFixedQty = classification === ITEM_CLASSIFICATIONS.CONSULTA ||
                       classification === ITEM_CLASSIFICATIONS.LABORATORIO ||
                       classification === ITEM_CLASSIFICATIONS.RX;
    const effectiveQty = isFixedQty ? 1 : Number(this.fastChargeQuantity);

    const requiresMedico = classification === ITEM_CLASSIFICATIONS.CONSULTA ||
      ((service.honorarioBase ?? 0) > 0 &&
       classification !== ITEM_CLASSIFICATIONS.RX &&
       classification !== ITEM_CLASSIFICATIONS.LABORATORIO);

    if (requiresMedico && !this.selectedMedicoId()) {
      alert('Por favor, seleccione el médico tratante para la consulta.');
      return;
    }

    const basePrice = this.customPrecio() !== null ? Number(this.customPrecio()) : (service.precioUsd ?? 0);
    const honoraryPrice = this.customHonorario() !== null ? Number(this.customHonorario()) : (service.honorarioBase ?? 0);

    const mainItem: CartItem = {
      id: getUuid(),
      servicioId: String(service.id),
      descripcion: service.descripcion,
      classification,
      precioBase: basePrice,
      honorario: honoraryPrice,
      cantidad: effectiveQty,
      medicoId: this.selectedMedicoId(),
      medicoNombre: this.getMedicoNombre(this.selectedMedicoId()),
      areaClinicaId: this.selectedAreaClinicaId(),
      areaClinicaNombre: this.getAreaClinicaNombre(this.selectedAreaClinicaId()),
      unidadMedida: service.unidadMedida || 'UD'
    };

    const newItems: CartItem[] = [mainItem];

    // Sugerencias dinámicas seleccionadas
    const activeSugs = this.activeSuggestions();
    const selSugs = this.selectedSuggestions();
    for (const sug of activeSugs) {
      if (selSugs[sug.id]) {
        const sugClass = classifyService(sug);
        newItems.push({
          id: getUuid(),
          servicioId: String(sug.id),
          descripcion: sug.descripcion,
          classification: sugClass,
          precioBase: sug.precioUsd ?? 0,
          honorario: sug.honorarioBase ?? 0,
          cantidad: 1,
          medicoId: null,
          medicoNombre: null,
          areaClinicaId: this.selectedAreaClinicaId(),
          areaClinicaNombre: this.getAreaClinicaNombre(this.selectedAreaClinicaId()),
          unidadMedida: sug.unidadMedida || 'UD'
        });
      }
    }

    this.cartItems.update(prev => [...prev, ...newItems]);
    this.resetCurrentItemSelection();
  }

  public removeCartItem(itemId: string): void {
    this.cartItems.update(prev => prev.filter(i => i.id !== itemId));
  }

  public editCartItem(itemId: string): void {
    const item = this.cartItems().find(i => i.id === itemId);
    if (!item) return;
    this.removeCartItem(itemId);
    const catalogItem = this.servicesCatalog().find(s => String(s.id) === item.servicioId);
    if (catalogItem) {
      this.selectedService.set(catalogItem);
      this.fastChargeSearchTerm.set(catalogItem.descripcion);
      this.fastChargeQuantity = item.cantidad;
      this.selectedMedicoId.set(item.medicoId);
      this.customPrecio.set(item.precioBase);
      this.customHonorario.set(item.honorario);
      this.selectedAreaClinicaId.set(item.areaClinicaId);
      const classification = classifyService(catalogItem);
      this.activeStepperMode.set(this.mapClassificationToMode(classification));
      this.currentStep.set(2);
    }
  }

  public submitAllCartItems(): void {
    const active = this.selectedAccount();
    const items = this.cartItems();
    if (!active || items.length === 0) return;

    this.isSavingFastCharge.set(true);
    this.errorMessage.set(null);

    const payload = {
      pacienteId: active.pacienteId,
      tipoIngreso: active.tipoIngreso,
      convenioId: active.convenioId,
      items: items.map(item => ({
        servicioId: item.servicioId,
        descripcion: item.descripcion,
        precio: item.precioBase + item.honorario,
        honorario: item.honorario,
        cantidad: item.cantidad,
        tipoServicio: item.classification === ITEM_CLASSIFICATIONS.CONSULTA ? 'Medico' : item.classification,
        medicoId: item.medicoId,
        horaCita: item.medicoId ? new Date().toISOString() : undefined,
        areaClinicaId: item.areaClinicaId,
        usuarioCarga: this.authService.currentUser()?.username || 'admin'
      }))
    };

    this.http.post(`${environment.apiUrl}/api/Billing/CargarServiciosMasivo`, payload)
      .subscribe({
        next: () => {
          this.actionMessage.set(`${items.length} servicio(s) cargado(s) exitosamente a la cuenta.`);
          this.cartItems.set([]);
          this.resetCurrentItemSelection();
          this.isSavingFastCharge.set(false);
          this.loadOpenAccounts();
          setTimeout(() => this.actionMessage.set(null), 4000);
        },
        error: (err: any) => {
          this.errorMessage.set('Error al cargar servicios: ' + (err.error?.Error || err.error?.message || err.message));
          this.isSavingFastCharge.set(false);
        }
      });
  }
}
