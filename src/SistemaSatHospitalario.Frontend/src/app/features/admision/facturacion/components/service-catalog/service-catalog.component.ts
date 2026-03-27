import { Component, EventEmitter, Input, Output, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LucideAngularModule } from 'lucide-angular';
import { BillingFacadeService } from '../../../../../core/services/billing-facade.service';
import { CurrencyBsPipe } from '../../../../../shared/pipes/currency-bs.pipe';
import { CatalogItem } from '../../../../../core/models/priced-item.model';

/**
 * ServiceCatalogComponent (Pachón Pro V7.0 - SOLID SRP)
 * Encapsula la búsqueda de servicios, selección de especialistas y catálogo.
 */
@Component({
  selector: 'app-service-catalog',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule, CurrencyBsPipe],
  templateUrl: './service-catalog.component.html'
})
export class ServiceCatalogComponent {
  public billingFacade = inject(BillingFacadeService);

  // --- Inputs de Control UI ---
  @Input() isRxAssistant = false;
  @Input() suggestedServiceId: string | null = null;
  @Input() selectedSlot: string | null = null;

  // --- Outputs de Orquestación ---
  @Output() next = new EventEmitter<void>();
  @Output() agendar = new EventEmitter<void>();
  @Output() cargar = new EventEmitter<string>();

  // Selectores delegados al Facade (Signals con tipos explícitos)
  public especialidades = this.billingFacade.especialidades;
  public medicosFiltrados = this.billingFacade.medicosFiltrados;
  public serviciosFiltrados = this.billingFacade.serviciosFiltrados;
  public serviciosCargados = this.billingFacade.serviciosCargados;
  public tasaCambioDia = this.billingFacade.tasaCambioDia;

  // Getters/Setters para el Template (Binding Two-Way Manual)
  public get searchTerm(): string { return this.billingFacade.searchTermServicio(); }
  public set searchTerm(val: string) { this.billingFacade.searchTermServicio.set(val); }

  public get selectedEsp(): string { return this.billingFacade.selectedEspecialidad() || ''; }
  public set selectedEsp(val: string) { 
    this.billingFacade.selectedEspecialidad.set(val || null); 
    this.billingFacade.selectedMedicoId.set(null);
  }

  public get selectedMedId(): string { return this.billingFacade.selectedMedicoId() || ''; }
  public set selectedMedId(val: string) { this.billingFacade.selectedMedicoId.set(val || null); }

  public seleccionarTipoConsulta(s: CatalogItem) {
    const desc = s.descripcion.toUpperCase();
    const match = this.especialidades().find((e: string) => desc.includes(e.toUpperCase().substring(0, 4)));
    if (match) this.selectedEsp = match;
  }

  public calculatePriceBs(item: CatalogItem): number {
    return item.getRawBs(this.tasaCambioDia());
  }
}
