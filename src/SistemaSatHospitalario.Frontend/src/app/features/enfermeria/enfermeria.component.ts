import { Component, inject, OnInit, signal, computed, effect, HostListener, ElementRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute } from '@angular/router';
import { DynamicStepperComponent } from './components/dynamic-stepper/dynamic-stepper.component';
import { NursingCartComponent } from './components/nursing-cart/nursing-cart.component';
import { StepCatalogSearchComponent } from './components/step-panels/step-catalog-search.component';
import { StepDoctorSelectComponent } from './components/step-panels/step-doctor-select.component';
import { StepLabRxPriceComponent } from './components/step-panels/step-lab-rx-price.component';
import { StepQuantityComponent } from './components/step-panels/step-quantity.component';
import { StepConfirmComponent } from './components/step-panels/step-confirm.component';
import { TrasladosDestinoComponent } from './components/traslados-destino/traslados-destino.component';
import { PedidosInterSedeComponent } from '../admin/inventory/pedidos-inter-sede.component';
import { StockLocalAreaComponent } from './components/stock-local-area/stock-local-area.component';
import { AuthService } from '../../core/services/auth.service';
import { environment } from '../../../environments/environment';
import { MedicoService, Medico } from '../../core/services/medico.service';
import { MultiSedeService, SedeDto, AreaClinica } from '../../core/services/multi-sede.service';
import { PatientService, PatientRecord } from '../../core/services/patient.service';
import { FacturacionService } from '../../core/services/facturacion.service';
import { TIPO_INGRESO, TipoIngresoType, matchTipoIngreso, normalizeTipoIngreso } from '../../core/constants/tipo-ingreso.constants';
import {
  LucideAngularModule,
  Search, RefreshCcw, Check, CheckCircle, X, ChevronRight, ChevronDown,
  AlertCircle, AlertTriangle, Info, Calendar, User, Plus, Stethoscope,
  LogOut, Activity, FileText, Heart, Clipboard, Thermometer, Droplet,
  Shuffle, Edit, UserPlus
} from 'lucide-angular';

export interface CuentaAdministrativa {
  cuentaId: string;
  pacienteId: string;
  pacienteNombre: string;
  pacienteCedula: string;
  tipoIngreso: string;
  convenioId: number | null;
  seguroNombre?: string;
  total?: number;
  totalPagado?: number;
  areaClinicaId?: string;
  areaClinicaNombre?: string;
  subAreaClinica?: string;
  medicoTratanteNombre?: string;
  medicoTratante?: string;
  medicoNombre?: string;
  detalles?: any[];
  [key: string]: any;
}

export interface ServicioCatalogo {
  id: string | number;
  codigo: string;
  descripcion: string;
  precioUsd?: number;
  honorarioBase?: number;
  categoryId?: number;
  tipo?: string;
  unidadMedida?: string;
  isConsultation?: boolean;
  honorariumCategory?: string;
  permiteFraccionamiento?: boolean;
  requiereMedico?: boolean;
  esInventariable?: boolean;
  tipoServicioId?: number;
  sugerenciasIds?: string[];
  SugerenciasIds?: string[];
  [key: string]: any;
}

export type StepperMode = 'catalog' | 'consulta' | 'lab-rx' | 'medicamento' | 'procedimiento';

export interface CartItem {
  id: string;
  servicioId: string;
  descripcion: string;
  classification: ItemClassification;
  precioBase: number;
  honorario: number;
  cantidad: number;
  medicoId: string | null;
  medicoNombre: string | null;
  areaClinicaId: string | null;
  areaClinicaNombre: string | null;
  unidadMedida: string;
}

export interface Convenio {
  id: number;
  nombre: string;
  [key: string]: any;
}

export interface TriageRecord {
  triageId?: string;
  valoracionId?: string;
  clasificacion?: string;
  motivoConsulta?: string;
  tensionArterial?: string;
  frecuenciaCardiaca?: number;
  frecuenciaRespiratoria?: number;
  temperatura?: number;
  saturacionO2?: number;
  glicemiaCapilar?: number | null;
  estadoConciencia?: string;
  glasgowOcular?: number;
  glasgowVerbal?: number;
  glasgowMotor?: number;
  glasgowTotal?: number;
  viaAerea?: string;
  ventilacion?: string;
  pulso?: string;
  pielMucosas?: string;
  llenadoCapilar?: string;
  pupilas?: string;
  alergias?: string;
  accesosVenosos?: string;
  pertenencias?: string;
  antecedentesMedicos?: string;
  registrarConstantesVitales?: boolean;
  registrarValoracionFisica?: boolean;
  registrarAntecedentes?: boolean;
  registrarEstadoActual?: boolean;
  descripcionRapida?: string;
  descripcionDetallada?: string;
  fechaRegistro?: string;
  usuarioRegistro?: string;
  diagnosticoPresuntivo?: string;
  [key: string]: any;
}

export const ITEM_CLASSIFICATIONS = {
  CONSULTA: 'Consulta',
  LABORATORIO: 'Laboratorio',
  RX: 'RX',
  MEDICAMENTO: 'Medicamento',
  PROCEDIMIENTO: 'Procedimiento',
} as const;

export type ItemClassification = (typeof ITEM_CLASSIFICATIONS)[keyof typeof ITEM_CLASSIFICATIONS];

export function classifyService(service: ServicioCatalogo | null | undefined): ItemClassification {
  if (!service) return ITEM_CLASSIFICATIONS.PROCEDIMIENTO;

  if (service.requiereMedico) return ITEM_CLASSIFICATIONS.CONSULTA;
  if (service.permiteFraccionamiento || service.esInventariable) return ITEM_CLASSIFICATIONS.MEDICAMENTO;

  const tipoId = service.tipoServicioId;
  if (tipoId === 3 || tipoId === 300) return ITEM_CLASSIFICATIONS.LABORATORIO;
  if (tipoId === 4 || tipoId === 400) return ITEM_CLASSIFICATIONS.RX;
  if (tipoId === 1 || tipoId === 100) return ITEM_CLASSIFICATIONS.MEDICAMENTO;
  if (tipoId === 2 || tipoId === 200) return ITEM_CLASSIFICATIONS.CONSULTA;

  return ITEM_CLASSIFICATIONS.PROCEDIMIENTO;
}

