import { Component, Input, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LucideAngularModule, Search, Printer, RefreshCcw, PackageCheck, FileText, X, CheckCircle, UserCheck } from 'lucide-angular';
import { InventoryService } from '../../../../core/services/inventory.service';
import { MultiSedeService, AreaClinica } from '../../../../core/services/multi-sede.service';

export interface StockSedeItem {
  insumoId: string;
  codigo: string;
  nombre: string;
  unidadMedidaBase: string;
  stockActual: number;
  stockMinimo?: number;
  stockMaximo?: number;
  reactivosCombinados?: string;
}

@Component({
  selector: 'app-stock-local-area',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule],
  templateUrl: './stock-local-area.component.html',
  styleUrls: ['./stock-local-area.component.css']
})
export class StockLocalAreaComponent implements OnInit {
  @Input() set areaNombre(val: string) {
    const norm = (val || 'EMERGENCIA').toUpperCase().trim();
    this.areaNombreSignal.set(norm);

    let resolvedId = '';
    if (norm.includes('EMERGENCIA')) {
      resolvedId = '10000000-0000-0000-0000-000000000002';
    } else if (norm.includes('HOSPITALIZACION') || norm.includes('HOSPITALARIA')) {
      resolvedId = '10000000-0000-0000-0000-000000000003';
    } else if (norm.includes('UCI')) {
      resolvedId = '10000000-0000-0000-0000-000000000004';
    }

    if (resolvedId && resolvedId !== this.sedeIdSignal()) {
      this.sedeIdSignal.set(resolvedId);
      this.cargarStock();
    }
  }

  @Input() set sedeId(val: string) {
    if (val && val !== this.sedeIdSignal()) {
      this.sedeIdSignal.set(val);
      this.cargarStock();
    }
  }

  private inventoryService = inject(InventoryService);
  private multiSedeService = inject(MultiSedeService);

  readonly icons = {
    Search,
    Printer,
    RefreshCcw,
    PackageCheck,
    FileText,
    X,
    CheckCircle,
    UserCheck
  };

  public areaNombreSignal = signal<string>('EMERGENCIA');
  public sedeIdSignal = signal<string>('');
  public stockItems = signal<StockSedeItem[]>([]);
  public isLoading = signal<boolean>(false);
  public searchTerm = signal<string>('');

  // Formulario Acta de Entrega de Turno
  public showActaModal = signal<boolean>(false);
  public usuarioEntregaNombre = signal<string>('');
  public usuarioEntregaCedula = signal<string>('');
  public usuarioRecibeNombre = signal<string>('');
  public usuarioRecibeCedula = signal<string>('');
  public fechaHoraActa = signal<string>('');

  public filteredStock = computed(() => {
    const term = this.searchTerm().trim().toLowerCase();
    const items = this.stockItems();
    if (!term) return items;
    return items.filter(i => 
      (i.nombre && i.nombre.toLowerCase().includes(term)) ||
      (i.codigo && i.codigo.toLowerCase().includes(term))
    );
  });

  ngOnInit(): void {
    this.cargarStock();
  }

  public cargarStock(): void {
    let targetSedeId = this.sedeIdSignal();

    if (!targetSedeId) {
      this.multiSedeService.getSedes().subscribe(sedes => {
        const principal = sedes.find(s => s.esPrincipal) || sedes[0];
        if (principal) {
          this.sedeIdSignal.set(principal.id);
          this.cargarStock();
        }
      });
      return;
    }

    this.isLoading.set(true);
    this.inventoryService.getStockSedeById(targetSedeId).subscribe({
      next: (data) => {
        this.stockItems.set(data || []);
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
      }
    });
  }

  public abrirModalActa(): void {
    const now = new Date();
    const fechaFormateada = now.toLocaleString('es-VE', { 
      day: '2-digit', 
      month: 'long', 
      year: 'numeric', 
      hour: '2-digit', 
      minute: '2-digit',
      hour12: true 
    });
    this.fechaHoraActa.set(fechaFormateada);
    this.showActaModal.set(true);
  }

