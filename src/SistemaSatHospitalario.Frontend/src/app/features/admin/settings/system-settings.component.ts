import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { 
  LucideAngularModule, 
  Settings, 
  Save, 
  RefreshCw, 
  Database, 
  Users, 
  Shield, 
  Plus, 
  Trash2, 
  Edit, 
  Clock, 
  Calendar as LucideCalendar, 
  Stethoscope,
  Lock,
  User,
  ChevronRight,
  Info,
  Check,
  X,
  AlertCircle,
  Search,
  Package
} from 'lucide-angular';
import { SettingsService, SecurityConfig } from '../../../core/services/settings.service';
import { ConveniosService } from '../../../core/services/convenios.service';
import { AppointmentsService } from '../../../core/services/appointments.service';
import { ConfiguracionGeneral, UserDto } from '../../../core/models/settings.model';
import { SeguroConvenio } from '../../../core/models/convenio.model';
import { PermissionService } from '../../../core/services/permission.service';
import { FilterByDayPipe } from '../../../shared/pipes/filter-by-day.pipe';

@Component({
  selector: 'app-system-settings',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule, FilterByDayPipe],
  templateUrl: './system-settings.component.html'
})
export class SystemSettingsComponent implements OnInit {
  private settingsService = inject(SettingsService);
  private conveniosService = inject(ConveniosService);
  private appointmentsService = inject(AppointmentsService);
  public permissionService = inject(PermissionService);
  private route = inject(ActivatedRoute);

  public activeTab = 'general';
  public isLoading = signal<boolean>(false);

  // --- GENERAL TAB ---
  public configData: ConfiguracionGeneral = { 
    nombreEmpresa: '', 
    rif: '', 
    iva: 16,
    facturarLaboratorio: false,
    mostrarDetalleFacturacion: false,
    claveSupervisor: '',
    logoBase64: ''
  };

  // --- CONVENIOS TAB ---
  public convenios = signal<SeguroConvenio[]>([]);
  public showConvenioModal = false;
  public newC: any = { nombre: '', rtn: '', direccion: '', telefono: '', email: '', activo: true };
  public managingPricesConvenioId: number | null = null;
  public selectedConvenioName = '';
  public perfilPrices = signal<any[]>([]);
  public searchPerfilesQuery = '';

  // --- SECURITY TAB (RBAC) ---
  public securityMatrix = signal<SecurityConfig | null>(null);
  public showRoleModal = false;
  public newRoleName = '';
  public showUserModal = false;
  public users = signal<UserDto[]>([]);
  public newU: any = { username: '', email: '', password: '', roles: [] };

  // --- MEDICOS TAB (SCHEDULES) ---
  public medicosHorarios = signal<any[]>([]);
  public selectedMedicoId = signal<string | null>(null);
  public selectedMedico = computed(() => this.medicosHorarios().find(m => m.MedicoId === this.selectedMedicoId()));
  
  // --- CITAS TAB (MONITORING) ---
  public activeAppointments = signal<any[]>([]);
  public fechaFiltroCitas = new Date().toISOString().split('T')[0];



  public readonly icons = { 
    Settings, Save, RefreshCw, Database, Users, User, Shield, Plus, Trash2, Edit, 
    Clock, Calendar: LucideCalendar, Stethoscope, Lock, ChevronRight, Info, Check, X, AlertCircle, Search, 
    Package: Package
  };

  ngOnInit() {
    this.loadData();
    this.route.queryParams.subscribe(params => {
      if (params['tab']) {
        this.activeTab = params['tab'];
        this.onTabChange();
      }
    });
  }

  onTabChange() {
    if (this.activeTab === 'citas') this.loadAppointments();
    if (this.activeTab === 'medicos') this.loadMedicosHorarios();
    if (this.activeTab === 'usuarios') this.loadSecurityMatrix();
    if (this.activeTab === 'convenios') this.loadConvenios();
  }

  loadData() {
    this.settingsService.getConfig().subscribe(data => { if (data) this.configData = data; });
    this.loadConvenios();
    this.settingsService.getUsers().subscribe(data => this.users.set(data));
  }

  loadConvenios() {
    this.conveniosService.getAll().subscribe(data => this.convenios.set(data));
  }

  // --- GENERAL LOGIC ---
  saveGeneral() {
    this.settingsService.updateConfig(this.configData).subscribe(() => {
      alert('Ajustes generales actualizados con éxito.');
    });
  }

