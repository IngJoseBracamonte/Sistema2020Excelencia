import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CatalogService, CatalogItem } from '../../../core/services/catalog.service';

@Component({
  selector: 'app-catalog-management',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './catalog-management.component.html'
})
export class CatalogManagementComponent implements OnInit {
  private catalogService = inject(CatalogService);

  public catalog = signal<CatalogItem[]>([]);
  public isLoading = signal<boolean>(false);
  
  // Modal State
  public showModal = signal<boolean>(false);
  public isEditing = signal<boolean>(false);
  public currentItem = signal<Partial<CatalogItem>>({
    codigo: '',
    descripcion: '',
    precio: 0,
    tipo: 'CONSULTA',
    activo: true
  });

  public tipos = ['CONSULTA', 'LABORATORIO', 'RX', 'PROCEDIMIENTO', 'MEDICINA', 'SERVICIO'];

  ngOnInit() {
    this.refreshCatalog();
  }

  refreshCatalog() {
    this.isLoading.set(true);
    this.catalogService.getUnifiedCatalog().subscribe({
      next: (res) => {
        this.catalog.set(res);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  openCreate() {
    this.isEditing.set(false);
    this.currentItem.set({
      codigo: '',
      descripcion: '',
      precio: 0,
      tipo: 'CONSULTA',
      activo: true
    });
    this.showModal.set(true);
  }

  openEdit(item: CatalogItem) {
    this.isEditing.set(true);
    this.currentItem.set({ ...item });
    this.showModal.set(true);
  }

  save() {
    const item = this.currentItem();
    if (this.isEditing() && item.id) {
      this.catalogService.updateItem(item as CatalogItem).subscribe(() => {
        this.showModal.set(false);
        this.refreshCatalog();
      });
    } else {
      this.catalogService.createItem(item).subscribe(() => {
        this.showModal.set(false);
        this.refreshCatalog();
      });
    }
  }

  getTipoColor(tipo: string): string {
    switch (tipo?.toUpperCase()) {
      case 'CONSULTA': return 'bg-blue-100 text-blue-700 border-blue-200';
      case 'LABORATORIO': return 'bg-emerald-100 text-emerald-700 border-emerald-200';
      case 'RX': return 'bg-rose-100 text-rose-700 border-rose-200';
      default: return 'bg-slate-100 text-slate-700 border-slate-200';
    }
  }
}
