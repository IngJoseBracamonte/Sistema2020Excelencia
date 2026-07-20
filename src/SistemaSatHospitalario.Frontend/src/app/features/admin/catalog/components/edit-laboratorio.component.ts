import { Component, signal, computed, output, input, inject, OnInit, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
  LucideAngularModule,
  X, Save, Loader2, Search, Trash2, Check, Package,
  FlaskConical, FileText, Stethoscope, Layers, Plus,
  Microscope, TestTube2
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

@Component({
  selector: 'app-edit-laboratorio',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule],
  templateUrl: './edit-laboratorio.component.html',
  styleUrls: ['./edit-laboratorio.component.scss']
})
export class EditLaboratorioComponent implements OnInit {
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
    FlaskConical,
    FileText,
    Stethoscope,
    Layers,
    Plus,
    Microscope,
    TestTube2,
  } as const;

  // ── Form State (Signals) ────────────────────────────────────────────────
  public readonly nombre = signal('');
  public readonly codigo = signal('');
  public readonly precioBaseUsd = signal(0);
  public readonly honorarioBase = signal(0);
  public readonly activo = signal(true);
  public readonly requiereAyuno = signal(false);
  public readonly tipoMuestra = signal('');
  public readonly tiempoResultado = signal('');
  public readonly condicionesEspeciales = signal('');
  public readonly metodologia = signal('');
  public readonly unidadMedida = signal('');
  public readonly valoresReferencia = signal('');
  public readonly interpretacion = signal('');

  // BOM / Receta
  public readonly bomLines = signal<BOMLine[]>([]);
  public readonly insumoSearchQuery = signal('');
  public readonly showInsumoDropdown = signal(false);

  // Sugerencias vinculadas
  public readonly sugerenciasSearchQuery = signal('');
  public readonly selectedSugerenciasIds = signal<string[]>([]);

  // UI State
  public readonly isSaving = signal(false);
  public readonly allInsumos = signal<Insumo[]>([]);
  public readonly allSugerencias = signal<CatalogItem[]>([]);

  // ── Computed ────────────────────────────────────────────────────────────
  public readonly filteredInsumos = computed(() => {
    const q = this.insumoSearchQuery().toLowerCase().trim();
    if (!q) return this.allInsumos().slice(0, 20);
    return this.allInsumos().filter(i =>
      i.nombre.toLowerCase().includes(q) ||
      i.codigo.toLowerCase().includes(q)
    ).slice(0, 20);
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
  constructor() {
    this.loadInsumos();
    this.loadSugerencias();

    // React to itemId changes (for editing different items without destroying component)
    effect(() => {
      const id = this.itemId();
      if (this.isEditing() && id) {
        this.loadItem(id);
      }
    });
  }

  ngOnInit() {
  }

  private loadInsumos() {
    this.inventoryService.getInsumos().subscribe({
      next: (data) => this.allInsumos.set(data),
      error: () => console.error('Error loading insumos')
    });
  }

  private loadSugerencias() {
    this.catalogService.getItems().subscribe({
      next: (data) => this.allSugerencias.set(data.filter(i => i.tipo !== 'LABORATORIO')),
      error: () => console.error('Error loading sugerencias')
    });
  }

  private loadItem(id: string) {
    this.catalogService.getItemById(id).subscribe({
      next: (item) => this.populateForm(item),
      error: () => console.error('Error loading laboratorio item')
    });
  }

  private populateForm(item: CatalogItem) {
    this.nombre.set(item.descripcion || '');
    this.codigo.set(item.codigo || '');
    this.precioBaseUsd.set(item.precioBaseUsd || 0);
    this.honorarioBase.set(item.honorarioBase || 0);
    this.activo.set(item.activo ?? true);
    this.requiereAyuno.set(item.requiereAyuno ?? false);
    this.tipoMuestra.set(item.tipoMuestra || '');
    this.tiempoResultado.set(item.tiempoResultado || '');
    this.condicionesEspeciales.set(item.condicionesEspeciales || '');
    this.metodologia.set(item.metodologia || '');
    this.unidadMedida.set(item.unidadMedida || '');
    this.valoresReferencia.set(item.valoresReferencia || '');
    this.interpretacion.set(item.interpretacion || '');

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

    // Load sugerencias vinculadas
    if (item.sugerenciasIds?.length) {
      this.selectedSugerenciasIds.set(item.sugerenciasIds);
    }
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
      tipo: 'LABORATORIO',
      activo: this.activo(),
      requiereAyuno: this.requiereAyuno(),
      tipoMuestra: this.tipoMuestra(),
      tiempoResultado: this.tiempoResultado(),
      condicionesEspeciales: this.condicionesEspeciales(),
      metodologia: this.metodologia(),
      unidadMedida: this.unidadMedida(),
      valoresReferencia: this.valoresReferencia(),
      interpretacion: this.interpretacion(),
      sugerenciasIds: this.selectedSugerenciasIds(),
      requiereInventario: this.bomLines().length > 0
    };

    if (this.isEditing() && this.itemId()) {
      this.catalogService.updateItem(this.itemId()!, item as CatalogItem).subscribe({
        next: () => this.saveRecipes(this.itemId()!),
        error: () => { this.isSaving.set(false); console.error('Error updating laboratorio'); }
      });
    } else {
      this.catalogService.createItem(item).subscribe({
        next: (newId) => this.saveRecipes(newId),
        error: () => { this.isSaving.set(false); console.error('Error creating laboratorio'); }
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
    let hasErrors = false;
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
            if (!hasErrors) {
              this.saved.emit();
              this.closed.emit();
            } else {
              console.error('Algunas recetas no se guardaron correctamente');
              this.isSaving.set(false);
            }
          }
        },
        error: (err) => {
          hasErrors = true;
          completed++;
          console.error('Error guardando receta para insumo:', line.insumoNombre, err);
          if (completed >= total) {
            this.isSaving.set(false);
            // Aunque hubo errores en recetas, el servicio principal se guardó
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
      default: return 'bg-slate-500/10 text-slate-400 border-slate-500/20';
    }
  }
}