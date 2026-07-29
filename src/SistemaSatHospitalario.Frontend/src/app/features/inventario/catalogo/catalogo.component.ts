import { Component, inject, signal, OnInit, computed, ViewChild, ElementRef, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { InventoryService } from '../../../core/services/inventory.service';
import { Insumo, PrincipioActivo, CreateInsumo, UpdateInsumo } from '../../../core/models/inventory.model';
import { 
  LucideAngularModule, 
  Package, 
  Search, 
  Plus, 
  Edit, 
  Trash2, 
  RotateCcw,
  Check, 
  X, 
  AlertCircle,
  Dna
} from 'lucide-angular';

@Component({
  selector: 'app-catalogo',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule],
  templateUrl: './catalogo.component.html'
})
export class CatalogoComponent implements OnInit {
  private inventoryService = inject(InventoryService);

  @ViewChild('searchInput') searchInput!: ElementRef<HTMLInputElement>;

  public insumos = signal<Insumo[]>([]);
  public principiosActivosList = signal<PrincipioActivo[]>([]);
  public isLoading = signal<boolean>(false);
  public isSubmitting = signal<boolean>(false);

  @HostListener('window:keydown', ['$event'])
  handleKeyDown(event: KeyboardEvent) {
    if ((event.ctrlKey || event.metaKey) && event.key.toLowerCase() === 'k') {
      const target = event.target as HTMLElement;
      if (target && (target.tagName === 'TEXTAREA' || (target.tagName === 'INPUT' && target.id !== 'catalogoSearchInput'))) {
        return;
      }
      event.preventDefault();
      this.focusSearchInput();
    }
  }

  focusSearchInput() {
    if (this.searchInput?.nativeElement) {
      this.searchInput.nativeElement.focus();
      this.searchInput.nativeElement.select();
    }
  }

  // Búsqueda
  public searchQuery = signal<string>('');
  public showInactive = signal<boolean>(false);

  // Formulario Inline Insumo (Crear / Editar)
  public isEditing = signal<boolean>(false);
  public editingInsumoId = signal<string | null>(null);
  public insumoForm = signal<{
    codigo: string;
    nombre: string;
    unidadMedidaBase: string;
    costoUnitarioBaseUSD: number;
    permiteFraccionamiento: boolean;
    categoria: string;
    stockInicial: number;
  }>({
    codigo: '',
    nombre: '',
    unidadMedidaBase: 'UNIDAD',
    costoUnitarioBaseUSD: 0,
    permiteFraccionamiento: true,
    categoria: 'Medicamento',
    stockInicial: 0
  });

  // Gestión Inline de Principios Activos del Insumo actual
  public selectedPaId = signal<string>('');
  public paConcentracion = signal<string>('');
  public newPaNombre = signal<string>('');

  // Alertas
  public successMessage = signal<string | null>(null);
  public errorMessage = signal<string | null>(null);

  readonly unidadesMedida = ['UNIDAD', 'KG', 'G', 'DG', 'MG', 'L', 'ML'];
  readonly categorias = ['Medicamento', 'Descartable', 'Material Médico', 'Reactivo', 'Otro'];

  readonly icons = {
    Package,
    Search,
    Plus,
    Edit,
    Trash2,
    RotateCcw,
    Check,
    X,
    AlertCircle,
    Dna
  };

  public filteredInsumos = computed(() => {
    const list = this.insumos();
    const query = this.searchQuery().toLowerCase().trim();
    const showHidden = this.showInactive();

    return list.filter(i => {
      if (!showHidden && i.isDeleted) return false;

      if (!query) return true;
      return i.nombre.toLowerCase().includes(query) ||
        i.codigo.toLowerCase().includes(query) ||
        (i.principiosActivos && i.principiosActivos.some(pa => (pa.nombre || '').toLowerCase().includes(query)));
    });
  });

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    this.isLoading.set(true);
    this.inventoryService.getInsumos(false).subscribe({
      next: (res) => {
        this.insumos.set(res);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });

    this.inventoryService.getPrincipiosActivos().subscribe({
      next: (res) => this.principiosActivosList.set(res)
    });
  }

  openCreateForm() {
    this.isEditing.set(false);
    this.editingInsumoId.set(null);
    this.insumoForm.set({
      codigo: '',
      nombre: '',
      unidadMedidaBase: 'UNIDAD',
      costoUnitarioBaseUSD: 0,
      permiteFraccionamiento: true,
      categoria: 'Medicamento',
      stockInicial: 0
    });
  }

  updateCodigo(val: string) {
    this.insumoForm.update(p => ({ ...p, codigo: val }));
  }

  updateNombre(val: string) {
    this.insumoForm.update(p => ({ ...p, nombre: val }));
  }

  updateUnidad(val: string) {
    this.insumoForm.update(p => ({ ...p, unidadMedidaBase: val }));
  }

  updateCategoria(val: string) {
    this.insumoForm.update(p => ({ ...p, categoria: val }));
  }

