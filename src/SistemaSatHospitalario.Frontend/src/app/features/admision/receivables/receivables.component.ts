import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { 
  LucideAngularModule, 
  DollarSign, 
  Search, 
  RefreshCcw, 
  Check, 
  X, 
  ChevronRight, 
  AlertCircle,
  Info,
  CreditCard,
  Plus,
  Trash2,
  Shield,
  Download
} from 'lucide-angular';
import { ReceivablesService, PendingAR, SettleARRequest, PaymentItemDto } from '../../../core/services/receivables.service';
import { FacturacionService } from '../../../core/services/facturacion.service';
import { SettingsService } from '../../../core/services/settings.service';
import { CatalogService } from '../../../core/services/catalog.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-receivables',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule, RouterLink],
  templateUrl: './receivables.component.html',
  styleUrl: './receivables.component.css'
})
export class ReceivablesComponent implements OnInit {
  readonly icons = {
    DollarSign,
    Search,
    RefreshCcw,
    Check,
    X,
    ChevronRight,
    AlertCircle,
    Info,
    CreditCard,
    Plus,
    Trash2,
    Shield,
    Download
  };
  private arService = inject(ReceivablesService);
  private facturacionService = inject(FacturacionService);
  private settingsService = inject(SettingsService);
  private catalogService = inject(CatalogService);
  private tasaSubscription?: Subscription;

  // Signals para estado
  public tasaCambio = signal<number>(0);
  public receivables = signal<PendingAR[]>([]);
  public searchTerm = signal<string>('');
  public filterEstado = signal<string>('Pendiente');
  public startDate = signal<string>(new Date().toISOString().split('T')[0]);
  public endDate = signal<string>(new Date().toISOString().split('T')[0]);
  public isLoading = signal<boolean>(false);
  public isSettling = signal<boolean>(false);

  public actionMessage = signal<string | null>(null);
  public errorMessage = signal<string | null>(null);

  // Gestión de Seguros (V12.1 Integration)
  public showInsuranceModal = signal(false);
  public isGarantia = signal(false);
  public isGenerating = signal(false);
  public compromisoData: any = {};

  // Accordion Logic (Angular-Pro Patterns)
  public expandedRows = signal<Set<string>>(new Set<string>());

  toggleRow(id: string) {
    const next = new Set(this.expandedRows());
    if (next.has(id)) next.delete(id);
    else next.add(id);
    this.expandedRows.set(next);
  }

  isExpanded(id: string): boolean {
    return this.expandedRows().has(id);
  }

  // Modal Settlement Premium (Refactored to Signals for Reactivity)
  public selectedAR = signal<PendingAR | null>(null);
  public payments = signal<PaymentItemDto[]>([]);
  public pmSelected = signal<string>('Punto');
  public amSelected = signal<number>(0);
  public rfSelected = signal<string>('');
  public catalogMethods = signal<any[]>([]);
  public catalogVueltos = signal<any[]>([]);
  public methodVuelto = signal<string>('');
  public observaciones = signal<string>('');

  // Currency helper (Reactive Signal dependency)
  public currentCurrency = computed(() => {
    const methodValue = this.pmSelected();
    const method = this.catalogMethods().find(m => m.value === methodValue);
    return method?.isUSD ? 'USD' : 'Bs.';
  });

  public currentChangeCurrency = computed(() => {
    const methodValue = this.methodVuelto();
    const method = this.catalogVueltos().find(m => m.value === methodValue);
    return method?.isUSD ? 'USD' : 'Bs.';
  });

  public totalPaidUSD = computed(() => {
    return this.payments().reduce((acc, curr) => acc + curr.amount, 0);
  });

  public initialDebtUSD = computed(() => {
    const ar = this.selectedAR();
    if (!ar) return 0;
    return ar.saldoPendiente; // Ya viene tabulado en USD Base
  });

  public requiresChange = computed(() => {
    return this.totalPaidUSD() > this.initialDebtUSD();
  });

