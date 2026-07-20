import { Component, signal, computed, output, input, inject, OnInit, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
  LucideAngularModule,
  X, Save, Loader2, Search, Trash2, Check, Package,
  Scissors, FileText, UserCog, Activity, Plus,
  DollarSign, MessageSquare, Layers
} from 'lucide-angular';

import { CatalogService } from '../../../../core/services/catalog.service';
import { InventoryService } from '../../../../core/services/inventory.service';
import { Insumo } from '../../../../core/models/inventory.model';
import { CatalogItem, BasePricedItem } from '../../../../core/models/priced-item.model';

interface BOMLine {
  insumoId: string;
  insumoNombre: string;
  insumoCodigo: string;
  cantidad: number;
  unidadMedida: string;
}

interface EquipoQuirurgico {
  id: string;
  nombre: string;
  codigo: string;
  cantidad: number;
}

@Component({
  selector: 'app-edit-cirugia',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule],
  templateUrl: './edit-cirugia.component.html',
  styleUrls: ['./edit-cirugia.component.scss']
})
export class EditCirugiaComponent implements OnInit {
  // ── Inputs ──────────────────────────────────────────────────────────────
  public readonly itemId = input<string | null>(null);
  public readonly isEditing = input<boolean>(false);

  // ── Outputs ─────────────────────────────────────────────────────────────
  public readonly saved = output<void>();
  public readonly closed = output<void>();

  // ── Services ────────────────────────────────────────────────────────────
  private readonly catalogService = inject(CatalogService);
  private readonly inventoryService = inject(InventoryService);

  // ── Icons ───────────────────────────────────────────────────────────────
  protected readonly icons = {
    X,
    Save,
    Loader2,
    Search,
    Trash2,
    Check,
    Package,
    Scissors,
    FileText,
    UserCog,
    Activity,
    Plus,
    DollarSign,
    MessageSquare,
    Layers,
  } as const;

  // ── Form State (Signals) ────────────────────────────────────────────────
  public readonly nombre = signal('');
  public readonly codigo = signal('');
  public readonly precioBaseUsd = signal(0);
  public readonly honorarioBase = signal(0);
  public readonly activo = signal(true);
  public readonly complejidad = signal('MEDIA');
  public readonly duracionEstimadaMinutos = signal(120);
  public readonly requiereAnestesia = signal(true);
  public readonly tipoAnestesia = signal('GENERAL');
  public readonly clasificacionRiesgo = signal('II');
  public readonly notasPreoperatorias = signal('');
  public readonly notasPostoperatorias = signal('');
  public readonly protocoloQuirurgico = signal('');
  public readonly indicaciones = signal('');
  public readonly contraindicaciones = signal('');

  // Equipo Quirúrgico
  public readonly equipoQuirurgico = signal<EquipoQuirurgico[]>([]);
  public readonly equipoSearchQuery = signal('');
  public readonly showEquipoDropdown = signal(false);

  // BOM / Receta (insumos quirúrgicos)
  public readonly bomLines = signal<BOMLine[]>([]);
  public readonly insumoSearchQuery = signal('');
  public readonly showInsumoDropdown = signal(false);

  // Honorarios del equipo
  public readonly honorariosEquipo = signal<Array<{ rol: string; honorarioUsd: number }>>([]);

  // Sugerencias vinculadas
  public readonly sugerenciasSearchQuery = signal('');
  public readonly selectedSugerenciasIds = signal<string[]>([]);

  // UI State
  public readonly isSaving = signal(false);
  public readonly allInsumos = signal<Insumo[]>([]);
  public readonly allSugerencias = signal<CatalogItem[]>([]);
  public readonly allEquipos = signal<CatalogItem[]>([]);

  // ── Computed ────────────────────────────────────────────────────────────
  public readonly filteredInsumos = computed(() => {
    const q = this.insumoSearchQuery().toLowerCase().trim();
    if (!q) return this.allInsumos().slice(0, 20);
    return this.allInsumos().filter(i =>
      i.nombre.toLowerCase().includes(q) ||
      i.codigo.toLowerCase().includes(q)
    ).slice(0, 20);
  });