  updateCosto(val: number) {
    this.insumoForm.update(p => ({ ...p, costoUnitarioBaseUSD: val }));
  }

  updateStockInicial(val: number) {
    this.insumoForm.update(p => ({ ...p, stockInicial: val }));
  }

  openEditForm(insumo: Insumo) {
    this.isEditing.set(true);
    this.editingInsumoId.set(insumo.id);
    this.insumoForm.set({
      codigo: insumo.codigo,
      nombre: insumo.nombre,
      unidadMedidaBase: insumo.unidadMedidaBase,
      costoUnitarioBaseUSD: insumo.costoUnitarioBaseUSD,
      permiteFraccionamiento: insumo.permiteFraccionamiento ?? true,
      categoria: insumo.categoria || 'Medicamento',
      stockInicial: 0
    });
  }

  saveInsumo() {
    const form = this.insumoForm();
    if (!form.codigo || !form.nombre) {
      this.showError('Código y Nombre son campos obligatorios.');
      return;
    }

    this.isSubmitting.set(true);

    if (this.isEditing()) {
      const id = this.editingInsumoId();
      if (!id) return;

      const dto: UpdateInsumo = {
        nombre: form.nombre,
        unidadMedidaBase: form.unidadMedidaBase,
        costoUnitarioBaseUSD: form.costoUnitarioBaseUSD,
        permiteFraccionamiento: form.permiteFraccionamiento,
        categoria: form.categoria
      };

      this.inventoryService.updateInsumo(id, dto).subscribe({
        next: () => {
          this.showSuccess('Insumo actualizado correctamente en el catálogo.');
          this.isSubmitting.set(false);
          this.openCreateForm();
          this.loadData();
        },
        error: (err) => {
          this.showError(err.error?.message || 'Error al actualizar el insumo.');
          this.isSubmitting.set(false);
        }
      });
    } else {
      const dto: CreateInsumo = {
        codigo: form.codigo,
        nombre: form.nombre,
        stockInicial: form.stockInicial,
        unidadMedidaBase: form.unidadMedidaBase,
        costoUnitarioBaseUSD: form.costoUnitarioBaseUSD,
        permiteFraccionamiento: form.permiteFraccionamiento,
        categoria: form.categoria
      };

      this.inventoryService.createInsumo(dto).subscribe({
        next: () => {
          this.showSuccess('Insumo creado correctamente.');
          this.isSubmitting.set(false);
          this.openCreateForm();
          this.loadData();
        },
        error: (err) => {
          this.showError(err.error?.message || 'Error al crear el insumo.');
          this.isSubmitting.set(false);
        }
      });
    }
  }

  softDeleteInsumo(insumo: Insumo) {
    if (!confirm(`¿Está seguro de inhabilitar (Soft Delete) el insumo '${insumo.nombre}'?`)) return;

    this.inventoryService.deleteInsumo(insumo.id).subscribe({
      next: () => {
        this.showSuccess('Insumo inhabilitado correctamente.');
        this.loadData();
      },
      error: (err) => this.showError(err.error?.message || 'Error al inhabilitar insumo.')
    });
  }

  restoreInsumo(insumo: Insumo) {
    this.inventoryService.restoreInsumo(insumo.id).subscribe({
      next: () => {
        this.showSuccess('Insumo restaurado correctamente.');
        this.loadData();
      },
      error: (err) => this.showError(err.error?.message || 'Error al restaurar insumo.')
    });
  }

  // --- Principios Activos Inline Vinculación ---

  vincularPA(insumoId: string) {
    const paId = this.selectedPaId();
    const conc = this.paConcentracion().trim();

    if (!paId || !conc) {
      this.showError('Debe seleccionar un Principio Activo y especificar su concentración (Ej: 400mg).');
      return;
    }

    this.inventoryService.vincularPrincipioActivo(insumoId, paId, conc).subscribe({
      next: () => {
        this.showSuccess('Principio Activo vinculado exitosamente al insumo.');
        this.selectedPaId.set('');
        this.paConcentracion.set('');
        this.loadData();
      },
      error: (err) => this.showError(err.error?.message || 'Error al vincular principio activo.')
    });
  }

  desvincularPA(insumoId: string, paId: string) {
    this.inventoryService.desvincularPrincipioActivo(insumoId, paId).subscribe({
      next: () => {
        this.showSuccess('Principio Activo desvinculado.');
        this.loadData();
      },
      error: (err) => this.showError(err.error?.message || 'Error al desvincular.')
    });
  }

  crearNuevoPA() {
    const nombre = this.newPaNombre().trim();
    if (!nombre) return;

    this.inventoryService.createPrincipioActivo(nombre).subscribe({
      next: (newPa) => {
        this.showSuccess(`Principio Activo '${newPa.nombre}' creado.`);
        this.newPaNombre.set('');
        this.loadData();
      },
      error: (err) => this.showError(err.error?.message || 'Error al crear principio activo.')
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
