import { Component, inject, signal, OnInit, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { InventoryService } from '../../../core/services/inventory.service';
import { MultiSedeService, Sede } from '../../../core/services/multi-sede.service';
import { 
  LucideAngularModule, 
  Package, 
  Search, 
  Printer, 
  RefreshCcw, 
  Building, 
  CheckCircle, 
  AlertTriangle, 
  XCircle, 
  Filter, 
  Layers
} from 'lucide-angular';

export interface StockSedeItem {
  insumoId: string;
  codigo: string;
  nombre: string;
  unidadMedidaBase: string;
  stockActual: number;
  stockMinimo?: number;
  stockMaximo?: number;
  reactivosCombinados?: string;
  sedeNombre?: string;
}

@Component({
  selector: 'app-stock-multisede',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule],
  templateUrl: './stock-multisede.component.html'
})
export class StockMultisedeComponent implements OnInit {
  private inventoryService = inject(InventoryService);
  private multiSedeService = inject(MultiSedeService);

  public activeTab = signal<'stock' | 'kardex'>('stock');

  public sedes = signal<Sede[]>([]);
  public insumosCatalog = signal<any[]>([]);
  public selectedSedeId = signal<string>('TODAS');
  public stockItems = signal<StockSedeItem[]>([]);
  public isLoading = signal<boolean>(false);
  public searchTerm = signal<string>('');

  // Signals para Kárdex con Rango de Fechas
  public kardexSedeId = signal<string>('TODAS');
  public kardexInsumoId = signal<string>('TODOS');
  public kardexFechaDesde = signal<string>(new Date().toISOString().split('T')[0]);
  public kardexFechaHasta = signal<string>(new Date().toISOString().split('T')[0]);
  public kardexResult = signal<any | null>(null);
  public isKardexLoading = signal<boolean>(false);

  readonly icons = {
    Package,
    Search,
    Printer,
    RefreshCcw,
    Building,
    CheckCircle,
    AlertTriangle,
    XCircle,
    Filter,
    Layers
  };

  public selectedSedeObj = computed(() => {
    const id = this.selectedSedeId();
    if (id === 'TODAS') return null;
    return this.sedes().find(s => s.id === id) || null;
  });

  public filteredStock = computed(() => {
    const term = this.searchTerm().trim().toLowerCase();
    const items = this.stockItems();
    if (!term) return items;
    return items.filter(i => 
      (i.nombre && i.nombre.toLowerCase().includes(term)) ||
      (i.codigo && i.codigo.toLowerCase().includes(term))
    );
  });

  public totalInsumos = computed(() => this.filteredStock().length);
  public insumosDisponibles = computed(() => this.filteredStock().filter(i => i.stockActual > (i.stockMinimo || 5)).length);
  public insumosStockBajo = computed(() => this.filteredStock().filter(i => i.stockActual > 0 && i.stockActual <= (i.stockMinimo || 5)).length);
  public insumosAgotados = computed(() => this.filteredStock().filter(i => i.stockActual <= 0).length);

  ngOnInit(): void {
    this.cargarSedes();
    this.cargarInsumosCatalog();
  }

  public cargarInsumosCatalog(): void {
    this.inventoryService.getInsumos(true).subscribe({
      next: (insumos) => this.insumosCatalog.set(insumos),
      error: (err) => console.error('Error al cargar insumos:', err)
    });
  }

  public cargarSedes(): void {
    this.isLoading.set(true);
    this.multiSedeService.getSedes().subscribe({
      next: (sedes) => {
        const activas = sedes.filter(s => s.activo);
        this.sedes.set(activas);
        this.cargarStock();
      },
      error: () => this.isLoading.set(false)
    });
  }

  public onSedeChange(sedeId: string): void {
    this.selectedSedeId.set(sedeId);
    this.cargarStock();
  }

  public cargarStock(): void {
    this.isLoading.set(true);
    const targetId = this.selectedSedeId();

    if (targetId === 'TODAS') {
      this.inventoryService.getStockConsolidado().subscribe({
        next: (items) => {
          const mapped: StockSedeItem[] = items.map(i => ({
            insumoId: i.insumoId || i.id,
            codigo: i.codigo,
            nombre: i.nombre,
            unidadMedidaBase: i.unidadMedidaBase,
            stockActual: i.stockActual,
            stockMinimo: i.stockMinimo || 5,
            sedeNombre: 'Consolidado Global'
          }));
          this.stockItems.set(mapped);
          this.isLoading.set(false);
        },
        error: () => this.isLoading.set(false)
      });
    } else {
      this.inventoryService.getStockSedeById(targetId).subscribe({
        next: (data) => {
          const sede = this.sedes().find(s => s.id === targetId);
          const mapped = (data || []).map(d => ({
            ...d,
            sedeNombre: sede?.nombre || 'Sede'
          }));
          this.stockItems.set(mapped);
          this.isLoading.set(false);
        },
        error: () => this.isLoading.set(false)
      });
    }
  }

  public cargarKardex(): void {
    this.isKardexLoading.set(true);
    this.inventoryService.getKardex(
      this.kardexSedeId(),
      this.kardexInsumoId(),
      this.kardexFechaDesde(),
      this.kardexFechaHasta()
    ).subscribe({
      next: (res: any) => {
        this.kardexResult.set(res);
        this.isKardexLoading.set(false);
      },
      error: (err: any) => {
        console.error('Error al cargar Kárdex:', err);
        this.isKardexLoading.set(false);
      }
    });
  }

  public onTabChange(tab: 'stock' | 'kardex'): void {
    this.activeTab.set(tab);
    if (tab === 'kardex' && !this.kardexResult()) {
      this.cargarKardex();
    }
  }

  public imprimirReporte(): void {
    window.print();
  }
}

