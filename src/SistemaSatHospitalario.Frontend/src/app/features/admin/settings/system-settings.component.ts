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
  Package,
  ArrowLeft,
  Mail,
  UserPlus,
  Eye,
  EyeOff
} from 'lucide-angular';
import { SettingsService, SecurityConfig } from '../../../core/services/settings.service';
import { ConveniosService } from '../../../core/services/convenios.service';
import { AppointmentsService } from '../../../core/services/appointments.service';
import { ConfiguracionGeneral, UserDto } from '../../../core/models/settings.model';
import { SeguroConvenio } from '../../../core/models/convenio.model';
import { PermissionService } from '../../../core/services/permission.service';


@Component({
  selector: 'app-system-settings',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule],
  templateUrl: './system-settings.component.html'
})
export class SystemSettingsComponent implements OnInit {
  private settingsService = inject(SettingsService);
  private conveniosService = inject(ConveniosService);
  private appointmentsService = inject(AppointmentsService);
  public permissionService = inject(PermissionService);
  private route = inject(ActivatedRoute);

  public activeTab: 'general' | 'convenios' | 'usuarios' | 'citas' = 'general';
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
  public securitySubTab = signal<'usuarios' | 'roles'>('usuarios');
  public users = signal<UserDto[]>([]);
  public roles = signal<string[]>([]);
  public selectedUserForPermissions = signal<UserDto | null>(null);
  public showPermissionsModal = signal<boolean>(false);
  public showPassword = false;
  public showConfirmPassword = false;
  public newU: any = { username: '', email: '', password: '', roles: [] };
  public confirmPassword = '';
  public editingUserId: string | null = null;

  // --- MONITOR TAB ---
  public activeAppointments = signal<any[]>([]);





  public readonly icons = {
    Settings, Save, RefreshCw, Database, Users, User, Shield, Plus, Trash2, Edit,
    Clock, Calendar: LucideCalendar, Stethoscope, Lock, ChevronRight, Info, Check, X, AlertCircle, Search,
    Package: Package,
    ArrowLeft: ArrowLeft,
    Mail: Mail,
    UserPlus: UserPlus,
    Eye: Eye,
    EyeOff: EyeOff
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

  get passwordRules() {
    const p = this.newU.password || '';
    return {
      length: p.length >= 8,
      upper: /[A-Z]/.test(p),
      lower: /[a-z]/.test(p),
      digit: /\d/.test(p)
    };
  }

  get isUsernameTaken() {
    if (this.editingUserId) return false;
    return this.users().some(u => u.username.toLowerCase() === (this.newU.username || '').toLowerCase());
  }

  onTabChange() {
    if (this.activeTab === 'usuarios') {
      this.loadSecurityMatrix();
      this.settingsService.getUsers().subscribe(data => this.users.set(data));
    }
    if (this.activeTab === 'convenios') this.loadConvenios();
    if (this.activeTab === 'citas') this.loadAppointments();
  }

  loadAppointments() {
    this.appointmentsService.getActiveAppointments().subscribe((res: any[]) => this.activeAppointments.set(res));
  }

  cancelAppointment(app: any) {
    const isReserva = app.status === 'RESERVA' || app.pacienteNombre.includes('RESERVA');
    const msg = isReserva ? 'RESERVA TEMPORAL' : 'CITA';
    
    if (confirm(`¿Está seguro que desea ANULAR esta ${msg}?`)) {
      this.appointmentsService.adminManageSchedule({
        action: 'Delete',
        type: isReserva ? 'Reserva' : 'Cita',
        targetId: app.id.toString()
      }).subscribe({
        next: () => {
          this.loadAppointments();
        },
        error: (err) => alert(err.error?.error || 'Error al anular')
      });
    }
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

  saveMedicoSchedule(medico: any) {
    this.settingsService.syncMedicoSchedules(medico.medicoId, medico.horarios, medico.telefono).subscribe(() => {
      alert(`Horario de ${medico.medicoNombre} sincronizado.`);
    });
  }



  // --- USER CREATION & EDITING ---
  openNewUserModal() {
    this.editingUserId = null;
    this.newU = { username: '', email: '', password: '', roles: [] };
    this.showUserModal = true;
  }

  editUser(user: UserDto) {
    this.editingUserId = user.id;
    this.newU = {
      username: user.username,
      email: user.email,
      password: '', // Leave blank
      roles: [...user.roles]
    };
    this.showUserModal = true;
  }

  saveUser() {
    if (this.editingUserId) {
      this.settingsService.updateUserRoles(this.editingUserId, this.newU.roles).subscribe({
        next: () => {
          this.showUserModal = false;
          this.loadData();
        },
        error: (err) => alert(err.error?.error || 'Error al actualizar roles')
      });
    } else {
      if (this.newU.password !== this.confirmPassword) {
        alert('Las contraseñas no coinciden.');
        return;
      }
      if (this.isUsernameTaken) {
        alert('El nombre de usuario ya está en uso.');
        return;
      }
      this.settingsService.createUser(this.newU).subscribe({
        next: () => {
          this.showUserModal = false;
          this.loadData();
        },
        error: (err) => alert(err.error?.error || 'Error al crear usuario')
      });
    }
  }

  // --- USER PERMISSIONS (GRANULAR) ---
  manageUserPermissions(user: UserDto) {
    this.selectedUserForPermissions.set({ ...user, permissions: [...(user.permissions || [])] });
    this.showPermissionsModal.set(true);
  }

  toggleUserPermission(permission: string) {
    const user = this.selectedUserForPermissions();
    if (!user) return;

    const perms = [...user.permissions];
    const index = perms.indexOf(permission);
    if (index > -1) {
      perms.splice(index, 1);
    } else {
      perms.push(permission);
    }
    user.permissions = perms;
    this.selectedUserForPermissions.set({ ...user });
  }

  saveUserPermissions() {
    const user = this.selectedUserForPermissions();
    if (!user) return;

    this.settingsService.updateUserPermissions(user.id, user.permissions).subscribe(() => {
      this.showPermissionsModal.set(false);
      this.loadData(); // Refresh list
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
