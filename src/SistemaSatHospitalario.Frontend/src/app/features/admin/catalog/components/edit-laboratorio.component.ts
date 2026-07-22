import { Component, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
  LucideAngularModule,
  X, Save, Loader2, Search, Trash2, Check, Package,
  FlaskConical, FileText, Stethoscope, Layers, Plus,
  Microscope, TestTube2
} from 'lucide-angular';
import { BaseCatalogEditComponent } from './base-catalog-edit.component';
import { CatalogItem } from '../../../../core/services/catalog.service';
import { Insumo } from '../../../../core/models/inventory.model';
import { BOMLine } from '../models/catalog-edit.models';

@Component({
  selector: 'app-edit-laboratorio',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule],
  templateUrl: './edit-laboratorio.component.html'
})
export class EditLaboratorioComponent extends BaseCatalogEditComponent implements OnInit {
  // ── Icons ───────────────────────────────────────────────────────────────
  protected readonly icons = {
    X, Save, Loader2, Search, Trash2, Check, Package,
    FlaskConical, FileText, Stethoscope, Layers, Plus,
    Microscope, TestTube2
  } as const;

  // ── Laboratorio Specific State (Signals) ────────────────────────────────
  public readonly requiereAyuno = signal<boolean>(false);
  public readonly tipoMuestra = signal<string>('');
  public readonly tiempoResultado = signal<string>('');
  public readonly condicionesEspeciales = signal<string>('');
  public readonly metodologia = signal<string>('');
  public readonly unidadMedida = signal<string>('');
  public readonly valoresReferencia = signal<string>('');
  public readonly interpretacion = signal<string>('');

  // ── Handlers & Compatibility Signals ────────────────────────────────────
  public readonly availableInsumos = this.bomHandler.availableInsumos;
  public readonly insumoSearchQuery = this.bomHandler.insumoSearchQuery;
  public readonly showInsumoDropdown = this.bomHandler.showInsumoDropdown;
  public readonly bomLines = this.bomHandler.bomLines;
  public readonly filteredInsumos = this.bomHandler.filteredInsumos;

  public readonly allSugerencias = this.sugerenciasHandler.allCatalogItems;
  public readonly sugerenciasSearchQuery = this.sugerenciasHandler.sugerenciasSearchQuery;
  public readonly selectedSugerenciasIds = this.sugerenciasHandler.sugerenciasIds;
  public readonly filteredSugerencias = this.sugerenciasHandler.filteredSugerencias;
  public readonly selectedSugerenciasCards = this.sugerenciasHandler.selectedSugerenciasCards;

  ngOnInit(): void {
    this.loadInsumos();
    this.loadCatalogForSugerencias('LABORATORIO');
  }

  protected loadItem(id: string): void {
    this.catalogService.getItemById(id).subscribe({
      next: (item) => this.populateForm(item),
      error: () => console.error('Error loading laboratorio item')
    });
  }

  protected resetForm(): void {
    this.resetBaseForm();
    this.requiereAyuno.set(false);
    this.tipoMuestra.set('');
    this.tiempoResultado.set('');
    this.condicionesEspeciales.set('');
    this.metodologia.set('');
    this.unidadMedida.set('');
    this.valoresReferencia.set('');
    this.interpretacion.set('');
  }

  private populateForm(item: CatalogItem): void {
    const itemAny = item as any;
    this.nombre.set(item.descripcion || '');
    this.codigo.set(item.codigo || '');
    this.precioBaseUsd.set(item.precioUsd ?? 0);
    this.honorarioBase.set(item.honorarioBase || 0);
    this.activo.set(item.activo ?? true);
    this.requiereAyuno.set(itemAny.requiereAyuno ?? false);
    this.tipoMuestra.set(itemAny.tipoMuestra || '');
    this.tiempoResultado.set(itemAny.tiempoResultado || '');
    this.condicionesEspeciales.set(itemAny.condicionesEspeciales || '');
    this.metodologia.set(itemAny.metodologia || '');
    this.unidadMedida.set(itemAny.unidadMedida || '');
    this.valoresReferencia.set(itemAny.valoresReferencia || '');
    this.interpretacion.set(itemAny.interpretacion || '');

    // Load recipe/BOM
    this.inventoryService.getRecetas().subscribe({
      next: (recetas: any[]) => {
        const itemRecetas = recetas.filter(r => r.servicioClinicoId === item.id);
        const lines: BOMLine[] = itemRecetas.map(r => ({
          insumoId: r.insumoId,
          insumoNombre: r.insumoNombre || (r.insumo ? r.insumo.nombre : ''),
          insumoCodigo: r.insumoCodigo || (r.insumo ? r.insumo.codigo : ''),
          cantidad: r.cantidad,
          unidadMedida: r.unidadMedidaConsumo
        }));
        this.bomLines.set(lines);
      },
      error: () => console.error('Error loading recipe')
    });

    if (item.sugerenciasIds?.length) {
      this.selectedSugerenciasIds.set(item.sugerenciasIds);
    }
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

  public onInsumoBlur(): void {
    this.bomHandler.onInsumoBlur();
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

  public close(): void {
    this.onClose();
  }

  public save(): void {
    if (!this.nombre() || !this.codigo() || this.precioBaseUsd() <= 0) return;
    this.isSaving.set(true);

    const item: any = {
      descripcion: this.nombre(),
      codigo: this.codigo(),
      precioUsd: this.precioBaseUsd(),
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

  private saveRecipes(servicioId: string): void {
    const lines = this.bomLines();
    if (lines.length === 0) {
      this.isSaving.set(false);
      this.saved.emit();
      this.onClose();
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
              this.onClose();
            } else {
              console.error('Algunas recetas no se guardaron correctamente');
            }
          }
        },
        error: (err) => {
          hasErrors = true;
          completed++;
          console.error('Error guardando receta para insumo:', line.insumoNombre, err);
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