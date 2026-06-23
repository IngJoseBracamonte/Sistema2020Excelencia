using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Interfaces;
using SistemaSatHospitalario.Core.Application.Commands.Admision;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaSatHospitalario.Core.Application.Queries.Admision
{
    public class ExportCashierAuditQuery : IRequest<byte[]>
    {
        public string? UserId { get; set; }
        public DateTime Date { get; set; } = DateTime.Today;
        public bool IsAuditMode { get; set; }
    }

    public class ExportCashierAuditQueryHandler : IRequestHandler<ExportCashierAuditQuery, byte[]>
    {
        private readonly IApplicationDbContext _context;
        private readonly IExcelService _excelService;
        private readonly IIdentityService _identityService;

        public ExportCashierAuditQueryHandler(IApplicationDbContext context, IExcelService excelService, IIdentityService identityService)
        {
            _context = context;
            _excelService = excelService;
            _identityService = identityService;
        }

        public async Task<byte[]> Handle(ExportCashierAuditQuery request, CancellationToken cancellationToken)
        {
            var start = request.Date.Date;
            var end = start.AddDays(1).AddTicks(-1);

            var userMap = (await _identityService.GetUsersAsync()).ToDictionary(u => u.Id.ToString(), u => u.FullName);

            // 1. Obtener todas las cajas para el día seleccionado
            var cajasQuery = _context.CajasDiarias.AsNoTracking()
                .Where(c => c.FechaApertura >= start && c.FechaApertura <= end);

            if (!string.IsNullOrEmpty(request.UserId))
            {
                cajasQuery = cajasQuery.Where(c => c.UsuarioId == request.UserId || c.NombreUsuario == request.UserId);
            }

            var cajas = await cajasQuery.ToListAsync(cancellationToken);

            var catalogoMetodos = await _context.CatalogoMetodosPago.AsNoTracking()
                .Where(m => m.Activo)
                .OrderBy(m => m.Orden)
                .ToListAsync(cancellationToken);

            var cajaIds = cajas.Select(c => c.Id).ToList();
            var recibos = await _context.RecibosFactura.AsNoTracking()
                .Include(r => r.DetallesPago)
                .Include(r => r.CuentaServicio)
                    .ThenInclude(cs => cs.Paciente)
                .Where(r => r.CajaDiariaId.HasValue && cajaIds.Contains(r.CajaDiariaId.Value) && r.EstadoFiscal != EstadoConstants.Anulada)
                .ToListAsync(cancellationToken);

            var receiptsByCaja = recibos.GroupBy(r => r.CajaDiariaId!.Value).ToDictionary(g => g.Key, g => g.ToList());

            var cajerosReport = new List<CajeroReportDto>();
            decimal grandTotalEsperado = 0;
            decimal grandTotalRecaudado = 0;

            foreach (var caja in cajas)
            {
                var userRecibos = receiptsByCaja.TryGetValue(caja.Id, out var rList) ? rList : new List<ReciboFactura>();
                var allPayments = userRecibos.SelectMany(r => r.DetallesPago).ToList();

                var pagosDetallados = new List<PagoDetalladoDto>();
                foreach (var r in userRecibos)
                {
                    decimal totalPagadoRecibo = r.DetallesPago.Sum(dp => dp.EquivalenteAbonadoBase);
                    decimal vueltoRecibo = r.MontoVueltoUSD;
                    decimal pendienteRecibo = Math.Max(0, r.TotalFacturadoUSD - totalPagadoRecibo);

                    foreach (var p in r.DetallesPago.Where(x => x.MontoAbonadoMoneda > 0))
                    {
                        var metodoCatalogObj = catalogoMetodos.FirstOrDefault(m => m.Valor == p.MetodoPago);
                        bool isUSD = metodoCatalogObj?.EsUSD ?? (p.MetodoPago == "Dolar Efectivo" || p.MetodoPago == "Zelle");
                        string vueltoDadoPor = vueltoRecibo > 0 ? (r.UsuarioEmision ?? caja.NombreUsuario ?? "System") : "-";

                        pagosDetallados.Add(new PagoDetalladoDto
                        {
                            Fecha = p.FechaPago,
                            PacienteNombre = r.CuentaServicio.Paciente.NombreCorto,
                            PacienteCedula = r.CuentaServicio.Paciente.CedulaPasaporte,
                            Concepto = $"Recibo: {r.NumeroRecibo}",
                            MetodoPago = p.MetodoPago,
                            Moneda = isUSD ? "$" : "Bs.",
                            MontoMonedaOriginal = p.MontoAbonadoMoneda,
                            EquivalenteUSD = p.EquivalenteAbonadoBase,
                            IngresadoPor = p.UsuarioCarga,
                            VueltoDadoPor = vueltoDadoPor,
                            TotalCuentaUSD = r.TotalFacturadoUSD,
                            PendienteCuentaUSD = pendienteRecibo,
                            VueltoUSD = vueltoRecibo
                        });
                    }
                }

                var desgloseMetodos = new List<DesgloseMetodoDto>();
                decimal totalCajaCobrado = 0;
                decimal totalCajaIngresado = 0;

                List<MetodoDeclaradoDto>? declarados = null;
                if (caja.Estado != EstadoConstants.CajaAbierta && !string.IsNullOrEmpty(caja.DeclaracionCierreJson))
                {
                    try
                    {
                        declarados = JsonSerializer.Deserialize<List<MetodoDeclaradoDto>>(caja.DeclaracionCierreJson);
                    }
                    catch { }
                }

                var metodosPrincipales = catalogoMetodos.Where(m => !m.EsVuelto).ToList();
                foreach (var metodo in metodosPrincipales)
                {
                    string vueltoMetodoValor = string.Empty;
                    if (metodo.Valor == "Dolar Efectivo") vueltoMetodoValor = "Vuelto Efectivo USD";
                    else if (metodo.Valor == "Efectivo BS") vueltoMetodoValor = "Vuelto Efectivo BS";
                    else if (metodo.Valor == "Pago Movil") vueltoMetodoValor = "Vuelto Pago Movil";

                    var pagosMetodo = allPayments.Where(p => p.MetodoPago == metodo.Valor && p.MontoAbonadoMoneda > 0).ToList();
                    decimal esperadoIngresoOriginal = pagosMetodo.Sum(p => p.MontoAbonadoMoneda);
                    decimal esperadoIngresoBase = pagosMetodo.Sum(p => p.EquivalenteAbonadoBase);

                    decimal esperadoVueltosOriginal = 0;
                    decimal esperadoVueltosBase = 0;
                    if (!string.IsNullOrEmpty(vueltoMetodoValor))
                    {
                        var vueltosMetodo = allPayments.Where(p => p.MetodoPago == vueltoMetodoValor).ToList();
                        esperadoVueltosOriginal = Math.Abs(vueltosMetodo.Sum(p => p.MontoAbonadoMoneda));
                        esperadoVueltosBase = Math.Abs(vueltosMetodo.Sum(p => p.EquivalenteAbonadoBase));
                    }

                    decimal esperadoNetoOriginal = esperadoIngresoOriginal - esperadoVueltosOriginal;
                    decimal esperadoNetoBase = esperadoIngresoBase - esperadoVueltosBase;

                    decimal declaradoNetoOriginal = esperadoNetoOriginal;
                    if (declarados != null)
                    {
                        var dec = declarados.FirstOrDefault(d => d.MetodoPago == metodo.Valor);
                        if (dec != null)
                        {
                            declaradoNetoOriginal = dec.MontoIngreso - dec.MontoVueltos;
                        }
                    }

                    decimal diferenciaOriginal = declaradoNetoOriginal - esperadoNetoOriginal;

                    totalCajaCobrado += esperadoNetoBase;

                    decimal tasaCambioCaja = userRecibos.FirstOrDefault(r => r.TasaCambioDia > 0)?.TasaCambioDia ?? 1;
                    decimal declaradoNetoBase = metodo.EsUSD ? declaradoNetoOriginal : (tasaCambioCaja > 0 ? declaradoNetoOriginal / tasaCambioCaja : 0);
                    totalCajaIngresado += declaradoNetoBase;

                    desgloseMetodos.Add(new DesgloseMetodoDto
                    {
                        Metodo = metodo.Valor,
                        Nombre = metodo.Nombre,
                        Esperado = esperadoNetoOriginal,
                        Declarado = declaradoNetoOriginal,
                        Diferencia = diferenciaOriginal,
                        EsUSD = metodo.EsUSD
                    });
                }

                cajerosReport.Add(new CajeroReportDto
                {
                    Username = caja.NombreUsuario,
                    FullName = userMap.TryGetValue(caja.UsuarioId, out var name) ? name : caja.NombreUsuario,
                    EstadoCaja = caja.Estado == EstadoConstants.CajaAbierta ? "ABIERTA" : (caja.Estado == EstadoConstants.CajaCerradaPorAsistente ? "CERRADA (PENDIENTE)" : "CONSOLIDADA"),
                    TotalCobrado = totalCajaCobrado,
                    TotalIngresado = totalCajaIngresado,
                    Diferencia = totalCajaIngresado - totalCajaCobrado,
                    Pagos = pagosDetallados,
                    Desglose = desgloseMetodos
                });

                grandTotalEsperado += totalCajaCobrado;
                grandTotalRecaudado += totalCajaIngresado;
            }

            return _excelService.GenerateDetailedCashierReport(cajerosReport, grandTotalEsperado, grandTotalRecaudado);
        }
    }
}
