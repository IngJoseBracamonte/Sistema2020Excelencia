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
            <th class="text-right">Precio Unidad ($)</th>
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

  generateDischargeReportHtml(patient: any, account: any, triages: any[], destino: string, personalRelevo: string, enfermeroEntrega: string): string {
    const triageEvents = (triages || []).map((t: any) => ({
      time: new Date(t.fechaRegistro || t.FechaRegistro),
      type: 'triage',
      data: t
    }));
    
    const serviceEvents = (account.detalles || []).map((d: any) => ({
      time: new Date(d.fechaCarga || d.FechaCarga),
      type: 'service',
      data: d
    }));
    
    const timeline = [...triageEvents, ...serviceEvents].sort((a, b) => a.time.getTime() - b.time.getTime());
    
    const formatTime = (date: Date) => {
      const hh = String(date.getHours()).padStart(2, '0');
      const mm = String(date.getMinutes()).padStart(2, '0');
      return `${hh}:${mm}`;
    };

    const destinations = ['Alta Médica', 'Hospitalización (Piso)', 'Quirófano', 'UCI', 'Traslado Externo', 'Alta Voluntaria'];

    return `
      <div style="border: 2px solid #0f172a; padding: 25px; border-radius: 12px; font-family: 'Inter', sans-serif; color: #0f172a; max-width: 800px; margin: 0 auto; box-sizing: border-box;">
        <div style="display: flex; justify-content: space-between; align-items: center; border-bottom: 3px double #0f172a; padding-bottom: 12px; margin-bottom: 20px;">
          <div>
            <div style="font-size: 20px; font-weight: 900; letter-spacing: -0.5px; text-transform: uppercase;">SAT HOSPITALARIO</div>
            <div style="font-size: 8px; font-weight: 700; color: #64748b; letter-spacing: 0.5px; text-transform: uppercase; margin-top: 2px;">Dirección de Atención Médica de Emergencia</div>
          </div>
          <div style="text-align: right;">
            <h1 style="font-size: 15px; font-weight: 900; margin: 0; color: #e11d48; text-transform: uppercase; letter-spacing: 0.5px;">Reporte de Egreso de Urgencias</h1>
            <p style="font-size: 9px; font-weight: 700; color: #64748b; margin: 3px 0 0; text-transform: uppercase;">Cuenta: ${account.cuentaId.substring(0, 8).toUpperCase()}</p>
          </div>
        </div>

        <!-- Ficha de Admisión -->
        <div style="display: grid; grid-template-columns: 2fr 1fr 1fr; gap: 15px; background: #f8fafc; padding: 15px; border-radius: 8px; border: 1px solid #e2e8f0; font-size: 11px; margin-bottom: 20px; font-weight: 500;">
          <div><label style="display: block; font-size: 8px; font-weight: 900; color: #64748b; text-transform: uppercase; margin-bottom: 2px;">Paciente</label><span style="font-weight: 700; font-size: 12px;">${account.pacienteNombre}</span></div>
          <div><label style="display: block; font-size: 8px; font-weight: 900; color: #64748b; text-transform: uppercase; margin-bottom: 2px;">Cédula / ID</label><span style="font-weight: 700; font-family: monospace; font-size: 12px;">${account.pacienteCedula}</span></div>
          <div><label style="display: block; font-size: 8px; font-weight: 900; color: #64748b; text-transform: uppercase; margin-bottom: 2px;">Convenio / Seguro</label><span style="font-weight: 700; font-size: 11px; color: #2563eb;">${account.seguroNombre || 'PARTICULAR'}</span></div>
          
          <div><label style="display: block; font-size: 8px; font-weight: 900; color: #64748b; text-transform: uppercase; margin-bottom: 2px;">Diagnóstico de Egreso</label><span style="font-weight: 700; text-transform: uppercase; font-size: 11px;">${patient.diagnostico || 'CONTROL MÉDICO / EVALUACION'}</span></div>
          <div><label style="display: block; font-size: 8px; font-weight: 900; color: #64748b; text-transform: uppercase; margin-bottom: 2px;">Fecha/Hora Ingreso</label><span>${new Date(account.fechaCarga).toLocaleString()}</span></div>
          <div><label style="display: block; font-size: 8px; font-weight: 900; color: #64748b; text-transform: uppercase; margin-bottom: 2px;">Fecha/Hora Egreso</label><span>${new Date().toLocaleString()}</span></div>
        </div>

        <!-- Sección 4: Procedimientos y Evolución -->
        <h3 style="font-size: 11px; font-weight: 900; text-transform: uppercase; color: #0f172a; border-bottom: 2px solid #0f172a; padding-bottom: 4px; margin-bottom: 12px; margin-top: 25px; letter-spacing: 0.5px;">
          4. PROCEDIMIENTOS, ADMINISTRACIÓN DE TRATAMIENTOS Y EVOLUCIÓN CRONOLÓGICA
        </h3>

        <table style="width: 100%; border-collapse: collapse; font-size: 10px; margin-bottom: 20px; border: 1px solid #cbd5e1;">
          <thead>
            <tr style="background: #f1f5f9;">
              <th style="padding: 10px 8px; text-align: left; width: 10%; border: 1px solid #cbd5e1; font-weight: 900; text-transform: uppercase; font-size: 9px;">Hora</th>
              <th style="padding: 10px 8px; text-align: left; width: 25%; border: 1px solid #cbd5e1; font-weight: 900; text-transform: uppercase; font-size: 9px;">S. Vitales</th>
              <th style="padding: 10px 8px; text-align: left; width: 50%; border: 1px solid #cbd5e1; font-weight: 900; text-transform: uppercase; font-size: 9px;">Notas de Enfermería, Procedimientos y Medicamentos Aplicados</th>
              <th style="padding: 10px 8px; text-align: center; width: 15%; border: 1px solid #cbd5e1; font-weight: 900; text-transform: uppercase; font-size: 9px;">Firma / Sello</th>
            </tr>
          </thead>
          <tbody>
            ${timeline.length === 0 ? `
              <tr>
                <td colspan="4" style="padding: 20px; text-align: center; color: #64748b; font-style: italic; border: 1px solid #e2e8f0; font-size: 11px;">
                  No se registran procedimientos ni valoraciones de triage para esta cuenta.
                </td>
              </tr>
            ` : timeline.map(event => {
              if (event.type === 'triage') {
                const t = event.data;
                const vitals = [];
                if (t.tensionArterial || t.TensionArterial) vitals.push(`TA: ${t.tensionArterial || t.TensionArterial}`);
                if (t.frecuenciaCardiaca || t.FrecuenciaCardiaca) vitals.push(`FC: ${t.frecuenciaCardiaca || t.FrecuenciaCardiaca} lpm`);
                if (t.frecuenciaRespiratoria || t.FrecuenciaRespiratoria) vitals.push(`FR: ${t.frecuenciaRespiratoria || t.FrecuenciaRespiratoria} rpm`);
                if (t.temperatura || t.Temperatura) vitals.push(`T°: ${t.temperatura || t.Temperatura} °C`);
                if (t.saturacionO2 || t.SaturacionO2) vitals.push(`Sat: ${t.saturacionO2 || t.SaturacionO2} %`);
                if (t.glicemiaCapilar || t.GlicemiaCapilar) vitals.push(`Glic: ${t.glicemiaCapilar || t.GlicemiaCapilar} mg/dL`);
                
                const notes = [];
                if (t.motivoConsulta || t.MotivoConsulta) notes.push(`<strong>Motivo de Consulta:</strong> ${t.motivoConsulta || t.MotivoConsulta}`);
                
                const physical = [];
                if (t.estadoConciencia || t.EstadoConciencia) physical.push(`Conciencia: ${t.estadoConciencia || t.EstadoConciencia}`);
                if (t.viaAerea || t.ViaAerea) physical.push(`Vía Aérea: ${t.viaAerea || t.ViaAerea}`);
                if (t.ventilacion || t.Ventilacion) physical.push(`Ventilación: ${t.ventilacion || t.Ventilacion}`);
                if (t.pulso || t.Pulso) physical.push(`Pulso: ${t.pulso || t.Pulso}`);
                if (t.pielMucosas || t.PielMucosas) physical.push(`Piel: ${t.pielMucosas || t.PielMucosas}`);
                if (t.llenadoCapilar || t.LlenadoCapilar) physical.push(`Llenado Capilar: ${t.llenadoCapilar || t.LlenadoCapilar}`);
                if (t.pupilas || t.Pupilas) physical.push(`Pupilas: ${t.pupilas || t.Pupilas}`);
                
                if (physical.length > 0) {
                  notes.push(`<strong>Valoración Física:</strong> ${physical.join(', ')}`);
                }
                
                if (t.alergias || t.Alergias) notes.push(`<strong>Alergias:</strong> ${t.alergias || t.Alergias}`);
                if (t.antecedentesMedicos || t.AntecedentesMedicos) notes.push(`<strong>Antecedentes Médicos:</strong> ${t.antecedentesMedicos || t.AntecedentesMedicos}`);

                return `
                  <tr style="border-bottom: 1px solid #cbd5e1;">
                    <td style="padding: 8px 6px; font-weight: 700; border: 1px solid #cbd5e1; text-align: center; vertical-align: top;">${formatTime(event.time)}</td>
                    <td style="padding: 8px 6px; font-family: monospace; line-height: 1.4; border: 1px solid #cbd5e1; vertical-align: top; font-size: 9px;">
                      ${vitals.join('<br>')}
                    </td>
                    <td style="padding: 8px 6px; border: 1px solid #cbd5e1; line-height: 1.5; vertical-align: top; font-size: 10px;">
                      <div style="font-weight: 900; color: #2563eb; font-size: 8px; text-transform: uppercase; margin-bottom: 4px; letter-spacing: 0.5px;">[VALORACIÓN CLÍNICA Y TRIAGE]</div>
                      ${notes.join('<br>')}
                    </td>
                    <td style="border: 1px solid #cbd5e1; vertical-align: bottom; text-align: center; font-size: 8px; color: #94a3b8; padding-bottom: 4px;">Firma de Guardia</td>
                  </tr>
                `;
              } else {
                const s = event.data;
                return `
                  <tr style="border-bottom: 1px solid #cbd5e1;">
                    <td style="padding: 8px 6px; font-weight: 700; border: 1px solid #cbd5e1; text-align: center; vertical-align: top;">${formatTime(event.time)}</td>
                    <td style="padding: 8px 6px; text-align: center; color: #94a3b8; font-style: italic; border: 1px solid #cbd5e1; vertical-align: top; font-size: 9px;">--</td>
                    <td style="padding: 8px 6px; border: 1px solid #cbd5e1; line-height: 1.5; vertical-align: top; font-size: 10px;">
                      <div style="font-weight: 900; color: #e11d48; font-size: 8px; text-transform: uppercase; margin-bottom: 4px; letter-spacing: 0.5px;">[PROCEDIMIENTO / TRATAMIENTO APLICADO]</div>
                      <strong style="font-size: 11px;">${s.descripcion}</strong><br>
                      Cantidad: ${s.cantidad} unidades | Registrado por: ${s.usuarioCarga || 'Clínico'}
                    </td>
                    <td style="border: 1px solid #cbd5e1; vertical-align: bottom; text-align: center; font-size: 8px; color: #94a3b8; padding-bottom: 4px;">Firma de Guardia</td>
                  </tr>
                `;
              }
            }).join('')}
          </tbody>
        </table>

        <!-- Sección 5: Condición y Destino Final -->
        <h3 style="font-size: 11px; font-weight: 900; text-transform: uppercase; color: #0f172a; border-bottom: 2px solid #0f172a; padding-bottom: 4px; margin-bottom: 12px; margin-top: 30px; letter-spacing: 0.5px;">
          5. CONDICIÓN Y DESTINO FINAL DE EGRESO DE URGENCIAS
        </h3>
        
        <div style="font-size: 11px; margin-bottom: 25px; border: 1px solid #cbd5e1; padding: 12px; border-radius: 6px;">
          <strong style="text-transform: uppercase; font-size: 9px; color: #64748b;">Destino del Paciente:</strong>
          <div style="display: grid; grid-template-columns: repeat(3, 1fr); gap: 10px; margin-top: 8px; font-weight: 700;">
            ${destinations.map(d => {
              const checked = d.toLowerCase() === destino.toLowerCase() ? '&#9746;' : '&#9744;';
              return `<div><span style="font-size: 14px; margin-right: 5px;">${checked}</span> ${d.toUpperCase()}</div>`;
            }).join('')}
          </div>
        </div>

        <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 40px; margin-top: 40px; font-size: 11px;">
          <div style="border-top: 1px solid #0f172a; padding-top: 12px; text-align: center;">
            <strong style="text-transform: uppercase; font-size: 9px; color: #64748b;">Enfermero(a) que Entrega/Egresa:</strong><br>
            <span style="font-size: 12px; font-weight: 900; color: #2563eb; text-transform: uppercase; display: inline-block; margin-top: 4px;">${enfermeroEntrega}</span><br>
            <span style="font-size: 9px; color: #94a3b8; display: inline-block; margin-top: 2px;">Firma y Sello</span>
          </div>
          <div style="border-top: 1px solid #0f172a; padding-top: 12px; text-align: center;">
            <strong style="text-transform: uppercase; font-size: 9px; color: #64748b;">Personal de Relevo / Piso que Recibe:</strong><br>
            <span style="font-size: 12px; font-weight: 900; color: #0f172a; text-transform: uppercase; display: inline-block; margin-top: 4px;">${personalRelevo || '___________________________'}</span><br>
            <span style="font-size: 9px; color: #94a3b8; display: inline-block; margin-top: 2px;">Firma y Sello</span>
          </div>
        </div>
      </div>
    `;
  }
}

