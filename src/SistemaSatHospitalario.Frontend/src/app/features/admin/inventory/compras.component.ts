import { Component, inject, signal, OnInit, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { InventoryService } from '../../../core/services/inventory.service';
import { MultiSedeService, Sede, PedidoInterSede } from '../../../core/services/multi-sede.service';
import { Insumo, CreateInsumo, UpdateInsumo, RecordPurchase, PurchaseItem } from '../../../core/models/inventory.model';
import { 
  LucideAngularModule, 
  Package, 
  Search, 
  Plus, 
  Edit, 
  Trash2, 
  Check, 
  X, 
  AlertCircle,
  FileText,
  RefreshCcw,
  PlusCircle,
  Calendar,
  Layers,
  Settings
} from 'lucide-angular';

interface CartItem {
  insumoId: string;
  codigo: string;
  nombre: string;
  cantidad: number;
  precioCostoUSD: number;
  fechaVencimiento?: string;
  totalUSD: number;
}

@Component({
  selector: 'app-compras',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule],
  templateUrl: './compras.component.html'
})
export class ComprasComponent implements OnInit {
  private inventoryService = inject(InventoryService);
  private multiSedeService = inject(MultiSedeService);

  // Lists
  public sedes = signal<Sede[]>([]);
  public insumos = signal<Insumo[]>([]);
  public pedidosPendientes = signal<PedidoInterSede[]>([]);

  // Navigation
  public activeTab = signal<'purchase' | 'pending' | 'crud'>('purchase');

  // Registrar Compra Form
  public selectedSedeId = signal<string>('');
  public purchaseSearchTerm = signal<string>('');
  public filteredPurchaseInsumos = signal<Insumo[]>([]);
  public selectedInsumo = signal<Insumo | null>(null);
  public purchaseQty = signal<number>(1);
  public purchaseCost = signal<number>(0);
  public purchaseVencimiento = signal<string>('');
  public cart = signal<CartItem[]>([]);
  public isSubmittingPurchase = signal<boolean>(false);

  // CRUD Form / Modal
  public crudSearchTerm = signal<string>('');
  public showCrudModal = signal<boolean>(false);
  public isEditingCrud = signal<boolean>(false);
  public currentInsumoId = signal<string | null>(null);

  public crudForm = {
    codigo: '',
    nombre: '',
    unidadMedidaBase: 'UNIDAD',
    costoUnitarioBaseUSD: 0,
    reactivosCombinados: '',
    indicaciones: '',
    fechaVencimiento: '',
    stockInicial: 0
  };

  public unidadesMedida = ['UNIDAD', 'KG', 'G', 'DG', 'MG', 'L', 'ML'];
  public categories = ['Medicamento', 'Descartable', 'Material Médico', 'Reactivo', 'Otro'];

  // Global Alerts
  public successMessage = signal<string | null>(null);
  public errorMessage = signal<string | null>(null);

  // Lucide Icons
  readonly icons = {
    Package,
    Search,
    Plus,
    Edit,
    Trash2,
    Check,
    X,
    AlertCircle,
    FileText,
    RefreshCcw,
    PlusCircle,
    Calendar,
    Layers,
    Settings
  };

  // Computed totals for purchase cart
  public cartTotal = computed(() => {
    return this.cart().reduce((sum, item) => sum + item.totalUSD, 0);
  });

  // Computed filter for catalog table
  public filteredInsumosCatalog = computed(() => {
    const term = this.crudSearchTerm().toLowerCase().trim();
    const list = this.insumos();

    if (!term) return list;

    return list.filter(i => 
      i.nombre.toLowerCase().includes(term) ||
      i.codigo.toLowerCase().includes(term) ||
      (i.reactivosCombinados || '').toLowerCase().includes(term)
    );
  });

  ngOnInit() {
    this.loadInitialData();
  }

  loadInitialData() {
    // Sedes
    this.multiSedeService.getSedes().subscribe({
      next: (res) => {
        const active = res.filter(s => s.activo);
        this.sedes.set(active);
        if (active.length > 0) {
          const principal = active.find(s => s.esPrincipal) || active[0];
          this.selectedSedeId.set(principal.id);
        }
      }
    });

    // Insumos (all of them, including hidden, to manage in CRUD)
    this.refreshInsumos();

    // Pedidos pendientes
    this.refreshPedidosPendientes();
  }

