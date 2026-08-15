import { Component, signal, computed, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
  LucideAngularModule, Package, Search, Plus, Trash2, X, Check,
  Stethoscope, Save, Loader2, Layers, FileText, UserCog, AlertCircle, FilePlus
} from 'lucide-angular';
import { BaseCatalogEditComponent } from './base-catalog-edit.component';
import { CatalogItem } from '../../../../core/services/catalog.service';
import { Insumo } from '../../../../core/models/inventory.model';
import { BOMLine, MedicoOption } from '../models/catalog-edit.models';

@Component({
  selector: 'app-edit-servicio',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule],
  templateUrl: './edit-servicio.component.html'
})
export class EditServicioComponent extends BaseCatalogEditComponent implements OnInit {
  protected readonly icons = {
    Package, Search, Plus, Trash2, X, Check,
    Stethoscope, Save, Loader2, Layers, FileText, UserCog, AlertCircle, FilePlus
  } as const;

  // Nuevas propiedades para Tipo de Servicio, Informes y Validaciones
  public readonly tipoServicioId = signal<number>(1);
  public readonly servicioInformeId = signal<string | null>(null);
  public readonly availableInformes = signal<CatalogItem[]>([]);

  public readonly esServicioInforme = computed(() => this.tipoServicioId() === 6);
  public readonly isImagingService = computed(() => this.tipoServicioId() === 3 || this.tipoServicioId() === 4);
  
  // Validaciones reactivas en tiempo real (Invariante: PrecioBase >= HonorarioBase para Informes)
  public readonly isPriceLessThanHonorarium = computed(() => {
    return this.esServicioInforme() && (this.precioBaseUsd() < this.honorarioBase());
  });

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
    this.loadCatalogForSugerencias('SERVICIO');
    this.loadInformesCatalog();
  }

  private loadInformesCatalog(): void {
    this.catalogService.getUnifiedCatalog().subscribe({
      next: (items) => {
        const informes = items.filter(i => i.esServicioInforme || i.tipoServicioId === 6 || (i.tipo && i.tipo.toUpperCase() === 'INFORME'));
        this.availableInformes.set(informes);
      },
      error: (err) => console.error('Error loading informes catalog:', err)
    });
  }

  protected loadItem(id: string): void {
    this.catalogService.getItemById(id).subscribe({
      next: (item) => this.populateForm(item),
      error: () => console.error('Error loading servicio item')
    });
  }

  protected resetForm(): void {
    this.resetBaseForm();
    this.tipoServicioId.set(1);
    this.servicioInformeId.set(null);
  }

  private populateForm(item: CatalogItem): void {
    this.nombre.set(item.descripcion || '');
    this.codigo.set(item.codigo || '');
    this.precioBaseUsd.set(item.precioUsd ?? 0);
    this.honorarioBase.set(item.honorarioBase || 0);
    this.activo.set(item.activo ?? true);
    this.tipoServicioId.set(item.tipoServicioId || (item.esServicioInforme ? 6 : 1));
    this.servicioInformeId.set(item.servicioInformeId || null);

    if (item.honorariosMedicos?.length) {
      this.honorariosMedicos.set(item.honorariosMedicos.map((h: any) => ({
        medicoId: h.medicoId,
        medicoNombre: h.medicoNombre || 'Médico',
        honorarioUsd: h.honorario
      })));
    }

    this.inventoryService.getRecetas().subscribe({
      next: (recetas: any[]) => {
        const safeRecetas = Array.isArray(recetas) ? recetas : [];
        const itemRecetas = safeRecetas.filter(r => r.servicioClinicoId === item.id);
        this.bomLines.set(itemRecetas.map((r: any) => ({
          insumoId: r.insumoId,
          insumoNombre: r.insumoNombre || (r.insumo ? r.insumo.nombre : ''),
          insumoCodigo: r.insumoCodigo || (r.insumo ? r.insumo.codigo : ''),
          cantidad: r.cantidad,
          unidadMedida: r.unidadMedidaConsumo
        })));
      },
      error: () => this.bomLines.set([])
    });

    if (item.sugerenciasIds?.length) {
      this.selectedSugerenciasIds.set(item.sugerenciasIds);
    }
  }

  public crearInformeRapido(): void {
    const baseName = this.nombre().trim();
    if (!baseName) {
      alert('Por favor ingrese primero el Nombre del Estudio Base.');
      return;
    }

    const reportName = `Informe de ${baseName}`;
    const reportCode = `INF-${this.codigo().trim() || 'NEW'}`;
    const reportPrice = Math.round((this.precioBaseUsd() * 0.4) * 100) / 100 || 15.00;
    const reportFee = Math.round((reportPrice * 0.7) * 100) / 100 || 10.00;

    const reportData: any = {
      descripcion: reportName,
      codigo: reportCode,
      precioUsd: reportPrice,
      honorarioBase: reportFee,
      tipo: 'INFORME',
      tipoServicioId: 6,
      esServicioInforme: true,
      activo: true
    };

    this.catalogService.createItem(reportData).subscribe({
      next: (newReportId: string) => {
        alert(`Informe '${reportName}' creado y vinculado con éxito.`);
        this.loadInformesCatalog();
        this.servicioInformeId.set(newReportId);
      },
      error: (err) => alert('Error al crear el informe rápido: ' + (err.error?.message || err.message))
    });
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
    if (this.isPriceLessThanHonorarium()) {
      alert(`El precio base del informe ($${this.precioBaseUsd()}) no puede ser inferior al honorario del médico ($${this.honorarioBase()}).`);
      return;
    }

    this.isSaving.set(true);

    const mappedTipo = this.tipoServicioId() === 6 ? 'INFORME' : 
                       (this.tipoServicioId() === 3 ? 'RX' : 
                        (this.tipoServicioId() === 4 ? 'TOMOGRAFIA' : 
                         (this.tipoServicioId() === 2 ? 'LABORATORIO' : 'SERVICIO')));

    const itemData: any = {
      descripcion: this.nombre(),
      codigo: this.codigo(),
      precioUsd: this.precioBaseUsd(),
      honorarioBase: this.honorarioBase(),
      tipo: mappedTipo,
      tipoServicioId: this.tipoServicioId(),
      esServicioInforme: this.esServicioInforme(),
      servicioInformeId: this.servicioInformeId(),
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
        error: () => { this.isSaving.set(false); console.error('Error updating servicio'); }
      });
    } else {
      this.catalogService.createItem(itemData).subscribe({
        next: (newId: string) => this.saveRecipeAndFinish(newId),
        error: () => { this.isSaving.set(false); console.error('Error creating servicio'); }
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