  onLogoUpload(event: any) {
    const file = event.target.files[0];
    if (file) {
      const reader = new FileReader();
      reader.onload = (e: any) => {
        this.configData.logoBase64 = e.target.result;
      };
      reader.readAsDataURL(file);
    }
  }

  // --- CONVENIOS LOGIC ---
  saveConvenio() {
    if (!this.newC.nombre) return;
    const action = this.newC.id ? this.conveniosService.update(this.newC) : this.conveniosService.create(this.newC);
    action.subscribe(() => {
      this.showConvenioModal = false;
      this.newC = { nombre: '', rtn: '', direccion: '', telefono: '', email: '', activo: true };
      this.loadConvenios();
    });
  }

  editConvenio(c: SeguroConvenio) {
    this.newC = { ...c };
    this.showConvenioModal = true;
  }

  deleteConvenio(id: number) {
    if (!confirm('¿Eliminar convenio?')) return;
    this.conveniosService.delete(id).subscribe(() => this.loadConvenios());
  }

  managePrices(c: SeguroConvenio) {
    this.managingPricesConvenioId = c.id!;
    this.selectedConvenioName = c.nombre;
    this.conveniosService.getPrecios(c.id!).subscribe(res => {
      this.perfilPrices.set(res);
    });
  }

  updatePrecio(p: any) {
    const cmd = {
      convenioId: this.managingPricesConvenioId,
      perfilId: p.perfilId,
      precioFijo: p.precioFijo
    };
    this.conveniosService.updatePrecio(cmd).subscribe(() => {
      // Opcional: Feedback visual
    });
  }

  // --- SECURITY & ROLE LOGIC ---
  loadSecurityMatrix() {
    this.settingsService.getSecurityMatrix().subscribe(res => this.securityMatrix.set(res));
  }

  createRole() {
    if (!this.newRoleName) return;
    this.settingsService.createRole(this.newRoleName).subscribe({
      next: () => {
        this.showRoleModal = false;
        this.newRoleName = '';
        this.loadSecurityMatrix();
      },
      error: (err) => alert(err.error?.error || 'Error al crear rol')
    });
  }

  deleteRole(name: string) {
    if (!confirm(`¿Eliminar el rol "${name}"? Esta acción no se puede deshacer.`)) return;
    this.settingsService.deleteRole(name).subscribe(() => this.loadSecurityMatrix());
  }

  togglePermission(role: any, permission: string) {
    const has = role.permissions.includes(permission);
    let newPerms = [...role.permissions];
    if (has) {
      newPerms = newPerms.filter(p => p !== permission);
    } else {
      newPerms.push(permission);
    }
    
    this.settingsService.updateRolePermissions(role.name, newPerms).subscribe(() => {
      role.permissions = newPerms;
    });
  }

  // --- MEDICOS & SCHEDULES ---
  loadMedicosHorarios() {
    this.settingsService.getMedicosHorarios().subscribe(res => this.medicosHorarios.set(res));
  }

  addScheduleBlock(medico: any, day: number) {
    medico.horarios.push({
      diaSemana: day,
      inicio: '08:00',
      fin: '12:00'
    });
  }

  removeScheduleBlock(medico: any, index: number) {
    medico.horarios.splice(index, 1);
  }

  saveMedicoSchedule(medico: any) {
    this.settingsService.syncMedicoSchedules(medico.medicoId, medico.horarios, medico.telefono).subscribe(() => {
      alert(`Horario de ${medico.medicoNombre} sincronizado.`);
    });
  }

  // --- CITAS MONITORING ---
  loadAppointments() {
    this.appointmentsService.getActiveAppointments(this.fechaFiltroCitas).subscribe(data => {
      this.activeAppointments.set(data);
    });
  }

  // --- USER CREATION ---
  saveUser() {
    this.settingsService.createUser(this.newU).subscribe(() => {
      this.showUserModal = false;
      this.loadData();
      alert('Usuario creado.');
    });
  }

  toggleNewUserRole(role: string) {
    const idx = this.newU.roles.indexOf(role);
    if (idx > -1) this.newU.roles.splice(idx, 1);
    else this.newU.roles.push(role);
  }

  // Helpers
  getDayName(day: number): string {
    const days = ['Domingo', 'Lunes', 'Martes', 'Miércoles', 'Jueves', 'Viernes', 'Sábado'];
    return days[day];
  }

  filteredPrices = () => {
    return this.perfilPrices().filter(p =>
      p.nombrePerfil.toLowerCase().includes(this.searchPerfilesQuery.toLowerCase()) ||
      p.perfilId.toString().includes(this.searchPerfilesQuery)
    );
  };
}
