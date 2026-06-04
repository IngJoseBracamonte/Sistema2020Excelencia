import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink, ActivatedRoute } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { 
  LucideAngularModule, 
  DollarSign, 
  Search, 
  RefreshCcw, 
  Check, 
  X, 
  ChevronRight, 
  ChevronDown,
  AlertCircle,
  Info,
  BarChart3,
  Eye,
  Calendar,
  User,
  Stethoscope,
  Clock,
  Plus,
  Activity,
  FileText,
  Settings
} from 'lucide-angular';
import { ReceivablesService, PendingAR } from '../../../core/services/receivables.service';
import { ConveniosService } from '../../../core/services/convenios.service';
import { MedicoService } from '../../../core/services/medico.service';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'app-auditing',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule, RouterLink],
  templateUrl: './auditing.component.html',
  styleUrl: './auditing.component.css'
})
export class AuditingComponent implements OnInit {
  readonly icons = {
    DollarSign,
    Search,
    RefreshCcw,
    Check,
    X,
    ChevronRight,
    ChevronDown,
    AlertCircle,
    Info,
    Reportes: BarChart3,
    Eye,
    Calendar,
    User,
    Stethoscope,
    Clock,
    Plus,
    Activity,
    FileText,
    Settings
  };

  private arService = inject(ReceivablesService);
  private conveniosService = inject(ConveniosService);
  private medicoService = inject(MedicoService);
  private http = inject(HttpClient);
  private route = inject(ActivatedRoute);

  // Tabs layout
  public currentTab = signal<'cuentas' | 'ordenes-directas'>('cuentas');

  // --- TAB 1: CUENTAS POR AUDITAR ---
  public receivables = signal<PendingAR[]>([]);
  public searchTerm = signal<string>('');
  public filterAudit = signal<string>('Pendiente'); // Pendiente (isAudited=false), Procesada (isAudited=true)
  public filterConvenio = signal<'convenios' | 'particular' | 'todos'>('convenios');
  
  public displayedReceivables = computed(() => {
    const list = this.receivables();
    const filter = this.filterConvenio();
    if (filter === 'convenios') {
      return list.filter(x => x.seguroNombre && x.seguroNombre.trim() !== '' && x.seguroNombre.toUpperCase() !== 'PARTICULAR');
    } else if (filter === 'particular') {
      return list.filter(x => !x.seguroNombre || x.seguroNombre.trim() === '' || x.seguroNombre.toUpperCase() === 'PARTICULAR');
    }
    return list;
  });

  public startDate = signal<string>(new Date().toLocaleDateString('sv-SE'));
  public endDate = signal<string>(new Date().toLocaleDateString('sv-SE'));
  public isLoading = signal<boolean>(false);
  public actionMessage = signal<string | null>(null);
  public expandedRows = signal<Set<string>>(new Set<string>());

  // --- TAB 2: VALIDACIÓN DE ÓRDENES DIRECTAS ---
  public directOrders = signal<any[]>([]);
  public filterDirectType = signal<string>('ALL'); // ALL, RX, TOMO
  public expandedDirectRows = signal<Set<number>>(new Set<number>());

  // Modal de Validación
  public showValidationModal = signal<boolean>(false);
  public selectedDirectOrder = signal<any | null>(null);
  public clinicalServices = signal<any[]>([]);
  public selectedService = signal<any | null>(null);
  public overridePrecio = signal<number | null>(null);
  public overrideHonorario = signal<number | null>(null);
  public tipoIngresoVal = signal<string>('Particular'); // Particular, Seguro, Hospitalizacion, Emergencia
  public modalidadIngresoVal = signal<'Particular' | 'Convenio'>('Particular');
  public convenioIdVal = signal<number | null>(null);
  public convenios = signal<any[]>([]);
  public activeOpenAccount = signal<any | null>(null);
  public cargarACuentaAbierta = signal<boolean>(false);

  // Señales de médico para validación administrativa
  public doctors = signal<any[]>([]);
  public doctorSearchTerm = signal<string>('');
  public selectedDoctor = signal<any | null>(null);
  public doctorsFound = computed(() => {
    const term = this.doctorSearchTerm().toLowerCase().trim();
    if (!term) return [];
    return this.doctors().filter(d => d.nombre.toLowerCase().includes(term));
  });

