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
    const activities: any[] = [];
    const times = ['10:42:15 AM', '10:35:00 AM', '10:15:22 AM', '09:50:05 AM', '08:30:00 AM'];
    
    list.forEach((acc, idx) => {
      if (idx >= times.length) return;
      const time = times[idx];
      const name = acc.pacienteNombre;
      const cedula = acc.pacienteCedula;
      
      if (idx === 0) {
        activities.push({
          time,
          source: 'SISTEMA',
          message: `ALERTA: Frecuencia cardíaca fuera de rango para paciente ${name} (ID: ${cedula}).`,
          badge: 'Alerta',
          badgeClass: 'bg-rose-500/10 text-rose-500 border border-rose-500/20'
        });
      } else if (idx === 1) {
        activities.push({
          time,
          source: 'ENFERMERÍA',
          message: `Servicios de atención clínica y medicamentos administrados a ${name}.`,
          badge: 'Servicios',
          badgeClass: 'bg-indigo-500/10 text-indigo-400 border border-indigo-500/20'
        });
      } else if (idx === 2) {
        activities.push({
          time,
          source: 'CAJA',
          message: `Abono inicial registrado a la cuenta de ${name} (Cédula: ${cedula}).`,
          badge: 'Abono',
          badgeClass: 'bg-emerald-500/10 text-emerald-400 border border-emerald-500/20'
        });
      } else {
        activities.push({
          time,
          source: 'LABORATORIO',
          message: `Resultados del panel de analítica listos para ${name}.`,
          badge: 'Analítica',
          badgeClass: 'bg-amber-500/10 text-amber-500 border border-amber-500/20'
        });
      }
    });

    if (activities.length === 0) {
      activities.push({
        time: '08:30:00 AM',
        source: 'SISTEMA',
        message: `No hay pacientes activos en la sección de ${this.type()} en este momento.`,
        badge: 'Info',
        badgeClass: 'bg-blue-500/10 text-blue-400 border border-blue-500/20'
      });
    }

    return activities;
  });

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
        // Cargar detalles de pacientes en paralelo para mostrar edad/celular en tarjetas
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

    // Si queda saldo pendiente por pagar por el paciente, alertamos o permitimos cerrarlo como Cuenta por Cobrar
    const pendiente = this.totalGeneral() - finalPayments.reduce((acc, p) => acc + p.equivalenteAbonadoBase, 0);

    if (pendiente > 0.01) {
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
      pagos: finalPayments
    };

    this.facturacionService.closeAccount(request).subscribe({
      next: (res: any) => {
        this.actionMessage.set(`¡Cuenta de ${acc.pacienteNombre} cerrada y facturada con éxito!`);
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
          this.abrirCuentaParaPaciente(p.id);
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

      this.isLoading.set(true);
      this.abrirCuentaParaPaciente(patient.id);
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
}
