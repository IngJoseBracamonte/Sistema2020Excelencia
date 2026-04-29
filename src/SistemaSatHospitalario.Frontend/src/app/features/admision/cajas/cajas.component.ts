import { Component, inject, signal, OnInit, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { 
  LucideAngularModule, 
  DollarSign, 
  CreditCard, 
  RefreshCcw, 
  AlertCircle, 
  Check, 
  LayoutDashboard, 
  Users, 
  Lock 
} from 'lucide-angular';
import { CajaService, ResumenCajaGlobalDto, CajaSummaryDto, DailyClosingReport } from '../../../core/services/caja.service';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-cajas',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink, LucideAngularModule],
  templateUrl: './cajas.component.html',
  styleUrl: './cajas.component.css'
})
export class CajasComponent implements OnInit {
  readonly icons = {
    DollarSign,
    CreditCard,
    RefreshCcw,
    AlertCircle,
    Check,
    LayoutDashboard,
    Users,
    Lock
  };
  private cajaService = inject(CajaService);
  public authService = inject(AuthService);
  private fb = inject(FormBuilder);

  // Estados Reactivos
  public isLoading = signal<boolean>(false);
  public actionMessage = signal<string | null>(null);
  public errorMessage = signal<string | null>(null);
  
  // Data para el Admin
  public resumenCaja = signal<ResumenCajaGlobalDto | null>(null);
  public historialAdmin = signal<CajaSummaryDto | null>(null);
  
  // Data para el Asistente/Cajero
  public personalReport = signal<DailyClosingReport | null>(null);
  public isMiCajaAbierta = signal<boolean>(false);

  public isAdministrador = this.authService.isAdmin;

  ngOnInit() {
    this.checkStatus();
    if (this.isAdministrador()) {
      this.refrescarAdmin();
    }
  }

  checkStatus() {
    // Si es asistente, simplemente intentamos cargar su reporte del día. 
    // Si hay datos y no está cerrado, asumimos abierta (simplificado para Micro-Ciclo 28)
    this.cajaService.getPersonalReport().subscribe({
      next: (res) => {
        this.personalReport.set(res);
        // Si el total de órdenes > 0 y estamos en este componente, mostramos opción de cerrar.
        this.isMiCajaAbierta.set(res.totalOrdenes > 0);
      },
      error: (err) => console.log("Usuario sin actividad hoy todavía.")
    });
  }

  // Los asistentes ya NO abren caja manualmente. 
  // El backend lo hace al primer cobro.

  cerrarMiCaja() {
    this.isLoading.set(true);
    this.errorMessage.set(null);
    this.actionMessage.set(null);

    this.cajaService.cerrarCaja().subscribe({
      next: (res: any) => {
        // [COMPLETENESS] Mostrar resumen financiero al cerrar (V12.4)
        const summary = `Cierre Exitoso. Total USD: $${res.saldoFinalUSD.toFixed(2)}. Usuario: ${res.usuario}`;
        this.actionMessage.set(summary);
        
        this.isMiCajaAbierta.set(false);
        this.isLoading.set(false);
        this.checkStatus();
      },
      error: (err: any) => {
        this.errorMessage.set(err.error?.error || 'Error al cerrar su caja.');
        this.isLoading.set(false);
      }
    });
  }

  refrescarAdmin() {
    if (!this.isAdministrador()) return;
    this.cajaService.obtenerResumenDiario().subscribe(res => this.resumenCaja.set(res));
    this.cajaService.obtenerHistorial().subscribe(res => this.historialAdmin.set(res));
  }
}
