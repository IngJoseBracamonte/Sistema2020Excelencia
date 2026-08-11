import { Component, inject, signal, OnInit, computed } from '@angular/core';
import { CommonModule, DecimalPipe, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { InventoryService } from '../../../core/services/inventory.service';
import { CuentasPorPagarService } from '../../../core/services/cuentas-por-pagar.service';
import { SettingsService } from '../../../core/services/settings.service';
import { Insumo, RecordPurchase, PurchaseItem, PrincipioActivo, Proveedor } from '../../../core/models/inventory.model';
import { OrdenCompraInventario, PagoProveedor, RegistrarPagoRequest } from '../../../core/models/cuentas-por-pagar.model';
import { SearchFocusDirective } from '../../../shared/directives/search-focus.directive';
import { 
  LucideAngularModule, 
  ShoppingCart, 
  Search, 
  Plus, 
  Trash2, 
  Check, 
  AlertCircle,
  Package,
  Layers,
  Building2,
  FileText,
  DollarSign,
  History,
  UserPlus,
  Phone,
  MapPin,
  X
} from 'lucide-angular';

interface CartItem {
  insumoId: string;
  codigo: string;
  nombre: string;
  cantidad: number;
  precioCostoUSD: number;
  totalUSD: number;
  principiosActivosNames?: string[];
}

@Component({
  selector: 'app-compras',
  standalone: true,
  imports: [CommonModule, FormsModule, DecimalPipe, DatePipe, LucideAngularModule, SearchFocusDirective],
  templateUrl: './compras.component.html'
})
export class ComprasComponent implements OnInit {
  private inventoryService = inject(InventoryService);
  private cuentasService = inject(CuentasPorPagarService);
  private settingsService = inject(SettingsService);

  public insumos = signal<Insumo[]>([]);
  public principiosActivosList = signal<PrincipioActivo[]>([]);

  // Buscador y Selección de Proveedores
  public proveedoresList = signal<Proveedor[]>([]);
  public proveedorSearchTerm = signal<string>('');
  public filteredProveedores = signal<Proveedor[]>([]);
  public selectedProveedor = signal<Proveedor | null>(null);
  public showProveedorDropdown = signal<boolean>(false);

  // Modal para Crear Nuevo Proveedor
  public showNewProveedorModal = signal<boolean>(false);
  public newProveedorRif = signal<string>('');
  public newProveedorRazonSocial = signal<string>('');
  public newProveedorDireccion = signal<string>('');
  public newProveedorTelefono = signal<string>('');
  public isSavingProveedor = signal<boolean>(false);

  // Datos de Proveedor e Insumo
  public proveedorNombreInput = signal<string>('');
  public numeroFacturaInput = signal<string>('');
  public tasaOficial = signal<number>(50.00);

  // Búsqueda e Insumo Seleccionado
  public purchaseSearchTerm = signal<string>('');
  public filteredInsumos = signal<Insumo[]>([]);
  public selectedInsumo = signal<Insumo | null>(null);

  // Formulario por renglón
  public purchaseQty = signal<number>(1);
  public costMode = signal<'total' | 'unitario'>('total');
  public purchaseCost = signal<number>(0);
  
  // Carrito de Compras
  public cart = signal<CartItem[]>([]);
  public isSubmitting = signal<boolean>(false);

  // Órdenes de Compra y Cuentas por Pagar (Compartido)
  public activeOrdenTab = signal<'ordenes' | 'historial'>('ordenes');
  public ordenes = signal<OrdenCompraInventario[]>([]);
  public historialPagos = signal<PagoProveedor[]>([]);
  public isLoadingOrdenes = signal<boolean>(false);
  public estadoFilter = signal<'PorPagar' | 'Pagado' | 'Todos'>('PorPagar');
  public ordenSearchQuery = signal<string>('');

  // Modal de Pago / Abono (Compartido con Cuentas por Pagar)
  public showPaymentModal = signal<boolean>(false);
  public selectedOrden = signal<OrdenCompraInventario | null>(null);
  public montoAbonarInput = signal<number>(0);
  public tasaCambioInput = signal<number>(50.00);
  public metodoPagoInput = signal<string>('Transferencia');
  public referenciaInput = signal<string>('');
  public observacionesInput = signal<string>('');
  public isSubmittingPago = signal<boolean>(false);

  // Mensajes de Alerta
  public successMessage = signal<string | null>(null);
  public errorMessage = signal<string | null>(null);

  readonly icons = {
    ShoppingCart,
    Search,
    Plus,
    Trash2,
    Check,
    AlertCircle,
    Package,
    Layers,
    Building2,
    FileText,
    DollarSign,
    History,
    UserPlus,
    Phone,
    MapPin,
    X
  };

  public calculatedUnitCost = computed(() => {
    const qty = Number(this.purchaseQty());
    const cost = Number(this.purchaseCost());
    if (qty <= 0) return 0;
    return this.costMode() === 'total' ? (cost / qty) : cost;
  });

  public calculatedTotalCost = computed(() => {
    const qty = Number(this.purchaseQty());
    const cost = Number(this.purchaseCost());
    if (qty <= 0) return 0;
    return this.costMode() === 'total' ? cost : (qty * cost);
  });

  public cartTotal = computed(() => {
    return this.cart().reduce((sum, item) => sum + item.totalUSD, 0);
  });

  public esMetodoBs = computed(() => {
    const m = (this.metodoPagoInput() || '').toUpperCase();
    return m.includes('BS') || m.includes('PAGO MÓVIL') || m.includes('PAGO MOVIL') || m.includes('PUNTO');
  });

  public montoAbonoUSDCalculado = computed(() => {
    const val = this.montoAbonarInput() || 0;
    const tasa = this.tasaCambioInput() || 1;
    if (this.esMetodoBs()) {
      return tasa > 0 ? Math.round((val / tasa) * 100) / 100 : 0;
    }
    return val;
  });

  public montoAbonarBsCalculado = computed(() => {
    const val = this.montoAbonarInput() || 0;
    const tasa = this.tasaCambioInput() || 1;
    if (this.esMetodoBs()) {
      return val;
    }
    return Math.round(val * tasa * 100) / 100;
  });

  public saldoRestanteCalculado = computed(() => {
    const orden = this.selectedOrden();
    if (!orden) return 0;
    const abonoUSD = this.montoAbonoUSDCalculado();
    return Math.max(0, Math.round((orden.saldoPendienteUSD - abonoUSD) * 100) / 100);
  });

  ngOnInit() {
    this.settingsService.tasa$.subscribe(val => {
      if (val > 0) {
        this.tasaOficial.set(val);
        this.tasaCambioInput.set(val);
      }
    });

    this.loadInsumos();
    this.loadPrincipiosActivos();
    this.loadProveedores();
    this.cargarOrdenes();
  }

  loadProveedores(query?: string) {
    this.inventoryService.getProveedores(query).subscribe({
      next: (res) => {
        this.proveedoresList.set(res || []);
        this.filteredProveedores.set(res || []);
      },
      error: (err) => console.error('Error al cargar proveedores', err)
    });
  }

  onProveedorSearchChange(val: string) {
    this.proveedorSearchTerm.set(val);
    const term = val.trim().toLowerCase();
    if (term) {
      this.filteredProveedores.set(
        this.proveedoresList().filter(p => 
          p.razonSocial.toLowerCase().includes(term) ||
          p.rif.toLowerCase().includes(term)
        )
      );
    } else {
      this.filteredProveedores.set(this.proveedoresList());
    }
    this.showProveedorDropdown.set(true);
  }

  selectProveedor(prov: Proveedor) {
    this.selectedProveedor.set(prov);
    this.proveedorNombreInput.set(prov.razonSocial);
    this.proveedorSearchTerm.set(`${prov.razonSocial} (${prov.rif})`);
    this.showProveedorDropdown.set(false);
  }

  clearSelectedProveedor() {
    this.selectedProveedor.set(null);
    this.proveedorNombreInput.set('');
    this.proveedorSearchTerm.set('');
    this.showProveedorDropdown.set(false);
  }

  openNewProveedorModal() {
    this.newProveedorRif.set('');
    this.newProveedorRazonSocial.set('');
    this.newProveedorDireccion.set('');
    this.newProveedorTelefono.set('');
    this.showNewProveedorModal.set(true);
  }

  closeNewProveedorModal() {
    this.showNewProveedorModal.set(false);
  }

  saveNewProveedor() {
    const rif = this.newProveedorRif().trim();
    const razonSocial = this.newProveedorRazonSocial().trim();

    if (!rif) {
      this.showError('El RIF del proveedor es requerido.');
      return;
    }
    if (!razonSocial) {
      this.showError('La Razón Social del proveedor es requerida.');
      return;
    }

    this.isSavingProveedor.set(true);
    this.inventoryService.createProveedor({
      rif,
      razonSocial,
      direccion: this.newProveedorDireccion().trim() || undefined,
      telefono: this.newProveedorTelefono().trim() || undefined
    }).subscribe({
      next: (newProv) => {
        this.isSavingProveedor.set(false);
        this.closeNewProveedorModal();
        this.showSuccess(`Proveedor '${newProv.razonSocial}' registrado exitosamente.`);
        this.loadProveedores();
        this.selectProveedor(newProv);
      },
      error: (err) => {
        this.isSavingProveedor.set(false);
        this.showError(err.error?.message || 'Error al guardar el nuevo proveedor.');
      }
    });
  }

  loadInsumos() {
    this.inventoryService.getInsumos(true).subscribe({
      next: (res) => this.insumos.set(res),
      error: (err) => console.error('Error al cargar insumos', err)
    });
  }

  loadPrincipiosActivos() {
    this.inventoryService.getPrincipiosActivos().subscribe({
      next: (res) => this.principiosActivosList.set(res),
      error: (err) => console.error('Error al cargar principios activos', err)
    });
  }

  cargarOrdenes() {
    this.isLoadingOrdenes.set(true);
    if (this.activeOrdenTab() === 'ordenes') {
      this.cuentasService.getOrdenes(
        this.estadoFilter(),
        this.ordenSearchQuery()
      ).subscribe({
        next: (data) => {
          this.ordenes.set(data || []);
          this.isLoadingOrdenes.set(false);
        },
        error: () => {
          this.ordenes.set([]);
          this.isLoadingOrdenes.set(false);
        }
      });
    } else {
      this.cuentasService.getHistorialPagos(
        this.ordenSearchQuery()
      ).subscribe({
        next: (data) => {
          this.historialPagos.set(data || []);
          this.isLoadingOrdenes.set(false);
        },
        error: () => {
          this.historialPagos.set([]);
          this.isLoadingOrdenes.set(false);
        }
      });
    }
  }

  setOrdenTab(tab: 'ordenes' | 'historial') {
    this.activeOrdenTab.set(tab);
    this.cargarOrdenes();
  }

  setEstadoFilter(estado: 'PorPagar' | 'Pagado' | 'Todos') {
    this.estadoFilter.set(estado);
    this.cargarOrdenes();
  }

  onOrdenSearchChange(val: string) {
    this.ordenSearchQuery.set(val);
    this.cargarOrdenes();
  }

  onSearchChange(val: string) {
    this.purchaseSearchTerm.set(val);
    const term = val.trim().toLowerCase();
    if (term.length >= 1) {
      this.filteredInsumos.set(
        this.insumos().filter(i => 
          i.nombre.toLowerCase().includes(term) || 
          i.codigo.toLowerCase().includes(term) ||
          (i.principiosActivos && i.principiosActivos.some(pa => (pa.nombre || '').toLowerCase().includes(term)))
        )
      );
    } else {
      this.filteredInsumos.set([]);
    }
  }

  selectInsumo(insumo: Insumo) {
    this.selectedInsumo.set(insumo);
    this.purchaseSearchTerm.set(insumo.nombre);
    this.filteredInsumos.set([]);
    this.purchaseCost.set(insumo.costoUnitarioBaseUSD);
    this.purchaseQty.set(1);
    this.costMode.set('total');
  }

  addToCart() {
    const item = this.selectedInsumo();
    if (!item) return;

    if (this.purchaseQty() <= 0) {
      this.showError('La cantidad debe ser mayor a cero.');
      return;
    }

    if (!item.permiteFraccionamiento && !Number.isInteger(Number(this.purchaseQty()))) {
      this.showError(`El insumo '${item.nombre}' es de Dosis Entera y no permite fraccionamiento. Ingrese un valor entero.`);
      return;
    }

    if (this.purchaseCost() < 0) {
      this.showError('El costo no puede ser negativo.');
      return;
    }

    const qty = Number(this.purchaseQty());
    const unitCost = this.calculatedUnitCost();
    const totalUSD = this.calculatedTotalCost();

    const paNames = item.principiosActivos ? item.principiosActivos.map(pa => `${pa.nombre} (${pa.concentracion})`) : [];

    const existingIndex = this.cart().findIndex(c => c.insumoId === item.id);
    if (existingIndex > -1) {
      this.cart.update(prev => {
        const updated = [...prev];
        updated[existingIndex].cantidad = qty;
        updated[existingIndex].precioCostoUSD = unitCost;
        updated[existingIndex].totalUSD = totalUSD;
        return updated;
      });
    } else {
      this.cart.update(prev => [
        ...prev,
        {
          insumoId: item.id,
          codigo: item.codigo,
          nombre: item.nombre,
          cantidad: qty,
          precioCostoUSD: unitCost,
          totalUSD: totalUSD,
          principiosActivosNames: paNames
        }
      ]);
    }

    // Reset selección
    this.selectedInsumo.set(null);
    this.purchaseSearchTerm.set('');
    this.purchaseQty.set(1);
    this.purchaseCost.set(0);
    this.costMode.set('total');
  }

  removeFromCart(index: number) {
    this.cart.update(prev => prev.filter((_, i) => i !== index));
  }

  submitPurchase() {
    if (this.cart().length === 0) {
      this.showError('El carrito de recepción de compras está vacío.');
      return;
    }

    this.isSubmitting.set(true);

    const dto: RecordPurchase = {
      proveedorId: this.selectedProveedor()?.id,
      proveedorNombre: this.selectedProveedor()?.razonSocial || this.proveedorNombreInput().trim() || 'Proveedor General',
      numeroFactura: this.numeroFacturaInput().trim() || undefined,
      tasaCambio: this.tasaOficial(),
      items: this.cart().map(i => ({
        insumoId: i.insumoId,
        cantidad: i.cantidad,
        precioCostoUSD: i.precioCostoUSD
      }))
    };

    this.inventoryService.recordPurchase(dto).subscribe({
      next: () => {
        this.showSuccess('Ingreso de compras e inserción en Cuentas por Pagar procesados exitosamente.');
        this.cart.set([]);
        this.clearSelectedProveedor();
        this.numeroFacturaInput.set('');
        this.isSubmitting.set(false);
        this.loadInsumos();
        this.cargarOrdenes();
      },
      error: (err) => {
        this.showError(err.error?.message || 'Error al registrar el ingreso de compras.');
        this.isSubmitting.set(false);
      }
    });
  }

  // --- MÉTODOS DE PAGO Y ABONO COMPARTIDOS CON CUENTAS POR PAGAR ---
  onMetodoPagoChange(metodo: string): void {
    this.metodoPagoInput.set(metodo);
    const orden = this.selectedOrden();
    const tasa = this.tasaCambioInput() || this.tasaOficial();
    if (!orden) return;

    const esBs = metodo.toUpperCase().includes('BS') || metodo.toUpperCase().includes('PAGO MÓVIL') || metodo.toUpperCase().includes('PAGO MOVIL') || metodo.toUpperCase().includes('PUNTO');

    if (esBs) {
      this.montoAbonarInput.set(Math.round(orden.saldoPendienteUSD * tasa * 100) / 100);
    } else {
      this.montoAbonarInput.set(orden.saldoPendienteUSD);
    }
  }

  openPaymentModal(orden: OrdenCompraInventario) {
    this.selectedOrden.set(orden);
    this.metodoPagoInput.set('EFECTIVO USD');
    this.montoAbonarInput.set(orden.saldoPendienteUSD);
    this.tasaCambioInput.set(this.tasaOficial());
    this.referenciaInput.set('');
    this.observacionesInput.set('');
    this.errorMessage.set(null);
    this.showPaymentModal.set(true);
  }

  closePaymentModal() {
    this.showPaymentModal.set(false);
    this.selectedOrden.set(null);
  }

  confirmarPago() {
    const orden = this.selectedOrden();
    if (!orden) return;

    const abonoUSD = this.montoAbonoUSDCalculado();
    if (!abonoUSD || abonoUSD <= 0) {
      this.showError('El monto a abonar debe ser mayor a cero.');
      return;
    }

    if (abonoUSD > orden.saldoPendienteUSD + 0.01) {
      this.showError(`El abono ($${abonoUSD.toFixed(2)}) supera el saldo pendiente ($${orden.saldoPendienteUSD.toFixed(2)}).`);
      return;
    }

    if (!this.referenciaInput().trim()) {
      this.showError('Debe ingresar un número de referencia para la auditoría.');
      return;
    }

    this.isSubmittingPago.set(true);

    const request: RegistrarPagoRequest = {
      ordenCompraId: orden.id,
      montoAbonadoUSD: abonoUSD,
      tasaCambio: this.tasaCambioInput() || this.tasaOficial(),
      metodoPago: this.metodoPagoInput(),
      referencia: this.referenciaInput().trim(),
      observaciones: this.observacionesInput().trim()
    };

    this.cuentasService.registrarPago(request).subscribe({
      next: () => {
        this.isSubmittingPago.set(false);
        this.showSuccess(`Pago abonado exitosamente a la orden #${orden.numeroFactura}.`);
        this.closePaymentModal();
        this.cargarOrdenes();
      },
      error: (err) => {
        this.isSubmittingPago.set(false);
        this.showError(err?.error?.message || 'Ocurrió un error al procesar el pago al proveedor.');
      }
    });
  }

  private showSuccess(msg: string) {
    this.successMessage.set(msg);
    this.errorMessage.set(null);
    setTimeout(() => this.successMessage.set(null), 5000);
  }

  private showError(msg: string) {
    this.errorMessage.set(msg);
    this.successMessage.set(null);
    setTimeout(() => this.errorMessage.set(null), 5000);
  }
}
