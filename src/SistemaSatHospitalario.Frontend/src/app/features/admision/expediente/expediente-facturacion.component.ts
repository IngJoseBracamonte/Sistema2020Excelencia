import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { 
  LucideAngularModule, 
  FileText, 
  Search, 
  RefreshCcw, 
  Check, 
  X, 
  ChevronRight, 
  Info,
  DollarSign,
  User,
  ShieldCheck
} from 'lucide-angular';
import { ExpedienteService, ExpedienteFacturacionRow } from '../../../core/services/expediente.service';

@Component({
  selector: 'app-expediente-facturacion',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule, RouterLink, CurrencyPipe],
  templateUrl: './expediente-facturacion.component.html'
})
export class ExpedienteFacturacionComponent implements OnInit {
  readonly icons = {
    FileText,
    Search,
    RefreshCcw,
    Check,
    X,
    ChevronRight,
    Info,
    DollarSign,
    User,
    ShieldCheck
  };

  private expedienteService = inject(ExpedienteService);

  // Signals
  public records = signal<ExpedienteFacturacionRow[]>([]);
  public searchTerm = signal<string>('');
  public startDate = signal<string>(new Date().toISOString().split('T')[0]);
  public endDate = signal<string>(new Date().toISOString().split('T')[0]);
  public isLoading = signal<boolean>(false);

  ngOnInit() {
    this.refresh();
  }

  refresh() {
    this.isLoading.set(true);
    this.expedienteService.getBillingReport(this.startDate(), this.endDate(), this.searchTerm()).subscribe({
      next: (res) => {
        this.records.set(res);
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
      }
    });
  }
}
