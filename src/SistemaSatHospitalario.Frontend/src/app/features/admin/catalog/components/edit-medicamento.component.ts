import { Component, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
  LucideAngularModule,
  Scissors, Pill, Search, X, Plus, Trash2, Save, Loader2, Package,
  DollarSign, AlertTriangle, Check, Layers, MessageSquare, FileText,
  UserCog, Activity
} from 'lucide-angular';
import { BaseCatalogEditComponent } from './base-catalog-edit.component';
import { CatalogItem } from '../../../../core/services/catalog.service';
import { Insumo } from '../../../../core/models/inventory.model';
import { FormOption, HonorarioMedico } from '../models/catalog-edit.models';

@Component({
  selector: 'app-edit-medicamento',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule],
  templateUrl: './edit-medicamento.component.html'
})
export class EditMedicamentoComponent extends BaseCatalogEditComponent implements OnInit {
  // ── Icons ───────────────────────────────────────────────────────────────
  protected readonly icons = {
    Scissors, Pill, Search, X, Plus, Trash2, Save, Loader2,
    Package, DollarSign, AlertTriangle, Check, Layers, MessageSquare,
    FileText, UserCog, Activity, Capsule: Pill
  } as const;

  // ── Medicamento Specific State (Signals) ────────────────────────────────
  public readonly principioActivo = signal<string>('');
  public readonly concentracion = signal<string>('');
  public readonly formaFarmaceutica = signal<string>('TABLETA');
  public readonly viaAdministracion = signal<string>('ORAL');
  public readonly laboratorio = signal<string>('');
  public readonly requiereReceta = signal<boolean>(true);
  public readonly controlado = signal<boolean>(false);
  public readonly stockMinimo = signal<number>(0);
  public readonly stockMaximo = signal<number>(0);
  public readonly lote = signal<string>('');
  public readonly fechaVencimiento = signal<string>('');
  public readonly showModal = signal<boolean>(false);

  // ── Handlers & Compatibility Signals ────────────────────────────────────
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

  public readonly allCatalogItems = this.sugerenciasHandler.allCatalogItems;
  public readonly sugerenciasSearchQuery = this.sugerenciasHandler.sugerenciasSearchQuery;
  public readonly sugerenciasIds = this.sugerenciasHandler.sugerenciasIds;
  public readonly filteredSugerencias = this.sugerenciasHandler.filteredSugerencias;
  public readonly selectedSugerenciasCards = this.sugerenciasHandler.selectedSugerenciasCards;

  // ── Select Options ──────────────────────────────────────────────────────
  readonly formasFarmaceuticas: FormOption[] = [
    { value: 'TABLETA', label: 'Tableta' },
    { value: 'CAPSULA', label: 'Cápsula' },
    { value: 'COMPRIMIDO', label: 'Comprimido' },
    { value: 'JARABE', label: 'Jarabe' },
    { value: 'SUSPENSION', label: 'Suspensión' },
    { value: 'SOLUCION_ORAL', label: 'Solución Oral' },
    { value: 'INYECTABLE_IV', label: 'Inyectable IV' },
    { value: 'INYECTABLE_IM', label: 'Inyectable IM' },
    { value: 'INYECTABLE_SC', label: 'Inyectable SC' },
    { value: 'GOTAS', label: 'Gotitas' },
    { value: 'CREMA', label: 'Crema' },
    { value: 'UNGUENTO', label: 'Ungüento' },
    { value: 'GEL', label: 'Gel' },
    { value: 'PARCHE', label: 'Parche Transdérmico' },
    { value: 'SUPPOSITORIO', label: 'Supositorio' },
    { value: 'OVULO', label: 'Óvulo' },
    { value: 'INHALADOR', label: 'Inhalador' },
    { value: 'NEBULIZADOR', label: 'Solución para Nebulizador' },
    { value: 'COLIRIO', label: 'Colirio' },
    { value: 'OTICO', label: 'Solución Ótica' },
    { value: 'NASAL', label: 'Spray Nasal' },
    { value: 'SUBLINGUAL', label: 'Tableta Sublingual' },
    { value: 'OTRO', label: 'Otro' }
  ];