  ngOnInit() {
    this.loadConvenios();
    this.medicoService.getAll().subscribe({
      next: (res) => this.doctors.set(res),
      error: (err) => console.error('[AUDITING] Error al cargar médicos:', err)
    });
    this.route.queryParams.subscribe(params => {
      const tab = params['tab'];
      if (tab === 'ordenes-directas') {
        this.setTab('ordenes-directas');
      } else {
        this.setTab('cuentas');
      }
    });
  }

  // Carga convenios generales para el dropdown de seguros
  private loadConvenios() {
    this.conveniosService.getAll().subscribe({
      next: (res) => this.convenios.set(res),
      error: (err) => console.error('[AUDITING] Error al cargar convenios:', err)
    });
  }

  public setTab(tab: 'cuentas' | 'ordenes-directas') {
    this.currentTab.set(tab);
    this.expandedRows.set(new Set());
    this.expandedDirectRows.set(new Set());
    
    if (tab === 'cuentas') {
      this.refresh();
    } else {
      this.loadPendingDirectOrders();
    }
  }

  // --- Lógica de Auditoría de Cuentas ---
  refresh() {
    this.isLoading.set(true);
    this.arService.getPending(this.searchTerm(), 'Todas', this.startDate(), this.endDate()).subscribe({
      next: (res: PendingAR[]) => {
        const isAuditedTarget = this.filterAudit() === 'Procesada';
        this.receivables.set(res.filter(x => x.isAudited === isAuditedTarget));
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
      }
    });
  }

  toggleRow(id: string) {
    const next = new Set(this.expandedRows());
    if (next.has(id)) next.delete(id);
    else next.add(id);
    this.expandedRows.set(next);
  }

  isExpanded(id: string): boolean {
    return this.expandedRows().has(id);
  }

  procesarAuditoria(ar: PendingAR) {
    if (confirm(`¿Marcar como auditada la cuenta de ${ar.pacienteNombre}?`)) {
      this.isLoading.set(true);
      this.arService.audit(ar.id).subscribe({
        next: () => {
          this.actionMessage.set(`Cuenta de ${ar.pacienteNombre} auditada correctamente.`);
          this.refresh();
          setTimeout(() => this.actionMessage.set(null), 5000);
        },
        error: () => {
          this.isLoading.set(false);
        }
      });
    }
  }

  onFilterChange(newVal: any) {
    this.filterAudit.set(newVal);
    this.refresh();
  }

  // --- Lógica de Validación de Órdenes Directas ---
  public loadPendingDirectOrders() {
    this.isLoading.set(true);
    const type = this.filterDirectType();
    let url = `${environment.apiUrl}/api/Imaging/pending-validation`;
    if (type !== 'ALL') {
      url += `?type=${type}`;
    }

    this.http.get<any[]>(url).subscribe({
      next: (res) => {
        this.directOrders.set(res);
        this.isLoading.set(false);
      },
      error: (err) => {
        console.error('[AUDITING] Error al obtener órdenes pendientes de validación:', err);
        this.isLoading.set(false);
      }
    });
  }

  public onDirectTypeFilterChange(newVal: string) {
    this.filterDirectType.set(newVal);
    this.loadPendingDirectOrders();
  }

  public toggleDirectRow(id: number) {
    const next = new Set(this.expandedDirectRows());
    if (next.has(id)) next.delete(id);
    else next.add(id);
    this.expandedDirectRows.set(next);
  }

  public isDirectExpanded(id: number): boolean {
    return this.expandedDirectRows().has(id);
  }