export const DEFAULT_TRIAGE = {
  CLASIFICACION: 'Nivel III (Amarillo)',
  ESTADO_CONCIENCIA: 'Alerta',
  VIA_AEREA: 'Permeable',
  VENTILACION: 'Normal',
  PULSO: 'Rítmico',
  PIEL_MUCOSAS: 'Normocoloreada',
  LLENADO_CAPILAR: '< 2 segundos',
  PUPILAS: 'Isocóricas',
  PERTENENCIAS: 'Entregadas a familiar',
  TIPO_CORREO: '@gmail.com',
  CODIGO_CELULAR: '0414',
  CODIGO_TELEFONO: '0274'
} as const;

@Component({
  selector: 'app-enfermeria',
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
    StepConfirmComponent,
    TrasladosDestinoComponent,
    PedidosInterSedeComponent,
    StockLocalAreaComponent
  ],
  templateUrl: './enfermeria.component.html',
  styleUrls: ['./enfermeria.component.css']
})
export class EnfermeriaComponent implements OnInit {
  public readonly TIPO_INGRESO = TIPO_INGRESO;
  private readonly http = inject(HttpClient);
  private readonly authService = inject(AuthService);
  private readonly medicoService = inject(MedicoService);
  public readonly multiSedeService = inject(MultiSedeService);
  private readonly patientService = inject(PatientService);
  private readonly facturacionService = inject(FacturacionService);
  private readonly elementRef = inject(ElementRef);
  private readonly route = inject(ActivatedRoute);

  readonly icons = {
    Search, RefreshCcw, Check, CheckCircle, X, ChevronRight, ChevronDown,
    AlertCircle, AlertTriangle, Info, Calendar, User, Plus, Stethoscope,
    LogOut, Activity, FileText, Heart, Clipboard, Thermometer, Droplet,
    Shuffle, Edit, UserPlus
  };

  // Flow State
  public moduleTab = signal<'triage' | 'pedidos' | 'stock'>('triage');
  public showIngresoModal = signal<boolean>(false);
  public showAltaDropdown = signal<boolean>(false);
  public ingresoStep = signal<number>(1);
  public showNewPatientForm = signal<boolean>(false);
  public searchIngresoTerm = signal<string>('');
  public isSearchingPatient = signal<boolean>(false);
  public patientsEncontrados = signal<PatientRecord[]>([]);
  public selectedPatientForIngreso = signal<PatientRecord | null>(null);
  public convenioIngresoId = signal<number | null>(null);
  public selectedCamaId = signal<string | null>(null);
  public medicoTratanteIngresoId = signal<string | null>(null);

  public esMedicoTratanteRequerido = computed(() => {
    const tipo = this.type();
    return tipo === TIPO_INGRESO.HOSPITALIZACION || tipo === TIPO_INGRESO.UCI;
  });
  public camasDisponibles = signal<any[]>([]);
  public errorMessage = signal<string | null>(null);
  public codigosCelular = ['0416', '0426', '0414', '0424', '0412', '0422'];

  public newPatientData = {
    cedula: '', nombre: '', apellidos: '', correo: '', celular: '', telefono: '', direccion: '',
    fechaNacimiento: new Date().toISOString().split('T')[0], sexo: 'ND',
    tipoCorreo: DEFAULT_TRIAGE.TIPO_CORREO, codigoCelular: DEFAULT_TRIAGE.CODIGO_CELULAR, codigoTelefono: DEFAULT_TRIAGE.CODIGO_TELEFONO
  };

  // Triage Signals
  public triageSelectedPatientId = signal<string | null>(null);
  public triageMotivoConsulta = signal<string>('');
  public triageTensionArterial = signal<string>('');
  public triageFrecuenciaCardiaca = signal<number>(80);
  public triageFrecuenciaRespiratoria = signal<number>(18);
  public triageTemperatura = signal<number>(37.0);
  public triageSaturacionO2 = signal<number>(98);
  public triageGlicemiaCapilar = signal<number | null>(null);
  public nivelesTriage = signal<any[]>([]);
  public triageClasificacion = signal<string>('Nivel III (Amarillo)');
  public triageEstadoConciencia = signal<string>(DEFAULT_TRIAGE.ESTADO_CONCIENCIA);
  public triageGlasgowOcular = signal<number>(4);
  public triageGlasgowVerbal = signal<number>(5);
  public triageGlasgowMotor = signal<number>(6);
  public triageGlasgowTotal = computed(() => this.triageGlasgowOcular() + this.triageGlasgowVerbal() + this.triageGlasgowMotor());
  public triageViaAerea = signal<string>(DEFAULT_TRIAGE.VIA_AEREA);
  public triageVentilacion = signal<string>(DEFAULT_TRIAGE.VENTILACION);
  public triagePulso = signal<string>(DEFAULT_TRIAGE.PULSO);
  public triagePielMucosas = signal<string>(DEFAULT_TRIAGE.PIEL_MUCOSAS);
  public triageLlenadoCapilar = signal<string>(DEFAULT_TRIAGE.LLENADO_CAPILAR);
  public triagePupilas = signal<string>(DEFAULT_TRIAGE.PUPILAS);
  public triageAlergiasNinguna = signal<boolean>(true);
  public triageAlergiasEspecificar = signal<string>('');
  public triageAccesosVenososTrae = signal<boolean>(false);
  public triageAccesosVenososLugar = signal<string>('');
  public triagePertenencias = signal<string>(DEFAULT_TRIAGE.PERTENENCIAS);
  public triageAntecedenteHTA = signal<boolean>(false);
  public triageAntecedenteDiabetes = signal<boolean>(false);
  public triageAntecedenteCardiopatia = signal<boolean>(false);

  public type = computed(() => this.nursingAreaFilter());

  // Sedes de la BD
  public sedes = signal<SedeDto[]>([]);
  public selectedSedeId = signal<string | null>(null);

  public sedesOperativas = computed(() => {
    const allSedes = this.sedes();
    const currentTab = normalizeTipoIngreso(this.nursingAreaFilter() || '');

    const sedeFiltrada = allSedes.filter(s => {
      const nameNorm = normalizeTipoIngreso(s.nombre || '');
      const codeNorm = normalizeTipoIngreso(s.codigo || '');
      return nameNorm.includes(currentTab) || currentTab.includes(nameNorm) || codeNorm === currentTab;
    });

    return sedeFiltrada.length > 0 ? sedeFiltrada : allSedes;
  });

  public currentAreaSedeId = computed<string>(() => {
    const activeId = this.selectedSedeId();
    if (activeId) return activeId;

    const list = this.sedesOperativas();
    return list[0]?.id || '';
  });

  // State Lists
  public activeAccounts = signal<CuentaAdministrativa[]>([]);
  public convenios = signal<Convenio[]>([]);
  public servicesCatalog = signal<ServicioCatalogo[]>([]);
  public medicos = signal<Medico[]>([]);
  public areasClinicas = signal<AreaClinica[]>([]);

