import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { LucideAngularModule, ShieldCheck, CheckCircle, XCircle, Clock, Search, RefreshCw } from 'lucide-angular';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'app-password-reset-requests',
  standalone: true,
  imports: [CommonModule, LucideAngularModule],
  templateUrl: './password-reset-requests.component.html'
})
export class PasswordResetRequestsComponent implements OnInit {
  private http = inject(HttpClient);
  
  readonly icons = { ShieldCheck, CheckCircle, XCircle, Clock, Search, RefreshCw };
  
  requests = signal<any[]>([]);
  loading = signal(false);

  ngOnInit() {
    this.loadRequests();
  }

  loadRequests() {
    this.loading.set(true);
    this.http.get<any[]>(`${environment.apiUrl}/api/Auth/pending-resets`)
      .subscribe({
        next: (data) => {
          this.requests.set(data);
          this.loading.set(false);
        },
        error: () => this.loading.set(false)
      });
  }

  approve(requestId: string) {
    this.http.post(`${environment.apiUrl}/api/Auth/approve-reset`, { requestId })
      .subscribe({
        next: () => {
          // Remover de la lista local
          this.requests.set(this.requests().filter(r => r.id !== requestId));
        },
        error: (err) => alert(err.error?.message || 'Error al aprobar')
      });
  }
}
