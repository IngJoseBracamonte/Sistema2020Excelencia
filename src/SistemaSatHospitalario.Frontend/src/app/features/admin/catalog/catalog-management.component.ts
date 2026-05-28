import { Component, inject, signal, OnInit, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CatalogService, CatalogItem } from '../../../core/services/catalog.service';
import { BillingFacadeService } from '../../../core/services/billing-facade.service';
import { ActivatedRoute } from '@angular/router';
import { MedicoService, Medico } from '../../../core/services/medico.service';
import { 
    LucideAngularModule, 
    Package, 
    Search, 
    Plus, 
    Edit, 
    Trash2,
    Database,
    Stethoscope,
    Scan,
    X,
    Check,
    Clock
} from 'lucide-angular';

@Component({
  selector: 'app-catalog-management',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule],
  templateUrl: './catalog-management.component.html'
})
export class CatalogManagementComponent implements OnInit {
  private catalogService = inject(CatalogService);
  private billingFacade = inject(BillingFacadeService);
  private route = inject(ActivatedRoute);
  private medicoService = inject(MedicoService);

  public tasaCambioDia = this.billingFacade.tasaCambioDia;

  public catalog = signal<CatalogItem[]>([]);
  public filteredCatalog = signal<CatalogItem[]>([]);
  public isLoading = signal<boolean>(false);
  public typeFilter = signal<string | null>(null);
  // Tab State
  public activeTab = signal<'general' | 'sugerencias'>('general');

  // Modal State
  public showModal = signal<boolean>(false);
  public isEditing = signal<boolean>(false);
  public searchQuery = signal<string>('');
  
  // Médicos y honorarios específicos
  public medicos = signal<Medico[]>([]);
  public doctorSearchQuery = signal<string>('');
  public honorariosLocales = signal<Record<string, number>>({});

  public filteredDoctors = computed(() => {
    const query = this.doctorSearchQuery().toLowerCase().trim();
    const list = this.medicos().filter(m => m.activo);
    if (query) {
      return list.filter(m => 
        m.nombre.toLowerCase().includes(query) || 
        (m.especialidad || '').toLowerCase().includes(query)
      );
    }
    return list;
  });

  public activeDoctorHonorariosCount = computed(() => {
    return Object.values(this.honorariosLocales()).filter(val => val > 0).length;
  });

  public getDoctorHonorario(medicoId: string | undefined): number {
    if (!medicoId) return 0;
    return this.honorariosLocales()[medicoId] || 0;
  }

  public setDoctorHonorario(medicoId: string | undefined, value: number) {
    if (!medicoId) return;
    this.honorariosLocales.update(prev => ({
      ...prev,
      [medicoId]: isNaN(value) ? 0 : value
    }));
  }

  public currentItem = signal<Partial<CatalogItem>>({
    codigo: '',
    descripcion: '',
    precioUsd: 0,
    honorarioBase: 0,
    tipo: 'CONSULTA',
    activo: true,
    sugerenciasIds: []
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
    Scan,
    X,
    Check,
    Clock
  };

  ngOnInit() {
    this.route.queryParams.subscribe((params: any) => {
      this.typeFilter.set(params['type'] || null);
      if (params['tab'] === 'sugerencias' || params['tab'] === 'general') {
        this.activeTab.set(params['tab']);
      }
      this.refreshCatalog();
      this.loadMedicos();
    });
  }

  loadMedicos() {
    this.medicoService.getAll().subscribe({
      next: (res) => this.medicos.set(res),
      error: () => console.error("Error al cargar médicos")
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
    let filtered = this.catalog();
    const type = this.typeFilter();
    const query = this.searchQuery().toLowerCase();
    const tab = this.activeTab();

    // Filtro por Tab (Desactivado para mostrar todo)
    /* if (tab === 'sugerencias') {
      filtered = filtered.filter(i => i.sugerenciasIds && i.sugerenciasIds.length > 0);
    } */

    // Filtro por Tipo
    if (type) {
      filtered = filtered.filter(i => i.tipo?.toUpperCase() === type.toUpperCase());
    }

    // Filtro por Búsqueda
    if (query) {
      filtered = filtered.filter(i => 
        i.descripcion?.toLowerCase().includes(query) || 
        i.codigo?.toLowerCase().includes(query)
      );
    }

    this.filteredCatalog.set(filtered);
  }

  onTabChange(tab: 'general' | 'sugerencias') {
    this.activeTab.set(tab);
    this.applyFilter();
  }

  toggleSugerencia(id: string) {
    const item = this.currentItem();
    const currentSugerencias = [...(item.sugerenciasIds || [])];
    const index = currentSugerencias.indexOf(id);

    if (index > -1) {
      currentSugerencias.splice(index, 1);
    } else {
      currentSugerencias.push(id);
    }

    this.currentItem.set({ ...item, sugerenciasIds: currentSugerencias });
  }

  isSugerenciaSelected(id: string): boolean {
    return (this.currentItem().sugerenciasIds || []).includes(id);
  }

  openCreate() {
    this.isEditing.set(false);
    this.currentItem.set({
      codigo: '',
      descripcion: '',
      precioUsd: 0,
      honorarioBase: 0,
      tipo: 'CONSULTA',
      activo: true,
      sugerenciasIds: []
    });
    this.honorariosLocales.set({});
    this.doctorSearchQuery.set('');
    this.showModal.set(true);
  }

  openEdit(item: CatalogItem) {
    this.isEditing.set(true);
    this.currentItem.set({ ...item });
    const localHons: Record<string, number> = {};
    if (item.honorariosMedicos) {
      item.honorariosMedicos.forEach(h => {
        localHons[h.medicoId] = h.honorario;
      });
    }
    this.honorariosLocales.set(localHons);
    this.doctorSearchQuery.set('');
    this.showModal.set(true);
  }

  save() {
    const item = { ...this.currentItem() };
    
    // Si es consulta, forzamos honorarioBase a 0 para evitar conflictos (User Request V2.0)
    if (item.tipo === 'CONSULTA') {
      item.honorarioBase = 0;
    }

    // Mapear honorarios locales a formato DTO
    const localHons = this.honorariosLocales();
    item.honorariosMedicos = Object.entries(localHons)
      .filter(([_, value]) => value > 0)
      .map(([medicoId, honorario]) => ({
        medicoId,
        honorario
      }));

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

  itemToDelete = signal<any>(null);

  confirmDelete(item: any) {
    this.itemToDelete.set(item);
  }

  executeDelete() {
    const item = this.itemToDelete();
    if (!item || !item.id) return;
    
    this.catalogService.deleteItem(item.id).subscribe({
      next: (success) => {
        this.itemToDelete.set(null);
        this.refreshCatalog();
      },
      error: (err) => {
        console.error('Error HTTP:', err);
        this.itemToDelete.set(null);
      }
    });
  }

  cancelDelete() {
    this.itemToDelete.set(null);
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
