import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MedicoService, DoctorHonorariaDto } from '../../../../../core/services/medico.service';
import {
    LucideAngularModule,
    FileText,
    TrendingUp,
    DollarSign,
    Users,
    ArrowLeft,
    RefreshCcw,
    ShieldCheck
} from 'lucide-angular';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-honoraria-report',
  standalone: true,
  imports: [CommonModule, LucideAngularModule, RouterModule],
  templateUrl: './honoraria-report.component.html'
})
export class HonorariaReportComponent implements OnInit {
  private medicoService = inject(MedicoService);
  
  public reportData = signal<DoctorHonorariaDto[]>([]);
  public isLoading = signal<boolean>(false);
  public lastUpdate = signal<Date>(new Date());

  readonly icons = {
    Report: FileText,
    Trend: TrendingUp,
    Dollar: DollarSign,
    Users: Users,
    Back: ArrowLeft,
    Refresh: RefreshCcw,
    Shield: ShieldCheck
  };

  ngOnInit() {
    this.loadReport();
  }

  loadReport() {
    this.isLoading.set(true);
    this.medicoService.getHonorariaReport().subscribe({
      next: (res) => {
        this.reportData.set(res);
        this.isLoading.set(false);
        this.lastUpdate.set(new Date());
      },
      error: () => this.isLoading.set(false)
    });
  }

  getTotals() {
      const data = this.reportData();
      return {
          medicosCount: data.length,
          totalConsultas: data.reduce((acc, curr) => acc + curr.totalConsultasMes, 0),
          promedioHonorario: data.length > 0 ? data.reduce((acc, curr) => acc + curr.honorarioBase, 0) / data.length : 0
      };
  }
}
