import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuditService, PriceAuditLog } from '../../../core/services/audit.service';
import { 
  LucideAngularModule, 
  Search, 
  RefreshCcw, 
  ShieldAlert, 
  ArrowUpRight, 
  ArrowDownRight,
  User,
  Calendar,
  DollarSign
} from 'lucide-angular';

@Component({
  selector: 'app-price-audit',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule],
  templateUrl: './price-audit.component.html'
})
export class PriceAuditComponent implements OnInit {
  private auditService = inject(AuditService);

  public logs = signal<PriceAuditLog[]>([]);
  public isLoading = signal<boolean>(false);
  public fechaDesde = signal<string>('');
  public fechaHasta = signal<string>('');

  readonly icons = {
    Search,
    Refresh: RefreshCcw,
    Alert: ShieldAlert,
    Increase: ArrowUpRight,
    Decrease: ArrowDownRight,
    User,
    Calendar,
    Currency: DollarSign
  };

  ngOnInit() {
    this.loadLogs();
  }

  loadLogs() {
    this.isLoading.set(true);
    this.auditService.getPriceAuditLogs(this.fechaDesde(), this.fechaHasta()).subscribe({
      next: (res) => {
        this.logs.set(res);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  getVarianzaClass(varianza: number): string {
    if (varianza > 0) return 'text-rose-500 bg-rose-500/10 border-rose-500/20';
    if (varianza < 0) return 'text-emerald-500 bg-emerald-500/10 border-emerald-500/20';
    return 'text-slate-400 bg-slate-400/10 border-slate-400/20';
  }
}
