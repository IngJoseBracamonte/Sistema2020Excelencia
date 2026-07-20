import { Component, signal, computed, inject, OnInit, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LucideAngularModule, Scissors, Pill, Search, X, Plus, Trash2, Save, Loader2, Package, DollarSign, AlertTriangle, Check, Layers, MessageSquare, FileText, UserCog, Activity, Scalpel } from 'lucide-angular';
import { CatalogService, CatalogItem } from '../../../../core/services/catalog.service';
import { InventoryService } from '../../../../core/services/inventory.service';
import { Insumo } from '../../../../core/models/inventory.model';
import { BillingFacadeService } from '../../../../core/services/billing-facade.service';
import { MedicoService } from '../../../../core/services/medico.service';

type ServiceCategory = 'CONSULTA' | 'PROCEDIMIENTO' | 'LABORATORIO' | 'TOMOGRAFIA' | 'CIRUGIA' | 'MEDICAMENTO' | 'OTRO';

interface HonorarioMedico {
  medicoId: string;
  medicoNombre: string;
  honorarioUsd: number;
}

interface BOMLine {
  insumoId: string;
  insumoCodigo: string;
  insumoNombre: string;
  cantidad: number;
  unidadMedida: string;
}

interface EquipoQuirurgico {
  id: string;
  codigo: string;
  nombre: string;
  cantidad: number;
}

interface SugerenciaVinculada {
  id: string;
  codigo: string;
  descripcion: string;
  tipo: ServiceCategory;
  precioUsd: number;
}

@Component({
  selector: 'app-edit-medicamento',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule],
  templateUrl: './edit-medicamento.component.html',
  styleUrls: ['./edit-medicamento.component.scss']
})
export class EditMedicamentoComponent implements OnInit {
  // Icons
  readonly icons = {
    Scissors, Pill, Search, X, Plus, Trash2, Save, Loader2,
    Package, DollarSign, AlertTriangle, Check, Layers, MessageSquare,
    FileText, UserCog, Activity, Scalpel
  };

  // Services
  private catalogService = inject(CatalogService);
  private inventoryService = inject(InventoryService);
  private billingFacade = inject(BillingFacadeService);
  private medicoService = inject(MedicoService);

  // Modal state
  showModal = signal(false);
  isEditing = signal(false);
  isSaving = signal(false);
  currentItem = signal<CatalogItem | null>(null);

  // Form fields - Basic
  nombre = signal('');
  codigo = signal('');
  precioBaseUsd = signal(0);
  honorarioBase = signal(0);
  activo = signal(true);

  // Form fields - Medicamento specific
  principioActivo = signal('');
  concentracion = signal('');
  formaFarmaceutica = signal('TABLETA');
  viaAdministracion = signal('ORAL');
  laboratorio = signal('');
  requiereReceta = signal(true);
  controlado = signal(false);
  stockMinimo = signal(0);
  stockMaximo = signal(0);
  lote = signal('');
  fechaVencimiento = signal('');

  // BOM (Bill of Materials) - for excipients/packaging
  bomLines = signal<BOMLine[]>([]);
  insumoSearchQuery = signal('');
  showInsumoDropdown = signal(false);
  availableInsumos = signal<Insumo[]>([]);

  // Honorarios Médicos
  honorariosMedicos = signal<HonorarioMedico[]>([]);
  medicoSearchQuery = signal('');
  showMedicoDropdown = signal(false);
  availableMedicos = signal<{ id: string; nombre: string; especialidad: string }[]>([]);

  // Sugerencias Vinculadas
  sugerenciasIds = signal<string[]>([]);
  sugerenciasSearchQuery = signal('');
  allCatalogItems = signal<CatalogItem[]>([]);

  // Computed
  filteredInsumos = computed(() => {
    const query = this.insumoSearchQuery().toLowerCase();
    if (!query) return this.availableInsumos().slice(0, 20);
    return this.availableInsumos().filter(i =>
      i.nombre.toLowerCase().includes(query) ||
      i.codigo.toLowerCase().includes(query)
    ).slice(0, 20);
  });

  filteredMedicos = computed(() => {
    const query = this.medicoSearchQuery().toLowerCase();
    if (!query) return this.availableMedicos().slice(0, 20);
    return this.availableMedicos().filter(m =>
      m.nombre.toLowerCase().includes(query) ||
      m.especialidad.toLowerCase().includes(query)
    ).slice(0, 20);
  });