  readonly viasAdministracion: FormOption[] = [
    { value: 'ORAL', label: 'Oral' },
    { value: 'SUBLINGUAL', label: 'Sublingual' },
    { value: 'RECTAL', label: 'Rectal' },
    { value: 'IV', label: 'Intravenosa (IV)' },
    { value: 'IM', label: 'Intramuscular (IM)' },
    { value: 'SC', label: 'Subcutánea (SC)' },
    { value: 'ID', label: 'Intradérmica (ID)' },
    { value: 'TOPICA', label: 'Tópica/Cutánea' },
    { value: 'TRANSDERMICA', label: 'Transdérmica' },
    { value: 'INHALADA', label: 'Inhalada' },
    { value: 'NASAL', label: 'Nasal' },
    { value: 'OFTALMICA', label: 'Oftálmica' },
    { value: 'OTICA', label: 'Ótica' },
    { value: 'VAGINAL', label: 'Vaginal' },
    { value: 'URETRAL', label: 'Uretral' },
    { value: 'OTRO', label: 'Otra' }
  ];

  ngOnInit(): void {
    this.loadInsumos();
    this.loadMedicos();
    this.loadCatalogForSugerencias();
  }

  protected loadItem(id: string): void {
    this.catalogService.getUnifiedCatalog().subscribe({
      next: (items) => {
        const item = items.find(i => i.id === id);
        if (item) {
          this.currentItem.set(item);
          this.populateForm(item);
        }
      }
    });
  }

  protected resetForm(): void {
    this.resetBaseForm();
    this.principioActivo.set('');
    this.concentracion.set('');
    this.formaFarmaceutica.set('TABLETA');
    this.viaAdministracion.set('ORAL');
    this.laboratorio.set('');
    this.requiereReceta.set(true);
    this.controlado.set(false);
    this.stockMinimo.set(0);
    this.stockMaximo.set(0);
    this.lote.set('');
    this.fechaVencimiento.set('');
  }

  private populateForm(item: CatalogItem): void {
    this.nombre.set(item.descripcion || '');
    this.codigo.set(item.codigo || '');
    this.precioBaseUsd.set(item.precioUsd || 0);
    this.honorarioBase.set(item.honorarioBase || 0);
    this.activo.set(item.activo !== false);

    const itemAny = item as any;
    this.principioActivo.set(itemAny.principioActivo || '');
    this.concentracion.set(itemAny.concentracion || '');
    this.formaFarmaceutica.set(itemAny.formaFarmaceutica || 'TABLETA');
    this.viaAdministracion.set(itemAny.viaAdministracion || 'ORAL');
    this.laboratorio.set(itemAny.laboratorio || '');
    this.requiereReceta.set(itemAny.requiereReceta !== false);
    this.controlado.set(itemAny.controlado === true);
    this.stockMinimo.set(itemAny.stockMinimo || 0);
    this.stockMaximo.set(itemAny.stockMaximo || 0);
    this.lote.set(itemAny.lote || '');
    this.fechaVencimiento.set(itemAny.fechaVencimiento || '');

    if (itemAny.bomLines && Array.isArray(itemAny.bomLines)) {
      this.bomLines.set(itemAny.bomLines);
    }
    if (item.honorariosMedicos && Array.isArray(item.honorariosMedicos)) {
      this.honorariosMedicos.set(item.honorariosMedicos.map((h: any) => ({
        medicoId: h.medicoId,
        medicoNombre: h.medicoNombre || '',
        honorarioUsd: h.honorarioUsd ?? h.honorario ?? 0
      })));
    }
    if (item.sugerenciasIds && Array.isArray(item.sugerenciasIds)) {
      this.sugerenciasIds.set(item.sugerenciasIds);
    }
  }

  // ── Template Actions ───────────────────────────────────────────────────
  public addInsumoToBOM(insumo: Insumo): void {
    this.bomHandler.addInsumo(insumo, 1);
  }