  public changeAmountUSD = computed(() => {
    return this.requiresChange() ? this.totalPaidUSD() - this.initialDebtUSD() : 0;
  });

  public remainingBalanceUSD = computed(() => {
    const remaining = this.initialDebtUSD() - this.totalPaidUSD();
    return remaining > 0 ? remaining : 0;
  });

  public remainingBalanceBs = computed(() => {
    return this.remainingBalanceUSD() * this.tasaCambio();
  });

  ngOnInit() {
    this.refresh();
    this.loadPaymentCatalogs();
    // Forzar actualización de tasa al entrar al módulo (Senior Pattern)
    this.settingsService.refreshTasa();
    
    this.tasaSubscription = this.settingsService.tasa$.subscribe(tasa => {
      this.tasaCambio.set(tasa);
    });
  }

  loadPaymentCatalogs() {
    this.catalogService.getPaymentMethods().subscribe(res => {
      const methods = res.filter(x => !x.isVuelto);
      const changes = res.filter(x => x.isVuelto);
      
      this.catalogMethods.set(methods);
      this.catalogVueltos.set(changes);
      
      if (changes.length > 0) {
        this.methodVuelto.set(changes[0].value);
      }
      
      // Intentar setear un default si existe
      if (methods.some(m => m.value === 'Punto')) {
          this.pmSelected.set('Punto');
      } else if (methods.length > 0) {
          this.pmSelected.set(methods[0].value);
      }
    });
  }

  ngOnDestroy() {
    this.tasaSubscription?.unsubscribe();
  }

  refresh() {
    this.isLoading.set(true);
    this.arService.getPending(this.searchTerm(), this.filterEstado(), this.startDate(), this.endDate()).subscribe({
      next: (res: PendingAR[]) => {
        this.receivables.set(res);
        this.isLoading.set(false);
      },
      error: () => {
        this.errorMessage.set("Error al cargar cuentas por cobrar.");
        this.isLoading.set(false);
      }
    });
  }

  abrirCobro(ar: PendingAR) {
    this.selectedAR.set(ar);
    this.payments.set([]);
    this.observaciones.set('');
    this.resetNewPayment(ar.saldoPendiente);
  }

  onMethodChange(newMethod: string) {
    this.pmSelected.set(newMethod);
    this.amSelected.set(0); // Limpiar el monto al cambiar de método para evitar errores
  }

  resetNewPayment(amountUSD: number = 0) {
    const isUSD = this.catalogMethods().find(m => m.value === this.pmSelected())?.isUSD ?? false;
    const initialAmount = isUSD ? amountUSD : amountUSD * this.tasaCambio();
    
    this.amSelected.set(initialAmount > 0 ? initialAmount : 0);
    this.rfSelected.set('');
  }

  addPayment() {
    const amountOriginal = this.amSelected();
    if (amountOriginal <= 0) return;
    
    let amountUSD = 0;

    // Normalización: Todo se convierte a USD ($) para el monto base
    if (this.currentCurrency() === 'USD') {
        amountUSD = amountOriginal;
    } else {
        // Bs -> USD: Dividir por la tasa
        amountUSD = amountOriginal / this.tasaCambio();
    }

    this.payments.update(p => [...p, { 
        method: this.pmSelected(),
        amount: amountUSD, // Guardar normalizado en USD
        amountMoneda: amountOriginal, // Guardar original (Bs o $)
        tasaAplicada: this.tasaCambio(),
        reference: this.rfSelected()
    }]);
    
    this.resetNewPayment(0);
  }

  addVuelto() {
    const amountUSD = this.changeAmountUSD();
    if (amountUSD <= 0) return;

    const method = this.catalogVueltos().find(m => m.value === this.methodVuelto());
    const isUSD = method?.isUSD ?? false;
    
    const amountOriginal = isUSD ? amountUSD : amountUSD * this.tasaCambio();

    this.payments.update(p => [...p, { 
        method: this.methodVuelto(),
        amount: -amountUSD, // Valor negativo para salida de caja
        amountMoneda: -amountOriginal, // Valor negativo para salida
        tasaAplicada: this.tasaCambio(),
        reference: 'VUELTO AL PACIENTE'
    }]);
  }

