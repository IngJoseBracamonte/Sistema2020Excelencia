import { Component, inject, signal, OnInit, computed, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { PatientService, PatientHistory, PatientRecord } from '../../../core/services/patient.service';
import { FacturacionService, DailyBilledPatient } from '../../../core/services/facturacion.service';
import { PrintService } from '../../../core/services/print.service';
import { LucideAngularModule, UserPlus, X, Check, Edit3, User } from 'lucide-angular';

@Component({
  selector: 'app-patient-history',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule],
  templateUrl: './patient-history.component.html'
})
export class PatientHistoryComponent implements OnInit {
  private patientService = inject(PatientService);
  private facturacionService = inject(FacturacionService);
  private printService = inject(PrintService);
  private route = inject(ActivatedRoute);

  readonly icons = { UserPlus, X, Check, Edit3, User };

  public searchTerm = signal<string>('');
  public patients = signal<PatientRecord[]>([]);
  public selectedPatient = signal<PatientRecord | null>(null);
  public history = signal<PatientHistory[]>([]);
  public isLoading = signal<boolean>(false);
  public isSearching = signal<boolean>(false);
  public dailyPatients = signal<DailyBilledPatient[]>([]);

  // Filtros de fecha (V3.2 Date-Precision Filter)
  public startDate = signal<string>(new Date().toLocaleDateString('sv-SE'));
  public endDate = signal<string>(new Date().toLocaleDateString('sv-SE'));

  // Standalone Patient Editing (Phase 4)
  public showRegisterModal = signal<boolean>(false);
  public isEditingPatient = signal<boolean>(true);
  public actionMessage = signal<string | null>(null);
  public errorMessage = signal<string | null>(null);

  public newPatientData: any = {
    id: '',
    cedula: '',
    nombre: '',
    apellidos: '',
    sexo: 'M',
    fechaNacimiento: new Date().toISOString().split('T')[0],
    celular: '',
    codigoCelular: '0414',
    telefono: '',
    codigoTelefono: '0274',
    direccion: ''
  };

  public codigosCelular = ['0416', '0426', '0414', '0424', '0412', '0422'];
  public codigosTelefonoCombinados = ['0274', '0273', '0251', '0212', '0281', '0241', '0416', '0426', '0414', '0424', '0412', '0422'];

  constructor() {
    effect(() => {
      const action = this.actionMessage();
      const error = this.errorMessage();

      if (action || error) {
        setTimeout(() => {
          this.actionMessage.set(null);
          this.errorMessage.set(null);
        }, 5000);
      }
    });
  }

  ngOnInit() {
    this.route.queryParams.subscribe(params => {
        const searchParam = params['search'];
        if (searchParam) {
            this.searchTerm.set(searchParam);
            this.buscar();
        } else {
            this.loadDailyPatients();
        }
    });
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
    const today = new Date().toLocaleDateString('sv-SE');
    this.selectedPatient.set(null);
    this.history.set([]);
    this.startDate.set(today);
    this.endDate.set(today);
  }

  editarPaciente(patient: PatientRecord) {
    if (!patient) return;

    let formattedDob = '';
    if (patient.fechaNacimiento) {
      formattedDob = patient.fechaNacimiento.split('T')[0];
    }

    this.newPatientData = {
      id: patient.id,
      idPacienteLegacy: patient.idPacienteLegacy,
      cedula: patient.cedula,
      nombre: patient.nombre,
      apellidos: patient.apellidos || '',
      sexo: patient.sexo || 'ND',
      fechaNacimiento: formattedDob,
      celular: patient.celular || '',
      codigoCelular: patient.codigoCelular || '0414',
      telefono: patient.telefono || '',
      codigoTelefono: patient.codigoTelefono || '0274',
      direccion: patient.direccion || ''
    };

    this.showRegisterModal.set(true);
  }

  guardarEdicion() {
    if (!this.newPatientData.cedula || 
        !this.newPatientData.nombre || 
        !this.newPatientData.apellidos || 
        !this.newPatientData.fechaNacimiento || 
        !this.newPatientData.celular || 
        !this.newPatientData.direccion) {
      this.errorMessage.set("Todos los campos marcados con (*) son obligatorios: Cédula, Nombres, Apellidos, Fecha de Nacimiento, Celular y Dirección.");
      return;
    }

    this.isLoading.set(true);
    this.patientService.updatePatient(this.newPatientData).subscribe({
      next: (success: boolean) => {
        this.isLoading.set(false);
        if (success) {
          this.showRegisterModal.set(false);
          this.actionMessage.set("Datos del paciente actualizados exitosamente.");

          const currentSelected = this.selectedPatient();
          if (currentSelected && currentSelected.id === this.newPatientData.id) {
            this.selectedPatient.set({
              ...currentSelected,
              ...this.newPatientData
            });
          }

          this.patients.update(list => list.map(p => p.id === this.newPatientData.id ? { ...p, ...this.newPatientData } : p));
          
          this.dailyPatients.update(list => list.map(p => p.pacienteId === this.newPatientData.id ? { 
            ...p, 
            nombre: this.newPatientData.nombre, 
            apellidos: this.newPatientData.apellidos,
            cedula: this.newPatientData.cedula
          } : p));
        } else {
          this.errorMessage.set("No se pudieron actualizar los datos del paciente.");
        }
      },
      error: (err: any) => {
        this.isLoading.set(false);
        this.errorMessage.set(err.error?.message || err.error?.error || "Error al actualizar paciente.");
      }
    });
  }
}
