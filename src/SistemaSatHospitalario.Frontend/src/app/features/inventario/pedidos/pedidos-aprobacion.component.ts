import { Component, inject, signal, OnInit, computed, ViewChild, ElementRef, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MultiSedeService, PedidoInterSede, Sede } from '../../../core/services/multi-sede.service';
import { InventoryService } from '../../../core/services/inventory.service';
import { Insumo } from '../../../core/models/inventory.model';
import { 
  LucideAngularModule, 
  ClipboardCheck, 
  Search, 
  Check, 
  X, 
  AlertCircle,
  Package,
  Building,
  MessageSquare,
  Calendar,
  Filter,
  CheckSquare,
  Square,
  RotateCcw,
  ChevronDown
} from 'lucide-angular';

@Component({
  selector: 'app-pedidos-aprobacion',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule],
  templateUrl: './pedidos-aprobacion.component.html'
})
export class PedidosAprobacionComponent implements OnInit {
  private multiSedeService = inject(MultiSedeService);
  private inventoryService = inject(InventoryService);

  @ViewChild('aprobacionSearchInput') aprobacionSearchInput!: ElementRef<HTMLInputElement>;

  public pedidos = signal<PedidoInterSede[]>([]);
  public insumos = signal<Insumo[]>([]);
  public sedes = signal<Sede[]>([]);
  public isLoading = signal<boolean>(false);
  public isSubmitting = signal<boolean>(false);

  @HostListener('window:keydown', ['$event'])
  handleKeyDown(event: KeyboardEvent) {
    if ((event.ctrlKey || event.metaKey) && event.key.toLowerCase() === 'k') {
      const target = event.target as HTMLElement;
      if (target && (target.tagName === 'TEXTAREA' || (target.tagName === 'INPUT' && target.id !== 'aprobacionSearchInput'))) {
        return;
      }
      event.preventDefault();
      this.focusSearchInput();
    }
  }

  focusSearchInput() {
    if (this.aprobacionSearchInput?.nativeElement) {
      this.aprobacionSearchInput.nativeElement.focus();
      this.aprobacionSearchInput.nativeElement.select();
    }
  }

  // Mapa local para edición de cantidades enviadas: { [pedidoId]: { [detalleId]: cantidad } }
  public cantidadesAEnviarMap = signal<Record<string, Record<string, number>>>({});

  // Mapa local para observaciones por ítem: { [pedidoId]: { [detalleId]: observacion } }
  public observacionesMap = signal<Record<string, Record<string, string>>>({});

  // Filtros Avanzados (Texto, MultiSelect por Área, Rango de Fechas)
  public searchQuery = signal<string>('');
  public selectedAreaIds = signal<string[]>([]);
  public fechaDesde = signal<string>('');
  public fechaHasta = signal<string>('');
  public showAreaDropdown = signal<boolean>(false);

  // Modal Rechazo (auxiliar)
  public showRechazoModal = signal<boolean>(false);
  public selectedPedidoRechazar = signal<PedidoInterSede | null>(null);
  public motivoRechazo = signal<string>('');

  // Alertas
  public successMessage = signal<string | null>(null);
  public errorMessage = signal<string | null>(null);

  readonly icons = {
    ClipboardCheck,
    Search,
    Check,
    X,
    AlertCircle,
    Package,
    Building,
    MessageSquare,
    Calendar,
    Filter,
    CheckSquare,
    Square,
    RotateCcw,
    ChevronDown
  };

  public activeTab = signal<'pendientes' | 'historial'>('pendientes');
  public historialPedidos = signal<PedidoInterSede[]>([]);

  // Lógica de Filtro Multicriterio (Área, Fechas y Texto)
  public filterPedido(p: PedidoInterSede): boolean {
    // 1. Filtro por Búsqueda de Texto (Correlativo, Sede o Insumo)
    const query = this.searchQuery().toLowerCase().trim();
    if (query) {
      const matchCorrelativo = (p.correlativo || '').toLowerCase().includes(query);
      const matchSede = (p.sedeSolicitanteNombre || '').toLowerCase().includes(query);
      const matchInsumo = p.detalles?.some(d => (d.insumoNombre || '').toLowerCase().includes(query));
      const matchEstado = (p.estado || '').toLowerCase().includes(query);
      if (!matchCorrelativo && !matchSede && !matchInsumo && !matchEstado) {
        return false;
      }
    }

    // 2. Filtro MultiSelect por ÁREA / Sede
    const selectedAreas = this.selectedAreaIds();
    if (selectedAreas.length > 0) {
      const matchArea = selectedAreas.some(sId => {
        if (p.sedeSolicitanteId === sId) return true;
        const s = this.sedes().find(x => x.id === sId);
        return s && (p.sedeSolicitanteNombre || '').toLowerCase().includes(s.nombre.toLowerCase());
      });
      if (!matchArea) return false;
    }

    // 3. Filtro por Fecha Desde
    if (this.fechaDesde()) {
      const fDesde = new Date(this.fechaDesde() + 'T00:00:00');
      const pFecha = new Date(p.fechaCreacion);
      if (pFecha < fDesde) return false;
    }

    // 4. Filtro por Fecha Hasta
    if (this.fechaHasta()) {
      const fHasta = new Date(this.fechaHasta() + 'T23:59:59');
      const pFecha = new Date(p.fechaCreacion);
      if (pFecha > fHasta) return false;
    }

    return true;
  }

  public filteredPedidos = computed(() => {
    return this.pedidos().filter(p => this.filterPedido(p));
  });

  public filteredHistorial = computed(() => {
    return this.historialPedidos().filter(p => this.filterPedido(p));
  });

