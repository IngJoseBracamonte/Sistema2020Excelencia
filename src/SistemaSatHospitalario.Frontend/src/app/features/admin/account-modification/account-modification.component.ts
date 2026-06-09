import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { 
  LucideAngularModule, 
  Search, 
  RefreshCcw, 
  Check, 
  X, 
  ChevronRight, 
  AlertCircle,
  Info,
  User,
  Settings,
  DollarSign,
  FileText,
  Trash2,
  Plus,
  Clock,
  History
} from 'lucide-angular';
import { AdminBillingService, CuentaAdministrativaDto, DetallePrecioCorreccionDto, HistorialModificacionCuentaDto } from '../../../core/services/admin-billing.service';
import { PatientService, PatientRecord } from '../../../core/services/patient.service';
import { ConveniosService } from '../../../core/services/convenios.service';

@Component({
  selector: 'app-account-modification',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule],
  templateUrl: './account-modification.component.html',
  styleUrl: './account-modification.component.css'
})
export class AccountModificationComponent implements OnInit {
  readonly icons = {
    Search,
    RefreshCcw,
    Check,
    X,
    ChevronRight,
    AlertCircle,
    Info,
    User,
    Settings,
    DollarSign,
    FileText,
    Trash2,
    Plus,
    Clock,
    History
  };

  private adminBillingService = inject(AdminBillingService);
  private patientService = inject(PatientService);
  private conveniosService = inject(ConveniosService);

  // signals
  public searchTerm = signal<string>('');
  public accounts = signal<CuentaAdministrativaDto[]>([]);
  public isLoading = signal<boolean>(false);
  public isSaving = signal<boolean>(false);
  public actionMessage = signal<string | null>(null);
  public errorMessage = signal<string | null>(null);

  // Selected Account
  public selectedAccount = signal<CuentaAdministrativaDto | null>(null);
  
  // Modification Form States (signals)
  public modTipoIngreso = signal<string>('Particular');
  public modConvenioId = signal<number | null>(null);
  
  // Patient transfer search states
  public patientSearchTerm = signal<string>('');
  public foundPatients = signal<PatientRecord[]>([]);
  public selectedTargetPatient = signal<PatientRecord | null>(null);
  
  // Convenios list
  public convenios = signal<any[]>([]);

  // Price edits temporary list
  public editedPrices = signal<{[key: string]: {precio: number, honorario: number} | undefined}>({});

  // History list
  public modificationHistory = signal<HistorialModificacionCuentaDto[]>([]);

  // Computed total based on edited prices
  public computedNewTotal = computed(() => {
    const account = this.selectedAccount();
    if (!account) return 0;
    
    let total = 0;
    for (const d of account.detalles) {
      const edits = this.editedPrices()[d.id];
      const precio = edits ? edits.precio : d.precio;
      total += precio * d.cantidad;
    }
    return total;
  });

  ngOnInit() {
    this.loadConvenios();
  }

  loadConvenios() {
    this.conveniosService.getAll().subscribe({
      next: (res) => this.convenios.set(res),
      error: (err) => console.error('[ACCOUNT-MOD] Error cargando convenios:', err)
    });
  }

  searchAccounts() {
    if (!this.searchTerm().trim()) {
      this.errorMessage.set('Debe ingresar un término de búsqueda.');
      setTimeout(() => this.errorMessage.set(null), 5000);
      return;
    }

    this.isLoading.set(true);
    this.accounts.set([]);
    this.selectedAccount.set(null);
    this.selectedTargetPatient.set(null);

    this.adminBillingService.getCuentasAdministrativas(this.searchTerm()).subscribe({
      next: (res) => {
        this.accounts.set(res);
        this.isLoading.set(false);
        if (res.length === 0) {
          this.errorMessage.set('No se encontraron cuentas.');
          setTimeout(() => this.errorMessage.set(null), 5000);
        }
      },
      error: (err) => {
        this.errorMessage.set('Error al buscar cuentas: ' + (err.error?.Error || err.message));
        setTimeout(() => this.errorMessage.set(null), 5000);
        this.isLoading.set(false);
      }
    });
  }

  selectAccount(account: CuentaAdministrativaDto) {
    this.selectedAccount.set(account);
    this.modTipoIngreso.set(account.tipoIngreso);
    this.modConvenioId.set(account.convenioId || null);
    this.selectedTargetPatient.set(null);
    this.patientSearchTerm.set('');
    this.foundPatients.set([]);
    
    // Initialize price edits
    const edits: {[key: string]: {precio: number, honorario: number}} = {};
    for (const d of account.detalles) {
      edits[d.id] = { precio: d.precio, honorario: d.honorario };
    }
    this.editedPrices.set(edits);

    // Load history
    this.loadModificationHistory(account.cuentaId);
  }

