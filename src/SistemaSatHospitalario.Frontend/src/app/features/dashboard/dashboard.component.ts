import { Component, inject, OnInit, OnDestroy, signal, computed } from '@angular/core';
import { NgFor, NgIf, NgClass, CurrencyPipe, DecimalPipe, DatePipe, PercentPipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { SignalrService } from '../../core/services/signalr.service';
import { AuthService } from '../../core/services/auth.service';
import { DashboardService, BusinessInsights } from '../../core/services/dashboard.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [NgFor, NgIf, NgClass, DecimalPipe, DatePipe, PercentPipe],
  templateUrl: './dashboard.component.html'
})
export class DashboardComponent implements OnInit, OnDestroy {
  public signalRService = inject(SignalrService);
  public authService = inject(AuthService);
  public dashboardService = inject(DashboardService);

  private jwtToken = this.authService.getToken() || '';

  // Reactividad Directa
  public userName = this.authService.currentUser()?.username || 'Usuario';
  public userRole = this.authService.currentUser()?.role || 'Asistente';
  
  public isAdmin = computed(() => this.userRole === 'Administrador');
  public isRxAssistant = computed(() => this.userRole === 'Asistente Rx');
  public isParticularAssistant = computed(() => this.userRole === 'Asistente Particular');

  public tickets = this.signalRService.incomingTickets;
  public now = new Date();
  
  // Dashboard KPIs (Ciclo 17-23)
  public insights = signal<BusinessInsights | null>(null);
  public isLoading = signal<boolean>(false);

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
