import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PatientService, PatientHistory, PatientRecord } from '../../../core/services/patient.service';
import { FacturacionService } from '../../../core/services/facturacion.service';
import { PrintService } from '../../../core/services/print.service';

@Component({
  selector: 'app-patient-history',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './patient-history.component.html'
})
export class PatientHistoryComponent {
  private patientService = inject(PatientService);
  private facturacionService = inject(FacturacionService);
  private printService = inject(PrintService);

  public searchTerm = signal<string>('');
  public patients = signal<PatientRecord[]>([]);
  public selectedPatient = signal<PatientRecord | null>(null);
  public history = signal<PatientHistory[]>([]);
  public isLoading = signal<boolean>(false);
  public isSearching = signal<boolean>(false);

  buscar() {
    if (this.searchTerm().length < 3) return;
    this.isSearching.set(true);
    this.patientService.searchPatients(this.searchTerm()).subscribe({
      next: (res) => {
        this.patients.set(res);
        this.isSearching.set(false);
      },
      error: () => this.isSearching.set(false)
    });
  }

  verHistorial(patient: PatientRecord) {
    if (!patient.id) return;
    this.selectedPatient.set(patient);
    this.isLoading.set(true);
    this.patientService.getHistory(patient.id).subscribe({
      next: (res) => {
        this.history.set(res);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  reimprimirRecibo(reciboId: string) {
    this.facturacionService.getReceiptPrintData(reciboId).subscribe({
      next: (data) => {
        const content = this.printService.generateReceiptHtml(data);
        this.printService.print(content, `Reimpresion ${data.numeroRecibo}`);
      }
    });
  }

  volverALaBusqueda() {
    this.selectedPatient.set(null);
    this.history.set([]);
  }
}
