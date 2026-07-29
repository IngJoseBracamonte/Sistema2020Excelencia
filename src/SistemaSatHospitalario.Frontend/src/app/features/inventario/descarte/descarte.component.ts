import { Component, inject, signal, OnInit, computed, ViewChild, ElementRef, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { InventoryService } from '../../../core/services/inventory.service';
import { Insumo, MovimientoInsumo, DescarteRequest } from '../../../core/models/inventory.model';
import { 
  LucideAngularModule, 
  Trash2, 
  Search, 
  Check, 
  AlertCircle,
  Package,
  History,
  FileText
} from 'lucide-angular';

@Component({
  selector: 'app-descarte',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule],
  templateUrl: './descarte.component.html'
})
export class DescarteComponent implements OnInit {
  private inventoryService = inject(InventoryService);

  @ViewChild('descarteSearchInput') descarteSearchInput!: ElementRef<HTMLInputElement>;

  public insumos = signal<Insumo[]>([]);
  public descartesHistory = signal<MovimientoInsumo[]>([]);
  public isLoading = signal<boolean>(false);
  public isSubmitting = signal<boolean>(false);

  @HostListener('window:keydown', ['$event'])
  handleKeyDown(event: KeyboardEvent) {
    if ((event.ctrlKey || event.metaKey) && event.key.toLowerCase() === 'k') {
      const target = event.target as HTMLElement;
      // No interrumpir si el usuario está escribiendo en un textarea o input de formulario que no sea el buscador
      if (target && target.tagName === 'TEXTAREA') return;

      event.preventDefault();
      this.focusSearchInput();
    }
  }

  focusSearchInput() {
    if (this.descarteSearchInput?.nativeElement) {
      this.descarteSearchInput.nativeElement.focus();
      this.descarteSearchInput.nativeElement.select();
    }
  }

  // Formulario de Descarte
  public descarteSearchTerm = signal<string>('');
  public filteredInsumos = signal<Insumo[]>([]);
  public selectedInsumo = signal<Insumo | null>(null);
  public cantidadDescarte = signal<number>(1);
  public motivoDescarte = signal<string>('');

  // Alertas
  public successMessage = signal<string | null>(null);
  public errorMessage = signal<string | null>(null);

  readonly icons = {
    Trash2,
    Search,
    Check,
    AlertCircle,
    Package,
    History,
    FileText
  };

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    this.isLoading.set(true);
    this.inventoryService.getInsumos(true).subscribe({
      next: (res) => this.insumos.set(res),
      error: () => this.isLoading.set(false)
    });

    this.inventoryService.getMovements().subscribe({
      next: (res) => {
        // Filtrar únicamente los movimientos tipo 'Descarte'
        const descartes = res.filter(m => 
          (m.tipoMovimiento || '').toLowerCase() === 'descarte'
        );
        this.descartesHistory.set(descartes);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  onSearchChange(val: string) {
    this.descarteSearchTerm.set(val);
    const term = val.trim().toLowerCase();
    if (term.length >= 1) {
      this.filteredInsumos.set(
        this.insumos().filter(i => 
          i.nombre.toLowerCase().includes(term) || 
          i.codigo.toLowerCase().includes(term) ||
          (i.principiosActivos && i.principiosActivos.some(pa => (pa.nombre || '').toLowerCase().includes(term)))
        )
      );
    } else {
      this.filteredInsumos.set([]);
    }
  }

  selectInsumo(insumo: Insumo) {
    this.selectedInsumo.set(insumo);
    this.descarteSearchTerm.set(insumo.nombre);
    this.filteredInsumos.set([]);
    this.cantidadDescarte.set(1);
  }

  submitDescarte() {
    const item = this.selectedInsumo();
    const cant = Number(this.cantidadDescarte());
    const motivo = this.motivoDescarte().trim();

    if (!item) {
      this.showError('Debe seleccionar un insumo a descartar.');
      return;
    }

    if (cant <= 0) {
      this.showError('La cantidad a descartar debe ser mayor a cero.');
      return;
    }

    if (!item.permiteFraccionamiento && !Number.isInteger(cant)) {
      this.showError(`El insumo '${item.nombre}' es de Dosis Entera y no permite fraccionamiento (ej. ${cant}). Ingrese un número entero.`);
      return;
    }

    if (!motivo) {
      this.showError('El motivo / justificación de descarte es obligatorio para auditoría.');
      return;
    }

    if (item.stockActual < cant) {
      this.showError(`El stock disponible (${item.stockActual}) es menor a la cantidad a descartar (${cant}).`);
      return;
    }

    this.isSubmitting.set(true);

    const dto: DescarteRequest = {
      insumoId: item.id,
      cantidad: cant,
      motivo: motivo
    };

    this.inventoryService.registrarDescarte(dto).subscribe({
      next: () => {
        this.showSuccess(`Descarte de ${cant} unidad(es) de '${item.nombre}' registrado con éxito.`);
        this.selectedInsumo.set(null);
        this.descarteSearchTerm.set('');
        this.cantidadDescarte.set(1);
        this.motivoDescarte.set('');
        this.isSubmitting.set(false);
        this.loadData();
      },
      error: (err) => {
        this.showError(err.error?.message || 'Error al registrar el descarte.');
        this.isSubmitting.set(false);
      }
    });
  }

  private showSuccess(msg: string) {
    this.successMessage.set(msg);
    this.errorMessage.set(null);
    setTimeout(() => this.successMessage.set(null), 5000);
  }

  private showError(msg: string) {
    this.errorMessage.set(msg);
    this.successMessage.set(null);
    setTimeout(() => this.errorMessage.set(null), 5000);
  }
}
