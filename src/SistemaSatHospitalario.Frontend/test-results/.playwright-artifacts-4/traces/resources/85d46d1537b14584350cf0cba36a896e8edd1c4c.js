import{P as m}from"./chunk-P7NNHKRF.js";import{a as f,b}from"./chunk-UVEM5LZV.js";var h=class d{print(i,a="Recibo de Pago"){let r=window.open("","_blank");r&&(r.document.write(`
      <html>
        <head>
          <title>${a}</title>
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
          ${i}
        </body>
      </html>
    `),r.document.close())}generateReceiptHtml(i){return`
      <div class="header">
        <div class="logo">SAT HOSPITALARIO</div>
        <div class="receipt-info">
          <p>Comprobante de Pago</p>
          <h1># ${i.numeroRecibo}</h1>
          <p>${new Date(i.fechaEmision).toLocaleString()}</p>
        </div>
      </div>
      <div class="patient-box">
        <div class="field"><label>Paciente</label><span>${i.pacienteNombre}</span></div>
        <div class="field"><label>C\xE9dula</label><span>${i.pacienteCedula}</span></div>
        <div class="field"><label>Tipo Ingreso</label><span>${i.tipoIngreso}</span></div>
        <div class="field"><label>Tasa BCV</label><span>${i.tasaBcv} Bs/USD</span></div>
      </div>
      <table>
        <thead>
          <tr>
            <th>Descripci\xF3n</th>
            <th>Cant</th>
            <th class="text-right">Precio Unidad ($)</th>
            <th class="text-right">Subtotal ($)</th>
          </tr>
        </thead>
        <tbody>
          ${i.detalles.map(a=>`
            <tr>
              <td>${a.descripcion}</td>
              <td>${a.cantidad}</td>
              <td class="text-right">$${a.precioUnitario.toFixed(2)}</td>
              <td class="text-right">$${a.subtotal.toFixed(2)}</td>
            </tr>
          `).join("")}
        </tbody>
      </table>
      <div class="totals">
        <div class="totals-row"><span>Monto Total USD</span><span>$${i.totalUSD.toFixed(2)}</span></div>
        <div class="totals-row"><span>Monto Total BS</span><span>Bs ${i.totalBS.toFixed(2)}</span></div>
        <div class="totals-row grand-total"><span>Total Recibido</span><span>$${i.totalUSD.toFixed(2)}</span></div>
      </div>
      <div class="footer">
        Este documento es un comprobante de transacci\xF3n interna. <br>
        Gracias por confiar en SAT Hospitalario.
      </div>
    `}generateHistoryHtml(i,a){let r=a.reduce((o,p)=>o+p.total,0),c=a.flatMap(o=>o.servicios.map(p=>b(f({},p),{fecha:new Date(o.fechaCreacion).toLocaleDateString(),tipoIngreso:o.tipoIngreso})));return`
      <div class="header">
        <div class="logo">SAT HOSPITALARIO</div>
        <div class="receipt-info">
          <p>Estado de Cuenta Consolidado</p>
          <h1>FACTURA RESUMEN</h1>
          <p>${new Date().toLocaleString()}</p>
        </div>
      </div>
      
      <div class="patient-box">
        <div class="field"><label>Paciente</label><span>${i.nombre} ${i.apellidos||""}</span></div>
        <div class="field"><label>C\xE9dula</label><span>${i.cedula}</span></div>
        <div class="field"><label>Fecha Reporte</label><span>${new Date().toLocaleDateString()}</span></div>
        <div class="field"><label>ID Cliente</label><span>${i.id.substring(0,8).toUpperCase()}</span></div>
      </div>

      <table style="margin-top: 20px;">
        <thead>
          <tr>
            <th style="width: 80px;">Fecha</th>
            <th>Descripci\xF3n del Servicio</th>
            <th style="width: 100px;">Categor\xEDa</th>
            <th class="text-right" style="width: 40px;">Cant</th>
            <th class="text-right" style="width: 100px;">Precio ($)</th>
            <th class="text-right" style="width: 100px;">Subtotal ($)</th>
          </tr>
        </thead>
        <tbody>
          ${c.map(o=>`
            <tr>
              <td style="font-size: 10px; color: #64748b;">${o.fecha}</td>
              <td style="font-weight: 700;">${o.descripcion}</td>
              <td style="font-size: 9px; text-transform: uppercase; color: #94a3b8;">${o.tipoIngreso}</td>
              <td class="text-right">${o.cantidad}</td>
              <td class="text-right">$${o.precio.toFixed(2)}</td>
              <td class="text-right">$${(o.precio*o.cantidad).toFixed(2)}</td>
            </tr>
          `).join("")}
        </tbody>
      </table>

      <div class="totals" style="margin-top: 40px; border-top: 2px solid #e2e8f0; padding-top: 20px;">
        <div class="totals-row" style="font-size: 14px; color: #64748b; font-weight: 700;">
          <span>Subtotal Servicios</span>
          <span>$${r.toFixed(2)}</span>
        </div>
        <div class="totals-row" style="font-size: 14px; color: #64748b; font-weight: 700; margin-top: 5px;">
          <span>Impuestos / Otros</span>
          <span>$0.00</span>
        </div>
        <div class="totals-row grand-total" style="background: #1e293b; color: white; padding: 15px 20px; border-radius: 8px; margin-top: 20px;">
          <span style="font-size: 12px; text-transform: uppercase; letter-spacing: 1px;">Total a Pagar USD</span>
          <span style="font-size: 24px;">$${r.toFixed(2)}</span>
        </div>
      </div>

      <div class="footer" style="margin-top: 60px;">
        Este documento es un resumen administrativo y no sustituye a la factura legal definitiva. <br>
        <strong>SAT HOSPITALARIO - Excelencia en Gesti\xF3n M\xE9dica</strong>
      </div>
    `}generateDischargeReportHtml(i,a,r,c,o,p){let u=(r||[]).map(e=>({time:new Date(e.fechaRegistro||e.FechaRegistro),type:"triage",data:e})),y=(a.detalles||[]).map(e=>({time:new Date(e.fechaCarga||e.FechaCarga),type:"service",data:e})),g=[...u,...y].sort((e,t)=>e.time.getTime()-t.time.getTime()),x=e=>{let t=String(e.getHours()).padStart(2,"0"),n=String(e.getMinutes()).padStart(2,"0");return`${t}:${n}`},v=["Alta M\xE9dica","Hospitalizaci\xF3n (Piso)","Quir\xF3fano","UCI","Traslado Externo","Alta Voluntaria"];return`
      <div style="border: 2px solid #0f172a; padding: 25px; border-radius: 12px; font-family: 'Inter', sans-serif; color: #0f172a; max-width: 800px; margin: 0 auto; box-sizing: border-box;">
        <div style="display: flex; justify-content: space-between; align-items: center; border-bottom: 3px double #0f172a; padding-bottom: 12px; margin-bottom: 20px;">
          <div>
            <div style="font-size: 20px; font-weight: 900; letter-spacing: -0.5px; text-transform: uppercase;">SAT HOSPITALARIO</div>
            <div style="font-size: 8px; font-weight: 700; color: #64748b; letter-spacing: 0.5px; text-transform: uppercase; margin-top: 2px;">Direcci\xF3n de Atenci\xF3n M\xE9dica de Emergencia</div>
          </div>
          <div style="text-align: right;">
            <h1 style="font-size: 15px; font-weight: 900; margin: 0; color: #e11d48; text-transform: uppercase; letter-spacing: 0.5px;">Reporte de Egreso de Urgencias</h1>
            <p style="font-size: 9px; font-weight: 700; color: #64748b; margin: 3px 0 0; text-transform: uppercase;">Cuenta: ${a.cuentaId.substring(0,8).toUpperCase()}</p>
          </div>
        </div>

        <!-- Ficha de Admisi\xF3n -->
        <div style="display: grid; grid-template-columns: 2fr 1fr 1fr; gap: 15px; background: #f8fafc; padding: 15px; border-radius: 8px; border: 1px solid #e2e8f0; font-size: 11px; margin-bottom: 20px; font-weight: 500;">
          <div><label style="display: block; font-size: 8px; font-weight: 900; color: #64748b; text-transform: uppercase; margin-bottom: 2px;">Paciente</label><span style="font-weight: 700; font-size: 12px;">${a.pacienteNombre}</span></div>
          <div><label style="display: block; font-size: 8px; font-weight: 900; color: #64748b; text-transform: uppercase; margin-bottom: 2px;">C\xE9dula / ID</label><span style="font-weight: 700; font-family: monospace; font-size: 12px;">${a.pacienteCedula}</span></div>
          <div><label style="display: block; font-size: 8px; font-weight: 900; color: #64748b; text-transform: uppercase; margin-bottom: 2px;">Convenio / Seguro</label><span style="font-weight: 700; font-size: 11px; color: #2563eb;">${a.seguroNombre||"PARTICULAR"}</span></div>
          
          <div><label style="display: block; font-size: 8px; font-weight: 900; color: #64748b; text-transform: uppercase; margin-bottom: 2px;">Diagn\xF3stico de Egreso</label><span style="font-weight: 700; text-transform: uppercase; font-size: 11px;">${i.diagnostico||"CONTROL M\xC9DICO / EVALUACION"}</span></div>
          <div><label style="display: block; font-size: 8px; font-weight: 900; color: #64748b; text-transform: uppercase; margin-bottom: 2px;">Fecha/Hora Ingreso</label><span>${new Date(a.fechaCarga).toLocaleString()}</span></div>
          <div><label style="display: block; font-size: 8px; font-weight: 900; color: #64748b; text-transform: uppercase; margin-bottom: 2px;">Fecha/Hora Egreso</label><span>${new Date().toLocaleString()}</span></div>
        </div>

        <!-- Secci\xF3n 4: Procedimientos y Evoluci\xF3n -->
        <h3 style="font-size: 11px; font-weight: 900; text-transform: uppercase; color: #0f172a; border-bottom: 2px solid #0f172a; padding-bottom: 4px; margin-bottom: 12px; margin-top: 25px; letter-spacing: 0.5px;">
          4. PROCEDIMIENTOS, ADMINISTRACI\xD3N DE TRATAMIENTOS Y EVOLUCI\xD3N CRONOL\xD3GICA
        </h3>

        <table style="width: 100%; border-collapse: collapse; font-size: 10px; margin-bottom: 20px; border: 1px solid #cbd5e1;">
          <thead>
            <tr style="background: #f1f5f9;">
              <th style="padding: 10px 8px; text-align: left; width: 10%; border: 1px solid #cbd5e1; font-weight: 900; text-transform: uppercase; font-size: 9px;">Hora</th>
              <th style="padding: 10px 8px; text-align: left; width: 25%; border: 1px solid #cbd5e1; font-weight: 900; text-transform: uppercase; font-size: 9px;">S. Vitales</th>
              <th style="padding: 10px 8px; text-align: left; width: 50%; border: 1px solid #cbd5e1; font-weight: 900; text-transform: uppercase; font-size: 9px;">Notas de Enfermer\xEDa, Procedimientos y Medicamentos Aplicados</th>
              <th style="padding: 10px 8px; text-align: center; width: 15%; border: 1px solid #cbd5e1; font-weight: 900; text-transform: uppercase; font-size: 9px;">Firma / Sello</th>
            </tr>
          </thead>
          <tbody>
            ${g.length===0?`
              <tr>
                <td colspan="4" style="padding: 20px; text-align: center; color: #64748b; font-style: italic; border: 1px solid #e2e8f0; font-size: 11px;">
                  No se registran procedimientos ni valoraciones de triage para esta cuenta.
                </td>
              </tr>
            `:g.map(e=>{if(e.type==="triage"){let t=e.data,n=[];(t.tensionArterial||t.TensionArterial)&&n.push(`TA: ${t.tensionArterial||t.TensionArterial}`),(t.frecuenciaCardiaca||t.FrecuenciaCardiaca)&&n.push(`FC: ${t.frecuenciaCardiaca||t.FrecuenciaCardiaca} lpm`),(t.frecuenciaRespiratoria||t.FrecuenciaRespiratoria)&&n.push(`FR: ${t.frecuenciaRespiratoria||t.FrecuenciaRespiratoria} rpm`),(t.temperatura||t.Temperatura)&&n.push(`T\xB0: ${t.temperatura||t.Temperatura} \xB0C`),(t.saturacionO2||t.SaturacionO2)&&n.push(`Sat: ${t.saturacionO2||t.SaturacionO2} %`),(t.glicemiaCapilar||t.GlicemiaCapilar)&&n.push(`Glic: ${t.glicemiaCapilar||t.GlicemiaCapilar} mg/dL`);let l=[];(t.motivoConsulta||t.MotivoConsulta)&&l.push(`<strong>Motivo de Consulta:</strong> ${t.motivoConsulta||t.MotivoConsulta}`);let s=[];return(t.estadoConciencia||t.EstadoConciencia)&&s.push(`Conciencia: ${t.estadoConciencia||t.EstadoConciencia}`),(t.viaAerea||t.ViaAerea)&&s.push(`V\xEDa A\xE9rea: ${t.viaAerea||t.ViaAerea}`),(t.ventilacion||t.Ventilacion)&&s.push(`Ventilaci\xF3n: ${t.ventilacion||t.Ventilacion}`),(t.pulso||t.Pulso)&&s.push(`Pulso: ${t.pulso||t.Pulso}`),(t.pielMucosas||t.PielMucosas)&&s.push(`Piel: ${t.pielMucosas||t.PielMucosas}`),(t.llenadoCapilar||t.LlenadoCapilar)&&s.push(`Llenado Capilar: ${t.llenadoCapilar||t.LlenadoCapilar}`),(t.pupilas||t.Pupilas)&&s.push(`Pupilas: ${t.pupilas||t.Pupilas}`),s.length>0&&l.push(`<strong>Valoraci\xF3n F\xEDsica:</strong> ${s.join(", ")}`),(t.alergias||t.Alergias)&&l.push(`<strong>Alergias:</strong> ${t.alergias||t.Alergias}`),(t.antecedentesMedicos||t.AntecedentesMedicos)&&l.push(`<strong>Antecedentes M\xE9dicos:</strong> ${t.antecedentesMedicos||t.AntecedentesMedicos}`),`
                  <tr style="border-bottom: 1px solid #cbd5e1;">
                    <td style="padding: 8px 6px; font-weight: 700; border: 1px solid #cbd5e1; text-align: center; vertical-align: top;">${x(e.time)}</td>
                    <td style="padding: 8px 6px; font-family: monospace; line-height: 1.4; border: 1px solid #cbd5e1; vertical-align: top; font-size: 9px;">
                      ${n.join("<br>")}
                    </td>
                    <td style="padding: 8px 6px; border: 1px solid #cbd5e1; line-height: 1.5; vertical-align: top; font-size: 10px;">
                      <div style="font-weight: 900; color: #2563eb; font-size: 8px; text-transform: uppercase; margin-bottom: 4px; letter-spacing: 0.5px;">[VALORACI\xD3N CL\xCDNICA Y TRIAGE]</div>
                      ${l.join("<br>")}
                    </td>
                    <td style="border: 1px solid #cbd5e1; vertical-align: bottom; text-align: center; font-size: 8px; color: #94a3b8; padding-bottom: 4px;">Firma de Guardia</td>
                  </tr>
                `}else{let t=e.data;return`
                  <tr style="border-bottom: 1px solid #cbd5e1;">
                    <td style="padding: 8px 6px; font-weight: 700; border: 1px solid #cbd5e1; text-align: center; vertical-align: top;">${x(e.time)}</td>
                    <td style="padding: 8px 6px; text-align: center; color: #94a3b8; font-style: italic; border: 1px solid #cbd5e1; vertical-align: top; font-size: 9px;">--</td>
                    <td style="padding: 8px 6px; border: 1px solid #cbd5e1; line-height: 1.5; vertical-align: top; font-size: 10px;">
                      <div style="font-weight: 900; color: #e11d48; font-size: 8px; text-transform: uppercase; margin-bottom: 4px; letter-spacing: 0.5px;">[PROCEDIMIENTO / TRATAMIENTO APLICADO]</div>
                      <strong style="font-size: 11px;">${t.descripcion}</strong><br>
                      Cantidad: ${t.cantidad} unidades | Registrado por: ${t.usuarioCarga||"Cl\xEDnico"}
                    </td>
                    <td style="border: 1px solid #cbd5e1; vertical-align: bottom; text-align: center; font-size: 8px; color: #94a3b8; padding-bottom: 4px;">Firma de Guardia</td>
                  </tr>
                `}}).join("")}
          </tbody>
        </table>

        <!-- Secci\xF3n 5: Condici\xF3n y Destino Final -->
        <h3 style="font-size: 11px; font-weight: 900; text-transform: uppercase; color: #0f172a; border-bottom: 2px solid #0f172a; padding-bottom: 4px; margin-bottom: 12px; margin-top: 30px; letter-spacing: 0.5px;">
          5. CONDICI\xD3N Y DESTINO FINAL DE EGRESO DE URGENCIAS
        </h3>
        
        <div style="font-size: 11px; margin-bottom: 25px; border: 1px solid #cbd5e1; padding: 12px; border-radius: 6px;">
          <strong style="text-transform: uppercase; font-size: 9px; color: #64748b;">Destino del Paciente:</strong>
          <div style="display: grid; grid-template-columns: repeat(3, 1fr); gap: 10px; margin-top: 8px; font-weight: 700;">
            ${v.map(e=>`<div><span style="font-size: 14px; margin-right: 5px;">${e.toLowerCase()===c.toLowerCase()?"&#9746;":"&#9744;"}</span> ${e.toUpperCase()}</div>`).join("")}
          </div>
        </div>

        <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 40px; margin-top: 40px; font-size: 11px;">
          <div style="border-top: 1px solid #0f172a; padding-top: 12px; text-align: center;">
            <strong style="text-transform: uppercase; font-size: 9px; color: #64748b;">Enfermero(a) que Entrega/Egresa:</strong><br>
            <span style="font-size: 12px; font-weight: 900; color: #2563eb; text-transform: uppercase; display: inline-block; margin-top: 4px;">${p}</span><br>
            <span style="font-size: 9px; color: #94a3b8; display: inline-block; margin-top: 2px;">Firma y Sello</span>
          </div>
          <div style="border-top: 1px solid #0f172a; padding-top: 12px; text-align: center;">
            <strong style="text-transform: uppercase; font-size: 9px; color: #64748b;">Personal de Relevo / Piso que Recibe:</strong><br>
            <span style="font-size: 12px; font-weight: 900; color: #0f172a; text-transform: uppercase; display: inline-block; margin-top: 4px;">${o||"___________________________"}</span><br>
            <span style="font-size: 9px; color: #94a3b8; display: inline-block; margin-top: 2px;">Firma y Sello</span>
          </div>
        </div>
      </div>
    `}static \u0275fac=function(a){return new(a||d)};static \u0275prov=m({token:d,factory:d.\u0275fac,providedIn:"root"})};export{h as a};
