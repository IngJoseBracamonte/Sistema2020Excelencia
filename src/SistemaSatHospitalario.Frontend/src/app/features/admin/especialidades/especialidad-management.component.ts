import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SpecialtyService, Especialidad } from '../../../core/services/specialty.service';
import {
    LucideAngularModule,
    Plus,
    Edit,
    Trash2,
    X,
    Save,
    Bookmark
} from 'lucide-angular';

@Component({
  selector: 'app-especialidad-management',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule],
  templateUrl: './especialidad-management.component.html'
})
export class EspecialidadManagementComponent implements OnInit {
  private specialtyService = inject(SpecialtyService);

  public especialidades = signal<Especialidad[]>([]);
  public isLoading = signal<boolean>(false);
  
  // Modal State
  public showModal = signal<boolean>(false);
  public isEditing = signal<boolean>(false);
  public currentEsp = signal<Partial<Especialidad>>({
    nombre: '',
    activo: true
  });

  readonly icons = {
    Plus: Plus,
    Edit: Edit,
    Delete: Trash2,
    Close: X,
    Save: Save,
    Category: Bookmark
  };

  ngOnInit() {
    this.refresh();
  }

  refresh() {
    this.isLoading.set(true);
    this.specialtyService.getAll().subscribe({
      next: (res) => {
        this.especialidades.set(res);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  openCreate() {
    this.isEditing.set(false);
    this.currentEsp.set({
      nombre: '',
      activo: true
    });
    this.showModal.set(true);
  }

  openEdit(esp: Especialidad) {
    this.isEditing.set(true);
    this.currentEsp.set({ ...esp });
    this.showModal.set(true);
  }

  save() {
    const data = this.currentEsp();
    if (this.isEditing() && data.id) {
      this.specialtyService.update(data.id, data).subscribe(() => {
        this.showModal.set(false);
        this.refresh();
      });
    } else {
      this.specialtyService.create(data).subscribe(() => {
        this.showModal.set(false);
        this.refresh();
      });
    }
  }

  delete(id: string) {
    if (confirm('¿Estás seguro de eliminar esta especialidad?')) {
      this.specialtyService.delete(id).subscribe(() => this.refresh());
    }
  }
}
