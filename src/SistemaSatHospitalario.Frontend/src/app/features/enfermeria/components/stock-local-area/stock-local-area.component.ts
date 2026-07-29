import { Component, Input, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LucideAngularModule, Search, Printer, RefreshCcw, PackageCheck, FileText, X, CheckCircle, UserCheck } from 'lucide-angular';
import { InventoryService } from '../../../../core/services/inventory.service';
import { MultiSedeService, AreaClinica } from '../../../../core/services/multi-sede.service';

export interface StockSedeItem {
  insumoId: string;
  codigo: string;
  nombre: string;
  unidadMedidaBase: string;
  stockActual: number;
  stockMinimo?: number;
  stockMaximo?: number;
  reactivosCombinados?: string;
}

@Component({
  selector: 'app-stock-local-area',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule],
  templateUrl: './stock-local-area.component.html',
  styleUrls: ['./stock-local-area.component.css']
})
export class StockLocalAreaComponent implements OnInit {
  @Input() set areaNombre(val: string) {
    this.areaNombreSignal.set(val || 'EMERGENCIA');
  }
  @Input() set sedeId(val: string) {
    if (val) {
      this.sedeIdSignal.set(val);
    }
  }

  private inventoryService = inject(InventoryService);
  private multiSedeService = inject(MultiSedeService);

  readonly icons = {
    Search,
    Printer,
    RefreshCcw,
    PackageCheck,
    FileText,
    X,
    CheckCircle,
    UserCheck
  };

  public areaNombreSignal = signal<string>('EMERGENCIA');
  public sedeIdSignal = signal<string>('');
  public stockItems = signal<StockSedeItem[]>([]);
  public isLoading = signal<boolean>(false);
  public searchTerm = signal<string>('');

  // Formulario Acta de Entrega de Turno
  public showActaModal = signal<boolean>(false);
  public usuarioEntregaNombre = signal<string>('');
  public usuarioEntregaCedula = signal<string>('');
  public usuarioRecibeNombre = signal<string>('');
  public usuarioRecibeCedula = signal<string>('');
  public fechaHoraActa = signal<string>('');

  public filteredStock = computed(() => {
    const term = this.searchTerm().trim().toLowerCase();
    const items = this.stockItems();
    if (!term) return items;
    return items.filter(i => 
      (i.nombre && i.nombre.toLowerCase().includes(term)) ||
      (i.codigo && i.codigo.toLowerCase().includes(term))
    );
  });

  ngOnInit(): void {
    this.cargarStock();
  }

  public cargarStock(): void {
    let targetSedeId = this.sedeIdSignal();

    if (!targetSedeId) {
      this.multiSedeService.getSedes().subscribe(sedes => {
        const principal = sedes.find(s => s.esPrincipal) || sedes[0];
        if (principal) {
          this.sedeIdSignal.set(principal.id);
          this.cargarStock();
        }
      });
      return;
    }

    this.isLoading.set(true);
    this.inventoryService.getStockSedeById(targetSedeId).subscribe({
      next: (data) => {
        this.stockItems.set(data || []);
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
      }
    });
  }

  public abrirModalActa(): void {
    const now = new Date();
    const fechaFormateada = now.toLocaleString('es-VE', { 
      day: '2-digit', 
      month: 'long', 
      year: 'numeric', 
      hour: '2-digit', 
      minute: '2-digit',
      hour12: true 
    });
    this.fechaHoraActa.set(fechaFormateada);
    this.showActaModal.set(true);
  }

  public imprimirActa(): void {
    window.print();
  }
}
