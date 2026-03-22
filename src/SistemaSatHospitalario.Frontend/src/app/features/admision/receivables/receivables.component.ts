import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ReceivablesService, PendingAR, SettleARRequest } from '../../../core/services/receivables.service';

@Component({
  selector: 'app-receivables',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './receivables.component.html',
  styleUrl: './receivables.component.css'
})
export class ReceivablesComponent implements OnInit {
  private arService = inject(ReceivablesService);

  // Signals para estado
  public receivables = signal<PendingAR[]>([]);
  public searchTerm = signal<string>('');
  public filterEstado = signal<string>('Pendiente');
  public isLoading = signal<boolean>(false);
  public isSettling = signal<boolean>(false);

  public actionMessage = signal<string | null>(null);
  public errorMessage = signal<string | null>(null);

  // Modal Settlement
  public selectedAR = signal<PendingAR | null>(null);
  public settlementData = {
    referenciaPago: '',
    observaciones: ''
  };

  ngOnInit() {
    this.refresh();
  }

  refresh() {
    this.isLoading.set(true);
    this.arService.getPending(this.searchTerm(), this.filterEstado()).subscribe({
      next: (res: PendingAR[]) => {
        this.receivables.set(res);
        this.isLoading.set(false);
      },
      error: () => {
        this.errorMessage.set("Error al cargar cuentas por cobrar.");
        this.isLoading.set(false);
      }
    });
  }

  abrirCobro(ar: PendingAR) {
    this.selectedAR.set(ar);
    this.settlementData = { referenciaPago: '', observaciones: '' };
  }

  liquidar() {
    if (!this.selectedAR() || !this.settlementData.referenciaPago) return;

    this.isSettling.set(true);
    const request: SettleARRequest = {
      arId: this.selectedAR()!.id,
      referenciaPago: this.settlementData.referenciaPago,
      observaciones: this.settlementData.observaciones
    };

    this.arService.settle(request).subscribe({
      next: () => {
        this.isSettling.set(false);
        this.selectedAR.set(null);
        this.actionMessage.set("Cobro registrado exitosamente.");
        this.refresh();
      },
      error: (err: any) => {
        this.isSettling.set(false);
        this.errorMessage.set(err.error?.error || "Error al procesar el cobro.");
      }
    });
  }

  onFilterChange(newVal: any) {
    this.filterEstado.set(newVal);
    this.refresh();
  }
}
