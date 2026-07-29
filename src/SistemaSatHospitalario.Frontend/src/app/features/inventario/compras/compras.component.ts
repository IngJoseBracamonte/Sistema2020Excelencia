import { Component, inject, signal, OnInit, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { InventoryService } from '../../../core/services/inventory.service';
import { Insumo, RecordPurchase, PurchaseItem, PrincipioActivo } from '../../../core/models/inventory.model';
import { 
  LucideAngularModule, 
  ShoppingCart, 
  Search, 
  Plus, 
  Trash2, 
  Check, 
  AlertCircle,
  Package,
  Layers
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
  imports: [CommonModule, FormsModule, LucideAngularModule],
  templateUrl: './compras.component.html'
})
export class ComprasComponent implements OnInit {
  private inventoryService = inject(InventoryService);

  public insumos = signal<Insumo[]>([]);
  public principiosActivosList = signal<PrincipioActivo[]>([]);

  // Búsqueda e Insumo Seleccionado
  public purchaseSearchTerm = signal<string>('');
  public filteredInsumos = signal<Insumo[]>([]);
  public selectedInsumo = signal<Insumo | null>(null);

  // Formulario por renglón
  public purchaseQty = signal<number>(1);
  public purchaseCost = signal<number>(0);
  
  // Carrito de Compras
  public cart = signal<CartItem[]>([]);
  public isSubmitting = signal<boolean>(false);

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
    Layers
  };

  public cartTotal = computed(() => {
    return this.cart().reduce((sum, item) => sum + item.totalUSD, 0);
  });

  ngOnInit() {
    this.loadInsumos();
    this.loadPrincipiosActivos();
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

    const qty = Number(this.purchaseQty());
    const cost = Number(this.purchaseCost());
    const totalUSD = qty * cost;

    const paNames = item.principiosActivos ? item.principiosActivos.map(pa => `${pa.nombre} (${pa.concentracion})`) : [];

    const existingIndex = this.cart().findIndex(c => c.insumoId === item.id);
    if (existingIndex > -1) {
      this.cart.update(prev => {
        const updated = [...prev];
        updated[existingIndex].cantidad = qty;
        updated[existingIndex].precioCostoUSD = cost;
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
          precioCostoUSD: cost,
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
      items: this.cart().map(i => ({
        insumoId: i.insumoId,
        cantidad: i.cantidad,
        precioCostoUSD: i.precioCostoUSD
      }))
    };

    this.inventoryService.recordPurchase(dto).subscribe({
      next: () => {
        this.showSuccess('Ingreso de compras registrado atómicamente en el Almacén Central.');
        this.cart.set([]);
        this.isSubmitting.set(false);
        this.loadInsumos();
      },
      error: (err) => {
        this.showError(err.error?.message || 'Error al registrar el ingreso de compras.');
        this.isSubmitting.set(false);
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
