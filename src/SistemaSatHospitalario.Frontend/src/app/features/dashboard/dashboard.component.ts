import { Component, inject, OnInit, OnDestroy, signal, computed } from '@angular/core';
import { NgFor, NgIf, NgClass, DecimalPipe, DatePipe, PercentPipe, NgTemplateOutlet } from '@angular/common';
import { RouterLink, Router } from '@angular/router';
import { NgApexchartsModule } from 'ng-apexcharts';
import { SignalrService } from '../../core/services/signalr.service';
import { AuthService } from '../../core/services/auth.service';
import { DashboardService, BusinessInsights } from '../../core/services/dashboard.service';
import { CajaService, DailyClosingReport } from '../../core/services/caja.service';
import { SettingsService } from '../../core/services/settings.service';
import { Subscription } from 'rxjs';
import {
  ApexAxisChartSeries,
  ApexChart,
  ApexXAxis,
  ApexDataLabels,
  ApexStroke,
  ApexGrid,
  ApexPlotOptions,
  ApexLegend,
  ApexTooltip,
  ApexFill,
  ApexResponsive
} from "ng-apexcharts";

export type ChartOptions = {
  series: ApexAxisChartSeries | any[];
  chart: ApexChart;
  xaxis: ApexXAxis;
  dataLabels: ApexDataLabels;
  grid: ApexGrid;
  stroke: ApexStroke;
  plotOptions: ApexPlotOptions;
  legend: ApexLegend;
  tooltip: ApexTooltip;
  fill: ApexFill;
  responsive: ApexResponsive[];
  colors: string[];
  labels: string[];
};

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [NgFor, NgIf, NgClass, DecimalPipe, DatePipe, NgTemplateOutlet, NgApexchartsModule],
  templateUrl: './dashboard.component.html'
})
export class DashboardComponent implements OnInit, OnDestroy {
  public signalRService = inject(SignalrService);
  public authService = inject(AuthService);
  public dashboardService = inject(DashboardService);
  public cajaService = inject(CajaService);
  public settingsService = inject(SettingsService);
  private router = inject(Router);

  private jwtToken = this.authService.getToken() || '';
  private tasaSubscription?: Subscription;

  // Reactividad Directa
  public userName = computed(() => this.authService.currentUser()?.username || 'Usuario');
  public userRole = computed(() => this.authService.currentUser()?.role || 'Asistente');
  
  public isAdmin = this.authService.isAdmin;
  public isRxAssistant = this.authService.isRxAssistant;
  public isParticularAssistant = this.authService.isParticularAssistant;
  public isInsuranceAssistant = this.authService.isInsuranceAssistant;
  public isHospitalAssistant = this.authService.isHospitalAssistant;
  public isEmergencyAssistant = this.authService.isEmergencyAssistant;

  public tickets = this.signalRService.incomingTickets;
  public now = new Date();
  
  // Dashboard KPIs (Ciclo 17-23)
  public insights = signal<BusinessInsights | null>(null);
  public isLoading = signal<boolean>(false);

  // Rediseño Dashboard (4 Paneles) - Filtros & Controles
  public revenueRange = signal<'Dia' | 'Semana' | 'Mes'>('Semana');
  public auditFilter = signal<string>('Todos');

  // Chart Options
  public revenueChartOptions: Partial<ChartOptions> = {};
  public purchasesChartOptions: Partial<ChartOptions> = {};
  public originChartOptions: Partial<ChartOptions> = {};

  // Lista de auditoría filtrada de forma reactiva
  public filteredAudit = computed(() => {
    const filter = this.auditFilter();
    const list = this.insights()?.historialAuditoria || [];
    if (filter === 'Todos') return list;
    return list.filter(item => 
      item.modulo.toLowerCase().includes(filter.toLowerCase()) || 
      item.tipoEvento.toLowerCase().includes(filter.toLowerCase())
    );
  });

  // Gestión de Cierre (Fase 14)
  public showClosingModal = signal<boolean>(false);
  public closingReport = signal<DailyClosingReport | null>(null);
  public errorMessage = signal<string | null>(null);

  // Currency Toggles (USD / Bs conversion display)
  public displayUsdRecaudacion = signal<boolean>(true);
  public displayUsdSaldo = signal<boolean>(true);
  public animatingRecaudacion = signal<boolean>(false);
  public animatingSaldo = signal<boolean>(false);
  public tasa = signal<number>(0);

