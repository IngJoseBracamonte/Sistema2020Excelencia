import { Component, inject, signal, OnInit, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { InventoryService } from '../../../core/services/inventory.service';
import { CuentasPorPagarService } from '../../../core/services/cuentas-por-pagar.service';
import { MultiSedeService } from '../../../core/services/multi-sede.service';
import { 
  LucideAngularModule, 
  History, 
  PackageCheck, 
  ShoppingCart, 
  Send, 
  Trash2, 
  CreditCard, 
  Search, 
  Calendar, 
  Filter, 
  RefreshCcw,
  Building,
  FileText
} from 'lucide-angular';

export type HistorialTab = 'ingresos' | 'compras' | 'pedidos' | 'envios' | 'descartes' | 'cxp';

@Component({
  selector: 'app-historiales',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule],
  templateUrl: './historiales.component.html'
})
export class HistorialesComponent implements OnInit {
  private inventoryService = inject(InventoryService);
  private cxpService = inject(CuentasPorPagarService);
  private multiSedeService = inject(MultiSedeService);

  public activeTab = signal<HistorialTab>('ingresos');
  public isLoading = signal<boolean>(false);

  // --- Filtros Globales ---
  public fechaDesde = signal<string>('');
  public fechaHasta = signal<string>('');
  public searchQuery = signal<string>('');

  // --- Datasets ---
  public historialIngresos = signal<any[]>([]);
  public historialCompras = signal<any[]>([]);
  public historialPedidos = signal<any[]>([]);
  public historialEnvios = signal<any[]>([]);
  public historialDescartes = signal<any[]>([]);
  public historialCxp = signal<any[]>([]);

  readonly icons = {
    History,
    PackageCheck,
    ShoppingCart,
    Send,
    Trash2,
    CreditCard,
    Search,
    Calendar,
    Filter,
    RefreshCcw,
    Building,
    FileText
  };

  // --- Catálogo DB-Driven ---
  public tiposMovimientoCatalog = signal<any[]>([]);

  ngOnInit() {
    this.inventoryService.getTiposMovimiento().subscribe({
      next: (tipos) => this.tiposMovimientoCatalog.set(tipos || [])
    });
    this.loadCurrentTab();
  }

  public setTab(tab: HistorialTab) {
    this.activeTab.set(tab);
    this.loadCurrentTab();
  }

  public loadCurrentTab() {
    this.isLoading.set(true);
    const tab = this.activeTab();
    const fDesde = this.fechaDesde();
    const fHasta = this.fechaHasta();
    const q = this.searchQuery();

    switch (tab) {
      case 'ingresos':
        this.inventoryService.getHistorialMovimientos('Ingreso', fDesde, fHasta, q).subscribe({
          next: (data: any[]) => { this.historialIngresos.set(Array.isArray(data) ? data : []); this.isLoading.set(false); },
          error: (e: any) => { console.error(e); this.historialIngresos.set([]); this.isLoading.set(false); }
        });
        break;

      case 'compras':
        this.inventoryService.getHistorialMovimientos('Ingreso', fDesde, fHasta, q).subscribe({
          next: (data: any[]) => {
            const list = Array.isArray(data) ? data : [];
            const compras = list.filter((d: any) => d && ((d.motivo || '').toLowerCase().includes('compra') || d.tipoMovimiento === 'Ingreso'));
            this.historialCompras.set(compras.length > 0 ? compras : list);
            this.isLoading.set(false);
          },
          error: (e: any) => { console.error(e); this.historialCompras.set([]); this.isLoading.set(false); }
        });
        break;

      case 'pedidos':
        this.multiSedeService.getPedidosRecibidos().subscribe({
          next: (data: any[]) => {
            const list = Array.isArray(data) ? data : [];
            const pedidos = list.filter((p: any) => p && p.estado !== 'Pendiente');
            this.historialPedidos.set(pedidos);
            this.isLoading.set(false);
          },
          error: (e: any) => { console.error(e); this.historialPedidos.set([]); this.isLoading.set(false); }
        });
        break;

      case 'envios':
        this.inventoryService.getHistorialMovimientos('EnvioSubArea', fDesde, fHasta, q).subscribe({
          next: (data: any[]) => { this.historialEnvios.set(Array.isArray(data) ? data : []); this.isLoading.set(false); },
          error: (e: any) => { console.error(e); this.historialEnvios.set([]); this.isLoading.set(false); }
        });
        break;

      case 'descartes':
        this.inventoryService.getHistorialMovimientos('Descarte', fDesde, fHasta, q).subscribe({
          next: (data: any[]) => { this.historialDescartes.set(Array.isArray(data) ? data : []); this.isLoading.set(false); },
          error: (e: any) => { console.error(e); this.historialDescartes.set([]); this.isLoading.set(false); }
        });
        break;

      case 'cxp':
        this.cxpService.getFacturas().subscribe({
          next: (data: any[]) => { this.historialCxp.set(Array.isArray(data) ? data : []); this.isLoading.set(false); },
          error: (e: any) => { console.error(e); this.historialCxp.set([]); this.isLoading.set(false); }
        });
        break;
    }
  }

  public resetFilters() {
    this.fechaDesde.set('');
    this.fechaHasta.set('');
    this.searchQuery.set('');
    this.loadCurrentTab();
  }
}