  // Métodos de Control para el MultiSelect de Áreas
  public toggleAreaSelection(areaId: string) {
    const current = this.selectedAreaIds();
    if (current.includes(areaId)) {
      this.selectedAreaIds.set(current.filter(id => id !== areaId));
    } else {
      this.selectedAreaIds.set([...current, areaId]);
    }
  }

  public selectAllAreas() {
    this.selectedAreaIds.set(this.sedes().map(s => s.id));
  }

  public clearAreaSelection() {
    this.selectedAreaIds.set([]);
  }

  public isAreaSelected(areaId: string): boolean {
    return this.selectedAreaIds().includes(areaId);
  }

  public getSelectedAreasLabel(): string {
    const selected = this.selectedAreaIds();
    if (selected.length === 0) return 'Todas las Áreas';
    if (selected.length === 1) {
      const s = this.sedes().find(x => x.id === selected[0]);
      return s ? s.nombre : '1 Área';
    }
    return `${selected.length} Áreas Seleccionadas`;
  }

  public resetFilters() {
    this.selectedAreaIds.set([]);
    this.fechaDesde.set('');
    this.fechaHasta.set('');
    this.searchQuery.set('');
  }


  ngOnInit() {
    this.loadData();
  }

  loadData() {
    this.isLoading.set(true);
    this.multiSedeService.getSedes().subscribe({
      next: (res) => this.sedes.set(res.filter(s => s.activo && !s.esPrincipal))
    });

    this.inventoryService.getInsumos(true).subscribe({
      next: (res) => this.insumos.set(res)
    });

    this.multiSedeService.getPedidosPendientes().subscribe({
      next: (res) => {
        this.pedidos.set(res);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });

    this.loadHistorial();
  }

  loadHistorial() {
    this.multiSedeService.getPedidosHistorial().subscribe({
      next: (res: any) => this.historialPedidos.set(res),
      error: (err: any) => console.error('Error al cargar historial de pedidos:', err)
    });
  }



  getStockDisponible(insumoId: string): number {
    return this.insumos().find(i => i.id === insumoId)?.stockActual ?? 0;
  }

  getCantidadAEnviar(pedidoId: string, detalleId: string, defaultVal: number): number {
    const pMap = this.cantidadesAEnviarMap()[pedidoId];
    if (pMap && pMap[detalleId] !== undefined) {
      return pMap[detalleId];
    }
    return defaultVal;
  }

  setCantidadAEnviar(pedidoId: string, detalleId: string, val: number) {
    const safeVal = Math.max(0, val);
    this.cantidadesAEnviarMap.update(prev => ({
      ...prev,
      [pedidoId]: {
        ...(prev[pedidoId] || {}),
        [detalleId]: safeVal
      }
    }));
  }

  getObservacionItem(pedidoId: string, detalleId: string): string {
    const pMap = this.observacionesMap()[pedidoId];
    return (pMap && pMap[detalleId]) ? pMap[detalleId] : '';
  }

  setObservacionItem(pedidoId: string, detalleId: string, obs: string) {
    this.observacionesMap.update(prev => ({
      ...prev,
      [pedidoId]: {
        ...(prev[pedidoId] || {}),
        [detalleId]: obs
      }
    }));
  }

  isObservacionRequired(pedidoId: string, detalleId: string, cantidadSolicitada: number): boolean {
    const enviada = this.getCantidadAEnviar(pedidoId, detalleId, cantidadSolicitada);
    return enviada < cantidadSolicitada;
  }

  despachar(pedido: PedidoInterSede) {
    const mapaCantidades = this.cantidadesAEnviarMap()[pedido.id] || {};
    const mapaObs = this.observacionesMap()[pedido.id] || {};

    // Validar si algún detalle requiere observación obligatoria y está vacía
    for (const det of pedido.detalles) {
      const cantAEnviar = mapaCantidades[det.id] !== undefined ? mapaCantidades[det.id] : det.cantidadSolicitada;
      if (cantAEnviar < det.cantidadSolicitada) {
        const obs = mapaObs[det.id] ? mapaObs[det.id].trim() : '';
        if (!obs) {
          this.showError(`Debe justificar en la observación el ajuste de cantidad enviada para '${det.insumoNombre}'.`);
          return;
        }
      }
    }

    this.isSubmitting.set(true);

    this.multiSedeService.despacharPedido(pedido.id, mapaCantidades, mapaObs).subscribe({
      next: () => {
        this.showSuccess(`Pedido ${pedido.correlativo} aprobado y despachado con éxito.`);
        this.isSubmitting.set(false);
        this.loadData();
      },
      error: (err) => {
        this.showError(err.error?.message || 'Error al despachar el pedido.');
        this.isSubmitting.set(false);
      }
    });
  }

  openRechazoModal(pedido: PedidoInterSede) {
    this.selectedPedidoRechazar.set(pedido);
    this.motivoRechazo.set('');
    this.showRechazoModal.set(true);
  }

  confirmarRechazo() {
    const pedido = this.selectedPedidoRechazar();
    const motivo = this.motivoRechazo().trim();

    if (!pedido || !motivo) {
      this.showError('El motivo de rechazo es obligatorio para la auditoría.');
      return;
    }

    this.isSubmitting.set(true);
    this.multiSedeService.rechazarPedido(pedido.id, motivo).subscribe({
      next: () => {
        this.showSuccess(`Pedido ${pedido.correlativo} rechazado.`);
        this.showRechazoModal.set(false);
        this.selectedPedidoRechazar.set(null);
        this.isSubmitting.set(false);
        this.loadData();
      },
      error: (err) => {
        this.showError(err.error?.message || 'Error al rechazar el pedido.');
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