  refreshInsumos() {
    this.inventoryService.getInsumos(false).subscribe({
      next: (res) => this.insumos.set(res)
    });
  }

  refreshPedidosPendientes() {
    this.multiSedeService.getPedidosPendientes().subscribe({
      next: (res) => {
        // En farmacia aprobamos los pedidos que provee el Almacen Principal
        this.pedidosPendientes.set(res);
      }
    });
  }

  // --- TAB 1: REGISTRAR COMPRA ---
  onPurchaseSearchChange(val: string) {
    this.purchaseSearchTerm.set(val);
    const term = val.trim().toLowerCase();
    if (term.length >= 1) {
      this.filteredPurchaseInsumos.set(
        this.insumos().filter(i => 
          i.nombre.toLowerCase().includes(term) || 
          i.codigo.toLowerCase().includes(term) ||
          (i.reactivosCombinados || '').toLowerCase().includes(term)
        )
      );
    } else {
      this.filteredPurchaseInsumos.set([]);
    }
  }

  selectItem(insumo: Insumo) {
    this.selectedInsumo.set(insumo);
    this.purchaseSearchTerm.set(insumo.nombre);
    this.filteredPurchaseInsumos.set([]);
    this.purchaseCost.set(insumo.costoUnitarioBaseUSD);
    this.purchaseQty.set(1);
    if (insumo.fechaVencimiento) {
      this.purchaseVencimiento.set(insumo.fechaVencimiento.split('T')[0]);
    } else {
      this.purchaseVencimiento.set('');
    }
  }

  addToCart() {
    const item = this.selectedInsumo();
    if (!item) return;

    if (this.purchaseQty() <= 0) {
      this.showError('La cantidad debe ser mayor a cero.');
      return;
    }

    if (this.purchaseCost() < 0) {
      this.showError('El costo no puede ser negativo.');
      return;
    }

    const existingIndex = this.cart().findIndex(i => i.insumoId === item.id);
    const qty = Number(this.purchaseQty());
    const cost = Number(this.purchaseCost());
    const totalUSD = qty * cost;

    if (existingIndex > -1) {
      this.cart.update(prev => {
        const updated = [...prev];
        updated[existingIndex].cantidad = qty;
        updated[existingIndex].precioCostoUSD = cost;
        updated[existingIndex].totalUSD = totalUSD;
        if (this.purchaseVencimiento()) {
          updated[existingIndex].fechaVencimiento = this.purchaseVencimiento();
        }
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
          precioCostoUSD: cost,
          fechaVencimiento: this.purchaseVencimiento() || undefined,
          totalUSD: totalUSD
        }
      ]);
    }

