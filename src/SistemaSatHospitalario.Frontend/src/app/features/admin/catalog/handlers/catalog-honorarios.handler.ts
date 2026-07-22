import { signal, computed } from '@angular/core';
import { HonorarioMedico, MedicoOption } from '../models/catalog-edit.models';

export class CatalogHonorariosHandler {
  public readonly availableMedicos = signal<MedicoOption[]>([]);
  public readonly medicoSearchQuery = signal<string>('');
  public readonly showMedicoDropdown = signal<boolean>(false);
  public readonly honorarios = signal<HonorarioMedico[]>([]);

  public readonly filteredMedicos = computed(() => {
    const query = this.medicoSearchQuery().toLowerCase().trim();
    const items = this.availableMedicos();
    const selectedIds = new Set(this.honorarios().map(h => h.medicoId));

    const unselected = items.filter(m => !selectedIds.has(m.id));
    if (!query) {
      return unselected.slice(0, 20);
    }
    return unselected
      .filter(m => m.nombre.toLowerCase().includes(query) || m.especialidad.toLowerCase().includes(query))
      .slice(0, 20);
  });

  public addHonorario(medico: MedicoOption, honorarioUsd: number = 0): void {
    const current = this.honorarios();
    if (current.some(h => h.medicoId === medico.id)) {
      return;
    }
    this.honorarios.set([
      ...current,
      {
        medicoId: medico.id,
        medicoNombre: medico.nombre,
        honorarioUsd
      }
    ]);
    this.medicoSearchQuery.set('');
    this.showMedicoDropdown.set(false);
  }

  public removeHonorario(medicoId: string): void {
    this.honorarios.set(this.honorarios().filter(h => h.medicoId !== medicoId));
  }

  public updateHonorarioUsd(medicoId: string, honorarioUsd: number): void {
    this.honorarios.set(
      this.honorarios().map(h => (h.medicoId === medicoId ? { ...h, honorarioUsd: Math.max(0, honorarioUsd) } : h))
    );
  }

  public onMedicoBlur(): void {
    setTimeout(() => this.showMedicoDropdown.set(false), 200);
  }

  public reset(): void {
    this.honorarios.set([]);
    this.medicoSearchQuery.set('');
    this.showMedicoDropdown.set(false);
  }
}
