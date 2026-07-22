import { Component, inject, signal, OnInit, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CatalogService, CatalogItem } from '../../../core/services/catalog.service';
import { BillingFacadeService } from '../../../core/services/billing-facade.service';
import { 
    LucideAngularModule, 
    Package, 
    Search, 
    Plus, 
    Edit, 
    Trash2,
    Database,
    Stethoscope,
    Scan,
    X,
    Check,
    Clock,
    ArrowUpDown,
    ArrowUp,
    ArrowDown,
    Filter,
    List,
    SlidersHorizontal
} from 'lucide-angular';
import { EditCirugiaComponent } from './components/edit-cirugia.component';
import { EditConsultaComponent } from './components/edit-consulta.component';
import { EditLaboratorioComponent } from './components/edit-laboratorio.component';
import { EditMedicamentoComponent } from './components/edit-medicamento.component';
import { EditProcedimientoComponent } from './components/edit-procedimiento.component';
import { EditTomografiaComponent } from './components/edit-tomografia.component';
import { getTipoBadgeStyle } from './models/catalog-edit.models';

export type SortOption = 'nombre-asc' | 'nombre-desc' | 'precio-desc' | 'precio-asc' | 'codigo-asc';

export type CatalogEditorType = 'CONSULTA' | 'MEDICAMENTO' | 'TOMOGRAFIA' | 'PROCEDIMIENTO' | 'CIRUGIA' | 'LABORATORIO';

const TIPO_MAP: Record<string, CatalogEditorType> = {
  MEDICINA: 'MEDICAMENTO',
  MEDICAMENTO: 'MEDICAMENTO',
  INSUMO: 'MEDICAMENTO',
  FARMACIA: 'MEDICAMENTO',
  RX: 'TOMOGRAFIA',
  TOMOGRAFIA: 'TOMOGRAFIA',
  TOMO: 'TOMOGRAFIA',
  RADIOGRAFIA: 'TOMOGRAFIA',
  IMAGEN: 'TOMOGRAFIA',
  CONSULTA: 'CONSULTA',
  CITAS: 'CONSULTA',
  MEDICO: 'CONSULTA',
  LABORATORIO: 'LABORATORIO',
  LAB: 'LABORATORIO',
  CIRUGIA: 'CIRUGIA',
  QUIRURGICO: 'CIRUGIA'
};

@Component({
  selector: 'app-catalog-management',
  standalone: true,
  imports: [
    CommonModule, 
    FormsModule, 
    LucideAngularModule,
    EditCirugiaComponent,
    EditConsultaComponent,
    EditLaboratorioComponent,
    EditMedicamentoComponent,
    EditProcedimientoComponent,
    EditTomografiaComponent
  ],
  templateUrl: './catalog-management.component.html'
})
export class CatalogManagementComponent implements OnInit {
  private readonly catalogService = inject(CatalogService);
  private readonly billingFacade = inject(BillingFacadeService);
  private readonly route = inject(ActivatedRoute);

  readonly tasaCambioDia = this.billingFacade.tasaCambioDia;

  // Estado del catálogo (Signals)
  readonly catalog = signal<CatalogItem[]>([]);
  readonly isLoading = signal<boolean>(false);
  readonly selectedTypes = signal<string[]>([]);
  readonly searchQuery = signal<string>('');
  readonly sortOption = signal<SortOption>('nombre-asc');
  
  // Estado de Modales y Edición Tipados Fuertemente
  readonly showModal = signal<boolean>(false);
  readonly isEditing = signal<boolean>(false);
  readonly selectedItemId = signal<string | null>(null);
  readonly activeEditorType = signal<CatalogEditorType | null>(null);
  readonly itemToDelete = signal<CatalogItem | null>(null);

  readonly availableTypes = ['CONSULTA', 'MEDICAMENTO', 'RX', 'TOMOGRAFIA', 'PROCEDIMIENTO', 'CIRUGIA'];

  readonly icons = {
    Package, Search, Plus, Edit, Trash2, Database, Stethoscope, Scan, X, Check, Clock,
    ArrowUpDown, ArrowUp, ArrowDown, Filter, List, SlidersHorizontal
  };

