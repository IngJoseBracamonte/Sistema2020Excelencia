import { Component, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
  LucideAngularModule, X, Save, Loader2, Search, Trash2, Check,
  FileText, UserCog, DollarSign, Building2, Bed, ArrowRightLeft, ShieldAlert
} from 'lucide-angular';
import { BaseCatalogEditComponent } from './base-catalog-edit.component';
import { CatalogItem } from '../../../../core/services/catalog.service';
import { AreaHospitalaria, ModalidadCobroHospitalario } from '../models/catalog-edit.models';

@Component({
  selector: 'app-edit-hospitalario',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule],
  templateUrl: './edit-hospitalario.component.html'
})
export class EditHospitalarioComponent extends BaseCatalogEditComponent implements OnInit {
  // ── Icons ───────────────────────────────────────────────────────────────
  protected readonly icons = {
    X, Save, Loader2, Search, Trash2, Check, FileText, UserCog, DollarSign,
    Building2, Bed, ArrowRightLeft, ShieldAlert
  } as const;

  // ── Hospitalario Form State (Signals) ───────────────────────────────────
  public readonly areaVinculada = signal<AreaHospitalaria>('HOSPITALIZACION');
  public readonly modalidadCobro = signal<ModalidadCobroHospitalario>('POR_TRASLADO');
  public readonly aplicaTraslado = signal<boolean>(true);
  public readonly notasFacturacion = signal<string>('');

  ngOnInit(): void {
    // Component initialization logic if needed
  }

  protected loadItem(id: string): void {
    this.catalogService.getItemById(id).subscribe({
      next: (item) => this.populateForm(item),
      error: () => console.error('Error loading hospitalario item')
    });
  }

  protected resetForm(): void {
    this.resetBaseForm();
    this.areaVinculada.set('HOSPITALIZACION');
    this.modalidadCobro.set('POR_TRASLADO');
    this.aplicaTraslado.set(true);
    this.notasFacturacion.set('');
  }

  private populateForm(item: CatalogItem): void {
    const itemAny = item as any;
    this.nombre.set(item.descripcion || '');
    this.codigo.set(item.codigo || '');
    this.precioBaseUsd.set(item.precioUsd ?? 0);
    this.honorarioBase.set(item.honorarioBase || 0);
    this.activo.set(item.activo ?? true);

    this.areaVinculada.set(itemAny.areaVinculada || 'HOSPITALIZACION');
    this.modalidadCobro.set(itemAny.modalidadCobro || 'POR_TRASLADO');
    this.aplicaTraslado.set(itemAny.aplicaTraslado ?? true);
    this.notasFacturacion.set(itemAny.notasFacturacion || '');
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
      tipo: 'HOSPITALARIO',
      activo: this.activo(),
      areaVinculada: this.areaVinculada(),
      modalidadCobro: this.modalidadCobro(),
      aplicaTraslado: this.aplicaTraslado(),
      notasFacturacion: this.notasFacturacion()
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
          console.error('Error updating hospitalario item');
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
          console.error('Error creating hospitalario item');
        }
      });
    }
  }
}
