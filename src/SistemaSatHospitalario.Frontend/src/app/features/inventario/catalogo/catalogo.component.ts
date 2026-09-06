import { Component, inject, signal, OnInit, computed, ViewChild, ElementRef, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { InventoryService } from '../../../core/services/inventory.service';
import { CatalogLookupService } from '../../../core/services/catalog-lookup.service';
import { Insumo, PrincipioActivo, CategoriaInsumo, CreateInsumo, UpdateInsumo } from '../../../core/models/inventory.model';
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
  Dna,
  Tag,
  Tags,
  SlidersHorizontal
} from 'lucide-angular';

@Component({
  selector: 'app-catalogo',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule],
  templateUrl: './catalogo.component.html'
})
export class CatalogoComponent implements OnInit {
  private inventoryService = inject(InventoryService);
  private catalogLookup = inject(CatalogLookupService);

  @ViewChild('searchInput') searchInput!: ElementRef<HTMLInputElement>;

  public insumos = signal<Insumo[]>([]);
  public principiosActivosList = signal<PrincipioActivo[]>([]);
  public categoriasInsumoList = signal<CategoriaInsumo[]>([]);
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

  // Gestión de Categorías
  public newCategoriaNombre = signal<string>('');
  public isManagingCategorias = signal<boolean>(false);
  public editingCategoriaId = signal<string | null>(null);
  public editingCategoriaNombre = signal<string>('');

  // Alertas
  public successMessage = signal<string | null>(null);
  public errorMessage = signal<string | null>(null);

