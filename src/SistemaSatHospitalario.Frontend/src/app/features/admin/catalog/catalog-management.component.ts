import { Component, signal, computed, inject, OnInit } from '@angular/core';
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
    SlidersHorizontal,
    Building2
} from 'lucide-angular';
import { EditCirugiaComponent } from './components/edit-cirugia.component';
import { EditConsultaComponent } from './components/edit-consulta.component';
import { EditLaboratorioComponent } from './components/edit-laboratorio.component';
import { EditMedicamentoComponent } from './components/edit-medicamento.component';
import { EditProcedimientoComponent } from './components/edit-procedimiento.component';
import { EditTomografiaComponent } from './components/edit-tomografia.component';
import { EditHospitalarioComponent } from './components/edit-hospitalario.component';
import { getTipoBadgeStyle as getBadgeStyle, CatalogEditorType } from './models/catalog-edit.models';

export type SortOption = 'nombre-asc' | 'nombre-desc' | 'precio-desc' | 'precio-asc' | 'codigo-asc';

const TIPO_MAP: Record<string, CatalogEditorType> = {
  MEDICINA: 'MEDICAMENTO',
  MEDICAMENTO: 'MEDICAMENTO',
  INSUMO: 'MEDICAMENTO',
  FARMACIA: 'MEDICAMENTO',
  MATERIAL: 'MEDICAMENTO',
  SOLUCION: 'MEDICAMENTO',
  AMPOLLA: 'MEDICAMENTO',
  TAB: 'MEDICAMENTO',
  CAPSULA: 'MEDICAMENTO',
  
  RX: 'TOMOGRAFIA',
  TOMOGRAFIA: 'TOMOGRAFIA',
  TOMO: 'TOMOGRAFIA',
  RADIOGRAFIA: 'TOMOGRAFIA',
  IMAGEN: 'TOMOGRAFIA',
  ECO: 'TOMOGRAFIA',
  ECOGRAFIA: 'TOMOGRAFIA',
  ULTRASONIDO: 'TOMOGRAFIA',
  RESONANCIA: 'TOMOGRAFIA',
  ESTUDIO: 'TOMOGRAFIA',
  ESTUDIOS: 'TOMOGRAFIA',

  CONSULTA: 'CONSULTA',
  CITAS: 'CONSULTA',
  MEDICO: 'CONSULTA',
  EVALUACION: 'CONSULTA',

  LABORATORIO: 'LABORATORIO',
  LAB: 'LABORATORIO',
  EXAMEN: 'LABORATORIO',
  EXAMENES: 'LABORATORIO',
  PERFIL: 'LABORATORIO',
  ANALISIS: 'LABORATORIO',

  CIRUGIA: 'CIRUGIA',
  QUIRURGICO: 'CIRUGIA',
  INTERVENCION: 'CIRUGIA',
  PABELLON: 'CIRUGIA',

  PROCEDIMIENTO: 'PROCEDIMIENTO',
  PROCEDIMIENTOS: 'PROCEDIMIENTO',
  TRATAMIENTO: 'PROCEDIMIENTO',
  ENFERMERIA: 'PROCEDIMIENTO',
  CURACION: 'PROCEDIMIENTO',

  HOSPITALARIO: 'HOSPITALARIO',
  HOSPITALIZACION: 'HOSPITALARIO',
  EMERGENCIA: 'HOSPITALARIO',
  UCI: 'HOSPITALARIO',
  TRASLADO: 'HOSPITALARIO',
  AREA: 'HOSPITALARIO',
  CAMAS: 'HOSPITALARIO',
  HABITACION: 'HOSPITALARIO',
  ESTANCIA: 'HOSPITALARIO'
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
    EditTomografiaComponent,
    EditHospitalarioComponent
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

  readonly availableTypes = ['CONSULTA', 'MEDICAMENTO', 'RX', 'TOMOGRAFIA', 'PROCEDIMIENTO', 'CIRUGIA', 'LABORATORIO', 'HOSPITALARIO'];

  readonly icons = {
    Package, Search, Plus, Edit, Trash2, Database, Stethoscope, Scan, X, Check, Clock,
    ArrowUpDown, ArrowUp, ArrowDown, Filter, List, SlidersHorizontal, Building2
  };

  // Computed Signal: Filtrado y ordenamiento reactivo declarativo (Estándar Senior)
  readonly filteredCatalog = computed(() => {
    let list = [...this.catalog()];
    const selected = this.selectedTypes();
    const query = this.searchQuery().trim().toLowerCase();
    const sort = this.sortOption();

    // 1. Filtro por tipos de servicio seleccionados
    if (selected.length > 0) {
      list = list.filter(item => {
        const normalizedItemType = this.getNormalizedEditorType(item.tipo);
        return selected.some(targetType => {
          const normalizedTarget = this.getNormalizedEditorType(targetType);
          return normalizedItemType === normalizedTarget;
        });
      });
    }

    // 2. Filtro por texto (Nombre o Código)
    if (query) {
      list = list.filter(item => 
        (item.descripcion || '').toLowerCase().includes(query) ||
        (item.codigo || '').toLowerCase().includes(query)
      );
    }

    // 3. Ordenamiento declarativo
    return list.sort((a, b) => {
      switch (sort) {
        case 'nombre-asc':
          return (a.descripcion || '').localeCompare(b.descripcion || '');
        case 'nombre-desc':
          return (b.descripcion || '').localeCompare(a.descripcion || '');
        case 'precio-desc':
          return (b.precioUsd || 0) - (a.precioUsd || 0);
        case 'precio-asc':
          return (a.precioUsd || 0) - (b.precioUsd || 0);
        case 'codigo-asc':
          return (a.codigo || '').localeCompare(b.codigo || '');
        default:
          return 0;
      }
    });
  });

  constructor() {
    this.route.queryParams.pipe(takeUntilDestroyed()).subscribe(params => {
      if (params['filter']) {
        const filterType = params['filter'].toUpperCase();
        if (this.availableTypes.includes(filterType)) {
          this.selectedTypes.set([filterType]);
        }
      }
    });
  }

  ngOnInit(): void {
    this.loadCatalog();
  }

  getNormalizedEditorType(rawType: string | null | undefined): CatalogEditorType {
    if (!rawType) return 'PROCEDIMIENTO';
    const key = rawType.toUpperCase().trim();
    return TIPO_MAP[key] || 'PROCEDIMIENTO';
  }

  loadCatalog(): void {
    this.isLoading.set(true);
    this.catalogService.getItems().subscribe({
      next: (data) => {
        this.catalog.set(data || []);
        this.isLoading.set(false);
      },
      error: (err) => {
        console.error('Error cargando catálogo:', err);
        this.isLoading.set(false);
      }
    });
  }

  setSortOption(option: SortOption): void {
    this.sortOption.set(option);
  }

  isTypeSelected(type: string): boolean {
    if (type === 'TODOS') return this.selectedTypes().length === 0;
    return this.selectedTypes().some(t => this.getNormalizedEditorType(t) === this.getNormalizedEditorType(type));
  }

  toggleTypeFilter(type: string): void {
    if (type === 'TODOS') {
      this.selectedTypes.set([]);
      return;
    }

    const current = this.selectedTypes();
    if (current.includes(type)) {
      this.selectedTypes.set(current.filter(t => t !== type));
    } else {
      this.selectedTypes.set([...current, type]);
    }
  }

  clearFilters(): void {
    this.selectedTypes.set([]);
    this.searchQuery.set('');
  }

  clearAllFilters(): void {
    this.clearFilters();
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

  confirmDelete(item: CatalogItem): void {
    this.itemToDelete.set(item);
  }

  cancelDelete(): void {
    this.itemToDelete.set(null);
  }

  executeDelete(): void {
    const item = this.itemToDelete();
    if (!item?.id) return;

    this.catalogService.deleteItem(item.id).subscribe({
      next: () => {
        this.catalog.set(this.catalog().filter(i => i.id !== item.id));
        this.itemToDelete.set(null);
      },
      error: (err) => console.error('Error eliminando ítem:', err)
    });
  }

  onEditorSaved(): void {
    this.showModal.set(false);
    this.loadCatalog();
  }

  onEditorClosed(): void {
    this.showModal.set(false);
  }

  getTipoBadgeStyle(tipo?: string | null): string {
    return getBadgeStyle(tipo);
  }

  getBadgeClass(tipo: string): string {
    return getBadgeStyle(tipo);
  }
}