  filteredSugerencias = computed(() => {
    const query = this.sugerenciasSearchQuery().toLowerCase();
    const excludedIds = new Set(this.sugerenciasIds());
    const currentId = this.currentItem()?.id;
    let items = this.allCatalogItems().filter(item =>
      item.id !== currentId && !excludedIds.has(item.id)
    );
    if (query) {
      items = items.filter(item =>
        item.descripcion.toLowerCase().includes(query) ||
        item.codigo.toLowerCase().includes(query)
      );
    }
    return items.slice(0, 30);
  });

  selectedSugerenciasCards = computed(() => {
    const selectedIds = new Set(this.sugerenciasIds());
    return this.allCatalogItems().filter(item => selectedIds.has(item.id));
  });

  // Formas farmacéuticas
  readonly formasFarmaceuticas = [
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

  // Vías de administración
  readonly viasAdministracion = [
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

  ngOnInit() {
    this.loadInsumos();
    this.loadMedicos();
    this.loadCatalogForSugerencias();
  }

  private loadInsumos() {
    this.inventoryService.getInsumos().subscribe({
      next: (insumos) => this.availableInsumos.set(insumos),
      error: (err) => console.error('Error loading insumos:', err)
    });
  }

  private loadMedicos() {
    this.medicoService.getMedicos().subscribe({
      next: (medicos) => this.availableMedicos.set(medicos.map(m => ({
        id: m.id,
        nombre: `${m.apellido}, ${m.nombre}`,
        especialidad: m.especialidad
      }))),
      error: (err) => console.error('Error loading medicos:', err)
    });
  }

  private loadCatalogForSugerencias() {
    this.catalogService.getCatalog().subscribe({
      next: (items) => this.allCatalogItems.set(items),
      error: (err) => console.error('Error loading catalog:', err)
    });
  }

  openCreate() {
    this.resetForm();
    this.isEditing.set(false);
    this.currentItem.set(null);
    this.showModal.set(true);
  }

  openEdit(item: CatalogItem) {
    this.resetForm();
    this.isEditing.set(true);
    this.currentItem.set(item);
    this.populateForm(item);
    this.showModal.set(true);
  }

  close() {
    this.showModal.set(false);
    this.resetForm();
  }

  private resetForm() {
    this.nombre.set('');
    this.codigo.set('');
    this.precioBaseUsd.set(0);
    this.honorarioBase.set(0);
    this.activo.set(true);
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
    this.bomLines.set([]);
    this.honorariosMedicos.set([]);
    this.sugerenciasIds.set([]);
    this.insumoSearchQuery.set('');
    this.medicoSearchQuery.set('');
    this.sugerenciasSearchQuery.set('');
    this.showInsumoDropdown.set(false);
    this.showMedicoDropdown.set(false);
  }

  private populateForm(item: CatalogItem) {
    this.nombre.set(item.descripcion || '');
    this.codigo.set(item.codigo || '');
    this.precioBaseUsd.set(item.precioUsd || 0);
    this.honorarioBase.set(item.honorarioBase || 0);
    this.activo.set(item.activo !== false);

    // Medicamento specific fields from extended properties
    this.principioActivo.set(item.principioActivo || '');
    this.concentracion.set(item.concentracion || '');
    this.formaFarmaceutica.set(item.formaFarmaceutica || 'TABLETA');
    this.viaAdministracion.set(item.viaAdministracion || 'ORAL');
    this.laboratorio.set(item.laboratorio || '');
    this.requiereReceta.set(item.requiereReceta !== false);
    this.controlado.set(item.controlado === true);
    this.stockMinimo.set(item.stockMinimo || 0);
    this.stockMaximo.set(item.stockMaximo || 0);
    this.lote.set(item.lote || '');
    this.fechaVencimiento.set(item.fechaVencimiento || '');

    // BOM lines
    if (item.bomLines && Array.isArray(item.bomLines)) {
      this.bomLines.set(item.bomLines.map((line: any) => ({
        insumoId: line.insumoId,
        insumoCodigo: line.insumoCodigo,
        insumoNombre: line.insumoNombre,
        cantidad: line.cantidad,
        unidadMedida: line.unidadMedida
      })));
    }

    // Honorarios médicos
    if (item.honorariosMedicos && Array.isArray(item.honorariosMedicos)) {
      this.honorariosMedicos.set(item.honorariosMedicos.map((h: any) => ({
        medicoId: h.medicoId,
        medicoNombre: h.medicoNombre,
        honorarioUsd: h.honorarioUsd
      })));
    }

    // Sugerencias
    if (item.sugerenciasIds && Array.isArray(item.sugerenciasIds)) {
      this.sugerenciasIds.set(item.sugerenciasIds);
    }
  }

  // BOM Methods
  addInsumoToBOM(insumo: Insumo) {
    const exists = this.bomLines().some(line => line.insumoId === insumo.id);
    if (exists) return;

    this.bomLines.update(lines => [...lines, {
      insumoId: insumo.id,
      insumoCodigo: insumo.codigo,
      insumoNombre: insumo.nombre,
      cantidad: 1,
      unidadMedida: insumo.unidadMedidaBase
    }]);
    this.insumoSearchQuery.set('');
    this.showInsumoDropdown.set(false);
  }

  updateBOMCantidad(index: number, cantidad: number) {
    this.bomLines.update(lines => {
      const newLines = [...lines];
      newLines[index] = { ...newLines[index], cantidad: Math.max(0, cantidad) };
      return newLines;
    });
  }

  removeBOMLine(index: number) {
    this.bomLines.update(lines => lines.filter((_, i) => i !== index));
  }

  // Honorarios Médicos Methods
  addHonorarioMedico() {
    this.honorariosMedicos.update(arr => [...arr, {
      medicoId: '',
      medicoNombre: '',
      honorarioUsd: 0
    }]);
  }

  updateHonorarioMedico(index: number, field: 'medicoId' | 'medicoNombre' | 'honorarioUsd', value: any) {
    this.honorariosMedicos.update(arr => {
      const newArr = [...arr];
      newArr[index] = { ...newArr[index], [field]: value };
      return newArr;
    });
  }

  removeHonorarioMedico(index: number) {
    this.honorariosMedicos.update(arr => arr.filter((_, i) => i !== index));
  }

  onMedicoSelect(medico: { id: string; nombre: string; especialidad: string }, index: number) {
    this.updateHonorarioMedico(index, 'medicoId', medico.id);
    this.updateHonorarioMedico(index, 'medicoNombre', medico.nombre);
    this.medicoSearchQuery.set('');
    this.showMedicoDropdown.set(false);
  }

  // Sugerencias Methods
  isSugerenciaSelected(id: string): boolean {
    return this.sugerenciasIds().includes(id);
  }

  toggleSugerencia(id: string) {
    this.sugerenciasIds.update(ids =>
      ids.includes(id) ? ids.filter(i => i !== id) : [...ids, id]
    );
  }

  removeSugerencia(id: string) {
    this.sugerenciasIds.update(ids => ids.filter(i => i !== id));
  }

  getTipoColor(tipo: ServiceCategory): string {
    const colors: Record<ServiceCategory, string> = {
      CONSULTA: 'bg-rose-500/20 text-rose-400 border-rose-500/30',
      PROCEDIMIENTO: 'bg-emerald-500/20 text-emerald-400 border-emerald-500/30',
      LABORATORIO: 'bg-blue-500/20 text-blue-400 border-blue-500/30',
      TOMOGRAFIA: 'bg-amber-500/20 text-amber-400 border-amber-500/30',
      CIRUGIA: 'bg-red-500/20 text-red-400 border-red-500/30',
      MEDICAMENTO: 'bg-violet-500/20 text-violet-400 border-violet-500/30',
      OTRO: 'bg-slate-500/20 text-slate-400 border-slate-500/30'
    };
    return colors[tipo] || colors.OTRO;
  }

  // Save
  async save() {
    if (!this.nombre() || !this.codigo() || this.precioBaseUsd() <= 0) return;
    if (!this.principioActivo() || !this.concentracion()) return;

    this.isSaving.set(true);

    const payload = {
      descripcion: this.nombre(),
      codigo: this.codigo(),
      precioUsd: this.precioBaseUsd(),
      tipo: 'MEDICAMENTO' as const,
      honorarioBase: this.honorarioBase(),
      requiereInventario: true, // Medicamentos siempre requieren inventario
      sugerenciasIds: this.sugerenciasIds(),
      honorariosMedicos: this.honorariosMedicos().filter(h => h.medicoId && h.honorarioUsd > 0),
      activo: this.activo(),
      // Medicamento specific fields
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
        await this.catalogService.updateItem(this.currentItem()!.id, payload).toPromise();
      } else {
        await this.catalogService.createItem(payload).toPromise();
      }
      this.close();
    } catch (error) {
      console.error('Error saving medicamento:', error);
    } finally {
      this.isSaving.set(false);
    }
  }
}