  ngOnInit(): void {
    const isExclusiveEmergency = this.isEmergencyAssistant() && !this.isAdmin() && !this.isParticularAssistant() && !this.isInsuranceAssistant() && !this.authService.isSupervisor();
    const isExclusiveHospital = this.isHospitalAssistant() && !this.isAdmin() && !this.isParticularAssistant() && !this.isInsuranceAssistant() && !this.authService.isSupervisor();

    if (isExclusiveEmergency) {
      this.router.navigate(['/enfermeria']);
      return;
    }
    if (isExclusiveHospital) {
      this.router.navigate(['/cierre-cuenta/Hospitalizacion']);
      return;
    }

    if (this.jwtToken) {
      const role = this.authService.currentUser()?.role || '';
      this.signalRService.startConnection(this.jwtToken, role);
    }
    this.refreshKPIs();
    
    // Sincronizar tasa en tiempo real
    this.tasaSubscription = this.settingsService.tasa$.subscribe(val => {
      this.tasa.set(val);
    });
  }

  refreshKPIs() {
    this.isLoading.set(true);
    this.dashboardService.getInsights().subscribe({
      next: (data: BusinessInsights) => {
        this.insights.set(data);
        this.initCharts(data);
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
      }
    });
  }

  setRevenueRange(range: 'Dia' | 'Semana' | 'Mes') {
    this.revenueRange.set(range);
    const currentData = this.insights();
    if (currentData) {
      this.initCharts(currentData);
    }
  }

  setAuditFilter(filter: string) {
    this.auditFilter.set(filter);
  }

  private initCharts(data: BusinessInsights) {
    // Zero-data safety handling
    const rawTrend = data?.tendenciaIngresos || [];
    let trendCategories: string[] = [];
    let trendData: number[] = [];

    const range = this.revenueRange();
    if (range === 'Dia') {
      trendCategories = ['08:00', '10:00', '12:00', '14:00', '16:00', '18:00'];
      const totalToday = data?.totalVentasHoy || 0;
      trendData = totalToday > 0 ? [totalToday * 0.1, totalToday * 0.25, totalToday * 0.35, totalToday * 0.15, totalToday * 0.1, totalToday * 0.05] : [0, 0, 0, 0, 0, 0];
    } else if (range === 'Mes') {
      trendCategories = ['Semana 1', 'Semana 2', 'Semana 3', 'Semana 4'];
      const totalMonth = (data?.totalVentasHoy || 0) * 4;
      trendData = totalMonth > 0 ? [totalMonth * 0.2, totalMonth * 0.3, totalMonth * 0.25, totalMonth * 0.25] : [0, 0, 0, 0];
    } else {
      // Semana (7 días)
      trendCategories = rawTrend.length > 0 ? rawTrend.map(t => t.fecha) : ['Lun', 'Mar', 'Mié', 'Jue', 'Vie', 'Sáb', 'Dom'];
      trendData = rawTrend.length > 0 ? rawTrend.map(t => t.monto) : [0, 0, 0, 0, 0, 0, 0];
    }

    // 1. Gráfica de Ingresos USD-First (Area Chart)
    this.revenueChartOptions = {
      series: [{
        name: "Total Neto Ingresado ($ USD)",
        data: trendData
      }],
      chart: {
        type: "area",
        height: 280,
        toolbar: { show: false },
        zoom: { enabled: false },
        background: 'transparent',
        foreColor: '#94a3b8'
      },
      colors: ['#10b981'], // Emerald 500
      stroke: { curve: 'smooth', width: 3 },
      fill: {
        type: 'gradient',
        gradient: {
          shadeIntensity: 1,
          opacityFrom: 0.35,
          opacityTo: 0.05,
          stops: [0, 90, 100]
        }
      },
      dataLabels: { enabled: false },
      xaxis: {
        categories: trendCategories,
        axisBorder: { show: false },
        axisTicks: { show: false }
      },
      grid: {
        borderColor: 'rgba(255,255,255,0.05)',
        strokeDashArray: 4
      },
      tooltip: {
        theme: 'dark',
        custom: ({ series, seriesIndex, dataPointIndex }) => {
          const val = series[seriesIndex][dataPointIndex] || 0;
          const pt = rawTrend[dataPointIndex] || { transacciones: val > 0 ? 1 : 0 };
          const txCount = pt.transacciones ?? (val > 0 ? 1 : 0);
          return `<div class="p-3 bg-slate-900 border border-slate-700 text-xs font-sans rounded-xl shadow-2xl">
            <div class="font-bold text-emerald-400 mb-1">$ ${val.toLocaleString('en-US', {minimumFractionDigits:2, maximumFractionDigits:2})} USD</div>
            <div class="text-slate-300">Transacciones: <span class="font-bold text-white">${txCount}</span></div>
          </div>`;
        }
      }
    };

    // 2. Gráfica de Compras e Inventario (Vertical Bar Chart)
    const purchasesList = data?.comprasPorCategoria || [];
    const purchaseCategories = purchasesList.length > 0 ? purchasesList.map(c => c.categoria) : ['Insumos', 'Reactivos', 'Quirúrgico', 'Medicamentos'];
    const purchaseData = purchasesList.length > 0 ? purchasesList.map(c => c.monto) : [0, 0, 0, 0];

    this.purchasesChartOptions = {
      series: [{
        name: "Órdenes de Compra ($ USD)",
        data: purchaseData
      }],
      chart: {
        type: "bar",
        height: 240,
        toolbar: { show: false },
        background: 'transparent',
        foreColor: '#94a3b8'
      },
      plotOptions: {
        bar: {
          columnWidth: '45%',
          borderRadius: 8,
          distributed: true
        }
      },
      colors: ['#3b82f6', '#06b6d4', '#8b5cf6', '#ec4899'], // Blue, Cyan, Purple, Pink
      dataLabels: { enabled: false },
      xaxis: {
        categories: purchaseCategories,
        axisBorder: { show: false },
        axisTicks: { show: false }
      },
      legend: { show: false },
      grid: {
        borderColor: 'rgba(255,255,255,0.05)',
        strokeDashArray: 4
      },
      tooltip: { theme: 'dark' }
    };

    // 3. Gráfica por Origen (Donut Chart)
    const originsList = data?.desglosePorOrigen || [];
    const originLabels = originsList.length > 0 ? originsList.map(o => o.origen) : ['Particular', 'Seguro', 'Emergencia', 'Hospitalización', 'UCI', 'Quirófano'];
    const originSeries = originsList.length > 0 ? originsList.map(o => o.cantidad) : [0, 0, 0, 0, 0, 0];
    
    // Anti-zero donut fallback to prevent chart render failure when all values are 0
    const hasAnyValue = originSeries.some(v => v > 0);
    const safeSeries = hasAnyValue ? originSeries : [1, 0, 0, 0, 0, 0];

    this.originChartOptions = {
      series: safeSeries,
      chart: {
        type: "donut",
        height: 220,
        background: 'transparent',
        foreColor: '#94a3b8'
      },
      labels: originLabels,
      colors: ['#38bdf8', '#818cf8', '#f43f5e', '#a855f7', '#eab308', '#10b981'],
      legend: { position: 'bottom', fontSize: '10px' },
      stroke: { show: false },
      plotOptions: {
        pie: {
          donut: {
            size: '72%',
            labels: {
              show: true,
              total: {
                show: true,
                label: 'TOTAL',
                color: '#f8fafc',
                formatter: () => (hasAnyValue ? originSeries.reduce((a, b) => a + b, 0).toString() : '0')
              }
            }
          }
        }
      },
      tooltip: { theme: 'dark' }
    };
  }

