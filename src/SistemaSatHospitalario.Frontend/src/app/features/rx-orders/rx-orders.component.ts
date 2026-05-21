import { Component, inject, OnInit, computed, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { AuthService } from '../../core/services/auth.service';
import { SignalrService } from '../../core/services/signalr.service';
import { PatientService } from '../../core/services/patient.service';
import { MedicoService } from '../../core/services/medico.service';
import { environment } from '../../../environments/environment';
import { 
  LucideAngularModule, 
  Search, 
  RefreshCcw, 
  Check, 
  X, 
  ChevronRight, 
  ChevronDown, 
  AlertCircle,
  Info,
  Calendar,
  User,
  Plus,
  Stethoscope,
  LogOut,
  Activity,
  FileText
} from 'lucide-angular';

@Component({
  selector: 'app-rx-orders',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule, RouterLink],
  templateUrl: './rx-orders.component.html'
})
export class RxOrdersComponent implements OnInit {
  private authService = inject(AuthService);
  private signalRService = inject(SignalrService);
  private route = inject(ActivatedRoute);
  private http = inject(HttpClient);
  private patientService = inject(PatientService);
  private medicoService = inject(MedicoService);

  readonly icons = {
    Search,
    RefreshCcw,
    Check,
    X,
    ChevronRight,
    ChevronDown,
    AlertCircle,
    Info,
    Calendar,
    User,
    Plus,
    Stethoscope,
    LogOut,
    Activity,
    FileText
  };

  // Buffer local para manejar persistencia + live updates
  private localTickets = signal<any[]>([]);

  // Filtros
  public searchTerm = signal<string>('');
  public filterAudit = signal<string>('Pendiente'); // Pendiente, Procesada, Anulado
  public startDate = signal<string>(new Date().toISOString().split('T')[0]);
  public endDate = signal<string>(new Date().toISOString().split('T')[0]);
  public isLoading = signal<boolean>(false);
  public actionMessage = signal<string | null>(null);

  // Acordeón de filas
  public expandedRows = signal<Set<number>>(new Set<number>());

  // Modal de Orden Directa
  public showDirectModal = signal<boolean>(false);
  public patientSearchTerm = signal<string>('');
  public patientsFound = signal<any[]>([]);
  public selectedPatient = signal<any | null>(null);
  public directEstudio = signal<string>('');
  public doctorSearchTerm = signal<string>('');
  public doctors = signal<any[]>([]);
  public selectedDoctor = signal<any | null>(null);
  public isSavingDirect = signal<boolean>(false);

  /**
   * Senior Imaging Strategy (V16.2):
   * Differentiates station identity based on the current URL path.
   */
  public isTomoRoute = computed(() => this.route.snapshot.url[0]?.path === 'tomo-orders');

  // Médicos filtrados por búsqueda en modal
  public doctorsFound = computed(() => {
    const term = this.doctorSearchTerm().toLowerCase().trim();
    if (!term) return [];
    return this.doctors().filter(d => d.nombre.toLowerCase().includes(term));
  });

  // Combinamos los tickets de SignalR (si el filtro es Pendiente) con los de la base de datos
  public incomingTickets = computed(() => {
    const live = this.signalRService.incomingTickets().map(t => this.normalizeOrder(t));
    const local = this.localTickets();
    const isTomo = this.isTomoRoute();
    const currentFilter = this.filterAudit();

    let combined: any[] = [];
    if (currentFilter === 'Pendiente') {
      combined = [...live, ...local];
    } else {
      combined = [...local];
    }

    const typeTarget = isTomo ? 'TOMO' : 'RX';
    
    // Mapear filtro a estados de base de datos
    let statusTarget = 'Pendiente';
    if (currentFilter === 'Procesada') statusTarget = 'Procesado';
    else if (currentFilter === 'Anulado') statusTarget = 'Anulado';

    // Filtrar por tipo de servicio y estado
    let filtered = combined.filter(t => 
      t.tipoServicio === typeTarget && 
      t.status === statusTarget
    );

    // Búsqueda en el cliente si es en vivo (para historial se hace en server, pero por seguridad aplicamos aquí también)
    const term = this.searchTerm().toLowerCase().trim();
    if (term) {
      filtered = filtered.filter(t => 
        t.patientName.toLowerCase().includes(term) || 
        t.servicioNombre.toLowerCase().includes(term) || 
        t.orderId.toString().includes(term)
      );
    }

    // Eliminar duplicados por orderId
    return filtered.filter((v, i, a) => a.findIndex(t => t.orderId === v.orderId) === i);
  });

