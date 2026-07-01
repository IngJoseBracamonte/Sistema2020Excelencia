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
  Eye,
  MessageSquare
} from 'lucide-angular';
import { ExpedienteService, ControlCitaRow } from '../../../core/services/expediente.service';

@Component({
  selector: 'app-control-citas',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule],
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
    Eye,
    MessageSquare
  };

  private expedienteService = inject(ExpedienteService);
  private router = inject(Router);

  // Signals
  public records = signal<ControlCitaRow[]>([]);
  public dateFilter = signal<string>(new Date().toLocaleDateString('sv-SE'));
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

  enviarWhatsapp(doc: { name: string, citas: ControlCitaRow[] }) {
    if (!doc.citas || doc.citas.length === 0) return;
    
    let medicoTel = doc.citas.find(c => c.medicoTelefono)?.medicoTelefono || '';
    
    const inputTel = prompt(
      `Confirme o ingrese el teléfono de WhatsApp para el doctor ${doc.name} (incluya el código de país, ej: +584121234567):`,
      medicoTel
    );
    
    if (inputTel === null) return;
    const telefonoFormateado = inputTel.replace(/\D/g, '');
    if (!telefonoFormateado) {
      alert('Debe ingresar un número de teléfono válido.');
      return;
    }

    const citasActivas = doc.citas.filter(c => c.estado !== 'Cancelada' && c.estado !== 'Cancelado');
    if (citasActivas.length === 0) {
      alert('No hay pacientes activos hoy para enviar en la lista.');
      return;
    }

    const dateStr = this.dateFilter();
    let formattedDate = dateStr;
    try {
      const parts = dateStr.split('-');
      if (parts.length === 3) {
        formattedDate = `${parts[2]}/${parts[1]}/${parts[0]}`;
      }
    } catch (e) {}

    let mensaje = `Saludos Cordiales. Constancia de pacientes para el Dr(a). ${doc.name} el día ${formattedDate}:\n\n`;
    
    let globalIdx = 1;
    const consultas: string[] = [];
    const hospitalizacion: string[] = [];
    const observacion: string[] = [];

    citasActivas.forEach((c) => {
      const tipo = c.tipoIngreso || 'Particular';
      const honorario = `$${c.montoUSD.toFixed(2)}`;
      
      // Determinamos habitación/box de forma consistente y determinista
      const seed = c.pacienteCedula.split('').reduce((sum, char) => sum + char.charCodeAt(0), 0) || 0;
      const roomNum = 100 + (seed % 15);
      const roomLet = String.fromCharCode(65 + (seed % 3));
      
      if (tipo === 'Hospitalizacion') {
        hospitalizacion.push(`{IDX}. ${c.pacienteNombre} (${honorario}) • Hab. ${roomNum}${roomLet}`);
      } else if (tipo === 'Emergencia') {
        observacion.push(`{IDX}. ${c.pacienteNombre} (${honorario}) • Box ${roomNum}${roomLet}`);
      } else {
        consultas.push(`{IDX}. ${c.pacienteNombre} (${honorario})`);
      }
    });

    if (consultas.length > 0) {
      mensaje += `*Área de Consultas Privadas*\n`;
      consultas.forEach(line => {
        mensaje += `${line.replace('{IDX}', globalIdx.toString())}\n`;
        globalIdx++;
      });
      mensaje += `\n`;
    }

    if (hospitalizacion.length > 0) {
      mensaje += `*Área de Hospitalización*\n`;
      hospitalizacion.forEach(line => {
        mensaje += `${line.replace('{IDX}', globalIdx.toString())}\n`;
        globalIdx++;
      });
      mensaje += `\n`;
    }

    if (observacion.length > 0) {
      mensaje += `*Área de Observación*\n`;
      observacion.forEach(line => {
        mensaje += `${line.replace('{IDX}', globalIdx.toString())}\n`;
        globalIdx++;
      });
      mensaje += `\n`;
    }

    const url = `https://web.whatsapp.com/send/?phone=${telefonoFormateado}&text=${encodeURIComponent(mensaje)}`;
    window.open(url, '_blank');
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