  toggleRecaudacion() {
    if (this.animatingRecaudacion()) return;
    this.animatingRecaudacion.set(true);
    setTimeout(() => {
      this.displayUsdRecaudacion.update(curr => !curr);
    }, 300);
    setTimeout(() => {
      this.animatingRecaudacion.set(false);
    }, 600);
  }

  toggleSaldo() {
    if (this.animatingSaldo()) return;
    this.animatingSaldo.set(true);
    setTimeout(() => {
      this.displayUsdSaldo.update(curr => !curr);
    }, 300);
    setTimeout(() => {
      this.animatingSaldo.set(false);
    }, 600);
  }

  generarCierreTurno() {
    const user = this.authService.currentUser();
    if (!user) {
      this.errorMessage.set("Debe iniciar sesión para generar el cierre.");
      return;
    }

    this.isLoading.set(true);
    this.cajaService.getPersonalReport(user.username).subscribe({
      next: (report: DailyClosingReport) => {
        this.isLoading.set(false);
        this.closingReport.set(report);
        this.showClosingModal.set(true);
      },
      error: () => {
        this.isLoading.set(false);
        this.errorMessage.set("No se pudo generar el reporte de cierre.");
      }
    });
  }

  exportarExcel() {
    const user = this.authService.currentUser();
    if (!user) return;

    this.isLoading.set(true);
    const dateStr = new Date().toISOString().split('T')[0];
    
    this.cajaService.exportExcelCashClosing(user.username, dateStr, false).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `Cierre_Caja_${user.username}_${dateStr}.xlsx`;
        a.click();
        window.URL.revokeObjectURL(url);
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
        this.errorMessage.set("Error al exportar Excel");
      }
    });
  }

  ngOnDestroy(): void {
    if (this.signalRService) {
      this.signalRService.stopConnection();
    }
    if (this.tasaSubscription) {
      this.tasaSubscription.unsubscribe();
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

  getAuditBadgeClass(modulo: string): string {
    switch (modulo?.toLowerCase()) {
      case 'facturación': return 'bg-emerald-500/10 text-emerald-400 border-emerald-500/30';
      case 'orden lab': return 'bg-cyan-500/10 text-cyan-400 border-cyan-500/30';
      case 'inventario': return 'bg-purple-500/10 text-purple-400 border-purple-500/30';
      case 'alta paciente': return 'bg-amber-500/10 text-amber-400 border-amber-500/30';
      default: return 'bg-blue-500/10 text-blue-400 border-blue-500/30';
    }
  }
}