  // Abre el modal de validación y carga catálogo de servicios según tipo (RX o TOMO)
  public openValidationModal(order: any) {
    this.selectedDirectOrder.set(order);
    this.selectedService.set(null);
    this.overridePrecio.set(null);
    this.overrideHonorario.set(null);
    this.tipoIngresoVal.set('Particular');
    this.modalidadIngresoVal.set('Particular');
    this.convenioIdVal.set(null);
    this.activeOpenAccount.set(null);
    this.cargarACuentaAbierta.set(false);
    
    // Inicializar médico asignado si viene de la orden
    this.selectedDoctor.set(order.medicoSolicitanteId ? { id: order.medicoSolicitanteId, nombre: order.medicoSolicitanteNombre } : null);
    this.doctorSearchTerm.set(order.medicoSolicitanteNombre || '');
    
    // Consultar cuenta abierta activa para el paciente (V12.1 Flow Integration)
    if (order.pacienteId) {
      this.http.get<any>(`${environment.apiUrl}/api/Billing/OpenAccount/${order.pacienteId}`).subscribe({
        next: (res) => {
          if (res) {
            this.activeOpenAccount.set(res);
            this.cargarACuentaAbierta.set(true); // Pre-seleccionar por defecto si existe cuenta abierta
          }
        },
        error: (err) => {
          console.error('[AUDITING] Error al consultar cuenta abierta del paciente:', err);
        }
      });
    }
    
    // Cargar servicios clínicos del catálogo correspondientes al tipo de orden (RX / TOMO)
    this.http.get<any[]>(`${environment.apiUrl}/api/Imaging/services?type=${order.tipoServicio}`)
      .subscribe({
        next: (services) => {
          this.clinicalServices.set(services);
          this.showValidationModal.set(true);
        },
        error: (err) => {
          console.error('[AUDITING] Error cargando catálogo de servicios:', err);
          alert('No se pudo cargar el catálogo de servicios clínicos.');
        }
      });
  }

  public closeValidationModal() {
    this.showValidationModal.set(false);
  }

  public selectService(service: any) {
    this.selectedService.set(service);
    // Inicializar los montos con los valores base del servicio clínico
    this.overridePrecio.set(service.precioBase);
    this.overrideHonorario.set(service.honorarioBase);
  }

  public selectDoctor(d: any) {
    this.selectedDoctor.set(d);
    this.doctorSearchTerm.set(d.nombre);
  }

  public validateDirectOrder() {
    if (!this.selectedDirectOrder() || !this.selectedService()) {
      alert('Debe mapear la orden a un servicio clínico del catálogo.');
      return;
    }

    const orderId = this.selectedDirectOrder().id;
    const payload: any = {
      servicioId: this.selectedService().id,
      precio: this.overridePrecio(),
      honorario: this.overrideHonorario()
    };

    if (this.selectedDoctor()) {
      payload.medicoSolicitanteId = this.selectedDoctor().id;
      payload.medicoSolicitanteNombre = this.selectedDoctor().nombre;
    }

    if (this.activeOpenAccount() && this.cargarACuentaAbierta()) {
      payload.cuentaId = this.activeOpenAccount().id;
      payload.tipoIngreso = null;
      payload.convenioId = null;
    } else {
      payload.cuentaId = null;
      payload.tipoIngreso = this.tipoIngresoVal();
      
      const isSeguro = this.tipoIngresoVal() === 'Seguro';
      const isCumulative = this.tipoIngresoVal() === 'Hospitalizacion' || this.tipoIngresoVal() === 'Emergencia';
      const hasConvenio = isSeguro || (isCumulative && this.modalidadIngresoVal() === 'Convenio');
      
      if (hasConvenio && this.convenioIdVal()) {
        payload.convenioId = this.convenioIdVal();
      } else {
        payload.convenioId = null;
      }
    }

    this.isLoading.set(true);
    this.http.post(`${environment.apiUrl}/api/Imaging/${orderId}/validate-direct`, payload)
      .subscribe({
        next: (res: any) => {
          this.actionMessage.set(`Órden directa #${orderId} de ${this.selectedDirectOrder().pacienteNombre} validada y cargada correctamente.`);
          this.showValidationModal.set(false);
          this.loadPendingDirectOrders();
          setTimeout(() => this.actionMessage.set(null), 5000);
        },
        error: (err) => {
          console.error('[AUDITING] Error al validar orden:', err);
          alert('Error al validar orden directa: ' + (err.error?.Message || err.message));
          this.isLoading.set(false);
        }
      });
  }

  public descargarFactura(ar: PendingAR) {
    if (ar.reciboId) {
      const downloadUrl = `${environment.apiUrl}/api/ReciboFactura/${ar.reciboId}/Download`;
      window.open(downloadUrl, '_blank');
    } else {
      alert('Esta cuenta aún no tiene un recibo de cobro generado.');
    }
  }
}
