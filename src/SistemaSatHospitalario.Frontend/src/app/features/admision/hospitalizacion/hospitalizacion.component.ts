import { Component, inject, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { environment } from '../../../../environments/environment';
import { MultiSedeService } from '../../../core/services/multi-sede.service';
import { AuthService } from '../../../core/services/auth.service';
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
  Eye, 
  ArrowRight,
  RefreshCcw,
  Sparkles,
  ClipboardList
} from 'lucide-angular';

@Component({
  selector: 'app-hospitalizacion',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule],
  templateUrl: './hospitalizacion.component.html'
})
export class HospitalizacionComponent implements OnInit {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);
  private readonly sedeService = inject(MultiSedeService);
  public readonly auth = inject(AuthService);

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
    Eye,
    ArrowRight,
    RefreshCcw,
    Sparkles,
    ClipboardList
  };

  // State Signals
  public camas = signal<any[]>([]);
  public sedes = signal<any[]>([]);
  public isLoading = signal<boolean>(false);
  public isSaving = signal<boolean>(false);
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

  // Filtros de Sede
  public selectedSedeFilter = signal<string>('');

  public isAdmin(): boolean {
    return this.auth.isAdmin();
  }

  ngOnInit() {
    this.cargarDatos();
  }

  public cargarDatos() {
    this.isLoading.set(true);
    this.errorMessage.set(null);

    // 1. Cargar Camas de Monitoreo
    this.sedeService.getCamasMonitoreo().subscribe({
      next: (res) => {
        this.camas.set(res);
        this.isLoading.set(false);
      },
      error: (err) => {
        console.error('[HOSPITALIZACION] Error al cargar camas:', err);
        this.errorMessage.set('No se pudo recuperar el listado de camas de monitoreo.');
        this.isLoading.set(false);
      }
    });

    // 2. Cargar Sedes activas
    this.sedeService.getSedes().subscribe({
      next: (res) => {
        this.sedes.set(res.filter(s => s.activo));
        if (res.length > 0) {
          this.nuevaCama.sedeId = res[0].id;
        }
      },
      error: (err) => console.error('[HOSPITALIZACION] Error al cargar sedes:', err)
    });
  }

  // Camas filtradas por sede seleccionada
  public camasFiltradas = computed(() => {
    const list = this.camas();
    const filter = this.selectedSedeFilter().toLowerCase().trim();
    if (!filter) return list;
    return list.filter(c => 
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

  private showSuccess(msg: string) {
    this.actionMessage.set(msg);
    setTimeout(() => this.actionMessage.set(null), 5000);
  }
}
