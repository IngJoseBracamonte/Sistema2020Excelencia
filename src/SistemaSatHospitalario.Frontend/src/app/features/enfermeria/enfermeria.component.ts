import { Component, inject, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { DynamicStepperComponent } from './components/dynamic-stepper/dynamic-stepper.component';
import { NursingCartComponent } from './components/nursing-cart/nursing-cart.component';
import { StepCatalogSearchComponent } from './components/step-panels/step-catalog-search.component';
import { StepDoctorSelectComponent } from './components/step-panels/step-doctor-select.component';
import { StepLabRxPriceComponent } from './components/step-panels/step-lab-rx-price.component';
import { StepQuantityComponent } from './components/step-panels/step-quantity.component';
import { StepConfirmComponent } from './components/step-panels/step-confirm.component';
import { AuthService } from '../../core/services/auth.service';
import { environment } from '../../../environments/environment';
import { MedicoService, Medico } from '../../core/services/medico.service';
import { MultiSedeService, AreaClinica } from '../../core/services/multi-sede.service';
import { PatientService, PatientRecord } from '../../core/services/patient.service';
import { FacturacionService } from '../../core/services/facturacion.service';
import { 
  LucideAngularModule, 
  Search, 
  RefreshCcw, 
  Check, 
  X, 
  ChevronRight, 
  ChevronDown, 
  AlertCircle, 
  Info, 
  Calendar, 
  User, 
  Plus, 
  Stethoscope, 
  LogOut, 
  Activity, 
  FileText, 
  Heart, 
  Clipboard, 
  Thermometer, 
  Droplet, 
  Shuffle, 
  Edit,
  UserPlus
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

// Item classification Constants & Types (Specification / Strategy Pattern)
export const ITEM_CLASSIFICATIONS = {
  CONSULTA: 'Consulta',
  LABORATORIO: 'Laboratorio',
  RX: 'RX',
  MEDICAMENTO: 'Medicamento',
  PROCEDIMIENTO: 'Procedimiento',
} as const;

export type ItemClassification = (typeof ITEM_CLASSIFICATIONS)[keyof typeof ITEM_CLASSIFICATIONS];

export const CATEGORY_IDS = {
  CONSULTA: 1,
  LABORATORIO: 2,
  RADIOLOGIA: 3,
  MEDICAMENTO: 4,
  IMAGENOLOGIA: 6,
} as const;

interface ClassificationRule {
  classification: ItemClassification;
  categoryIds: number[];
  keywords: string[];
  customCheck?: (s: ServicioCatalogo) => boolean;
}

export const CLASSIFICATION_RULES: readonly ClassificationRule[] = [
  {
    classification: ITEM_CLASSIFICATIONS.CONSULTA,
    categoryIds: [CATEGORY_IDS.CONSULTA],
    keywords: ['CONSULTA', 'MEDICO', 'CITA'],
    customCheck: (s) => Boolean(s.isConsultation)
  },
  {
    classification: ITEM_CLASSIFICATIONS.LABORATORIO,
    categoryIds: [CATEGORY_IDS.LABORATORIO],
    keywords: ['LAB', 'PERFIL', 'LABORATORIO']
  },
  {
    classification: ITEM_CLASSIFICATIONS.RX,
    categoryIds: [CATEGORY_IDS.RADIOLOGIA, CATEGORY_IDS.IMAGENOLOGIA],
    keywords: ['RX', 'RAYOS', 'RADIOLOGIA', 'TOMOGRAFIA', 'RADIOGRAF', 'ECO', 'TOMOGRAF']
  },
  {
    classification: ITEM_CLASSIFICATIONS.MEDICAMENTO,
    categoryIds: [CATEGORY_IDS.MEDICAMENTO],
    keywords: ['INSUMO', 'MEDICAMENTO', 'FARMACIA', 'AMPOLLA', 'TABLETA']
  }
];

/**
 * Pure strategy function to classify catalog services based on rules engine
 */
export function classifyService(service: ServicioCatalogo | null | undefined): ItemClassification {
  if (!service) return ITEM_CLASSIFICATIONS.PROCEDIMIENTO;

  const cat = service.categoryId;
  const tipoUpper = (service.tipo || '').toUpperCase();
  const descUpper = (service.descripcion || '').toUpperCase();
  const honorCatUpper = (service.honorariumCategory || '').toUpperCase();

  const matchesText = (keywords: string[]) => 
    keywords.some(kw => tipoUpper.includes(kw) || descUpper.includes(kw) || honorCatUpper.includes(kw));

  for (const rule of CLASSIFICATION_RULES) {
    if ((cat !== undefined && rule.categoryIds.includes(cat)) || rule.customCheck?.(service) || matchesText(rule.keywords)) {
      return rule.classification;
    }
  }

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
    StepConfirmComponent
  ],
  templateUrl: './enfermeria.component.html',
  styleUrls: ['./enfermeria.component.css']
})
export class EnfermeriaComponent implements OnInit {
  private readonly http = inject(HttpClient);
  private readonly authService = inject(AuthService);
  private readonly medicoService = inject(MedicoService);
  public readonly multiSedeService = inject(MultiSedeService);
  private readonly patientService = inject(PatientService);
  private readonly facturacionService = inject(FacturacionService);