    // Reset inputs
    this.selectedInsumo.set(null);
    this.purchaseSearchTerm.set('');
    this.purchaseQty.set(1);
    this.purchaseCost.set(0);
    this.purchaseVencimiento.set('');
  }

  removeFromCart(index: number) {
    this.cart.update(prev => prev.filter((_, i) => i !== index));
  }

  submitPurchase() {
    if (!this.selectedSedeId()) {
      this.showError('Debe seleccionar una sede para ingresar la compra.');
      return;
    }

    if (this.cart().length === 0) {
      this.showError('El carrito de compras está vacío.');
      return;
    }

    this.isSubmittingPurchase.set(true);

    const dto: RecordPurchase = {
      sedeId: this.selectedSedeId(),
      items: this.cart().map(item => ({
        insumoId: item.insumoId,
        cantidad: item.cantidad,
        precioCostoUSD: item.precioCostoUSD,
        fechaVencimiento: item.fechaVencimiento
      }))
    };

    this.inventoryService.recordPurchase(dto).subscribe({
      next: () => {
        this.showSuccess('Compra registrada e inventario actualizado exitosamente.');
        this.cart.set([]);
        this.isSubmittingPurchase.set(false);
        this.refreshInsumos();
      },
      error: (err) => {
        this.showError(err.error?.message || 'Error al registrar la compra.');
        this.isSubmittingPurchase.set(false);
      }
    });
  }

  // --- TAB 2: APROBAR PEDIDOS ---
  approvePedido(pedidoId: string) {
    if (!confirm('¿Está seguro de despachar y autorizar este pedido inter-sede?')) return;

    this.multiSedeService.despacharPedido(pedidoId).subscribe({
      next: () => {
        this.showSuccess('Pedido despachado y enviado correctamente.');
        this.refreshPedidosPendientes();
        this.refreshInsumos(); // refresh stock
      },
      error: (err) => {
        this.showError(err.error?.message || 'Error al despachar el pedido.');
      }
    });
  }

  // --- TAB 3: CRUD CATÁLOGO ---
  openCreateModal() {
    this.isEditingCrud.set(false);
    this.currentInsumoId.set(null);
    this.crudForm = {
      codigo: '',
      nombre: '',
      unidadMedidaBase: 'UNIDAD',
      costoUnitarioBaseUSD: 0,
      reactivosCombinados: '',
      indicaciones: '',
      fechaVencimiento: '',
      stockInicial: 0
    };
    this.showCrudModal.set(true);
  }

  openEditModal(insumo: Insumo) {
    this.isEditingCrud.set(true);
    this.currentInsumoId.set(insumo.id);
    this.crudForm = {
      codigo: insumo.codigo,
      nombre: insumo.nombre,
      unidadMedidaBase: insumo.unidadMedidaBase,
      costoUnitarioBaseUSD: insumo.costoUnitarioBaseUSD,
      reactivosCombinados: insumo.reactivosCombinados || '',
      indicaciones: insumo.indicaciones || '',
      fechaVencimiento: insumo.fechaVencimiento ? insumo.fechaVencimiento.split('T')[0] : '',
      stockInicial: 0
    };
    this.showCrudModal.set(true);
  }

  closeCrudModal() {
    this.showCrudModal.set(false);
  }

  saveCrudItem() {
    if (!this.crudForm.codigo || !this.crudForm.nombre) {
      alert('Código y Nombre son obligatorios.');
      return;
    }

    if (this.isEditingCrud()) {
      const id = this.currentInsumoId();
      if (!id) return;

      const dto: UpdateInsumo = {
        nombre: this.crudForm.nombre,
        costoUnitarioBaseUSD: this.crudForm.costoUnitarioBaseUSD,
        reactivosCombinados: this.crudForm.reactivosCombinados || undefined,
        indicaciones: this.crudForm.indicaciones || undefined,
        fechaVencimiento: this.crudForm.fechaVencimiento ? new Date(this.crudForm.fechaVencimiento).toISOString() : undefined
      };

      this.inventoryService.updateInsumo(id, dto).subscribe({
        next: () => {
          this.showSuccess('Insumo actualizado exitosamente.');
          this.closeCrudModal();
          this.refreshInsumos();
        },
        error: (err) => alert(err.error?.message || 'Error al guardar los cambios.')
      });
    } else {
      const dto: CreateInsumo = {
        codigo: this.crudForm.codigo,
        nombre: this.crudForm.nombre,
        stockInicial: this.crudForm.stockInicial,
        unidadMedidaBase: this.crudForm.unidadMedidaBase,
        costoUnitarioBaseUSD: this.crudForm.costoUnitarioBaseUSD,
        reactivosCombinados: this.crudForm.reactivosCombinados || undefined,
        indicaciones: this.crudForm.indicaciones || undefined,
        fechaVencimiento: this.crudForm.fechaVencimiento ? new Date(this.crudForm.fechaVencimiento).toISOString() : undefined
      };

      this.inventoryService.createInsumo(dto).subscribe({
        next: () => {
          this.showSuccess('Insumo creado exitosamente en el catálogo.');
          this.closeCrudModal();
          this.refreshInsumos();
        },
        error: (err) => alert(err.error?.message || 'Error al crear el insumo.')
      });
    }
  }

  toggleInsumoOculto(insumo: Insumo) {
    const ocultar = !insumo.ocultoEnTraslados;
    const msg = ocultar 
      ? '¿Está seguro de ocultar este insumo? No aparecerá en la sección de traslados/envíos inter-sede.' 
      : '¿Está seguro de restaurar este insumo? Volverá a aparecer en la sección de envíos.';

    if (!confirm(msg)) return;

    const req = ocultar 
      ? this.inventoryService.deleteInsumo(insumo.id)
      : this.inventoryService.restoreInsumo(insumo.id);

    req.subscribe({
      next: () => {
        this.showSuccess(ocultar ? 'Insumo ocultado de traslados con éxito.' : 'Insumo restaurado para traslados con éxito.');
        this.refreshInsumos();
      },
      error: (err) => {
        this.showError(err.error?.message || 'Error al cambiar estado del insumo.');
      }
    });
  }

  // Alert Helpers
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