  public stationName = computed(() => {
    return this.isTomoRoute() ? 'Tomografía' : 'RX';
  });

  public stationColor = computed(() => {
    return this.isTomoRoute() ? 'sky' : 'rose';
  });

  ngOnInit(): void {
    const token = this.authService.getToken() || '';
    const role = this.authService.currentUser()?.role || '';
    
    // 1. Iniciar conexión SignalR
    this.signalRService.startConnection(token, role);

    // 2. Cargar médicos para modal de orden directa
    this.loadDoctors();

    // 3. Cargar datos iniciales
    this.refresh();
  }

  public refresh(): void {
    this.isLoading.set(true);
    const type = this.isTomoRoute() ? 'TOMO' : 'RX';
    
    let statusTarget = 'Pendiente';
    if (this.filterAudit() === 'Procesada') statusTarget = 'Procesado';
    else if (this.filterAudit() === 'Anulado') statusTarget = 'Anulado';

    const params: any = {
      type: type,
      status: statusTarget,
      startDate: this.startDate(),
      endDate: this.endDate()
    };

    if (this.searchTerm().trim()) {
      params.search = this.searchTerm().trim();
    }

    this.http.get<any[]>(`${environment.apiUrl}/api/Imaging/history`, { params })
      .subscribe({
        next: (orders) => {
          const mapped = orders.map(o => this.normalizeOrder(o));
          this.localTickets.set(mapped);
          this.isLoading.set(false);
        },
        error: (err) => {
          console.error('[IMAGING] Error cargando órdenes:', err);
          this.isLoading.set(false);
        }
      });
  }

  private loadDoctors(): void {
    this.medicoService.getAll().subscribe({
      next: (res) => {
        this.doctors.set(res);
      },
      error: (err) => console.error('[IMAGING] Error cargando médicos:', err)
    });
  }

  private normalizeOrder(o: any): any {
    return {
      orderId: o.orderId ?? o.id,
      status: o.status ?? o.estado,
      patientName: o.patientName ?? o.pacienteNombre,
      servicioNombre: o.servicioNombre ?? o.estudio,
      tipoServicio: o.tipoServicio ?? o.tipoServicio,
      fechaCreacion: o.fechaCreacion,
      procesadoPor: o.procesadoPor,
      fechaProcesado: o.fechaProcesado,
      esDirecta: o.esDirecta,
      requiereValidacion: o.requiereValidacion,
      validada: o.validada,
      validadorPor: o.validadorPor,
      fechaValidacion: o.fechaValidacion,
      medicoSolicitanteId: o.medicoSolicitanteId,
      medicoSolicitanteNombre: o.medicoSolicitanteNombre,
      pacienteId: o.pacienteId
    };
  }

  public toggleRow(id: number): void {
    const next = new Set(this.expandedRows());
    if (next.has(id)) next.delete(id);
    else next.add(id);
    this.expandedRows.set(next);
  }

  public isExpanded(id: number): boolean {
    return this.expandedRows().has(id);
  }

  public onFilterChange(newVal: string): void {
    this.filterAudit.set(newVal);
    this.refresh();
  }

