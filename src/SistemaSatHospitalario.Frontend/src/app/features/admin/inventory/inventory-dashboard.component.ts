import { Component, inject, signal, OnInit, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { InventoryService } from '../../../core/services/inventory.service';
import { CatalogService, CatalogItem } from '../../../core/services/catalog.service';
import { 
  Insumo, 
  MovimientoInsumo, 
  ServicioInsumoReceta, 
  CreateInsumo, 
  RecordMovement, 
  CreateRecipe 
} from '../../../core/models/inventory.model';
import { 
  LucideAngularModule, 
  Package, 
  Search, 
  Plus, 
  Edit, 
  Trash2, 
  ClipboardList, 
  History, 
  X, 
  Check, 
  AlertCircle, 
  TrendingUp,
  DollarSign
} from 'lucide-angular';

@Component({
  selector: 'app-inventory-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule],
  templateUrl: './inventory-dashboard.component.html'
})
export class InventoryDashboardComponent implements OnInit {
  private inventoryService = inject(InventoryService);
  private catalogService = inject(CatalogService);

  // Lists
  public insumos = signal<Insumo[]>([]);
  public movements = signal<MovimientoInsumo[]>([]);
  public recipes = signal<ServicioInsumoReceta[]>([]);
  public services = signal<CatalogItem[]>([]);

  // State
  public activeTab = signal<'stock' | 'movement' | 'closing' | 'audit' | 'recipes'>('stock');
  public isLoading = signal<boolean>(false);
  public isSubmitting = signal<boolean>(false);

  // Search queries
  public searchQuery = signal<string>('');
  public recipeSearchQuery = signal<string>('');
  public movementSearchQuery = signal<string>('');

  // Modals
  public showInsumoModal = signal<boolean>(false);
  public isEditingInsumo = signal<boolean>(false);

  // Forms
  public insumoForm = signal<Partial<CreateInsumo>>({
    codigo: '',
    nombre: '',
    stockInicial: 0,
    unidadMedidaBase: 'UNIDAD',
    costoUnitarioBaseUSD: 0
  });
  public editingInsumoId = signal<string>('');

  public movementForm = signal<RecordMovement>({
    insumoId: '',
    tipoMovimiento: 'Ingreso',
    cantidadOriginal: 0,
    unidadMedidaOriginal: 'UNIDAD',
    motivo: ''
  });

  public recipeForm = signal<CreateRecipe>({
    servicioClinicoId: '',
    insumoId: '',
    cantidad: 0,
    unidadMedidaConsumo: 'UNIDAD'
  });

  public physicalCounts = signal<Record<string, number>>({});
  public closingObservations = signal<string>('');

  // Units
  public unidadesMedida = ['UNIDAD', 'KG', 'G', 'DG', 'MG', 'L', 'ML'];

  readonly icons = {
    Package,
    Search,
    Plus,
    Edit,
    Trash2,
    ClipboardList,
    History,
    X,
    Check,
    AlertCircle,
    TrendingUp,
    DollarSign
  };

  // Computeds for filtering
  public filteredInsumos = computed(() => {
    const list = this.insumos();
    const query = this.searchQuery().toLowerCase().trim();
    if (!query) return list;
    return list.filter(i => 
      i.nombre.toLowerCase().includes(query) || 
      i.codigo.toLowerCase().includes(query)
    );
  });

  public filteredRecipes = computed(() => {
    const list = this.recipes();
    const query = this.recipeSearchQuery().toLowerCase().trim();
    if (!query) return list;
    return list.filter(r => 
      (r.insumo?.nombre || '').toLowerCase().includes(query) ||
      (r.servicioClinico?.descripcion || '').toLowerCase().includes(query) ||
      r.servicioCodigo.toLowerCase().includes(query)
    );
  });

  public filteredMovements = computed(() => {
    const list = this.movements();
    const query = this.movementSearchQuery().toLowerCase().trim();
    if (!query) return list;
    return list.filter(m => 
      (m.insumo?.nombre || '').toLowerCase().includes(query) ||
      m.tipoMovimiento.toLowerCase().includes(query) ||
      m.usuario.toLowerCase().includes(query) ||
      m.motivo.toLowerCase().includes(query)
    );
  });

  public totalInventoryValue = computed(() => {
    return this.insumos().reduce((sum, item) => sum + (item.stockActual * item.costoUnitarioBaseUSD), 0);
  });

  ngOnInit() {
    this.loadAllData();
  }

  loadAllData() {
    this.isLoading.set(true);
    this.inventoryService.getInsumos().subscribe({
      next: (res) => {
        this.insumos.set(res);
        // Initialize physical counts
        const counts: Record<string, number> = {};
        res.forEach(i => {
          counts[i.id] = i.stockActual;
        });
        this.physicalCounts.set(counts);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });

    this.inventoryService.getMovements().subscribe({
      next: (res) => this.movements.set(res),
      error: (err) => console.error("Error loading movements", err)
    });

    this.inventoryService.getRecetas().subscribe({
      next: (res) => this.recipes.set(res),
      error: (err) => console.error("Error loading recipes", err)
    });

    this.catalogService.getUnifiedCatalog().subscribe({
      next: (res) => {
        // Only map/procedures/medicina categories if applicable, or all
        this.services.set(res.filter(s => s.activo));
      },
      error: (err) => console.error("Error loading services", err)
    });
  }

  // Tabs Change
  setTab(tab: 'stock' | 'movement' | 'closing' | 'audit' | 'recipes') {
    this.activeTab.set(tab);
    if (tab === 'stock' || tab === 'audit' || tab === 'recipes') {
      this.loadAllData();
    }
  }

  // CRUD Insumo
  openCreateInsumo() {
    this.isEditingInsumo.set(false);
    this.insumoForm.set({
      codigo: '',
      nombre: '',
      stockInicial: 0,
      unidadMedidaBase: 'UNIDAD',
      costoUnitarioBaseUSD: 0
    });
    this.showInsumoModal.set(true);
  }

  openEditInsumo(insumo: Insumo) {
    this.isEditingInsumo.set(true);
    this.editingInsumoId.set(insumo.id);
    this.insumoForm.set({
      codigo: insumo.codigo,
      nombre: insumo.nombre,
      stockInicial: insumo.stockActual,
      unidadMedidaBase: insumo.unidadMedidaBase,
      costoUnitarioBaseUSD: insumo.costoUnitarioBaseUSD
    });
    this.showInsumoModal.set(true);
  }

  saveInsumo() {
    const form = this.insumoForm();
    if (!form.codigo || !form.nombre) return;

    this.isSubmitting.set(true);
    if (this.isEditingInsumo()) {
      this.inventoryService.updateInsumo(this.editingInsumoId(), {
        nombre: form.nombre,
        costoUnitarioBaseUSD: form.costoUnitarioBaseUSD || 0
      }).subscribe({
        next: () => {
          this.showInsumoModal.set(false);
          this.isSubmitting.set(false);
          this.loadAllData();
        },
        error: () => this.isSubmitting.set(false)
      });
    } else {
      this.inventoryService.createInsumo(form as CreateInsumo).subscribe({
        next: () => {
          this.showInsumoModal.set(false);
          this.isSubmitting.set(false);
          this.loadAllData();
        },
        error: () => this.isSubmitting.set(false)
      });
    }
  }

  // Stock Movement (Restock / Discard)
  saveMovement() {
    const form = this.movementForm();
    if (!form.insumoId || form.cantidadOriginal <= 0) return;

    this.isSubmitting.set(true);
    this.inventoryService.recordMovement(form).subscribe({
      next: () => {
        this.isSubmitting.set(false);
        // Clear form
        this.movementForm.set({
          insumoId: '',
          tipoMovimiento: 'Ingreso',
          cantidadOriginal: 0,
          unidadMedidaOriginal: 'UNIDAD',
          motivo: ''
        });
        alert('Movimiento registrado con éxito.');
        this.setTab('stock');
      },
      error: (err) => {
        this.isSubmitting.set(false);
        console.error(err);
        alert('Error al registrar el movimiento.');
      }
    });
  }

  // Recipe (BOM)
  saveRecipe() {
    const form = this.recipeForm();
    if (!form.servicioClinicoId || !form.insumoId || form.cantidad <= 0) return;

    this.isSubmitting.set(true);
    this.inventoryService.createOrUpdateRecipe(form).subscribe({
      next: () => {
        this.isSubmitting.set(false);
        this.recipeForm.set({
          servicioClinicoId: '',
          insumoId: '',
          cantidad: 0,
          unidadMedidaConsumo: 'UNIDAD'
        });
        this.loadAllData();
      },
      error: () => this.isSubmitting.set(false)
    });
  }

  deleteRecipe(id: string) {
    if (confirm('¿Está seguro de eliminar esta receta de consumo?')) {
      this.inventoryService.deleteRecipe(id).subscribe({
        next: () => this.loadAllData(),
        error: (err) => console.error("Error deleting recipe", err)
      });
    }
  }

  // Inventory closing
  setPhysicalCount(insumoId: string, event: Event) {
    const value = parseFloat((event.target as HTMLInputElement).value);
    this.physicalCounts.update(prev => ({
      ...prev,
      [insumoId]: isNaN(value) ? 0 : value
    }));
  }

  submitClosing() {
    if (!confirm('¿Está seguro de realizar el cierre físico del inventario? Esto actualizará el stock actual del sistema con los valores reales digitados.')) {
      return;
    }

    this.isSubmitting.set(true);
    const details = Object.entries(this.physicalCounts()).map(([insumoId, stockReal]) => ({
      insumoId,
      stockReal
    }));

    this.inventoryService.performClosing({
      observaciones: this.closingObservations(),
      detalles: details
    }).subscribe({
      next: () => {
        this.isSubmitting.set(false);
        this.closingObservations.set('');
        alert('Cierre de inventario registrado y archivado exitosamente.');
        this.setTab('stock');
      },
      error: (err) => {
        this.isSubmitting.set(false);
        console.error(err);
        alert('Error al realizar el cierre de inventario.');
      }
    });
  }

  getDiscrepancy(insumo: Insumo): number {
    const physical = this.physicalCounts()[insumo.id] ?? insumo.stockActual;
    return physical - insumo.stockActual;
  }

  getDiscrepancyCost(insumo: Insumo): number {
    return this.getDiscrepancy(insumo) * insumo.costoUnitarioBaseUSD;
  }
}
