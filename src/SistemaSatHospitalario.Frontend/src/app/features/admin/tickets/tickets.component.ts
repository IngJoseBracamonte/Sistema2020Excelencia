import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TicketService } from '../../../core/services/ticket.service';
import { TicketError } from '../../../core/models/ticket.model';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../../core/services/auth.service';
import { 
    LucideAngularModule, 
    AlertTriangle, 
    Search, 
    RefreshCw, 
    Eye, 
    CheckCircle, 
    MessageSquare,
    Terminal,
    X,
    Clipboard,
    HardDrive
} from 'lucide-angular';

@Component({
  selector: 'app-admin-tickets',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule],
  templateUrl: './tickets.component.html'
})
export class AdminTicketsComponent implements OnInit {
  private ticketService = inject(TicketService);
  private authService = inject(AuthService);

  tickets = signal<TicketError[]>([]);
  loading = signal<boolean>(true);
  resolving = signal<boolean>(false);
  filterResueltos = signal<boolean | undefined>(undefined);
  
  // Modal State
  selectedTicket = signal<TicketError | null>(null);
  comentarios = signal<string>('');

  readonly icons = {
    AlertTriangle,
    Search,
    RefreshCw,
    Eye,
    CheckCircle,
    MessageSquare,
    Terminal,
    X,
    Clipboard,
    HardDrive
  };

  ngOnInit() {
    this.loadTickets();
  }

  loadTickets() {
    this.loading.set(true);
    this.ticketService.getTickets(this.filterResueltos()).subscribe({
      next: (data) => {
        this.tickets.set(data);
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Error fetching tickets', err);
        this.loading.set(false);
      }
    });
  }

  onFilterChange(event: any) {
    const value = event.target.value;
    if (value === 'todos') this.filterResueltos.set(undefined);
    else if (value === 'pendientes') this.filterResueltos.set(false);
    else if (value === 'resueltos') this.filterResueltos.set(true);

    this.loadTickets();
  }

  viewDetails(ticket: TicketError) {
    this.selectedTicket.set(ticket);
    this.comentarios.set(ticket.comentariosResolucion || '');
  }

  closeModal() {
    this.selectedTicket.set(null);
  }

  resolveTicket() {
    const ticket = this.selectedTicket();
    if (!ticket) return;

    this.resolving.set(true);
    this.ticketService.resolveTicket(ticket.id, { comentariosResolucion: this.comentarios() }).subscribe({
      next: (res) => {
        this.resolving.set(false);
        this.closeModal();
        this.loadTickets(); // Refresh list
      },
      error: (err) => {
        console.error('Error resolving ticket', err);
        this.resolving.set(false);
      }
    });
  }

  isAdmin = this.authService.isAdmin;
}