  public readonly filteredEquipos = computed(() => {
    const q = this.equipoSearchQuery().toLowerCase().trim();
    const selected = new Set(this.equipoQuirurgico().map(e => e.id));
    return this.allEquipos()
      .filter(e => !selected.has(e.id))
      .filter(e => !q || e.descripcion.toLowerCase().includes(q) || e.codigo.toLowerCase().includes(q))
      .slice(0, 20);
  });

  public readonly filteredSugerencias = computed(() => {
    const q = this.sugerenciasSearchQuery().toLowerCase().trim();
    const selected = new Set(this.selectedSugerenciasIds());
    return this.allSugerencias()
      .filter(s => !selected.has(s.id))
      .filter(s => !q || s.descripcion.toLowerCase().includes(q) || s.codigo.toLowerCase().includes(q));
  });

  public readonly selectedSugerenciasCards = computed(() => {
    const ids = new Set(this.selectedSugerenciasIds());
    return this.allSugerencias().filter(s => ids.has(s.id));
  });

  // ── Lifecycle ───────────────────────────────────────────────────────────
  ngOnInit() {
    this.loadInsumos();
    this.loadSugerencias();
    this.loadEquipos();

    if (this.isEditing() && this.itemId()) {
      this.loadItem(this.itemId()!);
    }
  }

  private loadInsumos() {
    this.inventoryService.getInsumos().subscribe({
      next: (data) => this.allInsumos.set(data),
      error: () => console.error('Error loading insumos')
    });
  }

  private loadSugerencias() {
    this.catalogService.getItems().subscribe({
      next: (data) => this.allSugerencias.set(data.filter(i => i.tipo !== 'CIRUGIA')),
      error: () => console.error('Error loading sugerencias')
    });
  }

  private loadEquipos() {
    this.catalogService.getItems().subscribe({
      next: (data) => this.allEquipos.set(data.filter(i => i.tipo === 'EQUIPO' || i.tipo === 'INSTRUMENTAL')),
      error: () => console.error('Error loading equipos')
    });
  }

  private loadItem(id: string) {
    this.catalogService.getItemById(id).subscribe({
      next: (item) => this.populateForm(item),
      error: () => console.error('Error loading cirugia item')
    });
  }

  private populateForm(item: CatalogItem) {
    this.nombre.set(item.descripcion || '');
    this.codigo.set(item.codigo || '');
    this.precioBaseUsd.set(item.precioBaseUsd || 0);
    this.honorarioBase.set(item.honorarioBase || 0);
    this.activo.set(item.activo ?? true);
    this.complejidad.set(item.complejidad || 'MEDIA');
    this.duracionEstimadaMinutos.set(item.duracionEstimadaMinutos || 120);
    this.requiereAnestesia.set(item.requiereAnestesia ?? true);
    this.tipoAnestesia.set(item.tipoAnestesia || 'GENERAL');
    this.clasificacionRiesgo.set(item.clasificacionRiesgo || 'II');
    this.notasPreoperatorias.set(item.notasPreoperatorias || '');
    this.notasPostoperatorias.set(item.notasPostoperatorias || '');
    this.protocoloQuirurgico.set(item.protocoloQuirurgico || '');
    this.indicaciones.set(item.indicaciones || '');
    this.contraindicaciones.set(item.contraindicaciones || '');

    // Load BOM
    this.inventoryService.getRecipe(item.id).subscribe({
      next: (recipe) => {
        const lines: BOMLine[] = recipe.map(r => ({
          insumoId: r.insumoId,
          insumoNombre: r.insumoNombre,
          insumoCodigo: r.insumoCodigo,
          cantidad: r.cantidad,
          unidadMedida: r.unidadMedidaConsumo
        }));
        this.bomLines.set(lines);
      },
      error: () => console.error('Error loading recipe')
    });

    // Load equipo quirúrgico
    if (item.equipoQuirurgico?.length) {
      this.equipoQuirurgico.set(item.equipoQuirurgico);
    }

    // Load honorarios equipo
    if (item.honorariosEquipo?.length) {
      this.honorariosEquipo.set(item.honorariosEquipo);
    }

    // Load sugerencias vinculadas
    if (item.sugerenciasIds?.length) {
      this.selectedSugerenciasIds.set(item.sugerenciasIds);
    }
  }

