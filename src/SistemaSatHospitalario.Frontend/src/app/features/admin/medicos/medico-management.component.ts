import { Component, inject, signal, OnInit, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MedicoService, Medico } from '../../../core/services/medico.service';
import { SpecialtyService, Especialidad } from '../../../core/services/specialty.service';
import {
    LucideAngularModule,
    X,
    Save,
    Stethoscope,
    FileText,
    Clock,
    Plus as PlusIcon,
    ArrowLeft,
    Database,
    UserPlus,
    UserCog,
    Trash2
} from 'lucide-angular';
import { RouterModule } from '@angular/router';
import { SettingsService } from '../../../core/services/settings.service';
import { AppointmentsService } from '../../../core/services/appointments.service';
import { FilterByDayPipe } from '../../../shared/pipes/filter-by-day.pipe';

@Component({
  selector: 'app-medico-management',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule, RouterModule, FilterByDayPipe],
  templateUrl: './medico-management.component.html'
})
export class MedicoManagementComponent implements OnInit {
  private medicoService = inject(MedicoService);
  private specialtyService = inject(SpecialtyService);
  private settingsService = inject(SettingsService);

  public medicos = signal<Medico[]>([]);
  public especialidadesCatalogo = signal<Especialidad[]>([]);
  public isLoading = signal<boolean>(false);
  
  // Modal State
  public showModal = signal<boolean>(false);
  public isEditing = signal<boolean>(false);
  public currentMedico = signal<Partial<Medico>>({
    nombre: '',
    especialidadId: '',
    activo: true,
    honorarioBase: 0,
    telefono: ''
  });
  // Schedule State
  public showScheduleView = signal<boolean>(false);
  public medicosHorarios = signal<any[]>([]);
  public selectedMedicoId = signal<string | null>(null);
  public selectedMedico = computed(() => {
    const id = this.selectedMedicoId();
    if (!id) return null;
    return this.medicosHorarios().find(m => m.medicoId?.toLowerCase() === id.toLowerCase());
  });

  readonly icons = {
    Plus: UserPlus,
    Edit: UserCog,
    Delete: Trash2,
    Close: X,
    Save: Save,
    Doctor: Stethoscope,
    Report: FileText,
    Clock: Clock,
    PlusSchedule: PlusIcon,
    ArrowLeft: ArrowLeft,
    X: X
  };

  ngOnInit() {
    this.refresh();
  }

  refresh() {
    this.isLoading.set(true);
    // Carga paralela de médicos y especialidades
    this.specialtyService.getAll().subscribe(res => this.especialidadesCatalogo.set(res));
    
    this.medicoService.getAll().subscribe({
      next: (res) => {
        this.medicos.set(res);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  openCreate() {
    this.isEditing.set(false);
    this.currentMedico.set({
      nombre: '',
      especialidadId: '',
      activo: true,
      honorarioBase: 0,
      telefono: ''
    });
    this.showModal.set(true);
  }

  openEdit(medico: Medico) {
    this.isEditing.set(true);
    // Normalizar datos para asegurar vinculación correcta en el modal
    this.currentMedico.set({ 
      ...medico,
      especialidadId: medico.especialidadId?.toLowerCase(),
      honorarioBase: medico.honorarioBase ?? 0
    });
    this.showModal.set(true);
  }

  save() {
    const data = this.currentMedico();
    if (this.isEditing() && data.id) {
      this.medicoService.update(data as Medico).subscribe(() => {
        this.showModal.set(false);
        this.refresh();
      });
    } else {
      this.medicoService.create(data).subscribe(() => {
        this.showModal.set(false);
        this.refresh();
      });
    }
  }

  delete(id: string) {
    if (confirm('¿Estás seguro de eliminar este médico?')) {
      this.medicoService.delete(id).subscribe(() => this.refresh());
    }
  }

  // --- SCHEDULE LOGIC (Moved from Settings) ---
  loadMedicosHorarios() {
    this.settingsService.getMedicosHorarios().subscribe(res => this.medicosHorarios.set(res));
  }

  openSchedule(medicoId: string) {
    this.selectedMedicoId.set(medicoId);
    this.loadMedicosHorarios();
    this.showScheduleView.set(true);
  }

  closeSchedule() {
    this.showScheduleView.set(false);
    this.selectedMedicoId.set(null);
  }

  getDayName(day: number): string {
    const days = ['Domingo', 'Lunes', 'Martes', 'Miércoles', 'Jueves', 'Viernes', 'Sábado'];
    return days[day];
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
}
