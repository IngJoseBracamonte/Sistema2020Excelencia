import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { 
  LucideAngularModule, 
  FileText, 
  Search, 
  RefreshCcw, 
  Check, 
  X, 
  ChevronRight, 
  Info,
  DollarSign,
  User,
  ShieldCheck,
  Calendar
} from 'lucide-angular';
import { ExpedienteService, ExpedienteFacturacionRow } from '../../../core/services/expediente.service';
import { FacturacionService } from '../../../core/services/facturacion.service';

@Component({
  selector: 'app-expediente-facturacion',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule],
  templateUrl: './expediente-facturacion.component.html'
})
export class ExpedienteFacturacionComponent implements OnInit {
  readonly icons = {
    FileText,
    Search,
    RefreshCcw,
    Check,
    X,
    ChevronRight,
    Info,
    DollarSign,
    User,
    ShieldCheck,
    Calendar
  };

  private expedienteService = inject(ExpedienteService);
  private facturacionService = inject(FacturacionService);

  // Signals
  public records = signal<ExpedienteFacturacionRow[]>([]);
  public searchTerm = signal<string>('');
  public startDate = signal<string>(new Date().toLocaleDateString('sv-SE'));
  public endDate = signal<string>(new Date().toLocaleDateString('sv-SE'));
  public filterType = signal<string>('convenio');
  public soloCompromiso = signal<boolean>(false);
  public isLoading = signal<boolean>(false);
  public isGeneratingPdf = signal<boolean>(false);

  ngOnInit() {
    this.refresh();
  }

  refresh() {
    this.isLoading.set(true);
    this.expedienteService.getBillingReport(this.startDate(), this.endDate(), this.searchTerm(), this.filterType(), this.soloCompromiso()).subscribe({
      next: (res) => {
        this.records.set(res);
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
      }
    });
  }

  private calcularEdad(fechaNacimiento?: string): number {
    if (!fechaNacimiento) return 0;
    const today = new Date();
    const birth = new Date(fechaNacimiento);
    let age = today.getFullYear() - birth.getFullYear();
    const m = today.getMonth() - birth.getMonth();
    if (m < 0 || (m === 0 && today.getDate() < birth.getDate())) {
      age--;
    }
    return age;
  }

  reimprimirCompromiso(row: ExpedienteFacturacionRow) {
    if (!row.cuentaPorCobrarId) return;
    if (row.seguroNombre?.toUpperCase().includes('PDVSA')) {
      this.reimprimirConformidad(row);
      return;
    }

    this.isGeneratingPdf.set(true);
    this.facturacionService.getGarantiasItems(row.cuentaPorCobrarId).subscribe({
      next: (items) => {
        const filteredItems = (items || []).filter(i => i.descripcion && i.descripcion.trim() !== '');
        const totalItemsVal = filteredItems.reduce((acc, curr) => acc + (curr.valorEstimado || 0), 0) || 0;
        const descItemsVal = filteredItems.map(i => i.descripcion).join(', ') || '';

        const dto = {
          cuentaPorCobrarId: row.cuentaPorCobrarId,
          nombreResponsable: row.pacienteNombre,
          relacionResponsable: 'Titular',
          cedulaResponsable: row.pacienteCedula,
          direccionResponsable: 'No especificada',
          telefonoResponsable: row.pacienteTelefono,
          conceptos: row.estudio,
          nombrePaciente: row.pacienteNombre,
          edadPaciente: this.calcularEdad(row.fechaNacimiento),
          cedulaPaciente: row.pacienteCedula,
          direccionPaciente: 'No especificada',
          telefonoPaciente: row.pacienteTelefono,
          montoTotal: row.montoUSD,
          diasLiquidar: 21,
          cuotas: 1,
          quienAutorizo: row.quienAutorizo || 'No especificado',
          doctorProcedimiento: row.doctorProcedimiento || 'No especificado',
          informacionAdicional: row.informacionAdicional || '',
          esPagoCompletado: row.metodoPago !== 'CRÉDITO' && row.metodoPago !== 'CREDITO',
          fechaCompromiso: row.fecha,
          fechaVencimiento: new Date(new Date(row.fecha).getTime() + (21 * 24 * 60 * 60 * 1000)).toISOString(),
          anexarGarantia: filteredItems.length > 0,
          garantiasItems: filteredItems,
          montoGarantia: totalItemsVal,
          descripcionGarantia: descItemsVal,
          nombreFiador: '',
          cedulaFiador: '',
          telefonoFiador: '',
          direccionFiador: ''
        };

        this.facturacionService.generarCompromisoPdf(dto).subscribe({
          next: (blob) => {
            const url = window.URL.createObjectURL(blob);
            window.open(url, '_blank');
            this.isGeneratingPdf.set(false);
          },
          error: (err) => {
            console.error(err);
            alert('Error al generar compromiso de pago');
            this.isGeneratingPdf.set(false);
          }
        });
      },
      error: (err) => {
        console.error(err);
        this.isGeneratingPdf.set(false);
        alert('Error al cargar ítems de garantía');
      }
    });
  }