  searchTargetPatients() {
    const term = this.patientSearchTerm().trim();
    if (term.length < 2) {
      this.foundPatients.set([]);
      return;
    }

    this.patientService.searchPatients(term).subscribe({
      next: (res) => {
        // filter out current patient
        const currentId = this.selectedAccount()?.pacienteId;
        this.foundPatients.set(res.filter(p => p.id !== currentId));
      },
      error: (err) => console.error('[ACCOUNT-MOD] Error al buscar pacientes destino:', err)
    });
  }

  selectTargetPatient(patient: PatientRecord) {
    this.selectedTargetPatient.set(patient);
    this.patientSearchTerm.set(`${patient.nombre} (${patient.cedula})`);
    this.foundPatients.set([]);
  }

  clearTargetPatient() {
    this.selectedTargetPatient.set(null);
    this.patientSearchTerm.set('');
    this.foundPatients.set([]);
  }

  updatePriceEdit(detailId: string, field: 'precio' | 'honorario', value: number) {
    this.editedPrices.update(prev => {
      const current = prev[detailId] || { precio: 0, honorario: 0 };
      return {
        ...prev,
        [detailId]: {
          ...current,
          [field]: Math.max(0, value)
        }
      };
    });
  }

  saveModifications() {
    const account = this.selectedAccount();
    if (!account) return;

    this.isSaving.set(true);
    this.errorMessage.set(null);
    this.actionMessage.set(null);

    // Prepare price corrections
    const correccionesPrecios: DetallePrecioCorreccionDto[] = [];
    const edits = this.editedPrices();
    for (const detailId in edits) {
      const orig = account.detalles.find(d => d.id === detailId);
      if (orig) {
        const edited = edits[detailId];
        if (edited && (edited.precio !== orig.precio || edited.honorario !== orig.honorario)) {
          correccionesPrecios.push({
            detalleId: detailId,
            nuevoPrecio: edited.precio,
            nuevoHonorario: edited.honorario
          });
        }
      }
    }

    // Determine type/convenio changes
    const convenioIdOriginal = account.convenioId ?? null;
    const convenioIdNuevo = this.modConvenioId() ?? null;
    const hasConvenioChange = convenioIdNuevo !== convenioIdOriginal;

    const tipoIngresoOriginal = account.tipoIngreso ?? null;
    const tipoIngresoNuevo = this.modTipoIngreso() ?? null;
    const hasTypeChange = (tipoIngresoNuevo !== tipoIngresoOriginal) || hasConvenioChange;

    const nuevoTipoIngreso = (tipoIngresoNuevo !== tipoIngresoOriginal) ? tipoIngresoNuevo : undefined;
    const nuevoConvenioId = hasConvenioChange ? convenioIdNuevo : undefined;
    
    // Check if anything actually changed
    const hasPatientChange = this.selectedTargetPatient() !== null;
    const hasPriceChange = correccionesPrecios.length > 0;

    if (!hasTypeChange && !hasPatientChange && !hasPriceChange) {
      this.errorMessage.set('No se han realizado modificaciones.');
      setTimeout(() => this.errorMessage.set(null), 5000);
      this.isSaving.set(false);
      return;
    }

    const command = {
      cuentaId: account.cuentaId,
      nuevoPacienteId: this.selectedTargetPatient()?.id || undefined,
      nuevoTipoIngreso: nuevoTipoIngreso,
      nuevoConvenioId: (this.modTipoIngreso() === 'Particular') ? null : (this.modConvenioId() || null),
      correccionesPrecios: correccionesPrecios.length > 0 ? correccionesPrecios : undefined
    };

    this.adminBillingService.updateCuentaAdministrativa(command as any).subscribe({
      next: (res) => {
        this.actionMessage.set('¡Cuenta modificada administrativamente con éxito!');
        this.isSaving.set(false);
        this.selectedAccount.set(null);
        this.searchAccounts(); // Refresh list
        setTimeout(() => this.actionMessage.set(null), 7000);
      },
      error: (err) => {
        this.errorMessage.set('Fallo al guardar modificaciones: ' + (err.error?.Error || err.error?.error || err.message));
        setTimeout(() => this.errorMessage.set(null), 7000);
        this.isSaving.set(false);
      }
    });
  }

  loadModificationHistory(cuentaId: string) {
    this.adminBillingService.getHistorialModificaciones(cuentaId).subscribe({
      next: (history) => this.modificationHistory.set(history),
      error: (err) => console.error('[ACCOUNT-MOD] Error cargando historial:', err)
    });
  }

  parseServiceChanges(jsonStr?: string): any[] {
    if (!jsonStr) return [];
    try {
      return JSON.parse(jsonStr);
    } catch (e) {
      return [];
    }
  }
}
