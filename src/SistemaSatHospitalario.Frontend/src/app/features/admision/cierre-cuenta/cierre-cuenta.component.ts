import { Component, inject, signal, computed, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AdminBillingService, CuentaAdministrativaDto } from '../../../core/services/admin-billing.service';
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
  imports: [CommonModule, FormsModule, LucideAngularModule],
  templateUrl: './cierre-cuenta.component.html',
  styleUrl: './cierre-cuenta.component.css'
})
export class CierreCuentaComponent implements OnInit, OnDestroy {
  // Services
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private adminBillingService = inject(AdminBillingService);
  private facturacionService = inject(FacturacionService);
  private patientService = inject(PatientService);
  private authService = inject(AuthService);
  private printService = inject(PrintService);
  private conveniosService = inject(ConveniosService);
  private settingsService = inject(SettingsService);
  private http = inject(HttpClient);
  private medicoService = inject(MedicoService);

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

  public newPatientData: any = {
    cedula: '',
    nombre: '',
    apellidos: '',
    sexo: 'M',
    fechaNacimiento: new Date().toISOString().split('T')[0],
    celular: '',
    codigoCelular: '0414',
    telefono: '',
    codigoTelefono: '0274',
    direccion: ''
  };

  public codigosCelular = ['0416', '0426', '0414', '0424', '0412', '0422'];
  public codigosTelefonoCombinados = ['0274', '0273', '0251', '0212', '0281', '0241', '0416', '0426', '0414', '0424', '0412', '0422'];

  // Selected Account State
  public selectedAccount = signal<CuentaAdministrativaDto | null>(null);
  public fechaEgreso = signal<string>(new Date().toISOString().split('T')[0]);
  public horaEgreso = signal<string>(new Date().toTimeString().substring(0, 5));
  public diagnostico = signal<string>('Diagnóstico General / Control Médico');
  public destinoPaciente = signal<string>('Alta Médica');
  public personalRelevo = signal<string>('');

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

  // Clinical Item Loading (Emergency Nurse Fast Charge)
  public servicesCatalog = signal<any[]>([]);
  public fastChargeSearchTerm = signal<string>('');
  public filteredServices = signal<any[]>([]);
  public selectedService = signal<any | null>(null);
  public fastChargeQuantity = signal<number>(1);
  public isSavingFastCharge = signal<boolean>(false);
  public triageHistoryMap = signal<Record<string, any[]>>({});
  // Medicos Catalog
  public medicos = signal<Medico[]>([]);
  public medicoTratanteId = signal<string | null>(null);

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
      const room = `${roomType} ${100 + (seed % 15)}${String.fromCharCode(65 + (seed % 3))}`;
      
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
  }

  ngOnDestroy() {
    if (this.paramSub) {
      this.paramSub.unsubscribe();
    }
  }

  // Cargar cuentas abiertas del tipo actual
  public loadOpenAccounts() {
    this.isLoading.set(true);
    this.adminBillingService.getCuentasAdministrativas(undefined, this.type(), 'Abierta').subscribe({
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
    this.newPatientData = {
      cedula: '',
      nombre: '',
      apellidos: '',
      sexo: 'M',
      fechaNacimiento: new Date().toISOString().split('T')[0],
      celular: '',
      codigoCelular: '0414',
      telefono: '',
      codigoTelefono: '0274',
      direccion: ''
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
    this.facturacionService.abrirCuenta(pacienteId, this.type(), this.convenioIngresoId()).subscribe({
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

  // --- Fast Charge / Add Service & Medicine Methods ---
  public onFastChargeSearchChange(val: string): void {
    this.fastChargeSearchTerm.set(val);
    const term = val.trim().toLowerCase();
    if (term.length >= 1) {
      const filtered = this.servicesCatalog().filter(s => 
        s.descripcion.toLowerCase().includes(term) || 
        (s.codigo && s.codigo.toLowerCase().includes(term))
      );
      this.filteredServices.set(filtered.slice(0, 10)); // Limit to top 10 results
    } else {
      this.filteredServices.set([]);
    }
  }

  public selectCatalogService(service: any): void {
    this.selectedService.set(service);
    this.fastChargeSearchTerm.set(service.descripcion);
    this.filteredServices.set([]);
    this.fastChargeQuantity.set(1);
  }

  public clearSelectedService(): void {
    this.selectedService.set(null);
    this.fastChargeSearchTerm.set('');
    this.filteredServices.set([]);
    this.fastChargeQuantity.set(1);
  }

  public submitFastCharge(): void {
    const active = this.selectedAccount();
    const service = this.selectedService();
    if (!active || !service) {
      this.errorMessage.set('Por favor, seleccione un servicio o insumo válido.');
      return;
    }

    if (this.fastChargeQuantity() <= 0) {
      this.errorMessage.set('La cantidad debe ser mayor a cero.');
      return;
    }

    this.isSavingFastCharge.set(true);
    this.errorMessage.set(null);

    const payload = {
      pacienteId: active.pacienteId,
      tipoIngreso: active.tipoIngreso,
      convenioId: active.convenioId,
      servicioId: service.id,
      descripcion: service.descripcion,
      precio: service.precioUsd,
      honorario: service.honorarioBase,
      cantidad: Number(this.fastChargeQuantity()),
      tipoServicio: service.tipo,
      usuarioCarga: this.authService.currentUser()?.username || 'admin'
    };

    this.http.post(`${environment.apiUrl}/api/Billing/CargarServicio`, payload)
      .subscribe({
        next: () => {
          this.actionMessage.set(`Se cargó exitosamente ${this.fastChargeQuantity()} unidad(es) de ${service.descripcion} a la cuenta.`);
          this.clearSelectedService();
          this.isSavingFastCharge.set(false);
          this.loadOpenAccounts(); // Auto-refresh selected account's details
          setTimeout(() => this.actionMessage.set(null), 3000);
        },
        error: (err) => {
          this.errorMessage.set('Error al cargar insumo: ' + (err.error?.Error || err.error?.message || err.message));
          this.isSavingFastCharge.set(false);
        }
      });
  }

  public obtenerNombreMedico(id: string | null): string {
    if (!id) return '--- SIN ASIGNAR ---';
    const medico = this.medicos().find(m => m.id === id);
    return medico ? medico.nombre : '--- SIN ASIGNAR ---';
  }
}
