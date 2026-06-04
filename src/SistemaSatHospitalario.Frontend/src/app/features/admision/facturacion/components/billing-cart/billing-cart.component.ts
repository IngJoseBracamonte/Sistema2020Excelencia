import { Component, EventEmitter, Input, Output, inject, computed, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LucideAngularModule, User, Edit3, Check, X, ShieldCheck, Slash, Trash2, RefreshCcw, ShieldCheck as ShieldCheckIcon } from 'lucide-angular';
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
  @Input() isInsuranceFlow = false;
  @Input() isAssistantOnly = false;

  // --- Outputs de Acción ---
  @Output() verCitas = new EventEmitter<void>();
  @Output() procesar = new EventEmitter<void>();
  @Output() omitir = new EventEmitter<boolean>();
  @Output() quitar = new EventEmitter<{ index: number, isBackend: boolean }>();
  @Output() editarPrecio = new EventEmitter<{ index: number, isBackend: boolean }>();
  @Output() precioCambiado = new EventEmitter<{ index: number, isBackend: boolean, newPrice: number, newHonorary: number }>();
  @Output() guardarCargos = new EventEmitter<void>();

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

  public isConsultationItem(s: any): boolean {
    if (s.isConsultation !== undefined) {
      if (typeof s.isConsultation === 'function') {
        return s.isConsultation();
      }
      return !!s.isConsultation;
    }
    if (s.categoryId === 1 || s.CategoryId === 1) return true;
    const tipo = s.tipo || s.Tipo;
    if (!tipo) return false;
    const t = tipo.toUpperCase();
    const prefixes = ['CONS', 'MEDI', 'MÉDI', 'OBST', 'GINE'];
    return prefixes.some((p: string) => t.includes(p));
  }

  public getItemBasePriceUsd(s: any): number {
    const total = s.precioUsd ?? s.PrecioUsd ?? 0;
    if (this.isConsultationItem(s)) {
      const honorary = s.honorarioUsd ?? s.HonorarioUsd ?? 0;
      return total - honorary;
    }
    return total;
  }

  public getItemHonoraryUsd(s: any): number {
    return s.honorarioUsd ?? s.HonorarioUsd ?? 0;
  }

  // --- Desglose de Totales para Consultas y Honorarios ---
  public totalBaseConsultasUSD = computed(() => {
    return this.serviciosCargados().reduce((acc: number, s: any) => {
      if (this.isConsultationItem(s)) {
        return acc + this.getItemBasePriceUsd(s);
      }
      return acc;
    }, 0);
  });

  public totalHonorariosMedicosUSD = computed(() => {
    return this.serviciosCargados().reduce((acc: number, s: any) => {
      if (this.isConsultationItem(s)) {
        return acc + this.getItemHonoraryUsd(s);
      }
      return acc;
    }, 0);
  });

  public totalOtrosServiciosUSD = computed(() => {
    return this.serviciosCargados().reduce((acc: number, s: any) => {
      if (!this.isConsultationItem(s)) {
        return acc + (s.precioUsd ?? s.PrecioUsd ?? 0);
      }
      return acc;
    }, 0);
  });

  public totalBaseConsultasBS = computed(() => this.totalBaseConsultasUSD() * this.tasaCambio());
  public totalHonorariosMedicosBS = computed(() => this.totalHonorariosMedicosUSD() * this.tasaCambio());
  public totalOtrosServiciosBS = computed(() => this.totalOtrosServiciosUSD() * this.tasaCambio());
  
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

    const p: any = this.patientData;
    const nombre = p.nombre || p.Nombre || '';
    const apellidos = p.apellidos || p.Apellidos || '';
    const cedula = p.cedula || p.Cedula || '';
    const celular = p.celular || p.Celular || '';
    const telefono = p.telefono || p.Telefono || '';
    const direccion = p.direccion || p.Direccion || 'No especificada';

    const price = s.precioUsd ?? s.PrecioUsd ?? s.precio ?? 0;
    
    const dto = {
      nombreResponsable: `${nombre} ${apellidos}`.trim(),
      relacionResponsable: 'Titular',
      cedulaResponsable: cedula,
      direccionResponsable: direccion,
      telefonoResponsable: celular || telefono || 'No especificado',
      conceptos: (s.descripcion || s.Descripcion || 'GARANTIA DE PAGO'),
      nombrePaciente: `${nombre} ${apellidos}`.trim(),
      edadPaciente: 0,
      cedulaPaciente: cedula,
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

  readonly icons = { User, Edit3, Check, X, ShieldCheck, Slash, Trash2, RefreshCcw };
}
