import { Injectable } from '@angular/core';

export interface ReceiptPrintData {
  id: string;
  numeroRecibo: string;
  fechaEmision: string;
  pacienteNombre: string;
  pacienteCedula: string;
  tipoIngreso: string;
  totalUSD: number;
  totalBS: number;
  tasaBcv: number;
  detalles: Array<{ descripcion: string; cantidad: number; precioUnitario: number; subtotal: number }>;
  pagos: Array<{ metodoPago: string; montoOriginal: number; equivalenteBase: number; referencia: string }>;
}

@Injectable({
  providedIn: 'root'
})
export class PrintService {
  print(content: string, title: string = 'Recibo de Pago') {
    const printWindow = window.open('', '_blank');
    if (!printWindow) return;

    printWindow.document.write(`
      <html>
        <head>
          <title>${title}</title>
          <style>
            @import url('https://fonts.googleapis.com/css2?family=Inter:wght@400;700;900&display=swap');
            body { font-family: 'Inter', sans-serif; padding: 40px; color: #1e293b; max-width: 800px; margin: 0 auto; background: white; }
            .header { border-bottom: 2px solid #e2e8f0; padding-bottom: 20px; margin-bottom: 30px; display: flex; justify-content: space-between; align-items: flex-end; }
            .logo { font-size: 24px; font-weight: 900; letter-spacing: -1px; text-transform: uppercase; color: #2563eb; }
            .receipt-info { text-align: right; }
            .receipt-info p { margin: 0; font-size: 12px; color: #64748b; font-weight: 700; text-transform: uppercase; }
            .receipt-info h1 { margin: 4px 0 0; font-size: 20px; font-weight: 900; }
            .patient-box { background: #f8fafc; padding: 20px; border-radius: 12px; margin-bottom: 30px; display: grid; grid-template-columns: 1fr 1fr; gap: 20px; }
            .field label { display: block; font-size: 9px; font-weight: 900; color: #94a3b8; text-transform: uppercase; letter-spacing: 1px; }
            .field span { font-weight: 700; font-size: 13px; }
            table { width: 100%; border-collapse: collapse; margin-bottom: 40px; }
            th { text-align: left; font-size: 10px; font-weight: 900; text-transform: uppercase; color: #64748b; padding-bottom: 12px; border-bottom: 1px solid #e2e8f0; }
            td { padding: 12px 0; border-bottom: 1px dashed #f1f5f9; font-size: 12px; font-weight: 500; }
            .text-right { text-align: right; }
            .totals { margin-left: auto; width: 300px; }
            .totals-row { display: flex; justify-content: space-between; padding: 8px 0; }
            .grand-total { border-top: 2px solid #1e293b; margin-top: 15px; padding-top: 15px; font-weight: 900; font-size: 18px; }
            .footer { margin-top: 100px; text-align: center; border-top: 1px solid #e2e8f0; padding-top: 20px; color: #94a3b8; font-size: 10px; font-weight: 500; }
            @media print { body { padding: 20px; } .no-print { display: none; } }
          </style>
        </head>
        <body onload="window.print(); window.close();">
          ${content}
        </body>
      </html>
    `);
    printWindow.document.close();
  }

  generateReceiptHtml(d: ReceiptPrintData): string {
    return `
      <div class="header">
        <div class="logo">SAT HOSPITALARIO</div>
        <div class="receipt-info">
          <p>Comprobante de Pago</p>
          <h1># ${d.numeroRecibo}</h1>
          <p>${new Date(d.fechaEmision).toLocaleString()}</p>
        </div>
      </div>
      <div class="patient-box">
        <div class="field"><label>Paciente</label><span>${d.pacienteNombre}</span></div>
        <div class="field"><label>Cédula</label><span>${d.pacienteCedula}</span></div>
        <div class="field"><label>Tipo Ingreso</label><span>${d.tipoIngreso}</span></div>
        <div class="field"><label>Tasa BCV</label><span>${d.tasaBcv} Bs/USD</span></div>
      </div>
      <table>
        <thead>
          <tr>
            <th>Descripción</th>
            <th>Cant</th>
            <th class="text-right">Precio unit ($)</th>
            <th class="text-right">Subtotal ($)</th>
          </tr>
        </thead>
        <tbody>
          ${d.detalles.map(it => `
            <tr>
              <td>${it.descripcion}</td>
              <td>${it.cantidad}</td>
              <td class="text-right">$${it.precioUnitario.toFixed(2)}</td>
              <td class="text-right">$${it.subtotal.toFixed(2)}</td>
            </tr>
          `).join('')}
        </tbody>
      </table>
      <div class="totals">
        <div class="totals-row"><span>Monto Total USD</span><span>$${d.totalUSD.toFixed(2)}</span></div>
        <div class="totals-row"><span>Monto Total BS</span><span>Bs ${d.totalBS.toFixed(2)}</span></div>
        <div class="totals-row grand-total"><span>Total Recibido</span><span>$${d.totalUSD.toFixed(2)}</span></div>
      </div>
      <div class="footer">
        Este documento es un comprobante de transacción interna. <br>
        Gracias por confiar en SAT Hospitalario.
      </div>
    `;
  }