  public updateBOMCantidad(index: number, cantidad: number): void {
    const line = this.bomLines()[index];
    if (line) {
      this.bomHandler.updateCantidad(line.insumoId, cantidad);
    }
  }

  public removeBOMLine(index: number): void {
    const line = this.bomLines()[index];
    if (line) {
      this.bomHandler.removeLine(line.insumoId);
    }
  }

  public addHonorarioMedico(): void {
    this.honorariosMedicos.update(arr => [...arr, { medicoId: '', medicoNombre: '', honorarioUsd: 0 }]);
  }

  public updateHonorarioMedico(index: number, field: keyof HonorarioMedico, value: any): void {
    this.honorariosMedicos.update(arr => {
      const copy = [...arr];
      copy[index] = { ...copy[index], [field]: value };
      return copy;
    });
  }

  public removeHonorarioMedico(index: number): void {
    this.honorariosMedicos.update(arr => arr.filter((_, i) => i !== index));
  }

  public onMedicoSelect(medico: { id: string; nombre: string; especialidad: string }, index: number): void {
    this.updateHonorarioMedico(index, 'medicoId', medico.id);
    this.updateHonorarioMedico(index, 'medicoNombre', medico.nombre);
    this.honorariosHandler.medicoSearchQuery.set('');
    this.honorariosHandler.showMedicoDropdown.set(false);
  }

  public isSugerenciaSelected(id: string): boolean {
    return this.sugerenciasIds().includes(id);
  }

  public toggleSugerencia(id: string): void {
    this.sugerenciasHandler.toggleSugerencia(id);
  }

  public removeSugerencia(id: string): void {
    this.sugerenciasHandler.removeSugerencia(id);
  }

  public onInsumoBlur(): void {
    this.bomHandler.onInsumoBlur();
  }

  public onMedicoBlur(): void {
    this.honorariosHandler.onMedicoBlur();
  }

  public openCreate(): void {
    this.resetForm();
    this.isEditing.set(false);
    this.showModal.set(true);
  }

  public openEdit(item: CatalogItem): void {
    this.resetForm();
    this.isEditing.set(true);
    this.currentItem.set(item);
    this.populateForm(item);
    this.showModal.set(true);
  }

  public close(): void {
    this.showModal.set(false);
    this.resetForm();
    this.onClose();
  }

  public async save(): Promise<void> {
    if (!this.nombre() || !this.codigo() || this.precioBaseUsd() <= 0) return;
    if (!this.principioActivo() || !this.concentracion()) return;

    this.isSaving.set(true);

    const payload = {
      descripcion: this.nombre(),
      codigo: this.codigo(),
      precioUsd: this.precioBaseUsd(),
      tipo: 'MEDICAMENTO' as const,
      honorarioBase: this.honorarioBase(),
      requiereInventario: true,
      sugerenciasIds: this.sugerenciasIds(),
      honorariosMedicos: this.honorariosMedicos()
        .filter(h => h.medicoId && h.honorarioUsd > 0)
        .map(h => ({ medicoId: h.medicoId, medicoNombre: h.medicoNombre, honorario: h.honorarioUsd })),
      activo: this.activo(),
      principioActivo: this.principioActivo(),
      concentracion: this.concentracion(),
      formaFarmaceutica: this.formaFarmaceutica(),
      viaAdministracion: this.viaAdministracion(),
      laboratorio: this.laboratorio(),
      requiereReceta: this.requiereReceta(),
      controlado: this.controlado(),
      stockMinimo: this.stockMinimo(),
      stockMaximo: this.stockMaximo(),
      lote: this.lote(),
      fechaVencimiento: this.fechaVencimiento() || null,
      bomLines: this.bomLines().filter(l => l.cantidad > 0)
    };

    try {
      if (this.isEditing() && this.currentItem()) {
        await this.catalogService.updateItem(this.currentItem()!.id, payload as any).toPromise();
      } else {
        await this.catalogService.createItem(payload as any).toPromise();
      }
      this.saved.emit();
      this.close();
    } catch (error) {
      console.error('Error saving medicamento:', error);
    } finally {
      this.isSaving.set(false);
    }
  }
}