  // ── Equipo Quirúrgico Handlers ──────────────────────────────────────────
  public addEquipoToList(equipo: CatalogItem) {
    const exists = this.equipoQuirurgico().some(e => e.id === equipo.id);
    if (exists) return;

    this.equipoQuirurgico.update(list => [...list, {
      id: equipo.id,
      nombre: equipo.descripcion,
      codigo: equipo.codigo,
      cantidad: 1
    }]);

    this.equipoSearchQuery.set('');
    this.showEquipoDropdown.set(false);
  }

  public updateEquipoCantidad(index: number, value: number) {
    this.equipoQuirurgico.update(list => {
      const newList = [...list];
      newList[index] = { ...newList[index], cantidad: Math.max(1, value) };
      return newList;
    });
  }

  public removeEquipo(index: number) {
    this.equipoQuirurgico.update(list => list.filter((_, i) => i !== index));
  }

  // ── BOM Handlers ────────────────────────────────────────────────────────
  public addInsumoToBOM(insumo: Insumo) {
    const exists = this.bomLines().some(l => l.insumoId === insumo.id);
    if (exists) return;

    this.bomLines.update(lines => [...lines, {
      insumoId: insumo.id,
      insumoNombre: insumo.nombre,
      insumoCodigo: insumo.codigo,
      cantidad: 1,
      unidadMedida: insumo.unidadMedidaBase
    }]);

    this.insumoSearchQuery.set('');
    this.showInsumoDropdown.set(false);
  }

  public updateBOMCantidad(index: number, value: number) {
    this.bomLines.update(lines => {
      const newLines = [...lines];
      newLines[index] = { ...newLines[index], cantidad: Math.max(0, value) };
      return newLines;
    });
  }

  public removeBOMLine(index: number) {
    this.bomLines.update(lines => lines.filter((_, i) => i !== index));
  }

  // ── Honorarios Equipo Handlers ──────────────────────────────────────────
  public addHonorarioRol() {
    this.honorariosEquipo.update(list => [...list, { rol: '', honorarioUsd: 0 }]);
  }

  public updateHonorarioRol(index: number, field: 'rol' | 'honorarioUsd', value: string | number) {
    this.honorariosEquipo.update(list => {
      const newList = [...list];
      newList[index] = { ...newList[index], [field]: value };
      return newList;
    });
  }

  public removeHonorarioRol(index: number) {
    this.honorariosEquipo.update(list => list.filter((_, i) => i !== index));
  }

  // ── Sugerencias Handlers ────────────────────────────────────────────────
  public toggleSugerencia(id: string) {
    this.selectedSugerenciasIds.update(ids => {
      const set = new Set(ids);
      if (set.has(id)) set.delete(id);
      else set.add(id);
      return Array.from(set);
    });
  }

  public isSugerenciaSelected(id: string): boolean {
    return this.selectedSugerenciasIds().includes(id);
  }

  public removeSugerencia(id: string) {
    this.selectedSugerenciasIds.update(ids => ids.filter(i => i !== id));
  }

