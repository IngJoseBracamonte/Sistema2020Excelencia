import { Component, inject, OnInit, OnDestroy, signal, computed, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { environment } from '../../../../environments/environment';
import { MultiSedeService } from '../../../core/services/multi-sede.service';
import { AuthService } from '../../../core/services/auth.service';
import { SignalrService } from '../../../core/services/signalr.service';
import { 
  LucideAngularModule, 
  Bed, 
  Activity, 
  Clock, 
  DollarSign, 
  FileText, 
  Plus, 
  X, 
  Check, 
  User, 
  UserCheck,
  CreditCard,
  Printer,
  Eye, 
  ArrowRight,
  RefreshCcw,
  Sparkles,
  ClipboardList,
  DoorOpen,
  Trash2,
  Layers
} from 'lucide-angular';

@Component({
  selector: 'app-hospitalizacion',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule],
  templateUrl: './hospitalizacion.component.html'
})
export class HospitalizacionComponent implements OnInit, OnDestroy {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);
  private readonly sedeService = inject(MultiSedeService);
  private readonly signalRService = inject(SignalrService);
  public readonly auth = inject(AuthService);

  // Control de Pestañas Principales: Camas & Habitaciones vs Quirófanos
  public activeTab = signal<'camas' | 'quirofanos'>('camas');

  // State version control for network cut reconciliation
  public currentStateVersion = signal<number>(0);

  constructor() {
    // Escuchar cambios reactivos de SignalR y manejar reconciliación de versión
    effect(() => {
      const update = this.signalRService.incomingCamaUpdates();
      if (update) {
        const localVer = this.currentStateVersion();
        const remoteVer = update.versionEstado;
        
        console.log(`[HOSPITALIZACION] Actualización SignalR recibida. Local: ${localVer}, Remote: ${remoteVer}`);
        
        if (remoteVer === localVer + 1) {
          // Incremento secuencial feliz, refrescamos silenciosamente
          this.currentStateVersion.set(remoteVer);
          this.refrescarCamasSilenciosamente();
        } else if (remoteVer !== localVer) {
          // Salto de versión detectado (micro-corte de red), reconciliamos todo con pull directo
          console.warn(`[HOSPITALIZACION] Brecha de versión en SignalR (${localVer} -> ${remoteVer}). Reconciliando...`);
          this.reconciliarEstadoCompleto();
        }
      }
    }, { allowSignalWrites: true });
  }

  readonly icons = {
    Bed,
    Activity,
    Clock,
    DollarSign,
    FileText,
    Plus,
    X,
    Check,
    User,
    UserCheck,
    CreditCard,
    Printer,
    Eye,
    ArrowRight,
    RefreshCcw,
    Sparkles,
    ClipboardList,
    DoorOpen,
    Trash2,
    Layers
  };

  // State Signals
  public camas = signal<any[]>([]);
  public quirofanos = signal<any[]>([]);
  public sedes = signal<any[]>([]);
  public isLoading = signal<boolean>(false);
  public isSaving = signal<boolean>(false);
  public isSavingQuirofano = signal<boolean>(false);
  public actionMessage = signal<string | null>(null);
  public errorMessage = signal<string | null>(null);

  // Modal de Historial Triage
  public showHistoryModal = signal<boolean>(false);
  public selectedCamaForHistory = signal<any | null>(null);

  // Formulario de Nueva Cama
  public showCreateForm = signal<boolean>(false);
  public nuevaCama = {
    codigo: '',
    nombre: '',
    sedeId: '',
    esAreaAdmision: false
  };

  // Formulario de Anexar Quirófano
  public showCreateQuirofanoModal = signal<boolean>(false);
  public nuevoQuirofano = {
    codigo: '',
    nombre: ''
  };

  // Filtros de Sede
  public selectedSedeFilter = signal<string>('');

  public isAdmin(): boolean {
    return this.auth.isAdmin();
  }

  ngOnInit() {
    const token = this.auth.getToken() || '';
    const role = this.auth.currentUser()?.role || '';
    this.signalRService.startConnection(token, role);
    this.cargarDatos();
  }

  ngOnDestroy() {
    this.signalRService.stopConnection();
  }

  public cargarDatos() {
    this.isLoading.set(true);
    this.errorMessage.set(null);

    // 1. Cargar Camas de Monitoreo
    this.sedeService.getCamasMonitoreo().subscribe({
      next: (res) => {
        this.camas.set(res);
        if (res && res.length > 0) {
          this.currentStateVersion.set(res[0].versionEstado || 0);
        }
        this.isLoading.set(false);
      },
      error: (err) => {
        console.error('[HOSPITALIZACION] Error al cargar camas:', err);
        this.errorMessage.set('No se pudo recuperar el listado de camas de monitoreo.');
        this.isLoading.set(false);
      }
    });

    // 2. Cargar Sedes activas (excluyendo Almacén Principal)
    this.sedeService.getSedes().subscribe({
      next: (res) => {
        const clinicalSedes = res.filter(s => s.activo && !s.esPrincipal);
        this.sedes.set(clinicalSedes);
        if (clinicalSedes.length > 0) {
          this.nuevaCama.sedeId = clinicalSedes[0].id;
        }
      },
      error: (err) => console.error('[HOSPITALIZACION] Error al cargar sedes:', err)
    });

    // 3. Cargar Quirófanos
    this.cargarQuirofanos();
  }

  public cargarQuirofanos() {
    this.sedeService.getAreasClinicas().subscribe({
      next: (areas: any[]) => {
        const cirugiaSedeId = '10000000-0000-0000-0000-000000000005';
        const listaQuirofanos = (areas || []).filter(a =>
          a.activo !== false && (
            a.sedeId === cirugiaSedeId ||
            (a.sedeNombre || '').toLowerCase().includes('cirug') ||
            (a.nombre || '').toLowerCase().includes('quiróf') ||
            (a.nombre || '').toLowerCase().includes('quirof') ||
            (a.nombre || '').toLowerCase().includes('parto') ||
            (a.codigo || '').toLowerCase().startsWith('qx') ||
            (a.codigo || '').toLowerCase().startsWith('q')
          )
        );
        this.quirofanos.set(listaQuirofanos);
      },
      error: (err) => console.error('[HOSPITALIZACION] Error al cargar quirófanos:', err)
    });
  }

  public refrescarCamasSilenciosamente() {
    this.sedeService.getCamasMonitoreo().subscribe({
      next: (res) => {
        this.camas.set(res);
      },
      error: (err) => console.error('[HOSPITALIZACION] Error en refresco silencioso de camas:', err)
    });
  }

  public reconciliarEstadoCompleto() {
    this.sedeService.getCamasMonitoreo().subscribe({
      next: (res) => {
        this.camas.set(res);
        if (res && res.length > 0) {
          this.currentStateVersion.set(res[0].versionEstado || 0);
        }
      },
      error: (err) => console.error('[HOSPITALIZACION] Error al reconciliar estado de camas:', err)
    });
  }

  // Camas filtradas por sede seleccionada (excluyendo Almacén Principal y Quirófanos de Cirugía)
  public camasFiltradas = computed(() => {
    const cirugiaSedeId = '10000000-0000-0000-0000-000000000005';
    const list = this.camas().filter(c => 
      !c.esPrincipal && 
      !(c.sedeNombre || '').toLowerCase().includes('principal') &&
      c.sedeId !== cirugiaSedeId &&
      !(c.sedeNombre || '').toLowerCase().includes('cirug') &&
      !(c.nombre || '').toLowerCase().includes('quiróf') &&
      !(c.nombre || '').toLowerCase().includes('quirof') &&
      !(c.nombre || '').toLowerCase().includes('parto') &&
      !(c.codigo || '').toLowerCase().startsWith('qx')
    );
    const filter = this.selectedSedeFilter().toLowerCase().trim();
    if (!filter) return list;
    return list.filter(c => 
      (c.nombre || '').toLowerCase().includes(filter) ||
      (c.codigo || '').toLowerCase().includes(filter) ||
      (c.sedeNombre || '').toLowerCase().includes(filter) || 
      (c.sedeId || '').toLowerCase().includes(filter)
    );
  });

  // Métricas rápidas
  public totalCamas = computed(() => this.camasFiltradas().length);
  public camasDisponiblesCount = computed(() => this.camasFiltradas().filter(c => c.estado === 'Disponible').length);
  public camasOcupadasCount = computed(() => this.camasFiltradas().filter(c => c.estado === 'Ocupada').length);
  
  public porcentajeOcupacion = computed(() => {
    const total = this.totalCamas();
    if (total === 0) return 0;
    return Math.round((this.camasOcupadasCount() / total) * 100);
  });

  // Abrir Modal de Historial Triage
  public verHistorialClinico(cama: any) {
    this.selectedCamaForHistory.set(cama);
    this.showHistoryModal.set(true);
  }

  // Redirección en caliente a la facturación
  public irAFacturacion(cama: any) {
    if (cama.pacienteCedula) {
      // Redirigir al Cierre de Cuenta con el queryParam de cedula
      this.router.navigate(['/admision/cierre-cuenta/Hospitalizacion'], {
        queryParams: { cedula: cama.pacienteCedula }
      });
    }
  }

  // Alta Rápida de Camas
  public crearNuevaCama() {
    this.errorMessage.set(null);
    if (!this.nuevaCama.codigo.trim() || !this.nuevaCama.nombre.trim() || !this.nuevaCama.sedeId) {
      this.errorMessage.set('Todos los campos son obligatorios.');
      return;
    }

    this.isSaving.set(true);
    const payload = {
      sedeId: this.nuevaCama.sedeId,
      codigo: this.nuevaCama.codigo.trim().toUpperCase(),
      nombre: this.nuevaCama.nombre.trim(),
      esAreaAdmision: this.nuevaCama.esAreaAdmision
    };

    this.sedeService.crearCama(payload).subscribe({
      next: () => {
        this.isSaving.set(false);
        this.showCreateForm.set(false);
        this.nuevaCama.codigo = '';
        this.nuevaCama.nombre = '';
        this.showSuccess('Cama registrada exitosamente en el sistema.');
        this.cargarDatos(); // Recargar listado
      },
      error: (err) => {
        console.error('[HOSPITALIZACION] Error al crear cama:', err);
        this.errorMessage.set(err.error?.Error || err.error?.message || 'Error al guardar la nueva cama.');
        this.isSaving.set(false);
      }
    });
  }

  // Métodos para Quirófanos
  public abrirModalCrearQuirofano() {
    this.nuevoQuirofano = { codigo: '', nombre: '' };
    this.showCreateQuirofanoModal.set(true);
  }

  public cerrarModalCrearQuirofano() {
    this.showCreateQuirofanoModal.set(false);
    this.nuevoQuirofano = { codigo: '', nombre: '' };
  }

  public anexarQuirofano() {
    this.errorMessage.set(null);
    const cod = this.nuevoQuirofano.codigo.trim().toUpperCase();
    const nom = this.nuevoQuirofano.nombre.trim();

    if (!cod || !nom) {
      this.errorMessage.set('El código y el nombre del quirófano son obligatorios.');
      return;
    }

    // Resolver ID de la Sede Cirugía
    const cirugiaSede = this.sedes().find(s => s.codigo === 'CIRUGIA' || (s.nombre || '').toLowerCase().includes('cirug'));
    const sedeId = cirugiaSede?.id || '10000000-0000-0000-0000-000000000005';

    this.isSavingQuirofano.set(true);
    this.sedeService.createAreaClinica({
      sedeId,
      codigo: cod,
      nombre: nom
    }).subscribe({
      next: () => {
        this.isSavingQuirofano.set(false);
        this.cerrarModalCrearQuirofano();
        this.showSuccess(`Quirófano "${nom}" anexado exitosamente.`);
        this.cargarQuirofanos();
      },
      error: (err) => {
        console.error('[HOSPITALIZACION] Error al anexar quirófano:', err);
        this.errorMessage.set(err.error?.Error || err.error?.message || 'Error al anexar el quirófano.');
        this.isSavingQuirofano.set(false);
      }
    });
  }

  public eliminarQuirofano(id: string, nombre: string) {
    if (confirm(`¿Está seguro de eliminar o desactivar el quirófano "${nombre}"?`)) {
      this.sedeService.deleteAreaClinica(id).subscribe({
        next: () => {
          this.showSuccess(`Quirófano "${nombre}" eliminado/desactivado.`);
          this.cargarQuirofanos();
        },
        error: (err) => {
          console.error('[HOSPITALIZACION] Error al eliminar quirófano:', err);
          this.errorMessage.set(err.error?.message || 'Error al eliminar el quirófano.');
        }
      });
    }
  }

  private showSuccess(msg: string) {
    this.actionMessage.set(msg);
    setTimeout(() => this.actionMessage.set(null), 5000);
  }

  public abrirModalAbono(cama: any) {
    if (cama.pacienteCedula) {
      this.router.navigate(['/admision/cierre-cuenta/Hospitalizacion'], {
        queryParams: { cedula: cama.pacienteCedula, tab: 'abonos' }
      });
    } else {
      this.errorMessage.set('La cama no tiene un paciente o cédula asociada.');
      setTimeout(() => this.errorMessage.set(null), 4000);
    }
  }

  public imprimirConstanciaAltaVoluntaria() {
    this.actionMessage.set('En desarrollo: Formato de constancia de Alta Voluntaria pendiente por definir');
    setTimeout(() => this.actionMessage.set(null), 5000);
  }
}
