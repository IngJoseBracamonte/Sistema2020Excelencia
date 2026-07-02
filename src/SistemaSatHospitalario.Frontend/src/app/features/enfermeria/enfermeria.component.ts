import { Component, inject, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
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
  [key: string]: any;
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
  imports: [CommonModule, FormsModule, LucideAngularModule],
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
      next: (res) => this.areasClinicas.set(res.filter(a => a.activo)),
      error: (err) => console.error('[ENFERMERIA] Error loading areas clinicas:', err)
    });
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

  public selectCatalogService(service: ServicioCatalogo): void {
    this.selectedService.set(service);
    this.fastChargeSearchTerm.set(service.descripcion);
    this.filteredServices.set([]);
    this.fastChargeQuantity = 1;
    this.selectedMedicoId.set(null);

    // Inicializar precio base del catálogo y honorario por defecto
    this.customPrecio.set(service.precioUsd ?? 0);
    this.customHonorario.set(service.honorarioBase ?? 0);

    // Avanzar al Paso 2 del Stepper
    this.currentStep.set(2);
  }

  public submitFastCharge(): void {
    const active = this.selectedAccount();
    const service = this.selectedService();
    if (!active || !service) return;

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

    this.isSavingFastCharge.set(true);

    const payload: Record<string, any> = {
      pacienteId: active.pacienteId,
      tipoIngreso: active.tipoIngreso,
      convenioId: active.convenioId,
      servicioId: service.id,
      descripcion: service.descripcion,
      precio: service.precioUsd,
      honorario: service.honorarioBase,
      cantidad: effectiveQty,
      tipoServicio: service.tipo,
      usuarioCarga: '' // Se sobreescribe en Backend
    };

    if (this.selectedMedicoId()) {
      payload['medicoId'] = this.selectedMedicoId();
      payload['horaCita'] = new Date().toISOString(); // Default current time for instant service/consultation
    }

    if (this.selectedAreaClinicaId()) {
      payload['areaClinicaId'] = this.selectedAreaClinicaId();
    }

    if (classification === ITEM_CLASSIFICATIONS.CONSULTA) {
      payload['precioModificado'] = this.precioFinalCalculado();
      payload['honorarioModificado'] = Number(this.customHonorario() ?? 0);
    }

    this.http.post(`${environment.apiUrl}/api/Billing/CargarServicio`, payload)
      .subscribe({
        next: () => {
          this.showSuccess(`Se cargó ${this.fastChargeQuantity} ${service.unidadMedida || 'Unidad(es)'} de ${service.descripcion} a la cuenta del paciente.`);
          this.selectedService.set(null);
          this.fastChargeSearchTerm.set('');
          this.fastChargeQuantity = 1;
          this.selectedMedicoId.set(null);
          this.selectedAreaClinicaId.set(null);
          this.customPrecio.set(null);
          this.customHonorario.set(null);
          this.currentStep.set(1); // Reiniciar Stepper
          this.isSavingFastCharge.set(false);
          this.refreshAccounts(); // Refresh to see total updates
        },
        error: (err) => {
          alert('Error al cargar insumo: ' + (err.error?.Error || err.error?.message || err.message));
          this.isSavingFastCharge.set(false);
        }
      });
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