  public imprimirActa(): void {
    const area = (this.areaNombreSignal() || 'EMERGENCIA').toUpperCase();
    const fecha = this.fechaHoraActa() || new Date().toLocaleString('es-VE');
    const entregaNombre = this.usuarioEntregaNombre() || '[NOMBRE ENFERMERA/O SALIENTE]';
    const entregaCedula = this.usuarioEntregaCedula() || 'V-XXXXXXXX';
    const recibeNombre = this.usuarioRecibeNombre() || '[NOMBRE ENFERMERA/O ENTRANTE]';
    const recibeCedula = this.usuarioRecibeCedula() || 'V-XXXXXXXX';

    const itemsHtml = this.stockItems().map(item => `
      <tr>
        <td style="font-family: monospace; font-weight: bold; border: 1px solid #000; padding: 5px 8px;">${item.codigo || ''}</td>
        <td style="border: 1px solid #000; padding: 5px 8px;">${item.nombre || ''}</td>
        <td style="border: 1px solid #000; padding: 5px 8px;">${item.unidadMedidaBase || 'UNIDAD'}</td>
        <td style="text-align: right; font-family: monospace; font-weight: bold; border: 1px solid #000; padding: 5px 8px;">${item.stockActual}</td>
      </tr>
    `).join('');

    const htmlContent = `
      <!DOCTYPE html>
      <html>
      <head>
        <title>Acta de Entrega - Recepción de Turno (${area})</title>
        <style>
          @page { size: A4 portrait; margin: 12mm 15mm 15mm 15mm; }
          body { font-family: Arial, Helvetica, sans-serif; font-size: 11pt; color: #000; background: #fff; margin: 0; padding: 20px; }
          .header { text-align: center; border-bottom: 2px solid #000; padding-bottom: 8px; margin-bottom: 12px; }
          .institution { font-size: 9pt; font-weight: bold; letter-spacing: 1px; color: #444; }
          .area { font-size: 13pt; font-weight: bold; margin-top: 2px; text-transform: uppercase; }
          .title { font-size: 11pt; font-weight: bold; margin-top: 4px; text-transform: uppercase; }
          .date { font-size: 9pt; font-style: italic; color: #333; margin-top: 4px; }
          .body-text { font-size: 10pt; text-align: justify; margin-bottom: 14px; line-height: 1.5; }
          table { width: 100%; border-collapse: collapse; margin-bottom: 20px; font-size: 9.5pt; }
          th { background-color: #f0f0f0; border: 1px solid #000; padding: 6px 8px; text-align: left; font-size: 9pt; text-transform: uppercase; }
          td { border: 1px solid #000; padding: 5px 8px; }
          .signatures { display: flex; justify-content: space-between; align-items: flex-end; margin-top: 50px; page-break-inside: avoid; }
          .sig-box { width: 45%; text-align: center; }
          .sig-line { border-top: 1.5px solid #000; width: 80%; margin: 0 auto 6px auto; }
          .sig-role { font-size: 9pt; font-weight: bold; text-transform: uppercase; }
          .sig-name { font-size: 10pt; font-weight: bold; margin-top: 2px; }
          .sig-detail { font-size: 8.5pt; color: #444; }
        </style>
      </head>
      <body>
        <div class="header">
          <div class="institution">SISTEMA SAT HOSPITALARIO</div>
          <div class="area">ÁREA DE ${area}</div>
          <div class="title">ACTA DE ENTREGA – RECEPCIÓN DE INVENTARIO DE TURNO</div>
          <div class="date">Siendo las ${fecha}, se suscribe la presente acta oficial.</div>
        </div>

        <div class="body-text">
          Reunidos en las instalaciones del Área de <strong>${area}</strong>, hace entrega de la custodia del inventario de insumos y medicamentos la/el C. <strong>${entregaNombre}</strong> [C.I. <strong>${entregaCedula}</strong>], haciendo recepción formal la/el C. <strong>${recibeNombre}</strong> [C.I. <strong>${recibeCedula}</strong>], procediendo a la verificación física de las existencias que a continuación se detallan:
        </div>

        <table>
          <thead>
            <tr>
              <th style="width: 15%;">CLAVE / CÓDIGO</th>
              <th style="width: 50%;">NOMBRE DEL MATERIAL / INSUMO</th>
              <th style="width: 20%;">PRESENTACIÓN</th>
              <th style="width: 15%; text-align: right;">CANTIDAD</th>
            </tr>
          </thead>
          <tbody>
            ${itemsHtml || '<tr><td colspan="4" style="text-align:center;">No hay insumos en el stock local.</td></tr>'}
          </tbody>
        </table>

        <div class="signatures">
          <div class="sig-box">
            <div class="sig-line"></div>
            <div class="sig-role">ENTREGADO POR (SALIENTE)</div>
            <div class="sig-name">${entregaNombre}</div>
            <div class="sig-detail">C.I.: ${entregaCedula}</div>
            <div class="sig-detail">Cargo: Enfermera/o Saliente</div>
          </div>

          <div class="sig-box">
            <div class="sig-line"></div>
            <div class="sig-role">RECIBIDO POR (ENTRANTE)</div>
            <div class="sig-name">${recibeNombre}</div>
            <div class="sig-detail">C.I.: ${recibeCedula}</div>
            <div class="sig-detail">Cargo: Enfermera/o Entrante</div>
          </div>
        </div>
      </body>
      </html>
    `;

    const printWin = window.open('', '_blank');
    if (printWin) {
      printWin.document.write(htmlContent);
      printWin.document.close();
      printWin.focus();
      setTimeout(() => {
        printWin.print();
        printWin.close();
      }, 300);
    }
  }
}