  confirmarProceso(id: number): void {
    if (confirm('¿Está seguro de que desea marcar este estudio como procesado?')) {
      this.isLoading.set(true);
      this.http.post(`${environment.apiUrl}/api/Imaging/${id}/complete`, {})
        .subscribe({
          next: () => {
            this.actionMessage.set('Estudio procesado correctamente.');
            // Remover localmente para feedback inmediato
            this.localTickets.update(tickets => tickets.filter(t => t.orderId !== id));
            this.signalRService.incomingTickets.update(tickets => tickets.filter(t => t.orderId !== id));
            this.refresh();
            setTimeout(() => this.actionMessage.set(null), 5000);
          },
          error: (err) => {
            alert('Error al procesar la orden: ' + err.message);
            this.isLoading.set(false);
          }
        });
    }
  }

  anularOrden(id: number): void {
    if (confirm('¿Está seguro de que desea anular esta orden?')) {
      this.isLoading.set(true);
      this.http.post(`${environment.apiUrl}/api/Imaging/${id}/cancel`, {})
        .subscribe({
          next: () => {
            this.actionMessage.set('Orden anulada exitosamente.');
            this.localTickets.update(tickets => tickets.filter(t => t.orderId !== id));
            this.signalRService.incomingTickets.update(tickets => tickets.filter(t => t.orderId !== id));
            this.refresh();
            setTimeout(() => this.actionMessage.set(null), 5000);
          },
          error: (err) => {
            alert('Error al anular la orden: ' + (err.error?.Message || err.message));
            this.isLoading.set(false);
          }
        });
    }
  }

  // Búsqueda de pacientes para modal de orden directa
  public onPatientSearchChange(term: string): void {
    this.patientSearchTerm.set(term);
    if (term.trim().length >= 3) {
      this.patientService.searchPatients(term).subscribe({
        next: (res) => this.patientsFound.set(res),
        error: (err) => console.error('[IMAGING] Error al buscar paciente:', err)
      });
    } else {
      this.patientsFound.set([]);
    }
  }

  public selectPatient(p: any): void {
    this.selectedPatient.set(p);
    this.patientSearchTerm.set(`${p.nombre} ${p.apellidos || ''}`.trim());
    this.patientsFound.set([]);
  }

  public selectDoctor(d: any): void {
    this.selectedDoctor.set(d);
    this.doctorSearchTerm.set(d.nombre);
  }

  public openDirectOrderModal(): void {
    this.resetDirectForm();
    this.showDirectModal.set(true);
  }

  public closeDirectOrderModal(): void {
    this.showDirectModal.set(false);
  }

  public submitDirectOrder(): void {
    if (!this.selectedPatient() || !this.directEstudio().trim()) {
      alert('Debe seleccionar un paciente y detallar el estudio clínico.');
      return;
    }

    this.isSavingDirect.set(true);
    const type = this.isTomoRoute() ? 'TOMO' : 'RX';

    const payload = {
      pacienteId: this.selectedPatient().id,
      pacienteNombre: `${this.selectedPatient().nombre} ${this.selectedPatient().apellidos || ''}`.trim(),
      estudio: this.directEstudio().trim(),
      tipoServicio: type,
      medicoSolicitanteId: this.selectedDoctor()?.id || null,
      medicoSolicitanteNombre: this.selectedDoctor()?.nombre || null
    };

    this.http.post(`${environment.apiUrl}/api/Imaging/direct`, payload)
      .subscribe({
        next: () => {
          this.actionMessage.set('Orden directa creada con éxito. Pendiente de validación administrativa.');
          this.showDirectModal.set(false);
          this.resetDirectForm();
          this.refresh();
          setTimeout(() => this.actionMessage.set(null), 6000);
        },
        error: (err) => {
          alert('Error al crear orden directa: ' + (err.error?.Message || err.message));
          this.isSavingDirect.set(false);
        }
      });
  }

  private resetDirectForm(): void {
    this.selectedPatient.set(null);
    this.patientSearchTerm.set('');
    this.patientsFound.set([]);
    this.directEstudio.set('');
    this.selectedDoctor.set(null);
    this.doctorSearchTerm.set('');
    this.isSavingDirect.set(false);
  }

  logout(): void {
    this.authService.logout();
    this.signalRService.stopConnection();
  }
}
