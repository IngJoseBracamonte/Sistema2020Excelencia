import { Component, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
  LucideAngularModule, Package, Search, Plus, Trash2, X, Check,
  Stethoscope, Save, Loader2, Layers
} from 'lucide-angular';
import { BaseCatalogEditComponent } from './base-catalog-edit.component';
import { CatalogItem } from '../../../../core/services/catalog.service';

@Component({
  selector: 'app-edit-servicio',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule],
  templateUrl: './edit-servicio.component.html'
})
export class EditServicioComponent extends BaseCatalogEditComponent implements OnInit {
  // ── Icons ───────────────────────────────────────────────────────────────
  protected readonly icons = {
    Package, Search, Plus, Trash2, X, Check,
    Stethoscope, Save, Loader2, Layers
  } as const;

  // State
  public readonly isLoading = signal<boolean>(false);

  // Sugerencias Signals
  public readonly allServices = this.sugerenciasHandler.allCatalogItems;
  public readonly sugerenciasSearchQuery = this.sugerenciasHandler.sugerenciasSearchQuery;
  public readonly selectedSugerenciasIds = this.sugerenciasHandler.sugerenciasIds;
  public readonly filteredSugerencias = this.sugerenciasHandler.filteredSugerencias;
  public readonly selectedSugerenciasCards = this.sugerenciasHandler.selectedSugerenciasCards;

  ngOnInit(): void {
    this.loadCatalogForSugerencias('SERVICIO');
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
        }
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  protected resetForm(): void {
    this.resetBaseForm();
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
    this.isSaving.set(true);
    const item: any = {
      descripcion: this.nombre(),
      codigo: this.codigo(),
      precioUsd: this.precioBaseUsd(),
      tipo: 'SERVICIO',
      activo: this.activo(),
      sugerenciasIds: this.selectedSugerenciasIds()
    };

    if (this.isEditing() && this.itemId()) {
      this.catalogService.updateItem(this.itemId()!, item as CatalogItem).subscribe({
        next: () => {
          this.isSaving.set(false);
          this.saved.emit();
          this.onClose();
        },
        error: () => {
          this.isSaving.set(false);
          console.error('Error updating servicio');
        }
      });
    } else {
      this.catalogService.createItem(item).subscribe({
        next: () => {
          this.isSaving.set(false);
          this.saved.emit();
          this.onClose();
        },
        error: () => {
          this.isSaving.set(false);
          console.error('Error creating servicio');
        }
      });
    }
  }
}