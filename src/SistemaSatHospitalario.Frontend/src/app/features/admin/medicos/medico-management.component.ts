import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MedicoService, Medico } from '../../../core/services/medico.service';
import { SpecialtyService, Especialidad } from '../../../core/services/specialty.service';
import {
    LucideAngularModule,
    UserPlus,
    UserCog,
    Trash2,
    X,
    Save,
    Stethoscope
} from 'lucide-angular';

@Component({
  selector: 'app-medico-management',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule],
  templateUrl: './medico-management.component.html'
})
export class MedicoManagementComponent implements OnInit {
  private medicoService = inject(MedicoService);
  private specialtyService = inject(SpecialtyService);

  public medicos = signal<Medico[]>([]);
  public especialidadesCatalogo = signal<Especialidad[]>([]);
  public isLoading = signal<boolean>(false);
  
  // Modal State
  public showModal = signal<boolean>(false);
  public isEditing = signal<boolean>(false);
  public currentMedico = signal<Partial<Medico>>({
    nombre: '',
    especialidadId: '',
    activo: true
  });

  readonly icons = {
    Plus: UserPlus,
    Edit: UserCog,
    Delete: Trash2,
    Close: X,
    Save: Save,
    Doctor: Stethoscope
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
      activo: true
    });
    this.showModal.set(true);
  }

  openEdit(medico: Medico) {
    this.isEditing.set(true);
    this.currentMedico.set({ ...medico });
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
}
