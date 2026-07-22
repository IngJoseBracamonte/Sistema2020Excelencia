import { Component, signal, computed, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
  LucideAngularModule,
  X, Save, Loader2, Search, Trash2, Check, Package,
  Scissors, FileText, UserCog, Activity, Plus,
  DollarSign, MessageSquare, Layers
} from 'lucide-angular';
import { BaseCatalogEditComponent } from './base-catalog-edit.component';
import { CatalogItem } from '../../../../core/services/catalog.service';
import { Insumo } from '../../../../core/models/inventory.model';

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
  templateUrl: './edit-cirugia.component.html'
})
export class EditCirugiaComponent extends BaseCatalogEditComponent implements OnInit {
  // ── Icons ───────────────────────────────────────────────────────────────
  protected readonly icons = {
    X, Save, Loader2, Search, Trash2, Check, Package,
    Scissors, FileText, UserCog, Activity, Plus,
    DollarSign, MessageSquare, Layers
  } as const;

  // ── Cirugía Specific Signals ─────────────────────────────────────────────
  public readonly complejidad = signal<string>('MEDIA');
  public readonly duracionEstimadaMinutos = signal<number>(120);
  public readonly requiereAnestesia = signal<boolean>(true);
  public readonly tipoAnestesia = signal<string>('GENERAL');
  public readonly clasificacionRiesgo = signal<string>('II');
  public readonly notasPreoperatorias = signal<string>('');
  public readonly notasPostoperatorias = signal<string>('');
  public readonly protocoloQuirurgico = signal<string>('');
  public readonly indicaciones = signal<string>('');
  public readonly contraindicaciones = signal<string>('');

  // Equipo Quirúrgico
  public readonly equipoQuirurgico = signal<EquipoQuirurgico[]>([]);
  public readonly equipoSearchQuery = signal<string>('');
  public readonly showEquipoDropdown = signal<boolean>(false);
  public readonly honorariosEquipo = signal<Array<{ rol: string; honorarioUsd: number }>>([]);
  public readonly allEquipos = signal<CatalogItem[]>([]);

  // Handlers & Compatibility Signals
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

  public readonly filteredEquipos = computed(() => {
    const q = this.equipoSearchQuery().toLowerCase().trim();
    const selected = new Set(this.equipoQuirurgico().map(e => e.id));
    return this.allEquipos()
      .filter(e => !selected.has(e.id))
      .filter(e => !q || e.descripcion.toLowerCase().includes(q) || e.codigo.toLowerCase().includes(q))
      .slice(0, 20);
  });

  ngOnInit(): void {
    this.loadInsumos();
    this.loadCatalogForSugerencias('CIRUGIA');
    this.loadEquipos();
  }

  private loadEquipos(): void {
    this.catalogService.getItems().subscribe({
      next: (data) => this.allEquipos.set(data.filter(i => i.tipo === 'EQUIPO' || i.tipo === 'INSTRUMENTAL')),
      error: () => console.error('Error loading equipos')
    });
  }

  protected loadItem(id: string): void {
    this.catalogService.getItemById(id).subscribe({
      next: (item) => this.populateForm(item),
      error: () => console.error('Error loading cirugia item')
    });
  }

  protected resetForm(): void {
    this.resetBaseForm();
    this.complejidad.set('MEDIA');
    this.duracionEstimadaMinutos.set(120);
    this.requiereAnestesia.set(true);
    this.tipoAnestesia.set('GENERAL');
    this.clasificacionRiesgo.set('II');
    this.notasPreoperatorias.set('');
    this.notasPostoperatorias.set('');
    this.protocoloQuirurgico.set('');
    this.indicaciones.set('');
    this.contraindicaciones.set('');
    this.equipoQuirurgico.set([]);
    this.honorariosEquipo.set([]);
  }

  private populateForm(item: CatalogItem): void {
    this.nombre.set(item.descripcion || '');
    this.codigo.set(item.codigo || '');
    this.precioBaseUsd.set(item.precioBaseUsd || item.precioUsd || 0);
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

    // Load recipe
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

  // ── Equipo Quirúrgico Actions ───────────────────────────────────────────
  public addEquipo(item: CatalogItem): void {
    this.addEquipoToList(item);
  }

  public addEquipoToList(item: CatalogItem): void {
    const exists = this.equipoQuirurgico().some(e => e.id === item.id);
    if (exists) return;

    this.equipoQuirurgico.update(eq => [...eq, {
      id: item.id,
      nombre: item.descripcion,
      codigo: item.codigo,
      cantidad: 1
    }]);
    this.equipoSearchQuery.set('');
    this.showEquipoDropdown.set(false);
  }

  public updateEquipoCantidad(index: number, cantidad: number): void {
    this.equipoQuirurgico.update(eq => {
      const copy = [...eq];
      copy[index] = { ...copy[index], cantidad: Math.max(1, cantidad) };
      return copy;
    });
  }

  public removeEquipo(index: number): void {
    this.equipoQuirurgico.update(eq => eq.filter((_, i) => i !== index));
  }

  public onEquipoBlur(): void {
    setTimeout(() => this.showEquipoDropdown.set(false), 200);
  }

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

  public addHonorarioRol(): void {
    this.honorariosEquipo.update(list => [...list, { rol: '', honorarioUsd: 0 }]);
  }

  public updateHonorarioRol(index: number, field: 'rol' | 'honorarioUsd', value: any): void {
    this.honorariosEquipo.update(list => {
      const copy = [...list];
      copy[index] = { ...copy[index], [field]: value };
      return copy;
    });
  }

  public removeHonorarioRol(index: number): void {
    this.honorariosEquipo.update(list => list.filter((_, i) => i !== index));
  }

  public save(): void {
    if (!this.nombre() || !this.codigo() || this.precioBaseUsd() <= 0) return;
    this.isSaving.set(true);

    const itemData: any = {
      descripcion: this.nombre(),
      codigo: this.codigo(),
      precioBaseUsd: this.precioBaseUsd(),
      precioUsd: this.precioBaseUsd(),
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
      sugerenciasIds: this.selectedSugerenciasIds(),
      requiereInventario: this.bomLines().length > 0
    };

    if (this.isEditing() && this.itemId()) {
      this.catalogService.updateItem(this.itemId()!, itemData as CatalogItem).subscribe({
        next: () => this.saveRecipeAndFinish(this.itemId()!),
        error: () => { this.isSaving.set(false); console.error('Error updating cirugia'); }
      });
    } else {
      this.catalogService.createItem(itemData).subscribe({
        next: (newId: string) => this.saveRecipeAndFinish(newId),
        error: () => { this.isSaving.set(false); console.error('Error creating cirugia'); }
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