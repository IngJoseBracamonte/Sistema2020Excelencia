import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink, Router } from '@angular/router';
import { 
  LucideAngularModule, 
  Calendar, 
  Search, 
  RefreshCcw, 
  Check, 
  X, 
  ChevronRight, 
  Info,
  DollarSign,
  User,
  ShieldCheck,
  LayoutGrid,
  List,
  Eye
} from 'lucide-angular';
import { ExpedienteService, ControlCitaRow } from '../../../core/services/expediente.service';

@Component({
  selector: 'app-control-citas',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule, RouterLink],
  templateUrl: './control-citas.component.html'
})
export class ControlCitasComponent implements OnInit {
  readonly icons = {
    Calendar,
    Search,
    RefreshCcw,
    Check,
    X,
    ChevronRight,
    Info,
    DollarSign,
    User,
    ShieldCheck,
    LayoutGrid,
    List,
    Eye
  };

  private expedienteService = inject(ExpedienteService);
  private router = inject(Router);

  // Signals
  public records = signal<ControlCitaRow[]>([]);
  public dateFilter = signal<string>(new Date().toISOString().split('T')[0]);
  public searchTerm = signal<string>('');
  public isLoading = signal<boolean>(false);
  
  // View mode: 'grouped' (Specialty -> Doctor) or 'flat' (Single list / By Doctor)
  public viewMode = signal<'grouped' | 'flat'>('grouped');

  // Computed data for grouping
  public groupedRecords = computed(() => {
    const data = this.records();
    if (this.viewMode() === 'flat') return [];

    const groups: { specialty: string, doctors: { name: string, citas: ControlCitaRow[] }[] }[] = [];

    data.forEach(item => {
      let specGroup = groups.find(g => g.specialty === item.especialidad);
      if (!specGroup) {
        specGroup = { specialty: item.especialidad, doctors: [] };
        groups.push(specGroup);
      }

      let docGroup = specGroup.doctors.find(d => d.name === item.medico);
      if (!docGroup) {
        docGroup = { name: item.medico, citas: [] };
        specGroup.doctors.push(docGroup);
      }

      docGroup.citas.push(item);
    });

    return groups;
  });

  ngOnInit() {
    this.refresh();
  }

  refresh() {
    this.isLoading.set(true);
    this.expedienteService.getControlCitas(this.dateFilter()).subscribe({
      next: (res) => {
        this.records.set(res);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  markAtendida(id: string) {
    if (!confirm('¿Marcar esta cita como ATENDIDA?')) return;
    this.expedienteService.markAtendida(id).subscribe(() => this.refresh());
  }

  cancelCita(id: string) {
    if (!confirm('¿Está seguro de CANCELAR esta cita?')) return;
    this.expedienteService.cancelCita(id).subscribe(() => this.refresh());
  }

  verFacturacion(cedula: string) {
    this.router.navigate(['/expediente-facturacion'], { queryParams: { searchTerm: cedula } });
  }

  get filteredRecords() {
    const term = this.searchTerm().toLowerCase();
    if (!term) return this.records();
    return this.records().filter(r => 
      r.pacienteNombre.toLowerCase().includes(term) || 
      r.pacienteCedula.toLowerCase().includes(term) ||
      r.medico.toLowerCase().includes(term)
    );
  }
}
