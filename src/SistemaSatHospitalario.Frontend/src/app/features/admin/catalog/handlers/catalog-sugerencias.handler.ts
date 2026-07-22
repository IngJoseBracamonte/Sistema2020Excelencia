import { signal, computed } from '@angular/core';
import { CatalogItem } from '../../../../core/models/priced-item.model';

export class CatalogSugerenciasHandler {
  public readonly allCatalogItems = signal<CatalogItem[]>([]);
  public readonly sugerenciasSearchQuery = signal<string>('');
  public readonly sugerenciasIds = signal<string[]>([]);

  public readonly filteredSugerencias = computed(() => {
    const query = this.sugerenciasSearchQuery().toLowerCase().trim();
    const selectedIds = new Set(this.sugerenciasIds());
    const items = this.allCatalogItems().filter(item => !selectedIds.has(item.id));

    if (!query) {
      return items.slice(0, 30);
    }
    return items
      .filter(item => item.descripcion.toLowerCase().includes(query) || item.codigo.toLowerCase().includes(query))
      .slice(0, 30);
  });

  public readonly selectedSugerenciasCards = computed(() => {
    const selectedIds = new Set(this.sugerenciasIds());
    return this.allCatalogItems().filter(item => selectedIds.has(item.id));
  });

  public addSugerencia(id: string): void {
    const current = this.sugerenciasIds();
    if (!current.includes(id)) {
      this.sugerenciasIds.set([...current, id]);
    }
  }

  public removeSugerencia(id: string): void {
    this.sugerenciasIds.set(this.sugerenciasIds().filter(sId => sId !== id));
  }

  public toggleSugerencia(id: string): void {
    if (this.sugerenciasIds().includes(id)) {
      this.removeSugerencia(id);
    } else {
      this.addSugerencia(id);
    }
  }

  public reset(): void {
    this.sugerenciasIds.set([]);
    this.sugerenciasSearchQuery.set('');
  }
}