  reimprimirConformidad(row: ExpedienteFacturacionRow) {
    if (!row.cuentaPorCobrarId) return;
    this.isGeneratingPdf.set(true);

    const dto = {
      cuentaPorCobrarId: row.cuentaPorCobrarId,
      nombreResponsable: row.pacienteNombre,
      relacionResponsable: 'Titular',
      cedulaResponsable: row.pacienteCedula,
      direccionResponsable: 'No especificada',
      telefonoResponsable: row.pacienteTelefono,
      conceptos: row.estudio,
      nombrePaciente: row.pacienteNombre,
      edadPaciente: this.calcularEdad(row.fechaNacimiento),
      cedulaPaciente: row.pacienteCedula,
      direccionPaciente: 'No especificada',
      telefonoPaciente: row.pacienteTelefono,
      montoTotal: row.montoUSD,
      diasLiquidar: 21,
      cuotas: 1,
      quienAutorizo: row.quienAutorizo || 'No especificado',
      doctorProcedimiento: row.doctorProcedimiento || 'No especificado',
      informacionAdicional: row.informacionAdicional || '',
      esPagoCompletado: row.metodoPago !== 'CRÉDITO' && row.metodoPago !== 'CREDITO',
      fechaCompromiso: row.fecha,
      fechaVencimiento: new Date(new Date(row.fecha).getTime() + (21 * 24 * 60 * 60 * 1000)).toISOString()
    };

    this.facturacionService.generarConformidadPdf(dto).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        window.open(url, '_blank');
        this.isGeneratingPdf.set(false);
      },
      error: (err) => {
        console.error(err);
        alert('Error al generar conformidad de servicios');
        this.isGeneratingPdf.set(false);
      }
    });
  }

  reimprimirGarantia(row: ExpedienteFacturacionRow) {
    if (!row.cuentaPorCobrarId) return;
    this.isGeneratingPdf.set(true);
    this.facturacionService.getGarantiasItems(row.cuentaPorCobrarId).subscribe({
      next: (items) => {
        const filteredItems = (items || []).filter(i => i.descripcion && i.descripcion.trim() !== '');
        const totalItemsVal = filteredItems.reduce((acc, curr) => acc + (curr.valorEstimado || 0), 0) || 0;
        const descItemsVal = filteredItems.map(i => i.descripcion).join(', ') || '';

        const dto = {
          cuentaPorCobrarId: row.cuentaPorCobrarId,
          nombreResponsable: row.pacienteNombre,
          relacionResponsable: 'Titular',
          cedulaResponsable: row.pacienteCedula,
          direccionResponsable: 'No especificada',
          telefonoResponsable: row.pacienteTelefono,
          conceptos: row.estudio,
          nombrePaciente: row.pacienteNombre,
          edadPaciente: this.calcularEdad(row.fechaNacimiento),
          cedulaPaciente: row.pacienteCedula,
          direccionPaciente: 'No especificada',
          telefonoPaciente: row.pacienteTelefono,
          montoTotal: row.montoUSD,
          diasLiquidar: 21,
          cuotas: 1,
          quienAutorizo: row.quienAutorizo || 'No especificado',
          doctorProcedimiento: row.doctorProcedimiento || 'No especificado',
          informacionAdicional: row.informacionAdicional || '',
          esPagoCompletado: row.metodoPago !== 'CRÉDITO' && row.metodoPago !== 'CREDITO',
          fechaCompromiso: row.fecha,
          fechaVencimiento: new Date(new Date(row.fecha).getTime() + (21 * 24 * 60 * 60 * 1000)).toISOString(),
          garantiasItems: filteredItems,
          montoGarantia: totalItemsVal,
          descripcionGarantia: descItemsVal,
          nombreFiador: '',
          cedulaFiador: '',
          telefonoFiador: '',
          direccionFiador: ''
        };

        this.facturacionService.generarGarantiaPdf(dto).subscribe({
          next: (blob) => {
            const url = window.URL.createObjectURL(blob);
            window.open(url, '_blank');
            this.isGeneratingPdf.set(false);
          },
          error: (err) => {
            console.error(err);
            alert('Error al generar garantía de pago');
            this.isGeneratingPdf.set(false);
          }
        });
      },
      error: (err) => {
        console.error(err);
        this.isGeneratingPdf.set(false);
        alert('Error al cargar ítems de garantía');
      }
    });
  }
}
