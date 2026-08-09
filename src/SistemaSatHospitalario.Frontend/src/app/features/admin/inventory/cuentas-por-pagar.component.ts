import { Component, inject, OnInit, OnDestroy, signal, computed } from '@angular/core';
import { CommonModule, DecimalPipe, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subscription } from 'rxjs';
import { CuentasPorPagarService } from '../../../core/services/cuentas-por-pagar.service';
import { SettingsService } from '../../../core/services/settings.service';
import { OrdenCompraInventario, PagoProveedor, RegistrarPagoRequest } from '../../../core/models/cuentas-por-pagar.model';

@Component({
  selector: 'app-cuentas-por-pagar',
  standalone: true,
  imports: [CommonModule, FormsModule, DecimalPipe, DatePipe],
  templateUrl: './cuentas-por-pagar.component.html'
})
export class CuentasPorPagarComponent implements OnInit, OnDestroy {
  private cuentasService = inject(CuentasPorPagarService);
  private settingsService = inject(SettingsService);
  private tasaSubscription?: Subscription;

  // Estado Principal
  public activeTab = signal<'ordenes' | 'historial'>('ordenes');
  public ordenes = signal<OrdenCompraInventario[]>([]);
  public historialPagos = signal<PagoProveedor[]>([]);
  public isLoading = signal<boolean>(false);

  // Filtros
  public estadoFilter = signal<'PorPagar' | 'Pagado' | 'Todos'>('PorPagar');
  public searchQuery = signal<string>('');
  public fechaDesdeFilter = signal<string>('');
  public fechaHastaFilter = signal<string>('');

  // Tasa de Cambio
  public tasaOficial = signal<number>(50.00);

  // Modal de Pago / Abono
  public showPaymentModal = signal<boolean>(false);
  public selectedOrden = signal<OrdenCompraInventario | null>(null);
  public montoAbonarInput = signal<number>(0);
  public tasaCambioInput = signal<number>(50.00);
  public metodoPagoInput = signal<string>('Transferencia');
  public referenciaInput = signal<string>('');
  public observacionesInput = signal<string>('');
  public isSubmittingPago = signal<boolean>(false);

  // Mensajes de Alerta
  public errorMessage = signal<string | null>(null);
  public successMessage = signal<string | null>(null);

  // Computados
  public saldoRestanteCalculado = computed(() => {
    const orden = this.selectedOrden();
    if (!orden) return 0;
    const abono = this.montoAbonarInput() || 0;
    return Math.max(0, orden.saldoPendienteUSD - abono);
  });

  public montoAbonarBsCalculado = computed(() => {
    const abono = this.montoAbonarInput() || 0;
    const tasa = this.tasaCambioInput() || 1;
    return abono * tasa;
  });

  public totalPorPagarUSD = computed(() => 
    this.ordenes().filter(o => o.estado === 'PorPagar').reduce((acc, o) => acc + o.saldoPendienteUSD, 0)
  );

  public totalPagadoUSD = computed(() => 
    this.ordenes().reduce((acc, o) => acc + o.totalAbonadoUSD, 0)
  );

  ngOnInit(): void {
    this.tasaSubscription = this.settingsService.tasa$.subscribe(val => {
      if (val > 0) {
        this.tasaOficial.set(val);
        this.tasaCambioInput.set(val);
      }
    });

    this.cargarDatos();
  }

  cargarDatos(): void {
    this.isLoading.set(true);
    if (this.activeTab() === 'ordenes') {
      this.cuentasService.getOrdenes(
        this.estadoFilter(),
        this.searchQuery(),
        this.fechaDesdeFilter(),
        this.fechaHastaFilter()
      ).subscribe({
        next: (data) => {
          this.ordenes.set(data || []);
          this.isLoading.set(false);
        },
        error: () => {
          this.ordenes.set([]);
          this.isLoading.set(false);
        }
      });
    } else {
      this.cuentasService.getHistorialPagos(
        this.searchQuery(),
        this.fechaDesdeFilter(),
        this.fechaHastaFilter()
      ).subscribe({
        next: (data) => {
          this.historialPagos.set(data || []);
          this.isLoading.set(false);
        },
        error: () => {
          this.historialPagos.set([]);
          this.isLoading.set(false);
        }
      });
    }
  }

  setTab(tab: 'ordenes' | 'historial'): void {
    this.activeTab.set(tab);
    this.cargarDatos();
  }

  setEstadoFilter(estado: 'PorPagar' | 'Pagado' | 'Todos'): void {
    this.estadoFilter.set(estado);
    this.cargarDatos();
  }

  onSearchChange(val: string): void {
    this.searchQuery.set(val);
    this.cargarDatos();
  }

  openPaymentModal(orden: OrdenCompraInventario): void {
    this.selectedOrden.set(orden);
    this.montoAbonarInput.set(orden.saldoPendienteUSD);
    this.tasaCambioInput.set(this.tasaOficial());
    this.metodoPagoInput.set('Transferencia');
    this.referenciaInput.set('');
    this.observacionesInput.set('');
    this.errorMessage.set(null);
    this.successMessage.set(null);
    this.showPaymentModal.set(true);
  }

  closePaymentModal(): void {
    this.showPaymentModal.set(false);
    this.selectedOrden.set(null);
  }

  confirmarPago(): void {
    const orden = this.selectedOrden();
    if (!orden) return;

    const abono = this.montoAbonarInput();
    if (!abono || abono <= 0) {
      this.errorMessage.set('El monto a abonar debe ser mayor a cero.');
      return;
    }

    if (abono > orden.saldoPendienteUSD) {
      this.errorMessage.set(`El abono ($${abono.toFixed(2)}) supera el saldo pendiente ($${orden.saldoPendienteUSD.toFixed(2)}).`);
      return;
    }

    if (!this.referenciaInput().trim()) {
      this.errorMessage.set('Debe ingresar un número de referencia para la auditoría.');
      return;
    }

    this.isSubmittingPago.set(true);
    this.errorMessage.set(null);

    const request: RegistrarPagoRequest = {
      ordenCompraId: orden.id,
      montoAbonadoUSD: abono,
      tasaCambio: this.tasaCambioInput() || this.tasaOficial(),
      metodoPago: this.metodoPagoInput(),
      referencia: this.referenciaInput().trim(),
      observaciones: this.observacionesInput().trim()
    };

    this.cuentasService.registrarPago(request).subscribe({
      next: () => {
        this.isSubmittingPago.set(false);
        this.successMessage.set(`Pago registrado exitosamente para la orden #${orden.numeroFactura}.`);
        setTimeout(() => {
          this.closePaymentModal();
          this.cargarDatos();
        }, 1200);
      },
      error: (err) => {
        this.isSubmittingPago.set(false);
        const msg = err?.error?.message || 'Ocurrió un error al procesar el pago al proveedor.';
        this.errorMessage.set(msg);
      }
    });
  }

  ngOnDestroy(): void {
    if (this.tasaSubscription) {
      this.tasaSubscription.unsubscribe();
    }
  }
}
