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

  public metodosDisponibles = [
    'Efectivo Dólar ($)', 
    'Zelle', 
    'USDT (Binance)', 
    'Punto Dólares', 
    'Efectivo BS', 
    'Pago Móvil', 
    'Transferencia', 
    'Punto de Venta Bs'
  ];

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

  /**
   * Manejador para entrada manual (cuando la directiva ATM está desactivada para USD)
   */
  public onManualInput(event: any) {
    const val = event.target.value.replace(',', '.');
    const numeric = parseFloat(val);
    if (!isNaN(numeric)) {
      this.currentPago.montoAbonadoMoneda = numeric;
      // No actualizamos displayMonto aquí para evitar saltos en el cursor mientras el usuario escribe
    } else {
      this.currentPago.montoAbonadoMoneda = 0;
    }
  }

  public isBs(): boolean {
    const m = (this.currentPago.metodoPago || '').toLowerCase();
    return m.includes('bs') || m.includes('móvil') || m.includes('punto de venta');
  }

  public agregarPago() {
    if (this.currentPago.montoAbonadoMoneda <= 0) return;

    const montoMoneda = this.currentPago.montoAbonadoMoneda;
    const isUSD = !this.isBs();
    
    this.billingFacade.addPago({
      metodoPago: this.currentPago.metodoPago,
      referenciaBancaria: this.currentPago.referenciaBancaria,
      montoAbonadoMoneda: montoMoneda,
      // Normalización a USD base ($)
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
