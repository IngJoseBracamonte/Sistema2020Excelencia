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
  Building2,
  ChevronDown,
  Scissors,
  FlaskConical,
  Pill,
  Syringe,
  RotateCcw
} from 'lucide-angular';
import { EditCirugiaComponent } from './components/edit-cirugia.component';
import { EditConsultaComponent } from './components/edit-consulta.component';
import { EditLaboratorioComponent } from './components/edit-laboratorio.component';
import { EditMedicamentoComponent } from './components/edit-medicamento.component';
import { EditProcedimientoComponent } from './components/edit-procedimiento.component';
import { EditTomografiaComponent } from './components/edit-tomografia.component';
import { EditHospitalarioComponent } from './components/edit-hospitalario.component';
import { EditServicioComponent } from './components/edit-servicio.component';
import { getTipoBadgeStyle as getBadgeStyle, CatalogEditorType } from './models/catalog-edit.models';

export type SortOption = 'nombre-asc' | 'nombre-desc' | 'precio-desc' | 'precio-asc' | 'codigo-asc';

export interface CreateTypeOption {
  type: CatalogEditorType;
  label: string;
  sublabel: string;
  icon: any;
  colorClass: string;
}

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
    EditHospitalarioComponent,
    EditServicioComponent
  ],
  templateUrl: './catalog-management.component.html'
})
export class CatalogManagementComponent implements OnInit {
  private readonly catalogService = inject(CatalogService);
  private readonly billingFacade = inject(BillingFacadeService);
  private readonly route = inject(ActivatedRoute);

  readonly tasaCambioDia = this.billingFacade.tasaCambioDia;

  // ── Estado del catálogo (Signals) ─────────────────────────────────────────
  readonly catalog = signal<CatalogItem[]>([]);
  readonly isLoading = signal<boolean>(false);
  readonly selectedTypes = signal<string[]>([]);
  readonly searchQuery = signal<string>('');
  readonly sortOption = signal<SortOption>('nombre-asc');

  // ── Estado de Modales y Edición Tipados Fuertemente ────────────────────────
  readonly showModal = signal<boolean>(false);
  readonly isEditing = signal<boolean>(false);
  readonly selectedItemId = signal<string | null>(null);
  readonly activeEditorType = signal<CatalogEditorType | null>(null);
  readonly itemToDelete = signal<CatalogItem | null>(null);

  // ── Estado del Menú Desplegable de Creación y Filtro de Estado ───────────
  readonly showCreateDropdown = signal<boolean>(false);
  readonly filtroEstado = signal<'TODOS' | 'ACTIVOS' | 'DESACTIVADOS'>('TODOS');

  readonly availableTypes = ['SERVICIO', 'CONSULTA', 'MEDICAMENTO', 'RX', 'TOMOGRAFIA', 'PROCEDIMIENTO', 'CIRUGIA', 'LABORATORIO', 'HOSPITALARIO'];

  readonly createTypeOptions: CreateTypeOption[] = [
    {
      type: 'CONSULTA',
      label: 'Consulta Médica',
      sublabel: 'Honorarios por médico, recetas BOM y sugerencias',
      icon: Stethoscope,
      colorClass: 'text-indigo-400 bg-indigo-500/10 border-indigo-500/20'
    },
    {
      type: 'CIRUGIA',
      label: 'Cirugía / Pabellón',
      sublabel: 'Tiempos, equipo quirúrgico y kits de quirófano',
      icon: Scissors,
      colorClass: 'text-rose-400 bg-rose-500/10 border-rose-500/20'
    },
    {
      type: 'LABORATORIO',
      label: 'Laboratorio Clínico',
      sublabel: 'Pruebas bioanalíticas, reactivos e insumos BOM',
      icon: FlaskConical,
      colorClass: 'text-emerald-400 bg-emerald-500/10 border-emerald-500/20'
    },
    {
      type: 'TOMOGRAFIA',
      label: 'Tomografía / Imágenes',
      sublabel: 'Cortes, placas, contrastes e informe médico',
      icon: Scan,
      colorClass: 'text-violet-400 bg-violet-500/10 border-violet-500/20'
    },
    {
      type: 'MEDICAMENTO',
      label: 'Medicamento / Fármaco',
      sublabel: 'Principio activo, dosis y control de Kárdex',
      icon: Pill,
      colorClass: 'text-teal-400 bg-teal-500/10 border-teal-500/20'
    },
    {
      type: 'PROCEDIMIENTO',
      label: 'Procedimiento Ambulatorio',
      sublabel: 'Salas de cura, honorarios y descartables',
      icon: Syringe,
      colorClass: 'text-amber-400 bg-amber-500/10 border-amber-500/20'
    },
    {
      type: 'HOSPITALARIO',
      label: 'Estancia Hospitalaria',
      sublabel: 'Habitaciones, camas, tarifa día y lencería',
      icon: Building2,
      colorClass: 'text-cyan-400 bg-cyan-500/10 border-cyan-500/20'
    },
    {
      type: 'SERVICIO',
      label: 'Servicio Base / General',
      sublabel: 'Servicios generales y administrativos',
      icon: Package,
      colorClass: 'text-blue-400 bg-blue-500/10 border-blue-500/20'
    }
  ];

  readonly icons = {
    Package, Search, Plus, Edit, Trash2, Database, Stethoscope, Scan, X, Check, Clock,
    ArrowUpDown, ArrowUp, ArrowDown, Filter, List, SlidersHorizontal, Building2,
    ChevronDown, Scissors, FlaskConical, Pill, Syringe, RotateCcw
  };

