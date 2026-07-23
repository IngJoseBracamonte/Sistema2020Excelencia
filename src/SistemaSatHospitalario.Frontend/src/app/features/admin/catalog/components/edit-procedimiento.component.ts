import { Component, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
  LucideAngularModule, Package, Search, Plus, Trash2, X, Check,
  Stethoscope, Save, Loader2, FileText, UserCog, Layers, Syringe
} from 'lucide-angular';
import { BaseCatalogEditComponent } from './base-catalog-edit.component';
import { CatalogItem } from '../../../../core/services/catalog.service';
import { Insumo } from '../../../../core/models/inventory.model';
import { BOMLine, MedicoOption } from '../models/catalog-edit.models';

@Component({
  selector: 'app-edit-procedimiento',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule],
  templateUrl: './edit-procedimiento.component.html'
})
export class EditProcedimientoComponent extends BaseCatalogEditComponent implements OnInit {
  protected readonly icons = {
    Package, Search, Plus, Trash2, X, Check,
    Stethoscope, Save, Loader2, FileText, UserCog, Layers, Syringe
  } as const;

  // Handlers & Compatibility Signals
  public readonly availableInsumos = this.bomHandler.availableInsumos;
  public readonly insumoSearchQuery = this.bomHandler.insumoSearchQuery;
  public readonly showInsumoDropdown = this.bomHandler.showInsumoDropdown;
  public readonly bomLines = this.bomHandler.bomLines;
  public readonly filteredInsumos = this.bomHandler.filteredInsumos;

  public readonly availableMedicos = this.honorariosHandler.availableMedicos;
  public readonly medicoSearchQuery = this.honorariosHandler.medicoSearchQuery;
  public readonly showMedicoDropdown = this.honorariosHandler.showMedicoDropdown;
  public readonly honorariosMedicos = this.honorariosHandler.honorarios;
  public readonly filteredMedicos = this.honorariosHandler.filteredMedicos;

  public readonly allSugerencias = this.sugerenciasHandler.allCatalogItems;
  public readonly sugerenciasSearchQuery = this.sugerenciasHandler.sugerenciasSearchQuery;
  public readonly selectedSugerenciasIds = this.sugerenciasHandler.sugerenciasIds;
  public readonly filteredSugerencias = this.sugerenciasHandler.filteredSugerencias;
  public readonly selectedSugerenciasCards = this.sugerenciasHandler.selectedSugerenciasCards;

  ngOnInit(): void {
    this.loadInsumos();
    this.loadMedicos();
    this.loadCatalogForSugerencias('PROCEDIMIENTO');
  }

  protected loadItem(id: string): void {
    this.catalogService.getItemById(id).subscribe({
      next: (item) => this.populateForm(item),
      error: () => console.error('Error loading procedimiento item')
    });
  }

  protected resetForm(): void {
    this.resetBaseForm();
  }

  private populateForm(item: CatalogItem): void {
    this.nombre.set(item.descripcion || '');
    this.codigo.set(item.codigo || '');
    this.precioBaseUsd.set(item.precioUsd ?? 0);
    this.honorarioBase.set(item.honorarioBase || 0);
    this.activo.set(item.activo ?? true);

    if (item.honorariosMedicos?.length) {
      this.honorariosMedicos.set(item.honorariosMedicos.map((h: any) => ({
        medicoId: h.medicoId,
        medicoNombre: h.medicoNombre || 'Médico',
        honorarioUsd: h.honorario
      })));
    }

    this.inventoryService.getRecetas().subscribe({
      next: (recetas: any[]) => {
        const itemRecetas = recetas.filter(r => r.servicioClinicoId === item.id);
        this.bomLines.set(itemRecetas.map((r: any) => ({
          insumoId: r.insumoId,
          insumoNombre: r.insumoNombre || (r.insumo ? r.insumo.nombre : ''),
          insumoCodigo: r.insumoCodigo || (r.insumo ? r.insumo.codigo : ''),
          cantidad: r.cantidad,
          unidadMedida: r.unidadMedidaConsumo
        })));
      },
      error: () => console.error('Error loading recipe')
    });

    if (item.sugerenciasIds?.length) {
      this.selectedSugerenciasIds.set(item.sugerenciasIds);
    }
  }

  // ── BOM Actions ──────────────────────────────────────────────────────────
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

  // ── Honorarios Médicos Actions ───────────────────────────────────────────
  public addMedicoToHonorarios(medico: MedicoOption, defaultFee: number): void {
    this.honorariosHandler.addHonorario(medico, defaultFee);
  }

  public updateHonorarioUsd(medicoId: string, fee: number): void {
    this.honorariosHandler.updateHonorarioUsd(medicoId, fee);
  }

  public removeHonorarioMedico(medicoId: string): void {
    this.honorariosHandler.removeHonorario(medicoId);
  }

  public onMedicoBlur(): void {
    this.honorariosHandler.onMedicoBlur();
  }

  // ── Sugerencias Actions ──────────────────────────────────────────────────
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

    const itemData: any = {
      descripcion: this.nombre(),
      codigo: this.codigo(),
      precioUsd: this.precioBaseUsd(),
      honorarioBase: this.honorarioBase(),
      tipo: 'PROCEDIMIENTO',
      activo: this.activo(),
      honorariosMedicos: this.honorariosMedicos().map(h => ({
        medicoId: h.medicoId,
        honorario: h.honorarioUsd
      })),
      sugerenciasIds: this.selectedSugerenciasIds(),
      requiereInventario: this.bomLines().length > 0
    };

    if (this.isEditing() && this.itemId()) {
      this.catalogService.updateItem(this.itemId()!, itemData as CatalogItem).subscribe({
        next: () => this.saveRecipeAndFinish(this.itemId()!),
        error: () => { this.isSaving.set(false); console.error('Error updating procedimiento'); }
      });
    } else {
      this.catalogService.createItem(itemData).subscribe({
        next: (newId: string) => this.saveRecipeAndFinish(newId),
        error: () => { this.isSaving.set(false); console.error('Error creating procedimiento'); }
      });
    }
  }

  private saveRecipeAndFinish(id: string): void {
    const lines = this.bomLines();
    if (lines.length === 0) {
      this.isSaving.set(false);
      this.saved.emit();
      this.onClose();
      return;
    }

    let completed = 0;
    lines.forEach(line => {
      this.inventoryService.createOrUpdateRecipe({
        servicioClinicoId: id,
        insumoId: line.insumoId,
        cantidad: line.cantidad,
        unidadMedidaConsumo: line.unidadMedida
      }).subscribe({
        next: () => {
          completed++;
          if (completed >= lines.length) {
            this.isSaving.set(false);
            this.saved.emit();
            this.onClose();
          }
        },
        error: () => {
          completed++;
          if (completed >= lines.length) {
            this.isSaving.set(false);
            this.saved.emit();
            this.onClose();
          }
        }
      });
    });
  }
}