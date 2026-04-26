import { Component, EventEmitter, Input, Output, inject, computed, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LucideAngularModule } from 'lucide-angular';
import { FormsModule } from '@angular/forms';
import { BillingFacadeService } from '../../../../../core/services/billing-facade.service';
import { CurrencyBsPipe } from '../../../../../shared/pipes/currency-bs.pipe';

/**
 * BillingCartComponent (Pachón Pro V7.0 - SOLID SRP)
 * Encapsula el resumen financiero, el listado de servicios cargados y la acción final de cobro.
 * Centraliza los cálculos de totales delegando al Facade.
 */
import { FacturacionService } from '../../../../../core/services/facturacion.service';

@Component({
  selector: 'app-billing-cart',
  standalone: true,
  imports: [CommonModule, LucideAngularModule, CurrencyBsPipe, FormsModule],
  templateUrl: './billing-cart.component.html'
})
export class BillingCartComponent {
  public billingFacade = inject(BillingFacadeService);
  private facturacionService = inject(FacturacionService);

  // --- Inputs de Contexto ---
  @Input() isAdmin = false;
  @Input() pacienteSeleccionado = false;
  @Input() patientData: any = null;
  @Input() currentStep = 1;
  @Input() isLoading = false;

  // --- Outputs de Acción ---
  @Output() verCitas = new EventEmitter<void>();
  @Output() procesar = new EventEmitter<void>();
  @Output() omitir = new EventEmitter<void>();
  @Output() quitar = new EventEmitter<{ index: number, isBackend: boolean }>();
  @Output() editarPrecio = new EventEmitter<{ index: number, isBackend: boolean }>();
  @Output() precioCambiado = new EventEmitter<{ index: number, isBackend: boolean, newPrice: number, newHonorary: number }>();

  // Selectores del Facade
  public serviciosCargados = this.billingFacade.serviciosCargados;
  public totalCargadoBS = this.billingFacade.totalCargadoBS;
  public totalCargadoUSD = this.billingFacade.totalCargadoUSD;
  public totalFacturadoUSD = this.billingFacade.totalFacturadoUSD;
  public tasaCambio = this.billingFacade.tasaCambioDia;
  public cuentaId = this.billingFacade.cuentaId;

  // --- Estado de Edición Inline ---
  public editingIndex = signal<number | null>(null);
  public isEditingBackend = signal<boolean>(false);
  public tempPrice = signal<number>(0);
  public tempHonorary = signal<number>(0);

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

  // --- Handlers de Edición Inline (Premium UX V1.0) ---
  public startEdit(index: number, isBackend: boolean, currentPrice: number, currentHonorary: number = 0) {
    this.editingIndex.set(index);
    this.isEditingBackend.set(isBackend);
    this.tempPrice.set(currentPrice);
    this.tempHonorary.set(currentHonorary);
  }

  public confirmEdit() {
    const index = this.editingIndex();
    if (index !== null) {
      this.precioCambiado.emit({
        index,
        isBackend: this.isEditingBackend(),
        newPrice: this.tempPrice(),
        newHonorary: this.tempHonorary()
      });
    }
    this.cancelEdit();
  }

  public cancelEdit() {
    this.editingIndex.set(null);
    this.tempPrice.set(0);
    this.tempHonorary.set(0);
  }

  // --- Lógica de Garantías (V12.5 - No Tech Debt) ---
  public isGuarantee(s: any): boolean {
    const desc = (s.descripcion || s.Descripcion || '').toUpperCase();
    return desc.includes('GARANTIA');
  }

  public imprimirGarantia(s: any) {
    if (!this.patientData) {
      alert('Debe seleccionar un paciente primero.');
      return;
    }

    const price = s.precioUsd ?? s.PrecioUsd ?? s.precio ?? 0;
    
    const dto = {
      nombreResponsable: this.patientData.nombre + ' ' + this.patientData.apellidos,
      relacionResponsable: 'Titular',
      cedulaResponsable: this.patientData.cedula,
      direccionResponsable: 'No especificada',
      telefonoResponsable: this.patientData.celular || 'No especificado',
      conceptos: (s.descripcion || s.Descripcion || 'GARANTIA DE PAGO'),
      nombrePaciente: this.patientData.nombre + ' ' + this.patientData.apellidos,
      edadPaciente: 0,
      cedulaPaciente: this.patientData.cedula,
      montoTotal: price,
      diasLiquidar: 30,
      cuotas: 1,
      fechaCompromiso: new Date().toISOString().split('T')[0],
      fechaVencimiento: new Date(new Date().setDate(new Date().getDate() + 30)).toISOString()
    };

    this.facturacionService.generarGarantiaPdf(dto).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        window.open(url, '_blank');
      },
      error: (err) => {
        console.error('Error al generar PDF de garantía:', err);
        alert('No se pudo generar el documento de garantía.');
      }
    });
  }
}
