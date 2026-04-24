import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { LucideAngularModule, Lock, ShieldCheck, Save, AlertCircle } from 'lucide-angular';
import { environment } from '../../../../environments/environment';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-reset-password',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule],
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.css']
})
export class ResetPasswordComponent {
  private http = inject(HttpClient);
  private router = inject(Router);
  private auth = inject(AuthService);

  readonly icons = { Lock, ShieldCheck, Save, AlertCircle };
  
  newPassword = signal('');
  confirmPassword = signal('');
  loading = signal(false);
  error = signal('');
  success = signal(false);

  onSubmit() {
    if (this.newPassword() !== this.confirmPassword()) {
      this.error.set('Las contraseñas no coinciden.');
      return;
    }

    if (this.newPassword().length < 8) {
      this.error.set('La contraseña debe tener al menos 8 caracteres.');
      return;
    }

    this.loading.set(true);
    this.error.set('');

    const payload = {
      newPassword: this.newPassword(),
      confirmPassword: this.confirmPassword()
    };

    this.http.post(`${environment.apiUrl}/api/Auth/complete-reset`, payload)
      .subscribe({
        next: () => {
          this.success.set(true);
          this.loading.set(false);
          // Actualizar estado en el auth service para quitar restricción
          const user = this.auth.currentUser();
          if (user) {
            this.auth.currentUser.set({ ...user, requirePasswordReset: false });
          }
          setTimeout(() => this.router.navigate(['/dashboard']), 2000);
        },
        error: (err) => {
          this.error.set(err.error?.message || 'Error al actualizar la contraseña.');
          this.loading.set(false);
        }
      });
  }
}