  // 3FN: unidades de medida desde el catálogo cacheado (reemplaza el array hardcodeado)
  readonly unidadesMedida = computed(() => this.catalogLookup.unidadesMedida().map(u => u.codigo));

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
    Dna,
    Tag,
    Tags,
    SlidersHorizontal
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
        (i.categoria && i.categoria.toLowerCase().includes(query)) ||
        (i.principiosActivos && i.principiosActivos.some(pa => (pa.nombre || '').toLowerCase().includes(query)));
    });
  });

  ngOnInit() {
    this.loadData();
    // 3FN: cargar catálogos cacheados (unidades de medida, categorías)
    this.catalogLookup.loadUnidadesMedida().subscribe();
    this.catalogLookup.loadCategoriasInsumo().subscribe();
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

    // 3FN: categorías desde el catálogo cacheado (fallback al servicio legacy si falla)
    this.catalogLookup.loadCategoriasInsumo().subscribe({
      next: (cats) => {
        this.categoriasInsumoList.set(cats.map(c => ({ id: c.id, nombre: c.nombre, codigo: c.codigo, activo: true, fechaCreacion: new Date().toISOString() })));
        if (!this.isEditing() && cats.length > 0 && !this.insumoForm().categoria) {
          this.insumoForm.update(p => ({ ...p, categoria: cats[0].nombre }));
        }
      },
      error: () => {
        // Fallback al servicio legacy si el catálogo no está disponible
        this.inventoryService.getCategorias().subscribe({
          next: (cats) => {
            this.categoriasInsumoList.set(cats);
            if (!this.isEditing() && cats.length > 0 && !this.insumoForm().categoria) {
              this.insumoForm.update(p => ({ ...p, categoria: cats[0].nombre }));
            }
          }
        });
      }
    });
  }

  openCreateForm() {
    this.isEditing.set(false);
    this.editingInsumoId.set(null);
    const defaultCat = this.categoriasInsumoList().find(c => c.nombre.toLowerCase() === 'medicamento')?.nombre 
      || (this.categoriasInsumoList().length > 0 ? this.categoriasInsumoList()[0].nombre : 'Medicamento');

    this.insumoForm.set({
      codigo: '',
      nombre: '',
      unidadMedidaBase: 'UNIDAD',
      costoUnitarioBaseUSD: 0,
      permiteFraccionamiento: true,
      categoria: defaultCat,
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
      categoria: insumo.categoria || (this.categoriasInsumoList().length > 0 ? this.categoriasInsumoList()[0].nombre : 'Medicamento'),
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
          if (err.status === 409 || err.error?.estaDesactivado) {
            const insumoId = err.error?.insumoId;
            const nombre = err.error?.nombre || form.nombre;
            if (insumoId && confirm(`El código '${form.codigo}' pertenece al insumo '${nombre}' que actualmente se encuentra inhabilitado.\n\n¿Desea reactivarlo y actualizar sus datos con la información de este formulario?`)) {
              this.reactivarYActualizar(insumoId, form);
              return;
            }
          }
          this.showError(err.error?.message || 'Error al crear el insumo.');
          this.isSubmitting.set(false);
        }
      });
    }
  }

  reactivarYActualizar(insumoId: string, form: any) {
    this.inventoryService.restoreInsumo(insumoId).subscribe({
      next: () => {
        const updateDto: UpdateInsumo = {
          nombre: form.nombre,
          unidadMedidaBase: form.unidadMedidaBase,
          costoUnitarioBaseUSD: form.costoUnitarioBaseUSD,
          permiteFraccionamiento: form.permiteFraccionamiento,
          categoria: form.categoria
        };
        this.inventoryService.updateInsumo(insumoId, updateDto).subscribe({
          next: () => {
            this.showSuccess('Insumo reactivado y actualizado exitosamente.');
            this.isSubmitting.set(false);
            this.openCreateForm();
            this.loadData();
          },
          error: () => {
            this.showSuccess('Insumo reactivado correctamente.');
            this.isSubmitting.set(false);
            this.openCreateForm();
            this.loadData();
          }
        });
      },
      error: (err) => {
        this.showError(err.error?.message || 'Error al reactivar el insumo.');
        this.isSubmitting.set(false);
      }
    });
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

  // --- Categorías de Insumos: Creación y Modificación Dinámica ---

  crearNuevaCategoria() {
    const nombre = this.newCategoriaNombre().trim();
    if (!nombre) {
      this.showError('Ingrese el nombre de la categoría.');
      return;
    }

    this.inventoryService.createCategoria(nombre).subscribe({
      next: (cat) => {
        this.showSuccess(`Categoría '${cat.nombre}' registrada correctamente.`);
        this.newCategoriaNombre.set('');
        this.insumoForm.update(p => ({ ...p, categoria: cat.nombre }));
        this.inventoryService.getCategorias().subscribe(cats => this.categoriasInsumoList.set(cats));
      },
      error: (err) => this.showError(err.error?.message || 'Error al crear la categoría.')
    });
  }

  iniciarEdicionCategoria(cat: CategoriaInsumo) {
    this.editingCategoriaId.set(cat.id);
    this.editingCategoriaNombre.set(cat.nombre);
  }

  cancelarEdicionCategoria() {
    this.editingCategoriaId.set(null);
    this.editingCategoriaNombre.set('');
  }

  guardarEdicionCategoria(cat: CategoriaInsumo) {
    const nuevoNombre = this.editingCategoriaNombre().trim();
    if (!nuevoNombre) {
      this.showError('El nombre de la categoría no puede estar vacío.');
      return;
    }

    this.inventoryService.updateCategoria(cat.id, nuevoNombre).subscribe({
      next: (updated) => {
        this.showSuccess(`Categoría actualizada a '${updated.nombre}'.`);
        
        // Si el formulario actual tenía seleccionada esta categoría, actualizarlo
        if (this.insumoForm().categoria === cat.nombre) {
          this.insumoForm.update(p => ({ ...p, categoria: updated.nombre }));
        }

        this.cancelarEdicionCategoria();
        this.loadData();
      },
      error: (err) => this.showError(err.error?.message || 'Error al actualizar el nombre de la categoría.')
    });
  }

  toggleGestionCategorias() {
    this.isManagingCategorias.update(v => !v);
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
