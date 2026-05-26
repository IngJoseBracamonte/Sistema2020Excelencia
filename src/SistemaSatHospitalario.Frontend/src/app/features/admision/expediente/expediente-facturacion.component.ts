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
  ShieldCheck,
  Calendar
} from 'lucide-angular';
import { ExpedienteService, ExpedienteFacturacionRow } from '../../../core/services/expediente.service';

@Component({
  selector: 'app-expediente-facturacion',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule],
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
    ShieldCheck,
    Calendar
  };

  private expedienteService = inject(ExpedienteService);

  // Signals
  public records = signal<ExpedienteFacturacionRow[]>([]);
  public searchTerm = signal<string>('');
  public startDate = signal<string>(new Date().toLocaleDateString('sv-SE'));
  public endDate = signal<string>(new Date().toLocaleDateString('sv-SE'));
  public filterType = signal<string>('convenio');
  public isLoading = signal<boolean>(false);

  ngOnInit() {
    this.refresh();
  }

  refresh() {
    this.isLoading.set(true);
    this.expedienteService.getBillingReport(this.startDate(), this.endDate(), this.searchTerm(), this.filterType()).subscribe({
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