  readonly icons = {
    Search,
    RefreshCcw,
    Check,
    X,
    ChevronRight,
    ChevronDown,
    AlertCircle,
    Info,
    Calendar,
    User,
    Plus,
    Stethoscope,
    LogOut,
    Activity,
    FileText,
    Heart,
    Clipboard,
    Thermometer,
    Droplet,
    Shuffle,
    Edit,
    UserPlus
  };

  // Admission flow states (V1.2.92 Onboarding)
  public showIngresoModal = signal<boolean>(false);
  public ingresoStep = signal<number>(1);
  public showNewPatientForm = signal<boolean>(false);
  public searchIngresoTerm = signal<string>('');
  public isSearchingPatient = signal<boolean>(false);
  public patientsEncontrados = signal<PatientRecord[]>([]);
  public selectedPatientForIngreso = signal<PatientRecord | null>(null);
  public convenioIngresoId = signal<number | null>(null);
  public errorMessage = signal<string | null>(null);
  public codigosCelular = ['0416', '0426', '0414', '0424', '0412', '0422'];

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
    tipoCorreo: DEFAULT_TRIAGE.TIPO_CORREO,
    codigoCelular: DEFAULT_TRIAGE.CODIGO_CELULAR,
    codigoTelefono: DEFAULT_TRIAGE.CODIGO_TELEFONO
  };

  // Triage Signals (for Emergency complete flow)
  public triageSelectedPatientId = signal<string | null>(null);
  public triageMotivoConsulta = signal<string>('');
  public triageTensionArterial = signal<string>('');
  public triageFrecuenciaCardiaca = signal<number>(80);
  public triageFrecuenciaRespiratoria = signal<number>(18);
  public triageTemperatura = signal<number>(37.0);
  public triageSaturacionO2 = signal<number>(98);
  public triageGlicemiaCapilar = signal<number | null>(null);
  public triageClasificacion = signal<string>(DEFAULT_TRIAGE.CLASIFICACION);
  public triageEstadoConciencia = signal<string>(DEFAULT_TRIAGE.ESTADO_CONCIENCIA);
  public triageGlasgowOcular = signal<number>(4);
  public triageGlasgowVerbal = signal<number>(5);
  public triageGlasgowMotor = signal<number>(6);
  public triageGlasgowTotal = computed(() => {
    return this.triageGlasgowOcular() + this.triageGlasgowVerbal() + this.triageGlasgowMotor();
  });
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

  // Determinar dinámicamente el tipo de ingreso según el área clínica activa en Enfermería
  public type = computed(() => this.nursingAreaFilter());


  // State Lists
  public activeAccounts = signal<CuentaAdministrativa[]>([]);
  public convenios = signal<Convenio[]>([]);
  public servicesCatalog = signal<ServicioCatalogo[]>([]);
  public medicos = signal<Medico[]>([]);
  
  // Selected Patient / Timeline
  public selectedAccount = signal<CuentaAdministrativa | null>(null);
  public nursingHistory = signal<TriageRecord[]>([]);
  
  // Tab / Filter UI
  public activeTab = signal<string>('triage'); // triage, fast-charge, transfer
  public searchTerm = signal<string>('');
  public isLoading = signal<boolean>(false);
  public actionMessage = signal<string | null>(null);

  // Modular update selectors
  public registrarConstantesVitales = true;
  public registrarValoracionFisica = true;
  public registrarAntecedentes = true;
  public registrarEstadoActual = true;

  // Descriptions
  public descripcionRapida = '';
  public descripcionDetallada = '';

  // Vital Signs Form
  public motivoConsulta = '';
  public tensionArterial = '';
  public frecuenciaCardiaca = 0;
  public frecuenciaRespiratoria = 0;
  public temperatura = 37;
  public saturacionO2 = 98;
  public glicemiaCapilar: number | null = null;

  // Physical Assessment Form
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

  // Editing state
  public showTriageForm = false;
  public isEditingTriage = false;
  public editingTriageId: string | null = null;
  public editingValoracionId: string | null = null;

  // Fast Charge Medication autocomplete
  public fastChargeSearchTerm = signal<string>('');
  public filteredServices = signal<ServicioCatalogo[]>([]);
  public selectedService = signal<ServicioCatalogo | null>(null);
  public selectedMedicoId = signal<string | null>(null);
  public selectedAreaClinicaId = signal<string | null>(null);
  public areasClinicas = signal<AreaClinica[]>([]);
  public fastChargeQuantity = 1;
  public isSavingFastCharge = signal<boolean>(false);

  // Stepper & Pricing properties
  public currentStep = signal<number>(1);
  public activeStepperMode = signal<StepperMode>('catalog');
  public cartItems = signal<CartItem[]>([]);
  public activeSuggestions = signal<ServicioCatalogo[]>([]);
  public selectedSuggestions = signal<Record<string, boolean>>({});

  public cartTotalUSD = computed(() => 
    this.cartItems().reduce((acc, item) => acc + (item.precioBase + item.honorario) * item.cantidad, 0)
  );

  public cartItemCount = computed(() => this.cartItems().length);
  public nursingAreaFilter = signal<'Emergencia' | 'Hospitalizacion' | 'UCI'>('Emergencia');
  public customPrecio = signal<number | null>(null);
  public customHonorario = signal<number | null>(null);

  // Computed Item Classification delegated to pure rules engine
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

  public precioFinalCalculado = computed<number>(() => {
    const s = this.selectedService();
    if (!s) return 0;
    const classification = this.itemClassification();
    const customPriceVal = this.customPrecio();
    const defaultHonorary = s.honorarioBase ?? 0;
    const isConsult = classification === ITEM_CLASSIFICATIONS.CONSULTA || (s.categoryId === 1);
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

  public incrementQuantity(): void {
    this.fastChargeQuantity = (Number(this.fastChargeQuantity) || 1) + 1;
  }

  public decrementQuantity(): void {
    if (this.fastChargeQuantity > 1) {
      this.fastChargeQuantity = Number(this.fastChargeQuantity) - 1;
    }
  }

  // Transfer Area Form
  public nuevoTipoIngreso = 'Hospitalizacion'; // UCI, Hospitalizacion, Emergencia, etc.
  public nuevoConvenioId: number | null = null;
  public esEgreso = false;
  public isSavingTransfer = signal<boolean>(false);

  // Filtered active accounts computed list
  public filteredAccounts = computed(() => {
    const list = this.activeAccounts();
    const term = this.searchTerm().trim().toLowerCase();
    
    // Filtro estricto de enfermería: Solo Emergencia, Hospitalizacion y UCI
    const nursingList = list.filter(acc => 
      acc.tipoIngreso === 'Emergencia' || 
      acc.tipoIngreso === 'Hospitalizacion' || 
      acc.tipoIngreso === 'UCI'
    );

    // Filtro por área clínica seleccionada
    const areaFiltered = nursingList.filter(acc => acc.tipoIngreso === this.nursingAreaFilter());

    if (!term) return areaFiltered;
    return areaFiltered.filter(acc => 
      acc.pacienteNombre.toLowerCase().includes(term) || 
      acc.pacienteCedula.toLowerCase().includes(term) ||
      acc.tipoIngreso.toLowerCase().includes(term)
    );
  });

  public estadoActualPaciente = computed(() => {
    const history = this.nursingHistory();
    if (!history || history.length === 0) return null;
    const latestWithState = history.find(h => h.descripcionRapida || h.descripcionDetallada);
    return latestWithState || history[0];
  });

  ngOnInit(): void {
    this.refreshAccounts();
    this.loadCatalogAndConvenios();
  }

  public refreshAccounts(): void {
    this.isLoading.set(true);
    // Cargar cuentas administrativas abiertas
    this.http.get<CuentaAdministrativa[]>(`${environment.apiUrl}/api/Billing/cuentas-administrativas?estado=Abierta`)
      .subscribe({
        next: (res) => {
          this.activeAccounts.set(res);
          this.isLoading.set(false);
          // Auto re-seleccionar la cuenta activa actual para ver cambios
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

  private loadCatalogAndConvenios(): void {
    // Cargar catalogo unificado de insumos/medicamentos/servicios
    this.http.get<ServicioCatalogo[]>(`${environment.apiUrl}/api/Catalog/unified`)
      .subscribe({
        next: (res) => {
          // Allow all items in the catalog for charging
          this.servicesCatalog.set(res);
        },
        error: (err) => console.error('[ENFERMERIA] Error loading catalog:', err)
      });

    // Cargar convenios
    this.http.get<Convenio[]>(`${environment.apiUrl}/api/Convenios`)
      .subscribe({
        next: (res) => this.convenios.set(res),
        error: (err) => console.error('[ENFERMERIA] Error loading convenios:', err)
      });

    // Cargar médicos activos
    this.medicoService.getAll().subscribe({
      next: (res) => this.medicos.set(res.filter(m => m.activo)),
      error: (err) => console.error('[ENFERMERIA] Error loading medicos:', err)
    });

    // Cargar áreas clínicas (Sedes)
    this.http.get<AreaClinica[]>(`${environment.apiUrl}/api/AreaClinica`).subscribe({
      next: (res) => {
        const activeAreas = res.filter(a => a.activo);
        this.areasClinicas.set(activeAreas);
        if (this.selectedAccount()) {
          this.autoSelectAreaClinicaForAccount(this.selectedAccount());
        }
      },
      error: (err) => console.error('[ENFERMERIA] Error loading areas clinicas:', err)
    });
  }

  public autoSelectAreaClinicaForAccount(account: CuentaAdministrativa | null): void {
    if (!account) return;
    const areas = this.areasClinicas();
    if (!areas || areas.length === 0) return;

    const ingresoNorm = (account.tipoIngreso || '').toLowerCase().trim();
    
    // Buscar coincidencia por nombre o código con el tipoIngreso del paciente (Emergencia, Hospitalizacion, UCI, etc.)
    const matched = areas.find(a => {
      const nameNorm = (a.nombre || '').toLowerCase().trim();
      const codeNorm = (a.codigo || '').toLowerCase().trim();
      return nameNorm.includes(ingresoNorm) || ingresoNorm.includes(nameNorm) || codeNorm === ingresoNorm;
    });

    if (matched) {
      this.selectedAreaClinicaId.set(matched.id);
    } else if (areas.length > 0 && !this.selectedAreaClinicaId()) {
      this.selectedAreaClinicaId.set(areas[0].id);
    }
  }

  public selectAccount(account: CuentaAdministrativa): void {
    this.selectedAccount.set(account);
    this.loadTriageHistory(account.cuentaId);
    this.resetTriageForm();
    this.activeTab.set('triage');
    
    // Auto-set the current convenio for transfer panel
    this.nuevoConvenioId = account.convenioId;
    this.nuevoTipoIngreso = account.tipoIngreso;
    this.esEgreso = false;

    // Auto pre-select Area Clinica matching patient's admission area / current location
    this.autoSelectAreaClinicaForAccount(account);
  }

  public loadTriageHistory(cuentaId: string): void {
    this.http.get<TriageRecord[]>(`${environment.apiUrl}/api/Enfermeria/triageHistorial/${cuentaId}`)
      .subscribe({
        next: (res) => {
          this.nursingHistory.set(res);
          if (res.length > 0) {
            this.showTriageForm = false;
          } else {
            this.showTriageForm = true;
          }
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
    
    if (this.nursingHistory().length > 0) {
      this.showTriageForm = false;
    } else {
      this.showTriageForm = true;
    }
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
    // Cargar registro seleccionado en el formulario para editarlo
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

    // Scroll vertical del panel al formulario
    setTimeout(() => {
      const formElement = document.getElementById('nursingForm');
      if (formElement) {
        formElement.scrollIntoView({ behavior: 'smooth' });
      }
    }, 50);
  }

  // Fast Charge Medication Autocomplete
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
    this.fastChargeQuantity = 1;
    this.selectedMedicoId.set(null);

    // Auto pre-select Area Clinica from active account
    if (this.selectedAccount()) {
      this.autoSelectAreaClinicaForAccount(this.selectedAccount());
    }

    // Inicializar precio base del catálogo y honorario por defecto
    this.customPrecio.set(service.precioUsd ?? 0);
    this.customHonorario.set(service.honorarioBase ?? 0);

    // Determinar modo del stepper
    const classification = classifyService(service);
    this.activeStepperMode.set(this.mapClassificationToMode(classification));

    // Sugerencias Dinámicas desde Maestro de Servicios / DB
    const sugIds = service.sugerenciasIds || service.SugerenciasIds || [];
    const suggestions = this.servicesCatalog().filter(item => sugIds.includes(String(item.id)));
    this.activeSuggestions.set(suggestions);

    const initialSelection: Record<string, boolean> = {};
    suggestions.forEach(s => initialSelection[s.id] = true);
    this.selectedSuggestions.set(initialSelection);

    // Avanzar al Paso 2 del Stepper
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

  public addCurrentItemToCart(): void {
    const active = this.selectedAccount();
    const service = this.selectedService();
    if (!active || !service) return;

    const getUuid = () => typeof crypto !== 'undefined' && crypto.randomUUID ? crypto.randomUUID() : 'id_' + Math.random().toString(36).substring(2, 9) + Date.now().toString(36);

    const classification = this.itemClassification();
    const isFixedQty = classification === ITEM_CLASSIFICATIONS.CONSULTA || 
                       classification === ITEM_CLASSIFICATIONS.LABORATORIO || 
                       classification === ITEM_CLASSIFICATIONS.RX;
    const effectiveQty = isFixedQty ? 1 : Number(this.fastChargeQuantity);

    const requiresMedico = classification === ITEM_CLASSIFICATIONS.CONSULTA || 
      ((service.honorarioBase ?? 0) > 0 && classification !== ITEM_CLASSIFICATIONS.RX && classification !== ITEM_CLASSIFICATIONS.LABORATORIO);
    
    if (requiresMedico && !this.selectedMedicoId()) {
      alert('Por favor, seleccione el médico tratante para la consulta.');
      return;
    }

    const defaultHonorary = service.honorarioBase ?? 0;
    const isConsult = classification === ITEM_CLASSIFICATIONS.CONSULTA || (service.categoryId === 1);
    const pureBasePrice = service.precioUsd ?? 0;
    const basePrice = this.customPrecio() !== null ? Number(this.customPrecio()) : pureBasePrice;
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
      areaClinicaId: this.selectedAreaClinicaId(),
      areaClinicaNombre: this.getAreaClinicaNombre(this.selectedAreaClinicaId()),
      unidadMedida: service.unidadMedida || 'UD'
    };

    const newItems: CartItem[] = [mainItem];

    // Sugerencias Dinámicas
    const activeSugs = this.activeSuggestions();
    const selSugs = this.selectedSuggestions();
    for (const sug of activeSugs) {
      if (selSugs[sug.id]) {
        const sugClass = classifyService(sug);
        const sugDefaultHonorary = sug.honorarioBase ?? 0;
        const sugIsConsult = sugClass === ITEM_CLASSIFICATIONS.CONSULTA || (sug.categoryId === 1);
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

  public getAreaClinicaNombre(areaId: string | null): string {
    if (!areaId) return '';
    const a = this.areasClinicas().find(x => x.id === areaId);
    return a ? a.nombre.toUpperCase() : 'DESCONOCIDA';
  }

  // Clinical Transfer / Discharge
  public submitTransfer(): void {
    const active = this.selectedAccount();
    if (!active) return;

    this.isSavingTransfer.set(true);

    const payload = {
      pacienteId: active.pacienteId,
      nuevoTipoIngreso: this.esEgreso ? 'Particular' : this.nuevoTipoIngreso, // Alta closes only
      nuevoConvenioId: this.esEgreso ? null : this.nuevoConvenioId,
      usuarioTraslado: '', // Se sobreescribe en Backend
      esEgreso: this.esEgreso
    };

    this.http.post(`${environment.apiUrl}/api/Enfermeria/Traslado`, payload)
      .subscribe({
        next: () => {
          if (this.esEgreso) {
            this.showSuccess('Paciente dado de alta administrativamente. La cuenta actual ha sido cerrada.');
          } else {
            this.showSuccess(`Paciente trasladado exitosamente a la ubicación: ${this.nuevoTipoIngreso}.`);
          }
          this.selectedAccount.set(null);
          this.nursingHistory.set([]);
          this.refreshAccounts();
          this.isSavingTransfer.set(false);
        },
        error: (err) => {
          alert('Error al procesar traslado: ' + (err.error?.Error || err.message));
          this.isSavingTransfer.set(false);
        }
      });
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

  public getMedicoEspecialidad(medicoId: string | null): string {
    if (!medicoId) return '';
    const m = this.medicos().find(x => x.id === medicoId);
    return m ? (m.especialidad?.toUpperCase() || 'GENERAL') : 'GENERAL';
  }

  // --- Ingresar Paciente / Abrir Cuenta Clinica Flow (V1.2.92) ---
  public abrirModalIngreso() {
    this.searchIngresoTerm.set('');
    this.patientsEncontrados.set([]);
    this.selectedPatientForIngreso.set(null);
    this.convenioIngresoId.set(null);
    this.showNewPatientForm.set(false);
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
      tipoCorreo: DEFAULT_TRIAGE.TIPO_CORREO,
      codigoCelular: DEFAULT_TRIAGE.CODIGO_CELULAR,
      codigoTelefono: DEFAULT_TRIAGE.CODIGO_TELEFONO
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

    // Si fechaNacimientoFormatted tiene un valor válido de 10 caracteres, actualizar la propiedad subyacente
    if (this.fechaNacimientoFormatted && this.fechaNacimientoFormatted.length === 10) {
      const parts = this.fechaNacimientoFormatted.split('-');
      if (parts.length === 3) {
        this.newPatientData.fechaNacimiento = `${parts[2]}-${parts[1]}-${parts[0]}`;
      }
    }

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
    this.facturacionService.abrirCuenta(pacienteId, this.type(), this.convenioIngresoId()).subscribe({
      next: () => {
        this.isLoading.set(false);
        this.showIngresoModal.set(false);
        this.actionMessage.set(`Paciente ingresado exitosamente a la sección de ${this.type()}.`);
        setTimeout(() => this.actionMessage.set(null), 5000);
        this.refreshAccounts(); // Refrescar lista de activos en Enfermería
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
    this.facturacionService.abrirCuenta(pacienteId, 'Emergencia', this.convenioIngresoId()).subscribe({
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
            this.refreshAccounts(); // Refrescar lista de activos en Enfermería
          },
          error: (err: any) => {
            this.isLoading.set(false);
            // Mostramos éxito del ingreso pero advertimos sobre el triage
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

  // Formateador de Fecha de Nacimiento
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
