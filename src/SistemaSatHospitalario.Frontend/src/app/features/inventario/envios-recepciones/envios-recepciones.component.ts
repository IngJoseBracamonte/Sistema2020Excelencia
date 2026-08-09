import { Component, inject, signal, OnInit, computed, ViewChild, ElementRef, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MultiSedeService, PedidoInterSede, Sede } from '../../../core/services/multi-sede.service';
import { InventoryService } from '../../../core/services/inventory.service';
import { Insumo } from '../../../core/models/inventory.model';
import { 
  LucideAngularModule, 
  Send,
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
  ChevronDown,
  ArrowRight,
  TrendingDown,
  History,
  CheckCircle2
} from 'lucide-angular';

export interface EnvioSubAreaForm {
  insumoId: string;
  nombreSubArea: string;
  cantidad: number;
  motivo: string;
}

@Component({
  selector: 'app-envios-recepciones',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule],
  templateUrl: './envios-recepciones.component.html'
})
export class EnviosRecepcionesComponent implements OnInit {
  private multiSedeService = inject(MultiSedeService);
  private inventoryService = inject(InventoryService);

  @ViewChild('searchInputRef') searchInputRef!: ElementRef<HTMLInputElement>;

  public activeMainTab = signal<'aprobacion' | 'envios-subareas'>('aprobacion');

  // --- Datos Generales ---
  public pedidos = signal<PedidoInterSede[]>([]);
  public insumos = signal<Insumo[]>([]);
  public sedes = signal<Sede[]>([]);
  public historialEnviosDirectos = signal<any[]>([]);

  public isLoading = signal<boolean>(false);
  public isSubmitting = signal<boolean>(false);

  // --- Filtros Pestaña Aprobación ---
  public searchQuery = signal<string>('');
  public selectedAreaIds = signal<string[]>([]);
  public fechaDesde = signal<string>('');
  public fechaHasta = signal<string>('');
  public showAreaDropdown = signal<boolean>(false);

  // Mapa local para edición de cantidades enviadas en aprobación
  public cantidadesAEnviarMap = signal<Record<string, Record<string, number>>>({});
  public observacionesMap = signal<Record<string, Record<string, string>>>({});

  // Modal Rechazo
  public showRechazoModal = signal<boolean>(false);
  public selectedPedidoRechazar = signal<PedidoInterSede | null>(null);
  public motivoRechazo = signal<string>('');

  // --- Formulario Envío Directo a Sub-Área ---
  public subAreasDisponibles = [
    'Emergencia',
    'Hospitalización',
    'Unidad de Cuidados Intensivos (UCI)',
    'Cirugía / Quirófano',
    'Laboratorio',
    'Farmacia',
    'Mantenimiento / Servicios Generales',
    'Consultorios Externos'
  ];

  public nuevoEnvio = signal<EnvioSubAreaForm>({
    insumoId: '',
    nombreSubArea: 'Laboratorio',
    cantidad: 1,
    motivo: ''
  });

  public selectedInsumoStock = computed(() => {
    const id = this.nuevoEnvio().insumoId;
    if (!id) return 0;
    const item = this.insumos().find(i => i.id === id);
    return item ? item.stockActual : 0;
  });

  public selectedInsumoUnidad = computed(() => {
    const id = this.nuevoEnvio().insumoId;
    if (!id) return 'Unidades';
    const item = this.insumos().find(i => i.id === id);
    return item ? item.unidadMedidaBase : 'Unidades';
  });

  // Alertas
  public successMessage = signal<string | null>(null);
  public errorMessage = signal<string | null>(null);

  readonly icons = {
    Send,
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
    ChevronDown,
    ArrowRight,
    TrendingDown,
    History,
    CheckCircle2
  };

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    this.isLoading.set(true);
    this.multiSedeService.getSedes().subscribe({
      next: (s) => this.sedes.set(s),
      error: (e) => console.error('Error al cargar sedes:', e)
    });

    this.multiSedeService.getPedidosRecibidos().subscribe({
      next: (data) => {
        this.pedidos.set(data);
        this.initMaps(data);
        this.isLoading.set(false);
      },
      error: (err) => {
        this.isLoading.set(false);
        this.showToast('Error al cargar pedidos inter-sede', 'error');
      }
    });

    this.inventoryService.getInsumos(true).subscribe({
      next: (items) => this.insumos.set(items),
      error: (err) => console.error('Error al cargar insumos:', err)
    });

