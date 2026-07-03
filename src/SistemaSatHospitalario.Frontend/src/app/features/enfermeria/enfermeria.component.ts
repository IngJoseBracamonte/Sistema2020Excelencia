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
  Edit
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
    Edit
  };

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

  public precioFinalCalculado = computed<number>(() => {
    const s = this.selectedService();
    if (!s) return 0;
    const classification = this.itemClassification();
    const customPriceVal = this.customPrecio();
    const basePrice = customPriceVal ?? s.precioUsd ?? 0;

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
        next: (res) => this.nursingHistory.set(res),
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

    this.estadoConciencia = 'Alerta';
    this.glasgowOcular = 4;
    this.glasgowVerbal = 5;
    this.glasgowMotor = 6;
    this.glasgowTotal = 15;
    this.viaAerea = 'Permeable';
    this.ventilacion = 'Normal';
    this.pulso = 'Rítmico';
    this.pielMucosas = 'Normocoloreada';
    this.llenadoCapilar = '< 2 segundos';
    this.pupilas = 'Isocóricas';
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

    // Scroll vertical del panel al formulario
    const formElement = document.getElementById('nursingForm');
    if (formElement) {
      formElement.scrollIntoView({ behavior: 'smooth' });
    }
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

    const payload = {
      pacienteId: active.pacienteId,
      tipoIngreso: active.tipoIngreso,
      convenioId: active.convenioId,
      items: items.map(item => ({
        servicioId: item.servicioId,
        descripcion: item.descripcion,
        precio: item.precioBase,
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

  logout(): void {
    this.authService.logout();
  }
}
