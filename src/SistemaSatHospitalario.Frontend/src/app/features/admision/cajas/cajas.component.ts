import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { CajaService } from '../../../core/services/caja.service';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-cajas',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './cajas.component.html',
  styleUrl: './cajas.component.css'
})
export class CajasComponent {
  private cajaService = inject(CajaService);
  public authService = inject(AuthService);
  private fb = inject(FormBuilder);

  // Estados Reactivos
  public isCajaAbierta = this.cajaService.isCajaAbierta;
  public isLoading = signal<boolean>(false);
  public actionMessage = signal<string | null>(null);
  public errorMessage = signal<string | null>(null);

  // Formularios
  public mainCajaForm = this.fb.group({
    montoInicialDivisa: [0, [Validators.min(0)]],
    montoInicialBs: [0, [Validators.min(0)]]
  });

  get isAdministrador(): boolean {
    return this.authService.currentUser()?.role === 'Administrador';
  }

  get cajeroUserId(): string {
    return this.authService.currentUser()?.username || 'unknown';
  }

  abrirCajaMatriz() {
    if (this.mainCajaForm.invalid) return;
    this.isLoading.set(true);
    this.errorMessage.set(null);
    this.actionMessage.set(null);

    const ds = this.mainCajaForm.value;
    this.cajaService.abrirCaja({
      montoInicialDivisa: ds.montoInicialDivisa || 0,
      montoInicialBs: ds.montoInicialBs || 0
    }).subscribe({
      next: (res) => {
        this.actionMessage.set(res.message || 'Caja Matriz Abierta exitosamente.');
        this.isLoading.set(false);
      },
      error: (err) => {
        this.errorMessage.set(err.error?.error || 'Error abriendo la caja matriz.');
        this.isLoading.set(false);
      }
    });
  }

  cerrarCajaMatriz() {
    this.isLoading.set(true);
    this.errorMessage.set(null);
    this.actionMessage.set(null);

    this.cajaService.cerrarCaja().subscribe({
      next: (res) => {
        this.actionMessage.set(res.message || 'Caja Matriz Clausurada exitosamente.');
        this.isLoading.set(false);
      },
      error: (err) => {
        this.errorMessage.set(err.error?.error || 'Error clausurando la caja matriz.');
        this.isLoading.set(false);
      }
    });
  }


}