  // Selected Patient
  public selectedAccount = signal<CuentaAdministrativa | null>(null);
  public isAccountSolvent = computed<boolean>(() => {
    const active = this.selectedAccount();
    if (!active) return true;
    const total = active.total || 0;
    const pagado = active['totalPagado'] || 0;
    const saldo = active['saldoPendiente'] !== undefined ? active['saldoPendiente'] : Math.max(0, total - pagado);
    return saldo <= 0;
  });
  public nursingHistory = signal<TriageRecord[]>([]);

  // Tab / Filter UI
  public activeTab = signal<'fast-charge' | 'triage' | 'transfer' | 'history'>('fast-charge');
  public searchTerm = signal<string>('');
  public isLoading = signal<boolean>(false);
  public actionMessage = signal<string | null>(null);

  // Vital Signs
  public registrarConstantesVitales = true;
  public registrarValoracionFisica = true;
  public registrarAntecedentes = true;
  public registrarEstadoActual = true;
  public descripcionRapida = '';
  public descripcionDetallada = '';
  public motivoConsulta = '';
  public tensionArterial = '';
  public frecuenciaCardiaca = 0;
  public frecuenciaRespiratoria = 0;
  public temperatura = 37;
  public saturacionO2 = 98;
  public glicemiaCapilar: number | null = null;

  public estadoConciencia = 'Alerta';
  public glasgowOcular = 4;
  public glasgowVerbal = 5;
  public glasgowMotor = 6;
  public glasgowTotal = 15;
  public viaAerea = 'Permeable';
  public ventilacion = 'Normal';
  public pulso = 'Rítmico';
  public pielMucosas = 'Normocoloreada';
  public llenadoCapilar = '< 2 segundos';
  public pupilas = 'Isocóricas';
  public alergias = '';
  public accesosVenosos = '';
  public pertenencias = '';
  public antecedentesMedicos = '';

  public showTriageForm = false;
  public isEditingTriage = false;
  public editingTriageId: string | null = null;
  public editingValoracionId: string | null = null;

  // Fast Charge Stepper Signals
  public fastChargeSearchTerm = signal<string>('');
  public filteredServices = signal<ServicioCatalogo[]>([]);
  public selectedService = signal<ServicioCatalogo | null>(null);
  public selectedMedicoId = signal<string | null>(null);

  // ** Signal reactivo para cantidad en Carga Rápida **
  public fastChargeQuantity = signal<number>(1);
  public isSavingFastCharge = signal<boolean>(false);

  public currentStep = signal<number>(1);
  public activeStepperMode = signal<StepperMode>('catalog');
  public cartItems = signal<CartItem[]>([]);
  public activeSuggestions = signal<ServicioCatalogo[]>([]);
  public selectedSuggestions = signal<Record<string, boolean>>({});

  public cartTotalUSD = computed(() =>
    this.cartItems().reduce((acc, item) => acc + (item.precioBase + item.honorario) * item.cantidad, 0)
  );

  public cartItemCount = computed(() => this.cartItems().length);
  public nursingAreaFilter = signal<TipoIngresoType>(TIPO_INGRESO.EMERGENCIA);
  public customPrecio = signal<number | null>(null);
  public customHonorario = signal<number | null>(null);

  public itemClassification = computed<ItemClassification>(() => classifyService(this.selectedService()));

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

  // ** Computado reactivo que multiplica el precio base por fastChargeQuantity() **
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

