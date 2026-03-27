import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LucideAngularModule } from 'lucide-angular';
import { BillingFacadeService } from '../../../../../core/services/billing-facade.service';
import { AtmAmountDirective } from '../../../../../shared/directives/atm-amount.directive';

/**
 * PaymentModuleComponent (Pachón Pro V7.0 - SOLID SRP)
 * Encapsula la gestión de abonos, tipos de pago y referencias bancarias.
 */
@Component({
  selector: 'app-payment-module',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule, AtmAmountDirective],
  templateUrl: './payment-module.component.html'
})
export class PaymentModuleComponent {
  public billingFacade = inject(BillingFacadeService);

  // --- Estado Local del Formulario de Pago ---
  public currentPago = {
    metodoPago: 'Punto de Venta Bs',
    referenciaBancaria: '',
    montoAbonadoMoneda: 0
  };
  // Campo de display separado para evitar conflicto con [value] binding de Angular
  public displayMonto = '0,00';

  public metodosDisponibles = ['Punto de Venta Bs', 'Pago Móvil', 'Efectivo Divisas', 'Zelle'];

  // Selectores del Facade
  public pagos = this.billingFacade.pagos;
  public tasaCambio = this.billingFacade.tasaCambioDia;

  // Icons proxy (lucide names)
  public icons = {
    CreditCard: 'credit-card',
    Trash2: 'trash-2'
  };

  public onMontoChanged(newAmount: number) {
    this.currentPago.montoAbonadoMoneda = newAmount;
    this.displayMonto = newAmount.toFixed(2).replace('.', ',');
  }

  public agregarPago() {
    if (this.currentPago.montoAbonadoMoneda <= 0) return;

    const montoMoneda = this.currentPago.montoAbonadoMoneda;
    const isUSD = this.currentPago.metodoPago.includes('Divisas') || this.currentPago.metodoPago.includes('Zelle');
    
    this.billingFacade.addPago({
      metodoPago: this.currentPago.metodoPago,
      referenciaBancaria: this.currentPago.referenciaBancaria,
      montoAbonadoMoneda: montoMoneda,
      equivalenteAbonadoBase: isUSD ? montoMoneda : montoMoneda / this.tasaCambio()
    });

    // Reset Form
    this.currentPago.referenciaBancaria = '';
    this.currentPago.montoAbonadoMoneda = 0;
    this.displayMonto = '0,00';
  }

  public removerPago(index: number) {
    this.billingFacade.removePago(index);
  }
}
