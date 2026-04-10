import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { 
  LucideAngularModule, 
  DollarSign, 
  Search, 
  RefreshCcw, 
  Check, 
  X, 
  ChevronRight, 
  AlertCircle,
  Info,
  BarChart3,
  Eye
} from 'lucide-angular';
import { ReceivablesService, PendingAR } from '../../../core/services/receivables.service';

@Component({
  selector: 'app-auditing',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule, RouterLink],
  templateUrl: './auditing.component.html',
  styleUrl: './auditing.component.css'
})
export class AuditingComponent implements OnInit {
  readonly icons = {
    DollarSign,
    Search,
    RefreshCcw,
    Check,
    X,
    ChevronRight,
    AlertCircle,
    Info,
    Reportes: BarChart3,
    Eye
  };

  private arService = inject(ReceivablesService);

  // Signals
  public receivables = signal<PendingAR[]>([]);
  public searchTerm = signal<string>('');
  public filterAudit = signal<string>('Pendiente'); // Pendiente (isAudited=false), Procesada (isAudited=true)
  public startDate = signal<string>(new Date().toISOString().split('T')[0]);
  public endDate = signal<string>(new Date().toISOString().split('T')[0]);
  public isLoading = signal<boolean>(false);
  public actionMessage = signal<string | null>(null);

  // Accordion Logic
  public expandedRows = signal<Set<string>>(new Set<string>());

  ngOnInit() {
    this.refresh();
  }

  refresh() {
    this.isLoading.set(true);
    // Nota: El backend aún no tiene el filtro de isAudited en el QueryHandler, por lo que filtraremos en el cliente por ahora.
    // Opcional: En una futura fase se puede mover el filtro al backend.
    this.arService.getPending(this.searchTerm(), 'Todas', this.startDate(), this.endDate()).subscribe({
      next: (res: PendingAR[]) => {
        const isAuditedTarget = this.filterAudit() === 'Procesada';
        this.receivables.set(res.filter(x => x.isAudited === isAuditedTarget));
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
      }
    });
  }

  toggleRow(id: string) {
    const next = new Set(this.expandedRows());
    if (next.has(id)) next.delete(id);
    else next.add(id);
    this.expandedRows.set(next);
  }

  isExpanded(id: string): boolean {
    return this.expandedRows().has(id);
  }

  procesarAuditoria(ar: PendingAR) {
    if (confirm(`¿Marcar como auditada la cuenta de ${ar.pacienteNombre}?`)) {
      this.isLoading.set(true);
      this.arService.audit(ar.id).subscribe({
        next: () => {
          this.actionMessage.set(`Cuenta de ${ar.pacienteNombre} auditada correctamente.`);
          this.refresh();
          setTimeout(() => this.actionMessage.set(null), 5000);
        },
        error: () => {
          this.isLoading.set(false);
        }
      });
    }
  }

  onFilterChange(newVal: any) {
    this.filterAudit.set(newVal);
    this.refresh();
  }
}
