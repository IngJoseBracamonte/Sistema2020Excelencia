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
  public trendChartOptions: Partial<ChartOptions> = {};
  public specialtyChartOptions: Partial<ChartOptions> = {};
  public patientMixChartOptions: Partial<ChartOptions> = {};

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
      error: () => this.isLoading.set(false)
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
    this.trendChartOptions = {
      series: [{
        name: "Ingresos (USD)",
        data: data.tendenciaIngresos.map(t => t.monto)
      }],
      chart: {
        type: "area",
        height: 300,
        toolbar: { show: false },
        zoom: { enabled: false },
        background: 'transparent',
        foreColor: '#94a3b8'
      },
      colors: ['#f43f5e'], // Rose 500
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
        categories: data.tendenciaIngresos.map(t => t.fecha),
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
    this.specialtyChartOptions = {
      series: [{
        name: "Monto Facturado",
        data: data.ventasPorEspecialidad.map(v => v.monto)
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
      colors: ['#10b981'], // Emerald 500
      dataLabels: { enabled: false },
      xaxis: {
        categories: data.ventasPorEspecialidad.map(v => v.especialidad),
        axisBorder: { show: false }
      },
      grid: {
        borderColor: '#1e293b',
        strokeDashArray: 4
      },
      tooltip: { theme: 'dark' }
    };

    // 3. Gráfico de Mix de Pacientes (Dona)
    this.patientMixChartOptions = {
      series: data.distribucionPacientes.map(d => d.valor),
      chart: {
        type: "donut",
        height: 300,
        background: 'transparent',
        foreColor: '#94a3b8'
      },
      labels: data.distribucionPacientes.map(d => d.etiqueta),
      colors: ['#38bdf8', '#f59e0b'], // Sky 400, Amber 500
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
                formatter: () => data.pacientesAtendidosHoy.toString()
              }
            }
          }
        }
      },
      tooltip: { theme: 'dark' }
    };
  }
}
