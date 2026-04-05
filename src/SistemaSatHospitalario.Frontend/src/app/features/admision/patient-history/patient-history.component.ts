import { Component, inject, signal, OnInit, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PatientService, PatientHistory, PatientRecord } from '../../../core/services/patient.service';
import { FacturacionService, DailyBilledPatient } from '../../../core/services/facturacion.service';
import { PrintService } from '../../../core/services/print.service';

@Component({
  selector: 'app-patient-history',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './patient-history.component.html'
})
export class PatientHistoryComponent implements OnInit {
  private patientService = inject(PatientService);
  private facturacionService = inject(FacturacionService);
  private printService = inject(PrintService);

  public searchTerm = signal<string>('');
  public patients = signal<PatientRecord[]>([]);
  public selectedPatient = signal<PatientRecord | null>(null);
  public history = signal<PatientHistory[]>([]);
  public isLoading = signal<boolean>(false);
  public isSearching = signal<boolean>(false);
  public dailyPatients = signal<DailyBilledPatient[]>([]);

  // Filtros de fecha (V3.2 Date-Precision Filter)
  public startDate = signal<string>(new Date().toISOString().split('T')[0]);
  public endDate = signal<string>(new Date().toISOString().split('T')[0]);

  ngOnInit() {
    this.loadDailyPatients();
  }

  loadDailyPatients() {
    this.isLoading.set(true);
    const selected = this.startDate();
    
    // Si no hay fecha (teóricamente imposible por init), no buscamos.
    if (!selected) return;

    this.facturacionService.getDailyBilledPatients(selected).subscribe({
      next: (res) => {
        this.dailyPatients.set(res);
        this.isLoading.set(false);
      },
      error: (err) => {
        console.warn('[Expedientes] Fallo al cargar diario de facturación:', err.status);
        this.dailyPatients.set([]);
        this.isLoading.set(false);
      }
    });
  }

  public filteredHistory = computed(() => {
    const rawHistory = this.history();
    const start = this.startDate();
    const end = this.endDate();

    if (!start && !end) return rawHistory;

    return rawHistory.filter(h => {
      const date = h.fechaCreacion.split('T')[0]; // Comparación por día YYYY-MM-DD
      const afterStart = !start || date >= start;
      const beforeEnd = !end || date <= end;
      return afterStart && beforeEnd;
    });
  });

  buscar() {
    const term = this.searchTerm();
    
    // Si hay un término de búsqueda válido, buscamos por paciente
    if (term.length >= 3) {
      this.isSearching.set(true);
      this.patientService.searchPatients(term).subscribe({
        next: (res) => {
          this.patients.set(res);
          this.isSearching.set(false);
        },
        error: () => this.isSearching.set(false)
      });
      return;
    }

    // Si el término está vacío pero tenemos fechas, refrescamos los pacientes del día
    if (this.startDate()) {
      this.patients.set([]); // Limpiamos búsqueda de pacientes para mostrar grid del día
      this.loadDailyPatients();
    }
  }

  verHistorial(patient: PatientRecord) {
    if (!patient.id) return;
    this.selectedPatient.set(patient);
    this.history.set([]); // Limpiar previo
    this.isLoading.set(true);
    this.patientService.getHistory(patient.id).subscribe({
      next: (res) => {
        this.history.set(res);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  verHistorialDaily(p: DailyBilledPatient) {
    const patient: PatientRecord = {
      id: p.pacienteId,
      cedula: p.cedula,
      nombre: p.nombre,
      apellidos: p.apellidos
    };
    this.verHistorial(patient);
  }

  reimprimirRecibo(reciboId: string) {
    this.facturacionService.getReceiptPrintData(reciboId).subscribe({
      next: (data) => {
        const content = this.printService.generateReceiptHtml(data);
        this.printService.print(content, `Reimpresion ${data.numeroRecibo}`);
      }
    });
  }

  imprimirTodo() {
    const patient = this.selectedPatient();
    const history = this.filteredHistory(); // Solo lo que está filtrado
    if (!patient || !history.length) return;

    const content = this.printService.generateHistoryHtml(patient, history);
    this.printService.print(content, `Historial_${patient.cedula}`);
  }

  volverALaBusqueda() {
    const today = new Date().toISOString().split('T')[0];
    this.selectedPatient.set(null);
    this.history.set([]);
    this.startDate.set(today);
    this.endDate.set(today);
  }
}
