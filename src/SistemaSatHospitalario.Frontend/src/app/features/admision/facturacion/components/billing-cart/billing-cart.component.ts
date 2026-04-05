import { Component, EventEmitter, Input, Output, inject, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LucideAngularModule } from 'lucide-angular';
import { BillingFacadeService } from '../../../../../core/services/billing-facade.service';
import { CurrencyBsPipe } from '../../../../../shared/pipes/currency-bs.pipe';

/**
 * BillingCartComponent (Pachón Pro V7.0 - SOLID SRP)
 * Encapsula el resumen financiero, el listado de servicios cargados y la acción final de cobro.
 * Centraliza los cálculos de totales delegando al Facade.
 */
@Component({
  selector: 'app-billing-cart',
  standalone: true,
  imports: [CommonModule, LucideAngularModule, CurrencyBsPipe],
  templateUrl: './billing-cart.component.html'
})
export class BillingCartComponent {
  public billingFacade = inject(BillingFacadeService);

  // --- Inputs de Contexto ---
  @Input() isAdmin = false;
  @Input() pacienteSeleccionado = false;
  @Input() currentStep = 1;
  @Input() isLoading = false;

  // --- Outputs de Acción ---
  @Output() verCitas = new EventEmitter<void>();
  @Output() procesar = new EventEmitter<void>();
  @Output() omitir = new EventEmitter<void>();
  @Output() quitar = new EventEmitter<number>();

  // Selectores del Facade
  public serviciosCargados = this.billingFacade.serviciosCargados;
  public totalCargadoBS = this.billingFacade.totalCargadoBS;
  public totalCargadoUSD = this.billingFacade.totalCargadoUSD;
  public totalFacturadoUSD = this.billingFacade.totalFacturadoUSD;
  public tasaCambio = this.billingFacade.tasaCambioDia;
  public cuentaId = this.billingFacade.cuentaId;

  // Cálculos Derivados para UI
  public totalFacturadoBS = computed(() => this.totalFacturadoUSD() * this.tasaCambio());
  
  public getHoraRango(hora: string): string {
    if (!hora) return '';
    const [h, m] = hora.split(':').map(Number);
    const endH = m >= 30 ? (h + 1) % 24 : h;
    const endM = m >= 30 ? '00' : '30';
    return `${hora} - ${String(endH).padStart(2, '0')}:${endM}`;
  }

  // Renamed calculatePriceBs to getFormattedBs as per instruction
  public getFormattedBs(s: any): number {
     const tasa = this.tasaCambio();
     const usd = s.precioUsd ?? s.PrecioUsd ?? 0;
     // Si hay USD, priorizar conversión. Si no, usar Bs base.
     return usd > 0 ? (usd * tasa) : (s.precioBs ?? s.PrecioBs ?? s.precio ?? 0);
  }

  public getDisplayPriceUsd(s: any): string {
    return (s.precioUsd || s.PrecioUsd || 0).toFixed(2);
  }
}
