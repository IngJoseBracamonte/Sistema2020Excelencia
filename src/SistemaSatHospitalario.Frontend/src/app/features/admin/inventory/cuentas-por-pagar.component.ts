import { Component, inject, OnInit, OnDestroy, signal, computed } from '@angular/core';
import { CommonModule, DecimalPipe, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subscription } from 'rxjs';
import { CuentasPorPagarService } from '../../../core/services/cuentas-por-pagar.service';
import { SettingsService } from '../../../core/services/settings.service';
import { CatalogService } from '../../../core/services/catalog.service';
import { OrdenCompraInventario, PagoProveedor, RegistrarPagoRequest, CatalogoMetodoPagoDto, MONEDA_USD_ID, MONEDA_BS_ID } from '../../../core/models/cuentas-por-pagar.model';

@Component({
  selector: 'app-cuentas-por-pagar',
  standalone: true,
  imports: [CommonModule, FormsModule, DecimalPipe, DatePipe],
  templateUrl: './cuentas-por-pagar.component.html'
})
export class CuentasPorPagarComponent implements OnInit, OnDestroy {
  private cuentasService = inject(CuentasPorPagarService);
  private settingsService = inject(SettingsService);
  private catalogService = inject(CatalogService);
  private tasaSubscription?: Subscription;

  // Estado Principal
  public activeTab = signal<'ordenes' | 'historial'>('ordenes');
  public ordenes = signal<OrdenCompraInventario[]>([]);
  public historialPagos = signal<PagoProveedor[]>([]);
  public isLoading = signal<boolean>(false);

  // Catálogo de Métodos de Pago DB-Driven
  public metodosPago = signal<CatalogoMetodoPagoDto[]>([]);

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
  public metodoPagoInput = signal<string>('');
  public referenciaInput = signal<string>('');
  public observacionesInput = signal<string>('');
  public isSubmittingPago = signal<boolean>(false);

  // Mensajes de Alerta
  public errorMessage = signal<string | null>(null);
  public successMessage = signal<string | null>(null);

  // Computados DB-Driven
  public esMetodoBs = computed(() => {
    const selectedId = this.metodoPagoInput();
    const metodo = this.metodosPago().find(m => m.id === selectedId);
    return metodo ? metodo.monedaId === MONEDA_BS_ID : false;
  });

  public montoAbonoUSDCalculado = computed(() => {
    const val = this.montoAbonarInput() || 0;
    const tasa = this.tasaCambioInput() || 1;
    if (this.esMetodoBs()) {
      return tasa > 0 ? this.redondearDosDecimales(val / tasa) : 0;
    }
    return val;
  });

  public montoAbonarBsCalculado = computed(() => {
    const val = this.montoAbonarInput() || 0;
    const tasa = this.tasaCambioInput() || 1;
    if (this.esMetodoBs()) {
      return val;
    }
    return this.redondearDosDecimales(val * tasa);
  });

