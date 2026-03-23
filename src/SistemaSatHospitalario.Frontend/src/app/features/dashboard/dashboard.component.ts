import { Component, inject, OnInit, OnDestroy, signal, computed } from '@angular/core';
import { NgFor, NgIf, NgClass, DecimalPipe, DatePipe, PercentPipe, NgTemplateOutlet } from '@angular/common';
import { RouterLink } from '@angular/router';
import { SignalrService } from '../../core/services/signalr.service';
import { AuthService } from '../../core/services/auth.service';
import { DashboardService, BusinessInsights } from '../../core/services/dashboard.service';
import { CajaService, DailyClosingReport } from '../../core/services/caja.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [NgFor, NgIf, NgClass, DecimalPipe, DatePipe, PercentPipe, NgTemplateOutlet],
  templateUrl: './dashboard.component.html'
})
export class DashboardComponent implements OnInit, OnDestroy {
  public signalRService = inject(SignalrService);
  public authService = inject(AuthService);
  public dashboardService = inject(DashboardService);
  public cajaService = inject(CajaService);

  private jwtToken = this.authService.getToken() || '';

  // Reactividad Directa
  public userName = computed(() => this.authService.currentUser()?.username || 'Usuario');
  public userRole = computed(() => this.authService.currentUser()?.role || 'Asistente');
  
  public isAdmin = computed(() => this.authService.currentUser()?.role === 'Administrador');
  public isRxAssistant = computed(() => this.authService.currentUser()?.role === 'Asistente Rx');
  public isParticularAssistant = computed(() => this.authService.currentUser()?.role === 'Asistente Particular');
  public isInsuranceAssistant = computed(() => this.authService.currentUser()?.role === 'Asistente Seguro');
  public isHospitalAssistant = computed(() => this.authService.currentUser()?.role === 'Asistente Hospitalario');
  public isEmergencyAssistant = computed(() => this.authService.currentUser()?.role === 'Asistente Emergencia');

  public tickets = this.signalRService.incomingTickets;
  public now = new Date();
  
  // Dashboard KPIs (Ciclo 17-23)
  public insights = signal<BusinessInsights | null>(null);
  public isLoading = signal<boolean>(false);

  // Gestión de Cierre (Fase 14)
  public showClosingModal = signal<boolean>(false);
  public closingReport = signal<DailyClosingReport | null>(null);
  public errorMessage = signal<string | null>(null);

  ngOnInit(): void {
    if (this.jwtToken) {
      this.signalRService.startConnection(this.jwtToken);
    }
    this.refreshKPIs();
  }

  refreshKPIs() {
    this.isLoading.set(true);
    this.dashboardService.getInsights().subscribe({
      next: (data: BusinessInsights) => {
        this.insights.set(data);
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
      }
    });
  }

  generarCierreTurno() {
    const user = this.authService.currentUser();
    if (!user) {
      this.errorMessage.set("Debe iniciar sesión para generar el cierre.");
      return;
    }

    this.isLoading.set(true);
    // Usar username como identificador de cajero
    this.cajaService.getPersonalReport(user.username).subscribe({
      next: (report: DailyClosingReport) => {
        this.isLoading.set(false);
        this.closingReport.set(report);
        this.showClosingModal.set(true);
      },
      error: (err: any) => {
        this.isLoading.set(false);
        this.errorMessage.set("No se pudo generar el reporte de cierre.");
      }
    });
  }

  ngOnDestroy(): void {
    if (this.signalRService) {
      this.signalRService.stopConnection();
    }
  }

  getStatusColor(status: string): string {
    switch (status?.toLowerCase()) {
      case 'completado': return 'bg-emerald-100 text-emerald-800 border-emerald-200';
      case 'en proceso': return 'bg-amber-100 text-amber-800 border-amber-200';
      case 'pendiente': return 'bg-slate-100 text-slate-800 border-slate-200';
      default: return 'bg-hospital-100 text-hospital-800 border-hospital-200';
    }
  }
}