  // ── Save ────────────────────────────────────────────────────────────────
  public save() {
    if (!this.nombre() || !this.codigo() || this.precioBaseUsd() <= 0) return;
    this.isSaving.set(true);

    const item: Partial<CatalogItem> = {
      descripcion: this.nombre(),
      codigo: this.codigo(),
      precioBaseUsd: this.precioBaseUsd(),
      honorarioBase: this.honorarioBase(),
      tipo: 'CIRUGIA',
      activo: this.activo(),
      complejidad: this.complejidad(),
      duracionEstimadaMinutos: this.duracionEstimadaMinutos(),
      requiereAnestesia: this.requiereAnestesia(),
      tipoAnestesia: this.tipoAnestesia(),
      clasificacionRiesgo: this.clasificacionRiesgo(),
      notasPreoperatorias: this.notasPreoperatorias(),
      notasPostoperatorias: this.notasPostoperatorias(),
      protocoloQuirurgico: this.protocoloQuirurgico(),
      indicaciones: this.indicaciones(),
      contraindicaciones: this.contraindicaciones(),
      equipoQuirurgico: this.equipoQuirurgico(),
      honorariosEquipo: this.honorariosEquipo(),
      sugerenciasIds: this.selectedSugerenciasIds(),
      requiereInventario: this.bomLines().length > 0
    };

    if (this.isEditing() && this.itemId()) {
      this.catalogService.updateItem(this.itemId()!, item as CatalogItem).subscribe({
        next: () => this.saveRecipes(this.itemId()!),
        error: () => { this.isSaving.set(false); console.error('Error updating cirugia'); }
      });
    } else {
      this.catalogService.createItem(item).subscribe({
        next: (newId) => this.saveRecipes(newId),
        error: () => { this.isSaving.set(false); console.error('Error creating cirugia'); }
      });
    }
  }

  private saveRecipes(servicioId: string) {
    const lines = this.bomLines();
    if (lines.length === 0) {
      this.isSaving.set(false);
      this.saved.emit();
      this.closed.emit();
      return;
    }

    let completed = 0;
    const total = lines.length;

    lines.forEach(line => {
      this.inventoryService.createOrUpdateRecipe({
        servicioClinicoId: servicioId,
        insumoId: line.insumoId,
        cantidad: line.cantidad,
        unidadMedidaConsumo: line.unidadMedida
      }).subscribe({
        next: () => {
          completed++;
          if (completed >= total) {
            this.isSaving.set(false);
            this.saved.emit();
            this.closed.emit();
          }
        },
        error: () => {
          completed++;
          if (completed >= total) {
            this.isSaving.set(false);
            this.saved.emit();
            this.closed.emit();
          }
        }
      });
    });
  }

  // ── Modal ───────────────────────────────────────────────────────────────
  public close() {
    this.closed.emit();
  }

  // ── Helpers ─────────────────────────────────────────────────────────────
  public getTipoColor(tipo: string): string {
    switch (tipo?.toUpperCase()) {
      case 'CONSULTA': return 'bg-rose-500/10 text-rose-500 border-rose-500/20';
      case 'LABORATORIO': return 'bg-emerald-500/10 text-emerald-400 border-emerald-500/20';
      case 'RX': return 'bg-rose-500/10 text-rose-500 border-rose-500/20';
      case 'PROCEDIMIENTO': return 'bg-amber-500/10 text-amber-400 border-amber-500/20';
      case 'TOMOGRAFIA': return 'bg-violet-500/10 text-violet-400 border-violet-500/20';
      case 'MEDICINA':
      case 'MEDICAMENTO': return 'bg-violet-500/10 text-violet-400 border-violet-500/20';
      case 'CIRUGIA': return 'bg-red-500/10 text-red-400 border-red-500/20';
      default: return 'bg-slate-500/10 text-slate-400 border-slate-500/20';
    }
  }

  public getComplejidadColor(complejidad: string): string {
    switch (complejidad) {
      case 'BAJA': return 'bg-green-500/10 text-green-400 border-green-500/20';
      case 'MEDIA': return 'bg-amber-500/10 text-amber-400 border-amber-500/20';
      case 'ALTA': return 'bg-orange-500/10 text-orange-400 border-orange-500/20';
      case 'MUY_ALTA': return 'bg-red-500/10 text-red-400 border-red-500/20';
      default: return 'bg-slate-500/10 text-slate-400 border-slate-500/20';
    }
  }

  public getRiesgoColor(riesgo: string): string {
    switch (riesgo) {
      case 'I': return 'bg-green-500/10 text-green-400 border-green-500/20';
      case 'II': return 'bg-amber-500/10 text-amber-400 border-amber-500/20';
      case 'III': return 'bg-orange-500/10 text-orange-400 border-orange-500/20';
      case 'IV': return 'bg-red-500/10 text-red-400 border-red-500/20';
      default: return 'bg-slate-500/10 text-slate-400 border-slate-500/20';
    }
  }
}