    const qty = this.fastChargeQuantity() || 1;
    return basePrice * qty;
  });

  public incrementQuantity(): void {
    this.fastChargeQuantity.update(v => (Number(v) || 1) + 1);
  }

  public decrementQuantity(): void {
    this.fastChargeQuantity.update(v => v > 1 ? v - 1 : 1);
  }

  // Transfer Area Form
  public nuevoTipoIngreso = signal<string>(TIPO_INGRESO.HOSPITALIZACION);
  public nuevoConvenioId: number | null = null;
  public esEgreso = false;
  public isSavingTransfer = signal<boolean>(false);
  public fechaHoraEgresoEfectiva: string | null = null;
  public montoSobrescrito: number | null = null;

  public camasDisponiblesIngreso = computed(() => {
    const camas = this.camasDisponibles();
    const targetSedeId = this.currentAreaSedeId();
    const filtered = targetSedeId ? camas.filter(c => c.sedeId === targetSedeId) : camas;
    return (filtered && filtered.length > 0) ? filtered : camas;
  });

  public filteredAccounts = computed(() => {
    const list = this.activeAccounts();
    const term = this.searchTerm().trim();
    const currentTab = this.nursingAreaFilter();

    const areaFiltered = list.filter(acc => matchTipoIngreso(acc.tipoIngreso, currentTab));

    if (!term) return areaFiltered;
    const termNorm = normalizeTipoIngreso(term);
    return areaFiltered.filter(acc =>
      normalizeTipoIngreso(acc.pacienteNombre).includes(termNorm) ||
      normalizeTipoIngreso(acc.pacienteCedula).includes(termNorm) ||
      normalizeTipoIngreso(acc.tipoIngreso).includes(termNorm)
    );
  });

  public estadoActualPaciente = computed(() => {
    const history = this.nursingHistory();
    if (!history || history.length === 0) return null;
    const latestWithState = history.find(h => h.descripcionRapida || h.descripcionDetallada);
    return latestWithState || history[0];
  });

  constructor() {
    effect(() => {
      const list = this.sedesOperativas();
      if (list.length > 0) {
        this.selectedSedeId.set(list[0].id);
      }
    });
  }

  ngOnInit(): void {
    this.http.get<any[]>(`${environment.apiUrl}/api/Triage/niveles`).subscribe({
      next: (niveles) => {
        this.nivelesTriage.set(niveles || []);
        if (niveles && niveles.length > 0) {
          this.triageClasificacion.set(niveles[0].nombre || niveles[0].descripcion);
        }
      },
      error: () => {
        this.nivelesTriage.set([
          { id: '1', nombre: 'Nivel I (Rojo) - Reanimación' },
          { id: '2', nombre: 'Nivel II (Naranja) - Emergencia' },
          { id: '3', nombre: 'Nivel III (Amarillo) - Urgencia' },
          { id: '4', nombre: 'Nivel IV (Verde) - Menor' }
        ]);
      }
    });

    this.refreshAccounts();
    this.loadCatalogAndConvenios();
    this.loadCamasDisponibles();

    this.route.queryParams.subscribe(params => {
      if (params['tab'] === 'stock' || params['action'] === 'stock') {
        this.moduleTab.set('stock');
      } else if (params['tab'] === 'pedidos' || params['action'] === 'pedidos') {
        this.moduleTab.set('pedidos');
      } else {
        this.moduleTab.set('triage');
      }
    });
  }

  public autoSelectSedeForPatient(account: CuentaAdministrativa | null): void {
    const list = this.sedesOperativas();
    if (!list || list.length === 0) return;

    const targetRaw = account?.subAreaClinica || account?.areaClinicaNombre || account?.tipoIngreso || this.nursingAreaFilter();
    const targetNorm = normalizeTipoIngreso(targetRaw || '');

    const match = list.find(s => {
      const nameNorm = normalizeTipoIngreso(s.nombre || '');
      const codeNorm = normalizeTipoIngreso(s.codigo || '');
      return nameNorm.includes(targetNorm) || targetNorm.includes(nameNorm) || codeNorm === targetNorm;
    });

    if (match) {
      this.selectedSedeId.set(match.id);
    } else {
      this.selectedSedeId.set(list[0].id);
    }
  }

  public loadCamasDisponibles() {
    this.http.get<any[]>(`${environment.apiUrl}/api/AreaClinica/monitoreo`).subscribe({
      next: (res) => {
        const libres = res.filter((c: any) => c.estado === 'Disponible');
        this.camasDisponibles.set(libres);
      },
      error: (err) => console.error('[ENFERMERIA] Error al cargar áreas clínicas/camas:', err)
    });
  }

  public refreshAccounts(): void {
    this.isLoading.set(true);
    this.http.get<CuentaAdministrativa[]>(`${environment.apiUrl}/api/Billing/cuentas-administrativas?estado=Abierta`)
      .subscribe({
        next: (res) => {
          this.activeAccounts.set(res);
          this.isLoading.set(false);
          const currentSelected = this.selectedAccount();
          if (currentSelected) {
            const updated = res.find(c => c.cuentaId === currentSelected.cuentaId);
            if (updated) {
              this.selectedAccount.set(updated);
            }
          }
        },
        error: (err) => {
          console.error('[ENFERMERIA] Error loading accounts:', err);
          this.isLoading.set(false);
        }
      });
  }

  public onTransferOrDischargeCompleted(msg?: any): void {
    const messageStr = typeof msg === 'string' ? msg : 'Operación de traslado / alta procesada exitosamente.';
    this.actionMessage.set(messageStr);
    this.refreshAccounts();
    this.loadCamasDisponibles();
    setTimeout(() => this.actionMessage.set(null), 4000);
  }

  public handleActionErrorMessage(msg: any): void {
    const errorStr = typeof msg === 'string' ? msg : (msg?.detail || msg?.message || 'Error en la operación');
    this.actionMessage.set(errorStr);
  }

  private loadCatalogAndConvenios(): void {
    this.http.get<ServicioCatalogo[]>(`${environment.apiUrl}/api/Catalog/unified`)
      .subscribe({
        next: (res) => this.servicesCatalog.set(res),
        error: (err) => console.error('[ENFERMERIA] Error loading catalog:', err)
      });

    this.http.get<Convenio[]>(`${environment.apiUrl}/api/Convenios`)
      .subscribe({
        next: (res) => this.convenios.set(res),
        error: (err) => console.error('[ENFERMERIA] Error loading convenios:', err)
      });

    this.medicoService.getAll().subscribe({
      next: (res) => this.medicos.set(res.filter(m => m.activo)),
      error: (err) => console.error('[ENFERMERIA] Error loading medicos:', err)
    });

    this.http.get<SedeDto[]>(`${environment.apiUrl}/api/Sede`).subscribe({
      next: (res) => {
        const activeSedes = res.filter(s => s.activo);
        this.sedes.set(activeSedes);
      },
      error: (err) => console.error('[ENFERMERIA] Error loading sedes:', err)
    });

    this.multiSedeService.getAreasClinicas().subscribe({
      next: (areas) => this.areasClinicas.set(areas.filter(a => a.activo !== false)),
      error: (err) => console.error('[ENFERMERIA] Error loading areas clinicas:', err)
    });
  }

  public selectAccount(account: CuentaAdministrativa): void {
    this.selectedAccount.set(account);
    this.loadTriageHistory(account.cuentaId);
    this.resetTriageForm();
    this.activeTab.set('fast-charge');

    this.nuevoConvenioId = account.convenioId;
    this.nuevoTipoIngreso.set(account.tipoIngreso);
    this.esEgreso = false;

    this.autoSelectSedeForPatient(account);
  }

  public loadTriageHistory(cuentaId: string): void {
    this.http.get<TriageRecord[]>(`${environment.apiUrl}/api/Enfermeria/triageHistorial/${cuentaId}`)
      .subscribe({
        next: (res) => {
          this.nursingHistory.set(res);
          this.showTriageForm = res.length === 0;
        },
        error: (err) => console.error('[ENFERMERIA] Error loading history:', err)
      });
  }

  public updateGlasgowTotal(): void {
    this.glasgowTotal = (Number(this.glasgowOcular) || 0) +
      (Number(this.glasgowVerbal) || 0) +
      (Number(this.glasgowMotor) || 0);
  }

  public resetTriageForm(): void {
    this.motivoConsulta = '';
    this.tensionArterial = '';
    this.frecuenciaCardiaca = 0;
    this.frecuenciaRespiratoria = 0;
    this.temperatura = 37;
    this.saturacionO2 = 98;
    this.glicemiaCapilar = null;

    this.estadoConciencia = DEFAULT_TRIAGE.ESTADO_CONCIENCIA;
    this.glasgowOcular = 4;
    this.glasgowVerbal = 5;
    this.glasgowMotor = 6;
    this.glasgowTotal = 15;
    this.viaAerea = DEFAULT_TRIAGE.VIA_AEREA;
    this.ventilacion = DEFAULT_TRIAGE.VENTILACION;
    this.pulso = DEFAULT_TRIAGE.PULSO;
    this.pielMucosas = DEFAULT_TRIAGE.PIEL_MUCOSAS;
    this.llenadoCapilar = DEFAULT_TRIAGE.LLENADO_CAPILAR;
    this.pupilas = DEFAULT_TRIAGE.PUPILAS;
    this.alergias = '';
    this.accesosVenosos = '';
    this.pertenencias = '';
    this.antecedentesMedicos = '';

    this.registrarConstantesVitales = true;
    this.registrarValoracionFisica = true;
    this.registrarAntecedentes = true;
    this.registrarEstadoActual = true;
    this.descripcionRapida = '';
    this.descripcionDetallada = '';

    this.isEditingTriage = false;
    this.editingTriageId = null;
    this.editingValoracionId = null;

    this.showTriageForm = this.nursingHistory().length === 0;
  }

  private buildTriagePayload(cuentaId?: string): Record<string, any> {
    return {
      ...(cuentaId ? { cuentaServicioId: cuentaId } : { triageId: this.editingTriageId, valoracionId: this.editingValoracionId }),
      motivoConsulta: this.motivoConsulta,
      tensionArterial: this.tensionArterial,
      frecuenciaCardiaca: Number(this.frecuenciaCardiaca),
      frecuenciaRespiratoria: Number(this.frecuenciaRespiratoria),
      temperatura: Number(this.temperatura),
      saturacionO2: Number(this.saturacionO2),
      glicemiaCapilar: this.glicemiaCapilar ? Number(this.glicemiaCapilar) : null,
      estadoConciencia: this.estadoConciencia,
      glasgowOcular: Number(this.glasgowOcular),
      glasgowVerbal: Number(this.glasgowVerbal),
      glasgowMotor: Number(this.glasgowMotor),
      glasgowTotal: Number(this.glasgowTotal),
      viaAerea: this.viaAerea,
      ventilacion: this.ventilacion,
      pulso: this.pulso,
      pielMucosas: this.pielMucosas,
      llenadoCapilar: this.llenadoCapilar,
      pupilas: this.pupilas,
      alergias: this.alergias,
      accesosVenosos: this.accesosVenosos,
      pertenencias: this.pertenencias,
      antecedentesMedicos: this.antecedentesMedicos,
      registrarConstantesVitales: this.registrarConstantesVitales,
      registrarValoracionFisica: this.registrarValoracionFisica,
      registrarAntecedentes: this.registrarAntecedentes,
      registrarEstadoActual: this.registrarEstadoActual,
      descripcionRapida: this.descripcionRapida,
      descripcionDetallada: this.descripcionDetallada
    };
  }

  public submitTriage(): void {
    const active = this.selectedAccount();
    if (!active) return;

    this.isLoading.set(true);

    if (this.isEditingTriage) {
      const payload = this.buildTriagePayload();
      this.http.put(`${environment.apiUrl}/api/Enfermeria/Triage`, payload)
        .subscribe({
          next: () => {
            this.showSuccess('Medición del historial corregida exitosamente.');
            this.resetTriageForm();
            this.loadTriageHistory(active.cuentaId);
            this.isLoading.set(false);
          },
          error: (err) => {
            alert('Error al modificar triage: ' + (err.error?.Error || err.message));
            this.isLoading.set(false);
          }
        });
    } else {
      const payload = this.buildTriagePayload(active.cuentaId);
      this.http.post(`${environment.apiUrl}/api/Enfermeria/Triage`, payload)
        .subscribe({
          next: () => {
            this.showSuccess('Signos vitales registrados en el historial correctamente.');
            this.resetTriageForm();
            this.loadTriageHistory(active.cuentaId);
            this.isLoading.set(false);
          },
          error: (err) => {
            alert('Error al registrar triage: ' + (err.error?.Error || err.message));
            this.isLoading.set(false);
          }
        });
    }
  }

  public onEditTriageClick(item: TriageRecord): void {
    this.motivoConsulta = item.motivoConsulta || '';
    this.tensionArterial = item.tensionArterial || '';
    this.frecuenciaCardiaca = item.frecuenciaCardiaca || 0;
    this.frecuenciaRespiratoria = item.frecuenciaRespiratoria || 0;
    this.temperatura = item.temperatura || 37;
    this.saturacionO2 = item.saturacionO2 || 98;
    this.glicemiaCapilar = item.glicemiaCapilar ?? null;

    this.estadoConciencia = item.estadoConciencia || 'Alerta';
    this.glasgowOcular = item.glasgowOcular || 4;
    this.glasgowVerbal = item.glasgowVerbal || 5;
    this.glasgowMotor = item.glasgowMotor || 6;
    this.glasgowTotal = item.glasgowTotal || 15;
    this.viaAerea = item.viaAerea || 'Permeable';
    this.ventilacion = item.ventilacion || 'Normal';
    this.pulso = item.pulso || 'Rítmico';
    this.pielMucosas = item.pielMucosas || 'Normocoloreada';
    this.llenadoCapilar = item.llenadoCapilar || '< 2 segundos';
    this.pupilas = item.pupilas || 'Isocóricas';
    this.alergias = item.alergias || '';
    this.accesosVenosos = item.accesosVenosos || '';
    this.pertenencias = item.pertenencias || '';
    this.antecedentesMedicos = item.antecedentesMedicos || '';

    this.registrarConstantesVitales = true;
    this.registrarValoracionFisica = true;
    this.registrarAntecedentes = true;
    this.registrarEstadoActual = true;
    this.descripcionRapida = item.descripcionRapida || '';
    this.descripcionDetallada = item.descripcionDetallada || '';

    this.isEditingTriage = true;
    this.editingTriageId = item.triageId || null;
    this.editingValoracionId = item.valoracionId || null;
    this.showTriageForm = true;

    setTimeout(() => {
      const formElement = document.getElementById('nursingForm');
      if (formElement) {
        formElement.scrollIntoView({ behavior: 'smooth' });
      }
    }, 50);
  }

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

  public mapClassificationToMode(c: ItemClassification): StepperMode {
    switch (c) {
      case ITEM_CLASSIFICATIONS.CONSULTA: return 'consulta';
      case ITEM_CLASSIFICATIONS.LABORATORIO:
      case ITEM_CLASSIFICATIONS.RX: return 'lab-rx';
      case ITEM_CLASSIFICATIONS.MEDICAMENTO: return 'medicamento';
      case ITEM_CLASSIFICATIONS.PROCEDIMIENTO: return 'procedimiento';
      default: return 'medicamento';
    }
  }

  public selectCatalogService(service: ServicioCatalogo): void {
    this.selectedService.set(service);
    this.fastChargeSearchTerm.set(service.descripcion);
    this.filteredServices.set([]);
    this.fastChargeQuantity.set(1);
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

  public toggleSuggestionSelection(id: string): void {
    this.selectedSuggestions.update(prev => ({
      ...prev,
      [id]: !prev[id]
    }));
  }

  public resetCurrentItemSelection(): void {
    this.selectedService.set(null);
    this.fastChargeSearchTerm.set('');
    this.fastChargeQuantity.set(1);
    this.selectedMedicoId.set(null);
    this.customPrecio.set(null);
    this.customHonorario.set(null);
    this.activeSuggestions.set([]);
    this.selectedSuggestions.set({});
    this.currentStep.set(1);
    this.activeStepperMode.set('catalog');
  }

  public addCurrentItemToCart(): void {
    const active = this.selectedAccount();
    const service = this.selectedService();
    if (!active || !service) return;

    const getUuid = () => typeof crypto !== 'undefined' && crypto.randomUUID ? crypto.randomUUID() : 'id_' + Math.random().toString(36).substring(2, 9) + Date.now().toString(36);

    const classification = this.itemClassification();
    const isFixedQty = classification === ITEM_CLASSIFICATIONS.CONSULTA ||
      classification === ITEM_CLASSIFICATIONS.LABORATORIO ||
      classification === ITEM_CLASSIFICATIONS.RX;
    const effectiveQty = isFixedQty ? 1 : Number(this.fastChargeQuantity());

    const requiresMedico = classification === ITEM_CLASSIFICATIONS.CONSULTA ||
      ((service.honorarioBase ?? 0) > 0 && classification !== ITEM_CLASSIFICATIONS.RX && classification !== ITEM_CLASSIFICATIONS.LABORATORIO);

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
      classification: classification,
      precioBase: basePrice,
      honorario: honoraryPrice,
      cantidad: effectiveQty,
      medicoId: this.selectedMedicoId(),
      medicoNombre: this.getMedicoNombre(this.selectedMedicoId()),
      areaClinicaId: this.selectedSedeId(),
      areaClinicaNombre: this.getSedeNombre(this.selectedSedeId()),
      unidadMedida: service.unidadMedida || 'UD'
    };

    const newItems: CartItem[] = [mainItem];

    const activeSugs = this.activeSuggestions();
    const selSugs = this.selectedSuggestions();
    for (const sug of activeSugs) {
      if (selSugs[sug.id]) {
        const sugClass = classifyService(sug);
        const sugDefaultHonorary = sug.honorarioBase ?? 0;
        const sugPureBasePrice = sug.precioUsd ?? 0;
        newItems.push({
          id: getUuid(),
          servicioId: String(sug.id),
          descripcion: sug.descripcion,
          classification: sugClass,
          precioBase: sugPureBasePrice,
          honorario: sugDefaultHonorary,
          cantidad: 1,
          medicoId: null,
          medicoNombre: null,
          areaClinicaId: this.selectedSedeId(),
          areaClinicaNombre: this.getSedeNombre(this.selectedSedeId()),
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
      this.fastChargeQuantity.set(item.cantidad);
      this.selectedMedicoId.set(item.medicoId);
      this.customPrecio.set(item.precioBase);
      this.customHonorario.set(item.honorario);
      this.selectedSedeId.set(item.areaClinicaId);

      const classification = classifyService(catalogItem);
      this.activeStepperMode.set(this.mapClassificationToMode(classification));
      this.currentStep.set(2);
    }
  }

  // **VERIFICACIÓN DEL FLUJO DE CARGA MASIVA Y ACTUALIZACIÓN DE CUENTA**
  public submitAllCartItems(): void {
    const active = this.selectedAccount();
    const items = this.cartItems();
    if (!active || items.length === 0) return;

    this.isSavingFastCharge.set(true);

    const payload = {
      cuentaId: active.cuentaId, // Vinculación directa con la cuenta abierta
      pacienteId: active.pacienteId,
      tipoIngreso: active.tipoIngreso,
      convenioId: active.convenioId,
      origenCarga: active.subAreaClinica || active.tipoIngreso,
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
        usuarioCarga: ''
      }))
    };

    this.http.post(`${environment.apiUrl}/api/Billing/CargarServiciosMasivo`, payload)
      .subscribe({
        next: () => {
          this.showSuccess(`${items.length} servicio(s) cargado(s) a la cuenta del paciente.`);
          this.cartItems.set([]);
          this.resetCurrentItemSelection();
          this.isSavingFastCharge.set(false);
          this.refreshAccounts();
        },
        error: (err: any) => {
          alert('Error al cargar servicios: ' + (err.error?.Error || err.error?.message || err.message));
          this.isSavingFastCharge.set(false);
        }
      });
  }

  public getSedeNombre(sedeId: string | null): string {
    if (!sedeId) return '';
    const s = this.sedes().find(x => x.id === sedeId);
    return s ? s.nombre.toUpperCase() : 'DESCONOCIDA';
  }

  public onSedeSelected(sedeId: string | null): void {
    this.selectedSedeId.set(sedeId);
  }

  public submitTransfer(): void {
    const active = this.selectedAccount();
    if (!active) return;

    this.isSavingTransfer.set(true);

    const payload = {
      pacienteId: active.pacienteId,
      nuevoTipoIngreso: this.esEgreso ? 'Particular' : this.nuevoTipoIngreso(),
      nuevoConvenioId: this.esEgreso ? null : this.nuevoConvenioId,
      usuarioTraslado: '',
      esEgreso: this.esEgreso,
      nuevaAreaClinicaId: this.esEgreso ? null : this.selectedCamaId(),
      fechaHoraEgresoEfectiva: this.fechaHoraEgresoEfectiva ? new Date(this.fechaHoraEgresoEfectiva).toISOString() : null,
      montoSobrescrito: this.montoSobrescrito || null
    };

    this.http.post(`${environment.apiUrl}/api/Enfermeria/Traslado`, payload)
      .subscribe({
        next: () => {
          if (this.esEgreso) {
            this.showSuccess('Paciente dado de alta administrativamente. La cuenta actual ha sido cerrada.');
          } else {
            this.showSuccess(`Paciente trasladado exitosamente a la ubicación: ${this.nuevoTipoIngreso()}.`);
          }
          this.selectedAccount.set(null);
          this.nursingHistory.set([]);
          this.fechaHoraEgresoEfectiva = null;
          this.montoSobrescrito = null;
          this.refreshAccounts();
          this.isSavingTransfer.set(false);
        },
        error: (err) => {
          alert('Error al procesar traslado: ' + (err.error?.Error || err.message));
          this.isSavingTransfer.set(false);
        }
      });
  }

  // Alta Médica Handlers
  public isAltaDropdownOpen = signal<boolean>(false);
  public showAltaModal = signal<boolean>(false);
  public showSolvenciaModal = signal<boolean>(false);
  public tipoAltaSeleccionada = signal<number>(0);
  public observacionesAlta = signal<string>('');
  public confirmadoEnfermeriaSinSolvencia = signal<boolean>(false);
  public isProcessingAlta = signal<boolean>(false);

  @HostListener('document:click', ['$event'])
  public onDocumentClick(event: MouseEvent): void {
    if (this.isAltaDropdownOpen()) {
      const target = event.target as HTMLElement;
      if (this.elementRef?.nativeElement && !this.elementRef.nativeElement.querySelector('.alta-dropdown-container')?.contains(target)) {
        this.isAltaDropdownOpen.set(false);
      }
    }
  }

  public toggleAltaDropdown(event?: Event): void {
    event?.stopPropagation();
    this.isAltaDropdownOpen.update(prev => !prev);
  }

  public closeAltaDropdown(): void {
    this.isAltaDropdownOpen.set(false);
  }

  public iniciarAltaMedica(tipo: number): void {
    this.isAltaDropdownOpen.set(false);
    const active = this.selectedAccount();
    if (!active) {
      alert('Debe seleccionar un paciente de la lista.');
      return;
    }
    this.tipoAltaSeleccionada.set(tipo);
    this.observacionesAlta.set('');
    this.confirmadoEnfermeriaSinSolvencia.set(false);

    const total = active.total || 0;
    const pagado = active['totalPagado'] || 0;
    const saldo = active['saldoPendiente'] !== undefined ? active['saldoPendiente'] : Math.max(0, total - pagado);

    if (saldo > 0) {
      this.showSolvenciaModal.set(true);
    } else {
      this.showAltaModal.set(true);
    }
  }

  public continuarAltaSinSolvencia(): void {
    this.confirmadoEnfermeriaSinSolvencia.set(true);
    this.showSolvenciaModal.set(false);
    this.showAltaModal.set(true);
  }

  public procesarAltaPaciente(): void {
    const active = this.selectedAccount();
    if (!active) return;

    this.isProcessingAlta.set(true);
    const payload = {
      pacienteId: active.pacienteId,
      admisionId: active.cuentaId,
      tipoAlta: this.tipoAltaSeleccionada(),
      observaciones: this.observacionesAlta(),
      confirmadoPorEnfermeriaSinSolvencia: this.confirmadoEnfermeriaSinSolvencia()
    };

    this.http.post(`${environment.apiUrl}/api/Enfermeria/Alta`, payload).subscribe({
      next: (res: any) => {
        this.isProcessingAlta.set(false);
        this.showAltaModal.set(false);
        this.showSuccess(res?.mensaje || 'Alta médica registrada exitosamente.');
        this.refreshAccounts();
      },
      error: (err: any) => {
        this.isProcessingAlta.set(false);
        const rawBody = err.error;
        const errorMsg =
          (typeof rawBody === 'string' ? rawBody : null) ||
          rawBody?.error ||
          rawBody?.Error ||
          rawBody?.message ||
          rawBody?.Message ||
          err.message ||
          'Error al registrar el alta médica.';

        if (errorMsg.toLowerCase().includes('saldo pendiente') || errorMsg.toLowerCase().includes('solvencia')) {
          this.showAltaModal.set(false);
          this.showSolvenciaModal.set(true);
        } else {
          alert('Error al registrar el alta médica: ' + errorMsg);
        }
      }
    });
  }

  public onTransferError(err: string): void {
    alert(err);
  }

  private showSuccess(msg: string): void {
    this.actionMessage.set(msg);
    setTimeout(() => this.actionMessage.set(null), 6000);
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

  public getMedicoNombre(medicoId: string | null): string {
    if (!medicoId) return '';
    const m = this.medicos().find(x => x.id === medicoId);
    return m ? m.nombre.toUpperCase() : 'DESCONOCIDO';
  }

  public getMedicoEspecialidad(medicoId: string | null): string {
    if (!medicoId) return '';
    const m = this.medicos().find(x => x.id === medicoId);
    return m ? (m.especialidad?.toUpperCase() || 'GENERAL') : 'GENERAL';
  }

  public abrirModalIngreso() {
    this.searchIngresoTerm.set('');
    this.patientsEncontrados.set([]);
    this.selectedPatientForIngreso.set(null);
    this.convenioIngresoId.set(null);
    this.showNewPatientForm.set(false);
    this.selectedCamaId.set(null);
    this.medicoTratanteIngresoId.set(null);
    this.loadCamasDisponibles();
    this.newPatientData = {
      cedula: '', nombre: '', apellidos: '', correo: '', celular: '', telefono: '', direccion: '',
      fechaNacimiento: new Date().toISOString().split('T')[0], sexo: 'ND',
      tipoCorreo: DEFAULT_TRIAGE.TIPO_CORREO, codigoCelular: DEFAULT_TRIAGE.CODIGO_CELULAR, codigoTelefono: DEFAULT_TRIAGE.CODIGO_TELEFONO
    };
    this.errorMessage.set(null);

    this.ingresoStep.set(1);
    this.triageSelectedPatientId.set(null);
    this.triageMotivoConsulta.set('');
    this.triageTensionArterial.set('');
    this.triageFrecuenciaCardiaca.set(80);
    this.triageFrecuenciaRespiratoria.set(18);
    this.triageTemperatura.set(37.0);
    this.triageSaturacionO2.set(98);
    this.triageGlicemiaCapilar.set(null);
    this.triageClasificacion.set(DEFAULT_TRIAGE.CLASIFICACION);
    this.triageEstadoConciencia.set(DEFAULT_TRIAGE.ESTADO_CONCIENCIA);
    this.triageGlasgowOcular.set(4);
    this.triageGlasgowVerbal.set(5);
    this.triageGlasgowMotor.set(6);
    this.triageViaAerea.set(DEFAULT_TRIAGE.VIA_AEREA);
    this.triageVentilacion.set(DEFAULT_TRIAGE.VENTILACION);
    this.triagePulso.set(DEFAULT_TRIAGE.PULSO);
    this.triagePielMucosas.set(DEFAULT_TRIAGE.PIEL_MUCOSAS);
    this.triageLlenadoCapilar.set(DEFAULT_TRIAGE.LLENADO_CAPILAR);
    this.triagePupilas.set(DEFAULT_TRIAGE.PUPILAS);
    this.triageAlergiasNinguna.set(true);
    this.triageAlergiasEspecificar.set('');
    this.triageAccesosVenososTrae.set(false);
    this.triageAccesosVenososLugar.set('');
    this.triagePertenencias.set(DEFAULT_TRIAGE.PERTENENCIAS);
    this.triageAntecedenteHTA.set(false);
    this.triageAntecedenteDiabetes.set(false);
    this.triageAntecedenteCardiopatia.set(false);

    this.showIngresoModal.set(true);
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  public buscarPacienteIngreso() {
    const term = this.searchIngresoTerm().trim();
    if (term.length >= 3) {
      this.isSearchingPatient.set(true);
      this.errorMessage.set(null);
      this.patientService.searchPatients(term).subscribe({
        next: (res) => {
          this.patientsEncontrados.set(res || []);
          this.isSearchingPatient.set(false);
          if (res && res.length === 1) {
            this.seleccionarPacienteIngreso(res[0]);
          } else if (res && res.length === 0) {
            this.errorMessage.set("No se encontró ningún paciente con la cédula o nombre ingresado.");
          }
        },
        error: (err) => {
          this.isSearchingPatient.set(false);
          this.errorMessage.set(err.error?.message || "Error al buscar paciente.");
        }
      });
    }
  }

  public seleccionarPacienteIngreso(p: PatientRecord) {
    this.selectedPatientForIngreso.set(p);
    this.patientsEncontrados.set([]);
    this.errorMessage.set(null);
  }

  public deseleccionarPacienteIngreso() {
    this.selectedPatientForIngreso.set(null);
  }

  public procesarIngreso() {
    this.errorMessage.set(null);

    if (this.esMedicoTratanteRequerido() && !this.medicoTratanteIngresoId()) {
      this.errorMessage.set('Debe seleccionar un Médico Tratante para ingreso a ' + this.type() + '.');
      return;
    }

    if (this.fechaNacimientoFormatted && this.fechaNacimientoFormatted.length === 10) {
      const parts = this.fechaNacimientoFormatted.split('-');
      if (parts.length === 3) {
        this.newPatientData.fechaNacimiento = `${parts[2]}-${parts[1]}-${parts[0]}`;
      }
    }

    if (this.showNewPatientForm()) {
      if (!this.newPatientData.cedula ||
        !this.newPatientData.nombre ||
        !this.newPatientData.apellidos ||
        !this.newPatientData.fechaNacimiento ||
        !this.newPatientData.celular) {
        this.errorMessage.set("Todos los campos marcados con (*) son obligatorios: Cédula, Nombres, Apellidos, Fecha de Nacimiento y Celular.");
        return;
      }

      this.isLoading.set(true);
      this.patientService.createPatient(this.newPatientData).subscribe({
        next: (p: PatientRecord) => {
          if (this.type() === TIPO_INGRESO.EMERGENCIA) {
            this.isLoading.set(false);
            this.triageSelectedPatientId.set(p.id);
            this.ingresoStep.set(2);
            window.scrollTo({ top: 0, behavior: 'smooth' });
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
      let patient = this.selectedPatientForIngreso();
      if (!patient && this.patientsEncontrados().length > 0) {
        patient = this.patientsEncontrados()[0];
        this.seleccionarPacienteIngreso(patient);
      }

      if (!patient) {
        this.errorMessage.set("Por favor busque y seleccione un paciente o complete el formulario de nuevo registro.");
        return;
      }

      if (this.type() === TIPO_INGRESO.EMERGENCIA) {
        this.triageSelectedPatientId.set(patient.id);
        this.ingresoStep.set(2);
        window.scrollTo({ top: 0, behavior: 'smooth' });
      } else {
        this.isLoading.set(true);
        this.abrirCuentaParaPaciente(patient.id);
      }
    }
  }

  private abrirCuentaParaPaciente(pacienteId: string) {
    const targetType = this.type();
    this.facturacionService.abrirCuenta(pacienteId, targetType, this.convenioIngresoId(), this.selectedCamaId(), this.medicoTratanteIngresoId()).subscribe({
      next: () => {
        this.isLoading.set(false);
        this.showIngresoModal.set(false);
        this.actionMessage.set(`Paciente ingresado exitosamente a la sección de ${targetType}.`);
        setTimeout(() => this.actionMessage.set(null), 5000);
        if (targetType === TIPO_INGRESO.EMERGENCIA || targetType === TIPO_INGRESO.HOSPITALIZACION || targetType === TIPO_INGRESO.UCI) {
          this.nursingAreaFilter.set(targetType as TipoIngresoType);
        }
        this.refreshAccounts();
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

    this.facturacionService.abrirCuenta(pacienteId, 'Emergencia', this.convenioIngresoId(), this.selectedCamaId(), this.medicoTratanteIngresoId()).subscribe({
      next: (res: any) => {
        const cuentaId = res?.cuentaId || res?.CuentaId || res?.id || (typeof res === 'string' ? res : null);
        if (!cuentaId) {
          this.isLoading.set(false);
          this.errorMessage.set("Error: No se pudo obtener el ID de la cuenta creada.");
          return;
        }

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

        this.http.post(`${environment.apiUrl}/api/Enfermeria/Triage`, triagePayload).subscribe({
          next: () => {
            this.isLoading.set(false);
            this.showIngresoModal.set(false);
            this.actionMessage.set(`Paciente ingresado y triage registrado exitosamente en Emergencia.`);
            setTimeout(() => this.actionMessage.set(null), 5000);
            this.nursingAreaFilter.set('Emergencia');
            this.refreshAccounts();
          },
          error: (err: any) => {
            this.isLoading.set(false);
            this.showIngresoModal.set(false);
            this.actionMessage.set(`Paciente ingresado, pero hubo un error al registrar el triage: ${err.error?.Error || err.message}`);
            setTimeout(() => this.actionMessage.set(null), 8000);
            this.refreshAccounts();
          }
        });
      },
      error: (err: any) => {
        this.isLoading.set(false);
        this.errorMessage.set(err.error?.Error || err.error?.message || "Error al abrir la cuenta clínica.");
      }
    });
  }

  public get fechaNacimientoFormatted(): string {
    if (!this.newPatientData.fechaNacimiento) return '';
    const parts = this.newPatientData.fechaNacimiento.split('-');
    if (parts.length === 3) {
      return `${parts[2]}-${parts[1]}-${parts[0]}`;
    }
    return this.newPatientData.fechaNacimiento;
  }
  public set fechaNacimientoFormatted(val: string) {
    if (val && val.length === 10) {
      const parts = val.split('-');
      if (parts.length === 3) {
        this.newPatientData.fechaNacimiento = `${parts[2]}-${parts[1]}-${parts[0]}`;
      }
    }
  }

  logout(): void {
    this.authService.logout();
  }
}