  removePayment(index: number) {
    this.payments.update(p => p.filter((_, i) => i !== index));
    this.resetNewPayment(this.remainingBalanceBs());
  }

  liquidar() {
    if (!this.selectedAR() || this.payments().length === 0) return;

    if (this.tasaCambio() <= 0) {
      this.errorMessage.set("No se puede procesar el cobro: No existe una tasa de cambio cargada. Por favor, espere a que se sincronice o contacte al administrador.");
      return;
    }

    this.isSettling.set(true);
    const command: SettleARRequest = {
      arId: this.selectedAR()!.id,
      payments: this.payments(),
      observaciones: this.observaciones()
    };

    const paciente = this.selectedAR()?.pacienteNombre;
    const total = this.totalPaidUSD();

    this.arService.settle(command).subscribe({
      next: () => {
        this.isSettling.set(false);
        this.selectedAR.set(null);
        this.payments.set([]);
        this.refresh();
        
        // Mensaje de éxito "que anuncie bien" (Senior UX)
        this.actionMessage.set(`¡LIQUIDACIÓN COMPLETADA! Se procesó el cobro de $${total.toFixed(2)} para ${paciente} exitosamente.`);
        setTimeout(() => this.actionMessage.set(null), 8000); 
      },
      error: (err: any) => {
        this.isSettling.set(false);
        const serverError = err.error?.Error || err.error?.error || "Error al procesar el cobro.";
        alert(`Fallo en la liquidación:\n${serverError}`);
        this.errorMessage.set(serverError);
      }
    });
  }

  onFilterChange(newVal: any) {
    this.filterEstado.set(newVal);
    this.refresh();
  }

  // --- Lógica de Seguros (Reutilizada de Seguros Dashboard) ---
  openCompromiso(ar: PendingAR, garantia: boolean = false) {
    const today = new Date();
    const futureDate = new Date();
    futureDate.setDate(futureDate.getDate() + 30);

    this.isGarantia.set(garantia);
    this.compromisoData = {
      cuentaPorCobrarId: ar.id,
      nombreResponsable: ar.pacienteNombre,
      relacionResponsable: 'Titular',
      cedulaResponsable: ar.pacienteCedula,
      direccionResponsable: 'No especificada',
      telefonoResponsable: 'No especificado',
      conceptos: ar.conceptos?.map(c => c.descripcion).join(', ') || 'Servicios Médicos Hospitalarios',
      nombrePaciente: ar.pacienteNombre,
      edadPaciente: 0,
      cedulaPaciente: ar.pacienteCedula,
      montoTotal: ar.montoTotal,
      diasLiquidar: 30,
      cuotas: 1,
      fechaCompromiso: today.toISOString().split('T')[0],
      fechaVencimiento: futureDate.toISOString().split('T')[0]
    };
    this.showInsuranceModal.set(true);
  }

  generarPdfSeguro() {
    this.isGenerating.set(true);
    
    const futureDate = new Date(this.compromisoData.fechaCompromiso);
    futureDate.setDate(futureDate.getDate() + this.compromisoData.diasLiquidar);
    this.compromisoData.fechaVencimiento = futureDate.toISOString();

    const request = this.isGarantia() ? 
      this.facturacionService.generarGarantiaPdf(this.compromisoData) : 
      this.facturacionService.generarCompromisoPdf(this.compromisoData);

    request.subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        window.open(url, '_blank');
        this.isGenerating.set(false);
        this.showInsuranceModal.set(false);
        this.actionMessage.set(this.isGarantia() ? 'Garantía generada con éxito' : 'Compromiso de pago generado con éxito');
      },
      error: () => {
        this.errorMessage.set('Error al generar el documento PDF');
        this.isGenerating.set(false);
      }
    });
  }
}