  public saldoRestanteCalculado = computed(() => {
    const orden = this.selectedOrden();
    if (!orden) return 0;
    const abonoUSD = this.montoAbonoUSDCalculado();
    return Math.max(0, this.redondearDosDecimales(orden.saldoPendienteUSD - abonoUSD));
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

    this.cargarMetodosPagoDb();
    this.cargarDatos();
  }

  cargarMetodosPagoDb(): void {
    this.catalogService.getPaymentMethods().subscribe({
      next: (res: any[]) => {
        const mapped: CatalogoMetodoPagoDto[] = (res || []).map(x => ({
          id: String(x.id || x.valor || x.value || x.nombre),
          nombre: x.nombre || x.name || x.valor,
          monedaId: (x.grupoMoneda === 2 || x.grupoMoneda === 'VES') ? MONEDA_BS_ID : MONEDA_USD_ID,
          activo: x.activo ?? true
        })).filter(m => m.activo);

        this.metodosPago.set(mapped);
      },
      error: () => {
        const defaultMethods: CatalogoMetodoPagoDto[] = [
          { id: '1', nombre: 'EFECTIVO USD ($)', monedaId: MONEDA_USD_ID, activo: true },
          { id: '2', nombre: 'ZELLE ($)', monedaId: MONEDA_USD_ID, activo: true },
          { id: '3', nombre: 'TRANSFERENCIA USD ($)', monedaId: MONEDA_USD_ID, activo: true },
          { id: '4', nombre: 'PAGO MÓVIL (Bs.)', monedaId: MONEDA_BS_ID, activo: true },
          { id: '5', nombre: 'TRANSFERENCIA (Bs.)', monedaId: MONEDA_BS_ID, activo: true },
          { id: '6', nombre: 'EFECTIVO (Bs.)', monedaId: MONEDA_BS_ID, activo: true },
          { id: '7', nombre: 'PUNTO DE VENTA (Bs.)', monedaId: MONEDA_BS_ID, activo: true }
        ];
        this.metodosPago.set(defaultMethods);
      }
    });
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

  onMetodoPagoChange(metodoPagoId: string): void {
    this.metodoPagoInput.set(metodoPagoId);
    
    const orden = this.selectedOrden();
    if (!orden) return;

    const metodoSeleccionado = this.metodosPago().find(m => m.id === metodoPagoId);
    if (!metodoSeleccionado) return;

    if (metodoSeleccionado.monedaId === MONEDA_BS_ID) {
      const tasa = this.tasaCambioInput() || this.tasaOficial();
      const montoCalculadoBs = this.redondearDosDecimales(orden.saldoPendienteUSD * tasa);
      this.montoAbonarInput.set(montoCalculadoBs);
    } else {
      this.montoAbonarInput.set(orden.saldoPendienteUSD);
    }
  }

  public redondearDosDecimales(valor: number): number {
    return Math.round(valor * 100) / 100;
  }

  openPaymentModal(orden: OrdenCompraInventario): void {
    this.selectedOrden.set(orden);
    this.tasaCambioInput.set(this.tasaOficial());
    this.referenciaInput.set('');
    this.observacionesInput.set('');
    this.errorMessage.set(null);
    this.successMessage.set(null);

    const metodos = this.metodosPago();
    if (metodos.length > 0) {
      const defaultId = metodos[0].id;
      this.metodoPagoInput.set(defaultId);
      this.onMetodoPagoChange(defaultId);
    } else {
      this.montoAbonarInput.set(orden.saldoPendienteUSD);
    }

    this.showPaymentModal.set(true);
  }

  closePaymentModal(): void {
    this.showPaymentModal.set(false);
    this.selectedOrden.set(null);
  }

  confirmarPago(): void {
    const orden = this.selectedOrden();
    if (!orden) return;

    const abonoUSD = this.montoAbonoUSDCalculado();
    if (!abonoUSD || abonoUSD <= 0) {
      this.errorMessage.set('El monto a abonar debe ser mayor a cero.');
      return;
    }

    if (abonoUSD > orden.saldoPendienteUSD + 0.01) {
      this.errorMessage.set(`El abono ($${abonoUSD.toFixed(2)}) supera el saldo pendiente ($${orden.saldoPendienteUSD.toFixed(2)}).`);
      return;
    }

    if (!this.referenciaInput().trim()) {
      this.errorMessage.set('Debe ingresar un número de referencia para la auditoría.');
      return;
    }

    this.isSubmittingPago.set(true);
    this.errorMessage.set(null);

    const metodoSeleccionado = this.metodosPago().find(m => m.id === this.metodoPagoInput());
    const nombreMetodo = metodoSeleccionado ? metodoSeleccionado.nombre : this.metodoPagoInput();

    const request: RegistrarPagoRequest = {
      ordenCompraId: orden.id,
      montoAbonadoUSD: abonoUSD,
      tasaCambio: this.tasaCambioInput() || this.tasaOficial(),
      metodoPago: nombreMetodo,
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
