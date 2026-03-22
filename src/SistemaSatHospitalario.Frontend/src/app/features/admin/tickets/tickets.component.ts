import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TicketService } from '../../../core/services/ticket.service';
import { TicketError } from '../../../core/models/ticket.model';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-admin-tickets',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './tickets.component.html',
  styleUrls: ['./tickets.component.css']
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
    if (value === 'all') this.filterResueltos.set(undefined);
    else if (value === 'pending') this.filterResueltos.set(false);
    else if (value === 'resolved') this.filterResueltos.set(true);

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

  isAdmin(): boolean {
    return this.authService.currentUser()?.role === 'Administrator';
  }
}