  // ── Computed Signal: Filtrado y ordenamiento reactivo declarativo ──────────
  readonly filteredCatalog = computed(() => {
    let list = [...this.catalog()];
    const selected = this.selectedTypes();
    const query = this.searchQuery().trim().toLowerCase();
    const sort = this.sortOption();
    const estado = this.filtroEstado();

    // 1. Filtro por Estado (TODOS / ACTIVOS / DESACTIVADOS)
    if (estado === 'ACTIVOS') {
      list = list.filter(item => item.activo !== false);
    } else if (estado === 'DESACTIVADOS') {
      list = list.filter(item => item.activo === false);
    }

    // 2. Filtro por tipos de servicio seleccionados
    if (selected.length > 0) {
      list = list.filter(item => {
        const itemType = (item.editorType || item.tipo || 'SERVICIO').toUpperCase();
        return selected.some(targetType => targetType.toUpperCase() === itemType);
      });
    }

    // 3. Filtro por texto (Nombre o Código)
    if (query) {
      list = list.filter(item =>
        (item.descripcion || '').toLowerCase().includes(query) ||
        (item.codigo || '').toLowerCase().includes(query)
      );
    }

    // 4. Ordenamiento declarativo
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

  readonly pendingEditId = signal<string | null>(null);

  constructor() {
    this.route.queryParams.pipe(takeUntilDestroyed()).subscribe(params => {
      if (params['filter']) {
        const filterType = params['filter'].toUpperCase();
        if (this.availableTypes.includes(filterType)) {
          this.selectedTypes.set([filterType]);
        }
      }
      const editId = params['edit'] || params['editServicioId'];
      if (editId) {
        this.pendingEditId.set(editId);
        this.checkPendingEdit();
      }
    });
  }

  ngOnInit(): void {
    this.loadCatalog();
  }

  loadCatalog(): void {
    this.isLoading.set(true);

    // Fallback defensivo para método de servicio
    const fetch$ = typeof this.catalogService.getItems === 'function'
      ? this.catalogService.getItems()
      : (this.catalogService as any).getUnifiedCatalog();

    fetch$.subscribe({
      next: (data: CatalogItem[]) => {
        this.catalog.set(data || []);
        this.isLoading.set(false);
        this.checkPendingEdit();
      },
      error: (err: any) => {
        console.error('Error cargando catálogo:', err);
        this.isLoading.set(false);
      }
    });
  }

  private checkPendingEdit(): void {
    const editId = this.pendingEditId();
    if (editId && this.catalog().length > 0) {
      const found = this.catalog().find(i => (i.id || (i as any)._id || i.codigo) === editId);
      if (found) {
        this.openEdit(found);
        this.pendingEditId.set(null);
      }
    }
  }

  setSortOption(option: SortOption): void {
    this.sortOption.set(option);
  }

  isTypeSelected(type: string): boolean {
    if (type === 'TODOS') return this.selectedTypes().length === 0;
    return this.selectedTypes().includes(type);
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
  resolveEditorType(item: CatalogItem | null | undefined): CatalogEditorType {
    if (!item) return 'SERVICIO';
    if (item.esLegacy) return 'LABORATORIO';

    const type = (item.editorType || item.tipo || '').toUpperCase().trim();

    switch (type) {
      case 'SERVICIO':
      case 'CONSULTA':
      case 'LABORATORIO':
      case 'TOMOGRAFIA':
      case 'MEDICAMENTO':
      case 'CIRUGIA':
      case 'HOSPITALARIO':
      case 'PROCEDIMIENTO':
        return type as CatalogEditorType;

      default:
        return 'SERVICIO'; // Fallback exclusivo para registros sin tipo asignado (Servicio Base)
    }
  }

  toggleCreateDropdown(): void {
    this.showCreateDropdown.update(v => !v);
  }

  closeCreateDropdown(): void {
    this.showCreateDropdown.set(false);
  }

  openCreateOfType(type: CatalogEditorType): void {
    this.closeCreateDropdown();
    this.isEditing.set(false);
    this.selectedItemId.set(null);
    this.activeEditorType.set(type);
    this.showModal.set(true);
  }

  openCreate(): void {
    this.toggleCreateDropdown();
  }

  openEdit(item: CatalogItem): void {
    const idToUse = item.id || (item as any)._id || item.codigo || null;

    this.isEditing.set(true);
    this.selectedItemId.set(idToUse);
    this.activeEditorType.set(this.resolveEditorType(item));
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
    const id = item?.id || (item as any)?._id;
    if (!id) return;

    this.catalogService.deleteItem(id).subscribe({
      next: () => {
        // Soft delete: actualizar el ítem localmente a activo = false para mantenerlo visible
        this.catalog.update(items =>
          items.map(i => (i.id || (i as any)._id) === id ? ({ ...i, activo: false } as CatalogItem) : i)
        );
        this.itemToDelete.set(null);
      },
      error: (err) => console.error('Error desactivando ítem:', err)
    });
  }

  reactivateItem(item: CatalogItem): void {
    const id = item.id || (item as any)._id;
    if (!id) return;

    this.catalogService.reactivateItem(id).subscribe({
      next: () => {
        this.catalog.update(items =>
          items.map(i => (i.id || (i as any)._id) === id ? ({ ...i, activo: true } as CatalogItem) : i)
        );
      },
      error: (err) => console.error('Error reactivando ítem:', err)
    });
  }

  onEditorSaved(): void {
    this.closeAndResetEditor();
    this.loadCatalog();
  }

  onEditorClosed(): void {
    this.closeAndResetEditor();
  }

  private closeAndResetEditor(): void {
    this.showModal.set(false);
    this.selectedItemId.set(null);
    this.activeEditorType.set(null);
    this.isEditing.set(false);
  }

  getTipoBadgeStyle(tipo?: string | null): string {
    return getBadgeStyle(tipo);
  }

  getBadgeClass(tipo: string): string {
    return getBadgeStyle(tipo);
  }
}