  generateHistoryHtml(patient: any, history: any[]): string {
    const totalGeneral = history.reduce((acc, h) => acc + h.total, 0);
    
    // Aplanamos todos los servicios de todas las cuentas en una sola lista
    const allServices = history.flatMap(h => h.servicios.map((s: any) => ({
      ...s,
      fecha: new Date(h.fechaCreacion).toLocaleDateString(),
      tipoIngreso: h.tipoIngreso
    })));

    return `
      <div class="header">
        <div class="logo">SAT HOSPITALARIO</div>
        <div class="receipt-info">
          <p>Estado de Cuenta Consolidado</p>
          <h1>FACTURA RESUMEN</h1>
          <p>${new Date().toLocaleString()}</p>
        </div>
      </div>
      
      <div class="patient-box">
        <div class="field"><label>Paciente</label><span>${patient.nombre} ${patient.apellidos || ''}</span></div>
        <div class="field"><label>Cédula</label><span>${patient.cedula}</span></div>
        <div class="field"><label>Fecha Reporte</label><span>${new Date().toLocaleDateString()}</span></div>
        <div class="field"><label>ID Cliente</label><span>${patient.id.substring(0,8).toUpperCase()}</span></div>
      </div>

      <table style="margin-top: 20px;">
        <thead>
          <tr>
            <th style="width: 80px;">Fecha</th>
            <th>Descripción del Servicio</th>
            <th style="width: 100px;">Categoría</th>
            <th class="text-right" style="width: 40px;">Cant</th>
            <th class="text-right" style="width: 100px;">Precio ($)</th>
            <th class="text-right" style="width: 100px;">Subtotal ($)</th>
          </tr>
        </thead>
        <tbody>
          ${allServices.map(s => `
            <tr>
              <td style="font-size: 10px; color: #64748b;">${s.fecha}</td>
              <td style="font-weight: 700;">${s.descripcion}</td>
              <td style="font-size: 9px; text-transform: uppercase; color: #94a3b8;">${s.tipoIngreso}</td>
              <td class="text-right">${s.cantidad}</td>
              <td class="text-right">$${s.precio.toFixed(2)}</td>
              <td class="text-right">$${(s.precio * s.cantidad).toFixed(2)}</td>
            </tr>
          `).join('')}
        </tbody>
      </table>

      <div class="totals" style="margin-top: 40px; border-top: 2px solid #e2e8f0; padding-top: 20px;">
        <div class="totals-row" style="font-size: 14px; color: #64748b; font-weight: 700;">
          <span>Subtotal Servicios</span>
          <span>$${totalGeneral.toFixed(2)}</span>
        </div>
        <div class="totals-row" style="font-size: 14px; color: #64748b; font-weight: 700; margin-top: 5px;">
          <span>Impuestos / Otros</span>
          <span>$0.00</span>
        </div>
        <div class="totals-row grand-total" style="background: #1e293b; color: white; padding: 15px 20px; border-radius: 8px; margin-top: 20px;">
          <span style="font-size: 12px; text-transform: uppercase; letter-spacing: 1px;">Total a Pagar USD</span>
          <span style="font-size: 24px;">$${totalGeneral.toFixed(2)}</span>
        </div>
      </div>

      <div class="footer" style="margin-top: 60px;">
        Este documento es un resumen administrativo y no sustituye a la factura legal definitiva. <br>
        <strong>SAT HOSPITALARIO - Excelencia en Gestión Médica</strong>
      </div>
    `;
  }
}
