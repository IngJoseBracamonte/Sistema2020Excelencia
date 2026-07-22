import { signal, computed } from '@angular/core';
import { Insumo } from '../../../../core/models/inventory.model';
import { BOMLine } from '../models/catalog-edit.models';

export class CatalogBOMHandler {
  public readonly availableInsumos = signal<Insumo[]>([]);
  public readonly insumoSearchQuery = signal<string>('');
  public readonly showInsumoDropdown = signal<boolean>(false);
  public readonly bomLines = signal<BOMLine[]>([]);

  public readonly filteredInsumos = computed(() => {
    const query = this.insumoSearchQuery().toLowerCase().trim();
    const items = this.availableInsumos();
    if (!query) {
      return items.slice(0, 20);
    }
    return items
      .filter(item => item.nombre.toLowerCase().includes(query) || item.codigo.toLowerCase().includes(query))
      .slice(0, 20);
  });

  public addInsumo(insumo: Insumo, cantidadDefault: number = 1): void {
    const current = this.bomLines();
    const existing = current.find(l => l.insumoId === insumo.id);
    if (existing) {
      this.updateCantidad(insumo.id, existing.cantidad + cantidadDefault);
    } else {
      this.bomLines.set([
        ...current,
        {
          insumoId: insumo.id,
          insumoCodigo: insumo.codigo,
          insumoNombre: insumo.nombre,
          cantidad: cantidadDefault,
          unidadMedida: (insumo as any).unidadMedida || insumo.unidadMedidaBase || 'UND'
        }
      ]);
    }
    this.insumoSearchQuery.set('');
    this.showInsumoDropdown.set(false);
  }

  public removeLine(insumoId: string): void {
    this.bomLines.set(this.bomLines().filter(l => l.insumoId !== insumoId));
  }

  public updateCantidad(insumoId: string, nuevaCantidad: number): void {
    if (nuevaCantidad <= 0) {
      this.removeLine(insumoId);
      return;
    }
    this.bomLines.set(
      this.bomLines().map(l => (l.insumoId === insumoId ? { ...l, cantidad: nuevaCantidad } : l))
    );
  }

  public onInsumoBlur(): void {
    setTimeout(() => this.showInsumoDropdown.set(false), 200);
  }

  public reset(): void {
    this.bomLines.set([]);
    this.insumoSearchQuery.set('');
    this.showInsumoDropdown.set(false);
  }
}
