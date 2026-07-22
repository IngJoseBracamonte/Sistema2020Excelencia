import { Directive, input, model, output, signal, inject, effect } from '@angular/core';
import { CatalogService, CatalogItem } from '../../../../core/services/catalog.service';
import { InventoryService } from '../../../../core/services/inventory.service';
import { MedicoService } from '../../../../core/services/medico.service';
import { BillingFacadeService } from '../../../../core/services/billing-facade.service';
import { CatalogBOMHandler } from '../handlers/catalog-bom.handler';
import { CatalogHonorariosHandler } from '../handlers/catalog-honorarios.handler';
import { CatalogSugerenciasHandler } from '../handlers/catalog-sugerencias.handler';
import { getTipoColor } from '../models/catalog-edit.models';

@Directive()
export abstract class BaseCatalogEditComponent {
  // ── Inputs & Models ────────────────────────────────────────────────────────
  public readonly itemId = input<string | null>(null);
  public readonly isEditing = model<boolean>(false);

  // ── Outputs ─────────────────────────────────────────────────────────────
  public readonly saved = output<void>();
  public readonly closed = output<void>();

  // ── Core Services ───────────────────────────────────────────────────────
  protected readonly catalogService = inject(CatalogService);
  protected readonly inventoryService = inject(InventoryService);
  protected readonly medicoService = inject(MedicoService);
  protected readonly billingFacade = inject(BillingFacadeService);

  // ── Common Handlers ─────────────────────────────────────────────────────
  public readonly bomHandler = new CatalogBOMHandler();
  public readonly honorariosHandler = new CatalogHonorariosHandler();
  public readonly sugerenciasHandler = new CatalogSugerenciasHandler();

  // ── Base Form Signals ───────────────────────────────────────────────────
  public readonly isSaving = signal<boolean>(false);
  public readonly currentItem = signal<CatalogItem | null>(null);

  public readonly nombre = signal<string>('');
  public readonly codigo = signal<string>('');
  public readonly precioBaseUsd = signal<number>(0);
  public readonly honorarioBase = signal<number>(0);
  public readonly activo = signal<boolean>(true);

  constructor() {
    effect(() => {
      const id = this.itemId();
      if (id) {
        this.loadItem(id);
      } else {
        this.resetBaseForm();
      }
    });
  }

  protected abstract loadItem(id: string): void;
  protected abstract resetForm(): void;

  protected resetBaseForm(): void {
    this.nombre.set('');
    this.codigo.set('');
    this.precioBaseUsd.set(0);
    this.honorarioBase.set(0);
    this.activo.set(true);
    this.currentItem.set(null);
    this.bomHandler.reset();
    this.honorariosHandler.reset();
    this.sugerenciasHandler.reset();
  }

  protected loadInsumos(): void {
    this.inventoryService.getInsumos().subscribe({
      next: (insumos) => this.bomHandler.availableInsumos.set(insumos || []),
      error: (err) => console.error('Error loading insumos:', err)
    });
  }

  protected loadMedicos(): void {
    this.medicoService.getAll().subscribe({
      next: (medicos: any[]) => {
        const formatted = (medicos || []).map((m: any) => ({
          id: m.id,
          nombre: `${m.apellido || ''}, ${m.nombre || ''}`.trim(),
          especialidad: m.especialidad || ''
        }));
        this.honorariosHandler.availableMedicos.set(formatted);
      },
      error: (err) => console.error('Error loading medicos:', err)
    });
  }

  protected loadCatalogForSugerencias(tipoExcluido?: string): void {
    this.catalogService.getItems().subscribe({
      next: (items) => {
        const filtered = tipoExcluido ? items.filter(i => i.tipo !== tipoExcluido) : items;
        this.sugerenciasHandler.allCatalogItems.set(filtered || []);
      },
      error: (err) => console.error('Error loading catalog suggestions:', err)
    });
  }

  public getTipoColor(tipo?: string | null): string {
    return getTipoColor(tipo);
  }

  public onClose(): void {
    this.closed.emit();
  }
}