  // Computed Signal: Filtrado y ordenamiento reactivo declarativo (Estándar Senior)
  readonly filteredCatalog = computed(() => {
    let list = [...this.catalog()];
    const selected = this.selectedTypes();
    const query = this.searchQuery().trim().toLowerCase();
    const sort = this.sortOption();

    // 1. Filtro por tipos de servicio seleccionados (Normalización de Dominio Tipada)
    if (selected.length > 0) {
      const selectedNormalized = selected.map(t => this.getNormalizedEditorType(t));
      list = list.filter(item => selectedNormalized.includes(this.getNormalizedEditorType(item.tipo)));
    }

    // 2. Búsqueda por descripción o código
    if (query) {
      list = list.filter(i => 
        (i.descripcion || '').toLowerCase().includes(query) || 
        (i.codigo || '').toLowerCase().includes(query)
      );
    }

    // 3. Ordenamiento reactivo
    return list.sort((a, b) => {
      switch (sort) {
        case 'nombre-asc': return (a.descripcion || '').localeCompare(b.descripcion || '');
        case 'nombre-desc': return (b.descripcion || '').localeCompare(a.descripcion || '');
        case 'precio-desc': return (b.precioUsd || 0) - (a.precioUsd || 0);
        case 'precio-asc': return (a.precioUsd || 0) - (b.precioUsd || 0);
        case 'codigo-asc': return (a.codigo || '').localeCompare(b.codigo || '');
        default: return 0;
      }
    });
  });

  constructor() {
    this.route.queryParams.pipe(takeUntilDestroyed()).subscribe(params => {
      if (params['type']) {
        this.selectedTypes.set([params['type'].toUpperCase()]);
      }
      this.refreshCatalog();
    });
  }

  ngOnInit(): void {}

  refreshCatalog(): void {
    this.isLoading.set(true);
    this.catalogService.getUnifiedCatalog().subscribe({
      next: (res: CatalogItem[]) => {
        this.catalog.set(res);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  toggleTypeFilter(tipo: string): void {
    if (tipo === 'TODOS') {
      this.selectedTypes.set([]);
      return;
    }
    const current = this.selectedTypes();
    this.selectedTypes.set(
      current.includes(tipo) ? current.filter(t => t !== tipo) : [...current, tipo]
    );
  }

  isTypeSelected(tipo: string): boolean {
    return tipo === 'TODOS' ? this.selectedTypes().length === 0 : this.selectedTypes().includes(tipo);
  }

  setSortOption(option: SortOption): void {
    this.sortOption.set(option);
  }

  clearAllFilters(): void {
    this.searchQuery.set('');
    this.selectedTypes.set([]);
    this.sortOption.set('nombre-asc');
  }

  getNormalizedEditorType(rawType: string | null | undefined): CatalogEditorType {
    if (!rawType) return 'PROCEDIMIENTO';
    const key = rawType.toUpperCase().trim();
    return TIPO_MAP[key] || 'PROCEDIMIENTO';
  }

  openCreate(): void {
    this.isEditing.set(false);
    this.selectedItemId.set(null);
    const selected = this.selectedTypes();
    const firstType = selected.length > 0 ? selected[0] : null;
    this.activeEditorType.set(this.getNormalizedEditorType(firstType));
    this.showModal.set(true);
  }

  openEdit(item: CatalogItem): void {
    this.isEditing.set(true);
    this.selectedItemId.set(item.id || null);
    this.activeEditorType.set(this.getNormalizedEditorType(item.tipo));
    this.showModal.set(true);
  }

  onEditorSaved(): void {
    this.closeModal();
    this.refreshCatalog();
  }

  onEditorClosed(): void {
    this.closeModal();
  }

  private closeModal(): void {
    this.showModal.set(false);
    this.activeEditorType.set(null);
    this.selectedItemId.set(null);
  }

  confirmDelete(item: CatalogItem): void {
    this.itemToDelete.set(item);
  }

  executeDelete(): void {
    const item = this.itemToDelete();
    if (!item?.id) return;
    
    this.catalogService.deleteItem(item.id).subscribe({
      next: () => {
        this.itemToDelete.set(null);
        this.refreshCatalog();
      },
      error: (err) => {
        console.error('Error HTTP al eliminar servicio del catálogo:', err);
        this.itemToDelete.set(null);
      }
    });
  }

  cancelDelete(): void {
    this.itemToDelete.set(null);
  }

  getTipoBadgeStyle(tipo?: string | null): string {
    return getTipoBadgeStyle(tipo);
  }
}
