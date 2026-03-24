import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CatalogService, CatalogItem } from '../../../core/services/catalog.service';
import { ActivatedRoute } from '@angular/router';
import { 
    LucideAngularModule, 
    Package, 
    Search, 
    Plus, 
    Edit, 
    Trash2,
    Database,
    Stethoscope,
    Microscope,
    Scan,
    X
} from 'lucide-angular';

@Component({
  selector: 'app-catalog-management',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule],
  templateUrl: './catalog-management.component.html'
})
export class CatalogManagementComponent implements OnInit {
  private catalogService = inject(CatalogService);
  private route = inject(ActivatedRoute);

  public catalog = signal<CatalogItem[]>([]);
  public filteredCatalog = signal<CatalogItem[]>([]);
  public isLoading = signal<boolean>(false);
  public typeFilter = signal<string | null>(null);
  
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

  readonly icons = {
    Package,
    Search,
    Plus,
    Edit,
    Trash2,
    Database,
    Stethoscope,
    Microscope,
    Scan,
    X
  };

  ngOnInit() {
    this.route.queryParams.subscribe((params: any) => {
      this.typeFilter.set(params['type'] || null);
      this.refreshCatalog();
    });
  }

  refreshCatalog() {
    this.isLoading.set(true);
    this.catalogService.getUnifiedCatalog().subscribe({
      next: (res: CatalogItem[]) => {
        this.catalog.set(res);
        this.applyFilter();
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  applyFilter() {
    const filter = this.typeFilter();
    if (filter) {
      this.filteredCatalog.set(this.catalog().filter((i: CatalogItem) => i.tipo?.toUpperCase() === filter.toUpperCase()));
    } else {
      this.filteredCatalog.set(this.catalog());
    }
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

  delete(id: string) {
    if (confirm('¿Estás seguro de eliminar este servicio?')) {
      this.catalogService.deleteItem(id).subscribe(() => this.refreshCatalog());
    }
  }

  getTipoColorPremium(tipo: string): string {
    switch (tipo?.toUpperCase()) {
      case 'CONSULTA': return 'bg-rose-500/10 text-rose-500 border-rose-500/20';
      case 'LABORATORIO': return 'bg-emerald-500/10 text-emerald-400 border-emerald-500/20';
      case 'RX': return 'bg-rose-500/10 text-rose-500 border-rose-500/20';
      default: return 'bg-slate-500/10 text-slate-400 border-slate-500/20';
    }
  }

  getTipoColor(tipo: string): string {
    return this.getTipoColorPremium(tipo);
  }
}
