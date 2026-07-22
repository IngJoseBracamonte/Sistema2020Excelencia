import { Component, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
  LucideAngularModule, Package, Search, Plus, Trash2, X, Check,
  FlaskConical, Syringe, Stethoscope, Beaker, Save, Loader2
} from 'lucide-angular';
import { BaseCatalogEditComponent } from './base-catalog-edit.component';
import { CatalogItem } from '../../../../core/services/catalog.service';
import { Insumo } from '../../../../core/models/inventory.model';
import { BOMLine } from '../models/catalog-edit.models';

@Component({
  selector: 'app-edit-procedimiento',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule],
  templateUrl: './edit-procedimiento.component.html'
})
export class EditProcedimientoComponent extends BaseCatalogEditComponent implements OnInit {
  // ── Icons ───────────────────────────────────────────────────────────────
  protected readonly icons = {
    Package, Search, Plus, Trash2, X, Check,
    FlaskConical, Syringe, Stethoscope, Beaker,
    Save, Loader2
  } as const;

  // State
  public readonly isLoading = signal<boolean>(false);

  // Handlers & Compatibility Signals
  public readonly availableInsumos = this.bomHandler.availableInsumos;
  public readonly insumos = this.bomHandler.availableInsumos;
  public readonly insumoSearchQuery = this.bomHandler.insumoSearchQuery;
  public readonly showInsumoDropdown = this.bomHandler.showInsumoDropdown;
  public readonly bomLines = this.bomHandler.bomLines;
  public readonly filteredInsumos = this.bomHandler.filteredInsumos;

  public readonly allServices = this.sugerenciasHandler.allCatalogItems;
  public readonly sugerenciasSearchQuery = this.sugerenciasHandler.sugerenciasSearchQuery;
  public readonly selectedSugerenciasIds = this.sugerenciasHandler.sugerenciasIds;
  public readonly filteredSugerencias = this.sugerenciasHandler.filteredSugerencias;
  public readonly selectedSugerenciasCards = this.sugerenciasHandler.selectedSugerenciasCards;

  ngOnInit(): void {
    this.loadInsumos();
    this.loadCatalogForSugerencias('PROCEDIMIENTO');
  }

  protected loadItem(id: string): void {
    this.isLoading.set(true);
    this.catalogService.getUnifiedCatalog().subscribe({
      next: (res) => {
        const item = res.find(i => i.id === id);
        if (item) {
          this.nombre.set(item.descripcion || '');
          this.codigo.set(item.codigo || '');
          this.precioBaseUsd.set(item.precioUsd ?? 0);
          this.activo.set(item.activo ?? true);
          this.selectedSugerenciasIds.set(item.sugerenciasIds ?? []);
          this.loadExistingRecipe(id);
        }
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  private loadExistingRecipe(servicioId: string): void {
    this.inventoryService.getRecetas().subscribe({
      next: (recetas) => {
        const relevant = recetas.filter(r => r.servicioClinicoId === servicioId);
        const lines: BOMLine[] = relevant.map(r => ({
          insumoId: r.insumoId,
          insumoNombre: r.insumo?.nombre ?? '',
          insumoCodigo: r.insumo?.codigo ?? '',
          cantidad: r.cantidad,
          unidadMedida: r.unidadMedidaConsumo || r.insumo?.unidadMedidaBase || 'UNIDAD'
        }));
        this.bomLines.set(lines);
      },
      error: () => console.error('Error loading recipes')
    });
  }

  protected resetForm(): void {
    this.resetBaseForm();
  }

  // ── BOM Actions ─────────────────────────────────────────────────────────
  public addInsumoToBOM(insumo: Insumo): void {
    this.bomHandler.addInsumo(insumo, 1);
  }

  public updateBOMCantidad(index: number, value: number): void {
    const line = this.bomLines()[index];
    if (line) {
      this.bomHandler.updateCantidad(line.insumoId, value);
    }
  }

  public removeBOMLine(index: number): void {
    const line = this.bomLines()[index];
    if (line) {
      this.bomHandler.removeLine(line.insumoId);
    }
  }

  // ── Sugerencias Actions ─────────────────────────────────────────────────
  public toggleSugerencia(id: string): void {
    this.sugerenciasHandler.toggleSugerencia(id);
  }

  public isSugerenciaSelected(id: string): boolean {
    return this.selectedSugerenciasIds().includes(id);
  }

  public removeSugerencia(id: string): void {
    this.sugerenciasHandler.removeSugerencia(id);
  }

  public getTipoColor(tipo: string): string {
    switch (tipo?.toUpperCase()) {
      case 'CONSULTA': return 'bg-rose-500/10 text-rose-500 border-rose-500/20';
      case 'LABORATORIO': return 'bg-emerald-500/10 text-emerald-400 border-emerald-500/20';
      case 'RX': return 'bg-rose-500/10 text-rose-500 border-rose-500/20';
      case 'PROCEDIMIENTO': return 'bg-amber-500/10 text-amber-400 border-amber-500/20';
      case 'MEDICINA':
      case 'MEDICAMENTO': return 'bg-violet-500/10 text-violet-400 border-violet-500/20';
      default: return 'bg-slate-500/10 text-slate-400 border-slate-500/20';
    }
  }

  public close(): void {
    this.onClose();
  }

  public save(): void {
    this.isSaving.set(true);
    const item: Partial<CatalogItem> = {
      id: this.itemId() ?? undefined,
      descripcion: this.nombre(),
      codigo: this.codigo(),
      precioUsd: this.precioBaseUsd(),
      tipo: 'PROCEDIMIENTO',
      activo: this.activo(),
      sugerenciasIds: this.selectedSugerenciasIds(),
      requiereInventario: this.bomLines().length > 0
    };

    if (this.isEditing() && this.itemId()) {
      this.catalogService.updateItem(this.itemId()!, item as CatalogItem).subscribe({
        next: () => this.saveRecipes(this.itemId()!),
        error: () => {
          this.isSaving.set(false);
          console.error('Error updating procedimiento');
        }
      });
    } else {
      this.catalogService.createItem(item).subscribe({
        next: (newId) => this.saveRecipes(newId),
        error: () => {
          this.isSaving.set(false);
          console.error('Error creating procedimiento');
        }
      });
    }
  }

  private saveRecipes(servicioId: string): void {
    const lines = this.bomLines();
    if (lines.length === 0) {
      this.isSaving.set(false);
      this.saved.emit();
      this.onClose();
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
            this.onClose();
          }
        },
        error: () => {
          completed++;
          if (completed >= total) {
            this.isSaving.set(false);
            this.saved.emit();
            this.onClose();
          }
        }
      });
    });
  }
}