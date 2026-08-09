import { Component, inject, signal, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DashboardService, BusinessInsights } from '../../../core/services/dashboard.service';
import { NgApexchartsModule, ChartComponent } from 'ng-apexcharts';
import { 
  LucideAngularModule, 
  TrendingUp, 
  Activity, 
  DollarSign, 
  Users, 
  BarChart3,
  PieChart as PieIcon,
  RefreshCcw,
  Calendar
} from 'lucide-angular';

import {
  ApexAxisChartSeries,
  ApexChart,
  ApexXAxis,
  ApexDataLabels,
  ApexTitleSubtitle,
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
  title: ApexTitleSubtitle;
  plotOptions: ApexPlotOptions;
  legend: ApexLegend;
  tooltip: ApexTooltip;
  fill: ApexFill;
  responsive: ApexResponsive[];
  colors: string[];
  labels: string[];
};

@Component({
  selector: 'app-admin-analytics',
  standalone: true,
  imports: [CommonModule, NgApexchartsModule, LucideAngularModule],
  templateUrl: './admin-analytics.component.html'
})
export class AdminAnalyticsComponent implements OnInit {
  private dashboardService = inject(DashboardService);
  
  public insights = signal<BusinessInsights | null>(null);
  public isLoading = signal<boolean>(false);

  // Opciones de Gráficos
  public trendChartOptions: Partial<ChartOptions> = { chart: { type: 'area', height: 300, background: 'transparent' } };
  public specialtyChartOptions: Partial<ChartOptions> = { chart: { type: 'bar', height: 300, background: 'transparent' } };
  public patientMixChartOptions: Partial<ChartOptions> = { chart: { type: 'donut', height: 300, background: 'transparent' } };

  readonly icons = {
    Trend: TrendingUp,
    Activity,
    Dollar: DollarSign,
    Users,
    Charts: BarChart3,
    Pie: PieIcon,
    Refresh: RefreshCcw,
    Calendar
  };

  ngOnInit() {
    this.refresh();
  }

  refresh() {
    this.isLoading.set(true);
    this.dashboardService.getInsights().subscribe({
      next: (data: BusinessInsights) => {
        this.insights.set(data);
        this.initCharts(data);
        this.isLoading.set(false);
      },
      error: (err) => {
        this.isLoading.set(false);
        console.warn('[Analytics] Error al cargar insights:', err?.status || err?.message);
      }
    });
  }

  private initCharts(data: BusinessInsights) {
    const commonTheme = {
      mode: 'dark',
      palette: 'palette1',
      monochrome: {
        enabled: false
      }
    };

    // 1. Gráfico de Tendencia de Ingresos (Líneas)
    const trendData = data.tendenciaIngresos || [];
    this.trendChartOptions = {
      series: [{
        name: "Ingresos (USD)",
        data: trendData.map(t => t.monto)
      }],
      chart: {
        type: "area",
        height: 300,
        toolbar: { show: false },
        zoom: { enabled: false },
        background: 'transparent',
        foreColor: '#94a3b8'
      },
      colors: ['#f43f5e'],
      stroke: { curve: 'smooth', width: 4 },
      fill: {
        type: 'gradient',
        gradient: {
          shadeIntensity: 1,
          opacityFrom: 0.4,
          opacityTo: 0.1,
          stops: [0, 90, 100]
        }
      },
      dataLabels: { enabled: false },
      xaxis: {
        categories: trendData.map(t => t.fecha),
        axisBorder: { show: false },
        axisTicks: { show: false }
      },
      grid: {
        borderColor: '#1e293b',
        strokeDashArray: 4,
        padding: { left: 20, right: 20 }
      },
      tooltip: { theme: 'dark' }
    };

    // 2. Gráfico por Especialidad (Barras Horizontales)
    const specData = data.ventasPorEspecialidad || [];
    this.specialtyChartOptions = {
      series: [{
        name: "Monto Facturado",
        data: specData.map(v => v.monto)
      }],
      chart: {
        type: "bar",
        height: 300,
        toolbar: { show: false },
        background: 'transparent',
        foreColor: '#94a3b8'
      },
      plotOptions: {
        bar: {
          horizontal: true,
          borderRadius: 8,
          barHeight: '60%'
        }
      },
      colors: ['#10b981'],
      dataLabels: { enabled: false },
      xaxis: {
        categories: specData.map(v => v.especialidad),
        axisBorder: { show: false }
      },
      grid: {
        borderColor: '#1e293b',
        strokeDashArray: 4
      },
      tooltip: { theme: 'dark' }
    };

    // 3. Gráfico de Mix de Pacientes (Dona)
    const distData = data.distribucionPacientes || [];
    const mixSeries = distData.map(d => d.valor);
    const hasMixData = mixSeries.some(v => v > 0);
    this.patientMixChartOptions = {
      series: hasMixData ? mixSeries : [1],
      chart: {
        type: "donut",
        height: 300,
        background: 'transparent',
        foreColor: '#94a3b8'
      },
      labels: hasMixData ? distData.map(d => d.etiqueta) : ['Sin datos'],
      colors: ['#38bdf8', '#f59e0b'],
      legend: { position: 'bottom', markers: { strokeWidth: 0 } },
      stroke: { show: false },
      plotOptions: {
        pie: {
          donut: {
            size: '75%',
            labels: {
              show: true,
              total: {
                show: true,
                label: 'TOTAL',
                color: '#f8fafc',
                formatter: () => (data.pacientesAtendidosHoy ?? 0).toString()
              }
            }
          }
        }
      },
      tooltip: { theme: 'dark' }
    };
  }
}
