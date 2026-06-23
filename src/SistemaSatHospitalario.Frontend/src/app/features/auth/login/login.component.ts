import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { LoginRedirectService } from '../../../core/services/login-redirect.service';
import { CommonModule } from '@angular/common';
import {
  LucideAngularModule,
  User,
  Lock,
  AlertCircle,
  RefreshCcw,
  Eye,
  EyeOff
} from 'lucide-angular';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, LucideAngularModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  readonly icons = {
    User,
    Lock,
    AlertCircle,
    RefreshCcw,
    Eye,
    EyeOff
  };
  
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router);
  private redirectService = inject(LoginRedirectService);

  loginForm = this.fb.group({
    username: ['', [Validators.required]],
    password: ['', Validators.required]
  });

  isLoading = signal(false);
  showPassword = signal(false);
  errorMessage = signal('');

  togglePasswordVisibility() {
    this.showPassword.update(v => !v);
  }

  onSubmit() {
    if (this.loginForm.invalid) return;

    this.isLoading.set(true);
    this.errorMessage.set('');

    this.authService.login({
      username: this.loginForm.value.username!,
      password: this.loginForm.value.password!
    }).subscribe({
      next: (res: any) => {
        this.isLoading.set(false);
        const targetRoute = this.redirectService.redirectRoute(res.requirePasswordReset);
        this.router.navigate(targetRoute);
      },
      error: (err: any) => {
        this.isLoading.set(false);
        this.errorMessage.set(err.message || 'Error de Autenticación');
      }
    });
  }

  requestPasswordReset() {
    const username = this.loginForm.value.username;
    if (!username) {
      this.errorMessage.set('Por favor, ingrese su nombre de usuario primero.');
      return;
    }

    this.isLoading.set(true);
    this.authService.requestPasswordReset(username).subscribe({
      next: (res) => {
        this.isLoading.set(false);
        this.errorMessage.set('Solicitud enviada. Contacte al Administrador.');
      },
      error: (err) => {
        this.isLoading.set(false);
        this.errorMessage.set(err.error?.message || 'Error al enviar solicitud.');
      }
    });
  }
}