    this.loadHistorialEnviosDirectos();
  }

  loadHistorialEnviosDirectos() {
    this.inventoryService.getHistorialMovimientos('EnvioSubArea').subscribe({
      next: (data) => this.historialEnviosDirectos.set(data),
      error: (err) => console.error('Error al cargar historial envíos directos:', err)
    });
  }

  private initMaps(list: PedidoInterSede[]) {
    const cantMap: Record<string, Record<string, number>> = {};
    const obsMap: Record<string, Record<string, string>> = {};

    list.forEach(p => {
      cantMap[p.id] = {};
      obsMap[p.id] = {};
      p.detalles.forEach(d => {
        cantMap[p.id][d.id] = d.cantidadDespachada > 0 ? d.cantidadDespachada : d.cantidadSolicitada;
        obsMap[p.id][d.id] = d.observacionesSupervisor || '';
      });
    });

    this.cantidadesAEnviarMap.set(cantMap);
    this.observacionesMap.set(obsMap);
  }

  // --- Filtros Pestaña Aprobación ---
  public filterPedido(p: PedidoInterSede): boolean {
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

    const selectedAreas = this.selectedAreaIds();
    if (selectedAreas.length > 0) {
      const matchArea = selectedAreas.some(sId => {
        if (p.sedeSolicitanteId === sId) return true;
        const s = this.sedes().find(x => x.id === sId);
        return s && (p.sedeSolicitanteNombre || '').toLowerCase().includes(s.nombre.toLowerCase());
      });
      if (!matchArea) return false;
    }

    if (this.fechaDesde()) {
      const fDesde = new Date(this.fechaDesde() + 'T00:00:00');
      const pFecha = new Date(p.fechaCreacion);
      if (pFecha < fDesde) return false;
    }

    if (this.fechaHasta()) {
      const fHasta = new Date(this.fechaHasta() + 'T23:59:59');
      const pFecha = new Date(p.fechaCreacion);
      if (pFecha > fHasta) return false;
    }

    return true;
  }

  public filteredPedidosPendientes = computed(() => {
    return this.pedidos().filter(p => p.estado === 'Pendiente' && this.filterPedido(p));
  });

  public updateCantidad(pedidoId: string, detalleId: string, cantidad: number) {
    const map = { ...this.cantidadesAEnviarMap() };
    if (!map[pedidoId]) map[pedidoId] = {};
    map[pedidoId][detalleId] = Math.max(0, cantidad);
    this.cantidadesAEnviarMap.set(map);
  }

  public getCantidad(pedidoId: string, detalleId: string, defaultVal: number): number {
    return this.cantidadesAEnviarMap()[pedidoId]?.[detalleId] ?? defaultVal;
  }

  public updateObservacion(pedidoId: string, detalleId: string, obs: string) {
    const map = { ...this.observacionesMap() };
    if (!map[pedidoId]) map[pedidoId] = {};
    map[pedidoId][detalleId] = obs;
    this.observacionesMap.set(map);
  }

  public getObservacion(pedidoId: string, detalleId: string): string {
    return this.observacionesMap()[pedidoId]?.[detalleId] ?? '';
  }

  public aprobarPedido(pedido: PedidoInterSede) {
    this.isSubmitting.set(true);

    const detalleAprobaciones: Record<string, number> = {};
    pedido.detalles.forEach(d => {
      detalleAprobaciones[d.id] = this.getCantidad(pedido.id, d.id, d.cantidadSolicitada);
    });

    this.multiSedeService.aprobarPedido(pedido.id, detalleAprobaciones).subscribe({
      next: () => {
        this.isSubmitting.set(false);
        this.showToast(`Pedido ${pedido.correlativo || pedido.id.slice(0, 8)} despachado exitosamente.`, 'success');
        this.loadData();
      },
      error: (err) => {
        this.isSubmitting.set(false);
        this.showToast(err?.error?.message || 'Error al despachar el pedido', 'error');
      }
    });
  }

  public abrirModalRechazo(pedido: PedidoInterSede) {
    this.selectedPedidoRechazar.set(pedido);
    this.motivoRechazo.set('');
    this.showRechazoModal.set(true);
  }

  public cerrarModalRechazo() {
    this.showRechazoModal.set(false);
    this.selectedPedidoRechazar.set(null);
    this.motivoRechazo.set('');
  }

  public confirmarRechazo() {
    const pedido = this.selectedPedidoRechazar();
    const motivo = this.motivoRechazo().trim();

    if (!pedido) return;
    if (!motivo) {
      this.showToast('El motivo de rechazo es obligatorio para auditoría.', 'error');
      return;
    }

    this.isSubmitting.set(true);
    this.multiSedeService.rechazarPedido(pedido.id, motivo).subscribe({
      next: () => {
        this.isSubmitting.set(false);
        this.cerrarModalRechazo();
        this.showToast(`Pedido ${pedido.correlativo || pedido.id.slice(0, 8)} rechazado.`, 'success');
        this.loadData();
      },
      error: (err) => {
        this.isSubmitting.set(false);
        this.showToast(err?.error?.message || 'Error al rechazar el pedido', 'error');
      }
    });
  }

  // --- Ejecución Envío Directo a Sub-Área ---
  public procesarEnvioSubArea() {
    const form = this.nuevoEnvio();

    if (!form.insumoId) {
      this.showToast('Por favor selecciona un insumo / medicamento.', 'error');
      return;
    }
    if (!form.nombreSubArea) {
      this.showToast('Selecciona la sub-área de destino.', 'error');
      return;
    }
    if (form.cantidad <= 0) {
      this.showToast('La cantidad debe ser mayor a 0.', 'error');
      return;
    }
    if (form.cantidad > this.selectedInsumoStock()) {
      this.showToast(`Stock insuficiente en Almacén Principal (${this.selectedInsumoStock()} disponibles).`, 'error');
      return;
    }

    this.isSubmitting.set(true);
    this.inventoryService.enviarASubArea({
      insumoId: form.insumoId,
      nombreSubArea: form.nombreSubArea,
      cantidad: form.cantidad,
      motivo: form.motivo
    }).subscribe({
      next: (res) => {
        this.isSubmitting.set(false);
        this.showToast(res.message || 'Envío directo procesado exitosamente.', 'success');
        // Reset form
        this.nuevoEnvio.set({
          insumoId: '',
          nombreSubArea: form.nombreSubArea,
          cantidad: 1,
          motivo: ''
        });
        this.loadData();
      },
      error: (err) => {
        this.isSubmitting.set(false);
        this.showToast(err?.error?.message || 'Error al procesar el envío directo.', 'error');
      }
    });
  }

  private showToast(msg: string, type: 'success' | 'error') {
    if (type === 'success') {
      this.successMessage.set(msg);
      this.errorMessage.set(null);
      setTimeout(() => this.successMessage.set(null), 5000);
    } else {
      this.errorMessage.set(msg);
      this.successMessage.set(null);
      setTimeout(() => this.errorMessage.set(null), 5000);
    }
  }
}
